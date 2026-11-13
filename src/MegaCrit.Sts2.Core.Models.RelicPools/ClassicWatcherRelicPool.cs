using System.Collections.Generic;
using Godot;
using MegaCrit.Sts2.Core.Models.Relics;
namespace MegaCrit.Sts2.Core.Models.RelicPools;
public sealed class ClassicWatcherRelicPool : RelicPoolModel
{
    public override string EnergyColorName => "necrobinder";
    public override Color LabOutlineColor => new("AA77DD");
    protected override IEnumerable<RelicModel> GenerateAllRelics() => [
        ModelDb.Relic<ClassicWatcherPureWater>(),
        ModelDb.Relic<ClassicWatcherHolyWater>(),
        ModelDb.Relic<ClassicWatcherDamaru>(),
        ModelDb.Relic<ClassicWatcherTeardropLocket>(),
        ModelDb.Relic<ClassicWatcherDuality>(),
        ModelDb.Relic<ClassicWatcherGoldenEye>(),
        ModelDb.Relic<ClassicWatcherVioletLotus>(),
        ModelDb.Relic<ClassicWatcherMelange>(),
        ModelDb.Relic<ClassicWatcherCloakClasp>(),
    ];
}
