using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace MegaCrit.Sts2.Core.Models.Powers;

public sealed class SlowPower : PowerModel
{
	private const string _slowAmountKey = "SlowAmount";

	private const string _displayAmountKey = "DisplayAmount";

	public override PowerType Type => PowerType.Debuff;

	public override PowerStackType StackType => PowerStackType.Counter;

	public override int DisplayAmount => DynamicVars["SlowAmount"].IntValue * 10;

	protected override IEnumerable<DynamicVar> CanonicalVars => new _003C_003Ez__ReadOnlyArray<DynamicVar>(new DynamicVar[2]
	{
		new DynamicVar("SlowAmount", 0m),
		new DynamicVar("DisplayAmount", 0m)
	});

	public override Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		DynamicVars["SlowAmount"].BaseValue++;
		DynamicVars["DisplayAmount"].BaseValue = DynamicVars["SlowAmount"].BaseValue * 10m;
		InvokeDisplayAmountChanged();
		return Task.CompletedTask;
	}

	public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource, CardPlay? cardPlay)
	{
		if (target != Owner)
		{
			return 1m;
		}
		if (!props.IsPoweredAttack())
		{
			return 1m;
		}
		return 1m + 0.1m * DynamicVars["SlowAmount"].BaseValue;
	}

	public override Task AfterModifyingDamageAmount(CardModel? cardSource)
	{
		Flash();
		return Task.CompletedTask;
	}

	public override Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
	{
		if (!participants.Contains(Owner))
		{
			return Task.CompletedTask;
		}
		DynamicVars["SlowAmount"].BaseValue = 0m;
		DynamicVars["DisplayAmount"].BaseValue = 0m;
		InvokeDisplayAmountChanged();
		return Task.CompletedTask;
	}
}
