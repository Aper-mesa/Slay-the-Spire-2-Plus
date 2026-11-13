using System.Text.Json.Serialization;
using MegaCrit.Sts2.Core.Models;

namespace MegaCrit.Sts2.Core.Saves;

/// <summary>
/// A one-time reminder of what happened to a character's Dread win streak the last time the
/// player finished a Dread run. Consumed and cleared by the Dread screen the next time it's opened,
/// so the reminder animation only plays once.
/// Tracked separately for singleplayer and multiplayer via
/// <see cref="P:MegaCrit.Sts2.Core.Saves.ProgressState.PendingSingleplayerDreadWinStreakResult" /> and
/// <see cref="P:MegaCrit.Sts2.Core.Saves.ProgressState.PendingMultiplayerDreadWinStreakResult" />.
/// </summary>
public class PendingDreadWinStreakResult
{
	[JsonPropertyName("character_id")]
	public ModelId CharacterId { get; set; } = ModelId.none;

	/// <summary>
	/// True if the win streak was reset to 0 (loss). False if it was increased (victory).
	/// </summary>
	[JsonPropertyName("was_reset")]
	public bool WasReset { get; set; }

	/// <summary>
	/// If WasReset is true, then this is set to the win streak before the reset occurred.
	/// </summary>
	[JsonPropertyName("win_streak_before_reset")]
	public int WinStreakBeforeReset { get; set; }

	/// <summary>
	/// The starting character for rotation before the winstreak was broken.
	/// Note that this is only set if the winstreak is the rotating streak (i.e. if CharacterId is RANDOM_CHARACTER).
	/// </summary>
	[JsonPropertyName("initial_rotating_character_before_reset")]
	public ModelId InitialRotatingCharacterBeforeReset { get; set; } = ModelId.none;

	/// <summary>
	/// How much the win streak increased by, will probably be 1 but there are talks about increasing the win streak per boss.
	/// This is because Dread might be too hard so the win streak may be too short for the vast majority of players.
	/// </summary>
	[JsonPropertyName("win_streak_increase")]
	public int WinStreakIncrease { get; set; }
}
