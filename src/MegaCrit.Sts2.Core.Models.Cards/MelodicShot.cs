using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Characters;
using MegaCrit.Sts2.Core.ValueProps;

namespace MegaCrit.Sts2.Core.Models.Cards;

public sealed class MelodicShot : CardModel
{
	private const string _calculatedHitsKey = "CalculatedHits";

	protected override HashSet<CardTag> CanonicalTags => new HashSet<CardTag> { CardTag.Shoot };

	protected override bool ShouldGlowRedInternal => !ArrowCmd.PlayerHasAnyArrows(Owner);

	protected override IEnumerable<IHoverTip> ExtraHoverTips => new _003C_003Ez__ReadOnlyArray<IHoverTip>(new IHoverTip[4]
	{
		ArrowHoverTip,
		HoverTipFactory.Static(StaticHoverTip.WahMelody),
		HoverTipFactory.Static(StaticHoverTip.ChoMelody),
		HoverTipFactory.Static(StaticHoverTip.KiMelody)
	});

	protected override IEnumerable<DynamicVar> CanonicalVars => new _003C_003Ez__ReadOnlyArray<DynamicVar>(new DynamicVar[4]
	{
		new DamageVar(12m, ValueProp.Move | ValueProp.FromArrow),
		new CalculationBaseVar(1m),
		new CalculationExtraVar(1m),
		new CalculatedHitCountVar("CalculatedHits").WithMultiplier((CardModel card, Creature? _) => MelodyCmd.MelodiesThisTurn(card.Owner))
	});

	public MelodicShot()
		: base(1, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
	{
	}

	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
		AttackCommand attackCommand = DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromArrow().FromCard(this, cardPlay)
			.WithHitCount((int)((CalculatedVar)DynamicVars["CalculatedHits"]).Calculate(cardPlay.Target))
			.OnlyPlayAnimOnce()
			.Targeting(cardPlay.Target)
			.WithAttackerAnim(Fool.GetShootBowAnimIfApplicable(Owner.Character), Fool.GetShootBowDelayIfApplicable(Owner.Character))
			.WithAttackerFx(null, "event:/sfx/characters/fool/fool_shot")
			.WithHitFx("vfx/vfx_attack_slash");
		await ArrowCmd.ShootArrowAtTarget(cardPlay.Target, Owner, attackCommand, choiceContext);
	}

	protected override void OnUpgrade()
	{
		EnergyCost.UpgradeBy(-1);
		DynamicVars.Damage.UpgradeValueBy(1m);
	}
}
