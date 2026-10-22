using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Orbs;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.ValueProps;

namespace MegaCrit.Sts2.Core.Models.Powers;

/// <summary>
/// [apermesa] STS1 Amplify: the next Power card this turn is played an extra time, then this expires.
/// (The STS2 SignalBoostPower is charge-based and does not expire at turn end.)
/// </summary>
public sealed class ClassicAmplifyPower : ClassicPowerModel
{
	public override PowerType Type => PowerType.Buff;

	public override PowerStackType StackType => PowerStackType.Counter;

	public override int ModifyCardPlayCount(CardModel card, Creature? target, int playCount)
	{
		if (card.Owner != Owner.Player || card.Type != CardType.Power)
		{
			return playCount;
		}
		return playCount + 1;
	}

	public override async Task AfterModifyingCardPlayCount(CardModel card)
	{
		if (card.Owner == Owner.Player && card.Type == CardType.Power)
		{
			await PowerCmd.Decrement(this);
		}
	}

	public override async Task BeforeSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
	{
		if (participants.Contains(Owner))
		{
			await PowerCmd.Remove(this);
		}
	}
}

/// <summary>
/// [apermesa] STS1 Electrodynamics: Lightning orbs hit all enemies.
/// </summary>
public sealed class ClassicElectroPower : ClassicPowerModel
{
	public override PowerType Type => PowerType.Buff;

	public override PowerStackType StackType => PowerStackType.Single;
}

/// <summary>
/// [apermesa] STS1 Lock-On: the target takes 50% more Lightning damage.
/// </summary>
public sealed class ClassicLockOnPower : ClassicPowerModel
{
	public override PowerType Type => PowerType.Debuff;

	public override PowerStackType StackType => PowerStackType.Counter;

	public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
	{
		if (side == CombatSide.Enemy)
		{
			await PowerCmd.TickDownDuration(this);
		}
	}
}

/// <summary>
/// [apermesa] STS1 Heatsinks: draw cards whenever you play a Power card.
/// </summary>
public sealed class ClassicHeatsinksPower : ClassicPowerModel
{
	private readonly HashSet<CardModel> _powerCardsSeen = new HashSet<CardModel>();

	public override PowerType Type => PowerType.Buff;

	public override PowerStackType StackType => PowerStackType.Counter;

	public override Task BeforeCardPlayed(CardPlay cardPlay)
	{
		if (cardPlay.Card.Owner == Owner.Player && cardPlay.Card.Type == CardType.Power)
		{
			_powerCardsSeen.Add(cardPlay.Card);
		}
		return Task.CompletedTask;
	}

	public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		if (_powerCardsSeen.Remove(cardPlay.Card))
		{
			await CardPileCmd.Draw(choiceContext, Amount, Owner.Player);
		}
	}
}

/// <summary>
/// [apermesa] STS1 Static Discharge: channel Lightning when taking unblocked attack damage.
/// </summary>
public sealed class ClassicStaticDischargePower : ClassicPowerModel
{
	public override PowerType Type => PowerType.Buff;

	public override PowerStackType StackType => PowerStackType.Counter;

	public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
	{
		if (target == Owner && result.UnblockedDamage > 0 && props.IsPoweredAttack())
		{
			for (int i = 0; i < Amount; i++)
			{
				await OrbCmd.Channel<LightningOrb>(choiceContext, Owner.Player);
			}
		}
	}
}

/// <summary>
/// [apermesa] STS1 Self Repair: heal at the end of combat.
/// </summary>
public sealed class ClassicSelfRepairPower : ClassicPowerModel
{
	public override PowerType Type => PowerType.Buff;

	public override PowerStackType StackType => PowerStackType.Counter;

	public override async Task AfterCombatEnd(CombatRoom room)
	{
		if (Owner.IsAlive)
		{
			await CreatureCmd.Heal(Owner, Amount);
		}
	}
}
