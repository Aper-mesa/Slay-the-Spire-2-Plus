using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Runs;

namespace MegaCrit.Sts2.Core.Entities.Merchant;

public sealed class BlitzMerchantCardRewardsEntry : BlitzMerchantEntry
{
	public override BlitzMerchantEntryType MerchantEntryType => BlitzMerchantEntryType.CardRewards;

	public BlitzMerchantCardRewardsEntry(Player player)
		: base(player)
	{
	}

	protected override async Task<bool> OnTrySelect()
	{
		List<Reward> list = new List<Reward>();
		CardCreationOptions options = new CardCreationOptions(new _003C_003Ez__ReadOnlySingleElementList<CardPoolModel>(_player.Character.CardPool), CardCreationSource.Other, CardRarityOddsType.Blitz).WithRngOverride(_player.PlayerRng.Shops);
		for (int i = 0; i < 2; i++)
		{
			list.Add(new CardReward(options, 3, _player));
		}
		await RewardsCmd.OfferCustom(_player, list);
		return true;
	}
}
