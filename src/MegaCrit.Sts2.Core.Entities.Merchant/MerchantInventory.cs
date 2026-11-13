using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Runs;

namespace MegaCrit.Sts2.Core.Entities.Merchant;

public class MerchantInventory
{
	private const int _blitzEntryCount = 6;

	private const int _defaultBlitzChoices = 3;

	private static readonly CardType[] _coloredCardTypes = new CardType[5]
	{
		CardType.Attack,
		CardType.Attack,
		CardType.Skill,
		CardType.Skill,
		CardType.Power
	};

	private static readonly CardRarity[] _colorlessCardRarities = new CardRarity[2]
	{
		CardRarity.Uncommon,
		CardRarity.Rare
	};

	private readonly List<MerchantCardEntry> _characterCardEntries;

	private readonly List<MerchantCardEntry> _colorlessCardEntries;

	private readonly List<MerchantRelicEntry> _relicEntries;

	private readonly List<MerchantPotionEntry> _potionEntries;

	private readonly List<BlitzMerchantEntry> _blitzEntries;

	/// <summary>
	/// For tests ONLY. These are not shown anywhere in the UI, but are given IDs.
	/// </summary>
	public List<MerchantEntry> testEntries = new List<MerchantEntry>();

	private bool _purchaseInFlight;

	public IReadOnlyList<MerchantCardEntry> CharacterCardEntries => _characterCardEntries;

	public IReadOnlyList<MerchantCardEntry> ColorlessCardEntries => _colorlessCardEntries;

	public IReadOnlyList<MerchantRelicEntry> RelicEntries => _relicEntries;

	public IReadOnlyList<MerchantPotionEntry> PotionEntries => _potionEntries;

	public MerchantCardRemovalEntry? CardRemovalEntry { get; private set; }

	public IReadOnlyList<BlitzMerchantEntry> BlitzEntries => _blitzEntries;

	public bool IsBlitzShop => BlitzEntries.Count > 0;

	public int BlitzChoicesRemaining { get; set; }

	public Player Player { get; }

	public List<MerchantEntry> AllEntries
	{
		get
		{
			List<MerchantEntry> list = new List<MerchantEntry>();
			list.AddRange(CardEntries);
			list.AddRange(RelicEntries);
			list.AddRange(PotionEntries);
			list.AddRange(BlitzEntries);
			list.AddRange((CardRemovalEntry == null) ? Array.Empty<MerchantCardRemovalEntry>() : new MerchantCardRemovalEntry[1] { CardRemovalEntry });
			list.AddRange(testEntries);
			return list;
		}
	}

	public IEnumerable<MerchantCardEntry> CardEntries => CharacterCardEntries.Concat(ColorlessCardEntries);

	/// <summary>
	/// Only one purchase may run at a time in a shop. A purchase spans the previews and vfx it plays, every slot
	/// stays clickable while it runs, and the shop's own state (the blitz pick count) is only updated at the end
	/// of it, so a click on a second slot would spend a pick the player no longer has.
	/// </summary>
	public bool TryBeginPurchase()
	{
		if (_purchaseInFlight)
		{
			return false;
		}
		_purchaseInFlight = true;
		return true;
	}

	public void EndPurchase()
	{
		_purchaseInFlight = false;
	}

	public MerchantInventory(Player player)
		: this(player, new List<MerchantCardEntry>(), new List<MerchantCardEntry>(), new List<MerchantRelicEntry>(), new List<MerchantPotionEntry>(), null, new List<BlitzMerchantEntry>())
	{
	}

	public MerchantInventory(Player player, List<MerchantCardEntry> characterCardEntries, List<MerchantCardEntry> colorlessCardEntries, List<MerchantRelicEntry> relicEntries, List<MerchantPotionEntry> potionEntries, MerchantCardRemovalEntry? cardRemovalEntry, List<BlitzMerchantEntry> blitzEntries)
	{
		Player = player;
		_characterCardEntries = characterCardEntries;
		_colorlessCardEntries = colorlessCardEntries;
		_relicEntries = relicEntries;
		_potionEntries = potionEntries;
		CardRemovalEntry = cardRemovalEntry;
		_blitzEntries = blitzEntries;
	}

	public static MerchantInventory CreateForNormalMerchant(Player player)
	{
		MerchantInventory merchantInventory = new MerchantInventory(player, GetDefaultCharacterCardEntries(player), GetDefaultColorlessCardEntries(player), GetDefaultRelicEntries(player), GetDefaultPotionEntries(player), new MerchantCardRemovalEntry(player), new List<BlitzMerchantEntry>());
		merchantInventory.Setup();
		return merchantInventory;
	}

	public static MerchantInventory CreateForBlitzMerchant(Player player)
	{
		MerchantInventory merchantInventory = new MerchantInventory(player, new List<MerchantCardEntry>(), new List<MerchantCardEntry>(), new List<MerchantRelicEntry>(), new List<MerchantPotionEntry>(), null, GetBlitzEntries(player));
		merchantInventory.Setup();
		return merchantInventory;
	}

	public void Setup()
	{
		List<MerchantEntry> allEntries = AllEntries;
		for (int i = 0; i < allEntries.Count; i++)
		{
			MerchantEntry merchantEntry = allEntries[i];
			merchantEntry.Id = (uint)i;
			merchantEntry.PurchaseCompleted += UpdateEntries;
		}
		foreach (MerchantCardEntry cardEntry in CardEntries)
		{
			cardEntry.Populate(this);
		}
		if (Player.RunState.GameMode == GameMode.Blitz)
		{
			BlitzChoicesRemaining = 3;
		}
		if (CardRemovalEntry != null && !Hook.ShouldAllowMerchantCardRemoval(Player.RunState, Player))
		{
			CardRemovalEntry.SetUsed();
		}
		if (_characterCardEntries.Count > 0)
		{
			int index = Player.PlayerRng.Shops.NextInt(_characterCardEntries.Count);
			_characterCardEntries[index].SetOnSale();
		}
	}

