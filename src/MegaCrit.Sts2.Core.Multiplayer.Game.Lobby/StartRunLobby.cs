using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Daily;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Acts;
using MegaCrit.Sts2.Core.Models.Characters;
using MegaCrit.Sts2.Core.Multiplayer.Game.PeerInput;
using MegaCrit.Sts2.Core.Multiplayer.Messages.Lobby;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Random;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Saves;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.Timeline;
using MegaCrit.Sts2.Core.Timeline.Epochs;
using MegaCrit.Sts2.Core.Unlocks;

namespace MegaCrit.Sts2.Core.Multiplayer.Game.Lobby;

/// <summary>
/// Class which handles the connection flow for players beginning a new run.
/// Exists before Run does, and provides player data to start a multiplayer run.
/// <see cref="T:MegaCrit.Sts2.Core.Multiplayer.Game.Lobby.RunLobby" /> handles player connection and disconnection after the run begins.
/// </summary>
public class StartRunLobby
{
	private struct ConnectingPlayer : IEquatable<ConnectingPlayer>
	{
		public ulong id;

		public CancellationTokenSource timeoutCancelToken;

		public bool Equals(ConnectingPlayer other)
		{
			if (id == other.id)
			{
				return timeoutCancelToken.Equals(other.timeoutCancelToken);
			}
			return false;
		}

		public override bool Equals(object? obj)
		{
			if (obj is ConnectingPlayer other)
			{
				return Equals(other);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(id, timeoutCancelToken);
		}
	}

	private readonly Logger _logger;

	/// <summary>
	/// Contains players who are in the process of connecting, but have not yet fully connected. Only set on host.
	/// </summary>
	private readonly List<ConnectingPlayer> _connectingPlayers = new List<ConnectingPlayer>();

	/// <summary>
	/// Set to true when the run starts, but the lobby is still listening for messages
	/// </summary>
	private bool _isBeginningRun;

	private readonly int _maxPlayers;

	private readonly List<ModifierModel> _modifiers = new List<ModifierModel>();

	public INetGameService NetService { get; }

	public IStartRunLobbyListener LobbyListener { get; }

	public PeerInputSynchronizer InputSynchronizer { get; }

	public int Ascension { get; private set; }

	public int MaxAscension { get; private set; }

	public string? Seed { get; private set; }

	public TimeServerResult? DailyTime { get; private set; }

	public GameMode GameMode { get; set; }

	public bool IsCustom { get; private set; }

	public IReadOnlyList<ModifierModel> Modifiers => _modifiers;

	/// <summary>
	/// If we are the host, this is the amount of time we give clients to send the initial message response in
	/// milliseconds. Public for tests.
	/// </summary>
	public int ClientResponseTimeout { get; set; } = 10000;

	/// <summary>
	/// TEMPORARY way for the host to manually specify which ActModel they want for act 1.
	/// </summary>
	public string Act1 { get; set; } = "random";

	public List<StartRunLobbyPlayer> Players { get; } = new List<StartRunLobbyPlayer>();

	public StartRunLobbyPlayer LocalPlayer => Players.Find((StartRunLobbyPlayer p) => p.id == NetService.NetId);

	public event Action<StartRunLobbyPlayer>? PlayerConnected;

	public event Action<StartRunLobbyPlayer>? PlayerDisconnected;

	/// <summary>
	/// Provides extended disconnection info to UI, but only when the local player is the host.
	/// </summary>
	public event Action<ulong, NetErrorInfo>? PlayerFailedToConnect;

	public StartRunLobby(GameMode gameMode, bool isCustom, INetGameService netService, IStartRunLobbyListener lobbyListener, int maxPlayers)
	{
		GameMode = gameMode;
		IsCustom = isCustom;
		NetService = netService;
		LobbyListener = lobbyListener;
		_maxPlayers = maxPlayers;
		InputSynchronizer = new PeerInputSynchronizer(netService);
		_logger = new Logger("StartRunLobby", LogType.Network);
		NetService.RegisterMessageHandler<ClientLobbyJoinRequestMessage>(HandleClientLobbyJoinRequestMessage);
		NetService.RegisterMessageHandler<ClientLoadJoinRequestMessage>(HandleClientLoadJoinRequestMessage);
		NetService.RegisterMessageHandler<ClientRejoinRequestMessage>(HandleClientRejoinRequestMessage);
		NetService.RegisterMessageHandler<PlayerJoinedMessage>(HandlePlayerJoinedMessage);
		NetService.RegisterMessageHandler<PlayerLeftMessage>(HandlePlayerLeftMessage);
		NetService.RegisterMessageHandler<LobbyPlayerChangedCharacterMessage>(HandleLobbyPlayerChangedCharacterMessage);
		NetService.RegisterMessageHandler<LobbyAscensionChangedMessage>(HandleAscensionChangedMessage);
		NetService.RegisterMessageHandler<LobbySeedChangedMessage>(HandleSeedChangedMessage);
		NetService.RegisterMessageHandler<LobbyModifiersChangedMessage>(HandleModifiersChangedMessage);
		NetService.RegisterMessageHandler<LobbyGameModeChangedMessage>(HandleGameModeChangedMessage);
		NetService.RegisterMessageHandler<LobbyPlayerSetReadyMessage>(HandlePlayerReadyMessage);
		NetService.RegisterMessageHandler<LobbyBeginRunMessage>(HandleLobbyBeginRunMessage);
		NetService.Disconnected += OnDisconnected;
		if (NetService.Type == NetGameType.Host)
		{
			INetHostGameService netHostGameService = (INetHostGameService)netService;
			netHostGameService.ClientConnected += OnConnectedToClientAsHost;
			netHostGameService.ClientDisconnected += OnDisconnectedFromClientAsHost;
			netHostGameService.ClientConnectionFailed += OnClientConnectionFailed;
		}
	}

