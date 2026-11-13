using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Rooms;

namespace MegaCrit.Sts2.Core.Models.Dread;

/// <summary>
/// This is the model that takes care of:
///  - The global effects Dread has on the entire run
///  - Applying the individual dominions for each individual boss in a Dread run
/// </summary>
public sealed class DreadRunModel : RunModel
{
	public override async Task AfterActEntered()
	{
		if (RunState.CurrentActIndex > 0)
		{
			Log.Info($"Giving ascender's bane after act {RunState.CurrentActIndex} entered");
			foreach (Player player in RunState.Players)
			{
				await CardPileCmd.AddCursesToDeck(new _003C_003Ez__ReadOnlySingleElementList<CardModel>(ModelDb.Card<AscendersBane>()), player);
			}
		}
		Log.Info($"Applying dominion because act {RunState.CurrentActIndex} entered");
		ApplyCurrentDominion();
	}

	public override void AfterRunLoaded()
	{
		Log.Info("Applying dominion because run loaded");
		ApplyCurrentDominion();
	}

	public override Task BeforeRoomEntered(AbstractRoom room)
	{
		if (!RemoveDominionIfRoomIsBoss(room) && RunState.VisitedMapCoords.Count == 0 && RunState.Dominion == null)
		{
			Log.Info("Applying dominion because we entered the first room without a dominion (assuming this is bootstrapper)");
			ApplyCurrentDominion();
			if (room is CombatRoom combatRoom)
			{
				combatRoom.CombatState.SetDominion(RunState.Dominion);
			}
		}
		return Task.CompletedTask;
	}

	private bool RemoveDominionIfRoomIsBoss(AbstractRoom room)
	{
		if (!(room is CombatRoom combatRoom))
		{
			return false;
		}
		if (combatRoom.Encounter.RoomType != RoomType.Boss)
		{
			return false;
		}
		if (combatRoom.Encounter.Id == RunState.Act.BossEncounter.Id || combatRoom.Encounter.Id == RunState.Act.SecondBossEncounter?.Id)
		{
			Log.Info("Removing dominion because we are entering the boss encounter");
			RunState.SetDominion(null);
			combatRoom.CombatState.SetDominion(null);
			return true;
		}
		return false;
	}

	private void ApplyCurrentDominion()
	{
		ActModel actModel = RunState.Acts[RunState.CurrentActIndex];
		EncounterModel encounterModel = actModel.SecondBossEncounter ?? actModel.BossEncounter;
		DominionModel dominion = encounterModel.GetDominion();
		if (dominion == null)
		{
			Log.Error($"Entered act {actModel} (index {RunState.CurrentActIndex}) with boss {encounterModel} which doesn't have any dominion set!");
		}
		Log.Info($"Setting dominion to {dominion}");
		RunState.SetDominion(dominion);
	}
}
