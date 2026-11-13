using System.Threading;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Random;
using MegaCrit.Sts2.Core.Runs;

namespace MegaCrit.Sts2.Core.AutoSlay.Navigation;

/// <summary>How a game mode gets from one room to the next.</summary>
public interface IRoomNavigator
{
	/// <summary>Moves the run from the finished room at <paramref name="from" /> into the next room.</summary>
	Task AdvanceAsync(RunState runState, RunPosition from, Rng random, CancellationToken ct);

	/// <summary>
	/// After a boss that ends an act (but not the run), waits until the next act is ready for
	/// <see cref="M:MegaCrit.Sts2.Core.AutoSlay.Navigation.IRoomNavigator.AdvanceAsync(MegaCrit.Sts2.Core.Runs.RunState,MegaCrit.Sts2.Core.AutoSlay.Navigation.RunPosition,MegaCrit.Sts2.Core.Random.Rng,System.Threading.CancellationToken)" />.
	/// </summary>
	Task WaitForNextActAsync(RunState runState, CancellationToken ct);
}