	public StartRunLobby(GameMode gameMode, bool isCustom, INetGameService netService, IStartRunLobbyListener lobbyListener, TimeServerResult timeServerResult, int maxPlayers)
		: this(gameMode, isCustom, netService, lobbyListener, maxPlayers)
	{
		DailyTime = timeServerResult;
	}

	public void InitializeFromMessage(ClientLobbyJoinResponseMessage message)
	{
		foreach (StartRunLobbyPlayer item in message.playersInLobby)
		{
			Players.Add(item);
		}
		_modifiers.Clear();
		_modifiers.AddRange(message.modifiers.Select(ModifierModel.FromSerializable));
		Ascension = message.ascension;
		GameMode = message.gameMode;
		IsCustom = message.isCustom;
		UpdateMaxSharedAscension();
		Seed = message.seed;
		LobbyListener.PlayerConnected(LocalPlayer);
		PlayerConnected?.Invoke(LocalPlayer);
	}

	/// <summary>
	/// This should be called to cleanup the lobby before exiting the lobby screen.
	/// </summary>
	/// <param name="disconnectSession">
	/// If true, the net service will be disconnected. Pass true if the lobby is being closed rather than transitioning
	/// to a run.
	/// </param>
	/// <param name="error">If disconnectSession is true, this is the error that is sent to clients.</param>
	public void CleanUp(bool disconnectSession, NetError error = NetError.Quit)
	{
		NetService.UnregisterMessageHandler<ClientLobbyJoinRequestMessage>(HandleClientLobbyJoinRequestMessage);
		NetService.UnregisterMessageHandler<ClientLoadJoinRequestMessage>(HandleClientLoadJoinRequestMessage);
		NetService.UnregisterMessageHandler<ClientRejoinRequestMessage>(HandleClientRejoinRequestMessage);
		NetService.UnregisterMessageHandler<PlayerJoinedMessage>(HandlePlayerJoinedMessage);
		NetService.UnregisterMessageHandler<PlayerLeftMessage>(HandlePlayerLeftMessage);
		NetService.UnregisterMessageHandler<LobbyPlayerChangedCharacterMessage>(HandleLobbyPlayerChangedCharacterMessage);
		NetService.UnregisterMessageHandler<LobbyAscensionChangedMessage>(HandleAscensionChangedMessage);
		NetService.UnregisterMessageHandler<LobbySeedChangedMessage>(HandleSeedChangedMessage);
		NetService.UnregisterMessageHandler<LobbyModifiersChangedMessage>(HandleModifiersChangedMessage);
		NetService.UnregisterMessageHandler<LobbyPlayerSetReadyMessage>(HandlePlayerReadyMessage);
		NetService.UnregisterMessageHandler<LobbyBeginRunMessage>(HandleLobbyBeginRunMessage);
		if (disconnectSession)
		{
			if (NetService.IsConnected)
			{
				NetService.Disconnect(error);
			}
			InputSynchronizer.Dispose();
		}
		NetService.Disconnected -= OnDisconnected;
		if (NetService.Type == NetGameType.Host)
		{
			INetHostGameService netHostGameService = (INetHostGameService)NetService;
			netHostGameService.ClientConnected -= OnConnectedToClientAsHost;
			netHostGameService.ClientDisconnected -= OnDisconnectedFromClientAsHost;
			netHostGameService.ClientConnectionFailed -= OnClientConnectionFailed;
		}
	}

	/// <summary>
	/// Should be called when the lobby opens on the host player's side to generate the host's lobby player.
	/// </summary>
	public StartRunLobbyPlayer? AddLocalHostPlayer(UnlockState unlocks, int maxStandardAscension, int maxBlitzAscension)
	{
		if (NetService.Type == NetGameType.Client)
		{
			throw new InvalidOperationException("Tried to add local host player as client!");
		}
		_logger.Context = $"{"StartRunLobby"} ({NetService.NetId})";
		SerializableUnlockState unlockState = unlocks.ToSerializable();
		return AddLocalHostPlayerInternal(unlockState, maxStandardAscension, maxBlitzAscension);
	}

	/// <summary>
	/// For use in tests and internally in this class.
	/// </summary>
	public StartRunLobbyPlayer? AddLocalHostPlayerInternal(SerializableUnlockState unlockState, int maxStandardAscension, int maxBlitzAscension)
	{
		StartRunLobbyPlayer? result = TryAddPlayerInFirstAvailableSlot(unlockState, maxStandardAscension, maxBlitzAscension, NetService.LocalVersion.IsModded(), NetService.NetId);
		if (result.HasValue)
		{
			LobbyListener.PlayerConnected(result.Value);
			PlayerConnected?.Invoke(result.Value);
		}
		UpdateMaxSharedAscension();
		return result;
	}

