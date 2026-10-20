using System.Collections.Generic;
using MegaCrit.Sts2.Core.Entities.Players;

namespace MegaCrit.Sts2.Core.Combat.History.Entries;

public class EnergySpentEntry(int amount, Player player, int roundNumber, CombatSide currentSide, CombatHistory history, IEnumerable<Player> players) : CombatHistoryEntry(player.Creature, roundNumber, currentSide, history, players)
{
	public int Amount { get; } = amount;

	public override string Description => Actor.Player.Character.Id.Entry + " spent Amount energy";
}
