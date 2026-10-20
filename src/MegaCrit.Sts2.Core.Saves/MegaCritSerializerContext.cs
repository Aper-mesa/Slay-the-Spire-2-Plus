using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Godot;
using MegaCrit.Sts2.Core.Achievements;
using MegaCrit.Sts2.Core.ControllerInput;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Entities.Rngs;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Map;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Badges;
using MegaCrit.Sts2.Core.Nodes.Screens.FeedbackScreen;
using MegaCrit.Sts2.Core.Platform;
using MegaCrit.Sts2.Core.Platform.Null;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Runs.History;
using MegaCrit.Sts2.Core.Saves.MapDrawing;
using MegaCrit.Sts2.Core.Saves.Migrations;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.Settings;
using MegaCrit.Sts2.Core.Unlocks;

namespace MegaCrit.Sts2.Core.Saves;

[JsonSourceGenerationOptions(WriteIndented = true, IncludeFields = true, ReadCommentHandling = JsonCommentHandling.Skip, Converters = new Type[]
{
	typeof(ModelIdRunSaveConverter),
	typeof(SnakeCaseJsonStringEnumConverter<Achievement>),
	typeof(SnakeCaseJsonStringEnumConverter<AspectRatioSetting>),
	typeof(SnakeCaseJsonStringEnumConverter<VSyncType>),
	typeof(SnakeCaseJsonStringEnumConverter<GameMode>),
	typeof(SnakeCaseJsonStringEnumConverter<RelicRarity>),
	typeof(SnakeCaseJsonStringEnumConverter<RunRngType>),
	typeof(SnakeCaseJsonStringEnumConverter<PlayerRngType>),
	typeof(SnakeCaseJsonStringEnumConverter<MapPointType>),
	typeof(SnakeCaseJsonStringEnumConverter<ModSource>),
	typeof(SnakeCaseJsonStringEnumConverter<PlatformType>),
	typeof(SnakeCaseJsonStringEnumConverter<RewardType>),
	typeof(SnakeCaseJsonStringEnumConverter<RoomType>),
	typeof(SnakeCaseJsonStringEnumConverter<EpochState>),
	typeof(SnakeCaseJsonStringEnumConverter<FastModeType>),
	typeof(SnakeCaseJsonStringEnumConverter<CardCreationSource>),
	typeof(SnakeCaseJsonStringEnumConverter<CardRarityOddsType>),
	typeof(SnakeCaseJsonStringEnumConverter<ControllerMappingType>),
	typeof(SnakeCaseJsonStringEnumConverter<BadgeRarity>)
}, UnmappedMemberHandling = JsonUnmappedMemberHandling.Skip, GenerationMode = JsonSourceGenerationMode.Metadata)]
[JsonSerializable(typeof(NullLeaderboardFile))]
[JsonSerializable(typeof(SerializableRun))]
[JsonSerializable(typeof(FeedbackData))]
[JsonSerializable(typeof(SettingsSave))]
[JsonSerializable(typeof(PrefsSave))]
[JsonSerializable(typeof(ProfileSave))]
[JsonSerializable(typeof(SerializableProgress))]
[JsonSerializable(typeof(RunHistory))]
[JsonSerializable(typeof(List<MigratingData>))]
[JsonSerializable(typeof(List<List<PlayerMapPointHistoryEntry>>))]
[JsonSerializable(typeof(ModManifest))]
[JsonSerializable(typeof(JsonObject))]
[JsonSerializable(typeof(List<JsonNode>))]
[JsonSerializable(typeof(long))]
[JsonSerializable(typeof(double))]
[JsonSerializable(typeof(List<Dictionary<string, object>>))]
[JsonSerializable(typeof(List<NullMultiplayerName>))]
[GeneratedCode("System.Text.Json.SourceGeneration", "9.0.12.31616")]
internal class MegaCritSerializerContext : JsonSerializerContext, IJsonTypeInfoResolver
{
	private JsonTypeInfo<bool>? _Boolean;

	private JsonTypeInfo<double>? _Double;

	private JsonTypeInfo<float>? _Single;

	private JsonTypeInfo<Vector2>? _Vector2;

	private JsonTypeInfo<Vector2I>? _Vector2I;

	private JsonTypeInfo<ControllerMappingType>? _ControllerMappingType;

	private JsonTypeInfo<RelicRarity>? _RelicRarity;

	private JsonTypeInfo<PlayerRngType>? _PlayerRngType;

	private JsonTypeInfo<RunRngType>? _RunRngType;

	private JsonTypeInfo<LocString>? _LocString;

	private JsonTypeInfo<MapCoord>? _MapCoord;

	private JsonTypeInfo<MapPointType>? _MapPointType;

	private JsonTypeInfo<ModDependency>? _ModDependency;

	private JsonTypeInfo<ModManifest>? _ModManifest;

	private JsonTypeInfo<ModSettings>? _ModSettings;

	private JsonTypeInfo<ModSource>? _ModSource;

	private JsonTypeInfo<SettingsSaveMod>? _SettingsSaveMod;

	private JsonTypeInfo<BadgeRarity>? _BadgeRarity;

	private JsonTypeInfo<ModelId>? _ModelId;

	private JsonTypeInfo<FeedbackData>? _FeedbackData;

	private JsonTypeInfo<NullLeaderboard>? _NullLeaderboard;

	private JsonTypeInfo<NullLeaderboardFile>? _NullLeaderboardFile;

	private JsonTypeInfo<NullLeaderboardFileEntry>? _NullLeaderboardFileEntry;

	private JsonTypeInfo<NullMultiplayerName>? _NullMultiplayerName;

	private JsonTypeInfo<PlatformType>? _PlatformType;

	private JsonTypeInfo<RewardType>? _RewardType;

	private JsonTypeInfo<RoomType>? _RoomType;

	private JsonTypeInfo<CardCreationSource>? _CardCreationSource;

	private JsonTypeInfo<CardRarityOddsType>? _CardRarityOddsType;

	private JsonTypeInfo<GameMode>? _GameMode;

	private JsonTypeInfo<AncientChoiceHistoryEntry>? _AncientChoiceHistoryEntry;

	private JsonTypeInfo<CardChoiceHistoryEntry>? _CardChoiceHistoryEntry;

	private JsonTypeInfo<CardEnchantmentHistoryEntry>? _CardEnchantmentHistoryEntry;

	private JsonTypeInfo<CardTransformationHistoryEntry>? _CardTransformationHistoryEntry;

	private JsonTypeInfo<EventOptionHistoryEntry>? _EventOptionHistoryEntry;

	private JsonTypeInfo<MapPointHistoryEntry>? _MapPointHistoryEntry;

	private JsonTypeInfo<MapPointRoomHistoryEntry>? _MapPointRoomHistoryEntry;

	private JsonTypeInfo<ModelChoiceHistoryEntry>? _ModelChoiceHistoryEntry;

	private JsonTypeInfo<PlayerMapPointHistoryEntry>? _PlayerMapPointHistoryEntry;

	private JsonTypeInfo<RunHistory>? _RunHistory;

	private JsonTypeInfo<RunHistoryPlayer>? _RunHistoryPlayer;

	private JsonTypeInfo<AncientCharacterStats>? _AncientCharacterStats;

	private JsonTypeInfo<AncientStats>? _AncientStats;

	private JsonTypeInfo<BadgeStats>? _BadgeStats;

	private JsonTypeInfo<CardStats>? _CardStats;

	private JsonTypeInfo<CharacterStats>? _CharacterStats;

	private JsonTypeInfo<EncounterStats>? _EncounterStats;

	private JsonTypeInfo<EnemyStats>? _EnemyStats;

	private JsonTypeInfo<EpochState>? _EpochState;

	private JsonTypeInfo<FightStats>? _FightStats;

	private JsonTypeInfo<SerializableMapDrawingLine>? _SerializableMapDrawingLine;

	private JsonTypeInfo<SerializableMapDrawings>? _SerializableMapDrawings;

	private JsonTypeInfo<SerializablePlayerMapDrawings>? _SerializablePlayerMapDrawings;

	private JsonTypeInfo<MigratingData>? _MigratingData;

	private JsonTypeInfo<PrefsSave>? _PrefsSave;

	private JsonTypeInfo<ProfileSave>? _ProfileSave;

	private JsonTypeInfo<SavedProperties>? _SavedProperties;

	private JsonTypeInfo<SavedProperties.SavedProperty<bool>>? _SavedPropertyBoolean;

	private JsonTypeInfo<SavedProperties.SavedProperty<ModelId>>? _SavedPropertyModelId;

	private JsonTypeInfo<SavedProperties.SavedProperty<SerializableCard[]>>? _SavedPropertySerializableCardArray;

	private JsonTypeInfo<SavedProperties.SavedProperty<SerializableCard>>? _SavedPropertySerializableCard;

	private JsonTypeInfo<SavedProperties.SavedProperty<int[]>>? _SavedPropertyInt32Array;

	private JsonTypeInfo<SavedProperties.SavedProperty<int>>? _SavedPropertyInt32;

	private JsonTypeInfo<SavedProperties.SavedProperty<string>>? _SavedPropertyString;

	private JsonTypeInfo<SerializableActMap>? _SerializableActMap;

	private JsonTypeInfo<SerializableActModel>? _SerializableActModel;

	private JsonTypeInfo<SerializableBadge>? _SerializableBadge;

	private JsonTypeInfo<SerializableCard>? _SerializableCard;

	private JsonTypeInfo<SerializableCard[]>? _SerializableCardArray;

	private JsonTypeInfo<SerializableEnchantment>? _SerializableEnchantment;

	private JsonTypeInfo<SerializableExtraRunFields>? _SerializableExtraRunFields;

	private JsonTypeInfo<SerializableMapPoint>? _SerializableMapPoint;

	private JsonTypeInfo<SerializableModifier>? _SerializableModifier;

	private JsonTypeInfo<SerializablePlayer>? _SerializablePlayer;

	private JsonTypeInfo<SerializablePlayerOddsSet>? _SerializablePlayerOddsSet;

	private JsonTypeInfo<SerializablePotion>? _SerializablePotion;

	private JsonTypeInfo<SerializableRelic>? _SerializableRelic;

	private JsonTypeInfo<SerializableRelicGrabBag>? _SerializableRelicGrabBag;

	private JsonTypeInfo<SerializableReward>? _SerializableReward;

	private JsonTypeInfo<SerializableRoom>? _SerializableRoom;

	private JsonTypeInfo<SerializableRoomSet>? _SerializableRoomSet;

	private JsonTypeInfo<SerializableRunOddsSet>? _SerializableRunOddsSet;

	private JsonTypeInfo<SerializableRunRngSet>? _SerializableRunRngSet;

	private JsonTypeInfo<SerializableEpoch>? _SerializableEpoch;

	private JsonTypeInfo<SerializableExtraPlayerFields>? _SerializableExtraPlayerFields;

	private JsonTypeInfo<SerializablePlayerRngSet>? _SerializablePlayerRngSet;

	private JsonTypeInfo<SerializableProgress>? _SerializableProgress;

	private JsonTypeInfo<SerializableRng>? _SerializableRng;

	private JsonTypeInfo<SerializableRun>? _SerializableRun;

	private JsonTypeInfo<SerializableUnlockedAchievement>? _SerializableUnlockedAchievement;

	private JsonTypeInfo<SettingsSave>? _SettingsSave;

	private JsonTypeInfo<AspectRatioSetting>? _AspectRatioSetting;

	private JsonTypeInfo<FastModeType>? _FastModeType;

	private JsonTypeInfo<VSyncType>? _VSyncType;

	private JsonTypeInfo<SerializableUnlockState>? _SerializableUnlockState;

	private JsonTypeInfo<Dictionary<RelicRarity, List<ModelId>>>? _DictionaryRelicRarityListModelId;

	private JsonTypeInfo<Dictionary<PlayerRngType, SerializableRng>>? _DictionaryPlayerRngTypeSerializableRng;

	private JsonTypeInfo<Dictionary<RunRngType, SerializableRng>>? _DictionaryRunRngTypeSerializableRng;

	private JsonTypeInfo<Dictionary<string, object>>? _DictionaryStringObject;

	private JsonTypeInfo<Dictionary<string, string>>? _DictionaryStringString;

	private JsonTypeInfo<Dictionary<ulong, List<SerializableReward>>>? _DictionaryUInt64ListSerializableReward;

	private JsonTypeInfo<IEnumerable<SerializableBadge>>? _IEnumerableSerializableBadge;

	private JsonTypeInfo<IEnumerable<SerializableCard>>? _IEnumerableSerializableCard;

	private JsonTypeInfo<IEnumerable<SerializablePotion>>? _IEnumerableSerializablePotion;

	private JsonTypeInfo<IEnumerable<SerializableRelic>>? _IEnumerableSerializableRelic;

	private JsonTypeInfo<List<Vector2>>? _ListVector2;

	private JsonTypeInfo<List<MapCoord>>? _ListMapCoord;

	private JsonTypeInfo<List<ModDependency>>? _ListModDependency;

	private JsonTypeInfo<List<SettingsSaveMod>>? _ListSettingsSaveMod;

	private JsonTypeInfo<List<ModelId>>? _ListModelId;

	private JsonTypeInfo<List<NullLeaderboard>>? _ListNullLeaderboard;

	private JsonTypeInfo<List<NullLeaderboardFileEntry>>? _ListNullLeaderboardFileEntry;

	private JsonTypeInfo<List<NullMultiplayerName>>? _ListNullMultiplayerName;

	private JsonTypeInfo<List<AncientChoiceHistoryEntry>>? _ListAncientChoiceHistoryEntry;

	private JsonTypeInfo<List<CardChoiceHistoryEntry>>? _ListCardChoiceHistoryEntry;

	private JsonTypeInfo<List<CardEnchantmentHistoryEntry>>? _ListCardEnchantmentHistoryEntry;

	private JsonTypeInfo<List<CardTransformationHistoryEntry>>? _ListCardTransformationHistoryEntry;

	private JsonTypeInfo<List<EventOptionHistoryEntry>>? _ListEventOptionHistoryEntry;

	private JsonTypeInfo<List<MapPointHistoryEntry>>? _ListMapPointHistoryEntry;

	private JsonTypeInfo<List<MapPointRoomHistoryEntry>>? _ListMapPointRoomHistoryEntry;

	private JsonTypeInfo<List<ModelChoiceHistoryEntry>>? _ListModelChoiceHistoryEntry;

	private JsonTypeInfo<List<PlayerMapPointHistoryEntry>>? _ListPlayerMapPointHistoryEntry;

	private JsonTypeInfo<List<RunHistoryPlayer>>? _ListRunHistoryPlayer;

	private JsonTypeInfo<List<AncientCharacterStats>>? _ListAncientCharacterStats;

	private JsonTypeInfo<List<AncientStats>>? _ListAncientStats;

	private JsonTypeInfo<List<BadgeStats>>? _ListBadgeStats;

	private JsonTypeInfo<List<CardStats>>? _ListCardStats;

	private JsonTypeInfo<List<CharacterStats>>? _ListCharacterStats;

	private JsonTypeInfo<List<EncounterStats>>? _ListEncounterStats;

	private JsonTypeInfo<List<EnemyStats>>? _ListEnemyStats;

	private JsonTypeInfo<List<FightStats>>? _ListFightStats;

	private JsonTypeInfo<List<SerializableMapDrawingLine>>? _ListSerializableMapDrawingLine;

	private JsonTypeInfo<List<SerializablePlayerMapDrawings>>? _ListSerializablePlayerMapDrawings;

	private JsonTypeInfo<List<MigratingData>>? _ListMigratingData;

	private JsonTypeInfo<List<SavedProperties.SavedProperty<bool>>>? _ListSavedPropertyBoolean;

	private JsonTypeInfo<List<SavedProperties.SavedProperty<ModelId>>>? _ListSavedPropertyModelId;

	private JsonTypeInfo<List<SavedProperties.SavedProperty<SerializableCard[]>>>? _ListSavedPropertySerializableCardArray;

	private JsonTypeInfo<List<SavedProperties.SavedProperty<SerializableCard>>>? _ListSavedPropertySerializableCard;

	private JsonTypeInfo<List<SavedProperties.SavedProperty<int[]>>>? _ListSavedPropertyInt32Array;

	private JsonTypeInfo<List<SavedProperties.SavedProperty<int>>>? _ListSavedPropertyInt32;

	private JsonTypeInfo<List<SavedProperties.SavedProperty<string>>>? _ListSavedPropertyString;

	private JsonTypeInfo<List<SerializableActModel>>? _ListSerializableActModel;

	private JsonTypeInfo<List<SerializableCard>>? _ListSerializableCard;

	private JsonTypeInfo<List<SerializableMapPoint>>? _ListSerializableMapPoint;

	private JsonTypeInfo<List<SerializableModifier>>? _ListSerializableModifier;

	private JsonTypeInfo<List<SerializablePlayer>>? _ListSerializablePlayer;

	private JsonTypeInfo<List<SerializablePotion>>? _ListSerializablePotion;

	private JsonTypeInfo<List<SerializableRelic>>? _ListSerializableRelic;

	private JsonTypeInfo<List<SerializableReward>>? _ListSerializableReward;

	private JsonTypeInfo<List<SerializableEpoch>>? _ListSerializableEpoch;

	private JsonTypeInfo<List<SerializableUnlockedAchievement>>? _ListSerializableUnlockedAchievement;

	private JsonTypeInfo<List<Dictionary<string, object>>>? _ListDictionaryStringObject;

	private JsonTypeInfo<List<List<MapPointHistoryEntry>>>? _ListListMapPointHistoryEntry;

	private JsonTypeInfo<List<List<PlayerMapPointHistoryEntry>>>? _ListListPlayerMapPointHistoryEntry;

	private JsonTypeInfo<List<JsonNode>>? _ListJsonNode;

	private JsonTypeInfo<List<string>>? _ListString;

	private JsonTypeInfo<List<ulong>>? _ListUInt64;

	private JsonTypeInfo<DateTimeOffset>? _DateTimeOffset;

	private JsonTypeInfo<DateTimeOffset?>? _NullableDateTimeOffset;

	private JsonTypeInfo<JsonNode>? _JsonNode;

	private JsonTypeInfo<JsonObject>? _JsonObject;

	private JsonTypeInfo<int>? _Int32;

	private JsonTypeInfo<int?>? _NullableInt32;

	private JsonTypeInfo<int[]>? _Int32Array;

	private JsonTypeInfo<long>? _Int64;

	private JsonTypeInfo<object>? _Object;

	private JsonTypeInfo<string>? _String;

	private JsonTypeInfo<ulong>? _UInt64;

	private static readonly JsonSerializerOptions s_defaultOptions = new JsonSerializerOptions
	{
		Converters = 
		{
			(JsonConverter)new ModelIdRunSaveConverter(),
			(JsonConverter)new SnakeCaseJsonStringEnumConverter<Achievement>(),
			(JsonConverter)new SnakeCaseJsonStringEnumConverter<AspectRatioSetting>(),
			(JsonConverter)new SnakeCaseJsonStringEnumConverter<VSyncType>(),
			(JsonConverter)new SnakeCaseJsonStringEnumConverter<GameMode>(),
			(JsonConverter)new SnakeCaseJsonStringEnumConverter<RelicRarity>(),
			(JsonConverter)new SnakeCaseJsonStringEnumConverter<RunRngType>(),
			(JsonConverter)new SnakeCaseJsonStringEnumConverter<PlayerRngType>(),
			(JsonConverter)new SnakeCaseJsonStringEnumConverter<MapPointType>(),
			(JsonConverter)new SnakeCaseJsonStringEnumConverter<ModSource>(),
			(JsonConverter)new SnakeCaseJsonStringEnumConverter<PlatformType>(),
			(JsonConverter)new SnakeCaseJsonStringEnumConverter<RewardType>(),
			(JsonConverter)new SnakeCaseJsonStringEnumConverter<RoomType>(),
			(JsonConverter)new SnakeCaseJsonStringEnumConverter<EpochState>(),
			(JsonConverter)new SnakeCaseJsonStringEnumConverter<FastModeType>(),
			(JsonConverter)new SnakeCaseJsonStringEnumConverter<CardCreationSource>(),
			(JsonConverter)new SnakeCaseJsonStringEnumConverter<CardRarityOddsType>(),
			(JsonConverter)new SnakeCaseJsonStringEnumConverter<ControllerMappingType>(),
			(JsonConverter)new SnakeCaseJsonStringEnumConverter<BadgeRarity>()
		},
		IncludeFields = true,
		ReadCommentHandling = JsonCommentHandling.Skip,
		UnmappedMemberHandling = JsonUnmappedMemberHandling.Skip,
		WriteIndented = true
	};

	private const BindingFlags InstanceMemberBindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

	public static JsonSerializerOptions DefaultGeneratedSerializerOptions => Default.GeneratedSerializerOptions;

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<bool> Boolean => _Boolean ?? (_Boolean = (JsonTypeInfo<bool>)Options.GetTypeInfo(typeof(bool)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<double> Double => _Double ?? (_Double = (JsonTypeInfo<double>)Options.GetTypeInfo(typeof(double)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<float> Single => _Single ?? (_Single = (JsonTypeInfo<float>)Options.GetTypeInfo(typeof(float)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<Vector2> Vector2 => _Vector2 ?? (_Vector2 = (JsonTypeInfo<Vector2>)Options.GetTypeInfo(typeof(Vector2)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<Vector2I> Vector2I => _Vector2I ?? (_Vector2I = (JsonTypeInfo<Vector2I>)Options.GetTypeInfo(typeof(Vector2I)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<ControllerMappingType> ControllerMappingType => _ControllerMappingType ?? (_ControllerMappingType = (JsonTypeInfo<ControllerMappingType>)Options.GetTypeInfo(typeof(ControllerMappingType)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<RelicRarity> RelicRarity => _RelicRarity ?? (_RelicRarity = (JsonTypeInfo<RelicRarity>)Options.GetTypeInfo(typeof(RelicRarity)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<PlayerRngType> PlayerRngType => _PlayerRngType ?? (_PlayerRngType = (JsonTypeInfo<PlayerRngType>)Options.GetTypeInfo(typeof(PlayerRngType)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<RunRngType> RunRngType => _RunRngType ?? (_RunRngType = (JsonTypeInfo<RunRngType>)Options.GetTypeInfo(typeof(RunRngType)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<LocString> LocString => _LocString ?? (_LocString = (JsonTypeInfo<LocString>)Options.GetTypeInfo(typeof(LocString)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<MapCoord> MapCoord => _MapCoord ?? (_MapCoord = (JsonTypeInfo<MapCoord>)Options.GetTypeInfo(typeof(MapCoord)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<MapPointType> MapPointType => _MapPointType ?? (_MapPointType = (JsonTypeInfo<MapPointType>)Options.GetTypeInfo(typeof(MapPointType)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<ModDependency> ModDependency => _ModDependency ?? (_ModDependency = (JsonTypeInfo<ModDependency>)Options.GetTypeInfo(typeof(ModDependency)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<ModManifest> ModManifest => _ModManifest ?? (_ModManifest = (JsonTypeInfo<ModManifest>)Options.GetTypeInfo(typeof(ModManifest)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<ModSettings> ModSettings => _ModSettings ?? (_ModSettings = (JsonTypeInfo<ModSettings>)Options.GetTypeInfo(typeof(ModSettings)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<ModSource> ModSource => _ModSource ?? (_ModSource = (JsonTypeInfo<ModSource>)Options.GetTypeInfo(typeof(ModSource)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<SettingsSaveMod> SettingsSaveMod => _SettingsSaveMod ?? (_SettingsSaveMod = (JsonTypeInfo<SettingsSaveMod>)Options.GetTypeInfo(typeof(SettingsSaveMod)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<BadgeRarity> BadgeRarity => _BadgeRarity ?? (_BadgeRarity = (JsonTypeInfo<BadgeRarity>)Options.GetTypeInfo(typeof(BadgeRarity)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<ModelId> ModelId => _ModelId ?? (_ModelId = (JsonTypeInfo<ModelId>)Options.GetTypeInfo(typeof(ModelId)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<FeedbackData> FeedbackData => _FeedbackData ?? (_FeedbackData = (JsonTypeInfo<FeedbackData>)Options.GetTypeInfo(typeof(FeedbackData)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<NullLeaderboard> NullLeaderboard => _NullLeaderboard ?? (_NullLeaderboard = (JsonTypeInfo<NullLeaderboard>)Options.GetTypeInfo(typeof(NullLeaderboard)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<NullLeaderboardFile> NullLeaderboardFile => _NullLeaderboardFile ?? (_NullLeaderboardFile = (JsonTypeInfo<NullLeaderboardFile>)Options.GetTypeInfo(typeof(NullLeaderboardFile)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<NullLeaderboardFileEntry> NullLeaderboardFileEntry => _NullLeaderboardFileEntry ?? (_NullLeaderboardFileEntry = (JsonTypeInfo<NullLeaderboardFileEntry>)Options.GetTypeInfo(typeof(NullLeaderboardFileEntry)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<NullMultiplayerName> NullMultiplayerName => _NullMultiplayerName ?? (_NullMultiplayerName = (JsonTypeInfo<NullMultiplayerName>)Options.GetTypeInfo(typeof(NullMultiplayerName)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<PlatformType> PlatformType => _PlatformType ?? (_PlatformType = (JsonTypeInfo<PlatformType>)Options.GetTypeInfo(typeof(PlatformType)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<RewardType> RewardType => _RewardType ?? (_RewardType = (JsonTypeInfo<RewardType>)Options.GetTypeInfo(typeof(RewardType)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<RoomType> RoomType => _RoomType ?? (_RoomType = (JsonTypeInfo<RoomType>)Options.GetTypeInfo(typeof(RoomType)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<CardCreationSource> CardCreationSource => _CardCreationSource ?? (_CardCreationSource = (JsonTypeInfo<CardCreationSource>)Options.GetTypeInfo(typeof(CardCreationSource)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<CardRarityOddsType> CardRarityOddsType => _CardRarityOddsType ?? (_CardRarityOddsType = (JsonTypeInfo<CardRarityOddsType>)Options.GetTypeInfo(typeof(CardRarityOddsType)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<GameMode> GameMode => _GameMode ?? (_GameMode = (JsonTypeInfo<GameMode>)Options.GetTypeInfo(typeof(GameMode)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<AncientChoiceHistoryEntry> AncientChoiceHistoryEntry => _AncientChoiceHistoryEntry ?? (_AncientChoiceHistoryEntry = (JsonTypeInfo<AncientChoiceHistoryEntry>)Options.GetTypeInfo(typeof(AncientChoiceHistoryEntry)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<CardChoiceHistoryEntry> CardChoiceHistoryEntry => _CardChoiceHistoryEntry ?? (_CardChoiceHistoryEntry = (JsonTypeInfo<CardChoiceHistoryEntry>)Options.GetTypeInfo(typeof(CardChoiceHistoryEntry)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<CardEnchantmentHistoryEntry> CardEnchantmentHistoryEntry => _CardEnchantmentHistoryEntry ?? (_CardEnchantmentHistoryEntry = (JsonTypeInfo<CardEnchantmentHistoryEntry>)Options.GetTypeInfo(typeof(CardEnchantmentHistoryEntry)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<CardTransformationHistoryEntry> CardTransformationHistoryEntry => _CardTransformationHistoryEntry ?? (_CardTransformationHistoryEntry = (JsonTypeInfo<CardTransformationHistoryEntry>)Options.GetTypeInfo(typeof(CardTransformationHistoryEntry)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<EventOptionHistoryEntry> EventOptionHistoryEntry => _EventOptionHistoryEntry ?? (_EventOptionHistoryEntry = (JsonTypeInfo<EventOptionHistoryEntry>)Options.GetTypeInfo(typeof(EventOptionHistoryEntry)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<MapPointHistoryEntry> MapPointHistoryEntry => _MapPointHistoryEntry ?? (_MapPointHistoryEntry = (JsonTypeInfo<MapPointHistoryEntry>)Options.GetTypeInfo(typeof(MapPointHistoryEntry)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<MapPointRoomHistoryEntry> MapPointRoomHistoryEntry => _MapPointRoomHistoryEntry ?? (_MapPointRoomHistoryEntry = (JsonTypeInfo<MapPointRoomHistoryEntry>)Options.GetTypeInfo(typeof(MapPointRoomHistoryEntry)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<ModelChoiceHistoryEntry> ModelChoiceHistoryEntry => _ModelChoiceHistoryEntry ?? (_ModelChoiceHistoryEntry = (JsonTypeInfo<ModelChoiceHistoryEntry>)Options.GetTypeInfo(typeof(ModelChoiceHistoryEntry)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<PlayerMapPointHistoryEntry> PlayerMapPointHistoryEntry => _PlayerMapPointHistoryEntry ?? (_PlayerMapPointHistoryEntry = (JsonTypeInfo<PlayerMapPointHistoryEntry>)Options.GetTypeInfo(typeof(PlayerMapPointHistoryEntry)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<RunHistory> RunHistory => _RunHistory ?? (_RunHistory = (JsonTypeInfo<RunHistory>)Options.GetTypeInfo(typeof(RunHistory)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<RunHistoryPlayer> RunHistoryPlayer => _RunHistoryPlayer ?? (_RunHistoryPlayer = (JsonTypeInfo<RunHistoryPlayer>)Options.GetTypeInfo(typeof(RunHistoryPlayer)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<AncientCharacterStats> AncientCharacterStats => _AncientCharacterStats ?? (_AncientCharacterStats = (JsonTypeInfo<AncientCharacterStats>)Options.GetTypeInfo(typeof(AncientCharacterStats)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<AncientStats> AncientStats => _AncientStats ?? (_AncientStats = (JsonTypeInfo<AncientStats>)Options.GetTypeInfo(typeof(AncientStats)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<BadgeStats> BadgeStats => _BadgeStats ?? (_BadgeStats = (JsonTypeInfo<BadgeStats>)Options.GetTypeInfo(typeof(BadgeStats)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<CardStats> CardStats => _CardStats ?? (_CardStats = (JsonTypeInfo<CardStats>)Options.GetTypeInfo(typeof(CardStats)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<CharacterStats> CharacterStats => _CharacterStats ?? (_CharacterStats = (JsonTypeInfo<CharacterStats>)Options.GetTypeInfo(typeof(CharacterStats)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<EncounterStats> EncounterStats => _EncounterStats ?? (_EncounterStats = (JsonTypeInfo<EncounterStats>)Options.GetTypeInfo(typeof(EncounterStats)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<EnemyStats> EnemyStats => _EnemyStats ?? (_EnemyStats = (JsonTypeInfo<EnemyStats>)Options.GetTypeInfo(typeof(EnemyStats)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<EpochState> EpochState => _EpochState ?? (_EpochState = (JsonTypeInfo<EpochState>)Options.GetTypeInfo(typeof(EpochState)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<FightStats> FightStats => _FightStats ?? (_FightStats = (JsonTypeInfo<FightStats>)Options.GetTypeInfo(typeof(FightStats)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<SerializableMapDrawingLine> SerializableMapDrawingLine => _SerializableMapDrawingLine ?? (_SerializableMapDrawingLine = (JsonTypeInfo<SerializableMapDrawingLine>)Options.GetTypeInfo(typeof(SerializableMapDrawingLine)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<SerializableMapDrawings> SerializableMapDrawings => _SerializableMapDrawings ?? (_SerializableMapDrawings = (JsonTypeInfo<SerializableMapDrawings>)Options.GetTypeInfo(typeof(SerializableMapDrawings)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<SerializablePlayerMapDrawings> SerializablePlayerMapDrawings => _SerializablePlayerMapDrawings ?? (_SerializablePlayerMapDrawings = (JsonTypeInfo<SerializablePlayerMapDrawings>)Options.GetTypeInfo(typeof(SerializablePlayerMapDrawings)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<MigratingData> MigratingData => _MigratingData ?? (_MigratingData = (JsonTypeInfo<MigratingData>)Options.GetTypeInfo(typeof(MigratingData)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<PrefsSave> PrefsSave => _PrefsSave ?? (_PrefsSave = (JsonTypeInfo<PrefsSave>)Options.GetTypeInfo(typeof(PrefsSave)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<ProfileSave> ProfileSave => _ProfileSave ?? (_ProfileSave = (JsonTypeInfo<ProfileSave>)Options.GetTypeInfo(typeof(ProfileSave)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<SavedProperties> SavedProperties => _SavedProperties ?? (_SavedProperties = (JsonTypeInfo<SavedProperties>)Options.GetTypeInfo(typeof(SavedProperties)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<SavedProperties.SavedProperty<bool>> SavedPropertyBoolean => _SavedPropertyBoolean ?? (_SavedPropertyBoolean = (JsonTypeInfo<SavedProperties.SavedProperty<bool>>)Options.GetTypeInfo(typeof(SavedProperties.SavedProperty<bool>)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<SavedProperties.SavedProperty<ModelId>> SavedPropertyModelId => _SavedPropertyModelId ?? (_SavedPropertyModelId = (JsonTypeInfo<SavedProperties.SavedProperty<ModelId>>)Options.GetTypeInfo(typeof(SavedProperties.SavedProperty<ModelId>)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<SavedProperties.SavedProperty<SerializableCard[]>> SavedPropertySerializableCardArray => _SavedPropertySerializableCardArray ?? (_SavedPropertySerializableCardArray = (JsonTypeInfo<SavedProperties.SavedProperty<SerializableCard[]>>)Options.GetTypeInfo(typeof(SavedProperties.SavedProperty<SerializableCard[]>)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<SavedProperties.SavedProperty<SerializableCard>> SavedPropertySerializableCard => _SavedPropertySerializableCard ?? (_SavedPropertySerializableCard = (JsonTypeInfo<SavedProperties.SavedProperty<SerializableCard>>)Options.GetTypeInfo(typeof(SavedProperties.SavedProperty<SerializableCard>)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<SavedProperties.SavedProperty<int[]>> SavedPropertyInt32Array => _SavedPropertyInt32Array ?? (_SavedPropertyInt32Array = (JsonTypeInfo<SavedProperties.SavedProperty<int[]>>)Options.GetTypeInfo(typeof(SavedProperties.SavedProperty<int[]>)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<SavedProperties.SavedProperty<int>> SavedPropertyInt32 => _SavedPropertyInt32 ?? (_SavedPropertyInt32 = (JsonTypeInfo<SavedProperties.SavedProperty<int>>)Options.GetTypeInfo(typeof(SavedProperties.SavedProperty<int>)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<SavedProperties.SavedProperty<string>> SavedPropertyString => _SavedPropertyString ?? (_SavedPropertyString = (JsonTypeInfo<SavedProperties.SavedProperty<string>>)Options.GetTypeInfo(typeof(SavedProperties.SavedProperty<string>)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<SerializableActMap> SerializableActMap => _SerializableActMap ?? (_SerializableActMap = (JsonTypeInfo<SerializableActMap>)Options.GetTypeInfo(typeof(SerializableActMap)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<SerializableActModel> SerializableActModel => _SerializableActModel ?? (_SerializableActModel = (JsonTypeInfo<SerializableActModel>)Options.GetTypeInfo(typeof(SerializableActModel)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<SerializableBadge> SerializableBadge => _SerializableBadge ?? (_SerializableBadge = (JsonTypeInfo<SerializableBadge>)Options.GetTypeInfo(typeof(SerializableBadge)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<SerializableCard> SerializableCard => _SerializableCard ?? (_SerializableCard = (JsonTypeInfo<SerializableCard>)Options.GetTypeInfo(typeof(SerializableCard)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<SerializableCard[]> SerializableCardArray => _SerializableCardArray ?? (_SerializableCardArray = (JsonTypeInfo<SerializableCard[]>)Options.GetTypeInfo(typeof(SerializableCard[])));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<SerializableEnchantment> SerializableEnchantment => _SerializableEnchantment ?? (_SerializableEnchantment = (JsonTypeInfo<SerializableEnchantment>)Options.GetTypeInfo(typeof(SerializableEnchantment)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<SerializableExtraRunFields> SerializableExtraRunFields => _SerializableExtraRunFields ?? (_SerializableExtraRunFields = (JsonTypeInfo<SerializableExtraRunFields>)Options.GetTypeInfo(typeof(SerializableExtraRunFields)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<SerializableMapPoint> SerializableMapPoint => _SerializableMapPoint ?? (_SerializableMapPoint = (JsonTypeInfo<SerializableMapPoint>)Options.GetTypeInfo(typeof(SerializableMapPoint)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<SerializableModifier> SerializableModifier => _SerializableModifier ?? (_SerializableModifier = (JsonTypeInfo<SerializableModifier>)Options.GetTypeInfo(typeof(SerializableModifier)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<SerializablePlayer> SerializablePlayer => _SerializablePlayer ?? (_SerializablePlayer = (JsonTypeInfo<SerializablePlayer>)Options.GetTypeInfo(typeof(SerializablePlayer)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<SerializablePlayerOddsSet> SerializablePlayerOddsSet => _SerializablePlayerOddsSet ?? (_SerializablePlayerOddsSet = (JsonTypeInfo<SerializablePlayerOddsSet>)Options.GetTypeInfo(typeof(SerializablePlayerOddsSet)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<SerializablePotion> SerializablePotion => _SerializablePotion ?? (_SerializablePotion = (JsonTypeInfo<SerializablePotion>)Options.GetTypeInfo(typeof(SerializablePotion)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<SerializableRelic> SerializableRelic => _SerializableRelic ?? (_SerializableRelic = (JsonTypeInfo<SerializableRelic>)Options.GetTypeInfo(typeof(SerializableRelic)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<SerializableRelicGrabBag> SerializableRelicGrabBag => _SerializableRelicGrabBag ?? (_SerializableRelicGrabBag = (JsonTypeInfo<SerializableRelicGrabBag>)Options.GetTypeInfo(typeof(SerializableRelicGrabBag)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<SerializableReward> SerializableReward => _SerializableReward ?? (_SerializableReward = (JsonTypeInfo<SerializableReward>)Options.GetTypeInfo(typeof(SerializableReward)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<SerializableRoom> SerializableRoom => _SerializableRoom ?? (_SerializableRoom = (JsonTypeInfo<SerializableRoom>)Options.GetTypeInfo(typeof(SerializableRoom)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<SerializableRoomSet> SerializableRoomSet => _SerializableRoomSet ?? (_SerializableRoomSet = (JsonTypeInfo<SerializableRoomSet>)Options.GetTypeInfo(typeof(SerializableRoomSet)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<SerializableRunOddsSet> SerializableRunOddsSet => _SerializableRunOddsSet ?? (_SerializableRunOddsSet = (JsonTypeInfo<SerializableRunOddsSet>)Options.GetTypeInfo(typeof(SerializableRunOddsSet)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<SerializableRunRngSet> SerializableRunRngSet => _SerializableRunRngSet ?? (_SerializableRunRngSet = (JsonTypeInfo<SerializableRunRngSet>)Options.GetTypeInfo(typeof(SerializableRunRngSet)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<SerializableEpoch> SerializableEpoch => _SerializableEpoch ?? (_SerializableEpoch = (JsonTypeInfo<SerializableEpoch>)Options.GetTypeInfo(typeof(SerializableEpoch)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<SerializableExtraPlayerFields> SerializableExtraPlayerFields => _SerializableExtraPlayerFields ?? (_SerializableExtraPlayerFields = (JsonTypeInfo<SerializableExtraPlayerFields>)Options.GetTypeInfo(typeof(SerializableExtraPlayerFields)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<SerializablePlayerRngSet> SerializablePlayerRngSet => _SerializablePlayerRngSet ?? (_SerializablePlayerRngSet = (JsonTypeInfo<SerializablePlayerRngSet>)Options.GetTypeInfo(typeof(SerializablePlayerRngSet)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<SerializableProgress> SerializableProgress => _SerializableProgress ?? (_SerializableProgress = (JsonTypeInfo<SerializableProgress>)Options.GetTypeInfo(typeof(SerializableProgress)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<SerializableRng> SerializableRng => _SerializableRng ?? (_SerializableRng = (JsonTypeInfo<SerializableRng>)Options.GetTypeInfo(typeof(SerializableRng)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<SerializableRun> SerializableRun => _SerializableRun ?? (_SerializableRun = (JsonTypeInfo<SerializableRun>)Options.GetTypeInfo(typeof(SerializableRun)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<SerializableUnlockedAchievement> SerializableUnlockedAchievement => _SerializableUnlockedAchievement ?? (_SerializableUnlockedAchievement = (JsonTypeInfo<SerializableUnlockedAchievement>)Options.GetTypeInfo(typeof(SerializableUnlockedAchievement)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<SettingsSave> SettingsSave => _SettingsSave ?? (_SettingsSave = (JsonTypeInfo<SettingsSave>)Options.GetTypeInfo(typeof(SettingsSave)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<AspectRatioSetting> AspectRatioSetting => _AspectRatioSetting ?? (_AspectRatioSetting = (JsonTypeInfo<AspectRatioSetting>)Options.GetTypeInfo(typeof(AspectRatioSetting)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<FastModeType> FastModeType => _FastModeType ?? (_FastModeType = (JsonTypeInfo<FastModeType>)Options.GetTypeInfo(typeof(FastModeType)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<VSyncType> VSyncType => _VSyncType ?? (_VSyncType = (JsonTypeInfo<VSyncType>)Options.GetTypeInfo(typeof(VSyncType)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<SerializableUnlockState> SerializableUnlockState => _SerializableUnlockState ?? (_SerializableUnlockState = (JsonTypeInfo<SerializableUnlockState>)Options.GetTypeInfo(typeof(SerializableUnlockState)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<Dictionary<RelicRarity, List<ModelId>>> DictionaryRelicRarityListModelId => _DictionaryRelicRarityListModelId ?? (_DictionaryRelicRarityListModelId = (JsonTypeInfo<Dictionary<RelicRarity, List<ModelId>>>)Options.GetTypeInfo(typeof(Dictionary<RelicRarity, List<ModelId>>)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<Dictionary<PlayerRngType, SerializableRng>> DictionaryPlayerRngTypeSerializableRng => _DictionaryPlayerRngTypeSerializableRng ?? (_DictionaryPlayerRngTypeSerializableRng = (JsonTypeInfo<Dictionary<PlayerRngType, SerializableRng>>)Options.GetTypeInfo(typeof(Dictionary<PlayerRngType, SerializableRng>)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<Dictionary<RunRngType, SerializableRng>> DictionaryRunRngTypeSerializableRng => _DictionaryRunRngTypeSerializableRng ?? (_DictionaryRunRngTypeSerializableRng = (JsonTypeInfo<Dictionary<RunRngType, SerializableRng>>)Options.GetTypeInfo(typeof(Dictionary<RunRngType, SerializableRng>)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<Dictionary<string, object>> DictionaryStringObject => _DictionaryStringObject ?? (_DictionaryStringObject = (JsonTypeInfo<Dictionary<string, object>>)Options.GetTypeInfo(typeof(Dictionary<string, object>)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<Dictionary<string, string>> DictionaryStringString => _DictionaryStringString ?? (_DictionaryStringString = (JsonTypeInfo<Dictionary<string, string>>)Options.GetTypeInfo(typeof(Dictionary<string, string>)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<Dictionary<ulong, List<SerializableReward>>> DictionaryUInt64ListSerializableReward => _DictionaryUInt64ListSerializableReward ?? (_DictionaryUInt64ListSerializableReward = (JsonTypeInfo<Dictionary<ulong, List<SerializableReward>>>)Options.GetTypeInfo(typeof(Dictionary<ulong, List<SerializableReward>>)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<IEnumerable<SerializableBadge>> IEnumerableSerializableBadge => _IEnumerableSerializableBadge ?? (_IEnumerableSerializableBadge = (JsonTypeInfo<IEnumerable<SerializableBadge>>)Options.GetTypeInfo(typeof(IEnumerable<SerializableBadge>)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<IEnumerable<SerializableCard>> IEnumerableSerializableCard => _IEnumerableSerializableCard ?? (_IEnumerableSerializableCard = (JsonTypeInfo<IEnumerable<SerializableCard>>)Options.GetTypeInfo(typeof(IEnumerable<SerializableCard>)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<IEnumerable<SerializablePotion>> IEnumerableSerializablePotion => _IEnumerableSerializablePotion ?? (_IEnumerableSerializablePotion = (JsonTypeInfo<IEnumerable<SerializablePotion>>)Options.GetTypeInfo(typeof(IEnumerable<SerializablePotion>)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<IEnumerable<SerializableRelic>> IEnumerableSerializableRelic => _IEnumerableSerializableRelic ?? (_IEnumerableSerializableRelic = (JsonTypeInfo<IEnumerable<SerializableRelic>>)Options.GetTypeInfo(typeof(IEnumerable<SerializableRelic>)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<List<Vector2>> ListVector2 => _ListVector2 ?? (_ListVector2 = (JsonTypeInfo<List<Vector2>>)Options.GetTypeInfo(typeof(List<Vector2>)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<List<MapCoord>> ListMapCoord => _ListMapCoord ?? (_ListMapCoord = (JsonTypeInfo<List<MapCoord>>)Options.GetTypeInfo(typeof(List<MapCoord>)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<List<ModDependency>> ListModDependency => _ListModDependency ?? (_ListModDependency = (JsonTypeInfo<List<ModDependency>>)Options.GetTypeInfo(typeof(List<ModDependency>)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<List<SettingsSaveMod>> ListSettingsSaveMod => _ListSettingsSaveMod ?? (_ListSettingsSaveMod = (JsonTypeInfo<List<SettingsSaveMod>>)Options.GetTypeInfo(typeof(List<SettingsSaveMod>)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<List<ModelId>> ListModelId => _ListModelId ?? (_ListModelId = (JsonTypeInfo<List<ModelId>>)Options.GetTypeInfo(typeof(List<ModelId>)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<List<NullLeaderboard>> ListNullLeaderboard => _ListNullLeaderboard ?? (_ListNullLeaderboard = (JsonTypeInfo<List<NullLeaderboard>>)Options.GetTypeInfo(typeof(List<NullLeaderboard>)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<List<NullLeaderboardFileEntry>> ListNullLeaderboardFileEntry => _ListNullLeaderboardFileEntry ?? (_ListNullLeaderboardFileEntry = (JsonTypeInfo<List<NullLeaderboardFileEntry>>)Options.GetTypeInfo(typeof(List<NullLeaderboardFileEntry>)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<List<NullMultiplayerName>> ListNullMultiplayerName => _ListNullMultiplayerName ?? (_ListNullMultiplayerName = (JsonTypeInfo<List<NullMultiplayerName>>)Options.GetTypeInfo(typeof(List<NullMultiplayerName>)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<List<AncientChoiceHistoryEntry>> ListAncientChoiceHistoryEntry => _ListAncientChoiceHistoryEntry ?? (_ListAncientChoiceHistoryEntry = (JsonTypeInfo<List<AncientChoiceHistoryEntry>>)Options.GetTypeInfo(typeof(List<AncientChoiceHistoryEntry>)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<List<CardChoiceHistoryEntry>> ListCardChoiceHistoryEntry => _ListCardChoiceHistoryEntry ?? (_ListCardChoiceHistoryEntry = (JsonTypeInfo<List<CardChoiceHistoryEntry>>)Options.GetTypeInfo(typeof(List<CardChoiceHistoryEntry>)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<List<CardEnchantmentHistoryEntry>> ListCardEnchantmentHistoryEntry => _ListCardEnchantmentHistoryEntry ?? (_ListCardEnchantmentHistoryEntry = (JsonTypeInfo<List<CardEnchantmentHistoryEntry>>)Options.GetTypeInfo(typeof(List<CardEnchantmentHistoryEntry>)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<List<CardTransformationHistoryEntry>> ListCardTransformationHistoryEntry => _ListCardTransformationHistoryEntry ?? (_ListCardTransformationHistoryEntry = (JsonTypeInfo<List<CardTransformationHistoryEntry>>)Options.GetTypeInfo(typeof(List<CardTransformationHistoryEntry>)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<List<EventOptionHistoryEntry>> ListEventOptionHistoryEntry => _ListEventOptionHistoryEntry ?? (_ListEventOptionHistoryEntry = (JsonTypeInfo<List<EventOptionHistoryEntry>>)Options.GetTypeInfo(typeof(List<EventOptionHistoryEntry>)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<List<MapPointHistoryEntry>> ListMapPointHistoryEntry => _ListMapPointHistoryEntry ?? (_ListMapPointHistoryEntry = (JsonTypeInfo<List<MapPointHistoryEntry>>)Options.GetTypeInfo(typeof(List<MapPointHistoryEntry>)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<List<MapPointRoomHistoryEntry>> ListMapPointRoomHistoryEntry => _ListMapPointRoomHistoryEntry ?? (_ListMapPointRoomHistoryEntry = (JsonTypeInfo<List<MapPointRoomHistoryEntry>>)Options.GetTypeInfo(typeof(List<MapPointRoomHistoryEntry>)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<List<ModelChoiceHistoryEntry>> ListModelChoiceHistoryEntry => _ListModelChoiceHistoryEntry ?? (_ListModelChoiceHistoryEntry = (JsonTypeInfo<List<ModelChoiceHistoryEntry>>)Options.GetTypeInfo(typeof(List<ModelChoiceHistoryEntry>)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<List<PlayerMapPointHistoryEntry>> ListPlayerMapPointHistoryEntry => _ListPlayerMapPointHistoryEntry ?? (_ListPlayerMapPointHistoryEntry = (JsonTypeInfo<List<PlayerMapPointHistoryEntry>>)Options.GetTypeInfo(typeof(List<PlayerMapPointHistoryEntry>)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<List<RunHistoryPlayer>> ListRunHistoryPlayer => _ListRunHistoryPlayer ?? (_ListRunHistoryPlayer = (JsonTypeInfo<List<RunHistoryPlayer>>)Options.GetTypeInfo(typeof(List<RunHistoryPlayer>)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<List<AncientCharacterStats>> ListAncientCharacterStats => _ListAncientCharacterStats ?? (_ListAncientCharacterStats = (JsonTypeInfo<List<AncientCharacterStats>>)Options.GetTypeInfo(typeof(List<AncientCharacterStats>)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<List<AncientStats>> ListAncientStats => _ListAncientStats ?? (_ListAncientStats = (JsonTypeInfo<List<AncientStats>>)Options.GetTypeInfo(typeof(List<AncientStats>)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<List<BadgeStats>> ListBadgeStats => _ListBadgeStats ?? (_ListBadgeStats = (JsonTypeInfo<List<BadgeStats>>)Options.GetTypeInfo(typeof(List<BadgeStats>)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<List<CardStats>> ListCardStats => _ListCardStats ?? (_ListCardStats = (JsonTypeInfo<List<CardStats>>)Options.GetTypeInfo(typeof(List<CardStats>)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<List<CharacterStats>> ListCharacterStats => _ListCharacterStats ?? (_ListCharacterStats = (JsonTypeInfo<List<CharacterStats>>)Options.GetTypeInfo(typeof(List<CharacterStats>)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<List<EncounterStats>> ListEncounterStats => _ListEncounterStats ?? (_ListEncounterStats = (JsonTypeInfo<List<EncounterStats>>)Options.GetTypeInfo(typeof(List<EncounterStats>)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<List<EnemyStats>> ListEnemyStats => _ListEnemyStats ?? (_ListEnemyStats = (JsonTypeInfo<List<EnemyStats>>)Options.GetTypeInfo(typeof(List<EnemyStats>)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<List<FightStats>> ListFightStats => _ListFightStats ?? (_ListFightStats = (JsonTypeInfo<List<FightStats>>)Options.GetTypeInfo(typeof(List<FightStats>)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<List<SerializableMapDrawingLine>> ListSerializableMapDrawingLine => _ListSerializableMapDrawingLine ?? (_ListSerializableMapDrawingLine = (JsonTypeInfo<List<SerializableMapDrawingLine>>)Options.GetTypeInfo(typeof(List<SerializableMapDrawingLine>)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<List<SerializablePlayerMapDrawings>> ListSerializablePlayerMapDrawings => _ListSerializablePlayerMapDrawings ?? (_ListSerializablePlayerMapDrawings = (JsonTypeInfo<List<SerializablePlayerMapDrawings>>)Options.GetTypeInfo(typeof(List<SerializablePlayerMapDrawings>)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<List<MigratingData>> ListMigratingData => _ListMigratingData ?? (_ListMigratingData = (JsonTypeInfo<List<MigratingData>>)Options.GetTypeInfo(typeof(List<MigratingData>)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<List<SavedProperties.SavedProperty<bool>>> ListSavedPropertyBoolean => _ListSavedPropertyBoolean ?? (_ListSavedPropertyBoolean = (JsonTypeInfo<List<SavedProperties.SavedProperty<bool>>>)Options.GetTypeInfo(typeof(List<SavedProperties.SavedProperty<bool>>)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<List<SavedProperties.SavedProperty<ModelId>>> ListSavedPropertyModelId => _ListSavedPropertyModelId ?? (_ListSavedPropertyModelId = (JsonTypeInfo<List<SavedProperties.SavedProperty<ModelId>>>)Options.GetTypeInfo(typeof(List<SavedProperties.SavedProperty<ModelId>>)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<List<SavedProperties.SavedProperty<SerializableCard[]>>> ListSavedPropertySerializableCardArray => _ListSavedPropertySerializableCardArray ?? (_ListSavedPropertySerializableCardArray = (JsonTypeInfo<List<SavedProperties.SavedProperty<SerializableCard[]>>>)Options.GetTypeInfo(typeof(List<SavedProperties.SavedProperty<SerializableCard[]>>)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<List<SavedProperties.SavedProperty<SerializableCard>>> ListSavedPropertySerializableCard => _ListSavedPropertySerializableCard ?? (_ListSavedPropertySerializableCard = (JsonTypeInfo<List<SavedProperties.SavedProperty<SerializableCard>>>)Options.GetTypeInfo(typeof(List<SavedProperties.SavedProperty<SerializableCard>>)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<List<SavedProperties.SavedProperty<int[]>>> ListSavedPropertyInt32Array => _ListSavedPropertyInt32Array ?? (_ListSavedPropertyInt32Array = (JsonTypeInfo<List<SavedProperties.SavedProperty<int[]>>>)Options.GetTypeInfo(typeof(List<SavedProperties.SavedProperty<int[]>>)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<List<SavedProperties.SavedProperty<int>>> ListSavedPropertyInt32 => _ListSavedPropertyInt32 ?? (_ListSavedPropertyInt32 = (JsonTypeInfo<List<SavedProperties.SavedProperty<int>>>)Options.GetTypeInfo(typeof(List<SavedProperties.SavedProperty<int>>)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<List<SavedProperties.SavedProperty<string>>> ListSavedPropertyString => _ListSavedPropertyString ?? (_ListSavedPropertyString = (JsonTypeInfo<List<SavedProperties.SavedProperty<string>>>)Options.GetTypeInfo(typeof(List<SavedProperties.SavedProperty<string>>)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<List<SerializableActModel>> ListSerializableActModel => _ListSerializableActModel ?? (_ListSerializableActModel = (JsonTypeInfo<List<SerializableActModel>>)Options.GetTypeInfo(typeof(List<SerializableActModel>)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<List<SerializableCard>> ListSerializableCard => _ListSerializableCard ?? (_ListSerializableCard = (JsonTypeInfo<List<SerializableCard>>)Options.GetTypeInfo(typeof(List<SerializableCard>)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<List<SerializableMapPoint>> ListSerializableMapPoint => _ListSerializableMapPoint ?? (_ListSerializableMapPoint = (JsonTypeInfo<List<SerializableMapPoint>>)Options.GetTypeInfo(typeof(List<SerializableMapPoint>)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<List<SerializableModifier>> ListSerializableModifier => _ListSerializableModifier ?? (_ListSerializableModifier = (JsonTypeInfo<List<SerializableModifier>>)Options.GetTypeInfo(typeof(List<SerializableModifier>)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<List<SerializablePlayer>> ListSerializablePlayer => _ListSerializablePlayer ?? (_ListSerializablePlayer = (JsonTypeInfo<List<SerializablePlayer>>)Options.GetTypeInfo(typeof(List<SerializablePlayer>)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<List<SerializablePotion>> ListSerializablePotion => _ListSerializablePotion ?? (_ListSerializablePotion = (JsonTypeInfo<List<SerializablePotion>>)Options.GetTypeInfo(typeof(List<SerializablePotion>)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<List<SerializableRelic>> ListSerializableRelic => _ListSerializableRelic ?? (_ListSerializableRelic = (JsonTypeInfo<List<SerializableRelic>>)Options.GetTypeInfo(typeof(List<SerializableRelic>)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<List<SerializableReward>> ListSerializableReward => _ListSerializableReward ?? (_ListSerializableReward = (JsonTypeInfo<List<SerializableReward>>)Options.GetTypeInfo(typeof(List<SerializableReward>)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<List<SerializableEpoch>> ListSerializableEpoch => _ListSerializableEpoch ?? (_ListSerializableEpoch = (JsonTypeInfo<List<SerializableEpoch>>)Options.GetTypeInfo(typeof(List<SerializableEpoch>)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<List<SerializableUnlockedAchievement>> ListSerializableUnlockedAchievement => _ListSerializableUnlockedAchievement ?? (_ListSerializableUnlockedAchievement = (JsonTypeInfo<List<SerializableUnlockedAchievement>>)Options.GetTypeInfo(typeof(List<SerializableUnlockedAchievement>)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<List<Dictionary<string, object>>> ListDictionaryStringObject => _ListDictionaryStringObject ?? (_ListDictionaryStringObject = (JsonTypeInfo<List<Dictionary<string, object>>>)Options.GetTypeInfo(typeof(List<Dictionary<string, object>>)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<List<List<MapPointHistoryEntry>>> ListListMapPointHistoryEntry => _ListListMapPointHistoryEntry ?? (_ListListMapPointHistoryEntry = (JsonTypeInfo<List<List<MapPointHistoryEntry>>>)Options.GetTypeInfo(typeof(List<List<MapPointHistoryEntry>>)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<List<List<PlayerMapPointHistoryEntry>>> ListListPlayerMapPointHistoryEntry => _ListListPlayerMapPointHistoryEntry ?? (_ListListPlayerMapPointHistoryEntry = (JsonTypeInfo<List<List<PlayerMapPointHistoryEntry>>>)Options.GetTypeInfo(typeof(List<List<PlayerMapPointHistoryEntry>>)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<List<JsonNode>> ListJsonNode => _ListJsonNode ?? (_ListJsonNode = (JsonTypeInfo<List<JsonNode>>)Options.GetTypeInfo(typeof(List<JsonNode>)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<List<string>> ListString => _ListString ?? (_ListString = (JsonTypeInfo<List<string>>)Options.GetTypeInfo(typeof(List<string>)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<List<ulong>> ListUInt64 => _ListUInt64 ?? (_ListUInt64 = (JsonTypeInfo<List<ulong>>)Options.GetTypeInfo(typeof(List<ulong>)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<DateTimeOffset> DateTimeOffset => _DateTimeOffset ?? (_DateTimeOffset = (JsonTypeInfo<DateTimeOffset>)Options.GetTypeInfo(typeof(DateTimeOffset)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<DateTimeOffset?> NullableDateTimeOffset => _NullableDateTimeOffset ?? (_NullableDateTimeOffset = (JsonTypeInfo<DateTimeOffset?>)Options.GetTypeInfo(typeof(DateTimeOffset?)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<JsonNode> JsonNode => _JsonNode ?? (_JsonNode = (JsonTypeInfo<JsonNode>)Options.GetTypeInfo(typeof(JsonNode)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<JsonObject> JsonObject => _JsonObject ?? (_JsonObject = (JsonTypeInfo<JsonObject>)Options.GetTypeInfo(typeof(JsonObject)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<int> Int32 => _Int32 ?? (_Int32 = (JsonTypeInfo<int>)Options.GetTypeInfo(typeof(int)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<int?> NullableInt32 => _NullableInt32 ?? (_NullableInt32 = (JsonTypeInfo<int?>)Options.GetTypeInfo(typeof(int?)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<int[]> Int32Array => _Int32Array ?? (_Int32Array = (JsonTypeInfo<int[]>)Options.GetTypeInfo(typeof(int[])));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<long> Int64 => _Int64 ?? (_Int64 = (JsonTypeInfo<long>)Options.GetTypeInfo(typeof(long)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<object> Object => _Object ?? (_Object = (JsonTypeInfo<object>)Options.GetTypeInfo(typeof(object)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<string> String => _String ?? (_String = (JsonTypeInfo<string>)Options.GetTypeInfo(typeof(string)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<ulong> UInt64 => _UInt64 ?? (_UInt64 = (JsonTypeInfo<ulong>)Options.GetTypeInfo(typeof(ulong)));

	/// <summary>
	/// The default <see cref="T:System.Text.Json.Serialization.JsonSerializerContext" /> associated with a default <see cref="T:System.Text.Json.JsonSerializerOptions" /> instance.
	/// </summary>
	public static MegaCritSerializerContext Default { get; } = new MegaCritSerializerContext(new JsonSerializerOptions(s_defaultOptions));

	/// <summary>
	/// The source-generated options associated with this context.
	/// </summary>
	protected override JsonSerializerOptions? GeneratedSerializerOptions { get; } = s_defaultOptions;

	private JsonTypeInfo<bool> Create_Boolean(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<bool> jsonTypeInfo))
		{
			jsonTypeInfo = JsonMetadataServices.CreateValueInfo<bool>(options, JsonMetadataServices.BooleanConverter);
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private JsonTypeInfo<double> Create_Double(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<double> jsonTypeInfo))
		{
			jsonTypeInfo = JsonMetadataServices.CreateValueInfo<double>(options, JsonMetadataServices.DoubleConverter);
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private JsonTypeInfo<float> Create_Single(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<float> jsonTypeInfo))
		{
			jsonTypeInfo = JsonMetadataServices.CreateValueInfo<float>(options, JsonMetadataServices.SingleConverter);
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private JsonTypeInfo<Vector2> Create_Vector2(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<Vector2> jsonTypeInfo))
		{
			JsonObjectInfoValues<Vector2> objectInfo = new JsonObjectInfoValues<Vector2>
			{
				ObjectCreator = () => default,
				ObjectWithParameterizedConstructorCreator = null,
				PropertyMetadataInitializer = (JsonSerializerContext _) => Vector2PropInit(options),
				ConstructorParameterMetadataInitializer = null,
				ConstructorAttributeProviderFactory = () => typeof(Vector2).GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Array.Empty<Type>(), null),
				SerializeHandler = null
			};
			jsonTypeInfo = JsonMetadataServices.CreateObjectInfo(options, objectInfo);
			jsonTypeInfo.NumberHandling = null;
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private static JsonPropertyInfo[] Vector2PropInit(JsonSerializerOptions options)
	{
		JsonPropertyInfo[] array = new JsonPropertyInfo[2];
		JsonPropertyInfoValues<float> propertyInfo = new JsonPropertyInfoValues<float>
		{
			IsProperty = false,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(Vector2),
			Converter = null,
			Getter = (object obj) => ((Vector2)obj).X,
			Setter = (object obj, float value) =>
			{
				Unsafe.Unbox<Vector2>(obj).X = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "X",
			JsonPropertyName = null,
			AttributeProviderFactory = () => typeof(Vector2).GetField("X", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
		};
		array[0] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo);
		JsonPropertyInfoValues<float> propertyInfo2 = new JsonPropertyInfoValues<float>
		{
			IsProperty = false,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(Vector2),
			Converter = null,
			Getter = (object obj) => ((Vector2)obj).Y,
			Setter = (object obj, float value) =>
			{
				Unsafe.Unbox<Vector2>(obj).Y = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "Y",
			JsonPropertyName = null,
			AttributeProviderFactory = () => typeof(Vector2).GetField("Y", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
		};
		array[1] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo2);
		return array;
	}

	private JsonTypeInfo<Vector2I> Create_Vector2I(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<Vector2I> jsonTypeInfo))
		{
			JsonObjectInfoValues<Vector2I> objectInfo = new JsonObjectInfoValues<Vector2I>
			{
				ObjectCreator = () => default,
				ObjectWithParameterizedConstructorCreator = null,
				PropertyMetadataInitializer = (JsonSerializerContext _) => Vector2IPropInit(options),
				ConstructorParameterMetadataInitializer = null,
				ConstructorAttributeProviderFactory = () => typeof(Vector2I).GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Array.Empty<Type>(), null),
				SerializeHandler = null
			};
			jsonTypeInfo = JsonMetadataServices.CreateObjectInfo(options, objectInfo);
			jsonTypeInfo.NumberHandling = null;
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private static JsonPropertyInfo[] Vector2IPropInit(JsonSerializerOptions options)
	{
		JsonPropertyInfo[] array = new JsonPropertyInfo[2];
		JsonPropertyInfoValues<int> propertyInfo = new JsonPropertyInfoValues<int>
		{
			IsProperty = false,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(Vector2I),
			Converter = null,
			Getter = (object obj) => ((Vector2I)obj).X,
			Setter = (object obj, int value) =>
			{
				Unsafe.Unbox<Vector2I>(obj).X = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "X",
			JsonPropertyName = null,
			AttributeProviderFactory = () => typeof(Vector2I).GetField("X", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
		};
		array[0] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo);
		JsonPropertyInfoValues<int> propertyInfo2 = new JsonPropertyInfoValues<int>
		{
			IsProperty = false,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(Vector2I),
			Converter = null,
			Getter = (object obj) => ((Vector2I)obj).Y,
			Setter = (object obj, int value) =>
			{
				Unsafe.Unbox<Vector2I>(obj).Y = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "Y",
			JsonPropertyName = null,
			AttributeProviderFactory = () => typeof(Vector2I).GetField("Y", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
		};
		array[1] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo2);
		return array;
	}

	private JsonTypeInfo<ControllerMappingType> Create_ControllerMappingType(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<ControllerMappingType> jsonTypeInfo))
		{
			jsonTypeInfo = JsonMetadataServices.CreateValueInfo<ControllerMappingType>(options, JsonMetadataServices.GetEnumConverter<ControllerMappingType>(options));
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private JsonTypeInfo<RelicRarity> Create_RelicRarity(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<RelicRarity> jsonTypeInfo))
		{
			jsonTypeInfo = JsonMetadataServices.CreateValueInfo<RelicRarity>(options, JsonMetadataServices.GetEnumConverter<RelicRarity>(options));
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private JsonTypeInfo<PlayerRngType> Create_PlayerRngType(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<PlayerRngType> jsonTypeInfo))
		{
			jsonTypeInfo = JsonMetadataServices.CreateValueInfo<PlayerRngType>(options, JsonMetadataServices.GetEnumConverter<PlayerRngType>(options));
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private JsonTypeInfo<RunRngType> Create_RunRngType(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<RunRngType> jsonTypeInfo))
		{
			jsonTypeInfo = JsonMetadataServices.CreateValueInfo<RunRngType>(options, JsonMetadataServices.GetEnumConverter<RunRngType>(options));
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private JsonTypeInfo<LocString> Create_LocString(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<LocString> jsonTypeInfo))
		{
			JsonObjectInfoValues<LocString> objectInfo = new JsonObjectInfoValues<LocString>
			{
				ObjectCreator = null,
				ObjectWithParameterizedConstructorCreator = (object[] args) => new LocString((string)args[0], (string)args[1]),
				PropertyMetadataInitializer = (JsonSerializerContext _) => LocStringPropInit(options),
				ConstructorParameterMetadataInitializer = LocStringCtorParamInit,
				ConstructorAttributeProviderFactory = () => typeof(LocString).GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[2]
				{
					typeof(string),
					typeof(string)
				}, null),
				SerializeHandler = null
			};
			jsonTypeInfo = JsonMetadataServices.CreateObjectInfo(options, objectInfo);
			jsonTypeInfo.NumberHandling = null;
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private static JsonPropertyInfo[] LocStringPropInit(JsonSerializerOptions options)
	{
		JsonPropertyInfo[] array = new JsonPropertyInfo[4];
		JsonPropertyInfoValues<string> propertyInfo = new JsonPropertyInfoValues<string>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(LocString),
			Converter = null,
			Getter = (object obj) => ((LocString)obj).LocTable,
			Setter = null,
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "LocTable",
			JsonPropertyName = "table",
			AttributeProviderFactory = () => typeof(LocString).GetProperty("LocTable", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(string), Array.Empty<Type>(), null)
		};
		array[0] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo);
		array[0].IsGetNullable = false;
		JsonPropertyInfoValues<string> propertyInfo2 = new JsonPropertyInfoValues<string>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(LocString),
			Converter = null,
			Getter = (object obj) => ((LocString)obj).LocEntryKey,
			Setter = null,
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "LocEntryKey",
			JsonPropertyName = "key",
			AttributeProviderFactory = () => typeof(LocString).GetProperty("LocEntryKey", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(string), Array.Empty<Type>(), null)
		};
		array[1] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo2);
		array[1].IsGetNullable = false;
		JsonPropertyInfoValues<bool> propertyInfo3 = new JsonPropertyInfoValues<bool>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(LocString),
			Converter = null,
			Getter = null,
			Setter = null,
			IgnoreCondition = JsonIgnoreCondition.Always,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "IsEmpty",
			JsonPropertyName = null,
			AttributeProviderFactory = () => typeof(LocString).GetProperty("IsEmpty", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(bool), Array.Empty<Type>(), null)
		};
		array[2] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo3);
		JsonPropertyInfoValues<IReadOnlyDictionary<string, object>> propertyInfo4 = new JsonPropertyInfoValues<IReadOnlyDictionary<string, object>>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(LocString),
			Converter = null,
			Getter = null,
			Setter = null,
			IgnoreCondition = JsonIgnoreCondition.Always,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "Variables",
			JsonPropertyName = null,
			AttributeProviderFactory = () => typeof(LocString).GetProperty("Variables", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(IReadOnlyDictionary<string, object>), Array.Empty<Type>(), null)
		};
		array[3] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo4);
		array[3].IsGetNullable = false;
		return array;
	}

	private static JsonParameterInfoValues[] LocStringCtorParamInit()
	{
		return new JsonParameterInfoValues[2]
		{
			new JsonParameterInfoValues
			{
				Name = "locTable",
				ParameterType = typeof(string),
				Position = 0,
				HasDefaultValue = false,
				DefaultValue = null,
				IsNullable = false
			},
			new JsonParameterInfoValues
			{
				Name = "locEntryKey",
				ParameterType = typeof(string),
				Position = 1,
				HasDefaultValue = false,
				DefaultValue = null,
				IsNullable = false
			}
		};
	}

	private JsonTypeInfo<MapCoord> Create_MapCoord(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<MapCoord> jsonTypeInfo))
		{
			JsonObjectInfoValues<MapCoord> objectInfo = new JsonObjectInfoValues<MapCoord>
			{
				ObjectCreator = () => default,
				ObjectWithParameterizedConstructorCreator = null,
				PropertyMetadataInitializer = (JsonSerializerContext _) => MapCoordPropInit(options),
				ConstructorParameterMetadataInitializer = null,
				ConstructorAttributeProviderFactory = () => typeof(MapCoord).GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Array.Empty<Type>(), null),
				SerializeHandler = null
			};
			jsonTypeInfo = JsonMetadataServices.CreateObjectInfo(options, objectInfo);
			jsonTypeInfo.NumberHandling = null;
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private static JsonPropertyInfo[] MapCoordPropInit(JsonSerializerOptions options)
	{
		JsonPropertyInfo[] array = new JsonPropertyInfo[2];
		JsonPropertyInfoValues<int> propertyInfo = new JsonPropertyInfoValues<int>
		{
			IsProperty = false,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(MapCoord),
			Converter = null,
			Getter = (object obj) => ((MapCoord)obj).col,
			Setter = (object obj, int value) =>
			{
				Unsafe.Unbox<MapCoord>(obj).col = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = true,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "col",
			JsonPropertyName = null,
			AttributeProviderFactory = () => typeof(MapCoord).GetField("col", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
		};
		array[0] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo);
		JsonPropertyInfoValues<int> propertyInfo2 = new JsonPropertyInfoValues<int>
		{
			IsProperty = false,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(MapCoord),
			Converter = null,
			Getter = (object obj) => ((MapCoord)obj).row,
			Setter = (object obj, int value) =>
			{
				Unsafe.Unbox<MapCoord>(obj).row = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = true,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "row",
			JsonPropertyName = null,
			AttributeProviderFactory = () => typeof(MapCoord).GetField("row", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
		};
		array[1] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo2);
		return array;
	}

	private JsonTypeInfo<MapPointType> Create_MapPointType(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<MapPointType> jsonTypeInfo))
		{
			jsonTypeInfo = JsonMetadataServices.CreateValueInfo<MapPointType>(options, JsonMetadataServices.GetEnumConverter<MapPointType>(options));
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private JsonTypeInfo<ModDependency> Create_ModDependency(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<ModDependency> jsonTypeInfo))
		{
			JsonObjectInfoValues<ModDependency> objectInfo = new JsonObjectInfoValues<ModDependency>
			{
				ObjectCreator = null,
				ObjectWithParameterizedConstructorCreator = (object[] args) => new ModDependency((string)args[0], (string)args[1]),
				PropertyMetadataInitializer = (JsonSerializerContext _) => ModDependencyPropInit(options),
				ConstructorParameterMetadataInitializer = ModDependencyCtorParamInit,
				ConstructorAttributeProviderFactory = () => typeof(ModDependency).GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[2]
				{
					typeof(string),
					typeof(string)
				}, null),
				SerializeHandler = null
			};
			jsonTypeInfo = JsonMetadataServices.CreateObjectInfo(options, objectInfo);
			jsonTypeInfo.NumberHandling = null;
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private static JsonPropertyInfo[] ModDependencyPropInit(JsonSerializerOptions options)
	{
		JsonPropertyInfo[] array = new JsonPropertyInfo[2];
		JsonPropertyInfoValues<string> propertyInfo = new JsonPropertyInfoValues<string>
		{
			IsProperty = false,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(ModDependency),
			Converter = null,
			Getter = (object obj) => ((ModDependency)obj).id,
			Setter = (object obj, string? value) =>
			{
				((ModDependency)obj).id = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "id",
			JsonPropertyName = "id",
			AttributeProviderFactory = () => typeof(ModDependency).GetField("id", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
		};
		array[0] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo);
		array[0].IsGetNullable = false;
		array[0].IsSetNullable = false;
		JsonPropertyInfoValues<string> propertyInfo2 = new JsonPropertyInfoValues<string>
		{
			IsProperty = false,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(ModDependency),
			Converter = null,
			Getter = (object obj) => ((ModDependency)obj).minVersion,
			Setter = (object obj, string? value) =>
			{
				((ModDependency)obj).minVersion = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "minVersion",
			JsonPropertyName = "min_version",
			AttributeProviderFactory = () => typeof(ModDependency).GetField("minVersion", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
		};
		array[1] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo2);
		return array;
	}

	private static JsonParameterInfoValues[] ModDependencyCtorParamInit()
	{
		return new JsonParameterInfoValues[2]
		{
			new JsonParameterInfoValues
			{
				Name = "id",
				ParameterType = typeof(string),
				Position = 0,
				HasDefaultValue = false,
				DefaultValue = null,
				IsNullable = false
			},
			new JsonParameterInfoValues
			{
				Name = "minVersion",
				ParameterType = typeof(string),
				Position = 1,
				HasDefaultValue = true,
				DefaultValue = null,
				IsNullable = true
			}
		};
	}

	private JsonTypeInfo<ModManifest> Create_ModManifest(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<ModManifest> jsonTypeInfo))
		{
			JsonObjectInfoValues<ModManifest> objectInfo = new JsonObjectInfoValues<ModManifest>
			{
				ObjectCreator = () => new ModManifest(),
				ObjectWithParameterizedConstructorCreator = null,
				PropertyMetadataInitializer = (JsonSerializerContext _) => ModManifestPropInit(options),
				ConstructorParameterMetadataInitializer = null,
				ConstructorAttributeProviderFactory = () => typeof(ModManifest).GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Array.Empty<Type>(), null),
				SerializeHandler = null
			};
			jsonTypeInfo = JsonMetadataServices.CreateObjectInfo(options, objectInfo);
			jsonTypeInfo.NumberHandling = null;
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private static JsonPropertyInfo[] ModManifestPropInit(JsonSerializerOptions options)
	{
		JsonPropertyInfo[] array = new JsonPropertyInfo[10];
		JsonPropertyInfoValues<string> propertyInfo = new JsonPropertyInfoValues<string>
		{
			IsProperty = false,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(ModManifest),
			Converter = null,
			Getter = (object obj) => ((ModManifest)obj).id,
			Setter = (object obj, string? value) =>
			{
				((ModManifest)obj).id = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "id",
			JsonPropertyName = "id",
			AttributeProviderFactory = () => typeof(ModManifest).GetField("id", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
		};
		array[0] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo);
		JsonPropertyInfoValues<string> propertyInfo2 = new JsonPropertyInfoValues<string>
		{
			IsProperty = false,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(ModManifest),
			Converter = null,
			Getter = (object obj) => ((ModManifest)obj).name,
			Setter = (object obj, string? value) =>
			{
				((ModManifest)obj).name = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "name",
			JsonPropertyName = "name",
			AttributeProviderFactory = () => typeof(ModManifest).GetField("name", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
		};
		array[1] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo2);
		JsonPropertyInfoValues<string> propertyInfo3 = new JsonPropertyInfoValues<string>
		{
			IsProperty = false,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(ModManifest),
			Converter = null,
			Getter = (object obj) => ((ModManifest)obj).author,
			Setter = (object obj, string? value) =>
			{
				((ModManifest)obj).author = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "author",
			JsonPropertyName = "author",
			AttributeProviderFactory = () => typeof(ModManifest).GetField("author", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
		};
		array[2] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo3);
		JsonPropertyInfoValues<string> propertyInfo4 = new JsonPropertyInfoValues<string>
		{
			IsProperty = false,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(ModManifest),
			Converter = null,
			Getter = (object obj) => ((ModManifest)obj).description,
			Setter = (object obj, string? value) =>
			{
				((ModManifest)obj).description = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "description",
			JsonPropertyName = "description",
			AttributeProviderFactory = () => typeof(ModManifest).GetField("description", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
		};
		array[3] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo4);
		JsonPropertyInfoValues<string> propertyInfo5 = new JsonPropertyInfoValues<string>
		{
			IsProperty = false,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(ModManifest),
			Converter = null,
			Getter = (object obj) => ((ModManifest)obj).version,
			Setter = (object obj, string? value) =>
			{
				((ModManifest)obj).version = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "version",
			JsonPropertyName = "version",
			AttributeProviderFactory = () => typeof(ModManifest).GetField("version", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
		};
		array[4] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo5);
		JsonPropertyInfoValues<bool> propertyInfo6 = new JsonPropertyInfoValues<bool>
		{
			IsProperty = false,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(ModManifest),
			Converter = null,
			Getter = (object obj) => ((ModManifest)obj).hasPck,
			Setter = (object obj, bool value) =>
			{
				((ModManifest)obj).hasPck = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "hasPck",
			JsonPropertyName = "has_pck",
			AttributeProviderFactory = () => typeof(ModManifest).GetField("hasPck", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
		};
		array[5] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo6);
		JsonPropertyInfoValues<bool> propertyInfo7 = new JsonPropertyInfoValues<bool>
		{
			IsProperty = false,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(ModManifest),
			Converter = null,
			Getter = (object obj) => ((ModManifest)obj).hasDll,
			Setter = (object obj, bool value) =>
			{
				((ModManifest)obj).hasDll = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "hasDll",
			JsonPropertyName = "has_dll",
			AttributeProviderFactory = () => typeof(ModManifest).GetField("hasDll", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
		};
		array[6] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo7);
		JsonPropertyInfoValues<List<ModDependency>> propertyInfo8 = new JsonPropertyInfoValues<List<ModDependency>>
		{
			IsProperty = false,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(ModManifest),
			Converter = null,
			Getter = (object obj) => ((ModManifest)obj).dependencies,
			Setter = (object obj, List<ModDependency>? value) =>
			{
				((ModManifest)obj).dependencies = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "dependencies",
			JsonPropertyName = "dependencies",
			AttributeProviderFactory = () => typeof(ModManifest).GetField("dependencies", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
		};
		array[7] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo8);
		JsonPropertyInfoValues<bool> propertyInfo9 = new JsonPropertyInfoValues<bool>
		{
			IsProperty = false,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(ModManifest),
			Converter = null,
			Getter = (object obj) => ((ModManifest)obj).affectsGameplay,
			Setter = (object obj, bool value) =>
			{
				((ModManifest)obj).affectsGameplay = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "affectsGameplay",
			JsonPropertyName = "affects_gameplay",
			AttributeProviderFactory = () => typeof(ModManifest).GetField("affectsGameplay", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
		};
		array[8] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo9);
		JsonPropertyInfoValues<string> propertyInfo10 = new JsonPropertyInfoValues<string>
		{
			IsProperty = false,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(ModManifest),
			Converter = null,
			Getter = (object obj) => ((ModManifest)obj).minGameVersion,
			Setter = (object obj, string? value) =>
			{
				((ModManifest)obj).minGameVersion = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "minGameVersion",
			JsonPropertyName = "min_game_version",
			AttributeProviderFactory = () => typeof(ModManifest).GetField("minGameVersion", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
		};
		array[9] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo10);
		return array;
	}

	private JsonTypeInfo<ModSettings> Create_ModSettings(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<ModSettings> jsonTypeInfo))
		{
			JsonObjectInfoValues<ModSettings> objectInfo = new JsonObjectInfoValues<ModSettings>
			{
				ObjectCreator = () => new ModSettings(),
				ObjectWithParameterizedConstructorCreator = null,
				PropertyMetadataInitializer = (JsonSerializerContext _) => ModSettingsPropInit(options),
				ConstructorParameterMetadataInitializer = null,
				ConstructorAttributeProviderFactory = () => typeof(ModSettings).GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Array.Empty<Type>(), null),
				SerializeHandler = null
			};
			jsonTypeInfo = JsonMetadataServices.CreateObjectInfo(options, objectInfo);
			jsonTypeInfo.NumberHandling = null;
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private static JsonPropertyInfo[] ModSettingsPropInit(JsonSerializerOptions options)
	{
		JsonPropertyInfo[] array = new JsonPropertyInfo[2];
		JsonPropertyInfoValues<bool> propertyInfo = new JsonPropertyInfoValues<bool>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(ModSettings),
			Converter = null,
			Getter = (object obj) => ((ModSettings)obj).PlayerAgreedToModLoading,
			Setter = (object obj, bool value) =>
			{
				((ModSettings)obj).PlayerAgreedToModLoading = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "PlayerAgreedToModLoading",
			JsonPropertyName = "mods_enabled",
			AttributeProviderFactory = () => typeof(ModSettings).GetProperty("PlayerAgreedToModLoading", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(bool), Array.Empty<Type>(), null)
		};
		array[0] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo);
		JsonPropertyInfoValues<List<SettingsSaveMod>> propertyInfo2 = new JsonPropertyInfoValues<List<SettingsSaveMod>>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(ModSettings),
			Converter = null,
			Getter = (object obj) => ((ModSettings)obj).ModList,
			Setter = (object obj, List<SettingsSaveMod>? value) =>
			{
				((ModSettings)obj).ModList = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "ModList",
			JsonPropertyName = "mod_list",
			AttributeProviderFactory = () => typeof(ModSettings).GetProperty("ModList", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(List<SettingsSaveMod>), Array.Empty<Type>(), null)
		};
		array[1] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo2);
		array[1].IsGetNullable = false;
		array[1].IsSetNullable = false;
		return array;
	}

	private JsonTypeInfo<ModSource> Create_ModSource(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<ModSource> jsonTypeInfo))
		{
			jsonTypeInfo = JsonMetadataServices.CreateValueInfo<ModSource>(options, JsonMetadataServices.GetEnumConverter<ModSource>(options));
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private JsonTypeInfo<SettingsSaveMod> Create_SettingsSaveMod(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<SettingsSaveMod> jsonTypeInfo))
		{
			JsonObjectInfoValues<SettingsSaveMod> objectInfo = new JsonObjectInfoValues<SettingsSaveMod>
			{
				ObjectCreator = () => new SettingsSaveMod(),
				ObjectWithParameterizedConstructorCreator = null,
				PropertyMetadataInitializer = (JsonSerializerContext _) => SettingsSaveModPropInit(options),
				ConstructorParameterMetadataInitializer = null,
				ConstructorAttributeProviderFactory = () => typeof(SettingsSaveMod).GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Array.Empty<Type>(), null),
				SerializeHandler = null
			};
			jsonTypeInfo = JsonMetadataServices.CreateObjectInfo(options, objectInfo);
			jsonTypeInfo.NumberHandling = null;
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private static JsonPropertyInfo[] SettingsSaveModPropInit(JsonSerializerOptions options)
	{
		JsonPropertyInfo[] array = new JsonPropertyInfo[3];
		JsonPropertyInfoValues<string> propertyInfo = new JsonPropertyInfoValues<string>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SettingsSaveMod),
			Converter = null,
			Getter = (object obj) => ((SettingsSaveMod)obj).Id,
			Setter = (object obj, string? value) =>
			{
				((SettingsSaveMod)obj).Id = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "Id",
			JsonPropertyName = "id",
			AttributeProviderFactory = () => typeof(SettingsSaveMod).GetProperty("Id", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(string), Array.Empty<Type>(), null)
		};
		array[0] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo);
		array[0].IsGetNullable = false;
		array[0].IsSetNullable = false;
		JsonPropertyInfoValues<ModSource> propertyInfo2 = new JsonPropertyInfoValues<ModSource>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SettingsSaveMod),
			Converter = null,
			Getter = (object obj) => ((SettingsSaveMod)obj).Source,
			Setter = (object obj, ModSource value) =>
			{
				((SettingsSaveMod)obj).Source = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "Source",
			JsonPropertyName = "source",
			AttributeProviderFactory = () => typeof(SettingsSaveMod).GetProperty("Source", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(ModSource), Array.Empty<Type>(), null)
		};
		array[1] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo2);
		JsonPropertyInfoValues<bool> propertyInfo3 = new JsonPropertyInfoValues<bool>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SettingsSaveMod),
			Converter = null,
			Getter = (object obj) => ((SettingsSaveMod)obj).IsEnabled,
			Setter = (object obj, bool value) =>
			{
				((SettingsSaveMod)obj).IsEnabled = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "IsEnabled",
			JsonPropertyName = "is_enabled",
			AttributeProviderFactory = () => typeof(SettingsSaveMod).GetProperty("IsEnabled", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(bool), Array.Empty<Type>(), null)
		};
		array[2] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo3);
		return array;
	}

	private JsonTypeInfo<BadgeRarity> Create_BadgeRarity(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<BadgeRarity> jsonTypeInfo))
		{
			jsonTypeInfo = JsonMetadataServices.CreateValueInfo<BadgeRarity>(options, JsonMetadataServices.GetEnumConverter<BadgeRarity>(options));
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private JsonTypeInfo<ModelId> Create_ModelId(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<ModelId> jsonTypeInfo))
		{
			JsonObjectInfoValues<ModelId> objectInfo = new JsonObjectInfoValues<ModelId>
			{
				ObjectCreator = null,
				ObjectWithParameterizedConstructorCreator = (object[] args) => new ModelId((string)args[0], (string)args[1]),
				PropertyMetadataInitializer = (JsonSerializerContext _) => ModelIdPropInit(options),
				ConstructorParameterMetadataInitializer = ModelIdCtorParamInit,
				ConstructorAttributeProviderFactory = () => typeof(ModelId).GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[2]
				{
					typeof(string),
					typeof(string)
				}, null),
				SerializeHandler = null
			};
			jsonTypeInfo = JsonMetadataServices.CreateObjectInfo(options, objectInfo);
			jsonTypeInfo.NumberHandling = null;
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private static JsonPropertyInfo[] ModelIdPropInit(JsonSerializerOptions options)
	{
		JsonPropertyInfo[] array = new JsonPropertyInfo[2];
		JsonPropertyInfoValues<string> propertyInfo = new JsonPropertyInfoValues<string>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(ModelId),
			Converter = null,
			Getter = (object obj) => ((ModelId)obj).Category,
			Setter = null,
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "Category",
			JsonPropertyName = null,
			AttributeProviderFactory = () => typeof(ModelId).GetProperty("Category", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(string), Array.Empty<Type>(), null)
		};
		array[0] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo);
		array[0].IsGetNullable = false;
		JsonPropertyInfoValues<string> propertyInfo2 = new JsonPropertyInfoValues<string>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(ModelId),
			Converter = null,
			Getter = (object obj) => ((ModelId)obj).Entry,
			Setter = null,
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "Entry",
			JsonPropertyName = null,
			AttributeProviderFactory = () => typeof(ModelId).GetProperty("Entry", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(string), Array.Empty<Type>(), null)
		};
		array[1] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo2);
		array[1].IsGetNullable = false;
		return array;
	}

	private static JsonParameterInfoValues[] ModelIdCtorParamInit()
	{
		return new JsonParameterInfoValues[2]
		{
			new JsonParameterInfoValues
			{
				Name = "category",
				ParameterType = typeof(string),
				Position = 0,
				HasDefaultValue = false,
				DefaultValue = null,
				IsNullable = false
			},
			new JsonParameterInfoValues
			{
				Name = "entry",
				ParameterType = typeof(string),
				Position = 1,
				HasDefaultValue = false,
				DefaultValue = null,
				IsNullable = false
			}
		};
	}

	private JsonTypeInfo<FeedbackData> Create_FeedbackData(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<FeedbackData> jsonTypeInfo))
		{
			JsonObjectInfoValues<FeedbackData> objectInfo = new JsonObjectInfoValues<FeedbackData>
			{
				ObjectCreator = () => default,
				ObjectWithParameterizedConstructorCreator = null,
				PropertyMetadataInitializer = (JsonSerializerContext _) => FeedbackDataPropInit(options),
				ConstructorParameterMetadataInitializer = null,
				ConstructorAttributeProviderFactory = () => typeof(FeedbackData).GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Array.Empty<Type>(), null),
				SerializeHandler = null
			};
			jsonTypeInfo = JsonMetadataServices.CreateObjectInfo(options, objectInfo);
			jsonTypeInfo.NumberHandling = null;
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private static JsonPropertyInfo[] FeedbackDataPropInit(JsonSerializerOptions options)
	{
		JsonPropertyInfo[] array = new JsonPropertyInfo[10];
		JsonPropertyInfoValues<string> propertyInfo = new JsonPropertyInfoValues<string>
		{
			IsProperty = false,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(FeedbackData),
			Converter = null,
			Getter = (object obj) => ((FeedbackData)obj).description,
			Setter = (object obj, string? value) =>
			{
				Unsafe.Unbox<FeedbackData>(obj).description = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "description",
			JsonPropertyName = "description",
			AttributeProviderFactory = () => typeof(FeedbackData).GetField("description", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
		};
		array[0] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo);
		array[0].IsGetNullable = false;
		array[0].IsSetNullable = false;
		JsonPropertyInfoValues<string> propertyInfo2 = new JsonPropertyInfoValues<string>
		{
			IsProperty = false,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(FeedbackData),
			Converter = null,
			Getter = (object obj) => ((FeedbackData)obj).category,
			Setter = (object obj, string? value) =>
			{
				Unsafe.Unbox<FeedbackData>(obj).category = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "category",
			JsonPropertyName = "category",
			AttributeProviderFactory = () => typeof(FeedbackData).GetField("category", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
		};
		array[1] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo2);
		array[1].IsGetNullable = false;
		array[1].IsSetNullable = false;
		JsonPropertyInfoValues<string> propertyInfo3 = new JsonPropertyInfoValues<string>
		{
			IsProperty = false,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(FeedbackData),
			Converter = null,
			Getter = (object obj) => ((FeedbackData)obj).gameVersion,
			Setter = (object obj, string? value) =>
			{
				Unsafe.Unbox<FeedbackData>(obj).gameVersion = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "gameVersion",
			JsonPropertyName = "game_version",
			AttributeProviderFactory = () => typeof(FeedbackData).GetField("gameVersion", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
		};
		array[2] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo3);
		array[2].IsGetNullable = false;
		array[2].IsSetNullable = false;
		JsonPropertyInfoValues<string> propertyInfo4 = new JsonPropertyInfoValues<string>
		{
			IsProperty = false,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(FeedbackData),
			Converter = null,
			Getter = (object obj) => ((FeedbackData)obj).uniqueId,
			Setter = (object obj, string? value) =>
			{
				Unsafe.Unbox<FeedbackData>(obj).uniqueId = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "uniqueId",
			JsonPropertyName = "unique_id",
			AttributeProviderFactory = () => typeof(FeedbackData).GetField("uniqueId", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
		};
		array[3] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo4);
		array[3].IsGetNullable = false;
		array[3].IsSetNullable = false;
		JsonPropertyInfoValues<string> propertyInfo5 = new JsonPropertyInfoValues<string>
		{
			IsProperty = false,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(FeedbackData),
			Converter = null,
			Getter = (object obj) => ((FeedbackData)obj).commit,
			Setter = (object obj, string? value) =>
			{
				Unsafe.Unbox<FeedbackData>(obj).commit = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "commit",
			JsonPropertyName = "commit",
			AttributeProviderFactory = () => typeof(FeedbackData).GetField("commit", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
		};
		array[4] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo5);
		array[4].IsGetNullable = false;
		array[4].IsSetNullable = false;
		JsonPropertyInfoValues<string> propertyInfo6 = new JsonPropertyInfoValues<string>
		{
			IsProperty = false,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(FeedbackData),
			Converter = null,
			Getter = (object obj) => ((FeedbackData)obj).platformBranch,
			Setter = (object obj, string? value) =>
			{
				Unsafe.Unbox<FeedbackData>(obj).platformBranch = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "platformBranch",
			JsonPropertyName = "platform_branch",
			AttributeProviderFactory = () => typeof(FeedbackData).GetField("platformBranch", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
		};
		array[5] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo6);
		JsonPropertyInfoValues<string> propertyInfo7 = new JsonPropertyInfoValues<string>
		{
			IsProperty = false,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(FeedbackData),
			Converter = null,
			Getter = (object obj) => ((FeedbackData)obj).sessionId,
			Setter = (object obj, string? value) =>
			{
				Unsafe.Unbox<FeedbackData>(obj).sessionId = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "sessionId",
			JsonPropertyName = "session_id",
			AttributeProviderFactory = () => typeof(FeedbackData).GetField("sessionId", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
		};
		array[6] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo7);
		array[6].IsGetNullable = false;
		array[6].IsSetNullable = false;
		JsonPropertyInfoValues<bool> propertyInfo8 = new JsonPropertyInfoValues<bool>
		{
			IsProperty = false,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(FeedbackData),
			Converter = null,
			Getter = (object obj) => ((FeedbackData)obj).isModded,
			Setter = (object obj, bool value) =>
			{
				Unsafe.Unbox<FeedbackData>(obj).isModded = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "isModded",
			JsonPropertyName = "is_modded",
			AttributeProviderFactory = () => typeof(FeedbackData).GetField("isModded", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
		};
		array[7] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo8);
		JsonPropertyInfoValues<bool> propertyInfo9 = new JsonPropertyInfoValues<bool>
		{
			IsProperty = false,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(FeedbackData),
			Converter = null,
			Getter = (object obj) => ((FeedbackData)obj).isFullConsole,
			Setter = (object obj, bool value) =>
			{
				Unsafe.Unbox<FeedbackData>(obj).isFullConsole = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "isFullConsole",
			JsonPropertyName = "is_full_console",
			AttributeProviderFactory = () => typeof(FeedbackData).GetField("isFullConsole", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
		};
		array[8] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo9);
		JsonPropertyInfoValues<string> propertyInfo10 = new JsonPropertyInfoValues<string>
		{
			IsProperty = false,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(FeedbackData),
			Converter = null,
			Getter = (object obj) => ((FeedbackData)obj).lang,
			Setter = (object obj, string? value) =>
			{
				Unsafe.Unbox<FeedbackData>(obj).lang = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "lang",
			JsonPropertyName = "lang",
			AttributeProviderFactory = () => typeof(FeedbackData).GetField("lang", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
		};
		array[9] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo10);
		array[9].IsGetNullable = false;
		array[9].IsSetNullable = false;
		return array;
	}

	private JsonTypeInfo<NullLeaderboard> Create_NullLeaderboard(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<NullLeaderboard> jsonTypeInfo))
		{
			JsonObjectInfoValues<NullLeaderboard> objectInfo = new JsonObjectInfoValues<NullLeaderboard>
			{
				ObjectCreator = () => new NullLeaderboard(),
				ObjectWithParameterizedConstructorCreator = null,
				PropertyMetadataInitializer = (JsonSerializerContext _) => NullLeaderboardPropInit(options),
				ConstructorParameterMetadataInitializer = null,
				ConstructorAttributeProviderFactory = () => typeof(NullLeaderboard).GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Array.Empty<Type>(), null),
				SerializeHandler = null
			};
			jsonTypeInfo = JsonMetadataServices.CreateObjectInfo(options, objectInfo);
			jsonTypeInfo.NumberHandling = null;
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private static JsonPropertyInfo[] NullLeaderboardPropInit(JsonSerializerOptions options)
	{
		JsonPropertyInfo[] array = new JsonPropertyInfo[2];
		JsonPropertyInfoValues<string> propertyInfo = new JsonPropertyInfoValues<string>
		{
			IsProperty = false,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(NullLeaderboard),
			Converter = null,
			Getter = (object obj) => ((NullLeaderboard)obj).name,
			Setter = (object obj, string? value) =>
			{
				((NullLeaderboard)obj).name = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "name",
			JsonPropertyName = "name",
			AttributeProviderFactory = () => typeof(NullLeaderboard).GetField("name", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
		};
		array[0] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo);
		array[0].IsGetNullable = false;
		array[0].IsSetNullable = false;
		JsonPropertyInfoValues<List<NullLeaderboardFileEntry>> propertyInfo2 = new JsonPropertyInfoValues<List<NullLeaderboardFileEntry>>
		{
			IsProperty = false,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(NullLeaderboard),
			Converter = null,
			Getter = (object obj) => ((NullLeaderboard)obj).entries,
			Setter = (object obj, List<NullLeaderboardFileEntry>? value) =>
			{
				((NullLeaderboard)obj).entries = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "entries",
			JsonPropertyName = "entries",
			AttributeProviderFactory = () => typeof(NullLeaderboard).GetField("entries", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
		};
		array[1] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo2);
		array[1].IsGetNullable = false;
		array[1].IsSetNullable = false;
		return array;
	}

	private JsonTypeInfo<NullLeaderboardFile> Create_NullLeaderboardFile(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<NullLeaderboardFile> jsonTypeInfo))
		{
			JsonObjectInfoValues<NullLeaderboardFile> objectInfo = new JsonObjectInfoValues<NullLeaderboardFile>
			{
				ObjectCreator = () => new NullLeaderboardFile(),
				ObjectWithParameterizedConstructorCreator = null,
				PropertyMetadataInitializer = (JsonSerializerContext _) => NullLeaderboardFilePropInit(options),
				ConstructorParameterMetadataInitializer = null,
				ConstructorAttributeProviderFactory = () => typeof(NullLeaderboardFile).GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Array.Empty<Type>(), null),
				SerializeHandler = null
			};
			jsonTypeInfo = JsonMetadataServices.CreateObjectInfo(options, objectInfo);
			jsonTypeInfo.NumberHandling = null;
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private static JsonPropertyInfo[] NullLeaderboardFilePropInit(JsonSerializerOptions options)
	{
		JsonPropertyInfo[] array = new JsonPropertyInfo[2];
		JsonPropertyInfoValues<int> propertyInfo = new JsonPropertyInfoValues<int>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(NullLeaderboardFile),
			Converter = null,
			Getter = (object obj) => ((NullLeaderboardFile)obj).SchemaVersion,
			Setter = (object obj, int value) =>
			{
				((NullLeaderboardFile)obj).SchemaVersion = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "SchemaVersion",
			JsonPropertyName = "version",
			AttributeProviderFactory = () => typeof(NullLeaderboardFile).GetProperty("SchemaVersion", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(int), Array.Empty<Type>(), null)
		};
		array[0] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo);
		JsonPropertyInfoValues<List<NullLeaderboard>> propertyInfo2 = new JsonPropertyInfoValues<List<NullLeaderboard>>
		{
			IsProperty = false,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(NullLeaderboardFile),
			Converter = null,
			Getter = (object obj) => ((NullLeaderboardFile)obj).leaderboards,
			Setter = (object obj, List<NullLeaderboard>? value) =>
			{
				((NullLeaderboardFile)obj).leaderboards = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "leaderboards",
			JsonPropertyName = "leaderboards",
			AttributeProviderFactory = () => typeof(NullLeaderboardFile).GetField("leaderboards", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
		};
		array[1] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo2);
		array[1].IsGetNullable = false;
		array[1].IsSetNullable = false;
		return array;
	}

	private JsonTypeInfo<NullLeaderboardFileEntry> Create_NullLeaderboardFileEntry(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<NullLeaderboardFileEntry> jsonTypeInfo))
		{
			JsonObjectInfoValues<NullLeaderboardFileEntry> objectInfo = new JsonObjectInfoValues<NullLeaderboardFileEntry>
			{
				ObjectCreator = null,
				ObjectWithParameterizedConstructorCreator = (object[] args) => new NullLeaderboardFileEntry
				{
					name = (string)args[0]
				},
				PropertyMetadataInitializer = (JsonSerializerContext _) => NullLeaderboardFileEntryPropInit(options),
				ConstructorParameterMetadataInitializer = NullLeaderboardFileEntryCtorParamInit,
				ConstructorAttributeProviderFactory = () => typeof(NullLeaderboardFileEntry).GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Array.Empty<Type>(), null),
				SerializeHandler = null
			};
			jsonTypeInfo = JsonMetadataServices.CreateObjectInfo(options, objectInfo);
			jsonTypeInfo.NumberHandling = null;
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private static JsonPropertyInfo[] NullLeaderboardFileEntryPropInit(JsonSerializerOptions options)
	{
		JsonPropertyInfo[] array = new JsonPropertyInfo[4];
		JsonPropertyInfoValues<string> propertyInfo = new JsonPropertyInfoValues<string>
		{
			IsProperty = false,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(NullLeaderboardFileEntry),
			Converter = null,
			Getter = (object obj) => ((NullLeaderboardFileEntry)obj).name,
			Setter = (object obj, string? value) =>
			{
				((NullLeaderboardFileEntry)obj).name = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "name",
			JsonPropertyName = "name",
			AttributeProviderFactory = () => typeof(NullLeaderboardFileEntry).GetField("name", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
		};
		array[0] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo);
		array[0].IsRequired = true;
		array[0].IsGetNullable = false;
		array[0].IsSetNullable = false;
		JsonPropertyInfoValues<int> propertyInfo2 = new JsonPropertyInfoValues<int>
		{
			IsProperty = false,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(NullLeaderboardFileEntry),
			Converter = null,
			Getter = (object obj) => ((NullLeaderboardFileEntry)obj).score,
			Setter = (object obj, int value) =>
			{
				((NullLeaderboardFileEntry)obj).score = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "score",
			JsonPropertyName = "score",
			AttributeProviderFactory = () => typeof(NullLeaderboardFileEntry).GetField("score", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
		};
		array[1] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo2);
		JsonPropertyInfoValues<ulong> propertyInfo3 = new JsonPropertyInfoValues<ulong>
		{
			IsProperty = false,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(NullLeaderboardFileEntry),
			Converter = null,
			Getter = (object obj) => ((NullLeaderboardFileEntry)obj).id,
			Setter = (object obj, ulong value) =>
			{
				((NullLeaderboardFileEntry)obj).id = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "id",
			JsonPropertyName = "id",
			AttributeProviderFactory = () => typeof(NullLeaderboardFileEntry).GetField("id", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
		};
		array[2] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo3);
		JsonPropertyInfoValues<List<ulong>> propertyInfo4 = new JsonPropertyInfoValues<List<ulong>>
		{
			IsProperty = false,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(NullLeaderboardFileEntry),
			Converter = null,
			Getter = (object obj) => ((NullLeaderboardFileEntry)obj).userIds,
			Setter = (object obj, List<ulong>? value) =>
			{
				((NullLeaderboardFileEntry)obj).userIds = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "userIds",
			JsonPropertyName = "other_ids",
			AttributeProviderFactory = () => typeof(NullLeaderboardFileEntry).GetField("userIds", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
		};
		array[3] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo4);
		array[3].IsGetNullable = false;
		array[3].IsSetNullable = false;
		return array;
	}

	private static JsonParameterInfoValues[] NullLeaderboardFileEntryCtorParamInit()
	{
		return new JsonParameterInfoValues[1]
		{
			new JsonParameterInfoValues
			{
				Name = "name",
				ParameterType = typeof(string),
				Position = 0,
				IsNullable = false,
				IsMemberInitializer = true
			}
		};
	}

	private JsonTypeInfo<NullMultiplayerName> Create_NullMultiplayerName(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<NullMultiplayerName> jsonTypeInfo))
		{
			JsonObjectInfoValues<NullMultiplayerName> objectInfo = new JsonObjectInfoValues<NullMultiplayerName>
			{
				ObjectCreator = () => new NullMultiplayerName(),
				ObjectWithParameterizedConstructorCreator = null,
				PropertyMetadataInitializer = (JsonSerializerContext _) => NullMultiplayerNamePropInit(options),
				ConstructorParameterMetadataInitializer = null,
				ConstructorAttributeProviderFactory = () => typeof(NullMultiplayerName).GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Array.Empty<Type>(), null),
				SerializeHandler = null
			};
			jsonTypeInfo = JsonMetadataServices.CreateObjectInfo(options, objectInfo);
			jsonTypeInfo.NumberHandling = null;
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private static JsonPropertyInfo[] NullMultiplayerNamePropInit(JsonSerializerOptions options)
	{
		JsonPropertyInfo[] array = new JsonPropertyInfo[2];
		JsonPropertyInfoValues<ulong> propertyInfo = new JsonPropertyInfoValues<ulong>
		{
			IsProperty = false,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(NullMultiplayerName),
			Converter = null,
			Getter = (object obj) => ((NullMultiplayerName)obj).netId,
			Setter = (object obj, ulong value) =>
			{
				((NullMultiplayerName)obj).netId = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "netId",
			JsonPropertyName = "net_id",
			AttributeProviderFactory = () => typeof(NullMultiplayerName).GetField("netId", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
		};
		array[0] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo);
		JsonPropertyInfoValues<string> propertyInfo2 = new JsonPropertyInfoValues<string>
		{
			IsProperty = false,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(NullMultiplayerName),
			Converter = null,
			Getter = (object obj) => ((NullMultiplayerName)obj).name,
			Setter = (object obj, string? value) =>
			{
				((NullMultiplayerName)obj).name = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "name",
			JsonPropertyName = "name",
			AttributeProviderFactory = () => typeof(NullMultiplayerName).GetField("name", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
		};
		array[1] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo2);
		array[1].IsGetNullable = false;
		array[1].IsSetNullable = false;
		return array;
	}

	private JsonTypeInfo<PlatformType> Create_PlatformType(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<PlatformType> jsonTypeInfo))
		{
			jsonTypeInfo = JsonMetadataServices.CreateValueInfo<PlatformType>(options, JsonMetadataServices.GetEnumConverter<PlatformType>(options));
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private JsonTypeInfo<RewardType> Create_RewardType(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<RewardType> jsonTypeInfo))
		{
			jsonTypeInfo = JsonMetadataServices.CreateValueInfo<RewardType>(options, JsonMetadataServices.GetEnumConverter<RewardType>(options));
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private JsonTypeInfo<RoomType> Create_RoomType(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<RoomType> jsonTypeInfo))
		{
			jsonTypeInfo = JsonMetadataServices.CreateValueInfo<RoomType>(options, JsonMetadataServices.GetEnumConverter<RoomType>(options));
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private JsonTypeInfo<CardCreationSource> Create_CardCreationSource(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<CardCreationSource> jsonTypeInfo))
		{
			jsonTypeInfo = JsonMetadataServices.CreateValueInfo<CardCreationSource>(options, JsonMetadataServices.GetEnumConverter<CardCreationSource>(options));
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private JsonTypeInfo<CardRarityOddsType> Create_CardRarityOddsType(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<CardRarityOddsType> jsonTypeInfo))
		{
			jsonTypeInfo = JsonMetadataServices.CreateValueInfo<CardRarityOddsType>(options, JsonMetadataServices.GetEnumConverter<CardRarityOddsType>(options));
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private JsonTypeInfo<GameMode> Create_GameMode(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<GameMode> jsonTypeInfo))
		{
			jsonTypeInfo = JsonMetadataServices.CreateValueInfo<GameMode>(options, JsonMetadataServices.GetEnumConverter<GameMode>(options));
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private JsonTypeInfo<AncientChoiceHistoryEntry> Create_AncientChoiceHistoryEntry(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<AncientChoiceHistoryEntry> jsonTypeInfo))
		{
			JsonObjectInfoValues<AncientChoiceHistoryEntry> objectInfo = new JsonObjectInfoValues<AncientChoiceHistoryEntry>
			{
				ObjectCreator = () => new AncientChoiceHistoryEntry(),
				ObjectWithParameterizedConstructorCreator = null,
				PropertyMetadataInitializer = (JsonSerializerContext _) => AncientChoiceHistoryEntryPropInit(options),
				ConstructorParameterMetadataInitializer = null,
				ConstructorAttributeProviderFactory = () => typeof(AncientChoiceHistoryEntry).GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Array.Empty<Type>(), null),
				SerializeHandler = null
			};
			jsonTypeInfo = JsonMetadataServices.CreateObjectInfo(options, objectInfo);
			jsonTypeInfo.NumberHandling = null;
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private static JsonPropertyInfo[] AncientChoiceHistoryEntryPropInit(JsonSerializerOptions options)
	{
		JsonPropertyInfo[] array = new JsonPropertyInfo[3];
		JsonPropertyInfoValues<LocString> propertyInfo = new JsonPropertyInfoValues<LocString>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(AncientChoiceHistoryEntry),
			Converter = null,
			Getter = (object obj) => ((AncientChoiceHistoryEntry)obj).Title,
			Setter = (object obj, LocString? value) =>
			{
				((AncientChoiceHistoryEntry)obj).Title = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "Title",
			JsonPropertyName = "title",
			AttributeProviderFactory = () => typeof(AncientChoiceHistoryEntry).GetProperty("Title", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(LocString), Array.Empty<Type>(), null)
		};
		array[0] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo);
		array[0].IsGetNullable = false;
		array[0].IsSetNullable = false;
		JsonPropertyInfoValues<bool> propertyInfo2 = new JsonPropertyInfoValues<bool>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(AncientChoiceHistoryEntry),
			Converter = null,
			Getter = (object obj) => ((AncientChoiceHistoryEntry)obj).WasChosen,
			Setter = (object obj, bool value) =>
			{
				((AncientChoiceHistoryEntry)obj).WasChosen = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "WasChosen",
			JsonPropertyName = "was_chosen",
			AttributeProviderFactory = () => typeof(AncientChoiceHistoryEntry).GetProperty("WasChosen", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(bool), Array.Empty<Type>(), null)
		};
		array[1] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo2);
		JsonPropertyInfoValues<string> propertyInfo3 = new JsonPropertyInfoValues<string>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(AncientChoiceHistoryEntry),
			Converter = null,
			Getter = (object obj) => ((AncientChoiceHistoryEntry)obj).TextKey,
			Setter = null,
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "TextKey",
			JsonPropertyName = "TextKey",
			AttributeProviderFactory = () => typeof(AncientChoiceHistoryEntry).GetProperty("TextKey", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(string), Array.Empty<Type>(), null)
		};
		array[2] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo3);
		array[2].IsGetNullable = false;
		return array;
	}

	private JsonTypeInfo<CardChoiceHistoryEntry> Create_CardChoiceHistoryEntry(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<CardChoiceHistoryEntry> jsonTypeInfo))
		{
			JsonObjectInfoValues<CardChoiceHistoryEntry> objectInfo = new JsonObjectInfoValues<CardChoiceHistoryEntry>
			{
				ObjectCreator = () => default,
				ObjectWithParameterizedConstructorCreator = null,
				PropertyMetadataInitializer = (JsonSerializerContext _) => CardChoiceHistoryEntryPropInit(options),
				ConstructorParameterMetadataInitializer = null,
				ConstructorAttributeProviderFactory = () => typeof(CardChoiceHistoryEntry).GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Array.Empty<Type>(), null),
				SerializeHandler = null
			};
			jsonTypeInfo = JsonMetadataServices.CreateObjectInfo(options, objectInfo);
			jsonTypeInfo.NumberHandling = null;
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private static JsonPropertyInfo[] CardChoiceHistoryEntryPropInit(JsonSerializerOptions options)
	{
		JsonPropertyInfo[] array = new JsonPropertyInfo[2];
		JsonPropertyInfoValues<SerializableCard> propertyInfo = new JsonPropertyInfoValues<SerializableCard>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(CardChoiceHistoryEntry),
			Converter = null,
			Getter = (object obj) => ((CardChoiceHistoryEntry)obj).Card,
			Setter = (object obj, SerializableCard? value) =>
			{
				Unsafe.Unbox<CardChoiceHistoryEntry>(obj).Card = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "Card",
			JsonPropertyName = "card",
			AttributeProviderFactory = () => typeof(CardChoiceHistoryEntry).GetProperty("Card", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(SerializableCard), Array.Empty<Type>(), null)
		};
		array[0] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo);
		array[0].IsGetNullable = false;
		array[0].IsSetNullable = false;
		JsonPropertyInfoValues<bool> propertyInfo2 = new JsonPropertyInfoValues<bool>
		{
			IsProperty = false,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(CardChoiceHistoryEntry),
			Converter = null,
			Getter = (object obj) => ((CardChoiceHistoryEntry)obj).wasPicked,
			Setter = (object obj, bool value) =>
			{
				Unsafe.Unbox<CardChoiceHistoryEntry>(obj).wasPicked = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "wasPicked",
			JsonPropertyName = "was_picked",
			AttributeProviderFactory = () => typeof(CardChoiceHistoryEntry).GetField("wasPicked", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
		};
		array[1] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo2);
		return array;
	}

	private JsonTypeInfo<CardEnchantmentHistoryEntry> Create_CardEnchantmentHistoryEntry(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<CardEnchantmentHistoryEntry> jsonTypeInfo))
		{
			JsonObjectInfoValues<CardEnchantmentHistoryEntry> objectInfo = new JsonObjectInfoValues<CardEnchantmentHistoryEntry>
			{
				ObjectCreator = () => default,
				ObjectWithParameterizedConstructorCreator = null,
				PropertyMetadataInitializer = (JsonSerializerContext _) => CardEnchantmentHistoryEntryPropInit(options),
				ConstructorParameterMetadataInitializer = null,
				ConstructorAttributeProviderFactory = () => typeof(CardEnchantmentHistoryEntry).GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Array.Empty<Type>(), null),
				SerializeHandler = null
			};
			jsonTypeInfo = JsonMetadataServices.CreateObjectInfo(options, objectInfo);
			jsonTypeInfo.NumberHandling = null;
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private static JsonPropertyInfo[] CardEnchantmentHistoryEntryPropInit(JsonSerializerOptions options)
	{
		JsonPropertyInfo[] array = new JsonPropertyInfo[2];
		JsonPropertyInfoValues<SerializableCard> propertyInfo = new JsonPropertyInfoValues<SerializableCard>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(CardEnchantmentHistoryEntry),
			Converter = null,
			Getter = (object obj) => ((CardEnchantmentHistoryEntry)obj).Card,
			Setter = (object obj, SerializableCard? value) =>
			{
				Unsafe.Unbox<CardEnchantmentHistoryEntry>(obj).Card = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "Card",
			JsonPropertyName = "card",
			AttributeProviderFactory = () => typeof(CardEnchantmentHistoryEntry).GetProperty("Card", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(SerializableCard), Array.Empty<Type>(), null)
		};
		array[0] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo);
		array[0].IsGetNullable = false;
		array[0].IsSetNullable = false;
		JsonPropertyInfoValues<ModelId> propertyInfo2 = new JsonPropertyInfoValues<ModelId>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(CardEnchantmentHistoryEntry),
			Converter = null,
			Getter = (object obj) => ((CardEnchantmentHistoryEntry)obj).Enchantment,
			Setter = (object obj, ModelId? value) =>
			{
				Unsafe.Unbox<CardEnchantmentHistoryEntry>(obj).Enchantment = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "Enchantment",
			JsonPropertyName = "enchantment",
			AttributeProviderFactory = () => typeof(CardEnchantmentHistoryEntry).GetProperty("Enchantment", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(ModelId), Array.Empty<Type>(), null)
		};
		array[1] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo2);
		array[1].IsGetNullable = false;
		array[1].IsSetNullable = false;
		return array;
	}

	private JsonTypeInfo<CardTransformationHistoryEntry> Create_CardTransformationHistoryEntry(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<CardTransformationHistoryEntry> jsonTypeInfo))
		{
			JsonObjectInfoValues<CardTransformationHistoryEntry> objectInfo = new JsonObjectInfoValues<CardTransformationHistoryEntry>
			{
				ObjectCreator = () => default,
				ObjectWithParameterizedConstructorCreator = null,
				PropertyMetadataInitializer = (JsonSerializerContext _) => CardTransformationHistoryEntryPropInit(options),
				ConstructorParameterMetadataInitializer = null,
				ConstructorAttributeProviderFactory = () => typeof(CardTransformationHistoryEntry).GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Array.Empty<Type>(), null),
				SerializeHandler = null
			};
			jsonTypeInfo = JsonMetadataServices.CreateObjectInfo(options, objectInfo);
			jsonTypeInfo.NumberHandling = null;
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private static JsonPropertyInfo[] CardTransformationHistoryEntryPropInit(JsonSerializerOptions options)
	{
		JsonPropertyInfo[] array = new JsonPropertyInfo[2];
		JsonPropertyInfoValues<SerializableCard> propertyInfo = new JsonPropertyInfoValues<SerializableCard>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(CardTransformationHistoryEntry),
			Converter = null,
			Getter = (object obj) => ((CardTransformationHistoryEntry)obj).OriginalCard,
			Setter = (object obj, SerializableCard? value) =>
			{
				Unsafe.Unbox<CardTransformationHistoryEntry>(obj).OriginalCard = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "OriginalCard",
			JsonPropertyName = "original_card",
			AttributeProviderFactory = () => typeof(CardTransformationHistoryEntry).GetProperty("OriginalCard", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(SerializableCard), Array.Empty<Type>(), null)
		};
		array[0] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo);
		array[0].IsGetNullable = false;
		array[0].IsSetNullable = false;
		JsonPropertyInfoValues<SerializableCard> propertyInfo2 = new JsonPropertyInfoValues<SerializableCard>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(CardTransformationHistoryEntry),
			Converter = null,
			Getter = (object obj) => ((CardTransformationHistoryEntry)obj).FinalCard,
			Setter = (object obj, SerializableCard? value) =>
			{
				Unsafe.Unbox<CardTransformationHistoryEntry>(obj).FinalCard = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "FinalCard",
			JsonPropertyName = "final_card",
			AttributeProviderFactory = () => typeof(CardTransformationHistoryEntry).GetProperty("FinalCard", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(SerializableCard), Array.Empty<Type>(), null)
		};
		array[1] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo2);
		array[1].IsGetNullable = false;
		array[1].IsSetNullable = false;
		return array;
	}

	private JsonTypeInfo<EventOptionHistoryEntry> Create_EventOptionHistoryEntry(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<EventOptionHistoryEntry> jsonTypeInfo))
		{
			JsonObjectInfoValues<EventOptionHistoryEntry> objectInfo = new JsonObjectInfoValues<EventOptionHistoryEntry>
			{
				ObjectCreator = () => default,
				ObjectWithParameterizedConstructorCreator = null,
				PropertyMetadataInitializer = (JsonSerializerContext _) => EventOptionHistoryEntryPropInit(options),
				ConstructorParameterMetadataInitializer = null,
				ConstructorAttributeProviderFactory = () => typeof(EventOptionHistoryEntry).GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Array.Empty<Type>(), null),
				SerializeHandler = null
			};
			jsonTypeInfo = JsonMetadataServices.CreateObjectInfo(options, objectInfo);
			jsonTypeInfo.NumberHandling = null;
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private static JsonPropertyInfo[] EventOptionHistoryEntryPropInit(JsonSerializerOptions options)
	{
		JsonPropertyInfo[] array = new JsonPropertyInfo[2];
		JsonPropertyInfoValues<LocString> propertyInfo = new JsonPropertyInfoValues<LocString>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(EventOptionHistoryEntry),
			Converter = null,
			Getter = (object obj) => ((EventOptionHistoryEntry)obj).Title,
			Setter = (object obj, LocString? value) =>
			{
				Unsafe.Unbox<EventOptionHistoryEntry>(obj).Title = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "Title",
			JsonPropertyName = "title",
			AttributeProviderFactory = () => typeof(EventOptionHistoryEntry).GetProperty("Title", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(LocString), Array.Empty<Type>(), null)
		};
		array[0] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo);
		array[0].IsGetNullable = false;
		array[0].IsSetNullable = false;
		JsonPropertyInfoValues<Dictionary<string, object>> propertyInfo2 = new JsonPropertyInfoValues<Dictionary<string, object>>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(EventOptionHistoryEntry),
			Converter = (JsonConverter<Dictionary<string, object>>)ExpandConverter(typeof(Dictionary<string, object>), new LocStringVariablesJsonConverter(), options),
			Getter = (object obj) => ((EventOptionHistoryEntry)obj).Variables,
			Setter = (object obj, Dictionary<string, object>? value) =>
			{
				Unsafe.Unbox<EventOptionHistoryEntry>(obj).Variables = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = true,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "Variables",
			JsonPropertyName = "variables",
			AttributeProviderFactory = () => typeof(EventOptionHistoryEntry).GetProperty("Variables", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(Dictionary<string, object>), Array.Empty<Type>(), null)
		};
		array[1] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo2);
		return array;
	}

	private JsonTypeInfo<MapPointHistoryEntry> Create_MapPointHistoryEntry(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<MapPointHistoryEntry> jsonTypeInfo))
		{
			JsonObjectInfoValues<MapPointHistoryEntry> objectInfo = new JsonObjectInfoValues<MapPointHistoryEntry>
			{
				ObjectCreator = () => new MapPointHistoryEntry(),
				ObjectWithParameterizedConstructorCreator = null,
				PropertyMetadataInitializer = (JsonSerializerContext _) => MapPointHistoryEntryPropInit(options),
				ConstructorParameterMetadataInitializer = null,
				ConstructorAttributeProviderFactory = () => typeof(MapPointHistoryEntry).GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Array.Empty<Type>(), null),
				SerializeHandler = null
			};
			jsonTypeInfo = JsonMetadataServices.CreateObjectInfo(options, objectInfo);
			jsonTypeInfo.NumberHandling = null;
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private static JsonPropertyInfo[] MapPointHistoryEntryPropInit(JsonSerializerOptions options)
	{
		JsonPropertyInfo[] array = new JsonPropertyInfo[3];
		JsonPropertyInfoValues<MapPointType> propertyInfo = new JsonPropertyInfoValues<MapPointType>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(MapPointHistoryEntry),
			Converter = null,
			Getter = (object obj) => ((MapPointHistoryEntry)obj).MapPointType,
			Setter = (object obj, MapPointType value) =>
			{
				((MapPointHistoryEntry)obj).MapPointType = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "MapPointType",
			JsonPropertyName = "map_point_type",
			AttributeProviderFactory = () => typeof(MapPointHistoryEntry).GetProperty("MapPointType", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(MapPointType), Array.Empty<Type>(), null)
		};
		array[0] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo);
		JsonPropertyInfoValues<List<MapPointRoomHistoryEntry>> propertyInfo2 = new JsonPropertyInfoValues<List<MapPointRoomHistoryEntry>>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(MapPointHistoryEntry),
			Converter = null,
			Getter = (object obj) => ((MapPointHistoryEntry)obj).Rooms,
			Setter = (object obj, List<MapPointRoomHistoryEntry>? value) =>
			{
				((MapPointHistoryEntry)obj).Rooms = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "Rooms",
			JsonPropertyName = "rooms",
			AttributeProviderFactory = () => typeof(MapPointHistoryEntry).GetProperty("Rooms", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(List<MapPointRoomHistoryEntry>), Array.Empty<Type>(), null)
		};
		array[1] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo2);
		array[1].IsGetNullable = false;
		array[1].IsSetNullable = false;
		JsonPropertyInfoValues<List<PlayerMapPointHistoryEntry>> propertyInfo3 = new JsonPropertyInfoValues<List<PlayerMapPointHistoryEntry>>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(MapPointHistoryEntry),
			Converter = null,
			Getter = (object obj) => ((MapPointHistoryEntry)obj).PlayerStats,
			Setter = (object obj, List<PlayerMapPointHistoryEntry>? value) =>
			{
				((MapPointHistoryEntry)obj).PlayerStats = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "PlayerStats",
			JsonPropertyName = "player_stats",
			AttributeProviderFactory = () => typeof(MapPointHistoryEntry).GetProperty("PlayerStats", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(List<PlayerMapPointHistoryEntry>), Array.Empty<Type>(), null)
		};
		array[2] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo3);
		array[2].IsGetNullable = false;
		array[2].IsSetNullable = false;
		return array;
	}

	private JsonTypeInfo<MapPointRoomHistoryEntry> Create_MapPointRoomHistoryEntry(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<MapPointRoomHistoryEntry> jsonTypeInfo))
		{
			JsonObjectInfoValues<MapPointRoomHistoryEntry> objectInfo = new JsonObjectInfoValues<MapPointRoomHistoryEntry>
			{
				ObjectCreator = () => new MapPointRoomHistoryEntry(),
				ObjectWithParameterizedConstructorCreator = null,
				PropertyMetadataInitializer = (JsonSerializerContext _) => MapPointRoomHistoryEntryPropInit(options),
				ConstructorParameterMetadataInitializer = null,
				ConstructorAttributeProviderFactory = () => typeof(MapPointRoomHistoryEntry).GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Array.Empty<Type>(), null),
				SerializeHandler = null
			};
			jsonTypeInfo = JsonMetadataServices.CreateObjectInfo(options, objectInfo);
			jsonTypeInfo.NumberHandling = null;
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private static JsonPropertyInfo[] MapPointRoomHistoryEntryPropInit(JsonSerializerOptions options)
	{
		JsonPropertyInfo[] array = new JsonPropertyInfo[4];
		JsonPropertyInfoValues<RoomType> propertyInfo = new JsonPropertyInfoValues<RoomType>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(MapPointRoomHistoryEntry),
			Converter = null,
			Getter = (object obj) => ((MapPointRoomHistoryEntry)obj).RoomType,
			Setter = (object obj, RoomType value) =>
			{
				((MapPointRoomHistoryEntry)obj).RoomType = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "RoomType",
			JsonPropertyName = "room_type",
			AttributeProviderFactory = () => typeof(MapPointRoomHistoryEntry).GetProperty("RoomType", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(RoomType), Array.Empty<Type>(), null)
		};
		array[0] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo);
		JsonPropertyInfoValues<ModelId> propertyInfo2 = new JsonPropertyInfoValues<ModelId>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(MapPointRoomHistoryEntry),
			Converter = null,
			Getter = (object obj) => ((MapPointRoomHistoryEntry)obj).ModelId,
			Setter = (object obj, ModelId? value) =>
			{
				((MapPointRoomHistoryEntry)obj).ModelId = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "ModelId",
			JsonPropertyName = "model_id",
			AttributeProviderFactory = () => typeof(MapPointRoomHistoryEntry).GetProperty("ModelId", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(ModelId), Array.Empty<Type>(), null)
		};
		array[1] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo2);
		JsonPropertyInfoValues<List<ModelId>> propertyInfo3 = new JsonPropertyInfoValues<List<ModelId>>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(MapPointRoomHistoryEntry),
			Converter = null,
			Getter = (object obj) => ((MapPointRoomHistoryEntry)obj).MonsterIds,
			Setter = (object obj, List<ModelId>? value) =>
			{
				((MapPointRoomHistoryEntry)obj).MonsterIds = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "MonsterIds",
			JsonPropertyName = "monster_ids",
			AttributeProviderFactory = () => typeof(MapPointRoomHistoryEntry).GetProperty("MonsterIds", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(List<ModelId>), Array.Empty<Type>(), null)
		};
		array[2] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo3);
		array[2].IsGetNullable = false;
		array[2].IsSetNullable = false;
		JsonPropertyInfoValues<int> propertyInfo4 = new JsonPropertyInfoValues<int>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(MapPointRoomHistoryEntry),
			Converter = null,
			Getter = (object obj) => ((MapPointRoomHistoryEntry)obj).TurnsTaken,
			Setter = (object obj, int value) =>
			{
				((MapPointRoomHistoryEntry)obj).TurnsTaken = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "TurnsTaken",
			JsonPropertyName = "turns_taken",
			AttributeProviderFactory = () => typeof(MapPointRoomHistoryEntry).GetProperty("TurnsTaken", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(int), Array.Empty<Type>(), null)
		};
		array[3] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo4);
		return array;
	}

	private JsonTypeInfo<ModelChoiceHistoryEntry> Create_ModelChoiceHistoryEntry(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<ModelChoiceHistoryEntry> jsonTypeInfo))
		{
			JsonObjectInfoValues<ModelChoiceHistoryEntry> objectInfo = new JsonObjectInfoValues<ModelChoiceHistoryEntry>
			{
				ObjectCreator = () => default,
				ObjectWithParameterizedConstructorCreator = null,
				PropertyMetadataInitializer = (JsonSerializerContext _) => ModelChoiceHistoryEntryPropInit(options),
				ConstructorParameterMetadataInitializer = null,
				ConstructorAttributeProviderFactory = () => typeof(ModelChoiceHistoryEntry).GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Array.Empty<Type>(), null),
				SerializeHandler = null
			};
			jsonTypeInfo = JsonMetadataServices.CreateObjectInfo(options, objectInfo);
			jsonTypeInfo.NumberHandling = null;
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private static JsonPropertyInfo[] ModelChoiceHistoryEntryPropInit(JsonSerializerOptions options)
	{
		JsonPropertyInfo[] array = new JsonPropertyInfo[2];
		JsonPropertyInfoValues<ModelId> propertyInfo = new JsonPropertyInfoValues<ModelId>
		{
			IsProperty = false,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(ModelChoiceHistoryEntry),
			Converter = null,
			Getter = (object obj) => ((ModelChoiceHistoryEntry)obj).choice,
			Setter = (object obj, ModelId? value) =>
			{
				Unsafe.Unbox<ModelChoiceHistoryEntry>(obj).choice = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "choice",
			JsonPropertyName = "choice",
			AttributeProviderFactory = () => typeof(ModelChoiceHistoryEntry).GetField("choice", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
		};
		array[0] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo);
		array[0].IsGetNullable = false;
		array[0].IsSetNullable = false;
		JsonPropertyInfoValues<bool> propertyInfo2 = new JsonPropertyInfoValues<bool>
		{
			IsProperty = false,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(ModelChoiceHistoryEntry),
			Converter = null,
			Getter = (object obj) => ((ModelChoiceHistoryEntry)obj).wasPicked,
			Setter = (object obj, bool value) =>
			{
				Unsafe.Unbox<ModelChoiceHistoryEntry>(obj).wasPicked = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "wasPicked",
			JsonPropertyName = "was_picked",
			AttributeProviderFactory = () => typeof(ModelChoiceHistoryEntry).GetField("wasPicked", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
		};
		array[1] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo2);
		return array;
	}

	private JsonTypeInfo<PlayerMapPointHistoryEntry> Create_PlayerMapPointHistoryEntry(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<PlayerMapPointHistoryEntry> jsonTypeInfo))
		{
			JsonObjectInfoValues<PlayerMapPointHistoryEntry> objectInfo = new JsonObjectInfoValues<PlayerMapPointHistoryEntry>
			{
				ObjectCreator = () => new PlayerMapPointHistoryEntry(),
				ObjectWithParameterizedConstructorCreator = null,
				PropertyMetadataInitializer = (JsonSerializerContext _) => PlayerMapPointHistoryEntryPropInit(options),
				ConstructorParameterMetadataInitializer = null,
				ConstructorAttributeProviderFactory = () => typeof(PlayerMapPointHistoryEntry).GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Array.Empty<Type>(), null),
				SerializeHandler = null
			};
			jsonTypeInfo = JsonMetadataServices.CreateObjectInfo(options, objectInfo);
			jsonTypeInfo.NumberHandling = null;
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private static JsonPropertyInfo[] PlayerMapPointHistoryEntryPropInit(JsonSerializerOptions options)
	{
		JsonPropertyInfo[] array = new JsonPropertyInfo[34];
		JsonPropertyInfoValues<ulong> propertyInfo = new JsonPropertyInfoValues<ulong>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(PlayerMapPointHistoryEntry),
			Converter = null,
			Getter = (object obj) => ((PlayerMapPointHistoryEntry)obj).PlayerId,
			Setter = (object obj, ulong value) =>
			{
				((PlayerMapPointHistoryEntry)obj).PlayerId = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "PlayerId",
			JsonPropertyName = "player_id",
			AttributeProviderFactory = () => typeof(PlayerMapPointHistoryEntry).GetProperty("PlayerId", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(ulong), Array.Empty<Type>(), null)
		};
		array[0] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo);
		JsonPropertyInfoValues<int> propertyInfo2 = new JsonPropertyInfoValues<int>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(PlayerMapPointHistoryEntry),
			Converter = null,
			Getter = (object obj) => ((PlayerMapPointHistoryEntry)obj).GoldGained,
			Setter = (object obj, int value) =>
			{
				((PlayerMapPointHistoryEntry)obj).GoldGained = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "GoldGained",
			JsonPropertyName = "gold_gained",
			AttributeProviderFactory = () => typeof(PlayerMapPointHistoryEntry).GetProperty("GoldGained", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(int), Array.Empty<Type>(), null)
		};
		array[1] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo2);
		JsonPropertyInfoValues<int> propertyInfo3 = new JsonPropertyInfoValues<int>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(PlayerMapPointHistoryEntry),
			Converter = null,
			Getter = (object obj) => ((PlayerMapPointHistoryEntry)obj).GoldSpent,
			Setter = (object obj, int value) =>
			{
				((PlayerMapPointHistoryEntry)obj).GoldSpent = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "GoldSpent",
			JsonPropertyName = "gold_spent",
			AttributeProviderFactory = () => typeof(PlayerMapPointHistoryEntry).GetProperty("GoldSpent", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(int), Array.Empty<Type>(), null)
		};
		array[2] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo3);
		JsonPropertyInfoValues<int> propertyInfo4 = new JsonPropertyInfoValues<int>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(PlayerMapPointHistoryEntry),
			Converter = null,
			Getter = (object obj) => ((PlayerMapPointHistoryEntry)obj).GoldLost,
			Setter = (object obj, int value) =>
			{
				((PlayerMapPointHistoryEntry)obj).GoldLost = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "GoldLost",
			JsonPropertyName = "gold_lost",
			AttributeProviderFactory = () => typeof(PlayerMapPointHistoryEntry).GetProperty("GoldLost", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(int), Array.Empty<Type>(), null)
		};
		array[3] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo4);
		JsonPropertyInfoValues<int> propertyInfo5 = new JsonPropertyInfoValues<int>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(PlayerMapPointHistoryEntry),
			Converter = null,
			Getter = (object obj) => ((PlayerMapPointHistoryEntry)obj).GoldStolen,
			Setter = (object obj, int value) =>
			{
				((PlayerMapPointHistoryEntry)obj).GoldStolen = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "GoldStolen",
			JsonPropertyName = "gold_stolen",
			AttributeProviderFactory = () => typeof(PlayerMapPointHistoryEntry).GetProperty("GoldStolen", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(int), Array.Empty<Type>(), null)
		};
		array[4] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo5);
		JsonPropertyInfoValues<int> propertyInfo6 = new JsonPropertyInfoValues<int>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(PlayerMapPointHistoryEntry),
			Converter = null,
			Getter = (object obj) => ((PlayerMapPointHistoryEntry)obj).StolenLoot,
			Setter = (object obj, int value) =>
			{
				((PlayerMapPointHistoryEntry)obj).StolenLoot = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "StolenLoot",
			JsonPropertyName = "stolen_loot",
			AttributeProviderFactory = () => typeof(PlayerMapPointHistoryEntry).GetProperty("StolenLoot", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(int), Array.Empty<Type>(), null)
		};
		array[5] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo6);
		JsonPropertyInfoValues<int> propertyInfo7 = new JsonPropertyInfoValues<int>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(PlayerMapPointHistoryEntry),
			Converter = null,
			Getter = (object obj) => ((PlayerMapPointHistoryEntry)obj).CurrentGold,
			Setter = (object obj, int value) =>
			{
				((PlayerMapPointHistoryEntry)obj).CurrentGold = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "CurrentGold",
			JsonPropertyName = "current_gold",
			AttributeProviderFactory = () => typeof(PlayerMapPointHistoryEntry).GetProperty("CurrentGold", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(int), Array.Empty<Type>(), null)
		};
		array[6] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo7);
		JsonPropertyInfoValues<int> propertyInfo8 = new JsonPropertyInfoValues<int>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(PlayerMapPointHistoryEntry),
			Converter = null,
			Getter = (object obj) => ((PlayerMapPointHistoryEntry)obj).CurrentHp,
			Setter = (object obj, int value) =>
			{
				((PlayerMapPointHistoryEntry)obj).CurrentHp = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "CurrentHp",
			JsonPropertyName = "current_hp",
			AttributeProviderFactory = () => typeof(PlayerMapPointHistoryEntry).GetProperty("CurrentHp", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(int), Array.Empty<Type>(), null)
		};
		array[7] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo8);
		JsonPropertyInfoValues<int> propertyInfo9 = new JsonPropertyInfoValues<int>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(PlayerMapPointHistoryEntry),
			Converter = null,
			Getter = (object obj) => ((PlayerMapPointHistoryEntry)obj).MaxHp,
			Setter = (object obj, int value) =>
			{
				((PlayerMapPointHistoryEntry)obj).MaxHp = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "MaxHp",
			JsonPropertyName = "max_hp",
			AttributeProviderFactory = () => typeof(PlayerMapPointHistoryEntry).GetProperty("MaxHp", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(int), Array.Empty<Type>(), null)
		};
		array[8] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo9);
		JsonPropertyInfoValues<int> propertyInfo10 = new JsonPropertyInfoValues<int>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(PlayerMapPointHistoryEntry),
			Converter = null,
			Getter = (object obj) => ((PlayerMapPointHistoryEntry)obj).DamageTaken,
			Setter = (object obj, int value) =>
			{
				((PlayerMapPointHistoryEntry)obj).DamageTaken = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "DamageTaken",
			JsonPropertyName = "damage_taken",
			AttributeProviderFactory = () => typeof(PlayerMapPointHistoryEntry).GetProperty("DamageTaken", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(int), Array.Empty<Type>(), null)
		};
		array[9] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo10);
		JsonPropertyInfoValues<int> propertyInfo11 = new JsonPropertyInfoValues<int>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(PlayerMapPointHistoryEntry),
			Converter = null,
			Getter = (object obj) => ((PlayerMapPointHistoryEntry)obj).HpHealed,
			Setter = (object obj, int value) =>
			{
				((PlayerMapPointHistoryEntry)obj).HpHealed = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "HpHealed",
			JsonPropertyName = "hp_healed",
			AttributeProviderFactory = () => typeof(PlayerMapPointHistoryEntry).GetProperty("HpHealed", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(int), Array.Empty<Type>(), null)
		};
		array[10] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo11);
		JsonPropertyInfoValues<int> propertyInfo12 = new JsonPropertyInfoValues<int>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(PlayerMapPointHistoryEntry),
			Converter = null,
			Getter = (object obj) => ((PlayerMapPointHistoryEntry)obj).MaxHpLost,
			Setter = (object obj, int value) =>
			{
				((PlayerMapPointHistoryEntry)obj).MaxHpLost = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "MaxHpLost",
			JsonPropertyName = "max_hp_lost",
			AttributeProviderFactory = () => typeof(PlayerMapPointHistoryEntry).GetProperty("MaxHpLost", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(int), Array.Empty<Type>(), null)
		};
		array[11] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo12);
		JsonPropertyInfoValues<int> propertyInfo13 = new JsonPropertyInfoValues<int>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(PlayerMapPointHistoryEntry),
			Converter = null,
			Getter = (object obj) => ((PlayerMapPointHistoryEntry)obj).MaxHpGained,
			Setter = (object obj, int value) =>
			{
				((PlayerMapPointHistoryEntry)obj).MaxHpGained = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "MaxHpGained",
			JsonPropertyName = "max_hp_gained",
			AttributeProviderFactory = () => typeof(PlayerMapPointHistoryEntry).GetProperty("MaxHpGained", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(int), Array.Empty<Type>(), null)
		};
		array[12] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo13);
		JsonPropertyInfoValues<List<AncientChoiceHistoryEntry>> propertyInfo14 = new JsonPropertyInfoValues<List<AncientChoiceHistoryEntry>>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(PlayerMapPointHistoryEntry),
			Converter = null,
			Getter = (object obj) => ((PlayerMapPointHistoryEntry)obj).AncientChoices,
			Setter = (object obj, List<AncientChoiceHistoryEntry>? value) =>
			{
				((PlayerMapPointHistoryEntry)obj).AncientChoices = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "AncientChoices",
			JsonPropertyName = "ancient_choice",
			AttributeProviderFactory = () => typeof(PlayerMapPointHistoryEntry).GetProperty("AncientChoices", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(List<AncientChoiceHistoryEntry>), Array.Empty<Type>(), null)
		};
		array[13] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo14);
		array[13].IsGetNullable = false;
		array[13].IsSetNullable = false;
		JsonPropertyInfoValues<List<SerializableCard>> propertyInfo15 = new JsonPropertyInfoValues<List<SerializableCard>>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(PlayerMapPointHistoryEntry),
			Converter = null,
			Getter = (object obj) => ((PlayerMapPointHistoryEntry)obj).CardsGained,
			Setter = (object obj, List<SerializableCard>? value) =>
			{
				((PlayerMapPointHistoryEntry)obj).CardsGained = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "CardsGained",
			JsonPropertyName = "cards_gained",
			AttributeProviderFactory = () => typeof(PlayerMapPointHistoryEntry).GetProperty("CardsGained", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(List<SerializableCard>), Array.Empty<Type>(), null)
		};
		array[14] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo15);
		array[14].IsGetNullable = false;
		array[14].IsSetNullable = false;
		JsonPropertyInfoValues<List<CardChoiceHistoryEntry>> propertyInfo16 = new JsonPropertyInfoValues<List<CardChoiceHistoryEntry>>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(PlayerMapPointHistoryEntry),
			Converter = null,
			Getter = (object obj) => ((PlayerMapPointHistoryEntry)obj).CardChoices,
			Setter = (object obj, List<CardChoiceHistoryEntry>? value) =>
			{
				((PlayerMapPointHistoryEntry)obj).CardChoices = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "CardChoices",
			JsonPropertyName = "card_choices",
			AttributeProviderFactory = () => typeof(PlayerMapPointHistoryEntry).GetProperty("CardChoices", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(List<CardChoiceHistoryEntry>), Array.Empty<Type>(), null)
		};
		array[15] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo16);
		array[15].IsGetNullable = false;
		array[15].IsSetNullable = false;
		JsonPropertyInfoValues<List<ModelChoiceHistoryEntry>> propertyInfo17 = new JsonPropertyInfoValues<List<ModelChoiceHistoryEntry>>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(PlayerMapPointHistoryEntry),
			Converter = null,
			Getter = (object obj) => ((PlayerMapPointHistoryEntry)obj).RelicChoices,
			Setter = (object obj, List<ModelChoiceHistoryEntry>? value) =>
			{
				((PlayerMapPointHistoryEntry)obj).RelicChoices = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "RelicChoices",
			JsonPropertyName = "relic_choices",
			AttributeProviderFactory = () => typeof(PlayerMapPointHistoryEntry).GetProperty("RelicChoices", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(List<ModelChoiceHistoryEntry>), Array.Empty<Type>(), null)
		};
		array[16] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo17);
		array[16].IsGetNullable = false;
		array[16].IsSetNullable = false;
		JsonPropertyInfoValues<List<ModelChoiceHistoryEntry>> propertyInfo18 = new JsonPropertyInfoValues<List<ModelChoiceHistoryEntry>>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(PlayerMapPointHistoryEntry),
			Converter = null,
			Getter = (object obj) => ((PlayerMapPointHistoryEntry)obj).PotionChoices,
			Setter = (object obj, List<ModelChoiceHistoryEntry>? value) =>
			{
				((PlayerMapPointHistoryEntry)obj).PotionChoices = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "PotionChoices",
			JsonPropertyName = "potion_choices",
			AttributeProviderFactory = () => typeof(PlayerMapPointHistoryEntry).GetProperty("PotionChoices", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(List<ModelChoiceHistoryEntry>), Array.Empty<Type>(), null)
		};
		array[17] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo18);
		array[17].IsGetNullable = false;
		array[17].IsSetNullable = false;
		JsonPropertyInfoValues<List<ModelId>> propertyInfo19 = new JsonPropertyInfoValues<List<ModelId>>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(PlayerMapPointHistoryEntry),
			Converter = null,
			Getter = (object obj) => ((PlayerMapPointHistoryEntry)obj).PotionDiscarded,
			Setter = (object obj, List<ModelId>? value) =>
			{
				((PlayerMapPointHistoryEntry)obj).PotionDiscarded = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "PotionDiscarded",
			JsonPropertyName = "potion_discarded",
			AttributeProviderFactory = () => typeof(PlayerMapPointHistoryEntry).GetProperty("PotionDiscarded", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(List<ModelId>), Array.Empty<Type>(), null)
		};
		array[18] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo19);
		array[18].IsGetNullable = false;
		array[18].IsSetNullable = false;
		JsonPropertyInfoValues<List<ModelId>> propertyInfo20 = new JsonPropertyInfoValues<List<ModelId>>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(PlayerMapPointHistoryEntry),
			Converter = null,
			Getter = (object obj) => ((PlayerMapPointHistoryEntry)obj).PotionUsed,
			Setter = (object obj, List<ModelId>? value) =>
			{
				((PlayerMapPointHistoryEntry)obj).PotionUsed = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "PotionUsed",
			JsonPropertyName = "potion_used",
			AttributeProviderFactory = () => typeof(PlayerMapPointHistoryEntry).GetProperty("PotionUsed", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(List<ModelId>), Array.Empty<Type>(), null)
		};
		array[19] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo20);
		array[19].IsGetNullable = false;
		array[19].IsSetNullable = false;
		JsonPropertyInfoValues<List<SerializableCard>> propertyInfo21 = new JsonPropertyInfoValues<List<SerializableCard>>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(PlayerMapPointHistoryEntry),
			Converter = null,
			Getter = (object obj) => ((PlayerMapPointHistoryEntry)obj).CardsRemoved,
			Setter = (object obj, List<SerializableCard>? value) =>
			{
				((PlayerMapPointHistoryEntry)obj).CardsRemoved = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "CardsRemoved",
			JsonPropertyName = "cards_removed",
			AttributeProviderFactory = () => typeof(PlayerMapPointHistoryEntry).GetProperty("CardsRemoved", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(List<SerializableCard>), Array.Empty<Type>(), null)
		};
		array[20] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo21);
		array[20].IsGetNullable = false;
		array[20].IsSetNullable = false;
		JsonPropertyInfoValues<List<ModelId>> propertyInfo22 = new JsonPropertyInfoValues<List<ModelId>>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(PlayerMapPointHistoryEntry),
			Converter = null,
			Getter = (object obj) => ((PlayerMapPointHistoryEntry)obj).RelicsRemoved,
			Setter = (object obj, List<ModelId>? value) =>
			{
				((PlayerMapPointHistoryEntry)obj).RelicsRemoved = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "RelicsRemoved",
			JsonPropertyName = "relics_removed",
			AttributeProviderFactory = () => typeof(PlayerMapPointHistoryEntry).GetProperty("RelicsRemoved", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(List<ModelId>), Array.Empty<Type>(), null)
		};
		array[21] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo22);
		array[21].IsGetNullable = false;
		array[21].IsSetNullable = false;
		JsonPropertyInfoValues<List<CardEnchantmentHistoryEntry>> propertyInfo23 = new JsonPropertyInfoValues<List<CardEnchantmentHistoryEntry>>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(PlayerMapPointHistoryEntry),
			Converter = null,
			Getter = (object obj) => ((PlayerMapPointHistoryEntry)obj).CardsEnchanted,
			Setter = (object obj, List<CardEnchantmentHistoryEntry>? value) =>
			{
				((PlayerMapPointHistoryEntry)obj).CardsEnchanted = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "CardsEnchanted",
			JsonPropertyName = "cards_enchanted",
			AttributeProviderFactory = () => typeof(PlayerMapPointHistoryEntry).GetProperty("CardsEnchanted", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(List<CardEnchantmentHistoryEntry>), Array.Empty<Type>(), null)
		};
		array[22] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo23);
		array[22].IsGetNullable = false;
		array[22].IsSetNullable = false;
		JsonPropertyInfoValues<List<CardTransformationHistoryEntry>> propertyInfo24 = new JsonPropertyInfoValues<List<CardTransformationHistoryEntry>>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(PlayerMapPointHistoryEntry),
			Converter = null,
			Getter = (object obj) => ((PlayerMapPointHistoryEntry)obj).CardsTransformed,
			Setter = (object obj, List<CardTransformationHistoryEntry>? value) =>
			{
				((PlayerMapPointHistoryEntry)obj).CardsTransformed = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "CardsTransformed",
			JsonPropertyName = "cards_transformed",
			AttributeProviderFactory = () => typeof(PlayerMapPointHistoryEntry).GetProperty("CardsTransformed", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(List<CardTransformationHistoryEntry>), Array.Empty<Type>(), null)
		};
		array[23] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo24);
		array[23].IsGetNullable = false;
		array[23].IsSetNullable = false;
		JsonPropertyInfoValues<List<ModelId>> propertyInfo25 = new JsonPropertyInfoValues<List<ModelId>>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(PlayerMapPointHistoryEntry),
			Converter = null,
			Getter = (object obj) => ((PlayerMapPointHistoryEntry)obj).UpgradedCards,
			Setter = (object obj, List<ModelId>? value) =>
			{
				((PlayerMapPointHistoryEntry)obj).UpgradedCards = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "UpgradedCards",
			JsonPropertyName = "upgraded_cards",
			AttributeProviderFactory = () => typeof(PlayerMapPointHistoryEntry).GetProperty("UpgradedCards", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(List<ModelId>), Array.Empty<Type>(), null)
		};
		array[24] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo25);
		array[24].IsGetNullable = false;
		array[24].IsSetNullable = false;
		JsonPropertyInfoValues<List<ModelId>> propertyInfo26 = new JsonPropertyInfoValues<List<ModelId>>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(PlayerMapPointHistoryEntry),
			Converter = null,
			Getter = (object obj) => ((PlayerMapPointHistoryEntry)obj).DowngradedCards,
			Setter = (object obj, List<ModelId>? value) =>
			{
				((PlayerMapPointHistoryEntry)obj).DowngradedCards = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "DowngradedCards",
			JsonPropertyName = "downgraded_cards",
			AttributeProviderFactory = () => typeof(PlayerMapPointHistoryEntry).GetProperty("DowngradedCards", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(List<ModelId>), Array.Empty<Type>(), null)
		};
		array[25] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo26);
		array[25].IsGetNullable = false;
		array[25].IsSetNullable = false;
		JsonPropertyInfoValues<List<EventOptionHistoryEntry>> propertyInfo27 = new JsonPropertyInfoValues<List<EventOptionHistoryEntry>>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(PlayerMapPointHistoryEntry),
			Converter = null,
			Getter = (object obj) => ((PlayerMapPointHistoryEntry)obj).EventChoices,
			Setter = (object obj, List<EventOptionHistoryEntry>? value) =>
			{
				((PlayerMapPointHistoryEntry)obj).EventChoices = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "EventChoices",
			JsonPropertyName = "event_choices",
			AttributeProviderFactory = () => typeof(PlayerMapPointHistoryEntry).GetProperty("EventChoices", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(List<EventOptionHistoryEntry>), Array.Empty<Type>(), null)
		};
		array[26] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo27);
		array[26].IsGetNullable = false;
		array[26].IsSetNullable = false;
		JsonPropertyInfoValues<List<string>> propertyInfo28 = new JsonPropertyInfoValues<List<string>>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(PlayerMapPointHistoryEntry),
			Converter = null,
			Getter = (object obj) => ((PlayerMapPointHistoryEntry)obj).RestSiteChoices,
			Setter = (object obj, List<string>? value) =>
			{
				((PlayerMapPointHistoryEntry)obj).RestSiteChoices = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "RestSiteChoices",
			JsonPropertyName = "rest_site_choices",
			AttributeProviderFactory = () => typeof(PlayerMapPointHistoryEntry).GetProperty("RestSiteChoices", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(List<string>), Array.Empty<Type>(), null)
		};
		array[27] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo28);
		array[27].IsGetNullable = false;
		array[27].IsSetNullable = false;
		JsonPropertyInfoValues<List<ModelId>> propertyInfo29 = new JsonPropertyInfoValues<List<ModelId>>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(PlayerMapPointHistoryEntry),
			Converter = null,
			Getter = (object obj) => ((PlayerMapPointHistoryEntry)obj).BoughtRelics,
			Setter = (object obj, List<ModelId>? value) =>
			{
				((PlayerMapPointHistoryEntry)obj).BoughtRelics = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "BoughtRelics",
			JsonPropertyName = "bought_relics",
			AttributeProviderFactory = () => typeof(PlayerMapPointHistoryEntry).GetProperty("BoughtRelics", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(List<ModelId>), Array.Empty<Type>(), null)
		};
		array[28] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo29);
		array[28].IsGetNullable = false;
		array[28].IsSetNullable = false;
		JsonPropertyInfoValues<List<ModelId>> propertyInfo30 = new JsonPropertyInfoValues<List<ModelId>>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(PlayerMapPointHistoryEntry),
			Converter = null,
			Getter = (object obj) => ((PlayerMapPointHistoryEntry)obj).BoughtPotions,
			Setter = (object obj, List<ModelId>? value) =>
			{
				((PlayerMapPointHistoryEntry)obj).BoughtPotions = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "BoughtPotions",
			JsonPropertyName = "bought_potions",
			AttributeProviderFactory = () => typeof(PlayerMapPointHistoryEntry).GetProperty("BoughtPotions", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(List<ModelId>), Array.Empty<Type>(), null)
		};
		array[29] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo30);
		array[29].IsGetNullable = false;
		array[29].IsSetNullable = false;
		JsonPropertyInfoValues<List<ModelId>> propertyInfo31 = new JsonPropertyInfoValues<List<ModelId>>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(PlayerMapPointHistoryEntry),
			Converter = null,
			Getter = (object obj) => ((PlayerMapPointHistoryEntry)obj).BoughtColorless,
			Setter = (object obj, List<ModelId>? value) =>
			{
				((PlayerMapPointHistoryEntry)obj).BoughtColorless = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "BoughtColorless",
			JsonPropertyName = "bought_colorless",
			AttributeProviderFactory = () => typeof(PlayerMapPointHistoryEntry).GetProperty("BoughtColorless", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(List<ModelId>), Array.Empty<Type>(), null)
		};
		array[30] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo31);
		array[30].IsGetNullable = false;
		array[30].IsSetNullable = false;
		JsonPropertyInfoValues<List<ModelId>> propertyInfo32 = new JsonPropertyInfoValues<List<ModelId>>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(PlayerMapPointHistoryEntry),
			Converter = null,
			Getter = (object obj) => ((PlayerMapPointHistoryEntry)obj).CompletedQuests,
			Setter = (object obj, List<ModelId>? value) =>
			{
				((PlayerMapPointHistoryEntry)obj).CompletedQuests = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "CompletedQuests",
			JsonPropertyName = "completed_quests",
			AttributeProviderFactory = () => typeof(PlayerMapPointHistoryEntry).GetProperty("CompletedQuests", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(List<ModelId>), Array.Empty<Type>(), null)
		};
		array[31] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo32);
		array[31].IsGetNullable = false;
		array[31].IsSetNullable = false;
		JsonPropertyInfoValues<bool> propertyInfo33 = new JsonPropertyInfoValues<bool>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(PlayerMapPointHistoryEntry),
			Converter = null,
			Getter = (object obj) => ((PlayerMapPointHistoryEntry)obj).IsAffectedByFurCoat,
			Setter = (object obj, bool value) =>
			{
				((PlayerMapPointHistoryEntry)obj).IsAffectedByFurCoat = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "IsAffectedByFurCoat",
			JsonPropertyName = "is_affected_by_fur_coat",
			AttributeProviderFactory = () => typeof(PlayerMapPointHistoryEntry).GetProperty("IsAffectedByFurCoat", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(bool), Array.Empty<Type>(), null)
		};
		array[32] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo33);
		JsonPropertyInfoValues<bool> propertyInfo34 = new JsonPropertyInfoValues<bool>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(PlayerMapPointHistoryEntry),
			Converter = null,
			Getter = null,
			Setter = null,
			IgnoreCondition = JsonIgnoreCondition.Always,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "WasMugged",
			JsonPropertyName = null,
			AttributeProviderFactory = () => typeof(PlayerMapPointHistoryEntry).GetProperty("WasMugged", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(bool), Array.Empty<Type>(), null)
		};
		array[33] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo34);
		return array;
	}

	private JsonTypeInfo<RunHistory> Create_RunHistory(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<RunHistory> jsonTypeInfo))
		{
			JsonObjectInfoValues<RunHistory> objectInfo = new JsonObjectInfoValues<RunHistory>
			{
				ObjectCreator = null,
				ObjectWithParameterizedConstructorCreator = (object[] args) => new RunHistory
				{
					PlatformType = (PlatformType)args[0],
					GameMode = (GameMode)args[1],
					Win = (bool)args[2],
					Seed = (string)args[3],
					StartTime = (long)args[4],
					RunTime = (float)args[5],
					Ascension = (int)args[6],
					BuildId = (string)args[7],
					WasAbandoned = (bool)args[8],
					KilledByEncounter = (ModelId)args[9],
					KilledByEvent = (ModelId)args[10],
					Players = (List<RunHistoryPlayer>)args[11],
					Acts = (List<ModelId>)args[12],
					Modifiers = (List<SerializableModifier>)args[13],
					MapPointHistory = (List<List<MapPointHistoryEntry>>)args[14]
				},
				PropertyMetadataInitializer = (JsonSerializerContext _) => RunHistoryPropInit(options),
				ConstructorParameterMetadataInitializer = RunHistoryCtorParamInit,
				ConstructorAttributeProviderFactory = () => typeof(RunHistory).GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Array.Empty<Type>(), null),
				SerializeHandler = null
			};
			jsonTypeInfo = JsonMetadataServices.CreateObjectInfo(options, objectInfo);
			jsonTypeInfo.NumberHandling = null;
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private static JsonPropertyInfo[] RunHistoryPropInit(JsonSerializerOptions options)
	{
		JsonPropertyInfo[] array = new JsonPropertyInfo[16];
		JsonPropertyInfoValues<int> propertyInfo = new JsonPropertyInfoValues<int>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(RunHistory),
			Converter = null,
			Getter = (object obj) => ((RunHistory)obj).SchemaVersion,
			Setter = (object obj, int value) =>
			{
				((RunHistory)obj).SchemaVersion = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "SchemaVersion",
			JsonPropertyName = "schema_version",
			AttributeProviderFactory = () => typeof(RunHistory).GetProperty("SchemaVersion", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(int), Array.Empty<Type>(), null)
		};
		array[0] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo);
		JsonPropertyInfoValues<PlatformType> propertyInfo2 = new JsonPropertyInfoValues<PlatformType>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(RunHistory),
			Converter = null,
			Getter = (object obj) => ((RunHistory)obj).PlatformType,
			Setter = (object obj, PlatformType value) =>
			{
				throw new InvalidOperationException("Setting init-only properties is not supported in source generation mode.");
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "PlatformType",
			JsonPropertyName = "platform_type",
			AttributeProviderFactory = () => typeof(RunHistory).GetProperty("PlatformType", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(PlatformType), Array.Empty<Type>(), null)
		};
		array[1] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo2);
		JsonPropertyInfoValues<GameMode> propertyInfo3 = new JsonPropertyInfoValues<GameMode>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(RunHistory),
			Converter = null,
			Getter = (object obj) => ((RunHistory)obj).GameMode,
			Setter = (object obj, GameMode value) =>
			{
				throw new InvalidOperationException("Setting init-only properties is not supported in source generation mode.");
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "GameMode",
			JsonPropertyName = "game_mode",
			AttributeProviderFactory = () => typeof(RunHistory).GetProperty("GameMode", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(GameMode), Array.Empty<Type>(), null)
		};
		array[2] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo3);
		JsonPropertyInfoValues<bool> propertyInfo4 = new JsonPropertyInfoValues<bool>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(RunHistory),
			Converter = null,
			Getter = (object obj) => ((RunHistory)obj).Win,
			Setter = (object obj, bool value) =>
			{
				throw new InvalidOperationException("Setting init-only properties is not supported in source generation mode.");
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "Win",
			JsonPropertyName = "win",
			AttributeProviderFactory = () => typeof(RunHistory).GetProperty("Win", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(bool), Array.Empty<Type>(), null)
		};
		array[3] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo4);
		JsonPropertyInfoValues<string> propertyInfo5 = new JsonPropertyInfoValues<string>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(RunHistory),
			Converter = null,
			Getter = (object obj) => ((RunHistory)obj).Seed,
			Setter = (object obj, string? value) =>
			{
				throw new InvalidOperationException("Setting init-only properties is not supported in source generation mode.");
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "Seed",
			JsonPropertyName = "seed",
			AttributeProviderFactory = () => typeof(RunHistory).GetProperty("Seed", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(string), Array.Empty<Type>(), null)
		};
		array[4] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo5);
		array[4].IsGetNullable = false;
		array[4].IsSetNullable = false;
		JsonPropertyInfoValues<long> propertyInfo6 = new JsonPropertyInfoValues<long>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(RunHistory),
			Converter = null,
			Getter = (object obj) => ((RunHistory)obj).StartTime,
			Setter = (object obj, long value) =>
			{
				throw new InvalidOperationException("Setting init-only properties is not supported in source generation mode.");
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "StartTime",
			JsonPropertyName = "start_time",
			AttributeProviderFactory = () => typeof(RunHistory).GetProperty("StartTime", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(long), Array.Empty<Type>(), null)
		};
		array[5] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo6);
		JsonPropertyInfoValues<float> propertyInfo7 = new JsonPropertyInfoValues<float>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(RunHistory),
			Converter = null,
			Getter = (object obj) => ((RunHistory)obj).RunTime,
			Setter = (object obj, float value) =>
			{
				throw new InvalidOperationException("Setting init-only properties is not supported in source generation mode.");
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "RunTime",
			JsonPropertyName = "run_time",
			AttributeProviderFactory = () => typeof(RunHistory).GetProperty("RunTime", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(float), Array.Empty<Type>(), null)
		};
		array[6] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo7);
		JsonPropertyInfoValues<int> propertyInfo8 = new JsonPropertyInfoValues<int>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(RunHistory),
			Converter = null,
			Getter = (object obj) => ((RunHistory)obj).Ascension,
			Setter = (object obj, int value) =>
			{
				throw new InvalidOperationException("Setting init-only properties is not supported in source generation mode.");
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "Ascension",
			JsonPropertyName = "ascension",
			AttributeProviderFactory = () => typeof(RunHistory).GetProperty("Ascension", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(int), Array.Empty<Type>(), null)
		};
		array[7] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo8);
		JsonPropertyInfoValues<string> propertyInfo9 = new JsonPropertyInfoValues<string>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(RunHistory),
			Converter = null,
			Getter = (object obj) => ((RunHistory)obj).BuildId,
			Setter = (object obj, string? value) =>
			{
				throw new InvalidOperationException("Setting init-only properties is not supported in source generation mode.");
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "BuildId",
			JsonPropertyName = "build_id",
			AttributeProviderFactory = () => typeof(RunHistory).GetProperty("BuildId", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(string), Array.Empty<Type>(), null)
		};
		array[8] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo9);
		array[8].IsGetNullable = false;
		array[8].IsSetNullable = false;
		JsonPropertyInfoValues<bool> propertyInfo10 = new JsonPropertyInfoValues<bool>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(RunHistory),
			Converter = null,
			Getter = (object obj) => ((RunHistory)obj).WasAbandoned,
			Setter = (object obj, bool value) =>
			{
				throw new InvalidOperationException("Setting init-only properties is not supported in source generation mode.");
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "WasAbandoned",
			JsonPropertyName = "was_abandoned",
			AttributeProviderFactory = () => typeof(RunHistory).GetProperty("WasAbandoned", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(bool), Array.Empty<Type>(), null)
		};
		array[9] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo10);
		JsonPropertyInfoValues<ModelId> propertyInfo11 = new JsonPropertyInfoValues<ModelId>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(RunHistory),
			Converter = null,
			Getter = (object obj) => ((RunHistory)obj).KilledByEncounter,
			Setter = (object obj, ModelId? value) =>
			{
				throw new InvalidOperationException("Setting init-only properties is not supported in source generation mode.");
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "KilledByEncounter",
			JsonPropertyName = "killed_by_encounter",
			AttributeProviderFactory = () => typeof(RunHistory).GetProperty("KilledByEncounter", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(ModelId), Array.Empty<Type>(), null)
		};
		array[10] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo11);
		array[10].IsGetNullable = false;
		array[10].IsSetNullable = false;
		JsonPropertyInfoValues<ModelId> propertyInfo12 = new JsonPropertyInfoValues<ModelId>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(RunHistory),
			Converter = null,
			Getter = (object obj) => ((RunHistory)obj).KilledByEvent,
			Setter = (object obj, ModelId? value) =>
			{
				throw new InvalidOperationException("Setting init-only properties is not supported in source generation mode.");
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "KilledByEvent",
			JsonPropertyName = "killed_by_event",
			AttributeProviderFactory = () => typeof(RunHistory).GetProperty("KilledByEvent", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(ModelId), Array.Empty<Type>(), null)
		};
		array[11] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo12);
		array[11].IsGetNullable = false;
		array[11].IsSetNullable = false;
		JsonPropertyInfoValues<List<RunHistoryPlayer>> propertyInfo13 = new JsonPropertyInfoValues<List<RunHistoryPlayer>>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(RunHistory),
			Converter = null,
			Getter = (object obj) => ((RunHistory)obj).Players,
			Setter = (object obj, List<RunHistoryPlayer>? value) =>
			{
				throw new InvalidOperationException("Setting init-only properties is not supported in source generation mode.");
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "Players",
			JsonPropertyName = "players",
			AttributeProviderFactory = () => typeof(RunHistory).GetProperty("Players", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(List<RunHistoryPlayer>), Array.Empty<Type>(), null)
		};
		array[12] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo13);
		array[12].IsGetNullable = false;
		array[12].IsSetNullable = false;
		JsonPropertyInfoValues<List<ModelId>> propertyInfo14 = new JsonPropertyInfoValues<List<ModelId>>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(RunHistory),
			Converter = null,
			Getter = (object obj) => ((RunHistory)obj).Acts,
			Setter = (object obj, List<ModelId>? value) =>
			{
				throw new InvalidOperationException("Setting init-only properties is not supported in source generation mode.");
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "Acts",
			JsonPropertyName = "acts",
			AttributeProviderFactory = () => typeof(RunHistory).GetProperty("Acts", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(List<ModelId>), Array.Empty<Type>(), null)
		};
		array[13] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo14);
		array[13].IsGetNullable = false;
		array[13].IsSetNullable = false;
		JsonPropertyInfoValues<List<SerializableModifier>> propertyInfo15 = new JsonPropertyInfoValues<List<SerializableModifier>>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(RunHistory),
			Converter = null,
			Getter = (object obj) => ((RunHistory)obj).Modifiers,
			Setter = (object obj, List<SerializableModifier>? value) =>
			{
				throw new InvalidOperationException("Setting init-only properties is not supported in source generation mode.");
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "Modifiers",
			JsonPropertyName = "modifiers",
			AttributeProviderFactory = () => typeof(RunHistory).GetProperty("Modifiers", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(List<SerializableModifier>), Array.Empty<Type>(), null)
		};
		array[14] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo15);
		array[14].IsGetNullable = false;
		array[14].IsSetNullable = false;
		JsonPropertyInfoValues<List<List<MapPointHistoryEntry>>> propertyInfo16 = new JsonPropertyInfoValues<List<List<MapPointHistoryEntry>>>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(RunHistory),
			Converter = null,
			Getter = (object obj) => ((RunHistory)obj).MapPointHistory,
			Setter = (object obj, List<List<MapPointHistoryEntry>>? value) =>
			{
				throw new InvalidOperationException("Setting init-only properties is not supported in source generation mode.");
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "MapPointHistory",
			JsonPropertyName = "map_point_history",
			AttributeProviderFactory = () => typeof(RunHistory).GetProperty("MapPointHistory", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(List<List<MapPointHistoryEntry>>), Array.Empty<Type>(), null)
		};
		array[15] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo16);
		array[15].IsGetNullable = false;
		array[15].IsSetNullable = false;
		return array;
	}

	private static JsonParameterInfoValues[] RunHistoryCtorParamInit()
	{
		return new JsonParameterInfoValues[15]
		{
			new JsonParameterInfoValues
			{
				Name = "PlatformType",
				ParameterType = typeof(PlatformType),
				Position = 0,
				IsNullable = false,
				IsMemberInitializer = true
			},
			new JsonParameterInfoValues
			{
				Name = "GameMode",
				ParameterType = typeof(GameMode),
				Position = 1,
				IsNullable = false,
				IsMemberInitializer = true
			},
			new JsonParameterInfoValues
			{
				Name = "Win",
				ParameterType = typeof(bool),
				Position = 2,
				IsNullable = false,
				IsMemberInitializer = true
			},
			new JsonParameterInfoValues
			{
				Name = "Seed",
				ParameterType = typeof(string),
				Position = 3,
				IsNullable = false,
				IsMemberInitializer = true
			},
			new JsonParameterInfoValues
			{
				Name = "StartTime",
				ParameterType = typeof(long),
				Position = 4,
				IsNullable = false,
				IsMemberInitializer = true
			},
			new JsonParameterInfoValues
			{
				Name = "RunTime",
				ParameterType = typeof(float),
				Position = 5,
				IsNullable = false,
				IsMemberInitializer = true
			},
			new JsonParameterInfoValues
			{
				Name = "Ascension",
				ParameterType = typeof(int),
				Position = 6,
				IsNullable = false,
				IsMemberInitializer = true
			},
			new JsonParameterInfoValues
			{
				Name = "BuildId",
				ParameterType = typeof(string),
				Position = 7,
				IsNullable = false,
				IsMemberInitializer = true
			},
			new JsonParameterInfoValues
			{
				Name = "WasAbandoned",
				ParameterType = typeof(bool),
				Position = 8,
				IsNullable = false,
				IsMemberInitializer = true
			},
			new JsonParameterInfoValues
			{
				Name = "KilledByEncounter",
				ParameterType = typeof(ModelId),
				Position = 9,
				IsNullable = false,
				IsMemberInitializer = true
			},
			new JsonParameterInfoValues
			{
				Name = "KilledByEvent",
				ParameterType = typeof(ModelId),
				Position = 10,
				IsNullable = false,
				IsMemberInitializer = true
			},
			new JsonParameterInfoValues
			{
				Name = "Players",
				ParameterType = typeof(List<RunHistoryPlayer>),
				Position = 11,
				IsNullable = false,
				IsMemberInitializer = true
			},
			new JsonParameterInfoValues
			{
				Name = "Acts",
				ParameterType = typeof(List<ModelId>),
				Position = 12,
				IsNullable = false,
				IsMemberInitializer = true
			},
			new JsonParameterInfoValues
			{
				Name = "Modifiers",
				ParameterType = typeof(List<SerializableModifier>),
				Position = 13,
				IsNullable = false,
				IsMemberInitializer = true
			},
			new JsonParameterInfoValues
			{
				Name = "MapPointHistory",
				ParameterType = typeof(List<List<MapPointHistoryEntry>>),
				Position = 14,
				IsNullable = false,
				IsMemberInitializer = true
			}
		};
	}

	private JsonTypeInfo<RunHistoryPlayer> Create_RunHistoryPlayer(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<RunHistoryPlayer> jsonTypeInfo))
		{
			JsonObjectInfoValues<RunHistoryPlayer> objectInfo = new JsonObjectInfoValues<RunHistoryPlayer>
			{
				ObjectCreator = null,
				ObjectWithParameterizedConstructorCreator = (object[] args) => new RunHistoryPlayer
				{
					Id = (ulong)args[0],
					Character = (ModelId)args[1]
				},
				PropertyMetadataInitializer = (JsonSerializerContext _) => RunHistoryPlayerPropInit(options),
				ConstructorParameterMetadataInitializer = RunHistoryPlayerCtorParamInit,
				ConstructorAttributeProviderFactory = () => typeof(RunHistoryPlayer).GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Array.Empty<Type>(), null),
				SerializeHandler = null
			};
			jsonTypeInfo = JsonMetadataServices.CreateObjectInfo(options, objectInfo);
			jsonTypeInfo.NumberHandling = null;
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private static JsonPropertyInfo[] RunHistoryPlayerPropInit(JsonSerializerOptions options)
	{
		JsonPropertyInfo[] array = new JsonPropertyInfo[7];
		JsonPropertyInfoValues<ulong> propertyInfo = new JsonPropertyInfoValues<ulong>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(RunHistoryPlayer),
			Converter = null,
			Getter = (object obj) => ((RunHistoryPlayer)obj).Id,
			Setter = (object obj, ulong value) =>
			{
				throw new InvalidOperationException("Setting init-only properties is not supported in source generation mode.");
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "Id",
			JsonPropertyName = "id",
			AttributeProviderFactory = () => typeof(RunHistoryPlayer).GetProperty("Id", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(ulong), Array.Empty<Type>(), null)
		};
		array[0] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo);
		JsonPropertyInfoValues<ModelId> propertyInfo2 = new JsonPropertyInfoValues<ModelId>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(RunHistoryPlayer),
			Converter = null,
			Getter = (object obj) => ((RunHistoryPlayer)obj).Character,
			Setter = (object obj, ModelId? value) =>
			{
				throw new InvalidOperationException("Setting init-only properties is not supported in source generation mode.");
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "Character",
			JsonPropertyName = "character",
			AttributeProviderFactory = () => typeof(RunHistoryPlayer).GetProperty("Character", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(ModelId), Array.Empty<Type>(), null)
		};
		array[1] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo2);
		array[1].IsGetNullable = false;
		array[1].IsSetNullable = false;
		JsonPropertyInfoValues<IEnumerable<SerializableCard>> propertyInfo3 = new JsonPropertyInfoValues<IEnumerable<SerializableCard>>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(RunHistoryPlayer),
			Converter = null,
			Getter = (object obj) => ((RunHistoryPlayer)obj).Deck,
			Setter = (object obj, IEnumerable<SerializableCard>? value) =>
			{
				((RunHistoryPlayer)obj).Deck = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "Deck",
			JsonPropertyName = "deck",
			AttributeProviderFactory = () => typeof(RunHistoryPlayer).GetProperty("Deck", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(IEnumerable<SerializableCard>), Array.Empty<Type>(), null)
		};
		array[2] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo3);
		array[2].IsGetNullable = false;
		array[2].IsSetNullable = false;
		JsonPropertyInfoValues<IEnumerable<SerializableRelic>> propertyInfo4 = new JsonPropertyInfoValues<IEnumerable<SerializableRelic>>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(RunHistoryPlayer),
			Converter = null,
			Getter = (object obj) => ((RunHistoryPlayer)obj).Relics,
			Setter = (object obj, IEnumerable<SerializableRelic>? value) =>
			{
				((RunHistoryPlayer)obj).Relics = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "Relics",
			JsonPropertyName = "relics",
			AttributeProviderFactory = () => typeof(RunHistoryPlayer).GetProperty("Relics", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(IEnumerable<SerializableRelic>), Array.Empty<Type>(), null)
		};
		array[3] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo4);
		array[3].IsGetNullable = false;
		array[3].IsSetNullable = false;
		JsonPropertyInfoValues<IEnumerable<SerializablePotion>> propertyInfo5 = new JsonPropertyInfoValues<IEnumerable<SerializablePotion>>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(RunHistoryPlayer),
			Converter = null,
			Getter = (object obj) => ((RunHistoryPlayer)obj).Potions,
			Setter = (object obj, IEnumerable<SerializablePotion>? value) =>
			{
				((RunHistoryPlayer)obj).Potions = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "Potions",
			JsonPropertyName = "potions",
			AttributeProviderFactory = () => typeof(RunHistoryPlayer).GetProperty("Potions", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(IEnumerable<SerializablePotion>), Array.Empty<Type>(), null)
		};
		array[4] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo5);
		array[4].IsGetNullable = false;
		array[4].IsSetNullable = false;
		JsonPropertyInfoValues<IEnumerable<SerializableBadge>> propertyInfo6 = new JsonPropertyInfoValues<IEnumerable<SerializableBadge>>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(RunHistoryPlayer),
			Converter = null,
			Getter = (object obj) => ((RunHistoryPlayer)obj).Badges,
			Setter = (object obj, IEnumerable<SerializableBadge>? value) =>
			{
				((RunHistoryPlayer)obj).Badges = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "Badges",
			JsonPropertyName = "badges",
			AttributeProviderFactory = () => typeof(RunHistoryPlayer).GetProperty("Badges", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(IEnumerable<SerializableBadge>), Array.Empty<Type>(), null)
		};
		array[5] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo6);
		array[5].IsGetNullable = false;
		array[5].IsSetNullable = false;
		JsonPropertyInfoValues<int> propertyInfo7 = new JsonPropertyInfoValues<int>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(RunHistoryPlayer),
			Converter = null,
			Getter = (object obj) => ((RunHistoryPlayer)obj).MaxPotionSlotCount,
			Setter = (object obj, int value) =>
			{
				((RunHistoryPlayer)obj).MaxPotionSlotCount = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "MaxPotionSlotCount",
			JsonPropertyName = "max_potion_slot_count",
			AttributeProviderFactory = () => typeof(RunHistoryPlayer).GetProperty("MaxPotionSlotCount", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(int), Array.Empty<Type>(), null)
		};
		array[6] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo7);
		return array;
	}

	private static JsonParameterInfoValues[] RunHistoryPlayerCtorParamInit()
	{
		return new JsonParameterInfoValues[2]
		{
			new JsonParameterInfoValues
			{
				Name = "Id",
				ParameterType = typeof(ulong),
				Position = 0,
				IsNullable = false,
				IsMemberInitializer = true
			},
			new JsonParameterInfoValues
			{
				Name = "Character",
				ParameterType = typeof(ModelId),
				Position = 1,
				IsNullable = false,
				IsMemberInitializer = true
			}
		};
	}

	private JsonTypeInfo<AncientCharacterStats> Create_AncientCharacterStats(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<AncientCharacterStats> jsonTypeInfo))
		{
			JsonObjectInfoValues<AncientCharacterStats> objectInfo = new JsonObjectInfoValues<AncientCharacterStats>
			{
				ObjectCreator = null,
				ObjectWithParameterizedConstructorCreator = (object[] args) => new AncientCharacterStats
				{
					Character = (ModelId)args[0]
				},
				PropertyMetadataInitializer = (JsonSerializerContext _) => AncientCharacterStatsPropInit(options),
				ConstructorParameterMetadataInitializer = AncientCharacterStatsCtorParamInit,
				ConstructorAttributeProviderFactory = () => typeof(AncientCharacterStats).GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Array.Empty<Type>(), null),
				SerializeHandler = null
			};
			jsonTypeInfo = JsonMetadataServices.CreateObjectInfo(options, objectInfo);
			jsonTypeInfo.NumberHandling = null;
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private static JsonPropertyInfo[] AncientCharacterStatsPropInit(JsonSerializerOptions options)
	{
		JsonPropertyInfo[] array = new JsonPropertyInfo[4];
		JsonPropertyInfoValues<ModelId> propertyInfo = new JsonPropertyInfoValues<ModelId>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(AncientCharacterStats),
			Converter = null,
			Getter = (object obj) => ((AncientCharacterStats)obj).Character,
			Setter = (object obj, ModelId? value) =>
			{
				throw new InvalidOperationException("Setting init-only properties is not supported in source generation mode.");
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "Character",
			JsonPropertyName = "character",
			AttributeProviderFactory = () => typeof(AncientCharacterStats).GetProperty("Character", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(ModelId), Array.Empty<Type>(), null)
		};
		array[0] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo);
		array[0].IsRequired = true;
		array[0].IsGetNullable = false;
		array[0].IsSetNullable = false;
		JsonPropertyInfoValues<int> propertyInfo2 = new JsonPropertyInfoValues<int>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(AncientCharacterStats),
			Converter = null,
			Getter = (object obj) => ((AncientCharacterStats)obj).Wins,
			Setter = (object obj, int value) =>
			{
				((AncientCharacterStats)obj).Wins = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "Wins",
			JsonPropertyName = "wins",
			AttributeProviderFactory = () => typeof(AncientCharacterStats).GetProperty("Wins", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(int), Array.Empty<Type>(), null)
		};
		array[1] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo2);
		JsonPropertyInfoValues<int> propertyInfo3 = new JsonPropertyInfoValues<int>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(AncientCharacterStats),
			Converter = null,
			Getter = (object obj) => ((AncientCharacterStats)obj).Losses,
			Setter = (object obj, int value) =>
			{
				((AncientCharacterStats)obj).Losses = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "Losses",
			JsonPropertyName = "losses",
			AttributeProviderFactory = () => typeof(AncientCharacterStats).GetProperty("Losses", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(int), Array.Empty<Type>(), null)
		};
		array[2] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo3);
		JsonPropertyInfoValues<int> propertyInfo4 = new JsonPropertyInfoValues<int>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(AncientCharacterStats),
			Converter = null,
			Getter = null,
			Setter = null,
			IgnoreCondition = JsonIgnoreCondition.Always,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "Visits",
			JsonPropertyName = null,
			AttributeProviderFactory = () => typeof(AncientCharacterStats).GetProperty("Visits", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(int), Array.Empty<Type>(), null)
		};
		array[3] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo4);
		return array;
	}

	private static JsonParameterInfoValues[] AncientCharacterStatsCtorParamInit()
	{
		return new JsonParameterInfoValues[1]
		{
			new JsonParameterInfoValues
			{
				Name = "Character",
				ParameterType = typeof(ModelId),
				Position = 0,
				IsNullable = false,
				IsMemberInitializer = true
			}
		};
	}

	private JsonTypeInfo<AncientStats> Create_AncientStats(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<AncientStats> jsonTypeInfo))
		{
			JsonObjectInfoValues<AncientStats> objectInfo = new JsonObjectInfoValues<AncientStats>
			{
				ObjectCreator = null,
				ObjectWithParameterizedConstructorCreator = (object[] args) => new AncientStats
				{
					Id = (ModelId)args[0],
					CharStats = (List<AncientCharacterStats>)args[1]
				},
				PropertyMetadataInitializer = (JsonSerializerContext _) => AncientStatsPropInit(options),
				ConstructorParameterMetadataInitializer = AncientStatsCtorParamInit,
				ConstructorAttributeProviderFactory = () => typeof(AncientStats).GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Array.Empty<Type>(), null),
				SerializeHandler = null
			};
			jsonTypeInfo = JsonMetadataServices.CreateObjectInfo(options, objectInfo);
			jsonTypeInfo.NumberHandling = null;
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private static JsonPropertyInfo[] AncientStatsPropInit(JsonSerializerOptions options)
	{
		JsonPropertyInfo[] array = new JsonPropertyInfo[5];
		JsonPropertyInfoValues<ModelId> propertyInfo = new JsonPropertyInfoValues<ModelId>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(AncientStats),
			Converter = null,
			Getter = (object obj) => ((AncientStats)obj).Id,
			Setter = (object obj, ModelId? value) =>
			{
				throw new InvalidOperationException("Setting init-only properties is not supported in source generation mode.");
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "Id",
			JsonPropertyName = "ancient_id",
			AttributeProviderFactory = () => typeof(AncientStats).GetProperty("Id", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(ModelId), Array.Empty<Type>(), null)
		};
		array[0] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo);
		array[0].IsRequired = true;
		array[0].IsGetNullable = false;
		array[0].IsSetNullable = false;
		JsonPropertyInfoValues<List<AncientCharacterStats>> propertyInfo2 = new JsonPropertyInfoValues<List<AncientCharacterStats>>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(AncientStats),
			Converter = null,
			Getter = (object obj) => ((AncientStats)obj).CharStats,
			Setter = (object obj, List<AncientCharacterStats>? value) =>
			{
				throw new InvalidOperationException("Setting init-only properties is not supported in source generation mode.");
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "CharStats",
			JsonPropertyName = "character_stats",
			AttributeProviderFactory = () => typeof(AncientStats).GetProperty("CharStats", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(List<AncientCharacterStats>), Array.Empty<Type>(), null)
		};
		array[1] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo2);
		array[1].IsGetNullable = false;
		array[1].IsSetNullable = false;
		JsonPropertyInfoValues<int> propertyInfo3 = new JsonPropertyInfoValues<int>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(AncientStats),
			Converter = null,
			Getter = null,
			Setter = null,
			IgnoreCondition = JsonIgnoreCondition.Always,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "TotalVisits",
			JsonPropertyName = null,
			AttributeProviderFactory = () => typeof(AncientStats).GetProperty("TotalVisits", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(int), Array.Empty<Type>(), null)
		};
		array[2] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo3);
		JsonPropertyInfoValues<int> propertyInfo4 = new JsonPropertyInfoValues<int>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(AncientStats),
			Converter = null,
			Getter = null,
			Setter = null,
			IgnoreCondition = JsonIgnoreCondition.Always,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "TotalWins",
			JsonPropertyName = null,
			AttributeProviderFactory = () => typeof(AncientStats).GetProperty("TotalWins", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(int), Array.Empty<Type>(), null)
		};
		array[3] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo4);
		JsonPropertyInfoValues<int> propertyInfo5 = new JsonPropertyInfoValues<int>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(AncientStats),
			Converter = null,
			Getter = null,
			Setter = null,
			IgnoreCondition = JsonIgnoreCondition.Always,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "TotalLosses",
			JsonPropertyName = null,
			AttributeProviderFactory = () => typeof(AncientStats).GetProperty("TotalLosses", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(int), Array.Empty<Type>(), null)
		};
		array[4] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo5);
		return array;
	}

	private static JsonParameterInfoValues[] AncientStatsCtorParamInit()
	{
		return new JsonParameterInfoValues[2]
		{
			new JsonParameterInfoValues
			{
				Name = "Id",
				ParameterType = typeof(ModelId),
				Position = 0,
				IsNullable = false,
				IsMemberInitializer = true
			},
			new JsonParameterInfoValues
			{
				Name = "CharStats",
				ParameterType = typeof(List<AncientCharacterStats>),
				Position = 1,
				IsNullable = false,
				IsMemberInitializer = true
			}
		};
	}

	private JsonTypeInfo<BadgeStats> Create_BadgeStats(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<BadgeStats> jsonTypeInfo))
		{
			JsonObjectInfoValues<BadgeStats> objectInfo = new JsonObjectInfoValues<BadgeStats>
			{
				ObjectCreator = null,
				ObjectWithParameterizedConstructorCreator = (object[] args) => new BadgeStats
				{
					Id = (string)args[0],
					Count = (int)args[1],
					Rarity = (BadgeRarity)args[2]
				},
				PropertyMetadataInitializer = (JsonSerializerContext _) => BadgeStatsPropInit(options),
				ConstructorParameterMetadataInitializer = BadgeStatsCtorParamInit,
				ConstructorAttributeProviderFactory = () => typeof(BadgeStats).GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Array.Empty<Type>(), null),
				SerializeHandler = null
			};
			jsonTypeInfo = JsonMetadataServices.CreateObjectInfo(options, objectInfo);
			jsonTypeInfo.NumberHandling = null;
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private static JsonPropertyInfo[] BadgeStatsPropInit(JsonSerializerOptions options)
	{
		JsonPropertyInfo[] array = new JsonPropertyInfo[3];
		JsonPropertyInfoValues<string> propertyInfo = new JsonPropertyInfoValues<string>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(BadgeStats),
			Converter = null,
			Getter = (object obj) => ((BadgeStats)obj).Id,
			Setter = (object obj, string? value) =>
			{
				throw new InvalidOperationException("Setting init-only properties is not supported in source generation mode.");
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "Id",
			JsonPropertyName = "id",
			AttributeProviderFactory = () => typeof(BadgeStats).GetProperty("Id", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(string), Array.Empty<Type>(), null)
		};
		array[0] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo);
		array[0].IsRequired = true;
		array[0].IsGetNullable = false;
		array[0].IsSetNullable = false;
		JsonPropertyInfoValues<int> propertyInfo2 = new JsonPropertyInfoValues<int>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(BadgeStats),
			Converter = null,
			Getter = (object obj) => ((BadgeStats)obj).Count,
			Setter = (object obj, int value) =>
			{
				((BadgeStats)obj).Count = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "Count",
			JsonPropertyName = "count",
			AttributeProviderFactory = () => typeof(BadgeStats).GetProperty("Count", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(int), Array.Empty<Type>(), null)
		};
		array[1] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo2);
		array[1].IsRequired = true;
		JsonPropertyInfoValues<BadgeRarity> propertyInfo3 = new JsonPropertyInfoValues<BadgeRarity>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(BadgeStats),
			Converter = null,
			Getter = (object obj) => ((BadgeStats)obj).Rarity,
			Setter = (object obj, BadgeRarity value) =>
			{
				((BadgeStats)obj).Rarity = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "Rarity",
			JsonPropertyName = "rarity",
			AttributeProviderFactory = () => typeof(BadgeStats).GetProperty("Rarity", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(BadgeRarity), Array.Empty<Type>(), null)
		};
		array[2] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo3);
		array[2].IsRequired = true;
		return array;
	}

	private static JsonParameterInfoValues[] BadgeStatsCtorParamInit()
	{
		return new JsonParameterInfoValues[3]
		{
			new JsonParameterInfoValues
			{
				Name = "Id",
				ParameterType = typeof(string),
				Position = 0,
				IsNullable = false,
				IsMemberInitializer = true
			},
			new JsonParameterInfoValues
			{
				Name = "Count",
				ParameterType = typeof(int),
				Position = 1,
				IsNullable = false,
				IsMemberInitializer = true
			},
			new JsonParameterInfoValues
			{
				Name = "Rarity",
				ParameterType = typeof(BadgeRarity),
				Position = 2,
				IsNullable = false,
				IsMemberInitializer = true
			}
		};
	}

	private JsonTypeInfo<CardStats> Create_CardStats(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<CardStats> jsonTypeInfo))
		{
			JsonObjectInfoValues<CardStats> objectInfo = new JsonObjectInfoValues<CardStats>
			{
				ObjectCreator = null,
				ObjectWithParameterizedConstructorCreator = (object[] args) => new CardStats
				{
					Id = (ModelId)args[0]
				},
				PropertyMetadataInitializer = (JsonSerializerContext _) => CardStatsPropInit(options),
				ConstructorParameterMetadataInitializer = CardStatsCtorParamInit,
				ConstructorAttributeProviderFactory = () => typeof(CardStats).GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Array.Empty<Type>(), null),
				SerializeHandler = null
			};
			jsonTypeInfo = JsonMetadataServices.CreateObjectInfo(options, objectInfo);
			jsonTypeInfo.NumberHandling = null;
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private static JsonPropertyInfo[] CardStatsPropInit(JsonSerializerOptions options)
	{
		JsonPropertyInfo[] array = new JsonPropertyInfo[5];
		JsonPropertyInfoValues<ModelId> propertyInfo = new JsonPropertyInfoValues<ModelId>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(CardStats),
			Converter = null,
			Getter = (object obj) => ((CardStats)obj).Id,
			Setter = (object obj, ModelId? value) =>
			{
				throw new InvalidOperationException("Setting init-only properties is not supported in source generation mode.");
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "Id",
			JsonPropertyName = "id",
			AttributeProviderFactory = () => typeof(CardStats).GetProperty("Id", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(ModelId), Array.Empty<Type>(), null)
		};
		array[0] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo);
		JsonPropertyInfoValues<long> propertyInfo2 = new JsonPropertyInfoValues<long>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(CardStats),
			Converter = null,
			Getter = (object obj) => ((CardStats)obj).TimesPicked,
			Setter = (object obj, long value) =>
			{
				((CardStats)obj).TimesPicked = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "TimesPicked",
			JsonPropertyName = "times_picked",
			AttributeProviderFactory = () => typeof(CardStats).GetProperty("TimesPicked", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(long), Array.Empty<Type>(), null)
		};
		array[1] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo2);
		JsonPropertyInfoValues<long> propertyInfo3 = new JsonPropertyInfoValues<long>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(CardStats),
			Converter = null,
			Getter = (object obj) => ((CardStats)obj).TimesSkipped,
			Setter = (object obj, long value) =>
			{
				((CardStats)obj).TimesSkipped = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "TimesSkipped",
			JsonPropertyName = "times_skipped",
			AttributeProviderFactory = () => typeof(CardStats).GetProperty("TimesSkipped", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(long), Array.Empty<Type>(), null)
		};
		array[2] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo3);
		JsonPropertyInfoValues<long> propertyInfo4 = new JsonPropertyInfoValues<long>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(CardStats),
			Converter = null,
			Getter = (object obj) => ((CardStats)obj).TimesWon,
			Setter = (object obj, long value) =>
			{
				((CardStats)obj).TimesWon = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "TimesWon",
			JsonPropertyName = "times_won",
			AttributeProviderFactory = () => typeof(CardStats).GetProperty("TimesWon", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(long), Array.Empty<Type>(), null)
		};
		array[3] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo4);
		JsonPropertyInfoValues<long> propertyInfo5 = new JsonPropertyInfoValues<long>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(CardStats),
			Converter = null,
			Getter = (object obj) => ((CardStats)obj).TimesLost,
			Setter = (object obj, long value) =>
			{
				((CardStats)obj).TimesLost = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "TimesLost",
			JsonPropertyName = "times_lost",
			AttributeProviderFactory = () => typeof(CardStats).GetProperty("TimesLost", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(long), Array.Empty<Type>(), null)
		};
		array[4] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo5);
		return array;
	}

	private static JsonParameterInfoValues[] CardStatsCtorParamInit()
	{
		return new JsonParameterInfoValues[1]
		{
			new JsonParameterInfoValues
			{
				Name = "Id",
				ParameterType = typeof(ModelId),
				Position = 0,
				IsNullable = true,
				IsMemberInitializer = true
			}
		};
	}

	private JsonTypeInfo<CharacterStats> Create_CharacterStats(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<CharacterStats> jsonTypeInfo))
		{
			JsonObjectInfoValues<CharacterStats> objectInfo = new JsonObjectInfoValues<CharacterStats>
			{
				ObjectCreator = null,
				ObjectWithParameterizedConstructorCreator = (object[] args) => new CharacterStats
				{
					Id = (ModelId)args[0]
				},
				PropertyMetadataInitializer = (JsonSerializerContext _) => CharacterStatsPropInit(options),
				ConstructorParameterMetadataInitializer = CharacterStatsCtorParamInit,
				ConstructorAttributeProviderFactory = () => typeof(CharacterStats).GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Array.Empty<Type>(), null),
				SerializeHandler = null
			};
			jsonTypeInfo = JsonMetadataServices.CreateObjectInfo(options, objectInfo);
			jsonTypeInfo.NumberHandling = null;
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private static JsonPropertyInfo[] CharacterStatsPropInit(JsonSerializerOptions options)
	{
		JsonPropertyInfo[] array = new JsonPropertyInfo[10];
		JsonPropertyInfoValues<ModelId> propertyInfo = new JsonPropertyInfoValues<ModelId>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(CharacterStats),
			Converter = null,
			Getter = (object obj) => ((CharacterStats)obj).Id,
			Setter = (object obj, ModelId? value) =>
			{
				throw new InvalidOperationException("Setting init-only properties is not supported in source generation mode.");
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "Id",
			JsonPropertyName = "id",
			AttributeProviderFactory = () => typeof(CharacterStats).GetProperty("Id", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(ModelId), Array.Empty<Type>(), null)
		};
		array[0] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo);
		JsonPropertyInfoValues<int> propertyInfo2 = new JsonPropertyInfoValues<int>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(CharacterStats),
			Converter = null,
			Getter = (object obj) => ((CharacterStats)obj).MaxAscension,
			Setter = (object obj, int value) =>
			{
				((CharacterStats)obj).MaxAscension = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "MaxAscension",
			JsonPropertyName = "max_ascension",
			AttributeProviderFactory = () => typeof(CharacterStats).GetProperty("MaxAscension", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(int), Array.Empty<Type>(), null)
		};
		array[1] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo2);
		JsonPropertyInfoValues<int> propertyInfo3 = new JsonPropertyInfoValues<int>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(CharacterStats),
			Converter = null,
			Getter = (object obj) => ((CharacterStats)obj).PreferredAscension,
			Setter = (object obj, int value) =>
			{
				((CharacterStats)obj).PreferredAscension = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "PreferredAscension",
			JsonPropertyName = "preferred_ascension",
			AttributeProviderFactory = () => typeof(CharacterStats).GetProperty("PreferredAscension", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(int), Array.Empty<Type>(), null)
		};
		array[2] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo3);
		JsonPropertyInfoValues<int> propertyInfo4 = new JsonPropertyInfoValues<int>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(CharacterStats),
			Converter = null,
			Getter = (object obj) => ((CharacterStats)obj).TotalWins,
			Setter = (object obj, int value) =>
			{
				((CharacterStats)obj).TotalWins = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "TotalWins",
			JsonPropertyName = "total_wins",
			AttributeProviderFactory = () => typeof(CharacterStats).GetProperty("TotalWins", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(int), Array.Empty<Type>(), null)
		};
		array[3] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo4);
		JsonPropertyInfoValues<int> propertyInfo5 = new JsonPropertyInfoValues<int>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(CharacterStats),
			Converter = null,
			Getter = (object obj) => ((CharacterStats)obj).TotalLosses,
			Setter = (object obj, int value) =>
			{
				((CharacterStats)obj).TotalLosses = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "TotalLosses",
			JsonPropertyName = "total_losses",
			AttributeProviderFactory = () => typeof(CharacterStats).GetProperty("TotalLosses", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(int), Array.Empty<Type>(), null)
		};
		array[4] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo5);
		JsonPropertyInfoValues<long> propertyInfo6 = new JsonPropertyInfoValues<long>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(CharacterStats),
			Converter = null,
			Getter = (object obj) => ((CharacterStats)obj).FastestWinTime,
			Setter = (object obj, long value) =>
			{
				((CharacterStats)obj).FastestWinTime = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "FastestWinTime",
			JsonPropertyName = "fastest_win_time",
			AttributeProviderFactory = () => typeof(CharacterStats).GetProperty("FastestWinTime", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(long), Array.Empty<Type>(), null)
		};
		array[5] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo6);
		JsonPropertyInfoValues<long> propertyInfo7 = new JsonPropertyInfoValues<long>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(CharacterStats),
			Converter = null,
			Getter = (object obj) => ((CharacterStats)obj).BestWinStreak,
			Setter = (object obj, long value) =>
			{
				((CharacterStats)obj).BestWinStreak = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "BestWinStreak",
			JsonPropertyName = "best_win_streak",
			AttributeProviderFactory = () => typeof(CharacterStats).GetProperty("BestWinStreak", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(long), Array.Empty<Type>(), null)
		};
		array[6] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo7);
		JsonPropertyInfoValues<long> propertyInfo8 = new JsonPropertyInfoValues<long>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(CharacterStats),
			Converter = null,
			Getter = (object obj) => ((CharacterStats)obj).CurrentWinStreak,
			Setter = (object obj, long value) =>
			{
				((CharacterStats)obj).CurrentWinStreak = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "CurrentWinStreak",
			JsonPropertyName = "current_streak",
			AttributeProviderFactory = () => typeof(CharacterStats).GetProperty("CurrentWinStreak", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(long), Array.Empty<Type>(), null)
		};
		array[7] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo8);
		JsonPropertyInfoValues<long> propertyInfo9 = new JsonPropertyInfoValues<long>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(CharacterStats),
			Converter = null,
			Getter = (object obj) => ((CharacterStats)obj).Playtime,
			Setter = (object obj, long value) =>
			{
				((CharacterStats)obj).Playtime = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "Playtime",
			JsonPropertyName = "playtime",
			AttributeProviderFactory = () => typeof(CharacterStats).GetProperty("Playtime", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(long), Array.Empty<Type>(), null)
		};
		array[8] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo9);
		JsonPropertyInfoValues<List<BadgeStats>> propertyInfo10 = new JsonPropertyInfoValues<List<BadgeStats>>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(CharacterStats),
			Converter = null,
			Getter = (object obj) => ((CharacterStats)obj).Badges,
			Setter = (object obj, List<BadgeStats>? value) =>
			{
				((CharacterStats)obj).Badges = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "Badges",
			JsonPropertyName = "badges",
			AttributeProviderFactory = () => typeof(CharacterStats).GetProperty("Badges", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(List<BadgeStats>), Array.Empty<Type>(), null)
		};
		array[9] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo10);
		array[9].IsGetNullable = false;
		array[9].IsSetNullable = false;
		return array;
	}

	private static JsonParameterInfoValues[] CharacterStatsCtorParamInit()
	{
		return new JsonParameterInfoValues[1]
		{
			new JsonParameterInfoValues
			{
				Name = "Id",
				ParameterType = typeof(ModelId),
				Position = 0,
				IsNullable = true,
				IsMemberInitializer = true
			}
		};
	}

	private JsonTypeInfo<EncounterStats> Create_EncounterStats(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<EncounterStats> jsonTypeInfo))
		{
			JsonObjectInfoValues<EncounterStats> objectInfo = new JsonObjectInfoValues<EncounterStats>
			{
				ObjectCreator = null,
				ObjectWithParameterizedConstructorCreator = (object[] args) => new EncounterStats
				{
					Id = (ModelId)args[0],
					FightStats = (List<FightStats>)args[1]
				},
				PropertyMetadataInitializer = (JsonSerializerContext _) => EncounterStatsPropInit(options),
				ConstructorParameterMetadataInitializer = EncounterStatsCtorParamInit,
				ConstructorAttributeProviderFactory = () => typeof(EncounterStats).GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Array.Empty<Type>(), null),
				SerializeHandler = null
			};
			jsonTypeInfo = JsonMetadataServices.CreateObjectInfo(options, objectInfo);
			jsonTypeInfo.NumberHandling = null;
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private static JsonPropertyInfo[] EncounterStatsPropInit(JsonSerializerOptions options)
	{
		JsonPropertyInfo[] array = new JsonPropertyInfo[4];
		JsonPropertyInfoValues<ModelId> propertyInfo = new JsonPropertyInfoValues<ModelId>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(EncounterStats),
			Converter = null,
			Getter = (object obj) => ((EncounterStats)obj).Id,
			Setter = (object obj, ModelId? value) =>
			{
				throw new InvalidOperationException("Setting init-only properties is not supported in source generation mode.");
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "Id",
			JsonPropertyName = "encounter_id",
			AttributeProviderFactory = () => typeof(EncounterStats).GetProperty("Id", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(ModelId), Array.Empty<Type>(), null)
		};
		array[0] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo);
		array[0].IsRequired = true;
		array[0].IsGetNullable = false;
		array[0].IsSetNullable = false;
		JsonPropertyInfoValues<List<FightStats>> propertyInfo2 = new JsonPropertyInfoValues<List<FightStats>>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(EncounterStats),
			Converter = null,
			Getter = (object obj) => ((EncounterStats)obj).FightStats,
			Setter = (object obj, List<FightStats>? value) =>
			{
				throw new InvalidOperationException("Setting init-only properties is not supported in source generation mode.");
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "FightStats",
			JsonPropertyName = "fight_stats",
			AttributeProviderFactory = () => typeof(EncounterStats).GetProperty("FightStats", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(List<FightStats>), Array.Empty<Type>(), null)
		};
		array[1] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo2);
		array[1].IsGetNullable = false;
		array[1].IsSetNullable = false;
		JsonPropertyInfoValues<int> propertyInfo3 = new JsonPropertyInfoValues<int>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(EncounterStats),
			Converter = null,
			Getter = null,
			Setter = null,
			IgnoreCondition = JsonIgnoreCondition.Always,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "TotalWins",
			JsonPropertyName = null,
			AttributeProviderFactory = () => typeof(EncounterStats).GetProperty("TotalWins", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(int), Array.Empty<Type>(), null)
		};
		array[2] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo3);
		JsonPropertyInfoValues<int> propertyInfo4 = new JsonPropertyInfoValues<int>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(EncounterStats),
			Converter = null,
			Getter = null,
			Setter = null,
			IgnoreCondition = JsonIgnoreCondition.Always,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "TotalLosses",
			JsonPropertyName = null,
			AttributeProviderFactory = () => typeof(EncounterStats).GetProperty("TotalLosses", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(int), Array.Empty<Type>(), null)
		};
		array[3] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo4);
		return array;
	}

	private static JsonParameterInfoValues[] EncounterStatsCtorParamInit()
	{
		return new JsonParameterInfoValues[2]
		{
			new JsonParameterInfoValues
			{
				Name = "Id",
				ParameterType = typeof(ModelId),
				Position = 0,
				IsNullable = false,
				IsMemberInitializer = true
			},
			new JsonParameterInfoValues
			{
				Name = "FightStats",
				ParameterType = typeof(List<FightStats>),
				Position = 1,
				IsNullable = false,
				IsMemberInitializer = true
			}
		};
	}

	private JsonTypeInfo<EnemyStats> Create_EnemyStats(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<EnemyStats> jsonTypeInfo))
		{
			JsonObjectInfoValues<EnemyStats> objectInfo = new JsonObjectInfoValues<EnemyStats>
			{
				ObjectCreator = null,
				ObjectWithParameterizedConstructorCreator = (object[] args) => new EnemyStats
				{
					Id = (ModelId)args[0],
					FightStats = (List<FightStats>)args[1]
				},
				PropertyMetadataInitializer = (JsonSerializerContext _) => EnemyStatsPropInit(options),
				ConstructorParameterMetadataInitializer = EnemyStatsCtorParamInit,
				ConstructorAttributeProviderFactory = () => typeof(EnemyStats).GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Array.Empty<Type>(), null),
				SerializeHandler = null
			};
			jsonTypeInfo = JsonMetadataServices.CreateObjectInfo(options, objectInfo);
			jsonTypeInfo.NumberHandling = null;
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private static JsonPropertyInfo[] EnemyStatsPropInit(JsonSerializerOptions options)
	{
		JsonPropertyInfo[] array = new JsonPropertyInfo[4];
		JsonPropertyInfoValues<ModelId> propertyInfo = new JsonPropertyInfoValues<ModelId>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(EnemyStats),
			Converter = null,
			Getter = (object obj) => ((EnemyStats)obj).Id,
			Setter = (object obj, ModelId? value) =>
			{
				throw new InvalidOperationException("Setting init-only properties is not supported in source generation mode.");
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "Id",
			JsonPropertyName = "enemy_id",
			AttributeProviderFactory = () => typeof(EnemyStats).GetProperty("Id", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(ModelId), Array.Empty<Type>(), null)
		};
		array[0] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo);
		array[0].IsRequired = true;
		array[0].IsGetNullable = false;
		array[0].IsSetNullable = false;
		JsonPropertyInfoValues<List<FightStats>> propertyInfo2 = new JsonPropertyInfoValues<List<FightStats>>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(EnemyStats),
			Converter = null,
			Getter = (object obj) => ((EnemyStats)obj).FightStats,
			Setter = (object obj, List<FightStats>? value) =>
			{
				throw new InvalidOperationException("Setting init-only properties is not supported in source generation mode.");
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "FightStats",
			JsonPropertyName = "fight_stats",
			AttributeProviderFactory = () => typeof(EnemyStats).GetProperty("FightStats", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(List<FightStats>), Array.Empty<Type>(), null)
		};
		array[1] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo2);
		array[1].IsGetNullable = false;
		array[1].IsSetNullable = false;
		JsonPropertyInfoValues<int> propertyInfo3 = new JsonPropertyInfoValues<int>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(EnemyStats),
			Converter = null,
			Getter = null,
			Setter = null,
			IgnoreCondition = JsonIgnoreCondition.Always,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "TotalWins",
			JsonPropertyName = null,
			AttributeProviderFactory = () => typeof(EnemyStats).GetProperty("TotalWins", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(int), Array.Empty<Type>(), null)
		};
		array[2] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo3);
		JsonPropertyInfoValues<int> propertyInfo4 = new JsonPropertyInfoValues<int>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(EnemyStats),
			Converter = null,
			Getter = null,
			Setter = null,
			IgnoreCondition = JsonIgnoreCondition.Always,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "TotalLosses",
			JsonPropertyName = null,
			AttributeProviderFactory = () => typeof(EnemyStats).GetProperty("TotalLosses", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(int), Array.Empty<Type>(), null)
		};
		array[3] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo4);
		return array;
	}

	private static JsonParameterInfoValues[] EnemyStatsCtorParamInit()
	{
		return new JsonParameterInfoValues[2]
		{
			new JsonParameterInfoValues
			{
				Name = "Id",
				ParameterType = typeof(ModelId),
				Position = 0,
				IsNullable = false,
				IsMemberInitializer = true
			},
			new JsonParameterInfoValues
			{
				Name = "FightStats",
				ParameterType = typeof(List<FightStats>),
				Position = 1,
				IsNullable = false,
				IsMemberInitializer = true
			}
		};
	}

	private JsonTypeInfo<EpochState> Create_EpochState(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<EpochState> jsonTypeInfo))
		{
			jsonTypeInfo = JsonMetadataServices.CreateValueInfo<EpochState>(options, JsonMetadataServices.GetEnumConverter<EpochState>(options));
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private JsonTypeInfo<FightStats> Create_FightStats(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<FightStats> jsonTypeInfo))
		{
			JsonObjectInfoValues<FightStats> objectInfo = new JsonObjectInfoValues<FightStats>
			{
				ObjectCreator = null,
				ObjectWithParameterizedConstructorCreator = (object[] args) => new FightStats
				{
					Character = (ModelId)args[0]
				},
				PropertyMetadataInitializer = (JsonSerializerContext _) => FightStatsPropInit(options),
				ConstructorParameterMetadataInitializer = FightStatsCtorParamInit,
				ConstructorAttributeProviderFactory = () => typeof(FightStats).GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Array.Empty<Type>(), null),
				SerializeHandler = null
			};
			jsonTypeInfo = JsonMetadataServices.CreateObjectInfo(options, objectInfo);
			jsonTypeInfo.NumberHandling = null;
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private static JsonPropertyInfo[] FightStatsPropInit(JsonSerializerOptions options)
	{
		JsonPropertyInfo[] array = new JsonPropertyInfo[3];
		JsonPropertyInfoValues<ModelId> propertyInfo = new JsonPropertyInfoValues<ModelId>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(FightStats),
			Converter = null,
			Getter = (object obj) => ((FightStats)obj).Character,
			Setter = (object obj, ModelId? value) =>
			{
				throw new InvalidOperationException("Setting init-only properties is not supported in source generation mode.");
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "Character",
			JsonPropertyName = "character",
			AttributeProviderFactory = () => typeof(FightStats).GetProperty("Character", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(ModelId), Array.Empty<Type>(), null)
		};
		array[0] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo);
		array[0].IsRequired = true;
		array[0].IsGetNullable = false;
		array[0].IsSetNullable = false;
		JsonPropertyInfoValues<int> propertyInfo2 = new JsonPropertyInfoValues<int>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(FightStats),
			Converter = null,
			Getter = (object obj) => ((FightStats)obj).Wins,
			Setter = (object obj, int value) =>
			{
				((FightStats)obj).Wins = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "Wins",
			JsonPropertyName = "wins",
			AttributeProviderFactory = () => typeof(FightStats).GetProperty("Wins", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(int), Array.Empty<Type>(), null)
		};
		array[1] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo2);
		JsonPropertyInfoValues<int> propertyInfo3 = new JsonPropertyInfoValues<int>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(FightStats),
			Converter = null,
			Getter = (object obj) => ((FightStats)obj).Losses,
			Setter = (object obj, int value) =>
			{
				((FightStats)obj).Losses = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "Losses",
			JsonPropertyName = "losses",
			AttributeProviderFactory = () => typeof(FightStats).GetProperty("Losses", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(int), Array.Empty<Type>(), null)
		};
		array[2] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo3);
		return array;
	}

	private static JsonParameterInfoValues[] FightStatsCtorParamInit()
	{
		return new JsonParameterInfoValues[1]
		{
			new JsonParameterInfoValues
			{
				Name = "Character",
				ParameterType = typeof(ModelId),
				Position = 0,
				IsNullable = false,
				IsMemberInitializer = true
			}
		};
	}

	private JsonTypeInfo<SerializableMapDrawingLine> Create_SerializableMapDrawingLine(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<SerializableMapDrawingLine> jsonTypeInfo))
		{
			JsonObjectInfoValues<SerializableMapDrawingLine> objectInfo = new JsonObjectInfoValues<SerializableMapDrawingLine>
			{
				ObjectCreator = () => new SerializableMapDrawingLine(),
				ObjectWithParameterizedConstructorCreator = null,
				PropertyMetadataInitializer = (JsonSerializerContext _) => SerializableMapDrawingLinePropInit(options),
				ConstructorParameterMetadataInitializer = null,
				ConstructorAttributeProviderFactory = () => typeof(SerializableMapDrawingLine).GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Array.Empty<Type>(), null),
				SerializeHandler = null
			};
			jsonTypeInfo = JsonMetadataServices.CreateObjectInfo(options, objectInfo);
			jsonTypeInfo.NumberHandling = null;
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private static JsonPropertyInfo[] SerializableMapDrawingLinePropInit(JsonSerializerOptions options)
	{
		JsonPropertyInfo[] array = new JsonPropertyInfo[2];
		JsonPropertyInfoValues<bool> propertyInfo = new JsonPropertyInfoValues<bool>
		{
			IsProperty = false,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializableMapDrawingLine),
			Converter = null,
			Getter = (object obj) => ((SerializableMapDrawingLine)obj).isEraser,
			Setter = (object obj, bool value) =>
			{
				((SerializableMapDrawingLine)obj).isEraser = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "isEraser",
			JsonPropertyName = null,
			AttributeProviderFactory = () => typeof(SerializableMapDrawingLine).GetField("isEraser", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
		};
		array[0] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo);
		JsonPropertyInfoValues<List<Vector2>> propertyInfo2 = new JsonPropertyInfoValues<List<Vector2>>
		{
			IsProperty = false,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializableMapDrawingLine),
			Converter = null,
			Getter = (object obj) => ((SerializableMapDrawingLine)obj).mapPoints,
			Setter = (object obj, List<Vector2>? value) =>
			{
				((SerializableMapDrawingLine)obj).mapPoints = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "mapPoints",
			JsonPropertyName = null,
			AttributeProviderFactory = () => typeof(SerializableMapDrawingLine).GetField("mapPoints", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
		};
		array[1] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo2);
		array[1].IsGetNullable = false;
		array[1].IsSetNullable = false;
		return array;
	}

	private JsonTypeInfo<SerializableMapDrawings> Create_SerializableMapDrawings(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<SerializableMapDrawings> jsonTypeInfo))
		{
			JsonObjectInfoValues<SerializableMapDrawings> objectInfo = new JsonObjectInfoValues<SerializableMapDrawings>
			{
				ObjectCreator = () => new SerializableMapDrawings(),
				ObjectWithParameterizedConstructorCreator = null,
				PropertyMetadataInitializer = (JsonSerializerContext _) => SerializableMapDrawingsPropInit(options),
				ConstructorParameterMetadataInitializer = null,
				ConstructorAttributeProviderFactory = () => typeof(SerializableMapDrawings).GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Array.Empty<Type>(), null),
				SerializeHandler = null
			};
			jsonTypeInfo = JsonMetadataServices.CreateObjectInfo(options, objectInfo);
			jsonTypeInfo.NumberHandling = null;
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private static JsonPropertyInfo[] SerializableMapDrawingsPropInit(JsonSerializerOptions options)
	{
		JsonPropertyInfo[] array = new JsonPropertyInfo[1];
		JsonPropertyInfoValues<List<SerializablePlayerMapDrawings>> propertyInfo = new JsonPropertyInfoValues<List<SerializablePlayerMapDrawings>>
		{
			IsProperty = false,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializableMapDrawings),
			Converter = null,
			Getter = (object obj) => ((SerializableMapDrawings)obj).drawings,
			Setter = (object obj, List<SerializablePlayerMapDrawings>? value) =>
			{
				((SerializableMapDrawings)obj).drawings = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "drawings",
			JsonPropertyName = null,
			AttributeProviderFactory = () => typeof(SerializableMapDrawings).GetField("drawings", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
		};
		array[0] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo);
		array[0].IsGetNullable = false;
		array[0].IsSetNullable = false;
		return array;
	}

	private JsonTypeInfo<SerializablePlayerMapDrawings> Create_SerializablePlayerMapDrawings(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<SerializablePlayerMapDrawings> jsonTypeInfo))
		{
			JsonObjectInfoValues<SerializablePlayerMapDrawings> objectInfo = new JsonObjectInfoValues<SerializablePlayerMapDrawings>
			{
				ObjectCreator = () => new SerializablePlayerMapDrawings(),
				ObjectWithParameterizedConstructorCreator = null,
				PropertyMetadataInitializer = (JsonSerializerContext _) => SerializablePlayerMapDrawingsPropInit(options),
				ConstructorParameterMetadataInitializer = null,
				ConstructorAttributeProviderFactory = () => typeof(SerializablePlayerMapDrawings).GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Array.Empty<Type>(), null),
				SerializeHandler = null
			};
			jsonTypeInfo = JsonMetadataServices.CreateObjectInfo(options, objectInfo);
			jsonTypeInfo.NumberHandling = null;
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private static JsonPropertyInfo[] SerializablePlayerMapDrawingsPropInit(JsonSerializerOptions options)
	{
		JsonPropertyInfo[] array = new JsonPropertyInfo[2];
		JsonPropertyInfoValues<ulong> propertyInfo = new JsonPropertyInfoValues<ulong>
		{
			IsProperty = false,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializablePlayerMapDrawings),
			Converter = null,
			Getter = (object obj) => ((SerializablePlayerMapDrawings)obj).playerId,
			Setter = (object obj, ulong value) =>
			{
				((SerializablePlayerMapDrawings)obj).playerId = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "playerId",
			JsonPropertyName = null,
			AttributeProviderFactory = () => typeof(SerializablePlayerMapDrawings).GetField("playerId", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
		};
		array[0] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo);
		JsonPropertyInfoValues<List<SerializableMapDrawingLine>> propertyInfo2 = new JsonPropertyInfoValues<List<SerializableMapDrawingLine>>
		{
			IsProperty = false,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializablePlayerMapDrawings),
			Converter = null,
			Getter = (object obj) => ((SerializablePlayerMapDrawings)obj).lines,
			Setter = (object obj, List<SerializableMapDrawingLine>? value) =>
			{
				((SerializablePlayerMapDrawings)obj).lines = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "lines",
			JsonPropertyName = null,
			AttributeProviderFactory = () => typeof(SerializablePlayerMapDrawings).GetField("lines", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
		};
		array[1] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo2);
		array[1].IsGetNullable = false;
		array[1].IsSetNullable = false;
		return array;
	}

	private JsonTypeInfo<MigratingData> Create_MigratingData(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<MigratingData> jsonTypeInfo))
		{
			JsonObjectInfoValues<MigratingData> objectInfo = new JsonObjectInfoValues<MigratingData>
			{
				ObjectCreator = null,
				ObjectWithParameterizedConstructorCreator = null,
				PropertyMetadataInitializer = (JsonSerializerContext _) => MigratingDataPropInit(options),
				ConstructorParameterMetadataInitializer = null,
				ConstructorAttributeProviderFactory = null,
				SerializeHandler = null
			};
			jsonTypeInfo = JsonMetadataServices.CreateObjectInfo(options, objectInfo);
			jsonTypeInfo.NumberHandling = null;
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private static JsonPropertyInfo[] MigratingDataPropInit(JsonSerializerOptions options)
	{
		return new JsonPropertyInfo[0];
	}

	private JsonTypeInfo<PrefsSave> Create_PrefsSave(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<PrefsSave> jsonTypeInfo))
		{
			JsonObjectInfoValues<PrefsSave> objectInfo = new JsonObjectInfoValues<PrefsSave>
			{
				ObjectCreator = () => new PrefsSave(),
				ObjectWithParameterizedConstructorCreator = null,
				PropertyMetadataInitializer = (JsonSerializerContext _) => PrefsSavePropInit(options),
				ConstructorParameterMetadataInitializer = null,
				ConstructorAttributeProviderFactory = () => typeof(PrefsSave).GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Array.Empty<Type>(), null),
				SerializeHandler = null
			};
			jsonTypeInfo = JsonMetadataServices.CreateObjectInfo(options, objectInfo);
			jsonTypeInfo.NumberHandling = null;
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private static JsonPropertyInfo[] PrefsSavePropInit(JsonSerializerOptions options)
	{
		JsonPropertyInfo[] array = new JsonPropertyInfo[13];
		JsonPropertyInfoValues<int> propertyInfo = new JsonPropertyInfoValues<int>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(PrefsSave),
			Converter = null,
			Getter = (object obj) => ((PrefsSave)obj).SchemaVersion,
			Setter = (object obj, int value) =>
			{
				((PrefsSave)obj).SchemaVersion = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "SchemaVersion",
			JsonPropertyName = "schema_version",
			AttributeProviderFactory = () => typeof(PrefsSave).GetProperty("SchemaVersion", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(int), Array.Empty<Type>(), null)
		};
		array[0] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo);
		JsonPropertyInfoValues<FastModeType> propertyInfo2 = new JsonPropertyInfoValues<FastModeType>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(PrefsSave),
			Converter = null,
			Getter = (object obj) => ((PrefsSave)obj).FastMode,
			Setter = (object obj, FastModeType value) =>
			{
				((PrefsSave)obj).FastMode = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "FastMode",
			JsonPropertyName = "fast_mode",
			AttributeProviderFactory = () => typeof(PrefsSave).GetProperty("FastMode", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(FastModeType), Array.Empty<Type>(), null)
		};
		array[1] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo2);
		JsonPropertyInfoValues<bool> propertyInfo3 = new JsonPropertyInfoValues<bool>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(PrefsSave),
			Converter = null,
			Getter = (object obj) => ((PrefsSave)obj).PhobiaMode,
			Setter = (object obj, bool value) =>
			{
				((PrefsSave)obj).PhobiaMode = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "PhobiaMode",
			JsonPropertyName = "phobia_mode",
			AttributeProviderFactory = () => typeof(PrefsSave).GetProperty("PhobiaMode", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(bool), Array.Empty<Type>(), null)
		};
		array[2] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo3);
		JsonPropertyInfoValues<int> propertyInfo4 = new JsonPropertyInfoValues<int>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(PrefsSave),
			Converter = null,
			Getter = (object obj) => ((PrefsSave)obj).ScreenShakeOptionIndex,
			Setter = (object obj, int value) =>
			{
				((PrefsSave)obj).ScreenShakeOptionIndex = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "ScreenShakeOptionIndex",
			JsonPropertyName = "screenshake",
			AttributeProviderFactory = () => typeof(PrefsSave).GetProperty("ScreenShakeOptionIndex", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(int), Array.Empty<Type>(), null)
		};
		array[3] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo4);
		JsonPropertyInfoValues<bool> propertyInfo5 = new JsonPropertyInfoValues<bool>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(PrefsSave),
			Converter = null,
			Getter = (object obj) => ((PrefsSave)obj).ShowRunTimer,
			Setter = (object obj, bool value) =>
			{
				((PrefsSave)obj).ShowRunTimer = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "ShowRunTimer",
			JsonPropertyName = "show_run_timer",
			AttributeProviderFactory = () => typeof(PrefsSave).GetProperty("ShowRunTimer", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(bool), Array.Empty<Type>(), null)
		};
		array[4] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo5);
		JsonPropertyInfoValues<bool> propertyInfo6 = new JsonPropertyInfoValues<bool>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(PrefsSave),
			Converter = null,
			Getter = (object obj) => ((PrefsSave)obj).ShowCardIndices,
			Setter = (object obj, bool value) =>
			{
				((PrefsSave)obj).ShowCardIndices = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "ShowCardIndices",
			JsonPropertyName = "show_card_indices",
			AttributeProviderFactory = () => typeof(PrefsSave).GetProperty("ShowCardIndices", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(bool), Array.Empty<Type>(), null)
		};
		array[5] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo6);
		JsonPropertyInfoValues<bool> propertyInfo7 = new JsonPropertyInfoValues<bool>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(PrefsSave),
			Converter = null,
			Getter = (object obj) => ((PrefsSave)obj).UploadData,
			Setter = (object obj, bool value) =>
			{
				((PrefsSave)obj).UploadData = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "UploadData",
			JsonPropertyName = "upload_data",
			AttributeProviderFactory = () => typeof(PrefsSave).GetProperty("UploadData", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(bool), Array.Empty<Type>(), null)
		};
		array[6] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo7);
		JsonPropertyInfoValues<bool> propertyInfo8 = new JsonPropertyInfoValues<bool>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(PrefsSave),
			Converter = null,
			Getter = (object obj) => ((PrefsSave)obj).MuteInBackground,
			Setter = (object obj, bool value) =>
			{
				((PrefsSave)obj).MuteInBackground = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "MuteInBackground",
			JsonPropertyName = "mute_in_background",
			AttributeProviderFactory = () => typeof(PrefsSave).GetProperty("MuteInBackground", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(bool), Array.Empty<Type>(), null)
		};
		array[7] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo8);
		JsonPropertyInfoValues<bool> propertyInfo9 = new JsonPropertyInfoValues<bool>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(PrefsSave),
			Converter = null,
			Getter = (object obj) => ((PrefsSave)obj).IsLongPressEnabled,
			Setter = (object obj, bool value) =>
			{
				((PrefsSave)obj).IsLongPressEnabled = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "IsLongPressEnabled",
			JsonPropertyName = "long_press",
			AttributeProviderFactory = () => typeof(PrefsSave).GetProperty("IsLongPressEnabled", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(bool), Array.Empty<Type>(), null)
		};
		array[8] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo9);
		JsonPropertyInfoValues<bool> propertyInfo10 = new JsonPropertyInfoValues<bool>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(PrefsSave),
			Converter = null,
			Getter = (object obj) => ((PrefsSave)obj).TextEffectsEnabled,
			Setter = (object obj, bool value) =>
			{
				((PrefsSave)obj).TextEffectsEnabled = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "TextEffectsEnabled",
			JsonPropertyName = "text_effects_enabled",
			AttributeProviderFactory = () => typeof(PrefsSave).GetProperty("TextEffectsEnabled", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(bool), Array.Empty<Type>(), null)
		};
		array[9] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo10);
		JsonPropertyInfoValues<bool> propertyInfo11 = new JsonPropertyInfoValues<bool>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(PrefsSave),
			Converter = null,
			Getter = (object obj) => ((PrefsSave)obj).ShowMultiplayerDrawings,
			Setter = (object obj, bool value) =>
			{
				((PrefsSave)obj).ShowMultiplayerDrawings = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "ShowMultiplayerDrawings",
			JsonPropertyName = "show_mp_drawings",
			AttributeProviderFactory = () => typeof(PrefsSave).GetProperty("ShowMultiplayerDrawings", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(bool), Array.Empty<Type>(), null)
		};
		array[10] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo11);
		JsonPropertyInfoValues<bool> propertyInfo12 = new JsonPropertyInfoValues<bool>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(PrefsSave),
			Converter = null,
			Getter = (object obj) => ((PrefsSave)obj).IsBestiaryActionsPreferred,
			Setter = (object obj, bool value) =>
			{
				((PrefsSave)obj).IsBestiaryActionsPreferred = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "IsBestiaryActionsPreferred",
			JsonPropertyName = "bestiary_actions_preferred",
			AttributeProviderFactory = () => typeof(PrefsSave).GetProperty("IsBestiaryActionsPreferred", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(bool), Array.Empty<Type>(), null)
		};
		array[11] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo12);
		JsonPropertyInfoValues<bool> propertyInfo13 = new JsonPropertyInfoValues<bool>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(PrefsSave),
			Converter = null,
			Getter = (object obj) => ((PrefsSave)obj).KeyboardMode,
			Setter = (object obj, bool value) =>
			{
				((PrefsSave)obj).KeyboardMode = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "KeyboardMode",
			JsonPropertyName = "keyboard_mode",
			AttributeProviderFactory = () => typeof(PrefsSave).GetProperty("KeyboardMode", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(bool), Array.Empty<Type>(), null)
		};
		array[12] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo13);
		return array;
	}

	private JsonTypeInfo<ProfileSave> Create_ProfileSave(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<ProfileSave> jsonTypeInfo))
		{
			JsonObjectInfoValues<ProfileSave> objectInfo = new JsonObjectInfoValues<ProfileSave>
			{
				ObjectCreator = () => new ProfileSave(),
				ObjectWithParameterizedConstructorCreator = null,
				PropertyMetadataInitializer = (JsonSerializerContext _) => ProfileSavePropInit(options),
				ConstructorParameterMetadataInitializer = null,
				ConstructorAttributeProviderFactory = () => typeof(ProfileSave).GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Array.Empty<Type>(), null),
				SerializeHandler = null
			};
			jsonTypeInfo = JsonMetadataServices.CreateObjectInfo(options, objectInfo);
			jsonTypeInfo.NumberHandling = null;
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private static JsonPropertyInfo[] ProfileSavePropInit(JsonSerializerOptions options)
	{
		JsonPropertyInfo[] array = new JsonPropertyInfo[2];
		JsonPropertyInfoValues<int> propertyInfo = new JsonPropertyInfoValues<int>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(ProfileSave),
			Converter = null,
			Getter = (object obj) => ((ProfileSave)obj).SchemaVersion,
			Setter = (object obj, int value) =>
			{
				((ProfileSave)obj).SchemaVersion = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "SchemaVersion",
			JsonPropertyName = "schema_version",
			AttributeProviderFactory = () => typeof(ProfileSave).GetProperty("SchemaVersion", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(int), Array.Empty<Type>(), null)
		};
		array[0] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo);
		JsonPropertyInfoValues<int> propertyInfo2 = new JsonPropertyInfoValues<int>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(ProfileSave),
			Converter = null,
			Getter = (object obj) => ((ProfileSave)obj).LastProfileId,
			Setter = (object obj, int value) =>
			{
				((ProfileSave)obj).LastProfileId = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "LastProfileId",
			JsonPropertyName = "last_profile_id",
			AttributeProviderFactory = () => typeof(ProfileSave).GetProperty("LastProfileId", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(int), Array.Empty<Type>(), null)
		};
		array[1] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo2);
		return array;
	}

	private JsonTypeInfo<SavedProperties> Create_SavedProperties(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<SavedProperties> jsonTypeInfo))
		{
			JsonObjectInfoValues<SavedProperties> objectInfo = new JsonObjectInfoValues<SavedProperties>
			{
				ObjectCreator = () => new SavedProperties(),
				ObjectWithParameterizedConstructorCreator = null,
				PropertyMetadataInitializer = (JsonSerializerContext _) => SavedPropertiesPropInit(options),
				ConstructorParameterMetadataInitializer = null,
				ConstructorAttributeProviderFactory = () => typeof(SavedProperties).GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Array.Empty<Type>(), null),
				SerializeHandler = null
			};
			jsonTypeInfo = JsonMetadataServices.CreateObjectInfo(options, objectInfo);
			jsonTypeInfo.NumberHandling = null;
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private static JsonPropertyInfo[] SavedPropertiesPropInit(JsonSerializerOptions options)
	{
		JsonPropertyInfo[] array = new JsonPropertyInfo[7];
		JsonPropertyInfoValues<List<SavedProperties.SavedProperty<int>>> propertyInfo = new JsonPropertyInfoValues<List<SavedProperties.SavedProperty<int>>>
		{
			IsProperty = false,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SavedProperties),
			Converter = null,
			Getter = (object obj) => ((SavedProperties)obj).ints,
			Setter = (object obj, List<SavedProperties.SavedProperty<int>>? value) =>
			{
				((SavedProperties)obj).ints = value;
			},
			IgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "ints",
			JsonPropertyName = "ints",
			AttributeProviderFactory = () => typeof(SavedProperties).GetField("ints", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
		};
		array[0] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo);
		JsonPropertyInfoValues<List<SavedProperties.SavedProperty<bool>>> propertyInfo2 = new JsonPropertyInfoValues<List<SavedProperties.SavedProperty<bool>>>
		{
			IsProperty = false,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SavedProperties),
			Converter = null,
			Getter = (object obj) => ((SavedProperties)obj).bools,
			Setter = (object obj, List<SavedProperties.SavedProperty<bool>>? value) =>
			{
				((SavedProperties)obj).bools = value;
			},
			IgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "bools",
			JsonPropertyName = "bools",
			AttributeProviderFactory = () => typeof(SavedProperties).GetField("bools", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
		};
		array[1] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo2);
		JsonPropertyInfoValues<List<SavedProperties.SavedProperty<string>>> propertyInfo3 = new JsonPropertyInfoValues<List<SavedProperties.SavedProperty<string>>>
		{
			IsProperty = false,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SavedProperties),
			Converter = null,
			Getter = (object obj) => ((SavedProperties)obj).strings,
			Setter = (object obj, List<SavedProperties.SavedProperty<string>>? value) =>
			{
				((SavedProperties)obj).strings = value;
			},
			IgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "strings",
			JsonPropertyName = "strings",
			AttributeProviderFactory = () => typeof(SavedProperties).GetField("strings", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
		};
		array[2] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo3);
		JsonPropertyInfoValues<List<SavedProperties.SavedProperty<int[]>>> propertyInfo4 = new JsonPropertyInfoValues<List<SavedProperties.SavedProperty<int[]>>>
		{
			IsProperty = false,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SavedProperties),
			Converter = null,
			Getter = (object obj) => ((SavedProperties)obj).intArrays,
			Setter = (object obj, List<SavedProperties.SavedProperty<int[]>>? value) =>
			{
				((SavedProperties)obj).intArrays = value;
			},
			IgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "intArrays",
			JsonPropertyName = "int_arrays",
			AttributeProviderFactory = () => typeof(SavedProperties).GetField("intArrays", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
		};
		array[3] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo4);
		JsonPropertyInfoValues<List<SavedProperties.SavedProperty<ModelId>>> propertyInfo5 = new JsonPropertyInfoValues<List<SavedProperties.SavedProperty<ModelId>>>
		{
			IsProperty = false,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SavedProperties),
			Converter = null,
			Getter = (object obj) => ((SavedProperties)obj).modelIds,
			Setter = (object obj, List<SavedProperties.SavedProperty<ModelId>>? value) =>
			{
				((SavedProperties)obj).modelIds = value;
			},
			IgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "modelIds",
			JsonPropertyName = "model_ids",
			AttributeProviderFactory = () => typeof(SavedProperties).GetField("modelIds", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
		};
		array[4] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo5);
		JsonPropertyInfoValues<List<SavedProperties.SavedProperty<SerializableCard>>> propertyInfo6 = new JsonPropertyInfoValues<List<SavedProperties.SavedProperty<SerializableCard>>>
		{
			IsProperty = false,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SavedProperties),
			Converter = null,
			Getter = (object obj) => ((SavedProperties)obj).cards,
			Setter = (object obj, List<SavedProperties.SavedProperty<SerializableCard>>? value) =>
			{
				((SavedProperties)obj).cards = value;
			},
			IgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "cards",
			JsonPropertyName = "cards",
			AttributeProviderFactory = () => typeof(SavedProperties).GetField("cards", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
		};
		array[5] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo6);
		JsonPropertyInfoValues<List<SavedProperties.SavedProperty<SerializableCard[]>>> propertyInfo7 = new JsonPropertyInfoValues<List<SavedProperties.SavedProperty<SerializableCard[]>>>
		{
			IsProperty = false,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SavedProperties),
			Converter = null,
			Getter = (object obj) => ((SavedProperties)obj).cardArrays,
			Setter = (object obj, List<SavedProperties.SavedProperty<SerializableCard[]>>? value) =>
			{
				((SavedProperties)obj).cardArrays = value;
			},
			IgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "cardArrays",
			JsonPropertyName = "card_arrays",
			AttributeProviderFactory = () => typeof(SavedProperties).GetField("cardArrays", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
		};
		array[6] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo7);
		return array;
	}

	private JsonTypeInfo<SavedProperties.SavedProperty<bool>> Create_SavedPropertyBoolean(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<SavedProperties.SavedProperty<bool>> jsonTypeInfo))
		{
			JsonObjectInfoValues<SavedProperties.SavedProperty<bool>> objectInfo = new JsonObjectInfoValues<SavedProperties.SavedProperty<bool>>
			{
				ObjectCreator = () => default,
				ObjectWithParameterizedConstructorCreator = null,
				PropertyMetadataInitializer = (JsonSerializerContext _) => SavedPropertyBooleanPropInit(options),
				ConstructorParameterMetadataInitializer = null,
				ConstructorAttributeProviderFactory = () => typeof(SavedProperties.SavedProperty<bool>).GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Array.Empty<Type>(), null),
				SerializeHandler = null
			};
			jsonTypeInfo = JsonMetadataServices.CreateObjectInfo(options, objectInfo);
			jsonTypeInfo.NumberHandling = null;
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private static JsonPropertyInfo[] SavedPropertyBooleanPropInit(JsonSerializerOptions options)
	{
		JsonPropertyInfo[] array = new JsonPropertyInfo[2];
		JsonPropertyInfoValues<string> propertyInfo = new JsonPropertyInfoValues<string>
		{
			IsProperty = false,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SavedProperties.SavedProperty<bool>),
			Converter = null,
			Getter = (object obj) => ((SavedProperties.SavedProperty<bool>)obj).name,
			Setter = (object obj, string? value) =>
			{
				Unsafe.Unbox<SavedProperties.SavedProperty<bool>>(obj).name = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "name",
			JsonPropertyName = null,
			AttributeProviderFactory = () => typeof(SavedProperties.SavedProperty<bool>).GetField("name", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
		};
		array[0] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo);
		array[0].IsGetNullable = false;
		array[0].IsSetNullable = false;
		JsonPropertyInfoValues<bool> propertyInfo2 = new JsonPropertyInfoValues<bool>
		{
			IsProperty = false,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SavedProperties.SavedProperty<bool>),
			Converter = null,
			Getter = (object obj) => ((SavedProperties.SavedProperty<bool>)obj).value,
			Setter = (object obj, bool value) =>
			{
				Unsafe.Unbox<SavedProperties.SavedProperty<bool>>(obj).value = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "value",
			JsonPropertyName = null,
			AttributeProviderFactory = () => typeof(SavedProperties.SavedProperty<bool>).GetField("value", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
		};
		array[1] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo2);
		return array;
	}

	private JsonTypeInfo<SavedProperties.SavedProperty<ModelId>> Create_SavedPropertyModelId(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<SavedProperties.SavedProperty<ModelId>> jsonTypeInfo))
		{
			JsonObjectInfoValues<SavedProperties.SavedProperty<ModelId>> objectInfo = new JsonObjectInfoValues<SavedProperties.SavedProperty<ModelId>>
			{
				ObjectCreator = () => default,
				ObjectWithParameterizedConstructorCreator = null,
				PropertyMetadataInitializer = (JsonSerializerContext _) => SavedPropertyModelIdPropInit(options),
				ConstructorParameterMetadataInitializer = null,
				ConstructorAttributeProviderFactory = () => typeof(SavedProperties.SavedProperty<ModelId>).GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Array.Empty<Type>(), null),
				SerializeHandler = null
			};
			jsonTypeInfo = JsonMetadataServices.CreateObjectInfo(options, objectInfo);
			jsonTypeInfo.NumberHandling = null;
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private static JsonPropertyInfo[] SavedPropertyModelIdPropInit(JsonSerializerOptions options)
	{
		JsonPropertyInfo[] array = new JsonPropertyInfo[2];
		JsonPropertyInfoValues<string> propertyInfo = new JsonPropertyInfoValues<string>
		{
			IsProperty = false,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SavedProperties.SavedProperty<ModelId>),
			Converter = null,
			Getter = (object obj) => ((SavedProperties.SavedProperty<ModelId>)obj).name,
			Setter = (object obj, string? value) =>
			{
				Unsafe.Unbox<SavedProperties.SavedProperty<ModelId>>(obj).name = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "name",
			JsonPropertyName = null,
			AttributeProviderFactory = () => typeof(SavedProperties.SavedProperty<ModelId>).GetField("name", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
		};
		array[0] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo);
		array[0].IsGetNullable = false;
		array[0].IsSetNullable = false;
		JsonPropertyInfoValues<ModelId> propertyInfo2 = new JsonPropertyInfoValues<ModelId>
		{
			IsProperty = false,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SavedProperties.SavedProperty<ModelId>),
			Converter = null,
			Getter = (object obj) => ((SavedProperties.SavedProperty<ModelId>)obj).value,
			Setter = (object obj, ModelId? value) =>
			{
				Unsafe.Unbox<SavedProperties.SavedProperty<ModelId>>(obj).value = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "value",
			JsonPropertyName = null,
			AttributeProviderFactory = () => typeof(SavedProperties.SavedProperty<ModelId>).GetField("value", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
		};
		array[1] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo2);
		return array;
	}

	private JsonTypeInfo<SavedProperties.SavedProperty<SerializableCard[]>> Create_SavedPropertySerializableCardArray(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<SavedProperties.SavedProperty<SerializableCard[]>> jsonTypeInfo))
		{
			JsonObjectInfoValues<SavedProperties.SavedProperty<SerializableCard[]>> objectInfo = new JsonObjectInfoValues<SavedProperties.SavedProperty<SerializableCard[]>>
			{
				ObjectCreator = () => default,
				ObjectWithParameterizedConstructorCreator = null,
				PropertyMetadataInitializer = (JsonSerializerContext _) => SavedPropertySerializableCardArrayPropInit(options),
				ConstructorParameterMetadataInitializer = null,
				ConstructorAttributeProviderFactory = () => typeof(SavedProperties.SavedProperty<SerializableCard[]>).GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Array.Empty<Type>(), null),
				SerializeHandler = null
			};
			jsonTypeInfo = JsonMetadataServices.CreateObjectInfo(options, objectInfo);
			jsonTypeInfo.NumberHandling = null;
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private static JsonPropertyInfo[] SavedPropertySerializableCardArrayPropInit(JsonSerializerOptions options)
	{
		JsonPropertyInfo[] array = new JsonPropertyInfo[2];
		JsonPropertyInfoValues<string> propertyInfo = new JsonPropertyInfoValues<string>
		{
			IsProperty = false,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SavedProperties.SavedProperty<SerializableCard[]>),
			Converter = null,
			Getter = (object obj) => ((SavedProperties.SavedProperty<SerializableCard[]>)obj).name,
			Setter = (object obj, string? value) =>
			{
				Unsafe.Unbox<SavedProperties.SavedProperty<SerializableCard[]>>(obj).name = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "name",
			JsonPropertyName = null,
			AttributeProviderFactory = () => typeof(SavedProperties.SavedProperty<SerializableCard[]>).GetField("name", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
		};
		array[0] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo);
		array[0].IsGetNullable = false;
		array[0].IsSetNullable = false;
		JsonPropertyInfoValues<SerializableCard[]> propertyInfo2 = new JsonPropertyInfoValues<SerializableCard[]>
		{
			IsProperty = false,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SavedProperties.SavedProperty<SerializableCard[]>),
			Converter = null,
			Getter = (object obj) => ((SavedProperties.SavedProperty<SerializableCard[]>)obj).value,
			Setter = (object obj, SerializableCard[]? value) =>
			{
				Unsafe.Unbox<SavedProperties.SavedProperty<SerializableCard[]>>(obj).value = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "value",
			JsonPropertyName = null,
			AttributeProviderFactory = () => typeof(SavedProperties.SavedProperty<SerializableCard[]>).GetField("value", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
		};
		array[1] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo2);
		return array;
	}

	private JsonTypeInfo<SavedProperties.SavedProperty<SerializableCard>> Create_SavedPropertySerializableCard(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<SavedProperties.SavedProperty<SerializableCard>> jsonTypeInfo))
		{
			JsonObjectInfoValues<SavedProperties.SavedProperty<SerializableCard>> objectInfo = new JsonObjectInfoValues<SavedProperties.SavedProperty<SerializableCard>>
			{
				ObjectCreator = () => default,
				ObjectWithParameterizedConstructorCreator = null,
				PropertyMetadataInitializer = (JsonSerializerContext _) => SavedPropertySerializableCardPropInit(options),
				ConstructorParameterMetadataInitializer = null,
				ConstructorAttributeProviderFactory = () => typeof(SavedProperties.SavedProperty<SerializableCard>).GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Array.Empty<Type>(), null),
				SerializeHandler = null
			};
			jsonTypeInfo = JsonMetadataServices.CreateObjectInfo(options, objectInfo);
			jsonTypeInfo.NumberHandling = null;
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private static JsonPropertyInfo[] SavedPropertySerializableCardPropInit(JsonSerializerOptions options)
	{
		JsonPropertyInfo[] array = new JsonPropertyInfo[2];
		JsonPropertyInfoValues<string> propertyInfo = new JsonPropertyInfoValues<string>
		{
			IsProperty = false,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SavedProperties.SavedProperty<SerializableCard>),
			Converter = null,
			Getter = (object obj) => ((SavedProperties.SavedProperty<SerializableCard>)obj).name,
			Setter = (object obj, string? value) =>
			{
				Unsafe.Unbox<SavedProperties.SavedProperty<SerializableCard>>(obj).name = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "name",
			JsonPropertyName = null,
			AttributeProviderFactory = () => typeof(SavedProperties.SavedProperty<SerializableCard>).GetField("name", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
		};
		array[0] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo);
		array[0].IsGetNullable = false;
		array[0].IsSetNullable = false;
		JsonPropertyInfoValues<SerializableCard> propertyInfo2 = new JsonPropertyInfoValues<SerializableCard>
		{
			IsProperty = false,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SavedProperties.SavedProperty<SerializableCard>),
			Converter = null,
			Getter = (object obj) => ((SavedProperties.SavedProperty<SerializableCard>)obj).value,
			Setter = (object obj, SerializableCard? value) =>
			{
				Unsafe.Unbox<SavedProperties.SavedProperty<SerializableCard>>(obj).value = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "value",
			JsonPropertyName = null,
			AttributeProviderFactory = () => typeof(SavedProperties.SavedProperty<SerializableCard>).GetField("value", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
		};
		array[1] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo2);
		return array;
	}

	private JsonTypeInfo<SavedProperties.SavedProperty<int[]>> Create_SavedPropertyInt32Array(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<SavedProperties.SavedProperty<int[]>> jsonTypeInfo))
		{
			JsonObjectInfoValues<SavedProperties.SavedProperty<int[]>> objectInfo = new JsonObjectInfoValues<SavedProperties.SavedProperty<int[]>>
			{
				ObjectCreator = () => default,
				ObjectWithParameterizedConstructorCreator = null,
				PropertyMetadataInitializer = (JsonSerializerContext _) => SavedPropertyInt32ArrayPropInit(options),
				ConstructorParameterMetadataInitializer = null,
				ConstructorAttributeProviderFactory = () => typeof(SavedProperties.SavedProperty<int[]>).GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Array.Empty<Type>(), null),
				SerializeHandler = null
			};
			jsonTypeInfo = JsonMetadataServices.CreateObjectInfo(options, objectInfo);
			jsonTypeInfo.NumberHandling = null;
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private static JsonPropertyInfo[] SavedPropertyInt32ArrayPropInit(JsonSerializerOptions options)
	{
		JsonPropertyInfo[] array = new JsonPropertyInfo[2];
		JsonPropertyInfoValues<string> propertyInfo = new JsonPropertyInfoValues<string>
		{
			IsProperty = false,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SavedProperties.SavedProperty<int[]>),
			Converter = null,
			Getter = (object obj) => ((SavedProperties.SavedProperty<int[]>)obj).name,
			Setter = (object obj, string? value) =>
			{
				Unsafe.Unbox<SavedProperties.SavedProperty<int[]>>(obj).name = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "name",
			JsonPropertyName = null,
			AttributeProviderFactory = () => typeof(SavedProperties.SavedProperty<int[]>).GetField("name", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
		};
		array[0] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo);
		array[0].IsGetNullable = false;
		array[0].IsSetNullable = false;
		JsonPropertyInfoValues<int[]> propertyInfo2 = new JsonPropertyInfoValues<int[]>
		{
			IsProperty = false,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SavedProperties.SavedProperty<int[]>),
			Converter = null,
			Getter = (object obj) => ((SavedProperties.SavedProperty<int[]>)obj).value,
			Setter = (object obj, int[]? value) =>
			{
				Unsafe.Unbox<SavedProperties.SavedProperty<int[]>>(obj).value = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "value",
			JsonPropertyName = null,
			AttributeProviderFactory = () => typeof(SavedProperties.SavedProperty<int[]>).GetField("value", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
		};
		array[1] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo2);
		return array;
	}

	private JsonTypeInfo<SavedProperties.SavedProperty<int>> Create_SavedPropertyInt32(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<SavedProperties.SavedProperty<int>> jsonTypeInfo))
		{
			JsonObjectInfoValues<SavedProperties.SavedProperty<int>> objectInfo = new JsonObjectInfoValues<SavedProperties.SavedProperty<int>>
			{
				ObjectCreator = () => default,
				ObjectWithParameterizedConstructorCreator = null,
				PropertyMetadataInitializer = (JsonSerializerContext _) => SavedPropertyInt32PropInit(options),
				ConstructorParameterMetadataInitializer = null,
				ConstructorAttributeProviderFactory = () => typeof(SavedProperties.SavedProperty<int>).GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Array.Empty<Type>(), null),
				SerializeHandler = null
			};
			jsonTypeInfo = JsonMetadataServices.CreateObjectInfo(options, objectInfo);
			jsonTypeInfo.NumberHandling = null;
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private static JsonPropertyInfo[] SavedPropertyInt32PropInit(JsonSerializerOptions options)
	{
		JsonPropertyInfo[] array = new JsonPropertyInfo[2];
		JsonPropertyInfoValues<string> propertyInfo = new JsonPropertyInfoValues<string>
		{
			IsProperty = false,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SavedProperties.SavedProperty<int>),
			Converter = null,
			Getter = (object obj) => ((SavedProperties.SavedProperty<int>)obj).name,
			Setter = (object obj, string? value) =>
			{
				Unsafe.Unbox<SavedProperties.SavedProperty<int>>(obj).name = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "name",
			JsonPropertyName = null,
			AttributeProviderFactory = () => typeof(SavedProperties.SavedProperty<int>).GetField("name", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
		};
		array[0] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo);
		array[0].IsGetNullable = false;
		array[0].IsSetNullable = false;
		JsonPropertyInfoValues<int> propertyInfo2 = new JsonPropertyInfoValues<int>
		{
			IsProperty = false,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SavedProperties.SavedProperty<int>),
			Converter = null,
			Getter = (object obj) => ((SavedProperties.SavedProperty<int>)obj).value,
			Setter = (object obj, int value) =>
			{
				Unsafe.Unbox<SavedProperties.SavedProperty<int>>(obj).value = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "value",
			JsonPropertyName = null,
			AttributeProviderFactory = () => typeof(SavedProperties.SavedProperty<int>).GetField("value", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
		};
		array[1] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo2);
		return array;
	}

	private JsonTypeInfo<SavedProperties.SavedProperty<string>> Create_SavedPropertyString(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<SavedProperties.SavedProperty<string>> jsonTypeInfo))
		{
			JsonObjectInfoValues<SavedProperties.SavedProperty<string>> objectInfo = new JsonObjectInfoValues<SavedProperties.SavedProperty<string>>
			{
				ObjectCreator = () => default,
				ObjectWithParameterizedConstructorCreator = null,
				PropertyMetadataInitializer = (JsonSerializerContext _) => SavedPropertyStringPropInit(options),
				ConstructorParameterMetadataInitializer = null,
				ConstructorAttributeProviderFactory = () => typeof(SavedProperties.SavedProperty<string>).GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Array.Empty<Type>(), null),
				SerializeHandler = null
			};
			jsonTypeInfo = JsonMetadataServices.CreateObjectInfo(options, objectInfo);
			jsonTypeInfo.NumberHandling = null;
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private static JsonPropertyInfo[] SavedPropertyStringPropInit(JsonSerializerOptions options)
	{
		JsonPropertyInfo[] array = new JsonPropertyInfo[2];
		JsonPropertyInfoValues<string> propertyInfo = new JsonPropertyInfoValues<string>
		{
			IsProperty = false,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SavedProperties.SavedProperty<string>),
			Converter = null,
			Getter = (object obj) => ((SavedProperties.SavedProperty<string>)obj).name,
			Setter = (object obj, string? value) =>
			{
				Unsafe.Unbox<SavedProperties.SavedProperty<string>>(obj).name = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "name",
			JsonPropertyName = null,
			AttributeProviderFactory = () => typeof(SavedProperties.SavedProperty<string>).GetField("name", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
		};
		array[0] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo);
		array[0].IsGetNullable = false;
		array[0].IsSetNullable = false;
		JsonPropertyInfoValues<string> propertyInfo2 = new JsonPropertyInfoValues<string>
		{
			IsProperty = false,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SavedProperties.SavedProperty<string>),
			Converter = null,
			Getter = (object obj) => ((SavedProperties.SavedProperty<string>)obj).value,
			Setter = (object obj, string? value) =>
			{
				Unsafe.Unbox<SavedProperties.SavedProperty<string>>(obj).value = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "value",
			JsonPropertyName = null,
			AttributeProviderFactory = () => typeof(SavedProperties.SavedProperty<string>).GetField("value", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
		};
		array[1] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo2);
		return array;
	}

	private JsonTypeInfo<SerializableActMap> Create_SerializableActMap(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<SerializableActMap> jsonTypeInfo))
		{
			JsonObjectInfoValues<SerializableActMap> objectInfo = new JsonObjectInfoValues<SerializableActMap>
			{
				ObjectCreator = () => new SerializableActMap(),
				ObjectWithParameterizedConstructorCreator = null,
				PropertyMetadataInitializer = (JsonSerializerContext _) => SerializableActMapPropInit(options),
				ConstructorParameterMetadataInitializer = null,
				ConstructorAttributeProviderFactory = () => typeof(SerializableActMap).GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Array.Empty<Type>(), null),
				SerializeHandler = null
			};
			jsonTypeInfo = JsonMetadataServices.CreateObjectInfo(options, objectInfo);
			jsonTypeInfo.NumberHandling = null;
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private static JsonPropertyInfo[] SerializableActMapPropInit(JsonSerializerOptions options)
	{
		JsonPropertyInfo[] array = new JsonPropertyInfo[7];
		JsonPropertyInfoValues<List<SerializableMapPoint>> propertyInfo = new JsonPropertyInfoValues<List<SerializableMapPoint>>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializableActMap),
			Converter = null,
			Getter = (object obj) => ((SerializableActMap)obj).Points,
			Setter = (object obj, List<SerializableMapPoint>? value) =>
			{
				((SerializableActMap)obj).Points = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "Points",
			JsonPropertyName = "points",
			AttributeProviderFactory = () => typeof(SerializableActMap).GetProperty("Points", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(List<SerializableMapPoint>), Array.Empty<Type>(), null)
		};
		array[0] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo);
		array[0].IsGetNullable = false;
		array[0].IsSetNullable = false;
		JsonPropertyInfoValues<SerializableMapPoint> propertyInfo2 = new JsonPropertyInfoValues<SerializableMapPoint>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializableActMap),
			Converter = null,
			Getter = (object obj) => ((SerializableActMap)obj).BossPoint,
			Setter = (object obj, SerializableMapPoint? value) =>
			{
				((SerializableActMap)obj).BossPoint = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "BossPoint",
			JsonPropertyName = "boss",
			AttributeProviderFactory = () => typeof(SerializableActMap).GetProperty("BossPoint", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(SerializableMapPoint), Array.Empty<Type>(), null)
		};
		array[1] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo2);
		array[1].IsGetNullable = false;
		array[1].IsSetNullable = false;
		JsonPropertyInfoValues<SerializableMapPoint> propertyInfo3 = new JsonPropertyInfoValues<SerializableMapPoint>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializableActMap),
			Converter = null,
			Getter = (object obj) => ((SerializableActMap)obj).SecondBossPoint,
			Setter = (object obj, SerializableMapPoint? value) =>
			{
				((SerializableActMap)obj).SecondBossPoint = value;
			},
			IgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "SecondBossPoint",
			JsonPropertyName = "second_boss",
			AttributeProviderFactory = () => typeof(SerializableActMap).GetProperty("SecondBossPoint", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(SerializableMapPoint), Array.Empty<Type>(), null)
		};
		array[2] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo3);
		JsonPropertyInfoValues<SerializableMapPoint> propertyInfo4 = new JsonPropertyInfoValues<SerializableMapPoint>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializableActMap),
			Converter = null,
			Getter = (object obj) => ((SerializableActMap)obj).StartingPoint,
			Setter = (object obj, SerializableMapPoint? value) =>
			{
				((SerializableActMap)obj).StartingPoint = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "StartingPoint",
			JsonPropertyName = "start",
			AttributeProviderFactory = () => typeof(SerializableActMap).GetProperty("StartingPoint", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(SerializableMapPoint), Array.Empty<Type>(), null)
		};
		array[3] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo4);
		array[3].IsGetNullable = false;
		array[3].IsSetNullable = false;
		JsonPropertyInfoValues<List<MapCoord>> propertyInfo5 = new JsonPropertyInfoValues<List<MapCoord>>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializableActMap),
			Converter = null,
			Getter = (object obj) => ((SerializableActMap)obj).StartMapPointCoords,
			Setter = (object obj, List<MapCoord>? value) =>
			{
				((SerializableActMap)obj).StartMapPointCoords = value;
			},
			IgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "StartMapPointCoords",
			JsonPropertyName = "start_coords",
			AttributeProviderFactory = () => typeof(SerializableActMap).GetProperty("StartMapPointCoords", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(List<MapCoord>), Array.Empty<Type>(), null)
		};
		array[4] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo5);
		JsonPropertyInfoValues<int> propertyInfo6 = new JsonPropertyInfoValues<int>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializableActMap),
			Converter = null,
			Getter = (object obj) => ((SerializableActMap)obj).GridWidth,
			Setter = (object obj, int value) =>
			{
				((SerializableActMap)obj).GridWidth = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "GridWidth",
			JsonPropertyName = "width",
			AttributeProviderFactory = () => typeof(SerializableActMap).GetProperty("GridWidth", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(int), Array.Empty<Type>(), null)
		};
		array[5] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo6);
		JsonPropertyInfoValues<int> propertyInfo7 = new JsonPropertyInfoValues<int>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializableActMap),
			Converter = null,
			Getter = (object obj) => ((SerializableActMap)obj).GridHeight,
			Setter = (object obj, int value) =>
			{
				((SerializableActMap)obj).GridHeight = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "GridHeight",
			JsonPropertyName = "height",
			AttributeProviderFactory = () => typeof(SerializableActMap).GetProperty("GridHeight", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(int), Array.Empty<Type>(), null)
		};
		array[6] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo7);
		return array;
	}

	private JsonTypeInfo<SerializableActModel> Create_SerializableActModel(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<SerializableActModel> jsonTypeInfo))
		{
			JsonObjectInfoValues<SerializableActModel> objectInfo = new JsonObjectInfoValues<SerializableActModel>
			{
				ObjectCreator = () => new SerializableActModel(),
				ObjectWithParameterizedConstructorCreator = null,
				PropertyMetadataInitializer = (JsonSerializerContext _) => SerializableActModelPropInit(options),
				ConstructorParameterMetadataInitializer = null,
				ConstructorAttributeProviderFactory = () => typeof(SerializableActModel).GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Array.Empty<Type>(), null),
				SerializeHandler = null
			};
			jsonTypeInfo = JsonMetadataServices.CreateObjectInfo(options, objectInfo);
			jsonTypeInfo.NumberHandling = null;
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private static JsonPropertyInfo[] SerializableActModelPropInit(JsonSerializerOptions options)
	{
		JsonPropertyInfo[] array = new JsonPropertyInfo[3];
		JsonPropertyInfoValues<ModelId> propertyInfo = new JsonPropertyInfoValues<ModelId>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializableActModel),
			Converter = null,
			Getter = (object obj) => ((SerializableActModel)obj).Id,
			Setter = (object obj, ModelId? value) =>
			{
				((SerializableActModel)obj).Id = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "Id",
			JsonPropertyName = "id",
			AttributeProviderFactory = () => typeof(SerializableActModel).GetProperty("Id", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(ModelId), Array.Empty<Type>(), null)
		};
		array[0] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo);
		JsonPropertyInfoValues<SerializableRoomSet> propertyInfo2 = new JsonPropertyInfoValues<SerializableRoomSet>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializableActModel),
			Converter = null,
			Getter = (object obj) => ((SerializableActModel)obj).SerializableRooms,
			Setter = (object obj, SerializableRoomSet? value) =>
			{
				((SerializableActModel)obj).SerializableRooms = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "SerializableRooms",
			JsonPropertyName = "rooms",
			AttributeProviderFactory = () => typeof(SerializableActModel).GetProperty("SerializableRooms", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(SerializableRoomSet), Array.Empty<Type>(), null)
		};
		array[1] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo2);
		array[1].IsGetNullable = false;
		array[1].IsSetNullable = false;
		JsonPropertyInfoValues<SerializableActMap> propertyInfo3 = new JsonPropertyInfoValues<SerializableActMap>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializableActModel),
			Converter = null,
			Getter = (object obj) => ((SerializableActModel)obj).SavedMap,
			Setter = (object obj, SerializableActMap? value) =>
			{
				((SerializableActModel)obj).SavedMap = value;
			},
			IgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "SavedMap",
			JsonPropertyName = "saved_map",
			AttributeProviderFactory = () => typeof(SerializableActModel).GetProperty("SavedMap", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(SerializableActMap), Array.Empty<Type>(), null)
		};
		array[2] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo3);
		return array;
	}

	private JsonTypeInfo<SerializableBadge> Create_SerializableBadge(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<SerializableBadge> jsonTypeInfo))
		{
			JsonObjectInfoValues<SerializableBadge> objectInfo = new JsonObjectInfoValues<SerializableBadge>
			{
				ObjectCreator = null,
				ObjectWithParameterizedConstructorCreator = (object[] args) => new SerializableBadge
				{
					Id = (string)args[0],
					Rarity = (BadgeRarity)args[1]
				},
				PropertyMetadataInitializer = (JsonSerializerContext _) => SerializableBadgePropInit(options),
				ConstructorParameterMetadataInitializer = SerializableBadgeCtorParamInit,
				ConstructorAttributeProviderFactory = () => typeof(SerializableBadge).GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Array.Empty<Type>(), null),
				SerializeHandler = null
			};
			jsonTypeInfo = JsonMetadataServices.CreateObjectInfo(options, objectInfo);
			jsonTypeInfo.NumberHandling = null;
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private static JsonPropertyInfo[] SerializableBadgePropInit(JsonSerializerOptions options)
	{
		JsonPropertyInfo[] array = new JsonPropertyInfo[2];
		JsonPropertyInfoValues<string> propertyInfo = new JsonPropertyInfoValues<string>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializableBadge),
			Converter = null,
			Getter = (object obj) => ((SerializableBadge)obj).Id,
			Setter = (object obj, string? value) =>
			{
				((SerializableBadge)obj).Id = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "Id",
			JsonPropertyName = "id",
			AttributeProviderFactory = () => typeof(SerializableBadge).GetProperty("Id", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(string), Array.Empty<Type>(), null)
		};
		array[0] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo);
		array[0].IsRequired = true;
		array[0].IsGetNullable = false;
		array[0].IsSetNullable = false;
		JsonPropertyInfoValues<BadgeRarity> propertyInfo2 = new JsonPropertyInfoValues<BadgeRarity>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializableBadge),
			Converter = null,
			Getter = (object obj) => ((SerializableBadge)obj).Rarity,
			Setter = (object obj, BadgeRarity value) =>
			{
				((SerializableBadge)obj).Rarity = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "Rarity",
			JsonPropertyName = "rarity",
			AttributeProviderFactory = () => typeof(SerializableBadge).GetProperty("Rarity", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(BadgeRarity), Array.Empty<Type>(), null)
		};
		array[1] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo2);
		array[1].IsRequired = true;
		return array;
	}

	private static JsonParameterInfoValues[] SerializableBadgeCtorParamInit()
	{
		return new JsonParameterInfoValues[2]
		{
			new JsonParameterInfoValues
			{
				Name = "Id",
				ParameterType = typeof(string),
				Position = 0,
				IsNullable = false,
				IsMemberInitializer = true
			},
			new JsonParameterInfoValues
			{
				Name = "Rarity",
				ParameterType = typeof(BadgeRarity),
				Position = 1,
				IsNullable = false,
				IsMemberInitializer = true
			}
		};
	}

	private JsonTypeInfo<SerializableCard> Create_SerializableCard(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<SerializableCard> jsonTypeInfo))
		{
			JsonObjectInfoValues<SerializableCard> objectInfo = new JsonObjectInfoValues<SerializableCard>
			{
				ObjectCreator = () => new SerializableCard(),
				ObjectWithParameterizedConstructorCreator = null,
				PropertyMetadataInitializer = (JsonSerializerContext _) => SerializableCardPropInit(options),
				ConstructorParameterMetadataInitializer = null,
				ConstructorAttributeProviderFactory = () => typeof(SerializableCard).GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Array.Empty<Type>(), null),
				SerializeHandler = null
			};
			jsonTypeInfo = JsonMetadataServices.CreateObjectInfo(options, objectInfo);
			jsonTypeInfo.NumberHandling = null;
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private static JsonPropertyInfo[] SerializableCardPropInit(JsonSerializerOptions options)
	{
		JsonPropertyInfo[] array = new JsonPropertyInfo[5];
		JsonPropertyInfoValues<ModelId> propertyInfo = new JsonPropertyInfoValues<ModelId>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializableCard),
			Converter = null,
			Getter = (object obj) => ((SerializableCard)obj).Id,
			Setter = (object obj, ModelId? value) =>
			{
				((SerializableCard)obj).Id = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "Id",
			JsonPropertyName = "id",
			AttributeProviderFactory = () => typeof(SerializableCard).GetProperty("Id", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(ModelId), Array.Empty<Type>(), null)
		};
		array[0] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo);
		JsonPropertyInfoValues<int> propertyInfo2 = new JsonPropertyInfoValues<int>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializableCard),
			Converter = null,
			Getter = (object obj) => ((SerializableCard)obj).CurrentUpgradeLevel,
			Setter = (object obj, int value) =>
			{
				((SerializableCard)obj).CurrentUpgradeLevel = value;
			},
			IgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "CurrentUpgradeLevel",
			JsonPropertyName = "current_upgrade_level",
			AttributeProviderFactory = () => typeof(SerializableCard).GetProperty("CurrentUpgradeLevel", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(int), Array.Empty<Type>(), null)
		};
		array[1] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo2);
		JsonPropertyInfoValues<SerializableEnchantment> propertyInfo3 = new JsonPropertyInfoValues<SerializableEnchantment>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializableCard),
			Converter = null,
			Getter = (object obj) => ((SerializableCard)obj).Enchantment,
			Setter = (object obj, SerializableEnchantment? value) =>
			{
				((SerializableCard)obj).Enchantment = value;
			},
			IgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "Enchantment",
			JsonPropertyName = "enchantment",
			AttributeProviderFactory = () => typeof(SerializableCard).GetProperty("Enchantment", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(SerializableEnchantment), Array.Empty<Type>(), null)
		};
		array[2] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo3);
		JsonPropertyInfoValues<SavedProperties> propertyInfo4 = new JsonPropertyInfoValues<SavedProperties>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializableCard),
			Converter = null,
			Getter = (object obj) => ((SerializableCard)obj).Props,
			Setter = (object obj, SavedProperties? value) =>
			{
				((SerializableCard)obj).Props = value;
			},
			IgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "Props",
			JsonPropertyName = "props",
			AttributeProviderFactory = () => typeof(SerializableCard).GetProperty("Props", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(SavedProperties), Array.Empty<Type>(), null)
		};
		array[3] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo4);
		JsonPropertyInfoValues<int?> propertyInfo5 = new JsonPropertyInfoValues<int?>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializableCard),
			Converter = null,
			Getter = (object obj) => ((SerializableCard)obj).FloorAddedToDeck,
			Setter = (object obj, int? value) =>
			{
				((SerializableCard)obj).FloorAddedToDeck = value;
			},
			IgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "FloorAddedToDeck",
			JsonPropertyName = "floor_added_to_deck",
			AttributeProviderFactory = () => typeof(SerializableCard).GetProperty("FloorAddedToDeck", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(int?), Array.Empty<Type>(), null)
		};
		array[4] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo5);
		return array;
	}

	private JsonTypeInfo<SerializableCard[]> Create_SerializableCardArray(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<SerializableCard[]> jsonTypeInfo))
		{
			JsonCollectionInfoValues<SerializableCard[]> collectionInfo = new JsonCollectionInfoValues<SerializableCard[]>
			{
				ObjectCreator = null,
				SerializeHandler = null
			};
			jsonTypeInfo = JsonMetadataServices.CreateArrayInfo(options, collectionInfo);
			jsonTypeInfo.NumberHandling = null;
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private JsonTypeInfo<SerializableEnchantment> Create_SerializableEnchantment(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<SerializableEnchantment> jsonTypeInfo))
		{
			JsonObjectInfoValues<SerializableEnchantment> objectInfo = new JsonObjectInfoValues<SerializableEnchantment>
			{
				ObjectCreator = () => new SerializableEnchantment(),
				ObjectWithParameterizedConstructorCreator = null,
				PropertyMetadataInitializer = (JsonSerializerContext _) => SerializableEnchantmentPropInit(options),
				ConstructorParameterMetadataInitializer = null,
				ConstructorAttributeProviderFactory = () => typeof(SerializableEnchantment).GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Array.Empty<Type>(), null),
				SerializeHandler = null
			};
			jsonTypeInfo = JsonMetadataServices.CreateObjectInfo(options, objectInfo);
			jsonTypeInfo.NumberHandling = null;
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private static JsonPropertyInfo[] SerializableEnchantmentPropInit(JsonSerializerOptions options)
	{
		JsonPropertyInfo[] array = new JsonPropertyInfo[3];
		JsonPropertyInfoValues<ModelId> propertyInfo = new JsonPropertyInfoValues<ModelId>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializableEnchantment),
			Converter = null,
			Getter = (object obj) => ((SerializableEnchantment)obj).Id,
			Setter = (object obj, ModelId? value) =>
			{
				((SerializableEnchantment)obj).Id = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "Id",
			JsonPropertyName = "id",
			AttributeProviderFactory = () => typeof(SerializableEnchantment).GetProperty("Id", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(ModelId), Array.Empty<Type>(), null)
		};
		array[0] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo);
		JsonPropertyInfoValues<int> propertyInfo2 = new JsonPropertyInfoValues<int>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializableEnchantment),
			Converter = null,
			Getter = (object obj) => ((SerializableEnchantment)obj).Amount,
			Setter = (object obj, int value) =>
			{
				((SerializableEnchantment)obj).Amount = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "Amount",
			JsonPropertyName = "amount",
			AttributeProviderFactory = () => typeof(SerializableEnchantment).GetProperty("Amount", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(int), Array.Empty<Type>(), null)
		};
		array[1] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo2);
		JsonPropertyInfoValues<SavedProperties> propertyInfo3 = new JsonPropertyInfoValues<SavedProperties>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializableEnchantment),
			Converter = null,
			Getter = (object obj) => ((SerializableEnchantment)obj).Props,
			Setter = (object obj, SavedProperties? value) =>
			{
				((SerializableEnchantment)obj).Props = value;
			},
			IgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "Props",
			JsonPropertyName = "props",
			AttributeProviderFactory = () => typeof(SerializableEnchantment).GetProperty("Props", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(SavedProperties), Array.Empty<Type>(), null)
		};
		array[2] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo3);
		return array;
	}

	private JsonTypeInfo<SerializableExtraRunFields> Create_SerializableExtraRunFields(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<SerializableExtraRunFields> jsonTypeInfo))
		{
			JsonObjectInfoValues<SerializableExtraRunFields> objectInfo = new JsonObjectInfoValues<SerializableExtraRunFields>
			{
				ObjectCreator = () => new SerializableExtraRunFields(),
				ObjectWithParameterizedConstructorCreator = null,
				PropertyMetadataInitializer = (JsonSerializerContext _) => SerializableExtraRunFieldsPropInit(options),
				ConstructorParameterMetadataInitializer = null,
				ConstructorAttributeProviderFactory = () => typeof(SerializableExtraRunFields).GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Array.Empty<Type>(), null),
				SerializeHandler = null
			};
			jsonTypeInfo = JsonMetadataServices.CreateObjectInfo(options, objectInfo);
			jsonTypeInfo.NumberHandling = null;
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private static JsonPropertyInfo[] SerializableExtraRunFieldsPropInit(JsonSerializerOptions options)
	{
		JsonPropertyInfo[] array = new JsonPropertyInfo[3];
		JsonPropertyInfoValues<bool> propertyInfo = new JsonPropertyInfoValues<bool>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializableExtraRunFields),
			Converter = null,
			Getter = (object obj) => ((SerializableExtraRunFields)obj).StartedWithNeow,
			Setter = (object obj, bool value) =>
			{
				((SerializableExtraRunFields)obj).StartedWithNeow = value;
			},
			IgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "StartedWithNeow",
			JsonPropertyName = "started_with_neow",
			AttributeProviderFactory = () => typeof(SerializableExtraRunFields).GetProperty("StartedWithNeow", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(bool), Array.Empty<Type>(), null)
		};
		array[0] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo);
		JsonPropertyInfoValues<int> propertyInfo2 = new JsonPropertyInfoValues<int>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializableExtraRunFields),
			Converter = null,
			Getter = (object obj) => ((SerializableExtraRunFields)obj).TestSubjectKills,
			Setter = (object obj, int value) =>
			{
				((SerializableExtraRunFields)obj).TestSubjectKills = value;
			},
			IgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "TestSubjectKills",
			JsonPropertyName = "test_subject_kills",
			AttributeProviderFactory = () => typeof(SerializableExtraRunFields).GetProperty("TestSubjectKills", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(int), Array.Empty<Type>(), null)
		};
		array[1] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo2);
		JsonPropertyInfoValues<bool> propertyInfo3 = new JsonPropertyInfoValues<bool>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializableExtraRunFields),
			Converter = null,
			Getter = (object obj) => ((SerializableExtraRunFields)obj).FreedRepy,
			Setter = (object obj, bool value) =>
			{
				((SerializableExtraRunFields)obj).FreedRepy = value;
			},
			IgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "FreedRepy",
			JsonPropertyName = "freed_repy",
			AttributeProviderFactory = () => typeof(SerializableExtraRunFields).GetProperty("FreedRepy", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(bool), Array.Empty<Type>(), null)
		};
		array[2] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo3);
		return array;
	}

	private JsonTypeInfo<SerializableMapPoint> Create_SerializableMapPoint(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<SerializableMapPoint> jsonTypeInfo))
		{
			JsonObjectInfoValues<SerializableMapPoint> objectInfo = new JsonObjectInfoValues<SerializableMapPoint>
			{
				ObjectCreator = () => new SerializableMapPoint(),
				ObjectWithParameterizedConstructorCreator = null,
				PropertyMetadataInitializer = (JsonSerializerContext _) => SerializableMapPointPropInit(options),
				ConstructorParameterMetadataInitializer = null,
				ConstructorAttributeProviderFactory = () => typeof(SerializableMapPoint).GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Array.Empty<Type>(), null),
				SerializeHandler = null
			};
			jsonTypeInfo = JsonMetadataServices.CreateObjectInfo(options, objectInfo);
			jsonTypeInfo.NumberHandling = null;
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private static JsonPropertyInfo[] SerializableMapPointPropInit(JsonSerializerOptions options)
	{
		JsonPropertyInfo[] array = new JsonPropertyInfo[4];
		JsonPropertyInfoValues<MapCoord> propertyInfo = new JsonPropertyInfoValues<MapCoord>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializableMapPoint),
			Converter = null,
			Getter = (object obj) => ((SerializableMapPoint)obj).Coord,
			Setter = (object obj, MapCoord value) =>
			{
				((SerializableMapPoint)obj).Coord = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "Coord",
			JsonPropertyName = "coord",
			AttributeProviderFactory = () => typeof(SerializableMapPoint).GetProperty("Coord", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(MapCoord), Array.Empty<Type>(), null)
		};
		array[0] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo);
		JsonPropertyInfoValues<MapPointType> propertyInfo2 = new JsonPropertyInfoValues<MapPointType>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializableMapPoint),
			Converter = null,
			Getter = (object obj) => ((SerializableMapPoint)obj).PointType,
			Setter = (object obj, MapPointType value) =>
			{
				((SerializableMapPoint)obj).PointType = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "PointType",
			JsonPropertyName = "type",
			AttributeProviderFactory = () => typeof(SerializableMapPoint).GetProperty("PointType", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(MapPointType), Array.Empty<Type>(), null)
		};
		array[1] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo2);
		JsonPropertyInfoValues<bool> propertyInfo3 = new JsonPropertyInfoValues<bool>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializableMapPoint),
			Converter = null,
			Getter = (object obj) => ((SerializableMapPoint)obj).CanBeModified,
			Setter = (object obj, bool value) =>
			{
				((SerializableMapPoint)obj).CanBeModified = value;
			},
			IgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "CanBeModified",
			JsonPropertyName = "can_modify",
			AttributeProviderFactory = () => typeof(SerializableMapPoint).GetProperty("CanBeModified", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(bool), Array.Empty<Type>(), null)
		};
		array[2] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo3);
		JsonPropertyInfoValues<List<MapCoord>> propertyInfo4 = new JsonPropertyInfoValues<List<MapCoord>>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializableMapPoint),
			Converter = null,
			Getter = (object obj) => ((SerializableMapPoint)obj).ChildCoords,
			Setter = (object obj, List<MapCoord>? value) =>
			{
				((SerializableMapPoint)obj).ChildCoords = value;
			},
			IgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "ChildCoords",
			JsonPropertyName = "children",
			AttributeProviderFactory = () => typeof(SerializableMapPoint).GetProperty("ChildCoords", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(List<MapCoord>), Array.Empty<Type>(), null)
		};
		array[3] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo4);
		return array;
	}

	private JsonTypeInfo<SerializableModifier> Create_SerializableModifier(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<SerializableModifier> jsonTypeInfo))
		{
			JsonObjectInfoValues<SerializableModifier> objectInfo = new JsonObjectInfoValues<SerializableModifier>
			{
				ObjectCreator = () => new SerializableModifier(),
				ObjectWithParameterizedConstructorCreator = null,
				PropertyMetadataInitializer = (JsonSerializerContext _) => SerializableModifierPropInit(options),
				ConstructorParameterMetadataInitializer = null,
				ConstructorAttributeProviderFactory = () => typeof(SerializableModifier).GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Array.Empty<Type>(), null),
				SerializeHandler = null
			};
			jsonTypeInfo = JsonMetadataServices.CreateObjectInfo(options, objectInfo);
			jsonTypeInfo.NumberHandling = null;
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private static JsonPropertyInfo[] SerializableModifierPropInit(JsonSerializerOptions options)
	{
		JsonPropertyInfo[] array = new JsonPropertyInfo[2];
		JsonPropertyInfoValues<ModelId> propertyInfo = new JsonPropertyInfoValues<ModelId>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializableModifier),
			Converter = null,
			Getter = (object obj) => ((SerializableModifier)obj).Id,
			Setter = (object obj, ModelId? value) =>
			{
				((SerializableModifier)obj).Id = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "Id",
			JsonPropertyName = "id",
			AttributeProviderFactory = () => typeof(SerializableModifier).GetProperty("Id", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(ModelId), Array.Empty<Type>(), null)
		};
		array[0] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo);
		JsonPropertyInfoValues<SavedProperties> propertyInfo2 = new JsonPropertyInfoValues<SavedProperties>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializableModifier),
			Converter = null,
			Getter = (object obj) => ((SerializableModifier)obj).Props,
			Setter = (object obj, SavedProperties? value) =>
			{
				((SerializableModifier)obj).Props = value;
			},
			IgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "Props",
			JsonPropertyName = "props",
			AttributeProviderFactory = () => typeof(SerializableModifier).GetProperty("Props", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(SavedProperties), Array.Empty<Type>(), null)
		};
		array[1] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo2);
		return array;
	}

	private JsonTypeInfo<SerializablePlayer> Create_SerializablePlayer(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<SerializablePlayer> jsonTypeInfo))
		{
			JsonObjectInfoValues<SerializablePlayer> objectInfo = new JsonObjectInfoValues<SerializablePlayer>
			{
				ObjectCreator = () => new SerializablePlayer(),
				ObjectWithParameterizedConstructorCreator = null,
				PropertyMetadataInitializer = (JsonSerializerContext _) => SerializablePlayerPropInit(options),
				ConstructorParameterMetadataInitializer = null,
				ConstructorAttributeProviderFactory = () => typeof(SerializablePlayer).GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Array.Empty<Type>(), null),
				SerializeHandler = null
			};
			jsonTypeInfo = JsonMetadataServices.CreateObjectInfo(options, objectInfo);
			jsonTypeInfo.NumberHandling = null;
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private static JsonPropertyInfo[] SerializablePlayerPropInit(JsonSerializerOptions options)
	{
		JsonPropertyInfo[] array = new JsonPropertyInfo[21];
		JsonPropertyInfoValues<ModelId> propertyInfo = new JsonPropertyInfoValues<ModelId>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializablePlayer),
			Converter = null,
			Getter = (object obj) => ((SerializablePlayer)obj).CharacterId,
			Setter = (object obj, ModelId? value) =>
			{
				((SerializablePlayer)obj).CharacterId = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "CharacterId",
			JsonPropertyName = "character_id",
			AttributeProviderFactory = () => typeof(SerializablePlayer).GetProperty("CharacterId", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(ModelId), Array.Empty<Type>(), null)
		};
		array[0] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo);
		JsonPropertyInfoValues<int> propertyInfo2 = new JsonPropertyInfoValues<int>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializablePlayer),
			Converter = null,
			Getter = (object obj) => ((SerializablePlayer)obj).CurrentHp,
			Setter = (object obj, int value) =>
			{
				((SerializablePlayer)obj).CurrentHp = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "CurrentHp",
			JsonPropertyName = "current_hp",
			AttributeProviderFactory = () => typeof(SerializablePlayer).GetProperty("CurrentHp", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(int), Array.Empty<Type>(), null)
		};
		array[1] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo2);
		JsonPropertyInfoValues<int> propertyInfo3 = new JsonPropertyInfoValues<int>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializablePlayer),
			Converter = null,
			Getter = (object obj) => ((SerializablePlayer)obj).MaxHp,
			Setter = (object obj, int value) =>
			{
				((SerializablePlayer)obj).MaxHp = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "MaxHp",
			JsonPropertyName = "max_hp",
			AttributeProviderFactory = () => typeof(SerializablePlayer).GetProperty("MaxHp", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(int), Array.Empty<Type>(), null)
		};
		array[2] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo3);
		JsonPropertyInfoValues<int> propertyInfo4 = new JsonPropertyInfoValues<int>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializablePlayer),
			Converter = null,
			Getter = (object obj) => ((SerializablePlayer)obj).MaxEnergy,
			Setter = (object obj, int value) =>
			{
				((SerializablePlayer)obj).MaxEnergy = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "MaxEnergy",
			JsonPropertyName = "max_energy",
			AttributeProviderFactory = () => typeof(SerializablePlayer).GetProperty("MaxEnergy", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(int), Array.Empty<Type>(), null)
		};
		array[3] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo4);
		JsonPropertyInfoValues<int> propertyInfo5 = new JsonPropertyInfoValues<int>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializablePlayer),
			Converter = null,
			Getter = (object obj) => ((SerializablePlayer)obj).MaxPotionSlotCount,
			Setter = (object obj, int value) =>
			{
				((SerializablePlayer)obj).MaxPotionSlotCount = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "MaxPotionSlotCount",
			JsonPropertyName = "max_potion_slot_count",
			AttributeProviderFactory = () => typeof(SerializablePlayer).GetProperty("MaxPotionSlotCount", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(int), Array.Empty<Type>(), null)
		};
		array[4] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo5);
		JsonPropertyInfoValues<int> propertyInfo6 = new JsonPropertyInfoValues<int>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializablePlayer),
			Converter = null,
			Getter = (object obj) => ((SerializablePlayer)obj).Gold,
			Setter = (object obj, int value) =>
			{
				((SerializablePlayer)obj).Gold = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "Gold",
			JsonPropertyName = "gold",
			AttributeProviderFactory = () => typeof(SerializablePlayer).GetProperty("Gold", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(int), Array.Empty<Type>(), null)
		};
		array[5] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo6);
		JsonPropertyInfoValues<int> propertyInfo7 = new JsonPropertyInfoValues<int>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializablePlayer),
			Converter = null,
			Getter = (object obj) => ((SerializablePlayer)obj).BaseOrbSlotCount,
			Setter = (object obj, int value) =>
			{
				((SerializablePlayer)obj).BaseOrbSlotCount = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "BaseOrbSlotCount",
			JsonPropertyName = "base_orb_slot_count",
			AttributeProviderFactory = () => typeof(SerializablePlayer).GetProperty("BaseOrbSlotCount", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(int), Array.Empty<Type>(), null)
		};
		array[6] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo7);
		JsonPropertyInfoValues<ulong> propertyInfo8 = new JsonPropertyInfoValues<ulong>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializablePlayer),
			Converter = null,
			Getter = (object obj) => ((SerializablePlayer)obj).NetId,
			Setter = (object obj, ulong value) =>
			{
				((SerializablePlayer)obj).NetId = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "NetId",
			JsonPropertyName = "net_id",
			AttributeProviderFactory = () => typeof(SerializablePlayer).GetProperty("NetId", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(ulong), Array.Empty<Type>(), null)
		};
		array[7] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo8);
		JsonPropertyInfoValues<List<SerializableCard>> propertyInfo9 = new JsonPropertyInfoValues<List<SerializableCard>>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializablePlayer),
			Converter = null,
			Getter = (object obj) => ((SerializablePlayer)obj).Deck,
			Setter = (object obj, List<SerializableCard>? value) =>
			{
				((SerializablePlayer)obj).Deck = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "Deck",
			JsonPropertyName = "deck",
			AttributeProviderFactory = () => typeof(SerializablePlayer).GetProperty("Deck", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(List<SerializableCard>), Array.Empty<Type>(), null)
		};
		array[8] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo9);
		array[8].IsGetNullable = false;
		array[8].IsSetNullable = false;
		JsonPropertyInfoValues<List<SerializableRelic>> propertyInfo10 = new JsonPropertyInfoValues<List<SerializableRelic>>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializablePlayer),
			Converter = null,
			Getter = (object obj) => ((SerializablePlayer)obj).Relics,
			Setter = (object obj, List<SerializableRelic>? value) =>
			{
				((SerializablePlayer)obj).Relics = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "Relics",
			JsonPropertyName = "relics",
			AttributeProviderFactory = () => typeof(SerializablePlayer).GetProperty("Relics", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(List<SerializableRelic>), Array.Empty<Type>(), null)
		};
		array[9] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo10);
		array[9].IsGetNullable = false;
		array[9].IsSetNullable = false;
		JsonPropertyInfoValues<List<SerializablePotion>> propertyInfo11 = new JsonPropertyInfoValues<List<SerializablePotion>>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializablePlayer),
			Converter = null,
			Getter = (object obj) => ((SerializablePlayer)obj).Potions,
			Setter = (object obj, List<SerializablePotion>? value) =>
			{
				((SerializablePlayer)obj).Potions = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "Potions",
			JsonPropertyName = "potions",
			AttributeProviderFactory = () => typeof(SerializablePlayer).GetProperty("Potions", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(List<SerializablePotion>), Array.Empty<Type>(), null)
		};
		array[10] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo11);
		array[10].IsGetNullable = false;
		array[10].IsSetNullable = false;
		JsonPropertyInfoValues<SerializablePlayerRngSet> propertyInfo12 = new JsonPropertyInfoValues<SerializablePlayerRngSet>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializablePlayer),
			Converter = null,
			Getter = (object obj) => ((SerializablePlayer)obj).Rng,
			Setter = (object obj, SerializablePlayerRngSet? value) =>
			{
				((SerializablePlayer)obj).Rng = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "Rng",
			JsonPropertyName = "rng",
			AttributeProviderFactory = () => typeof(SerializablePlayer).GetProperty("Rng", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(SerializablePlayerRngSet), Array.Empty<Type>(), null)
		};
		array[11] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo12);
		array[11].IsGetNullable = false;
		array[11].IsSetNullable = false;
		JsonPropertyInfoValues<SerializablePlayerOddsSet> propertyInfo13 = new JsonPropertyInfoValues<SerializablePlayerOddsSet>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializablePlayer),
			Converter = null,
			Getter = (object obj) => ((SerializablePlayer)obj).Odds,
			Setter = (object obj, SerializablePlayerOddsSet? value) =>
			{
				((SerializablePlayer)obj).Odds = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "Odds",
			JsonPropertyName = "odds",
			AttributeProviderFactory = () => typeof(SerializablePlayer).GetProperty("Odds", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(SerializablePlayerOddsSet), Array.Empty<Type>(), null)
		};
		array[12] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo13);
		array[12].IsGetNullable = false;
		array[12].IsSetNullable = false;
		JsonPropertyInfoValues<SerializableRelicGrabBag> propertyInfo14 = new JsonPropertyInfoValues<SerializableRelicGrabBag>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializablePlayer),
			Converter = null,
			Getter = (object obj) => ((SerializablePlayer)obj).RelicGrabBag,
			Setter = (object obj, SerializableRelicGrabBag? value) =>
			{
				((SerializablePlayer)obj).RelicGrabBag = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "RelicGrabBag",
			JsonPropertyName = "relic_grab_bag",
			AttributeProviderFactory = () => typeof(SerializablePlayer).GetProperty("RelicGrabBag", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(SerializableRelicGrabBag), Array.Empty<Type>(), null)
		};
		array[13] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo14);
		array[13].IsGetNullable = false;
		array[13].IsSetNullable = false;
		JsonPropertyInfoValues<SerializableExtraPlayerFields> propertyInfo15 = new JsonPropertyInfoValues<SerializableExtraPlayerFields>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializablePlayer),
			Converter = null,
			Getter = (object obj) => ((SerializablePlayer)obj).ExtraFields,
			Setter = (object obj, SerializableExtraPlayerFields? value) =>
			{
				((SerializablePlayer)obj).ExtraFields = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "ExtraFields",
			JsonPropertyName = "extra_fields",
			AttributeProviderFactory = () => typeof(SerializablePlayer).GetProperty("ExtraFields", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(SerializableExtraPlayerFields), Array.Empty<Type>(), null)
		};
		array[14] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo15);
		array[14].IsGetNullable = false;
		array[14].IsSetNullable = false;
		JsonPropertyInfoValues<SerializableUnlockState> propertyInfo16 = new JsonPropertyInfoValues<SerializableUnlockState>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializablePlayer),
			Converter = null,
			Getter = (object obj) => ((SerializablePlayer)obj).UnlockState,
			Setter = (object obj, SerializableUnlockState? value) =>
			{
				((SerializablePlayer)obj).UnlockState = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "UnlockState",
			JsonPropertyName = "unlock_state",
			AttributeProviderFactory = () => typeof(SerializablePlayer).GetProperty("UnlockState", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(SerializableUnlockState), Array.Empty<Type>(), null)
		};
		array[15] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo16);
		array[15].IsGetNullable = false;
		array[15].IsSetNullable = false;
		JsonPropertyInfoValues<List<ModelId>> propertyInfo17 = new JsonPropertyInfoValues<List<ModelId>>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializablePlayer),
			Converter = null,
			Getter = (object obj) => ((SerializablePlayer)obj).DiscoveredCards,
			Setter = (object obj, List<ModelId>? value) =>
			{
				((SerializablePlayer)obj).DiscoveredCards = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "DiscoveredCards",
			JsonPropertyName = "discovered_cards",
			AttributeProviderFactory = () => typeof(SerializablePlayer).GetProperty("DiscoveredCards", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(List<ModelId>), Array.Empty<Type>(), null)
		};
		array[16] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo17);
		array[16].IsGetNullable = false;
		array[16].IsSetNullable = false;
		JsonPropertyInfoValues<List<ModelId>> propertyInfo18 = new JsonPropertyInfoValues<List<ModelId>>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializablePlayer),
			Converter = null,
			Getter = (object obj) => ((SerializablePlayer)obj).DiscoveredEnemies,
			Setter = (object obj, List<ModelId>? value) =>
			{
				((SerializablePlayer)obj).DiscoveredEnemies = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "DiscoveredEnemies",
			JsonPropertyName = "discovered_enemies",
			AttributeProviderFactory = () => typeof(SerializablePlayer).GetProperty("DiscoveredEnemies", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(List<ModelId>), Array.Empty<Type>(), null)
		};
		array[17] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo18);
		array[17].IsGetNullable = false;
		array[17].IsSetNullable = false;
		JsonPropertyInfoValues<List<string>> propertyInfo19 = new JsonPropertyInfoValues<List<string>>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializablePlayer),
			Converter = (JsonConverter<List<string>>)ExpandConverter(typeof(List<string>), new EpochIdListConverter(), options),
			Getter = (object obj) => ((SerializablePlayer)obj).DiscoveredEpochs,
			Setter = (object obj, List<string>? value) =>
			{
				((SerializablePlayer)obj).DiscoveredEpochs = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "DiscoveredEpochs",
			JsonPropertyName = "discovered_epochs",
			AttributeProviderFactory = () => typeof(SerializablePlayer).GetProperty("DiscoveredEpochs", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(List<string>), Array.Empty<Type>(), null)
		};
		array[18] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo19);
		array[18].IsGetNullable = false;
		array[18].IsSetNullable = false;
		JsonPropertyInfoValues<List<ModelId>> propertyInfo20 = new JsonPropertyInfoValues<List<ModelId>>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializablePlayer),
			Converter = null,
			Getter = (object obj) => ((SerializablePlayer)obj).DiscoveredPotions,
			Setter = (object obj, List<ModelId>? value) =>
			{
				((SerializablePlayer)obj).DiscoveredPotions = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "DiscoveredPotions",
			JsonPropertyName = "discovered_potions",
			AttributeProviderFactory = () => typeof(SerializablePlayer).GetProperty("DiscoveredPotions", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(List<ModelId>), Array.Empty<Type>(), null)
		};
		array[19] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo20);
		array[19].IsGetNullable = false;
		array[19].IsSetNullable = false;
		JsonPropertyInfoValues<List<ModelId>> propertyInfo21 = new JsonPropertyInfoValues<List<ModelId>>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializablePlayer),
			Converter = null,
			Getter = (object obj) => ((SerializablePlayer)obj).DiscoveredRelics,
			Setter = (object obj, List<ModelId>? value) =>
			{
				((SerializablePlayer)obj).DiscoveredRelics = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "DiscoveredRelics",
			JsonPropertyName = "discovered_relics",
			AttributeProviderFactory = () => typeof(SerializablePlayer).GetProperty("DiscoveredRelics", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(List<ModelId>), Array.Empty<Type>(), null)
		};
		array[20] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo21);
		array[20].IsGetNullable = false;
		array[20].IsSetNullable = false;
		return array;
	}

	private JsonTypeInfo<SerializablePlayerOddsSet> Create_SerializablePlayerOddsSet(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<SerializablePlayerOddsSet> jsonTypeInfo))
		{
			JsonObjectInfoValues<SerializablePlayerOddsSet> objectInfo = new JsonObjectInfoValues<SerializablePlayerOddsSet>
			{
				ObjectCreator = () => new SerializablePlayerOddsSet(),
				ObjectWithParameterizedConstructorCreator = null,
				PropertyMetadataInitializer = (JsonSerializerContext _) => SerializablePlayerOddsSetPropInit(options),
				ConstructorParameterMetadataInitializer = null,
				ConstructorAttributeProviderFactory = () => typeof(SerializablePlayerOddsSet).GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Array.Empty<Type>(), null),
				SerializeHandler = null
			};
			jsonTypeInfo = JsonMetadataServices.CreateObjectInfo(options, objectInfo);
			jsonTypeInfo.NumberHandling = null;
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private static JsonPropertyInfo[] SerializablePlayerOddsSetPropInit(JsonSerializerOptions options)
	{
		JsonPropertyInfo[] array = new JsonPropertyInfo[2];
		JsonPropertyInfoValues<float> propertyInfo = new JsonPropertyInfoValues<float>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializablePlayerOddsSet),
			Converter = null,
			Getter = (object obj) => ((SerializablePlayerOddsSet)obj).CardRarityOddsValue,
			Setter = (object obj, float value) =>
			{
				((SerializablePlayerOddsSet)obj).CardRarityOddsValue = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "CardRarityOddsValue",
			JsonPropertyName = "card_rarity_odds_value",
			AttributeProviderFactory = () => typeof(SerializablePlayerOddsSet).GetProperty("CardRarityOddsValue", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(float), Array.Empty<Type>(), null)
		};
		array[0] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo);
		JsonPropertyInfoValues<float> propertyInfo2 = new JsonPropertyInfoValues<float>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializablePlayerOddsSet),
			Converter = null,
			Getter = (object obj) => ((SerializablePlayerOddsSet)obj).PotionRewardOddsValue,
			Setter = (object obj, float value) =>
			{
				((SerializablePlayerOddsSet)obj).PotionRewardOddsValue = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "PotionRewardOddsValue",
			JsonPropertyName = "potion_reward_odds_value",
			AttributeProviderFactory = () => typeof(SerializablePlayerOddsSet).GetProperty("PotionRewardOddsValue", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(float), Array.Empty<Type>(), null)
		};
		array[1] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo2);
		return array;
	}

	private JsonTypeInfo<SerializablePotion> Create_SerializablePotion(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<SerializablePotion> jsonTypeInfo))
		{
			JsonObjectInfoValues<SerializablePotion> objectInfo = new JsonObjectInfoValues<SerializablePotion>
			{
				ObjectCreator = () => new SerializablePotion(),
				ObjectWithParameterizedConstructorCreator = null,
				PropertyMetadataInitializer = (JsonSerializerContext _) => SerializablePotionPropInit(options),
				ConstructorParameterMetadataInitializer = null,
				ConstructorAttributeProviderFactory = () => typeof(SerializablePotion).GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Array.Empty<Type>(), null),
				SerializeHandler = null
			};
			jsonTypeInfo = JsonMetadataServices.CreateObjectInfo(options, objectInfo);
			jsonTypeInfo.NumberHandling = null;
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private static JsonPropertyInfo[] SerializablePotionPropInit(JsonSerializerOptions options)
	{
		JsonPropertyInfo[] array = new JsonPropertyInfo[2];
		JsonPropertyInfoValues<ModelId> propertyInfo = new JsonPropertyInfoValues<ModelId>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializablePotion),
			Converter = null,
			Getter = (object obj) => ((SerializablePotion)obj).Id,
			Setter = (object obj, ModelId? value) =>
			{
				((SerializablePotion)obj).Id = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "Id",
			JsonPropertyName = "id",
			AttributeProviderFactory = () => typeof(SerializablePotion).GetProperty("Id", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(ModelId), Array.Empty<Type>(), null)
		};
		array[0] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo);
		JsonPropertyInfoValues<int> propertyInfo2 = new JsonPropertyInfoValues<int>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializablePotion),
			Converter = null,
			Getter = (object obj) => ((SerializablePotion)obj).SlotIndex,
			Setter = (object obj, int value) =>
			{
				((SerializablePotion)obj).SlotIndex = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "SlotIndex",
			JsonPropertyName = "slot_index",
			AttributeProviderFactory = () => typeof(SerializablePotion).GetProperty("SlotIndex", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(int), Array.Empty<Type>(), null)
		};
		array[1] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo2);
		return array;
	}

	private JsonTypeInfo<SerializableRelic> Create_SerializableRelic(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<SerializableRelic> jsonTypeInfo))
		{
			JsonObjectInfoValues<SerializableRelic> objectInfo = new JsonObjectInfoValues<SerializableRelic>
			{
				ObjectCreator = () => new SerializableRelic(),
				ObjectWithParameterizedConstructorCreator = null,
				PropertyMetadataInitializer = (JsonSerializerContext _) => SerializableRelicPropInit(options),
				ConstructorParameterMetadataInitializer = null,
				ConstructorAttributeProviderFactory = () => typeof(SerializableRelic).GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Array.Empty<Type>(), null),
				SerializeHandler = null
			};
			jsonTypeInfo = JsonMetadataServices.CreateObjectInfo(options, objectInfo);
			jsonTypeInfo.NumberHandling = null;
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private static JsonPropertyInfo[] SerializableRelicPropInit(JsonSerializerOptions options)
	{
		JsonPropertyInfo[] array = new JsonPropertyInfo[3];
		JsonPropertyInfoValues<ModelId> propertyInfo = new JsonPropertyInfoValues<ModelId>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializableRelic),
			Converter = null,
			Getter = (object obj) => ((SerializableRelic)obj).Id,
			Setter = (object obj, ModelId? value) =>
			{
				((SerializableRelic)obj).Id = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "Id",
			JsonPropertyName = "id",
			AttributeProviderFactory = () => typeof(SerializableRelic).GetProperty("Id", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(ModelId), Array.Empty<Type>(), null)
		};
		array[0] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo);
		JsonPropertyInfoValues<SavedProperties> propertyInfo2 = new JsonPropertyInfoValues<SavedProperties>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializableRelic),
			Converter = null,
			Getter = (object obj) => ((SerializableRelic)obj).Props,
			Setter = (object obj, SavedProperties? value) =>
			{
				((SerializableRelic)obj).Props = value;
			},
			IgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "Props",
			JsonPropertyName = "props",
			AttributeProviderFactory = () => typeof(SerializableRelic).GetProperty("Props", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(SavedProperties), Array.Empty<Type>(), null)
		};
		array[1] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo2);
		JsonPropertyInfoValues<int?> propertyInfo3 = new JsonPropertyInfoValues<int?>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializableRelic),
			Converter = null,
			Getter = (object obj) => ((SerializableRelic)obj).FloorAddedToDeck,
			Setter = (object obj, int? value) =>
			{
				((SerializableRelic)obj).FloorAddedToDeck = value;
			},
			IgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "FloorAddedToDeck",
			JsonPropertyName = "floor_added_to_deck",
			AttributeProviderFactory = () => typeof(SerializableRelic).GetProperty("FloorAddedToDeck", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(int?), Array.Empty<Type>(), null)
		};
		array[2] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo3);
		return array;
	}

	private JsonTypeInfo<SerializableRelicGrabBag> Create_SerializableRelicGrabBag(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<SerializableRelicGrabBag> jsonTypeInfo))
		{
			JsonObjectInfoValues<SerializableRelicGrabBag> objectInfo = new JsonObjectInfoValues<SerializableRelicGrabBag>
			{
				ObjectCreator = () => new SerializableRelicGrabBag(),
				ObjectWithParameterizedConstructorCreator = null,
				PropertyMetadataInitializer = (JsonSerializerContext _) => SerializableRelicGrabBagPropInit(options),
				ConstructorParameterMetadataInitializer = null,
				ConstructorAttributeProviderFactory = () => typeof(SerializableRelicGrabBag).GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Array.Empty<Type>(), null),
				SerializeHandler = null
			};
			jsonTypeInfo = JsonMetadataServices.CreateObjectInfo(options, objectInfo);
			jsonTypeInfo.NumberHandling = null;
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private static JsonPropertyInfo[] SerializableRelicGrabBagPropInit(JsonSerializerOptions options)
	{
		JsonPropertyInfo[] array = new JsonPropertyInfo[1];
		JsonPropertyInfoValues<Dictionary<RelicRarity, List<ModelId>>> propertyInfo = new JsonPropertyInfoValues<Dictionary<RelicRarity, List<ModelId>>>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializableRelicGrabBag),
			Converter = null,
			Getter = (object obj) => ((SerializableRelicGrabBag)obj).RelicIdLists,
			Setter = (object obj, Dictionary<RelicRarity, List<ModelId>>? value) =>
			{
				((SerializableRelicGrabBag)obj).RelicIdLists = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "RelicIdLists",
			JsonPropertyName = "relic_id_lists",
			AttributeProviderFactory = () => typeof(SerializableRelicGrabBag).GetProperty("RelicIdLists", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(Dictionary<RelicRarity, List<ModelId>>), Array.Empty<Type>(), null)
		};
		array[0] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo);
		array[0].IsGetNullable = false;
		array[0].IsSetNullable = false;
		return array;
	}

	private JsonTypeInfo<SerializableReward> Create_SerializableReward(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<SerializableReward> jsonTypeInfo))
		{
			JsonObjectInfoValues<SerializableReward> objectInfo = new JsonObjectInfoValues<SerializableReward>
			{
				ObjectCreator = () => new SerializableReward(),
				ObjectWithParameterizedConstructorCreator = null,
				PropertyMetadataInitializer = (JsonSerializerContext _) => SerializableRewardPropInit(options),
				ConstructorParameterMetadataInitializer = null,
				ConstructorAttributeProviderFactory = () => typeof(SerializableReward).GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Array.Empty<Type>(), null),
				SerializeHandler = null
			};
			jsonTypeInfo = JsonMetadataServices.CreateObjectInfo(options, objectInfo);
			jsonTypeInfo.NumberHandling = null;
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private static JsonPropertyInfo[] SerializableRewardPropInit(JsonSerializerOptions options)
	{
		JsonPropertyInfo[] array = new JsonPropertyInfo[10];
		JsonPropertyInfoValues<RewardType> propertyInfo = new JsonPropertyInfoValues<RewardType>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializableReward),
			Converter = null,
			Getter = (object obj) => ((SerializableReward)obj).RewardType,
			Setter = (object obj, RewardType value) =>
			{
				((SerializableReward)obj).RewardType = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "RewardType",
			JsonPropertyName = "reward_type",
			AttributeProviderFactory = () => typeof(SerializableReward).GetProperty("RewardType", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(RewardType), Array.Empty<Type>(), null)
		};
		array[0] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo);
		JsonPropertyInfoValues<ModelId> propertyInfo2 = new JsonPropertyInfoValues<ModelId>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializableReward),
			Converter = null,
			Getter = (object obj) => ((SerializableReward)obj).PredeterminedModelId,
			Setter = (object obj, ModelId? value) =>
			{
				((SerializableReward)obj).PredeterminedModelId = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "PredeterminedModelId",
			JsonPropertyName = "predetermined_model_id",
			AttributeProviderFactory = () => typeof(SerializableReward).GetProperty("PredeterminedModelId", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(ModelId), Array.Empty<Type>(), null)
		};
		array[1] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo2);
		array[1].IsGetNullable = false;
		array[1].IsSetNullable = false;
		JsonPropertyInfoValues<SerializableCard> propertyInfo3 = new JsonPropertyInfoValues<SerializableCard>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializableReward),
			Converter = null,
			Getter = (object obj) => ((SerializableReward)obj).SpecialCard,
			Setter = (object obj, SerializableCard? value) =>
			{
				((SerializableReward)obj).SpecialCard = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "SpecialCard",
			JsonPropertyName = "special_card",
			AttributeProviderFactory = () => typeof(SerializableReward).GetProperty("SpecialCard", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(SerializableCard), Array.Empty<Type>(), null)
		};
		array[2] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo3);
		JsonPropertyInfoValues<int> propertyInfo4 = new JsonPropertyInfoValues<int>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializableReward),
			Converter = null,
			Getter = (object obj) => ((SerializableReward)obj).GoldAmount,
			Setter = (object obj, int value) =>
			{
				((SerializableReward)obj).GoldAmount = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "GoldAmount",
			JsonPropertyName = "gold_amount",
			AttributeProviderFactory = () => typeof(SerializableReward).GetProperty("GoldAmount", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(int), Array.Empty<Type>(), null)
		};
		array[3] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo4);
		JsonPropertyInfoValues<bool> propertyInfo5 = new JsonPropertyInfoValues<bool>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializableReward),
			Converter = null,
			Getter = (object obj) => ((SerializableReward)obj).WasGoldStolenBack,
			Setter = (object obj, bool value) =>
			{
				((SerializableReward)obj).WasGoldStolenBack = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "WasGoldStolenBack",
			JsonPropertyName = "was_gold_stolen_back",
			AttributeProviderFactory = () => typeof(SerializableReward).GetProperty("WasGoldStolenBack", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(bool), Array.Empty<Type>(), null)
		};
		array[4] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo5);
		JsonPropertyInfoValues<CardCreationSource> propertyInfo6 = new JsonPropertyInfoValues<CardCreationSource>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializableReward),
			Converter = null,
			Getter = (object obj) => ((SerializableReward)obj).Source,
			Setter = (object obj, CardCreationSource value) =>
			{
				((SerializableReward)obj).Source = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "Source",
			JsonPropertyName = "source",
			AttributeProviderFactory = () => typeof(SerializableReward).GetProperty("Source", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(CardCreationSource), Array.Empty<Type>(), null)
		};
		array[5] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo6);
		JsonPropertyInfoValues<CardRarityOddsType> propertyInfo7 = new JsonPropertyInfoValues<CardRarityOddsType>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializableReward),
			Converter = null,
			Getter = (object obj) => ((SerializableReward)obj).RarityOdds,
			Setter = (object obj, CardRarityOddsType value) =>
			{
				((SerializableReward)obj).RarityOdds = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "RarityOdds",
			JsonPropertyName = "rarity_odds",
			AttributeProviderFactory = () => typeof(SerializableReward).GetProperty("RarityOdds", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(CardRarityOddsType), Array.Empty<Type>(), null)
		};
		array[6] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo7);
		JsonPropertyInfoValues<List<ModelId>> propertyInfo8 = new JsonPropertyInfoValues<List<ModelId>>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializableReward),
			Converter = null,
			Getter = (object obj) => ((SerializableReward)obj).CardPoolIds,
			Setter = (object obj, List<ModelId>? value) =>
			{
				((SerializableReward)obj).CardPoolIds = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "CardPoolIds",
			JsonPropertyName = "card_pools",
			AttributeProviderFactory = () => typeof(SerializableReward).GetProperty("CardPoolIds", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(List<ModelId>), Array.Empty<Type>(), null)
		};
		array[7] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo8);
		array[7].IsGetNullable = false;
		array[7].IsSetNullable = false;
		JsonPropertyInfoValues<int> propertyInfo9 = new JsonPropertyInfoValues<int>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializableReward),
			Converter = null,
			Getter = (object obj) => ((SerializableReward)obj).OptionCount,
			Setter = (object obj, int value) =>
			{
				((SerializableReward)obj).OptionCount = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "OptionCount",
			JsonPropertyName = "option_count",
			AttributeProviderFactory = () => typeof(SerializableReward).GetProperty("OptionCount", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(int), Array.Empty<Type>(), null)
		};
		array[8] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo9);
		JsonPropertyInfoValues<ModelId> propertyInfo10 = new JsonPropertyInfoValues<ModelId>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializableReward),
			Converter = null,
			Getter = (object obj) => ((SerializableReward)obj).CustomDescriptionEncounterSourceId,
			Setter = (object obj, ModelId? value) =>
			{
				((SerializableReward)obj).CustomDescriptionEncounterSourceId = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "CustomDescriptionEncounterSourceId",
			JsonPropertyName = "custom_description_encounter_source_id",
			AttributeProviderFactory = () => typeof(SerializableReward).GetProperty("CustomDescriptionEncounterSourceId", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(ModelId), Array.Empty<Type>(), null)
		};
		array[9] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo10);
		array[9].IsGetNullable = false;
		array[9].IsSetNullable = false;
		return array;
	}

	private JsonTypeInfo<SerializableRoom> Create_SerializableRoom(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<SerializableRoom> jsonTypeInfo))
		{
			JsonObjectInfoValues<SerializableRoom> objectInfo = new JsonObjectInfoValues<SerializableRoom>
			{
				ObjectCreator = () => new SerializableRoom(),
				ObjectWithParameterizedConstructorCreator = null,
				PropertyMetadataInitializer = (JsonSerializerContext _) => SerializableRoomPropInit(options),
				ConstructorParameterMetadataInitializer = null,
				ConstructorAttributeProviderFactory = () => typeof(SerializableRoom).GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Array.Empty<Type>(), null),
				SerializeHandler = null
			};
			jsonTypeInfo = JsonMetadataServices.CreateObjectInfo(options, objectInfo);
			jsonTypeInfo.NumberHandling = null;
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private static JsonPropertyInfo[] SerializableRoomPropInit(JsonSerializerOptions options)
	{
		JsonPropertyInfo[] array = new JsonPropertyInfo[9];
		JsonPropertyInfoValues<RoomType> propertyInfo = new JsonPropertyInfoValues<RoomType>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializableRoom),
			Converter = null,
			Getter = (object obj) => ((SerializableRoom)obj).RoomType,
			Setter = (object obj, RoomType value) =>
			{
				((SerializableRoom)obj).RoomType = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "RoomType",
			JsonPropertyName = "room_type",
			AttributeProviderFactory = () => typeof(SerializableRoom).GetProperty("RoomType", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(RoomType), Array.Empty<Type>(), null)
		};
		array[0] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo);
		JsonPropertyInfoValues<ModelId> propertyInfo2 = new JsonPropertyInfoValues<ModelId>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializableRoom),
			Converter = null,
			Getter = (object obj) => ((SerializableRoom)obj).EncounterId,
			Setter = (object obj, ModelId? value) =>
			{
				((SerializableRoom)obj).EncounterId = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "EncounterId",
			JsonPropertyName = "encounter_id",
			AttributeProviderFactory = () => typeof(SerializableRoom).GetProperty("EncounterId", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(ModelId), Array.Empty<Type>(), null)
		};
		array[1] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo2);
		JsonPropertyInfoValues<ModelId> propertyInfo3 = new JsonPropertyInfoValues<ModelId>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializableRoom),
			Converter = null,
			Getter = (object obj) => ((SerializableRoom)obj).EventId,
			Setter = (object obj, ModelId? value) =>
			{
				((SerializableRoom)obj).EventId = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "EventId",
			JsonPropertyName = "event_id",
			AttributeProviderFactory = () => typeof(SerializableRoom).GetProperty("EventId", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(ModelId), Array.Empty<Type>(), null)
		};
		array[2] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo3);
		JsonPropertyInfoValues<bool> propertyInfo4 = new JsonPropertyInfoValues<bool>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializableRoom),
			Converter = null,
			Getter = (object obj) => ((SerializableRoom)obj).IsPreFinished,
			Setter = (object obj, bool value) =>
			{
				((SerializableRoom)obj).IsPreFinished = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "IsPreFinished",
			JsonPropertyName = "is_pre_finished",
			AttributeProviderFactory = () => typeof(SerializableRoom).GetProperty("IsPreFinished", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(bool), Array.Empty<Type>(), null)
		};
		array[3] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo4);
		JsonPropertyInfoValues<float> propertyInfo5 = new JsonPropertyInfoValues<float>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializableRoom),
			Converter = null,
			Getter = (object obj) => ((SerializableRoom)obj).GoldProportion,
			Setter = (object obj, float value) =>
			{
				((SerializableRoom)obj).GoldProportion = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "GoldProportion",
			JsonPropertyName = "reward_proportion",
			AttributeProviderFactory = () => typeof(SerializableRoom).GetProperty("GoldProportion", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(float), Array.Empty<Type>(), null)
		};
		array[4] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo5);
		JsonPropertyInfoValues<Dictionary<ulong, List<SerializableReward>>> propertyInfo6 = new JsonPropertyInfoValues<Dictionary<ulong, List<SerializableReward>>>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializableRoom),
			Converter = null,
			Getter = (object obj) => ((SerializableRoom)obj).ExtraRewards,
			Setter = (object obj, Dictionary<ulong, List<SerializableReward>>? value) =>
			{
				((SerializableRoom)obj).ExtraRewards = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "ExtraRewards",
			JsonPropertyName = "extra_rewards",
			AttributeProviderFactory = () => typeof(SerializableRoom).GetProperty("ExtraRewards", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(Dictionary<ulong, List<SerializableReward>>), Array.Empty<Type>(), null)
		};
		array[5] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo6);
		array[5].IsGetNullable = false;
		array[5].IsSetNullable = false;
		JsonPropertyInfoValues<ModelId> propertyInfo7 = new JsonPropertyInfoValues<ModelId>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializableRoom),
			Converter = null,
			Getter = (object obj) => ((SerializableRoom)obj).ParentEventId,
			Setter = (object obj, ModelId? value) =>
			{
				((SerializableRoom)obj).ParentEventId = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "ParentEventId",
			JsonPropertyName = "parent_event_id",
			AttributeProviderFactory = () => typeof(SerializableRoom).GetProperty("ParentEventId", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(ModelId), Array.Empty<Type>(), null)
		};
		array[6] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo7);
		JsonPropertyInfoValues<bool> propertyInfo8 = new JsonPropertyInfoValues<bool>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializableRoom),
			Converter = null,
			Getter = (object obj) => ((SerializableRoom)obj).ShouldResumeParentEvent,
			Setter = (object obj, bool value) =>
			{
				((SerializableRoom)obj).ShouldResumeParentEvent = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "ShouldResumeParentEvent",
			JsonPropertyName = "should_resume_parent_event",
			AttributeProviderFactory = () => typeof(SerializableRoom).GetProperty("ShouldResumeParentEvent", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(bool), Array.Empty<Type>(), null)
		};
		array[7] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo8);
		JsonPropertyInfoValues<Dictionary<string, string>> propertyInfo9 = new JsonPropertyInfoValues<Dictionary<string, string>>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializableRoom),
			Converter = null,
			Getter = (object obj) => ((SerializableRoom)obj).EncounterState,
			Setter = (object obj, Dictionary<string, string>? value) =>
			{
				((SerializableRoom)obj).EncounterState = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "EncounterState",
			JsonPropertyName = "encounter_state",
			AttributeProviderFactory = () => typeof(SerializableRoom).GetProperty("EncounterState", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(Dictionary<string, string>), Array.Empty<Type>(), null)
		};
		array[8] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo9);
		array[8].IsGetNullable = false;
		array[8].IsSetNullable = false;
		return array;
	}

	private JsonTypeInfo<SerializableRoomSet> Create_SerializableRoomSet(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<SerializableRoomSet> jsonTypeInfo))
		{
			JsonObjectInfoValues<SerializableRoomSet> objectInfo = new JsonObjectInfoValues<SerializableRoomSet>
			{
				ObjectCreator = () => new SerializableRoomSet(),
				ObjectWithParameterizedConstructorCreator = null,
				PropertyMetadataInitializer = (JsonSerializerContext _) => SerializableRoomSetPropInit(options),
				ConstructorParameterMetadataInitializer = null,
				ConstructorAttributeProviderFactory = () => typeof(SerializableRoomSet).GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Array.Empty<Type>(), null),
				SerializeHandler = null
			};
			jsonTypeInfo = JsonMetadataServices.CreateObjectInfo(options, objectInfo);
			jsonTypeInfo.NumberHandling = null;
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private static JsonPropertyInfo[] SerializableRoomSetPropInit(JsonSerializerOptions options)
	{
		JsonPropertyInfo[] array = new JsonPropertyInfo[10];
		JsonPropertyInfoValues<List<ModelId>> propertyInfo = new JsonPropertyInfoValues<List<ModelId>>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializableRoomSet),
			Converter = null,
			Getter = (object obj) => ((SerializableRoomSet)obj).EventIds,
			Setter = (object obj, List<ModelId>? value) =>
			{
				((SerializableRoomSet)obj).EventIds = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "EventIds",
			JsonPropertyName = "event_ids",
			AttributeProviderFactory = () => typeof(SerializableRoomSet).GetProperty("EventIds", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(List<ModelId>), Array.Empty<Type>(), null)
		};
		array[0] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo);
		array[0].IsGetNullable = false;
		array[0].IsSetNullable = false;
		JsonPropertyInfoValues<int> propertyInfo2 = new JsonPropertyInfoValues<int>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializableRoomSet),
			Converter = null,
			Getter = (object obj) => ((SerializableRoomSet)obj).EventsVisited,
			Setter = (object obj, int value) =>
			{
				((SerializableRoomSet)obj).EventsVisited = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "EventsVisited",
			JsonPropertyName = "events_visited",
			AttributeProviderFactory = () => typeof(SerializableRoomSet).GetProperty("EventsVisited", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(int), Array.Empty<Type>(), null)
		};
		array[1] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo2);
		JsonPropertyInfoValues<List<ModelId>> propertyInfo3 = new JsonPropertyInfoValues<List<ModelId>>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializableRoomSet),
			Converter = null,
			Getter = (object obj) => ((SerializableRoomSet)obj).NormalEncounterIds,
			Setter = (object obj, List<ModelId>? value) =>
			{
				((SerializableRoomSet)obj).NormalEncounterIds = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "NormalEncounterIds",
			JsonPropertyName = "normal_encounter_ids",
			AttributeProviderFactory = () => typeof(SerializableRoomSet).GetProperty("NormalEncounterIds", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(List<ModelId>), Array.Empty<Type>(), null)
		};
		array[2] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo3);
		array[2].IsGetNullable = false;
		array[2].IsSetNullable = false;
		JsonPropertyInfoValues<int> propertyInfo4 = new JsonPropertyInfoValues<int>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializableRoomSet),
			Converter = null,
			Getter = (object obj) => ((SerializableRoomSet)obj).NormalEncountersVisited,
			Setter = (object obj, int value) =>
			{
				((SerializableRoomSet)obj).NormalEncountersVisited = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "NormalEncountersVisited",
			JsonPropertyName = "normal_encounters_visited",
			AttributeProviderFactory = () => typeof(SerializableRoomSet).GetProperty("NormalEncountersVisited", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(int), Array.Empty<Type>(), null)
		};
		array[3] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo4);
		JsonPropertyInfoValues<List<ModelId>> propertyInfo5 = new JsonPropertyInfoValues<List<ModelId>>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializableRoomSet),
			Converter = null,
			Getter = (object obj) => ((SerializableRoomSet)obj).EliteEncounterIds,
			Setter = (object obj, List<ModelId>? value) =>
			{
				((SerializableRoomSet)obj).EliteEncounterIds = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "EliteEncounterIds",
			JsonPropertyName = "elite_encounter_ids",
			AttributeProviderFactory = () => typeof(SerializableRoomSet).GetProperty("EliteEncounterIds", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(List<ModelId>), Array.Empty<Type>(), null)
		};
		array[4] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo5);
		array[4].IsGetNullable = false;
		array[4].IsSetNullable = false;
		JsonPropertyInfoValues<int> propertyInfo6 = new JsonPropertyInfoValues<int>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializableRoomSet),
			Converter = null,
			Getter = (object obj) => ((SerializableRoomSet)obj).EliteEncountersVisited,
			Setter = (object obj, int value) =>
			{
				((SerializableRoomSet)obj).EliteEncountersVisited = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "EliteEncountersVisited",
			JsonPropertyName = "elite_encounters_visited",
			AttributeProviderFactory = () => typeof(SerializableRoomSet).GetProperty("EliteEncountersVisited", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(int), Array.Empty<Type>(), null)
		};
		array[5] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo6);
		JsonPropertyInfoValues<int> propertyInfo7 = new JsonPropertyInfoValues<int>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializableRoomSet),
			Converter = null,
			Getter = (object obj) => ((SerializableRoomSet)obj).BossEncountersVisited,
			Setter = (object obj, int value) =>
			{
				((SerializableRoomSet)obj).BossEncountersVisited = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "BossEncountersVisited",
			JsonPropertyName = "boss_encounters_visited",
			AttributeProviderFactory = () => typeof(SerializableRoomSet).GetProperty("BossEncountersVisited", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(int), Array.Empty<Type>(), null)
		};
		array[6] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo7);
		JsonPropertyInfoValues<ModelId> propertyInfo8 = new JsonPropertyInfoValues<ModelId>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializableRoomSet),
			Converter = null,
			Getter = (object obj) => ((SerializableRoomSet)obj).BossId,
			Setter = (object obj, ModelId? value) =>
			{
				((SerializableRoomSet)obj).BossId = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "BossId",
			JsonPropertyName = "boss_id",
			AttributeProviderFactory = () => typeof(SerializableRoomSet).GetProperty("BossId", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(ModelId), Array.Empty<Type>(), null)
		};
		array[7] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo8);
		JsonPropertyInfoValues<ModelId> propertyInfo9 = new JsonPropertyInfoValues<ModelId>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializableRoomSet),
			Converter = null,
			Getter = (object obj) => ((SerializableRoomSet)obj).SecondBossId,
			Setter = (object obj, ModelId? value) =>
			{
				((SerializableRoomSet)obj).SecondBossId = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "SecondBossId",
			JsonPropertyName = "second_boss_id",
			AttributeProviderFactory = () => typeof(SerializableRoomSet).GetProperty("SecondBossId", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(ModelId), Array.Empty<Type>(), null)
		};
		array[8] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo9);
		JsonPropertyInfoValues<ModelId> propertyInfo10 = new JsonPropertyInfoValues<ModelId>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializableRoomSet),
			Converter = null,
			Getter = (object obj) => ((SerializableRoomSet)obj).AncientId,
			Setter = (object obj, ModelId? value) =>
			{
				((SerializableRoomSet)obj).AncientId = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "AncientId",
			JsonPropertyName = "ancient_id",
			AttributeProviderFactory = () => typeof(SerializableRoomSet).GetProperty("AncientId", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(ModelId), Array.Empty<Type>(), null)
		};
		array[9] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo10);
		return array;
	}

	private JsonTypeInfo<SerializableRunOddsSet> Create_SerializableRunOddsSet(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<SerializableRunOddsSet> jsonTypeInfo))
		{
			JsonObjectInfoValues<SerializableRunOddsSet> objectInfo = new JsonObjectInfoValues<SerializableRunOddsSet>
			{
				ObjectCreator = () => new SerializableRunOddsSet(),
				ObjectWithParameterizedConstructorCreator = null,
				PropertyMetadataInitializer = (JsonSerializerContext _) => SerializableRunOddsSetPropInit(options),
				ConstructorParameterMetadataInitializer = null,
				ConstructorAttributeProviderFactory = () => typeof(SerializableRunOddsSet).GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Array.Empty<Type>(), null),
				SerializeHandler = null
			};
			jsonTypeInfo = JsonMetadataServices.CreateObjectInfo(options, objectInfo);
			jsonTypeInfo.NumberHandling = null;
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private static JsonPropertyInfo[] SerializableRunOddsSetPropInit(JsonSerializerOptions options)
	{
		JsonPropertyInfo[] array = new JsonPropertyInfo[4];
		JsonPropertyInfoValues<float> propertyInfo = new JsonPropertyInfoValues<float>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializableRunOddsSet),
			Converter = null,
			Getter = (object obj) => ((SerializableRunOddsSet)obj).UnknownMapPointMonsterOddsValue,
			Setter = (object obj, float value) =>
			{
				((SerializableRunOddsSet)obj).UnknownMapPointMonsterOddsValue = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "UnknownMapPointMonsterOddsValue",
			JsonPropertyName = "unknown_map_point_monster_odds_value",
			AttributeProviderFactory = () => typeof(SerializableRunOddsSet).GetProperty("UnknownMapPointMonsterOddsValue", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(float), Array.Empty<Type>(), null)
		};
		array[0] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo);
		JsonPropertyInfoValues<float> propertyInfo2 = new JsonPropertyInfoValues<float>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializableRunOddsSet),
			Converter = null,
			Getter = (object obj) => ((SerializableRunOddsSet)obj).UnknownMapPointEliteOddsValue,
			Setter = (object obj, float value) =>
			{
				((SerializableRunOddsSet)obj).UnknownMapPointEliteOddsValue = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "UnknownMapPointEliteOddsValue",
			JsonPropertyName = "unknown_map_point_elite_odds_value",
			AttributeProviderFactory = () => typeof(SerializableRunOddsSet).GetProperty("UnknownMapPointEliteOddsValue", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(float), Array.Empty<Type>(), null)
		};
		array[1] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo2);
		JsonPropertyInfoValues<float> propertyInfo3 = new JsonPropertyInfoValues<float>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializableRunOddsSet),
			Converter = null,
			Getter = (object obj) => ((SerializableRunOddsSet)obj).UnknownMapPointTreasureOddsValue,
			Setter = (object obj, float value) =>
			{
				((SerializableRunOddsSet)obj).UnknownMapPointTreasureOddsValue = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "UnknownMapPointTreasureOddsValue",
			JsonPropertyName = "unknown_map_point_treasure_odds_value",
			AttributeProviderFactory = () => typeof(SerializableRunOddsSet).GetProperty("UnknownMapPointTreasureOddsValue", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(float), Array.Empty<Type>(), null)
		};
		array[2] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo3);
		JsonPropertyInfoValues<float> propertyInfo4 = new JsonPropertyInfoValues<float>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializableRunOddsSet),
			Converter = null,
			Getter = (object obj) => ((SerializableRunOddsSet)obj).UnknownMapPointShopOddsValue,
			Setter = (object obj, float value) =>
			{
				((SerializableRunOddsSet)obj).UnknownMapPointShopOddsValue = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "UnknownMapPointShopOddsValue",
			JsonPropertyName = "unknown_map_point_shop_odds_value",
			AttributeProviderFactory = () => typeof(SerializableRunOddsSet).GetProperty("UnknownMapPointShopOddsValue", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(float), Array.Empty<Type>(), null)
		};
		array[3] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo4);
		return array;
	}

	private JsonTypeInfo<SerializableRunRngSet> Create_SerializableRunRngSet(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<SerializableRunRngSet> jsonTypeInfo))
		{
			JsonObjectInfoValues<SerializableRunRngSet> objectInfo = new JsonObjectInfoValues<SerializableRunRngSet>
			{
				ObjectCreator = () => new SerializableRunRngSet(),
				ObjectWithParameterizedConstructorCreator = null,
				PropertyMetadataInitializer = (JsonSerializerContext _) => SerializableRunRngSetPropInit(options),
				ConstructorParameterMetadataInitializer = null,
				ConstructorAttributeProviderFactory = () => typeof(SerializableRunRngSet).GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Array.Empty<Type>(), null),
				SerializeHandler = null
			};
			jsonTypeInfo = JsonMetadataServices.CreateObjectInfo(options, objectInfo);
			jsonTypeInfo.NumberHandling = null;
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private static JsonPropertyInfo[] SerializableRunRngSetPropInit(JsonSerializerOptions options)
	{
		JsonPropertyInfo[] array = new JsonPropertyInfo[2];
		JsonPropertyInfoValues<string> propertyInfo = new JsonPropertyInfoValues<string>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializableRunRngSet),
			Converter = null,
			Getter = (object obj) => ((SerializableRunRngSet)obj).Seed,
			Setter = (object obj, string? value) =>
			{
				((SerializableRunRngSet)obj).Seed = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "Seed",
			JsonPropertyName = "seed",
			AttributeProviderFactory = () => typeof(SerializableRunRngSet).GetProperty("Seed", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(string), Array.Empty<Type>(), null)
		};
		array[0] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo);
		JsonPropertyInfoValues<Dictionary<RunRngType, SerializableRng>> propertyInfo2 = new JsonPropertyInfoValues<Dictionary<RunRngType, SerializableRng>>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializableRunRngSet),
			Converter = null,
			Getter = (object obj) => ((SerializableRunRngSet)obj).Rngs,
			Setter = (object obj, Dictionary<RunRngType, SerializableRng>? value) =>
			{
				((SerializableRunRngSet)obj).Rngs = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "Rngs",
			JsonPropertyName = "rngs",
			AttributeProviderFactory = () => typeof(SerializableRunRngSet).GetProperty("Rngs", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(Dictionary<RunRngType, SerializableRng>), Array.Empty<Type>(), null)
		};
		array[1] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo2);
		array[1].IsGetNullable = false;
		array[1].IsSetNullable = false;
		return array;
	}

	private JsonTypeInfo<SerializableEpoch> Create_SerializableEpoch(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<SerializableEpoch> jsonTypeInfo))
		{
			JsonObjectInfoValues<SerializableEpoch> objectInfo = new JsonObjectInfoValues<SerializableEpoch>
			{
				ObjectCreator = null,
				ObjectWithParameterizedConstructorCreator = (object[] args) => new SerializableEpoch((string)args[0], (EpochState)args[1]),
				PropertyMetadataInitializer = (JsonSerializerContext _) => SerializableEpochPropInit(options),
				ConstructorParameterMetadataInitializer = SerializableEpochCtorParamInit,
				ConstructorAttributeProviderFactory = () => typeof(SerializableEpoch).GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[2]
				{
					typeof(string),
					typeof(EpochState)
				}, null),
				SerializeHandler = null
			};
			jsonTypeInfo = JsonMetadataServices.CreateObjectInfo(options, objectInfo);
			jsonTypeInfo.NumberHandling = null;
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private static JsonPropertyInfo[] SerializableEpochPropInit(JsonSerializerOptions options)
	{
		JsonPropertyInfo[] array = new JsonPropertyInfo[3];
		JsonPropertyInfoValues<string> propertyInfo = new JsonPropertyInfoValues<string>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializableEpoch),
			Converter = null,
			Getter = (object obj) => ((SerializableEpoch)obj).Id,
			Setter = null,
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "Id",
			JsonPropertyName = "id",
			AttributeProviderFactory = () => typeof(SerializableEpoch).GetProperty("Id", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(string), Array.Empty<Type>(), null)
		};
		array[0] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo);
		array[0].IsGetNullable = false;
		JsonPropertyInfoValues<EpochState> propertyInfo2 = new JsonPropertyInfoValues<EpochState>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializableEpoch),
			Converter = null,
			Getter = (object obj) => ((SerializableEpoch)obj).State,
			Setter = (object obj, EpochState value) =>
			{
				((SerializableEpoch)obj).State = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "State",
			JsonPropertyName = "state",
			AttributeProviderFactory = () => typeof(SerializableEpoch).GetProperty("State", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(EpochState), Array.Empty<Type>(), null)
		};
		array[1] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo2);
		JsonPropertyInfoValues<long> propertyInfo3 = new JsonPropertyInfoValues<long>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializableEpoch),
			Converter = null,
			Getter = (object obj) => ((SerializableEpoch)obj).ObtainDate,
			Setter = (object obj, long value) =>
			{
				((SerializableEpoch)obj).ObtainDate = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "ObtainDate",
			JsonPropertyName = "obtain_date",
			AttributeProviderFactory = () => typeof(SerializableEpoch).GetProperty("ObtainDate", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(long), Array.Empty<Type>(), null)
		};
		array[2] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo3);
		return array;
	}

	private static JsonParameterInfoValues[] SerializableEpochCtorParamInit()
	{
		return new JsonParameterInfoValues[2]
		{
			new JsonParameterInfoValues
			{
				Name = "id",
				ParameterType = typeof(string),
				Position = 0,
				HasDefaultValue = false,
				DefaultValue = null,
				IsNullable = false
			},
			new JsonParameterInfoValues
			{
				Name = "state",
				ParameterType = typeof(EpochState),
				Position = 1,
				HasDefaultValue = false,
				DefaultValue = null,
				IsNullable = false
			}
		};
	}

	private JsonTypeInfo<SerializableExtraPlayerFields> Create_SerializableExtraPlayerFields(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<SerializableExtraPlayerFields> jsonTypeInfo))
		{
			JsonObjectInfoValues<SerializableExtraPlayerFields> objectInfo = new JsonObjectInfoValues<SerializableExtraPlayerFields>
			{
				ObjectCreator = () => new SerializableExtraPlayerFields(),
				ObjectWithParameterizedConstructorCreator = null,
				PropertyMetadataInitializer = (JsonSerializerContext _) => SerializableExtraPlayerFieldsPropInit(options),
				ConstructorParameterMetadataInitializer = null,
				ConstructorAttributeProviderFactory = () => typeof(SerializableExtraPlayerFields).GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Array.Empty<Type>(), null),
				SerializeHandler = null
			};
			jsonTypeInfo = JsonMetadataServices.CreateObjectInfo(options, objectInfo);
			jsonTypeInfo.NumberHandling = null;
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private static JsonPropertyInfo[] SerializableExtraPlayerFieldsPropInit(JsonSerializerOptions options)
	{
		JsonPropertyInfo[] array = new JsonPropertyInfo[5];
		JsonPropertyInfoValues<int> propertyInfo = new JsonPropertyInfoValues<int>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializableExtraPlayerFields),
			Converter = null,
			Getter = (object obj) => ((SerializableExtraPlayerFields)obj).CardShopRemovalsUsed,
			Setter = (object obj, int value) =>
			{
				((SerializableExtraPlayerFields)obj).CardShopRemovalsUsed = value;
			},
			IgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "CardShopRemovalsUsed",
			JsonPropertyName = "card_shop_removals_used",
			AttributeProviderFactory = () => typeof(SerializableExtraPlayerFields).GetProperty("CardShopRemovalsUsed", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(int), Array.Empty<Type>(), null)
		};
		array[0] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo);
		JsonPropertyInfoValues<int> propertyInfo2 = new JsonPropertyInfoValues<int>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializableExtraPlayerFields),
			Converter = null,
			Getter = (object obj) => ((SerializableExtraPlayerFields)obj).WongoPoints,
			Setter = (object obj, int value) =>
			{
				((SerializableExtraPlayerFields)obj).WongoPoints = value;
			},
			IgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "WongoPoints",
			JsonPropertyName = "wongo_points",
			AttributeProviderFactory = () => typeof(SerializableExtraPlayerFields).GetProperty("WongoPoints", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(int), Array.Empty<Type>(), null)
		};
		array[1] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo2);
		JsonPropertyInfoValues<bool> propertyInfo3 = new JsonPropertyInfoValues<bool>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializableExtraPlayerFields),
			Converter = null,
			Getter = (object obj) => ((SerializableExtraPlayerFields)obj).CccomboBadgeUnlocked,
			Setter = (object obj, bool value) =>
			{
				((SerializableExtraPlayerFields)obj).CccomboBadgeUnlocked = value;
			},
			IgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "CccomboBadgeUnlocked",
			JsonPropertyName = "ccccombo_badge_unlocked",
			AttributeProviderFactory = () => typeof(SerializableExtraPlayerFields).GetProperty("CccomboBadgeUnlocked", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(bool), Array.Empty<Type>(), null)
		};
		array[2] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo3);
		JsonPropertyInfoValues<int> propertyInfo4 = new JsonPropertyInfoValues<int>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializableExtraPlayerFields),
			Converter = null,
			Getter = (object obj) => ((SerializableExtraPlayerFields)obj).DamageDealt,
			Setter = (object obj, int value) =>
			{
				((SerializableExtraPlayerFields)obj).DamageDealt = value;
			},
			IgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "DamageDealt",
			JsonPropertyName = "damage_dealt",
			AttributeProviderFactory = () => typeof(SerializableExtraPlayerFields).GetProperty("DamageDealt", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(int), Array.Empty<Type>(), null)
		};
		array[3] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo4);
		JsonPropertyInfoValues<int> propertyInfo5 = new JsonPropertyInfoValues<int>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializableExtraPlayerFields),
			Converter = null,
			Getter = (object obj) => ((SerializableExtraPlayerFields)obj).DebuffsApplied,
			Setter = (object obj, int value) =>
			{
				((SerializableExtraPlayerFields)obj).DebuffsApplied = value;
			},
			IgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "DebuffsApplied",
			JsonPropertyName = "debuffs_applied",
			AttributeProviderFactory = () => typeof(SerializableExtraPlayerFields).GetProperty("DebuffsApplied", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(int), Array.Empty<Type>(), null)
		};
		array[4] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo5);
		return array;
	}

	private JsonTypeInfo<SerializablePlayerRngSet> Create_SerializablePlayerRngSet(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<SerializablePlayerRngSet> jsonTypeInfo))
		{
			JsonObjectInfoValues<SerializablePlayerRngSet> objectInfo = new JsonObjectInfoValues<SerializablePlayerRngSet>
			{
				ObjectCreator = () => new SerializablePlayerRngSet(),
				ObjectWithParameterizedConstructorCreator = null,
				PropertyMetadataInitializer = (JsonSerializerContext _) => SerializablePlayerRngSetPropInit(options),
				ConstructorParameterMetadataInitializer = null,
				ConstructorAttributeProviderFactory = () => typeof(SerializablePlayerRngSet).GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Array.Empty<Type>(), null),
				SerializeHandler = null
			};
			jsonTypeInfo = JsonMetadataServices.CreateObjectInfo(options, objectInfo);
			jsonTypeInfo.NumberHandling = null;
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private static JsonPropertyInfo[] SerializablePlayerRngSetPropInit(JsonSerializerOptions options)
	{
		JsonPropertyInfo[] array = new JsonPropertyInfo[2];
		JsonPropertyInfoValues<ulong> propertyInfo = new JsonPropertyInfoValues<ulong>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializablePlayerRngSet),
			Converter = null,
			Getter = (object obj) => ((SerializablePlayerRngSet)obj).Seed,
			Setter = (object obj, ulong value) =>
			{
				((SerializablePlayerRngSet)obj).Seed = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "Seed",
			JsonPropertyName = "seed",
			AttributeProviderFactory = () => typeof(SerializablePlayerRngSet).GetProperty("Seed", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(ulong), Array.Empty<Type>(), null)
		};
		array[0] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo);
		JsonPropertyInfoValues<Dictionary<PlayerRngType, SerializableRng>> propertyInfo2 = new JsonPropertyInfoValues<Dictionary<PlayerRngType, SerializableRng>>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializablePlayerRngSet),
			Converter = null,
			Getter = (object obj) => ((SerializablePlayerRngSet)obj).Rngs,
			Setter = (object obj, Dictionary<PlayerRngType, SerializableRng>? value) =>
			{
				((SerializablePlayerRngSet)obj).Rngs = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "Rngs",
			JsonPropertyName = "rngs",
			AttributeProviderFactory = () => typeof(SerializablePlayerRngSet).GetProperty("Rngs", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(Dictionary<PlayerRngType, SerializableRng>), Array.Empty<Type>(), null)
		};
		array[1] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo2);
		array[1].IsGetNullable = false;
		array[1].IsSetNullable = false;
		return array;
	}

	private JsonTypeInfo<SerializableProgress> Create_SerializableProgress(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<SerializableProgress> jsonTypeInfo))
		{
			JsonObjectInfoValues<SerializableProgress> objectInfo = new JsonObjectInfoValues<SerializableProgress>
			{
				ObjectCreator = null,
				ObjectWithParameterizedConstructorCreator = (object[] args) => new SerializableProgress
				{
					UniqueId = (string)args[0]
				},
				PropertyMetadataInitializer = (JsonSerializerContext _) => SerializableProgressPropInit(options),
				ConstructorParameterMetadataInitializer = SerializableProgressCtorParamInit,
				ConstructorAttributeProviderFactory = () => typeof(SerializableProgress).GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Array.Empty<Type>(), null),
				SerializeHandler = null
			};
			jsonTypeInfo = JsonMetadataServices.CreateObjectInfo(options, objectInfo);
			jsonTypeInfo.NumberHandling = null;
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private static JsonPropertyInfo[] SerializableProgressPropInit(JsonSerializerOptions options)
	{
		JsonPropertyInfo[] array = new JsonPropertyInfo[31];
		JsonPropertyInfoValues<int> propertyInfo = new JsonPropertyInfoValues<int>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializableProgress),
			Converter = null,
			Getter = (object obj) => ((SerializableProgress)obj).SchemaVersion,
			Setter = (object obj, int value) =>
			{
				((SerializableProgress)obj).SchemaVersion = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "SchemaVersion",
			JsonPropertyName = "schema_version",
			AttributeProviderFactory = () => typeof(SerializableProgress).GetProperty("SchemaVersion", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(int), Array.Empty<Type>(), null)
		};
		array[0] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo);
		JsonPropertyInfoValues<string> propertyInfo2 = new JsonPropertyInfoValues<string>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializableProgress),
			Converter = null,
			Getter = (object obj) => ((SerializableProgress)obj).UniqueId,
			Setter = (object obj, string? value) =>
			{
				throw new InvalidOperationException("Setting init-only properties is not supported in source generation mode.");
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "UniqueId",
			JsonPropertyName = "unique_id",
			AttributeProviderFactory = () => typeof(SerializableProgress).GetProperty("UniqueId", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(string), Array.Empty<Type>(), null)
		};
		array[1] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo2);
		array[1].IsGetNullable = false;
		array[1].IsSetNullable = false;
		JsonPropertyInfoValues<List<CharacterStats>> propertyInfo3 = new JsonPropertyInfoValues<List<CharacterStats>>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializableProgress),
			Converter = null,
			Getter = (object obj) => ((SerializableProgress)obj).CharStats,
			Setter = (object obj, List<CharacterStats>? value) =>
			{
				((SerializableProgress)obj).CharStats = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "CharStats",
			JsonPropertyName = "character_stats",
			AttributeProviderFactory = () => typeof(SerializableProgress).GetProperty("CharStats", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(List<CharacterStats>), Array.Empty<Type>(), null)
		};
		array[2] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo3);
		array[2].IsGetNullable = false;
		array[2].IsSetNullable = false;
		JsonPropertyInfoValues<List<CardStats>> propertyInfo4 = new JsonPropertyInfoValues<List<CardStats>>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializableProgress),
			Converter = null,
			Getter = (object obj) => ((SerializableProgress)obj).CardStats,
			Setter = (object obj, List<CardStats>? value) =>
			{
				((SerializableProgress)obj).CardStats = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "CardStats",
			JsonPropertyName = "card_stats",
			AttributeProviderFactory = () => typeof(SerializableProgress).GetProperty("CardStats", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(List<CardStats>), Array.Empty<Type>(), null)
		};
		array[3] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo4);
		array[3].IsGetNullable = false;
		array[3].IsSetNullable = false;
		JsonPropertyInfoValues<List<EncounterStats>> propertyInfo5 = new JsonPropertyInfoValues<List<EncounterStats>>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializableProgress),
			Converter = null,
			Getter = (object obj) => ((SerializableProgress)obj).EncounterStats,
			Setter = (object obj, List<EncounterStats>? value) =>
			{
				((SerializableProgress)obj).EncounterStats = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "EncounterStats",
			JsonPropertyName = "encounter_stats",
			AttributeProviderFactory = () => typeof(SerializableProgress).GetProperty("EncounterStats", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(List<EncounterStats>), Array.Empty<Type>(), null)
		};
		array[4] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo5);
		array[4].IsGetNullable = false;
		array[4].IsSetNullable = false;
		JsonPropertyInfoValues<List<EnemyStats>> propertyInfo6 = new JsonPropertyInfoValues<List<EnemyStats>>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializableProgress),
			Converter = null,
			Getter = (object obj) => ((SerializableProgress)obj).EnemyStats,
			Setter = (object obj, List<EnemyStats>? value) =>
			{
				((SerializableProgress)obj).EnemyStats = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "EnemyStats",
			JsonPropertyName = "enemy_stats",
			AttributeProviderFactory = () => typeof(SerializableProgress).GetProperty("EnemyStats", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(List<EnemyStats>), Array.Empty<Type>(), null)
		};
		array[5] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo6);
		array[5].IsGetNullable = false;
		array[5].IsSetNullable = false;
		JsonPropertyInfoValues<List<AncientStats>> propertyInfo7 = new JsonPropertyInfoValues<List<AncientStats>>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializableProgress),
			Converter = null,
			Getter = (object obj) => ((SerializableProgress)obj).AncientStats,
			Setter = (object obj, List<AncientStats>? value) =>
			{
				((SerializableProgress)obj).AncientStats = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "AncientStats",
			JsonPropertyName = "ancient_stats",
			AttributeProviderFactory = () => typeof(SerializableProgress).GetProperty("AncientStats", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(List<AncientStats>), Array.Empty<Type>(), null)
		};
		array[6] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo7);
		array[6].IsGetNullable = false;
		array[6].IsSetNullable = false;
		JsonPropertyInfoValues<bool> propertyInfo8 = new JsonPropertyInfoValues<bool>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializableProgress),
			Converter = null,
			Getter = (object obj) => ((SerializableProgress)obj).EnableFtues,
			Setter = (object obj, bool value) =>
			{
				((SerializableProgress)obj).EnableFtues = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "EnableFtues",
			JsonPropertyName = "enable_ftues",
			AttributeProviderFactory = () => typeof(SerializableProgress).GetProperty("EnableFtues", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(bool), Array.Empty<Type>(), null)
		};
		array[7] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo8);
		JsonPropertyInfoValues<List<SerializableEpoch>> propertyInfo9 = new JsonPropertyInfoValues<List<SerializableEpoch>>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializableProgress),
			Converter = null,
			Getter = (object obj) => ((SerializableProgress)obj).Epochs,
			Setter = (object obj, List<SerializableEpoch>? value) =>
			{
				((SerializableProgress)obj).Epochs = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "Epochs",
			JsonPropertyName = "epochs",
			AttributeProviderFactory = () => typeof(SerializableProgress).GetProperty("Epochs", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(List<SerializableEpoch>), Array.Empty<Type>(), null)
		};
		array[8] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo9);
		array[8].IsGetNullable = false;
		array[8].IsSetNullable = false;
		JsonPropertyInfoValues<List<string>> propertyInfo10 = new JsonPropertyInfoValues<List<string>>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializableProgress),
			Converter = null,
			Getter = (object obj) => ((SerializableProgress)obj).FtueCompleted,
			Setter = (object obj, List<string>? value) =>
			{
				((SerializableProgress)obj).FtueCompleted = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "FtueCompleted",
			JsonPropertyName = "ftue_completed",
			AttributeProviderFactory = () => typeof(SerializableProgress).GetProperty("FtueCompleted", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(List<string>), Array.Empty<Type>(), null)
		};
		array[9] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo10);
		array[9].IsGetNullable = false;
		array[9].IsSetNullable = false;
		JsonPropertyInfoValues<List<SerializableUnlockedAchievement>> propertyInfo11 = new JsonPropertyInfoValues<List<SerializableUnlockedAchievement>>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializableProgress),
			Converter = null,
			Getter = (object obj) => ((SerializableProgress)obj).UnlockedAchievements,
			Setter = (object obj, List<SerializableUnlockedAchievement>? value) =>
			{
				((SerializableProgress)obj).UnlockedAchievements = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "UnlockedAchievements",
			JsonPropertyName = "unlocked_achievements",
			AttributeProviderFactory = () => typeof(SerializableProgress).GetProperty("UnlockedAchievements", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(List<SerializableUnlockedAchievement>), Array.Empty<Type>(), null)
		};
		array[10] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo11);
		array[10].IsGetNullable = false;
		array[10].IsSetNullable = false;
		JsonPropertyInfoValues<List<ModelId>> propertyInfo12 = new JsonPropertyInfoValues<List<ModelId>>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializableProgress),
			Converter = null,
			Getter = (object obj) => ((SerializableProgress)obj).DiscoveredCards,
			Setter = (object obj, List<ModelId>? value) =>
			{
				((SerializableProgress)obj).DiscoveredCards = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "DiscoveredCards",
			JsonPropertyName = "discovered_cards",
			AttributeProviderFactory = () => typeof(SerializableProgress).GetProperty("DiscoveredCards", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(List<ModelId>), Array.Empty<Type>(), null)
		};
		array[11] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo12);
		array[11].IsGetNullable = false;
		array[11].IsSetNullable = false;
		JsonPropertyInfoValues<List<ModelId>> propertyInfo13 = new JsonPropertyInfoValues<List<ModelId>>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializableProgress),
			Converter = null,
			Getter = (object obj) => ((SerializableProgress)obj).DiscoveredRelics,
			Setter = (object obj, List<ModelId>? value) =>
			{
				((SerializableProgress)obj).DiscoveredRelics = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "DiscoveredRelics",
			JsonPropertyName = "discovered_relics",
			AttributeProviderFactory = () => typeof(SerializableProgress).GetProperty("DiscoveredRelics", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(List<ModelId>), Array.Empty<Type>(), null)
		};
		array[12] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo13);
		array[12].IsGetNullable = false;
		array[12].IsSetNullable = false;
		JsonPropertyInfoValues<List<ModelId>> propertyInfo14 = new JsonPropertyInfoValues<List<ModelId>>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializableProgress),
			Converter = null,
			Getter = (object obj) => ((SerializableProgress)obj).DiscoveredEvents,
			Setter = (object obj, List<ModelId>? value) =>
			{
				((SerializableProgress)obj).DiscoveredEvents = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "DiscoveredEvents",
			JsonPropertyName = "discovered_events",
			AttributeProviderFactory = () => typeof(SerializableProgress).GetProperty("DiscoveredEvents", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(List<ModelId>), Array.Empty<Type>(), null)
		};
		array[13] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo14);
		array[13].IsGetNullable = false;
		array[13].IsSetNullable = false;
		JsonPropertyInfoValues<List<ModelId>> propertyInfo15 = new JsonPropertyInfoValues<List<ModelId>>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializableProgress),
			Converter = null,
			Getter = (object obj) => ((SerializableProgress)obj).DiscoveredPotions,
			Setter = (object obj, List<ModelId>? value) =>
			{
				((SerializableProgress)obj).DiscoveredPotions = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "DiscoveredPotions",
			JsonPropertyName = "discovered_potions",
			AttributeProviderFactory = () => typeof(SerializableProgress).GetProperty("DiscoveredPotions", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(List<ModelId>), Array.Empty<Type>(), null)
		};
		array[14] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo15);
		array[14].IsGetNullable = false;
		array[14].IsSetNullable = false;
		JsonPropertyInfoValues<List<ModelId>> propertyInfo16 = new JsonPropertyInfoValues<List<ModelId>>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializableProgress),
			Converter = null,
			Getter = (object obj) => ((SerializableProgress)obj).DiscoveredActs,
			Setter = (object obj, List<ModelId>? value) =>
			{
				((SerializableProgress)obj).DiscoveredActs = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "DiscoveredActs",
			JsonPropertyName = "discovered_acts",
			AttributeProviderFactory = () => typeof(SerializableProgress).GetProperty("DiscoveredActs", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(List<ModelId>), Array.Empty<Type>(), null)
		};
		array[15] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo16);
		array[15].IsGetNullable = false;
		array[15].IsSetNullable = false;
		JsonPropertyInfoValues<long> propertyInfo17 = new JsonPropertyInfoValues<long>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializableProgress),
			Converter = null,
			Getter = (object obj) => ((SerializableProgress)obj).TotalPlaytime,
			Setter = (object obj, long value) =>
			{
				((SerializableProgress)obj).TotalPlaytime = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "TotalPlaytime",
			JsonPropertyName = "total_playtime",
			AttributeProviderFactory = () => typeof(SerializableProgress).GetProperty("TotalPlaytime", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(long), Array.Empty<Type>(), null)
		};
		array[16] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo17);
		JsonPropertyInfoValues<int> propertyInfo18 = new JsonPropertyInfoValues<int>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializableProgress),
			Converter = null,
			Getter = (object obj) => ((SerializableProgress)obj).TotalUnlocks,
			Setter = (object obj, int value) =>
			{
				((SerializableProgress)obj).TotalUnlocks = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "TotalUnlocks",
			JsonPropertyName = "total_unlocks",
			AttributeProviderFactory = () => typeof(SerializableProgress).GetProperty("TotalUnlocks", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(int), Array.Empty<Type>(), null)
		};
		array[17] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo18);
		JsonPropertyInfoValues<int> propertyInfo19 = new JsonPropertyInfoValues<int>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializableProgress),
			Converter = null,
			Getter = (object obj) => ((SerializableProgress)obj).CurrentScore,
			Setter = (object obj, int value) =>
			{
				((SerializableProgress)obj).CurrentScore = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "CurrentScore",
			JsonPropertyName = "current_score",
			AttributeProviderFactory = () => typeof(SerializableProgress).GetProperty("CurrentScore", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(int), Array.Empty<Type>(), null)
		};
		array[18] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo19);
		JsonPropertyInfoValues<long> propertyInfo20 = new JsonPropertyInfoValues<long>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializableProgress),
			Converter = null,
			Getter = (object obj) => ((SerializableProgress)obj).FloorsClimbed,
			Setter = (object obj, long value) =>
			{
				((SerializableProgress)obj).FloorsClimbed = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "FloorsClimbed",
			JsonPropertyName = "floors_climbed",
			AttributeProviderFactory = () => typeof(SerializableProgress).GetProperty("FloorsClimbed", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(long), Array.Empty<Type>(), null)
		};
		array[19] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo20);
		JsonPropertyInfoValues<long> propertyInfo21 = new JsonPropertyInfoValues<long>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializableProgress),
			Converter = null,
			Getter = (object obj) => ((SerializableProgress)obj).ArchitectDamage,
			Setter = (object obj, long value) =>
			{
				((SerializableProgress)obj).ArchitectDamage = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "ArchitectDamage",
			JsonPropertyName = "architect_damage",
			AttributeProviderFactory = () => typeof(SerializableProgress).GetProperty("ArchitectDamage", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(long), Array.Empty<Type>(), null)
		};
		array[20] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo21);
		JsonPropertyInfoValues<int> propertyInfo22 = new JsonPropertyInfoValues<int>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializableProgress),
			Converter = null,
			Getter = (object obj) => ((SerializableProgress)obj).WongoPoints,
			Setter = (object obj, int value) =>
			{
				((SerializableProgress)obj).WongoPoints = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "WongoPoints",
			JsonPropertyName = "wongo_points",
			AttributeProviderFactory = () => typeof(SerializableProgress).GetProperty("WongoPoints", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(int), Array.Empty<Type>(), null)
		};
		array[21] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo22);
		JsonPropertyInfoValues<int> propertyInfo23 = new JsonPropertyInfoValues<int>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializableProgress),
			Converter = null,
			Getter = (object obj) => ((SerializableProgress)obj).PreferredMultiplayerAscension,
			Setter = (object obj, int value) =>
			{
				((SerializableProgress)obj).PreferredMultiplayerAscension = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "PreferredMultiplayerAscension",
			JsonPropertyName = "preferred_multiplayer_ascension",
			AttributeProviderFactory = () => typeof(SerializableProgress).GetProperty("PreferredMultiplayerAscension", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(int), Array.Empty<Type>(), null)
		};
		array[22] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo23);
		JsonPropertyInfoValues<int> propertyInfo24 = new JsonPropertyInfoValues<int>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializableProgress),
			Converter = null,
			Getter = (object obj) => ((SerializableProgress)obj).MaxMultiplayerAscension,
			Setter = (object obj, int value) =>
			{
				((SerializableProgress)obj).MaxMultiplayerAscension = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "MaxMultiplayerAscension",
			JsonPropertyName = "max_multiplayer_ascension",
			AttributeProviderFactory = () => typeof(SerializableProgress).GetProperty("MaxMultiplayerAscension", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(int), Array.Empty<Type>(), null)
		};
		array[23] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo24);
		JsonPropertyInfoValues<int> propertyInfo25 = new JsonPropertyInfoValues<int>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializableProgress),
			Converter = null,
			Getter = (object obj) => ((SerializableProgress)obj).TestSubjectKills,
			Setter = (object obj, int value) =>
			{
				((SerializableProgress)obj).TestSubjectKills = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "TestSubjectKills",
			JsonPropertyName = "test_subject_kills",
			AttributeProviderFactory = () => typeof(SerializableProgress).GetProperty("TestSubjectKills", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(int), Array.Empty<Type>(), null)
		};
		array[24] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo25);
		JsonPropertyInfoValues<ModelId> propertyInfo26 = new JsonPropertyInfoValues<ModelId>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializableProgress),
			Converter = null,
			Getter = (object obj) => ((SerializableProgress)obj).PendingCharacterUnlock,
			Setter = (object obj, ModelId? value) =>
			{
				((SerializableProgress)obj).PendingCharacterUnlock = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "PendingCharacterUnlock",
			JsonPropertyName = "pending_character_unlock",
			AttributeProviderFactory = () => typeof(SerializableProgress).GetProperty("PendingCharacterUnlock", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(ModelId), Array.Empty<Type>(), null)
		};
		array[25] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo26);
		array[25].IsGetNullable = false;
		array[25].IsSetNullable = false;
		JsonPropertyInfoValues<int> propertyInfo27 = new JsonPropertyInfoValues<int>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializableProgress),
			Converter = null,
			Getter = null,
			Setter = null,
			IgnoreCondition = JsonIgnoreCondition.Always,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "Wins",
			JsonPropertyName = null,
			AttributeProviderFactory = () => typeof(SerializableProgress).GetProperty("Wins", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(int), Array.Empty<Type>(), null)
		};
		array[26] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo27);
		JsonPropertyInfoValues<int> propertyInfo28 = new JsonPropertyInfoValues<int>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializableProgress),
			Converter = null,
			Getter = null,
			Setter = null,
			IgnoreCondition = JsonIgnoreCondition.Always,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "Losses",
			JsonPropertyName = null,
			AttributeProviderFactory = () => typeof(SerializableProgress).GetProperty("Losses", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(int), Array.Empty<Type>(), null)
		};
		array[27] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo28);
		JsonPropertyInfoValues<long> propertyInfo29 = new JsonPropertyInfoValues<long>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializableProgress),
			Converter = null,
			Getter = null,
			Setter = null,
			IgnoreCondition = JsonIgnoreCondition.Always,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "FastestVictory",
			JsonPropertyName = null,
			AttributeProviderFactory = () => typeof(SerializableProgress).GetProperty("FastestVictory", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(long), Array.Empty<Type>(), null)
		};
		array[28] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo29);
		JsonPropertyInfoValues<long> propertyInfo30 = new JsonPropertyInfoValues<long>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializableProgress),
			Converter = null,
			Getter = null,
			Setter = null,
			IgnoreCondition = JsonIgnoreCondition.Always,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "BestWinStreak",
			JsonPropertyName = null,
			AttributeProviderFactory = () => typeof(SerializableProgress).GetProperty("BestWinStreak", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(long), Array.Empty<Type>(), null)
		};
		array[29] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo30);
		JsonPropertyInfoValues<int> propertyInfo31 = new JsonPropertyInfoValues<int>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializableProgress),
			Converter = null,
			Getter = null,
			Setter = null,
			IgnoreCondition = JsonIgnoreCondition.Always,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "NumberOfRuns",
			JsonPropertyName = null,
			AttributeProviderFactory = () => typeof(SerializableProgress).GetProperty("NumberOfRuns", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(int), Array.Empty<Type>(), null)
		};
		array[30] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo31);
		return array;
	}

	private static JsonParameterInfoValues[] SerializableProgressCtorParamInit()
	{
		return new JsonParameterInfoValues[1]
		{
			new JsonParameterInfoValues
			{
				Name = "UniqueId",
				ParameterType = typeof(string),
				Position = 0,
				IsNullable = false,
				IsMemberInitializer = true
			}
		};
	}

	private JsonTypeInfo<SerializableRng> Create_SerializableRng(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<SerializableRng> jsonTypeInfo))
		{
			JsonObjectInfoValues<SerializableRng> objectInfo = new JsonObjectInfoValues<SerializableRng>
			{
				ObjectCreator = () => new SerializableRng(),
				ObjectWithParameterizedConstructorCreator = null,
				PropertyMetadataInitializer = (JsonSerializerContext _) => SerializableRngPropInit(options),
				ConstructorParameterMetadataInitializer = null,
				ConstructorAttributeProviderFactory = () => typeof(SerializableRng).GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Array.Empty<Type>(), null),
				SerializeHandler = null
			};
			jsonTypeInfo = JsonMetadataServices.CreateObjectInfo(options, objectInfo);
			jsonTypeInfo.NumberHandling = null;
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private static JsonPropertyInfo[] SerializableRngPropInit(JsonSerializerOptions options)
	{
		JsonPropertyInfo[] array = new JsonPropertyInfo[5];
		JsonPropertyInfoValues<int> propertyInfo = new JsonPropertyInfoValues<int>
		{
			IsProperty = false,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializableRng),
			Converter = null,
			Getter = (object obj) => ((SerializableRng)obj).counter,
			Setter = (object obj, int value) =>
			{
				((SerializableRng)obj).counter = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "counter",
			JsonPropertyName = "counter",
			AttributeProviderFactory = () => typeof(SerializableRng).GetField("counter", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
		};
		array[0] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo);
		JsonPropertyInfoValues<ulong> propertyInfo2 = new JsonPropertyInfoValues<ulong>
		{
			IsProperty = false,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializableRng),
			Converter = null,
			Getter = (object obj) => ((SerializableRng)obj).state0,
			Setter = (object obj, ulong value) =>
			{
				((SerializableRng)obj).state0 = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "state0",
			JsonPropertyName = "s0",
			AttributeProviderFactory = () => typeof(SerializableRng).GetField("state0", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
		};
		array[1] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo2);
		JsonPropertyInfoValues<ulong> propertyInfo3 = new JsonPropertyInfoValues<ulong>
		{
			IsProperty = false,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializableRng),
			Converter = null,
			Getter = (object obj) => ((SerializableRng)obj).state1,
			Setter = (object obj, ulong value) =>
			{
				((SerializableRng)obj).state1 = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "state1",
			JsonPropertyName = "s1",
			AttributeProviderFactory = () => typeof(SerializableRng).GetField("state1", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
		};
		array[2] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo3);
		JsonPropertyInfoValues<ulong> propertyInfo4 = new JsonPropertyInfoValues<ulong>
		{
			IsProperty = false,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializableRng),
			Converter = null,
			Getter = (object obj) => ((SerializableRng)obj).state2,
			Setter = (object obj, ulong value) =>
			{
				((SerializableRng)obj).state2 = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "state2",
			JsonPropertyName = "s2",
			AttributeProviderFactory = () => typeof(SerializableRng).GetField("state2", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
		};
		array[3] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo4);
		JsonPropertyInfoValues<ulong> propertyInfo5 = new JsonPropertyInfoValues<ulong>
		{
			IsProperty = false,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializableRng),
			Converter = null,
			Getter = (object obj) => ((SerializableRng)obj).state3,
			Setter = (object obj, ulong value) =>
			{
				((SerializableRng)obj).state3 = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "state3",
			JsonPropertyName = "s3",
			AttributeProviderFactory = () => typeof(SerializableRng).GetField("state3", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
		};
		array[4] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo5);
		return array;
	}

	private JsonTypeInfo<SerializableRun> Create_SerializableRun(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<SerializableRun> jsonTypeInfo))
		{
			JsonObjectInfoValues<SerializableRun> objectInfo = new JsonObjectInfoValues<SerializableRun>
			{
				ObjectCreator = () => new SerializableRun(),
				ObjectWithParameterizedConstructorCreator = null,
				PropertyMetadataInitializer = (JsonSerializerContext _) => SerializableRunPropInit(options),
				ConstructorParameterMetadataInitializer = null,
				ConstructorAttributeProviderFactory = () => typeof(SerializableRun).GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Array.Empty<Type>(), null),
				SerializeHandler = null
			};
			jsonTypeInfo = JsonMetadataServices.CreateObjectInfo(options, objectInfo);
			jsonTypeInfo.NumberHandling = null;
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private static JsonPropertyInfo[] SerializableRunPropInit(JsonSerializerOptions options)
	{
		JsonPropertyInfo[] array = new JsonPropertyInfo[24];
		JsonPropertyInfoValues<int> propertyInfo = new JsonPropertyInfoValues<int>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializableRun),
			Converter = null,
			Getter = (object obj) => ((SerializableRun)obj).SchemaVersion,
			Setter = (object obj, int value) =>
			{
				((SerializableRun)obj).SchemaVersion = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "SchemaVersion",
			JsonPropertyName = "schema_version",
			AttributeProviderFactory = () => typeof(SerializableRun).GetProperty("SchemaVersion", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(int), Array.Empty<Type>(), null)
		};
		array[0] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo);
		JsonPropertyInfoValues<List<SerializableActModel>> propertyInfo2 = new JsonPropertyInfoValues<List<SerializableActModel>>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializableRun),
			Converter = null,
			Getter = (object obj) => ((SerializableRun)obj).Acts,
			Setter = (object obj, List<SerializableActModel>? value) =>
			{
				((SerializableRun)obj).Acts = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "Acts",
			JsonPropertyName = "acts",
			AttributeProviderFactory = () => typeof(SerializableRun).GetProperty("Acts", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(List<SerializableActModel>), Array.Empty<Type>(), null)
		};
		array[1] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo2);
		array[1].IsGetNullable = false;
		array[1].IsSetNullable = false;
		JsonPropertyInfoValues<List<SerializableModifier>> propertyInfo3 = new JsonPropertyInfoValues<List<SerializableModifier>>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializableRun),
			Converter = null,
			Getter = (object obj) => ((SerializableRun)obj).Modifiers,
			Setter = (object obj, List<SerializableModifier>? value) =>
			{
				((SerializableRun)obj).Modifiers = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "Modifiers",
			JsonPropertyName = "modifiers",
			AttributeProviderFactory = () => typeof(SerializableRun).GetProperty("Modifiers", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(List<SerializableModifier>), Array.Empty<Type>(), null)
		};
		array[2] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo3);
		array[2].IsGetNullable = false;
		array[2].IsSetNullable = false;
		JsonPropertyInfoValues<DateTimeOffset?> propertyInfo4 = new JsonPropertyInfoValues<DateTimeOffset?>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializableRun),
			Converter = null,
			Getter = (object obj) => ((SerializableRun)obj).DailyTime,
			Setter = (object obj, DateTimeOffset? value) =>
			{
				((SerializableRun)obj).DailyTime = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "DailyTime",
			JsonPropertyName = "dailyTime",
			AttributeProviderFactory = () => typeof(SerializableRun).GetProperty("DailyTime", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(DateTimeOffset?), Array.Empty<Type>(), null)
		};
		array[3] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo4);
		JsonPropertyInfoValues<int> propertyInfo5 = new JsonPropertyInfoValues<int>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializableRun),
			Converter = null,
			Getter = (object obj) => ((SerializableRun)obj).CurrentActIndex,
			Setter = (object obj, int value) =>
			{
				((SerializableRun)obj).CurrentActIndex = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "CurrentActIndex",
			JsonPropertyName = "current_act_index",
			AttributeProviderFactory = () => typeof(SerializableRun).GetProperty("CurrentActIndex", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(int), Array.Empty<Type>(), null)
		};
		array[4] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo5);
		JsonPropertyInfoValues<List<ModelId>> propertyInfo6 = new JsonPropertyInfoValues<List<ModelId>>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializableRun),
			Converter = null,
			Getter = (object obj) => ((SerializableRun)obj).EventsSeen,
			Setter = (object obj, List<ModelId>? value) =>
			{
				((SerializableRun)obj).EventsSeen = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "EventsSeen",
			JsonPropertyName = "events_seen",
			AttributeProviderFactory = () => typeof(SerializableRun).GetProperty("EventsSeen", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(List<ModelId>), Array.Empty<Type>(), null)
		};
		array[5] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo6);
		array[5].IsGetNullable = false;
		array[5].IsSetNullable = false;
		JsonPropertyInfoValues<SerializableRoom> propertyInfo7 = new JsonPropertyInfoValues<SerializableRoom>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializableRun),
			Converter = null,
			Getter = (object obj) => ((SerializableRun)obj).PreFinishedRoom,
			Setter = (object obj, SerializableRoom? value) =>
			{
				((SerializableRun)obj).PreFinishedRoom = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "PreFinishedRoom",
			JsonPropertyName = "pre_finished_room",
			AttributeProviderFactory = () => typeof(SerializableRun).GetProperty("PreFinishedRoom", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(SerializableRoom), Array.Empty<Type>(), null)
		};
		array[6] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo7);
		JsonPropertyInfoValues<SerializableRunOddsSet> propertyInfo8 = new JsonPropertyInfoValues<SerializableRunOddsSet>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializableRun),
			Converter = null,
			Getter = (object obj) => ((SerializableRun)obj).SerializableOdds,
			Setter = (object obj, SerializableRunOddsSet? value) =>
			{
				((SerializableRun)obj).SerializableOdds = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "SerializableOdds",
			JsonPropertyName = "odds",
			AttributeProviderFactory = () => typeof(SerializableRun).GetProperty("SerializableOdds", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(SerializableRunOddsSet), Array.Empty<Type>(), null)
		};
		array[7] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo8);
		array[7].IsGetNullable = false;
		array[7].IsSetNullable = false;
		JsonPropertyInfoValues<SerializableRelicGrabBag> propertyInfo9 = new JsonPropertyInfoValues<SerializableRelicGrabBag>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializableRun),
			Converter = null,
			Getter = (object obj) => ((SerializableRun)obj).SerializableSharedRelicGrabBag,
			Setter = (object obj, SerializableRelicGrabBag? value) =>
			{
				((SerializableRun)obj).SerializableSharedRelicGrabBag = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "SerializableSharedRelicGrabBag",
			JsonPropertyName = "shared_relic_grab_bag",
			AttributeProviderFactory = () => typeof(SerializableRun).GetProperty("SerializableSharedRelicGrabBag", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(SerializableRelicGrabBag), Array.Empty<Type>(), null)
		};
		array[8] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo9);
		array[8].IsGetNullable = false;
		array[8].IsSetNullable = false;
		JsonPropertyInfoValues<List<SerializablePlayer>> propertyInfo10 = new JsonPropertyInfoValues<List<SerializablePlayer>>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializableRun),
			Converter = null,
			Getter = (object obj) => ((SerializableRun)obj).Players,
			Setter = (object obj, List<SerializablePlayer>? value) =>
			{
				((SerializableRun)obj).Players = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "Players",
			JsonPropertyName = "players",
			AttributeProviderFactory = () => typeof(SerializableRun).GetProperty("Players", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(List<SerializablePlayer>), Array.Empty<Type>(), null)
		};
		array[9] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo10);
		array[9].IsGetNullable = false;
		array[9].IsSetNullable = false;
		JsonPropertyInfoValues<SerializableRunRngSet> propertyInfo11 = new JsonPropertyInfoValues<SerializableRunRngSet>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializableRun),
			Converter = null,
			Getter = (object obj) => ((SerializableRun)obj).SerializableRng,
			Setter = (object obj, SerializableRunRngSet? value) =>
			{
				((SerializableRun)obj).SerializableRng = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "SerializableRng",
			JsonPropertyName = "rng",
			AttributeProviderFactory = () => typeof(SerializableRun).GetProperty("SerializableRng", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(SerializableRunRngSet), Array.Empty<Type>(), null)
		};
		array[10] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo11);
		array[10].IsGetNullable = false;
		array[10].IsSetNullable = false;
		JsonPropertyInfoValues<List<MapCoord>> propertyInfo12 = new JsonPropertyInfoValues<List<MapCoord>>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializableRun),
			Converter = null,
			Getter = (object obj) => ((SerializableRun)obj).VisitedMapCoords,
			Setter = (object obj, List<MapCoord>? value) =>
			{
				((SerializableRun)obj).VisitedMapCoords = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "VisitedMapCoords",
			JsonPropertyName = "visited_map_coords",
			AttributeProviderFactory = () => typeof(SerializableRun).GetProperty("VisitedMapCoords", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(List<MapCoord>), Array.Empty<Type>(), null)
		};
		array[11] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo12);
		array[11].IsGetNullable = false;
		array[11].IsSetNullable = false;
		JsonPropertyInfoValues<List<List<MapPointHistoryEntry>>> propertyInfo13 = new JsonPropertyInfoValues<List<List<MapPointHistoryEntry>>>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializableRun),
			Converter = null,
			Getter = (object obj) => ((SerializableRun)obj).MapPointHistory,
			Setter = (object obj, List<List<MapPointHistoryEntry>>? value) =>
			{
				((SerializableRun)obj).MapPointHistory = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "MapPointHistory",
			JsonPropertyName = "map_point_history",
			AttributeProviderFactory = () => typeof(SerializableRun).GetProperty("MapPointHistory", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(List<List<MapPointHistoryEntry>>), Array.Empty<Type>(), null)
		};
		array[12] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo13);
		array[12].IsGetNullable = false;
		array[12].IsSetNullable = false;
		JsonPropertyInfoValues<long> propertyInfo14 = new JsonPropertyInfoValues<long>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializableRun),
			Converter = null,
			Getter = (object obj) => ((SerializableRun)obj).SaveTime,
			Setter = (object obj, long value) =>
			{
				((SerializableRun)obj).SaveTime = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "SaveTime",
			JsonPropertyName = "save_time",
			AttributeProviderFactory = () => typeof(SerializableRun).GetProperty("SaveTime", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(long), Array.Empty<Type>(), null)
		};
		array[13] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo14);
		JsonPropertyInfoValues<long> propertyInfo15 = new JsonPropertyInfoValues<long>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializableRun),
			Converter = null,
			Getter = (object obj) => ((SerializableRun)obj).StartTime,
			Setter = (object obj, long value) =>
			{
				((SerializableRun)obj).StartTime = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "StartTime",
			JsonPropertyName = "start_time",
			AttributeProviderFactory = () => typeof(SerializableRun).GetProperty("StartTime", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(long), Array.Empty<Type>(), null)
		};
		array[14] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo15);
		JsonPropertyInfoValues<long> propertyInfo16 = new JsonPropertyInfoValues<long>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializableRun),
			Converter = null,
			Getter = (object obj) => ((SerializableRun)obj).RunTime,
			Setter = (object obj, long value) =>
			{
				((SerializableRun)obj).RunTime = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "RunTime",
			JsonPropertyName = "run_time",
			AttributeProviderFactory = () => typeof(SerializableRun).GetProperty("RunTime", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(long), Array.Empty<Type>(), null)
		};
		array[15] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo16);
		JsonPropertyInfoValues<long> propertyInfo17 = new JsonPropertyInfoValues<long>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializableRun),
			Converter = null,
			Getter = (object obj) => ((SerializableRun)obj).WinTime,
			Setter = (object obj, long value) =>
			{
				((SerializableRun)obj).WinTime = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "WinTime",
			JsonPropertyName = "win_time",
			AttributeProviderFactory = () => typeof(SerializableRun).GetProperty("WinTime", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(long), Array.Empty<Type>(), null)
		};
		array[16] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo17);
		JsonPropertyInfoValues<int> propertyInfo18 = new JsonPropertyInfoValues<int>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializableRun),
			Converter = null,
			Getter = (object obj) => ((SerializableRun)obj).Ascension,
			Setter = (object obj, int value) =>
			{
				((SerializableRun)obj).Ascension = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "Ascension",
			JsonPropertyName = "ascension",
			AttributeProviderFactory = () => typeof(SerializableRun).GetProperty("Ascension", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(int), Array.Empty<Type>(), null)
		};
		array[17] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo18);
		JsonPropertyInfoValues<int> propertyInfo19 = new JsonPropertyInfoValues<int>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializableRun),
			Converter = null,
			Getter = (object obj) => ((SerializableRun)obj).NumReloads,
			Setter = (object obj, int value) =>
			{
				((SerializableRun)obj).NumReloads = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "NumReloads",
			JsonPropertyName = "num_reloads",
			AttributeProviderFactory = () => typeof(SerializableRun).GetProperty("NumReloads", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(int), Array.Empty<Type>(), null)
		};
		array[18] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo19);
		JsonPropertyInfoValues<PlatformType> propertyInfo20 = new JsonPropertyInfoValues<PlatformType>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializableRun),
			Converter = null,
			Getter = (object obj) => ((SerializableRun)obj).PlatformType,
			Setter = (object obj, PlatformType value) =>
			{
				((SerializableRun)obj).PlatformType = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "PlatformType",
			JsonPropertyName = "platform_type",
			AttributeProviderFactory = () => typeof(SerializableRun).GetProperty("PlatformType", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(PlatformType), Array.Empty<Type>(), null)
		};
		array[19] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo20);
		JsonPropertyInfoValues<SerializableMapDrawings> propertyInfo21 = new JsonPropertyInfoValues<SerializableMapDrawings>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializableRun),
			Converter = (JsonConverter<SerializableMapDrawings>)ExpandConverter(typeof(SerializableMapDrawings), new SerializableMapDrawingsJsonConverter(), options),
			Getter = (object obj) => ((SerializableRun)obj).MapDrawings,
			Setter = (object obj, SerializableMapDrawings? value) =>
			{
				((SerializableRun)obj).MapDrawings = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "MapDrawings",
			JsonPropertyName = "map_drawings",
			AttributeProviderFactory = () => typeof(SerializableRun).GetProperty("MapDrawings", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(SerializableMapDrawings), Array.Empty<Type>(), null)
		};
		array[20] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo21);
		JsonPropertyInfoValues<SerializableExtraRunFields> propertyInfo22 = new JsonPropertyInfoValues<SerializableExtraRunFields>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializableRun),
			Converter = null,
			Getter = (object obj) => ((SerializableRun)obj).ExtraFields,
			Setter = (object obj, SerializableExtraRunFields? value) =>
			{
				((SerializableRun)obj).ExtraFields = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "ExtraFields",
			JsonPropertyName = "extra_fields",
			AttributeProviderFactory = () => typeof(SerializableRun).GetProperty("ExtraFields", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(SerializableExtraRunFields), Array.Empty<Type>(), null)
		};
		array[21] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo22);
		array[21].IsGetNullable = false;
		array[21].IsSetNullable = false;
		JsonPropertyInfoValues<GameMode> propertyInfo23 = new JsonPropertyInfoValues<GameMode>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializableRun),
			Converter = null,
			Getter = (object obj) => ((SerializableRun)obj).GameMode,
			Setter = (object obj, GameMode value) =>
			{
				((SerializableRun)obj).GameMode = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "GameMode",
			JsonPropertyName = "game_mode",
			AttributeProviderFactory = () => typeof(SerializableRun).GetProperty("GameMode", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(GameMode), Array.Empty<Type>(), null)
		};
		array[22] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo23);
		JsonPropertyInfoValues<int> propertyInfo24 = new JsonPropertyInfoValues<int>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializableRun),
			Converter = null,
			Getter = null,
			Setter = null,
			IgnoreCondition = JsonIgnoreCondition.Always,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "FloorReached",
			JsonPropertyName = null,
			AttributeProviderFactory = () => typeof(SerializableRun).GetProperty("FloorReached", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(int), Array.Empty<Type>(), null)
		};
		array[23] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo24);
		return array;
	}

	private JsonTypeInfo<SerializableUnlockedAchievement> Create_SerializableUnlockedAchievement(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<SerializableUnlockedAchievement> jsonTypeInfo))
		{
			JsonObjectInfoValues<SerializableUnlockedAchievement> objectInfo = new JsonObjectInfoValues<SerializableUnlockedAchievement>
			{
				ObjectCreator = null,
				ObjectWithParameterizedConstructorCreator = (object[] args) => new SerializableUnlockedAchievement
				{
					Achievement = (string)args[0],
					UnlockTime = (long)args[1]
				},
				PropertyMetadataInitializer = (JsonSerializerContext _) => SerializableUnlockedAchievementPropInit(options),
				ConstructorParameterMetadataInitializer = SerializableUnlockedAchievementCtorParamInit,
				ConstructorAttributeProviderFactory = () => typeof(SerializableUnlockedAchievement).GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Array.Empty<Type>(), null),
				SerializeHandler = null
			};
			jsonTypeInfo = JsonMetadataServices.CreateObjectInfo(options, objectInfo);
			jsonTypeInfo.NumberHandling = null;
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private static JsonPropertyInfo[] SerializableUnlockedAchievementPropInit(JsonSerializerOptions options)
	{
		JsonPropertyInfo[] array = new JsonPropertyInfo[2];
		JsonPropertyInfoValues<string> propertyInfo = new JsonPropertyInfoValues<string>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializableUnlockedAchievement),
			Converter = null,
			Getter = (object obj) => ((SerializableUnlockedAchievement)obj).Achievement,
			Setter = (object obj, string? value) =>
			{
				throw new InvalidOperationException("Setting init-only properties is not supported in source generation mode.");
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "Achievement",
			JsonPropertyName = "achievement",
			AttributeProviderFactory = () => typeof(SerializableUnlockedAchievement).GetProperty("Achievement", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(string), Array.Empty<Type>(), null)
		};
		array[0] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo);
		array[0].IsGetNullable = false;
		array[0].IsSetNullable = false;
		JsonPropertyInfoValues<long> propertyInfo2 = new JsonPropertyInfoValues<long>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializableUnlockedAchievement),
			Converter = null,
			Getter = (object obj) => ((SerializableUnlockedAchievement)obj).UnlockTime,
			Setter = (object obj, long value) =>
			{
				throw new InvalidOperationException("Setting init-only properties is not supported in source generation mode.");
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "UnlockTime",
			JsonPropertyName = "unlock_time",
			AttributeProviderFactory = () => typeof(SerializableUnlockedAchievement).GetProperty("UnlockTime", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(long), Array.Empty<Type>(), null)
		};
		array[1] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo2);
		return array;
	}

	private static JsonParameterInfoValues[] SerializableUnlockedAchievementCtorParamInit()
	{
		return new JsonParameterInfoValues[2]
		{
			new JsonParameterInfoValues
			{
				Name = "Achievement",
				ParameterType = typeof(string),
				Position = 0,
				IsNullable = false,
				IsMemberInitializer = true
			},
			new JsonParameterInfoValues
			{
				Name = "UnlockTime",
				ParameterType = typeof(long),
				Position = 1,
				IsNullable = false,
				IsMemberInitializer = true
			}
		};
	}

	private JsonTypeInfo<SettingsSave> Create_SettingsSave(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<SettingsSave> jsonTypeInfo))
		{
			JsonObjectInfoValues<SettingsSave> objectInfo = new JsonObjectInfoValues<SettingsSave>
			{
				ObjectCreator = () => new SettingsSave(),
				ObjectWithParameterizedConstructorCreator = null,
				PropertyMetadataInitializer = (JsonSerializerContext _) => SettingsSavePropInit(options),
				ConstructorParameterMetadataInitializer = null,
				ConstructorAttributeProviderFactory = () => typeof(SettingsSave).GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Array.Empty<Type>(), null),
				SerializeHandler = null
			};
			jsonTypeInfo = JsonMetadataServices.CreateObjectInfo(options, objectInfo);
			jsonTypeInfo.NumberHandling = null;
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private static JsonPropertyInfo[] SettingsSavePropInit(JsonSerializerOptions options)
	{
		JsonPropertyInfo[] array = new JsonPropertyInfo[24];
		JsonPropertyInfoValues<int> propertyInfo = new JsonPropertyInfoValues<int>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SettingsSave),
			Converter = null,
			Getter = (object obj) => ((SettingsSave)obj).SchemaVersion,
			Setter = (object obj, int value) =>
			{
				((SettingsSave)obj).SchemaVersion = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "SchemaVersion",
			JsonPropertyName = "schema_version",
			AttributeProviderFactory = () => typeof(SettingsSave).GetProperty("SchemaVersion", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(int), Array.Empty<Type>(), null)
		};
		array[0] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo);
		JsonPropertyInfoValues<int> propertyInfo2 = new JsonPropertyInfoValues<int>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SettingsSave),
			Converter = null,
			Getter = (object obj) => ((SettingsSave)obj).FpsLimit,
			Setter = (object obj, int value) =>
			{
				((SettingsSave)obj).FpsLimit = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "FpsLimit",
			JsonPropertyName = "fps_limit",
			AttributeProviderFactory = () => typeof(SettingsSave).GetProperty("FpsLimit", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(int), Array.Empty<Type>(), null)
		};
		array[1] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo2);
		JsonPropertyInfoValues<string> propertyInfo3 = new JsonPropertyInfoValues<string>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SettingsSave),
			Converter = null,
			Getter = (object obj) => ((SettingsSave)obj).Language,
			Setter = (object obj, string? value) =>
			{
				((SettingsSave)obj).Language = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "Language",
			JsonPropertyName = "language",
			AttributeProviderFactory = () => typeof(SettingsSave).GetProperty("Language", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(string), Array.Empty<Type>(), null)
		};
		array[2] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo3);
		JsonPropertyInfoValues<Vector2I> propertyInfo4 = new JsonPropertyInfoValues<Vector2I>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SettingsSave),
			Converter = null,
			Getter = (object obj) => ((SettingsSave)obj).WindowPosition,
			Setter = (object obj, Vector2I value) =>
			{
				((SettingsSave)obj).WindowPosition = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "WindowPosition",
			JsonPropertyName = "window_position",
			AttributeProviderFactory = () => typeof(SettingsSave).GetProperty("WindowPosition", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(Vector2I), Array.Empty<Type>(), null)
		};
		array[3] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo4);
		JsonPropertyInfoValues<Vector2I> propertyInfo5 = new JsonPropertyInfoValues<Vector2I>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SettingsSave),
			Converter = null,
			Getter = (object obj) => ((SettingsSave)obj).WindowSize,
			Setter = (object obj, Vector2I value) =>
			{
				((SettingsSave)obj).WindowSize = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "WindowSize",
			JsonPropertyName = "window_size",
			AttributeProviderFactory = () => typeof(SettingsSave).GetProperty("WindowSize", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(Vector2I), Array.Empty<Type>(), null)
		};
		array[4] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo5);
		JsonPropertyInfoValues<bool> propertyInfo6 = new JsonPropertyInfoValues<bool>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SettingsSave),
			Converter = null,
			Getter = (object obj) => ((SettingsSave)obj).Fullscreen,
			Setter = (object obj, bool value) =>
			{
				((SettingsSave)obj).Fullscreen = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "Fullscreen",
			JsonPropertyName = "fullscreen",
			AttributeProviderFactory = () => typeof(SettingsSave).GetProperty("Fullscreen", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(bool), Array.Empty<Type>(), null)
		};
		array[5] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo6);
		JsonPropertyInfoValues<AspectRatioSetting> propertyInfo7 = new JsonPropertyInfoValues<AspectRatioSetting>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SettingsSave),
			Converter = null,
			Getter = (object obj) => ((SettingsSave)obj).AspectRatioSetting,
			Setter = (object obj, AspectRatioSetting value) =>
			{
				((SettingsSave)obj).AspectRatioSetting = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "AspectRatioSetting",
			JsonPropertyName = "aspect_ratio",
			AttributeProviderFactory = () => typeof(SettingsSave).GetProperty("AspectRatioSetting", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(AspectRatioSetting), Array.Empty<Type>(), null)
		};
		array[6] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo7);
		JsonPropertyInfoValues<int> propertyInfo8 = new JsonPropertyInfoValues<int>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SettingsSave),
			Converter = null,
			Getter = (object obj) => ((SettingsSave)obj).TargetDisplay,
			Setter = (object obj, int value) =>
			{
				((SettingsSave)obj).TargetDisplay = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "TargetDisplay",
			JsonPropertyName = "target_display",
			AttributeProviderFactory = () => typeof(SettingsSave).GetProperty("TargetDisplay", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(int), Array.Empty<Type>(), null)
		};
		array[7] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo8);
		JsonPropertyInfoValues<bool> propertyInfo9 = new JsonPropertyInfoValues<bool>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SettingsSave),
			Converter = null,
			Getter = (object obj) => ((SettingsSave)obj).ResizeWindows,
			Setter = (object obj, bool value) =>
			{
				((SettingsSave)obj).ResizeWindows = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "ResizeWindows",
			JsonPropertyName = "resize_windows",
			AttributeProviderFactory = () => typeof(SettingsSave).GetProperty("ResizeWindows", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(bool), Array.Empty<Type>(), null)
		};
		array[8] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo9);
		JsonPropertyInfoValues<VSyncType> propertyInfo10 = new JsonPropertyInfoValues<VSyncType>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SettingsSave),
			Converter = null,
			Getter = (object obj) => ((SettingsSave)obj).VSync,
			Setter = (object obj, VSyncType value) =>
			{
				((SettingsSave)obj).VSync = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "VSync",
			JsonPropertyName = "vsync",
			AttributeProviderFactory = () => typeof(SettingsSave).GetProperty("VSync", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(VSyncType), Array.Empty<Type>(), null)
		};
		array[9] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo10);
		JsonPropertyInfoValues<int> propertyInfo11 = new JsonPropertyInfoValues<int>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SettingsSave),
			Converter = null,
			Getter = (object obj) => ((SettingsSave)obj).Msaa,
			Setter = (object obj, int value) =>
			{
				((SettingsSave)obj).Msaa = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "Msaa",
			JsonPropertyName = "msaa",
			AttributeProviderFactory = () => typeof(SettingsSave).GetProperty("Msaa", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(int), Array.Empty<Type>(), null)
		};
		array[10] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo11);
		JsonPropertyInfoValues<float> propertyInfo12 = new JsonPropertyInfoValues<float>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SettingsSave),
			Converter = null,
			Getter = (object obj) => ((SettingsSave)obj).VolumeBgm,
			Setter = (object obj, float value) =>
			{
				((SettingsSave)obj).VolumeBgm = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "VolumeBgm",
			JsonPropertyName = "volume_bgm",
			AttributeProviderFactory = () => typeof(SettingsSave).GetProperty("VolumeBgm", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(float), Array.Empty<Type>(), null)
		};
		array[11] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo12);
		JsonPropertyInfoValues<float> propertyInfo13 = new JsonPropertyInfoValues<float>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SettingsSave),
			Converter = null,
			Getter = (object obj) => ((SettingsSave)obj).VolumeMaster,
			Setter = (object obj, float value) =>
			{
				((SettingsSave)obj).VolumeMaster = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "VolumeMaster",
			JsonPropertyName = "volume_master",
			AttributeProviderFactory = () => typeof(SettingsSave).GetProperty("VolumeMaster", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(float), Array.Empty<Type>(), null)
		};
		array[12] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo13);
		JsonPropertyInfoValues<float> propertyInfo14 = new JsonPropertyInfoValues<float>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SettingsSave),
			Converter = null,
			Getter = (object obj) => ((SettingsSave)obj).VolumeSfx,
			Setter = (object obj, float value) =>
			{
				((SettingsSave)obj).VolumeSfx = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "VolumeSfx",
			JsonPropertyName = "volume_sfx",
			AttributeProviderFactory = () => typeof(SettingsSave).GetProperty("VolumeSfx", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(float), Array.Empty<Type>(), null)
		};
		array[13] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo14);
		JsonPropertyInfoValues<float> propertyInfo15 = new JsonPropertyInfoValues<float>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SettingsSave),
			Converter = null,
			Getter = (object obj) => ((SettingsSave)obj).VolumeAmbience,
			Setter = (object obj, float value) =>
			{
				((SettingsSave)obj).VolumeAmbience = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "VolumeAmbience",
			JsonPropertyName = "volume_ambience",
			AttributeProviderFactory = () => typeof(SettingsSave).GetProperty("VolumeAmbience", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(float), Array.Empty<Type>(), null)
		};
		array[14] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo15);
		JsonPropertyInfoValues<ModSettings> propertyInfo16 = new JsonPropertyInfoValues<ModSettings>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SettingsSave),
			Converter = null,
			Getter = (object obj) => ((SettingsSave)obj).ModSettings,
			Setter = (object obj, ModSettings? value) =>
			{
				((SettingsSave)obj).ModSettings = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "ModSettings",
			JsonPropertyName = "mod_settings",
			AttributeProviderFactory = () => typeof(SettingsSave).GetProperty("ModSettings", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(ModSettings), Array.Empty<Type>(), null)
		};
		array[15] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo16);
		JsonPropertyInfoValues<bool> propertyInfo17 = new JsonPropertyInfoValues<bool>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SettingsSave),
			Converter = null,
			Getter = (object obj) => ((SettingsSave)obj).SkipIntroLogo,
			Setter = (object obj, bool value) =>
			{
				((SettingsSave)obj).SkipIntroLogo = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "SkipIntroLogo",
			JsonPropertyName = "skip_intro_logo",
			AttributeProviderFactory = () => typeof(SettingsSave).GetProperty("SkipIntroLogo", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(bool), Array.Empty<Type>(), null)
		};
		array[16] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo17);
		JsonPropertyInfoValues<Dictionary<string, string>> propertyInfo18 = new JsonPropertyInfoValues<Dictionary<string, string>>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SettingsSave),
			Converter = null,
			Getter = (object obj) => ((SettingsSave)obj).KeyboardMapping,
			Setter = (object obj, Dictionary<string, string>? value) =>
			{
				((SettingsSave)obj).KeyboardMapping = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "KeyboardMapping",
			JsonPropertyName = "keyboard_mapping",
			AttributeProviderFactory = () => typeof(SettingsSave).GetProperty("KeyboardMapping", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(Dictionary<string, string>), Array.Empty<Type>(), null)
		};
		array[17] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo18);
		array[17].IsGetNullable = false;
		array[17].IsSetNullable = false;
		JsonPropertyInfoValues<Dictionary<string, string>> propertyInfo19 = new JsonPropertyInfoValues<Dictionary<string, string>>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SettingsSave),
			Converter = null,
			Getter = (object obj) => ((SettingsSave)obj).KbOnlyMapping,
			Setter = (object obj, Dictionary<string, string>? value) =>
			{
				((SettingsSave)obj).KbOnlyMapping = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "KbOnlyMapping",
			JsonPropertyName = "keyboard_only_mapping",
			AttributeProviderFactory = () => typeof(SettingsSave).GetProperty("KbOnlyMapping", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(Dictionary<string, string>), Array.Empty<Type>(), null)
		};
		array[18] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo19);
		array[18].IsGetNullable = false;
		array[18].IsSetNullable = false;
		JsonPropertyInfoValues<ControllerMappingType> propertyInfo20 = new JsonPropertyInfoValues<ControllerMappingType>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SettingsSave),
			Converter = null,
			Getter = (object obj) => ((SettingsSave)obj).ControllerMappingType,
			Setter = (object obj, ControllerMappingType value) =>
			{
				((SettingsSave)obj).ControllerMappingType = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "ControllerMappingType",
			JsonPropertyName = "controller_mapping_type",
			AttributeProviderFactory = () => typeof(SettingsSave).GetProperty("ControllerMappingType", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(ControllerMappingType), Array.Empty<Type>(), null)
		};
		array[19] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo20);
		JsonPropertyInfoValues<Dictionary<string, string>> propertyInfo21 = new JsonPropertyInfoValues<Dictionary<string, string>>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SettingsSave),
			Converter = null,
			Getter = (object obj) => ((SettingsSave)obj).ControllerMapping,
			Setter = (object obj, Dictionary<string, string>? value) =>
			{
				((SettingsSave)obj).ControllerMapping = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "ControllerMapping",
			JsonPropertyName = "controller_mapping",
			AttributeProviderFactory = () => typeof(SettingsSave).GetProperty("ControllerMapping", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(Dictionary<string, string>), Array.Empty<Type>(), null)
		};
		array[20] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo21);
		array[20].IsGetNullable = false;
		array[20].IsSetNullable = false;
		JsonPropertyInfoValues<bool> propertyInfo22 = new JsonPropertyInfoValues<bool>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SettingsSave),
			Converter = null,
			Getter = (object obj) => ((SettingsSave)obj).LimitFpsInBackground,
			Setter = (object obj, bool value) =>
			{
				((SettingsSave)obj).LimitFpsInBackground = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "LimitFpsInBackground",
			JsonPropertyName = "limit_fps_in_background",
			AttributeProviderFactory = () => typeof(SettingsSave).GetProperty("LimitFpsInBackground", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(bool), Array.Empty<Type>(), null)
		};
		array[21] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo22);
		JsonPropertyInfoValues<bool> propertyInfo23 = new JsonPropertyInfoValues<bool>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SettingsSave),
			Converter = null,
			Getter = (object obj) => ((SettingsSave)obj).FullConsole,
			Setter = (object obj, bool value) =>
			{
				((SettingsSave)obj).FullConsole = value;
			},
			IgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "FullConsole",
			JsonPropertyName = "full_console",
			AttributeProviderFactory = () => typeof(SettingsSave).GetProperty("FullConsole", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(bool), Array.Empty<Type>(), null)
		};
		array[22] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo23);
		JsonPropertyInfoValues<bool> propertyInfo24 = new JsonPropertyInfoValues<bool>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SettingsSave),
			Converter = null,
			Getter = (object obj) => ((SettingsSave)obj).SeenEaDisclaimer,
			Setter = (object obj, bool value) =>
			{
				((SettingsSave)obj).SeenEaDisclaimer = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "SeenEaDisclaimer",
			JsonPropertyName = "seen_ea_disclaimer",
			AttributeProviderFactory = () => typeof(SettingsSave).GetProperty("SeenEaDisclaimer", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(bool), Array.Empty<Type>(), null)
		};
		array[23] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo24);
		return array;
	}

	private JsonTypeInfo<AspectRatioSetting> Create_AspectRatioSetting(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<AspectRatioSetting> jsonTypeInfo))
		{
			jsonTypeInfo = JsonMetadataServices.CreateValueInfo<AspectRatioSetting>(options, JsonMetadataServices.GetEnumConverter<AspectRatioSetting>(options));
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private JsonTypeInfo<FastModeType> Create_FastModeType(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<FastModeType> jsonTypeInfo))
		{
			jsonTypeInfo = JsonMetadataServices.CreateValueInfo<FastModeType>(options, JsonMetadataServices.GetEnumConverter<FastModeType>(options));
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private JsonTypeInfo<VSyncType> Create_VSyncType(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<VSyncType> jsonTypeInfo))
		{
			jsonTypeInfo = JsonMetadataServices.CreateValueInfo<VSyncType>(options, JsonMetadataServices.GetEnumConverter<VSyncType>(options));
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private JsonTypeInfo<SerializableUnlockState> Create_SerializableUnlockState(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<SerializableUnlockState> jsonTypeInfo))
		{
			JsonObjectInfoValues<SerializableUnlockState> objectInfo = new JsonObjectInfoValues<SerializableUnlockState>
			{
				ObjectCreator = () => new SerializableUnlockState(),
				ObjectWithParameterizedConstructorCreator = null,
				PropertyMetadataInitializer = (JsonSerializerContext _) => SerializableUnlockStatePropInit(options),
				ConstructorParameterMetadataInitializer = null,
				ConstructorAttributeProviderFactory = () => typeof(SerializableUnlockState).GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Array.Empty<Type>(), null),
				SerializeHandler = null
			};
			jsonTypeInfo = JsonMetadataServices.CreateObjectInfo(options, objectInfo);
			jsonTypeInfo.NumberHandling = null;
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private static JsonPropertyInfo[] SerializableUnlockStatePropInit(JsonSerializerOptions options)
	{
		JsonPropertyInfo[] array = new JsonPropertyInfo[3];
		JsonPropertyInfoValues<List<string>> propertyInfo = new JsonPropertyInfoValues<List<string>>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializableUnlockState),
			Converter = null,
			Getter = (object obj) => ((SerializableUnlockState)obj).UnlockedEpochs,
			Setter = (object obj, List<string>? value) =>
			{
				((SerializableUnlockState)obj).UnlockedEpochs = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "UnlockedEpochs",
			JsonPropertyName = "unlocked_epochs",
			AttributeProviderFactory = () => typeof(SerializableUnlockState).GetProperty("UnlockedEpochs", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(List<string>), Array.Empty<Type>(), null)
		};
		array[0] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo);
		array[0].IsGetNullable = false;
		array[0].IsSetNullable = false;
		JsonPropertyInfoValues<List<ModelId>> propertyInfo2 = new JsonPropertyInfoValues<List<ModelId>>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializableUnlockState),
			Converter = null,
			Getter = (object obj) => ((SerializableUnlockState)obj).EncountersSeen,
			Setter = (object obj, List<ModelId>? value) =>
			{
				((SerializableUnlockState)obj).EncountersSeen = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "EncountersSeen",
			JsonPropertyName = "encounters_seen",
			AttributeProviderFactory = () => typeof(SerializableUnlockState).GetProperty("EncountersSeen", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(List<ModelId>), Array.Empty<Type>(), null)
		};
		array[1] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo2);
		array[1].IsGetNullable = false;
		array[1].IsSetNullable = false;
		JsonPropertyInfoValues<int> propertyInfo3 = new JsonPropertyInfoValues<int>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SerializableUnlockState),
			Converter = null,
			Getter = (object obj) => ((SerializableUnlockState)obj).NumberOfRuns,
			Setter = (object obj, int value) =>
			{
				((SerializableUnlockState)obj).NumberOfRuns = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "NumberOfRuns",
			JsonPropertyName = "number_of_runs",
			AttributeProviderFactory = () => typeof(SerializableUnlockState).GetProperty("NumberOfRuns", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(int), Array.Empty<Type>(), null)
		};
		array[2] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo3);
		return array;
	}

	private JsonTypeInfo<Dictionary<RelicRarity, List<ModelId>>> Create_DictionaryRelicRarityListModelId(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<Dictionary<RelicRarity, List<ModelId>>> jsonTypeInfo))
		{
			JsonCollectionInfoValues<Dictionary<RelicRarity, List<ModelId>>> collectionInfo = new JsonCollectionInfoValues<Dictionary<RelicRarity, List<ModelId>>>
			{
				ObjectCreator = () => new Dictionary<RelicRarity, List<ModelId>>(),
				SerializeHandler = null
			};
			jsonTypeInfo = JsonMetadataServices.CreateDictionaryInfo<Dictionary<RelicRarity, List<ModelId>>, RelicRarity, List<ModelId>>(options, collectionInfo);
			jsonTypeInfo.NumberHandling = null;
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private JsonTypeInfo<Dictionary<PlayerRngType, SerializableRng>> Create_DictionaryPlayerRngTypeSerializableRng(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<Dictionary<PlayerRngType, SerializableRng>> jsonTypeInfo))
		{
			JsonCollectionInfoValues<Dictionary<PlayerRngType, SerializableRng>> collectionInfo = new JsonCollectionInfoValues<Dictionary<PlayerRngType, SerializableRng>>
			{
				ObjectCreator = () => new Dictionary<PlayerRngType, SerializableRng>(),
				SerializeHandler = null
			};
			jsonTypeInfo = JsonMetadataServices.CreateDictionaryInfo<Dictionary<PlayerRngType, SerializableRng>, PlayerRngType, SerializableRng>(options, collectionInfo);
			jsonTypeInfo.NumberHandling = null;
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private JsonTypeInfo<Dictionary<RunRngType, SerializableRng>> Create_DictionaryRunRngTypeSerializableRng(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<Dictionary<RunRngType, SerializableRng>> jsonTypeInfo))
		{
			JsonCollectionInfoValues<Dictionary<RunRngType, SerializableRng>> collectionInfo = new JsonCollectionInfoValues<Dictionary<RunRngType, SerializableRng>>
			{
				ObjectCreator = () => new Dictionary<RunRngType, SerializableRng>(),
				SerializeHandler = null
			};
			jsonTypeInfo = JsonMetadataServices.CreateDictionaryInfo<Dictionary<RunRngType, SerializableRng>, RunRngType, SerializableRng>(options, collectionInfo);
			jsonTypeInfo.NumberHandling = null;
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private JsonTypeInfo<Dictionary<string, object>> Create_DictionaryStringObject(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<Dictionary<string, object>> jsonTypeInfo))
		{
			JsonCollectionInfoValues<Dictionary<string, object>> collectionInfo = new JsonCollectionInfoValues<Dictionary<string, object>>
			{
				ObjectCreator = () => new Dictionary<string, object>(),
				SerializeHandler = null
			};
			jsonTypeInfo = JsonMetadataServices.CreateDictionaryInfo<Dictionary<string, object>, string, object>(options, collectionInfo);
			jsonTypeInfo.NumberHandling = null;
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private JsonTypeInfo<Dictionary<string, string>> Create_DictionaryStringString(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<Dictionary<string, string>> jsonTypeInfo))
		{
			JsonCollectionInfoValues<Dictionary<string, string>> collectionInfo = new JsonCollectionInfoValues<Dictionary<string, string>>
			{
				ObjectCreator = () => new Dictionary<string, string>(),
				SerializeHandler = null
			};
			jsonTypeInfo = JsonMetadataServices.CreateDictionaryInfo<Dictionary<string, string>, string, string>(options, collectionInfo);
			jsonTypeInfo.NumberHandling = null;
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private JsonTypeInfo<Dictionary<ulong, List<SerializableReward>>> Create_DictionaryUInt64ListSerializableReward(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<Dictionary<ulong, List<SerializableReward>>> jsonTypeInfo))
		{
			JsonCollectionInfoValues<Dictionary<ulong, List<SerializableReward>>> collectionInfo = new JsonCollectionInfoValues<Dictionary<ulong, List<SerializableReward>>>
			{
				ObjectCreator = () => new Dictionary<ulong, List<SerializableReward>>(),
				SerializeHandler = null
			};
			jsonTypeInfo = JsonMetadataServices.CreateDictionaryInfo<Dictionary<ulong, List<SerializableReward>>, ulong, List<SerializableReward>>(options, collectionInfo);
			jsonTypeInfo.NumberHandling = null;
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private JsonTypeInfo<IEnumerable<SerializableBadge>> Create_IEnumerableSerializableBadge(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<IEnumerable<SerializableBadge>> jsonTypeInfo))
		{
			JsonCollectionInfoValues<IEnumerable<SerializableBadge>> collectionInfo = new JsonCollectionInfoValues<IEnumerable<SerializableBadge>>
			{
				ObjectCreator = null,
				SerializeHandler = null
			};
			jsonTypeInfo = JsonMetadataServices.CreateIEnumerableInfo<IEnumerable<SerializableBadge>, SerializableBadge>(options, collectionInfo);
			jsonTypeInfo.NumberHandling = null;
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private JsonTypeInfo<IEnumerable<SerializableCard>> Create_IEnumerableSerializableCard(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<IEnumerable<SerializableCard>> jsonTypeInfo))
		{
			JsonCollectionInfoValues<IEnumerable<SerializableCard>> collectionInfo = new JsonCollectionInfoValues<IEnumerable<SerializableCard>>
			{
				ObjectCreator = null,
				SerializeHandler = null
			};
			jsonTypeInfo = JsonMetadataServices.CreateIEnumerableInfo<IEnumerable<SerializableCard>, SerializableCard>(options, collectionInfo);
			jsonTypeInfo.NumberHandling = null;
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private JsonTypeInfo<IEnumerable<SerializablePotion>> Create_IEnumerableSerializablePotion(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<IEnumerable<SerializablePotion>> jsonTypeInfo))
		{
			JsonCollectionInfoValues<IEnumerable<SerializablePotion>> collectionInfo = new JsonCollectionInfoValues<IEnumerable<SerializablePotion>>
			{
				ObjectCreator = null,
				SerializeHandler = null
			};
			jsonTypeInfo = JsonMetadataServices.CreateIEnumerableInfo<IEnumerable<SerializablePotion>, SerializablePotion>(options, collectionInfo);
			jsonTypeInfo.NumberHandling = null;
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private JsonTypeInfo<IEnumerable<SerializableRelic>> Create_IEnumerableSerializableRelic(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<IEnumerable<SerializableRelic>> jsonTypeInfo))
		{
			JsonCollectionInfoValues<IEnumerable<SerializableRelic>> collectionInfo = new JsonCollectionInfoValues<IEnumerable<SerializableRelic>>
			{
				ObjectCreator = null,
				SerializeHandler = null
			};
			jsonTypeInfo = JsonMetadataServices.CreateIEnumerableInfo<IEnumerable<SerializableRelic>, SerializableRelic>(options, collectionInfo);
			jsonTypeInfo.NumberHandling = null;
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private JsonTypeInfo<List<Vector2>> Create_ListVector2(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<List<Vector2>> jsonTypeInfo))
		{
			JsonCollectionInfoValues<List<Vector2>> collectionInfo = new JsonCollectionInfoValues<List<Vector2>>
			{
				ObjectCreator = () => new List<Vector2>(),
				SerializeHandler = null
			};
			jsonTypeInfo = JsonMetadataServices.CreateListInfo<List<Vector2>, Vector2>(options, collectionInfo);
			jsonTypeInfo.NumberHandling = null;
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private JsonTypeInfo<List<MapCoord>> Create_ListMapCoord(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<List<MapCoord>> jsonTypeInfo))
		{
			JsonCollectionInfoValues<List<MapCoord>> collectionInfo = new JsonCollectionInfoValues<List<MapCoord>>
			{
				ObjectCreator = () => new List<MapCoord>(),
				SerializeHandler = null
			};
			jsonTypeInfo = JsonMetadataServices.CreateListInfo<List<MapCoord>, MapCoord>(options, collectionInfo);
			jsonTypeInfo.NumberHandling = null;
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private JsonTypeInfo<List<ModDependency>> Create_ListModDependency(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<List<ModDependency>> jsonTypeInfo))
		{
			JsonCollectionInfoValues<List<ModDependency>> collectionInfo = new JsonCollectionInfoValues<List<ModDependency>>
			{
				ObjectCreator = () => new List<ModDependency>(),
				SerializeHandler = null
			};
			jsonTypeInfo = JsonMetadataServices.CreateListInfo<List<ModDependency>, ModDependency>(options, collectionInfo);
			jsonTypeInfo.NumberHandling = null;
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private JsonTypeInfo<List<SettingsSaveMod>> Create_ListSettingsSaveMod(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<List<SettingsSaveMod>> jsonTypeInfo))
		{
			JsonCollectionInfoValues<List<SettingsSaveMod>> collectionInfo = new JsonCollectionInfoValues<List<SettingsSaveMod>>
			{
				ObjectCreator = () => new List<SettingsSaveMod>(),
				SerializeHandler = null
			};
			jsonTypeInfo = JsonMetadataServices.CreateListInfo<List<SettingsSaveMod>, SettingsSaveMod>(options, collectionInfo);
			jsonTypeInfo.NumberHandling = null;
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private JsonTypeInfo<List<ModelId>> Create_ListModelId(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<List<ModelId>> jsonTypeInfo))
		{
			JsonCollectionInfoValues<List<ModelId>> collectionInfo = new JsonCollectionInfoValues<List<ModelId>>
			{
				ObjectCreator = () => new List<ModelId>(),
				SerializeHandler = null
			};
			jsonTypeInfo = JsonMetadataServices.CreateListInfo<List<ModelId>, ModelId>(options, collectionInfo);
			jsonTypeInfo.NumberHandling = null;
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private JsonTypeInfo<List<NullLeaderboard>> Create_ListNullLeaderboard(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<List<NullLeaderboard>> jsonTypeInfo))
		{
			JsonCollectionInfoValues<List<NullLeaderboard>> collectionInfo = new JsonCollectionInfoValues<List<NullLeaderboard>>
			{
				ObjectCreator = () => new List<NullLeaderboard>(),
				SerializeHandler = null
			};
			jsonTypeInfo = JsonMetadataServices.CreateListInfo<List<NullLeaderboard>, NullLeaderboard>(options, collectionInfo);
			jsonTypeInfo.NumberHandling = null;
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private JsonTypeInfo<List<NullLeaderboardFileEntry>> Create_ListNullLeaderboardFileEntry(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<List<NullLeaderboardFileEntry>> jsonTypeInfo))
		{
			JsonCollectionInfoValues<List<NullLeaderboardFileEntry>> collectionInfo = new JsonCollectionInfoValues<List<NullLeaderboardFileEntry>>
			{
				ObjectCreator = () => new List<NullLeaderboardFileEntry>(),
				SerializeHandler = null
			};
			jsonTypeInfo = JsonMetadataServices.CreateListInfo<List<NullLeaderboardFileEntry>, NullLeaderboardFileEntry>(options, collectionInfo);
			jsonTypeInfo.NumberHandling = null;
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private JsonTypeInfo<List<NullMultiplayerName>> Create_ListNullMultiplayerName(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<List<NullMultiplayerName>> jsonTypeInfo))
		{
			JsonCollectionInfoValues<List<NullMultiplayerName>> collectionInfo = new JsonCollectionInfoValues<List<NullMultiplayerName>>
			{
				ObjectCreator = () => new List<NullMultiplayerName>(),
				SerializeHandler = null
			};
			jsonTypeInfo = JsonMetadataServices.CreateListInfo<List<NullMultiplayerName>, NullMultiplayerName>(options, collectionInfo);
			jsonTypeInfo.NumberHandling = null;
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private JsonTypeInfo<List<AncientChoiceHistoryEntry>> Create_ListAncientChoiceHistoryEntry(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<List<AncientChoiceHistoryEntry>> jsonTypeInfo))
		{
			JsonCollectionInfoValues<List<AncientChoiceHistoryEntry>> collectionInfo = new JsonCollectionInfoValues<List<AncientChoiceHistoryEntry>>
			{
				ObjectCreator = () => new List<AncientChoiceHistoryEntry>(),
				SerializeHandler = null
			};
			jsonTypeInfo = JsonMetadataServices.CreateListInfo<List<AncientChoiceHistoryEntry>, AncientChoiceHistoryEntry>(options, collectionInfo);
			jsonTypeInfo.NumberHandling = null;
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private JsonTypeInfo<List<CardChoiceHistoryEntry>> Create_ListCardChoiceHistoryEntry(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<List<CardChoiceHistoryEntry>> jsonTypeInfo))
		{
			JsonCollectionInfoValues<List<CardChoiceHistoryEntry>> collectionInfo = new JsonCollectionInfoValues<List<CardChoiceHistoryEntry>>
			{
				ObjectCreator = () => new List<CardChoiceHistoryEntry>(),
				SerializeHandler = null
			};
			jsonTypeInfo = JsonMetadataServices.CreateListInfo<List<CardChoiceHistoryEntry>, CardChoiceHistoryEntry>(options, collectionInfo);
			jsonTypeInfo.NumberHandling = null;
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private JsonTypeInfo<List<CardEnchantmentHistoryEntry>> Create_ListCardEnchantmentHistoryEntry(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<List<CardEnchantmentHistoryEntry>> jsonTypeInfo))
		{
			JsonCollectionInfoValues<List<CardEnchantmentHistoryEntry>> collectionInfo = new JsonCollectionInfoValues<List<CardEnchantmentHistoryEntry>>
			{
				ObjectCreator = () => new List<CardEnchantmentHistoryEntry>(),
				SerializeHandler = null
			};
			jsonTypeInfo = JsonMetadataServices.CreateListInfo<List<CardEnchantmentHistoryEntry>, CardEnchantmentHistoryEntry>(options, collectionInfo);
			jsonTypeInfo.NumberHandling = null;
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private JsonTypeInfo<List<CardTransformationHistoryEntry>> Create_ListCardTransformationHistoryEntry(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<List<CardTransformationHistoryEntry>> jsonTypeInfo))
		{
			JsonCollectionInfoValues<List<CardTransformationHistoryEntry>> collectionInfo = new JsonCollectionInfoValues<List<CardTransformationHistoryEntry>>
			{
				ObjectCreator = () => new List<CardTransformationHistoryEntry>(),
				SerializeHandler = null
			};
			jsonTypeInfo = JsonMetadataServices.CreateListInfo<List<CardTransformationHistoryEntry>, CardTransformationHistoryEntry>(options, collectionInfo);
			jsonTypeInfo.NumberHandling = null;
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private JsonTypeInfo<List<EventOptionHistoryEntry>> Create_ListEventOptionHistoryEntry(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<List<EventOptionHistoryEntry>> jsonTypeInfo))
		{
			JsonCollectionInfoValues<List<EventOptionHistoryEntry>> collectionInfo = new JsonCollectionInfoValues<List<EventOptionHistoryEntry>>
			{
				ObjectCreator = () => new List<EventOptionHistoryEntry>(),
				SerializeHandler = null
			};
			jsonTypeInfo = JsonMetadataServices.CreateListInfo<List<EventOptionHistoryEntry>, EventOptionHistoryEntry>(options, collectionInfo);
			jsonTypeInfo.NumberHandling = null;
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private JsonTypeInfo<List<MapPointHistoryEntry>> Create_ListMapPointHistoryEntry(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<List<MapPointHistoryEntry>> jsonTypeInfo))
		{
			JsonCollectionInfoValues<List<MapPointHistoryEntry>> collectionInfo = new JsonCollectionInfoValues<List<MapPointHistoryEntry>>
			{
				ObjectCreator = () => new List<MapPointHistoryEntry>(),
				SerializeHandler = null
			};
			jsonTypeInfo = JsonMetadataServices.CreateListInfo<List<MapPointHistoryEntry>, MapPointHistoryEntry>(options, collectionInfo);
			jsonTypeInfo.NumberHandling = null;
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private JsonTypeInfo<List<MapPointRoomHistoryEntry>> Create_ListMapPointRoomHistoryEntry(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<List<MapPointRoomHistoryEntry>> jsonTypeInfo))
		{
			JsonCollectionInfoValues<List<MapPointRoomHistoryEntry>> collectionInfo = new JsonCollectionInfoValues<List<MapPointRoomHistoryEntry>>
			{
				ObjectCreator = () => new List<MapPointRoomHistoryEntry>(),
				SerializeHandler = null
			};
			jsonTypeInfo = JsonMetadataServices.CreateListInfo<List<MapPointRoomHistoryEntry>, MapPointRoomHistoryEntry>(options, collectionInfo);
			jsonTypeInfo.NumberHandling = null;
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private JsonTypeInfo<List<ModelChoiceHistoryEntry>> Create_ListModelChoiceHistoryEntry(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<List<ModelChoiceHistoryEntry>> jsonTypeInfo))
		{
			JsonCollectionInfoValues<List<ModelChoiceHistoryEntry>> collectionInfo = new JsonCollectionInfoValues<List<ModelChoiceHistoryEntry>>
			{
				ObjectCreator = () => new List<ModelChoiceHistoryEntry>(),
				SerializeHandler = null
			};
			jsonTypeInfo = JsonMetadataServices.CreateListInfo<List<ModelChoiceHistoryEntry>, ModelChoiceHistoryEntry>(options, collectionInfo);
			jsonTypeInfo.NumberHandling = null;
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private JsonTypeInfo<List<PlayerMapPointHistoryEntry>> Create_ListPlayerMapPointHistoryEntry(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<List<PlayerMapPointHistoryEntry>> jsonTypeInfo))
		{
			JsonCollectionInfoValues<List<PlayerMapPointHistoryEntry>> collectionInfo = new JsonCollectionInfoValues<List<PlayerMapPointHistoryEntry>>
			{
				ObjectCreator = () => new List<PlayerMapPointHistoryEntry>(),
				SerializeHandler = null
			};
			jsonTypeInfo = JsonMetadataServices.CreateListInfo<List<PlayerMapPointHistoryEntry>, PlayerMapPointHistoryEntry>(options, collectionInfo);
			jsonTypeInfo.NumberHandling = null;
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private JsonTypeInfo<List<RunHistoryPlayer>> Create_ListRunHistoryPlayer(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<List<RunHistoryPlayer>> jsonTypeInfo))
		{
			JsonCollectionInfoValues<List<RunHistoryPlayer>> collectionInfo = new JsonCollectionInfoValues<List<RunHistoryPlayer>>
			{
				ObjectCreator = () => new List<RunHistoryPlayer>(),
				SerializeHandler = null
			};
			jsonTypeInfo = JsonMetadataServices.CreateListInfo<List<RunHistoryPlayer>, RunHistoryPlayer>(options, collectionInfo);
			jsonTypeInfo.NumberHandling = null;
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private JsonTypeInfo<List<AncientCharacterStats>> Create_ListAncientCharacterStats(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<List<AncientCharacterStats>> jsonTypeInfo))
		{
			JsonCollectionInfoValues<List<AncientCharacterStats>> collectionInfo = new JsonCollectionInfoValues<List<AncientCharacterStats>>
			{
				ObjectCreator = () => new List<AncientCharacterStats>(),
				SerializeHandler = null
			};
			jsonTypeInfo = JsonMetadataServices.CreateListInfo<List<AncientCharacterStats>, AncientCharacterStats>(options, collectionInfo);
			jsonTypeInfo.NumberHandling = null;
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private JsonTypeInfo<List<AncientStats>> Create_ListAncientStats(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<List<AncientStats>> jsonTypeInfo))
		{
			JsonCollectionInfoValues<List<AncientStats>> collectionInfo = new JsonCollectionInfoValues<List<AncientStats>>
			{
				ObjectCreator = () => new List<AncientStats>(),
				SerializeHandler = null
			};
			jsonTypeInfo = JsonMetadataServices.CreateListInfo<List<AncientStats>, AncientStats>(options, collectionInfo);
			jsonTypeInfo.NumberHandling = null;
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private JsonTypeInfo<List<BadgeStats>> Create_ListBadgeStats(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<List<BadgeStats>> jsonTypeInfo))
		{
			JsonCollectionInfoValues<List<BadgeStats>> collectionInfo = new JsonCollectionInfoValues<List<BadgeStats>>
			{
				ObjectCreator = () => new List<BadgeStats>(),
				SerializeHandler = null
			};
			jsonTypeInfo = JsonMetadataServices.CreateListInfo<List<BadgeStats>, BadgeStats>(options, collectionInfo);
			jsonTypeInfo.NumberHandling = null;
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private JsonTypeInfo<List<CardStats>> Create_ListCardStats(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<List<CardStats>> jsonTypeInfo))
		{
			JsonCollectionInfoValues<List<CardStats>> collectionInfo = new JsonCollectionInfoValues<List<CardStats>>
			{
				ObjectCreator = () => new List<CardStats>(),
				SerializeHandler = null
			};
			jsonTypeInfo = JsonMetadataServices.CreateListInfo<List<CardStats>, CardStats>(options, collectionInfo);
			jsonTypeInfo.NumberHandling = null;
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private JsonTypeInfo<List<CharacterStats>> Create_ListCharacterStats(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<List<CharacterStats>> jsonTypeInfo))
		{
			JsonCollectionInfoValues<List<CharacterStats>> collectionInfo = new JsonCollectionInfoValues<List<CharacterStats>>
			{
				ObjectCreator = () => new List<CharacterStats>(),
				SerializeHandler = null
			};
			jsonTypeInfo = JsonMetadataServices.CreateListInfo<List<CharacterStats>, CharacterStats>(options, collectionInfo);
			jsonTypeInfo.NumberHandling = null;
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private JsonTypeInfo<List<EncounterStats>> Create_ListEncounterStats(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<List<EncounterStats>> jsonTypeInfo))
		{
			JsonCollectionInfoValues<List<EncounterStats>> collectionInfo = new JsonCollectionInfoValues<List<EncounterStats>>
			{
				ObjectCreator = () => new List<EncounterStats>(),
				SerializeHandler = null
			};
			jsonTypeInfo = JsonMetadataServices.CreateListInfo<List<EncounterStats>, EncounterStats>(options, collectionInfo);
			jsonTypeInfo.NumberHandling = null;
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private JsonTypeInfo<List<EnemyStats>> Create_ListEnemyStats(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<List<EnemyStats>> jsonTypeInfo))
		{
			JsonCollectionInfoValues<List<EnemyStats>> collectionInfo = new JsonCollectionInfoValues<List<EnemyStats>>
			{
				ObjectCreator = () => new List<EnemyStats>(),
				SerializeHandler = null
			};
			jsonTypeInfo = JsonMetadataServices.CreateListInfo<List<EnemyStats>, EnemyStats>(options, collectionInfo);
			jsonTypeInfo.NumberHandling = null;
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private JsonTypeInfo<List<FightStats>> Create_ListFightStats(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<List<FightStats>> jsonTypeInfo))
		{
			JsonCollectionInfoValues<List<FightStats>> collectionInfo = new JsonCollectionInfoValues<List<FightStats>>
			{
				ObjectCreator = () => new List<FightStats>(),
				SerializeHandler = null
			};
			jsonTypeInfo = JsonMetadataServices.CreateListInfo<List<FightStats>, FightStats>(options, collectionInfo);
			jsonTypeInfo.NumberHandling = null;
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private JsonTypeInfo<List<SerializableMapDrawingLine>> Create_ListSerializableMapDrawingLine(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<List<SerializableMapDrawingLine>> jsonTypeInfo))
		{
			JsonCollectionInfoValues<List<SerializableMapDrawingLine>> collectionInfo = new JsonCollectionInfoValues<List<SerializableMapDrawingLine>>
			{
				ObjectCreator = () => new List<SerializableMapDrawingLine>(),
				SerializeHandler = null
			};
			jsonTypeInfo = JsonMetadataServices.CreateListInfo<List<SerializableMapDrawingLine>, SerializableMapDrawingLine>(options, collectionInfo);
			jsonTypeInfo.NumberHandling = null;
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private JsonTypeInfo<List<SerializablePlayerMapDrawings>> Create_ListSerializablePlayerMapDrawings(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<List<SerializablePlayerMapDrawings>> jsonTypeInfo))
		{
			JsonCollectionInfoValues<List<SerializablePlayerMapDrawings>> collectionInfo = new JsonCollectionInfoValues<List<SerializablePlayerMapDrawings>>
			{
				ObjectCreator = () => new List<SerializablePlayerMapDrawings>(),
				SerializeHandler = null
			};
			jsonTypeInfo = JsonMetadataServices.CreateListInfo<List<SerializablePlayerMapDrawings>, SerializablePlayerMapDrawings>(options, collectionInfo);
			jsonTypeInfo.NumberHandling = null;
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private JsonTypeInfo<List<MigratingData>> Create_ListMigratingData(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<List<MigratingData>> jsonTypeInfo))
		{
			JsonCollectionInfoValues<List<MigratingData>> collectionInfo = new JsonCollectionInfoValues<List<MigratingData>>
			{
				ObjectCreator = () => new List<MigratingData>(),
				SerializeHandler = null
			};
			jsonTypeInfo = JsonMetadataServices.CreateListInfo<List<MigratingData>, MigratingData>(options, collectionInfo);
			jsonTypeInfo.NumberHandling = null;
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private JsonTypeInfo<List<SavedProperties.SavedProperty<bool>>> Create_ListSavedPropertyBoolean(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<List<SavedProperties.SavedProperty<bool>>> jsonTypeInfo))
		{
			JsonCollectionInfoValues<List<SavedProperties.SavedProperty<bool>>> collectionInfo = new JsonCollectionInfoValues<List<SavedProperties.SavedProperty<bool>>>
			{
				ObjectCreator = () => new List<SavedProperties.SavedProperty<bool>>(),
				SerializeHandler = null
			};
			jsonTypeInfo = JsonMetadataServices.CreateListInfo<List<SavedProperties.SavedProperty<bool>>, SavedProperties.SavedProperty<bool>>(options, collectionInfo);
			jsonTypeInfo.NumberHandling = null;
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private JsonTypeInfo<List<SavedProperties.SavedProperty<ModelId>>> Create_ListSavedPropertyModelId(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<List<SavedProperties.SavedProperty<ModelId>>> jsonTypeInfo))
		{
			JsonCollectionInfoValues<List<SavedProperties.SavedProperty<ModelId>>> collectionInfo = new JsonCollectionInfoValues<List<SavedProperties.SavedProperty<ModelId>>>
			{
				ObjectCreator = () => new List<SavedProperties.SavedProperty<ModelId>>(),
				SerializeHandler = null
			};
			jsonTypeInfo = JsonMetadataServices.CreateListInfo<List<SavedProperties.SavedProperty<ModelId>>, SavedProperties.SavedProperty<ModelId>>(options, collectionInfo);
			jsonTypeInfo.NumberHandling = null;
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private JsonTypeInfo<List<SavedProperties.SavedProperty<SerializableCard[]>>> Create_ListSavedPropertySerializableCardArray(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<List<SavedProperties.SavedProperty<SerializableCard[]>>> jsonTypeInfo))
		{
			JsonCollectionInfoValues<List<SavedProperties.SavedProperty<SerializableCard[]>>> collectionInfo = new JsonCollectionInfoValues<List<SavedProperties.SavedProperty<SerializableCard[]>>>
			{
				ObjectCreator = () => new List<SavedProperties.SavedProperty<SerializableCard[]>>(),
				SerializeHandler = null
			};
			jsonTypeInfo = JsonMetadataServices.CreateListInfo<List<SavedProperties.SavedProperty<SerializableCard[]>>, SavedProperties.SavedProperty<SerializableCard[]>>(options, collectionInfo);
			jsonTypeInfo.NumberHandling = null;
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private JsonTypeInfo<List<SavedProperties.SavedProperty<SerializableCard>>> Create_ListSavedPropertySerializableCard(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<List<SavedProperties.SavedProperty<SerializableCard>>> jsonTypeInfo))
		{
			JsonCollectionInfoValues<List<SavedProperties.SavedProperty<SerializableCard>>> collectionInfo = new JsonCollectionInfoValues<List<SavedProperties.SavedProperty<SerializableCard>>>
			{
				ObjectCreator = () => new List<SavedProperties.SavedProperty<SerializableCard>>(),
				SerializeHandler = null
			};
			jsonTypeInfo = JsonMetadataServices.CreateListInfo<List<SavedProperties.SavedProperty<SerializableCard>>, SavedProperties.SavedProperty<SerializableCard>>(options, collectionInfo);
			jsonTypeInfo.NumberHandling = null;
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private JsonTypeInfo<List<SavedProperties.SavedProperty<int[]>>> Create_ListSavedPropertyInt32Array(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<List<SavedProperties.SavedProperty<int[]>>> jsonTypeInfo))
		{
			JsonCollectionInfoValues<List<SavedProperties.SavedProperty<int[]>>> collectionInfo = new JsonCollectionInfoValues<List<SavedProperties.SavedProperty<int[]>>>
			{
				ObjectCreator = () => new List<SavedProperties.SavedProperty<int[]>>(),
				SerializeHandler = null
			};
			jsonTypeInfo = JsonMetadataServices.CreateListInfo<List<SavedProperties.SavedProperty<int[]>>, SavedProperties.SavedProperty<int[]>>(options, collectionInfo);
			jsonTypeInfo.NumberHandling = null;
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private JsonTypeInfo<List<SavedProperties.SavedProperty<int>>> Create_ListSavedPropertyInt32(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<List<SavedProperties.SavedProperty<int>>> jsonTypeInfo))
		{
			JsonCollectionInfoValues<List<SavedProperties.SavedProperty<int>>> collectionInfo = new JsonCollectionInfoValues<List<SavedProperties.SavedProperty<int>>>
			{
				ObjectCreator = () => new List<SavedProperties.SavedProperty<int>>(),
				SerializeHandler = null
			};
			jsonTypeInfo = JsonMetadataServices.CreateListInfo<List<SavedProperties.SavedProperty<int>>, SavedProperties.SavedProperty<int>>(options, collectionInfo);
			jsonTypeInfo.NumberHandling = null;
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private JsonTypeInfo<List<SavedProperties.SavedProperty<string>>> Create_ListSavedPropertyString(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<List<SavedProperties.SavedProperty<string>>> jsonTypeInfo))
		{
			JsonCollectionInfoValues<List<SavedProperties.SavedProperty<string>>> collectionInfo = new JsonCollectionInfoValues<List<SavedProperties.SavedProperty<string>>>
			{
				ObjectCreator = () => new List<SavedProperties.SavedProperty<string>>(),
				SerializeHandler = null
			};
			jsonTypeInfo = JsonMetadataServices.CreateListInfo<List<SavedProperties.SavedProperty<string>>, SavedProperties.SavedProperty<string>>(options, collectionInfo);
			jsonTypeInfo.NumberHandling = null;
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private JsonTypeInfo<List<SerializableActModel>> Create_ListSerializableActModel(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<List<SerializableActModel>> jsonTypeInfo))
		{
			JsonCollectionInfoValues<List<SerializableActModel>> collectionInfo = new JsonCollectionInfoValues<List<SerializableActModel>>
			{
				ObjectCreator = () => new List<SerializableActModel>(),
				SerializeHandler = null
			};
			jsonTypeInfo = JsonMetadataServices.CreateListInfo<List<SerializableActModel>, SerializableActModel>(options, collectionInfo);
			jsonTypeInfo.NumberHandling = null;
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private JsonTypeInfo<List<SerializableCard>> Create_ListSerializableCard(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<List<SerializableCard>> jsonTypeInfo))
		{
			JsonCollectionInfoValues<List<SerializableCard>> collectionInfo = new JsonCollectionInfoValues<List<SerializableCard>>
			{
				ObjectCreator = () => new List<SerializableCard>(),
				SerializeHandler = null
			};
			jsonTypeInfo = JsonMetadataServices.CreateListInfo<List<SerializableCard>, SerializableCard>(options, collectionInfo);
			jsonTypeInfo.NumberHandling = null;
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private JsonTypeInfo<List<SerializableMapPoint>> Create_ListSerializableMapPoint(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<List<SerializableMapPoint>> jsonTypeInfo))
		{
			JsonCollectionInfoValues<List<SerializableMapPoint>> collectionInfo = new JsonCollectionInfoValues<List<SerializableMapPoint>>
			{
				ObjectCreator = () => new List<SerializableMapPoint>(),
				SerializeHandler = null
			};
			jsonTypeInfo = JsonMetadataServices.CreateListInfo<List<SerializableMapPoint>, SerializableMapPoint>(options, collectionInfo);
			jsonTypeInfo.NumberHandling = null;
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private JsonTypeInfo<List<SerializableModifier>> Create_ListSerializableModifier(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<List<SerializableModifier>> jsonTypeInfo))
		{
			JsonCollectionInfoValues<List<SerializableModifier>> collectionInfo = new JsonCollectionInfoValues<List<SerializableModifier>>
			{
				ObjectCreator = () => new List<SerializableModifier>(),
				SerializeHandler = null
			};
			jsonTypeInfo = JsonMetadataServices.CreateListInfo<List<SerializableModifier>, SerializableModifier>(options, collectionInfo);
			jsonTypeInfo.NumberHandling = null;
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private JsonTypeInfo<List<SerializablePlayer>> Create_ListSerializablePlayer(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<List<SerializablePlayer>> jsonTypeInfo))
		{
			JsonCollectionInfoValues<List<SerializablePlayer>> collectionInfo = new JsonCollectionInfoValues<List<SerializablePlayer>>
			{
				ObjectCreator = () => new List<SerializablePlayer>(),
				SerializeHandler = null
			};
			jsonTypeInfo = JsonMetadataServices.CreateListInfo<List<SerializablePlayer>, SerializablePlayer>(options, collectionInfo);
			jsonTypeInfo.NumberHandling = null;
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private JsonTypeInfo<List<SerializablePotion>> Create_ListSerializablePotion(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<List<SerializablePotion>> jsonTypeInfo))
		{
			JsonCollectionInfoValues<List<SerializablePotion>> collectionInfo = new JsonCollectionInfoValues<List<SerializablePotion>>
			{
				ObjectCreator = () => new List<SerializablePotion>(),
				SerializeHandler = null
			};
			jsonTypeInfo = JsonMetadataServices.CreateListInfo<List<SerializablePotion>, SerializablePotion>(options, collectionInfo);
			jsonTypeInfo.NumberHandling = null;
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private JsonTypeInfo<List<SerializableRelic>> Create_ListSerializableRelic(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<List<SerializableRelic>> jsonTypeInfo))
		{
			JsonCollectionInfoValues<List<SerializableRelic>> collectionInfo = new JsonCollectionInfoValues<List<SerializableRelic>>
			{
				ObjectCreator = () => new List<SerializableRelic>(),
				SerializeHandler = null
			};
			jsonTypeInfo = JsonMetadataServices.CreateListInfo<List<SerializableRelic>, SerializableRelic>(options, collectionInfo);
			jsonTypeInfo.NumberHandling = null;
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private JsonTypeInfo<List<SerializableReward>> Create_ListSerializableReward(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<List<SerializableReward>> jsonTypeInfo))
		{
			JsonCollectionInfoValues<List<SerializableReward>> collectionInfo = new JsonCollectionInfoValues<List<SerializableReward>>
			{
				ObjectCreator = () => new List<SerializableReward>(),
				SerializeHandler = null
			};
			jsonTypeInfo = JsonMetadataServices.CreateListInfo<List<SerializableReward>, SerializableReward>(options, collectionInfo);
			jsonTypeInfo.NumberHandling = null;
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private JsonTypeInfo<List<SerializableEpoch>> Create_ListSerializableEpoch(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<List<SerializableEpoch>> jsonTypeInfo))
		{
			JsonCollectionInfoValues<List<SerializableEpoch>> collectionInfo = new JsonCollectionInfoValues<List<SerializableEpoch>>
			{
				ObjectCreator = () => new List<SerializableEpoch>(),
				SerializeHandler = null
			};
			jsonTypeInfo = JsonMetadataServices.CreateListInfo<List<SerializableEpoch>, SerializableEpoch>(options, collectionInfo);
			jsonTypeInfo.NumberHandling = null;
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private JsonTypeInfo<List<SerializableUnlockedAchievement>> Create_ListSerializableUnlockedAchievement(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<List<SerializableUnlockedAchievement>> jsonTypeInfo))
		{
			JsonCollectionInfoValues<List<SerializableUnlockedAchievement>> collectionInfo = new JsonCollectionInfoValues<List<SerializableUnlockedAchievement>>
			{
				ObjectCreator = () => new List<SerializableUnlockedAchievement>(),
				SerializeHandler = null
			};
			jsonTypeInfo = JsonMetadataServices.CreateListInfo<List<SerializableUnlockedAchievement>, SerializableUnlockedAchievement>(options, collectionInfo);
			jsonTypeInfo.NumberHandling = null;
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private JsonTypeInfo<List<Dictionary<string, object>>> Create_ListDictionaryStringObject(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<List<Dictionary<string, object>>> jsonTypeInfo))
		{
			JsonCollectionInfoValues<List<Dictionary<string, object>>> collectionInfo = new JsonCollectionInfoValues<List<Dictionary<string, object>>>
			{
				ObjectCreator = () => new List<Dictionary<string, object>>(),
				SerializeHandler = null
			};
			jsonTypeInfo = JsonMetadataServices.CreateListInfo<List<Dictionary<string, object>>, Dictionary<string, object>>(options, collectionInfo);
			jsonTypeInfo.NumberHandling = null;
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private JsonTypeInfo<List<List<MapPointHistoryEntry>>> Create_ListListMapPointHistoryEntry(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<List<List<MapPointHistoryEntry>>> jsonTypeInfo))
		{
			JsonCollectionInfoValues<List<List<MapPointHistoryEntry>>> collectionInfo = new JsonCollectionInfoValues<List<List<MapPointHistoryEntry>>>
			{
				ObjectCreator = () => new List<List<MapPointHistoryEntry>>(),
				SerializeHandler = null
			};
			jsonTypeInfo = JsonMetadataServices.CreateListInfo<List<List<MapPointHistoryEntry>>, List<MapPointHistoryEntry>>(options, collectionInfo);
			jsonTypeInfo.NumberHandling = null;
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private JsonTypeInfo<List<List<PlayerMapPointHistoryEntry>>> Create_ListListPlayerMapPointHistoryEntry(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<List<List<PlayerMapPointHistoryEntry>>> jsonTypeInfo))
		{
			JsonCollectionInfoValues<List<List<PlayerMapPointHistoryEntry>>> collectionInfo = new JsonCollectionInfoValues<List<List<PlayerMapPointHistoryEntry>>>
			{
				ObjectCreator = () => new List<List<PlayerMapPointHistoryEntry>>(),
				SerializeHandler = null
			};
			jsonTypeInfo = JsonMetadataServices.CreateListInfo<List<List<PlayerMapPointHistoryEntry>>, List<PlayerMapPointHistoryEntry>>(options, collectionInfo);
			jsonTypeInfo.NumberHandling = null;
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private JsonTypeInfo<List<JsonNode>> Create_ListJsonNode(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<List<JsonNode>> jsonTypeInfo))
		{
			JsonCollectionInfoValues<List<JsonNode>> collectionInfo = new JsonCollectionInfoValues<List<JsonNode>>
			{
				ObjectCreator = () => new List<JsonNode>(),
				SerializeHandler = null
			};
			jsonTypeInfo = JsonMetadataServices.CreateListInfo<List<JsonNode>, JsonNode>(options, collectionInfo);
			jsonTypeInfo.NumberHandling = null;
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private JsonTypeInfo<List<string>> Create_ListString(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<List<string>> jsonTypeInfo))
		{
			JsonCollectionInfoValues<List<string>> collectionInfo = new JsonCollectionInfoValues<List<string>>
			{
				ObjectCreator = () => new List<string>(),
				SerializeHandler = null
			};
			jsonTypeInfo = JsonMetadataServices.CreateListInfo<List<string>, string>(options, collectionInfo);
			jsonTypeInfo.NumberHandling = null;
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private JsonTypeInfo<List<ulong>> Create_ListUInt64(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<List<ulong>> jsonTypeInfo))
		{
			JsonCollectionInfoValues<List<ulong>> collectionInfo = new JsonCollectionInfoValues<List<ulong>>
			{
				ObjectCreator = () => new List<ulong>(),
				SerializeHandler = null
			};
			jsonTypeInfo = JsonMetadataServices.CreateListInfo<List<ulong>, ulong>(options, collectionInfo);
			jsonTypeInfo.NumberHandling = null;
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private JsonTypeInfo<DateTimeOffset> Create_DateTimeOffset(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<DateTimeOffset> jsonTypeInfo))
		{
			jsonTypeInfo = JsonMetadataServices.CreateValueInfo<DateTimeOffset>(options, JsonMetadataServices.DateTimeOffsetConverter);
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private JsonTypeInfo<DateTimeOffset?> Create_NullableDateTimeOffset(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<DateTimeOffset?> jsonTypeInfo))
		{
			JsonConverter nullableConverter = JsonMetadataServices.GetNullableConverter<DateTimeOffset>(options);
			jsonTypeInfo = JsonMetadataServices.CreateValueInfo<DateTimeOffset?>(options, nullableConverter);
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private JsonTypeInfo<JsonNode> Create_JsonNode(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<JsonNode> jsonTypeInfo))
		{
			jsonTypeInfo = JsonMetadataServices.CreateValueInfo<JsonNode>(options, JsonMetadataServices.JsonNodeConverter);
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private JsonTypeInfo<JsonObject> Create_JsonObject(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<JsonObject> jsonTypeInfo))
		{
			jsonTypeInfo = JsonMetadataServices.CreateValueInfo<JsonObject>(options, JsonMetadataServices.JsonObjectConverter);
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private JsonTypeInfo<int> Create_Int32(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<int> jsonTypeInfo))
		{
			jsonTypeInfo = JsonMetadataServices.CreateValueInfo<int>(options, JsonMetadataServices.Int32Converter);
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private JsonTypeInfo<int?> Create_NullableInt32(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<int?> jsonTypeInfo))
		{
			JsonConverter nullableConverter = JsonMetadataServices.GetNullableConverter<int>(options);
			jsonTypeInfo = JsonMetadataServices.CreateValueInfo<int?>(options, nullableConverter);
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private JsonTypeInfo<int[]> Create_Int32Array(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<int[]> jsonTypeInfo))
		{
			JsonCollectionInfoValues<int[]> collectionInfo = new JsonCollectionInfoValues<int[]>
			{
				ObjectCreator = null,
				SerializeHandler = null
			};
			jsonTypeInfo = JsonMetadataServices.CreateArrayInfo(options, collectionInfo);
			jsonTypeInfo.NumberHandling = null;
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private JsonTypeInfo<long> Create_Int64(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<long> jsonTypeInfo))
		{
			jsonTypeInfo = JsonMetadataServices.CreateValueInfo<long>(options, JsonMetadataServices.Int64Converter);
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private JsonTypeInfo<object> Create_Object(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<object> jsonTypeInfo))
		{
			jsonTypeInfo = JsonMetadataServices.CreateValueInfo<object>(options, JsonMetadataServices.ObjectConverter);
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private JsonTypeInfo<string> Create_String(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<string> jsonTypeInfo))
		{
			jsonTypeInfo = JsonMetadataServices.CreateValueInfo<string>(options, JsonMetadataServices.StringConverter);
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private JsonTypeInfo<ulong> Create_UInt64(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<ulong> jsonTypeInfo))
		{
			jsonTypeInfo = JsonMetadataServices.CreateValueInfo<ulong>(options, JsonMetadataServices.UInt64Converter);
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	/// <inheritdoc />
	public MegaCritSerializerContext()
		: base(null)
	{
	}

	/// <inheritdoc />
	public MegaCritSerializerContext(JsonSerializerOptions options)
		: base(options)
	{
	}

	private static bool TryGetTypeInfoForRuntimeCustomConverter<TJsonMetadataType>(JsonSerializerOptions options, out JsonTypeInfo<TJsonMetadataType> jsonTypeInfo)
	{
		JsonConverter runtimeConverterForType = GetRuntimeConverterForType(typeof(TJsonMetadataType), options);
		if (runtimeConverterForType != null)
		{
			jsonTypeInfo = JsonMetadataServices.CreateValueInfo<TJsonMetadataType>(options, runtimeConverterForType);
			return true;
		}
		jsonTypeInfo = null;
		return false;
	}

	private static JsonConverter? GetRuntimeConverterForType(Type type, JsonSerializerOptions options)
	{
		for (int i = 0; i < options.Converters.Count; i++)
		{
			JsonConverter jsonConverter = options.Converters[i];
			if (jsonConverter != null && jsonConverter.CanConvert(type))
			{
				return ExpandConverter(type, jsonConverter, options, validateCanConvert: false);
			}
		}
		return null;
	}

	private static JsonConverter ExpandConverter(Type type, JsonConverter converter, JsonSerializerOptions options, bool validateCanConvert = true)
	{
		if (validateCanConvert && !converter.CanConvert(type))
		{
			throw new InvalidOperationException($"The converter '{converter.GetType()}' is not compatible with the type '{type}'.");
		}
		if (converter is JsonConverterFactory jsonConverterFactory)
		{
			converter = jsonConverterFactory.CreateConverter(type, options);
			if (converter == null || converter is JsonConverterFactory)
			{
				throw new InvalidOperationException($"The converter '{jsonConverterFactory.GetType()}' cannot return null or a JsonConverterFactory instance.");
			}
		}
		return converter;
	}

	/// <inheritdoc />
	public override JsonTypeInfo? GetTypeInfo(Type type)
	{
		Options.TryGetTypeInfo(type, out JsonTypeInfo typeInfo);
		return typeInfo;
	}

	JsonTypeInfo? IJsonTypeInfoResolver.GetTypeInfo(Type type, JsonSerializerOptions options)
	{
		if (type == typeof(bool))
		{
			return Create_Boolean(options);
		}
		if (type == typeof(double))
		{
			return Create_Double(options);
		}
		if (type == typeof(float))
		{
			return Create_Single(options);
		}
		if (type == typeof(Vector2))
		{
			return Create_Vector2(options);
		}
		if (type == typeof(Vector2I))
		{
			return Create_Vector2I(options);
		}
		if (type == typeof(ControllerMappingType))
		{
			return Create_ControllerMappingType(options);
		}
		if (type == typeof(RelicRarity))
		{
			return Create_RelicRarity(options);
		}
		if (type == typeof(PlayerRngType))
		{
			return Create_PlayerRngType(options);
		}
		if (type == typeof(RunRngType))
		{
			return Create_RunRngType(options);
		}
		if (type == typeof(LocString))
		{
			return Create_LocString(options);
		}
		if (type == typeof(MapCoord))
		{
			return Create_MapCoord(options);
		}
		if (type == typeof(MapPointType))
		{
			return Create_MapPointType(options);
		}
		if (type == typeof(ModDependency))
		{
			return Create_ModDependency(options);
		}
		if (type == typeof(ModManifest))
		{
			return Create_ModManifest(options);
		}
		if (type == typeof(ModSettings))
		{
			return Create_ModSettings(options);
		}
		if (type == typeof(ModSource))
		{
			return Create_ModSource(options);
		}
		if (type == typeof(SettingsSaveMod))
		{
			return Create_SettingsSaveMod(options);
		}
		if (type == typeof(BadgeRarity))
		{
			return Create_BadgeRarity(options);
		}
		if (type == typeof(ModelId))
		{
			return Create_ModelId(options);
		}
		if (type == typeof(FeedbackData))
		{
			return Create_FeedbackData(options);
		}
		if (type == typeof(NullLeaderboard))
		{
			return Create_NullLeaderboard(options);
		}
		if (type == typeof(NullLeaderboardFile))
		{
			return Create_NullLeaderboardFile(options);
		}
		if (type == typeof(NullLeaderboardFileEntry))
		{
			return Create_NullLeaderboardFileEntry(options);
		}
		if (type == typeof(NullMultiplayerName))
		{
			return Create_NullMultiplayerName(options);
		}
		if (type == typeof(PlatformType))
		{
			return Create_PlatformType(options);
		}
		if (type == typeof(RewardType))
		{
			return Create_RewardType(options);
		}
		if (type == typeof(RoomType))
		{
			return Create_RoomType(options);
		}
		if (type == typeof(CardCreationSource))
		{
			return Create_CardCreationSource(options);
		}
		if (type == typeof(CardRarityOddsType))
		{
			return Create_CardRarityOddsType(options);
		}
		if (type == typeof(GameMode))
		{
			return Create_GameMode(options);
		}
		if (type == typeof(AncientChoiceHistoryEntry))
		{
			return Create_AncientChoiceHistoryEntry(options);
		}
		if (type == typeof(CardChoiceHistoryEntry))
		{
			return Create_CardChoiceHistoryEntry(options);
		}
		if (type == typeof(CardEnchantmentHistoryEntry))
		{
			return Create_CardEnchantmentHistoryEntry(options);
		}
		if (type == typeof(CardTransformationHistoryEntry))
		{
			return Create_CardTransformationHistoryEntry(options);
		}
		if (type == typeof(EventOptionHistoryEntry))
		{
			return Create_EventOptionHistoryEntry(options);
		}
		if (type == typeof(MapPointHistoryEntry))
		{
			return Create_MapPointHistoryEntry(options);
		}
		if (type == typeof(MapPointRoomHistoryEntry))
		{
			return Create_MapPointRoomHistoryEntry(options);
		}
		if (type == typeof(ModelChoiceHistoryEntry))
		{
			return Create_ModelChoiceHistoryEntry(options);
		}
		if (type == typeof(PlayerMapPointHistoryEntry))
		{
			return Create_PlayerMapPointHistoryEntry(options);
		}
		if (type == typeof(RunHistory))
		{
			return Create_RunHistory(options);
		}
		if (type == typeof(RunHistoryPlayer))
		{
			return Create_RunHistoryPlayer(options);
		}
		if (type == typeof(AncientCharacterStats))
		{
			return Create_AncientCharacterStats(options);
		}
		if (type == typeof(AncientStats))
		{
			return Create_AncientStats(options);
		}
		if (type == typeof(BadgeStats))
		{
			return Create_BadgeStats(options);
		}
		if (type == typeof(CardStats))
		{
			return Create_CardStats(options);
		}
		if (type == typeof(CharacterStats))
		{
			return Create_CharacterStats(options);
		}
		if (type == typeof(EncounterStats))
		{
			return Create_EncounterStats(options);
		}
		if (type == typeof(EnemyStats))
		{
			return Create_EnemyStats(options);
		}
		if (type == typeof(EpochState))
		{
			return Create_EpochState(options);
		}
		if (type == typeof(FightStats))
		{
			return Create_FightStats(options);
		}
		if (type == typeof(SerializableMapDrawingLine))
		{
			return Create_SerializableMapDrawingLine(options);
		}
		if (type == typeof(SerializableMapDrawings))
		{
			return Create_SerializableMapDrawings(options);
		}
		if (type == typeof(SerializablePlayerMapDrawings))
		{
			return Create_SerializablePlayerMapDrawings(options);
		}
		if (type == typeof(MigratingData))
		{
			return Create_MigratingData(options);
		}
		if (type == typeof(PrefsSave))
		{
			return Create_PrefsSave(options);
		}
		if (type == typeof(ProfileSave))
		{
			return Create_ProfileSave(options);
		}
		if (type == typeof(SavedProperties))
		{
			return Create_SavedProperties(options);
		}
		if (type == typeof(SavedProperties.SavedProperty<bool>))
		{
			return Create_SavedPropertyBoolean(options);
		}
		if (type == typeof(SavedProperties.SavedProperty<ModelId>))
		{
			return Create_SavedPropertyModelId(options);
		}
		if (type == typeof(SavedProperties.SavedProperty<SerializableCard[]>))
		{
			return Create_SavedPropertySerializableCardArray(options);
		}
		if (type == typeof(SavedProperties.SavedProperty<SerializableCard>))
		{
			return Create_SavedPropertySerializableCard(options);
		}
		if (type == typeof(SavedProperties.SavedProperty<int[]>))
		{
			return Create_SavedPropertyInt32Array(options);
		}
		if (type == typeof(SavedProperties.SavedProperty<int>))
		{
			return Create_SavedPropertyInt32(options);
		}
		if (type == typeof(SavedProperties.SavedProperty<string>))
		{
			return Create_SavedPropertyString(options);
		}
		if (type == typeof(SerializableActMap))
		{
			return Create_SerializableActMap(options);
		}
		if (type == typeof(SerializableActModel))
		{
			return Create_SerializableActModel(options);
		}
		if (type == typeof(SerializableBadge))
		{
			return Create_SerializableBadge(options);
		}
		if (type == typeof(SerializableCard))
		{
			return Create_SerializableCard(options);
		}
		if (type == typeof(SerializableCard[]))
		{
			return Create_SerializableCardArray(options);
		}
		if (type == typeof(SerializableEnchantment))
		{
			return Create_SerializableEnchantment(options);
		}
		if (type == typeof(SerializableExtraRunFields))
		{
			return Create_SerializableExtraRunFields(options);
		}
		if (type == typeof(SerializableMapPoint))
		{
			return Create_SerializableMapPoint(options);
		}
		if (type == typeof(SerializableModifier))
		{
			return Create_SerializableModifier(options);
		}
		if (type == typeof(SerializablePlayer))
		{
			return Create_SerializablePlayer(options);
		}
		if (type == typeof(SerializablePlayerOddsSet))
		{
			return Create_SerializablePlayerOddsSet(options);
		}
		if (type == typeof(SerializablePotion))
		{
			return Create_SerializablePotion(options);
		}
		if (type == typeof(SerializableRelic))
		{
			return Create_SerializableRelic(options);
		}
		if (type == typeof(SerializableRelicGrabBag))
		{
			return Create_SerializableRelicGrabBag(options);
		}
		if (type == typeof(SerializableReward))
		{
			return Create_SerializableReward(options);
		}
		if (type == typeof(SerializableRoom))
		{
			return Create_SerializableRoom(options);
		}
		if (type == typeof(SerializableRoomSet))
		{
			return Create_SerializableRoomSet(options);
		}
		if (type == typeof(SerializableRunOddsSet))
		{
			return Create_SerializableRunOddsSet(options);
		}
		if (type == typeof(SerializableRunRngSet))
		{
			return Create_SerializableRunRngSet(options);
		}
		if (type == typeof(SerializableEpoch))
		{
			return Create_SerializableEpoch(options);
		}
		if (type == typeof(SerializableExtraPlayerFields))
		{
			return Create_SerializableExtraPlayerFields(options);
		}
		if (type == typeof(SerializablePlayerRngSet))
		{
			return Create_SerializablePlayerRngSet(options);
		}
		if (type == typeof(SerializableProgress))
		{
			return Create_SerializableProgress(options);
		}
		if (type == typeof(SerializableRng))
		{
			return Create_SerializableRng(options);
		}
		if (type == typeof(SerializableRun))
		{
			return Create_SerializableRun(options);
		}
		if (type == typeof(SerializableUnlockedAchievement))
		{
			return Create_SerializableUnlockedAchievement(options);
		}
		if (type == typeof(SettingsSave))
		{
			return Create_SettingsSave(options);
		}
		if (type == typeof(AspectRatioSetting))
		{
			return Create_AspectRatioSetting(options);
		}
		if (type == typeof(FastModeType))
		{
			return Create_FastModeType(options);
		}
		if (type == typeof(VSyncType))
		{
			return Create_VSyncType(options);
		}
		if (type == typeof(SerializableUnlockState))
		{
			return Create_SerializableUnlockState(options);
		}
		if (type == typeof(Dictionary<RelicRarity, List<ModelId>>))
		{
			return Create_DictionaryRelicRarityListModelId(options);
		}
		if (type == typeof(Dictionary<PlayerRngType, SerializableRng>))
		{
			return Create_DictionaryPlayerRngTypeSerializableRng(options);
		}
		if (type == typeof(Dictionary<RunRngType, SerializableRng>))
		{
			return Create_DictionaryRunRngTypeSerializableRng(options);
		}
		if (type == typeof(Dictionary<string, object>))
		{
			return Create_DictionaryStringObject(options);
		}
		if (type == typeof(Dictionary<string, string>))
		{
			return Create_DictionaryStringString(options);
		}
		if (type == typeof(Dictionary<ulong, List<SerializableReward>>))
		{
			return Create_DictionaryUInt64ListSerializableReward(options);
		}
		if (type == typeof(IEnumerable<SerializableBadge>))
		{
			return Create_IEnumerableSerializableBadge(options);
		}
		if (type == typeof(IEnumerable<SerializableCard>))
		{
			return Create_IEnumerableSerializableCard(options);
		}
		if (type == typeof(IEnumerable<SerializablePotion>))
		{
			return Create_IEnumerableSerializablePotion(options);
		}
		if (type == typeof(IEnumerable<SerializableRelic>))
		{
			return Create_IEnumerableSerializableRelic(options);
		}
		if (type == typeof(List<Vector2>))
		{
			return Create_ListVector2(options);
		}
		if (type == typeof(List<MapCoord>))
		{
			return Create_ListMapCoord(options);
		}
		if (type == typeof(List<ModDependency>))
		{
			return Create_ListModDependency(options);
		}
		if (type == typeof(List<SettingsSaveMod>))
		{
			return Create_ListSettingsSaveMod(options);
		}
		if (type == typeof(List<ModelId>))
		{
			return Create_ListModelId(options);
		}
		if (type == typeof(List<NullLeaderboard>))
		{
			return Create_ListNullLeaderboard(options);
		}
		if (type == typeof(List<NullLeaderboardFileEntry>))
		{
			return Create_ListNullLeaderboardFileEntry(options);
		}
		if (type == typeof(List<NullMultiplayerName>))
		{
			return Create_ListNullMultiplayerName(options);
		}
		if (type == typeof(List<AncientChoiceHistoryEntry>))
		{
			return Create_ListAncientChoiceHistoryEntry(options);
		}
		if (type == typeof(List<CardChoiceHistoryEntry>))
		{
			return Create_ListCardChoiceHistoryEntry(options);
		}
		if (type == typeof(List<CardEnchantmentHistoryEntry>))
		{
			return Create_ListCardEnchantmentHistoryEntry(options);
		}
		if (type == typeof(List<CardTransformationHistoryEntry>))
		{
			return Create_ListCardTransformationHistoryEntry(options);
		}
		if (type == typeof(List<EventOptionHistoryEntry>))
		{
			return Create_ListEventOptionHistoryEntry(options);
		}
		if (type == typeof(List<MapPointHistoryEntry>))
		{
			return Create_ListMapPointHistoryEntry(options);
		}
		if (type == typeof(List<MapPointRoomHistoryEntry>))
		{
			return Create_ListMapPointRoomHistoryEntry(options);
		}
		if (type == typeof(List<ModelChoiceHistoryEntry>))
		{
			return Create_ListModelChoiceHistoryEntry(options);
		}
		if (type == typeof(List<PlayerMapPointHistoryEntry>))
		{
			return Create_ListPlayerMapPointHistoryEntry(options);
		}
		if (type == typeof(List<RunHistoryPlayer>))
		{
			return Create_ListRunHistoryPlayer(options);
		}
		if (type == typeof(List<AncientCharacterStats>))
		{
			return Create_ListAncientCharacterStats(options);
		}
		if (type == typeof(List<AncientStats>))
		{
			return Create_ListAncientStats(options);
		}
		if (type == typeof(List<BadgeStats>))
		{
			return Create_ListBadgeStats(options);
		}
		if (type == typeof(List<CardStats>))
		{
			return Create_ListCardStats(options);
		}
		if (type == typeof(List<CharacterStats>))
		{
			return Create_ListCharacterStats(options);
		}
		if (type == typeof(List<EncounterStats>))
		{
			return Create_ListEncounterStats(options);
		}
		if (type == typeof(List<EnemyStats>))
		{
			return Create_ListEnemyStats(options);
		}
		if (type == typeof(List<FightStats>))
		{
			return Create_ListFightStats(options);
		}
		if (type == typeof(List<SerializableMapDrawingLine>))
		{
			return Create_ListSerializableMapDrawingLine(options);
		}
		if (type == typeof(List<SerializablePlayerMapDrawings>))
		{
			return Create_ListSerializablePlayerMapDrawings(options);
		}
		if (type == typeof(List<MigratingData>))
		{
			return Create_ListMigratingData(options);
		}
		if (type == typeof(List<SavedProperties.SavedProperty<bool>>))
		{
			return Create_ListSavedPropertyBoolean(options);
		}
		if (type == typeof(List<SavedProperties.SavedProperty<ModelId>>))
		{
			return Create_ListSavedPropertyModelId(options);
		}
		if (type == typeof(List<SavedProperties.SavedProperty<SerializableCard[]>>))
		{
			return Create_ListSavedPropertySerializableCardArray(options);
		}
		if (type == typeof(List<SavedProperties.SavedProperty<SerializableCard>>))
		{
			return Create_ListSavedPropertySerializableCard(options);
		}
		if (type == typeof(List<SavedProperties.SavedProperty<int[]>>))
		{
			return Create_ListSavedPropertyInt32Array(options);
		}
		if (type == typeof(List<SavedProperties.SavedProperty<int>>))
		{
			return Create_ListSavedPropertyInt32(options);
		}
		if (type == typeof(List<SavedProperties.SavedProperty<string>>))
		{
			return Create_ListSavedPropertyString(options);
		}
		if (type == typeof(List<SerializableActModel>))
		{
			return Create_ListSerializableActModel(options);
		}
		if (type == typeof(List<SerializableCard>))
		{
			return Create_ListSerializableCard(options);
		}
		if (type == typeof(List<SerializableMapPoint>))
		{
			return Create_ListSerializableMapPoint(options);
		}
		if (type == typeof(List<SerializableModifier>))
		{
			return Create_ListSerializableModifier(options);
		}
		if (type == typeof(List<SerializablePlayer>))
		{
			return Create_ListSerializablePlayer(options);
		}
		if (type == typeof(List<SerializablePotion>))
		{
			return Create_ListSerializablePotion(options);
		}
		if (type == typeof(List<SerializableRelic>))
		{
			return Create_ListSerializableRelic(options);
		}
		if (type == typeof(List<SerializableReward>))
		{
			return Create_ListSerializableReward(options);
		}
		if (type == typeof(List<SerializableEpoch>))
		{
			return Create_ListSerializableEpoch(options);
		}
		if (type == typeof(List<SerializableUnlockedAchievement>))
		{
			return Create_ListSerializableUnlockedAchievement(options);
		}
		if (type == typeof(List<Dictionary<string, object>>))
		{
			return Create_ListDictionaryStringObject(options);
		}
		if (type == typeof(List<List<MapPointHistoryEntry>>))
		{
			return Create_ListListMapPointHistoryEntry(options);
		}
		if (type == typeof(List<List<PlayerMapPointHistoryEntry>>))
		{
			return Create_ListListPlayerMapPointHistoryEntry(options);
		}
		if (type == typeof(List<JsonNode>))
		{
			return Create_ListJsonNode(options);
		}
		if (type == typeof(List<string>))
		{
			return Create_ListString(options);
		}
		if (type == typeof(List<ulong>))
		{
			return Create_ListUInt64(options);
		}
		if (type == typeof(DateTimeOffset))
		{
			return Create_DateTimeOffset(options);
		}
		if (type == typeof(DateTimeOffset?))
		{
			return Create_NullableDateTimeOffset(options);
		}
		if (type == typeof(JsonNode))
		{
			return Create_JsonNode(options);
		}
		if (type == typeof(JsonObject))
		{
			return Create_JsonObject(options);
		}
		if (type == typeof(int))
		{
			return Create_Int32(options);
		}
		if (type == typeof(int?))
		{
			return Create_NullableInt32(options);
		}
		if (type == typeof(int[]))
		{
			return Create_Int32Array(options);
		}
		if (type == typeof(long))
		{
			return Create_Int64(options);
		}
		if (type == typeof(object))
		{
			return Create_Object(options);
		}
		if (type == typeof(string))
		{
			return Create_String(options);
		}
		if (type == typeof(ulong))
		{
			return Create_UInt64(options);
		}
		return null;
	}
}
