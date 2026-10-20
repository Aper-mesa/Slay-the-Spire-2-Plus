using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Map;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;

namespace MegaCrit.Sts2.Core.Models.Relics;

public sealed class Planisphere : RelicModel
{
	public override RelicRarity Rarity => RelicRarity.Uncommon;

	protected override IEnumerable<DynamicVar> CanonicalVars => new _003C_003Ez__ReadOnlySingleElementList<DynamicVar>(new HealVar(5m));

	public override bool IsAllowed(IRunState runState)
	{
		return RelicModel.IsBeforeAct3TreasureChest(runState);
	}

	public override async Task AfterRoomEntered(AbstractRoom room)
	{
		if (!Owner.Creature.IsDead)
		{
			MapPoint? currentMapPoint = Owner.RunState.CurrentMapPoint;
			if (currentMapPoint != null && currentMapPoint.PointType == MapPointType.Unknown && Owner.RunState.CurrentRoomCount <= 1)
			{
				Flash();
				await CreatureCmd.Heal(Owner.Creature, DynamicVars.Heal.BaseValue);
			}
		}
	}
}
