using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace MegaCrit.Sts2.Core.Models.Powers;

public sealed class InsatiableDominionPower : PowerModel
{
	private const int _baseDamage = 2;

	public override PowerType Type => PowerType.Debuff;

	public override PowerStackType StackType => PowerStackType.Counter;

	public override int DisplayAmount => 2 + (Owner.Player?.PlayerCombatState?.TurnNumber).GetValueOrDefault();

	protected override IEnumerable<DynamicVar> CanonicalVars => new _003C_003Ez__ReadOnlySingleElementList<DynamicVar>(new DamageVar(2m, ValueProp.Unpowered));

	public override Task AfterApplied(Creature? applier, CardModel? cardSource)
	{
		RefreshDamage();
		return Task.CompletedTask;
	}

	public override Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
	{
		if (side != CombatSide.Player)
		{
			return Task.CompletedTask;
		}
		RefreshDamage();
		return Task.CompletedTask;
	}

	private void RefreshDamage()
	{
		DynamicVars.Damage.BaseValue = DisplayAmount;
		InvokeDisplayAmountChanged();
	}

	public override async Task AfterSideTurnEndLate(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
	{
		if (participants.Contains(Owner))
		{
			Flash();
			await CreatureCmd.Damage(choiceContext, Owner, DynamicVars.Damage, null, null, null);
		}
	}
}
