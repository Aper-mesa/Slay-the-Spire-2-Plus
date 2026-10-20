using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Rooms;

namespace MegaCrit.Sts2.Core.Models.Relics;

public sealed class MeatOnTheBone : RelicModel
{
	private const string _hpThresholdKey = "HpThreshold";

	public override RelicRarity Rarity => RelicRarity.Rare;

	protected override IEnumerable<DynamicVar> CanonicalVars => new _003C_003Ez__ReadOnlyArray<DynamicVar>(new DynamicVar[2]
	{
		new DynamicVar("HpThreshold", 50m),
		new HealVar(12m)
	});

	public override Task BeforeCombatStart()
	{
		if (WillHealOnCombatFinished())
		{
			Status = RelicStatus.Active;
		}
		return Task.CompletedTask;
	}

	public override Task AfterCurrentHpChanged(Creature creature, decimal delta)
	{
		if (creature != Owner.Creature)
		{
			return Task.CompletedTask;
		}
		if (!CombatManager.Instance.IsInProgress)
		{
			return Task.CompletedTask;
		}
		Status = (WillHealOnCombatFinished() ? RelicStatus.Active : RelicStatus.Normal);
		return Task.CompletedTask;
	}

	public override async Task AfterCombatVictoryEarly(CombatRoom _)
	{
		if (!Owner.Creature.IsDead)
		{
			Creature creature = Owner.Creature;
			if (WillHealOnCombatFinished())
			{
				Status = RelicStatus.Normal;
				await CreatureCmd.Heal(creature, DynamicVars.Heal.BaseValue);
			}
		}
	}

	private bool WillHealOnCombatFinished()
	{
		Creature creature = Owner.Creature;
		int num = (int)((decimal)creature.MaxHp * (DynamicVars["HpThreshold"].BaseValue / 100m));
		return creature.CurrentHp <= num;
	}
}
