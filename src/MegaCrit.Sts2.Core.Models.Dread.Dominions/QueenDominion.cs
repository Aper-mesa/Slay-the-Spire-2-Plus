using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Rooms;

namespace MegaCrit.Sts2.Core.Models.Dread.Dominions;

public class QueenDominion : DominionModel
{
	private const int _debuffAmount = 3;

	public override IEnumerable<IHoverTip> ExtraHoverTips
	{
		get
		{
			List<IHoverTip> list = new List<IHoverTip>();
			list.AddRange(HoverTipFactory.FromPowerWithPowerHoverTips<WeakPower>(3));
			list.AddRange(HoverTipFactory.FromPowerWithPowerHoverTips<VulnerablePower>(3));
			list.AddRange(HoverTipFactory.FromPowerWithPowerHoverTips<FrailPower>(3));
			return new _003C_003Ez__ReadOnlyList<IHoverTip>(list);
		}
	}

	public override async Task AfterRoomEntered(AbstractRoom room)
	{
		if (room is CombatRoom combatRoom)
		{
			Creature[] targets = combatRoom.CombatState.Players.Select((Player p) => p.Creature).ToArray();
			await PowerCmd.Apply<YouAreMinePower>(new ThrowingPlayerChoiceContext(), targets, 3m, null, null);
		}
	}
}
