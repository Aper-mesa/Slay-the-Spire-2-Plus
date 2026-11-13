using MegaCrit.Sts2.Core.Saves;

namespace MegaCrit.Sts2.Core.Entities.Save;

public record struct PostRunProgressUpdateResult
{
	/// <summary>
	/// Whether the progress was successfully updated. Possible reasons this could be false:
	///  - ShouldSave was set to false on the run (for bootstrap)
	///  - There was a bug and we tried to saved the progress twice for the same run
	/// </summary>
	public bool success;

	/// <summary>
	/// The run that just ended.
	/// This should always be non-null if <see cref="F:MegaCrit.Sts2.Core.Entities.Save.PostRunProgressUpdateResult.success" /> is true, and is non-null in most cases even when success
	/// is false.
	/// </summary>
	public SerializableRun? run;

	/// <summary>
	/// True if the run was singleplayer.
	/// </summary>
	public bool isSingleplayer;

	/// <summary>
	/// Old maximum ascension for the LOCAL player.
	/// </summary>
	public int oldMaxAscension;

	/// <summary>
	/// New maximum ascension for the LOCAL player.
	/// In multiplayer, this might be different across different players.
	/// </summary>
	public int newMaxAscension;

	public static PostRunProgressUpdateResult Unsuccessful(SerializableRun run)
	{
		return new PostRunProgressUpdateResult
		{
			success = false,
			run = run
		};
	}
}
