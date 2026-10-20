using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;

namespace MegaCrit.Sts2.Core.Models.Powers;

public sealed class ArtifactPower : PowerModel
{
	public override PowerType Type => PowerType.Buff;

	public override PowerStackType StackType => PowerStackType.Counter;

	public override bool ShouldScaleInMultiplayer => true;

	public override bool TryModifyPowerAmountReceived(PowerModel canonicalPower, Creature target, decimal amount, Creature? _, out decimal modifiedAmount)
	{
		if (target != Owner)
		{
			modifiedAmount = amount;
			return false;
		}
		if (canonicalPower.GetTypeForAmount(amount) != PowerType.Debuff)
		{
			modifiedAmount = amount;
			return false;
		}
		if (!canonicalPower.IsVisible)
		{
			modifiedAmount = amount;
			return false;
		}
		modifiedAmount = 0m;
		return true;
	}

	public override async Task AfterModifyingPowerAmountReceived(PowerModel power)
	{
		await PowerCmd.Decrement(this);
	}

	public override decimal GetScaledAmountForMultiplayer(ICombatState combatState, Creature? applier, decimal amount, Creature target, CardModel? cardSource)
	{
		return amount + (decimal)combatState.Players.Count - 1m;
	}
}
