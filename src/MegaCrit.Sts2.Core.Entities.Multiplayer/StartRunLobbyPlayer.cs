using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Multiplayer.Serialization;
using MegaCrit.Sts2.Core.Unlocks;

namespace MegaCrit.Sts2.Core.Entities.Multiplayer;

public struct StartRunLobbyPlayer : IPacketSerializable
{
	public ulong id;

	public int slotId;

	public CharacterModel character;

	public SerializableUnlockState unlockState;

	public int maxStandardAscension;

	public int maxBlitzAscension;

	public bool isRotatingDread;

	public bool isModded;

	public bool isReady;

	public void Serialize(PacketWriter writer)
	{
		writer.WriteULong(id);
		writer.WriteInt(slotId, 2);
		writer.WriteModel(character);
		writer.Write(unlockState);
		writer.WriteInt(maxStandardAscension, 8);
		writer.WriteInt(maxBlitzAscension, 8);
		writer.WriteBool(isRotatingDread);
		writer.WriteBool(isModded);
		writer.WriteBool(isReady);
	}

	public void Deserialize(PacketReader reader)
	{
		id = reader.ReadULong();
		slotId = reader.ReadInt(2);
		character = reader.ReadModel<CharacterModel>();
		unlockState = reader.Read<SerializableUnlockState>();
		maxStandardAscension = reader.ReadInt(8);
		maxBlitzAscension = reader.ReadInt(8);
		isRotatingDread = reader.ReadBool();
		isModded = reader.ReadBool();
		isReady = reader.ReadBool();
	}

	public override string ToString()
	{
		return $"Player {id}, {character?.Id.Entry}";
	}
}
