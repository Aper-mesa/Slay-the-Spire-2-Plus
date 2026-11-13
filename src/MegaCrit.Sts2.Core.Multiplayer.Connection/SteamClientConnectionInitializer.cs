using System;
using System.Threading;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.Multiplayer.Game;
using MegaCrit.Sts2.Core.Multiplayer.Transport.Steam;
using MegaCrit.Sts2.Core.Platform;
using Steamworks;

namespace MegaCrit.Sts2.Core.Multiplayer.Connection;

public class SteamClientConnectionInitializer : IClientConnectionInitializer
{
	private ulong? _playerSteamId;

	private ulong? _lobbySteamId;

	public PlatformType Platform => PlatformType.Steam;

	public static SteamClientConnectionInitializer FromPlayer(ulong playerSteamId)
	{
		return new SteamClientConnectionInitializer
		{
			_playerSteamId = playerSteamId
		};
	}

	public static SteamClientConnectionInitializer FromLobby(ulong lobbySteamId)
	{
		return new SteamClientConnectionInitializer
		{
			_lobbySteamId = lobbySteamId
		};
	}

	public ulong? GetHostId()
	{
		if (_playerSteamId.HasValue)
		{
			return _playerSteamId;
		}
		if (_lobbySteamId.HasValue)
		{
			return SteamMatchmaking.GetLobbyOwner(new CSteamID(_lobbySteamId.Value)).m_SteamID;
		}
		return null;
	}

	public async Task<NetErrorInfo?> Connect(INetClientGameService gameService, CancellationToken cancelToken = default(CancellationToken))
	{
		if (gameService.IsConnected)
		{
			throw new InvalidOperationException("NetClientGameService must not be connected when passed to SteamClientConnectionInitializer!");
		}
		MegaCrit.Sts2.Core.Multiplayer.Transport.Steam.SteamClient steamClient = new MegaCrit.Sts2.Core.Multiplayer.Transport.Steam.SteamClient(gameService);
		gameService.Initialize(steamClient, PlatformType.Steam);
		if (_playerSteamId.HasValue)
		{
			return await steamClient.ConnectToLobbyOwnedByFriend(_playerSteamId.Value, cancelToken);
		}
		if (_lobbySteamId.HasValue)
		{
			return await steamClient.ConnectToLobby(_lobbySteamId.Value, cancelToken);
		}
		throw new InvalidOperationException("Neither player nor lobby is set!");
	}

	public override string ToString()
	{
		return $"{"SteamClientConnectionInitializer"} player: {_playerSteamId} lobby: {_lobbySteamId}";
	}
}
