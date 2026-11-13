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
using MegaCrit.Sts2.Core.ValueProps;

namespace MegaCrit.Sts2.Core.Models.Cards;

public sealed class SkyShot : CardModel
{
	protected override IEnumerable<IHoverTip> ExtraHoverTips => new _003C_003Ez__ReadOnlySingleElementList<IHoverTip>(ArrowHoverTip);

	protected override HashSet<CardTag> CanonicalTags => new HashSet<CardTag> { CardTag.Shoot };

	public override IEnumerable<CardKeyword> CanonicalKeywords => new _003C_003Ez__ReadOnlyArray<CardKeyword>(new CardKeyword[2]
	{
		CardKeyword.Exhaust,
		CardKeyword.Innate
	});

	protected override bool ShouldGlowRedInternal => !ArrowCmd.PlayerHasAnyArrows(Owner);

	protected override IEnumerable<DynamicVar> CanonicalVars => new _003C_003Ez__ReadOnlySingleElementList<DynamicVar>(new DamageVar(30m, ValueProp.Move | ValueProp.FromArrow));

	public SkyShot()
		: base(1, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
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
		await ArrowCmd.ShootArrowAtTarget(cardPlay.Target, Owner, attackCommand, choiceContext);
	}

	protected override void OnUpgrade()
	{
		DynamicVars.Damage.UpgradeValueBy(10m);
	}
}
