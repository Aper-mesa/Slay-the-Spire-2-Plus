using Godot;

namespace MegaCrit.Sts2.Core.Debug;

/// <summary>One screenshot attempt: what happened, the image if there is one, and why if there is not.</summary>
public sealed record CaptureResult(CaptureStatus Status, Image? Image, string Detail)
{
	public static CaptureResult Captured(Image image)
	{
		return new CaptureResult(CaptureStatus.Captured, image, $"{image.GetWidth()}x{image.GetHeight()}");
	}

	public static CaptureResult SkippedHeadless(string displayServerName)
	{
		return new CaptureResult(CaptureStatus.SkippedHeadless, null, "Display server '" + displayServerName + "' cannot draw.");
	}

	public static CaptureResult Failed(string detail)
	{
		return new CaptureResult(CaptureStatus.Failed, null, detail);
	}
}
