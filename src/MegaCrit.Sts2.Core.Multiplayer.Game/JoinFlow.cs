using System;
using System.Threading;
using System.Threading.Tasks;
using Godot;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Multiplayer.Connection;
using MegaCrit.Sts2.Core.Multiplayer.Messages.Lobby;
using MegaCrit.Sts2.Core.Saves;
using MegaCrit.Sts2.Core.TestSupport;
using MegaCrit.Sts2.Core.Unlocks;

namespace MegaCrit.Sts2.Core.Multiplayer.Game;

/// <summary>
/// Object which manages the join flow for the local player, who joins as a client.
/// </summary>
public class JoinFlow
{
	private TaskCompletionSource<InitialGameInfoMessage>? _connectCompletion;

	private TaskCompletionSource<ClientRejoinResponseMessage>? _rejoinCompletion;

	private TaskCompletionSource<ClientLoadJoinResponseMessage>? _loadJoinCompletion;

	private TaskCompletionSource<ClientLobbyJoinResponseMessage>? _joinCompletion;

	private readonly MegaCrit.Sts2.Core.Logging.Logger _logger = new MegaCrit.Sts2.Core.Logging.Logger("JoinFlow", LogType.Network);

	private readonly IClientConnectionInitializer _initializer;

	public INetClientGameService NetService { get; }

	public CancellationTokenSource CancelToken { get; } = new CancellationTokenSource();

	public JoinFlow(INetClientGameService netService, IClientConnectionInitializer initializer)
	{
		_initializer = initializer;
		NetService = netService;
	}

	/// <summary>
	/// Begins the join flow.
	/// This task returns either a JoinResult with the final message in the join flow, for use initializing the run or
	/// the Lobby in the case of a rejoin or a join, respectively.
	/// </summary>
	/// <param name="sceneTree">Scene tree to use for hooking into update.
	/// If null, NetService.Update is NOT called and the caller needs to do it manually.</param>
	/// <throws>ClientConnectionFailedException if the join fails. In this case, you should not use NetService. An error
	/// should be shown to the user and they may try to join again.</throws>
	public async Task<JoinResult> Begin(SceneTree? sceneTree)
	{
		if (TestMode.IsOff)
		{
			MegaCrit.Sts2.Core.Logging.Logger.logLevelTypeMap[LogType.Network] = LogLevel.Debug;
			MegaCrit.Sts2.Core.Logging.Logger.logLevelTypeMap[LogType.Actions] = LogLevel.VeryDebug;
			MegaCrit.Sts2.Core.Logging.Logger.logLevelTypeMap[LogType.GameSync] = LogLevel.VeryDebug;
		}
		if (_connectCompletion != null)
		{
			throw new InvalidOperationException("JoinFlow object can only be used once!");
		}
		_logger.Info($"Beginning join with initializer {_initializer}");
		CancelToken.Token.Register(Cancel);
		CancellationTokenSource updateLoopCancelSource = new CancellationTokenSource();
		if (sceneTree != null)
		{
			TaskHelper.RunSafely(NetServiceUpdateLoop(updateLoopCancelSource, sceneTree));
		}
		JoinResult result;
		try
		{
			_ = 4;
			try
			{
				NetService.RegisterMessageHandler<InitialGameInfoMessage>(HandleInitialGameInfoMessage);
				NetService.RegisterMessageHandler<ClientLobbyJoinResponseMessage>(HandleJoinResponseMessage);
				NetService.RegisterMessageHandler<ClientLoadJoinResponseMessage>(HandleLoadJoinResponseMessage);
				NetService.RegisterMessageHandler<ClientRejoinResponseMessage>(HandleRejoinResponseMessage);
				NetService.Disconnected += OnDisconnected;
				NetService.ConnectionFailed += OnFailedToConnectToHost;
				_connectCompletion = new TaskCompletionSource<InitialGameInfoMessage>();
				NetErrorInfo? value = await _initializer.Connect(NetService, CancelToken.Token);
				if (value.HasValue)
				{
					_logger.Info($"Connection failed: {value}");
					NetErrorInfo value2 = value.Value;
					value2.JoinExtraInfo = new JoinFailureExtraInfo(_initializer);
					throw new ClientConnectionFailedException("Could not connect", new NetErrorInfo?(value2).Value);
				}
				_logger.Info("Initializer connection completed, awaiting initial game info message");
				RunSessionState state = (await _connectCompletion.Task).sessionState;
				switch (state)
				{
				case RunSessionState.InLobby:
				{
					ClientLobbyJoinResponseMessage value5 = await AttemptJoin();
					result = new JoinResult
					{
						gameMode = value5.gameMode,
						isCustom = value5.isCustom,
						sessionState = state,
						joinResponse = value5
					};
					break;
				}
				case RunSessionState.InLoadedLobby:
				{
					ClientLoadJoinResponseMessage value4 = await AttemptLoadJoin();
					result = new JoinResult
					{
						gameMode = value4.serializableRun.GameMode,
						isCustom = value4.serializableRun.IsCustomGame,
						sessionState = state,
						loadJoinResponse = value4
					};
					break;
				}
				case RunSessionState.Running:
				{
					ClientRejoinResponseMessage value3 = await AttemptRejoin();
					result = new JoinResult
					{
						gameMode = value3.serializableRun.GameMode,
						isCustom = value3.serializableRun.IsCustomGame,
						sessionState = state,
						rejoinResponse = value3
					};
					break;
				}
				default:
					NetService.Disconnect(NetError.InternalError, now: true);
					throw new InvalidOperationException($"Received invalid state {state} from connection!");
				}
			}
			catch (Exception ex)
			{
				NetError reason;
				if (ex is ClientConnectionFailedException ex2)
				{
					reason = ex2.info.GetReason();
				}
				else
				{
					reason = ((ex is OperationCanceledException) ? NetError.CancelledJoin : NetError.InternalError);
				}
				NetService.Disconnected -= OnDisconnected;
				SetDisconnectionException(ex);
				NetService.Disconnect(reason);
				MegaCrit.Sts2.Core.Logging.Logger.logLevelTypeMap[LogType.Network] = LogLevel.Info;
				MegaCrit.Sts2.Core.Logging.Logger.logLevelTypeMap[LogType.Actions] = LogLevel.Info;
				MegaCrit.Sts2.Core.Logging.Logger.logLevelTypeMap[LogType.GameSync] = LogLevel.Info;
				throw;
			}
		}
		finally
		{
			await updateLoopCancelSource.CancelAsync();
			NetService.UnregisterMessageHandler<InitialGameInfoMessage>(HandleInitialGameInfoMessage);
			NetService.UnregisterMessageHandler<ClientLobbyJoinResponseMessage>(HandleJoinResponseMessage);
			NetService.UnregisterMessageHandler<ClientLoadJoinResponseMessage>(HandleLoadJoinResponseMessage);
			NetService.UnregisterMessageHandler<ClientRejoinResponseMessage>(HandleRejoinResponseMessage);
			NetService.Disconnected -= OnDisconnected;
		}
		return result;
	}

