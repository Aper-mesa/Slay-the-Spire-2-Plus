using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.CommonUi;

namespace MegaCrit.Sts2.Core.Entities.Merchant;

public sealed class BlitzMerchantCardUpgradeEntry : BlitzMerchantEntry
{
	public override BlitzMerchantEntryType MerchantEntryType => BlitzMerchantEntryType.CardUpgrade;

	public BlitzMerchantCardUpgradeEntry(Player player)
		: base(player)
	{
	}

	protected override async Task<bool> OnTrySelect()
	{
		CardSelectorPrefs prefs = new CardSelectorPrefs(CardSelectorPrefs.UpgradeSelectionPrompt, 1)
		{
			Cancelable = true,
			RequireManualConfirmation = true
		};
		IEnumerable<CardModel> enumerable = await CardSelectCmd.FromDeckForUpgrade(_player, prefs);
		if (!enumerable.Any())
		{
			return false;
		}
		foreach (CardModel item in enumerable)
		{
			CardCmd.Upgrade(item, CardPreviewStyle.None);
		}
		return true;
	}
}
