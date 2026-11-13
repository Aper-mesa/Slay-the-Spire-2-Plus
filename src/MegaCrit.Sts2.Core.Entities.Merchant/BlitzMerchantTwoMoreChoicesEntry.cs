using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Runs;

namespace MegaCrit.Sts2.Core.Entities.Merchant;

public sealed class BlitzMerchantTwoMoreChoicesEntry : BlitzMerchantEntry
{
	public override BlitzMerchantEntryType MerchantEntryType => BlitzMerchantEntryType.TwoMoreChoices;

	public BlitzMerchantTwoMoreChoicesEntry(Player player)
		: base(player)
	{
	}

	protected override async Task<bool> OnTrySelect()
	{
		await GiveCurse();
		RunManager.Instance.MerchantSynchronizer.GetInventoryFor(_player).BlitzChoicesRemaining += 2;
		return true;
	}
}
