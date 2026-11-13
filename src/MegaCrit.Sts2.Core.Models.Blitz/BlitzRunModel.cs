using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models.Enchantments;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Runs.History;

namespace MegaCrit.Sts2.Core.Models.Blitz;

public sealed class BlitzRunModel : RunModel
{
	public static List<(EnchantmentModel, int)> shopEnchantmentOptions;

	public static async Task GiveNeowRewards(Player player)
	{
		CardCreationOptions options = new CardCreationOptions(new _003C_003Ez__ReadOnlySingleElementList<CardPoolModel>(player.Character.CardPool), CardCreationSource.Other, CardRarityOddsType.BlitzNeow).WithFlags(CardCreationFlags.NoUpgrades);
		List<CardCreationResult> options2 = (from r in CardFactory.CreateForReward(player, 10, options)
			orderby r.Card.Rarity
			select r).ToList();
		IEnumerable<CardModel> selectedCards = await CardSelectCmd.FromSimpleGridForRewards(prefs: new CardSelectorPrefs(new LocString("ascension", "BLITZ.NEOW.selectionScreenPrompt"), 3), context: new BlockingPlayerChoiceContext(), cards: options2, player: player);
		foreach (CardModel item in selectedCards)
		{
			CardCmd.PreviewCardPileAdd(await CardPileCmd.Add(item, PileType.Deck));
		}
		foreach (CardCreationResult item2 in options2)
		{
			if (!selectedCards.Contains(item2.Card))
			{
				player.RunState.CurrentMapPointHistoryEntry.GetEntry(player.NetId).CardChoices.Add(new CardChoiceHistoryEntry(item2.Card, wasPicked: false));
			}
		}
	}

	public override bool TryModifyCardRewardOptionsLate(Player player, List<CardCreationResult> cardRewards, CardCreationOptions options)
	{
		IRunState runState = player.RunState;
		if (runState.CurrentActIndex == 0)
		{
			return false;
		}
		if (!options.Flags.HasFlag(CardCreationFlags.IsFromCombat))
		{
			return false;
		}
		AbstractRoom currentRoom = runState.CurrentRoom;
		if (currentRoom != null && currentRoom.RoomType == RoomType.Boss)
		{
			return false;
		}
		List<CardCreationResult> list = new List<CardCreationResult>();
		if (runState.CurrentActIndex == 1)
		{
			int num = 1;
			List<CardCreationResult> list2 = new List<CardCreationResult>(num);
			CollectionsMarshal.SetCount(list2, num);
			Span<CardCreationResult> span = CollectionsMarshal.AsSpan(list2);
			int index = 0;
			span[index] = player.PlayerRng.Rewards.NextItem(cardRewards);
			list = list2;
		}
		else if (runState.CurrentActIndex == 2)
		{
			list = cardRewards.UnstableShuffle(player.PlayerRng.Rewards).Take(2).ToList();
		}
		foreach (CardCreationResult item in list)
		{
			CardModel card = item.Card;
			if (card.IsUpgradable)
			{
				CardModel card2 = runState.CloneCard(card);
				CardCmd.Upgrade(card2);
				item.ModifyCard(card2);
			}
		}
		return true;
	}

	public override bool TryReplaceDefaultRoomRewards(Player player, List<Reward> rewards, AbstractRoom room)
	{
		if (room.RoomType == RoomType.Monster)
		{
			rewards.Add(new CardReward(GetCardRewardOptions(player, CardRarity.Common), 3, player));
			if (room is CombatRoom combatRoom && !combatRoom.Encounter.IsWeak)
			{
				rewards.Add(new PotionReward(player));
			}
		}
		else if (room.RoomType == RoomType.Elite)
		{
			rewards.Add(new CardReward(GetCardRewardOptions(player, CardRarity.Uncommon), 3, player));
			rewards.Add(new RelicReward(player));
			rewards.Add(new RelicReward(player));
		}
		else
		{
			if (room.RoomType != RoomType.Boss || RunState.CurrentActIndex >= 2)
			{
				return false;
			}
			rewards.Add(new CardReward(GetCardRewardOptions(player, CardRarity.Rare), 3, player));
			rewards.Add(new PotionReward(player));
		}
		return true;
	}

	/// <summary>
	/// Card rewards in blitz are a fixed rarity, and are upgraded by
	/// <see cref="M:MegaCrit.Sts2.Core.Models.Blitz.BlitzRunModel.TryModifyCardRewardOptionsLate(MegaCrit.Sts2.Core.Entities.Players.Player,System.Collections.Generic.List{MegaCrit.Sts2.Core.Entities.Cards.CardCreationResult},MegaCrit.Sts2.Core.Runs.CardCreationOptions)" /> rather than by an upgrade roll.
	/// <see cref="F:MegaCrit.Sts2.Core.Runs.CardCreationFlags.IsFromCombat" /> is what that method upgrades on, so it marks these rewards as
	/// the ones the fight itself handed out.
	/// </summary>
	private static CardCreationOptions GetCardRewardOptions(Player player, CardRarity rarity)
	{
		return CardCreationOptions.ForNonCombatWithUniformOdds(new _003C_003Ez__ReadOnlySingleElementList<CardPoolModel>(player.Character.CardPool), (CardModel c) => c.Rarity == rarity).WithFlags(CardCreationFlags.NoUpgradeRoll | CardCreationFlags.IsFromCombat);
	}

	static BlitzRunModel()
	{
		int num = 6;
		List<(EnchantmentModel, int)> list = new List<(EnchantmentModel, int)>(num);
		CollectionsMarshal.SetCount(list, num);
		Span<(EnchantmentModel, int)> span = CollectionsMarshal.AsSpan(list);
		int num2 = 0;
		span[num2] = (ModelDb.Enchantment<Sharp>(), 2);
		num2++;
		span[num2] = (ModelDb.Enchantment<Nimble>(), 3);
		num2++;
		span[num2] = (ModelDb.Enchantment<Adroit>(), 3);
		num2++;
		span[num2] = (ModelDb.Enchantment<Steady>(), 1);
		num2++;
		span[num2] = (ModelDb.Enchantment<Swift>(), 2);
		num2++;
		span[num2] = (ModelDb.Enchantment<Vigorous>(), 5);
		shopEnchantmentOptions = list;
	}
}
