using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Achievements;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Audio.Debug;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Daily;
using MegaCrit.Sts2.Core.Entities.Ascension;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Save;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Map;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Events;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Multiplayer;
using MegaCrit.Sts2.Core.Multiplayer.Game;
using MegaCrit.Sts2.Core.Multiplayer.Game.Lobby;
using MegaCrit.Sts2.Core.Multiplayer.Game.PeerInput;
using MegaCrit.Sts2.Core.Multiplayer.Messages.Lobby;
using MegaCrit.Sts2.Core.Multiplayer.Replay;
using MegaCrit.Sts2.Core.Multiplayer.Serialization;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Audio;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.Screens.Capstones;
using MegaCrit.Sts2.Core.Nodes.Screens.Map;
using MegaCrit.Sts2.Core.Nodes.Screens.Overlays;
using MegaCrit.Sts2.Core.Nodes.Screens.ScreenContext;
using MegaCrit.Sts2.Core.Platform;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs.History;
using MegaCrit.Sts2.Core.Runs.Metrics;
using MegaCrit.Sts2.Core.Saves;
using MegaCrit.Sts2.Core.Saves.MapDrawing;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.TestSupport;
using MegaCrit.Sts2.Core.Timeline.Epochs;

namespace MegaCrit.Sts2.Core.Runs;

public class RunManager : IRunLobbyListener
{
	private long _startTime;

	private long _prevSessionRunTime;

	private long _activeRunTime;

	private long _startOfCurrentActiveRunTime;

	private bool _isPaused;

	private bool _runHistoryWasUploaded;

	private int _numReloads;

	public Action? debugAfterCombatRewardsOverride;

	public static RunManager Instance { get; } = new RunManager();

	public AscensionManager AscensionManager { get; private set; }

	public bool ShouldSave { get; private set; }

	public DateTimeOffset? DailyTime { get; private set; }

	public bool IsInProgress => State != null;

	public bool IsCleaningUp { get; private set; }

	public bool ForceDiscoveryOrderModifications { get; set; }

	public bool IsGameOver
	{
		get
		{
			if (IsInProgress)
			{
				return State.IsGameOver;
			}
			return false;
		}
	}

	public bool IsAbandoned { get; private set; }

	public bool HasRunEnded { get; private set; }

	public bool IsPaused
	{
		get
		{
			return _isPaused;
		}
		set
		{
			_isPaused = value;
			RunState state = State;
			if (state == null)
			{
				return;
			}
			IReadOnlyList<Player> players = state.Players;
			if (players != null && players.Count == 1)
			{
				if (_isPaused)
				{
					_activeRunTime += DateTimeOffset.UtcNow.ToUnixTimeSeconds() - _startOfCurrentActiveRunTime;
				}
				else
				{
					_startOfCurrentActiveRunTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
				}
			}
		}
	}

	public RunHistory? History { get; set; }

	public INetGameService NetService { get; private set; }

	public ChecksumTracker ChecksumTracker { get; private set; }

	public RunLocationTargetedMessageBuffer RunLocationTargetedBuffer { get; private set; }

	public CombatReplayWriter CombatReplayWriter { get; private set; }

	public RunLobby? RunLobby { get; private set; }

	public CombatStateSynchronizer CombatStateSynchronizer { get; private set; }

	public MapSelectionSynchronizer MapSelectionSynchronizer { get; private set; }

	public ActChangeSynchronizer ActChangeSynchronizer { get; private set; }

	public PlayerChoiceSynchronizer PlayerChoiceSynchronizer { get; private set; }

	public EventSynchronizer EventSynchronizer { get; private set; }

	public RewardSynchronizer RewardSynchronizer { get; private set; }

	public RewardsSetSynchronizer RewardsSetSynchronizer { get; private set; }

	public RestSiteSynchronizer RestSiteSynchronizer { get; private set; }

	public MerchantSynchronizer MerchantSynchronizer { get; private set; }

	public OneOffSynchronizer OneOffSynchronizer { get; private set; }

	public TreasureRoomRelicSynchronizer TreasureRoomRelicSynchronizer { get; private set; }

	public FlavorSynchronizer FlavorSynchronizer { get; private set; }

	public PeerInputSynchronizer InputSynchronizer { get; private set; }

	public HoveredModelTracker HoveredModelTracker { get; private set; }

	public ActionQueueSet ActionQueueSet { get; private set; }

	public ActionExecutor ActionExecutor { get; private set; }

	public ActionQueueSynchronizer ActionQueueSynchronizer { get; private set; }

	public CardSelectorStack CardSelectorStack { get; private set; }

	public long WinTime { get; set; }

	public long RunTime
	{
		get
		{
			if (WinTime > 0)
			{
				return WinTime;
			}
			if (IsPaused)
			{
				RunState state = State;
				if (state != null)
				{
					IReadOnlyList<Player> players = state.Players;
					if (players != null && players.Count == 1)
					{
						return _activeRunTime + _prevSessionRunTime;
					}
				}
			}
			return DateTimeOffset.UtcNow.ToUnixTimeSeconds() - _startOfCurrentActiveRunTime + _activeRunTime + _prevSessionRunTime;
		}
	}

	public bool IsSingleplayerOrFakeMultiplayer
	{
		get
		{
			if (IsInProgress)
			{
				return NetService.Type == NetGameType.Singleplayer;
			}
			return false;
		}
	}

	public SerializableMapDrawings? MapDrawingsToLoad { get; set; }

	public Dictionary<int, SerializableActMap>? SavedMapsToLoad { get; set; }

	public bool IsChangingRooms { get; private set; }

	public bool IsPotionUseLocked
	{
		get
		{
			if (!IsChangingRooms)
			{
				return CombatManager.Instance.IsStarting;
			}
			return true;
		}
	}

	private RunState? State { get; set; }

	public event Action<RunState>? RunStarted;

	public event Action? RoomEntered;

	public event Action? RoomExited;

	public event Action? OnIsChangingRoomsModified;

	public event Action? ActEntered;

	public event Action<bool>? MapChanged;

	public event Func<Task>? TestFadeOut;

	public event Func<Task>? TestFadeIn;

	private RunManager()
	{
	}

	public void SetUpNewSingleplayer(RunState state, bool shouldSave, DateTimeOffset? dailyTime = null)
	{
		if (State != null)
		{
			throw new InvalidOperationException("State is already set.");
		}
		State = state;
		INetGameService netGameService = new NetSingleplayerGameService();
		InitializeShared(netGameService, new PeerInputSynchronizer(netGameService), shouldSave, dailyTime, DateTimeOffset.UtcNow.ToUnixTimeSeconds(), 0L, 0L, 0);
		IEnumerable<RunLobbyPlayer> players = new _003C_003Ez__ReadOnlySingleElementList<RunLobbyPlayer>(new RunLobbyPlayer
		{
			id = netGameService.NetId,
			isModded = netGameService.LocalVersion.IsModded()
		});
		InitializeRunLobby(netGameService, state, players);
		InitializeNewRun();
		GenerateRooms();
	}

