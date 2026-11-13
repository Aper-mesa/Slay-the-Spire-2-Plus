using System;
using System.Collections.Generic;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Runs;

namespace MegaCrit.Sts2.Core.Entities.Ascension;

/// <summary>
/// Manages ascension logic for runs.
/// Responsible for tracking ascension levels and applying ascension effects.
/// </summary>
public class AscensionManager
{
	/// <summary>
	/// The maximum ascension level allowed in standard mode.
	/// </summary>
	public const int maxAscensionAllowed = 10;

	/// <summary>
	/// The maximum ascension level allowed in blitz mode.
	/// </summary>
	public const int maxAscensionAllowedBlitz = 5;

	/// <summary>
	/// The current ascensions active for the run.
	/// </summary>
	private readonly List<AscensionLevel> _ascensions;

	/// <summary>
	/// The maximum ascension level allowed in the given game mode.
	/// </summary>
	public static int GetMaxAllowed(GameMode gameMode)
	{
		if (gameMode != GameMode.Blitz)
		{
			return 10;
		}
		return 5;
	}

	/// <summary>
	/// Clamps an ascension level read from a save or a history entry to the range the game mode supports.
	/// Blitz allowed 10 levels before it was cut to 5, so older data can carry a higher value.
	/// </summary>
	public static int ClampToMode(GameMode gameMode, int level)
	{
		return Math.Clamp(level, 0, GetMaxAllowed(gameMode));
	}

	/// <summary>
	/// Create a new AscensionManager with the specified ascension level for the given game mode.
	/// </summary>
	/// <param name="gameMode">The game mode of this run.</param>
	/// <param name="level">The ascension level for this run.</param>
	public AscensionManager(GameMode gameMode, int level)
	{
		_ascensions = new List<AscensionLevel>();
		for (int i = 1; i <= level; i++)
		{
			_ascensions.Add((gameMode == GameMode.Blitz) ? AscensionHelper.GetLevelForBlitz(i) : ((AscensionLevel)i));
		}
	}

	/// <summary>
	/// Check if a given ascension level is active in the current run.
	/// </summary>
	/// <param name="level">The ascension level to check.</param>
	/// <returns>True if the specified level is active.</returns>
	public bool HasLevel(AscensionLevel level)
	{
		return _ascensions.Contains(level);
	}

	/// <summary>
	/// Apply ascension effects to a player based on the current ascension level.
	/// </summary>
	/// <param name="player">The player to apply effects to.</param>
	public void ApplyEffectsTo(Player player)
	{
		if (HasLevel(AscensionLevel.TightBelt))
		{
			player.SubtractFromMaxPotionCount(1);
		}
		if (HasLevel(AscensionLevel.AscendersBane))
		{
			AscendersBane ascendersBane = player.RunState.CreateCard<AscendersBane>(player);
			ascendersBane.FloorAddedToDeck = 1;
			player.Deck.AddInternal(ascendersBane, -1, silent: true);
		}
	}
}
