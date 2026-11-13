namespace MegaCrit.Sts2.Core.AutoSlay.Scenarios;

/// <summary>
/// One executable line of a scenario script. The subtypes are the complete set: a parsed script
/// cannot hold a line that is neither a directive nor a console command.
/// </summary>
public abstract record ScenarioStatement
{
	/// <summary>
	/// A line handed to the dev console verbatim.
	/// </summary>
	public sealed record ConsoleLine(int LineNumber, string Text) : ScenarioStatement(LineNumber);

	/// <summary>
	/// `wait &lt;condition&gt;`.
	/// </summary>
	public sealed record WaitForCondition(int LineNumber, ScenarioWaitCondition Condition) : ScenarioStatement(LineNumber);

	/// <summary>
	/// `wait &lt;seconds&gt;`.
	/// </summary>
	public sealed record WaitForDuration(int LineNumber, float Seconds) : ScenarioStatement(LineNumber);

	/// <summary>
	/// `endturn`: end the player's turn.
	/// </summary>
	public sealed record EndTurn(int LineNumber) : ScenarioStatement(LineNumber);

	/// <summary>
	/// 1-based line in the source file, used for every failure message.
	/// </summary>
	public int LineNumber { get; }

	private protected ScenarioStatement(int lineNumber)
	{
		LineNumber = lineNumber;
	}
}
