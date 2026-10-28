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

public sealed class ClassicIroncladRecklessCharge : ClassicIroncladCardModel
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromCard<Dazed>()];

	protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(7m, ValueProp.Move)];
	public ClassicIroncladRecklessCharge() : base(0, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy) { }
	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this, cardPlay).WithHitCount(1).Targeting(cardPlay.Target).WithHitFx("vfx/vfx_attack_slash").Execute(choiceContext);
		await CardPileCmd.AddGeneratedCardToCombat(CombatState.CreateCard<Dazed>(Owner), PileType.Draw, Owner, CardPilePosition.Random);
	}
	protected override void OnUpgrade()
	{
		DynamicVars.Damage.UpgradeValueBy(3m);
	}
}
