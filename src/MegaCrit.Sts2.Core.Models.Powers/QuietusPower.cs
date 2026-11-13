using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.FoolArrows;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;

namespace MegaCrit.Sts2.Core.Models.Powers;

public sealed class QuietusPower : PowerModel
{
	private class Data
	{
		public AttackCommand? commandToModify;
	}

	public override PowerType Type => PowerType.Buff;

	public override PowerStackType StackType => PowerStackType.Counter;

	protected override object InitInternalData()
	{
		return new Data();
	}

	public override Task AfterArrowShot(Player owner, ArrowSlot? arrow, PlayerChoiceContext context, CardModel? source)
	{
		if (arrow == null)
		{
			return Task.CompletedTask;
		}
		if (source == null)
		{
			return Task.CompletedTask;
		}
		if (owner.Creature != Owner)
		{
			return Task.CompletedTask;
		}
		arrow.DestroyAfterImpaled = true;
		return Task.CompletedTask;
	}

	public override Task BeforeAttack(AttackCommand command)
	{
		if (command.Attacker != Owner)
		{
			return Task.CompletedTask;
		}
		if (!command.DamageProps.IsPoweredAttack())
		{
			return Task.CompletedTask;
		}
		if (!command.DamageProps.HasFlag(ValueProp.FromArrow))
		{
			return Task.CompletedTask;
		}
		Data internalData = GetInternalData<Data>();
		if (internalData.commandToModify != null)
		{
			return Task.CompletedTask;
		}
		if (command.ModelSource != null && !(command.ModelSource is CardModel))
		{
			return Task.CompletedTask;
		}
		if (!command.DamageProps.IsPoweredAttack())
		{
			return Task.CompletedTask;
		}
		internalData.commandToModify = command;
		return Task.CompletedTask;
	}

	public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource, CardPlay? cardPlay)
	{
		if (Owner != dealer)
		{
			return 1m;
		}
		if (!props.IsPoweredAttack())
		{
			return 1m;
		}
		if (!props.HasFlag(ValueProp.FromArrow))
		{
			return 1m;
		}
		Data internalData = GetInternalData<Data>();
		if (internalData.commandToModify != null && cardSource != null && cardSource != internalData.commandToModify.ModelSource)
		{
			return 1m;
		}
		return Amount;
	}

	public override async Task AfterAttack(PlayerChoiceContext choiceContext, AttackCommand command)
	{
		Data internalData = GetInternalData<Data>();
		if (command == internalData.commandToModify)
		{
			await PowerCmd.Remove(this);
		}
	}

	public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
	{
		if (participants.Contains(Owner))
		{
			await PowerCmd.Remove(this);
		}
	}
}
