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

public sealed class ClassicIroncladLimitBreak : ClassicIroncladCardModel
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<StrengthPower>()];

	public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
	public ClassicIroncladLimitBreak() : base(1, CardType.Skill, CardRarity.Rare, TargetType.Self) { }
	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		await PowerCmd.Apply<StrengthPower>(choiceContext, Owner.Creature, Owner.Creature.GetPowerAmount<StrengthPower>(), Owner.Creature, this);
	}
	protected override void OnUpgrade()
	{
		RemoveKeyword(CardKeyword.Exhaust);
	}
}
