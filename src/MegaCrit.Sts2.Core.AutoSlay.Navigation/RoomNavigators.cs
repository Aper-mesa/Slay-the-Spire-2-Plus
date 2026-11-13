using System;

namespace MegaCrit.Sts2.Core.AutoSlay.Navigation;

public static class RoomNavigators
{
	public static IRoomNavigator For(AutoSlayMode mode)
	{
		switch (mode)
		{
		case AutoSlayMode.Standard:
		case AutoSlayMode.Dread:
			return new MapNavigator();
		case AutoSlayMode.Blitz:
			return new BlitzNavigator();
		default:
			throw new ArgumentOutOfRangeException("mode", mode, null);
		}
	}
}
