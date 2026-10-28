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

public sealed class ClassicIroncladFlex : ClassicIroncladCardModel
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<StrengthPower>()];

	protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<StrengthPower>("MagicNumber", 2m)];
	public ClassicIroncladFlex() : base(0, CardType.Skill, CardRarity.Common, TargetType.Self) { }
	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		await PowerCmd.Apply<StrengthPower>(choiceContext, Owner.Creature, DynamicVars["MagicNumber"].BaseValue, Owner.Creature, this);
		await PowerCmd.Apply<ClassicIroncladLoseStrengthPower>(choiceContext, Owner.Creature, DynamicVars["MagicNumber"].BaseValue, Owner.Creature, this);
	}
	protected override void OnUpgrade()
	{
		DynamicVars["MagicNumber"].UpgradeValueBy(2m);
	}
}
