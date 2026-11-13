using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Multiplayer.Serialization;
using MegaCrit.Sts2.Core.Multiplayer.Transport;
using MegaCrit.Sts2.Core.Runs;

namespace MegaCrit.Sts2.Core.Multiplayer.Messages.Lobby;

/// <summary>
/// Sent by the host when the chosen ascension level changes.
/// </summary>
public struct LobbyGameModeChangedMessage : INetMessage, IPacketSerializable
{
	public GameMode gameMode;

	public int ascension;

	public int maxAscension;

	public bool ShouldBroadcast => true;

	public NetTransferMode Mode => NetTransferMode.Reliable;

	public LogLevel LogLevel => LogLevel.VeryDebug;

	public bool ShouldBuffer => true;

	public void Serialize(PacketWriter writer)
	{
		writer.WriteEnum(gameMode);
		writer.WriteInt(ascension, 8);
		writer.WriteInt(maxAscension, 8);
	}

	public void Deserialize(PacketReader reader)
	{
		gameMode = reader.ReadEnum<GameMode>();
		ascension = reader.ReadInt(8);
		maxAscension = reader.ReadInt(8);
	}
}
