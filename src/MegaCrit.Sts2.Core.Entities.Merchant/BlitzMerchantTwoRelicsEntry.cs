using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rewards;

namespace MegaCrit.Sts2.Core.Entities.Merchant;

public sealed class BlitzMerchantTwoRelicsEntry : BlitzMerchantEntry
{
	public override BlitzMerchantEntryType MerchantEntryType => BlitzMerchantEntryType.TwoRelics;

	public BlitzMerchantTwoRelicsEntry(Player player)
		: base(player)
	{
	}

	protected override async Task<bool> OnTrySelect()
	{
		await GiveCurse();
		List<Reward> list = new List<Reward>();
		for (int i = 0; i < 2; i++)
		{
			RelicModel relic = RelicFactory.PullNextRelicFromBack(_player, RelicFactory.RollRarity(_player.PlayerRng.Shops), (RelicModel _) => true).ToMutable();
			list.Add(new RelicReward(relic, _player));
		}
		await RewardsCmd.OfferCustom(_player, list);
		return true;
	}
}