	private async Task NetServiceUpdateLoop(CancellationTokenSource token, SceneTree sceneTree)
	{
		while (!token.IsCancellationRequested)
		{
			try
			{
				NetService.Update();
			}
			catch (Exception ex)
			{
				Log.Error(ex.ToString());
			}
			await sceneTree.ToSignal(sceneTree, SceneTree.SignalName.ProcessFrame);
		}
	}

	private async Task<ClientLobbyJoinResponseMessage> AttemptJoin()
	{
		_joinCompletion = new TaskCompletionSource<ClientLobbyJoinResponseMessage>();
		_logger.Info("Sending ClientLobbyJoinRequestMessage and waiting for response message");
		UnlockState unlockState = SaveManager.Instance.GenerateUnlockStateFromProgress();
		ClientLobbyJoinRequestMessage message = new ClientLobbyJoinRequestMessage
		{
			unlockState = unlockState.ToSerializable(),
			maxBlitzAscension = SaveManager.Instance.Progress.MaxBlitzAscension,
			maxStandardAscension = SaveManager.Instance.Progress.MaxMultiplayerAscension
		};
		NetService.SendMessage(message);
		ClientLobbyJoinResponseMessage clientLobbyJoinResponseMessage = await _joinCompletion.Task;
		_logger.Info($"Received {"ClientLobbyJoinResponseMessage"}: {clientLobbyJoinResponseMessage}");
		return clientLobbyJoinResponseMessage;
	}

	private async Task<ClientLoadJoinResponseMessage> AttemptLoadJoin()
	{
		_loadJoinCompletion = new TaskCompletionSource<ClientLoadJoinResponseMessage>();
		_logger.Info("Sending ClientLoadJoinRequestMessage and waiting for rejoin response message");
		NetService.SendMessage<ClientLoadJoinRequestMessage>(default);
		ClientLoadJoinResponseMessage clientLoadJoinResponseMessage = await _loadJoinCompletion.Task;
		_logger.Info($"Received ClientLoadJoinResponseMessage: {clientLoadJoinResponseMessage}");
		return clientLoadJoinResponseMessage;
	}

	private async Task<ClientRejoinResponseMessage> AttemptRejoin()
	{
		_rejoinCompletion = new TaskCompletionSource<ClientRejoinResponseMessage>();
		_logger.Info("Sending ClientRequestRejoinMessage and waiting for rejoin response message");
		NetService.SendMessage<ClientRejoinRequestMessage>(default);
		ClientRejoinResponseMessage clientRejoinResponseMessage = await _rejoinCompletion.Task;
		_logger.Info($"Received ClientRejoinResponseMessage: {clientRejoinResponseMessage}");
		return clientRejoinResponseMessage;
	}

