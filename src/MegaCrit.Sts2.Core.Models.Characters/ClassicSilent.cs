using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Godot;
using MegaCrit.Sts2.Core.Animation;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.PotionPools;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Nodes.Vfx;

namespace MegaCrit.Sts2.Core.Models.Characters;

public sealed class ClassicSilent : CharacterModel
{
	public const string shivTrigger = "Shiv";

	public const string energyColorName = "silent";

	public override Color NameColor => StsColors.green;

	public override CharacterGender Gender => CharacterGender.Feminine;

	/// <remarks>
	/// You'll always unlock Silent by doing a run with Ironclad since he's the only character you start out with, but
	/// technically you unlock her by doing a run with any character.
	/// </remarks>
	protected override CharacterModel? UnlocksAfterRunAs => null;

	public override int StartingHp => 70;

	public override int StartingGold => 99;

	public override CardPoolModel CardPool => ModelDb.CardPool<ClassicSilentCardPool>();

	public override RelicPoolModel RelicPool => ModelDb.RelicPool<ClassicSilentRelicPool>();

	public override PotionPoolModel PotionPool => ModelDb.PotionPool<SilentPotionPool>();

	public override IEnumerable<CardModel> StartingDeck => new _003C_003Ez__ReadOnlyArray<CardModel>(new CardModel[12]
	{
		ModelDb.Card<ClassicSilentStrikeSilent>(),
		ModelDb.Card<ClassicSilentStrikeSilent>(),
		ModelDb.Card<ClassicSilentStrikeSilent>(),
		ModelDb.Card<ClassicSilentStrikeSilent>(),
		ModelDb.Card<ClassicSilentStrikeSilent>(),
		ModelDb.Card<ClassicSilentDefendSilent>(),
		ModelDb.Card<ClassicSilentDefendSilent>(),
		ModelDb.Card<ClassicSilentDefendSilent>(),
		ModelDb.Card<ClassicSilentDefendSilent>(),
		ModelDb.Card<ClassicSilentDefendSilent>(),
		ModelDb.Card<ClassicSilentNeutralize>(),
		ModelDb.Card<ClassicSilentSurvivor>()
	});

	public override IReadOnlyList<RelicModel> StartingRelics => new _003C_003Ez__ReadOnlySingleElementList<RelicModel>(ModelDb.Relic<ClassicSilentRingOfTheSnake>());

	public override float AttackAnimDelay => 0.15f;

	public override float CastAnimDelay => 0.25f;

	public override Color EnergyLabelOutlineColor => new Color("004f04FF");

	public override Color DialogueColor => new Color("284719");

	public override VfxColor SpeechBubbleColor => VfxColor.Swamp;

	public override Color MapDrawingColor => new Color("2F6729");

	public override Color RemoteTargetingLineColor => new Color("2EBD5EFF");

	public override Color RemoteTargetingLineOutline => new Color("004f04FF");

	protected override List<(AnimState, string)> AnimationStates => base.AnimationStates.Concat(new _003C_003Ez__ReadOnlySingleElementList<(AnimState, string)>((new AnimState("shiv"), "Shiv"))).ToList();

	public override string CharacterTransitionSfx => "event:/sfx/ui/wipe_silent";

	public override string CharacterSelectSfx => "event:/sfx/characters/silent/silent_select";

	public override string AttackSfx => "event:/sfx/characters/silent/silent_attack";

	public override string CastSfx => "event:/sfx/characters/silent/silent_cast";

	public override string DeathSfx => "event:/sfx/characters/silent/silent_die";

	public override MegaCrit.Sts2.Core.Achievements.Achievement RunWonAchievement => MegaCrit.Sts2.Core.Achievements.Achievement.SilentWin;

	protected override string VisualsPath => SceneHelper.GetScenePath("creature_visuals/silent");

	protected override string IconTexturePath => ImageHelper.GetImagePath("ui/top_panel/character_icon_silent.png");

	protected override string IconOutlineTexturePath => ImageHelper.GetImagePath("ui/top_panel/character_icon_silent_outline.png");

	protected override string IconPath => SceneHelper.GetScenePath("ui/character_icons/silent_icon");

	public override string EnergyCounterPath => SceneHelper.GetScenePath("combat/energy_counters/silent_energy_counter");

	public override string MerchantAnimPath => SceneHelper.GetScenePath("merchant/characters/silent_merchant");

	public override string RestSiteAnimPath => SceneHelper.GetScenePath("rest_site/characters/silent_rest_site");

	protected override string ArmPointingTexturePath => ImageHelper.GetImagePath("ui/hands/multiplayer_hand_silent_point.png");

	protected override string ArmRockTexturePath => ImageHelper.GetImagePath("ui/hands/multiplayer_hand_silent_rock.png");

	protected override string ArmPaperTexturePath => ImageHelper.GetImagePath("ui/hands/multiplayer_hand_silent_paper.png");

	protected override string ArmScissorsTexturePath => ImageHelper.GetImagePath("ui/hands/multiplayer_hand_silent_scissors.png");

	public override string CharacterSelectBg => SceneHelper.GetScenePath("screens/char_select/char_select_bg_silent");

	protected override string CharacterSelectIconPath => ImageHelper.GetImagePath("packed/character_select/char_select_silent.png");

	protected override string CharacterSelectLockedIconPath => ImageHelper.GetImagePath("packed/character_select/char_select_silent_locked.png");

	protected override string MapMarkerPath => ImageHelper.GetImagePath("packed/map/icons/map_marker_silent.png");

	public override string CharacterSelectTransitionPath => "res://materials/transitions/silent_transition_mat.tres";

	public override string TrailPath => SceneHelper.GetScenePath("vfx/card_trail_silent");

	public override List<string> GetArchitectAttackVfx()
	{
		int num = 4;
		List<string> list = new List<string>(num);
		CollectionsMarshal.SetCount(list, num);
		Span<string> span = CollectionsMarshal.AsSpan(list);
		int num2 = 0;
		span[num2] = "vfx/vfx_dagger_spray";
		num2++;
		span[num2] = "vfx/vfx_flying_slash";
		num2++;
		span[num2] = "vfx/vfx_dramatic_stab";
		num2++;
		span[num2] = "vfx/vfx_dagger_throw";
		return list;
	}
}
