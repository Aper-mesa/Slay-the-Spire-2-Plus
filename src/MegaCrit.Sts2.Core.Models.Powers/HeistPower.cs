using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Rooms;

namespace MegaCrit.Sts2.Core.Models.Powers;

public sealed class HeistPower : PowerModel
{
	public override PowerType Type => PowerType.Buff;

	public override PowerStackType StackType => PowerStackType.Counter;

	public override PowerInstanceType InstanceType => PowerInstanceType.Instanced;

	public override Task BeforeDeath(Creature target)
	{
		if (Owner != target)
		{
			return Task.CompletedTask;
		}
		if (CombatState.RunState.CurrentRoom is CombatRoom combatRoom)
		{
			combatRoom.AddExtraReward(Target.Player, new GoldReward(Amount, Target.Player, wasGoldStolenBack: true));
		}
		CombatState.RunState.CurrentMapPointHistoryEntry?.GetEntry(Target.Player.NetId).MarkLootReturned(Amount);
		return Task.CompletedTask;
	}
}
