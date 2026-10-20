using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Multiplayer.Game;
using MegaCrit.Sts2.Core.Multiplayer.Game.Lobby;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Audio;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.Multiplayer;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Saves;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace MegaCrit.Sts2.Core.Plus;

/// <summary>[apermesa] Host side of the built-in multiplayer Save &amp; Load.</summary>
internal static class PlusSaveLoadHost
{
	private static readonly object SetupLock = new();

	private static HashSet<ulong>? _setupExpected;

	private static TaskCompletionSource<bool>? _setupTcs;

	private static uint _activeRequestId;

	private static int _nextRequestId;

	public static void Reset()
	{
		lock (SetupLock)
		{
			_setupExpected = null;
			_setupTcs?.TrySetCanceled();
			_setupTcs = null;
		}
	}

	public static async Task RunAsync(NGame game, RunManager runManager)
	{
		uint requestId = unchecked((uint)Interlocked.Increment(ref _nextRequestId));
		await RunInternalAsync(game, runManager, requestId);
	}

	public static void HandleInitiate(PlusSlInitiateMessage message, ulong senderId)
	{
		if (!PlusSaveLoadService.TryEnter())
		{
			Reply(RunManager.Instance, senderId, message.ClientRequestId, approved: false, PlusSaveLoadCancelReason.InvalidState);
			return;
		}
		TaskHelper.RunSafely(HandleInitiateAsync(message, senderId));
	}

	public static void HandleSetupReady(PlusSlSetupReadyMessage message, ulong senderId)
	{
		if (RunManager.Instance.NetService.Type != NetGameType.Host || message.RequestId != _activeRequestId)
		{
			return;
		}
		CompleteSetup(senderId);
	}

	private static void CompleteSetup(ulong playerId)
	{
		lock (SetupLock)
		{
			if (_setupExpected == null)
			{
				return;
			}
			_setupExpected.Remove(playerId);
			if (_setupExpected.Count == 0)
			{
				_setupTcs?.TrySetResult(result: true);
			}
		}
	}

	private static async Task<bool> WaitAllSetupReadyAsync(IEnumerable<ulong> expectedIds, uint requestId)
	{
		TaskCompletionSource<bool> tcs = new(TaskCreationOptions.RunContinuationsAsynchronously);
		lock (SetupLock)
		{
			_activeRequestId = requestId;
			_setupExpected = new HashSet<ulong>(expectedIds);
			_setupTcs = tcs;
			if (_setupExpected.Count == 0)
			{
				tcs.TrySetResult(result: true);
			}
		}
		Task completed = await Task.WhenAny(tcs.Task, Task.Delay(TimeSpan.FromSeconds(30)));
		bool ok = completed == tcs.Task && tcs.Task.Status == TaskStatus.RanToCompletion;
		lock (SetupLock)
		{
			_setupExpected = null;
			_setupTcs = null;
		}
		return ok;
	}

	private static async Task HandleInitiateAsync(PlusSlInitiateMessage message, ulong senderId)
	{
		try
		{
			RunManager runManager = RunManager.Instance;
			INetGameService netService = runManager.NetService;
			if (netService.Type != NetGameType.Host || !runManager.IsInProgress || runManager.IsCleaningUp)
			{
				Reply(runManager, senderId, message.ClientRequestId, approved: false, PlusSaveLoadCancelReason.InvalidState);
				return;
			}
			if (runManager.RunLobby == null || runManager.RunLobby.Players.All(p => p.id != senderId))
			{
				Reply(runManager, senderId, message.ClientRequestId, approved: false, PlusSaveLoadCancelReason.InvalidState);
				return;
			}
			bool approved = await AskHostAsync();
			Reply(runManager, senderId, message.ClientRequestId, approved, PlusSaveLoadCancelReason.Rejected);
			if (!approved)
			{
				return;
			}
			uint requestId = unchecked((uint)Interlocked.Increment(ref _nextRequestId));
			NGame? game = NGame.Instance;
			if (game != null)
			{
				await RunInternalAsync(game, runManager, requestId);
			}
		}
		catch (Exception ex)
		{
			Log.Error($"[PlusSL] Host request handling failed: {ex}");
		}
		finally
		{
			PlusSaveLoadService.Exit();
		}
	}

	private static async Task<bool> AskHostAsync()
	{
		NGenericPopup? popup = NGenericPopup.Create();
		if (popup == null)
		{
			return false;
		}
		NModalContainer.Instance.Add(popup);
		LocString body = new LocString("gameplay_ui", "PLUS_SL.HOST_CONFIRM.body");
		LocString header = new LocString("gameplay_ui", "PLUS_SL.HOST_CONFIRM.title");
		LocString cancel = new LocString("gameplay_ui", "PLUS_SL.HOST_CONFIRM.cancel");
		LocString confirm = new LocString("gameplay_ui", "PLUS_SL.HOST_CONFIRM.confirm");
		return await popup.WaitForConfirmation(body, header, cancel, confirm);
	}

	private static void Reply(RunManager runManager, ulong playerId, uint clientRequestId, bool approved, PlusSaveLoadCancelReason reason)
	{
		INetGameService netService = runManager.NetService;
		if (netService.Type == NetGameType.Host && netService.IsConnected)
		{
			netService.SendMessage(new PlusSlInitiateResponseMessage
			{
				ClientRequestId = clientRequestId,
				Approved = approved,
				Reason = (byte)reason
			}, playerId);
		}
	}

