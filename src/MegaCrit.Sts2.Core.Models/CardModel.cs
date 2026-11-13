using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Godot;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Saves;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.TestSupport;

namespace MegaCrit.Sts2.Core.Models;

public abstract class CardModel : AbstractModel
{
	private enum DescriptionPreviewType
	{
		None,
		Upgrade
	}

	private LocString? _titleLocString;

	private CardPoolModel? _pool;

	private Player? _owner;

	private CardEnergyCost? _energyCost;

	private int _baseReplayCount;

	private int _extraReplayThisTurn;

	private bool _starCostSet;

	private int _baseStarCost;

	private bool _wasStarCostJustUpgraded;

	private List<TemporaryCardCost> _temporaryStarCosts = new List<TemporaryCardCost>();

	private int _lastStarsSpent;

	private HashSet<CardKeyword>? _keywords;

	private HashSet<CardKeyword> _keywordsAddedFromOutside = new HashSet<CardKeyword>();

	private HashSet<CardKeyword> _keywordsRemovedFromOutside = new HashSet<CardKeyword>();

	private bool _isUpgrading;

	private HashSet<CardTag>? _tags;

	private DynamicVarSet? _dynamicVars;

	private bool _exhaustOnNextPlay;

	private bool _hasSingleTurnRetain;

	private bool _hasSingleTurnSly;

	private CardModel? _cloneOf;

	private bool _isDupe;

	private int _currentUpgradeLevel;

	private CardUpgradePreviewType _upgradePreviewType;

	private bool _isEnchantmentPreview;

	private int? _floorAddedToDeck;

	private Creature? _currentTarget;

	private int _currentPlayIndex;

	private CardModel? _deckVersion;

	private CardModel? _transformIntoAfterPlay;

	private CardModel? _canonicalInstance;

	public LocString TitleLocString => _titleLocString ?? (_titleLocString = new LocString("cards", Id.Entry + ".title"));

	public virtual string Title
	{
		get
		{
			LocString titleLocString = TitleLocString;
			if (!IsUpgraded)
			{
				return titleLocString.GetFormattedText();
			}
			if (MaxUpgradeLevel > 1)
			{
				return $"{titleLocString.GetFormattedText()}+{CurrentUpgradeLevel}";
			}
			return titleLocString.GetFormattedText() + "+";
		}
	}

	public LocString Description => new LocString("cards", Id.Entry + ".description");

	protected LocString SelectionScreenPrompt
	{
		get
		{
			LocString locString = new LocString("cards", Id.Entry + ".selectionScreenPrompt");
			if (!locString.Exists())
			{
				throw new InvalidOperationException($"No selection screen prompt for {Id}.");
			}
			DynamicVars.AddTo(locString);
			return locString;
		}
	}

	public virtual string PortraitPath => ImageHelper.GetImagePath($"atlases/card_atlas.sprites/{Pool.Title.ToLowerInvariant()}/{Id.Entry.ToLowerInvariant()}.tres");

	public virtual string BetaPortraitPath => ImageHelper.GetImagePath($"atlases/card_atlas.sprites/{Pool.Title.ToLowerInvariant()}/beta/{Id.Entry.ToLowerInvariant()}.tres");

	public static string MissingPortraitPath => ImageHelper.GetImagePath("atlases/card_atlas.sprites/beta.tres");

	protected virtual string PortraitPngPath => ImageHelper.GetImagePath($"packed/card_portraits/{Pool.Title.ToLowerInvariant()}/{Id.Entry.ToLowerInvariant()}.png");

	private string BetaPortraitPngPath => ImageHelper.GetImagePath($"packed/card_portraits/{Pool.Title.ToLowerInvariant()}/beta/{Id.Entry.ToLowerInvariant()}.png");

	public bool HasPortrait => ResourceLoader.Exists(PortraitPngPath);

	public bool HasBetaPortrait => ResourceLoader.Exists(BetaPortraitPngPath);

	public virtual Texture2D Portrait => ResourceLoader.Load<Texture2D>(PortraitPath, null, ResourceLoader.CacheMode.Reuse);

	public Texture2D BigPortrait
	{
		get
		{
			if (!HasPortrait)
			{
				return Portrait;
			}
			return PreloadManager.Cache.GetTexture2D(PortraitPngPath);
		}
	}

