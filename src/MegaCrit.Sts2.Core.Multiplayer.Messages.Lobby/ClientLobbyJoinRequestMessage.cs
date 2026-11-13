using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Multiplayer.Serialization;
using MegaCrit.Sts2.Core.Multiplayer.Transport;
using MegaCrit.Sts2.Core.Unlocks;

namespace MegaCrit.Sts2.Core.Multiplayer.Messages.Lobby;

/// <summary>
/// Sent from a newly connected client to the host as the first message in the joining flow.
/// </summary>
public struct ClientLobbyJoinRequestMessage : INetMessage, IPacketSerializable
{
	public SerializableUnlockState unlockState;

	public int maxStandardAscension;

	public int maxBlitzAscension;

	public bool ShouldBroadcast => false;

	public NetTransferMode Mode => NetTransferMode.Reliable;

	public LogLevel LogLevel => LogLevel.Info;

	public bool ShouldBuffer => true;

	public void Serialize(PacketWriter writer)
	{
		writer.Write(unlockState);
		writer.WriteInt(maxStandardAscension, 8);
		writer.WriteInt(maxBlitzAscension, 8);
	}

	public void Deserialize(PacketReader reader)
	{
		unlockState = reader.Read<SerializableUnlockState>();
		maxStandardAscension = reader.ReadInt(8);
		maxBlitzAscension = reader.ReadInt(8);
	}
}
