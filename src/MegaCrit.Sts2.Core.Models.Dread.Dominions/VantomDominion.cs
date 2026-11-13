using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Rooms;

namespace MegaCrit.Sts2.Core.Models.Dread.Dominions;

public class VantomDominion : DominionModel
{
	public override IEnumerable<IHoverTip> ExtraHoverTips
	{
		get
		{
			List<IHoverTip> list = new List<IHoverTip>();
			list.AddRange(HoverTipFactory.FromPowerWithPowerHoverTips<SlipperyPower>(2));
			return new _003C_003Ez__ReadOnlyList<IHoverTip>(list);
		}
	}

	public override async Task AfterRoomEntered(AbstractRoom room)
	{
		await base.AfterRoomEntered(room);
		if (room is CombatRoom combatRoom)
		{
			int num = 1;
			if (combatRoom.Encounter.RoomType == RoomType.Elite)
			{
				num = 2;
			}
			await PowerCmd.Apply<SlipperyPower>(new ThrowingPlayerChoiceContext(), combatRoom.Enemies, num, null, null);
		}
	}

	public override async Task AfterCreatureAddedToCombat(Creature creature)
	{
		await base.AfterCreatureAddedToCombat(creature);
		if (creature.Side != CombatSide.Player)
		{
			await PowerCmd.Apply<SlipperyPower>(new ThrowingPlayerChoiceContext(), creature, 1m, null, null);
		}
	}
}
