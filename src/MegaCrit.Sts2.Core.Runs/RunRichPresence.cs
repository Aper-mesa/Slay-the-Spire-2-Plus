using System.Collections.Generic;

namespace MegaCrit.Sts2.Core.Runs;

/// <summary>
/// Chooses the rich presence status for a run in progress.
/// The text for each token is in the rich_presence localization table.
/// </summary>
public static class RunRichPresence
{
	public static string GetToken(GameMode gameMode)
	{
		if (gameMode != GameMode.Dread)
		{
			return "IN_RUN";
		}
		return "IN_DREAD_RUN";
	}

	/// <summary>
	/// The values that the status text can refer to as %Key%.
	/// </summary>
	public static IReadOnlyDictionary<string, string> GetValues(string characterEntry, string actEntry, int ascensionLevel, int dreadWinStreak)
	{
		return new Dictionary<string, string>
		{
			["Character"] = characterEntry,
			["Act"] = actEntry,
			["Ascension"] = ascensionLevel.ToString(),
			["DreadStreak"] = dreadWinStreak.ToString()
		};
	}
}
