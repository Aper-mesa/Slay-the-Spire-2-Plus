using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Multiplayer.Game;
using MegaCrit.Sts2.Core.Multiplayer.Game.Lobby;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Audio;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Saves;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace MegaCrit.Sts2.Core.Plus;

/// <summary>[apermesa] Client side of the built-in multiplayer Save &amp; Load.</summary>
internal static class PlusSaveLoadClient
{
	private static int _nextRequestId;

	private static uint _activeClientRequestId;

	private static uint _activeRequestId;

	private static TaskCompletionSource<PlusSlInitiateResponseMessage>? _initiateResponse;

	private static TaskCompletionSource<bool>? _executionCompletion;

	private static TaskCompletionSource<bool>? _beginRunTcs;

	private static TaskCompletionSource<bool>? _runBeginTcs;

	private static CancellationTokenSource? _requestCts;

	private static bool _executing;

	public static void Reset()
	{
		_initiateResponse?.TrySetCanceled();
		_initiateResponse = null;
		_executionCompletion?.TrySetCanceled();
		_executionCompletion = null;
		_beginRunTcs?.TrySetCanceled();
		_beginRunTcs = null;
		_runBeginTcs?.TrySetCanceled();
		_runBeginTcs = null;
		try
		{
			_requestCts?.Cancel();
		}
		catch (ObjectDisposedException)
		{
		}
		_requestCts = null;
		_executing = false;
	}

	public static async Task RequestAsync(NGame game, RunManager runManager)
	{
		INetGameService netService = runManager.NetService;
		if (netService.Type != NetGameType.Client || _initiateResponse != null || _executing)
		{
			return;
		}
		uint requestId = unchecked((uint)Interlocked.Increment(ref _nextRequestId));
		_activeClientRequestId = requestId;
		_initiateResponse = new TaskCompletionSource<PlusSlInitiateResponseMessage>(TaskCreationOptions.RunContinuationsAsynchronously);
		netService.SendMessage(new PlusSlInitiateMessage
		{
			ClientRequestId = requestId
		});

		bool responded = await WaitForAsync(_initiateResponse.Task, TimeSpan.FromSeconds(10), CancellationToken.None);
		if (!responded)
		{
			Reset();
			PlusSaveLoadService.Notify("PLUS_SL.TIMEOUT.body");
			return;
		}
		PlusSlInitiateResponseMessage response = await _initiateResponse.Task;
		_initiateResponse = null;
		if (!response.Approved)
		{
			PlusSaveLoadService.Notify("PLUS_SL.REJECTED.body");
			return;
		}

		TaskCompletionSource<bool> completion = _executionCompletion ??= new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
		bool finished = await WaitForAsync(completion.Task, TimeSpan.FromSeconds(30), CancellationToken.None);
		if (!finished)
		{
			PlusSaveLoadService.Notify("PLUS_SL.TIMEOUT.body");
		}
	}

	public static void HandleInitiateResponse(PlusSlInitiateResponseMessage message, ulong senderId)
	{
		if (!IsHost(senderId) || message.ClientRequestId != _activeClientRequestId)
		{
			return;
		}
		_initiateResponse?.TrySetResult(message);
	}

	public static void HandleExecute(PlusSlExecuteMessage message, ulong senderId)
	{
		if (!IsHost(senderId) || _executing)
		{
			return;
		}
		_executing = true;
		_activeRequestId = message.RequestId;
		_requestCts = new CancellationTokenSource();
		_executionCompletion ??= new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
		TaskHelper.RunSafely(ExecuteLocalAsync(message, _requestCts.Token));
	}

	public static void HandleRunBegin(PlusSlRunBeginMessage message, ulong senderId)
	{
		if (!IsHost(senderId) || message.RequestId != _activeRequestId)
		{
			return;
		}
		_runBeginTcs?.TrySetResult(result: true);
	}

	public static void HandleCancel(PlusSlCancelMessage message, ulong senderId)
	{
		if (!IsHost(senderId))
		{
			return;
		}
		PlusSaveLoadCancelReason reason = (PlusSaveLoadCancelReason)message.Reason;
		Reset();
		PlusSaveLoadService.Notify(reason == PlusSaveLoadCancelReason.Rejected ? "PLUS_SL.REJECTED.body" : "PLUS_SL.FAILED.body");
	}

