using System;

namespace MegaCrit.Sts2.Core.AutoSlay;

/// <summary>
/// Exception thrown when an automated driver (AutoSlay or a scenario run) times out waiting for a
/// condition or for progress. Filtered from Sentry since these are expected during automated testing.
/// </summary>
public class DriverTimeoutException : TimeoutException
{
	public DriverTimeoutException(string message)
		: base(message)
	{
	}
}
