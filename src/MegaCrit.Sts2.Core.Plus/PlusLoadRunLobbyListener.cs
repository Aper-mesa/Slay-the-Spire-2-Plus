using System;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.Multiplayer.Game.Lobby;

namespace MegaCrit.Sts2.Core.Plus;

/// <summary>[apermesa] Passive listener used while reloading a multiplayer run.</summary>
internal sealed class PlusLoadRunLobbyListener : ILoadRunLobbyListener
{
	private readonly TaskCompletionSource<bool> _beginRun;

	private readonly Action<ulong>? _onRemoteDisconnected;

	public PlusLoadRunLobbyListener(TaskCompletionSource<bool> beginRun, Action<ulong>? onRemoteDisconnected = null)
	{
		_beginRun = beginRun;
		_onRemoteDisconnected = onRemoteDisconnected;
	}

	public void PlayerConnected(LoadRunLobbyPlayer player)
	{
	}

	public void RemotePlayerDisconnected(ulong playerId)
	{
		_onRemoteDisconnected?.Invoke(playerId);
	}

	public Task<bool> ShouldAllowRunToBegin()
	{
		return Task.FromResult(result: true);
	}

	public void BeginRun()
	{
		_beginRun.TrySetResult(result: true);
	}

	public void PlayerReadyChanged(ulong playerId)
	{
	}

	public void LocalPlayerDisconnected(NetErrorInfo info)
	{
		_beginRun.TrySetException(new InvalidOperationException($"Disconnected during Save & Load: {info}"));
	}
}
