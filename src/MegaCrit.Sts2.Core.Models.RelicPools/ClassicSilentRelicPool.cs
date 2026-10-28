using System.Collections.Generic;
using Godot;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models.Relics;

namespace MegaCrit.Sts2.Core.Models.RelicPools;

public sealed class ClassicSilentRelicPool : RelicPoolModel
{
    public override string EnergyColorName => "silent";
    public override Color LabOutlineColor => StsColors.green;
    protected override IEnumerable<RelicModel> GenerateAllRelics() =>
    [
        ModelDb.Relic<ClassicSilentRingOfTheSnake>(),
        ModelDb.Relic<ClassicSilentSneckoSkull>(),
        ModelDb.Relic<ClassicSilentNinjaScroll>(),
        ModelDb.Relic<ClassicSilentPaperKrane>(),
        ModelDb.Relic<ClassicSilentTingsha>(),
        ModelDb.Relic<ClassicSilentToughBandages>(),
        ModelDb.Relic<ClassicSilentTwistedFunnel>(),
        ModelDb.Relic<ClassicSilentTheSpecimen>(),
        ModelDb.Relic<ClassicSilentRingOfTheSerpent>(),
        ModelDb.Relic<ClassicSilentWristBlade>(),
        ModelDb.Relic<ClassicSilentHoveringKite>(),
    ];
}
