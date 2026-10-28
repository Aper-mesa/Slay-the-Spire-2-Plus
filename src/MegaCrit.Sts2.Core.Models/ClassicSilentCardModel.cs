using Godot;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Helpers;

namespace MegaCrit.Sts2.Core.Models;

// Independent STS1 cards, with STS2 portraits chosen by visual similarity when no matching card exists.
public abstract class ClassicSilentCardModel : CardModel
{
    protected ClassicSilentCardModel(int cost, CardType type, CardRarity rarity, TargetType target)
        : base(cost, type, rarity, target) { }

    private string ArtEntry => Id.Entry.Replace("CLASSIC_SILENT_", "").ToLowerInvariant() switch
    {
        "a_thousand_cuts" => "afterimage",
        "all_out_attack" => "dagger_spray",
        "bane" => "poisoned_stab",
        "catalyst" => "accelerant",
        "choke" => "strangle",
        "concentrate" => "prepared",
        "distraction" => "hand_trick",
        "outmaneuver" => "anticipate",
        "corpse_explosion" => "outbreak",
        "crippling_poison" => "noxious_fumes",
        "die_die_die" => "dagger_spray",
        "doppelganger" => "shadow_step",
        "endless_agony" => "echoing_slash",
        "eviscerate" => "finisher",
        "flying_knee" => "pounce",
        "glass_knife" => "hidden_daggers",
        "heel_hook" => "sucker_punch",
        "masterful_stab" => "backstab",
        "phantasmal_killer" => "nightmare",
        "quick_slash" => "slice",
        "riddle_with_holes" => "flechettes",
        "setup" => "prepared",
        "sneaky_strike" => "sneaky",
        "terror" => "piercing_wail",
        "unload" => "dagger_throw",
        var entry => entry
    };

    private string ArtPool => ArtEntry == "alchemize" ? "colorless" : ArtEntry == "caltrops" ? "event" : "silent";

    public override string PortraitPath
    {
        get
        {
            string path = ImageHelper.GetImagePath("atlases/card_atlas.sprites/" + ArtPool + "/" + ArtEntry + ".tres");
            if (ResourceLoader.Exists(path)) return path;
            return ImageHelper.GetImagePath("atlases/card_atlas.sprites/silent/" +
                (Type == CardType.Attack ? "strike_silent" : Type == CardType.Power ? "footwork" : "defend_silent") + ".tres");
        }
    }
    public override string BetaPortraitPath => PortraitPath;
    protected override string PortraitPngPath => ImageHelper.GetImagePath("packed/card_portraits/" + ArtPool + "/" + ArtEntry + ".png");
}
