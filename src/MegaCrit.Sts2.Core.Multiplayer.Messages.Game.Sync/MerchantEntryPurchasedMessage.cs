using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Multiplayer.Serialization;
using MegaCrit.Sts2.Core.Multiplayer.Transport;
using MegaCrit.Sts2.Core.Runs;

namespace MegaCrit.Sts2.Core.Multiplayer.Messages.Game.Sync;

/// <summary>
/// Sent when a player attempts to purchase a merchant option.
/// Note that receiving this message doesn't immediately mean the player actually received the result; it only means
/// they began the process of purchasing the item. For example, they may still cancel out of a card removal.
/// </summary>
public class MerchantEntryPurchasedMessage : INetMessage, IPacketSerializable, IRunLocationTargetedMessage
{
	public uint id;

	public bool ShouldBroadcast => true;

	public NetTransferMode Mode => NetTransferMode.Reliable;

	public LogLevel LogLevel => LogLevel.VeryDebug;

	public bool ShouldBuffer => true;

	public RunLocation Location { get; set; }

	public void Serialize(PacketWriter writer)
	{
		writer.Write(Location);
		writer.WriteUInt(id, 8);
	}

	public void Deserialize(PacketReader reader)
	{
		Location = reader.Read<RunLocation>();
		id = reader.ReadUInt(8);
	}
}
