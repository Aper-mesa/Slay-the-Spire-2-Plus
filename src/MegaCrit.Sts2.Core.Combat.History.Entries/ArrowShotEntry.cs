using System.Collections.Generic;
using MegaCrit.Sts2.Core.Entities.FoolArrows;
using MegaCrit.Sts2.Core.Entities.Players;

namespace MegaCrit.Sts2.Core.Combat.History.Entries;

public class ArrowShotEntry : CombatHistoryEntry
{
	public ArrowSlot Slot { get; }

	public override string Description => Actor.Player.Character.Id.Entry + " shot arrow";

	public ArrowShotEntry(ArrowSlot slot, int roundNumber, CombatSide currentSide, CombatHistory history, IEnumerable<Player> players)
		: base(slot.owner.Creature, roundNumber, currentSide, history, players)
	{
		Slot = slot;
	}
}
