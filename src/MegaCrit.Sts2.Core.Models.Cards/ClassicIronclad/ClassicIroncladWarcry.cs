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

public sealed class ClassicIroncladWarcry : ClassicIroncladCardModel
{
	protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(1)];
	public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
	public ClassicIroncladWarcry() : base(0, CardType.Skill, CardRarity.Common, TargetType.Self) { }
	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.IntValue, Owner);
		CardModel selected = (await CardSelectCmd.FromHand(choiceContext, Owner, new CardSelectorPrefs(SelectionScreenPrompt, 1), null, this)).FirstOrDefault();
		if (selected != null) await CardPileCmd.Add(selected, PileType.Draw, CardPilePosition.Top);
	}
	protected override void OnUpgrade()
	{
		DynamicVars.Cards.UpgradeValueBy(1m);
	}
}
