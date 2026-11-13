namespace MegaCrit.Sts2.Core.AutoSlay.Scenarios;

/// <summary>
/// The outcome of parsing a scenario. Either a script or a described failure, never both and never
/// a script that still needs checking.
/// </summary>
public abstract record ScenarioParseResult
{
	/// <summary>
	/// The script parsed and every statement is executable.
	/// </summary>
	public sealed record Success(ScenarioScript Script) : ScenarioParseResult
	{
		/// <inheritdoc />
		public override string Describe()
		{
			return "Scenario parsed successfully";
		}
	}

	/// <summary>
	/// A statement was rejected.
	/// </summary>
	public sealed record LineError(int LineNumber, string Message) : ScenarioParseResult
	{
		/// <inheritdoc />
		public override string Describe()
		{
			return $"Scenario parse error on line {LineNumber}: {Message}";
		}
	}

	/// <summary>
	/// The scenario file could not be read. Raised before any game state is touched.
	/// </summary>
	public sealed record FileError(string Path, string Message) : ScenarioParseResult
	{
		/// <inheritdoc />
		public override string Describe()
		{
			return "Scenario file '" + Path + "' could not be read: " + Message;
		}
	}

	/// <summary>
	/// The directive registry itself is wrong: a directive name collides with a registered console
	/// command, or a name in the registry has no parse branch.
	/// </summary>
	public sealed record ConfigurationError(string Message) : ScenarioParseResult
	{
		/// <inheritdoc />
		public override string Describe()
		{
			return "Scenario configuration error: " + Message;
		}
	}

	private protected ScenarioParseResult()
	{
	}

	/// <summary>
	/// A log-ready description of this outcome.
	/// </summary>
	public abstract string Describe();
}
