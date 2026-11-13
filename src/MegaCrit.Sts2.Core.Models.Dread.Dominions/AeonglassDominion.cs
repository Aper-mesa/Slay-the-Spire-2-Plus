using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Rooms;

namespace MegaCrit.Sts2.Core.Models.Dread.Dominions;

public class AeonglassDominion : DominionModel
{
	public override IEnumerable<IHoverTip> ExtraHoverTips
	{
		get
		{
			List<IHoverTip> list = new List<IHoverTip>();
			list.AddRange(HoverTipFactory.FromCardWithCardHoverTips<Wither>());
			return new _003C_003Ez__ReadOnlyList<IHoverTip>(list);
		}
	}

	public override async Task AfterRoomEntered(AbstractRoom room)
	{
		if (room is CombatRoom combatRoom)
		{
			await PowerCmd.Apply<AeonglassDominionPower>(new ThrowingPlayerChoiceContext(), combatRoom.CombatState.Players.Select((Player p) => p.Creature), 1m, null, null);
		}
	}
}
