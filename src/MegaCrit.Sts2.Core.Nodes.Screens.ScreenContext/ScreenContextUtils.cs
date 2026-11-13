using Godot;

namespace MegaCrit.Sts2.Core.Nodes.Screens.ScreenContext;

public static class ScreenContextUtils
{
	/// <summary>
	/// A screen also stays enabled while the current screen is one of its descendants, such as the merchant
	/// inventory inside the merchant room. The descendant inherits this screen's focus behavior, so disabling
	/// this screen would disable the current screen too.
	/// </summary>
	public static void UpdateControllerNavEnabled<T>(this T screenContext) where T : Control, IScreenContext
	{
		IScreenContext currentScreen = ActiveScreenContext.Instance.GetCurrentScreen();
		bool flag = currentScreen is Node node && screenContext.IsAncestorOf(node);
		if ((currentScreen == screenContext) | flag)
		{
			screenContext.SetFocusBehaviorRecursive(Control.FocusBehaviorRecursiveEnum.Inherited);
		}
		else
		{
			screenContext.SetFocusBehaviorRecursive(Control.FocusBehaviorRecursiveEnum.Disabled);
		}
	}
}
