using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Multiplayer.Serialization;

namespace MegaCrit.Sts2.Core.Transfer;

public struct NetTransferRelicAction : INetAction, IPacketSerializable
{
	public ulong giverNetId;

	public ulong targetPlayerId;

	public bool returnMode;

	public GameAction ToGameAction(Player player)
	{
		Player giver = player.RunState.GetPlayer(giverNetId) ?? player;
		return new TransferRelicAction(giver, targetPlayerId, returnMode);
	}

	public void Serialize(PacketWriter writer)
	{
		writer.WriteULong(giverNetId);
		writer.WriteULong(targetPlayerId);
		writer.WriteBool(returnMode);
	}

	public void Deserialize(PacketReader reader)
	{
		giverNetId = reader.ReadULong();
		targetPlayerId = reader.ReadULong();
		returnMode = reader.ReadBool();
	}

	public override string ToString()
	{
		return $"NetTransferRelicAction giver: {giverNetId} target: {targetPlayerId} return: {returnMode}";
	}
}