	public void SetUpNewMultiplayer(RunState state, StartRunLobby lobby, bool shouldSave, DateTimeOffset? dailyTime = null)
	{
		if (State != null)
		{
			throw new InvalidOperationException("State is already set.");
		}
		State = state;
		InitializeShared(lobby.NetService, lobby.InputSynchronizer, shouldSave, dailyTime, DateTimeOffset.UtcNow.ToUnixTimeSeconds(), 0L, 0L, 0);
		IEnumerable<RunLobbyPlayer> players = lobby.Players.Select((StartRunLobbyPlayer p) => new RunLobbyPlayer
		{
			id = p.id,
			isModded = p.isModded
		});
		InitializeRunLobby(lobby.NetService, state, players);
		InitializeNewRun();
		GenerateRooms();
	}

	public async Task SetUpSavedSingleplayer(RunState state, SerializableRun save)
	{
		if (State != null)
		{
			throw new InvalidOperationException("State is already set.");
		}
		State = state;
		await SaveManager.Instance.IncrementNumReloads(save, NetGameType.Singleplayer);
		INetGameService netGameService = new NetSingleplayerGameService();
		InitializeShared(netGameService, new PeerInputSynchronizer(netGameService), shouldSave: true, save.DailyTime, save.StartTime, save.RunTime, save.WinTime, save.NumReloads);
		IEnumerable<RunLobbyPlayer> players = new _003C_003Ez__ReadOnlySingleElementList<RunLobbyPlayer>(new RunLobbyPlayer
		{
			id = netGameService.NetId,
			isModded = netGameService.LocalVersion.IsModded()
		});
		InitializeRunLobby(netGameService, state, players);
		InitializeSavedRun(save);
	}

	public async Task SetUpSavedMultiplayer(RunState state, LoadRunLobby lobby)
	{
		if (State != null)
		{
			throw new InvalidOperationException("State is already set.");
		}
		State = state;
		SerializableRun save = lobby.Run;
		await SaveManager.Instance.IncrementNumReloads(save, lobby.NetService.Type);
		InitializeShared(lobby.NetService, lobby.InputSynchronizer, shouldSave: true, save.DailyTime, save.StartTime, save.RunTime, save.WinTime, save.NumReloads);
		IEnumerable<RunLobbyPlayer> players = lobby.Players.Select((LoadRunLobbyPlayer p) => new RunLobbyPlayer
		{
			id = p.id,
			isModded = p.isModded
		});
		InitializeRunLobby(lobby.NetService, state, players);
		InitializeSavedRun(save);
	}

	public void SetUpReplay(RunState state, CombatReplay replay, ulong playerIdToLoad)
	{
		if (State != null)
		{
			throw new InvalidOperationException("State is already set.");
		}
		State = state;
		SerializableRun serializableRun = replay.serializableRun;
		NetReplayGameService netService = new NetReplayGameService(playerIdToLoad);
		InitializeShared(netService, new PeerInputSynchronizer(netService), shouldSave: true, serializableRun.DailyTime, serializableRun.StartTime, serializableRun.RunTime, serializableRun.WinTime, serializableRun.NumReloads);
		IEnumerable<RunLobbyPlayer> players = state.Players.Select((Player p) => new RunLobbyPlayer
		{
			id = p.NetId,
			isModded = netService.LocalVersion.IsModded()
		});
		InitializeRunLobby(netService, state, players);
		InitializeSavedRun(serializableRun);
	}

	public void SetUpTest(RunState state, INetGameService gameService, bool disableCombatStateSync = true, bool shouldSave = false)
	{
		if (State != null)
		{
			throw new InvalidOperationException("State is already set.");
		}
		State = state;
		InitializeShared(gameService, new PeerInputSynchronizer(gameService), shouldSave, null, DateTimeOffset.UtcNow.ToUnixTimeSeconds(), 0L, 0L, 0);
		IEnumerable<RunLobbyPlayer> players = state.Players.Select((Player p) => new RunLobbyPlayer
		{
			id = p.NetId,
			isModded = gameService.LocalVersion.IsModded()
		});
		InitializeRunLobby(gameService, state, players);
		CombatStateSynchronizer.IsDisabled = disableCombatStateSync;
		InitializeNewRun();
	}

	private void InitializeShared(INetGameService netService, PeerInputSynchronizer inputSynchronizer, bool shouldSave, DateTimeOffset? dailyTime, long startTime, long runTime, long winTime, int numReloads)
	{
		if (State == null)
		{
			throw new InvalidOperationException("State is not set.");
		}
		NetService = netService;
		ulong netId = NetService.NetId;
		ChecksumTracker = new ChecksumTracker(NetService, State);
		ChecksumTracker.IsEnabled = !TestMode.IsOn;
		RunLocationTargetedBuffer = new RunLocationTargetedMessageBuffer(NetService);
		FlavorSynchronizer = new FlavorSynchronizer(NetService, State, netId);
		ActionQueueSet = new ActionQueueSet(State.Players);
		ActionExecutor = new ActionExecutor(ActionQueueSet);
		ActionQueueSynchronizer = new ActionQueueSynchronizer(State, ActionQueueSet, RunLocationTargetedBuffer, NetService);
		CardSelectorStack = new CardSelectorStack(State);
		PlayerChoiceSynchronizer = new PlayerChoiceSynchronizer(NetService, State);
		MapSelectionSynchronizer = new MapSelectionSynchronizer(NetService, ActionQueueSynchronizer, State);
		ActChangeSynchronizer = new ActChangeSynchronizer(State);
		EventSynchronizer = new EventSynchronizer(RunLocationTargetedBuffer, NetService, State, State, netId, State.Rng.Seed);
		RewardSynchronizer = new RewardSynchronizer(RunLocationTargetedBuffer, NetService, State, netId);
		RewardsSetSynchronizer = new RewardsSetSynchronizer(RunLocationTargetedBuffer, NetService, State, netId);
		MerchantSynchronizer = new MerchantSynchronizer(NetService, State, RunLocationTargetedBuffer);
		OneOffSynchronizer = new OneOffSynchronizer(RunLocationTargetedBuffer, NetService, State, netId);
		TreasureRoomRelicSynchronizer = new TreasureRoomRelicSynchronizer(State, netId, ActionQueueSynchronizer, State.SharedRelicGrabBag, State.Rng.TreasureRoomRelics);
		CombatReplayWriter = new CombatReplayWriter(PlayerChoiceSynchronizer, RewardsSetSynchronizer, ActionQueueSet, ActionQueueSynchronizer, ChecksumTracker);
		CombatReplayWriter.IsEnabled = !TestMode.IsOn;
		ActionExecutor.JustBeforeActionFinishedExecuting += SendPostActionChecksum;
		ChecksumTracker.StateDiverged += StateDiverged;
		ActionExecutor.Pause();
		IsAbandoned = false;
		HasRunEnded = false;
		AscensionManager = new AscensionManager(State.GameMode, State.AscensionLevel);
		ShouldSave = shouldSave;
		DailyTime = dailyTime;
		_startTime = startTime;
		_prevSessionRunTime = runTime;
		_activeRunTime = 0L;
		WinTime = winTime;
		_numReloads = numReloads;
		_startOfCurrentActiveRunTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
		InputSynchronizer = inputSynchronizer;
		HoveredModelTracker = new HoveredModelTracker(InputSynchronizer, State);
	}

