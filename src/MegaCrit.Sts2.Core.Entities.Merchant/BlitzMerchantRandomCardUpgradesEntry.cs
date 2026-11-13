using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.Models;

namespace MegaCrit.Sts2.Core.Entities.Merchant;

public sealed class BlitzMerchantRandomCardUpgradesEntry : BlitzMerchantEntry
{
	public override BlitzMerchantEntryType MerchantEntryType => BlitzMerchantEntryType.RandomUpgrades;

	public BlitzMerchantRandomCardUpgradesEntry(Player player)
		: base(player)
	{
	}

	protected override Task<bool> OnTrySelect()
	{
		List<CardModel> list = PileType.Deck.GetPile(_player).Cards.Where((CardModel c) => c?.IsUpgradable ?? false).ToList().StableShuffle(_player.PlayerRng.Shops)
			.Take(2)
			.ToList();
		if (list.Count == 0)
		{
			InvokePurchaseFailed(PurchaseStatus.FailureForbidden);
			return Task.FromResult(result: false);
		}
		foreach (CardModel item in list)
		{
			CardCmd.Upgrade(item);
		}
		return Task.FromResult(result: true);
	}
}
