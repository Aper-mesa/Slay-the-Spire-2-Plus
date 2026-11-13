using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models.Acts;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Characters;
using MegaCrit.Sts2.Core.Models.Dread;
using MegaCrit.Sts2.Core.Models.Encounters;
using MegaCrit.Sts2.Core.Models.Events;
using MegaCrit.Sts2.Core.Models.Exceptions;
using MegaCrit.Sts2.Core.Models.Modifiers;
using MegaCrit.Sts2.Core.Models.Orbs;
using MegaCrit.Sts2.Core.Models.PotionPools;
using MegaCrit.Sts2.Core.Models.RelicPools;

namespace MegaCrit.Sts2.Core.Models;

public static class ModelDb
{
	private const int _initialCapacity = 4096;

	private static readonly Dictionary<ModelId, AbstractModel> _contentById = new Dictionary<ModelId, AbstractModel>(4096);

	private static Type[]? _allAbstractModelSubtypes;

	private static IEnumerable<CardModel>? _allCards;

	private static IEnumerable<CardPoolModel>? _allCardPools;

	private static IEnumerable<CardPoolModel>? _allCharacterCardPools;

	private static IEnumerable<EventModel>? _allSharedEvents;

	private static IEnumerable<EventModel>? _allEvents;

	private static IEnumerable<EncounterModel>? _allEncounters;

	private static IEnumerable<EncounterModel>? _eventEncounters;

	private static IEnumerable<PotionModel>? _allPotions;

	private static IEnumerable<PotionPoolModel>? _allPotionPools;

	private static IEnumerable<PotionPoolModel>? _allCharacterPotionPools;

	private static IEnumerable<RelicPoolModel>? _allCharacterRelicPools;

	private static IEnumerable<PotionPoolModel>? _allSharedPotionPools;

	private static IEnumerable<PowerModel>? _allPowers;

	private static IEnumerable<RelicModel>? _allRelics;

	private static List<ActModel>? _acts;

	private static List<List<ActModel>>? _actsByIndex;

	private static List<BadgeModel>? _badges;

	private static List<AchievementModel>? _achievements;

	public static Type[] AllAbstractModelSubtypes
	{
		get
		{
			if (_allAbstractModelSubtypes != null)
			{
				return _allAbstractModelSubtypes;
			}
			HashSet<Type> hashSet = new HashSet<Type>();
			foreach (Type item in AbstractModelSubtypes.All)
			{
				hashSet.Add(item);
			}
			foreach (Type subtypesInMod in ReflectionHelper.GetSubtypesInMods<AbstractModel>())
			{
				hashSet.Add(subtypesInMod);
			}
			_allAbstractModelSubtypes = hashSet.ToArray();
			return _allAbstractModelSubtypes;
		}
	}

	public static IEnumerable<AbstractModel> All => _contentById.Values;

	public static IEnumerable<AfflictionModel> DebugAfflictions => from t in AllAbstractModelSubtypes
		where t.IsSubclassOf(typeof(AfflictionModel))
		select (AfflictionModel)Get(t);

	public static IEnumerable<EnchantmentModel> DebugEnchantments => from t in AllAbstractModelSubtypes
		where t.IsSubclassOf(typeof(EnchantmentModel))
		select (EnchantmentModel)Get(t);

	public static IEnumerable<CardModel> AllCards => _allCards ?? (_allCards = AllCardPools.SelectMany((CardPoolModel p) => p.AllCards).Concat(AllCharacters.SelectMany((CharacterModel c) => c.StartingDeck).Distinct()).Distinct());

	public static IEnumerable<CardPoolModel> AllCardPools => _allCardPools ?? (_allCardPools = AllCharacterCardPools.Concat(AllSharedCardPools).Distinct());

	public static IEnumerable<CardPoolModel> AllSharedCardPools => new _003C_003Ez__ReadOnlyArray<CardPoolModel>(new CardPoolModel[7]
	{
		CardPool<ColorlessCardPool>(),
		CardPool<CurseCardPool>(),
		CardPool<DeprecatedCardPool>(),
		CardPool<EventCardPool>(),
		CardPool<QuestCardPool>(),
		CardPool<StatusCardPool>(),
		CardPool<TokenCardPool>()
	});

