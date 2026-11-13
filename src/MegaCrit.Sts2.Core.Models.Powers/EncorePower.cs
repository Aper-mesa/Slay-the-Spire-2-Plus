using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace MegaCrit.Sts2.Core.Models.Powers;

public sealed class EncorePower : PowerModel
{
	private class Data
	{
		/// <summary>
		/// Keep track of the cards we've seen played..
		/// This lets Ideas avoid triggering on cards that started play before it was applied.
		/// </summary>
		public readonly List<CardModel> cardsBeingPlayed = new List<CardModel>();

		public int numberOfCardsPlayedSinceApplied;
	}

	private const string _playsRemainingKey = "PlaysRemaining";

	public override PowerType Type => PowerType.Debuff;

	public override PowerStackType StackType => PowerStackType.Counter;

	public override int DisplayAmount => PlaysRemaining;

	private int PlaysRemaining => Mathf.Max(0, Amount - GetInternalData<Data>().numberOfCardsPlayedSinceApplied);

	protected override IEnumerable<DynamicVar> CanonicalVars => new _003C_003Ez__ReadOnlySingleElementList<DynamicVar>(new DynamicVar("PlaysRemaining", 0m));

	protected override object InitInternalData()
	{
		return new Data();
	}

	public override Task AfterApplied(Creature? applier, CardModel? cardSource)
	{
		DynamicVars["PlaysRemaining"].BaseValue = PlaysRemaining;
		return Task.CompletedTask;
	}

	public override Task BeforeCardPlayed(CardPlay cardPlay)
	{
		if (cardPlay.Card.Owner.Creature != Owner)
		{
			return Task.CompletedTask;
		}
		GetInternalData<Data>().cardsBeingPlayed.Add(cardPlay.Card);
		return Task.CompletedTask;
	}

	public override bool ShouldPlay(CardModel card, AutoPlayType _)
	{
		if (card.Owner.Creature != Owner)
		{
			return true;
		}
		Data internalData = GetInternalData<Data>();
		return internalData.numberOfCardsPlayedSinceApplied + internalData.cardsBeingPlayed.Count < Amount;
	}

	public override Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		if (cardPlay.Card.Owner.Creature != Owner)
		{
			return Task.CompletedTask;
		}
		if (!GetInternalData<Data>().cardsBeingPlayed.Remove(cardPlay.Card))
		{
			return Task.CompletedTask;
		}
		GetInternalData<Data>().numberOfCardsPlayedSinceApplied++;
		DynamicVars["PlaysRemaining"].BaseValue = PlaysRemaining;
		InvokeDisplayAmountChanged();
		return Task.CompletedTask;
	}

	public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
	{
		if (participants.Contains(Owner))
		{
			Flash();
			await PowerCmd.Remove(this);
		}
	}
}
