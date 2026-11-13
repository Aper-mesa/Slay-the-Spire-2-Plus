using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Powers;

namespace MegaCrit.Sts2.Core.Models.Dread.Dominions;

public class CeremonialBeastDominion : DominionModel
{
	private const int _ritualAmount = 1;

	public override IEnumerable<IHoverTip> ExtraHoverTips
	{
		get
		{
			List<IHoverTip> list = new List<IHoverTip>();
			list.AddRange(HoverTipFactory.FromPowerWithPowerHoverTips<RitualPower>(1));
			return new _003C_003Ez__ReadOnlyList<IHoverTip>(list);
		}
	}

	protected override Task AfterEnemiesEnteredCombat(IEnumerable<Creature> creatures)
	{
		return PowerCmd.Apply<RitualPower>(new ThrowingPlayerChoiceContext(), creatures, 1m, null, null);
	}
}
