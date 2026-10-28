using System;
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

public sealed class ClassicSilentQuickSlash : ClassicSilentCardModel
{
	protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(8m, ValueProp.Move)];
	public ClassicSilentQuickSlash() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy) { }
	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this, cardPlay).WithHitCount(1).Targeting(cardPlay.Target).WithHitFx("vfx/vfx_attack_slash").Execute(choiceContext);
		await CardPileCmd.Draw(choiceContext, 1, Owner);
	}
	protected override void OnUpgrade()
	{
		DynamicVars.Damage.UpgradeValueBy(4m);
	}
}
