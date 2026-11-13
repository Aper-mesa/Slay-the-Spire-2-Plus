using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.FoolArrows;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.ValueProps;

namespace MegaCrit.Sts2.Core.Models.Powers;

public sealed class CoveringFirePower : PowerModel
{
	public override PowerType Type => PowerType.Buff;

	public override PowerStackType StackType => PowerStackType.Counter;

	protected override IEnumerable<IHoverTip> ExtraHoverTips => new _003C_003Ez__ReadOnlySingleElementList<IHoverTip>(HoverTipFactory.Static(StaticHoverTip.Block));

	public override async Task AfterArrowShot(Player owner, ArrowSlot? arrow, PlayerChoiceContext context, CardModel? source)
	{
		if (owner.Creature != Owner || source == null)
		{
			return;
		}
		Flash();
		IEnumerable<Creature> enumerable = from c in CombatState.GetTeammatesOf(Owner)
			where c != null && c.IsAlive && c.IsPlayer && c.Player.Creature != Owner
			select c;
		foreach (Creature item in enumerable)
		{
			await CreatureCmd.GainBlock(item, Amount, ValueProp.Unpowered, null);
		}
	}
}
