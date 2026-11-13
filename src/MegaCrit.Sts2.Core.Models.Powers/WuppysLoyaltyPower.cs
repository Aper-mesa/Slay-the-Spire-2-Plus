using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Saves;
using MegaCrit.Sts2.Core.ValueProps;

namespace MegaCrit.Sts2.Core.Models.Powers;

public sealed class WuppysLoyaltyPower : PowerModel
{
	public override PowerType Type => PowerType.Buff;

	public override PowerStackType StackType => PowerStackType.Single;

	public override bool ShouldPlayVfx => false;

	public override LocString Title
	{
		get
		{
			LocString title = base.Title;
			title.Add("HasSeenOsty", SaveManager.Instance.IsRelicSeen(ModelDb.Relic<BoundPhylactery>()));
			return title;
		}
	}

	public override Creature? ModifyPreBlockDamageTarget(Creature originalTarget, Creature? target, decimal amount, ValueProp props, Creature? dealer)
	{
		if (originalTarget != Owner.PetOwner?.Creature)
		{
			return target;
		}
		if (Owner.IsDead)
		{
			return target;
		}
		if (!props.IsPoweredAttack())
		{
			return target;
		}
		return Owner;
	}

	/// <summary>
	/// This is so Wuppy won't receive powers while it is dead
	/// </summary>
	public override bool ShouldAllowHitting(Creature creature)
	{
		return creature.IsAlive;
	}

	public override bool ShouldCreatureBeRemovedFromCombatAfterDeath(Creature creature)
	{
		if (creature != Owner)
		{
			return true;
		}
		return false;
	}

	public override bool ShouldPowerBeRemovedAfterOwnerDeath()
	{
		return false;
	}
}
