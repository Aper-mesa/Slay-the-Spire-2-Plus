using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rewards;

namespace MegaCrit.Sts2.Core.Entities.Merchant;

public sealed class BlitzMerchantRelicEntry : BlitzMerchantEntry
{
	public override BlitzMerchantEntryType MerchantEntryType => BlitzMerchantEntryType.Relic;

	public BlitzMerchantRelicEntry(Player player)
		: base(player)
	{
	}

	protected override async Task<bool> OnTrySelect()
	{
		RelicModel relic = RelicFactory.PullNextRelicFromBack(_player, RelicFactory.RollRarity(_player.PlayerRng.Shops), (RelicModel _) => true).ToMutable();
		await RewardsCmd.OfferCustom(_player, new List<Reward>(1)
		{
			new RelicReward(relic, _player)
		});
		return true;
	}
}
