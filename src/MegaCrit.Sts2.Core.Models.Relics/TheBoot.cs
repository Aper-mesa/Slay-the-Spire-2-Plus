using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace MegaCrit.Sts2.Core.Models.Relics;

public sealed class TheBoot : RelicModel
{
	private const int _damageMinimum = 5;

	private const string _damageMinimumKey = "DamageMinimum";

	private const string _damageThresholdKey = "DamageThreshold";

	public override RelicRarity Rarity => RelicRarity.Event;

	protected override IEnumerable<DynamicVar> CanonicalVars => new _003C_003Ez__ReadOnlyArray<DynamicVar>(new DynamicVar[2]
	{
		new DynamicVar("DamageMinimum", 5m),
		new DynamicVar("DamageThreshold", 4m)
	});

	public override decimal ModifyHpLostAfterOstyLate(Creature target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
	{
		if (dealer != Owner.Creature && dealer != Owner.Osty)
		{
			return amount;
		}
		if (target == Owner.Creature)
		{
			return amount;
		}
		if (!props.IsPoweredAttack())
		{
			return amount;
		}
		if (amount < 1m)
		{
			return amount;
		}
		if (amount >= DynamicVars["DamageMinimum"].BaseValue)
		{
			return amount;
		}
		return DynamicVars["DamageMinimum"].BaseValue;
	}

	public override Task AfterModifyingHpLostAfterOsty()
	{
		Flash();
		return Task.CompletedTask;
	}
}