	private static async Task ExecuteLocalAsync(PlusSlExecuteMessage message, CancellationToken token)
	{
		NGame? game = NGame.Instance;
		if (game == null)
		{
			return;
		}
		RunManager runManager = RunManager.Instance;
		INetGameService netService = runManager.NetService;
		bool fadedOut = false;
		bool cleanedUp = false;
		LoadRunLobby? lobby = null;
		try
		{
			if (netService.Type != NetGameType.Client || !runManager.IsInProgress || runManager.IsCleaningUp)
			{
				return;
			}
			ReadSaveResult<SerializableRun> readResult = JsonSerializationUtility.FromJson<SerializableRun>(message.RunSaveJson);
			if (!readResult.Success || readResult.SaveData == null)
			{
				Log.Warn($"[PlusSL] Failed to parse host checkpoint: {readResult.Status}");
				PlusSaveLoadService.Notify("PLUS_SL.FAILED.body");
				return;
			}
			SerializableRun save;
			try
			{
				save = RunManager.CanonicalizeSave(readResult.SaveData, netService.NetId);
			}
			catch (Exception ex)
			{
				Log.Warn($"[PlusSL] Host checkpoint does not contain the local player: {ex}");
				PlusSaveLoadService.Notify("PLUS_SL.FAILED.body");
				return;
			}
			List<ulong> connectedIds = message.ConnectedPlayerIds?.Distinct().ToList() ?? new List<ulong>();
			if (!connectedIds.Contains(netService.NetId))
			{
				connectedIds.Add(netService.NetId);
			}
			List<RunLobbyPlayer> previousPlayers = runManager.RunLobby?.Players.ToList() ?? new List<RunLobbyPlayer>();

			// Build the lobby without touching the current run; abort is still possible before BeginRun.
			TaskCompletionSource<bool> beginRun = new(TaskCreationOptions.RunContinuationsAsynchronously);
			_beginRunTcs = beginRun;
			PlusLoadRunLobbyListener listener = new(beginRun);
			lobby = new LoadRunLobby(netService, listener, save);
			foreach (ulong playerId in connectedIds)
			{
				bool isModded = playerId == netService.NetId
					? netService.LocalVersion.IsModded()
					: previousPlayers.FirstOrDefault(p => p.id == playerId).isModded;
				lobby.Players.Add(new LoadRunLobbyPlayer
				{
					id = playerId,
					isModded = isModded,
					isReady = false
				});
			}
			lobby.SetReady(ready: true);

			bool begun = await WaitForAsync(beginRun.Task, TimeSpan.FromSeconds(30), token);
			if (!begun || token.IsCancellationRequested)
			{
				lobby.CleanUp(disconnectSession: false);
				lobby = null;
				if (!token.IsCancellationRequested)
				{
					PlusSaveLoadService.Notify("PLUS_SL.TIMEOUT.body");
				}
				return;
			}

			runManager.ActionExecutor.Cancel();
			runManager.ActionQueueSet.Reset();
			NRunMusicController.Instance?.StopMusic();
			await game.Transition.FadeOut(0.8f, PlusSaveLoadService.FadeTransitionPath);
			fadedOut = true;
			DisposeOldRunSystems(runManager);
			runManager.CleanUp(graceful: true, disconnect: false);
			cleanedUp = true;

			RunState state = RunState.FromSerializable(save);
			await runManager.SetUpSavedMultiplayer(state, lobby);
			game.RemoteCursorContainer.Initialize(lobby.InputSynchronizer, lobby.PlayerIds);
			game.ReactionContainer.InitializeNetworking(netService);

			// Phase B: report that the new run systems are registered, then wait for the host.
			netService.SendMessage(new PlusSlSetupReadyMessage
			{
				RequestId = message.RequestId
			});
			_runBeginTcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
			bool runBegin = await WaitForAsync(_runBeginTcs.Task, TimeSpan.FromSeconds(30), token);
			if (!runBegin || token.IsCancellationRequested)
			{
				PlusSaveLoadService.Notify("PLUS_SL.TIMEOUT.body");
				lobby.CleanUp(disconnectSession: true);
				lobby = null;
				await game.ReturnToMainMenu();
				return;
			}

			await game.LoadRun(state, save.PreFinishedRoom);
			lobby.CleanUp(disconnectSession: false);
			lobby = null;
			await game.Transition.FadeIn(0.8f, PlusSaveLoadService.FadeTransitionPath);
			fadedOut = false;
			Log.Info("[PlusSL] Multiplayer client reload complete.");
		}
		catch (Exception ex)
		{
			Log.Error($"[PlusSL] Multiplayer client reload failed: {ex}");
			PlusSaveLoadService.Notify("PLUS_SL.FAILED.body");
			if (lobby != null)
			{
				try
				{
					lobby.CleanUp(disconnectSession: true);
				}
				catch (Exception cleanupEx)
				{
					Log.Error($"[PlusSL] Lobby cleanup failed: {cleanupEx}");
				}
			}
			await PlusSaveLoadService.RecoverAsync(game, fadedOut, cleanedUp);
		}
		finally
		{
			_executing = false;
			_executionCompletion?.TrySetResult(result: true);
			_executionCompletion = null;
			_beginRunTcs = null;
			_runBeginTcs = null;
		}
	}

	private static async Task<bool> WaitForAsync(Task task, TimeSpan timeout, CancellationToken token)
	{
		Task delay = Task.Delay(timeout, token);
		Task completed = await Task.WhenAny(task, delay);
		return completed == task && task.Status == TaskStatus.RanToCompletion;
	}

	private static void DisposeOldRunSystems(RunManager runManager)
	{
		try
		{
			runManager.CombatStateSynchronizer?.Dispose();
		}
		catch (Exception ex)
		{
			Log.Warn($"[PlusSL] Failed to dispose CombatStateSynchronizer: {ex}");
		}
		try
		{
			runManager.InputSynchronizer?.Dispose();
		}
		catch (Exception ex)
		{
			Log.Warn($"[PlusSL] Failed to dispose InputSynchronizer: {ex}");
		}
	}

	private static bool IsHost(ulong senderId)
	{
		INetGameService netService = RunManager.Instance.NetService;
		return netService is INetClientGameService clientService && clientService.NetClient != null && clientService.NetClient.HostNetId == senderId;
	}
}
