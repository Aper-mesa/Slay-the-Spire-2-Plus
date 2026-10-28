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

public sealed class ClassicSilentExpertise : ClassicSilentCardModel
{
	protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(6)];
	public ClassicSilentExpertise() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }
	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		int count = DynamicVars.Cards.IntValue - PileType.Hand.GetPile(Owner).Cards.Count;
		if (count > 0) await CardPileCmd.Draw(choiceContext, count, Owner);
	}
	protected override void OnUpgrade()
	{
		DynamicVars.Cards.UpgradeValueBy(1m);
	}
}
