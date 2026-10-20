using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using MegaCrit.Sts2.Core.Rooms;

namespace MegaCrit.Sts2.Core.Models.Powers;

public sealed class IllusionPower : PowerModel
{
	private class Data
	{
		public bool isReviving;
	}

	public const string stunTrigger = "StunTrigger";

	public const string wakeUpTrigger = "WakeUpTrigger";

	private string? _followUpStateId;

	public override PowerType Type => PowerType.Buff;

	public override PowerStackType StackType => PowerStackType.Single;

	public override bool ShouldPlayVfx => false;

	public string? FollowUpStateId
	{
		get
		{
			return _followUpStateId;
		}
		set
		{
			AssertMutable();
			_followUpStateId = value;
		}
	}

	public bool IsReviving => GetInternalData<Data>().isReviving;

	protected override object InitInternalData()
	{
		return new Data();
	}

	/// <summary>
	/// Illusions keep their buffs (including IllusionPower itself) after dying.
	/// We ignore Temporary powers (ie temp strength down from dark shackles or dying star) so that the
	/// temp powers they are applying also go away
	/// </summary>
	public override bool ShouldPowerBeRemovedOnDeath(PowerModel power)
	{
		if (power.Type == PowerType.Debuff)
		{
			return !(power is ITemporaryPower);
		}
		return false;
	}

	public override Task AfterApplied(Creature? applier, CardModel? cardSource)
	{
		if (Owner.HasPower<MinionPower>())
		{
			return Task.CompletedTask;
		}
		return PowerCmd.Apply<MinionPower>(new ThrowingPlayerChoiceContext(), Owner, 1m, null, null);
	}

	public override async Task AfterDeath(PlayerChoiceContext choiceContext, Creature creature, bool wasRemovalPrevented, float deathAnimLength)
	{
		if (!wasRemovalPrevented && creature == Owner)
		{
			await CreatureCmd.TriggerAnim(Owner, "StunTrigger", 0f);
			GetInternalData<Data>().isReviving = true;
			MoveState state = new MoveState("REVIVE_MOVE", ReviveMove, new HealIntent())
			{
				FollowUpStateId = (FollowUpStateId ?? Owner.Monster.MoveStateMachine.StateLog.Last().Id),
				MustPerformOnceBeforeTransitioning = true
			};
			Owner.Monster.SetMoveImmediate(state);
		}
	}

	/// <summary>
	/// This is so the owner doesn't receive powers while it is reviving.
	/// </summary>
	public override bool ShouldAllowHitting(Creature creature)
	{
		if (creature != Owner)
		{
			return true;
		}
		if (IsReviving)
		{
			return false;
		}
		return true;
	}

	public override bool ShouldCreatureBeRemovedFromCombatAfterDeath(Creature creature)
	{
		if (creature != Owner)
		{
			return true;
		}
		return false;
	}

	public override async Task AfterCombatEnd(CombatRoom room)
	{
		if (!Owner.IsAlive)
		{
			await CreatureCmd.TriggerAnim(Owner, "Dead", 0.1f);
		}
	}

	private async Task ReviveMove(IReadOnlyList<Creature> targets)
	{
		await CreatureCmd.TriggerAnim(Owner, "WakeUpTrigger", 0f);
		GetInternalData<Data>().isReviving = false;
		await CreatureCmd.Heal(Owner, Owner.MaxHp - Owner.CurrentHp);
		if (Owner.Monster is Parafright)
		{
			SfxCmd.Play("event:/sfx/enemy/enemy_attacks/obscura/obscura_hologram_heal");
		}
	}
}
