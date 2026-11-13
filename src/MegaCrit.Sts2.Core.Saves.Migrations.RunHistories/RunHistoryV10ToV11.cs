using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Runs;

namespace MegaCrit.Sts2.Core.Saves.Migrations.RunHistories;

/// <summary>
/// Adds RunHistory.IsCustomGame to out of date run saves
/// for runs that used to have GameMode = Custom
/// </summary>
[Migration(typeof(RunHistory), 10, 11)]
public class RunHistoryV10ToV11 : MigrationBase<RunHistory>
{
	protected override void ApplyMigration(MigratingData saveData)
	{
		Log.Info("RunHistory migration v10 -> v11: histories that are custom games now use RunHistory.IsCustomGame");
		string text = saveData.GetAs<string>("game_mode");
		if (text == "custom")
		{
			saveData.Remove("game_mode");
			saveData.Set("game_mode", GameMode.Standard);
			saveData.Set("is_custom_game", value: true);
		}
	}
}
