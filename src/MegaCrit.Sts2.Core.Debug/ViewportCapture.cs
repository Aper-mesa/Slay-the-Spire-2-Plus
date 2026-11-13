using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Godot;
using MegaCrit.Sts2.Core.AutoSlay;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;

namespace MegaCrit.Sts2.Core.Debug;

/// <summary>
/// Wraps the viewport screenshot primitive: attempt the capture, then classify what came back.
/// </summary>
/// <remarks>
/// The display mode is consulted only to explain a capture that already failed, never to skip
/// attempting one.
/// </remarks>
public static class ViewportCapture
{
	/// <summary>The one display server name known to draw nothing at all.</summary>
	private const string _headlessDisplayServer = "headless";

	/// <summary>
	/// Display servers that actually draw. Anything else, known non-drawing or unrecognised,
	/// takes the process frame fallback: awaiting FramePostDraw on a server that never draws hangs
	/// forever, while the fallback only costs a little timing precision.
	/// </summary>
	private static readonly HashSet<string> _renderingDisplayServers = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "windows", "macos", "x11", "wayland", "android", "ios", "web" };

	/// <summary>
	/// How long to wait for the draw of the frame being captured. A drawing build produces one every
	/// frame, so this only ever expires when something is wrong.
	/// </summary>
	private static readonly TimeSpan _framePostDrawTimeout = TimeSpan.FromSeconds(10L);

	/// <summary>
	/// Latched: RendererDummy returns an empty render-target RID, so every headless attempt prints
	/// `ERROR: Parameter "t" is null`. Latching emits that once rather than per screenshot.
	/// </summary>
	private static string? _serverProvenUnableToDraw;

	/// <summary>
	/// Whether the viewport should be read after RenderingServer's FramePostDraw rather than after a
	/// second process frame.
	/// </summary>
	public static bool ShouldAwaitFramePostDraw(string? displayServerName)
	{
		return _renderingDisplayServers.Contains(displayServerName ?? string.Empty);
	}

	/// <summary>
	/// Decides what a capture attempt means. Pure, because choosing wrong here either hides a real
	/// rendering failure behind "you are headless" or reports a headless CI job as broken.
	/// </summary>
	/// <remarks>
	/// Asymmetric with <see cref="M:MegaCrit.Sts2.Core.Debug.ViewportCapture.ShouldAwaitFramePostDraw(System.String)" />: guessing "drawing" there
	/// blocks the run forever, while guessing "not drawing" here would swallow a real failure.
	/// </remarks>
	public static CaptureResult Classify(Image? image, string? displayServerName)
	{
		if (image != null)
		{
			return CaptureResult.Captured(image);
		}
		string text = displayServerName ?? string.Empty;
		if (text.Equals("headless", StringComparison.OrdinalIgnoreCase))
		{
			return CaptureResult.SkippedHeadless(text);
		}
		return CaptureResult.Failed("The viewport produced no image, and the display server reports '" + text + "', which is not the headless server. Treated as a failure rather than a skip so a real rendering problem is not filed away as an expected one.");
	}

	/// <summary>
	/// Waits for the frame the caller asked about to be drawn, then reads it.
	/// </summary>
	/// <remarks>
	/// `texture_2d_get` reads back the last completed draw, so a capture without this barrier
	/// returns the frame before the caller asked for one. Only the FramePostDraw half is bounded:
	/// the process frame awaits use a `ToSignal` that takes no token, so a tree that stops producing
	/// frames blocks here until the caller's own timeout fires.
	/// </remarks>
	public static async Task<CaptureResult> GrabAsync(CancellationToken ct = default(CancellationToken))
	{
		await AwaitDrawnFrameAsync(ct);
		return Grab();
	}

	private static async Task AwaitDrawnFrameAsync(CancellationToken ct)
	{
		NGame game = NGame.Instance;
		await game.AwaitProcessFrame(ct);
		if (!ShouldAwaitFramePostDraw(DisplayServer.GetName()))
		{
			await game.AwaitProcessFrame(ct);
			return;
		}
		try
		{
			await AwaitFramePostDrawAsync(game).WaitAsync(_framePostDrawTimeout, ct);
		}
		catch (TimeoutException)
		{
			throw new DriverTimeoutException($"No frame was drawn within {_framePostDrawTimeout.TotalSeconds}s while preparing a capture. The display server reports '{DisplayServer.GetName()}', which is treated as drawing.");
		}
	}

	private static async Task AwaitFramePostDrawAsync(NGame game)
	{
		await game.ToSignal(RenderingServer.Singleton, RenderingServer.SignalName.FramePostDraw);
	}

	/// <summary>
	/// Reads the viewport as it currently stands. The only impure part.
	/// </summary>
	/// <remarks>
	/// Synchronous: another frame boundary here would read a different frame than the
	/// barrier waited for.
	/// </remarks>
	public static CaptureResult Grab()
	{
		try
		{
			if (_serverProvenUnableToDraw == DisplayServer.GetName())
			{
				return CaptureResult.SkippedHeadless(_serverProvenUnableToDraw)with
				{
					Detail = "Display server '" + _serverProvenUnableToDraw + "' cannot draw. Not re-attempted: the first attempt this run already established it."
				};
			}
			Viewport viewport = NGame.Instance?.GetViewport();
			if (viewport == null)
			{
				return CaptureResult.Failed("There is no viewport to capture: the game node is gone.");
			}
			CaptureResult captureResult = Classify(viewport.GetTexture()?.GetImage(), DisplayServer.GetName());
			if (captureResult.Status == CaptureStatus.SkippedHeadless)
			{
				_serverProvenUnableToDraw = DisplayServer.GetName();
			}
			return captureResult;
		}
		catch (Exception ex)
		{
			return CaptureResult.Failed("The viewport capture threw " + ex.GetType().Name + ": " + ex.Message);
		}
	}

	/// <remarks>
	/// System.IO, not FileAccess: FileAccess discards fflush's and fclose's returns, so a truncated
	/// write reports success. Image.SavePng goes through it too, hence SavePngToBuffer.
	/// </remarks>
	public static void Write(string filePath, Image image)
	{
		byte[] array = image.SavePngToBuffer();
		if (array.Length == 0)
		{
			throw new IOException("PNG encoding produced no bytes for " + filePath + ".");
		}
		string path = ProjectSettings.GlobalizePath(filePath);
		string directoryName = Path.GetDirectoryName(path);
		if (!string.IsNullOrEmpty(directoryName))
		{
			Directory.CreateDirectory(directoryName);
		}
		File.WriteAllBytes(path, array);
	}
}
