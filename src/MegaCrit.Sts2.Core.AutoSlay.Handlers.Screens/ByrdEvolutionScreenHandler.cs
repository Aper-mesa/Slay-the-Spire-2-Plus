using System;
using System.Threading;
using System.Threading.Tasks;
using Godot;
using MegaCrit.Sts2.Core.AutoSlay.Helpers;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Screens.Overlays;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.Random;

namespace MegaCrit.Sts2.Core.AutoSlay.Handlers.Screens;

/// <summary>
/// Handles the Byrdpip evolution animation, which the Evolve rest site option shows as an overlay.
/// The overlay closes itself, so this handler waits for it instead of clicking anything.
/// </summary>
public class ByrdEvolutionScreenHandler : IScreenHandler, IHandler
{
	public Type ScreenType => typeof(NByrdEvolutionVfx);

	public TimeSpan Timeout => TimeSpan.FromSeconds(30L);

	public async Task HandleAsync(Rng random, CancellationToken ct)
	{
		AutoSlayLog.EnterScreen("NByrdEvolutionVfx");
		NByrdEvolutionVfx screen = AutoSlayer.GetCurrentScreen<NByrdEvolutionVfx>();
		AutoSlayLog.Action("Waiting for the evolution animation to finish");
		await WaitHelper.Until(() => NOverlayStack.Instance?.Peek() != screen, ct, TimeSpan.FromSeconds(20L), "Evolution animation did not finish");
		NProceedButton proceedButton = NRestSiteRoom.Instance?.ProceedButton;
		if (proceedButton != null)
		{
			await WaitHelper.Until(() => !GodotObject.IsInstanceValid(proceedButton) || proceedButton.IsEnabled, ct, TimeSpan.FromSeconds(5L), "Rest site proceed button did not become enabled after the evolution animation");
		}
	}
}
