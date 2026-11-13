namespace MegaCrit.Sts2.Core.AutoSlay.Scenarios;

/// <summary>
/// The process exit code for an outcome. Pure so it can be asserted without a running game.
/// </summary>
public static class ScenarioExitCode
{
	public static int For(ScenarioOutcome outcome)
	{
		return outcome switch
		{
			ScenarioOutcome.Completed => 0,
			ScenarioOutcome.Failed => 1,
			ScenarioOutcome.Stalled => 2,
			ScenarioOutcome.InternalError => 3,
			_ => 3,
		};
	}
}