	/// <summary>
	/// Admits clients that finished the handshake before this lobby existed. A daily run starts hosting before it
	/// can build its lobby, because the lobby needs the time server's answer first. A player who joins in that
	/// window finishes the handshake with nothing listening to <see cref="E:MegaCrit.Sts2.Core.Multiplayer.Game.INetHostGameService.ClientConnected" />,
	/// so without this they would wait for an initial game info message that never gets sent.
	/// Call this once, after the local host player is added.
	/// </summary>
	public void AdmitClientsConnectedBeforeLobbyExisted()
	{
		if (NetService.Type != NetGameType.Host)
		{
			return;
		}
		INetHostGameService netHostGameService = (INetHostGameService)NetService;
		foreach (NetClientData peer in netHostGameService.ConnectedPeers.ToList())
		{
			if (!_connectingPlayers.Any((ConnectingPlayer p) => p.id == peer.peerId) && !Players.Any((StartRunLobbyPlayer p) => p.id == peer.peerId))
			{
				_logger.Info($"Client {peer.peerId} connected before the lobby existed. Admitting them now.");
				OnConnectedToClientAsHost(peer.peerId);
			}
		}
	}

	private void HandleClientLobbyJoinRequestMessage(ClientLobbyJoinRequestMessage message, ulong senderId)
	{
		if (NetService.Type != NetGameType.Host)
		{
			throw new InvalidOperationException("Received ClientLobbyJoinRequestMessage as non-host!");
		}
		INetHostGameService netHostGameService = (INetHostGameService)NetService;
		try
		{
			if (Players.Count >= _maxPlayers)
			{
				_logger.Warn($"Client {senderId} sent ClientLobbyJoinRequestMessage but we are at maximum players!");
				netHostGameService.DisconnectClient(senderId, NetError.LobbyFull);
				return;
			}
			_logger.Info($"Received ClientLobbyJoinRequestMessage for {senderId}");
			if (!IsCustom)
			{
				if (GameMode == GameMode.Dread && !message.unlockState.UnlockedEpochs.Contains(EpochModel.GetId<DreadEpoch>()))
				{
					_logger.Info($"Client {senderId} tried to join a Dread lobby without the Dread epoch unlocked. Disconnecting client.");
					netHostGameService.DisconnectClient(senderId, NetError.DreadNotUnlocked);
					return;
				}
				if (GameMode == GameMode.Blitz && !message.unlockState.UnlockedEpochs.Contains(EpochModel.GetId<BlitzEpoch>()))
				{
					_logger.Info($"Client {senderId} tried to join a Blitz lobby without the Blitz epoch unlocked. Disconnecting client.");
					netHostGameService.DisconnectClient(senderId, NetError.BlitzNotUnlocked);
					return;
				}
			}
			StartRunLobbyPlayer? startRunLobbyPlayer = TryAddPlayerInFirstAvailableSlot(message.unlockState, message.maxStandardAscension, message.maxBlitzAscension, netHostGameService.GetVersionInfoForPeer(senderId)?.IsModded() ?? false, senderId);
			if (!startRunLobbyPlayer.HasValue)
			{
				return;
			}
			UpdateMaxSharedAscension();
			ClientLobbyJoinResponseMessage message2 = new ClientLobbyJoinResponseMessage
			{
				playersInLobby = Players,
				ascension = Ascension,
				dailyTime = DailyTime,
				gameMode = GameMode,
				isCustom = IsCustom,
				seed = Seed,
				modifiers = Modifiers.Select((ModifierModel m) => m.ToSerializable()).ToList()
			};
			_logger.Debug($"Sending ClientLobbyJoinResponseMessage length ({message2.playersInLobby.Count}) to ({startRunLobbyPlayer.Value.id})");
			netHostGameService.SendMessage(message2, senderId);
			netHostGameService.SetPeerReadyForBroadcasting(senderId);
			PlayerJoinedMessage message3 = new PlayerJoinedMessage
			{
				lobbyPlayer = startRunLobbyPlayer.Value
			};
			foreach (StartRunLobbyPlayer player in Players)
			{
				if (player.id != NetService.NetId && player.id != startRunLobbyPlayer.Value.id)
				{
					NetService.SendMessage(message3, player.id);
				}
			}
			RemoveConnectingPlayer(startRunLobbyPlayer.Value.id);
			LobbyListener.PlayerConnected(startRunLobbyPlayer.Value);
			PlayerConnected?.Invoke(startRunLobbyPlayer.Value);
		}
		catch
		{
			netHostGameService.DisconnectClient(senderId, NetError.InternalError);
			throw;
		}
	}

	/// <summary>
	/// Updates the max ascension in Blitz and multiplayer.
	/// In Blitz, ascensions are shared across all characters.
	/// In multiplayer, the party is limited to the lowest Ascension any member has unlocked. A player with a higher
	/// unlock cannot raise that limit, and after a win only the members who were on their own maximum advance. See
	/// <see cref="M:MegaCrit.Sts2.Core.Saves.Managers.ProgressSaveManager.IncrementMultiplayerAscension(MegaCrit.Sts2.Core.Saves.SerializableRun)" />.
	/// </summary>
	private void UpdateMaxSharedAscension(bool sync = true)
	{
		if (!NetService.Type.IsMultiplayer() && GameMode != GameMode.Blitz)
		{
			return;
		}
		int num = Players.Min((StartRunLobbyPlayer p) => (GameMode != GameMode.Blitz) ? p.maxStandardAscension : p.maxBlitzAscension);
		if (num != MaxAscension)
		{
			MaxAscension = num;
			LobbyListener.MaxAscensionChanged();
			if (Ascension > MaxAscension && NetService.Type == NetGameType.Host)
			{
				SetAscensionInternal(MaxAscension, sync);
			}
		}
	}

