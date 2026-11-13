using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Powers;

namespace MegaCrit.Sts2.Core.Models.Dread.Dominions;

public class TestSubjectDominion : DominionModel
{
	public override IEnumerable<IHoverTip> ExtraHoverTips
	{
		get
		{
			List<IHoverTip> list = new List<IHoverTip>();
			list.AddRange(HoverTipFactory.FromPowerWithPowerHoverTips<StrengthPower>());
			return new _003C_003Ez__ReadOnlyList<IHoverTip>(list);
		}
	}

	protected override Task AfterEnemiesEnteredCombat(IEnumerable<Creature> creatures)
	{
		return PowerCmd.Apply<StrengthPower>(new ThrowingPlayerChoiceContext(), creatures, 3m, null, null);
	}
}