	private void InitializeRunLobby(INetGameService netService, RunState state, IEnumerable<RunLobbyPlayer> players)
	{
		if (netService.Type.IsMultiplayer())
		{
			RunLobby = new RunLobby(state.GameMode, netService, this, state, players);
			RunLobby.RemotePlayerDisconnected += RemotePlayerDisconnected;
		}
		CombatStateSynchronizer = new CombatStateSynchronizer(NetService, RunLobby, state);
		RestSiteSynchronizer = new RestSiteSynchronizer(RunLocationTargetedBuffer, NetService, state, NetService.NetId, RunLobby);
	}

	private void InitializeNewRun()
	{
		State.SharedRelicGrabBag.Populate(ModelDb.RelicPool<SharedRelicPool>().GetUnlockedRelics(State.UnlockState), State.Rng.UpFront);
		foreach (Player player in State.Players)
		{
			player.PopulateRelicGrabBagIfNecessary(State.Rng.UpFront);
		}
		SetStartedWithNeowFlag();
		foreach (ModifierModel modifier in State.Modifiers)
		{
			modifier.OnRunCreated(State);
		}
		foreach (RunModel runModel in State.RunModels)
		{
			runModel.OnRunCreated(State);
		}
		foreach (Player player2 in State.Players)
		{
			ApplyAscensionEffects(player2);
		}
	}

	private void InitializeSavedRun(SerializableRun save)
	{
		foreach (ActModel act in State.Acts)
		{
			act.ValidateRoomsAfterLoad(State.Rng.UpFront);
		}
		AfterMapLocationChanged();
		MapDrawingsToLoad = save.MapDrawings;
		SavedMapsToLoad = null;
		for (int i = 0; i < save.Acts.Count; i++)
		{
			SerializableActMap savedMap = save.Acts[i].SavedMap;
			if (savedMap != null)
			{
				if (SavedMapsToLoad == null)
				{
					Dictionary<int, SerializableActMap> dictionary = (SavedMapsToLoad = new Dictionary<int, SerializableActMap>());
				}
				SavedMapsToLoad[i] = savedMap;
			}
		}
		foreach (ModifierModel modifier in State.Modifiers)
		{
			modifier.OnRunLoaded(State);
		}
		foreach (RunModel runModel in State.RunModels)
		{
			runModel.OnRunLoaded(State);
		}
	}

	private void SendPostActionChecksum(GameAction action)
	{
		if (CombatManager.Instance.IsInProgress && ((!(action is EndPlayerTurnAction) && !(action is ReadyToBeginEnemyTurnAction)) || 1 == 0))
		{
			ChecksumTracker.GenerateChecksum($"finished action execution {action}", action);
		}
	}

	private void SetStartedWithNeowFlag()
	{
		State.ExtraFields.StartedWithNeow = State.UnlockState.IsEpochRevealed<NeowEpoch>();
	}

	public static SerializableRun CanonicalizeSave(SerializableRun save, ulong localPlayerId)
	{
		if (save.Players.FirstOrDefault((SerializablePlayer p) => p.NetId == localPlayerId) == null)
		{
			throw new InvalidOperationException($"Save is invalid! Players does not contain local player Id. IDs in save file: {string.Join(",", save.Players.Select((SerializablePlayer p) => p.NetId))}. Local ID: {localPlayerId}.");
		}
		RunState runState = RunState.FromSerializable(save);
		int latestSchemaVersion = SaveManager.Instance.GetLatestSchemaVersion<SerializableRun>();
		SerializableRun serializableRun = new SerializableRun
		{
			SchemaVersion = latestSchemaVersion,
			Acts = runState.Acts.Zip(save.Acts, (ActModel act, SerializableActModel savedAct) =>
			{
				SerializableActModel serializableActModel = act.ToSave();
				serializableActModel.SavedMap = savedAct.SavedMap;
				return serializableActModel;
			}).ToList(),
			Modifiers = runState.Modifiers.Select((ModifierModel m) => m.ToSerializable()).ToList(),
			DailyTime = save.DailyTime,
			GameMode = runState.GameMode,
			CurrentActIndex = runState.CurrentActIndex,
			EventsSeen = runState.VisitedEventIds.ToList(),
			SerializableOdds = runState.Odds.ToSerializable(),
			SerializableSharedRelicGrabBag = runState.SharedRelicGrabBag.ToSerializable(),
			Players = runState.Players.Select((Player p) => p.ToSerializable()).ToList(),
			SerializableRng = runState.Rng.ToSerializable(),
			VisitedMapCoords = runState.VisitedMapCoords.ToList(),
			MapPointHistory = runState.MapPointHistory.Select((IReadOnlyList<MapPointHistoryEntry> l) => l.ToList()).ToList(),
			SaveTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
			StartTime = save.StartTime,
			RunTime = save.RunTime,
			WinTime = save.WinTime,
			Ascension = runState.AscensionLevel,
			PlatformType = save.PlatformType,
			MapDrawings = save.MapDrawings,
			NumReloads = save.NumReloads,
			ExtraFields = runState.ExtraFields.ToSerializable(),
			PreFinishedRoom = save.PreFinishedRoom
		};
		PacketWriter packetWriter = new PacketWriter();
		packetWriter.Write(serializableRun);
		return serializableRun;
	}

	public static HashSet<RoomType> BuildRoomTypeBlacklist(MapPointHistoryEntry? previousMapPointEntry, IReadOnlyCollection<MapPoint> nextMapPoints)
	{
		HashSet<RoomType> hashSet = new HashSet<RoomType>();
		if ((previousMapPointEntry != null && previousMapPointEntry.HasRoomOfType(RoomType.Shop)) || (nextMapPoints.Count > 0 && nextMapPoints.All((MapPoint p) => p.PointType == MapPointType.Shop)))
		{
			hashSet.Add(RoomType.Shop);
		}
		return hashSet;
	}

