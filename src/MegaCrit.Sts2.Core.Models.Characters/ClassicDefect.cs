using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Godot;
using MegaCrit.Sts2.Core.Achievements;
using MegaCrit.Sts2.Core.Animation;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.PotionPools;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Nodes.Vfx;

namespace MegaCrit.Sts2.Core.Models.Characters;

/// <summary>
/// [apermesa] The Slay the Spire 1 version of the Defect, kept separate from the STS2 redesign.
/// Visual assets currently reuse the STS2 Defect Spine/UI; the card/relic pools are the STS1 sets.
/// </summary>
public sealed class ClassicDefect : CharacterModel
{
	public const string energyColorName = "defect";

	public override Color NameColor => StsColors.blue;

	public override CharacterGender Gender => CharacterGender.Neutral;

	protected override CharacterModel? UnlocksAfterRunAs => null;

	public override int StartingHp => 75;

	public override int StartingGold => 99;

	public override CardPoolModel CardPool => ModelDb.CardPool<ClassicDefectCardPool>();

	public override RelicPoolModel RelicPool => ModelDb.RelicPool<ClassicDefectRelicPool>();

	public override PotionPoolModel PotionPool => ModelDb.PotionPool<DefectPotionPool>();

	public static Vector2 EyelineOffset => new Vector2(34f, -30f);

	public override IEnumerable<CardModel> StartingDeck => new _003C_003Ez__ReadOnlyArray<CardModel>(new CardModel[10]
	{
		ModelDb.Card<ClassicStrike>(),
		ModelDb.Card<ClassicStrike>(),
		ModelDb.Card<ClassicStrike>(),
		ModelDb.Card<ClassicStrike>(),
		ModelDb.Card<ClassicDefend>(),
		ModelDb.Card<ClassicDefend>(),
		ModelDb.Card<ClassicDefend>(),
		ModelDb.Card<ClassicDefend>(),
		ModelDb.Card<ClassicZap>(),
		ModelDb.Card<ClassicDualcast>()
	});

	public override IReadOnlyList<RelicModel> StartingRelics => new _003C_003Ez__ReadOnlySingleElementList<RelicModel>(ModelDb.Relic<ClassicCrackedCore>());

	public override float AttackAnimDelay => 0.15f;

	public override float CastAnimDelay => 0.25f;

	public override float PowerUpAnimDelay => 0.5f;

	public override Color EnergyLabelOutlineColor => new Color("163E64FF");

	public override int BaseOrbSlotCount => 3;

	public override Color DialogueColor => new Color("13446B");

	public override VfxColor SpeechBubbleColor => VfxColor.Blue;

	public override Color MapDrawingColor => new Color("0D638C");

	public override Color RemoteTargetingLineColor => new Color("70B6EDFF");

	public override Color RemoteTargetingLineOutline => new Color("163E64FF");

	public override string CharacterTransitionSfx => "event:/sfx/ui/wipe_ironclad";

	public override string CharacterSelectSfx => "event:/sfx/characters/defect/defect_select";

	public override string AttackSfx => "event:/sfx/characters/defect/defect_attack";

	public override string CastSfx => "event:/sfx/characters/defect/defect_cast";

	public override string DeathSfx => "event:/sfx/characters/defect/defect_die";

	public override Achievement RunWonAchievement => Achievement.DefectWin;

	protected override string VisualsPath => SceneHelper.GetScenePath("creature_visuals/defect");

	protected override string IconTexturePath => ImageHelper.GetImagePath("ui/top_panel/character_icon_defect.png");

	protected override string IconOutlineTexturePath => ImageHelper.GetImagePath("ui/top_panel/character_icon_defect_outline.png");

	protected override string IconPath => SceneHelper.GetScenePath("ui/character_icons/defect_icon");

	public override string EnergyCounterPath => SceneHelper.GetScenePath("combat/energy_counters/defect_energy_counter");

	public override string MerchantAnimPath => SceneHelper.GetScenePath("merchant/characters/defect_merchant");

	public override string RestSiteAnimPath => SceneHelper.GetScenePath("rest_site/characters/defect_rest_site");

	protected override string ArmPointingTexturePath => ImageHelper.GetImagePath("ui/hands/multiplayer_hand_defect_point.png");

	protected override string ArmRockTexturePath => ImageHelper.GetImagePath("ui/hands/multiplayer_hand_defect_rock.png");

	protected override string ArmPaperTexturePath => ImageHelper.GetImagePath("ui/hands/multiplayer_hand_defect_paper.png");

	protected override string ArmScissorsTexturePath => ImageHelper.GetImagePath("ui/hands/multiplayer_hand_defect_scissors.png");

	public override string CharacterSelectBg => SceneHelper.GetScenePath("screens/char_select/char_select_bg_defect");

	protected override string CharacterSelectIconPath => ImageHelper.GetImagePath("packed/character_select/char_select_defect.png");

	protected override string CharacterSelectLockedIconPath => ImageHelper.GetImagePath("packed/character_select/char_select_defect_locked.png");

	protected override string MapMarkerPath => ImageHelper.GetImagePath("packed/map/icons/map_marker_defect.png");

	public override string CharacterSelectTransitionPath => "res://materials/transitions/defect_transition_mat.tres";

	public override string TrailPath => SceneHelper.GetScenePath("vfx/card_trail_defect");

	protected override List<(AnimState, string)> AnimationStates => new List<(AnimState, string)>
	{
		(new AnimState("attack"), "Attack"),
		(new AnimState("hurt"), "Hit"),
		(new AnimState("cast"), "Cast"),
		(new AnimState("process"), "PowerUp")
	};

	public override List<string> GetArchitectAttackVfx()
	{
		int num = 5;
		List<string> list = new List<string>(num);
		CollectionsMarshal.SetCount(list, num);
		Span<string> span = CollectionsMarshal.AsSpan(list);
		int num2 = 0;
		span[num2] = "vfx/vfx_attack_lightning";
		num2++;
		span[num2] = "vfx/vfx_attack_blunt";
		num2++;
		span[num2] = "vfx/vfx_scratch";
		num2++;
		span[num2] = "vfx/vfx_attack_slash";
		num2++;
		span[num2] = "vfx/vfx_heavy_blunt";
		return list;
	}
}
