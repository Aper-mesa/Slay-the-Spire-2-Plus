using System.Collections.Generic;
using Godot;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models.Relics;

namespace MegaCrit.Sts2.Core.Models.RelicPools;

public sealed class ClassicIroncladRelicPool : RelicPoolModel
{
    public override string EnergyColorName => "ironclad";
    public override Color LabOutlineColor => StsColors.red;
    protected override IEnumerable<RelicModel> GenerateAllRelics() =>
    [
        ModelDb.Relic<ClassicIroncladBurningBlood>(),
        ModelDb.Relic<ClassicIroncladRedSkull>(),
        ModelDb.Relic<ClassicIroncladPaperPhrog>(),
        ModelDb.Relic<ClassicIroncladSelfFormingClay>(),
        ModelDb.Relic<ClassicIroncladCharonsAshes>(),
        ModelDb.Relic<ClassicIroncladBrimstone>(),
        ModelDb.Relic<ClassicIroncladChampionsBelt>(),
        ModelDb.Relic<ClassicIroncladMagicFlower>(),
        ModelDb.Relic<ClassicIroncladBlackBlood>(),
        ModelDb.Relic<ClassicIroncladMarkOfPain>(),
        ModelDb.Relic<ClassicIroncladRunicCube>(),
    ];
}
