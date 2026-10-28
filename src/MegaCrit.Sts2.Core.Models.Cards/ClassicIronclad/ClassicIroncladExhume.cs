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

public sealed class ClassicIroncladExhume : ClassicIroncladCardModel
{
	public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
	public ClassicIroncladExhume() : base(1, CardType.Skill, CardRarity.Rare, TargetType.Self) { }
	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		CardModel selected = (await CardSelectCmd.FromCombatPile(choiceContext, PileType.Exhaust.GetPile(Owner), Owner,
		    new CardSelectorPrefs(SelectionScreenPrompt, 1), c => c is not ClassicIroncladExhume)).FirstOrDefault();
		if (selected != null) await CardPileCmd.Add(selected, PileType.Hand);
	}
	protected override void OnUpgrade()
	{
		EnergyCost.UpgradeBy(-1);
	}
}
