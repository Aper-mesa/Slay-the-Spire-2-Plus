using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Runs;

namespace MegaCrit.Sts2.Core.Models.Events;

public sealed class TheFutureOfPotions : EventModel
{
	private const string _choiceKey = "THE_FUTURE_OF_POTIONS.pages.INITIAL.options.POTION";

	private const string _potionKey = "Potion";

	private const string _rarityKey = "Rarity";

	private const string _typeKey = "Type";

	private Dictionary<PotionModel, CardType>? _cardTypes;

	private LocString ChoiceTitle => new LocString("events", "THE_FUTURE_OF_POTIONS.pages.INITIAL.options.POTION.title");

	private LocString ChoiceDescription => new LocString("events", "THE_FUTURE_OF_POTIONS.pages.INITIAL.options.POTION.description");

	private Dictionary<PotionModel, CardType> PotionToCardType
	{
		get
		{
			AssertMutable();
			if (_cardTypes == null)
			{
				_cardTypes = new Dictionary<PotionModel, CardType>();
				foreach (PotionModel potion in Owner.Potions)
				{
					int num = 3;
					List<CardType> list = new List<CardType>(num);
					CollectionsMarshal.SetCount(list, num);
					Span<CardType> span = CollectionsMarshal.AsSpan(list);
					int num2 = 0;
					span[num2] = CardType.Attack;
					num2++;
					span[num2] = CardType.Skill;
					num2++;
					span[num2] = CardType.Power;
					List<CardType> list2 = list;
					if (potion.Rarity == PotionRarity.Common || potion.Rarity == PotionRarity.Token)
					{
						list2.Remove(CardType.Power);
					}
					_cardTypes.Add(potion, Rng.NextItem(list2));
				}
			}
			return _cardTypes;
		}
	}

	public override IEnumerable<LocString> GameInfoOptions
	{
		get
		{
			List<LocString> list = base.GameInfoOptions.ToList();
			if (list.Count != 2)
			{
				throw new InvalidOperationException("TheFutureOfPotions must've changed loc format, please update its\nGameInfoOptions method.");
			}
			LocString locString = list.First((LocString o) => o.LocEntryKey.EndsWith(".title"));
			locString.Add("Rarity", "[rarity]");
			LocString locString2 = list.First((LocString o) => o.LocEntryKey.EndsWith(".description"));
			locString2.Add("Potion", "[potion]");
			locString2.Add("Rarity", "[same-rarity]");
			locString2.Add("Type", "[card of random type]");
			return list;
		}
	}

	public override bool IsAllowed(IRunState runState)
	{
		return runState.Players.All((Player p) => p.Potions.Count() >= 2);
	}

	protected override Task BeforeEventStarted(bool isPreFinished)
	{
		Owner.CanUseOrRemovePotions = false;
		return Task.CompletedTask;
	}

	protected override void OnEventFinished()
	{
		Owner.CanUseOrRemovePotions = true;
	}

	protected override IReadOnlyList<EventOption> GenerateInitialOptions()
	{
		List<EventOption> list = new List<EventOption>();
		List<PotionModel> list2 = Owner.Potions.ToList();
		int num = Mathf.Min(3, list2.Count);
		for (int i = 0; i < num; i++)
		{
			PotionModel potion = list2[i];
			LocString choiceTitle = ChoiceTitle;
			choiceTitle.Add("Rarity", potion.Rarity.ToLocString().GetFormattedText());
			LocString choiceDescription = ChoiceDescription;
			choiceDescription.Add("Potion", potion.Title.GetFormattedText());
			choiceDescription.Add("Rarity", GetCardRarity(potion).ToLocString().GetFormattedText());
			choiceDescription.Add("Type", PotionToCardType[potion].ToLocString().GetFormattedText());
			list.Add(new EventOption(this, async () =>
			{
				await Trade(potion);
			}, choiceTitle, choiceDescription, "THE_FUTURE_OF_POTIONS.pages.INITIAL.options.POTION", potion.HoverTips).ThatHasDynamicTitle());
		}
		return list;
	}

	private async Task Trade(PotionModel potion)
	{
		CardRarity targetRarity = GetCardRarity(potion);
		await PotionCmd.Discard(potion);
		CardCreationOptions options = CardCreationOptions.ForNonCombatWithUniformOdds(new _003C_003Ez__ReadOnlySingleElementList<CardPoolModel>(Owner.Character.CardPool), (CardModel c) => c.Rarity == targetRarity && c.Type == PotionToCardType[potion]).WithFlags(CardCreationFlags.NoRarityModification | CardCreationFlags.NoCardPoolModifications);
		CardReward reward = new CardReward(options, 3, Owner);
		reward.AfterGenerated += UpgradeCardsInReward;
		await RewardsCmd.OfferCustom(Owner, new List<Reward>(1) { reward });
		await Done();
		void UpgradeCardsInReward()
		{
			foreach (CardModel card in reward.Cards)
			{
				CardCmd.Upgrade(card);
			}
		}
	}

	private Task Done()
	{
		SetEventFinished(L10NLookup("THE_FUTURE_OF_POTIONS.pages.DONE.description"));
		return Task.CompletedTask;
	}

	private CardRarity GetCardRarity(PotionModel potion)
	{
		switch (potion.Rarity)
		{
		case PotionRarity.Rare:
		case PotionRarity.Event:
			return CardRarity.Rare;
		case PotionRarity.Uncommon:
			return CardRarity.Uncommon;
		case PotionRarity.Common:
		case PotionRarity.Token:
			return CardRarity.Common;
		default:
			throw new InvalidOperationException($"Potion {potion.Id.Entry} has invalid rarity {potion.Rarity}");
		}
	}
}