	public static IEnumerable<CardPoolModel> AllCharacterCardPools => _allCharacterCardPools ?? (_allCharacterCardPools = AllCharacters.Select((CharacterModel c) => c.CardPool));

	public static IEnumerable<CharacterModel> AllCharacters => new _003C_003Ez__ReadOnlyArray<CharacterModel>(new CharacterModel[10]
	{
		Character<Ironclad>(),
		Character<Silent>(),
		Character<Regent>(),
		Character<Necrobinder>(),
		Character<Defect>(),
		Character<Fool>(),
		Character<ClassicDefect>(),
		Character<ClassicSilent>(),
		Character<ClassicIronclad>(),
		Character<ClassicWatcher>()
	});

	public static IEnumerable<EventModel> AllSharedEvents => _allSharedEvents ?? (_allSharedEvents = new _003C_003Ez__ReadOnlyArray<EventModel>(new EventModel[19]
	{
		Event<BrainLeech>(),
		Event<CrystalSphere>(),
		Event<DollRoom>(),
		Event<FakeMerchant>(),
		Event<MuseumOfTorture>(),
		Event<PotionCourier>(),
		Event<RanwidTheElder>(),
		Event<RelicTrader>(),
		Event<RoomFullOfCheese>(),
		Event<SelfHelpBook>(),
		Event<SlipperyBridge>(),
		Event<StoneOfAllTime>(),
		Event<Symbiote>(),
		Event<TeaMaster>(),
		Event<TheFutureOfPotions>(),
		Event<TheLegendsWereTrue>(),
		Event<ThisOrThat>(),
		Event<WarHistorianRepy>(),
		Event<WelcomeToWongos>()
	}));

	public static IEnumerable<AncientEventModel> AllAncients => Acts.SelectMany((ActModel a) => a.AllAncients).Concat(AllSharedAncients).Distinct();

	public static IEnumerable<AncientEventModel> AllSharedAncients => new _003C_003Ez__ReadOnlySingleElementList<AncientEventModel>(AncientEvent<Darv>());

	public static IEnumerable<EventModel> AllEvents => _allEvents ?? (_allEvents = Acts.SelectMany((ActModel a) => a.AllEvents).Concat(AllSharedEvents).Distinct());

	public static IEnumerable<MonsterModel> Monsters => Acts.SelectMany((ActModel act) => act.AllMonsters).Concat(EventEncounters.SelectMany((EncounterModel e) => e.AllPossibleMonsters)).Distinct();

	public static IEnumerable<EncounterModel> AllEncounters => _allEncounters ?? (_allEncounters = Acts.SelectMany((ActModel a) => a.AllEncounters).Concat(EventEncounters).Distinct());

	public static IEnumerable<EncounterModel> EventEncounters => _eventEncounters ?? (_eventEncounters = new _003C_003Ez__ReadOnlyArray<EncounterModel>(new EncounterModel[5]
	{
		Encounter<FakeMerchantEventEncounter>(),
		Encounter<MysteriousKnightEventEncounter>(),
		Encounter<BattlewornDummyEventV1Encounter>(),
		Encounter<BattlewornDummyEventV2Encounter>(),
		Encounter<BattlewornDummyEventV3Encounter>()
	}));

	public static IEnumerable<PotionModel> AllPotions => _allPotions ?? (_allPotions = from p in AllPotionPools.SelectMany((PotionPoolModel p) => p.AllPotions).Distinct()
		orderby p.Id.Entry
		select p);

	public static IEnumerable<PotionPoolModel> AllPotionPools => _allPotionPools ?? (_allPotionPools = AllCharacterPotionPools.Concat(AllSharedPotionPools).Distinct());

	public static IEnumerable<PotionPoolModel> AllCharacterPotionPools => _allCharacterPotionPools ?? (_allCharacterPotionPools = AllCharacters.Select((CharacterModel c) => c.PotionPool));

