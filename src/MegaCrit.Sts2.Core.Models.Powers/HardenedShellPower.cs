using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;

namespace MegaCrit.Sts2.Core.Models.Powers;

public sealed class HardenedShellPower : PowerModel
{
	private class Data
	{
		public decimal damageReceivedThisTurn;
	}

	public override PowerType Type => PowerType.Buff;

	public override PowerStackType StackType => PowerStackType.Counter;

	public override bool ShouldScaleInMultiplayer => true;

	public override int DisplayAmount => (int)Math.Max(0m, (decimal)Amount - GetInternalData<Data>().damageReceivedThisTurn);

	protected override object InitInternalData()
	{
		return new Data();
	}

	/// <summary>
	/// Card damage previews never show this cap. DamageVar.UpdateCardPreview calls Hook.ModifyDamage, which reaches
	/// ModifyDamageCap but not this hook, and <see cref="T:MegaCrit.Sts2.Core.Models.Powers.IntangiblePower" /> and <see cref="T:MegaCrit.Sts2.Core.Models.Powers.HardToKillPower" /> are the
	/// only powers overriding ModifyDamageCap. Closed as designed in PRG-6920 and PRG-6970.
	/// </summary>
	public override decimal ModifyHpLostBeforeOstyLate(Creature target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
	{
		if (target != Owner)
		{
			return amount;
		}
		if (amount == 0m)
		{
			return amount;
		}
		return Math.Min(amount, (decimal)Amount - GetInternalData<Data>().damageReceivedThisTurn);
	}

	public override Task AfterModifyingHpLostBeforeOsty()
	{
		Flash();
		return Task.CompletedTask;
	}

	public override Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
	{
		if (target != Owner)
		{
			return Task.CompletedTask;
		}
		if (result.WasFullyBlocked)
		{
			return Task.CompletedTask;
		}
		Data internalData = GetInternalData<Data>();
		internalData.damageReceivedThisTurn += (decimal)result.UnblockedDamage;
		InvokeDisplayAmountChanged();
		if (internalData.damageReceivedThisTurn >= (decimal)Amount)
		{
			Owner.HpDisplay = HpDisplay.InfiniteWithNumbers;
		}
		return Task.CompletedTask;
	}

	/// <summary>
	/// The received-damage counter resets at the start of every side turn, not once per round, so the owner caps
	/// damage at its full amount again on the enemy turn as well as on the player turn. There is no filter on
	/// <paramref name="side" /> for that reason. Closed as designed in PRG-4482 and PRG-6913.
	/// </summary>
	public override Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
	{
		GetInternalData<Data>().damageReceivedThisTurn = 0m;
		InvokeDisplayAmountChanged();
		Owner.HpDisplay = HpDisplay.Normal;
		return Task.CompletedTask;
	}
}
