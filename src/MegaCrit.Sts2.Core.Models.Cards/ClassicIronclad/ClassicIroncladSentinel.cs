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

public sealed class ClassicIroncladSentinel : ClassicIroncladCardModel
{
	protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(5m, ValueProp.Move), new EnergyVar(2)];
	public ClassicIroncladSentinel() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }
	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
	}
	protected override void OnUpgrade()
	{
		DynamicVars.Block.UpgradeValueBy(3m);
		DynamicVars.Energy.UpgradeValueBy(1m);
	}

    public override bool GainsBlock => true;
    public override async Task AfterCardExhausted(PlayerChoiceContext choiceContext, CardModel card, bool causedByEthereal)
    {
        if (card == this) await PlayerCmd.GainEnergy(DynamicVars.Energy.IntValue, Owner);
    }
}
