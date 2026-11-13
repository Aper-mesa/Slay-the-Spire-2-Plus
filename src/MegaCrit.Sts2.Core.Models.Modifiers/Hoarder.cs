using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;

namespace MegaCrit.Sts2.Core.Models.Modifiers;

public class Hoarder : ModifierModel
{
	public override async Task AfterCardChangedPiles(CardModel card, PileType oldPileType, AbstractModel? clonedBy)
	{
		if (oldPileType != PileType.None)
		{
			return;
		}
		CardPile? pile = card.Pile;
		if (pile != null && pile.Type == PileType.Deck && clonedBy == null)
		{
			for (int i = 0; i < 2; i++)
			{
				CardModel card2 = card.Owner.RunState.CloneCard(card);
				CardCmd.PreviewCardPileAdd(await CardPileCmd.Add(card2, PileType.Deck, CardPilePosition.Bottom, this));
			}
		}
	}

	public override bool ShouldAllowMerchantCardRemoval(Player player)
	{
		return false;
	}
}
