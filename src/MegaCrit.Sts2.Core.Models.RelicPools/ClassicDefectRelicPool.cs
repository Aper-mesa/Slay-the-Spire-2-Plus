using System.Collections.Generic;
using System.Linq;
using Godot;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;

namespace MegaCrit.Sts2.Core.Models.RelicPools;

/// <summary>
/// [apermesa] Relic pool for the classic (Slay the Spire 1) Defect.
/// </summary>
public sealed class ClassicDefectRelicPool : RelicPoolModel
{
	public override string EnergyColorName => "defect";

	public override Color LabOutlineColor => StsColors.blue;

	protected override IEnumerable<RelicModel> GenerateAllRelics()
	{
		return new _003C_003Ez__ReadOnlyArray<RelicModel>(new RelicModel[9]
		{
			ModelDb.Relic<ClassicCrackedCore>(),
			ModelDb.Relic<ClassicDataDisk>(),
			ModelDb.Relic<ClassicEmotionChip>(),
			ModelDb.Relic<ClassicGoldPlatedCables>(),
			ModelDb.Relic<ClassicRunicCapacitor>(),
			ModelDb.Relic<ClassicSymbioticVirus>(),
			ModelDb.Relic<ClassicFrozenCore>(),
			ModelDb.Relic<ClassicInserter>(),
			ModelDb.Relic<ClassicNuclearBattery>()
		});
	}
}
