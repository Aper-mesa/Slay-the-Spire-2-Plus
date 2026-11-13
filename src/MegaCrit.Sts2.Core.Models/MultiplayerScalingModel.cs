using System;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.ValueProps;

namespace MegaCrit.Sts2.Core.Models;

/// <summary>
/// Scales block gained by enemies.
/// </summary>
public class MultiplayerScalingModel : RunModel
{
	public override decimal ModifyBlockMultiplicative(Creature target, decimal block, ValueProp props, CardModel? cardSource, CardPlay? cardPlay)
	{
		if (target != null && !target.IsPrimaryEnemy && !target.IsSecondaryEnemy)
		{
			return 1m;
		}
		if (!props.IsPoweredCardOrMonsterMoveBlock())
		{
			return 1m;
		}
		int count = RunState.Players.Count;
		if (count <= 2)
		{
			return count;
		}
		return (decimal)count * GetMultiplayerScaling(CombatState.Encounter, RunState.CurrentActIndex);
	}

	public static decimal GetMultiplayerScaling(EncounterModel? encounter, int actIndex)
	{
		switch (actIndex)
		{
		case 0:
			return 1.1m;
		case 1:
			return 1.2m;
		case 2:
			if (encounter != null && encounter.RoomType == RoomType.Boss)
			{
				return 1.3m;
			}
			return 1.2m;
		default:
			throw new ArgumentOutOfRangeException("actIndex", actIndex, "Invalid act index for HP scaling");
		}
	}
}