	private void HandleInitialGameInfoMessage(InitialGameInfoMessage message, ulong _)
	{
		if (_connectCompletion == null || _connectCompletion.Task.IsCompleted)
		{
			_logger.Warn($"Received {"InitialGameInfoMessage"} when we weren't expecting it! Completion status: {_connectCompletion}");
		}
		else
		{
			_connectCompletion.SetResult(message);
		}
	}

	private void HandleRejoinResponseMessage(ClientRejoinResponseMessage message, ulong senderId)
	{
		if (_rejoinCompletion == null || _rejoinCompletion.Task.IsCompleted)
		{
			_logger.Warn($"Received {"ClientRejoinResponseMessage"} when we weren't expecting it! Completion status: {_connectCompletion}");
		}
		else
		{
			_rejoinCompletion.SetResult(message);
		}
	}

	private void HandleLoadJoinResponseMessage(ClientLoadJoinResponseMessage message, ulong senderId)
	{
		if (_loadJoinCompletion == null || _loadJoinCompletion.Task.IsCompleted)
		{
			_logger.Warn($"Received {"ClientLoadJoinResponseMessage"} when we weren't expecting it! Completion status: {_connectCompletion}");
		}
		else
		{
			_loadJoinCompletion.SetResult(message);
		}
	}

	private void HandleJoinResponseMessage(ClientLobbyJoinResponseMessage message, ulong senderId)
	{
		if (_joinCompletion == null || _joinCompletion.Task.IsCompleted)
		{
			_logger.Warn($"Received {"ClientLobbyJoinResponseMessage"} when we weren't expecting it! Completion status: {_connectCompletion}");
		}
		else
		{
			_joinCompletion.SetResult(message);
		}
	}

	private void OnDisconnected(NetErrorInfo info)
	{
		_logger.Info($"Disconnected during join flow, reason: {info.GetReason()}. Failing with an exception");
		NetErrorInfo netErrorInfo = info;
		netErrorInfo.JoinExtraInfo = new JoinFailureExtraInfo(_initializer);
		info = netErrorInfo;
		ClientConnectionFailedException disconnectionException = new ClientConnectionFailedException($"Unexpectedly disconnected from host while joining. Reason: {info.GetReason()}", info);
		SetDisconnectionException(disconnectionException);
	}

	private void OnFailedToConnectToHost(NetErrorInfo info)
	{
		_logger.Info($"Failed to connect to the host. Reason: {info.GetReason()}.");
		NetErrorInfo netErrorInfo = info;
		netErrorInfo.JoinExtraInfo = new JoinFailureExtraInfo(_initializer);
		info = netErrorInfo;
		ClientConnectionFailedException disconnectionException = new ClientConnectionFailedException($"Failed to connect to the host. Reason: {info.GetReason()}", info);
		SetDisconnectionException(disconnectionException);
	}

	private void SetDisconnectionException(Exception exception)
	{
		Fault(_connectCompletion, exception);
		Fault(_joinCompletion, exception);
		Fault(_loadJoinCompletion, exception);
		Fault(_rejoinCompletion, exception);
	}

	private static void Fault<T>(TaskCompletionSource<T>? completion, Exception exception)
	{
		if (completion != null)
		{
			Task<T> task = completion.Task;
			if (task != null && !task.IsCompleted)
			{
				completion.SetException(exception);
				_ = completion.Task.Exception;
			}
		}
	}

	private void Cancel()
	{
		TaskCompletionSource<InitialGameInfoMessage> connectCompletion = _connectCompletion;
		if (connectCompletion != null)
		{
			Task<InitialGameInfoMessage> task = connectCompletion.Task;
			if (task != null && !task.IsCompleted)
			{
				_connectCompletion.SetCanceled();
			}
		}
		TaskCompletionSource<ClientLobbyJoinResponseMessage> joinCompletion = _joinCompletion;
		if (joinCompletion != null)
		{
			Task<ClientLobbyJoinResponseMessage> task2 = joinCompletion.Task;
			if (task2 != null && !task2.IsCompleted)
			{
				_joinCompletion?.SetCanceled();
			}
		}
		TaskCompletionSource<ClientLoadJoinResponseMessage> loadJoinCompletion = _loadJoinCompletion;
		if (loadJoinCompletion != null)
		{
			Task<ClientLoadJoinResponseMessage> task3 = loadJoinCompletion.Task;
			if (task3 != null && !task3.IsCompleted)
			{
				_loadJoinCompletion?.SetCanceled();
			}
		}
		TaskCompletionSource<ClientRejoinResponseMessage> rejoinCompletion = _rejoinCompletion;
		if (rejoinCompletion != null)
		{
			Task<ClientRejoinResponseMessage> task4 = rejoinCompletion.Task;
			if (task4 != null && !task4.IsCompleted)
			{
				_rejoinCompletion?.SetCanceled();
			}
		}
	}
}
