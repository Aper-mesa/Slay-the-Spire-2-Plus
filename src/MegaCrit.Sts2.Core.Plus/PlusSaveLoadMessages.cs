using System;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Multiplayer.Serialization;
using MegaCrit.Sts2.Core.Multiplayer.Transport;

namespace MegaCrit.Sts2.Core.Plus;

public enum PlusSaveLoadCancelReason : byte
{
	Rejected,
	Timeout,
	InvalidState,
	Disconnected,
	Failed
}

/// <summary>[apermesa] Client asks the host to reload the latest multiplayer checkpoint.</summary>
public struct PlusSlInitiateMessage : INetMessage, IPacketSerializable
{
	public uint ClientRequestId;

	public bool ShouldBroadcast => false;

	public NetTransferMode Mode => NetTransferMode.Reliable;

	public LogLevel LogLevel => LogLevel.Debug;

	public bool ShouldBuffer => false;

	public void Serialize(PacketWriter writer)
	{
		writer.WriteUInt(ClientRequestId);
	}

	public void Deserialize(PacketReader reader)
	{
		ClientRequestId = reader.ReadUInt();
	}
}

/// <summary>[apermesa] Host answers a client SL request.</summary>
public struct PlusSlInitiateResponseMessage : INetMessage, IPacketSerializable
{
	public uint ClientRequestId;

	public bool Approved;

	public byte Reason;

	public bool ShouldBroadcast => false;

	public NetTransferMode Mode => NetTransferMode.Reliable;

	public LogLevel LogLevel => LogLevel.Debug;

	public bool ShouldBuffer => false;

	public void Serialize(PacketWriter writer)
	{
		writer.WriteUInt(ClientRequestId);
		writer.WriteBool(Approved);
		writer.WriteByte(Reason);
	}

	public void Deserialize(PacketReader reader)
	{
		ClientRequestId = reader.ReadUInt();
		Approved = reader.ReadBool();
		Reason = reader.ReadByte();
	}
}

/// <summary>[apermesa] Host distributes the canonical checkpoint and starts the reload.</summary>
public struct PlusSlExecuteMessage : INetMessage, IPacketSerializable
{
	public const int MaxPayloadBytes = 1048576;

	public const int MaxPlayers = 64;

	public uint RequestId;

	public string RunSaveJson;

	public ulong[] ConnectedPlayerIds;

	public bool ShouldBroadcast => false;

	public NetTransferMode Mode => NetTransferMode.Reliable;

	public LogLevel LogLevel => LogLevel.Debug;

	public bool ShouldBuffer => false;

	public void Serialize(PacketWriter writer)
	{
		writer.WriteUInt(RequestId);
		writer.WriteString(RunSaveJson ?? string.Empty);
		ulong[] ids = ConnectedPlayerIds ?? Array.Empty<ulong>();
		writer.WriteInt(ids.Length);
		foreach (ulong id in ids)
		{
			writer.WriteULong(id);
		}
	}

	public void Deserialize(PacketReader reader)
	{
		RequestId = reader.ReadUInt();
		RunSaveJson = reader.ReadString();
		int count = reader.ReadInt();
		if (count < 0 || count > MaxPlayers)
		{
			count = 0;
		}
		ConnectedPlayerIds = new ulong[count];
		for (int i = 0; i < count; i++)
		{
			ConnectedPlayerIds[i] = reader.ReadULong();
		}
	}
}

/// <summary>[apermesa] Client reports that its new run systems are set up and it is ready to load.</summary>
public struct PlusSlSetupReadyMessage : INetMessage, IPacketSerializable
{
	public uint RequestId;

	public bool ShouldBroadcast => false;

	public NetTransferMode Mode => NetTransferMode.Reliable;

	public LogLevel LogLevel => LogLevel.Debug;

	public bool ShouldBuffer => false;

	public void Serialize(PacketWriter writer)
	{
		writer.WriteUInt(RequestId);
	}

	public void Deserialize(PacketReader reader)
	{
		RequestId = reader.ReadUInt();
	}
}

/// <summary>[apermesa] Host tells every client to start LoadRun.</summary>
public struct PlusSlRunBeginMessage : INetMessage, IPacketSerializable
{
	public uint RequestId;

	public bool ShouldBroadcast => false;

	public NetTransferMode Mode => NetTransferMode.Reliable;

	public LogLevel LogLevel => LogLevel.Debug;

	public bool ShouldBuffer => false;

	public void Serialize(PacketWriter writer)
	{
		writer.WriteUInt(RequestId);
	}

	public void Deserialize(PacketReader reader)
	{
		RequestId = reader.ReadUInt();
	}
}

/// <summary>[apermesa] Host cancels an in-flight SL request.</summary>
public struct PlusSlCancelMessage : INetMessage, IPacketSerializable
{
	public uint RequestId;

	public byte Reason;

	public bool ShouldBroadcast => false;

	public NetTransferMode Mode => NetTransferMode.Reliable;

	public LogLevel LogLevel => LogLevel.Debug;

	public bool ShouldBuffer => false;

	public void Serialize(PacketWriter writer)
	{
		writer.WriteUInt(RequestId);
		writer.WriteByte(Reason);
	}

	public void Deserialize(PacketReader reader)
	{
		RequestId = reader.ReadUInt();
		Reason = reader.ReadByte();
	}
}
