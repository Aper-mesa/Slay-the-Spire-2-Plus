using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Characters;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace MegaCrit.Sts2.Core.Models.Cards;

public sealed class HobblingShot : CardModel
{
	private const string _powerKey = "Power";

	protected override HashSet<CardTag> CanonicalTags => new HashSet<CardTag> { CardTag.Shoot };

	protected override bool ShouldGlowRedInternal => !ArrowCmd.PlayerHasAnyArrows(Owner);

	protected override IEnumerable<IHoverTip> ExtraHoverTips => new _003C_003Ez__ReadOnlyArray<IHoverTip>(new IHoverTip[3]
	{
		ArrowHoverTip,
		HoverTipFactory.FromPower<WeakPower>(),
		HoverTipFactory.FromPower<VulnerablePower>()
	});

	protected override IEnumerable<DynamicVar> CanonicalVars => new _003C_003Ez__ReadOnlyArray<DynamicVar>(new DynamicVar[2]
	{
		new DamageVar(13m, ValueProp.Move | ValueProp.FromArrow),
		new DynamicVar("Power", 1m)
	});

	public HobblingShot()
		: base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
	{
	}

	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
		AttackCommand attackCommand = DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromArrow().FromCard(this, cardPlay)
			.WithAttackerAnim(Fool.GetShootBowAnimIfApplicable(Owner.Character), Fool.GetShootBowDelayIfApplicable(Owner.Character))
			.WithAttackerFx(null, "event:/sfx/characters/fool/fool_shot")
			.Targeting(cardPlay.Target)
			.WithHitFx("vfx/vfx_attack_slash");
		if (await ArrowCmd.ShootArrowAtTarget(cardPlay.Target, Owner, attackCommand, choiceContext))
		{
			await PowerCmd.Apply<WeakPower>(choiceContext, cardPlay.Target, DynamicVars["Power"].BaseValue, Owner.Creature, this);
			await PowerCmd.Apply<VulnerablePower>(choiceContext, cardPlay.Target, DynamicVars["Power"].BaseValue, Owner.Creature, this);
		}
	}

	protected override void OnUpgrade()
	{
		DynamicVars.Damage.UpgradeValueBy(1m);
		DynamicVars["Power"].UpgradeValueBy(1m);
	}
}
