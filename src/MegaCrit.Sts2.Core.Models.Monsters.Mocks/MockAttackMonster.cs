using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;

namespace MegaCrit.Sts2.Core.Models.Monsters.Mocks;

public sealed class MockAttackMonster : MonsterModel
{
	private decimal _damageToDeal = 1m;

	public override bool IsMock => true;

	protected override string VisualsPath => SceneHelper.GetScenePath("creature_visuals/defect");

	public override int MinInitialHp => 9999;

	public override int MaxInitialHp => 9999;

	public decimal DamageToDeal
	{
		get
		{
			return _damageToDeal;
		}
		set
		{
			AssertMutable();
			_damageToDeal = value;
		}
	}

	protected override MonsterMoveStateMachine GenerateMoveStateMachine()
	{
		List<MonsterState> list = new List<MonsterState>();
		MoveState moveState = new MoveState("ATTACK", AttackMove, new SingleAttackIntent((int)DamageToDeal));
		moveState.FollowUpState = moveState;
		list.Add(moveState);
		return new MonsterMoveStateMachine(list, moveState);
	}

	private async Task AttackMove(IReadOnlyList<Creature> targets)
	{
		await DamageCmd.Attack(DamageToDeal).FromMonster(this).Execute(null);
	}
}
