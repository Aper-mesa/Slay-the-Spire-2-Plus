using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Godot;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Enchantments;
using MegaCrit.Sts2.Core.Runs.History;

namespace MegaCrit.Sts2.Core.Entities.Merchant;

public abstract class BlitzMerchantEntry : MerchantEntry
{
	public enum BlitzEntryStatus
	{
		Available,
		Used,
		Disabled
	}

	public const BlitzMerchantEntryType firstCursedOption = BlitzMerchantEntryType.Glam;

	private static readonly string _errorIconPath = ImageHelper.GetImagePath("packed/blitz/entry_error.png");

	public static List<BlitzMerchantEntryType> RareBlitzEntries
	{
		get
		{
			int num = 3;
			List<BlitzMerchantEntryType> list = new List<BlitzMerchantEntryType>(num);
			CollectionsMarshal.SetCount(list, num);
			Span<BlitzMerchantEntryType> span = CollectionsMarshal.AsSpan(list);
			int num2 = 0;
			span[num2] = BlitzMerchantEntryType.PrismaticCardReward;
			num2++;
			span[num2] = BlitzMerchantEntryType.RareCardReward;
			num2++;
			span[num2] = BlitzMerchantEntryType.UpgradedRareCard;
			return list;
		}
	}

	public abstract BlitzMerchantEntryType MerchantEntryType { get; }

	public bool IsCursed => MerchantEntryType >= BlitzMerchantEntryType.Glam;

	public bool IsRare => RareBlitzEntries.Contains(MerchantEntryType);

	public virtual LocString Title => new LocString("merchant_room", "MERCHANT.blitz." + StringHelper.SnakeCase(MerchantEntryType.ToString()) + "_entry.title");

	public virtual LocString Description => new LocString("merchant_room", "MERCHANT.blitz." + StringHelper.SnakeCase(MerchantEntryType.ToString()) + "_entry.description");

	private string EntryIconPath => ImageHelper.GetImagePath("packed/blitz/entry_" + StringHelper.SnakeCase(MerchantEntryType.ToString()) + ".png");

	public string IconPath
	{
		get
		{
			if (!ResourceLoader.Exists(EntryIconPath))
			{
				return _errorIconPath;
			}
			return EntryIconPath;
		}
	}

	public Texture2D Icon => PreloadManager.Cache.GetTexture2D(IconPath);

	private string SmallEntryIconPath => ImageHelper.GetImagePath("packed/blitz/small_tokens/entry_" + StringHelper.SnakeCase(MerchantEntryType.ToString()) + ".png");

	public string SmallIconPath
	{
		get
		{
			if (!ResourceLoader.Exists(SmallEntryIconPath))
			{
				return _errorIconPath;
			}
			return SmallEntryIconPath;
		}
	}

	public Texture2D SmallIcon => PreloadManager.Cache.GetTexture2D(SmallIconPath);

	protected virtual IEnumerable<IHoverTip> ExtraHoverTips => Array.Empty<IHoverTip>();

	public IEnumerable<IHoverTip> HoverTips
	{
		get
		{
			List<IHoverTip> list = new List<IHoverTip>();
			LocString title = Title;
			LocString description = Description;
			list.Add(new HoverTip(title, description));
			list.AddRange(ExtraHoverTips);
			return list;
		}
	}

	public BlitzEntryStatus Status { get; private set; }

	public override bool IsStocked => Status == BlitzEntryStatus.Available;

	public override int Cost => 0;

	protected BlitzMerchantEntry(Player player)
		: base(player)
	{
	}

	public override void CalcCost()
	{
	}

	protected override void ClearAfterPurchase()
	{
	}

	protected override void RestockAfterPurchase(MerchantInventory? inventory)
	{
	}

	protected override async Task<(bool, int)> OnTryPurchaseInternal(MerchantInventory? inventory, bool ignoreCost)
	{
		if (Status != BlitzEntryStatus.Available)
		{
			return (false, 0);
		}
		bool flag = await OnTrySelect();
		if (flag)
		{
			SetUsed();
			BlitzMerchantHistoryEntry item = GenerateHistoryEntry(wasPicked: true);
			_player.RunState.CurrentMapPointHistoryEntry?.GetEntry(_player.NetId).BlitzMerchantChoices.Add(item);
		}
		return (flag, 0);
	}

	protected virtual Task<bool> OnTrySelect()
	{
		return Task.FromResult(result: true);
	}

	public void SetUsed()
	{
		Status = BlitzEntryStatus.Used;
	}

	public void SetDisabled()
	{
		Status = BlitzEntryStatus.Disabled;
	}

