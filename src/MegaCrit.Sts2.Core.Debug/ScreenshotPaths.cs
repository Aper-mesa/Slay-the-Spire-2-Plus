using System;
using System.Globalization;
using System.Text;

namespace MegaCrit.Sts2.Core.Debug;

/// <summary>
/// Where a `screenshot` is written. Pure, with the clock injected, so the naming rules are
/// assertable without a running game.
/// </summary>
public static class ScreenshotPaths
{
	private const string _fallbackName = "scenario";

	private const int _maxNameLength = 64;

	/// <summary>
	/// Where a `screenshot` is written. The label comes from a human or a script line and has never
	/// been validated, so it is sanitized here.
	/// </summary>
	public static string BuildScreenshotPath(string label, DateTime takenAt)
	{
		return $"user://screenshots/{takenAt.ToString("yyyyMMdd-HHmmss-fff", CultureInfo.InvariantCulture)}-{Sanitize(label)}.png";
	}

	/// <summary>
	/// Lowercases and reduces to ASCII letters, digits, '-' and '_'. Anything else, including a path
	/// separator or a run of dots, collapses to a single '-'.
	/// </summary>
	private static string Sanitize(string value)
	{
		StringBuilder stringBuilder = new StringBuilder(value.Length);
		bool flag = false;
		foreach (char c in value)
		{
			if (char.IsAsciiLetterOrDigit(c) || c == '_')
			{
				stringBuilder.Append(char.ToLowerInvariant(c));
				flag = false;
			}
			else if (!flag)
			{
				stringBuilder.Append('-');
				flag = true;
			}
		}
		string text = stringBuilder.ToString().Trim('-');
		if (text.Length > 64)
		{
			text = text.Substring(0, 64).TrimEnd('-');
		}
		if (text.Length != 0)
		{
			return text;
		}
		return "scenario";
	}
}
