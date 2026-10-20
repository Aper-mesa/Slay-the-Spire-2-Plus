using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Rooms;

namespace MegaCrit.Sts2.Core.Models.Powers;

public sealed class ImprovementPower : PowerModel
{
	public override PowerType Type => PowerType.Buff;

	public override PowerStackType StackType => PowerStackType.Counter;

	public override Task AfterCombatEnd(CombatRoom room)
	{
		List<CardModel> list = PileType.Deck.GetPile(Owner.Player).Cards.Where((CardModel c) => c.IsUpgradable).ToList();
		for (int num = 0; num < Amount; num++)
		{
			if (list.Count == 0)
			{
				break;
			}
			CardModel cardModel = Owner.Player.RunState.Rng.CombatCardSelection.NextItem(list);
			list.Remove(cardModel);
			CardCmd.Upgrade(cardModel);
		}
		return Task.CompletedTask;
	}
}
