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
using MegaCrit.Sts2.Core.Models.Enchantments;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Vfx;

namespace MegaCrit.Sts2.Core.Entities.Merchant;

public sealed class BlitzMerchantGlamEntry : BlitzMerchantEntry
{
	private readonly EnchantmentModel _enchantment;

	public override BlitzMerchantEntryType MerchantEntryType => BlitzMerchantEntryType.Glam;

	public override LocString Title
	{
		get
		{
			LocString title = base.Title;
			title.Add("Enchantment", _enchantment.Title);
			return title;
		}
	}

	public override LocString Description
	{
		get
		{
			LocString description = base.Description;
			description.Add("Enchantment", _enchantment.Title);
			description.Add("Amount", 1m);
			return description;
		}
	}

	protected override IEnumerable<IHoverTip> ExtraHoverTips => HoverTipFactory.FromEnchantment(_enchantment);

	public BlitzMerchantGlamEntry(Player player)
		: base(player)
	{
		_enchantment = ModelDb.Enchantment<Glam>();
	}

	protected override async Task<bool> OnTrySelect()
	{
		CardSelectorPrefs prefs = new CardSelectorPrefs(CardSelectorPrefs.EnchantSelectionPrompt, 1)
		{
			Cancelable = true
		};
		CardModel card = (await CardSelectCmd.FromDeckForEnchantment(_player, _enchantment, 1, prefs)).FirstOrDefault();
		if (card == null)
		{
			return false;
		}
		await GiveCurse();
		CardCmd.Enchant(_enchantment.ToMutable(), card, 1m);
		NCardEnchantVfx nCardEnchantVfx = NCardEnchantVfx.Create(card);
		if (nCardEnchantVfx != null)
		{
			NRun.Instance?.GlobalUi.CardPreviewContainer.AddChildSafely(nCardEnchantVfx);
		}
		return true;
	}
}
