using System;
using System.Collections.Generic;
using System.IO;
using Godot;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Helpers;

namespace MegaCrit.Sts2.Core.Models;

/// <summary>
/// [apermesa] Base class for the classic Defect card pool. Reuses the Defect card art when a matching
/// sprite exists and falls back to the STS1 art embedded in the assembly, then to the missing-card portrait.
/// </summary>
public abstract class ClassicCardModel : CardModel
{
	private static readonly Dictionary<string, Texture2D> _embeddedPortraits = new Dictionary<string, Texture2D>();

	protected ClassicCardModel(int cost, CardType type, CardRarity rarity, TargetType target)
		: base(cost, type, rarity, target)
	{
	}

	private string ArtEntry
	{
		get
		{
			return Id.Entry switch
			{
				"CLASSIC_GASH" => "claw",
				"CLASSIC_STEAM_POWER" => "overclock",
				"CLASSIC_STRIKE" => "strike_defect",
				"CLASSIC_DEFEND" => "defend_defect",
				_ => Id.Entry.ToLowerInvariant().Replace("classic_", "")
			};
		}
	}

	public override string PortraitPath
	{
		get
		{
			string path = ImageHelper.GetImagePath("atlases/card_atlas.sprites/defect/" + ArtEntry + ".tres");
			return ResourceLoader.Exists(path) ? path : CardModel.MissingPortraitPath;
		}
	}

	public override Texture2D Portrait
	{
		get
		{
			string key = Id.Entry.ToLowerInvariant().Replace("classic_", "");
			if (!_embeddedPortraits.TryGetValue(key, out Texture2D cached))
			{
				cached = LoadEmbeddedPortrait(key);
				if (cached != null)
				{
					_embeddedPortraits[key] = cached;
				}
			}
			return cached ?? base.Portrait;
		}
	}

	private static Texture2D LoadEmbeddedPortrait(string key)
	{
		string suffix = "." + key + ".png";
		foreach (string resourceName in typeof(ClassicCardModel).Assembly.GetManifestResourceNames())
		{
			if (!resourceName.EndsWith(suffix, StringComparison.OrdinalIgnoreCase))
			{
				continue;
			}
			using Stream stream = typeof(ClassicCardModel).Assembly.GetManifestResourceStream(resourceName);
			if (stream == null)
			{
				return null;
			}
			using MemoryStream memoryStream = new MemoryStream();
			stream.CopyTo(memoryStream);
			Image image = new Image();
			if (image.LoadPngFromBuffer(memoryStream.ToArray()) != Error.Ok)
			{
				return null;
			}
			return ImageTexture.CreateFromImage(image);
		}
		return null;
	}

	public override string BetaPortraitPath => CardModel.MissingPortraitPath;

	protected override string PortraitPngPath => ImageHelper.GetImagePath("packed/card_portraits/defect/" + ArtEntry + ".png");
}
