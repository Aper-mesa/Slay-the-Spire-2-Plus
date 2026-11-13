using Godot;
using MegaCrit.Sts2.addons.mega_text;

namespace MegaCrit.Sts2.Core.Localization;

/// <summary>
/// Helpers for mirroring UI when the current language is read right-to-left (see <see cref="P:MegaCrit.Sts2.Core.Localization.LocManager.IsRtl" />).
/// They do nothing in left-to-right languages, so they're safe to call unconditionally.
///
/// Godot can mirror UI on its own, but it decides when to based on Godot translation files, which we don't use. So
/// instead, each piece of UI that supports right-to-left languages opts in with one of these.
/// </summary>
public static class RtlControlUtils
{
	private static bool IsRtl
	{
		get
		{
			if (LocManager.Instance != null)
			{
				return LocManager.Instance.IsRtl;
			}
			return false;
		}
	}

	/// <summary>
	/// Mirrors everything under the control: anchored children flip horizontally, containers lay their children out
	/// right-to-left, labels swap left and right alignment, and MegaLabels and MegaRichTextLabels read right-to-left.
	/// Art isn't flipped, and MarginContainers keep their margins where they are (see <see cref="M:MegaCrit.Sts2.Core.Localization.RtlControlUtils.ApplyRtlMargins(Godot.MarginContainer)" />).
	///
	/// The control's own anchored position gets mirrored within its parent too, so call this on controls that a
	/// container places or that fill their parent.
	/// </summary>
	public static void ApplyRtlLayout(this Control control)
	{
		if (IsRtl)
		{
			control.LayoutDirection = Control.LayoutDirectionEnum.Rtl;
		}
	}

	/// <summary>
	/// Swaps a MarginContainer's left and right margins, which a mirrored layout doesn't do on its own. Use this on
	/// containers whose margins should mirror with everything else (like an indent), but not on ones whose margins line
	/// up with art that isn't flipped.
	/// Call it after anything else that sets the margins.
	/// </summary>
	public static void ApplyRtlMargins(this MarginContainer container)
	{
		if (IsRtl)
		{
			int themeConstant = container.GetThemeConstant(ThemeConstants.MarginContainer.MarginLeft);
			int themeConstant2 = container.GetThemeConstant(ThemeConstants.MarginContainer.MarginRight);
			container.AddThemeConstantOverride(ThemeConstants.MarginContainer.MarginLeft, themeConstant2);
			container.AddThemeConstantOverride(ThemeConstants.MarginContainer.MarginRight, themeConstant);
		}
	}

	/// <summary>
	/// Makes a label read right-to-left without mirroring anything around it. Left-aligned text moves to the right,
	/// and centered and right-aligned text stays put.
	/// For labels that aren't in a mirrored layout (see <see cref="M:MegaCrit.Sts2.Core.Localization.RtlControlUtils.ApplyRtlLayout(Godot.Control)" />).
	/// </summary>
	public static void ApplyRtlText(this Label label)
	{
		if (IsRtl)
		{
			if (label.HorizontalAlignment == HorizontalAlignment.Left)
			{
				label.HorizontalAlignment = HorizontalAlignment.Right;
			}
			label.TextDirection = Control.TextDirection.Rtl;
		}
	}

	/// <inheritdoc cref="M:MegaCrit.Sts2.Core.Localization.RtlControlUtils.ApplyRtlText(Godot.Label)" />
	public static void ApplyRtlText(this RichTextLabel label)
	{
		if (IsRtl)
		{
			if (label.HorizontalAlignment == HorizontalAlignment.Right)
			{
				label.HorizontalAlignment = HorizontalAlignment.Left;
			}
			label.TextDirection = Control.TextDirection.Rtl;
		}
	}

	/// <summary>
	/// Calls <see cref="M:MegaCrit.Sts2.Core.Localization.RtlControlUtils.ApplyRtlText(Godot.Label)" /> on every label under the node.
	/// </summary>
	public static void ApplyRtlTextToLabels(this Node node)
	{
		if (!IsRtl)
		{
			return;
		}
		foreach (Node child in node.GetChildren())
		{
			if (child is Label label)
			{
				label.ApplyRtlText();
			}
			else if (child is RichTextLabel label2)
			{
				label2.ApplyRtlText();
			}
			child.ApplyRtlTextToLabels();
		}
	}
}
