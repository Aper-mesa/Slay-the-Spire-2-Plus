using System.Text.Json;

namespace MegaCrit.Sts2.Core.Saves;

/// <summary>
/// Tells a whole save from what an interrupted write leaves behind. Every save is written by JsonSerializer, so content
/// that doesn't parse as JSON as written (empty, whitespace, zeroed, cut off) is an interrupted write, even when
/// MigrationManager's JSON repair can fix it. Loading uses this to decide whether a run's .backup is tried, and writing
/// to decide whether the primary may replace its .backup, so the two agree on which file is broken.
/// </summary>
public static class SaveContent
{
	public static bool IsWhole(string content)
	{
		try
		{
			using (JsonDocument.Parse(content))
			{
				return true;
			}
		}
		catch (JsonException)
		{
			return false;
		}
	}
}
