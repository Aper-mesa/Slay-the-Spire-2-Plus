using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Merchant;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models.Encounters;
using MegaCrit.Sts2.Core.Models.Potions;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Nodes.Events.Custom;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Runs.History;

namespace MegaCrit.Sts2.Core.Models.Events;

public sealed class FakeMerchant : EventModel
{
	public const int relicCost = 50;

	/// <summary>
	/// Note: don't add <see cref="T:MegaCrit.Sts2.Core.Models.Relics.FakeMerchantsRug" /> to this list, it's a combat reward, not a relic you can buy.
	/// </summary>
	private static readonly RelicModel[] _inventoryRelics = new RelicModel[9]
	{
		ModelDb.Relic<FakeAnchor>(),
		ModelDb.Relic<FakeBloodVial>(),
		ModelDb.Relic<FakeHappyFlower>(),
		ModelDb.Relic<FakeLeesWaffle>(),
		ModelDb.Relic<FakeMango>(),
		ModelDb.Relic<FakeOrichalcum>(),
		ModelDb.Relic<FakeSneckoEye>(),
		ModelDb.Relic<FakeStrikeDummy>(),
		ModelDb.Relic<FakeVenerableTeaSet>()
	};

	private static MerchantDialogueSet? _dialogue;

	private MerchantInventory? _inventory;

	private bool _startedFight;

	public static MerchantDialogueSet Dialogue
	{
		get
		{
			if (_dialogue != null)
			{
				return _dialogue;
			}
			LocTable table = LocManager.Instance.GetTable("events");
			string keyPrefix = StringHelper.Slugify("FakeMerchant") + ".talk.";
			IReadOnlyList<LocString> locStringsWithPrefix = table.GetLocStringsWithPrefix(keyPrefix);
			_dialogue = MerchantDialogueSet.CreateFromLocStrings(locStringsWithPrefix);
			return _dialogue;
		}
	}

	public override EventLayoutType LayoutType => EventLayoutType.Custom;

	public override bool IsShared => true;

	public MerchantInventory Inventory
	{
		get
		{
			return _inventory;
		}
		private set
		{
			AssertMutable();
			_inventory = value;
		}
	}

	public bool StartedFight
	{
		get
		{
			return _startedFight;
		}
		private set
		{
			AssertMutable();
			_startedFight = value;
		}
	}

	public override IEnumerable<LocString> GameInfoOptions => Array.Empty<LocString>();

	protected override IReadOnlyList<EventOption> GenerateInitialOptions()
	{
		return Array.Empty<EventOption>();
	}

	public override bool IsAllowed(IRunState runState)
	{
		if (runState.CurrentActIndex < 1)
		{
			return false;
		}
		if (runState.Players.Count > 1)
		{
			return false;
		}
		return runState.Players.All((Player player) => player.Gold >= 100 || player.Potions.Any((PotionModel potion) => potion is FoulPotion));
	}

	protected override Task BeforeEventStarted(bool isPreFinished)
	{
		List<RelicModel> list = _inventoryRelics.ToList().UnstableShuffle(Rng).Take(6)
			.ToList();
		List<MerchantRelicEntry> list2 = new List<MerchantRelicEntry>();
		foreach (RelicModel item2 in list)
		{
			MerchantRelicEntry item = new MerchantRelicEntry(item2.ToMutable(), Owner);
			list2.Add(item);
		}
		Inventory = new MerchantInventory(Owner, new List<MerchantCardEntry>(), new List<MerchantCardEntry>(), list2, new List<MerchantPotionEntry>(), null, new List<BlitzMerchantEntry>());
		Inventory.Setup();
		RunManager.Instance.MerchantSynchronizer.SetMerchantInventory(Owner, Inventory);
		return Task.CompletedTask;
	}

	protected override void OnEventFinished()
	{
		if (StartedFight)
		{
			return;
		}
		PlayerMapPointHistoryEntry playerMapPointHistoryEntry = Owner.RunState.CurrentMapPointHistoryEntry?.GetEntry(Owner.NetId);
		if (playerMapPointHistoryEntry == null)
		{
			return;
		}
		foreach (MerchantRelicEntry relicEntry in Inventory.RelicEntries)
		{
			if (relicEntry.IsStocked)
			{
				playerMapPointHistoryEntry.RelicChoices.Add(new ModelChoiceHistoryEntry(relicEntry.Model.Id, wasPicked: false));
			}
		}
	}

	public async Task FoulPotionThrown(FoulPotion potion)
	{
		if (Node is NFakeMerchant nFakeMerchant)
		{
			if (nFakeMerchant.Inventory.IsOpen)
			{
				nFakeMerchant.Inventory.Close();
			}
			await nFakeMerchant.FoulPotionThrown();
		}
		StartedFight = true;
		List<Reward> list = new List<Reward>();
		list.Add(new RelicReward(ModelDb.Relic<FakeMerchantsRug>().ToMutable(), Owner));
		foreach (MerchantRelicEntry relicEntry in Inventory.RelicEntries)
		{
			if (relicEntry.IsStocked)
			{
				list.Add(new RelicReward(relicEntry.Model, Owner));
			}
		}
		EnterCombatWithoutExitingEvent<FakeMerchantEventEncounter>(list, shouldResumeAfterCombat: false);
	}
}
