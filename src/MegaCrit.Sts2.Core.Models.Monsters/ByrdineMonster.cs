using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models.Characters;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;

namespace MegaCrit.Sts2.Core.Models.Monsters;

public sealed class ByrdineMonster : MonsterModel
{
	public override int MinInitialHp => 9999;

	public override int MaxInitialHp => 9999;

	public override bool IsInteractable => false;

	public override void SetupSkins(MegaSprite spine, MegaSkeleton skeleton)
	{
		MegaSkeletonDataResource data = skeleton.GetData();
		string skinName;
		MegaSkin megaSkin;
		if (IsMutable)
		{
			skinName = Creature.PetOwner.GetRelic<Byrdine>().Hair;
			CharacterModel character = Creature.PetOwner.Character;
			string text = ((character is Necrobinder) ? "necro" : character.Id.Entry.ToLowerInvariant());
			megaSkin = data.FindSkin("eye/" + text);
			if (megaSkin == null)
			{
				megaSkin = data.FindSkin("eye/ironclad");
			}
		}
		else
		{
			skinName = Byrdine.HairOptions[0];
			megaSkin = data.FindSkin("eye/ironclad");
		}
		MegaSkin megaSkin2 = spine.NewSkin("custom-skin");
		megaSkin2.AddSkin(data.FindSkin(skinName));
		megaSkin2.AddSkin(megaSkin);
		skeleton.SetSkin(megaSkin2);
		skeleton.SetSlotsToSetupPose();
	}

	protected override MonsterMoveStateMachine GenerateMoveStateMachine()
	{
		MoveState moveState = new MoveState("NOTHING_MOVE", (IReadOnlyList<Creature> _) => Task.CompletedTask);
		moveState.FollowUpState = moveState;
		return new MonsterMoveStateMachine(new _003C_003Ez__ReadOnlySingleElementList<MonsterState>(moveState), moveState);
	}
}
