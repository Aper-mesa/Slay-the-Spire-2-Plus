using System;
using System.Text.Json.Serialization;
using MegaCrit.Sts2.Core.Entities.Merchant;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Multiplayer.Serialization;

namespace MegaCrit.Sts2.Core.Runs.History;

[Serializable]
public struct BlitzMerchantHistoryEntry : IPacketSerializable
{
	[JsonPropertyName("was_picked")]
	public bool wasPicked;

	[JsonPropertyName("model_id")]
	public ModelId? modelId;

	[JsonPropertyName("type")]
	public BlitzMerchantEntryType Type { get; set; }

	public BlitzMerchantHistoryEntry(BlitzMerchantEntryType type, bool wasPicked)
	{
		modelId = null;
		Type = type;
		this.wasPicked = wasPicked;
	}

	public void Serialize(PacketWriter writer)
	{
		writer.WriteEnum(Type);
		writer.WriteBool(wasPicked);
		writer.WriteBool(modelId != null);
		if (modelId != null)
		{
			writer.WriteFullModelId(modelId);
		}
	}

	public void Deserialize(PacketReader reader)
	{
		Type = reader.ReadEnum<BlitzMerchantEntryType>();
		wasPicked = reader.ReadBool();
		if (reader.ReadBool())
		{
			modelId = reader.ReadFullModelId();
		}
	}
}
