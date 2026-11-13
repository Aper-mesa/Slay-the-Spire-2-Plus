using System;
using System.IO;
using System.Threading.Tasks;
using Godot;
using MegaCrit.Sts2.Core.AutoSlay.Scenarios;
using MegaCrit.Sts2.Core.Debug;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Debug;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;

namespace MegaCrit.Sts2.Core.DevConsole.ConsoleCommands;

/// <summary>
/// Writes the current viewport to a PNG. A real command rather than a scenario directive, because
/// the value of it is having it by hand during triage.
/// </summary>
public class ScreenshotConsoleCmd : AbstractConsoleCmd
{
	public override string CmdName => "screenshot";

	public override string Args => "[label:string]";

	public override string Description => "Writes the current viewport to a PNG under user://screenshots/.";

	public override bool IsNetworked => false;

	public override CmdResult Process(Player? issuingPlayer, string[] args)
	{
		if (args.Length > 1)
		{
			return new CmdResult(success: false, "Too many arguments, expected '[label]' but got '" + string.Join(' ', args) + "'.");
		}
		if (NGame.Instance == null)
		{
			return new CmdResult(success: false, "There is no viewport to capture yet.");
		}
		string label = ((args.Length == 1) ? args[0] : "screenshot");
		string path = ScreenshotPaths.BuildScreenshotPath(label, DateTime.Now);
		return new CmdResult(TakeAsync(path), success: true, "Writing a screenshot to " + ProjectSettings.GlobalizePath(path));
	}

	private static async Task TakeAsync(string path)
	{
		NDevConsole console = NDevConsole.InstanceOrNull;
		bool wasVisible = console?.Visible ?? false;
		if (wasVisible)
		{
			console.HideConsole();
			await console.AwaitProcessFrame();
		}
		CaptureResult captureResult;
		try
		{
			captureResult = await ViewportCapture.GrabAsync();
		}
		finally
		{
			if (wasVisible)
			{
				console.ShowConsole();
			}
		}
		switch (captureResult.Status)
		{
		case CaptureStatus.Captured:
			ViewportCapture.Write(path, captureResult.Image);
			ScenarioLog.Info("Screenshot written to " + ProjectSettings.GlobalizePath(path));
			break;
		case CaptureStatus.SkippedHeadless:
			ScenarioLog.Warn("No screenshot was taken. " + captureResult.Detail);
			break;
		default:
			throw new IOException("Screenshot failed. " + captureResult.Detail);
		}
	}
}
