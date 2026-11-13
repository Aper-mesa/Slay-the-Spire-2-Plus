using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Rooms;

namespace MegaCrit.Sts2.Core.Models.Dread.Mocks;

public class MockDominion : DominionModel
{
	public override async Task AfterRoomEntered(AbstractRoom room)
	{
		if (room is CombatRoom combatRoom)
		{
			await PowerCmd.Apply<StrengthPower>(new ThrowingPlayerChoiceContext(), combatRoom.CombatState.Players.Select((Player p) => p.Creature), -1m, null, null);
			await PowerCmd.Apply<DexterityPower>(new ThrowingPlayerChoiceContext(), combatRoom.CombatState.Players.Select((Player p) => p.Creature), -1m, null, null);
		}
	}
}
