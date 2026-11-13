using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace MegaCrit.Sts2.Core.Models.Relics;

public sealed class BathRobes : RelicModel
{
	private bool _wasUsed;

	public override RelicRarity Rarity => RelicRarity.Event;

	public override bool IsUsedUp => _wasUsed;

	[SavedProperty]
	public bool WasUsed
	{
		get
		{
			return _wasUsed;
		}
		set
		{
			AssertMutable();
			_wasUsed = value;
			if (IsUsedUp)
			{
				Status = RelicStatus.Disabled;
			}
		}
	}

	public override Task AfterRoomEntered(AbstractRoom room)
	{
		if (Owner.Creature.IsDead)
		{
			return Task.CompletedTask;
		}
		bool flag = Owner.RunState.Map.BossMapPoint.parents.Contains(Owner.RunState.CurrentMapPoint);
		Status = (flag ? RelicStatus.Active : RelicStatus.Normal);
		return Task.CompletedTask;
	}

	public override async Task BeforeCombatStart()
	{
		if (!WasUsed && !Owner.Creature.IsDead && Owner.RunState.CurrentRoom.RoomType == RoomType.Boss)
		{
			Flash();
			await CreatureCmd.Heal(Owner.Creature, Owner.Creature.MaxHp - Owner.Creature.CurrentHp);
			WasUsed = true;
		}
	}
}
