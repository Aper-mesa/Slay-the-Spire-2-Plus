using System;
using System.Threading;
using System.Threading.Tasks;
using Godot;
using MegaCrit.Sts2.Core.AutoSlay.Helpers;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Screens.MainMenu;
using MegaCrit.Sts2.Core.Random;

namespace MegaCrit.Sts2.Core.AutoSlay.Launchers;

/// <summary>
/// Starts a standard run the way a player does: the Singleplayer button, then Standard if the submenu shows.
/// </summary>
public class StandardRunLauncher : IRunLauncher
{
	private const string _characterSelectPath = "Submenus/CharacterSelectScreen";

	public async Task LaunchAsync(NMainMenu mainMenu, Rng random, string seed, CancellationToken ct)
	{
		NButton node = mainMenu.GetNode<NButton>("MainMenuTextButtons/SingleplayerButton");
		AutoSlayLog.Action("Clicking singleplayer");
		await UiHelper.Click(node);
		Control charSelectScreen = null;
		NButton standardButton = null;
		await WaitHelper.Until(() =>
		{
			charSelectScreen = mainMenu.GetNodeOrNull<Control>("Submenus/CharacterSelectScreen");
			standardButton = FindStandardRunButton(mainMenu);
			Control control2 = charSelectScreen;
			return (control2 != null && control2.Visible) || (standardButton?.Visible ?? false);
		}, ct, AutoSlayConfig.nodeWaitTimeout, "Neither CharacterSelectScreen nor SingleplayerSubmenu became visible");
		if (standardButton?.Visible ?? false)
		{
			Control control = charSelectScreen;
			if (control == null || !control.Visible)
			{
				AutoSlayLog.Action("Clicking standard run");
				await UiHelper.Click(standardButton);
				await WaitHelper.Until(() => mainMenu.GetNodeOrNull<Control>("Submenus/CharacterSelectScreen")?.Visible ?? false, ct, AutoSlayConfig.nodeWaitTimeout, "CharacterSelectScreen did not become visible");
				goto IL_0299;
			}
		}
		AutoSlayLog.Action("Skipping submenu (first run)");
		goto IL_0299;
		IL_0299:
		Control screen = mainMenu.GetNode<Control>("Submenus/CharacterSelectScreen");
		await ModeScreen.SelectRandomCharacterAsync(screen, random, ct);
		await ModeScreen.EmbarkAsync(screen, seed, ct);
	}

	/// <summary>
	/// Finds the Standard button of the singleplayer submenu, or null if the submenu is not in the tree yet.
	/// The button is found by its unique name, so the lookup does not depend on the layout of the submenu scene.
	/// </summary>
	/// <exception cref="T:System.InvalidOperationException">The submenu is visible but has no Standard button.</exception>
	private static NButton? FindStandardRunButton(Node mainMenu)
	{
		Control nodeOrNull = mainMenu.GetNodeOrNull<Control>("Submenus/SingleplayerSubmenu");
		if (nodeOrNull == null)
		{
			return null;
		}
		NButton nodeOrNull2 = nodeOrNull.GetNodeOrNull<NButton>("%StandardButton");
		if (nodeOrNull2 == null && nodeOrNull.Visible)
		{
			throw new InvalidOperationException("SingleplayerSubmenu is visible but has no node with the unique name StandardButton");
		}
		return nodeOrNull2;
	}
}
