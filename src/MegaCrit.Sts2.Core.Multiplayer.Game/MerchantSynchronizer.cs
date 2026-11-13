using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Merchant;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Multiplayer.Messages.Game.Sync;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Runs.History;

namespace MegaCrit.Sts2.Core.Multiplayer.Game;

public class MerchantSynchronizer : IDisposable
{
	private readonly IRunState _runState;

	private readonly INetGameService _netService;

	private readonly RunLocationTargetedMessageBuffer _locationBuffer;

	private List<MerchantInventory> _inventories = new List<MerchantInventory>();

	public MerchantSynchronizer(INetGameService netService, IRunState runState, RunLocationTargetedMessageBuffer locationBuffer)
	{
		_netService = netService;
		_runState = runState;
		_locationBuffer = locationBuffer;
		_locationBuffer.RegisterMessageHandler<MerchantEntryPurchasedMessage>(HandleMerchantOptionPurchasedMessage);
	}

	public void Dispose()
	{
		_locationBuffer.UnregisterMessageHandler<MerchantEntryPurchasedMessage>(HandleMerchantOptionPurchasedMessage);
	}

	public void OnMerchantEntered(List<MerchantInventory>? inventories = null)
	{
		if (_inventories.Count > 0)
		{
			Log.Warn("Entered a merchant while a merchant was already active!");
		}
		if (inventories != null)
		{
			_inventories = inventories;
			return;
		}
		foreach (Player player in _runState.Players)
		{
			MerchantInventory inventory = ((player.RunState.GameMode != GameMode.Blitz) ? MerchantInventory.CreateForNormalMerchant(player) : MerchantInventory.CreateForBlitzMerchant(player));
			SetMerchantInventory(player, inventory);
		}
	}

	/// <summary>
	/// This is public specifically for the fake merchant to be able to set inventories individually.
	/// </summary>
	public void SetMerchantInventory(Player player, MerchantInventory inventory)
	{
		int playerSlotIndex = _runState.GetPlayerSlotIndex(player);
		for (int i = _inventories.Count; i <= playerSlotIndex; i++)
		{
			_inventories.Add(new MerchantInventory(player));
		}
		_inventories[playerSlotIndex] = inventory;
	}

	public MerchantInventory GetLocalInventory()
	{
		return GetInventoryFor(LocalContext.GetMe(_runState));
	}

	public MerchantInventory GetInventoryFor(Player player)
	{
		return _inventories[_runState.GetPlayerSlotIndex(player)];
	}

	/// <summary>
	/// Call this when the local player purchases a merchant entry.
	/// The purchase is propagated to other players, and all players run the entry purchase logic deterministically.
	/// The purchase is immediately run locally, and the task for it is returned.
	/// </summary>
	public async Task<bool> PurchaseLocalEntry(MerchantEntry entry)
	{
		Log.Debug($"Local player purchasing merchant entry {entry.GetType()} with id {entry.Id}");
		if (!entry.Id.HasValue)
		{
			throw new InvalidOperationException("Attempted to purchase entry with null ID! Either it was never added to a MerchantInventory or Setup was never called on the inventory.");
		}
		int playerSlotIndex = _runState.GetPlayerSlotIndex(entry.Player);
		MerchantInventory inventory = _inventories[playerSlotIndex];
		if (!inventory.TryBeginPurchase())
		{
			return false;
		}
		try
		{
			MerchantEntryPurchasedMessage message = new MerchantEntryPurchasedMessage
			{
				Location = _locationBuffer.CurrentLocation,
				id = entry.Id.Value
			};
			_netService.SendMessage(message);
			return await PurchaseEntry(LocalContext.GetMe(_runState), entry.Id.Value);
		}
		finally
		{
			inventory.EndPurchase();
		}
	}

	private void HandleMerchantOptionPurchasedMessage(MerchantEntryPurchasedMessage message, ulong senderId)
	{
		Log.Debug($"Received {"MerchantEntryPurchasedMessage"} from player {senderId} with id {message.id}");
		Player player = _runState.GetPlayer(senderId);
		TaskHelper.RunSafely(PurchaseEntry(player, message.id));
	}

	private Task<bool> PurchaseEntry(Player player, uint entryId)
	{
		int playerSlotIndex = _runState.GetPlayerSlotIndex(player);
		MerchantInventory merchantInventory = _inventories[playerSlotIndex];
		return merchantInventory.TryPurchaseEntryWithIdUnsynchronized(entryId);
	}

	public void OnMerchantExited()
	{
		for (int i = 0; i < _inventories.Count; i++)
		{
			MerchantInventory merchantInventory = _inventories[i];
			Player player = _runState.Players[i];
			PlayerMapPointHistoryEntry playerMapPointHistoryEntry = _runState.CurrentMapPointHistoryEntry?.GetEntry(player.NetId);
			if (playerMapPointHistoryEntry == null)
			{
				continue;
			}
			foreach (MerchantCardEntry characterCardEntry in merchantInventory.CharacterCardEntries)
			{
				if (characterCardEntry.IsStocked)
				{
					playerMapPointHistoryEntry.CardChoices.Add(new CardChoiceHistoryEntry(characterCardEntry.CreationResult.Card, wasPicked: false));
				}
			}
			foreach (MerchantCardEntry colorlessCardEntry in merchantInventory.ColorlessCardEntries)
			{
				if (colorlessCardEntry.IsStocked)
				{
					playerMapPointHistoryEntry.CardChoices.Add(new CardChoiceHistoryEntry(colorlessCardEntry.CreationResult.Card, wasPicked: false));
				}
			}
			foreach (MerchantRelicEntry relicEntry in merchantInventory.RelicEntries)
			{
				if (relicEntry.IsStocked)
				{
					playerMapPointHistoryEntry.RelicChoices.Add(new ModelChoiceHistoryEntry(relicEntry.Model.Id, wasPicked: false));
				}
			}
			foreach (MerchantPotionEntry potionEntry in merchantInventory.PotionEntries)
			{
				if (potionEntry.IsStocked)
				{
					playerMapPointHistoryEntry.PotionChoices.Add(new ModelChoiceHistoryEntry(potionEntry.Model.Id, wasPicked: false));
				}
			}
			foreach (BlitzMerchantEntry blitzEntry in merchantInventory.BlitzEntries)
			{
				if (blitzEntry.Status != BlitzMerchantEntry.BlitzEntryStatus.Used)
				{
					BlitzMerchantHistoryEntry item = blitzEntry.GenerateHistoryEntry(wasPicked: false);
					playerMapPointHistoryEntry.BlitzMerchantChoices.Add(item);
				}
			}
		}
		_inventories.Clear();
	}
}