	private static List<MerchantCardEntry> GetDefaultCharacterCardEntries(Player player)
	{
		List<CardModel> cardPool = player.Character.CardPool.GetUnlockedCards(player.UnlockState, player.RunState.CardMultiplayerConstraint).ToList();
		List<MerchantCardEntry> list = new List<MerchantCardEntry>();
		for (int i = 0; i < _coloredCardTypes.Length; i++)
		{
			MerchantCardEntry item = new MerchantCardEntry(player, cardPool, _coloredCardTypes[i]);
			list.Add(item);
		}
		return list;
	}

	private static List<MerchantCardEntry> GetDefaultColorlessCardEntries(Player player)
	{
		List<CardModel> cardPool = ModelDb.CardPool<ColorlessCardPool>().GetUnlockedCards(player.UnlockState, player.RunState.CardMultiplayerConstraint).ToList();
		List<MerchantCardEntry> list = new List<MerchantCardEntry>();
		CardRarity[] colorlessCardRarities = _colorlessCardRarities;
		foreach (CardRarity cardRarity in colorlessCardRarities)
		{
			MerchantCardEntry item = new MerchantCardEntry(player, cardPool, cardRarity);
			list.Add(item);
		}
		return list;
	}

	private static List<MerchantRelicEntry> GetDefaultRelicEntries(Player player)
	{
		RelicRarity[] array = new RelicRarity[3]
		{
			RelicFactory.RollRarity(player.PlayerRng.Shops),
			RelicFactory.RollRarity(player.PlayerRng.Shops),
			RelicRarity.Shop
		};
		List<MerchantRelicEntry> list = new List<MerchantRelicEntry>();
		RelicRarity[] array2 = array;
		foreach (RelicRarity rarity in array2)
		{
			list.Add(new MerchantRelicEntry(rarity, player));
		}
		return list;
	}

	private static List<BlitzMerchantEntry> GetBlitzEntries(Player player)
	{
		List<BlitzMerchantEntryType> list = new List<BlitzMerchantEntryType>();
		List<BlitzMerchantEntryType> list2 = new List<BlitzMerchantEntryType>();
		List<BlitzMerchantEntry> list3 = new List<BlitzMerchantEntry>();
		BlitzMerchantEntryType[] values = Enum.GetValues<BlitzMerchantEntryType>();
		foreach (BlitzMerchantEntryType blitzMerchantEntryType in values)
		{
			if (blitzMerchantEntryType < BlitzMerchantEntryType.Glam)
			{
				list.Add(blitzMerchantEntryType);
			}
			else
			{
				list2.Add(blitzMerchantEntryType);
			}
		}
		int num = 5;
		while (list3.Count < num && list.Count > 0)
		{
			BlitzMerchantEntryType blitzMerchantEntryType2 = player.PlayerRng.Shops.WeightedNextItem(list, (BlitzMerchantEntryType e) => (!BlitzMerchantEntry.RareBlitzEntries.Contains(e)) ? 1f : 0.5f);
			list.Remove(blitzMerchantEntryType2);
			list3.Add(BlitzMerchantEntry.Generate(player, blitzMerchantEntryType2));
		}
		BlitzMerchantEntryType merchantEntryType = player.PlayerRng.Shops.NextItem(list2);
		list3.Add(BlitzMerchantEntry.Generate(player, merchantEntryType));
		return list3;
	}

	private static List<MerchantPotionEntry> GetDefaultPotionEntries(Player player)
	{
		List<PotionModel> list = PotionFactory.CreateRandomPotionsOutOfCombat(player, 3, player.PlayerRng.Shops).ToList();
		List<MerchantPotionEntry> list2 = new List<MerchantPotionEntry>();
		foreach (PotionModel item in list)
		{
			list2.Add(new MerchantPotionEntry(item.ToMutable(), player));
		}
		return list2;
	}

	/// <summary>
	/// Attempt to purchase the entry with the given ID.
	/// Throws if an entry with the ID doesn't exist.
	/// This method is unsynchronized and may cause divergences. Usually, you want
	/// <see cref="M:MegaCrit.Sts2.Core.Multiplayer.Game.MerchantSynchronizer.PurchaseLocalEntry(MegaCrit.Sts2.Core.Entities.Merchant.MerchantEntry)" />.
	/// </summary>
	public Task<bool> TryPurchaseEntryWithIdUnsynchronized(uint id)
	{
		MerchantEntry merchantEntry = AllEntries.FirstOrDefault((MerchantEntry e) => e.Id == id);
		if (merchantEntry == null)
		{
			throw new InvalidOperationException($"Tried to purchase entry with id {id} from inventory owned by {Player.NetId}, but no entry was found with that ID!");
		}
		return merchantEntry.OnTryPurchaseUnsynchronized(this);
	}

	private void UpdateEntries(PurchaseStatus _, MerchantEntry boughtEntry)
	{
		if (boughtEntry is BlitzMerchantEntry)
		{
			BlitzChoicesRemaining--;
			if (BlitzChoicesRemaining <= 0)
			{
				foreach (BlitzMerchantEntry item in AllEntries.OfType<BlitzMerchantEntry>())
				{
					if (item.Status == BlitzMerchantEntry.BlitzEntryStatus.Available)
					{
						item.SetDisabled();
					}
				}
			}
		}
		foreach (MerchantEntry allEntry in AllEntries)
		{
			allEntry.OnMerchantInventoryUpdated();
		}
	}
}
