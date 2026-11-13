using System;
using System.Collections.Generic;
using Godot;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Timeline.Epochs;
using MegaCrit.Sts2.Core.Unlocks;

namespace MegaCrit.Sts2.Core.Models.PotionPools;

public sealed class FoolPotionPool : PotionPoolModel
{
	public override string EnergyColorName => "fool";

	public override Color LabOutlineColor => StsColors.red;

	protected override IEnumerable<PotionModel> GenerateAllPotions()
	{
		return Fool4Epoch.Potions;
	}

	/// <summary>
	/// Only return the Potions if the associated Epoch is revealed.
	/// </summary>
	public override IEnumerable<PotionModel> GetUnlockedPotions(UnlockState unlockState)
	{
		if (!unlockState.IsEpochRevealed<Fool4Epoch>())
		{
			return Array.Empty<PotionModel>();
		}
		return GenerateAllPotions();
	}
}
