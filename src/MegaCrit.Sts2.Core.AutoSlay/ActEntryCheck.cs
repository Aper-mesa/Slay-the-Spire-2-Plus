using System;
using System.Threading;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.AutoSlay.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Dread;
using MegaCrit.Sts2.Core.Runs;

namespace MegaCrit.Sts2.Core.AutoSlay;

/// <summary>Logs each act as the run enters it, and checks what the mode sets up per act.</summary>
public static class ActEntryCheck
{
	/// <summary>
	/// How long Dread may take to set the act's dominion. It is set from <c>AfterActEntered</c>, which runs after the
	/// act's fade, so the first room can be entered a moment before it is set.
	/// </summary>
	private static readonly TimeSpan _dominionTimeout = TimeSpan.FromSeconds(10L);

	/// <summary>Call at the first room of each act.</summary>
	/// <exception cref="T:MegaCrit.Sts2.Core.AutoSlay.DriverTimeoutException">A Dread act did not get its boss's dominion.</exception>
	public static Task RunAsync(RunState runState, AutoSlayMode mode, CancellationToken ct)
	{
		return RunAsync(runState, mode, _dominionTimeout, ct);
	}

	internal static async Task RunAsync(RunState runState, AutoSlayMode mode, TimeSpan dominionTimeout, CancellationToken ct)
	{
		ActModel act = runState.Act;
		EncounterModel encounterModel = act.SecondBossEncounter ?? act.BossEncounter;
		string bosses = ((act.SecondBossEncounter == null) ? act.BossEncounter.Id.Entry : (act.BossEncounter.Id.Entry + "+" + act.SecondBossEncounter.Id.Entry));
		if (mode == AutoSlayMode.Dread)
		{
			DominionModel expected = encounterModel.GetDominion();
			await WaitHelper.Until(() => expected != null && runState.Dominion?.Id == expected.Id, ct, dominionTimeout, () => $"Dread act {runState.CurrentActIndex + 1} ({act.Id.Entry}) entered with dominion {runState.Dominion?.Id.Entry ?? "none"}, expected {expected?.Id.Entry ?? "none (boss has no dominion)"}");
		}
		AutoSlayLog.Info($"Act {runState.CurrentActIndex + 1} entered: act={act.Id.Entry}, boss={bosses}, map={runState.Map.GetType().Name}, dominion={runState.Dominion?.Id.Entry ?? "none"}");
	}
}
