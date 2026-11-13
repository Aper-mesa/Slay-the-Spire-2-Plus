using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace MegaCrit.Sts2.Core.Models.Powers;

public sealed class JugglingPower : PowerModel
{
	private class Data
	{
		public int attacksPlayedThisTurn;

		public CardModel? cardToClone;
	}

	public override PowerType Type => PowerType.Buff;

	public override PowerStackType StackType => PowerStackType.Counter;

	protected override object InitInternalData()
	{
		return new Data();
	}

	public override Task AfterApplied(Creature? applier, CardModel? cardSource)
	{
		GetInternalData<Data>().attacksPlayedThisTurn = CombatManager.Instance.History.CardPlaysStarted.Count((CardPlayStartedEntry e) => e.CardPlay.Card.Type == CardType.Attack && e.CardPlay.Player == Owner.Player && e.HappenedThisTurn(CombatState));
		return Task.CompletedTask;
	}

	public override Task BeforeCardPlayed(CardPlay cardPlay)
	{
		if (cardPlay.Card.Owner != Owner.Player)
		{
			return Task.CompletedTask;
		}
		if (cardPlay.Card.Type != CardType.Attack)
		{
			return Task.CompletedTask;
		}
		GetInternalData<Data>().attacksPlayedThisTurn++;
		if (GetInternalData<Data>().attacksPlayedThisTurn == 3)
		{
			GetInternalData<Data>().cardToClone = cardPlay.Card;
		}
		return Task.CompletedTask;
	}

	public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		CardModel cardToClone = GetInternalData<Data>().cardToClone;
		if (cardToClone == cardPlay.Card)
		{
			GetInternalData<Data>().cardToClone = null;
			Flash();
			for (int i = 0; i < Amount; i++)
			{
				CardModel card = cardPlay.Card.CreateClone();
				await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, Owner.Player);
			}
		}
	}

	public override Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
	{
		if (!participants.Contains(Owner))
		{
			return Task.CompletedTask;
		}
		GetInternalData<Data>().attacksPlayedThisTurn = 0;
		return Task.CompletedTask;
	}
}
