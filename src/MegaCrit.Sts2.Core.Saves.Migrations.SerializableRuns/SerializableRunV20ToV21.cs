using System.Text.Json.Nodes;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Runs;

namespace MegaCrit.Sts2.Core.Saves.Migrations.SerializableRuns;

/// <summary>
/// Adds SerializableRun.IsCustomGame to out of date run saves
/// </summary>
[Migration(typeof(SerializableRun), 20, 21)]
public class SerializableRunV20ToV21 : MigrationBase<SerializableRun>
{
	protected override void ApplyMigration(MigratingData saveData)
	{
		Log.Info("SerializableRun migration v20 -> v21: Adds SerializableRun.IsCustomGame");
		if (saveData.GetRawNode("game_mode") is JsonValue jsonValue && jsonValue.TryGetValue<string>(out string value) && value == "custom")
		{
			saveData.Remove("game_mode");
			saveData.Set("game_mode", GameMode.Standard);
			saveData.Set("is_custom_game", value: true);
		}
	}
}
