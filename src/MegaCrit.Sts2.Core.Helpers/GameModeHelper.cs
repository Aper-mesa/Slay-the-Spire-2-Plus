using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Saves;

namespace MegaCrit.Sts2.Core.Helpers;

public static class GameModeHelper
{
	public static bool AreAchievementsAndEpochsLockedForRun(IRunState run)
	{
		if (!(run is RunState runState))
		{
			return run.GameMode == GameMode.Daily;
		}
		return AreAchievementsAndEpochsLocked(runState.GameMode, runState.IsCustomGame);
	}

	public static bool AreAchievementsAndEpochsLockedForHistory(RunHistory history)
	{
		return AreAchievementsAndEpochsLocked(history.GameMode, history.IsCustomGame);
	}

	public static bool AreAchievementsAndEpochsLockedForSerializableRun(SerializableRun serializableRun)
	{
		return AreAchievementsAndEpochsLocked(serializableRun.GameMode, serializableRun.IsCustomGame);
	}

	public static bool AreAchievementsAndEpochsLocked(GameMode gameMode, bool isCustomGame)
	{
		return (gameMode == GameMode.Daily) | isCustomGame;
	}
}
