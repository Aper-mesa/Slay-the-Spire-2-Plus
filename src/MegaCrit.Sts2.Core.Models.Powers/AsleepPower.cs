using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.ValueProps;

namespace MegaCrit.Sts2.Core.Models.Powers;

public sealed class AsleepPower : PowerModel
{
	public override PowerType Type => PowerType.Buff;

	public override PowerStackType StackType => PowerStackType.Counter;

	public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
	{
		if (target == Owner && result.UnblockedDamage != 0)
		{
			if (Owner.HasPower<PlatingPower>())
			{
				await PowerCmd.Remove(Owner.GetPower<PlatingPower>());
			}
			LagavulinMatriarch monster = (LagavulinMatriarch)Owner.Monster;
			SfxCmd.Play("event:/sfx/enemy/enemy_attacks/lagavulin_matriarch/lagavulin_matriarch_awaken");
			await CreatureCmd.TriggerAnim(Owner, "Wake", 0.6f);
			monster.IsAwake = true;
			await CreatureCmd.Stun(Owner, monster.WakeUpMove, "SLASH_MOVE");
			await PowerCmd.Remove(this);
		}
	}

	public override async Task BeforeSideTurnEndVeryEarly(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
	{
		if (participants.Contains(Owner) && Amount <= 1 && Owner.HasPower<PlatingPower>())
		{
			await PowerCmd.Remove(Owner.GetPower<PlatingPower>());
		}
	}

	public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
	{
		if (participants.Contains(Owner))
		{
			await PowerCmd.Decrement(this);
			if (Amount <= 0)
			{
				LagavulinMatriarch lagavulinMatriarch = (LagavulinMatriarch)Owner.Monster;
				await lagavulinMatriarch.WakeUpMove(Array.Empty<Creature>());
			}
		}
	}
}