	public SerializableRun ToSave(AbstractRoom? preFinishedRoom)
	{
		int latestSchemaVersion = SaveManager.Instance.GetLatestSchemaVersion<SerializableRun>();
		List<SerializableActModel> list = new List<SerializableActModel>();
		for (int i = 0; i < State.Acts.Count; i++)
		{
			SerializableActModel serializableActModel = State.Acts[i].ToSave();
			if (i == State.CurrentActIndex && State.Map != null)
			{
				serializableActModel.SavedMap = SerializableActMap.FromActMap(State.Map);
			}
			list.Add(serializableActModel);
		}
		return new SerializableRun
		{
			SchemaVersion = latestSchemaVersion,
			Acts = list,
			Modifiers = State.Modifiers.Select((ModifierModel m) => m.ToSerializable()).ToList(),
			DailyTime = DailyTime,
			CurrentActIndex = State.CurrentActIndex,
			EventsSeen = State.VisitedEventIds.ToList(),
			GameMode = State.GameMode,
			IsCustomGame = State.IsCustomGame,
			SerializableOdds = State.Odds.ToSerializable(),
			SerializableSharedRelicGrabBag = State.SharedRelicGrabBag.ToSerializable(),
			Players = State.Players.Select((Player p) => p.ToSerializable()).ToList(),
			SerializableRng = State.Rng.ToSerializable(),
			VisitedMapCoords = State.VisitedMapCoords.ToList(),
			MapPointHistory = State.MapPointHistory.Select((IReadOnlyList<MapPointHistoryEntry> l) => l.ToList()).ToList(),
			SaveTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
			StartTime = _startTime,
			RunTime = RunTime,
			WinTime = WinTime,
			NumReloads = _numReloads,
			Ascension = State.AscensionLevel,
			PlatformType = NetService.Platform,
			MapDrawings = NRun.Instance?.GlobalUi.MapScreen.Drawings.GetSerializableMapDrawings(),
			ExtraFields = State.ExtraFields.ToSerializable(),
			PreFinishedRoom = preFinishedRoom?.ToSerializable()
		};
	}

	public RunState Launch()
	{
		LocalContext.NetId = NetService.NetId;
		NetService.SetBufferMessages(bufferMessages: false);
		RunStarted?.Invoke(State);
		UpdateRichPresence();
		return State;
	}

	public async Task FinalizeStartingRelics()
	{
		if (State == null)
		{
			return;
		}
		foreach (Player player in State.Players)
		{
			foreach (RelicModel relic in player.Relics)
			{
				await relic.AfterObtained();
			}
		}
	}

	public void GenerateRooms()
	{
		List<AncientEventModel> list = State.UnlockState.SharedAncients.ToList().UnstableShuffle(State.Rng.UpFront);
		foreach (ActModel item in State.Acts.Skip(1))
		{
			int count = State.Rng.UpFront.NextInt(list.Count + 1);
			List<AncientEventModel> list2 = list.Take(count).ToList();
			list = list.Except(list2).ToList();
			item.SetSharedAncientSubset(list2);
		}
		for (int i = 0; i < State.Acts.Count; i++)
		{
			ActModel act = State.Acts[i];
			act.GenerateRooms(State);
			if (ShouldApplyTutorialModifications())
			{
				act.ApplyDiscoveryOrderModifications(State.UnlockState);
			}
			if (i == State.Acts.Count - 1 && AscensionManager.HasLevel(AscensionLevel.DoubleBoss))
			{
				EncounterModel secondBossEncounter = State.Rng.UpFront.NextItem(act.AllBossEncounters.Where((EncounterModel e) => e.Id != act.BossEncounter.Id));
				act.SetSecondBossEncounter(secondBossEncounter);
			}
		}
	}

	public bool ShouldApplyTutorialModifications()
	{
		if (ForceDiscoveryOrderModifications)
		{
			return true;
		}
		if (TestMode.IsOn)
		{
			return false;
		}
		if (State == null)
		{
			return false;
		}
		if (State.GameMode != GameMode.Standard || State.IsCustomGame)
		{
			return false;
		}
		return true;
	}

	public async Task GenerateMap()
	{
		if (State == null)
		{
			throw new InvalidOperationException("State is not set.");
		}
		MapSelectionSynchronizer.BeforeMapGenerated();
		ActMap map;
		if (SavedMapsToLoad != null && SavedMapsToLoad.TryGetValue(State.CurrentActIndex, out SerializableActMap value))
		{
			map = ((State.GameMode != GameMode.Blitz) ? ((ActMap)new SavedActMap(value)) : ((ActMap)new BlitzActMap(value, State)));
			SavedMapsToLoad.Remove(State.CurrentActIndex);
			if (SavedMapsToLoad.Count == 0)
			{
				SavedMapsToLoad = null;
			}
			map = Hook.ModifyGeneratedMapLate(State, map, State.CurrentActIndex);
			await Hook.AfterMapGenerated(State, map, State.CurrentActIndex);
		}
		else if (State.GameMode == GameMode.Blitz)
		{
			BlitzActMap map2 = new BlitzActMap(State);
			map = Hook.ModifyGeneratedMap(State, map2, State.CurrentActIndex);
			await Hook.AfterMapGenerated(State, map, State.CurrentActIndex);
		}
		else
		{
			ActMap map3 = State.Act.CreateMap(State, replaceTreasureWithElites: false);
			map = Hook.ModifyGeneratedMap(State, map3, State.CurrentActIndex);
			await Hook.AfterMapGenerated(State, map, State.CurrentActIndex);
			if (!State.ExtraFields.StartedWithNeow && State.CurrentActIndex == 0)
			{
				map.StartingMapPoint.PointType = MapPointType.Monster;
			}
		}
		State.Map = map;
		State.RemoveStaleVisitedMapCoords(map);
		InvokeMapChanged(clearDrawings: true);
	}

	public void InvokeMapChanged(bool clearDrawings)
	{
		MapChanged?.Invoke(clearDrawings);
	}

	public Task EnterMapCoord(MapCoord coord)
	{
		if (State == null)
		{
			return Task.CompletedTask;
		}
		if (!State.AddVisitedMapCoord(coord))
		{
			return Task.CompletedTask;
		}
		return EnterMapCoordInternal(coord, null, saveGame: true);
	}

	public async Task LoadIntoLatestMapCoord(AbstractRoom? preFinishedRoom)
	{
		if (State != null)
		{
			if (State.VisitedMapCoords.Count > 0)
			{
				RunManager runManager = this;
				IReadOnlyList<MapCoord> visitedMapCoords = State.VisitedMapCoords;
				await runManager.EnterMapCoordInternal(visitedMapCoords[visitedMapCoords.Count - 1], preFinishedRoom, saveGame: false);
			}
			else
			{
				await EnterRoomInternal(new MapRoom());
			}
		}
	}

	private Task EnterMapCoordInternal(MapCoord coord, AbstractRoom? preFinishedRoom, bool saveGame)
	{
		if (State == null)
		{
			return Task.CompletedTask;
		}
		MapPoint point = State.Map.GetPoint(coord);
		return EnterMapPointInternal(coord.row + 1, point.PointType, preFinishedRoom, saveGame);
	}

