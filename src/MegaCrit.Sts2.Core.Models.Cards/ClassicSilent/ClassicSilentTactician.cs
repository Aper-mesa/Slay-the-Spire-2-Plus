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

public sealed class ClassicSilentTactician : ClassicSilentCardModel
{
	protected override IEnumerable<DynamicVar> CanonicalVars => [new EnergyVar(1)];
	public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Unplayable];
	public ClassicSilentTactician() : base(-1, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }
	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		await Task.CompletedTask;
	}
	protected override void OnUpgrade()
	{
		DynamicVars.Energy.UpgradeValueBy(1m);
	}

    public override async Task AfterCardDiscarded(PlayerChoiceContext choiceContext, CardModel card)
    {
        if (card == this)
        {
            await PlayerCmd.GainEnergy(DynamicVars.Energy.IntValue, Owner);
        }
    }
}
