namespace MegaCrit.Sts2.Core.Debug;

/// <summary>What happened to one screenshot attempt.</summary>
public enum CaptureStatus
{
	/// <summary>An image came back and was written.</summary>
	Captured,
	/// <summary>No image, and the display server says nothing can draw. Expected, not a problem.</summary>
	SkippedHeadless,
	/// <summary>No image on a build that should have been able to draw one. A real problem.</summary>
	Failed
}