	public async Task EnterMapPointInternal(int actFloor, MapPointType pointType, AbstractRoom? preFinishedRoom, bool saveGame)
	{
		if (State == null)
		{
			return;
		}
		SetIsChangingRooms(isChangingRooms: true);
		try
		{
			using (new NetLoadingHandle(NetService))
			{
				if (State.MapPointHistory.Count > 0)
				{
					UpdatePlayerStatsInMapPointHistory();
				}
				State.ActFloor = actFloor;
				await ExitCurrentRooms();
				if (preFinishedRoom == null)
				{
					CombatStateSynchronizer.StartSync();
				}
				ClearScreens();
				if (preFinishedRoom == null)
				{
					await CombatStateSynchronizer.WaitForSync();
				}
				if (saveGame)
				{
					await SaveManager.Instance.SaveRun(null);
				}
				if (CombatReplayWriter.IsEnabled)
				{
					CombatReplayWriter.RecordInitialState(ToSave(null));
				}
				RoomType roomType;
				if (pointType == MapPointType.Unknown && preFinishedRoom != null)
				{
					roomType = RoomType.Monster;
				}
				else
				{
					HashSet<RoomType> blacklist = BuildRoomTypeBlacklist(State.CurrentMapPointHistoryEntry, State.CurrentMapPoint?.Children ?? new HashSet<MapPoint>());
					roomType = RollRoomTypeFor(pointType, blacklist);
				}
				AbstractRoom abstractRoom = ((preFinishedRoom == null) ? CreateRoom(roomType, pointType) : preFinishedRoom);
				ActionExecutor.Pause();
				if (preFinishedRoom == null)
				{
					State.AppendToMapPointHistory(pointType, abstractRoom.RoomType, abstractRoom.ModelId);
				}
				if (abstractRoom is CombatRoom { IsPreFinished: not false, ParentEventId: not null } combatRoom)
				{
					EventRoom room = new EventRoom(ModelDb.GetById<EventModel>(combatRoom.ParentEventId));
					await EnterRoomInternal(room, isRestoringRoomStackBase: true);
					await EnterRoomInternal(combatRoom);
				}
				else
				{
					await EnterRoom(abstractRoom);
				}
				if (NRun.Instance != null)
				{
					NRun.Instance.GlobalUi.MapScreen.IsTraveling = false;
				}
				AfterMapLocationChanged();
			}
		}
		finally
		{
			SetIsChangingRooms(isChangingRooms: false);
		}
	}

	private void SetIsChangingRooms(bool isChangingRooms)
	{
		if (IsChangingRooms != isChangingRooms)
		{
			IsChangingRooms = isChangingRooms;
			OnIsChangingRoomsModified?.Invoke();
		}
	}

	private AbstractRoom CreateRoom(RoomType roomType, MapPointType mapPointType = MapPointType.Unassigned, AbstractModel? model = null)
	{
		if (State == null)
		{
			throw new InvalidOperationException("RunState is not set.");
		}
		switch (roomType)
		{
		case RoomType.Monster:
		case RoomType.Elite:
		case RoomType.Boss:
			return new CombatRoom((model as EncounterModel) ?? State.Act.PullNextEncounter(roomType).ToMutable(), State);
		case RoomType.Treasure:
			return new TreasureRoom(State.CurrentActIndex);
		case RoomType.Shop:
			return new MerchantRoom();
		case RoomType.Event:
			return new EventRoom((model as EventModel) ?? ((mapPointType == MapPointType.Ancient) ? State.Act.PullAncient() : State.Act.PullNextEvent(State)));
		case RoomType.RestSite:
			return new RestSiteRoom();
		case RoomType.Map:
			return new MapRoom();
		default:
			throw new InvalidOperationException($"Unexpected RoomType: {roomType}");
		}
	}

	private RoomType RollRoomTypeFor(MapPointType pointType, IEnumerable<RoomType> blacklist)
	{
		if (TryGetRoomTypeForTutorial(pointType, out var roomType))
		{
			return roomType;
		}
		return pointType switch
		{
			MapPointType.Unassigned => RoomType.Unassigned,
			MapPointType.Unknown => State.Odds.UnknownMapPoint.Roll(blacklist, State),
			MapPointType.Shop => RoomType.Shop,
			MapPointType.Treasure => RoomType.Treasure,
			MapPointType.RestSite => RoomType.RestSite,
			MapPointType.Monster => RoomType.Monster,
			MapPointType.Elite => RoomType.Elite,
			MapPointType.Boss => RoomType.Boss,
			MapPointType.Ancient => RoomType.Event,
			_ => throw new ArgumentOutOfRangeException("pointType", pointType, null),
		};
	}

	private bool TryGetRoomTypeForTutorial(MapPointType pointType, out RoomType roomType)
	{
		roomType = RoomType.Unassigned;
		if (!TestMode.IsOn)
		{
			RunState? state = State;
			if (state == null || state.Players.Count <= 1)
			{
				if (pointType != MapPointType.Unassigned)
				{
					return false;
				}
				if (SaveManager.Instance.Progress.NumberOfRuns > 0)
				{
					return false;
				}
				RunState? state2 = State;
				if (state2 != null && state2.MapPointHistory.SelectMany((IReadOnlyList<MapPointHistoryEntry> l) => l).Any((MapPointHistoryEntry e) => e.MapPointType == MapPointType.Unassigned))
				{
					return false;
				}
				roomType = RoomType.Event;
				return true;
			}
		}
		return false;
	}

	public async Task FadeIn(bool showTransition = true)
	{
		if (TestMode.IsOn)
		{
			Task task = TestFadeIn?.Invoke();
			if (task != null)
			{
				await task;
			}
		}
		else
		{
			await NGame.Instance.Transition.RoomFadeIn(showTransition);
		}
	}

	public async Task FadeOut()
	{
		if (TestMode.IsOn)
		{
			Task task = TestFadeOut?.Invoke();
			if (task != null)
			{
				await task;
			}
		}
		else
		{
			await NGame.Instance.Transition.RoomFadeOut();
		}
	}

	private void ClearScreens()
	{
		if (!TestMode.IsOn)
		{
			NOverlayStack.Instance.Clear();
			NCapstoneContainer.Instance.Close();
			NMapScreen.Instance.Close(animateOut: false);
		}
	}

	public async Task EnterMapCoordDebug(MapCoord coord, RoomType roomType, MapPointType pointType = MapPointType.Unassigned, AbstractModel? model = null, bool showTransition = true)
	{
		State.AddVisitedMapCoord(coord);
		await EnterRoomDebug(roomType, pointType, model, showTransition);
	}

	public async Task<AbstractRoom> EnterRoomDebug(RoomType roomType, MapPointType pointType = MapPointType.Unassigned, AbstractModel? model = null, bool showTransition = true)
	{
		SetIsChangingRooms(isChangingRooms: true);
		try
		{
			using (new NetLoadingHandle(NetService))
			{
				ClearScreens();
				await ExitCurrentRooms();
				CombatStateSynchronizer.StartSync();
				if (model is EncounterModel encounterModel)
				{
					roomType = encounterModel.RoomType;
				}
				else if (model is EventModel)
				{
					roomType = RoomType.Event;
				}
				if (pointType == MapPointType.Unassigned)
				{
					pointType = roomType switch
					{
						RoomType.Monster => MapPointType.Monster,
						RoomType.Elite => MapPointType.Elite,
						RoomType.Boss => MapPointType.Boss,
						RoomType.Treasure => MapPointType.Treasure,
						RoomType.Shop => MapPointType.Shop,
						RoomType.Event => MapPointType.Unknown,
						RoomType.RestSite => MapPointType.RestSite,
						RoomType.Unassigned => MapPointType.Unassigned,
						RoomType.Map => MapPointType.Unassigned,
					};
				}
				if (CombatReplayWriter.IsEnabled)
				{
					CombatReplayWriter.RecordInitialState(ToSave(null));
				}
				State.AppendToMapPointHistory(pointType, roomType, model?.Id);
				NRun.Instance?.GlobalUi.TopBar.RoomIcon.DebugSetMapPointTypeOverride(pointType);
				if (State.Map is MockSinglePointActMap mockSinglePointActMap)
				{
					mockSinglePointActMap.MockCurrentMapPointType(pointType);
				}
				await CombatStateSynchronizer.WaitForSync();
				AbstractRoom room = CreateRoom(roomType, MapPointType.Unassigned, model);
				await EnterRoom(room);
				await FadeIn(showTransition);
				return room;
			}
		}
		finally
		{
			SetIsChangingRooms(isChangingRooms: false);
		}
	}