	public static IEnumerable<RelicPoolModel> AllCharacterRelicPools => _allCharacterRelicPools ?? (_allCharacterRelicPools = AllCharacters.Select((CharacterModel c) => c.RelicPool));

	private static IEnumerable<PotionPoolModel> AllSharedPotionPools => _allSharedPotionPools ?? (_allSharedPotionPools = new _003C_003Ez__ReadOnlyArray<PotionPoolModel>(new PotionPoolModel[5]
	{
		PotionPool<DeprecatedPotionPool>(),
		PotionPool<EventPotionPool>(),
		PotionPool<MockPotionPool>(),
		PotionPool<SharedPotionPool>(),
		PotionPool<TokenPotionPool>()
	}));

	public static IEnumerable<PowerModel> AllPowers => _allPowers ?? (_allPowers = from t in AllAbstractModelSubtypes
		where t.IsSubclassOf(typeof(PowerModel))
		select (PowerModel)Get(t));

	public static IEnumerable<RelicModel> AllRelics => _allRelics ?? (_allRelics = from r in AllRelicPools.SelectMany((RelicPoolModel p) => p.AllRelics).Concat(AllCharacters.SelectMany((CharacterModel c) => c.StartingRelics)).Distinct()
		orderby r.Id.Entry
		select r);

	public static IEnumerable<RelicPoolModel> AllRelicPools => CharacterRelicPools.Concat(AllSharedRelicPools).Distinct();

	public static IEnumerable<RelicPoolModel> CharacterRelicPools => AllCharacters.Select((CharacterModel c) => c.RelicPool);

	private static IEnumerable<RelicPoolModel> AllSharedRelicPools => new _003C_003Ez__ReadOnlyArray<RelicPoolModel>(new RelicPoolModel[4]
	{
		RelicPool<DeprecatedRelicPool>(),
		RelicPool<EventRelicPool>(),
		RelicPool<FallbackRelicPool>(),
		RelicPool<SharedRelicPool>()
	});

	public static IEnumerable<OrbModel> Orbs => new _003C_003Ez__ReadOnlyArray<OrbModel>(new OrbModel[5]
	{
		Orb<LightningOrb>(),
		Orb<FrostOrb>(),
		Orb<DarkOrb>(),
		Orb<PlasmaOrb>(),
		Orb<GlassOrb>()
	});

	public static IEnumerable<ActModel> Acts
	{
		get
		{
			if (_acts == null)
			{
				int num = 4;
				List<ActModel> list = new List<ActModel>(num);
				CollectionsMarshal.SetCount(list, num);
				Span<ActModel> span = CollectionsMarshal.AsSpan(list);
				int num2 = 0;
				span[num2] = Act<Overgrowth>();
				num2++;
				span[num2] = Act<Underdocks>();
				num2++;
				span[num2] = Act<Hive>();
				num2++;
				span[num2] = Act<Glory>();
				_acts = list;
			}
			return _acts;
		}
	}

	public static IReadOnlyList<IReadOnlyList<ActModel>> ActsByIndex
	{
		get
		{
			if (_actsByIndex != null)
			{
				return _actsByIndex;
			}
			_actsByIndex = new List<List<ActModel>>();
			foreach (ActModel act in Acts)
			{
				if (act.Index >= 0)
				{
					for (int i = _actsByIndex.Count; i <= act.Index; i++)
					{
						_actsByIndex.Add(new List<ActModel>());
					}
					_actsByIndex[act.Index].Add(act);
				}
			}
			return _actsByIndex;
		}
	}

	public static IReadOnlyList<BadgeModel> BadgeModels
	{
		get
		{
			if (_badges == null)
			{
				_badges = new List<BadgeModel>();
				Type[] allAbstractModelSubtypes = AllAbstractModelSubtypes;
				foreach (Type type in allAbstractModelSubtypes)
				{
					if (type.IsSubclassOf(typeof(BadgeModel)))
					{
						_badges.Add((BadgeModel)Get(type));
					}
				}
			}
			return _badges;
		}
	}

