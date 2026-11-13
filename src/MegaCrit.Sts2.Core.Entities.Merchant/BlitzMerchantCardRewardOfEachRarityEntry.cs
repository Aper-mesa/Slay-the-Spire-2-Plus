using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Runs;

namespace MegaCrit.Sts2.Core.Entities.Merchant;

public sealed class BlitzMerchantCardRewardOfEachRarityEntry : BlitzMerchantEntry
{
	public override BlitzMerchantEntryType MerchantEntryType => BlitzMerchantEntryType.CardRewardOfEachRarity;

	public BlitzMerchantCardRewardOfEachRarityEntry(Player player)
		: base(player)
	{
	}

	protected override async Task<bool> OnTrySelect()
	{
		await GiveCurse();
		List<Reward> list = new List<Reward>();
		CardCreationOptions options = CardCreationOptions.ForNonCombatWithUniformOdds(new _003C_003Ez__ReadOnlySingleElementList<CardPoolModel>(_player.Character.CardPool), (CardModel c) => c.Rarity == CardRarity.Common).WithFlags(CardCreationFlags.NoRarityModification);
		CardCreationOptions options2 = CardCreationOptions.ForNonCombatWithUniformOdds(new _003C_003Ez__ReadOnlySingleElementList<CardPoolModel>(_player.Character.CardPool), (CardModel c) => c.Rarity == CardRarity.Uncommon).WithFlags(CardCreationFlags.NoRarityModification);
		CardCreationOptions options3 = CardCreationOptions.ForNonCombatWithUniformOdds(new _003C_003Ez__ReadOnlySingleElementList<CardPoolModel>(_player.Character.CardPool), (CardModel c) => c.Rarity == CardRarity.Rare).WithFlags(CardCreationFlags.NoRarityModification);
		list.Add(new CardReward(options, 3, _player));
		list.Add(new CardReward(options2, 3, _player));
		list.Add(new CardReward(options3, 3, _player));
		await RewardsCmd.OfferCustom(_player, list);
		return true;
	}
}
