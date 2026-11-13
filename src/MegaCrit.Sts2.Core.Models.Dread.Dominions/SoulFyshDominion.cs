using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Cards;

namespace MegaCrit.Sts2.Core.Models.Dread.Dominions;

public class SoulFyshDominion : DominionModel
{
	public override IEnumerable<IHoverTip> ExtraHoverTips
	{
		get
		{
			List<IHoverTip> list = new List<IHoverTip>();
			list.AddRange(HoverTipFactory.FromCardWithCardHoverTips<Beckon>());
			return new _003C_003Ez__ReadOnlyList<IHoverTip>(list);
		}
	}

	protected override async Task AfterPlayersEnteredCombat(IReadOnlyList<Player> players)
	{
		AssertMutable();
		foreach (Player player in players)
		{
			await CardPileCmd.AddToCombatAndPreview<Beckon>(player.Creature, PileType.Draw, 1, player, CardPilePosition.Random);
		}
	}
}
