using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Orbs;

namespace MegaCrit.Sts2.Core.Models.Relics;

public sealed class ClassicFrozenCore : ClassicRelicModel
{
	public override RelicRarity Rarity => RelicRarity.Rare;

	public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
	{
		if (participants.Contains(Owner.Creature)
			&& Owner.PlayerCombatState.OrbQueue.Orbs.Count < Owner.PlayerCombatState.OrbQueue.Capacity)
		{
			Flash();
			await OrbCmd.Channel<FrostOrb>(choiceContext, Owner);
		}
	}
}

public sealed class ClassicInserter : ClassicRelicModel
{
	private int _turns;

	public override RelicRarity Rarity => RelicRarity.Rare;

	public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
	{
		if (player != Owner)
		{
			return;
		}
		_turns++;
		if (_turns >= 2)
		{
			_turns = 0;
			Flash();
			await OrbCmd.AddSlots(Owner, 1);
		}
	}
}

public sealed class ClassicNuclearBattery : ClassicRelicModel
{
	public override RelicRarity Rarity => RelicRarity.Rare;

	public override async Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
	{
		if (participants.Contains(Owner.Creature) && Owner.PlayerCombatState.TurnNumber <= 1)
		{
			await OrbCmd.Channel<PlasmaOrb>(new BlockingPlayerChoiceContext(), Owner);
		}
	}
}
