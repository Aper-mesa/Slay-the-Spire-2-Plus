using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace MegaCrit.Sts2.Core.Models.Cards;

public sealed class RepeatShot : CardModel
{
	protected override IEnumerable<IHoverTip> ExtraHoverTips => new _003C_003Ez__ReadOnlySingleElementList<IHoverTip>(ArrowHoverTip);

	protected override HashSet<CardTag> CanonicalTags => new HashSet<CardTag> { CardTag.Shoot };

	protected override bool HasEnergyCostX => true;

	protected override bool ShouldGlowRedInternal => !ArrowCmd.PlayerHasAnyArrows(Owner);

	protected override IEnumerable<DynamicVar> CanonicalVars => new _003C_003Ez__ReadOnlySingleElementList<DynamicVar>(new DamageVar(15m, ValueProp.Move | ValueProp.FromArrow));

	public RepeatShot()
		: base(0, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
	{
	}

	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
		int num = ResolveEnergyXValue();
		if (num > 0)
		{
			AttackCommand attackCommand = DamageCmd.Attack(DynamicVars.Damage.BaseValue).WithHitCount(num).FromCard(this, cardPlay)
				.FromArrow()
				.WithAttackerFx(null, "event:/sfx/characters/fool/fool_shot")
				.Targeting(cardPlay.Target)
				.OnlyPlayAnimOnce()
				.WithHitFx("vfx/vfx_giant_horizontal_slash");
			await ArrowCmd.ShootArrowAtTarget(cardPlay.Target, Owner, attackCommand, choiceContext);
		}
	}

	protected override void OnUpgrade()
	{
		DynamicVars.Damage.UpgradeValueBy(5m);
	}
}
