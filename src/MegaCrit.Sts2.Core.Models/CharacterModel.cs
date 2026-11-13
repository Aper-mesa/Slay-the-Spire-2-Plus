using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using MegaCrit.Sts2.Core.Achievements;
using MegaCrit.Sts2.Core.Animation;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.Saves;
using MegaCrit.Sts2.Core.Unlocks;

namespace MegaCrit.Sts2.Core.Models;

public abstract class CharacterModel : AbstractModel
{
	public const string locTable = "characters";

	protected const string _relaxedTrigger = "Relaxed";

	protected const string _idleTrigger = "Idle";

	public const string relaxedAnim = "relaxed_loop";

	public virtual bool IsPlayable => true;

	public LocString ShortTitle => new LocString("characters", Id.Entry + ".shortTitle");

	public LocString Title => new LocString("characters", Id.Entry + ".title");

	public LocString TitleObject => new LocString("characters", Id.Entry + ".titleObject");

	public abstract Color NameColor { get; }

	public abstract CharacterGender Gender { get; }

	protected abstract CharacterModel? UnlocksAfterRunAs { get; }

	public LocString BestiarySeenQuote => new LocString("characters", Id.Entry + ".bestiaryQuote");

	public LocString? BestiaryKillQuote => LocString.GetIfExists("characters", Id.Entry + ".bestiaryKillQuote");

	public LocString CardsModifierTitle => new LocString("characters", Id.Entry + ".cardsModifierTitle");

	public LocString CardsModifierDescription => new LocString("characters", Id.Entry + ".cardsModifierDescription");

	public LocString EventDeathPreventionLine => new LocString("characters", Id.Entry + ".eventDeathPrevention");

	public virtual string TrailPath => SceneHelper.GetScenePath("vfx/card_trail_" + Id.Entry.ToLowerInvariant());

	public abstract int StartingHp { get; }

	public abstract int StartingGold { get; }

	public virtual int MaxEnergy => 3;

	public virtual Color EnergyLabelOutlineColor => new Color("0000000D");

	public virtual int BaseOrbSlotCount => 0;

	public virtual int BaseArrowSlotCount => 0;

	public virtual bool ShouldAlwaysShowStarCounter => false;

	public abstract CardPoolModel CardPool { get; }

	public abstract RelicPoolModel RelicPool { get; }

	public abstract PotionPoolModel PotionPool { get; }

	public abstract IEnumerable<CardModel> StartingDeck { get; }

	public abstract IReadOnlyList<RelicModel> StartingRelics { get; }

	public virtual IReadOnlyList<PotionModel> StartingPotions => Array.Empty<PotionModel>();

	protected virtual string VisualsPath => SceneHelper.GetScenePath("creature_visuals/" + Id.Entry.ToLowerInvariant());

	protected virtual string IconTexturePath => ImageHelper.GetImagePath("ui/top_panel/character_icon_" + Id.Entry.ToLowerInvariant() + ".png");

	public virtual Texture2D IconTexture => PreloadManager.Cache.GetTexture2D(IconTexturePath);

	protected virtual string IconOutlineTexturePath => ImageHelper.GetImagePath("ui/top_panel/character_icon_" + Id.Entry.ToLowerInvariant() + "_outline.png");

	public virtual Texture2D IconOutlineTexture => PreloadManager.Cache.GetTexture2D(IconOutlineTexturePath);

	protected virtual string IconPath => SceneHelper.GetScenePath("ui/character_icons/" + Id.Entry.ToLowerInvariant() + "_icon");

	public virtual Control Icon => PreloadManager.Cache.GetScene(IconPath).Instantiate<Control>(PackedScene.GenEditState.Disabled);

	public virtual string EnergyCounterPath => SceneHelper.GetScenePath("combat/energy_counters/" + Id.Entry.ToLowerInvariant() + "_energy_counter");

	public virtual string MerchantAnimPath => SceneHelper.GetScenePath("merchant/characters/" + Id.Entry.ToLowerInvariant() + "_merchant");

	public virtual string RestSiteAnimPath => SceneHelper.GetScenePath("rest_site/characters/" + Id.Entry.ToLowerInvariant() + "_rest_site");

	protected virtual string ArmPointingTexturePath => ImageHelper.GetImagePath("ui/hands/multiplayer_hand_" + Id.Entry.ToLowerInvariant() + "_point.png");

	public Texture2D ArmPointingTexture => PreloadManager.Cache.GetTexture2D(ArmPointingTexturePath);

