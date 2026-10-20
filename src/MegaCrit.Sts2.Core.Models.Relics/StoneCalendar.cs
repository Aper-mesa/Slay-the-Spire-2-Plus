using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.ValueProps;

namespace MegaCrit.Sts2.Core.Models.Relics;

public sealed class StoneCalendar : RelicModel
{
	private const string _damageTurnKey = "DamageTurn";

	private bool _isActivating;

	public override RelicRarity Rarity => RelicRarity.Rare;

	public override bool ShowCounter => DisplayAmount > -1;

	public override int DisplayAmount
	{
		get
		{
			if (!CombatManager.Instance.IsInProgress)
			{
				return -1;
			}
			if (IsCanonical)
			{
				return -1;
			}
			int intValue = DynamicVars["DamageTurn"].IntValue;
			if (IsActivating)
			{
				return intValue;
			}
			int turnNumber = Owner.PlayerCombatState.TurnNumber;
			if (turnNumber >= intValue)
			{
				return -1;
			}
			return turnNumber;
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
			InvokeDisplayAmountChanged();
		}
	}

	protected override IEnumerable<DynamicVar> CanonicalVars => new _003C_003Ez__ReadOnlyArray<DynamicVar>(new DynamicVar[2]
	{
		new DamageVar(52m, ValueProp.Unpowered),
		new DynamicVar("DamageTurn", 7m)
	});

	public override Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
	{
		if (!participants.Contains(Owner.Creature))
		{
			return Task.CompletedTask;
		}
		if (Owner.PlayerCombatState.TurnNumber == DynamicVars["DamageTurn"].IntValue)
		{
			Status = RelicStatus.Active;
		}
		InvokeDisplayAmountChanged();
		return Task.CompletedTask;
	}

	public override async Task BeforeSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
	{
		if (participants.Contains(Owner.Creature))
		{
			int intValue = DynamicVars["DamageTurn"].IntValue;
			int turnNumber = Owner.PlayerCombatState.TurnNumber;
			Status = RelicStatus.Normal;
			if (turnNumber == intValue)
			{
				TaskHelper.RunSafely(DoActivateVisuals());
				await CreatureCmd.Damage(choiceContext, Owner.Creature.CombatState.HittableEnemies, DynamicVars.Damage, Owner.Creature);
				InvokeDisplayAmountChanged();
			}
		}
	}

	public override Task AfterCombatEnd(CombatRoom _)
	{
		Status = RelicStatus.Normal;
		InvokeDisplayAmountChanged();
		return Task.CompletedTask;
	}

	public override Task AfterRoomEntered(AbstractRoom room)
	{
		if (!(room is CombatRoom))
		{
			return Task.CompletedTask;
		}
		Status = RelicStatus.Normal;
		InvokeDisplayAmountChanged();
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
