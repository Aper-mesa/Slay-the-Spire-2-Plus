using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;

namespace MegaCrit.Sts2.Core.Models.Powers;

public sealed class AllIsWellPower : PowerModel
{
	public override PowerType Type => PowerType.Buff;

	protected override IEnumerable<IHoverTip> ExtraHoverTips => new _003C_003Ez__ReadOnlyArray<IHoverTip>(new IHoverTip[2]
	{
		HoverTipFactory.FromCard<Sadness>(),
		HoverTipFactory.Static(StaticHoverTip.Block)
	});

	public override PowerStackType StackType => PowerStackType.Counter;

	public override async Task AfterCardGeneratedForCombat(CardModel card, Player? creator)
	{
		if (creator != null && creator.Creature == Owner && card is Sadness)
		{
			Flash();
			await CreatureCmd.GainBlock(Owner, Amount, ValueProp.Unpowered, null);
		}
	}
}
