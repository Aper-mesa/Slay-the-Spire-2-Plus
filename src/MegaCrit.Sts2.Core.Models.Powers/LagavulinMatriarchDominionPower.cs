using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;

namespace MegaCrit.Sts2.Core.Models.Powers;

public sealed class LagavulinMatriarchDominionPower : PowerModel
{
	public override PowerType Type => PowerType.Debuff;

	public override PowerStackType StackType => PowerStackType.Single;

	protected override IEnumerable<IHoverTip> ExtraHoverTips
	{
		get
		{
			List<IHoverTip> list = new List<IHoverTip>();
			list.AddRange(HoverTipFactory.FromPowerWithPowerHoverTips<StrengthPower>());
			list.AddRange(HoverTipFactory.FromPowerWithPowerHoverTips<DexterityPower>());
			return new _003C_003Ez__ReadOnlyList<IHoverTip>(list);
		}
	}

	public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
	{
		if (side == CombatSide.Player && participants.Contains(Owner))
		{
			Flash();
			await PowerCmd.Apply<StrengthPower>(new ThrowingPlayerChoiceContext(), Owner, -Amount, null, null);
			await PowerCmd.Apply<DexterityPower>(new ThrowingPlayerChoiceContext(), Owner, -Amount, null, null);
			await PowerCmd.Remove(this);
		}
	}
}
