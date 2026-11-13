using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Blitz;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.Runs.History;

namespace MegaCrit.Sts2.Core.Entities.Merchant;

public sealed class BlitzMerchantCardEnchantEntry : BlitzMerchantEntry
{
	private (EnchantmentModel Model, int Amount) _enchantment;

	public override BlitzMerchantEntryType MerchantEntryType => BlitzMerchantEntryType.CardEnchant;

	public override LocString Title
	{
		get
		{
			LocString title = base.Title;
			title.Add("Enchantment", _enchantment.Model.Title);
			return title;
		}
	}

	public override LocString Description
	{
		get
		{
			LocString description = base.Description;
			description.Add("Enchantment", _enchantment.Model.Title);
			if (_enchantment.Model.ShowAmount)
			{
				description.Add("Amount", _enchantment.Amount);
			}
			else
			{
				description.Add("Amount", 1m);
			}
			return description;
		}
	}

	protected override IEnumerable<IHoverTip> ExtraHoverTips => HoverTipFactory.FromEnchantment(_enchantment.Model, _enchantment.Amount);

	public BlitzMerchantCardEnchantEntry(Player player)
		: base(player)
	{
		List<(EnchantmentModel, int)> items = BlitzRunModel.shopEnchantmentOptions.Where(((EnchantmentModel, int) e) => player.Deck.Cards.Any((CardModel c) => e.Item1.CanEnchant(c))).ToList();
		_enchantment = _player.PlayerRng.Shops.NextItem(items);
	}

	protected override async Task<bool> OnTrySelect()
	{
		CardSelectorPrefs prefs = new CardSelectorPrefs(CardSelectorPrefs.EnchantSelectionPrompt, 1)
		{
			Cancelable = true
		};
		CardModel cardModel = (await CardSelectCmd.FromDeckForEnchantment(_player, _enchantment.Model, _enchantment.Amount, prefs)).FirstOrDefault();
		if (cardModel == null)
		{
			return false;
		}
		CardCmd.Enchant(_enchantment.Model.ToMutable(), cardModel, _enchantment.Amount);
		NCardEnchantVfx nCardEnchantVfx = NCardEnchantVfx.Create(cardModel);
		if (nCardEnchantVfx != null)
		{
			NRun.Instance?.GlobalUi.CardPreviewContainer.AddChildSafely(nCardEnchantVfx);
		}
		return true;
	}

	public override BlitzMerchantHistoryEntry GenerateHistoryEntry(bool wasPicked)
	{
		BlitzMerchantHistoryEntry result = new BlitzMerchantHistoryEntry(MerchantEntryType, wasPicked);
		result.modelId = _enchantment.Model.Id;
		return result;
	}
}
