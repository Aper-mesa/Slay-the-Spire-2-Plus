using System;

namespace MegaCrit.Sts2.Core.AutoSlay;

/// <summary>
/// Configuration constants for AutoSlay timeouts and settings.
/// </summary>
public static class AutoSlayConfig
{
	/// <summary>Maximum time for a complete run.</summary>
	public static readonly TimeSpan runTimeout = TimeSpan.FromMinutes(25L);

	/// <summary>Default timeout for room handlers.</summary>
	public static readonly TimeSpan defaultRoomTimeout = TimeSpan.FromMinutes(2L);

	/// <summary>Default timeout for screen handlers.</summary>
	public static readonly TimeSpan defaultScreenTimeout = TimeSpan.FromSeconds(30L);

	/// <summary>Timeout for waiting for game instance to initialize.</summary>
	public static readonly TimeSpan gameInitTimeout = TimeSpan.FromSeconds(10L);

	/// <summary>Timeout for waiting for run state to initialize.</summary>
	public static readonly TimeSpan runStateTimeout = TimeSpan.FromSeconds(30L);

	/// <summary>Timeout for waiting for nodes to appear.</summary>
	public static readonly TimeSpan nodeWaitTimeout = TimeSpan.FromSeconds(10L);

	/// <summary>Timeout for waiting for map screen.</summary>
	public static readonly TimeSpan mapScreenTimeout = TimeSpan.FromSeconds(10L);

	/// <summary>
	/// Timeout for a map point to become travelable. On a post-boss act transition the new act's
	/// map opens via a fire-and-forget act change, so travel can be enabled several seconds after
	/// the run state reports the act switch. Sized to match <see cref="F:MegaCrit.Sts2.Core.AutoSlay.AutoSlayConfig.watchdogTimeout" /> so the
	/// watchdog stays the single hard backstop for genuine hangs.
	/// </summary>
	public static readonly TimeSpan mapPointEnabledTimeout = TimeSpan.FromSeconds(30L);

	/// <summary>
	/// Timeout for the room to be entered after a map click. The watchdog is reset before the map
	/// handler and checked on every slice of this wait, so in AutoSlay it always reports first and
	/// this bound only applies when no watchdog is installed. Equal to <see cref="F:MegaCrit.Sts2.Core.AutoSlay.AutoSlayConfig.watchdogTimeout" />
	/// rather than lower, so it can never fail a run the watchdog would have let through.
	/// </summary>
	public static readonly TimeSpan roomEnterTimeout = TimeSpan.FromSeconds(30L);

	/// <summary>Polling interval for condition checks.</summary>
	public static readonly TimeSpan pollingInterval = TimeSpan.FromMilliseconds(100L, 0L);

	/// <summary>Delay between button interactions.</summary>
	public static readonly TimeSpan buttonClickDelay = TimeSpan.FromMilliseconds(100L, 0L);

	/// <summary>
	/// Floor at which a run that has not ended is abandoned. Seed 1 won on floor 48 in Standard and on floor 49 in
	/// Dread, whose act 3 has a second boss, so this is well above both.
	/// </summary>
	public const int maxFloor = 60;

	/// <summary>
	/// Turn at which AutoSlay starts ramping up Strength. Fights that end before this play out with
	/// no offensive buff so real combat is exercised; only dragging fights (bosses, and DPS-check
	/// encounters like The Insatiable, whose <c>SandpitPower</c> counter hard-kills the player when
	/// it expires) get forced toward a kill. Kept low so the ramp closes the fight out before such a
	/// counter runs down: in local runs The Insatiable dies around turn 4, well ahead of its kill.
	/// </summary>
	public const int combatStrengthRampStartTurn = 3;

	/// <summary>
	/// Strength added each turn once the ramp starts. Stacks additively, so a dragging fight ends
	/// within a turn or two of the ramp beginning, staying ahead of instakill counters and the turn cap.
	/// </summary>
	public const int combatStrengthRampPerTurn = 200;

	/// <summary>Watchdog timeout. If no progress for this long, dump state and fail.</summary>
	public static readonly TimeSpan watchdogTimeout = TimeSpan.FromSeconds(30L);

	/// <summary>Interval for periodic state logging when stuck detection is active.</summary>
	public static readonly TimeSpan watchdogLogInterval = TimeSpan.FromSeconds(5L);
}
