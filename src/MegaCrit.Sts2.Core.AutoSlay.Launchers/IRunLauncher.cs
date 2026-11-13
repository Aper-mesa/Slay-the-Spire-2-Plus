using System.Threading;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Nodes.Screens.MainMenu;
using MegaCrit.Sts2.Core.Random;

namespace MegaCrit.Sts2.Core.AutoSlay.Launchers;

/// <summary>
/// Drives the main menu into a new run of one game mode: opens the mode's screen, picks a character and embarks.
/// </summary>
public interface IRunLauncher
{
	/// <param name="mainMenu">The visible main menu, with no run left to abandon.</param>
	/// <param name="random">AutoSlay's decision RNG.</param>
	/// <param name="seed">The run seed. Every mode screen clears the debug seed override when it opens, so the
	/// launcher has to set it again just before embarking.</param>
	/// <param name="ct">Cancels the launch.</param>
	Task LaunchAsync(NMainMenu mainMenu, Rng random, string seed, CancellationToken ct);
}