	private async Task ExitCurrentRooms()
	{
		if (State != null)
		{
			while (State.CurrentRoomCount > 0)
			{
				await ExitCurrentRoom();
			}
			NRun.Instance?.GlobalUi.TopBar.RoomIcon.DebugClearMapPointTypeOverride();
		}
	}

	private async Task<AbstractRoom?> ExitCurrentRoom()
	{
		if (State == null)
		{
			return null;
		}
		RewardsSetSynchronizer.BeforeLeavingRoom();
		AbstractRoom currentRoom = State.PopCurrentRoom();
		await currentRoom.Exit(State);
		RoomExited?.Invoke();
		return currentRoom;
	}

	private async Task EnterRoomInternal(AbstractRoom room, bool isRestoringRoomStackBase = false)
	{
		if (State == null)
		{
			return;
		}
		bool flag = isRestoringRoomStackBase;
		bool flag2 = flag;
		bool flag3;
		if (!flag2)
		{
			if (room is CombatRoom combatRoom)
			{
				if (combatRoom.IsPreFinished)
				{
					goto IL_0072;
				}
			}
			else if (room is EventRoom { IsPreFinished: not false })
			{
				goto IL_0072;
			}
			flag3 = false;
			goto IL_007a;
		}
		goto IL_007d;
		IL_007d:
		bool runExternalEffects = !flag2;
		State.PushRoom(room);
		if (runExternalEffects && !(room is MapRoom))
		{
			await Hook.BeforeRoomEntered(State, room);
		}
		await room.Enter(State, isRestoringRoomStackBase);
		if (runExternalEffects)
		{
			NRunMusicController.Instance?.UpdateTrack();
			if (State.CurrentRoomCount == 1)
			{
				State.Act.MarkRoomVisited(room.RoomType);
			}
		}
		RunLocationTargetedBuffer.OnLocationChanged(State.RunLocation);
		if (!(room is CombatRoom))
		{
			ActionExecutor.Unpause();
		}
		NRunMusicController.Instance?.UpdateAmbience();
		RoomEntered?.Invoke();
		return;
		IL_007a:
		flag2 = flag3;
		goto IL_007d;
		IL_0072:
		flag3 = true;
		goto IL_007a;
	}

	public async Task EnterRoom(AbstractRoom room)
	{
		await ExitCurrentRooms();
		await EnterRoomInternal(room);
	}

	public async Task EnterRoomWithoutExitingCurrentRoom(AbstractRoom room, bool fadeToBlack)
	{
		if (State == null)
		{
			return;
		}
		SetIsChangingRooms(isChangingRooms: true);
		try
		{
			ActionExecutor.Pause();
			CombatStateSynchronizer.StartSync();
			using (new NetLoadingHandle(NetService))
			{
				if (fadeToBlack)
				{
					await FadeOut();
					ClearScreens();
				}
				await CombatStateSynchronizer.WaitForSync();
				State.CurrentMapPointHistoryEntry?.Rooms.Add(new MapPointRoomHistoryEntry
				{
					RoomType = room.RoomType,
					ModelId = room.ModelId
				});
				await EnterRoomInternal(room);
				ActiveScreenContext.Instance.Update();
				if (fadeToBlack)
				{
					await FadeIn();
				}
			}
		}
		finally
		{
			SetIsChangingRooms(isChangingRooms: false);
		}
	}

	public async Task EnterNextAct()
	{
		if (State == null)
		{
			return;
		}
		using (new NetLoadingHandle(NetService))
		{
			if (State.CurrentActIndex >= State.Acts.Count - 1)
			{
				AbstractRoom currentRoom = State.CurrentRoom;
				if (currentRoom != null && currentRoom.IsVictoryRoom)
				{
					throw new InvalidOperationException("EnterNextAct called at victory room! WinRun should be called instead");
				}
				await FadeOut();
				ClearScreens();
				await EnterRoom(new EventRoom(ModelDb.Event<TheArchitect>()));
				await FadeIn();
			}
			else
			{
				await EnterAct(State.CurrentActIndex + 1);
			}
		}
	}

	public async Task WinRun()
	{
		if (State != null)
		{
			PostRunProgressUpdateResult result = OnEnded(isVictory: true);
			await GuaranteeKillAllPlayers();
			NRun.Instance?.ShowGameOverScreen(result);
		}
	}

	public async Task EnterAct(int currentActIndex, bool doTransition = true)
	{
		if (State == null)
		{
			return;
		}
		await FadeOut();
		using (new NetLoadingHandle(NetService))
		{
			ClearScreens();
			await ExitCurrentRooms();
			await SetActInternal(currentActIndex);
			if ((currentActIndex == 0 && State.ExtraFields.StartedWithNeow) || State.GameMode == GameMode.Blitz)
			{
				if (NRun.Instance != null)
				{
					NMapScreen.Instance?.InitMarker(State.Map.StartingMapPoint.coord);
				}
				await EnterMapCoord(State.Map.StartingMapPoint.coord);
				NMapScreen.Instance?.RefreshAllMapPointVotes();
			}
			else
			{
				await EnterRoomInternal(new MapRoom());
				ActEntered?.Invoke();
			}
			await FadeIn(doTransition);
			await Hook.AfterActEntered(State);
		}
	}

	public async Task SetActInternal(int actIndex)
	{
		if (State != null)
		{
			State.CurrentActIndex = actIndex;
			State.ClearVisitedMapCoordsDebug();
			State.Odds.UnknownMapPoint.ResetToBase();
			AfterMapLocationChanged();
			await PreloadManager.LoadActAssets(State.Act);
			await GenerateMap();
			NMapScreen.Instance?.SetTravelEnabled(enabled: false);
			NRunMusicController.Instance?.UpdateMusic();
			UpdateRichPresence();
		}
	}

	private void UpdateRichPresence()
	{
		if (TestMode.IsOn || State == null)
		{
			return;
		}
		Player me = LocalContext.GetMe(State);
		int dreadStreak = AscensionHelper.GetDreadStreak(State);
		PlatformUtil.SetRichPresence(RunRichPresence.GetToken(State.GameMode), NetService.GetRawLobbyIdentifier(), State.Players.Count);
		foreach (var (key, value) in RunRichPresence.GetValues(me.Character.Id.Entry, State.Act.Id.Entry, State.AscensionLevel, dreadStreak))
		{
			PlatformUtil.SetRichPresenceValue(key, value);
		}
	}

