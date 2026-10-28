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

public sealed class ClassicSilentConcentrate : ClassicSilentCardModel
{
	protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("MagicNumber", 3m)];
	public ClassicSilentConcentrate() : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }
	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		await CardCmd.Discard(choiceContext, await CardSelectCmd.FromHandForDiscard(choiceContext, Owner, new CardSelectorPrefs(CardSelectorPrefs.DiscardSelectionPrompt, DynamicVars["MagicNumber"].IntValue), null, this));
		await PlayerCmd.GainEnergy(2, Owner);
	}
	protected override void OnUpgrade()
	{
		DynamicVars["MagicNumber"].UpgradeValueBy(-1m);
	}
}