	private void HandleClientLoadJoinRequestMessage(ClientLoadJoinRequestMessage _, ulong senderId)
	{
		if (NetService.Type != NetGameType.Host)
		{
			throw new InvalidOperationException("Received ClientLoadJoinRequestMessage as non-host!");
		}
		_logger.Info($"Received invalid ClientLoadJoinRequestMessage for {senderId}");
		NetHostGameService netHostGameService = (NetHostGameService)NetService;
		netHostGameService.DisconnectClient(senderId, NetError.InvalidJoin);
	}

	private void HandleClientRejoinRequestMessage(ClientRejoinRequestMessage _, ulong senderId)
	{
		if (NetService.Type != NetGameType.Host)
		{
			throw new InvalidOperationException("Received ClientRejoinRequestMessage as non-host!");
		}
		_logger.Info($"Received invalid ClientRejoinRequestMessage for {senderId}");
		NetHostGameService netHostGameService = (NetHostGameService)NetService;
		netHostGameService.DisconnectClient(senderId, NetError.InvalidJoin);
	}

	private void HandlePlayerJoinedMessage(PlayerJoinedMessage message, ulong senderId)
	{
		_logger.Debug($"Received PlayerJoinedMessage with ({message.lobbyPlayer})");
		Players.Add(message.lobbyPlayer);
		LobbyListener.PlayerConnected(message.lobbyPlayer);
		PlayerConnected?.Invoke(message.lobbyPlayer);
		UpdateMaxSharedAscension();
	}

	private void HandlePlayerLeftMessage(PlayerLeftMessage message, ulong senderId)
	{
		_logger.Debug($"Received PlayerLeftMessage for {message.playerId}");
		int num = Players.FindIndex((StartRunLobbyPlayer p) => p.id == message.playerId);
		if (num >= 0)
		{
			StartRunLobbyPlayer startRunLobbyPlayer = Players[num];
			Players.RemoveAt(num);
			InputSynchronizer.OnPlayerDisconnected(startRunLobbyPlayer.id);
			LobbyListener.RemotePlayerDisconnected(startRunLobbyPlayer);
			PlayerDisconnected?.Invoke(startRunLobbyPlayer);
		}
	}

	private void HandleLobbyPlayerChangedCharacterMessage(LobbyPlayerChangedCharacterMessage message, ulong senderId)
	{
		_logger.Debug($"Received LobbyPlayerChangedCharacterMessage for {senderId} ({message.character}). Is rotating: {message.isRotatingDread}");
		ChangeCharacter(senderId, message.character, message.isRotatingDread);
	}

	private void HandleAscensionChangedMessage(LobbyAscensionChangedMessage message, ulong _)
	{
		if (_isBeginningRun)
		{
			Log.Warn($"Received AscensionChangedMessage with ascension {message.ascension} while run was already starting! Ignoring");
		}
		else
		{
			_logger.Debug($"Received AscensionChangedMessage, new ascension: {message.ascension}");
			Ascension = message.ascension;
			LobbyListener.AscensionChanged();
		}
	}

	private void HandleSeedChangedMessage(LobbySeedChangedMessage message, ulong _)
	{
		if (_isBeginningRun)
		{
			Log.Warn("Received SeedChangedMessage with seed " + message.seed + " while run was already starting! Ignoring");
			return;
		}
		_logger.Debug("Received SeedChangedMessage, new seed: " + message.seed);
		Seed = message.seed;
		LobbyListener.SeedChanged();
	}

	private void HandleModifiersChangedMessage(LobbyModifiersChangedMessage message, ulong _)
	{
		_logger.Debug("Received ModifiersChangedMessage, new modifiers: " + string.Join(",", message.modifiers.Select((SerializableModifier m) => m.Id)));
		if (_isBeginningRun)
		{
			Log.Warn($"Received ModifiersChangedMessage with {message.modifiers.Count} while run was already starting! Ignoring");
		}
		else
		{
			_modifiers.Clear();
			_modifiers.AddRange(message.modifiers.Select(ModifierModel.FromSerializable));
			LobbyListener.ModifiersChanged();
		}
	}

	private void HandleGameModeChangedMessage(LobbyGameModeChangedMessage message, ulong _)
	{
		_logger.Debug($"Received GameModeChangedMessage: {message.gameMode}");
		if (_isBeginningRun)
		{
			Log.Warn($"Received GameModeChangedMessage with {message.gameMode} while run was already starting! Ignoring");
		}
		else
		{
			GameMode = message.gameMode;
			Ascension = message.ascension;
			MaxAscension = message.maxAscension;
			LobbyListener.GameModeChanged();
		}
	}

	private void HandlePlayerReadyMessage(LobbyPlayerSetReadyMessage message, ulong senderId)
	{
		_logger.Debug($"Received LobbyPlayerSetReadyMessage for player {senderId} with value {message.ready}");
		int num = Players.FindIndex((StartRunLobbyPlayer p) => p.id == senderId);
		if (num >= 0)
		{
			StartRunLobbyPlayer startRunLobbyPlayer = Players[num];
			startRunLobbyPlayer.isReady = message.ready;
			Players[num] = startRunLobbyPlayer;
			LobbyListener.PlayerChanged(startRunLobbyPlayer);
			BeginRunForAllPlayersIfAllReady();
		}
	}

	private void HandleLobbyBeginRunMessage(LobbyBeginRunMessage message, ulong senderId)
	{
		_logger.Debug("Received LobbyBeginRunMessage");
		Players.Clear();
		Players.AddRange(message.playersInLobby);
		Act1 = message.act1;
		BeginRunLocally(message.seed, message.modifiers.Select(ModifierModel.FromSerializable).ToList());
	}

