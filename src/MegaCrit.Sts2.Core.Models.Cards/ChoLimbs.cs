using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;

namespace MegaCrit.Sts2.Core.Models.Cards;

public sealed class ChoLimbs : CardModel
{
	private const int _melodyCostReduction = 2;

	public override bool GainsBlock => true;

	protected override bool ShouldGlowGoldInternal => HasWahOrKiMelody;

	protected override HashSet<CardTag> CanonicalTags => new HashSet<CardTag> { CardTag.Cho };

	protected override IEnumerable<IHoverTip> ExtraHoverTips => new _003C_003Ez__ReadOnlyArray<IHoverTip>(new IHoverTip[2]
	{
		HoverTipFactory.Static(StaticHoverTip.WahMelody),
		HoverTipFactory.Static(StaticHoverTip.KiMelody)
	});

	protected override IEnumerable<DynamicVar> CanonicalVars => new _003C_003Ez__ReadOnlySingleElementList<DynamicVar>(new BlockVar(5m, ValueProp.Move));

	protected override IEnumerable<string> ExtraRunAssetPaths => NChoVfx.AssetPaths;

	private bool HasWahOrKiMelody
	{
		get
		{
			if (!MelodyCmd.PlayedWahThisTurn(Owner))
			{
				return MelodyCmd.PlayedKiThisTurn(Owner);
			}
			return true;
		}
	}

	public ChoLimbs()
		: base(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
	{
	}

	public override bool TryModifyEnergyCostInCombat(CardModel card, decimal originalCost, out decimal modifiedCost)
	{
		modifiedCost = originalCost;
		if (card != this)
		{
			return false;
		}
		if (originalCost <= 0m)
		{
			return false;
		}
		if (!HasWahOrKiMelody)
		{
			return false;
		}
		modifiedCost = Math.Max(0m, originalCost - 2m);
		return true;
	}

	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
		await MelodyCmd.PlayMelodyVfx(Owner.Creature, CardTag.Cho);
		for (int i = 0; i < 2; i++)
		{
			await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
		}
	}

	protected override void OnUpgrade()
	{
		DynamicVars.Block.UpgradeValueBy(2m);
	}
}
