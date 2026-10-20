using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Godot;
using MegaCrit.Sts2.Core.Achievements;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Settings;
using MegaCrit.Sts2.Core.Timeline;
using MegaCrit.Sts2.GameInfo;

namespace MegaCrit.Sts2.Core.Runs.Metrics;

[JsonSourceGenerationOptions(WriteIndented = true, IncludeFields = true, UseStringEnumConverter = true, PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase, Converters = new Type[] { typeof(ModelIdMetricsConverter) })]
[JsonSerializable(typeof(RunMetrics))]
[JsonSerializable(typeof(AchievementMetric))]
[JsonSerializable(typeof(EpochMetric))]
[JsonSerializable(typeof(SettingsDataMetric))]
[GeneratedCode("System.Text.Json.SourceGeneration", "9.0.12.31616")]
internal class MetricsSerializerContext : JsonSerializerContext, IJsonTypeInfoResolver
{
	private JsonTypeInfo<bool>? _Boolean;

	private JsonTypeInfo<float>? _Single;

	private JsonTypeInfo<Vector2I>? _Vector2I;

	private JsonTypeInfo<AchievementMetric>? _AchievementMetric;

	private JsonTypeInfo<ModelId>? _ModelId;

	private JsonTypeInfo<ActWinMetric>? _ActWinMetric;

	private JsonTypeInfo<AncientMetric>? _AncientMetric;

	private JsonTypeInfo<CardChoiceMetric>? _CardChoiceMetric;

	private JsonTypeInfo<EncounterMetric>? _EncounterMetric;

	private JsonTypeInfo<EventChoiceMetric>? _EventChoiceMetric;

	private JsonTypeInfo<RunMetrics>? _RunMetrics;

	private JsonTypeInfo<SettingsDataMetric>? _SettingsDataMetric;

	private JsonTypeInfo<AspectRatioSetting>? _AspectRatioSetting;

	private JsonTypeInfo<FastModeType>? _FastModeType;

	private JsonTypeInfo<VSyncType>? _VSyncType;

	private JsonTypeInfo<EpochMetric>? _EpochMetric;

	private JsonTypeInfo<IEnumerable<ModelId>>? _IEnumerableModelId;

	private JsonTypeInfo<List<ModelId>>? _ListModelId;

	private JsonTypeInfo<List<ActWinMetric>>? _ListActWinMetric;

	private JsonTypeInfo<List<AncientMetric>>? _ListAncientMetric;

	private JsonTypeInfo<List<CardChoiceMetric>>? _ListCardChoiceMetric;

	private JsonTypeInfo<List<EncounterMetric>>? _ListEncounterMetric;

	private JsonTypeInfo<List<EventChoiceMetric>>? _ListEventChoiceMetric;

	private JsonTypeInfo<List<string>>? _ListString;

	private JsonTypeInfo<int>? _Int32;

	private JsonTypeInfo<long>? _Int64;

	private JsonTypeInfo<string>? _String;

	private static readonly JsonSerializerOptions s_defaultOptions = new JsonSerializerOptions
	{
		Converters = { (JsonConverter)new ModelIdMetricsConverter() },
		IncludeFields = true,
		PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
		WriteIndented = true
	};

	private const BindingFlags InstanceMemberBindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

	private static readonly JsonEncodedText PropName_x = JsonEncodedText.Encode("x");

	private static readonly JsonEncodedText PropName_y = JsonEncodedText.Encode("y");

	private static readonly JsonEncodedText PropName_buildId = JsonEncodedText.Encode("buildId");

	private static readonly JsonEncodedText PropName_achievement = JsonEncodedText.Encode("achievement");

	private static readonly JsonEncodedText PropName_totalAchievements = JsonEncodedText.Encode("totalAchievements");

	private static readonly JsonEncodedText PropName_totalPlaytime = JsonEncodedText.Encode("totalPlaytime");

	private static readonly JsonEncodedText PropName_totalRuns = JsonEncodedText.Encode("totalRuns");

	private static readonly JsonEncodedText PropName_category = JsonEncodedText.Encode("category");

	private static readonly JsonEncodedText PropName_entry = JsonEncodedText.Encode("entry");

	private static readonly JsonEncodedText PropName_act = JsonEncodedText.Encode("act");

	private static readonly JsonEncodedText PropName_win = JsonEncodedText.Encode("win");

	private static readonly JsonEncodedText PropName_picked = JsonEncodedText.Encode("picked");

	private static readonly JsonEncodedText PropName_skipped = JsonEncodedText.Encode("skipped");

	private static readonly JsonEncodedText PropName_id = JsonEncodedText.Encode("id");

	private static readonly JsonEncodedText PropName_damage = JsonEncodedText.Encode("damage");

	private static readonly JsonEncodedText PropName_turns = JsonEncodedText.Encode("turns");

	private static readonly JsonEncodedText PropName_playerId = JsonEncodedText.Encode("playerId");

	private static readonly JsonEncodedText PropName_character = JsonEncodedText.Encode("character");

	private static readonly JsonEncodedText PropName_numPlayers = JsonEncodedText.Encode("numPlayers");

	private static readonly JsonEncodedText PropName_team = JsonEncodedText.Encode("team");

	private static readonly JsonEncodedText PropName_buildType = JsonEncodedText.Encode("buildType");

	private static readonly JsonEncodedText PropName_ascension = JsonEncodedText.Encode("ascension");

	private static readonly JsonEncodedText PropName_totalWinRate = JsonEncodedText.Encode("totalWinRate");

	private static readonly JsonEncodedText PropName_numReloads = JsonEncodedText.Encode("numReloads");

	private static readonly JsonEncodedText PropName_runPlaytime = JsonEncodedText.Encode("runPlaytime");

	private static readonly JsonEncodedText PropName_floorReached = JsonEncodedText.Encode("floorReached");

	private static readonly JsonEncodedText PropName_killedByEncounter = JsonEncodedText.Encode("killedByEncounter");

	private static readonly JsonEncodedText PropName_cardChoices = JsonEncodedText.Encode("cardChoices");

	private static readonly JsonEncodedText PropName_campfireUpgrades = JsonEncodedText.Encode("campfireUpgrades");

	private static readonly JsonEncodedText PropName_eventChoices = JsonEncodedText.Encode("eventChoices");

	private static readonly JsonEncodedText PropName_ancientChoices = JsonEncodedText.Encode("ancientChoices");

	private static readonly JsonEncodedText PropName_relicBuys = JsonEncodedText.Encode("relicBuys");

	private static readonly JsonEncodedText PropName_potionBuys = JsonEncodedText.Encode("potionBuys");

	private static readonly JsonEncodedText PropName_colorlessBuys = JsonEncodedText.Encode("colorlessBuys");

	private static readonly JsonEncodedText PropName_potionDiscards = JsonEncodedText.Encode("potionDiscards");

	private static readonly JsonEncodedText PropName_encounters = JsonEncodedText.Encode("encounters");

	private static readonly JsonEncodedText PropName_actWins = JsonEncodedText.Encode("actWins");

	private static readonly JsonEncodedText PropName_deck = JsonEncodedText.Encode("deck");

	private static readonly JsonEncodedText PropName_relics = JsonEncodedText.Encode("relics");

	private static readonly JsonEncodedText PropName_os = JsonEncodedText.Encode("os");

	private static readonly JsonEncodedText PropName_platform = JsonEncodedText.Encode("platform");

	private static readonly JsonEncodedText PropName_systemRam = JsonEncodedText.Encode("systemRam");

	private static readonly JsonEncodedText PropName_language = JsonEncodedText.Encode("language");

	private static readonly JsonEncodedText PropName_combatSpeed = JsonEncodedText.Encode("combatSpeed");

	private static readonly JsonEncodedText PropName_screenshake = JsonEncodedText.Encode("screenshake");

	private static readonly JsonEncodedText PropName_runTimer = JsonEncodedText.Encode("runTimer");

	private static readonly JsonEncodedText PropName_phobiaMode = JsonEncodedText.Encode("phobiaMode");

	private static readonly JsonEncodedText PropName_cardIndices = JsonEncodedText.Encode("cardIndices");

	private static readonly JsonEncodedText PropName_displayCount = JsonEncodedText.Encode("displayCount");

	private static readonly JsonEncodedText PropName_displayResolution = JsonEncodedText.Encode("displayResolution");

	private static readonly JsonEncodedText PropName_fullscreen = JsonEncodedText.Encode("fullscreen");

	private static readonly JsonEncodedText PropName_aspectRatio = JsonEncodedText.Encode("aspectRatio");

	private static readonly JsonEncodedText PropName_resizeWindows = JsonEncodedText.Encode("resizeWindows");

	private static readonly JsonEncodedText PropName_vSync = JsonEncodedText.Encode("vSync");

	private static readonly JsonEncodedText PropName_fpsLimit = JsonEncodedText.Encode("fpsLimit");

	private static readonly JsonEncodedText PropName_msaa = JsonEncodedText.Encode("msaa");

	private static readonly JsonEncodedText PropName_epoch = JsonEncodedText.Encode("epoch");