	private void ChangeCharacter(ulong playerId, CharacterModel character, bool isRotatingDread)
	{
		if (_isBeginningRun)
		{
			Log.Warn($"Player {playerId} tried to change character while run was already starting! Ignoring");
			return;
		}
		int num = Players.FindIndex((StartRunLobbyPlayer p) => p.id == playerId);
		if (num >= 0)
		{
			StartRunLobbyPlayer startRunLobbyPlayer = Players[num];
			startRunLobbyPlayer.character = character;
			startRunLobbyPlayer.isRotatingDread = isRotatingDread;
			Players[num] = startRunLobbyPlayer;
			LobbyListener.PlayerChanged(startRunLobbyPlayer);
		}
	}

	private void BeginRunForAllPlayers(string seed, List<ModifierModel> modifiers)
	{
		if (NetService.Type == NetGameType.Client)
		{
			throw new InvalidOperationException("Can only begin run as host!");
		}
		if (_isBeginningRun)
		{
			_logger.Warn("Tried to begin run twice, ignoring second one!");
			return;
		}
		UpdatePreferredAscension();
		LobbyBeginRunMessage message = new LobbyBeginRunMessage
		{
			playersInLobby = Players,
			seed = seed,
			modifiers = modifiers.Select((ModifierModel m) => m.ToSerializable()).ToList(),
			act1 = Act1
		};
		NetService.SendMessage(message);
		BeginRunLocally(seed, modifiers);
		if (NetService.Type == NetGameType.Host)
		{
			INetHostGameService netHostGameService = (INetHostGameService)NetService;
			netHostGameService.NetHost?.SetHostIsClosed(isClosed: true);
		}
	}

	private void BeginRunLocally(string seed, List<ModifierModel> modifiers)
	{
		Rng rng = new Rng(StringHelper.GetDeterministicHashCode(seed), "act_selection");
		List<ActModel> list = ActModel.GetRandomList(rng, GetUnlockState(), NetService.Type.IsMultiplayer()).ToList();
		list[0] = GetAct(Act1) ?? list[0];
		for (int i = 0; i < Players.Count; i++)
		{
			StartRunLobbyPlayer startRunLobbyPlayer = Players[i];
			CharacterModel character = startRunLobbyPlayer.character;
			if ((character is RandomCharacter || character is RotatingCharacter) ? true : false)
			{
				if (GameMode == GameMode.Dread)
				{
					throw new InvalidOperationException("Random or rotating character selected in Dread! This should never happen.");
				}
				CharacterModel character2 = rng.NextItem(ModelDb.AllCharacters);
				ChangeCharacter(startRunLobbyPlayer.id, character2, isRotatingDread: false);
			}
		}
		if (NetService.Type == NetGameType.Singleplayer && GameMode != GameMode.Blitz)
		{
			CharacterStats orCreateCharacterStats = SaveManager.Instance.Progress.GetOrCreateCharacterStats(Players[0].character.Id);
			int num = Math.Min(Ascension, orCreateCharacterStats.MaxAscension);
			if (Ascension != num)
			{
				Ascension = num;
				LobbyListener.AscensionChanged();
			}
			if (MaxAscension != orCreateCharacterStats.MaxAscension)
			{
				MaxAscension = orCreateCharacterStats.MaxAscension;
				LobbyListener.MaxAscensionChanged();
			}
		}
		NetService.SetBufferMessages(bufferMessages: true);
		_isBeginningRun = true;
		LobbyListener.BeginRun(seed, list, modifiers);
	}

	private static ActModel? GetAct(string act1Key)
	{
		if (!(act1Key == "overgrowth"))
		{
			if (act1Key == "underdocks")
			{
				return ModelDb.Act<Underdocks>();
			}
			return null;
		}
		return ModelDb.Act<Overgrowth>();
	}

	private void SetSingleplayerAscensionAfterCharacterChanged(ModelId characterId, bool sync = true)
	{
		if (NetService.Type.IsMultiplayer() || GameMode == GameMode.Blitz || GameMode == GameMode.Dread)
		{
			return;
		}
		CharacterStats orCreateCharacterStats = SaveManager.Instance.Progress.GetOrCreateCharacterStats(characterId);
		bool flag = IsAscensionEpochRevealed(characterId);
		if (characterId == ModelDb.GetId<RandomCharacter>())
		{
			MaxAscension = GetMaxAscensionAcrossAllCharacters();
			SetAscensionInternal(Math.Min(orCreateCharacterStats.PreferredAscension, MaxAscension), sync);
			LobbyListener.MaxAscensionChanged();
			_logger.Info($"{characterId} ascension set to Max: {Ascension}");
		}
		else if (orCreateCharacterStats == null || orCreateCharacterStats.MaxAscension <= 0 || !flag)
		{
			MaxAscension = 0;
			SetAscensionInternal(0, sync);
			LobbyListener.MaxAscensionChanged();
			if (!flag)
			{
				_logger.Info($"{characterId} has not revealed the Ascension Epoch, disabling Ascension.");
			}
			else
			{
				_logger.Info($"{characterId} has no progress, disabling Ascension.");
			}
		}
		else
		{
			MaxAscension = orCreateCharacterStats.MaxAscension;
			SetAscensionInternal(Math.Min(orCreateCharacterStats.PreferredAscension, orCreateCharacterStats.MaxAscension), sync);
			LobbyListener.MaxAscensionChanged();
			_logger.Info($"{characterId} ascension set to preferred: {Ascension}");
		}
	}

	public int GetMaxAscensionAcrossAllCharacters()
	{
		int num = 0;
		foreach (CharacterStats value in SaveManager.Instance.Progress.CharacterStats.Values)
		{
			num = Math.Max(num, value.MaxAscension);
		}
		return num;
	}

