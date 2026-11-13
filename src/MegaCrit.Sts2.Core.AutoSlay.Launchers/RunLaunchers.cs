using System;

namespace MegaCrit.Sts2.Core.AutoSlay.Launchers;

public static class RunLaunchers
{
	public static IRunLauncher For(AutoSlayMode mode)
	{
		return mode switch
		{
			AutoSlayMode.Standard => (IRunLauncher)new StandardRunLauncher(),
			AutoSlayMode.Blitz => new BlitzRunLauncher(),
			AutoSlayMode.Dread => new DreadRunLauncher(),
			_ => throw new ArgumentOutOfRangeException("mode", mode, null),
		};
	}
}
