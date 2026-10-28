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

public sealed class ClassicSilentDoppelganger : ClassicSilentCardModel
{
	public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
	public ClassicSilentDoppelganger() : base(0, CardType.Skill, CardRarity.Rare, TargetType.Self) { }
	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		int amount = ResolveEnergyXValue() + (IsUpgraded ? 1 : 0);
		if (amount > 0)
		{
		await PowerCmd.Apply<DrawCardsNextTurnPower>(choiceContext, Owner.Creature, amount, Owner.Creature, this);
		await PowerCmd.Apply<EnergyNextTurnPower>(choiceContext, Owner.Creature, amount, Owner.Creature, this);
		}
	}
	protected override bool HasEnergyCostX => true;
}