	private bool IsAscensionEpochRevealed(ModelId characterId)
	{
		if (characterId == ModelDb.GetId<Ironclad>())
		{
			return SaveManager.Instance.IsEpochRevealed<Ironclad4Epoch>();
		}
		if (characterId == ModelDb.GetId<Silent>())
		{
			return SaveManager.Instance.IsEpochRevealed<Silent4Epoch>();
		}
		if (characterId == ModelDb.GetId<Regent>())
		{
			return SaveManager.Instance.IsEpochRevealed<Regent4Epoch>();
		}
		if (characterId == ModelDb.GetId<Defect>())
		{
			return SaveManager.Instance.IsEpochRevealed<Defect4Epoch>();
		}
		if (characterId == ModelDb.GetId<Necrobinder>())
		{
			return SaveManager.Instance.IsEpochRevealed<Necrobinder4Epoch>();
		}
		if (characterId == ModelDb.GetId<Fool>())
		{
			return SaveManager.Instance.IsEpochRevealed<Fool4Epoch>();
		}
		return true;
	}

	/// <summary>
	/// Updates the preferred Ascension. Does nothing for Daily and Dread.
	/// </summary>
	private void UpdatePreferredAscension()
	{
		GameMode gameMode = GameMode;
		if ((uint)(gameMode - 2) <= 1u)
		{
			return;
		}
		if (GameMode == GameMode.Blitz && NetService.Type != NetGameType.Client)
		{
			ProgressState progress = SaveManager.Instance.Progress;
			if (progress.PreferredBlitzAscension != Ascension)
			{
				_logger.Info($"Setting preferred blitz ascension to {Ascension}");
				progress.PreferredBlitzAscension = Ascension;
				SaveManager.Instance.SaveProgressFile();
			}
		}
		else if (NetService.Type == NetGameType.Singleplayer)
		{
			if (Players.Count != 0)
			{
				CharacterStats orCreateCharacterStats = SaveManager.Instance.Progress.GetOrCreateCharacterStats(LocalPlayer.character.Id);
				if ((orCreateCharacterStats.MaxAscension != 0 || !(orCreateCharacterStats.Id != ModelDb.Character<RandomCharacter>().Id)) && orCreateCharacterStats.PreferredAscension != Ascension)
				{
					_logger.Info($"Setting preferred Ascension for {LocalPlayer.character.Id} to {Ascension}");
					orCreateCharacterStats.PreferredAscension = Ascension;
					SaveManager.Instance.SaveProgressFile();
				}
			}
		}
		else if (NetService.Type == NetGameType.Host)
		{
			ProgressState progress2 = SaveManager.Instance.Progress;
			if (progress2.PreferredMultiplayerAscension != Ascension)
			{
				_logger.Info($"Setting preferred multiplayer ascension to {Ascension}");
				progress2.PreferredMultiplayerAscension = Ascension;
				SaveManager.Instance.SaveProgressFile();
			}
		}
	}

	/// <summary>
	/// Sets the character chosen by the local player and replicates that to all peers.
	/// </summary>
	/// <param name="character">The character the player is now playing as.</param>
	/// <param name="isRotatingDread">If playing dread and the local player is doing a rotating streak (either beginning
	/// or continuing one), then this should be set to true so that state is saved correctly.</param>
	public void SetLocalCharacter(CharacterModel character, bool isRotatingDread = false)
	{
		ChangeCharacter(NetService.NetId, character, isRotatingDread);
		LobbyPlayerChangedCharacterMessage message = new LobbyPlayerChangedCharacterMessage
		{
			character = character,
			isRotatingDread = isRotatingDread
		};
		NetService.SendMessage(message);
		SetSingleplayerAscensionAfterCharacterChanged(character.Id);
	}

	/// <summary>
	/// Sets the seed to use for the run.
	/// This can only be called as host or singleplayer. Calling it on the client will throw an exception and have no
	/// effect. The seed is synced to the clients for display use, but the final seed that is used is sent in the
	/// <see cref="T:MegaCrit.Sts2.Core.Multiplayer.Messages.Lobby.LobbyBeginRunMessage" />.
	/// </summary>
	public void SetSeed(string? seed)
	{
		NetGameType type = NetService.Type;
		if ((uint)(type - 1) > 1u)
		{
			throw new InvalidOperationException("Can only be called on host or singleplayer");
		}
		Seed = seed;
		LobbySeedChangedMessage message = new LobbySeedChangedMessage
		{
			seed = seed
		};
		NetService.SendMessage(message);
		LobbyListener.SeedChanged();
	}

	/// <summary>
	/// Sets the modifiers to use for the run.
	/// This can only be called as host or singleplayer. Calling it on the client will throw an exception and have no
	/// effect. The seed is synced to the clients for display use, but the final seed that is used is sent in the
	/// <see cref="T:MegaCrit.Sts2.Core.Multiplayer.Messages.Lobby.LobbyBeginRunMessage" />.
	/// </summary>
	public void SetModifiers(IReadOnlyCollection<ModifierModel> modifiers)
	{
		NetGameType type = NetService.Type;
		if ((uint)(type - 1) > 1u)
		{
			throw new InvalidOperationException("Can only be called on host or singleplayer");
		}
		if (_isBeginningRun)
		{
			Log.Warn("Tried to change modifiers while run was already starting! Ignoring");
			return;
		}
		_modifiers.Clear();
		_modifiers.AddRange(modifiers);
		LobbyModifiersChangedMessage message = new LobbyModifiersChangedMessage
		{
			modifiers = modifiers.Select((ModifierModel m) => m.ToSerializable()).ToList()
		};
		NetService.SendMessage(message);
		LobbyListener.ModifiersChanged();
	}

