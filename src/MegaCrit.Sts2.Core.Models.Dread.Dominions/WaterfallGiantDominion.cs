using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Rooms;

namespace MegaCrit.Sts2.Core.Models.Dread.Dominions;

public class WaterfallGiantDominion : DominionModel
{
	private const int _weakExplosiveAmount = 4;

	private const int _strongExplosiveAmount = 5;

	private const int _eliteExplosiveAmount = 7;

	private const int _multiExplosiveAmount = 3;

	public override async Task AfterRoomEntered(AbstractRoom room)
	{
		if (room is CombatRoom combatRoom)
		{
			await PowerCmd.Apply<ExplosivePower>(new ThrowingPlayerChoiceContext(), combatRoom.CombatState.Enemies, GetExplosiveAmount(combatRoom.Encounter), null, null);
		}
	}

	protected override Task AfterEnemiesEnteredCombat(IEnumerable<Creature> creatures)
	{
		Creature[] array = creatures.Where((Creature c) => !c.HasPower<MinionPower>()).ToArray();
		if (array.Length == 0)
		{
			return Task.CompletedTask;
		}
		if (!(array[0].CombatState?.RunState.CurrentRoom is CombatRoom combatRoom))
		{
			return Task.CompletedTask;
		}
		return PowerCmd.Apply<ExplosivePower>(new ThrowingPlayerChoiceContext(), array, GetExplosiveAmount(combatRoom.Encounter), null, null);
	}

	private int GetExplosiveAmount(EncounterModel encounter)
	{
		if (encounter.MonstersWithSlots.Count > 1)
		{
			return 3;
		}
		if (encounter.IsWeak)
		{
			return 4;
		}
		if (encounter.RoomType == RoomType.Elite)
		{
			return 7;
		}
		return 5;
	}
}
