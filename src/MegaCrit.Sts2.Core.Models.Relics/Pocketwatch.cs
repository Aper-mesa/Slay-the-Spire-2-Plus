using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Rooms;

namespace MegaCrit.Sts2.Core.Models.Relics;

public sealed class Pocketwatch : RelicModel
{
	private const string _cardThresholdKey = "CardThreshold";

	private int _cardsPlayedThisTurn;

	private int _cardsPlayedLastTurn;

	public override RelicRarity Rarity => RelicRarity.Rare;

	public override bool ShowCounter => CombatManager.Instance.IsInProgress;

	public override int DisplayAmount => _cardsPlayedThisTurn;

	protected override IEnumerable<DynamicVar> CanonicalVars => new _003C_003Ez__ReadOnlyArray<DynamicVar>(new DynamicVar[2]
	{
		new DynamicVar("CardThreshold", 3m),
		new CardsVar(3)
	});

	public override Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		if (cardPlay.Card.Owner != Owner)
		{
			return Task.CompletedTask;
		}
		if (!CombatManager.Instance.IsInProgress)
		{
			return Task.CompletedTask;
		}
		_cardsPlayedThisTurn++;
		RefreshCounter();
		return Task.CompletedTask;
	}

	public override decimal ModifyHandDraw(Player player, decimal count)
	{
		if (player != Owner)
		{
			return count;
		}
		if (Owner.PlayerCombatState.TurnNumber == 1)
		{
			return count;
		}
		if ((decimal)_cardsPlayedLastTurn > DynamicVars["CardThreshold"].BaseValue)
		{
			return count;
		}
		return count + DynamicVars.Cards.BaseValue;
	}

	public override Task AfterModifyingHandDraw()
	{
		Flash();
		return Task.CompletedTask;
	}

	public override Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
	{
		if (!participants.Contains(Owner.Creature))
		{
			return Task.CompletedTask;
		}
		_cardsPlayedLastTurn = _cardsPlayedThisTurn;
		_cardsPlayedThisTurn = 0;
		return Task.CompletedTask;
	}

	public override Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
	{
		if (!participants.Contains(Owner.Creature))
		{
			return Task.CompletedTask;
		}
		RefreshCounter();
		return Task.CompletedTask;
	}

	private void RefreshCounter()
	{
		Status = (((decimal)_cardsPlayedThisTurn <= DynamicVars["CardThreshold"].BaseValue) ? RelicStatus.Active : RelicStatus.Normal);
		InvokeDisplayAmountChanged();
	}

	public override Task AfterCombatEnd(CombatRoom _)
	{
		_cardsPlayedThisTurn = 0;
		_cardsPlayedLastTurn = 0;
		Status = RelicStatus.Normal;
		InvokeDisplayAmountChanged();
		return Task.CompletedTask;
	}
}
