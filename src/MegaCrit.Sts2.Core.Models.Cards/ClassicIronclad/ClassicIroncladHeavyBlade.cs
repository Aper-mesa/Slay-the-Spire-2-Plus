using System;
using MegaCrit.Sts2.Core.HoverTips;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace MegaCrit.Sts2.Core.Models.Cards;

public sealed class ClassicIroncladHeavyBlade : ClassicIroncladCardModel
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<StrengthPower>()];

	protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(14m, ValueProp.Move), new DynamicVar("MagicNumber", 3m)];
	public ClassicIroncladHeavyBlade() : base(2, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy) { }
	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this, cardPlay).WithHitCount(1).Targeting(cardPlay.Target).WithHitFx("vfx/vfx_attack_slash").Execute(choiceContext);
	}
	protected override void OnUpgrade()
	{
		DynamicVars["MagicNumber"].UpgradeValueBy(2m);
	}

    public override decimal ModifyDamageAdditive(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource, CardPlay? cardPlay)
    {
        if (cardSource == this && dealer == Owner.Creature && props.IsPoweredAttack())
            return Owner.Creature.GetPowerAmount<StrengthPower>() * (DynamicVars["MagicNumber"].BaseValue - 1);
        return 0;
    }
}