	private string FramePath
	{
		get
		{
			CardType cardType;
			switch (Type)
			{
			case CardType.None:
			case CardType.Status:
			case CardType.Curse:
				cardType = CardType.Skill;
				break;
			case CardType.Attack:
			case CardType.Skill:
			case CardType.Power:
			case CardType.Quest:
				cardType = Type;
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
			if (Rarity != CardRarity.Ancient)
			{
				return ImageHelper.GetImagePath("atlases/ui_atlas.sprites/card/card_frame_" + cardType.ToString().ToLowerInvariant() + "_s.tres");
			}
			return ImageHelper.GetImagePath("atlases/card_atlas.sprites/beta.tres");
		}
	}

	public Texture2D Frame => ResourceLoader.Load<Texture2D>(FramePath, null, ResourceLoader.CacheMode.Reuse);

	private string PortraitBorderPath
	{
		get
		{
			CardType cardType;
			switch (Type)
			{
			case CardType.None:
			case CardType.Status:
			case CardType.Curse:
			case CardType.Quest:
				cardType = CardType.Skill;
				break;
			case CardType.Attack:
			case CardType.Skill:
			case CardType.Power:
				cardType = Type;
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
			return ImageHelper.GetImagePath("atlases/ui_atlas.sprites/card/card_portrait_border_" + cardType.ToString().ToLowerInvariant() + "_s.tres");
		}
	}

	private static string AncientBorderPath => ImageHelper.GetImagePath("atlases/compressed_atlas.sprites/ancient_card_border.tres");

	private string AncientTextBgPath
	{
		get
		{
			if (Rarity != CardRarity.Ancient)
			{
				throw new InvalidOperationException("This card is not an ancient card.");
			}
			CardType cardType;
			switch (Type)
			{
			case CardType.None:
			case CardType.Status:
			case CardType.Curse:
				cardType = CardType.Skill;
				break;
			case CardType.Attack:
			case CardType.Skill:
			case CardType.Power:
			case CardType.Quest:
				cardType = Type;
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
			CardType cardType2 = cardType;
			return ImageHelper.GetImagePath("atlases/compressed_atlas.sprites/ancient_text_bg_" + cardType2.ToString().ToLowerInvariant() + ".tres");
		}
	}

	public Texture2D AncientTextBg => ResourceLoader.Load<Texture2D>(AncientTextBgPath, null, ResourceLoader.CacheMode.Reuse);

	public Texture2D AncientBorder => ResourceLoader.Load<Texture2D>(AncientBorderPath, null, ResourceLoader.CacheMode.Reuse);

	public Texture2D PortraitBorder => ResourceLoader.Load<Texture2D>(PortraitBorderPath, null, ResourceLoader.CacheMode.Reuse);

	private string EnergyIconPath => VisualCardPool.EnergyIconPath;

	public Texture2D EnergyIcon => ResourceLoader.Load<Texture2D>(EnergyIconPath, null, ResourceLoader.CacheMode.Reuse);

	protected IHoverTip EnergyHoverTip => HoverTipFactory.ForEnergy(this);

	protected IHoverTip ArrowHoverTip
	{
		get
		{
			object player;
			if (IsMutable)
			{
				CardPile? pile = Pile;
				if (pile != null && pile.IsCombatPile)
				{
					player = _owner;
					goto IL_0025;
				}
			}
			player = null;
			goto IL_0025;
			IL_0025:
			return HoverTipFactory.ForArrow((Player?)player);
		}
	}

	private string BannerTexturePath
	{
		get
		{
			if (Rarity != CardRarity.Ancient)
			{
				return ImageHelper.GetImagePath("atlases/ui_atlas.sprites/card/card_banner.tres");
			}
			return ImageHelper.GetImagePath("atlases/ui_atlas.sprites/card/card_banner_ancient_s.tres");
		}
	}

	public Texture2D BannerTexture => ResourceLoader.Load<Texture2D>(BannerTexturePath, null, ResourceLoader.CacheMode.Reuse);

	private string BannerMaterialPath => Rarity switch
	{
		CardRarity.Uncommon => "res://materials/cards/banners/card_banner_uncommon_mat.tres",
		CardRarity.Rare => "res://materials/cards/banners/card_banner_rare_mat.tres",
		CardRarity.Curse => "res://materials/cards/banners/card_banner_curse_mat.tres",
		CardRarity.Status => "res://materials/cards/banners/card_banner_status_mat.tres",
		CardRarity.Event => "res://materials/cards/banners/card_banner_event_mat.tres",
		CardRarity.Quest => "res://materials/cards/banners/card_banner_quest_mat.tres",
		CardRarity.Ancient => "res://materials/cards/banners/card_banner_ancient_mat.tres",
		_ => "res://materials/cards/banners/card_banner_common_mat.tres",
	};

	public Material BannerMaterial => PreloadManager.Cache.GetMaterial(BannerMaterialPath);

	public Material FrameMaterial => VisualCardPool.FrameMaterial;

	public virtual CardType Type { get; }

	public virtual CardRarity Rarity { get; }

	public virtual CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.None;

	public virtual bool IsAllowedInBlitz => true;

	public virtual CardPoolModel Pool
	{
		get
		{
			if (_pool != null)
			{
				return _pool;
			}
			_pool = ModelDb.AllCardPools.FirstOrDefault((CardPoolModel pool) => pool.AllCardIds.Contains(Id));
			if (_pool != null)
			{
				return _pool;
			}
			if (ModelDb.CardPool<MockCardPool>().AllCardIds.Contains(Id))
			{
				_pool = ModelDb.CardPool<MockCardPool>();
				return _pool;
			}
			throw new InvalidProgramException($"Card {this} is not in any card pool!");
		}
	}

	public virtual CardPoolModel VisualCardPool => Pool;

	public Player Owner
	{
		get
		{
			AssertMutable();
			return _owner;
		}
		set
		{
			AssertMutable();
			if (_owner != null && value != null)
			{
				throw new InvalidOperationException("Card " + Id.Entry + " already has an owner.");
			}
			_owner = value;
		}
	}

	public CardPile? Pile => _owner?.Piles.FirstOrDefault((CardPile p) => p.Cards.Contains(this));

	protected virtual int CanonicalEnergyCost { get; }

	protected virtual bool HasEnergyCostX => false;

	public CardEnergyCost EnergyCost
	{
		get
		{
			if (_energyCost == null)
			{
				_energyCost = new CardEnergyCost(this, CanonicalEnergyCost, HasEnergyCostX);
			}
			return _energyCost;
		}
	}

	public int BaseReplayCount
	{
		get
		{
			return _baseReplayCount;
		}
		set
		{
			AssertMutable();
			_baseReplayCount = value;
			ReplayCountChanged?.Invoke();
		}
	}

	public int ExtraReplayThisTurn
	{
		get
		{
			return _extraReplayThisTurn;
		}
		set
		{
			AssertMutable();
			_extraReplayThisTurn = value;
			ReplayCountChanged?.Invoke();
		}
	}

	public virtual int CanonicalStarCost => -1;

	public int BaseStarCost
	{
		get
		{
			if (!IsMutable)
			{
				return CanonicalStarCost;
			}
			if (!_starCostSet)
			{
				_baseStarCost = CanonicalStarCost;
				_starCostSet = true;
			}
			return _baseStarCost;
		}
		private set
		{
			AssertMutable();
			if (!HasStarCostX)
			{
				_baseStarCost = value;
				_starCostSet = true;
			}
			StarCostChanged?.Invoke();
		}
	}

	public bool WasStarCostJustUpgraded => _wasStarCostJustUpgraded;

	public TemporaryCardCost? TemporaryStarCost => _temporaryStarCosts.LastOrDefault();

	public virtual int CurrentStarCost
	{
		get
		{
			int? num = _temporaryStarCosts.LastOrDefault()?.Cost;
			if (num.HasValue)
			{
				if (num == 0 && BaseStarCost < 0)
				{
					return BaseStarCost;
				}
				return num.Value;
			}
			return BaseStarCost;
		}
	}

	public virtual bool HasStarCostX => false;

	public int LastStarsSpent
	{
		get
		{
			return _lastStarsSpent;
		}
		set
		{
			AssertMutable();
			_lastStarsSpent = value;
		}
	}

	public virtual TargetType TargetType { get; }

	public virtual IEnumerable<CardKeyword> CanonicalKeywords => Array.Empty<CardKeyword>();

	private HashSet<CardKeyword> LocalKeywords
	{
		get
		{
			if (_keywords != null)
			{
				return _keywords;
			}
			_keywords = new HashSet<CardKeyword>();
			_keywords.UnionWith(CanonicalKeywords);
			return _keywords;
		}
	}

	public IReadOnlySet<CardKeyword> Keywords => GetKeywordsWithSources(KeywordSources.All);

	public virtual IEnumerable<CardTag> Tags => _tags ?? (_tags = CanonicalTags);

	protected virtual HashSet<CardTag> CanonicalTags => new HashSet<CardTag>();

	public DynamicVarSet DynamicVars
	{
		get
		{
			if (_dynamicVars != null)
			{
				return _dynamicVars;
			}
			_dynamicVars = new DynamicVarSet(CanonicalVars);
			_dynamicVars.InitializeWithOwner(this);
			return _dynamicVars;
		}
	}

	protected virtual IEnumerable<DynamicVar> CanonicalVars => Array.Empty<DynamicVar>();

	public bool ExhaustOnNextPlay
	{
		get
		{
			return _exhaustOnNextPlay;
		}
		set
		{
			AssertMutable();
			_exhaustOnNextPlay = value;
		}
	}

	private bool HasSingleTurnRetain
	{
		get
		{
			return _hasSingleTurnRetain;
		}
		set
		{
			AssertMutable();
			_hasSingleTurnRetain = value;
		}
	}

	public bool ShouldRetainThisTurn
	{
		get
		{
			if (!Keywords.Contains(CardKeyword.Retain))
			{
				return HasSingleTurnRetain;
			}
			return true;
		}
	}

	private bool HasSingleTurnSly
	{
		get
		{
			return _hasSingleTurnSly;
		}
		set
		{
			AssertMutable();
			_hasSingleTurnSly = value;
		}
	}

	public bool IsSlyThisTurn
	{
		get
		{
			if (!Keywords.Contains(CardKeyword.Sly))
			{
				return HasSingleTurnSly;
			}
			return true;
		}
	}

	public EnchantmentModel? Enchantment { get; private set; }

	public AfflictionModel? Affliction { get; private set; }

	public virtual bool CanBeGeneratedInCombat => true;

	public virtual bool CanBeGeneratedByModifiers => true;

	public virtual OrbEvokeType OrbEvokeType => OrbEvokeType.None;

	public virtual bool GainsBlock => false;

	public virtual bool IsBasicStrikeOrDefend
	{
		get
		{
			if (Rarity != CardRarity.Basic)
			{
				return false;
			}
			if (Tags.Contains(CardTag.Strike))
			{
				return true;
			}
			if (Tags.Contains(CardTag.Defend))
			{
				return true;
			}
			return false;
		}
	}

	public CardModel? CloneOf => _cloneOf;

	public bool IsClone => CloneOf != null;

	public CardModel? DupeOf
	{
		get
		{
			if (!IsDupe)
			{
				return null;
			}
			return CloneOf;
		}
	}

	public bool IsDupe
	{
		get
		{
			return _isDupe;
		}
		private set
		{
			AssertMutable();
			_isDupe = value;
		}
	}

	public bool IsRemovable => !Keywords.Contains(CardKeyword.Eternal);

	public bool IsTransformable
	{
		get
		{
			if (!IsRemovable)
			{
				CardPile pile = Pile;
				return pile == null || pile.Type != PileType.Deck;
			}
			return true;
		}
	}

	public bool IsInCombat
	{
		get
		{
			if (IsMutable)
			{
				return Pile?.IsCombatPile ?? false;
			}
			return false;
		}
	}

	public int CurrentUpgradeLevel
	{
		get
		{
			return _currentUpgradeLevel;
		}
		private set
		{
			AssertMutable();
			if (value > MaxUpgradeLevel)
			{
				throw new InvalidOperationException($"{Id} cannot be upgraded past its MaxUpgradeLevel.");
			}
			_currentUpgradeLevel = value;
		}
	}

	public virtual int MaxUpgradeLevel => 1;

	public bool IsUpgraded => CurrentUpgradeLevel > 0;

	public bool IsUpgradable
	{
		get
		{
			if (CurrentUpgradeLevel >= MaxUpgradeLevel)
			{
				return false;
			}
			return true;
		}
	}

	public CardUpgradePreviewType UpgradePreviewType
	{
		get
		{
			return _upgradePreviewType;
		}
		set
		{
			AssertMutable();
			if (!value.IsPreview() && _upgradePreviewType.IsPreview())
			{
				throw new InvalidOperationException("A card cannot go to from being upgrade preview. Consider making a new card model instead.");
			}
			_upgradePreviewType = value;
		}
	}

	protected virtual bool IsPlayable => true;

	public bool ShouldShowInCardLibrary { get; }

	public bool ShouldGlowGold
	{
		get
		{
			if (!ShouldGlowGoldInternal)
			{
				return Enchantment?.ShouldGlowGold ?? false;
			}
			return true;
		}
	}

	public bool ShouldGlowRed
	{
		get
		{
			if (!ShouldGlowRedInternal)
			{
				return Enchantment?.ShouldGlowRed ?? false;
			}
			return true;
		}
	}

	protected virtual bool ShouldGlowGoldInternal => false;

	protected virtual bool ShouldGlowRedInternal => false;

	public bool IsEnchantmentPreview
	{
		get
		{
			return _isEnchantmentPreview;
		}
		set
		{
			AssertMutable();
			_isEnchantmentPreview = value;
		}
	}

	public virtual bool HasBuiltInOverlay => false;

	public string OverlayPath => SceneHelper.GetScenePath("cards/overlays/" + Id.Entry.ToLowerInvariant());

	public int? FloorAddedToDeck
	{
		get
		{
			return _floorAddedToDeck;
		}
		set
		{
			AssertMutable();
			_floorAddedToDeck = value;
		}
	}

	public Creature? CurrentTarget
	{
		get
		{
			return _currentTarget;
		}
		private set
		{
			AssertMutable();
			_currentTarget = value;
		}
	}

	public int CurrentPlayIndex
	{
		get
		{
			return _currentPlayIndex;
		}
		private set
		{
			AssertMutable();
			_currentPlayIndex = value;
		}
	}

	public CardModel? DeckVersion
	{
		get
		{
			return _deckVersion;
		}
		set
		{
			AssertMutable();
			_deckVersion = value;
		}
	}

	public CardModel? TransformIntoAfterPlay
	{
		get
		{
			return _transformIntoAfterPlay;
		}
		protected set
		{
			AssertMutable();
			_transformIntoAfterPlay = value;
		}
	}

	public bool HasBeenRemovedFromState { get; set; }

	protected virtual IEnumerable<IHoverTip> ExtraHoverTips => Array.Empty<IHoverTip>();

	public IEnumerable<IHoverTip> HoverTips
	{
		get
		{
			List<IHoverTip> list = ExtraHoverTips.ToList();
			if (Enchantment != null)
			{
				list.AddRange(Enchantment.HoverTips);
			}
			if (Affliction != null)
			{
				list.AddRange(Affliction.HoverTips);
			}
			int enchantedReplayCount = GetEnchantedReplayCount();
			if (enchantedReplayCount > 0)
			{
				list.Add(HoverTipFactory.Static(StaticHoverTip.ReplayDynamic, new DynamicVar("Times", enchantedReplayCount)));
			}
			if (OrbEvokeType != OrbEvokeType.None)
			{
				list.Add(HoverTipFactory.Static(StaticHoverTip.Evoke));
			}
			if (GainsBlock)
			{
				list.Add(HoverTipFactory.Static(StaticHoverTip.Block));
			}
			foreach (CardKeyword keyword in Keywords)
			{
				list.Add(HoverTipFactory.FromKeyword(keyword));
				if (keyword == CardKeyword.Ethereal)
				{
					list.Add(HoverTipFactory.FromKeyword(CardKeyword.Exhaust));
				}
			}
			return list.Distinct();
		}
	}

	public CardModel CanonicalInstance
	{
		get
		{
			if (!IsMutable)
			{
				return this;
			}
			return _canonicalInstance;
		}
		private set
		{
			AssertMutable();
			_canonicalInstance = value;
		}
	}

	public IRunState? RunState => _owner?.RunState;

	public ICombatState? CombatState
	{
		get
		{
			CardPile pile = Pile;
			if ((pile != null && pile.IsCombatPile) || UpgradePreviewType == CardUpgradePreviewType.Combat)
			{
				return _owner?.Creature.CombatState;
			}
			return null;
		}
	}

	public ICardScope? CardScope => ((ICardScope)CombatState) ?? ((ICardScope)(_owner?.Creature.CombatState)) ?? RunState;

	public virtual bool HasTurnEndInHandEffect => false;

	public override bool ShouldReceiveCombatHooks => Pile?.IsCombatPile ?? false;

	public virtual IEnumerable<string> AllPortraitPaths => new _003C_003Ez__ReadOnlySingleElementList<string>(PortraitPath);

	public IEnumerable<string> RunAssetPaths => ExtraRunAssetPaths;

	protected virtual IEnumerable<string> ExtraRunAssetPaths => Array.Empty<string>();

	public event Action? AfflictionChanged;

	public event Action? EnchantmentChanged;

	public event Action? EnergyCostChanged;

	public event Action? KeywordsChanged;

	public event Action? ReplayCountChanged;

	public event Action? Played;

	public event Action? Drawn;

	public event Action? StarCostChanged;

	public event Action? Upgraded;

	public event Action? Forged;

	protected CardModel(int canonicalEnergyCost, CardType type, CardRarity rarity, TargetType targetType, bool shouldShowInCardLibrary = true)
	{
		CanonicalEnergyCost = canonicalEnergyCost;
		Type = type;
		Rarity = rarity;
		TargetType = targetType;
		ShouldShowInCardLibrary = shouldShowInCardLibrary;
	}

	public bool IsAllowedInRun(IRunState runState)
	{
		if (runState.Players.Count > 1)
		{
			if (MultiplayerConstraint == CardMultiplayerConstraint.SingleplayerOnly)
			{
				return false;
			}
		}
		else if (MultiplayerConstraint == CardMultiplayerConstraint.MultiplayerOnly)
		{
			return false;
		}
		if (runState.GameMode == GameMode.Blitz && !IsAllowedInBlitz)
		{
			return false;
		}
		return true;
	}

	protected void MockSetEnergyCost(CardEnergyCost cost)
	{
		_energyCost = cost;
	}

	public void InvokeEnergyCostChanged()
	{
		EnergyCostChanged?.Invoke();
	}

	public int ResolveEnergyXValue()
	{
		if (!EnergyCost.CostsX)
		{
			throw new InvalidOperationException("This card does not have an X-cost.");
		}
		return Hook.ModifyXValue(CombatState, this, EnergyCost.CapturedXValue);
	}

	public int GetEnchantedReplayCount()
	{
		return Enchantment?.EnchantPlayCount(BaseReplayCount + ExtraReplayThisTurn) ?? (BaseReplayCount + ExtraReplayThisTurn);
	}

	public int ResolveStarXValue()
	{
		if (!HasStarCostX)
		{
			throw new InvalidOperationException("This card does not have an X-cost.");
		}
		return Hook.ModifyXValue(CombatState, this, LastStarsSpent);
	}

	public IReadOnlySet<CardKeyword> GetKeywordsWithSources(KeywordSources sources)
	{
		bool flag = sources.HasFlag(KeywordSources.Local);
		if (!sources.HasFlag(KeywordSources.Global) || IsCanonical || CombatState == null)
		{
			if (!flag)
			{
				return ImmutableHashSet<CardKeyword>.Empty;
			}
			return LocalKeywords;
		}
		HashSet<CardKeyword> hashSet2;
		if (flag)
		{
			HashSet<CardKeyword> hashSet = new HashSet<CardKeyword>();
			foreach (CardKeyword localKeyword in LocalKeywords)
			{
				hashSet.Add(localKeyword);
			}
			hashSet2 = hashSet;
		}
		else
		{
			hashSet2 = new HashSet<CardKeyword>();
		}
		HashSet<CardKeyword> hashSet3 = hashSet2;
		Hook.ModifyKeywordsInCombat(CombatState, this, hashSet3);
		return hashSet3;
	}

	public Control CreateOverlay()
	{
		return PreloadManager.Cache.GetScene(OverlayPath).Instantiate<Control>(PackedScene.GenEditState.Disabled);
	}

	public CardModel ToMutable()
	{
		AssertCanonical();
		return (CardModel)MutableClone();
	}

	protected override void DeepCloneFields()
	{
		HashSet<CardKeyword> hashSet = new HashSet<CardKeyword>();
		foreach (CardKeyword keywordsWithSource in GetKeywordsWithSources(KeywordSources.Local))
		{
			hashSet.Add(keywordsWithSource);
		}
		_keywords = hashSet;
		HashSet<CardKeyword> hashSet2 = new HashSet<CardKeyword>();
		foreach (CardKeyword item in _keywordsAddedFromOutside)
		{
			hashSet2.Add(item);
		}
		_keywordsAddedFromOutside = hashSet2;
		HashSet<CardKeyword> hashSet3 = new HashSet<CardKeyword>();
		foreach (CardKeyword item2 in _keywordsRemovedFromOutside)
		{
			hashSet3.Add(item2);
		}
		_keywordsRemovedFromOutside = hashSet3;
		_dynamicVars = DynamicVars.Clone(this);
		_energyCost = _energyCost?.Clone(this);
		_temporaryStarCosts = _temporaryStarCosts.ToList();
		if (Enchantment != null)
		{
			EnchantmentModel enchantmentModel = (EnchantmentModel)Enchantment.ClonePreservingMutability();
			Enchantment = null;
			EnchantInternal(enchantmentModel, enchantmentModel.Amount);
		}
		if (Affliction != null)
		{
			AfflictionModel afflictionModel = (AfflictionModel)Affliction.ClonePreservingMutability();
			Affliction = null;
			AfflictInternal(afflictionModel, afflictionModel.Amount);
		}
	}

	protected override void AfterCloned()
	{
		base.AfterCloned();
		if (_canonicalInstance == null)
		{
			_canonicalInstance = ModelDb.GetById<CardModel>(Id);
		}
		CurrentTarget = null;
		CurrentPlayIndex = 0;
		DeckVersion = null;
		HasBeenRemovedFromState = false;
		TransformIntoAfterPlay = null;
		_isUpgrading = false;
		AfflictionChanged = null;
		Drawn = null;
		EnchantmentChanged = null;
		EnergyCostChanged = null;
		Forged = null;
		KeywordsChanged = null;
		Played = null;
		ReplayCountChanged = null;
		StarCostChanged = null;
		Upgraded = null;
	}

	public virtual void AfterCreated()
	{
	}

	protected virtual void AfterDeserialized()
	{
	}

	protected void NeverEverCallThisOutsideOfTests_ClearOwner()
	{
		if (TestMode.IsOff)
		{
			throw new InvalidOperationException("You monster!");
		}
		_owner = null;
	}

	public void SetToFreeThisTurn()
	{
		EnergyCost.SetThisTurnOrUntilPlayed(0);
		SetStarCostThisTurn(0);
	}

	public void SetToFreeThisCombat()
	{
		EnergyCost.SetThisCombat(0);
		SetStarCostThisCombat(0);
	}

	public void SetStarCostUntilPlayed(int cost)
	{
		AddTemporaryStarCost(TemporaryCardCost.UntilPlayed(cost));
	}

	public void SetStarCostThisTurn(int cost)
	{
		AddTemporaryStarCost(TemporaryCardCost.ThisTurn(cost));
	}

	public void SetStarCostThisCombat(int cost)
	{
		AddTemporaryStarCost(TemporaryCardCost.ThisCombat(cost));
	}

	public int GetStarCostThisCombat()
	{
		return _temporaryStarCosts.FirstOrDefault((TemporaryCardCost cost) => cost != null && !cost.ClearsWhenTurnEnds && !cost.ClearsWhenCardIsPlayed)?.Cost ?? BaseStarCost;
	}

	private void AddTemporaryStarCost(TemporaryCardCost cost)
	{
		AssertMutable();
		_temporaryStarCosts.Add(cost);
		StarCostChanged?.Invoke();
	}

	protected void UpgradeStarCostBy(int addend)
	{
		if (HasStarCostX)
		{
			throw new InvalidOperationException("UpgradeStarCostBy called on " + Id.Entry + " which has star cost X.");
		}
		if (addend == 0)
		{
			return;
		}
		int baseStarCost = BaseStarCost;
		BaseStarCost += addend;
		_wasStarCostJustUpgraded = true;
		if (BaseStarCost < baseStarCost)
		{
			_temporaryStarCosts.RemoveAll((TemporaryCardCost c) => c.Cost > BaseStarCost);
		}
	}

	public void AddKeyword(CardKeyword keyword)
	{
		AssertMutable();
		LocalKeywords.Add(keyword);
		if (!_isUpgrading)
		{
			_keywordsAddedFromOutside.Add(keyword);
			_keywordsRemovedFromOutside.Remove(keyword);
		}
		KeywordsChanged?.Invoke();
	}

	public void RemoveKeyword(CardKeyword keyword)
	{
		AssertMutable();
		LocalKeywords.Remove(keyword);
		if (!_isUpgrading)
		{
			_keywordsRemovedFromOutside.Add(keyword);
			_keywordsAddedFromOutside.Remove(keyword);
		}
		KeywordsChanged?.Invoke();
	}

	public void InvokeKeywordsChanged()
	{
		KeywordsChanged?.Invoke();
	}

	public void GiveSingleTurnRetain()
	{
		HasSingleTurnRetain = true;
	}

	public void GiveSingleTurnSly()
	{
		HasSingleTurnSly = true;
	}

	public string GetDescriptionForPile(PileType pileType, Creature? target = null)
	{
		return GetDescriptionForPile(pileType, DescriptionPreviewType.None, target);
	}

	public string GetDescriptionForUpgradePreview()
	{
		return GetDescriptionForPile(PileType.None, DescriptionPreviewType.Upgrade);
	}

	private string GetDescriptionForPile(PileType pileType, DescriptionPreviewType previewType, Creature? target = null)
	{
		LocString description = Description;
		DynamicVars.AddTo(description);
		AddExtraArgsToDescription(description);
		UpgradeDisplay upgradeDisplay = ((previewType == DescriptionPreviewType.Upgrade) ? UpgradeDisplay.UpgradePreview : (IsUpgraded ? UpgradeDisplay.Upgraded : UpgradeDisplay.Normal));
		description.Add(new IfUpgradedVar(upgradeDisplay));
		bool flag = ((pileType == PileType.Hand || pileType == PileType.Play) ? true : false);
		bool variable = flag;
		description.Add("OnTable", variable);
		bool variable2 = CombatManager.Instance.IsInProgress && (Pile?.IsCombatPile ?? pileType.IsCombatPile());
		description.Add("InCombat", variable2);
		description.Add("IsTargeting", target != null);
		description.Add("TargetType", TargetType.ToString());
		description.Add("GainsBlock", GainsBlock);
		description.Add("IsOstyAlive", IsMutable && (Owner?.IsOstyAlive ?? false));
		string prefix = EnergyIconHelper.GetPrefix(this);
		description.Add("energyPrefix", prefix);
		description.Add("singleStarIcon", "[img]res://images/packed/sprite_fonts/star_icon.png[/img]");
		foreach (KeyValuePair<string, object> variable3 in description.Variables)
		{
			if (variable3.Value is EnergyVar energyVar)
			{
				energyVar.ColorPrefix = prefix;
			}
		}
		int num = 1;
		List<string> list = new List<string>(num);
		CollectionsMarshal.SetCount(list, num);
		Span<string> span = CollectionsMarshal.AsSpan(list);
		int index = 0;
		span[index] = description.GetFormattedText();
		List<string> list2 = list;
		LocString locString = Enchantment?.DynamicExtraCardText;
		if (locString != null)
		{
			list2.Add("[purple]" + locString.GetFormattedText() + "[/purple]");
		}
		LocString locString2 = Affliction?.DynamicExtraCardText;
		if (locString2 != null)
		{
			list2.Add("[purple]" + locString2.GetFormattedText() + "[/purple]");
		}
		IReadOnlySet<CardKeyword> keywords = Keywords;
		CardKeyword[] beforeDescription = CardKeywordOrder.beforeDescription;
		foreach (CardKeyword cardKeyword in beforeDescription)
		{
			if (cardKeyword switch
			{
				CardKeyword.Sly => IsSlyThisTurn,
				CardKeyword.Retain => ShouldRetainThisTurn,
				_ => keywords.Contains(cardKeyword),
			})
			{
				list2.Insert(0, cardKeyword.GetCardText());
			}
		}
		int enchantedReplayCount = GetEnchantedReplayCount();
		if (enchantedReplayCount > 0)
		{
			LocString locString3 = new LocString("static_hover_tips", "REPLAY.extraText");
			locString3.Add("Times", enchantedReplayCount);
			list2.Add(locString3.GetFormattedText() ?? "");
		}
		foreach (CardKeyword item in CardKeywordOrder.afterDescription.Intersect(keywords))
		{
			list2.Add(item.GetCardText());
		}
		return string.Join('\n', list2.Where((string l) => !string.IsNullOrEmpty(l)));
	}

	public void UpdateDynamicVarPreview(CardPreviewMode previewMode, Creature? target, DynamicVarSet dynamicVarSet)
	{
		if (RunState == null && CombatState == null)
		{
			return;
		}
		bool flag = CombatState != null;
		bool flag2 = flag;
		if (flag2)
		{
			bool flag3;
			switch (Pile?.Type)
			{
			case PileType.Hand:
			case PileType.Play:
				flag3 = true;
				break;
			default:
				flag3 = false;
				break;
			}
			flag2 = flag3 || UpgradePreviewType == CardUpgradePreviewType.Combat;
		}
		bool runGlobalHooks = flag2;
		IEnumerable<DynamicVar> enumerable = dynamicVarSet.Values.ToList();
		foreach (DynamicVar item in enumerable)
		{
			item.UpdateCardPreview(this, previewMode, target, runGlobalHooks);
		}
	}

	public void EnchantInternal(EnchantmentModel enchantment, decimal amount)
	{
		AssertMutable();
		enchantment.AssertMutable();
		Enchantment = enchantment;
		Enchantment.ApplyInternal(this, amount);
		EnchantmentChanged?.Invoke();
	}

	public void AfflictInternal(AfflictionModel affliction, decimal amount)
	{
		AssertMutable();
		affliction.AssertMutable();
		if (Affliction != null)
		{
			throw new InvalidOperationException($"Attempted to afflict card {this} that was already afflicted! This is not allowed");
		}
		Affliction = affliction;
		Affliction.Card = this;
		Affliction.Amount = (int)amount;
		AfflictionChanged?.Invoke();
	}

	public void ClearEnchantmentInternal()
	{
		if (Enchantment != null)
		{
			AssertMutable();
			Enchantment.ClearInternal();
			Enchantment = null;
			EnchantmentChanged?.Invoke();
		}
	}

	public void ClearAfflictionInternal()
	{
		AssertMutable();
		if (Affliction != null)
		{
			Affliction.ClearInternal();
			Affliction = null;
			Owner.PlayerCombatState.RecalculateCardValues();
			AfflictionChanged?.Invoke();
		}
	}

	protected virtual void AddExtraArgsToDescription(LocString description)
	{
	}

	public int GetStarCostWithModifiers()
	{
		if (HasStarCostX)
		{
			return Owner.PlayerCombatState?.Stars ?? 0;
		}
		CardPile pile = Pile;
		if (pile != null && pile.IsCombatPile && CombatState != null)
		{
			return (int)Hook.ModifyStarCost(CombatState, this, CurrentStarCost);
		}
		return CurrentStarCost;
	}

	public bool CostsEnergyOrStars(bool includeGlobalModifiers)
	{
		if (includeGlobalModifiers)
		{
			if (!EnergyCost.CostsX && EnergyCost.GetWithModifiers(CostModifiers.All) > 0)
			{
				return true;
			}
			if (!HasStarCostX && GetStarCostWithModifiers() > 0)
			{
				return true;
			}
		}
		else if (EnergyCost.GetWithModifiers(CostModifiers.Local) > 0 || CurrentStarCost > 0)
		{
			return true;
		}
		return false;
	}

	public void RemoveFromCurrentPile(bool silent = false)
	{
		AssertMutable();
		Pile?.RemoveInternal(this, silent);
	}

	public void RemoveFromState()
	{
		RemoveFromCurrentPile();
		HasBeenRemovedFromState = true;
	}

	public void EndOfTurnCleanup()
	{
		ExhaustOnNextPlay = false;
		HasSingleTurnRetain = false;
		HasSingleTurnSly = false;
		if (ExtraReplayThisTurn != 0)
		{
			ExtraReplayThisTurn = 0;
		}
		if (EnergyCost.EndOfTurnCleanup())
		{
			EnergyCostChanged?.Invoke();
		}
		if (_temporaryStarCosts.RemoveAll((TemporaryCardCost c) => c.ClearsWhenTurnEnds) > 0)
		{
			StarCostChanged?.Invoke();
		}
	}

	public void AfterThisWasTransformedInto(CardModel card)
	{
		AfterThisWasTransformedIntoInternal(card);
	}

	protected virtual void AfterThisWasTransformedIntoInternal(CardModel card)
	{
	}

	public virtual void AfterThisWasTransformedFrom(CardModel card)
	{
	}

	public void AfterForged()
	{
		Forged?.Invoke();
	}

	protected virtual Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		return Task.CompletedTask;
	}

	public virtual Task OnEnqueuePlayVfx(Creature? target)
	{
		return Task.CompletedTask;
	}

	protected virtual void OnUpgrade()
	{
	}

	protected virtual Task OnTurnEndInHand(PlayerChoiceContext choiceContext)
	{
		return Task.CompletedTask;
	}

	public Task OnTurnEndInHandWrapper(PlayerChoiceContext choiceContext)
	{
		return OnTurnEndInHand(choiceContext);
	}

	public bool CanPlayTargeting(Creature? target)
	{
		if (!IsValidTarget(target))
		{
			return false;
		}
		return CanPlay();
	}

	public bool CanPlay()
	{
		UnplayableReason reason;
		AbstractModel preventer;
		return CanPlay(out reason, out preventer);
	}

	public bool CanPlay(out UnplayableReason reason, out AbstractModel? preventer)
	{
		reason = UnplayableReason.None;
		ICombatState combatState = CombatState ?? _owner?.Creature.CombatState;
		if (combatState == null || Owner.PlayerCombatState == null)
		{
			preventer = null;
			return false;
		}
		if (Keywords.Contains(CardKeyword.Unplayable))
		{
			reason |= UnplayableReason.HasUnplayableKeyword;
		}
		if (!Owner.PlayerCombatState.HasEnoughResourcesFor(this, out var reason2))
		{
			reason |= reason2;
		}
		if (TargetType == TargetType.AnyAlly && combatState.PlayerCreatures.Count((Creature c) => c.IsAlive) <= 1)
		{
			reason |= UnplayableReason.NoLivingAllies;
		}
		if (!Hook.ShouldPlay(combatState, this, out preventer, AutoPlayType.None))
		{
			reason |= UnplayableReason.BlockedByHook;
		}
		if (!IsPlayable)
		{
			reason |= UnplayableReason.BlockedByCardLogic;
		}
		return reason == UnplayableReason.None;
	}

	public bool IsValidTarget(Creature? target)
	{
		if (target == null)
		{
			if (TargetType != TargetType.AnyEnemy)
			{
				return TargetType != TargetType.AnyAlly;
			}
			return false;
		}
		if (!target.IsAlive)
		{
			return false;
		}
		if (TargetType == TargetType.AnyEnemy)
		{
			return target.Side != Owner.Creature.Side;
		}
		if (TargetType == TargetType.AnyAlly)
		{
			return target.Side == Owner.Creature.Side;
		}
		return false;
	}

	public bool TryManualPlay(Creature? target)
	{
		if (CanPlayTargeting(target))
		{
			EnqueueManualPlay(target);
			return true;
		}
		return false;
	}

	private void EnqueueManualPlay(Creature? target)
	{
		TaskHelper.RunSafely(OnEnqueuePlayVfx(target));
		RunManager.Instance.ActionQueueSynchronizer.RequestEnqueue(new PlayCardAction(this, target));
	}

	public async Task<(int, int)> SpendResources()
	{
		int energy = Owner.PlayerCombatState.Energy;
		int energyToSpend = EnergyCost.GetAmountToSpend();
		int starsToSpend = Math.Max(0, GetStarCostWithModifiers());
		if (energyToSpend > energy && Hook.ShouldPayExcessEnergyCostWithStars(CombatState, Owner))
		{
			starsToSpend += (energyToSpend - energy) * 2;
			energyToSpend = energy;
		}
		await SpendEnergy(energyToSpend);
		await SpendStars(starsToSpend);
		return (energyToSpend, starsToSpend);
	}

	private async Task SpendEnergy(int amount)
	{
		if (EnergyCost.CostsX)
		{
			EnergyCost.CapturedXValue = amount;
		}
		if (amount > 0)
		{
			CombatManager.Instance.History.EnergySpent(CombatState, amount, Owner);
			Owner.PlayerCombatState.LoseEnergy(Math.Max(0, amount));
		}
		await Hook.AfterEnergySpent(CombatState, this, amount);
	}

	private async Task SpendStars(int amount)
	{
		LastStarsSpent = amount;
		if (amount > 0)
		{
			Owner.PlayerCombatState.LoseStars(amount);
			await Hook.AfterStarsSpent(Owner.Creature.CombatState, amount, Owner);
		}
	}

	public async Task OnPlayWrapper(PlayerChoiceContext choiceContext, Creature? target, bool isAutoPlay, ResourceInfo resources, bool skipCardPileVisuals = false)
	{
		choiceContext.PushModel(this);
		await CombatManager.Instance.WaitForUnpause();
		CurrentTarget = target;
		CurrentPlayIndex = 0;
		if (!isAutoPlay)
		{
			await CardPileCmd.AddDuringManualCardPlay(this);
		}
		else
		{
			await CardPileCmd.Add(this, PileType.Play, CardPilePosition.Bottom, null, skipCardPileVisuals);
			if (!skipCardPileVisuals)
			{
				await Cmd.CustomScaledWait(0.25f, 0.35f);
			}
		}
		ICombatState combatState = CombatState;
		if (combatState == null)
		{
			return;
		}
		CardLocation resultLocation = GetResultLocationForCardPlay();
		resultLocation = Hook.ModifyCardPlayResultLocation(combatState, this, isAutoPlay, resources, resultLocation, out IEnumerable<AbstractModel> modifiers);
		foreach (AbstractModel item in modifiers)
		{
			await item.AfterModifyingCardPlayResultLocation(this, resultLocation);
		}
		int playCount = await GeneratePlayCount(combatState, target);
		if (Owner.Creature.IsDead)
		{
			return;
		}
		ulong playStartTime = Time.GetTicksMsec();
		if (TransformIntoAfterPlay != null)
		{
			Log.Error("Card TransformIntoAfterPlay field was set outside of OnPlay! Ignoring");
			TransformIntoAfterPlay = null;
		}
		CombatId? effectCombatId = CombatManager.Instance.BeginCardOrPotionEffect(Owner);
		CardModel card = this;
		try
		{
			for (int i = 0; i < playCount; i++)
			{
				if (CombatManager.Instance.IsOverOrEnding)
				{
					break;
				}
				CurrentPlayIndex = i;
				if (Type == CardType.Power)
				{
					await PlayPowerCardFlyVfx();
				}
				else if (i > 0)
				{
					NCard nCard = NCard.FindOnTable(this);
					if (nCard != null)
					{
						await nCard.AnimMultiCardPlay();
					}
				}
				CardPlay cardPlay = new CardPlay
				{
					Card = this,
					Player = Owner,
					Target = target,
					ResultPile = resultLocation.pileType,
					Resources = resources,
					IsAutoPlay = isAutoPlay,
					PlayIndex = i,
					PlayCount = playCount
				};
				await Hook.BeforeCardPlayed(combatState, cardPlay);
				if (CombatManager.Instance.IsCurrentLiveCombat(effectCombatId))
				{
					CombatManager.Instance.History.CardPlayStarted(combatState, cardPlay);
				}
				BranchingPlayerChoiceContext branchingPlayerChoiceContext = new BranchingPlayerChoiceContext(this, LocalContext.NetId.Value, choiceContext);
				branchingPlayerChoiceContext.PushModel(this);
				Task task = OnPlay(branchingPlayerChoiceContext, cardPlay);
				await branchingPlayerChoiceContext.AssignTaskAndWaitForPauseOrCompletion(task);
				if (Owner.Creature.IsDead)
				{
					return;
				}
				InvokeExecutionFinished();
				if (Enchantment != null)
				{
					EnchantmentModel enchantment = Enchantment;
					await enchantment.OnPlay(choiceContext, cardPlay);
					if (Owner.Creature.IsDead)
					{
						return;
					}
					enchantment.InvokeExecutionFinished();
				}
				if (Affliction != null)
				{
					AfflictionModel affliction = Affliction;
					await affliction.OnPlay(choiceContext, target);
					if (Owner.Creature.IsDead)
					{
						return;
					}
					affliction.InvokeExecutionFinished();
				}
				if (CombatManager.Instance.IsCurrentLiveCombat(effectCombatId))
				{
					CombatManager.Instance.History.CardPlayFinished(combatState, cardPlay);
				}
				if (CombatManager.Instance.IsInProgress)
				{
					await Hook.AfterCardPlayed(combatState, choiceContext, cardPlay);
					if (Owner.Creature.IsDead)
					{
						return;
					}
				}
				if (TransformIntoAfterPlay != null)
				{
					CardPileAddResult? cardPileAddResult = await CardCmd.Transform(this, TransformIntoAfterPlay);
					if (cardPileAddResult.HasValue)
					{
						TransformIntoAfterPlay = null;
						card = cardPileAddResult.Value.cardAdded;
						break;
					}
				}
			}
		}
		finally
		{
			await CombatManager.Instance.EndCardOrPotionEffect(effectCombatId, Owner);
		}
		if (!skipCardPileVisuals)
		{
			float num = (float)(Time.GetTicksMsec() - playStartTime) / 1000f;
			await Cmd.CustomScaledWait(0.15f - num, 0.3f - num);
		}
		Player originalOwner = Owner;
		if (CombatManager.Instance.CurrentCombatId == effectCombatId)
		{
			if (originalOwner != resultLocation.player && resultLocation.pileType != PileType.None)
			{
				await CardPileCmd.GiveToAnotherPlayer(card, resultLocation.player, resultLocation.pileType, resultLocation.position);
			}
			CardPile? pile = card.Pile;
			if (pile != null && pile.Type == PileType.Play)
			{
				switch (resultLocation.pileType)
				{
				case PileType.None:
					await CardPileCmd.RemoveFromCombat(card, skipCardPileVisuals);
					break;
				case PileType.Exhaust:
					await CardCmd.Exhaust(choiceContext, card, causedByEthereal: false, skipCardPileVisuals);
					break;
				default:
					await CardPileCmd.Add(card, resultLocation.pileType, resultLocation.position, null, skipCardPileVisuals);
					break;
				}
			}
		}
		await CombatManager.Instance.CheckForEmptyHand(effectCombatId, choiceContext, originalOwner);
		if (EnergyCost.AfterCardPlayedCleanup())
		{
			EnergyCostChanged?.Invoke();
		}
		if (_temporaryStarCosts.RemoveAll((TemporaryCardCost c) => c.ClearsWhenCardIsPlayed) > 0)
		{
			StarCostChanged?.Invoke();
		}
		CurrentTarget = null;
		CurrentPlayIndex = 0;
		Played?.Invoke();
		choiceContext.PopModel(this);
	}

	protected async Task<int> GeneratePlayCount(ICombatState combatState, Creature? target)
	{
		int playCount = GetEnchantedReplayCount() + 1;
		playCount = Hook.ModifyCardPlayCount(combatState, this, playCount, target, out List<AbstractModel> modifyingModels);
		await Hook.AfterModifyingCardPlayCount(combatState, this, modifyingModels);
		return playCount;
	}

	private async Task PlayPowerCardFlyVfx()
	{
		NCard node = NCard.FindOnTable(this);
		bool flag = false;
		if (node != null)
		{
			foreach (NCardFlyPowerVfx item in NCombatRoom.Instance.CombatVfxContainer.GetChildren().OfType<NCardFlyPowerVfx>())
			{
				if (item.CardNode == node)
				{
					flag = true;
					break;
				}
			}
		}
		if ((node == null) | flag)
		{
			node = NCard.Create(this);
			if (node != null)
			{
				Tween tween = node.CreateTween();
				tween.Parallel().TweenProperty(node, "scale", Vector2.One * 1f, 0.10000000149011612).From(Vector2.Zero)
					.SetEase(Tween.EaseType.Out)
					.SetTrans(Tween.TransitionType.Cubic);
				NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(node);
				node.GlobalPosition = PileType.Play.GetTargetPosition(node);
				node.UpdateVisuals(PileType.Play, CardPreviewMode.Normal);
			}
			await Cmd.CustomScaledWait(0.1f, 0.8f);
		}
		if (node != null)
		{
			NCardFlyPowerVfx nCardFlyPowerVfx = NCardFlyPowerVfx.Create(node);
			NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(nCardFlyPowerVfx);
			TaskHelper.RunSafely(nCardFlyPowerVfx.PlayAnim());
			float duration = nCardFlyPowerVfx.GetDuration();
			await Cmd.CustomScaledWait(duration * 0.2f, duration);
		}
	}

	protected virtual CardLocation GetResultLocationForCardPlay()
	{
		if (IsDupe || Type == CardType.Power)
		{
			return new CardLocation(Owner, PileType.None, CardPilePosition.Bottom);
		}
		if (ExhaustOnNextPlay || Keywords.Contains(CardKeyword.Exhaust))
		{
			ExhaustOnNextPlay = false;
			return new CardLocation(Owner, PileType.Exhaust, CardPilePosition.Bottom);
		}
		return new CardLocation(Owner, PileType.Discard, CardPilePosition.Bottom);
	}

	public async Task MoveToResultPileWithoutPlaying(PlayerChoiceContext choiceContext)
	{
		CardPile? pile = Pile;
		if (pile != null && pile.Type == PileType.Play)
		{
			if (IsDupe)
			{
				await CardPileCmd.RemoveFromCombat(this);
			}
			else if (ExhaustOnNextPlay || Keywords.Contains(CardKeyword.Exhaust))
			{
				await CardCmd.Exhaust(choiceContext, this);
			}
			else
			{
				await CardPileCmd.Add(this, PileType.Discard);
			}
		}
	}

	public void UpgradeInternal()
	{
		AssertMutable();
		CurrentUpgradeLevel++;
		_isUpgrading = true;
		try
		{
			OnUpgrade();
		}
		finally
		{
			_isUpgrading = false;
		}
		DynamicVars.RecalculateForUpgradeOrEnchant();
		Upgraded?.Invoke();
	}

	public void FinalizeUpgradeInternal()
	{
		DynamicVars.FinalizeUpgrade();
		EnergyCost.FinalizeUpgrade();
		_wasStarCostJustUpgraded = false;
	}

	public void DowngradeInternal()
	{
		AssertMutable();
		CurrentUpgradeLevel = 0;
		CardModel cardModel = ModelDb.GetById<CardModel>(Id).ToMutable();
		_dynamicVars = cardModel.DynamicVars.Clone(this);
		_dynamicVars.InitializeWithOwner(this);
		EnergyCost.ResetForDowngrade();
		_baseStarCost = cardModel.CanonicalStarCost;
		HashSet<CardKeyword> hashSet = new HashSet<CardKeyword>();
		foreach (CardKeyword keywordsWithSource in cardModel.GetKeywordsWithSources(KeywordSources.Local))
		{
			hashSet.Add(keywordsWithSource);
		}
		HashSet<CardKeyword> hashSet2 = hashSet;
		hashSet2.UnionWith(_keywordsAddedFromOutside);
		hashSet2.ExceptWith(_keywordsRemovedFromOutside);
		_keywords = hashSet2;
		AfterDowngraded();
		Enchantment?.ModifyCard();
		Affliction?.AfterApplied();
		Upgraded?.Invoke();
	}

	protected virtual void AfterDowngraded()
	{
	}

	public void InvokeDrawn()
	{
		Drawn?.Invoke();
	}

	public CardModel CreateClone()
	{
		if (Pile != null && !Pile.Type.IsCombatPile())
		{
			throw new InvalidOperationException("Cannot create a clone of a card that is not in a combat pile.");
		}
		AssertMutable();
		CardModel cardModel = this;
		if (cardModel != null && cardModel.IsDupe && cardModel.DupeOf != null)
		{
			cardModel = cardModel.DupeOf;
		}
		CardModel cardModel2 = CardScope.CloneCard(cardModel);
		cardModel2._cloneOf = cardModel;
		cardModel2.ExhaustOnNextPlay = false;
		return cardModel2;
	}

	public CardModel CreateCloneForPlayer(Player player)
	{
		CardModel cardModel = CreateClone();
		cardModel._owner = player;
		return cardModel;
	}

	public void GiveToAnotherPlayer(Player player)
	{
		_owner = player;
	}

	public CardModel CreateDupe(Player newOwner)
	{
		if (IsDupe)
		{
			return DupeOf.CreateDupe(newOwner);
		}
		AssertMutable();
		CardModel cardModel = CreateCloneForPlayer(newOwner);
		cardModel.IsDupe = true;
		cardModel.RemoveKeyword(CardKeyword.Exhaust);
		return cardModel;
	}

	public SerializableCard ToSerializable()
	{
		AssertMutable();
		return new SerializableCard
		{
			Id = Id,
			CurrentUpgradeLevel = CurrentUpgradeLevel,
			Props = SavedProperties.From(this),
			Enchantment = Enchantment?.ToSerializable(),
			FloorAddedToDeck = FloorAddedToDeck
		};
	}

	public static CardModel FromSerializable(SerializableCard save)
	{
		CardModel cardModel = SaveUtil.CardOrDeprecated(save.Id).ToMutable();
		save.Props?.Fill(cardModel);
		if (save.FloorAddedToDeck.HasValue)
		{
			cardModel.FloorAddedToDeck = save.FloorAddedToDeck;
		}
		cardModel.AfterDeserialized();
		if (!(cardModel is DeprecatedCard))
		{
			if (save.Enchantment != null)
			{
				cardModel.EnchantInternal(EnchantmentModel.FromSerializable(save.Enchantment), save.Enchantment.Amount);
				cardModel.Enchantment.ModifyCard();
				cardModel.FinalizeUpgradeInternal();
			}
			for (int i = 0; i < save.CurrentUpgradeLevel; i++)
			{
				cardModel.UpgradeInternal();
				cardModel.FinalizeUpgradeInternal();
			}
		}
		return cardModel;
	}

	public override int CompareTo(AbstractModel? other)
	{
		if (this == other)
		{
			return 0;
		}
		if (other == null)
		{
			return 1;
		}
		int num = base.CompareTo(other);
		if (num != 0)
		{
			return num;
		}
		CardModel cardModel = (CardModel)other;
		int num2 = CurrentUpgradeLevel.CompareTo(cardModel.CurrentUpgradeLevel);
		if (num2 != 0)
		{
			return num2;
		}
		int num3 = ModelId.Compare(Enchantment?.Id, cardModel.Enchantment?.Id);
		if (num3 != 0)
		{
			return num3;
		}
		int num4 = ModelId.Compare(Affliction?.Id, cardModel.Affliction?.Id);
		if (num4 != 0)
		{
			return num4;
		}
		int num5 = EnergyCost.GetResolved().CompareTo(cardModel.EnergyCost.GetResolved());
		if (num5 != 0)
		{
			return num5;
		}
		int num6 = BaseReplayCount.CompareTo(cardModel.BaseReplayCount);
		if (num6 != 0)
		{
			return num6;
		}
		foreach (string item in DynamicVars.Keys.Order(StringComparer.Ordinal))
		{
			if (cardModel.DynamicVars.TryGetValue(item, out DynamicVar value))
			{
				int num7 = DynamicVars[item].BaseValue.CompareTo(value.BaseValue);
				if (num7 != 0)
				{
					return num7;
				}
			}
		}
		return 0;
	}
}
