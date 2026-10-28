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

public sealed class ClassicIronclad : CharacterModel
{
	public const string heavyAttackTrigger = "heavyAttack";

	public const string energyColorName = "ironclad";

	public override CharacterGender Gender => CharacterGender.Masculine;

	/// <remarks>
	/// Ironclad starts out unlocked.
	/// </remarks>
	protected override CharacterModel? UnlocksAfterRunAs => null;

	public override Color NameColor => StsColors.red;

	public override int StartingHp => 80;

	public override int StartingGold => 99;

	public override CardPoolModel CardPool => ModelDb.CardPool<ClassicIroncladCardPool>();

	public override PotionPoolModel PotionPool => ModelDb.PotionPool<IroncladPotionPool>();

	public override RelicPoolModel RelicPool => ModelDb.RelicPool<ClassicIroncladRelicPool>();

	public override IEnumerable<CardModel> StartingDeck => new _003C_003Ez__ReadOnlyArray<CardModel>(new CardModel[10]
	{
		ModelDb.Card<ClassicIroncladStrikeIronclad>(),
		ModelDb.Card<ClassicIroncladStrikeIronclad>(),
		ModelDb.Card<ClassicIroncladStrikeIronclad>(),
		ModelDb.Card<ClassicIroncladStrikeIronclad>(),
		ModelDb.Card<ClassicIroncladStrikeIronclad>(),
		ModelDb.Card<ClassicIroncladDefendIronclad>(),
		ModelDb.Card<ClassicIroncladDefendIronclad>(),
		ModelDb.Card<ClassicIroncladDefendIronclad>(),
		ModelDb.Card<ClassicIroncladDefendIronclad>(),
		ModelDb.Card<ClassicIroncladBash>()
	});

	public override IReadOnlyList<RelicModel> StartingRelics => new _003C_003Ez__ReadOnlySingleElementList<RelicModel>(ModelDb.Relic<ClassicIroncladBurningBlood>());

	public override float AttackAnimDelay => 0.15f;

	public override float CastAnimDelay => 0.25f;

	public override Color EnergyLabelOutlineColor => new Color("801212FF");

	public override Color DialogueColor => new Color("590700");

	public override VfxColor SpeechBubbleColor => VfxColor.Red;

	public override Color MapDrawingColor => new Color("CB282B");

	public override Color RemoteTargetingLineColor => new Color("E15847FF");

	public override Color RemoteTargetingLineOutline => new Color("801212FF");

	protected override List<(AnimState, string)> AnimationStates => base.AnimationStates.Concat(new _003C_003Ez__ReadOnlySingleElementList<(AnimState, string)>((new AnimState("attack_heavy"), "heavyAttack"))).ToList();

	public override string CharacterTransitionSfx => "event:/sfx/ui/wipe_ironclad";

	public override string CharacterSelectSfx => "event:/sfx/characters/ironclad/ironclad_select";

	public override string AttackSfx => "event:/sfx/characters/ironclad/ironclad_attack";

	public override string CastSfx => "event:/sfx/characters/ironclad/ironclad_cast";

	public override string DeathSfx => "event:/sfx/characters/ironclad/ironclad_die";

	public override MegaCrit.Sts2.Core.Achievements.Achievement RunWonAchievement => MegaCrit.Sts2.Core.Achievements.Achievement.IroncladWin;

	protected override string VisualsPath => SceneHelper.GetScenePath("creature_visuals/ironclad");

	protected override string IconTexturePath => ImageHelper.GetImagePath("ui/top_panel/character_icon_ironclad.png");

	protected override string IconOutlineTexturePath => ImageHelper.GetImagePath("ui/top_panel/character_icon_ironclad_outline.png");

	protected override string IconPath => SceneHelper.GetScenePath("ui/character_icons/ironclad_icon");

	public override string EnergyCounterPath => SceneHelper.GetScenePath("combat/energy_counters/ironclad_energy_counter");

	public override string MerchantAnimPath => SceneHelper.GetScenePath("merchant/characters/ironclad_merchant");

	public override string RestSiteAnimPath => SceneHelper.GetScenePath("rest_site/characters/ironclad_rest_site");

	protected override string ArmPointingTexturePath => ImageHelper.GetImagePath("ui/hands/multiplayer_hand_ironclad_point.png");

	protected override string ArmRockTexturePath => ImageHelper.GetImagePath("ui/hands/multiplayer_hand_ironclad_rock.png");

	protected override string ArmPaperTexturePath => ImageHelper.GetImagePath("ui/hands/multiplayer_hand_ironclad_paper.png");

	protected override string ArmScissorsTexturePath => ImageHelper.GetImagePath("ui/hands/multiplayer_hand_ironclad_scissors.png");

	public override string CharacterSelectBg => SceneHelper.GetScenePath("screens/char_select/char_select_bg_ironclad");

	protected override string CharacterSelectIconPath => ImageHelper.GetImagePath("packed/character_select/char_select_ironclad.png");

	protected override string CharacterSelectLockedIconPath => ImageHelper.GetImagePath("packed/character_select/char_select_ironclad_locked.png");

	protected override string MapMarkerPath => ImageHelper.GetImagePath("packed/map/icons/map_marker_ironclad.png");

	public override string CharacterSelectTransitionPath => "res://materials/transitions/ironclad_transition_mat.tres";

	public override string TrailPath => SceneHelper.GetScenePath("vfx/card_trail_ironclad");

	public override List<string> GetArchitectAttackVfx()
	{
		int num = 5;
		List<string> list = new List<string>(num);
		CollectionsMarshal.SetCount(list, num);
		Span<string> span = CollectionsMarshal.AsSpan(list);
		int num2 = 0;
		span[num2] = "vfx/vfx_attack_blunt";
		num2++;
		span[num2] = "vfx/vfx_heavy_blunt";
		num2++;
		span[num2] = "vfx/vfx_attack_slash";
		num2++;
		span[num2] = "vfx/vfx_bloody_impact";
		num2++;
		span[num2] = "vfx/vfx_rock_shatter";
		return list;
	}

}
