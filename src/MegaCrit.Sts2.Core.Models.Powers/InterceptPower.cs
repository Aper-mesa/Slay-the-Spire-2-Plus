using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Platform;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.ValueProps;

namespace MegaCrit.Sts2.Core.Models.Powers;

public sealed class InterceptPower : PowerModel
{
	private class Data
	{
		public readonly List<Creature> coveredCreatures = new List<Creature>();
	}

	private const string _coveringKey = "Covering";

	public override PowerType Type => PowerType.Buff;

	public override PowerStackType StackType => PowerStackType.Single;

	protected override IEnumerable<DynamicVar> CanonicalVars => new _003C_003Ez__ReadOnlySingleElementList<DynamicVar>(new StringVar("Covering"));

	public bool IsCovering => GetInternalData<Data>().coveredCreatures.Count > 0;

	protected override object InitInternalData()
	{
		return new Data();
	}

	public void AddCoveredCreature(Creature c)
	{
		List<Creature> coveredCreatures = GetInternalData<Data>().coveredCreatures;
		if (!coveredCreatures.Contains(c))
		{
			coveredCreatures.Add(c);
		}
		RefreshCoveringNames();
	}

	public void RemoveCoveredCreature(Creature c)
	{
		GetInternalData<Data>().coveredCreatures.Remove(c);
		RefreshCoveringNames();
	}

	private void RefreshCoveringNames()
	{
		List<Creature> coveredCreatures = GetInternalData<Data>().coveredCreatures;
		StringVar stringVar = (StringVar)DynamicVars["Covering"];
		stringVar.StringValue = "";
		for (int i = 0; i < coveredCreatures.Count; i++)
		{
			stringVar.StringValue += PlatformUtil.GetPlayerName(RunManager.Instance.NetService.Platform, coveredCreatures[i].Player.NetId);
			if (i == coveredCreatures.Count - 2)
			{
				stringVar.StringValue += ", and ";
			}
			else if (i < coveredCreatures.Count - 2)
			{
				stringVar.StringValue += ", ";
			}
		}
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
		return GetInternalData<Data>().coveredCreatures.Count + 1;
	}

	public override async Task AfterRemoved(Creature oldOwner)
	{
		if (oldOwner.CombatState == null)
		{
			return;
		}
		List<CoveredPower> list = (from c in oldOwner.CombatState.GetTeammatesOf(oldOwner).SelectMany((Creature t) => t.GetPowerInstances<CoveredPower>())
			where c.Applier == oldOwner
			select c).ToList();
		foreach (CoveredPower item in list)
		{
			await PowerCmd.Remove(item);
		}
	}

	public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
	{
		if (side == CombatSide.Enemy && Owner.Powers.Contains(this))
		{
			await PowerCmd.Remove(this);
		}
	}
}
