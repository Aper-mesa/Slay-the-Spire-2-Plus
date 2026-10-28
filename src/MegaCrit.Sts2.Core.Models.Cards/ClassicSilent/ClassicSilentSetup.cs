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

public sealed class ClassicSilentSetup : ClassicSilentCardModel
{
	public ClassicSilentSetup() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }
	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		CardModel selected = (await CardSelectCmd.FromHand(choiceContext, Owner, new CardSelectorPrefs(SelectionScreenPrompt, 1), null, this)).FirstOrDefault();
		if (selected != null)
		{
		    selected.EnergyCost.SetUntilPlayed(0);
		    await CardPileCmd.Add(selected, PileType.Draw, CardPilePosition.Top);
		}
	}
	protected override void OnUpgrade()
	{
		EnergyCost.UpgradeBy(-1);
	}
}
