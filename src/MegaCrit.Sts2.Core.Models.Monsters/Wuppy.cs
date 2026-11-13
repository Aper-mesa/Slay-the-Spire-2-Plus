using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using MegaCrit.Sts2.Core.Animation;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;

namespace MegaCrit.Sts2.Core.Models.Monsters;

public sealed class Wuppy : MonsterModel
{
	public static readonly Vector2 heightControlOffset = new Vector2(200f, 0f);

	public static readonly Vector2 boundsOffset = new Vector2(0f, -135f);

	public override int MinInitialHp => 50;

	public override int MaxInitialHp => 50;

	public override bool IsInteractable => true;

	protected override MonsterMoveStateMachine GenerateMoveStateMachine()
	{
		MoveState moveState = new MoveState("NOTHING_MOVE", (IReadOnlyList<Creature> _) => Task.CompletedTask);
		moveState.FollowUpState = moveState;
		return new MonsterMoveStateMachine(new _003C_003Ez__ReadOnlySingleElementList<MonsterState>(moveState), moveState);
	}

	public override CreatureAnimator GenerateAnimator(MegaSprite controller)
	{
		AnimState animState = new AnimState("idle_loop", isLooping: true);
		AnimState animState2 = new AnimState("hurt");
		AnimState state = new AnimState("die");
		animState.AddBranch("Hit", animState2);
		animState2.NextState = animState;
		animState2.AddBranch("Hit", animState2);
		CreatureAnimator creatureAnimator = new CreatureAnimator(animState, controller);
		creatureAnimator.AddAnyState("Dead", state);
		return creatureAnimator;
	}
}
