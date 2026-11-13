using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;

namespace MegaCrit.Sts2.Core.Entities.Merchant;

public sealed class BlitzMerchantCardTransformEntry : BlitzMerchantEntry
{
	public override BlitzMerchantEntryType MerchantEntryType => BlitzMerchantEntryType.CardTransform;

	public BlitzMerchantCardTransformEntry(Player player)
		: base(player)
	{
	}

	protected override async Task<bool> OnTrySelect()
	{
		CardSelectorPrefs prefs = new CardSelectorPrefs(CardSelectorPrefs.TransformSelectionPrompt, 1)
		{
			Cancelable = true
		};
		IEnumerable<CardModel> enumerable = await CardSelectCmd.FromDeckForTransformation(_player, prefs);
		if (!enumerable.Any())
		{
			return false;
		}
		foreach (CardModel item in enumerable)
		{
			await CardCmd.TransformToRandom(item, _player.PlayerRng.Transformations);
		}
		return true;
	}
}
