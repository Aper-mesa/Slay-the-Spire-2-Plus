using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Rooms;

namespace MegaCrit.Sts2.Core.Models.Relics;

public sealed class Pantograph : RelicModel
{
	public override RelicRarity Rarity => RelicRarity.Uncommon;

	protected override IEnumerable<DynamicVar> CanonicalVars => new _003C_003Ez__ReadOnlySingleElementList<DynamicVar>(new HealVar(25m));

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
		if (!Owner.Creature.IsDead && Owner.RunState.CurrentRoom.RoomType == RoomType.Boss)
		{
			Flash();
			await CreatureCmd.Heal(Owner.Creature, DynamicVars.Heal.BaseValue);
		}
	}
}
