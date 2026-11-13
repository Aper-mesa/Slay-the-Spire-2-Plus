using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Runs;

namespace MegaCrit.Sts2.Core.Entities.Merchant;

public sealed class BlitzMerchantColorlessCardRewardEntry : BlitzMerchantEntry
{
	public override BlitzMerchantEntryType MerchantEntryType => BlitzMerchantEntryType.ColorlessCardReward;

	public BlitzMerchantColorlessCardRewardEntry(Player player)
		: base(player)
	{
	}

	protected override async Task<bool> OnTrySelect()
	{
		if (Status != BlitzEntryStatus.Available)
		{
			return false;
		}
		CardCreationOptions options = new CardCreationOptions(new _003C_003Ez__ReadOnlySingleElementList<CardPoolModel>(ModelDb.CardPool<ColorlessCardPool>()), CardCreationSource.Other, CardRarityOddsType.Blitz).WithRngOverride(_player.PlayerRng.Shops);
		CardReward item = new CardReward(options, 3, _player);
		await RewardsCmd.OfferCustom(_player, new List<Reward>(1) { item });
		return true;
	}
}