	private static async Task RunInternalAsync(NGame game, RunManager runManager, uint requestId)
	{
		bool fadedOut = false;
		bool cleanedUp = false;
		LoadRunLobby? lobby = null;
		INetGameService netService = runManager.NetService;
		try
		{
			if (netService.Type != NetGameType.Host || !runManager.IsInProgress || runManager.IsCleaningUp)
			{
				return;
			}
			List<RunLobbyPlayer> players = CollectPlayers(runManager, netService);
			List<ulong> connectedIds = players.Select(p => p.id).Distinct().ToList();
			if (!connectedIds.Contains(netService.NetId))
			{
				connectedIds.Add(netService.NetId);
			}
			List<ulong> remoteIds = connectedIds.Where(id => id != netService.NetId).ToList();
			if (remoteIds.Count == 0)
			{
				PlusSaveLoadService.Notify("PLUS_SL.NO_PEERS.body");
				return;
			}

			SaveManager saveManager = SaveManager.Instance;
			Task? currentSaveTask = saveManager.CurrentRunSaveTask;
			if (currentSaveTask != null)
			{
				await currentSaveTask;
			}
			ReadSaveResult<SerializableRun> readResult = saveManager.LoadAndCanonicalizeMultiplayerRunSave(netService.NetId);
			if (!readResult.Success || readResult.SaveData == null)
			{
				Log.Warn($"[PlusSL] Failed to read multiplayer run save: {readResult.Status}");
				SendCancel(netService, connectedIds, requestId, PlusSaveLoadCancelReason.Failed);
				return;
			}
			SerializableRun save = readResult.SaveData;
			string saveJson = JsonSerializationUtility.ToJson(save);
			if (Encoding.UTF8.GetByteCount(saveJson) > PlusSlExecuteMessage.MaxPayloadBytes)
			{
				Log.Warn("[PlusSL] Multiplayer run save exceeds the 1 MiB transfer limit.");
				SendCancel(netService, connectedIds, requestId, PlusSaveLoadCancelReason.Failed);
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

			TaskCompletionSource<bool> beginRun = new(TaskCreationOptions.RunContinuationsAsynchronously);
			PlusLoadRunLobbyListener listener = new(beginRun, CompleteSetup);
			lobby = new LoadRunLobby(netService, listener, save);
			lobby.AddLocalHostPlayer();
			foreach (RunLobbyPlayer player in players)
			{
				if (player.id != netService.NetId)
				{
					lobby.Players.Add(new LoadRunLobbyPlayer
					{
						id = player.id,
						isModded = player.isModded,
						isReady = false
					});
				}
			}

			PlusSlExecuteMessage execute = new()
			{
				RequestId = requestId,
				RunSaveJson = saveJson,
				ConnectedPlayerIds = connectedIds.ToArray()
			};
			foreach (ulong playerId in remoteIds)
			{
				if (netService.IsConnected)
				{
					netService.SendMessage(execute, playerId);
				}
			}

			lobby.SetReady(ready: true);

			Task completed = await Task.WhenAny(beginRun.Task, Task.Delay(TimeSpan.FromSeconds(30)));
			if (completed != beginRun.Task)
			{
				Log.Warn("[PlusSL] Timed out waiting for clients to become ready.");
				SendCancel(netService, connectedIds, requestId, PlusSaveLoadCancelReason.Timeout);
				lobby.CleanUp(disconnectSession: true);
				lobby = null;
				await game.ReturnToMainMenu();
				return;
			}
			await beginRun.Task;

			RunState state = RunState.FromSerializable(save);
			await runManager.SetUpSavedMultiplayer(state, lobby);
			game.RemoteCursorContainer.Initialize(lobby.InputSynchronizer, lobby.PlayerIds);
			game.ReactionContainer.InitializeNetworking(netService);

			bool setupReady = await WaitAllSetupReadyAsync(remoteIds, requestId);
			if (!setupReady)
			{
				Log.Warn("[PlusSL] Timed out waiting for clients to finish setup.");
				SendCancel(netService, connectedIds, requestId, PlusSaveLoadCancelReason.Timeout);
				lobby.CleanUp(disconnectSession: true);
				lobby = null;
				await game.ReturnToMainMenu();
				return;
			}

			PlusSlRunBeginMessage runBegin = new()
			{
				RequestId = requestId
			};
			foreach (ulong playerId in remoteIds)
			{
				if (netService.IsConnected)
				{
					netService.SendMessage(runBegin, playerId);
				}
			}

			await game.LoadRun(state, save.PreFinishedRoom);
			lobby.CleanUp(disconnectSession: false);
			lobby = null;
			await game.Transition.FadeIn(0.8f, PlusSaveLoadService.FadeTransitionPath);
			fadedOut = false;
			Log.Info("[PlusSL] Multiplayer host reload complete.");
		}
		catch (Exception ex)
		{
			Log.Error($"[PlusSL] Multiplayer host reload failed: {ex}");
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
	}

	private static List<RunLobbyPlayer> CollectPlayers(RunManager runManager, INetGameService netService)
	{
		List<RunLobbyPlayer> players = runManager.RunLobby?.Players.ToList() ?? new List<RunLobbyPlayer>();
		if (players.All(p => p.id != netService.NetId))
		{
			players.Add(new RunLobbyPlayer
			{
				id = netService.NetId,
				isModded = netService.LocalVersion.IsModded()
			});
		}
		return players;
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

	private static void SendCancel(INetGameService netService, IEnumerable<ulong> connectedIds, uint requestId, PlusSaveLoadCancelReason reason)
	{
		if (!netService.IsConnected)
		{
			return;
		}
		PlusSlCancelMessage message = new()
		{
			RequestId = requestId,
			Reason = (byte)reason
		};
		foreach (ulong playerId in connectedIds)
		{
			if (playerId != netService.NetId)
			{
				netService.SendMessage(message, playerId);
			}
		}
	}
}
