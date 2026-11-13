using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Ascension;
using MegaCrit.Sts2.Core.Entities.Gold;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes;

namespace MegaCrit.Sts2.Core.Entities.Merchant;

/// <summary>
/// Manages the card removal option of a merchant shop
/// </summary>
public sealed class MerchantCardRemovalEntry : MerchantEntry
{
	private bool _isCancelable = true;

	public bool Used { get; private set; }

	public override bool IsStocked => !Used;

	private static int BaseCost => AscensionHelper.GetValueIfAscension(AscensionLevel.Inflation, 100, 75);

	public static int PriceIncrease => AscensionHelper.GetValueIfAscension(AscensionLevel.Inflation, 50, 25);

	public MerchantCardRemovalEntry(Player player)
		: base(player)
	{
		CalcCost();
	}

	public override void CalcCost()
	{
		_cost = BaseCost + PriceIncrease * _player.ExtraFields.CardShopRemovalsUsed;
	}

	public void SetPurchaseUncancelable()
	{
		_isCancelable = false;
	}

	protected override async Task<(bool, int)> OnTryPurchaseInternal(MerchantInventory? inventory, bool ignoreCost)
	{
		if (Used)
		{
			return (false, 0);
		}
		int goldToSpend = ((!ignoreCost) ? Cost : 0);
		if (!(await DoCardRemoval(_player, _isCancelable)))
		{
			return (false, 0);
		}
		if (!ignoreCost)
		{
			await PlayerCmd.LoseGold(goldToSpend, _player, GoldLossType.Spent);
		}
		SetUsed();
		if (LocalContext.IsMe(_player))
		{
			NRun.Instance?.MerchantRoom?.Inventory.OnCardRemovalUsed();
		}
		return (true, goldToSpend);
	}

	public static async Task<bool> DoCardRemoval(Player player, bool isCancelable)
	{
		CardSelectorPrefs prefs = new CardSelectorPrefs(CardSelectorPrefs.RemoveSelectionPrompt, 1)
		{
			Cancelable = isCancelable,
			RequireManualConfirmation = true
		};
		CardModel card = (await CardSelectCmd.FromDeckForRemoval(player, prefs)).FirstOrDefault();
		if (card != null)
		{
			await CardPileCmd.RemoveFromDeck(card);
			player.ExtraFields.CardShopRemovalsUsed++;
		}
		return card != null;
	}

	protected override void ClearAfterPurchase()
	{
	}

	protected override void RestockAfterPurchase(MerchantInventory? inventory)
	{
	}

	public void SetUsed()
	{
		Used = true;
	}
}
