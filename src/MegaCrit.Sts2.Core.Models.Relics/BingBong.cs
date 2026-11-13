using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;

namespace MegaCrit.Sts2.Core.Models.Relics;

public sealed class BingBong : RelicModel
{
	public override RelicRarity Rarity => RelicRarity.Event;

	public override async Task AfterCardChangedPiles(CardModel card, PileType oldPileType, AbstractModel? clonedBy)
	{
		CardPile? pile = card.Pile;
		if (pile != null && pile.Type == PileType.Deck && card.Owner == Owner && clonedBy == null)
		{
			Flash();
			CardModel card2 = Owner.RunState.CloneCard(card);
			CardCmd.PreviewCardPileAdd(await CardPileCmd.Add(card2, PileType.Deck, CardPilePosition.Bottom, this));
		}
	}
}
