using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;

namespace MegaCrit.Sts2.Core.Models.Dread;

/// <summary>
/// A model which modifies the act in which a boss resides.
/// Used to apply Dread Mode effects to an act for the current boss.
/// </summary>
public abstract class DominionModel : AbstractModel
{
	private RunState? _runState;

	private const string _entryPrefix = "_DOMINION";

	/// <summary>
	/// Saves a ref of the players entering combat due to hook timings.
	/// This list is created in AfterRoomEntered and is then used by AfterPlayersEnteredCombat.
	/// </summary>
	private IReadOnlyList<Player>? _playersEnteringCombat;

	public RunState? RunState
	{
		get
		{
			AssertMutable();
			return _runState;
		}
		set
		{
			AssertMutable();
			if (_runState != null && _runState != value)
			{
				throw new InvalidOperationException("Run State has already been set");
			}
			_runState = value;
		}
	}

	public override bool ShouldReceiveCombatHooks => true;

	public LocString Description
	{
		get
		{
			string entry = Id.Entry;
			int length = "_DOMINION".Length;
			return new LocString("ascension", "DREAD." + entry.Substring(0, entry.Length - length) + ".description");
		}
	}

	public virtual IEnumerable<IHoverTip> ExtraHoverTips => Array.Empty<IHoverTip>();

	public override Task AfterRoomEntered(AbstractRoom room)
	{
		if (!(room is CombatRoom combatRoom))
		{
			return Task.CompletedTask;
		}
		AssertMutable();
		_playersEnteringCombat = combatRoom.CombatState.Players;
		return AfterEnemiesEnteredCombat(combatRoom.Enemies);
	}

	public override Task AfterCreatureAddedToCombat(Creature creature)
	{
		if (creature.Side == CombatSide.Player)
		{
			return Task.CompletedTask;
		}
		return AfterEnemiesEnteredCombat(new _003C_003Ez__ReadOnlySingleElementList<Creature>(creature));
	}

	public override async Task BeforeCombatStart()
	{
		if (_playersEnteringCombat != null)
		{
			AssertMutable();
			IReadOnlyList<Player> playersEnteringCombat = _playersEnteringCombat;
			_playersEnteringCombat = null;
			await AfterPlayersEnteredCombat(playersEnteringCombat);
		}
	}

	/// <summary>
	/// Runs when a creature enters combat. This happens both when:
	///  - Combat starts with a creature already in the room
	///  - A creature spawns during combat
	///
	/// This is a very common operation for dominions, which is why it's here, but we can repurpose this into a general
	/// hook if desired.
	/// </summary>
	/// <param name="creatures">The creatures that entered combat.</param>
	protected virtual Task AfterEnemiesEnteredCombat(IEnumerable<Creature> creatures)
	{
		return Task.CompletedTask;
	}

	/// <summary>
	/// Runs once combat is in progress for the players who are entering it. Used to add cards to
	/// their draw piles with things like SoulFyshDominion. Fires once per combat, via BeforeCombatStart.
	/// </summary>
	/// <param name="players">The players that entered combat.</param>
	protected virtual Task AfterPlayersEnteredCombat(IReadOnlyList<Player> players)
	{
		return Task.CompletedTask;
	}
}
