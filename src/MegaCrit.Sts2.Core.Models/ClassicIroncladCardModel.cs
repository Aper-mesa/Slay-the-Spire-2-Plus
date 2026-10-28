using Godot;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Helpers;

namespace MegaCrit.Sts2.Core.Models;

public abstract class ClassicIroncladCardModel : CardModel
{
    protected ClassicIroncladCardModel(int cost, CardType type, CardRarity rarity, TargetType target)
        : base(cost, type, rarity, target) { }

    private string ArtEntry => Id.Entry.Replace("CLASSIC_IRONCLAD_", "").ToLowerInvariant() switch
    {
        "berserk" => "demon_form",
        "blood_for_blood" => "hemokinesis",
        "brutality" => "rupture",
        "carnage" => "bludgeon",
        "cleave" => "whirlwind",
        "clothesline" => "uppercut",
        "clash" => "anger",
        "dual_wield" => "twin_strike",
        "entrench" => "barricade",
        "combust" => "fiend_fire",
        "disarm" => "uppercut",
        "double_tap" => "twin_strike",
        "dropkick" => "headbutt",
        "evolve" => "dark_embrace",
        "exhume" => "burning_pact",
        "fire_breathing" => "flame_barrier",
        "flex" => "inflame",
        "ghostly_armor" => "impervious",
        "heavy_blade" => "bludgeon",
        "immolate" => "fiend_fire",
        "intimidate" => "shockwave",
        "limit_break" => "inflame",
        "metallicize" => "barricade",
        "power_through" => "shrug_it_off",
        "pummel" => "twin_strike",
        "reaper" => "feed",
        "reckless_charge" => "anger",
        "searing_blow" => "bludgeon",
        "seeing_red" => "bloodletting",
        "sentinel" => "defend_ironclad",
        "sever_soul" => "fiend_fire",
        "spot_weakness" => "bash",
        "warcry" => "battle_trance",
        "wild_strike" => "strike_ironclad",
        var entry => entry
    };
    private string ArtPool => ArtEntry == "shockwave" ? "colorless" : "ironclad";

    public override string PortraitPath
    {
        get
        {
            string path = ImageHelper.GetImagePath("atlases/card_atlas.sprites/" + ArtPool + "/" + ArtEntry + ".tres");
            return ResourceLoader.Exists(path) ? path : ImageHelper.GetImagePath("atlases/card_atlas.sprites/ironclad/" +
                (Type == CardType.Attack ? "strike_ironclad" : Type == CardType.Power ? "inflame" : "defend_ironclad") + ".tres");
        }
    }
    public override string BetaPortraitPath => PortraitPath;
    protected override string PortraitPngPath => ImageHelper.GetImagePath("packed/card_portraits/" + ArtPool + "/" + ArtEntry + ".png");
}
