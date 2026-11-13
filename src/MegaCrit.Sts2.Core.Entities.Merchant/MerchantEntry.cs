using System;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.TestSupport;

namespace MegaCrit.Sts2.Core.Entities.Merchant;

/// <summary>
/// Represents purchasable entry in the merchant shop.
/// Wrapped in an NMerchant_Node that manages the UI interactions.
/// </summary>
public abstract class MerchantEntry
{
	protected readonly Player _player;

	protected int _cost;

	public virtual int Cost
	{
		get
		{
			decimal num = _cost;
			if (_player.RunState.CurrentRoom is MerchantRoom)
			{
				num = Hook.ModifyMerchantPrice(_player.RunState, _player, this, _cost);
			}
			return (int)num;
		}
	}

	public bool EnoughGold => Cost <= _player.Gold;

	/// <summary>
	/// Player to whom the entry is being presented.
	/// </summary>
	public Player Player => _player;

	/// <summary>
	/// Id of the entry within the containing merchant inventory.
	/// If this entry is not contained within a merchant inventory, then this will be null.
	/// </summary>
	public uint? Id { get; set; }

	public abstract bool IsStocked { get; }

	public event Action<PurchaseStatus, MerchantEntry>? PurchaseCompleted;

	public event Action<PurchaseStatus>? PurchaseFailed;

	public event Action? EntryUpdated;

	public void InvokePurchaseCompleted(MerchantEntry entry)
	{
		PurchaseCompleted?.Invoke(PurchaseStatus.Success, entry);
	}

	public void InvokePurchaseFailed(PurchaseStatus status)
	{
		PurchaseFailed?.Invoke(status);
	}

	protected MerchantEntry(Player player)
	{
		_player = player;
	}

	protected virtual void UpdateEntry()
	{
	}

	public void OnMerchantInventoryUpdated()
	{
		UpdateEntry();
		EntryUpdated?.Invoke();
	}

	public abstract void CalcCost();

	/// <summary>
	/// Attempts to purchase the item and returns false if it could not be purchased.
	/// Also invokes <see cref="E:MegaCrit.Sts2.Core.Entities.Merchant.MerchantEntry.PurchaseFailed" /> with the reason if it could not be purchased.
	/// See <see cref="T:MegaCrit.Sts2.Core.Entities.Merchant.PurchaseStatus" /> for possible failure reasons.
	/// This method is unsynchronized, and therefore may cause divergences, so only call it if you know what you are
	/// doing. In most cases, you should call <see cref="M:MegaCrit.Sts2.Core.Multiplayer.Game.MerchantSynchronizer.PurchaseLocalEntry(MegaCrit.Sts2.Core.Entities.Merchant.MerchantEntry)" />.
	/// </summary>
	public async Task<bool> OnTryPurchaseUnsynchronized(MerchantInventory? inventory, bool ignoreCost = false)
	{
		if (TestMode.IsOff && !RunManager.Instance.IsInProgress)
		{
			return false;
		}
		if (!IsStocked)
		{
			InvokePurchaseFailed(PurchaseStatus.FailureOutOfStock);
			return false;
		}
		if (!EnoughGold && !ignoreCost)
		{
			InvokePurchaseFailed(PurchaseStatus.FailureGold);
			return false;
		}
		var (success, goldSpent) = await OnTryPurchaseInternal(inventory, ignoreCost);
		if (success)
		{
			if (_player.RunState.CurrentRoom is MerchantRoom && Hook.ShouldRefillMerchantEntry(_player.RunState, this, _player))
			{
				RestockAfterPurchase(inventory);
			}
			else
			{
				ClearAfterPurchase();
			}
			await Hook.AfterItemPurchased(_player.RunState, _player, this, goldSpent);
			InvokePurchaseCompleted(this);
		}
		return success;
	}

	/// <summary>
	/// Subclass-specific logic for purchasing the item in the entry.
	/// </summary>
	/// <param name="inventory">Inventory that the entry is being purchased in.</param>
	/// <param name="ignoreCost">Whether or not the cost should be ignored</param>
	/// <returns>Tuple containing whether or not the purchase was successful and the amount of gold spent.</returns>
	protected abstract Task<(bool, int)> OnTryPurchaseInternal(MerchantInventory? inventory, bool ignoreCost);

	/// <summary>
	/// Subclass-specific logic for clearing the entry after a purchase.
	/// </summary>
	protected abstract void ClearAfterPurchase();

	/// <summary>
	/// Subclass-specific logic for restocking the entry after a purchase if the player has an effect like
	/// <see cref="T:MegaCrit.Sts2.Core.Models.Relics.TheCourier" />.
	/// </summary>
	protected abstract void RestockAfterPurchase(MerchantInventory? inventory);
}
