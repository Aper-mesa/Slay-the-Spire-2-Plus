using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Events;

namespace MegaCrit.Sts2.Core.Models.Events;

public sealed class DoorsOfLightAndDark : EventModel
{
	protected override IReadOnlyList<EventOption> GenerateInitialOptions()
	{
		return new _003C_003Ez__ReadOnlyArray<EventOption>(new EventOption[2]
		{
			new EventOption(this, Light, "DOORS_OF_LIGHT_AND_DARK.pages.INITIAL.options.LIGHT"),
			new EventOption(this, Dark, "DOORS_OF_LIGHT_AND_DARK.pages.INITIAL.options.DARK")
		});
	}

	private async Task Light()
	{
		CardModel cardModel = (await CardSelectCmd.FromDeckForUpgrade(Owner, new CardSelectorPrefs(CardSelectorPrefs.UpgradeSelectionPrompt, 1))).FirstOrDefault();
		if (cardModel != null)
		{
			CardCmd.Upgrade(cardModel);
		}
		SetEventFinished(L10NLookup("DOORS_OF_LIGHT_AND_DARK.pages.LIGHT.description"));
	}

	private async Task Dark()
	{
		List<CardModel> cards = (await CardSelectCmd.FromDeckForRemoval(prefs: new CardSelectorPrefs(CardSelectorPrefs.RemoveSelectionPrompt, 1), player: Owner)).ToList();
		await CardPileCmd.RemoveFromDeck(cards);
		SetEventFinished(L10NLookup("DOORS_OF_LIGHT_AND_DARK.pages.DARK.description"));
	}
}
