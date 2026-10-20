using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace MegaCrit.Sts2.Core.Models.Relics;

public sealed class PollinousCore : RelicModel
{
	private const string _turnsKey = "Turns";

	private bool _isActivating;

	private int _turnsSeen;

	public override RelicRarity Rarity => RelicRarity.Event;

	public override bool ShowCounter => true;

	public override int DisplayAmount
	{
		get
		{
			if (!IsActivating)
			{
				return TurnsSeen;
			}
			return DynamicVars["Turns"].IntValue;
		}
	}

	protected override IEnumerable<DynamicVar> CanonicalVars => new _003C_003Ez__ReadOnlyArray<DynamicVar>(new DynamicVar[2]
	{
		new CardsVar(2),
		new DynamicVar("Turns", 4m)
	});

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
	public int TurnsSeen
	{
		get
		{
			return _turnsSeen;
		}
		set
		{
			AssertMutable();
			_turnsSeen = value;
			UpdateDisplay();
		}
	}

	private void UpdateDisplay()
	{
		if (IsActivating)
		{
			Status = RelicStatus.Normal;
		}
		else
		{
			int intValue = DynamicVars["Turns"].IntValue;
			Status = ((TurnsSeen == intValue - 1) ? RelicStatus.Active : RelicStatus.Normal);
		}
		InvokeDisplayAmountChanged();
	}

	public override Task BeforeHandDraw(Player player, PlayerChoiceContext choiceContext, ICombatState combatState)
	{
		if (player != Owner)
		{
			return Task.CompletedTask;
		}
		TurnsSeen++;
		return Task.CompletedTask;
	}

	public override Task AfterCombatEnd(CombatRoom _)
	{
		Status = RelicStatus.Normal;
		return Task.CompletedTask;
	}

	public override decimal ModifyHandDraw(Player player, decimal count)
	{
		if (player != Owner)
		{
			return count;
		}
		if (TurnsSeen < DynamicVars["Turns"].IntValue)
		{
			return count;
		}
		return count + DynamicVars.Cards.BaseValue;
	}

	public override Task AfterModifyingHandDraw()
	{
		TurnsSeen = 0;
		TaskHelper.RunSafely(DoActivateVisuals());
		return Task.CompletedTask;
	}

	private async Task DoActivateVisuals()
	{
		IsActivating = true;
		Flash();
		await Cmd.Wait(1f);
		IsActivating = false;
	}
}
