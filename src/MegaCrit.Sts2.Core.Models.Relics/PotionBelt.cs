using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Runs;

namespace MegaCrit.Sts2.Core.Models.Relics;

public sealed class PotionBelt : RelicModel
{
	private const string _potionSlotsKey = "PotionSlots";

	public override RelicRarity Rarity => RelicRarity.Common;

	public override bool HasUponPickupEffect => true;

	protected override IEnumerable<DynamicVar> CanonicalVars => new _003C_003Ez__ReadOnlySingleElementList<DynamicVar>(new DynamicVar("PotionSlots", 2m));

	protected override bool IsAllowedInternal(IRunState runState)
	{
		if (RelicModel.IsLateInAct3(runState))
		{
			return runState.GameMode != GameMode.Blitz;
		}
		return true;
	}

	public override async Task AfterObtained()
	{
		await PlayerCmd.GainMaxPotionCount(DynamicVars["PotionSlots"].IntValue, Owner);
	}
}
