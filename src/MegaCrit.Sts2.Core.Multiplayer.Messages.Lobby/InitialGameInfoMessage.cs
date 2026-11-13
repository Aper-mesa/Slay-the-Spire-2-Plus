using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Multiplayer.Serialization;
using MegaCrit.Sts2.Core.Multiplayer.Transport;

namespace MegaCrit.Sts2.Core.Multiplayer.Messages.Lobby;

/// <summary>
/// Sent from the host to the client as the first message after the client connects.
/// </summary>
public struct InitialGameInfoMessage : INetMessage, IPacketSerializable
{
	/// <summary>
	/// What state the run is currently in.
	/// </summary>
	public RunSessionState sessionState;

	/// <summary>
	/// If the host is about to disconnect the client, why.
	/// </summary>
	public ConnectionFailureReason? connectionFailureReason;

	public bool ShouldBroadcast => false;

	public NetTransferMode Mode => NetTransferMode.Reliable;

	public LogLevel LogLevel => LogLevel.Info;

	public bool ShouldBuffer => true;

	public void Serialize(PacketWriter writer)
	{
		writer.WriteEnum(sessionState);
		writer.WriteBool(connectionFailureReason.HasValue);
		if (connectionFailureReason.HasValue)
		{
			writer.WriteEnum(connectionFailureReason.Value);
		}
	}

	public void Deserialize(PacketReader reader)
	{
		sessionState = reader.ReadEnum<RunSessionState>();
		if (reader.ReadBool())
		{
			connectionFailureReason = reader.ReadEnum<ConnectionFailureReason>();
		}
	}
}
