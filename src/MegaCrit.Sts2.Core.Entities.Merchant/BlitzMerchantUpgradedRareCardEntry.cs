using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;

namespace MegaCrit.Sts2.Core.Entities.Merchant;

public sealed class BlitzMerchantUpgradedRareCardEntry : BlitzMerchantEntry
{
	public override BlitzMerchantEntryType MerchantEntryType => BlitzMerchantEntryType.UpgradedRareCard;

	public BlitzMerchantUpgradedRareCardEntry(Player player)
		: base(player)
	{
	}

	protected override async Task<bool> OnTrySelect()
	{
		CardCreationOptions options = new CardCreationOptions(new _003C_003Ez__ReadOnlySingleElementList<CardPoolModel>(_player.Character.CardPool), CardCreationSource.Other, CardRarityOddsType.Uniform, (CardModel c) => c.Rarity == CardRarity.Rare).WithFlags(CardCreationFlags.NoUpgradeRoll).WithRngOverride(_player.PlayerRng.Shops);
		List<CardModel> list = (from r in CardFactory.CreateForReward(_player, 1, options)
			select r.Card).ToList();
		if (list.Count > 0)
		{
			CardModel card = list[0];
			CardCmd.Upgrade(card);
			CardCmd.PreviewCardPileAdd(await CardPileCmd.Add(card, PileType.Deck));
		}
		return true;
	}
}
