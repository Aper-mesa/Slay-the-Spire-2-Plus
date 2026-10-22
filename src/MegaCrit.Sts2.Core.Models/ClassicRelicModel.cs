using Godot;
using MegaCrit.Sts2.Core.Helpers;

namespace MegaCrit.Sts2.Core.Models;

/// <summary>
/// [apermesa] Base class for the classic Defect relic pool. Reuses the Defect relic icon when a matching
/// sprite exists and falls back to Cracked Core's icon otherwise.
/// </summary>
public abstract class ClassicRelicModel : RelicModel
{
	protected override string IconBaseName => Id.Entry.ToLowerInvariant().Replace("classic_", "");

	public override string PackedIconPath
	{
		get
		{
			string path = base.PackedIconPath;
			return ResourceLoader.Exists(path) ? path : ImageHelper.GetImagePath("atlases/relic_atlas.sprites/cracked_core.tres");
		}
	}

	protected override string PackedIconOutlinePath
	{
		get
		{
			string path = base.PackedIconOutlinePath;
			return ResourceLoader.Exists(path) ? path : ImageHelper.GetImagePath("atlases/relic_outline_atlas.sprites/cracked_core.tres");
		}
	}
}
