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

public sealed class ClassicIroncladClothesline : ClassicIroncladCardModel
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<WeakPower>()];

	protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(12m, ValueProp.Move), new PowerVar<WeakPower>("MagicNumber", 2m)];
	public ClassicIroncladClothesline() : base(2, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy) { }
	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this, cardPlay).WithHitCount(1).Targeting(cardPlay.Target).WithHitFx("vfx/vfx_attack_slash").Execute(choiceContext);
		await PowerCmd.Apply<WeakPower>(choiceContext, cardPlay.Target, DynamicVars["MagicNumber"].BaseValue, Owner.Creature, this);
	}
	protected override void OnUpgrade()
	{
		DynamicVars.Damage.UpgradeValueBy(2m);
		DynamicVars["MagicNumber"].UpgradeValueBy(1m);
	}
}
