using System;
using System.Collections.Generic;
using System.Globalization;
using Godot;

namespace MegaCrit.Sts2.Core.AutoSlay.Scenarios;

/// <summary>
/// A parsed scenario: an ordered, validated statement list. Node-free and singleton-free on
/// purpose, so it is testable without a running game.
/// </summary>
public sealed class ScenarioScript
{
	/// <summary>
	/// The longest a single `wait &lt;seconds&gt;` may block. Anything longer is a hang, not a wait.
	/// </summary>
	/// <remarks>
	/// Under run_scenario.py's 360s kill, which skips the runner's `finally` and so writes no
	/// sentinel. Bounds one statement, not the script: several long waits can still outlast it.
	/// </remarks>
	private const float _maxWaitSeconds = 120f;

	/// <summary>
	/// The statements to execute, in file order. Comments and blank lines are already gone.
	/// </summary>
	public IReadOnlyList<ScenarioStatement> Statements { get; }

	private ScenarioScript(IReadOnlyList<ScenarioStatement> statements)
	{
		Statements = statements;
	}

	/// <summary>
	/// Reads and parses a scenario file. Fails before any game state is touched.
	/// </summary>
	public static ScenarioParseResult ParseFile(string path, IReadOnlySet<string> reservedNames)
	{
		using FileAccess fileAccess = FileAccess.Open(path, FileAccess.ModeFlags.Read);
		if (fileAccess == null)
		{
			return new ScenarioParseResult.FileError(path, FileAccess.GetOpenError().ToString());
		}
		string asText = fileAccess.GetAsText();
		Error error = fileAccess.GetError();
		if (error != Error.Ok)
		{
			return new ScenarioParseResult.FileError(path, error.ToString());
		}
		return Parse(asText.Split('\n'), reservedNames);
	}

	/// <summary>
	/// Parses scenario lines. <paramref name="reservedNames" /> is the registered console command
	/// set; a directive that shadows one is a configuration error rather than a silent shadow.
	/// </summary>
	public static ScenarioParseResult Parse(IEnumerable<string> lines, IReadOnlySet<string> reservedNames)
	{
		IReadOnlyList<string> readOnlyList = ScenarioDirectives.FindCollisions(reservedNames);
		if (readOnlyList.Count > 0)
		{
			return new ScenarioParseResult.ConfigurationError("Directives shadow registered console commands: " + string.Join(", ", readOnlyList));
		}
		List<ScenarioStatement> list = new List<ScenarioStatement>();
		int num = 0;
		foreach (string line in lines)
		{
			num++;
			string text = StripComment(line.Trim()).TrimEnd();
			if (text.Length == 0)
			{
				continue;
			}
			string[] array = text.Split(new char[2] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
			string text2 = array[0];
			if (!ScenarioDirectives.all.Contains(text2))
			{
				list.Add(new ScenarioStatement.ConsoleLine(num, string.Join(' ', array)));
			}
			else if (text2.Equals("wait", StringComparison.OrdinalIgnoreCase))
			{
				if (array.Length != 2)
				{
					return ArgumentCountError(num, "wait", "exactly one condition name or duration in seconds");
				}
				if (ScenarioDirectives.waitConditions.TryGetValue(array[1], out var value))
				{
					list.Add(new ScenarioStatement.WaitForCondition(num, value));
					continue;
				}
				if (!float.TryParse(array[1], NumberStyles.Float, CultureInfo.InvariantCulture, out var result) || !float.IsFinite(result) || result < 0f)
				{
					return new ScenarioParseResult.LineError(num, $"'{array[1]}' is neither a known wait condition ({string.Join(", ", ScenarioDirectives.waitConditions.Keys)}) nor a non-negative number of seconds.");
				}
				if (result > 120f)
				{
					return new ScenarioParseResult.LineError(num, $"'wait {array[1]}' exceeds the maximum wait of {120f} seconds.");
				}
				list.Add(new ScenarioStatement.WaitForDuration(num, result));
			}
			else
			{
				if (!text2.Equals("endturn", StringComparison.OrdinalIgnoreCase))
				{
					return new ScenarioParseResult.ConfigurationError("Directive '" + text2 + "' is in ScenarioDirectives.all but has no parse branch.");
				}
				if (array.Length != 1)
				{
					return ArgumentCountError(num, "endturn", "no arguments");
				}
				list.Add(new ScenarioStatement.EndTurn(num));
			}
		}
		return new ScenarioParseResult.Success(new ScenarioScript(list));
	}

	/// <summary>
	/// Discards an inline comment: a '#' preceded by whitespace, or at the start of the line. The
	/// console commands here take model ids and numbers, so a literal whitespace-then-hash argument
	/// is not a real case.
	/// </summary>
	private static string StripComment(string line)
	{
		for (int i = 0; i < line.Length; i++)
		{
			if (line[i] == '#' && (i == 0 || char.IsWhiteSpace(line[i - 1])))
			{
				return line.Substring(0, i);
			}
		}
		return line;
	}

	private static ScenarioParseResult.LineError ArgumentCountError(int lineNumber, string directive, string expected)
	{
		return new ScenarioParseResult.LineError(lineNumber, $"'{directive}' takes {expected}.");
	}
}
