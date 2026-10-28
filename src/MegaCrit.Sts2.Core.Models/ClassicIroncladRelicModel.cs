using Godot;
using MegaCrit.Sts2.Core.Helpers;

namespace MegaCrit.Sts2.Core.Models;

public abstract class ClassicIroncladRelicModel : RelicModel
{
    protected override string IconBaseName => Id.Entry.Replace("CLASSIC_IRONCLAD_", "").ToLowerInvariant() switch
    {
        "black_blood" => "burning_blood",
        "champions_belt" => "ruined_helmet",
        "magic_flower" => "blood_soaked_rose",
        "runic_cube" => "red_skull",
        "mark_of_pain" => "brimstone",
        var name => name
    };
    public override string PackedIconPath => ResourceLoader.Exists(base.PackedIconPath)
        ? base.PackedIconPath : ImageHelper.GetImagePath("atlases/relic_atlas.sprites/burning_blood.tres");
    protected override string PackedIconOutlinePath => ResourceLoader.Exists(base.PackedIconOutlinePath)
        ? base.PackedIconOutlinePath : ImageHelper.GetImagePath("atlases/relic_outline_atlas.sprites/burning_blood.tres");
}
