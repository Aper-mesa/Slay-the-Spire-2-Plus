using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Rewards;

namespace MegaCrit.Sts2.Core.Entities.Merchant;

public sealed class BlitzMerchantPotionsEntry : BlitzMerchantEntry
{
	public override BlitzMerchantEntryType MerchantEntryType => BlitzMerchantEntryType.Potions;

	public BlitzMerchantPotionsEntry(Player player)
		: base(player)
	{
	}

	protected override async Task<bool> OnTrySelect()
	{
		List<Reward> rewards = new List<Reward>();
		await PlayerCmd.GainMaxPotionCount(1, _player);
		for (int i = 0; i < 2; i++)
		{
			rewards.Add(new PotionReward(_player).SetRng(_player.PlayerRng.Shops));
		}
		await RewardsCmd.OfferCustom(_player, rewards);
		return true;
	}
}
