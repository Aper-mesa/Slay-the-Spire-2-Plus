using System;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.Logging;

namespace MegaCrit.Sts2.Core.Platform.Steam;

public static class SteamDisconnectionReasonExtensions
{
	/// <summary>
	/// The Steam close code for a version mismatch, pinned to the value shipped builds understand.
	/// A close code is only ever read by a peer we cannot otherwise talk to, which is exactly the case where our
	/// own enum values mean nothing to them, so this one number has to stay fixed forever.
	/// </summary>
	private const int _legacyVersionMismatchCode = 1010;

	/// <summary>
	/// Steam close code a pre-v0.111.0 peer sends when its lobby-join handshake expires. Across a version gap that
	/// only ever happens because the two sides cannot read each other, so we report it as a version mismatch.
	/// </summary>
	private const int _legacyHandshakeTimeoutCode = 1013;

	public static SteamDisconnectionReason ToSteam(this NetError reason)
	{
		if (reason == NetError.VersionMismatch)
		{
			return (SteamDisconnectionReason)1010;
		}
		return (SteamDisconnectionReason)(1000 + reason);
	}

	public static NetError ToApp(this SteamDisconnectionReason steamReason)
	{
		if (steamReason >= SteamDisconnectionReason.AppGeneric && steamReason <= (SteamDisconnectionReason)1999)
		{
			if (steamReason == (SteamDisconnectionReason)1010 || steamReason == (SteamDisconnectionReason)1013)
			{
				return NetError.VersionMismatch;
			}
			NetError netError = (NetError)(steamReason - 1000);
			if (!Enum.IsDefined(netError))
			{
				Log.Error($"Received unknown application error from Steam: {steamReason}");
				return NetError.UnknownNetworkError;
			}
			return netError;
		}
		if (steamReason >= SteamDisconnectionReason.LocalMin)
		{
			if (steamReason > SteamDisconnectionReason.LocalMax)
			{
				switch (steamReason)
				{
				case SteamDisconnectionReason.RelayConnectivity:
				case SteamDisconnectionReason.SteamConnectivity:
					break;
				case SteamDisconnectionReason.RemoteTimeout:
				case SteamDisconnectionReason.MiscTimeout:
					return NetError.Timeout;
				case SteamDisconnectionReason.RendezvousTimeout:
				case SteamDisconnectionReason.RelayReceivedUnexpectedNoConnectionPacket:
					return NetError.TryAgainLater;
				case SteamDisconnectionReason.BadCrypt:
				case SteamDisconnectionReason.BadCert:
					return NetError.SecureConnectionFailed;
				default:
					goto IL_00d5;
				}
			}
			return NetError.NoInternet;
		}
		if (steamReason == SteamDisconnectionReason.None)
		{
			return NetError.None;
		}
		goto IL_00d5;
		IL_00d5:
		return NetError.UnknownNetworkError;
	}
}
