namespace MegaCrit.Sts2.Core.AutoSlay.Scenarios;

/// <summary>
/// How a scenario run ended. The log sentinel is the primary signal; the exit code is the
/// machine-readable echo of it.
/// </summary>
public enum ScenarioOutcome
{
	/// <summary>Every statement executed and the script ran to its end.</summary>
	Completed,
	/// <summary>The script was rejected, or a statement failed. The scenario's own fault.</summary>
	Failed,
	/// <summary>A wait condition or the watchdog timed out. Distinguished so a hang is not read as a bug in the script.</summary>
	Stalled,
	/// <summary>An unexpected exception escaped the runner. The runner's own fault.</summary>
	InternalError
}
