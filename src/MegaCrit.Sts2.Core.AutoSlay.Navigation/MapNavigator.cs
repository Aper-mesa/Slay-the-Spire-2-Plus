using System;
using System.Threading;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.AutoSlay.Handlers.Screens;
using MegaCrit.Sts2.Core.AutoSlay.Helpers;
using MegaCrit.Sts2.Core.Random;
using MegaCrit.Sts2.Core.Runs;

namespace MegaCrit.Sts2.Core.AutoSlay.Navigation;

/// <summary>Picks each next room on the map screen.</summary>
public class MapNavigator : IRoomNavigator
{
	private readonly MapScreenHandler _mapHandler = new MapScreenHandler();

	public Task AdvanceAsync(RunState runState, RunPosition from, Rng random, CancellationToken ct)
	{
		return _mapHandler.HandleAsync(random, ct);
	}

	public async Task WaitForNextActAsync(RunState runState, CancellationToken ct)
	{
		await WaitHelper.Until(() => runState.VisitedMapCoords.Count == 0, ct, TimeSpan.FromSeconds(5L), "Act transition did not complete (VisitedMapCoords not cleared)");
	}
}