	/// <summary>
	/// Sets the game mode that will be played during the run, replicating it to peers.
	/// </summary>
	public void SetGameMode(GameMode gameMode)
	{
		NetGameType type = NetService.Type;
		if ((uint)(type - 1) > 1u)
		{
			throw new InvalidOperationException("Can only be called on host or singleplayer");
		}
		if (_isBeginningRun)
		{
			Log.Warn("Tried to change game mode while run was already starting! Ignoring");
			return;
		}
		GameMode = gameMode;
		if (GameMode == GameMode.Dread)
		{
			MaxAscension = 10;
			Ascension = MaxAscension;
		}
		else if (NetService.Type.IsMultiplayer() || GameMode == GameMode.Blitz)
		{
			if (GameMode == GameMode.Blitz)
			{
				Ascension = SaveManager.Instance.Progress.PreferredBlitzAscension;
			}
			else
			{
				Ascension = SaveManager.Instance.Progress.PreferredMultiplayerAscension;
			}
			UpdateMaxSharedAscension(sync: false);
		}
		else
		{
			SetSingleplayerAscensionAfterCharacterChanged(LocalPlayer.character.Id, sync: false);
		}
		LobbyGameModeChangedMessage message = new LobbyGameModeChangedMessage
		{
			gameMode = gameMode,
			ascension = Ascension,
			maxAscension = MaxAscension
		};
		NetService.SendMessage(message);
		LobbyListener.GameModeChanged();
	}

	/// <summary>
	/// Sets the local player to be ready or unready and syncs the state to all peers.
	/// This is called for both Singleplayer and Multiplayer (singleplayer uses a local lobby).
	/// </summary>
	/// <param name="ready"></param>
	public void SetReady(bool ready)
	{
		int num = Players.FindIndex((StartRunLobbyPlayer p) => p.id == NetService.NetId);
		if (num < 0)
		{
			throw new InvalidOperationException("Tried to set local player ready, but they are not in the list of players in the lobby!");
		}
		if (_isBeginningRun)
		{
			Log.Warn("Tried to set ready while run was already starting! Ignoring");
			return;
		}
		StartRunLobbyPlayer value = Players[num];
		value.isReady = ready;
		Players[num] = value;
		LobbyPlayerSetReadyMessage message = new LobbyPlayerSetReadyMessage
		{
			ready = ready
		};
		NetService.SendMessage(message);
		LobbyListener.PlayerChanged(LocalPlayer);
		_logger.Info($"Local player {LocalPlayer.id} is ready");
		BeginRunForAllPlayersIfAllReady();
	}

	private void BeginRunForAllPlayersIfAllReady()
	{
		if (IsAboutToBeginGame())
		{
			NetGameType type = NetService.Type;
			if ((uint)(type - 1) <= 1u)
			{
				string seed;
				if (NGame.Instance?.DebugSeedOverride != null)
				{
					seed = NGame.Instance.DebugSeedOverride;
				}
				else
				{
					seed = ((Seed == null) ? SeedHelper.GetRandomSeed() : SeedHelper.CanonicalizeSeed(Seed));
				}
				BeginRunForAllPlayers(seed, _modifiers);
			}
		}
	}

	public bool IsAboutToBeginGame()
	{
		if (_connectingPlayers.Count > 0)
		{
			return false;
		}
		if (NetService.Type.IsMultiplayer() && Players.Count == 1)
		{
			return false;
		}
		if (!Players.All((StartRunLobbyPlayer p) => p.isReady))
		{
			return false;
		}
		return true;
	}

	/// <summary>
	/// Sets the ascension level. Should only be called on the host.
	/// </summary>
	public void SyncAscensionChange(int ascension)
	{
		if (NetService.Type == NetGameType.Client)
		{
			throw new InvalidOperationException("Client attempted to change ascension level!");
		}
		if (_isBeginningRun)
		{
			Log.Warn($"Tried to set ascension to {ascension} while run was already starting! Ignoring");
		}
		else
		{
			SetAscensionInternal(ascension);
		}
	}

	private void SetAscensionInternal(int ascension, bool sync = true)
	{
		if (Ascension == ascension)
		{
			return;
		}
		Ascension = ascension;
		if (sync)
		{
			if (NetService.IsConnected)
			{
				LobbyAscensionChangedMessage message = new LobbyAscensionChangedMessage
				{
					ascension = ascension
				};
				NetService.SendMessage(message);
			}
			UpdatePreferredAscension();
			LobbyListener.AscensionChanged();
		}
	}

	private StartRunLobbyPlayer? TryAddPlayerInFirstAvailableSlot(SerializableUnlockState unlockState, int maxStandardAscension, int maxBlitzAscension, bool isModded, ulong playerId)
	{
		int num = -1;
		int i;
		for (i = 0; i < _maxPlayers; i++)
		{
			int num2 = Players.FindIndex((StartRunLobbyPlayer p) => p.slotId == i);
			if (num2 < 0)
			{
				num = i;
				break;
			}
		}
		if (num < 0)
		{
			return null;
		}
		StartRunLobbyPlayer startRunLobbyPlayer = new StartRunLobbyPlayer
		{
			character = ModelDb.Character<Ironclad>(),
			id = playerId,
			slotId = num,
			maxBlitzAscension = maxBlitzAscension,
			maxStandardAscension = maxStandardAscension,
			unlockState = unlockState,
			isModded = isModded
		};
		Players.Add(startRunLobbyPlayer);
		return startRunLobbyPlayer;
	}

