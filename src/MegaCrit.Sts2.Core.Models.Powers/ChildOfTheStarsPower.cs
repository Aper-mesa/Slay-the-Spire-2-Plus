using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace MegaCrit.Sts2.Core.Models.Powers;

public sealed class ChildOfTheStarsPower : PowerModel
{
	public override PowerType Type => PowerType.Buff;

	public override PowerStackType StackType => PowerStackType.Counter;

	public override async Task AfterStarsSpent(int amount, Player spender)
	{
		if (amount > 0 && spender == Owner.Player)
		{
			Flash();
			await CreatureCmd.GainBlock(Owner, Amount * amount, ValueProp.Unpowered, null);
		}
	}
}