	protected virtual string ArmRockTexturePath => ImageHelper.GetImagePath("ui/hands/multiplayer_hand_" + Id.Entry.ToLowerInvariant() + "_rock.png");

	public Texture2D ArmRockTexture => PreloadManager.Cache.GetTexture2D(ArmRockTexturePath);

	protected virtual string ArmPaperTexturePath => ImageHelper.GetImagePath("ui/hands/multiplayer_hand_" + Id.Entry.ToLowerInvariant() + "_paper.png");

	public Texture2D ArmPaperTexture => PreloadManager.Cache.GetTexture2D(ArmPaperTexturePath);

	protected virtual string ArmScissorsTexturePath => ImageHelper.GetImagePath("ui/hands/multiplayer_hand_" + Id.Entry.ToLowerInvariant() + "_scissors.png");

	public Texture2D ArmScissorsTexture => PreloadManager.Cache.GetTexture2D(ArmScissorsTexturePath);

	public virtual Achievement RunWonAchievement => Enum.Parse<Achievement>(Id.Entry.Capitalize() + "Win");

	protected virtual IEnumerable<string> ExtraAssetPaths => Array.Empty<string>();

	public string CharacterSelectTitle => Id.Entry.ToUpperInvariant() + ".title";

	public string CharacterSelectDesc => Id.Entry.ToUpperInvariant() + ".description";

	public virtual Control CreateCharacterSelectBackground() => PreloadManager.Cache.GetScene(CharacterSelectBg).Instantiate<Control>(PackedScene.GenEditState.Disabled);

	public virtual string CharacterSelectBg => SceneHelper.GetScenePath("screens/char_select/char_select_bg_" + Id.Entry.ToLowerInvariant());

	protected virtual string CharacterSelectIconPath => ImageHelper.GetImagePath("packed/character_select/char_select_" + Id.Entry.ToLowerInvariant() + ".png");

	public CompressedTexture2D CharacterSelectIcon => ResourceLoader.Load<CompressedTexture2D>(CharacterSelectIconPath, null, ResourceLoader.CacheMode.Reuse);

	protected virtual string CharacterSelectLockedIconPath => ImageHelper.GetImagePath("packed/character_select/char_select_" + Id.Entry.ToLowerInvariant() + "_locked.png");

	public CompressedTexture2D CharacterSelectLockedIcon => ResourceLoader.Load<CompressedTexture2D>(CharacterSelectLockedIconPath, null, ResourceLoader.CacheMode.Reuse);

	public virtual string CharacterSelectTransitionPath => "res://materials/transitions/" + Id.Entry.ToLowerInvariant() + "_transition_mat.tres";

	public virtual float DreadBgBrightness => 0.7f;

	public virtual float DreadBgContrast => 1.1f;

	public virtual Color DreadNameColor => new Color("ffd500c0");

	protected virtual string MapMarkerPath => ImageHelper.GetImagePath("packed/map/icons/map_marker_" + Id.Entry.ToLowerInvariant() + ".png");

	public CompressedTexture2D MapMarker => PreloadManager.Cache.GetCompressedTexture2D(MapMarkerPath);

	public virtual Color DialogueColor { get; } = new Color("28454f");

	public virtual VfxColor SpeechBubbleColor { get; } = VfxColor.Cyan;

	public virtual Color MapDrawingColor => Colors.Black;

	public virtual Color RemoteTargetingLineColor => Colors.Black;

	public virtual Color RemoteTargetingLineOutline => Colors.Black;

	public IEnumerable<string> AssetPathsCharacterSelect => new _003C_003Ez__ReadOnlyArray<string>(new string[5] { CharacterSelectBg, CharacterSelectIconPath, IconTexturePath, CharacterSelectLockedIconPath, CharacterSelectTransitionPath });

	public IEnumerable<string> AssetPaths => new string[9] { VisualsPath, IconTexturePath, IconPath, EnergyCounterPath, RestSiteAnimPath, MerchantAnimPath, CharacterSelectTransitionPath, MapMarkerPath, TrailPath }.Concat(ExtraAssetPaths);

	public abstract float AttackAnimDelay { get; }

	public abstract float CastAnimDelay { get; }

	public virtual float PowerUpAnimDelay => CastAnimDelay;

	public virtual string CharacterSelectSfx => $"event:/sfx/characters/{Id.Entry.ToLowerInvariant()}/{Id.Entry.ToLowerInvariant()}_select";

