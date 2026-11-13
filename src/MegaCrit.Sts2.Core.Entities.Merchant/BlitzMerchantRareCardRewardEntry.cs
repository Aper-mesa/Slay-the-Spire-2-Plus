using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Runs;

namespace MegaCrit.Sts2.Core.Entities.Merchant;

public sealed class BlitzMerchantRareCardRewardEntry : BlitzMerchantEntry
{
	public override BlitzMerchantEntryType MerchantEntryType => BlitzMerchantEntryType.RareCardReward;

	public BlitzMerchantRareCardRewardEntry(Player player)
		: base(player)
	{
	}

	protected override async Task<bool> OnTrySelect()
	{
		CardCreationOptions options = new CardCreationOptions(new _003C_003Ez__ReadOnlySingleElementList<CardPoolModel>(_player.Character.CardPool), CardCreationSource.Other, CardRarityOddsType.Blitz, (CardModel c) => c.Rarity == CardRarity.Rare).WithRngOverride(_player.PlayerRng.Shops);
		CardReward item = new CardReward(options, 3, _player);
		await RewardsCmd.OfferCustom(_player, new List<Reward>(1) { item });
		return true;
	}
}
