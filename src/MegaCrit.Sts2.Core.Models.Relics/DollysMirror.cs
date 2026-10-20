using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;

namespace MegaCrit.Sts2.Core.Models.Relics;

public sealed class DollysMirror : RelicModel
{
	public override RelicRarity Rarity => RelicRarity.Shop;

	public override bool HasUponPickupEffect => true;

	public override async Task AfterObtained()
	{
		CardModel cardModel = (await CardSelectCmd.FromDeckGeneric(prefs: new CardSelectorPrefs(SelectionScreenPrompt, 1), player: Owner, filter: Filter)).FirstOrDefault();
		if (cardModel != null)
		{
			CardModel card = Owner.RunState.CloneCard(cardModel);
			CardCmd.PreviewCardPileAdd(await CardPileCmd.Add(card, PileType.Deck));
		}
	}

	private bool Filter(CardModel c)
	{
		return c.Type != CardType.Quest;
	}
}
