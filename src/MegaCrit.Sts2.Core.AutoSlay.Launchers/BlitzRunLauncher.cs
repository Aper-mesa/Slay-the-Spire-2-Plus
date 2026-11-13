using System.Threading;
using System.Threading.Tasks;
using Godot;
using MegaCrit.Sts2.Core.Nodes.Screens.MainMenu;
using MegaCrit.Sts2.Core.Random;

namespace MegaCrit.Sts2.Core.AutoSlay.Launchers;

/// <summary>
/// Starts a Blitz run from the Blitz screen. The ascension panel is left at the profile's default.
/// </summary>
public class BlitzRunLauncher : IRunLauncher
{
	public async Task LaunchAsync(NMainMenu mainMenu, Rng random, string seed, CancellationToken ct)
	{
		Control screen = await ModeScreen.OpenModeScreenAsync(await ModeScreen.OpenSingleplayerSubmenuAsync(mainMenu, ct), "BlitzButton", mainMenu, "Submenus/BlitzScreen", ct);
		await ModeScreen.SelectRandomCharacterAsync(screen, random, ct);
		await ModeScreen.EmbarkAsync(screen, seed, ct);
	}
}
