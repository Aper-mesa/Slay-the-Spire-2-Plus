using System.Collections.Generic;
using System.Text.Json.Serialization;
using MegaCrit.Sts2.Core.Models;

namespace MegaCrit.Sts2.Core.Saves;

/// <summary>
/// Holds progress data on a per-character basis.
/// Used by SerializableProgress
/// </summary>
public class CharacterStats
{
	[JsonPropertyName("id")]
	public ModelId? Id { get; init; }

	[JsonPropertyName("max_ascension")]
	public int MaxAscension { get; set; }

	/// <summary>
	/// The player's preferred Ascension level.
	/// Is set when the player chooses an Ascension level that isn't the max level during character select
	/// OR the max ascension level is incremented (they played on their highest difficulty and won).
	/// </summary>
	[JsonPropertyName("preferred_ascension")]
	public int PreferredAscension { get; set; }

	[JsonPropertyName("total_wins")]
	public int TotalWins { get; set; }

	[JsonPropertyName("total_losses")]
	public int TotalLosses { get; set; }

	[JsonPropertyName("fastest_win_time")]
	public long FastestWinTime { get; set; } = -1L;

	[JsonPropertyName("best_win_streak")]
	public long BestWinStreak { get; set; }

	[JsonPropertyName("current_streak")]
	public long CurrentWinStreak { get; set; }

	/// <summary>
	/// Win streak tracking for Dread, separate from the
	/// general win streak above (delete later?).
	/// Tracked separately for singleplayer and multiplayer runs.
	///
	/// The best streak is split into a "this version" and "old version" record rather than one
	/// combined best, since a balance patch can make older records invalid. Both
	/// halves carry the game version they were set on: "this version"'s tag is compared against the
	/// running game version to detect when it has gone stale (see
	/// <see cref="M:MegaCrit.Sts2.Core.Saves.Managers.ProgressSaveManager.UpdateDreadWinStreakOnVictory(MegaCrit.Sts2.Core.Saves.CharacterStats,System.Boolean)" />), at which point it rolls
	/// into the "old version" slot if it beats what's already there.
	/// </summary>
	[JsonPropertyName("best_singleplayer_dread_win_streak_this_version")]
	public int BestSingleplayerDreadWinStreakThisVersion { get; set; }

	[JsonPropertyName("best_singleplayer_dread_win_streak_this_version_tag")]
	public string? BestSingleplayerDreadWinStreakThisVersionTag { get; set; }

	[JsonPropertyName("best_singleplayer_dread_win_streak_old_version")]
	public int BestSingleplayerDreadWinStreakOldVersion { get; set; }

	[JsonPropertyName("best_singleplayer_dread_win_streak_old_version_tag")]
	public string? BestSingleplayerDreadWinStreakOldVersionTag { get; set; }

	[JsonPropertyName("current_singleplayer_dread_win_streak")]
	public int CurrentSingleplayerDreadWinStreak { get; set; }

	[JsonPropertyName("best_multiplayer_dread_win_streak_this_version")]
	public int BestMultiplayerDreadWinStreakThisVersion { get; set; }

	[JsonPropertyName("best_multiplayer_dread_win_streak_this_version_tag")]
	public string? BestMultiplayerDreadWinStreakThisVersionTag { get; set; }

	[JsonPropertyName("best_multiplayer_dread_win_streak_old_version")]
	public int BestMultiplayerDreadWinStreakOldVersion { get; set; }

	[JsonPropertyName("best_multiplayer_dread_win_streak_old_version_tag")]
	public string? BestMultiplayerDreadWinStreakOldVersionTag { get; set; }

	[JsonPropertyName("current_multiplayer_dread_win_streak")]
	public int CurrentMultiplayerDreadWinStreak { get; set; }

	/// <summary>
	/// Cumulative time played for this specific character (in seconds).
	/// </summary>
	[JsonPropertyName("playtime")]
	public long Playtime { get; set; }

	[JsonPropertyName("badges")]
	public List<BadgeStats> Badges { get; set; } = new List<BadgeStats>();
}
