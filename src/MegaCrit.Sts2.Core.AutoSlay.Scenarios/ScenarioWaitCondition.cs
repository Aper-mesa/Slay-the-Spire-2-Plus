namespace MegaCrit.Sts2.Core.AutoSlay.Scenarios;

/// <summary>
/// The named conditions a scenario's `wait` directive can block on. Conditions are preferred over
/// durations because durations change meaning with FastMode.
/// </summary>
public enum ScenarioWaitCondition
{
	/// <summary>
	/// Combat is in progress and the local player has reached its play phase. IsInProgress alone
	/// flips true during combat setup, when statements are not yet safe to run.
	/// </summary>
	CombatStarted,
	/// <summary>
	/// The player's turn number has advanced past the turn observed at the preceding `endturn`, and
	/// the turn is in its play phase.
	/// </summary>
	TurnStarted
}
