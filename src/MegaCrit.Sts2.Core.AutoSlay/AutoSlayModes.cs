using System;
using System.Linq;
using MegaCrit.Sts2.Core.Runs;

namespace MegaCrit.Sts2.Core.AutoSlay;

public static class AutoSlayModes
{
	/// <summary>Parses the <c>--mode</c> value. A missing value means <see cref="F:MegaCrit.Sts2.Core.AutoSlay.AutoSlayMode.Standard" />.</summary>
	/// <exception cref="T:System.ArgumentException">The value names no mode AutoSlay can play.</exception>
	public static AutoSlayMode Parse(string? value)
	{
		if (value == null)
		{
			return AutoSlayMode.Standard;
		}
		AutoSlayMode[] values = Enum.GetValues<AutoSlayMode>();
		for (int i = 0; i < values.Length; i++)
		{
			AutoSlayMode result = values[i];
			if (string.Equals(result.ToString(), value, StringComparison.OrdinalIgnoreCase))
			{
				return result;
			}
		}
		string text = string.Join(", ", from n in Enum.GetNames<AutoSlayMode>()
			select n.ToLowerInvariant());
		throw new ArgumentException("Unknown --mode '" + value + "'. Expected one of: " + text);
	}

	/// <summary>The <see cref="T:MegaCrit.Sts2.Core.Runs.GameMode" /> a run started in <paramref name="mode" /> must report.</summary>
	public static GameMode ToGameMode(this AutoSlayMode mode)
	{
		return mode switch
		{
			AutoSlayMode.Standard => GameMode.Standard,
			AutoSlayMode.Blitz => GameMode.Blitz,
			AutoSlayMode.Dread => GameMode.Dread,
			_ => throw new ArgumentOutOfRangeException("mode", mode, null),
		};
	}
}
