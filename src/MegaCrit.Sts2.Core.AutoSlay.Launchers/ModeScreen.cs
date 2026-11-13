using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Godot;
using MegaCrit.Sts2.Core.AutoSlay.Helpers;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Screens.CharacterSelect;
using MegaCrit.Sts2.Core.Nodes.Screens.MainMenu;
using MegaCrit.Sts2.Core.Random;

namespace MegaCrit.Sts2.Core.AutoSlay.Launchers;

/// <summary>
/// Steps shared by the character select, Blitz and Dread screens, which lay out their character buttons and
/// embark button the same way.
/// </summary>
internal static class ModeScreen
{
	private const string _characterButtonsPath = "CharSelectButtons/ButtonContainer";

	private const string _embarkButtonPath = "ConfirmButton";

	/// <summary>
	/// Opens the singleplayer submenu without the Singleplayer button. On a profile with no finished run that button
	/// skips the submenu and goes straight to character select, and CI profiles are always empty.
	/// </summary>
	public static async Task<NSingleplayerSubmenu> OpenSingleplayerSubmenuAsync(NMainMenu mainMenu, CancellationToken ct)
	{
		AutoSlayLog.Action("Opening singleplayer submenu");
		NSingleplayerSubmenu submenu = mainMenu.OpenSingleplayerSubmenu();
		await WaitHelper.Until(() => submenu.IsVisibleInTree(), ct, AutoSlayConfig.nodeWaitTimeout, "SingleplayerSubmenu did not become visible");
		return submenu;
	}

	/// <summary>
	/// Clicks a submenu button that opens a mode screen, and waits for that screen to show.
	/// </summary>
	/// <exception cref="T:System.InvalidOperationException">
	/// The button is disabled. AutoSlay clicks by emitting the release signal directly, which would open a mode the
	/// profile has not unlocked, so a disabled button means <see cref="T:MegaCrit.Sts2.Core.AutoSlay.AutoSlayUnlocks" /> is missing its epoch.
	/// </exception>
	public static async Task<Control> OpenModeScreenAsync(NSingleplayerSubmenu submenu, string buttonUniqueName, NMainMenu mainMenu, string screenPath, CancellationToken ct)
	{
		NButton node = submenu.GetNode<NButton>("%" + buttonUniqueName);
		if (!node.IsEnabled || !node.Visible)
		{
			throw new InvalidOperationException(buttonUniqueName + " is disabled or hidden; AutoSlayUnlocks does not reveal the epoch that gates it");
		}
		AutoSlayLog.Action("Clicking " + buttonUniqueName);
		await UiHelper.Click(node);
		await WaitHelper.Until(() => mainMenu.GetNodeOrNull<Control>(screenPath)?.IsVisibleInTree() ?? false, ct, AutoSlayConfig.nodeWaitTimeout, screenPath + " did not become visible");
		return mainMenu.GetNode<Control>(screenPath);
	}

	/// <summary>Selects a random unlocked character on <paramref name="screen" />.</summary>
	public static async Task SelectRandomCharacterAsync(Control screen, Rng random, CancellationToken ct)
	{
		Node node = screen.GetNode("CharSelectButtons/ButtonContainer");
		List<NCharacterSelectButton> list = UiHelper.FindAll<NCharacterSelectButton>(node);
		foreach (NCharacterSelectButton item in list)
		{
			item.UnlockIfPossible();
		}
		List<NCharacterSelectButton> items = list.Where((NCharacterSelectButton b) => !b.IsLocked && b.Visible).ToList();
		NCharacterSelectButton nCharacterSelectButton = random.NextItem(items);
		AutoSlayLog.Action($"Selecting character: {nCharacterSelectButton.Character.Id}");
		nCharacterSelectButton.Select();
		await Task.Delay(100, ct);
	}

	/// <summary>Sets the run seed again and clicks the embark button on <paramref name="screen" />.</summary>
	public static async Task EmbarkAsync(Control screen, string seed, CancellationToken ct)
	{
		NButton button = await WaitHelper.ForNode<NButton>(screen, "ConfirmButton", ct);
		NGame.Instance.DebugSeedOverride = seed;
		AutoSlayLog.Action("Embarking");
		await UiHelper.Click(button);
	}
}
