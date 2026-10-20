using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace MegaCrit.Sts2.Core.Models.Monsters;

public class MysteriousKnight : FlailKnight
{
	public override async Task AfterAddedToRoom()
	{
		await base.AfterAddedToRoom();
		await PowerCmd.Apply<StrengthPower>(new ThrowingPlayerChoiceContext(), Creature, 6m, Creature, null);
		await PowerCmd.Apply<PlatingPower>(new ThrowingPlayerChoiceContext(), Creature, 6m, Creature, null);
	}
}
