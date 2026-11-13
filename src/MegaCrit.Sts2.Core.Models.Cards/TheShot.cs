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

public sealed class TheShot : CardModel
{
	private decimal _cardsExhaustedByFlorescence;

	protected override HashSet<CardTag> CanonicalTags => new HashSet<CardTag> { CardTag.Shoot };

	protected override IEnumerable<IHoverTip> ExtraHoverTips => new _003C_003Ez__ReadOnlyArray<IHoverTip>(new IHoverTip[3]
	{
		ArrowHoverTip,
		HoverTipFactory.FromKeyword(CardKeyword.Exhaust),
		HoverTipFactory.FromCard<Florescence>()
	});

	protected override bool ShouldGlowRedInternal => !ArrowCmd.PlayerHasAnyArrows(Owner);

	private decimal CardsExhaustedByFlorescence
	{
		get
		{
			return _cardsExhaustedByFlorescence;
		}
		set
		{
			AssertMutable();
			_cardsExhaustedByFlorescence = value;
		}
	}

	protected override IEnumerable<DynamicVar> CanonicalVars => new _003C_003Ez__ReadOnlyArray<DynamicVar>(new DynamicVar[3]
	{
		new CalculationBaseVar(0m),
		new ExtraDamageVar(10m),
		new CalculatedDamageVar(ValueProp.Move | ValueProp.FromArrow).WithMultiplier((CardModel card, Creature? _) => ((TheShot)card).CardsExhaustedByFlorescence)
	});

	public TheShot()
		: base(1, CardType.Attack, CardRarity.Token, TargetType.AnyEnemy)
	{
	}

	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
		AttackCommand attackCommand = DamageCmd.Attack(DynamicVars.CalculatedDamage).FromArrow().FromCard(this, cardPlay)
			.Targeting(cardPlay.Target)
			.WithAttackerAnim(Fool.GetShootBowAnimIfApplicable(Owner.Character), Fool.GetShootBowDelayIfApplicable(Owner.Character))
			.WithAttackerFx(null, "event:/sfx/characters/fool/fool_shot")
			.WithHitFx("vfx/vfx_attack_slash");
		await ArrowCmd.ShootArrowAtTarget(cardPlay.Target, Owner, attackCommand, choiceContext);
	}

	protected override void OnUpgrade()
	{
		DynamicVars.ExtraDamage.UpgradeValueBy(3m);
	}

	public void BuffFromFlorescence(decimal extraDamage)
	{
		CardsExhaustedByFlorescence += extraDamage;
	}
}
