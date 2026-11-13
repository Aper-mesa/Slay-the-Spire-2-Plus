using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Cards;

namespace MegaCrit.Sts2.Core.Models.Powers;

public sealed class ShowOffPower : PowerModel
{
	private ShowOff? _showOffInstance;

	public override PowerType Type => PowerType.Buff;

	public override PowerStackType StackType => PowerStackType.Counter;

	public override PowerInstanceType InstanceType => PowerInstanceType.Instanced;

	public ShowOff? ShowOffInstance
	{
		get
		{
			return _showOffInstance;
		}
		set
		{
			AssertMutable();
			_showOffInstance = value;
		}
	}

	public override async Task AfterDeath(PlayerChoiceContext choiceContext, Creature target, CardModel? source, bool wasRemovalPrevented, float deathAnimLength)
	{
		if (target.Side != Owner.Side && target.Powers.All((PowerModel p) => p.ShouldOwnerDeathTriggerFatal()))
		{
			Flash();
			ShowOffInstance?.IncreaseAim(Amount);
			await PowerCmd.Remove(this);
		}
	}

	public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
	{
		if (participants.Contains(Owner))
		{
			await PowerCmd.Remove(this);
		}
	}
}
