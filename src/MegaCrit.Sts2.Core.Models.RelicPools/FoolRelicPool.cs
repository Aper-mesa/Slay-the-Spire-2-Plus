using System.Collections.Generic;
using System.Linq;
using Godot;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Timeline.Epochs;
using MegaCrit.Sts2.Core.Unlocks;

namespace MegaCrit.Sts2.Core.Models.RelicPools;

public sealed class FoolRelicPool : RelicPoolModel
{
	public override string EnergyColorName => "fool";

	public override Color LabOutlineColor => StsColors.red;

	protected override IEnumerable<RelicModel> GenerateAllRelics()
	{
		return new _003C_003Ez__ReadOnlyArray<RelicModel>(new RelicModel[8]
		{
			ModelDb.Relic<BizarreLamp>(),
			ModelDb.Relic<ConductorsBaton>(),
			ModelDb.Relic<ExerciseMat>(),
			ModelDb.Relic<Fertilizer>(),
			ModelDb.Relic<Ocarina>(),
			ModelDb.Relic<SongBow>(),
			ModelDb.Relic<Twine>(),
			ModelDb.Relic<WhittlingKnife>()
		});
	}

	public override IEnumerable<RelicModel> GetUnlockedRelics(UnlockState unlockState)
	{
		List<RelicModel> list = AllRelics.ToList();
		if (!unlockState.IsEpochRevealed<Fool3Epoch>())
		{
			list.RemoveAll((RelicModel r) => Fool3Epoch.Relics.Any((RelicModel relic) => relic.Id == r.Id));
		}
		if (!unlockState.IsEpochRevealed<Fool6Epoch>())
		{
			list.RemoveAll((RelicModel r) => Fool6Epoch.Relics.Any((RelicModel relic) => relic.Id == r.Id));
		}
		return list;
	}
}
