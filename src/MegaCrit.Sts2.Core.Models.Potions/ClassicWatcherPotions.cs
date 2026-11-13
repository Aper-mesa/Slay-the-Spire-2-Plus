using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.ClassicContent;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;

namespace MegaCrit.Sts2.Core.Models.Potions;

public abstract class ClassicWatcherPotionModel : PotionModel
{
    private string Kind => GetType().Name.Substring(14);
    private string Art => Kind == "BottledMiracle" ? "energy_potion" : Kind == "Ambrosia" ? "strength_potion" : "flex_potion";
    protected override string PackedImagePath => ImageHelper.GetImagePath("atlases/potion_atlas.sprites/"+Art+".tres");
    protected override string PackedOutlinePath => ImageHelper.GetImagePath("atlases/potion_outline_atlas.sprites/"+Art+".tres");
    public override string LargeImagePath => ImageHelper.GetImagePath("potions/large/"+Art+".png");
    public override PotionUsage Usage => PotionUsage.CombatOnly;
    public override TargetType TargetType => TargetType.Self;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(2)];
    protected override async Task OnUse(PlayerChoiceContext c, Creature target)
    {
        if (Kind == "BottledMiracle")
            for(int i=0;i<DynamicVars.Cards.IntValue;i++) await ClassicWatcherMechanics.Generate<ClassicWatcherMiracle>(Owner);
        else if (Kind == "Ambrosia") await ClassicWatcherMechanics.ChangeStance(c,Owner,ClassicWatcherStance.Divinity);
        else
        {
            var choices = new List<CardModel> { Owner.Creature.CombatState.CreateCard<ClassicWatcherCrescendo>(Owner), Owner.Creature.CombatState.CreateCard<ClassicWatcherTranquility>(Owner) };
            var chosen = await CardSelectCmd.FromChooseACardScreen(c,choices,Owner,canSkip:false);
            if(chosen != null) await ClassicWatcherMechanics.ChangeStance(c,Owner,chosen is ClassicWatcherCrescendo ? ClassicWatcherStance.Wrath : ClassicWatcherStance.Calm);
        }
    }
}
public sealed class ClassicWatcherBottledMiracle : ClassicWatcherPotionModel { public override PotionRarity Rarity => PotionRarity.Common; }
public sealed class ClassicWatcherStancePotion : ClassicWatcherPotionModel { public override PotionRarity Rarity => PotionRarity.Uncommon; }
public sealed class ClassicWatcherAmbrosia : ClassicWatcherPotionModel { public override PotionRarity Rarity => PotionRarity.Rare; }
