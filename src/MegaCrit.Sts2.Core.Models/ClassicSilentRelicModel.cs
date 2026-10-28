using Godot;
using MegaCrit.Sts2.Core.Helpers;

namespace MegaCrit.Sts2.Core.Models;

public abstract class ClassicSilentRelicModel : RelicModel
{
    protected override string IconBaseName => Id.Entry.Replace("CLASSIC_SILENT_", "").ToLowerInvariant() switch
    {
        "ring_of_the_serpent" => "ring_of_the_snake",
        "wrist_blade" => "helical_dart",
        "hovering_kite" => "ninja_scroll",
        "the_specimen" => "snecko_skull",
        var name => name
    };
    public override string PackedIconPath => ResourceLoader.Exists(base.PackedIconPath)
        ? base.PackedIconPath : ImageHelper.GetImagePath("atlases/relic_atlas.sprites/ring_of_the_snake.tres");
    protected override string PackedIconOutlinePath => ResourceLoader.Exists(base.PackedIconOutlinePath)
        ? base.PackedIconOutlinePath : ImageHelper.GetImagePath("atlases/relic_outline_atlas.sprites/ring_of_the_snake.tres");
}
