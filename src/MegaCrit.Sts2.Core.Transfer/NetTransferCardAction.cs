using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Multiplayer.Serialization;

namespace MegaCrit.Sts2.Core.Transfer;

public struct NetTransferCardAction : INetAction, IPacketSerializable
{
	public ulong giverNetId;

	public ulong targetPlayerId;

	public GameAction ToGameAction(Player player)
	{
		Player giver = player.RunState.GetPlayer(giverNetId) ?? player;
		return new TransferCardAction(giver, targetPlayerId);
	}

	public void Serialize(PacketWriter writer)
	{
		writer.WriteULong(giverNetId);
		writer.WriteULong(targetPlayerId);
	}

	public void Deserialize(PacketReader reader)
	{
		giverNetId = reader.ReadULong();
		targetPlayerId = reader.ReadULong();
	}

	public override string ToString()
	{
		return $"NetTransferCardAction giver: {giverNetId} target: {targetPlayerId}";
	}
}