	private void OnConnectedToClientAsHost(ulong playerId)
	{
		_logger.Info($"Client {playerId} connected. Sending initial game info message");
		InitialGameInfoMessage message = new InitialGameInfoMessage
		{
			sessionState = RunSessionState.InLobby
		};
		if (_isBeginningRun)
		{
			message.connectionFailureReason = ConnectionFailureReason.RunInProgress;
			NetService.SendMessage(message, playerId);
			_logger.Warn($"Client {playerId} connected but we are already beginning the run!");
			((NetHostGameService)NetService).DisconnectClient(playerId, NetError.RunInProgress);
		}
		else if (Players.Count >= _maxPlayers)
		{
			message.connectionFailureReason = ConnectionFailureReason.LobbyFull;
			NetService.SendMessage(message, playerId);
			_logger.Warn($"Client {playerId} connected but we are at maximum players!");
			((NetHostGameService)NetService).DisconnectClient(playerId, NetError.LobbyFull);
		}
		else
		{
			ConnectingPlayer connectingPlayer = new ConnectingPlayer
			{
				id = playerId,
				timeoutCancelToken = new CancellationTokenSource()
			};
			_connectingPlayers.Add(connectingPlayer);
			NetService.SendMessage(message, playerId);
			TaskHelper.RunSafely(BeginClientResponseTimeout(connectingPlayer));
		}
	}

	private async Task BeginClientResponseTimeout(ConnectingPlayer connectingPlayer)
	{
		await Task.Delay(ClientResponseTimeout, connectingPlayer.timeoutCancelToken.Token);
		if (!connectingPlayer.timeoutCancelToken.IsCancellationRequested)
		{
			int num = _connectingPlayers.IndexOf(connectingPlayer);
			if (num >= 0)
			{
				_logger.Info($"Disconnecting player {connectingPlayer.id} because they did not respond to the initial game join message within {ClientResponseTimeout}ms");
				INetHostGameService netHostGameService = (INetHostGameService)NetService;
				netHostGameService.DisconnectClient(connectingPlayer.id, NetError.LobbyJoinTimeout);
			}
		}
	}

	private void OnDisconnectedFromClientAsHost(ulong playerId, NetErrorInfo info)
	{
		_logger.Info($"Client {playerId} disconnected, reason: {info.GetReason()}");
		RemoveConnectingPlayer(playerId);
		int num = Players.FindIndex((StartRunLobbyPlayer p) => p.id == playerId);
		if (num < 0)
		{
			_logger.Info($"Player {playerId} not found in players list. Assuming they disconnected before fully finishing connection");
			if ((object)info.ConnectionExtraInfo == null)
			{
				ConnectionFailureExtraInfo connectionFailureExtraInfo = new ConnectionFailureExtraInfo
				{
					localIsHost = true
				};
				ConnectionFailureExtraInfo connectionFailureExtraInfo2 = connectionFailureExtraInfo;
				info.ConnectionExtraInfo = connectionFailureExtraInfo;
			}
			OnClientConnectionFailed(playerId, info);
		}
		else
		{
			StartRunLobbyPlayer startRunLobbyPlayer = Players[num];
			PlayerLeftMessage message = new PlayerLeftMessage
			{
				playerId = playerId
			};
			NetService.SendMessage(message);
			Players.RemoveAt(num);
			InputSynchronizer.OnPlayerDisconnected(startRunLobbyPlayer.id);
			LobbyListener.RemotePlayerDisconnected(startRunLobbyPlayer);
			PlayerDisconnected?.Invoke(startRunLobbyPlayer);
			UpdateMaxSharedAscension();
			BeginRunForAllPlayersIfAllReady();
		}
	}

	public List<Player> GeneratePlayers(string seed)
	{
		List<Player> list = new List<Player>();
		for (int i = 0; i < Players.Count; i++)
		{
			StartRunLobbyPlayer startRunLobbyPlayer = Players[i];
			Player player = Player.CreateForNewRun(startRunLobbyPlayer.character, UnlockState.FromSerializable(startRunLobbyPlayer.unlockState), startRunLobbyPlayer.id);
			player.ExtraFields.IsPlayingRotatingCharacter = startRunLobbyPlayer.isRotatingDread;
			list.Add(player);
		}
		return list;
	}

	private void OnClientConnectionFailed(ulong playerId, NetErrorInfo info)
	{
		PlayerFailedToConnect?.Invoke(playerId, info);
	}

	private UnlockState GetUnlockState()
	{
		if (GameMode == GameMode.Daily)
		{
			return UnlockState.all;
		}
		return new UnlockState(Players.Select((StartRunLobbyPlayer p) => UnlockState.FromSerializable(p.unlockState)));
	}

	private void RemoveConnectingPlayer(ulong playerId)
	{
		for (int i = 0; i < _connectingPlayers.Count; i++)
		{
			if (_connectingPlayers[i].id == playerId)
			{
				_connectingPlayers[i].timeoutCancelToken.Cancel();
				_logger.Info($"Cancel initial message timeout for {playerId}");
				_connectingPlayers.RemoveAt(i);
				i--;
			}
		}
	}

	private void OnDisconnected(NetErrorInfo info)
	{
		_logger.Info($"Disconnected from host, reason: {info.GetReason()}");
		LobbyListener.LocalPlayerDisconnected(info);
	}
}