	public void ProceedFromTerminalRewardsScreen()
	{
		TaskHelper.RunSafely(ProceedFromTerminalRewardsScreenAsync());
	}

	public async Task ProceedFromTerminalRewardsScreenAsync()
	{
		if (State == null)
		{
			return;
		}
		if (State.CurrentRoomCount > 1)
		{
			if (State.CurrentRoom is CombatRoom { ShouldResumeParentEventAfterCombat: not false })
			{
				await ResumePreviousRoom();
			}
			else
			{
				Proceed();
			}
		}
		else
		{
			Proceed();
		}
	}

	public void Proceed()
	{
		if (!TryAdvanceToNextBlitzNode())
		{
			NMapScreen.Instance?.SetTravelEnabled(enabled: true);
			NMapScreen.Instance?.Open();
		}
	}

	private bool TryAdvanceToNextBlitzNode()
	{
		if (State == null)
		{
			return false;
		}
		if (State.GameMode != GameMode.Blitz)
		{
			return false;
		}
		if (!(State.Map is BlitzActMap blitzActMap))
		{
			throw new InvalidOperationException("Game mode is Blitz, but act map is not Blitz map!");
		}
		if (IsChangingRooms)
		{
			return true;
		}
		Player me = LocalContext.GetMe(State);
		List<MapPoint> mapPoints = blitzActMap.GetMapPoints();
		int num = ((State.CurrentMapPoint != null) ? (mapPoints.IndexOf(State.CurrentMapPoint) + 1) : 0);
		if (num >= mapPoints.Count)
		{
			ActChangeSynchronizer.SetLocalPlayerReady();
		}
		else if (MapSelectionSynchronizer.GetVote(me).HasValue)
		{
			if (State.Players.Count > 1)
			{
				FlavorSynchronizer.SendMapPing(mapPoints[num].coord);
			}
		}
		else
		{
			MapSelectionSynchronizer.OnLocalPlayerVotedFor(mapPoints[num].coord);
		}
		return true;
	}

	private async Task ResumePreviousRoom()
	{
		if (State != null)
		{
			ClearScreens();
			AbstractRoom abstractRoom = await ExitCurrentRoom();
			if (abstractRoom != null)
			{
				await State.CurrentRoom.Resume(abstractRoom, State);
				NRunMusicController.Instance?.UpdateTrack();
				await FadeIn();
			}
			else
			{
				Log.Error("Current room returned null while exiting.");
			}
		}
	}

	private void AfterMapLocationChanged()
	{
		MapSelectionSynchronizer.OnLocationChanged(State.MapLocation);
		RunLocationTargetedBuffer.OnLocationChanged(State.RunLocation);
	}

	public void Abandon()
	{
		Log.Info("Abandoning an in-progress run (player-initiated)");
		if (NetService.Type == NetGameType.Singleplayer)
		{
			TaskHelper.RunSafely(AbandonInternal());
		}
		else
		{
			RunLobby.AbandonRun();
		}
	}

	void IRunLobbyListener.RunAbandoned()
	{
		Log.Info("The host told us to abandon the run");
		NMapScreen.Instance?.Close(animateOut: false);
		NCapstoneContainer.Instance?.Close();
		TaskHelper.RunSafely(AbandonInternal());
	}

	private async Task AbandonInternal()
	{
		try
		{
			NCapstoneContainer.Instance.Close();
			NMapScreen.Instance.Close(animateOut: false);
		}
		catch (Exception value)
		{
			Log.Error($"Exception thrown while trying to abandon run: {value}");
		}
		IsAbandoned = true;
		await GuaranteeKillAllPlayers();
		if (NetService.Type == NetGameType.Client)
		{
			NetErrorInfo info = new NetErrorInfo(NetError.HostAbandoned, selfInitiated: false);
			info.SetModdedFlagIfModded(RunLobby);
			NErrorPopup nErrorPopup = NErrorPopup.Create(info);
			if (nErrorPopup != null)
			{
				NModalContainer.Instance.Add(nErrorPopup);
			}
		}
	}

	private async Task GuaranteeKillAllPlayers()
	{
		if (State == null)
		{
			return;
		}
		foreach (Player player in State.Players)
		{
			await CreatureCmd.Kill(player.Creature, force: true);
			await Cmd.CustomScaledWait(0.25f, 0.5f);
		}
	}

	private void StateDiverged(ulong divergedFrom, NetFullCombatState state)
	{
		if (NetService.Type == NetGameType.Replay)
		{
			return;
		}
		if (NetService.Type == NetGameType.Client)
		{
			Log.Info("Abandoning run and returning to main menu because our state diverged from host's");
			WriteReplay(stopRecording: false);
			return;
		}
		NetErrorInfo info = new NetErrorInfo(NetError.StateDivergence, selfInitiated: true);
		info.SetModdedFlagIfModded(RunLobby);
		NErrorPopup nErrorPopup = NErrorPopup.Create(info);
		if (nErrorPopup != null)
		{
			NModalContainer.Instance?.Add(nErrorPopup);
		}
	}

	public void WriteReplay(bool stopRecording)
	{
		string profileScopedPath = SaveManager.Instance.GetProfileScopedPath("replays/latest.mcr");
		CombatReplayWriter.WriteReplay(profileScopedPath, stopRecording);
	}

	public void CleanUp(bool graceful = true, bool disconnect = true)
	{
		if (State == null)
		{
			return;
		}
		ShouldSave = false;
		IsCleaningUp = true;
		try
		{
			_runHistoryWasUploaded = false;
			ActionQueueSet.Reset();
			NAudioManager.Instance?.StopAllLoops();
			NOverlayStack.Instance?.Clear();
			NCapstoneContainer.Instance?.CleanUp();
			NMapScreen.Instance?.CleanUp();
			NModalContainer.Instance?.Clear();
			CombatManager.Instance.Reset(graceful);
			NDebugAudioManager.Instance?.StopAll();
			if (CombatReplayWriter.IsRecordingReplay)
			{
				WriteReplay(stopRecording: true);
			}
			ActionExecutor.JustBeforeActionFinishedExecuting -= SendPostActionChecksum;
			CombatReplayWriter.Dispose();
			ActionQueueSynchronizer.Dispose();
			PlayerChoiceSynchronizer.Dispose();
			EventSynchronizer?.Dispose();
			RewardSynchronizer.Dispose();
			RewardsSetSynchronizer.Dispose();
			RestSiteSynchronizer.Dispose();
			MerchantSynchronizer?.Dispose();
			OneOffSynchronizer?.Dispose();
			FlavorSynchronizer.Dispose();
			ChecksumTracker.Dispose();
			if (RunLobby != null)
			{
				RunLobby.RemotePlayerDisconnected -= RemotePlayerDisconnected;
				RunLobby.Dispose();
			}
			if (disconnect)
			{
				NetService.Disconnect(NetError.Quit, !graceful);
			}
		}
		finally
		{
			NHotkeyManager.Instance?.ClearHotkeys();
			IsCleaningUp = false;
			LocalContext.NetId = null;
			State = null;
			SetIsChangingRooms(isChangingRooms: false);
			DailyTime = null;
		}
	}

