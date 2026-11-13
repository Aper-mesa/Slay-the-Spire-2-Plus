using System.Collections.Generic;
using Godot;
using MegaCrit.Sts2.Core.Models.Potions;
namespace MegaCrit.Sts2.Core.Models.PotionPools;
public sealed class ClassicWatcherPotionPool : PotionPoolModel
{
    public override string EnergyColorName => "necrobinder";
    public override Color LabOutlineColor => new("AA77DD");
    protected override IEnumerable<PotionModel> GenerateAllPotions() => [ModelDb.Potion<ClassicWatcherBottledMiracle>(),ModelDb.Potion<ClassicWatcherStancePotion>(),ModelDb.Potion<ClassicWatcherAmbrosia>()];
}
