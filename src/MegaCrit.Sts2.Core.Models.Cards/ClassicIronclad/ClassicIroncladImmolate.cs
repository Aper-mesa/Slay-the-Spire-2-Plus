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

public sealed class ClassicIroncladImmolate : ClassicIroncladCardModel
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromCard<Burn>()];

	protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(21m, ValueProp.Move)];
	public ClassicIroncladImmolate() : base(2, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies) { }
	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this, cardPlay).WithHitCount(1).TargetingAllOpponents(CombatState).WithHitFx("vfx/vfx_attack_slash").Execute(choiceContext);
		await CardPileCmd.AddGeneratedCardToCombat(CombatState.CreateCard<Burn>(Owner), PileType.Discard, Owner);
	}
	protected override void OnUpgrade()
	{
		DynamicVars.Damage.UpgradeValueBy(7m);
	}
}
