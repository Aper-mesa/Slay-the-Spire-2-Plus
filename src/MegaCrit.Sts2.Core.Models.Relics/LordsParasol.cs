using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Debug;
using MegaCrit.Sts2.Core.Entities.Merchant;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Screens.Map;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.TestSupport;

namespace MegaCrit.Sts2.Core.Models.Relics;

public sealed class LordsParasol : RelicModel
{
	public override RelicRarity Rarity => RelicRarity.Ancient;

	/// <summary>
	/// For testing.
	/// </summary>
	public Task? LastPurchaseEverythingTask { get; private set; }

	public override Task AfterRoomEntered(AbstractRoom room)
	{
		if (!(room is MerchantRoom))
		{
			return Task.CompletedTask;
		}
		MerchantInventory inventoryFor = RunManager.Instance.MerchantSynchronizer.GetInventoryFor(Owner);
		LastPurchaseEverythingTask = TaskHelper.RunSafely(PurchaseEverything(inventoryFor));
		return Task.CompletedTask;
	}

	private async Task PurchaseEverything(MerchantInventory inventory)
	{
		bool isLocked = inventory.TryBeginPurchase();
		try
		{
			await PurchaseEverythingLocked(inventory);
		}
		finally
		{
			if (isLocked)
			{
				inventory.EndPurchase();
			}
		}
	}

	private async Task PurchaseEverythingLocked(MerchantInventory inventory)
	{
		bool uiBlocked = false;
		bool ownerIsLocal = LocalContext.IsMe(Owner);
		try
		{
			if (TestMode.IsOff & ownerIsLocal)
			{
				NRun.Instance?.GlobalUi.TopBar.Map.Disable();
				NRun.Instance?.GlobalUi.TopBar.Deck.Disable();
				NMapScreen.Instance?.SetTravelEnabled(enabled: false);
				if (NRun.Instance != null)
				{
					await NRun.Instance.AwaitProcessFrame();
				}
				uiBlocked = true;
				NMerchantRoom.Instance?.Inventory.BlockInput();
				await Cmd.Wait(0.75f);
				NMerchantRoom.Instance?.Inventory.Open();
				await Cmd.Wait(1f);
			}
			foreach (MerchantCardEntry characterCardEntry in inventory.CharacterCardEntries)
			{
				if (!characterCardEntry.IsStocked)
				{
					string text = characterCardEntry.CreationResult?.Card.Id.Entry ?? "NULL";
					SentryService.CaptureMessage("LordsParasol tried to buy an out-of-stock character card: " + text);
				}
				else
				{
					await characterCardEntry.OnTryPurchaseUnsynchronized(inventory, ignoreCost: true);
					await Cmd.Wait(0.25f);
				}
			}
			foreach (MerchantCardEntry colorlessCardEntry in inventory.ColorlessCardEntries)
			{
				if (!colorlessCardEntry.IsStocked)
				{
					string text2 = colorlessCardEntry.CreationResult?.Card.Id.Entry ?? "NULL";
					SentryService.CaptureMessage("LordsParasol tried to buy an out-of-stock colorless card: " + text2);
				}
				else
				{
					await colorlessCardEntry.OnTryPurchaseUnsynchronized(inventory, ignoreCost: true);
					await Cmd.Wait(0.25f);
				}
			}
			foreach (MerchantRelicEntry relicEntry in inventory.RelicEntries)
			{
				if (ownerIsLocal)
				{
					NMerchantRoom.Instance?.Inventory.UnblockInput();
					NRun.Instance?.GlobalUi.TopBar.Map.Enable();
					NRun.Instance?.GlobalUi.TopBar.Deck.Enable();
				}
				await relicEntry.OnTryPurchaseUnsynchronized(inventory, ignoreCost: true);
				if (ownerIsLocal)
				{
					NMerchantRoom.Instance?.Inventory.BlockInput();
					NRun.Instance?.GlobalUi.TopBar.Deck.Disable();
					NRun.Instance?.GlobalUi.TopBar.Map.Disable();
				}
				await Cmd.Wait(0.25f);
			}
			foreach (MerchantPotionEntry potionEntry in inventory.PotionEntries)
			{
				await potionEntry.OnTryPurchaseUnsynchronized(inventory, ignoreCost: true);
				await Cmd.Wait(0.25f);
			}
		}
		finally
		{
			if (uiBlocked)
			{
				NMerchantRoom.Instance?.Inventory.UnblockInput();
				NRun.Instance?.GlobalUi.TopBar.Map.Enable();
				NRun.Instance?.GlobalUi.TopBar.Deck.Enable();
				NMapScreen.Instance?.SetTravelEnabled(enabled: true);
			}
		}
		if (inventory.CardRemovalEntry != null && RunManager.Instance.IsInProgress)
		{
			if (ownerIsLocal)
			{
				NMapScreen.Instance?.SetTravelEnabled(enabled: false);
			}
			inventory.CardRemovalEntry.SetPurchaseUncancelable();
			await inventory.CardRemovalEntry.OnTryPurchaseUnsynchronized(inventory, ignoreCost: true);
			if (ownerIsLocal)
			{
				NMapScreen.Instance?.SetTravelEnabled(enabled: true);
			}
		}
	}
}
