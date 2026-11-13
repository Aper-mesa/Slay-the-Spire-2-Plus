using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Powers;

namespace MegaCrit.Sts2.Core.Models.Dread.Dominions;

public class KaiserCrabDominion : DominionModel
{
	private const int _platingAmount = 7;

	public override IEnumerable<IHoverTip> ExtraHoverTips
	{
		get
		{
			List<IHoverTip> list = new List<IHoverTip>();
			list.AddRange(HoverTipFactory.FromPowerWithPowerHoverTips<PlatingPower>(7));
			return new _003C_003Ez__ReadOnlyList<IHoverTip>(list);
		}
	}

	protected override async Task AfterEnemiesEnteredCombat(IEnumerable<Creature> creatures)
	{
		await PowerCmd.Apply<PlatingPower>(new ThrowingPlayerChoiceContext(), creatures, 7m, null, null);
	}
}
