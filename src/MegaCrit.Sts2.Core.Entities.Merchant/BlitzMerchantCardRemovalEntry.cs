using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Players;

namespace MegaCrit.Sts2.Core.Entities.Merchant;

public sealed class BlitzMerchantCardRemovalEntry : BlitzMerchantEntry
{
	public override BlitzMerchantEntryType MerchantEntryType => BlitzMerchantEntryType.CardRemoval;

	public BlitzMerchantCardRemovalEntry(Player player)
		: base(player)
	{
	}

	protected override async Task<bool> OnTrySelect()
	{
		if (Status != BlitzEntryStatus.Available)
		{
			return false;
		}
		return await MerchantCardRemovalEntry.DoCardRemoval(_player, isCancelable: true);
	}
}