	public static IReadOnlyList<AchievementModel> Achievements
	{
		get
		{
			if (_achievements == null)
			{
				_achievements = new List<AchievementModel>();
				Type[] allAbstractModelSubtypes = AllAbstractModelSubtypes;
				foreach (Type type in allAbstractModelSubtypes)
				{
					if (type.IsSubclassOf(typeof(AchievementModel)))
					{
						_achievements.Add((AchievementModel)Get(type));
					}
				}
			}
			return _achievements;
		}
	}

	public static IReadOnlyList<ModifierModel> GoodModifiers => new _003C_003Ez__ReadOnlyArray<ModifierModel>(new ModifierModel[9]
	{
		Modifier<Draft>(),
		Modifier<SealedDeck>(),
		Modifier<Hoarder>(),
		Modifier<Specialized>(),
		Modifier<Insanity>(),
		Modifier<AllStar>(),
		Modifier<Flight>(),
		Modifier<Vintage>(),
		Modifier<CharacterCards>()
	});

	public static IReadOnlyList<ModifierModel> BadModifiers => new _003C_003Ez__ReadOnlyArray<ModifierModel>(new ModifierModel[7]
	{
		Modifier<DeadlyEvents>(),
		Modifier<CursedRun>(),
		Modifier<BigGameHunter>(),
		Modifier<Midas>(),
		Modifier<Murderous>(),
		Modifier<NightTerrors>(),
		Modifier<Terminal>()
	});

	public static IReadOnlyList<IReadOnlySet<ModifierModel>> MutuallyExclusiveModifiers => new _003C_003Ez__ReadOnlySingleElementList<IReadOnlySet<ModifierModel>>(new HashSet<ModifierModel>
	{
		Modifier<SealedDeck>(),
		Modifier<Draft>(),
		Modifier<Insanity>()
	});

	public static void Init(Type[]? injectedModelTypes = null)
	{
		Type[] array = injectedModelTypes ?? AllAbstractModelSubtypes;
		foreach (Type type in array)
		{
			ModelId id = GetId(type);
			AbstractModel value = (AbstractModel)Activator.CreateInstance(type);
			_contentById[id] = value;
		}
	}

	public static void Inject([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] Type type)
	{
		if (!Contains(type))
		{
			ModelId id = GetId(type);
			AbstractModel value = (AbstractModel)Activator.CreateInstance(type);
			_contentById[id] = value;
		}
	}

	public static void Remove(Type type)
	{
		ModelId id = GetId(type);
		_contentById.Remove(id);
	}

	public static void ResetForTest()
	{
		_allAbstractModelSubtypes = null;
		_contentById.Clear();
		_achievements = null;
		_acts = null;
		_actsByIndex = null;
		_allCardPools = null;
		_allCards = null;
		_allCharacterCardPools = null;
		_allCharacterPotionPools = null;
		_allCharacterRelicPools = null;
		_allEncounters = null;
		_allEvents = null;
		_allPotionPools = null;
		_allPotions = null;
		_allPowers = null;
		_allRelics = null;
		_allSharedEvents = null;
		_allSharedPotionPools = null;
		_badges = null;
		_eventEncounters = null;
	}

	public static void InitIds()
	{
		foreach (KeyValuePair<ModelId, AbstractModel> item in _contentById)
		{
			item.Value.InitId(item.Key);
		}
	}

	public static void Preload()
	{
		_ = AllCards;
		_ = AllCharacterCardPools;
		_ = AllSharedEvents;
		_ = AllEvents;
		_ = AllRelics;
		_ = AllPotions;
		_ = AllEncounters;
		_ = Achievements;
		foreach (CardModel allCard in AllCards)
		{
			_ = allCard.Pool;
			_ = allCard.AllPortraitPaths;
		}
		foreach (RelicModel allRelic in AllRelics)
		{
			_ = allRelic.IconPath;
		}
		foreach (PowerModel allPower in AllPowers)
		{
			_ = allPower.IconPath;
			_ = allPower.ResolvedBigIconPath;
		}
	}

