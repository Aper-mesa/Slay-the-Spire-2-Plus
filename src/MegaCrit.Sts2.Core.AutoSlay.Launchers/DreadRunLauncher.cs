using System.Threading;
using System.Threading.Tasks;
using Godot;
using MegaCrit.Sts2.Core.AutoSlay.Helpers;
using MegaCrit.Sts2.Core.Nodes.Screens.Dread;
using MegaCrit.Sts2.Core.Nodes.Screens.MainMenu;
using MegaCrit.Sts2.Core.Random;

namespace MegaCrit.Sts2.Core.AutoSlay.Launchers;

/// <summary>
/// Starts a Dread run from the Dread screen. Ascension is fixed by the mode. Whether the run begins a character
/// rotation is picked at random, so both paths get played across seeds.
/// </summary>
public class DreadRunLauncher : IRunLauncher
{
	public async Task LaunchAsync(NMainMenu mainMenu, Rng random, string seed, CancellationToken ct)
	{
		Control screen = await ModeScreen.OpenModeScreenAsync(await ModeScreen.OpenSingleplayerSubmenuAsync(mainMenu, ct), "DreadButton", mainMenu, "Submenus/DreadScreen", ct);
		await ModeScreen.SelectRandomCharacterAsync(screen, random, ct);
		await SetBeginRotationAsync(screen, random.NextBool());
		await ModeScreen.EmbarkAsync(screen, seed, ct);
	}

	private static async Task SetBeginRotationAsync(Control screen, bool beginRotation)
	{
		NDreadBeginRotationTickbox tickbox = screen.GetNode<NDreadBeginRotationTickbox>("%RotatingTickbox");
		if (!tickbox.IsVisibleInTree() || !tickbox.IsEnabled)
		{
			AutoSlayLog.Info("Begin-rotation tickbox not available, leaving it alone");
			return;
		}
		if (tickbox.IsTicked != beginRotation)
		{
			await UiHelper.Click(tickbox);
		}
		AutoSlayLog.Action($"Begin rotation: {tickbox.IsTicked}");
	}
}