	public PostRunProgressUpdateResult OnEnded(bool isVictory)
	{
		if (HasRunEnded)
		{
			Log.Error("OnEnded called twice! This should not occur. Ignoring second end.");
			return default;
		}
		HasRunEnded = true;
		UpdatePlayerStatsInMapPointHistory();
		RunState state = State;
		Player me = LocalContext.GetMe(state);
		if (state.CurrentRoom is CombatRoom combatRoom)
		{
			MapPointRoomHistoryEntry mapPointRoomHistoryEntry = state.CurrentMapPointHistoryEntry.Rooms.Last();
			mapPointRoomHistoryEntry.TurnsTaken = me.PlayerCombatState?.TurnNumber ?? combatRoom.CombatState.RoundNumber;
		}
		SerializableRun serializableRun = ToSave(null);
		SerializablePlayer me2 = LocalContext.GetMe(serializableRun);
		if (_runHistoryWasUploaded)
		{
			return PostRunProgressUpdateResult.Unsuccessful(serializableRun);
		}
		_runHistoryWasUploaded = true;
		if (!isVictory && state.CurrentRoom is CombatRoom combatRoom2)
		{
			foreach (var monstersWithSlot in combatRoom2.Encounter.MonstersWithSlots)
			{
				MonsterModel item = monstersWithSlot.Item1;
				CheckUpdateEnemyDiscoveryAfterLoss(me, item.Id);
			}
		}
		PostRunProgressUpdateResult? postRunProgressUpdateResult = null;
		if (ShouldSave)
		{
			using (SaveManager.Instance.BeginSaveBatch())
			{
				postRunProgressUpdateResult = SaveManager.Instance.UpdateProgressWithRunData(serializableRun, isVictory);
				foreach (string discoveredEpoch in me2.DiscoveredEpochs)
				{
					if (!me.DiscoveredEpochs.Contains(discoveredEpoch))
					{
						me.DiscoveredEpochs.Add(discoveredEpoch);
					}
				}
				AchievementsHelper.AfterRunEnded(state, me, isVictory);
				RunHistoryUtilities.CreateRunHistoryEntry(serializableRun, isVictory, IsAbandoned, NetService.Platform);
				MetricUtilities.UploadRunMetrics(serializableRun, isVictory, NetService.NetId);
				if (SaveManager.Instance.Progress.NumberOfRuns == 5)
				{
					MetricUtilities.UploadSettingsMetric();
				}
				if (NetService.Type == NetGameType.Singleplayer)
				{
					SaveManager.Instance.DeleteCurrentRun();
				}
				else if (NetService.Type == NetGameType.Host)
				{
					SaveManager.Instance.DeleteCurrentMultiplayerRun();
				}
			}
			if (isVictory)
			{
				int score = ScoreUtility.CalculateScore(serializableRun, isVictory);
				StatsManager.IncrementArchitectDamage(score);
			}
		}
		if (DailyTime.HasValue)
		{
			NetGameType type = NetService.Type;
			if ((uint)(type - 1) <= 1u)
			{
				int score2 = ScoreUtility.CalculateDailyScore(serializableRun, me.NetId, isVictory);
				TaskHelper.RunSafely(DailyRunUtility.UploadScore(DailyTime.Value, score2, serializableRun.Players));
			}
			else if (NetService.Type == NetGameType.Client)
			{
				TaskHelper.RunSafely(DailyRunUtility.UploadScore(DailyTime.Value, -999999999, serializableRun.Players));
			}
		}
		return postRunProgressUpdateResult ?? PostRunProgressUpdateResult.Unsuccessful(serializableRun);
	}

	private static void CheckUpdateEnemyDiscoveryAfterLoss(Player player, ModelId monster)
	{
		EnemyStats enemyStats = (SaveManager.Instance.Progress.EnemyStats.TryGetValue(monster, out EnemyStats value) ? value : null);
		if (enemyStats == null)
		{
			player.DiscoveredEnemies.Add(monster);
		}
	}

	private void UpdatePlayerStatsInMapPointHistory()
	{
		if (TestMode.IsOn || State == null)
		{
			return;
		}
		foreach (Player player in State.Players)
		{
			PlayerMapPointHistoryEntry playerMapPointHistoryEntry = State.CurrentMapPointHistoryEntry?.GetEntry(player.NetId);
			if (playerMapPointHistoryEntry != null)
			{
				playerMapPointHistoryEntry.CurrentGold = player.Gold;
				playerMapPointHistoryEntry.CurrentHp = player.Creature.CurrentHp;
				playerMapPointHistoryEntry.MaxHp = player.Creature.MaxHp;
			}
		}
	}

	public bool HasAscension(AscensionLevel level)
	{
		if (!IsInProgress)
		{
			return false;
		}
		return AscensionManager.HasLevel(level);
	}

	public bool IsDread()
	{
		if (!IsInProgress)
		{
			return false;
		}
		return State.GameMode == GameMode.Dread;
	}

	public void ApplyAscensionEffects(Player player)
	{
		AscensionManager.ApplyEffectsTo(player);
	}

	public ClientRejoinResponseMessage GetRejoinMessage()
	{
		return new ClientRejoinResponseMessage
		{
			serializableRun = ToSave(null),
			combatState = NetFullCombatState.FromRun(State, null)
		};
	}

	public void LocalPlayerDisconnected(NetErrorInfo info)
	{
		info.SetModdedFlagIfModded(RunLobby);
		foreach (Player player in State.Players)
		{
			if (!LocalContext.IsMe(player))
			{
				InputSynchronizer.OnPlayerDisconnected(player.NetId);
			}
		}
		if (info.GetReason() != NetError.QuitGameOver && !IsAbandoned && !State.IsGameOver)
		{
			TaskHelper.RunSafely(ReturnToMainMenuWithError(info));
		}
	}

	private void RemotePlayerDisconnected(ulong playerId)
	{
		InputSynchronizer.OnPlayerDisconnected(playerId);
	}

	private async Task ReturnToMainMenuWithError(NetErrorInfo info)
	{
		info.SetModdedFlagIfModded(RunLobby);
		NCapstoneContainer.Instance?.Close();
		NMapScreen.Instance?.Close(animateOut: false);
		if (TestMode.IsOff)
		{
			await NGame.Instance.ReturnToMainMenuAfterRun();
			NErrorPopup nErrorPopup = NErrorPopup.Create(info);
			if (nErrorPopup != null)
			{
				NModalContainer.Instance.Add(nErrorPopup);
			}
		}
	}

	public string? GetLocalCharacterEnergyIconPrefix()
	{
		CardPoolModel cardPoolModel = LocalContext.GetMe(State)?.Character.CardPool;
		if (cardPoolModel != null)
		{
			return EnergyIconHelper.GetPrefix(cardPoolModel);
		}
		return null;
	}

	public RunState? DebugOnlyGetState()
	{
		return State;
	}
}