	public static ModelId GetId<T>() where T : AbstractModel
	{
		return GetId(typeof(T));
	}

	public static ModelId GetId(Type type)
	{
		return new ModelId(GetCategory(type), GetEntry(type));
	}

	public static Type GetCategoryType(Type type)
	{
		Type type2 = type;
		while (type2.BaseType != typeof(AbstractModel))
		{
			type2 = type2.BaseType;
		}
		return type2;
	}

	public static string GetCategory(Type type)
	{
		return ModelId.SlugifyCategory(GetCategoryType(type).Name);
	}

	public static string GetEntry(Type type)
	{
		return StringHelper.Slugify(type.Name);
	}

	public static T? GetByIdOrNull<T>(ModelId id) where T : AbstractModel
	{
		if (_contentById.TryGetValue(id, out AbstractModel value))
		{
			return (T)value;
		}
		return null;
	}

	public static T GetById<T>(ModelId id) where T : AbstractModel
	{
		T byIdOrNull = GetByIdOrNull<T>(id);
		return byIdOrNull ?? throw new ModelNotFoundException(id);
	}

	public static bool Contains(Type type)
	{
		return _contentById.ContainsKey(GetId(type));
	}

	private static T Get<T>() where T : AbstractModel
	{
		return (T)_contentById[GetId<T>()];
	}

	private static AbstractModel Get(Type type)
	{
		if (!type.IsSubclassOf(typeof(AbstractModel)))
		{
			throw new InvalidOperationException();
		}
		ModelId id = GetId(type);
		if (_contentById.TryGetValue(id, out AbstractModel value))
		{
			return value;
		}
		throw new ModelNotFoundException(id);
	}

	public static T Affliction<T>() where T : AfflictionModel
	{
		return Get<T>();
	}

	public static T Enchantment<T>() where T : EnchantmentModel
	{
		return Get<T>();
	}

	public static T Card<T>() where T : CardModel
	{
		return Get<T>();
	}

	public static T CardPool<T>() where T : CardPoolModel
	{
		return Get<T>();
	}

	public static T Character<T>() where T : CharacterModel
	{
		return Get<T>();
	}

	public static T Event<T>() where T : EventModel
	{
		return Get<T>();
	}

	public static T AncientEvent<T>() where T : AncientEventModel
	{
		return Get<T>();
	}

	public static T Monster<T>() where T : MonsterModel
	{
		return Get<T>();
	}

	public static T Encounter<T>() where T : EncounterModel
	{
		return Get<T>();
	}

	public static T Potion<T>() where T : PotionModel
	{
		return Get<T>();
	}

	public static T PotionPool<T>() where T : PotionPoolModel
	{
		return Get<T>();
	}

	public static T Power<T>() where T : PowerModel
	{
		return Get<T>();
	}

	public static PowerModel DebugPower(Type type)
	{
		return (PowerModel)Get(type);
	}

	public static T Relic<T>() where T : RelicModel
	{
		return Get<T>();
	}

	public static T RelicPool<T>() where T : RelicPoolModel
	{
		return Get<T>();
	}

	public static T Orb<T>() where T : OrbModel
	{
		return Get<T>();
	}

	public static OrbModel? DebugOrb(Type type)
	{
		try
		{
			return (OrbModel)Get(type);
		}
		catch
		{
			return null;
		}
	}

	public static T Act<T>() where T : ActModel
	{
		return Get<T>();
	}

	public static T RunModel<T>() where T : RunModel
	{
		return Get<T>();
	}

	public static T Dominion<T>() where T : DominionModel
	{
		return Get<T>();
	}

	public static T Badge<T>() where T : BadgeModel
	{
		return Get<T>();
	}

	public static T Achievement<T>() where T : AchievementModel
	{
		return Get<T>();
	}

	public static T Modifier<T>() where T : ModifierModel
	{
		return Get<T>();
	}
}
