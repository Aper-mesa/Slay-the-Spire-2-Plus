using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace MegaCrit.Sts2.Core.Models.Powers;

public sealed class NemesisPower : PowerModel
{
	private bool _shouldApplyIntangible;

	public override PowerType Type => PowerType.Buff;

	public override PowerStackType StackType => PowerStackType.Single;

	public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
	{
		if (participants.Contains(Owner))
		{
			_shouldApplyIntangible = !_shouldApplyIntangible;
			if (_shouldApplyIntangible)
			{
				Flash();
				await PowerCmd.Apply<IntangiblePower>(choiceContext, Owner, 1m, Owner, null);
			}
			else if (Owner.HasPower<IntangiblePower>())
			{
				await PowerCmd.Remove(Owner.GetPower<IntangiblePower>());
			}
		}
	}
}
