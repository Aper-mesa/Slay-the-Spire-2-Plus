using System;
using System.Collections.Generic;
using System.IO;
using Godot;

namespace MegaCrit.Sts2.Core.Models.Powers;

/// <summary>
/// [apermesa] Base class for STS1-only powers. Uses the STS1 power icons embedded in the assembly so
/// the powers can be shown in the power bar instead of being hidden.
/// </summary>
public abstract class ClassicPowerModel : PowerModel
{
	private static readonly Dictionary<string, Texture2D> _icons = new Dictionary<string, Texture2D>();

	private string IconKey
	{
		get
		{
			return Id.Entry switch
			{
				"CLASSIC_AMPLIFY_POWER" => "amplify",
				"CLASSIC_ELECTRO_POWER" => "mastery",
				"CLASSIC_HEATSINKS_POWER" => "heatsink",
				"CLASSIC_LOCK_ON_POWER" => "lockon",
				"CLASSIC_SELF_REPAIR_POWER" => "repair",
				"CLASSIC_STATIC_DISCHARGE_POWER" => "static_discharge",
				_ => null
			};
		}
	}

	public override Texture2D Icon => LoadIcon(48) ?? base.Icon;

	public override Texture2D BigIcon => LoadIcon(128) ?? base.BigIcon;

	private Texture2D LoadIcon(int size)
	{
		string key = IconKey;
		if (key == null)
		{
			return null;
		}
		string cacheKey = key + "_" + size;
		if (_icons.TryGetValue(cacheKey, out Texture2D cached))
		{
			return cached;
		}
		Texture2D texture = LoadEmbeddedIcon("power_" + key + "_" + size + ".png");
		if (texture != null)
		{
			_icons[cacheKey] = texture;
		}
		return texture;
	}

	private static Texture2D LoadEmbeddedIcon(string fileName)
	{
		string suffix = "." + fileName;
		foreach (string resourceName in typeof(ClassicPowerModel).Assembly.GetManifestResourceNames())
		{
			if (!resourceName.EndsWith(suffix, StringComparison.OrdinalIgnoreCase))
			{
				continue;
			}
			using Stream stream = typeof(ClassicPowerModel).Assembly.GetManifestResourceStream(resourceName);
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
}
