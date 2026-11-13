using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.FoolArrows;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Platform;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.ValueProps;

namespace MegaCrit.Sts2.Core.Models.Powers;

public sealed class ImpaledPower : PowerModel
{
	private class Data
	{
		public List<ArrowSlot> arrowSources = new List<ArrowSlot>();
	}

	private const string _applierTag = "Applier";

	public override PowerType Type => PowerType.None;

	public override PowerStackType StackType => PowerStackType.Counter;

	public override PowerInstanceType InstanceType => PowerInstanceType.InstancedPerApplier;

	public override int DisplayAmount => HitCounter;

	protected override bool IsVisibleInternal => LocalContext.IsMe(Applier);

	public IReadOnlyList<ArrowSlot> Arrows => GetInternalData<Data>().arrowSources.ToList();

	protected override IEnumerable<DynamicVar> CanonicalVars => new _003C_003Ez__ReadOnlyArray<DynamicVar>(new DynamicVar[2]
	{
		new DynamicVar("HitCounter", 3m),
		new StringVar("Applier")
	});

	public int HitCounter
	{
		get
		{
			return DynamicVars["HitCounter"].IntValue;
		}
		set
		{
			AssertMutable();
			DynamicVars["HitCounter"].BaseValue = value;
		}
	}

	protected override object InitInternalData()
	{
		return new Data();
	}

	public void AddArrow(ArrowSlot arrow)
	{
		AssertMutable();
		GetInternalData<Data>().arrowSources.Add(arrow);
	}

	public ArrowSlot RemoveFirstArrow()
	{
		AssertMutable();
		if (Arrows.Count == 0)
		{
			throw new InvalidOperationException("There are no arrows to remove");
		}
		ArrowSlot arrowSlot = GetInternalData<Data>().arrowSources.First();
		GetInternalData<Data>().arrowSources.Remove(arrowSlot);
		return arrowSlot;
	}

	public async Task RemoveIfNoArrows()
	{
		if (Arrows.Count <= 0 && Owner.Powers.Contains(this))
		{
			await PowerCmd.Remove(this);
		}
	}

	public override Task AfterApplied(Creature? applier, CardModel? cardSource)
	{
		((StringVar)DynamicVars["Applier"]).StringValue = PlatformUtil.GetPlayerName(RunManager.Instance.NetService.Platform, Applier.Player.NetId);
		return Task.CompletedTask;
	}

	public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult _, ValueProp props, Creature? dealer, CardModel? source)
	{
		if (target == Owner && dealer != null && dealer == Applier && props.IsPoweredAttack())
		{
			HitCounter--;
			InvokeDisplayAmountChanged();
			if (HitCounter <= 0)
			{
				HitCounter = 3;
				InvokeDisplayAmountChanged();
				await ArrowCmd.DislodgeArrow(CombatState, this, choiceContext, source);
				await RemoveIfNoArrows();
			}
		}
	}

	public override async Task AfterDeath(PlayerChoiceContext choiceContext, Creature creature, CardModel? source, bool wasRemovalPrevented, float deathAnimLength)
	{
		if (creature == Owner && !Applier.IsDead)
		{
			await DislodgeAllArrows(choiceContext, source);
			await PowerCmd.Remove(this);
		}
	}

	public async Task DislodgeAllArrows(PlayerChoiceContext choiceContext, CardModel? source)
	{
		while (Arrows.Count > 0)
		{
			await ArrowCmd.DislodgeArrow(CombatState, this, choiceContext, source);
		}
	}

	public async Task ForceRemoveArrow(ArrowSlot arrow)
	{
		GetInternalData<Data>().arrowSources.Remove(arrow);
		await RemoveIfNoArrows();
	}
}
