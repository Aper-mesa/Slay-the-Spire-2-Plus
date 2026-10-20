using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.ValueProps;

namespace MegaCrit.Sts2.Core.Models.Relics;

public sealed class GalacticDust : RelicModel
{
	private bool _isActivating;

	private int _starsSpent;

	public override bool ShowCounter => true;

	public override int DisplayAmount
	{
		get
		{
			if (!IsActivating)
			{
				return StarsSpent % DynamicVars.Stars.IntValue;
			}
			return DynamicVars.Stars.IntValue;
		}
	}

	private bool IsActivating
	{
		get
		{
			return _isActivating;
		}
		set
		{
			AssertMutable();
			_isActivating = value;
			UpdateDisplay();
		}
	}

	[SavedProperty]
	public int StarsSpent
	{
		get
		{
			return _starsSpent;
		}
		set
		{
			AssertMutable();
			_starsSpent = value;
			UpdateDisplay();
		}
	}

	protected override IEnumerable<IHoverTip> ExtraHoverTips => new _003C_003Ez__ReadOnlySingleElementList<IHoverTip>(HoverTipFactory.Static(StaticHoverTip.Block));

	public override RelicRarity Rarity => RelicRarity.Uncommon;

	protected override IEnumerable<DynamicVar> CanonicalVars => new _003C_003Ez__ReadOnlyArray<DynamicVar>(new DynamicVar[2]
	{
		new StarsVar(10),
		new BlockVar(10m, ValueProp.Unpowered)
	});

	private void UpdateDisplay()
	{
		if (IsActivating)
		{
			Status = RelicStatus.Normal;
		}
		else
		{
			int intValue = DynamicVars.Stars.IntValue;
			Status = ((StarsSpent == intValue - 1) ? RelicStatus.Active : RelicStatus.Normal);
		}
		InvokeDisplayAmountChanged();
	}

	public override async Task AfterStarsSpent(int amount, Player spender)
	{
		if (spender == Owner)
		{
			StarsSpent += amount;
			if (StarsSpent >= DynamicVars.Stars.IntValue)
			{
				TaskHelper.RunSafely(DoActivateVisuals());
				await CreatureCmd.GainBlock(Owner.Creature, Mathf.FloorToInt((float)StarsSpent / (float)DynamicVars.Stars.IntValue) * DynamicVars.Block.IntValue, ValueProp.Unpowered, null);
				StarsSpent %= DynamicVars.Stars.IntValue;
			}
			InvokeDisplayAmountChanged();
		}
	}

	private async Task DoActivateVisuals()
	{
		IsActivating = true;
		Flash();
		await Cmd.Wait(1f);
		IsActivating = false;
	}
}
