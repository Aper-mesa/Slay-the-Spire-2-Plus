using MegaCrit.Sts2.Core.Multiplayer.Connection;
using MegaCrit.Sts2.Core.Platform;

namespace MegaCrit.Sts2.Core.Entities.Multiplayer;

public class JoinFailureExtraInfo
{
	public PlatformType platform;

	public ulong? hostId;

	public JoinFailureExtraInfo(IClientConnectionInitializer initializer)
	{
		platform = initializer.Platform;
		hostId = initializer.GetHostId();
	}
}
