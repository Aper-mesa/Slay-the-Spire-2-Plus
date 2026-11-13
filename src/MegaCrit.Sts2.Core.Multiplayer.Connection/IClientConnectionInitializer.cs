using System.Threading;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.Multiplayer.Game;
using MegaCrit.Sts2.Core.Platform;

namespace MegaCrit.Sts2.Core.Multiplayer.Connection;

public interface IClientConnectionInitializer
{
	PlatformType Platform { get; }

	ulong? GetHostId();

	Task<NetErrorInfo?> Connect(INetClientGameService netService, CancellationToken cancelToken = default(CancellationToken));
}
