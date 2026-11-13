using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Blitz;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Runs;

namespace MegaCrit.Sts2.Core.Entities.Merchant;

public sealed class BlitzMerchantEnchantedCardRewardEntry : BlitzMerchantEntry
{
	public override BlitzMerchantEntryType MerchantEntryType => BlitzMerchantEntryType.EnchantedCardReward;

	public BlitzMerchantEnchantedCardRewardEntry(Player player)
		: base(player)
	{
	}

	protected override async Task<bool> OnTrySelect()
	{
		if (Status != BlitzEntryStatus.Available)
		{
			return false;
		}
		CardCreationOptions options = new CardCreationOptions(new _003C_003Ez__ReadOnlySingleElementList<CardPoolModel>(_player.Character.CardPool), CardCreationSource.Other, CardRarityOddsType.Blitz).WithRngOverride(_player.PlayerRng.Shops);
		CardReward reward = new CardReward(options, 3, _player);
		reward.AfterGenerated += EnchantCardsInReward;
		await RewardsCmd.OfferCustom(_player, new List<Reward>(1) { reward });
		return true;
		void EnchantCardsInReward()
		{
			foreach (CardModel card in reward.Cards)
			{
				if (card.Enchantment == null)
				{
					(EnchantmentModel, int) tuple = _player.PlayerRng.Shops.NextItem(BlitzRunModel.shopEnchantmentOptions.Where(((EnchantmentModel, int) e) => e.Item1.CanEnchant(card)));
					CardCmd.Enchant(tuple.Item1.ToMutable(), card, tuple.Item2);
				}
			}
		}
	}
}