	private static readonly JsonEncodedText PropName_totalEpochs = JsonEncodedText.Encode("totalEpochs");

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<bool> Boolean => _Boolean ?? (_Boolean = (JsonTypeInfo<bool>)Options.GetTypeInfo(typeof(bool)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<float> Single => _Single ?? (_Single = (JsonTypeInfo<float>)Options.GetTypeInfo(typeof(float)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<Vector2I> Vector2I => _Vector2I ?? (_Vector2I = (JsonTypeInfo<Vector2I>)Options.GetTypeInfo(typeof(Vector2I)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<AchievementMetric> AchievementMetric => _AchievementMetric ?? (_AchievementMetric = (JsonTypeInfo<AchievementMetric>)Options.GetTypeInfo(typeof(AchievementMetric)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<ModelId> ModelId => _ModelId ?? (_ModelId = (JsonTypeInfo<ModelId>)Options.GetTypeInfo(typeof(ModelId)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<ActWinMetric> ActWinMetric => _ActWinMetric ?? (_ActWinMetric = (JsonTypeInfo<ActWinMetric>)Options.GetTypeInfo(typeof(ActWinMetric)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<AncientMetric> AncientMetric => _AncientMetric ?? (_AncientMetric = (JsonTypeInfo<AncientMetric>)Options.GetTypeInfo(typeof(AncientMetric)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<CardChoiceMetric> CardChoiceMetric => _CardChoiceMetric ?? (_CardChoiceMetric = (JsonTypeInfo<CardChoiceMetric>)Options.GetTypeInfo(typeof(CardChoiceMetric)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<EncounterMetric> EncounterMetric => _EncounterMetric ?? (_EncounterMetric = (JsonTypeInfo<EncounterMetric>)Options.GetTypeInfo(typeof(EncounterMetric)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<EventChoiceMetric> EventChoiceMetric => _EventChoiceMetric ?? (_EventChoiceMetric = (JsonTypeInfo<EventChoiceMetric>)Options.GetTypeInfo(typeof(EventChoiceMetric)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<RunMetrics> RunMetrics => _RunMetrics ?? (_RunMetrics = (JsonTypeInfo<RunMetrics>)Options.GetTypeInfo(typeof(RunMetrics)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<SettingsDataMetric> SettingsDataMetric => _SettingsDataMetric ?? (_SettingsDataMetric = (JsonTypeInfo<SettingsDataMetric>)Options.GetTypeInfo(typeof(SettingsDataMetric)));

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
	public JsonTypeInfo<EpochMetric> EpochMetric => _EpochMetric ?? (_EpochMetric = (JsonTypeInfo<EpochMetric>)Options.GetTypeInfo(typeof(EpochMetric)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<IEnumerable<ModelId>> IEnumerableModelId => _IEnumerableModelId ?? (_IEnumerableModelId = (JsonTypeInfo<IEnumerable<ModelId>>)Options.GetTypeInfo(typeof(IEnumerable<ModelId>)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<List<ModelId>> ListModelId => _ListModelId ?? (_ListModelId = (JsonTypeInfo<List<ModelId>>)Options.GetTypeInfo(typeof(List<ModelId>)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<List<ActWinMetric>> ListActWinMetric => _ListActWinMetric ?? (_ListActWinMetric = (JsonTypeInfo<List<ActWinMetric>>)Options.GetTypeInfo(typeof(List<ActWinMetric>)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<List<AncientMetric>> ListAncientMetric => _ListAncientMetric ?? (_ListAncientMetric = (JsonTypeInfo<List<AncientMetric>>)Options.GetTypeInfo(typeof(List<AncientMetric>)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<List<CardChoiceMetric>> ListCardChoiceMetric => _ListCardChoiceMetric ?? (_ListCardChoiceMetric = (JsonTypeInfo<List<CardChoiceMetric>>)Options.GetTypeInfo(typeof(List<CardChoiceMetric>)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<List<EncounterMetric>> ListEncounterMetric => _ListEncounterMetric ?? (_ListEncounterMetric = (JsonTypeInfo<List<EncounterMetric>>)Options.GetTypeInfo(typeof(List<EncounterMetric>)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<List<EventChoiceMetric>> ListEventChoiceMetric => _ListEventChoiceMetric ?? (_ListEventChoiceMetric = (JsonTypeInfo<List<EventChoiceMetric>>)Options.GetTypeInfo(typeof(List<EventChoiceMetric>)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<List<string>> ListString => _ListString ?? (_ListString = (JsonTypeInfo<List<string>>)Options.GetTypeInfo(typeof(List<string>)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<int> Int32 => _Int32 ?? (_Int32 = (JsonTypeInfo<int>)Options.GetTypeInfo(typeof(int)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<long> Int64 => _Int64 ?? (_Int64 = (JsonTypeInfo<long>)Options.GetTypeInfo(typeof(long)));

	/// <summary>
	/// Defines the source generated JSON serialization contract metadata for a given type.
	/// </summary>
	public JsonTypeInfo<string> String => _String ?? (_String = (JsonTypeInfo<string>)Options.GetTypeInfo(typeof(string)));

	/// <summary>
	/// The default <see cref="T:System.Text.Json.Serialization.JsonSerializerContext" /> associated with a default <see cref="T:System.Text.Json.JsonSerializerOptions" /> instance.
	/// </summary>
	public static MetricsSerializerContext Default { get; } = new MetricsSerializerContext(new JsonSerializerOptions(s_defaultOptions));

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

	private JsonTypeInfo<float> Create_Single(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<float> jsonTypeInfo))
		{
			jsonTypeInfo = JsonMetadataServices.CreateValueInfo<float>(options, JsonMetadataServices.SingleConverter);
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
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
				SerializeHandler = Vector2ISerializeHandler
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

	private void Vector2ISerializeHandler(Utf8JsonWriter writer, Vector2I value)
	{
		writer.WriteStartObject();
		writer.WriteNumber(PropName_x, value.X);
		writer.WriteNumber(PropName_y, value.Y);
		writer.WriteEndObject();
	}

	private JsonTypeInfo<AchievementMetric> Create_AchievementMetric(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<AchievementMetric> jsonTypeInfo))
		{
			JsonObjectInfoValues<AchievementMetric> objectInfo = new JsonObjectInfoValues<AchievementMetric>
			{
				ObjectCreator = null,
				ObjectWithParameterizedConstructorCreator = (object[] args) => new AchievementMetric
				{
					BuildId = (string)args[0],
					Achievement = (string)args[1]
				},
				PropertyMetadataInitializer = (JsonSerializerContext _) => AchievementMetricPropInit(options),
				ConstructorParameterMetadataInitializer = AchievementMetricCtorParamInit,
				ConstructorAttributeProviderFactory = () => typeof(AchievementMetric).GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Array.Empty<Type>(), null),
				SerializeHandler = AchievementMetricSerializeHandler
			};
			jsonTypeInfo = JsonMetadataServices.CreateObjectInfo(options, objectInfo);
			jsonTypeInfo.NumberHandling = null;
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private static JsonPropertyInfo[] AchievementMetricPropInit(JsonSerializerOptions options)
	{
		JsonPropertyInfo[] array = new JsonPropertyInfo[5];
		JsonPropertyInfoValues<string> propertyInfo = new JsonPropertyInfoValues<string>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(AchievementMetric),
			Converter = null,
			Getter = (object obj) => ((AchievementMetric)obj).BuildId,
			Setter = (object obj, string? value) =>
			{
				((AchievementMetric)obj).BuildId = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "BuildId",
			JsonPropertyName = "buildId",
			AttributeProviderFactory = () => typeof(AchievementMetric).GetProperty("BuildId", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(string), Array.Empty<Type>(), null)
		};
		array[0] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo);
		array[0].IsRequired = true;
		array[0].IsGetNullable = false;
		array[0].IsSetNullable = false;
		JsonPropertyInfoValues<string> propertyInfo2 = new JsonPropertyInfoValues<string>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(AchievementMetric),
			Converter = null,
			Getter = (object obj) => ((AchievementMetric)obj).Achievement,
			Setter = (object obj, string? value) =>
			{
				((AchievementMetric)obj).Achievement = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "Achievement",
			JsonPropertyName = "achievement",
			AttributeProviderFactory = () => typeof(AchievementMetric).GetProperty("Achievement", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(string), Array.Empty<Type>(), null)
		};
		array[1] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo2);
		array[1].IsRequired = true;
		array[1].IsGetNullable = false;
		array[1].IsSetNullable = false;
		JsonPropertyInfoValues<int> propertyInfo3 = new JsonPropertyInfoValues<int>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(AchievementMetric),
			Converter = null,
			Getter = (object obj) => ((AchievementMetric)obj).TotalAchievements,
			Setter = (object obj, int value) =>
			{
				((AchievementMetric)obj).TotalAchievements = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "TotalAchievements",
			JsonPropertyName = "totalAchievements",
			AttributeProviderFactory = () => typeof(AchievementMetric).GetProperty("TotalAchievements", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(int), Array.Empty<Type>(), null)
		};
		array[2] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo3);
		JsonPropertyInfoValues<long> propertyInfo4 = new JsonPropertyInfoValues<long>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(AchievementMetric),
			Converter = null,
			Getter = (object obj) => ((AchievementMetric)obj).TotalPlaytime,
			Setter = (object obj, long value) =>
			{
				((AchievementMetric)obj).TotalPlaytime = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "TotalPlaytime",
			JsonPropertyName = "totalPlaytime",
			AttributeProviderFactory = () => typeof(AchievementMetric).GetProperty("TotalPlaytime", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(long), Array.Empty<Type>(), null)
		};
		array[3] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo4);
		JsonPropertyInfoValues<int> propertyInfo5 = new JsonPropertyInfoValues<int>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(AchievementMetric),
			Converter = null,
			Getter = (object obj) => ((AchievementMetric)obj).TotalRuns,
			Setter = (object obj, int value) =>
			{
				((AchievementMetric)obj).TotalRuns = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "TotalRuns",
			JsonPropertyName = "totalRuns",
			AttributeProviderFactory = () => typeof(AchievementMetric).GetProperty("TotalRuns", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(int), Array.Empty<Type>(), null)
		};
		array[4] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo5);
		return array;
	}

	private void AchievementMetricSerializeHandler(Utf8JsonWriter writer, AchievementMetric? value)
	{
		if (value == null)
		{
			writer.WriteNullValue();
			return;
		}
		writer.WriteStartObject();
		writer.WriteString(PropName_buildId, value.BuildId);
		writer.WriteString(PropName_achievement, value.Achievement);
		writer.WriteNumber(PropName_totalAchievements, value.TotalAchievements);
		writer.WriteNumber(PropName_totalPlaytime, value.TotalPlaytime);
		writer.WriteNumber(PropName_totalRuns, value.TotalRuns);
		writer.WriteEndObject();
	}

	private static JsonParameterInfoValues[] AchievementMetricCtorParamInit()
	{
		return new JsonParameterInfoValues[2]
		{
			new JsonParameterInfoValues
			{
				Name = "BuildId",
				ParameterType = typeof(string),
				Position = 0,
				IsNullable = false,
				IsMemberInitializer = true
			},
			new JsonParameterInfoValues
			{
				Name = "Achievement",
				ParameterType = typeof(string),
				Position = 1,
				IsNullable = false,
				IsMemberInitializer = true
			}
		};
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
				SerializeHandler = ModelIdSerializeHandler
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

	private void ModelIdSerializeHandler(Utf8JsonWriter writer, ModelId? value)
	{
		if ((object)value == null)
		{
			writer.WriteNullValue();
			return;
		}
		writer.WriteStartObject();
		writer.WriteString(PropName_category, value.Category);
		writer.WriteString(PropName_entry, value.Entry);
		writer.WriteEndObject();
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

	private JsonTypeInfo<ActWinMetric> Create_ActWinMetric(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<ActWinMetric> jsonTypeInfo))
		{
			JsonObjectInfoValues<ActWinMetric> objectInfo = new JsonObjectInfoValues<ActWinMetric>
			{
				ObjectCreator = () => default,
				ObjectWithParameterizedConstructorCreator = null,
				PropertyMetadataInitializer = (JsonSerializerContext _) => ActWinMetricPropInit(options),
				ConstructorParameterMetadataInitializer = null,
				ConstructorAttributeProviderFactory = () => typeof(ActWinMetric).GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Array.Empty<Type>(), null),
				SerializeHandler = ActWinMetricSerializeHandler
			};
			jsonTypeInfo = JsonMetadataServices.CreateObjectInfo(options, objectInfo);
			jsonTypeInfo.NumberHandling = null;
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private static JsonPropertyInfo[] ActWinMetricPropInit(JsonSerializerOptions options)
	{
		JsonPropertyInfo[] array = new JsonPropertyInfo[2];
		JsonPropertyInfoValues<string> propertyInfo = new JsonPropertyInfoValues<string>
		{
			IsProperty = false,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(ActWinMetric),
			Converter = null,
			Getter = (object obj) => ((ActWinMetric)obj).act,
			Setter = null,
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "act",
			JsonPropertyName = null,
			AttributeProviderFactory = () => typeof(ActWinMetric).GetField("act", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
		};
		array[0] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo);
		array[0].IsGetNullable = false;
		array[0].IsSetNullable = false;
		JsonPropertyInfoValues<bool> propertyInfo2 = new JsonPropertyInfoValues<bool>
		{
			IsProperty = false,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(ActWinMetric),
			Converter = null,
			Getter = (object obj) => ((ActWinMetric)obj).win,
			Setter = null,
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "win",
			JsonPropertyName = null,
			AttributeProviderFactory = () => typeof(ActWinMetric).GetField("win", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
		};
		array[1] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo2);
		return array;
	}

	private void ActWinMetricSerializeHandler(Utf8JsonWriter writer, ActWinMetric value)
	{
		writer.WriteStartObject();
		writer.WriteString(PropName_act, value.act);
		writer.WriteBoolean(PropName_win, value.win);
		writer.WriteEndObject();
	}

	private JsonTypeInfo<AncientMetric> Create_AncientMetric(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<AncientMetric> jsonTypeInfo))
		{
			JsonObjectInfoValues<AncientMetric> objectInfo = new JsonObjectInfoValues<AncientMetric>
			{
				ObjectCreator = () => default,
				ObjectWithParameterizedConstructorCreator = null,
				PropertyMetadataInitializer = (JsonSerializerContext _) => AncientMetricPropInit(options),
				ConstructorParameterMetadataInitializer = null,
				ConstructorAttributeProviderFactory = () => typeof(AncientMetric).GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Array.Empty<Type>(), null),
				SerializeHandler = AncientMetricSerializeHandler
			};
			jsonTypeInfo = JsonMetadataServices.CreateObjectInfo(options, objectInfo);
			jsonTypeInfo.NumberHandling = null;
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private static JsonPropertyInfo[] AncientMetricPropInit(JsonSerializerOptions options)
	{
		JsonPropertyInfo[] array = new JsonPropertyInfo[2];
		JsonPropertyInfoValues<string> propertyInfo = new JsonPropertyInfoValues<string>
		{
			IsProperty = false,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(AncientMetric),
			Converter = null,
			Getter = (object obj) => ((AncientMetric)obj).picked,
			Setter = null,
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "picked",
			JsonPropertyName = null,
			AttributeProviderFactory = () => typeof(AncientMetric).GetField("picked", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
		};
		array[0] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo);
		array[0].IsGetNullable = false;
		array[0].IsSetNullable = false;
		JsonPropertyInfoValues<List<string>> propertyInfo2 = new JsonPropertyInfoValues<List<string>>
		{
			IsProperty = false,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(AncientMetric),
			Converter = null,
			Getter = (object obj) => ((AncientMetric)obj).skipped,
			Setter = null,
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "skipped",
			JsonPropertyName = null,
			AttributeProviderFactory = () => typeof(AncientMetric).GetField("skipped", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
		};
		array[1] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo2);
		array[1].IsGetNullable = false;
		array[1].IsSetNullable = false;
		return array;
	}

	private void AncientMetricSerializeHandler(Utf8JsonWriter writer, AncientMetric value)
	{
		writer.WriteStartObject();
		writer.WriteString(PropName_picked, value.picked);
		writer.WritePropertyName(PropName_skipped);
		ListStringSerializeHandler(writer, value.skipped);
		writer.WriteEndObject();
	}

	private JsonTypeInfo<CardChoiceMetric> Create_CardChoiceMetric(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<CardChoiceMetric> jsonTypeInfo))
		{
			JsonObjectInfoValues<CardChoiceMetric> objectInfo = new JsonObjectInfoValues<CardChoiceMetric>
			{
				ObjectCreator = () => default,
				ObjectWithParameterizedConstructorCreator = null,
				PropertyMetadataInitializer = (JsonSerializerContext _) => CardChoiceMetricPropInit(options),
				ConstructorParameterMetadataInitializer = null,
				ConstructorAttributeProviderFactory = () => typeof(CardChoiceMetric).GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Array.Empty<Type>(), null),
				SerializeHandler = CardChoiceMetricSerializeHandler
			};
			jsonTypeInfo = JsonMetadataServices.CreateObjectInfo(options, objectInfo);
			jsonTypeInfo.NumberHandling = null;
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private static JsonPropertyInfo[] CardChoiceMetricPropInit(JsonSerializerOptions options)
	{
		JsonPropertyInfo[] array = new JsonPropertyInfo[2];
		JsonPropertyInfoValues<List<string>> propertyInfo = new JsonPropertyInfoValues<List<string>>
		{
			IsProperty = false,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(CardChoiceMetric),
			Converter = null,
			Getter = (object obj) => ((CardChoiceMetric)obj).picked,
			Setter = null,
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "picked",
			JsonPropertyName = null,
			AttributeProviderFactory = () => typeof(CardChoiceMetric).GetField("picked", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
		};
		array[0] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo);
		array[0].IsGetNullable = false;
		array[0].IsSetNullable = false;
		JsonPropertyInfoValues<List<string>> propertyInfo2 = new JsonPropertyInfoValues<List<string>>
		{
			IsProperty = false,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(CardChoiceMetric),
			Converter = null,
			Getter = (object obj) => ((CardChoiceMetric)obj).skipped,
			Setter = null,
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "skipped",
			JsonPropertyName = null,
			AttributeProviderFactory = () => typeof(CardChoiceMetric).GetField("skipped", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
		};
		array[1] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo2);
		array[1].IsGetNullable = false;
		array[1].IsSetNullable = false;
		return array;
	}

	private void CardChoiceMetricSerializeHandler(Utf8JsonWriter writer, CardChoiceMetric value)
	{
		writer.WriteStartObject();
		writer.WritePropertyName(PropName_picked);
		ListStringSerializeHandler(writer, value.picked);
		writer.WritePropertyName(PropName_skipped);
		ListStringSerializeHandler(writer, value.skipped);
		writer.WriteEndObject();
	}

	private JsonTypeInfo<EncounterMetric> Create_EncounterMetric(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<EncounterMetric> jsonTypeInfo))
		{
			JsonObjectInfoValues<EncounterMetric> objectInfo = new JsonObjectInfoValues<EncounterMetric>
			{
				ObjectCreator = () => default,
				ObjectWithParameterizedConstructorCreator = null,
				PropertyMetadataInitializer = (JsonSerializerContext _) => EncounterMetricPropInit(options),
				ConstructorParameterMetadataInitializer = null,
				ConstructorAttributeProviderFactory = () => typeof(EncounterMetric).GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Array.Empty<Type>(), null),
				SerializeHandler = EncounterMetricSerializeHandler
			};
			jsonTypeInfo = JsonMetadataServices.CreateObjectInfo(options, objectInfo);
			jsonTypeInfo.NumberHandling = null;
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private static JsonPropertyInfo[] EncounterMetricPropInit(JsonSerializerOptions options)
	{
		JsonPropertyInfo[] array = new JsonPropertyInfo[3];
		JsonPropertyInfoValues<string> propertyInfo = new JsonPropertyInfoValues<string>
		{
			IsProperty = false,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(EncounterMetric),
			Converter = null,
			Getter = (object obj) => ((EncounterMetric)obj).id,
			Setter = null,
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "id",
			JsonPropertyName = null,
			AttributeProviderFactory = () => typeof(EncounterMetric).GetField("id", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
		};
		array[0] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo);
		array[0].IsGetNullable = false;
		array[0].IsSetNullable = false;
		JsonPropertyInfoValues<int> propertyInfo2 = new JsonPropertyInfoValues<int>
		{
			IsProperty = false,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(EncounterMetric),
			Converter = null,
			Getter = (object obj) => ((EncounterMetric)obj).damage,
			Setter = null,
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "damage",
			JsonPropertyName = null,
			AttributeProviderFactory = () => typeof(EncounterMetric).GetField("damage", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
		};
		array[1] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo2);
		JsonPropertyInfoValues<int> propertyInfo3 = new JsonPropertyInfoValues<int>
		{
			IsProperty = false,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(EncounterMetric),
			Converter = null,
			Getter = (object obj) => ((EncounterMetric)obj).turns,
			Setter = null,
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "turns",
			JsonPropertyName = null,
			AttributeProviderFactory = () => typeof(EncounterMetric).GetField("turns", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
		};
		array[2] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo3);
		return array;
	}

	private void EncounterMetricSerializeHandler(Utf8JsonWriter writer, EncounterMetric value)
	{
		writer.WriteStartObject();
		writer.WriteString(PropName_id, value.id);
		writer.WriteNumber(PropName_damage, value.damage);
		writer.WriteNumber(PropName_turns, value.turns);
		writer.WriteEndObject();
	}

	private JsonTypeInfo<EventChoiceMetric> Create_EventChoiceMetric(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<EventChoiceMetric> jsonTypeInfo))
		{
			JsonObjectInfoValues<EventChoiceMetric> objectInfo = new JsonObjectInfoValues<EventChoiceMetric>
			{
				ObjectCreator = () => default,
				ObjectWithParameterizedConstructorCreator = null,
				PropertyMetadataInitializer = (JsonSerializerContext _) => EventChoiceMetricPropInit(options),
				ConstructorParameterMetadataInitializer = null,
				ConstructorAttributeProviderFactory = () => typeof(EventChoiceMetric).GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Array.Empty<Type>(), null),
				SerializeHandler = EventChoiceMetricSerializeHandler
			};
			jsonTypeInfo = JsonMetadataServices.CreateObjectInfo(options, objectInfo);
			jsonTypeInfo.NumberHandling = null;
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private static JsonPropertyInfo[] EventChoiceMetricPropInit(JsonSerializerOptions options)
	{
		JsonPropertyInfo[] array = new JsonPropertyInfo[3];
		JsonPropertyInfoValues<string> propertyInfo = new JsonPropertyInfoValues<string>
		{
			IsProperty = false,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(EventChoiceMetric),
			Converter = null,
			Getter = (object obj) => ((EventChoiceMetric)obj).id,
			Setter = null,
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "id",
			JsonPropertyName = null,
			AttributeProviderFactory = () => typeof(EventChoiceMetric).GetField("id", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
		};
		array[0] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo);
		array[0].IsGetNullable = false;
		array[0].IsSetNullable = false;
		JsonPropertyInfoValues<string> propertyInfo2 = new JsonPropertyInfoValues<string>
		{
			IsProperty = false,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(EventChoiceMetric),
			Converter = null,
			Getter = (object obj) => ((EventChoiceMetric)obj).act,
			Setter = null,
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "act",
			JsonPropertyName = null,
			AttributeProviderFactory = () => typeof(EventChoiceMetric).GetField("act", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
		};
		array[1] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo2);
		array[1].IsGetNullable = false;
		array[1].IsSetNullable = false;
		JsonPropertyInfoValues<string> propertyInfo3 = new JsonPropertyInfoValues<string>
		{
			IsProperty = false,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(EventChoiceMetric),
			Converter = null,
			Getter = (object obj) => ((EventChoiceMetric)obj).picked,
			Setter = null,
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "picked",
			JsonPropertyName = null,
			AttributeProviderFactory = () => typeof(EventChoiceMetric).GetField("picked", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
		};
		array[2] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo3);
		array[2].IsGetNullable = false;
		array[2].IsSetNullable = false;
		return array;
	}

	private void EventChoiceMetricSerializeHandler(Utf8JsonWriter writer, EventChoiceMetric value)
	{
		writer.WriteStartObject();
		writer.WriteString(PropName_id, value.id);
		writer.WriteString(PropName_act, value.act);
		writer.WriteString(PropName_picked, value.picked);
		writer.WriteEndObject();
	}

	private JsonTypeInfo<RunMetrics> Create_RunMetrics(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<RunMetrics> jsonTypeInfo))
		{
			JsonObjectInfoValues<RunMetrics> objectInfo = new JsonObjectInfoValues<RunMetrics>
			{
				ObjectCreator = null,
				ObjectWithParameterizedConstructorCreator = (object[] args) => new RunMetrics
				{
					BuildId = (string)args[0],
					PlayerId = (string)args[1],
					Character = (ModelId)args[2],
					Win = (bool)args[3],
					NumPlayers = (int)args[4],
					Team = (List<ModelId>)args[5],
					BuildType = (string)args[6],
					Ascension = (int)args[7],
					TotalPlaytime = (float)args[8],
					TotalWinRate = (float)args[9],
					NumReloads = (int)args[10],
					RunPlaytime = (float)args[11],
					FloorReached = (int)args[12],
					KilledByEncounter = (ModelId)args[13],
					CardChoices = (List<CardChoiceMetric>)args[14],
					CampfireUpgrades = (List<string>)args[15],
					EventChoices = (List<EventChoiceMetric>)args[16],
					AncientChoices = (List<AncientMetric>)args[17],
					RelicBuys = (List<string>)args[18],
					PotionBuys = (List<string>)args[19],
					ColorlessBuys = (List<string>)args[20],
					PotionDiscards = (List<string>)args[21],
					Encounters = (List<EncounterMetric>)args[22],
					ActWins = (List<ActWinMetric>)args[23],
					Deck = (IEnumerable<ModelId>)args[24],
					Relics = (IEnumerable<ModelId>)args[25]
				},
				PropertyMetadataInitializer = (JsonSerializerContext _) => RunMetricsPropInit(options),
				ConstructorParameterMetadataInitializer = RunMetricsCtorParamInit,
				ConstructorAttributeProviderFactory = () => typeof(RunMetrics).GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Array.Empty<Type>(), null),
				SerializeHandler = RunMetricsSerializeHandler
			};
			jsonTypeInfo = JsonMetadataServices.CreateObjectInfo(options, objectInfo);
			jsonTypeInfo.NumberHandling = null;
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private static JsonPropertyInfo[] RunMetricsPropInit(JsonSerializerOptions options)
	{
		JsonPropertyInfo[] array = new JsonPropertyInfo[26];
		JsonPropertyInfoValues<string> propertyInfo = new JsonPropertyInfoValues<string>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(RunMetrics),
			Converter = null,
			Getter = (object obj) => ((RunMetrics)obj).BuildId,
			Setter = (object obj, string? value) =>
			{
				throw new InvalidOperationException("Setting init-only properties is not supported in source generation mode.");
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "BuildId",
			JsonPropertyName = null,
			AttributeProviderFactory = () => typeof(RunMetrics).GetProperty("BuildId", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(string), Array.Empty<Type>(), null)
		};
		array[0] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo);
		array[0].IsRequired = true;
		array[0].IsGetNullable = false;
		array[0].IsSetNullable = false;
		JsonPropertyInfoValues<string> propertyInfo2 = new JsonPropertyInfoValues<string>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(RunMetrics),
			Converter = null,
			Getter = (object obj) => ((RunMetrics)obj).PlayerId,
			Setter = (object obj, string? value) =>
			{
				throw new InvalidOperationException("Setting init-only properties is not supported in source generation mode.");
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "PlayerId",
			JsonPropertyName = null,
			AttributeProviderFactory = () => typeof(RunMetrics).GetProperty("PlayerId", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(string), Array.Empty<Type>(), null)
		};
		array[1] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo2);
		array[1].IsRequired = true;
		array[1].IsGetNullable = false;
		array[1].IsSetNullable = false;
		JsonPropertyInfoValues<ModelId> propertyInfo3 = new JsonPropertyInfoValues<ModelId>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(RunMetrics),
			Converter = null,
			Getter = (object obj) => ((RunMetrics)obj).Character,
			Setter = (object obj, ModelId? value) =>
			{
				throw new InvalidOperationException("Setting init-only properties is not supported in source generation mode.");
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "Character",
			JsonPropertyName = null,
			AttributeProviderFactory = () => typeof(RunMetrics).GetProperty("Character", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(ModelId), Array.Empty<Type>(), null)
		};
		array[2] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo3);
		array[2].IsRequired = true;
		array[2].IsGetNullable = false;
		array[2].IsSetNullable = false;
		JsonPropertyInfoValues<bool> propertyInfo4 = new JsonPropertyInfoValues<bool>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(RunMetrics),
			Converter = null,
			Getter = (object obj) => ((RunMetrics)obj).Win,
			Setter = (object obj, bool value) =>
			{
				throw new InvalidOperationException("Setting init-only properties is not supported in source generation mode.");
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "Win",
			JsonPropertyName = null,
			AttributeProviderFactory = () => typeof(RunMetrics).GetProperty("Win", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(bool), Array.Empty<Type>(), null)
		};
		array[3] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo4);
		array[3].IsRequired = true;
		JsonPropertyInfoValues<int> propertyInfo5 = new JsonPropertyInfoValues<int>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(RunMetrics),
			Converter = null,
			Getter = (object obj) => ((RunMetrics)obj).NumPlayers,
			Setter = (object obj, int value) =>
			{
				throw new InvalidOperationException("Setting init-only properties is not supported in source generation mode.");
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "NumPlayers",
			JsonPropertyName = null,
			AttributeProviderFactory = () => typeof(RunMetrics).GetProperty("NumPlayers", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(int), Array.Empty<Type>(), null)
		};
		array[4] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo5);
		array[4].IsRequired = true;
		JsonPropertyInfoValues<List<ModelId>> propertyInfo6 = new JsonPropertyInfoValues<List<ModelId>>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(RunMetrics),
			Converter = null,
			Getter = (object obj) => ((RunMetrics)obj).Team,
			Setter = (object obj, List<ModelId>? value) =>
			{
				throw new InvalidOperationException("Setting init-only properties is not supported in source generation mode.");
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "Team",
			JsonPropertyName = null,
			AttributeProviderFactory = () => typeof(RunMetrics).GetProperty("Team", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(List<ModelId>), Array.Empty<Type>(), null)
		};
		array[5] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo6);
		array[5].IsRequired = true;
		array[5].IsGetNullable = false;
		array[5].IsSetNullable = false;
		JsonPropertyInfoValues<string> propertyInfo7 = new JsonPropertyInfoValues<string>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(RunMetrics),
			Converter = null,
			Getter = (object obj) => ((RunMetrics)obj).BuildType,
			Setter = (object obj, string? value) =>
			{
				throw new InvalidOperationException("Setting init-only properties is not supported in source generation mode.");
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "BuildType",
			JsonPropertyName = null,
			AttributeProviderFactory = () => typeof(RunMetrics).GetProperty("BuildType", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(string), Array.Empty<Type>(), null)
		};
		array[6] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo7);
		array[6].IsRequired = true;
		array[6].IsGetNullable = false;
		array[6].IsSetNullable = false;
		JsonPropertyInfoValues<int> propertyInfo8 = new JsonPropertyInfoValues<int>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(RunMetrics),
			Converter = null,
			Getter = (object obj) => ((RunMetrics)obj).Ascension,
			Setter = (object obj, int value) =>
			{
				throw new InvalidOperationException("Setting init-only properties is not supported in source generation mode.");
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "Ascension",
			JsonPropertyName = null,
			AttributeProviderFactory = () => typeof(RunMetrics).GetProperty("Ascension", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(int), Array.Empty<Type>(), null)
		};
		array[7] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo8);
		JsonPropertyInfoValues<float> propertyInfo9 = new JsonPropertyInfoValues<float>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(RunMetrics),
			Converter = null,
			Getter = (object obj) => ((RunMetrics)obj).TotalPlaytime,
			Setter = (object obj, float value) =>
			{
				throw new InvalidOperationException("Setting init-only properties is not supported in source generation mode.");
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "TotalPlaytime",
			JsonPropertyName = null,
			AttributeProviderFactory = () => typeof(RunMetrics).GetProperty("TotalPlaytime", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(float), Array.Empty<Type>(), null)
		};
		array[8] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo9);
		array[8].IsRequired = true;
		JsonPropertyInfoValues<float> propertyInfo10 = new JsonPropertyInfoValues<float>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(RunMetrics),
			Converter = null,
			Getter = (object obj) => ((RunMetrics)obj).TotalWinRate,
			Setter = (object obj, float value) =>
			{
				throw new InvalidOperationException("Setting init-only properties is not supported in source generation mode.");
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "TotalWinRate",
			JsonPropertyName = null,
			AttributeProviderFactory = () => typeof(RunMetrics).GetProperty("TotalWinRate", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(float), Array.Empty<Type>(), null)
		};
		array[9] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo10);
		array[9].IsRequired = true;
		JsonPropertyInfoValues<int> propertyInfo11 = new JsonPropertyInfoValues<int>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(RunMetrics),
			Converter = null,
			Getter = (object obj) => ((RunMetrics)obj).NumReloads,
			Setter = (object obj, int value) =>
			{
				throw new InvalidOperationException("Setting init-only properties is not supported in source generation mode.");
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "NumReloads",
			JsonPropertyName = null,
			AttributeProviderFactory = () => typeof(RunMetrics).GetProperty("NumReloads", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(int), Array.Empty<Type>(), null)
		};
		array[10] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo11);
		array[10].IsRequired = true;
		JsonPropertyInfoValues<float> propertyInfo12 = new JsonPropertyInfoValues<float>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(RunMetrics),
			Converter = null,
			Getter = (object obj) => ((RunMetrics)obj).RunPlaytime,
			Setter = (object obj, float value) =>
			{
				throw new InvalidOperationException("Setting init-only properties is not supported in source generation mode.");
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "RunPlaytime",
			JsonPropertyName = null,
			AttributeProviderFactory = () => typeof(RunMetrics).GetProperty("RunPlaytime", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(float), Array.Empty<Type>(), null)
		};
		array[11] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo12);
		array[11].IsRequired = true;
		JsonPropertyInfoValues<int> propertyInfo13 = new JsonPropertyInfoValues<int>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(RunMetrics),
			Converter = null,
			Getter = (object obj) => ((RunMetrics)obj).FloorReached,
			Setter = (object obj, int value) =>
			{
				throw new InvalidOperationException("Setting init-only properties is not supported in source generation mode.");
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "FloorReached",
			JsonPropertyName = null,
			AttributeProviderFactory = () => typeof(RunMetrics).GetProperty("FloorReached", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(int), Array.Empty<Type>(), null)
		};
		array[12] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo13);
		array[12].IsRequired = true;
		JsonPropertyInfoValues<ModelId> propertyInfo14 = new JsonPropertyInfoValues<ModelId>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(RunMetrics),
			Converter = null,
			Getter = (object obj) => ((RunMetrics)obj).KilledByEncounter,
			Setter = (object obj, ModelId? value) =>
			{
				throw new InvalidOperationException("Setting init-only properties is not supported in source generation mode.");
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "KilledByEncounter",
			JsonPropertyName = null,
			AttributeProviderFactory = () => typeof(RunMetrics).GetProperty("KilledByEncounter", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(ModelId), Array.Empty<Type>(), null)
		};
		array[13] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo14);
		array[13].IsRequired = true;
		array[13].IsGetNullable = false;
		array[13].IsSetNullable = false;
		JsonPropertyInfoValues<List<CardChoiceMetric>> propertyInfo15 = new JsonPropertyInfoValues<List<CardChoiceMetric>>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(RunMetrics),
			Converter = null,
			Getter = (object obj) => ((RunMetrics)obj).CardChoices,
			Setter = (object obj, List<CardChoiceMetric>? value) =>
			{
				throw new InvalidOperationException("Setting init-only properties is not supported in source generation mode.");
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "CardChoices",
			JsonPropertyName = null,
			AttributeProviderFactory = () => typeof(RunMetrics).GetProperty("CardChoices", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(List<CardChoiceMetric>), Array.Empty<Type>(), null)
		};
		array[14] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo15);
		array[14].IsRequired = true;
		array[14].IsGetNullable = false;
		array[14].IsSetNullable = false;
		JsonPropertyInfoValues<List<string>> propertyInfo16 = new JsonPropertyInfoValues<List<string>>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(RunMetrics),
			Converter = null,
			Getter = (object obj) => ((RunMetrics)obj).CampfireUpgrades,
			Setter = (object obj, List<string>? value) =>
			{
				throw new InvalidOperationException("Setting init-only properties is not supported in source generation mode.");
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "CampfireUpgrades",
			JsonPropertyName = null,
			AttributeProviderFactory = () => typeof(RunMetrics).GetProperty("CampfireUpgrades", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(List<string>), Array.Empty<Type>(), null)
		};
		array[15] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo16);
		array[15].IsRequired = true;
		array[15].IsGetNullable = false;
		array[15].IsSetNullable = false;
		JsonPropertyInfoValues<List<EventChoiceMetric>> propertyInfo17 = new JsonPropertyInfoValues<List<EventChoiceMetric>>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(RunMetrics),
			Converter = null,
			Getter = (object obj) => ((RunMetrics)obj).EventChoices,
			Setter = (object obj, List<EventChoiceMetric>? value) =>
			{
				throw new InvalidOperationException("Setting init-only properties is not supported in source generation mode.");
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "EventChoices",
			JsonPropertyName = null,
			AttributeProviderFactory = () => typeof(RunMetrics).GetProperty("EventChoices", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(List<EventChoiceMetric>), Array.Empty<Type>(), null)
		};
		array[16] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo17);
		array[16].IsRequired = true;
		array[16].IsGetNullable = false;
		array[16].IsSetNullable = false;
		JsonPropertyInfoValues<List<AncientMetric>> propertyInfo18 = new JsonPropertyInfoValues<List<AncientMetric>>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(RunMetrics),
			Converter = null,
			Getter = (object obj) => ((RunMetrics)obj).AncientChoices,
			Setter = (object obj, List<AncientMetric>? value) =>
			{
				throw new InvalidOperationException("Setting init-only properties is not supported in source generation mode.");
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "AncientChoices",
			JsonPropertyName = null,
			AttributeProviderFactory = () => typeof(RunMetrics).GetProperty("AncientChoices", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(List<AncientMetric>), Array.Empty<Type>(), null)
		};
		array[17] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo18);
		array[17].IsRequired = true;
		array[17].IsGetNullable = false;
		array[17].IsSetNullable = false;
		JsonPropertyInfoValues<List<string>> propertyInfo19 = new JsonPropertyInfoValues<List<string>>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(RunMetrics),
			Converter = null,
			Getter = (object obj) => ((RunMetrics)obj).RelicBuys,
			Setter = (object obj, List<string>? value) =>
			{
				throw new InvalidOperationException("Setting init-only properties is not supported in source generation mode.");
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "RelicBuys",
			JsonPropertyName = null,
			AttributeProviderFactory = () => typeof(RunMetrics).GetProperty("RelicBuys", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(List<string>), Array.Empty<Type>(), null)
		};
		array[18] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo19);
		array[18].IsRequired = true;
		array[18].IsGetNullable = false;
		array[18].IsSetNullable = false;
		JsonPropertyInfoValues<List<string>> propertyInfo20 = new JsonPropertyInfoValues<List<string>>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(RunMetrics),
			Converter = null,
			Getter = (object obj) => ((RunMetrics)obj).PotionBuys,
			Setter = (object obj, List<string>? value) =>
			{
				throw new InvalidOperationException("Setting init-only properties is not supported in source generation mode.");
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "PotionBuys",
			JsonPropertyName = null,
			AttributeProviderFactory = () => typeof(RunMetrics).GetProperty("PotionBuys", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(List<string>), Array.Empty<Type>(), null)
		};
		array[19] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo20);
		array[19].IsRequired = true;
		array[19].IsGetNullable = false;
		array[19].IsSetNullable = false;
		JsonPropertyInfoValues<List<string>> propertyInfo21 = new JsonPropertyInfoValues<List<string>>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(RunMetrics),
			Converter = null,
			Getter = (object obj) => ((RunMetrics)obj).ColorlessBuys,
			Setter = (object obj, List<string>? value) =>
			{
				throw new InvalidOperationException("Setting init-only properties is not supported in source generation mode.");
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "ColorlessBuys",
			JsonPropertyName = null,
			AttributeProviderFactory = () => typeof(RunMetrics).GetProperty("ColorlessBuys", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(List<string>), Array.Empty<Type>(), null)
		};
		array[20] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo21);
		array[20].IsRequired = true;
		array[20].IsGetNullable = false;
		array[20].IsSetNullable = false;
		JsonPropertyInfoValues<List<string>> propertyInfo22 = new JsonPropertyInfoValues<List<string>>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(RunMetrics),
			Converter = null,
			Getter = (object obj) => ((RunMetrics)obj).PotionDiscards,
			Setter = (object obj, List<string>? value) =>
			{
				throw new InvalidOperationException("Setting init-only properties is not supported in source generation mode.");
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "PotionDiscards",
			JsonPropertyName = null,
			AttributeProviderFactory = () => typeof(RunMetrics).GetProperty("PotionDiscards", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(List<string>), Array.Empty<Type>(), null)
		};
		array[21] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo22);
		array[21].IsRequired = true;
		array[21].IsGetNullable = false;
		array[21].IsSetNullable = false;
		JsonPropertyInfoValues<List<EncounterMetric>> propertyInfo23 = new JsonPropertyInfoValues<List<EncounterMetric>>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(RunMetrics),
			Converter = null,
			Getter = (object obj) => ((RunMetrics)obj).Encounters,
			Setter = (object obj, List<EncounterMetric>? value) =>
			{
				throw new InvalidOperationException("Setting init-only properties is not supported in source generation mode.");
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "Encounters",
			JsonPropertyName = null,
			AttributeProviderFactory = () => typeof(RunMetrics).GetProperty("Encounters", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(List<EncounterMetric>), Array.Empty<Type>(), null)
		};
		array[22] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo23);
		array[22].IsRequired = true;
		array[22].IsGetNullable = false;
		array[22].IsSetNullable = false;
		JsonPropertyInfoValues<List<ActWinMetric>> propertyInfo24 = new JsonPropertyInfoValues<List<ActWinMetric>>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(RunMetrics),
			Converter = null,
			Getter = (object obj) => ((RunMetrics)obj).ActWins,
			Setter = (object obj, List<ActWinMetric>? value) =>
			{
				throw new InvalidOperationException("Setting init-only properties is not supported in source generation mode.");
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "ActWins",
			JsonPropertyName = null,
			AttributeProviderFactory = () => typeof(RunMetrics).GetProperty("ActWins", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(List<ActWinMetric>), Array.Empty<Type>(), null)
		};
		array[23] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo24);
		array[23].IsRequired = true;
		array[23].IsGetNullable = false;
		array[23].IsSetNullable = false;
		JsonPropertyInfoValues<IEnumerable<ModelId>> propertyInfo25 = new JsonPropertyInfoValues<IEnumerable<ModelId>>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(RunMetrics),
			Converter = null,
			Getter = (object obj) => ((RunMetrics)obj).Deck,
			Setter = (object obj, IEnumerable<ModelId>? value) =>
			{
				throw new InvalidOperationException("Setting init-only properties is not supported in source generation mode.");
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "Deck",
			JsonPropertyName = null,
			AttributeProviderFactory = () => typeof(RunMetrics).GetProperty("Deck", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(IEnumerable<ModelId>), Array.Empty<Type>(), null)
		};
		array[24] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo25);
		array[24].IsRequired = true;
		array[24].IsGetNullable = false;
		array[24].IsSetNullable = false;
		JsonPropertyInfoValues<IEnumerable<ModelId>> propertyInfo26 = new JsonPropertyInfoValues<IEnumerable<ModelId>>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(RunMetrics),
			Converter = null,
			Getter = (object obj) => ((RunMetrics)obj).Relics,
			Setter = (object obj, IEnumerable<ModelId>? value) =>
			{
				throw new InvalidOperationException("Setting init-only properties is not supported in source generation mode.");
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "Relics",
			JsonPropertyName = null,
			AttributeProviderFactory = () => typeof(RunMetrics).GetProperty("Relics", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(IEnumerable<ModelId>), Array.Empty<Type>(), null)
		};
		array[25] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo26);
		array[25].IsRequired = true;
		array[25].IsGetNullable = false;
		array[25].IsSetNullable = false;
		return array;
	}

	private void RunMetricsSerializeHandler(Utf8JsonWriter writer, RunMetrics? value)
	{
		if (value == null)
		{
			writer.WriteNullValue();
			return;
		}
		writer.WriteStartObject();
		writer.WriteString(PropName_buildId, value.BuildId);
		writer.WriteString(PropName_playerId, value.PlayerId);
		writer.WritePropertyName(PropName_character);
		ModelIdSerializeHandler(writer, value.Character);
		writer.WriteBoolean(PropName_win, value.Win);
		writer.WriteNumber(PropName_numPlayers, value.NumPlayers);
		writer.WritePropertyName(PropName_team);
		ListModelIdSerializeHandler(writer, value.Team);
		writer.WriteString(PropName_buildType, value.BuildType);
		writer.WriteNumber(PropName_ascension, value.Ascension);
		writer.WriteNumber(PropName_totalPlaytime, value.TotalPlaytime);
		writer.WriteNumber(PropName_totalWinRate, value.TotalWinRate);
		writer.WriteNumber(PropName_numReloads, value.NumReloads);
		writer.WriteNumber(PropName_runPlaytime, value.RunPlaytime);
		writer.WriteNumber(PropName_floorReached, value.FloorReached);
		writer.WritePropertyName(PropName_killedByEncounter);
		ModelIdSerializeHandler(writer, value.KilledByEncounter);
		writer.WritePropertyName(PropName_cardChoices);
		ListCardChoiceMetricSerializeHandler(writer, value.CardChoices);
		writer.WritePropertyName(PropName_campfireUpgrades);
		ListStringSerializeHandler(writer, value.CampfireUpgrades);
		writer.WritePropertyName(PropName_eventChoices);
		ListEventChoiceMetricSerializeHandler(writer, value.EventChoices);
		writer.WritePropertyName(PropName_ancientChoices);
		ListAncientMetricSerializeHandler(writer, value.AncientChoices);
		writer.WritePropertyName(PropName_relicBuys);
		ListStringSerializeHandler(writer, value.RelicBuys);
		writer.WritePropertyName(PropName_potionBuys);
		ListStringSerializeHandler(writer, value.PotionBuys);
		writer.WritePropertyName(PropName_colorlessBuys);
		ListStringSerializeHandler(writer, value.ColorlessBuys);
		writer.WritePropertyName(PropName_potionDiscards);
		ListStringSerializeHandler(writer, value.PotionDiscards);
		writer.WritePropertyName(PropName_encounters);
		ListEncounterMetricSerializeHandler(writer, value.Encounters);
		writer.WritePropertyName(PropName_actWins);
		ListActWinMetricSerializeHandler(writer, value.ActWins);
		writer.WritePropertyName(PropName_deck);
		IEnumerableModelIdSerializeHandler(writer, value.Deck);
		writer.WritePropertyName(PropName_relics);
		IEnumerableModelIdSerializeHandler(writer, value.Relics);
		writer.WriteEndObject();
	}

	private static JsonParameterInfoValues[] RunMetricsCtorParamInit()
	{
		return new JsonParameterInfoValues[26]
		{
			new JsonParameterInfoValues
			{
				Name = "BuildId",
				ParameterType = typeof(string),
				Position = 0,
				IsNullable = false,
				IsMemberInitializer = true
			},
			new JsonParameterInfoValues
			{
				Name = "PlayerId",
				ParameterType = typeof(string),
				Position = 1,
				IsNullable = false,
				IsMemberInitializer = true
			},
			new JsonParameterInfoValues
			{
				Name = "Character",
				ParameterType = typeof(ModelId),
				Position = 2,
				IsNullable = false,
				IsMemberInitializer = true
			},
			new JsonParameterInfoValues
			{
				Name = "Win",
				ParameterType = typeof(bool),
				Position = 3,
				IsNullable = false,
				IsMemberInitializer = true
			},
			new JsonParameterInfoValues
			{
				Name = "NumPlayers",
				ParameterType = typeof(int),
				Position = 4,
				IsNullable = false,
				IsMemberInitializer = true
			},
			new JsonParameterInfoValues
			{
				Name = "Team",
				ParameterType = typeof(List<ModelId>),
				Position = 5,
				IsNullable = false,
				IsMemberInitializer = true
			},
			new JsonParameterInfoValues
			{
				Name = "BuildType",
				ParameterType = typeof(string),
				Position = 6,
				IsNullable = false,
				IsMemberInitializer = true
			},
			new JsonParameterInfoValues
			{
				Name = "Ascension",
				ParameterType = typeof(int),
				Position = 7,
				IsNullable = false,
				IsMemberInitializer = true
			},
			new JsonParameterInfoValues
			{
				Name = "TotalPlaytime",
				ParameterType = typeof(float),
				Position = 8,
				IsNullable = false,
				IsMemberInitializer = true
			},
			new JsonParameterInfoValues
			{
				Name = "TotalWinRate",
				ParameterType = typeof(float),
				Position = 9,
				IsNullable = false,
				IsMemberInitializer = true
			},
			new JsonParameterInfoValues
			{
				Name = "NumReloads",
				ParameterType = typeof(int),
				Position = 10,
				IsNullable = false,
				IsMemberInitializer = true
			},
			new JsonParameterInfoValues
			{
				Name = "RunPlaytime",
				ParameterType = typeof(float),
				Position = 11,
				IsNullable = false,
				IsMemberInitializer = true
			},
			new JsonParameterInfoValues
			{
				Name = "FloorReached",
				ParameterType = typeof(int),
				Position = 12,
				IsNullable = false,
				IsMemberInitializer = true
			},
			new JsonParameterInfoValues
			{
				Name = "KilledByEncounter",
				ParameterType = typeof(ModelId),
				Position = 13,
				IsNullable = false,
				IsMemberInitializer = true
			},
			new JsonParameterInfoValues
			{
				Name = "CardChoices",
				ParameterType = typeof(List<CardChoiceMetric>),
				Position = 14,
				IsNullable = false,
				IsMemberInitializer = true
			},
			new JsonParameterInfoValues
			{
				Name = "CampfireUpgrades",
				ParameterType = typeof(List<string>),
				Position = 15,
				IsNullable = false,
				IsMemberInitializer = true
			},
			new JsonParameterInfoValues
			{
				Name = "EventChoices",
				ParameterType = typeof(List<EventChoiceMetric>),
				Position = 16,
				IsNullable = false,
				IsMemberInitializer = true
			},
			new JsonParameterInfoValues
			{
				Name = "AncientChoices",
				ParameterType = typeof(List<AncientMetric>),
				Position = 17,
				IsNullable = false,
				IsMemberInitializer = true
			},
			new JsonParameterInfoValues
			{
				Name = "RelicBuys",
				ParameterType = typeof(List<string>),
				Position = 18,
				IsNullable = false,
				IsMemberInitializer = true
			},
			new JsonParameterInfoValues
			{
				Name = "PotionBuys",
				ParameterType = typeof(List<string>),
				Position = 19,
				IsNullable = false,
				IsMemberInitializer = true
			},
			new JsonParameterInfoValues
			{
				Name = "ColorlessBuys",
				ParameterType = typeof(List<string>),
				Position = 20,
				IsNullable = false,
				IsMemberInitializer = true
			},
			new JsonParameterInfoValues
			{
				Name = "PotionDiscards",
				ParameterType = typeof(List<string>),
				Position = 21,
				IsNullable = false,
				IsMemberInitializer = true
			},
			new JsonParameterInfoValues
			{
				Name = "Encounters",
				ParameterType = typeof(List<EncounterMetric>),
				Position = 22,
				IsNullable = false,
				IsMemberInitializer = true
			},
			new JsonParameterInfoValues
			{
				Name = "ActWins",
				ParameterType = typeof(List<ActWinMetric>),
				Position = 23,
				IsNullable = false,
				IsMemberInitializer = true
			},
			new JsonParameterInfoValues
			{
				Name = "Deck",
				ParameterType = typeof(IEnumerable<ModelId>),
				Position = 24,
				IsNullable = false,
				IsMemberInitializer = true
			},
			new JsonParameterInfoValues
			{
				Name = "Relics",
				ParameterType = typeof(IEnumerable<ModelId>),
				Position = 25,
				IsNullable = false,
				IsMemberInitializer = true
			}
		};
	}

	private JsonTypeInfo<SettingsDataMetric> Create_SettingsDataMetric(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<SettingsDataMetric> jsonTypeInfo))
		{
			JsonObjectInfoValues<SettingsDataMetric> objectInfo = new JsonObjectInfoValues<SettingsDataMetric>
			{
				ObjectCreator = null,
				ObjectWithParameterizedConstructorCreator = (object[] args) => new SettingsDataMetric
				{
					BuildId = (string)args[0],
					FastModeType = (FastModeType)args[1]
				},
				PropertyMetadataInitializer = (JsonSerializerContext _) => SettingsDataMetricPropInit(options),
				ConstructorParameterMetadataInitializer = SettingsDataMetricCtorParamInit,
				ConstructorAttributeProviderFactory = () => typeof(SettingsDataMetric).GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Array.Empty<Type>(), null),
				SerializeHandler = SettingsDataMetricSerializeHandler
			};
			jsonTypeInfo = JsonMetadataServices.CreateObjectInfo(options, objectInfo);
			jsonTypeInfo.NumberHandling = null;
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private static JsonPropertyInfo[] SettingsDataMetricPropInit(JsonSerializerOptions options)
	{
		JsonPropertyInfo[] array = new JsonPropertyInfo[18];
		JsonPropertyInfoValues<string> propertyInfo = new JsonPropertyInfoValues<string>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SettingsDataMetric),
			Converter = null,
			Getter = (object obj) => ((SettingsDataMetric)obj).BuildId,
			Setter = (object obj, string? value) =>
			{
				Unsafe.Unbox<SettingsDataMetric>(obj).BuildId = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "BuildId",
			JsonPropertyName = "buildId",
			AttributeProviderFactory = () => typeof(SettingsDataMetric).GetProperty("BuildId", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(string), Array.Empty<Type>(), null)
		};
		array[0] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo);
		array[0].IsRequired = true;
		array[0].IsGetNullable = false;
		array[0].IsSetNullable = false;
		JsonPropertyInfoValues<string> propertyInfo2 = new JsonPropertyInfoValues<string>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SettingsDataMetric),
			Converter = null,
			Getter = (object obj) => ((SettingsDataMetric)obj).Os,
			Setter = (object obj, string? value) =>
			{
				Unsafe.Unbox<SettingsDataMetric>(obj).Os = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "Os",
			JsonPropertyName = "os",
			AttributeProviderFactory = () => typeof(SettingsDataMetric).GetProperty("Os", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(string), Array.Empty<Type>(), null)
		};
		array[1] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo2);
		array[1].IsGetNullable = false;
		array[1].IsSetNullable = false;
		JsonPropertyInfoValues<string> propertyInfo3 = new JsonPropertyInfoValues<string>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SettingsDataMetric),
			Converter = null,
			Getter = (object obj) => ((SettingsDataMetric)obj).Platform,
			Setter = (object obj, string? value) =>
			{
				Unsafe.Unbox<SettingsDataMetric>(obj).Platform = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "Platform",
			JsonPropertyName = "platform",
			AttributeProviderFactory = () => typeof(SettingsDataMetric).GetProperty("Platform", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(string), Array.Empty<Type>(), null)
		};
		array[2] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo3);
		array[2].IsGetNullable = false;
		array[2].IsSetNullable = false;
		JsonPropertyInfoValues<int> propertyInfo4 = new JsonPropertyInfoValues<int>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SettingsDataMetric),
			Converter = null,
			Getter = (object obj) => ((SettingsDataMetric)obj).SystemRam,
			Setter = (object obj, int value) =>
			{
				Unsafe.Unbox<SettingsDataMetric>(obj).SystemRam = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "SystemRam",
			JsonPropertyName = "systemRam",
			AttributeProviderFactory = () => typeof(SettingsDataMetric).GetProperty("SystemRam", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(int), Array.Empty<Type>(), null)
		};
		array[3] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo4);
		JsonPropertyInfoValues<string> propertyInfo5 = new JsonPropertyInfoValues<string>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SettingsDataMetric),
			Converter = null,
			Getter = (object obj) => ((SettingsDataMetric)obj).LanguageCode,
			Setter = (object obj, string? value) =>
			{
				Unsafe.Unbox<SettingsDataMetric>(obj).LanguageCode = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "LanguageCode",
			JsonPropertyName = "language",
			AttributeProviderFactory = () => typeof(SettingsDataMetric).GetProperty("LanguageCode", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(string), Array.Empty<Type>(), null)
		};
		array[4] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo5);
		array[4].IsGetNullable = false;
		array[4].IsSetNullable = false;
		JsonPropertyInfoValues<FastModeType> propertyInfo6 = new JsonPropertyInfoValues<FastModeType>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SettingsDataMetric),
			Converter = null,
			Getter = (object obj) => ((SettingsDataMetric)obj).FastModeType,
			Setter = (object obj, FastModeType value) =>
			{
				Unsafe.Unbox<SettingsDataMetric>(obj).FastModeType = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "FastModeType",
			JsonPropertyName = "combatSpeed",
			AttributeProviderFactory = () => typeof(SettingsDataMetric).GetProperty("FastModeType", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(FastModeType), Array.Empty<Type>(), null)
		};
		array[5] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo6);
		array[5].IsRequired = true;
		JsonPropertyInfoValues<int> propertyInfo7 = new JsonPropertyInfoValues<int>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SettingsDataMetric),
			Converter = null,
			Getter = (object obj) => ((SettingsDataMetric)obj).Screenshake,
			Setter = (object obj, int value) =>
			{
				Unsafe.Unbox<SettingsDataMetric>(obj).Screenshake = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "Screenshake",
			JsonPropertyName = "screenshake",
			AttributeProviderFactory = () => typeof(SettingsDataMetric).GetProperty("Screenshake", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(int), Array.Empty<Type>(), null)
		};
		array[6] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo7);
		JsonPropertyInfoValues<bool> propertyInfo8 = new JsonPropertyInfoValues<bool>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SettingsDataMetric),
			Converter = null,
			Getter = (object obj) => ((SettingsDataMetric)obj).ShowRunTimer,
			Setter = (object obj, bool value) =>
			{
				Unsafe.Unbox<SettingsDataMetric>(obj).ShowRunTimer = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "ShowRunTimer",
			JsonPropertyName = "runTimer",
			AttributeProviderFactory = () => typeof(SettingsDataMetric).GetProperty("ShowRunTimer", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(bool), Array.Empty<Type>(), null)
		};
		array[7] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo8);
		JsonPropertyInfoValues<bool> propertyInfo9 = new JsonPropertyInfoValues<bool>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SettingsDataMetric),
			Converter = null,
			Getter = (object obj) => ((SettingsDataMetric)obj).PhobiaMode,
			Setter = (object obj, bool value) =>
			{
				Unsafe.Unbox<SettingsDataMetric>(obj).PhobiaMode = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "PhobiaMode",
			JsonPropertyName = "phobiaMode",
			AttributeProviderFactory = () => typeof(SettingsDataMetric).GetProperty("PhobiaMode", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(bool), Array.Empty<Type>(), null)
		};
		array[8] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo9);
		JsonPropertyInfoValues<bool> propertyInfo10 = new JsonPropertyInfoValues<bool>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SettingsDataMetric),
			Converter = null,
			Getter = (object obj) => ((SettingsDataMetric)obj).ShowCardIndices,
			Setter = (object obj, bool value) =>
			{
				Unsafe.Unbox<SettingsDataMetric>(obj).ShowCardIndices = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "ShowCardIndices",
			JsonPropertyName = "cardIndices",
			AttributeProviderFactory = () => typeof(SettingsDataMetric).GetProperty("ShowCardIndices", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(bool), Array.Empty<Type>(), null)
		};
		array[9] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo10);
		JsonPropertyInfoValues<int> propertyInfo11 = new JsonPropertyInfoValues<int>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SettingsDataMetric),
			Converter = null,
			Getter = (object obj) => ((SettingsDataMetric)obj).DisplayCount,
			Setter = (object obj, int value) =>
			{
				Unsafe.Unbox<SettingsDataMetric>(obj).DisplayCount = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "DisplayCount",
			JsonPropertyName = "displayCount",
			AttributeProviderFactory = () => typeof(SettingsDataMetric).GetProperty("DisplayCount", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(int), Array.Empty<Type>(), null)
		};
		array[10] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo11);
		JsonPropertyInfoValues<Vector2I> propertyInfo12 = new JsonPropertyInfoValues<Vector2I>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SettingsDataMetric),
			Converter = null,
			Getter = (object obj) => ((SettingsDataMetric)obj).DisplayResolution,
			Setter = (object obj, Vector2I value) =>
			{
				Unsafe.Unbox<SettingsDataMetric>(obj).DisplayResolution = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "DisplayResolution",
			JsonPropertyName = "displayResolution",
			AttributeProviderFactory = () => typeof(SettingsDataMetric).GetProperty("DisplayResolution", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(Vector2I), Array.Empty<Type>(), null)
		};
		array[11] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo12);
		JsonPropertyInfoValues<bool> propertyInfo13 = new JsonPropertyInfoValues<bool>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SettingsDataMetric),
			Converter = null,
			Getter = (object obj) => ((SettingsDataMetric)obj).Fullscreen,
			Setter = (object obj, bool value) =>
			{
				Unsafe.Unbox<SettingsDataMetric>(obj).Fullscreen = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "Fullscreen",
			JsonPropertyName = "fullscreen",
			AttributeProviderFactory = () => typeof(SettingsDataMetric).GetProperty("Fullscreen", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(bool), Array.Empty<Type>(), null)
		};
		array[12] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo13);
		JsonPropertyInfoValues<AspectRatioSetting> propertyInfo14 = new JsonPropertyInfoValues<AspectRatioSetting>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SettingsDataMetric),
			Converter = null,
			Getter = (object obj) => ((SettingsDataMetric)obj).AspectRatio,
			Setter = (object obj, AspectRatioSetting value) =>
			{
				Unsafe.Unbox<SettingsDataMetric>(obj).AspectRatio = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "AspectRatio",
			JsonPropertyName = "aspectRatio",
			AttributeProviderFactory = () => typeof(SettingsDataMetric).GetProperty("AspectRatio", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(AspectRatioSetting), Array.Empty<Type>(), null)
		};
		array[13] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo14);
		JsonPropertyInfoValues<bool> propertyInfo15 = new JsonPropertyInfoValues<bool>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SettingsDataMetric),
			Converter = null,
			Getter = (object obj) => ((SettingsDataMetric)obj).ResizeWindows,
			Setter = (object obj, bool value) =>
			{
				Unsafe.Unbox<SettingsDataMetric>(obj).ResizeWindows = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "ResizeWindows",
			JsonPropertyName = "resizeWindows",
			AttributeProviderFactory = () => typeof(SettingsDataMetric).GetProperty("ResizeWindows", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(bool), Array.Empty<Type>(), null)
		};
		array[14] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo15);
		JsonPropertyInfoValues<VSyncType> propertyInfo16 = new JsonPropertyInfoValues<VSyncType>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SettingsDataMetric),
			Converter = null,
			Getter = (object obj) => ((SettingsDataMetric)obj).VSync,
			Setter = (object obj, VSyncType value) =>
			{
				Unsafe.Unbox<SettingsDataMetric>(obj).VSync = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "VSync",
			JsonPropertyName = "vSync",
			AttributeProviderFactory = () => typeof(SettingsDataMetric).GetProperty("VSync", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(VSyncType), Array.Empty<Type>(), null)
		};
		array[15] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo16);
		JsonPropertyInfoValues<int> propertyInfo17 = new JsonPropertyInfoValues<int>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SettingsDataMetric),
			Converter = null,
			Getter = (object obj) => ((SettingsDataMetric)obj).FpsLimit,
			Setter = (object obj, int value) =>
			{
				Unsafe.Unbox<SettingsDataMetric>(obj).FpsLimit = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "FpsLimit",
			JsonPropertyName = "fpsLimit",
			AttributeProviderFactory = () => typeof(SettingsDataMetric).GetProperty("FpsLimit", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(int), Array.Empty<Type>(), null)
		};
		array[16] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo17);
		JsonPropertyInfoValues<int> propertyInfo18 = new JsonPropertyInfoValues<int>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(SettingsDataMetric),
			Converter = null,
			Getter = (object obj) => ((SettingsDataMetric)obj).Msaa,
			Setter = (object obj, int value) =>
			{
				Unsafe.Unbox<SettingsDataMetric>(obj).Msaa = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "Msaa",
			JsonPropertyName = "msaa",
			AttributeProviderFactory = () => typeof(SettingsDataMetric).GetProperty("Msaa", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(int), Array.Empty<Type>(), null)
		};
		array[17] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo18);
		return array;
	}

	private void SettingsDataMetricSerializeHandler(Utf8JsonWriter writer, SettingsDataMetric value)
	{
		writer.WriteStartObject();
		JsonEncodedText propName_buildId = PropName_buildId;
		SettingsDataMetric settingsDataMetric = value;
		writer.WriteString(propName_buildId, settingsDataMetric.BuildId);
		JsonEncodedText propName_os = PropName_os;
		settingsDataMetric = value;
		writer.WriteString(propName_os, settingsDataMetric.Os);
		JsonEncodedText propName_platform = PropName_platform;
		settingsDataMetric = value;
		writer.WriteString(propName_platform, settingsDataMetric.Platform);
		JsonEncodedText propName_systemRam = PropName_systemRam;
		settingsDataMetric = value;
		writer.WriteNumber(propName_systemRam, settingsDataMetric.SystemRam);
		JsonEncodedText propName_language = PropName_language;
		settingsDataMetric = value;
		writer.WriteString(propName_language, settingsDataMetric.LanguageCode);
		writer.WritePropertyName(PropName_combatSpeed);
		settingsDataMetric = value;
		JsonSerializer.Serialize(writer, settingsDataMetric.FastModeType, FastModeType);
		JsonEncodedText propName_screenshake = PropName_screenshake;
		settingsDataMetric = value;
		writer.WriteNumber(propName_screenshake, settingsDataMetric.Screenshake);
		JsonEncodedText propName_runTimer = PropName_runTimer;
		settingsDataMetric = value;
		writer.WriteBoolean(propName_runTimer, settingsDataMetric.ShowRunTimer);
		JsonEncodedText propName_phobiaMode = PropName_phobiaMode;
		settingsDataMetric = value;
		writer.WriteBoolean(propName_phobiaMode, settingsDataMetric.PhobiaMode);
		JsonEncodedText propName_cardIndices = PropName_cardIndices;
		settingsDataMetric = value;
		writer.WriteBoolean(propName_cardIndices, settingsDataMetric.ShowCardIndices);
		JsonEncodedText propName_displayCount = PropName_displayCount;
		settingsDataMetric = value;
		writer.WriteNumber(propName_displayCount, settingsDataMetric.DisplayCount);
		writer.WritePropertyName(PropName_displayResolution);
		settingsDataMetric = value;
		Vector2ISerializeHandler(writer, settingsDataMetric.DisplayResolution);
		JsonEncodedText propName_fullscreen = PropName_fullscreen;
		settingsDataMetric = value;
		writer.WriteBoolean(propName_fullscreen, settingsDataMetric.Fullscreen);
		writer.WritePropertyName(PropName_aspectRatio);
		settingsDataMetric = value;
		JsonSerializer.Serialize(writer, settingsDataMetric.AspectRatio, AspectRatioSetting);
		JsonEncodedText propName_resizeWindows = PropName_resizeWindows;
		settingsDataMetric = value;
		writer.WriteBoolean(propName_resizeWindows, settingsDataMetric.ResizeWindows);
		writer.WritePropertyName(PropName_vSync);
		settingsDataMetric = value;
		JsonSerializer.Serialize(writer, settingsDataMetric.VSync, VSyncType);
		JsonEncodedText propName_fpsLimit = PropName_fpsLimit;
		settingsDataMetric = value;
		writer.WriteNumber(propName_fpsLimit, settingsDataMetric.FpsLimit);
		JsonEncodedText propName_msaa = PropName_msaa;
		settingsDataMetric = value;
		writer.WriteNumber(propName_msaa, settingsDataMetric.Msaa);
		writer.WriteEndObject();
	}

	private static JsonParameterInfoValues[] SettingsDataMetricCtorParamInit()
	{
		return new JsonParameterInfoValues[2]
		{
			new JsonParameterInfoValues
			{
				Name = "BuildId",
				ParameterType = typeof(string),
				Position = 0,
				IsNullable = false,
				IsMemberInitializer = true
			},
			new JsonParameterInfoValues
			{
				Name = "FastModeType",
				ParameterType = typeof(FastModeType),
				Position = 1,
				IsNullable = false,
				IsMemberInitializer = true
			}
		};
	}

	private JsonTypeInfo<AspectRatioSetting> Create_AspectRatioSetting(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<AspectRatioSetting> jsonTypeInfo))
		{
			JsonConverter converter = ExpandConverter(typeof(AspectRatioSetting), new JsonStringEnumConverter<AspectRatioSetting>(), options);
			jsonTypeInfo = JsonMetadataServices.CreateValueInfo<AspectRatioSetting>(options, converter);
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private JsonTypeInfo<FastModeType> Create_FastModeType(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<FastModeType> jsonTypeInfo))
		{
			JsonConverter converter = ExpandConverter(typeof(FastModeType), new JsonStringEnumConverter<FastModeType>(), options);
			jsonTypeInfo = JsonMetadataServices.CreateValueInfo<FastModeType>(options, converter);
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private JsonTypeInfo<VSyncType> Create_VSyncType(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<VSyncType> jsonTypeInfo))
		{
			JsonConverter converter = ExpandConverter(typeof(VSyncType), new JsonStringEnumConverter<VSyncType>(), options);
			jsonTypeInfo = JsonMetadataServices.CreateValueInfo<VSyncType>(options, converter);
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private JsonTypeInfo<EpochMetric> Create_EpochMetric(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<EpochMetric> jsonTypeInfo))
		{
			JsonObjectInfoValues<EpochMetric> objectInfo = new JsonObjectInfoValues<EpochMetric>
			{
				ObjectCreator = null,
				ObjectWithParameterizedConstructorCreator = (object[] args) => new EpochMetric
				{
					BuildId = (string)args[0],
					Epoch = (string)args[1]
				},
				PropertyMetadataInitializer = (JsonSerializerContext _) => EpochMetricPropInit(options),
				ConstructorParameterMetadataInitializer = EpochMetricCtorParamInit,
				ConstructorAttributeProviderFactory = () => typeof(EpochMetric).GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Array.Empty<Type>(), null),
				SerializeHandler = EpochMetricSerializeHandler
			};
			jsonTypeInfo = JsonMetadataServices.CreateObjectInfo(options, objectInfo);
			jsonTypeInfo.NumberHandling = null;
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private static JsonPropertyInfo[] EpochMetricPropInit(JsonSerializerOptions options)
	{
		JsonPropertyInfo[] array = new JsonPropertyInfo[5];
		JsonPropertyInfoValues<string> propertyInfo = new JsonPropertyInfoValues<string>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(EpochMetric),
			Converter = null,
			Getter = (object obj) => ((EpochMetric)obj).BuildId,
			Setter = (object obj, string? value) =>
			{
				((EpochMetric)obj).BuildId = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "BuildId",
			JsonPropertyName = "buildId",
			AttributeProviderFactory = () => typeof(EpochMetric).GetProperty("BuildId", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(string), Array.Empty<Type>(), null)
		};
		array[0] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo);
		array[0].IsRequired = true;
		array[0].IsGetNullable = false;
		array[0].IsSetNullable = false;
		JsonPropertyInfoValues<string> propertyInfo2 = new JsonPropertyInfoValues<string>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(EpochMetric),
			Converter = null,
			Getter = (object obj) => ((EpochMetric)obj).Epoch,
			Setter = (object obj, string? value) =>
			{
				((EpochMetric)obj).Epoch = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "Epoch",
			JsonPropertyName = "epoch",
			AttributeProviderFactory = () => typeof(EpochMetric).GetProperty("Epoch", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(string), Array.Empty<Type>(), null)
		};
		array[1] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo2);
		array[1].IsRequired = true;
		array[1].IsGetNullable = false;
		array[1].IsSetNullable = false;
		JsonPropertyInfoValues<int> propertyInfo3 = new JsonPropertyInfoValues<int>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(EpochMetric),
			Converter = null,
			Getter = (object obj) => ((EpochMetric)obj).TotalEpochs,
			Setter = (object obj, int value) =>
			{
				((EpochMetric)obj).TotalEpochs = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "TotalEpochs",
			JsonPropertyName = "totalEpochs",
			AttributeProviderFactory = () => typeof(EpochMetric).GetProperty("TotalEpochs", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(int), Array.Empty<Type>(), null)
		};
		array[2] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo3);
		JsonPropertyInfoValues<long> propertyInfo4 = new JsonPropertyInfoValues<long>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(EpochMetric),
			Converter = null,
			Getter = (object obj) => ((EpochMetric)obj).TotalPlaytime,
			Setter = (object obj, long value) =>
			{
				((EpochMetric)obj).TotalPlaytime = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "TotalPlaytime",
			JsonPropertyName = "totalPlaytime",
			AttributeProviderFactory = () => typeof(EpochMetric).GetProperty("TotalPlaytime", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(long), Array.Empty<Type>(), null)
		};
		array[3] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo4);
		JsonPropertyInfoValues<int> propertyInfo5 = new JsonPropertyInfoValues<int>
		{
			IsProperty = true,
			IsPublic = true,
			IsVirtual = false,
			DeclaringType = typeof(EpochMetric),
			Converter = null,
			Getter = (object obj) => ((EpochMetric)obj).TotalRuns,
			Setter = (object obj, int value) =>
			{
				((EpochMetric)obj).TotalRuns = value;
			},
			IgnoreCondition = null,
			HasJsonInclude = false,
			IsExtensionData = false,
			NumberHandling = null,
			PropertyName = "TotalRuns",
			JsonPropertyName = "totalRuns",
			AttributeProviderFactory = () => typeof(EpochMetric).GetProperty("TotalRuns", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, typeof(int), Array.Empty<Type>(), null)
		};
		array[4] = JsonMetadataServices.CreatePropertyInfo(options, propertyInfo5);
		return array;
	}

	private void EpochMetricSerializeHandler(Utf8JsonWriter writer, EpochMetric? value)
	{
		if (value == null)
		{
			writer.WriteNullValue();
			return;
		}
		writer.WriteStartObject();
		writer.WriteString(PropName_buildId, value.BuildId);
		writer.WriteString(PropName_epoch, value.Epoch);
		writer.WriteNumber(PropName_totalEpochs, value.TotalEpochs);
		writer.WriteNumber(PropName_totalPlaytime, value.TotalPlaytime);
		writer.WriteNumber(PropName_totalRuns, value.TotalRuns);
		writer.WriteEndObject();
	}

	private static JsonParameterInfoValues[] EpochMetricCtorParamInit()
	{
		return new JsonParameterInfoValues[2]
		{
			new JsonParameterInfoValues
			{
				Name = "BuildId",
				ParameterType = typeof(string),
				Position = 0,
				IsNullable = false,
				IsMemberInitializer = true
			},
			new JsonParameterInfoValues
			{
				Name = "Epoch",
				ParameterType = typeof(string),
				Position = 1,
				IsNullable = false,
				IsMemberInitializer = true
			}
		};
	}

	private JsonTypeInfo<IEnumerable<ModelId>> Create_IEnumerableModelId(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<IEnumerable<ModelId>> jsonTypeInfo))
		{
			JsonCollectionInfoValues<IEnumerable<ModelId>> collectionInfo = new JsonCollectionInfoValues<IEnumerable<ModelId>>
			{
				ObjectCreator = null,
				SerializeHandler = IEnumerableModelIdSerializeHandler
			};
			jsonTypeInfo = JsonMetadataServices.CreateIEnumerableInfo<IEnumerable<ModelId>, ModelId>(options, collectionInfo);
			jsonTypeInfo.NumberHandling = null;
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private void IEnumerableModelIdSerializeHandler(Utf8JsonWriter writer, IEnumerable<ModelId>? value)
	{
		if (value == null)
		{
			writer.WriteNullValue();
			return;
		}
		writer.WriteStartArray();
		foreach (ModelId item in value)
		{
			ModelIdSerializeHandler(writer, item);
		}
		writer.WriteEndArray();
	}

	private JsonTypeInfo<List<ModelId>> Create_ListModelId(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<List<ModelId>> jsonTypeInfo))
		{
			JsonCollectionInfoValues<List<ModelId>> collectionInfo = new JsonCollectionInfoValues<List<ModelId>>
			{
				ObjectCreator = () => new List<ModelId>(),
				SerializeHandler = ListModelIdSerializeHandler
			};
			jsonTypeInfo = JsonMetadataServices.CreateListInfo<List<ModelId>, ModelId>(options, collectionInfo);
			jsonTypeInfo.NumberHandling = null;
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private void ListModelIdSerializeHandler(Utf8JsonWriter writer, List<ModelId>? value)
	{
		if (value == null)
		{
			writer.WriteNullValue();
			return;
		}
		writer.WriteStartArray();
		for (int i = 0; i < value.Count; i++)
		{
			ModelIdSerializeHandler(writer, value[i]);
		}
		writer.WriteEndArray();
	}

	private JsonTypeInfo<List<ActWinMetric>> Create_ListActWinMetric(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<List<ActWinMetric>> jsonTypeInfo))
		{
			JsonCollectionInfoValues<List<ActWinMetric>> collectionInfo = new JsonCollectionInfoValues<List<ActWinMetric>>
			{
				ObjectCreator = () => new List<ActWinMetric>(),
				SerializeHandler = ListActWinMetricSerializeHandler
			};
			jsonTypeInfo = JsonMetadataServices.CreateListInfo<List<ActWinMetric>, ActWinMetric>(options, collectionInfo);
			jsonTypeInfo.NumberHandling = null;
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private void ListActWinMetricSerializeHandler(Utf8JsonWriter writer, List<ActWinMetric>? value)
	{
		if (value == null)
		{
			writer.WriteNullValue();
			return;
		}
		writer.WriteStartArray();
		for (int i = 0; i < value.Count; i++)
		{
			ActWinMetricSerializeHandler(writer, value[i]);
		}
		writer.WriteEndArray();
	}

	private JsonTypeInfo<List<AncientMetric>> Create_ListAncientMetric(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<List<AncientMetric>> jsonTypeInfo))
		{
			JsonCollectionInfoValues<List<AncientMetric>> collectionInfo = new JsonCollectionInfoValues<List<AncientMetric>>
			{
				ObjectCreator = () => new List<AncientMetric>(),
				SerializeHandler = ListAncientMetricSerializeHandler
			};
			jsonTypeInfo = JsonMetadataServices.CreateListInfo<List<AncientMetric>, AncientMetric>(options, collectionInfo);
			jsonTypeInfo.NumberHandling = null;
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private void ListAncientMetricSerializeHandler(Utf8JsonWriter writer, List<AncientMetric>? value)
	{
		if (value == null)
		{
			writer.WriteNullValue();
			return;
		}
		writer.WriteStartArray();
		for (int i = 0; i < value.Count; i++)
		{
			AncientMetricSerializeHandler(writer, value[i]);
		}
		writer.WriteEndArray();
	}

	private JsonTypeInfo<List<CardChoiceMetric>> Create_ListCardChoiceMetric(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<List<CardChoiceMetric>> jsonTypeInfo))
		{
			JsonCollectionInfoValues<List<CardChoiceMetric>> collectionInfo = new JsonCollectionInfoValues<List<CardChoiceMetric>>
			{
				ObjectCreator = () => new List<CardChoiceMetric>(),
				SerializeHandler = ListCardChoiceMetricSerializeHandler
			};
			jsonTypeInfo = JsonMetadataServices.CreateListInfo<List<CardChoiceMetric>, CardChoiceMetric>(options, collectionInfo);
			jsonTypeInfo.NumberHandling = null;
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private void ListCardChoiceMetricSerializeHandler(Utf8JsonWriter writer, List<CardChoiceMetric>? value)
	{
		if (value == null)
		{
			writer.WriteNullValue();
			return;
		}
		writer.WriteStartArray();
		for (int i = 0; i < value.Count; i++)
		{
			CardChoiceMetricSerializeHandler(writer, value[i]);
		}
		writer.WriteEndArray();
	}

	private JsonTypeInfo<List<EncounterMetric>> Create_ListEncounterMetric(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<List<EncounterMetric>> jsonTypeInfo))
		{
			JsonCollectionInfoValues<List<EncounterMetric>> collectionInfo = new JsonCollectionInfoValues<List<EncounterMetric>>
			{
				ObjectCreator = () => new List<EncounterMetric>(),
				SerializeHandler = ListEncounterMetricSerializeHandler
			};
			jsonTypeInfo = JsonMetadataServices.CreateListInfo<List<EncounterMetric>, EncounterMetric>(options, collectionInfo);
			jsonTypeInfo.NumberHandling = null;
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private void ListEncounterMetricSerializeHandler(Utf8JsonWriter writer, List<EncounterMetric>? value)
	{
		if (value == null)
		{
			writer.WriteNullValue();
			return;
		}
		writer.WriteStartArray();
		for (int i = 0; i < value.Count; i++)
		{
			EncounterMetricSerializeHandler(writer, value[i]);
		}
		writer.WriteEndArray();
	}

	private JsonTypeInfo<List<EventChoiceMetric>> Create_ListEventChoiceMetric(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<List<EventChoiceMetric>> jsonTypeInfo))
		{
			JsonCollectionInfoValues<List<EventChoiceMetric>> collectionInfo = new JsonCollectionInfoValues<List<EventChoiceMetric>>
			{
				ObjectCreator = () => new List<EventChoiceMetric>(),
				SerializeHandler = ListEventChoiceMetricSerializeHandler
			};
			jsonTypeInfo = JsonMetadataServices.CreateListInfo<List<EventChoiceMetric>, EventChoiceMetric>(options, collectionInfo);
			jsonTypeInfo.NumberHandling = null;
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private void ListEventChoiceMetricSerializeHandler(Utf8JsonWriter writer, List<EventChoiceMetric>? value)
	{
		if (value == null)
		{
			writer.WriteNullValue();
			return;
		}
		writer.WriteStartArray();
		for (int i = 0; i < value.Count; i++)
		{
			EventChoiceMetricSerializeHandler(writer, value[i]);
		}
		writer.WriteEndArray();
	}

	private JsonTypeInfo<List<string>> Create_ListString(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<List<string>> jsonTypeInfo))
		{
			JsonCollectionInfoValues<List<string>> collectionInfo = new JsonCollectionInfoValues<List<string>>
			{
				ObjectCreator = () => new List<string>(),
				SerializeHandler = ListStringSerializeHandler
			};
			jsonTypeInfo = JsonMetadataServices.CreateListInfo<List<string>, string>(options, collectionInfo);
			jsonTypeInfo.NumberHandling = null;
		}
		jsonTypeInfo.OriginatingResolver = this;
		return jsonTypeInfo;
	}

	private void ListStringSerializeHandler(Utf8JsonWriter writer, List<string>? value)
	{
		if (value == null)
		{
			writer.WriteNullValue();
			return;
		}
		writer.WriteStartArray();
		for (int i = 0; i < value.Count; i++)
		{
			writer.WriteStringValue(value[i]);
		}
		writer.WriteEndArray();
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

	private JsonTypeInfo<long> Create_Int64(JsonSerializerOptions options)
	{
		if (!TryGetTypeInfoForRuntimeCustomConverter(options, out JsonTypeInfo<long> jsonTypeInfo))
		{
			jsonTypeInfo = JsonMetadataServices.CreateValueInfo<long>(options, JsonMetadataServices.Int64Converter);
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

	/// <inheritdoc />
	public MetricsSerializerContext()
		: base(null)
	{
	}

	/// <inheritdoc />
	public MetricsSerializerContext(JsonSerializerOptions options)
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
		if (type == typeof(float))
		{
			return Create_Single(options);
		}
		if (type == typeof(Vector2I))
		{
			return Create_Vector2I(options);
		}
		if (type == typeof(AchievementMetric))
		{
			return Create_AchievementMetric(options);
		}
		if (type == typeof(ModelId))
		{
			return Create_ModelId(options);
		}
		if (type == typeof(ActWinMetric))
		{
			return Create_ActWinMetric(options);
		}
		if (type == typeof(AncientMetric))
		{
			return Create_AncientMetric(options);
		}
		if (type == typeof(CardChoiceMetric))
		{
			return Create_CardChoiceMetric(options);
		}
		if (type == typeof(EncounterMetric))
		{
			return Create_EncounterMetric(options);
		}
		if (type == typeof(EventChoiceMetric))
		{
			return Create_EventChoiceMetric(options);
		}
		if (type == typeof(RunMetrics))
		{
			return Create_RunMetrics(options);
		}
		if (type == typeof(SettingsDataMetric))
		{
			return Create_SettingsDataMetric(options);
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
		if (type == typeof(EpochMetric))
		{
			return Create_EpochMetric(options);
		}
		if (type == typeof(IEnumerable<ModelId>))
		{
			return Create_IEnumerableModelId(options);
		}
		if (type == typeof(List<ModelId>))
		{
			return Create_ListModelId(options);
		}
		if (type == typeof(List<ActWinMetric>))
		{
			return Create_ListActWinMetric(options);
		}
		if (type == typeof(List<AncientMetric>))
		{
			return Create_ListAncientMetric(options);
		}
		if (type == typeof(List<CardChoiceMetric>))
		{
			return Create_ListCardChoiceMetric(options);
		}
		if (type == typeof(List<EncounterMetric>))
		{
			return Create_ListEncounterMetric(options);
		}
		if (type == typeof(List<EventChoiceMetric>))
		{
			return Create_ListEventChoiceMetric(options);
		}
		if (type == typeof(List<string>))
		{
			return Create_ListString(options);
		}
		if (type == typeof(int))
		{
			return Create_Int32(options);
		}
		if (type == typeof(long))
		{
			return Create_Int64(options);
		}
		if (type == typeof(string))
		{
			return Create_String(options);
		}
		return null;
	}
}