	public static BlitzMerchantEntry Generate(Player player, BlitzMerchantEntryType merchantEntryType)
	{
		return merchantEntryType switch
		{
			BlitzMerchantEntryType.CardUpgrade => (BlitzMerchantEntry)new BlitzMerchantCardUpgradeEntry(player),
			BlitzMerchantEntryType.CardRewards => new BlitzMerchantCardRewardsEntry(player),
			BlitzMerchantEntryType.UpgradedCardReward => new BlitzMerchantUpgradedCardRewardEntry(player),
			BlitzMerchantEntryType.RandomUpgrades => new BlitzMerchantRandomCardUpgradesEntry(player),
			BlitzMerchantEntryType.CardTransform => new BlitzMerchantCardTransformEntry(player),
			BlitzMerchantEntryType.CardRemoval => new BlitzMerchantCardRemovalEntry(player),
			BlitzMerchantEntryType.ColorlessCardReward => new BlitzMerchantColorlessCardRewardEntry(player),
			BlitzMerchantEntryType.Potions => new BlitzMerchantPotionsEntry(player),
			BlitzMerchantEntryType.CardEnchant => new BlitzMerchantCardEnchantEntry(player),
			BlitzMerchantEntryType.EnchantedCardReward => new BlitzMerchantEnchantedCardRewardEntry(player),
			BlitzMerchantEntryType.PrismaticCardReward => new BlitzMerchantPrismaticCardRewardEntry(player),
			BlitzMerchantEntryType.RareCardReward => new BlitzMerchantRareCardRewardEntry(player),
			BlitzMerchantEntryType.UpgradedRareCard => new BlitzMerchantUpgradedRareCardEntry(player),
			BlitzMerchantEntryType.Relic => new BlitzMerchantRelicEntry(player),
			BlitzMerchantEntryType.Glam => new BlitzMerchantGlamEntry(player),
			BlitzMerchantEntryType.TwoMoreChoices => new BlitzMerchantTwoMoreChoicesEntry(player),
			BlitzMerchantEntryType.CardRewardOfEachRarity => new BlitzMerchantCardRewardOfEachRarityEntry(player),
			BlitzMerchantEntryType.UpgradedRareCardReward => new BlitzMerchantUpgradedRareCardRewardEntry(player),
			BlitzMerchantEntryType.TwoRelics => new BlitzMerchantTwoRelicsEntry(player),
			BlitzMerchantEntryType.FullHealAndMaxHp => new BlitzMerchantFullHealAndMaxHpEntry(player),
			_ => throw new InvalidOperationException($"Unexpected entry type: {merchantEntryType}"),
		};
	}

	/// <summary>
	/// Used by cursed entries.
	/// </summary>
	public async Task GiveCurse()
	{
		IEnumerable<CardModel> items = from c in ModelDb.CardPool<CurseCardPool>().GetUnlockedCards(_player.UnlockState, _player.RunState.CardMultiplayerConstraint)
			where c.CanBeGeneratedByModifiers && c.IsAllowedInRun(_player.RunState)
			orderby c.Id
			select c;
		CardModel item = _player.PlayerRng.Shops.NextItem(items);
		await CardPileCmd.AddCursesToDeck(new _003C_003Ez__ReadOnlySingleElementList<CardModel>(item), _player);
	}

	public virtual BlitzMerchantHistoryEntry GenerateHistoryEntry(bool wasPicked)
	{
		return new BlitzMerchantHistoryEntry(MerchantEntryType, wasPicked);
	}

	public static LocString GetTitleForHistory(BlitzMerchantHistoryEntry entry)
	{
		LocString locString = new LocString("merchant_room", "MERCHANT.blitz." + StringHelper.SnakeCase(entry.Type.ToString()) + "_entry.title");
		switch (entry.Type)
		{
		case BlitzMerchantEntryType.CardUpgrade:
		case BlitzMerchantEntryType.UpgradedCardReward:
		case BlitzMerchantEntryType.RandomUpgrades:
		case BlitzMerchantEntryType.CardTransform:
		case BlitzMerchantEntryType.CardRemoval:
		case BlitzMerchantEntryType.CardRewards:
		case BlitzMerchantEntryType.ColorlessCardReward:
		case BlitzMerchantEntryType.Potions:
		case BlitzMerchantEntryType.EnchantedCardReward:
		case BlitzMerchantEntryType.RareCardReward:
		case BlitzMerchantEntryType.UpgradedRareCard:
		case BlitzMerchantEntryType.Relic:
		case BlitzMerchantEntryType.TwoMoreChoices:
		case BlitzMerchantEntryType.CardRewardOfEachRarity:
		case BlitzMerchantEntryType.UpgradedRareCardReward:
		case BlitzMerchantEntryType.TwoRelics:
		case BlitzMerchantEntryType.FullHealAndMaxHp:
			return locString;
		case BlitzMerchantEntryType.Glam:
			locString.Add("Enchantment", ModelDb.Enchantment<Glam>().Title);
			return locString;
		case BlitzMerchantEntryType.PrismaticCardReward:
			if (entry.modelId != null)
			{
				locString.Add("Character", ModelDb.GetById<CharacterModel>(entry.modelId).ShortTitle);
			}
			return locString;
		case BlitzMerchantEntryType.CardEnchant:
			if (entry.modelId != null)
			{
				locString.Add("Enchantment", ModelDb.GetById<EnchantmentModel>(entry.modelId).Title);
			}
			return locString;
		default:
			throw new InvalidOperationException($"Couldn't generate title for blitz merchant entry {entry.Type}!");
		}
	}
}