	public virtual string AttackSfx => $"event:/sfx/characters/{Id.Entry.ToLowerInvariant()}/{Id.Entry.ToLowerInvariant()}_attack";

	public virtual string CastSfx => $"event:/sfx/characters/{Id.Entry.ToLowerInvariant()}/{Id.Entry.ToLowerInvariant()}_cast";

	public virtual string PowerUpSfx => CastSfx;

	public virtual string DeathSfx => $"event:/sfx/characters/{Id.Entry.ToLowerInvariant()}/{Id.Entry.ToLowerInvariant()}_die";

	public virtual string CharacterTransitionSfx => "event:/sfx/ui/wipe_" + Id.Entry.ToLowerInvariant();

	protected Func<Creature, bool> IsLowHealth => (Creature creature) => creature.GetHpPercentRemaining() <= 0.25;

	protected virtual List<(AnimState, string)> AnimationStates => new List<(AnimState, string)>
	{
		(new AnimState("cast"), "Cast"),
		(new AnimState("attack"), "Attack"),
		(new AnimState("hurt"), "Hit"),
		(new AnimState("cast"), "PowerUp")
	};

	public override bool ShouldReceiveCombatHooks => false;

	private LocString GetPronoun(string pronounKey, FoolPronounGuess pronounGuess)
	{
		if (Gender != CharacterGender.Tree)
		{
			return new LocString("characters", Id.Entry + "." + pronounKey);
		}
		return new LocString("characters", $"{Id.Entry}.guess.{pronounGuess.ToLocKey()}.{pronounKey}");
	}

	private CharacterGender GetGrammaticalGender(FoolPronounGuess pronounGuess)
	{
		if (Gender != CharacterGender.Tree)
		{
			return Gender;
		}
		return pronounGuess.ToGrammaticalGender();
	}

	public virtual NCreatureVisuals CreateVisuals()
	{
		return PreloadManager.Cache.GetScene(VisualsPath).Instantiate<NCreatureVisuals>(PackedScene.GenEditState.Disabled);
	}

	public abstract List<string> GetArchitectAttackVfx();

	public virtual CreatureAnimator GenerateAnimator(MegaSprite controller, Creature creature)
	{
		AnimState animState = new AnimState("idle_loop", isLooping: true);
		AnimState state = new AnimState("die");
		AnimState animState2 = new AnimState("low_health_loop", isLooping: true);
		AnimState state2 = new AnimState("relaxed_loop", isLooping: true);
		CreatureAnimator creatureAnimator = new CreatureAnimator(IsLowHealth(creature) ? animState2 : animState, controller);
		creatureAnimator.AddAnyState("Idle", animState, () => !IsLowHealth(creature));
		creatureAnimator.AddAnyState("Idle", animState2, () => IsLowHealth(creature));
		creatureAnimator.AddAnyState("Relaxed", state2);
		creatureAnimator.AddAnyState("Dead", state);
		foreach (var animationState in AnimationStates)
		{
			animationState.Item1.AddNextState(animState, () => !IsLowHealth(creature));
			animationState.Item1.AddNextState(animState2, () => IsLowHealth(creature));
			creatureAnimator.AddAnyState(animationState.Item2, animationState.Item1);
		}
		return creatureAnimator;
	}

	public void AddDetailsTo(LocString str, FoolPronounGuess pronounGuess = FoolPronounGuess.Neutral)
	{
		str.Add("character", Title);
		str.Add("characterObject", TitleObject);
		str.Add("characterGender", GetGrammaticalGender(pronounGuess).ToString().ToLowerInvariant());
		str.Add("possessiveAdjective", GetPronoun("possessiveAdjective", pronounGuess));
		str.Add("pronounObject", GetPronoun("pronounObject", pronounGuess));
		str.Add("pronounPossessive", GetPronoun("pronounPossessive", pronounGuess));
		str.Add("pronounSubject", GetPronoun("pronounSubject", pronounGuess));
	}

	public LocString GetUnlockText()
	{
		LocString locString = new LocString("characters", Id.Entry + ".unlockText");
		LocString locString2 = new LocString("characters", "LOCKED.title");
		LocString variable;
		if (UnlocksAfterRunAs == null)
		{
			variable = locString2;
		}
		else
		{
			UnlockState unlockState = SaveManager.Instance.GenerateUnlockStateFromProgress();
			variable = ((!unlockState.Characters.Contains(UnlocksAfterRunAs)) ? locString2 : UnlocksAfterRunAs.Title);
		}
		locString.Add("Prerequisite", variable);
		return locString;
	}
}
