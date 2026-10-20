using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace MegaCrit.Sts2.Core.Models.Powers;

public sealed class PanachePower : PowerModel
{
	private class Data
	{
		/// <summary>
		/// We track this so we don't count the Panache card towards itself.
		/// </summary>
		public bool alreadyApplied;
	}

	private const int _baseCardsLeft = 5;

	private const string _cardsLeftKey = "CardsLeft";

	public override PowerType Type => PowerType.Buff;

	public override PowerStackType StackType => PowerStackType.Counter;

	public override int DisplayAmount => DynamicVars["CardsLeft"].IntValue;

	public override PowerInstanceType InstanceType => PowerInstanceType.Instanced;

	protected override IEnumerable<DynamicVar> CanonicalVars => new _003C_003Ez__ReadOnlySingleElementList<DynamicVar>(new DynamicVar("CardsLeft", 5m));

	protected override object InitInternalData()
	{
		return new Data();
	}

	public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		if (cardPlay.Card.Owner != Owner.Player)
		{
			return;
		}
		Data data = GetInternalData<Data>();
		if (data.alreadyApplied)
		{
			DynamicVars["CardsLeft"].BaseValue--;
			InvokeDisplayAmountChanged();
			if (DynamicVars["CardsLeft"].IntValue <= 0)
			{
				await Cmd.Wait(0.5f);
				await CreatureCmd.Damage(choiceContext, CombatState.HittableEnemies, Amount, ValueProp.Unpowered, Owner);
				DynamicVars["CardsLeft"].BaseValue = 5m;
				InvokeDisplayAmountChanged();
			}
		}
		data.alreadyApplied = true;
	}

	public override Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
	{
		if (!participants.Contains(Owner))
		{
			return Task.CompletedTask;
		}
		DynamicVars["CardsLeft"].BaseValue = 5m;
		InvokeDisplayAmountChanged();
		return Task.CompletedTask;
	}
}
