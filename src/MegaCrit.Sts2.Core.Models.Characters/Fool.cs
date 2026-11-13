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

public sealed class Fool : CharacterModel
{
	private const string _shootBowTrigger = "shootBow";

	public const string energyColorName = "fool";

	public const string joySfx = "event:/sfx/characters/fugitive/fugitive_joy";

	public const string arrowShootSfx = "event:/sfx/characters/fool/fool_shot";

	public const string arrowDislodgeSfx = "event:/sfx/characters/fool/fool_dislodge";

	public const string humSfx = "event:/sfx/characters/fugitive/fugitive_hum";

	public const string shockSfx = "event:/sfx/characters/fugitive/fugitive_shock";

	/// <summary>
	/// Special gender. Events/Ancients will guess this character's gender because of this flag.
	/// Do NOT use for other characters. This is MY bit &gt;:(
	/// </summary>
	public override CharacterGender Gender => CharacterGender.Tree;

	protected override CharacterModel UnlocksAfterRunAs => ModelDb.Character<Defect>();

	public override Color NameColor => StsColors.yellow;

	public override int StartingHp => 77;

	public override int BaseArrowSlotCount => 1;

	public override int StartingGold => 99;

	public override CardPoolModel CardPool => ModelDb.CardPool<FoolCardPool>();

	public override PotionPoolModel PotionPool => ModelDb.PotionPool<FoolPotionPool>();

	public override RelicPoolModel RelicPool => ModelDb.RelicPool<FoolRelicPool>();

	public override IEnumerable<CardModel> StartingDeck => new _003C_003Ez__ReadOnlyArray<CardModel>(new CardModel[10]
	{
		ModelDb.Card<StrikeFool>(),
		ModelDb.Card<StrikeFool>(),
		ModelDb.Card<StrikeFool>(),
		ModelDb.Card<StrikeFool>(),
		ModelDb.Card<StrikeFool>(),
		ModelDb.Card<DefendFool>(),
		ModelDb.Card<DefendFool>(),
		ModelDb.Card<DefendFool>(),
		ModelDb.Card<DefendFool>(),
		ModelDb.Card<FoolishShot>()
	});

	public override IReadOnlyList<RelicModel> StartingRelics => new _003C_003Ez__ReadOnlySingleElementList<RelicModel>(ModelDb.Relic<SongBow>());

	public override float AttackAnimDelay => 0.15f;

	public override float CastAnimDelay => 0.25f;

	public override Color EnergyLabelOutlineColor => new Color("5a4411");

	public override Color DialogueColor => new Color("590700");

	public override VfxColor SpeechBubbleColor => VfxColor.Fool;

	public override Color MapDrawingColor => new Color("79570F");

	public override Color RemoteTargetingLineColor => new Color("E15847FF");

	public override Color RemoteTargetingLineOutline => new Color("801212FF");

	public override string CharacterSelectSfx => "event:/sfx/characters/fool/fool_select_B";

	public override string DeathSfx => "event:/sfx/characters/fugitive/fugitive_die";

	protected override List<(AnimState, string)> AnimationStates => base.AnimationStates.Concat(new _003C_003Ez__ReadOnlySingleElementList<(AnimState, string)>((new AnimState("shoot"), "shootBow"))).ToList();

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

	public static string GetShootBowAnimIfApplicable(CharacterModel character)
	{
		if (!(character is Fool))
		{
			return "Attack";
		}
		return "shootBow";
	}

	public static float GetShootBowDelayIfApplicable(CharacterModel character)
	{
		if (!(character is Fool))
		{
			return character.AttackAnimDelay;
		}
		return 0.45f;
	}
}
