using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Debug;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Exceptions;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Multiplayer.Game;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Screens.CardSelection;
using MegaCrit.Sts2.Core.Nodes.Screens.Overlays;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Saves;
using MegaCrit.Sts2.Core.TestSupport;

namespace MegaCrit.Sts2.Core.Commands;

public static class CardSelectCmd
{
	private static bool ShouldSelectLocalCard(Player player)
	{
		if (LocalContext.IsMe(player))
		{
			return RunManager.Instance.NetService.Type != NetGameType.Replay;
		}
		return false;
	}

	private static void ReportSoftlock()
	{
		string text = "A selection screen was about to be shown with 0 options. Returning empty to prevent softlock.";
		Log.Error(text);
		SentryService.CaptureException(new SoftlockException(text));
	}

	public static async Task<CardModel?> FromChooseACardScreen(PlayerChoiceContext context, IReadOnlyList<CardModel> cards, Player player, bool canSkip = false)
	{
		if (cards.Count > 3)
		{
			throw new ArgumentException("Only works with less than 3 cards", "cards");
		}
		if (cards.Count == 0)
		{
			ReportSoftlock();
			return null;
		}
		UndoEndTurnIfNecessary(player);
		ICardSelector selector = RunManager.Instance.CardSelectorStack.GetSelector(player);
		CardModel result;
		if (selector != null)
		{
			result = (await selector.GetSelectedCards(cards, 0, 1)).FirstOrDefault();
		}
		else
		{
			uint choiceId = RunManager.Instance.PlayerChoiceSynchronizer.ReserveChoiceId(player);
			await context.SignalPlayerChoiceBegun(player, PlayerChoiceOptions.None);
			if (ShouldSelectLocalCard(player))
			{
				ICardSelector localSelector = RunManager.Instance.CardSelectorStack.LocalSelector;
				if (localSelector != null)
				{
					result = (await localSelector.GetSelectedCards(cards, 0, 1)).FirstOrDefault();
				}
				else
				{
					NPlayerHand.Instance?.CancelAllCardPlay();
					NChooseACardSelectionScreen nChooseACardSelectionScreen = NChooseACardSelectionScreen.ShowScreen(cards, canSkip);
					if (LocalContext.IsMe(player))
					{
						foreach (CardModel card in cards)
						{
							SaveManager.Instance.MarkCardAsSeen(card);
						}
					}
					result = (await nChooseACardSelectionScreen.CardsSelected()).FirstOrDefault();
					int value = cards.IndexOf(result);
					PlayerChoiceResult result2 = PlayerChoiceResult.FromIndex(value);
					RunManager.Instance.PlayerChoiceSynchronizer.SyncLocalChoice(player, choiceId, result2);
				}
			}
			else
			{
				int num = (await RunManager.Instance.PlayerChoiceSynchronizer.WaitForRemoteChoice(player, choiceId)).AsIndex();
				result = ((num < 0) ? null : cards[num]);
			}
			await context.SignalPlayerChoiceEnded();
		}
		LogChoice(player, new _003C_003Ez__ReadOnlySingleElementList<CardModel>(result));
		return result;
	}

	public static async Task<IEnumerable<CardModel>> FromSimpleGridForRewards(PlayerChoiceContext context, List<CardCreationResult> cards, Player player, CardSelectorPrefs prefs)
	{
		if (CombatManager.Instance.IsEnding)
		{
			return Array.Empty<CardModel>();
		}
		UndoEndTurnIfNecessary(player);
		if (cards.Count == 0)
		{
			ReportSoftlock();
			return Array.Empty<CardModel>();
		}
		uint? choiceId = null;
		ICardSelector selector = RunManager.Instance.CardSelectorStack.GetSelector(player);
		if (selector == null)
		{
			choiceId = RunManager.Instance.PlayerChoiceSynchronizer.ReserveChoiceId(player);
			await context.SignalPlayerChoiceBegun(player, PlayerChoiceOptions.None);
		}
		List<CardModel> result;
		if (!prefs.RequireManualConfirmation && cards.Count <= prefs.MinSelect)
		{
			result = cards.Select((CardCreationResult c) => c.Card).ToList();
		}
		else if (selector != null)
		{
			IEnumerable<CardModel> options = cards.Select((CardCreationResult c) => c.Card);
			result = (await selector.GetSelectedCards(options, prefs.MinSelect, prefs.MaxSelect)).ToList();
		}
		else if (ShouldSelectLocalCard(player))
		{
			ICardSelector localSelector = RunManager.Instance.CardSelectorStack.LocalSelector;
			if (localSelector != null)
			{
				IEnumerable<CardModel> options2 = cards.Select((CardCreationResult c) => c.Card);
				result = (await localSelector.GetSelectedCards(options2, prefs.MinSelect, prefs.MaxSelect)).ToList();
			}
			else
			{
				NSimpleCardSelectScreen nSimpleCardSelectScreen = NSimpleCardSelectScreen.Create(cards, prefs);
				NOverlayStack.Instance.Push(nSimpleCardSelectScreen);
				result = (await nSimpleCardSelectScreen.CardsSelected()).ToList();
				List<int> indexes = result.Select((CardModel c) => cards.FindIndex((CardCreationResult r) => r.Card == c)).ToList();
				RunManager.Instance.PlayerChoiceSynchronizer.SyncLocalChoice(player, choiceId.Value, PlayerChoiceResult.FromIndexes(indexes));
			}
		}
		else
		{
			result = (from i in (await RunManager.Instance.PlayerChoiceSynchronizer.WaitForRemoteChoice(player, choiceId.Value)).AsIndexes()
				select cards[i].Card).ToList();
		}
		if (choiceId.HasValue)
		{
			await context.SignalPlayerChoiceEnded();
		}
		LogChoice(player, result);
		return result;
	}

	public static async Task<IEnumerable<CardModel>> FromSimpleGrid(PlayerChoiceContext context, IReadOnlyList<CardModel> cardsIn, Player player, CardSelectorPrefs prefs)
	{
		if (CombatManager.Instance.IsEnding)
		{
			return Array.Empty<CardModel>();
		}
		UndoEndTurnIfNecessary(player);
		List<CardModel> cards = cardsIn.ToList();
		uint? choiceId = null;
		ICardSelector selector = RunManager.Instance.CardSelectorStack.GetSelector(player);
		if (selector == null)
		{
			choiceId = RunManager.Instance.PlayerChoiceSynchronizer.ReserveChoiceId(player);
			await context.SignalPlayerChoiceBegun(player, PlayerChoiceOptions.None);
		}
		List<CardModel> result;
		if (cards.Count == 0)
		{
			result = new List<CardModel>();
		}
		else if (!prefs.RequireManualConfirmation && cards.Count <= prefs.MinSelect)
		{
			result = cards.ToList();
		}
		else if (selector != null)
		{
			result = (await selector.GetSelectedCards(cards, prefs.MinSelect, prefs.MaxSelect)).ToList();
		}
		else if (ShouldSelectLocalCard(player))
		{
			ICardSelector localSelector = RunManager.Instance.CardSelectorStack.LocalSelector;
			if (localSelector != null)
			{
				result = (await localSelector.GetSelectedCards(cards, prefs.MinSelect, prefs.MaxSelect)).ToList();
			}
			else
			{
				NPlayerHand.Instance?.CancelAllCardPlay();
				NSimpleCardSelectScreen nSimpleCardSelectScreen = NSimpleCardSelectScreen.Create(cards, prefs);
				NOverlayStack.Instance.Push(nSimpleCardSelectScreen);
				result = (await nSimpleCardSelectScreen.CardsSelected()).ToList();
				List<int> indexes = result.Select((CardModel c) => cards.IndexOf(c)).ToList();
				RunManager.Instance.PlayerChoiceSynchronizer.SyncLocalChoice(player, choiceId.Value, PlayerChoiceResult.FromIndexes(indexes));
			}
		}
		else
		{
			result = (from i in (await RunManager.Instance.PlayerChoiceSynchronizer.WaitForRemoteChoice(player, choiceId.Value)).AsIndexes()
				select cards[i]).ToList();
		}
		if (choiceId.HasValue)
		{
			await context.SignalPlayerChoiceEnded();
		}
		LogChoice(player, result);
		return result;
	}

	public static async Task<IEnumerable<CardModel>> FromCombatPile(PlayerChoiceContext context, CardPile pile, Player player, CardSelectorPrefs prefs)
	{
		return await FromCombatPile(context, pile, player, prefs, (CardModel _) => true);
	}

	public static async Task<IEnumerable<CardModel>> FromCombatPile(PlayerChoiceContext context, CardPile pile, Player player, CardSelectorPrefs prefs, Func<CardModel, bool>? filter)
	{
		if (CombatManager.Instance.IsEnding)
		{
			return Array.Empty<CardModel>();
		}
		if (player.Creature.IsDead)
		{
			return Array.Empty<CardModel>();
		}
		if (!pile.IsCombatPile)
		{
			throw new InvalidOperationException("Cannot perform on a non combat pile");
		}
		UndoEndTurnIfNecessary(player);
		uint? choiceId = null;
		ICardSelector selector = RunManager.Instance.CardSelectorStack.GetSelector(player);
		if (selector == null)
		{
			choiceId = RunManager.Instance.PlayerChoiceSynchronizer.ReserveChoiceId(player);
			await context.SignalPlayerChoiceBegun(player, PlayerChoiceOptions.None);
		}
		IReadOnlyList<CardModel> readOnlyList;
		if (filter == null)
		{
			readOnlyList = pile.Cards;
		}
		else
		{
			IReadOnlyList<CardModel> readOnlyList2 = pile.Cards.Where(filter).ToList();
			readOnlyList = readOnlyList2;
		}
		IEnumerable<CardModel> enumerable = readOnlyList;
		int num = enumerable.Count();
		IEnumerable<CardModel> result;
		if (num == 0)
		{
			result = Array.Empty<CardModel>();
		}
		else if (!prefs.RequireManualConfirmation && num <= prefs.MinSelect)
		{
			result = enumerable;
		}
		else if (selector != null)
		{
			if (pile.Type == PileType.Draw)
			{
				enumerable = from x in enumerable
					orderby x.Rarity, x
					select x;
			}
			result = (await selector.GetSelectedCards(enumerable, prefs.MinSelect, prefs.MaxSelect)).ToList();
		}
		else if (ShouldSelectLocalCard(player))
		{
			ICardSelector localSelector = RunManager.Instance.CardSelectorStack.LocalSelector;
			if (localSelector != null)
			{
				if (pile.Type == PileType.Draw)
				{
					enumerable = from x in enumerable
						orderby x.Rarity, x
						select x;
				}
				result = (await localSelector.GetSelectedCards(enumerable, prefs.MinSelect, prefs.MaxSelect)).ToList();
			}
			else
			{
				NPlayerHand.Instance?.CancelAllCardPlay();
				NCombatPileCardSelectScreen nCombatPileCardSelectScreen = NCombatPileCardSelectScreen.Create(pile, prefs, filter);
				NOverlayStack.Instance.Push(nCombatPileCardSelectScreen);
				result = (await nCombatPileCardSelectScreen.CardsSelected()).ToList();
				RunManager.Instance.PlayerChoiceSynchronizer.SyncLocalChoice(player, choiceId.Value, PlayerChoiceResult.FromMutableCombatCards(result));
			}
		}
		else
		{
			result = (await RunManager.Instance.PlayerChoiceSynchronizer.WaitForRemoteChoice(player, choiceId.Value)).AsCombatCards();
		}
		if (choiceId.HasValue)
		{
			await context.SignalPlayerChoiceEnded();
		}
		LogChoice(player, result);
		return result;
	}

	public static async Task<IEnumerable<CardModel>> FromDeckForUpgrade(Player player, CardSelectorPrefs prefs)
	{
		List<CardModel> list = PileType.Deck.GetPile(player).Cards.Where((CardModel c) => c.IsUpgradable).ToList();
		if (list.Count == 0)
		{
			return Array.Empty<CardModel>();
		}
		ICardSelector selector = RunManager.Instance.CardSelectorStack.GetSelector(player);
		IEnumerable<CardModel> enumerable;
		if (list.Count <= prefs.MinSelect && !prefs.RequireManualConfirmation)
		{
			enumerable = list;
		}
		else if (selector != null)
		{
			enumerable = await selector.GetSelectedCards(list, prefs.MinSelect, prefs.MaxSelect);
		}
		else
		{
			uint choiceId = RunManager.Instance.PlayerChoiceSynchronizer.ReserveChoiceId(player);
			if (ShouldSelectLocalCard(player))
			{
				ICardSelector localSelector = RunManager.Instance.CardSelectorStack.LocalSelector;
				if (localSelector != null)
				{
					enumerable = await localSelector.GetSelectedCards(list, prefs.MinSelect, prefs.MaxSelect);
				}
				else
				{
					NDeckUpgradeSelectScreen nDeckUpgradeSelectScreen = NDeckUpgradeSelectScreen.ShowScreen(list, prefs, player.RunState);
					enumerable = await nDeckUpgradeSelectScreen.CardsSelected();
					RunManager.Instance.PlayerChoiceSynchronizer.SyncLocalChoice(player, choiceId, PlayerChoiceResult.FromMutableDeckCards(enumerable));
				}
			}
			else
			{
				enumerable = (await RunManager.Instance.PlayerChoiceSynchronizer.WaitForRemoteChoice(player, choiceId)).AsDeckCards();
			}
		}
		LogChoice(player, enumerable);
		return enumerable;
	}

	public static async Task<IEnumerable<CardModel>> FromDeckForTransformation(Player player, CardSelectorPrefs prefs, Func<CardModel, CardTransformation>? cardToTransformation = null)
	{
		List<CardModel> list = PileType.Deck.GetPile(player).Cards.Where((CardModel c) => c.Type != CardType.Quest && c.IsTransformable).ToList();
		if (list.Count == 0)
		{
			return Array.Empty<CardModel>();
		}
		ICardSelector selector = RunManager.Instance.CardSelectorStack.GetSelector(player);
		IEnumerable<CardModel> enumerable;
		if (list.Count <= prefs.MinSelect && !prefs.RequireManualConfirmation)
		{
			enumerable = list;
		}
		else if (selector != null)
		{
			enumerable = await selector.GetSelectedCards(list, prefs.MinSelect, prefs.MaxSelect);
		}
		else
		{
			uint choiceId = RunManager.Instance.PlayerChoiceSynchronizer.ReserveChoiceId(player);
			if (ShouldSelectLocalCard(player))
			{
				ICardSelector localSelector = RunManager.Instance.CardSelectorStack.LocalSelector;
				if (localSelector != null)
				{
					enumerable = await localSelector.GetSelectedCards(list, prefs.MinSelect, prefs.MaxSelect);
				}
				else
				{
					if (cardToTransformation == null)
					{
						cardToTransformation = (CardModel c) => new CardTransformation(c);
					}
					NDeckTransformSelectScreen nDeckTransformSelectScreen = NDeckTransformSelectScreen.ShowScreen(list, cardToTransformation, prefs);
					enumerable = await nDeckTransformSelectScreen.CardsSelected();
					RunManager.Instance.PlayerChoiceSynchronizer.SyncLocalChoice(player, choiceId, PlayerChoiceResult.FromMutableDeckCards(enumerable));
				}
			}
			else
			{
				enumerable = (await RunManager.Instance.PlayerChoiceSynchronizer.WaitForRemoteChoice(player, choiceId)).AsDeckCards();
			}
		}
		LogChoice(player, enumerable);
		return enumerable;
	}

	public static async Task<IEnumerable<CardModel>> FromDeckForEnchantment(Player player, EnchantmentModel enchantment, int amount, CardSelectorPrefs prefs)
	{
		return await FromDeckForEnchantment(player, enchantment, amount, null, prefs);
	}

	public static async Task<IEnumerable<CardModel>> FromDeckForEnchantment(Player player, EnchantmentModel enchantment, int amount, Func<CardModel?, bool>? additionalFilter, CardSelectorPrefs prefs)
	{
		IReadOnlyList<CardModel> cards = PileType.Deck.GetPile(player).Cards.Where((CardModel c) => enchantment.CanEnchant(c) && (additionalFilter?.Invoke(c) ?? true)).ToList();
		return await FromDeckForEnchantment(cards, enchantment, amount, prefs);
	}

	public static async Task<IEnumerable<CardModel>> FromDeckForEnchantment(IReadOnlyList<CardModel> cards, EnchantmentModel enchantment, int amount, CardSelectorPrefs prefs)
	{
		if (cards.Any((CardModel c) => c.Pile.Type != PileType.Deck || !enchantment.CanEnchant(c)))
		{
			throw new ArgumentException("All cards must be in the player's deck and enchantable.");
		}
		List<CardModel> list = new List<CardModel>();
		if (cards.Count > 0)
		{
			Player owner = cards[0].Owner;
			Dictionary<CardModel, int> indexMap = PileType.Deck.GetPile(owner).Cards.Select((CardModel card, int index) => new { card, index }).ToDictionary(x => x.card, x => x.index);
			list = cards.OrderBy((CardModel c) => indexMap[c]).ToList();
		}
		ICardSelector cardSelector = ((cards.Count > 0) ? RunManager.Instance.CardSelectorStack.GetSelector(cards[0].Owner) : null);
		IEnumerable<CardModel> enumerable;
		if (cards.Count <= prefs.MinSelect)
		{
			enumerable = cards;
		}
		else if (cardSelector != null)
		{
			enumerable = await cardSelector.GetSelectedCards(list, prefs.MinSelect, prefs.MaxSelect);
		}
		else
		{
			Player player = cards[0].Owner;
			if (player.Creature.IsDead)
			{
				return Array.Empty<CardModel>();
			}
			uint choiceId = RunManager.Instance.PlayerChoiceSynchronizer.ReserveChoiceId(player);
			if (ShouldSelectLocalCard(player))
			{
				ICardSelector localSelector = RunManager.Instance.CardSelectorStack.LocalSelector;
				if (localSelector != null)
				{
					enumerable = await localSelector.GetSelectedCards(list, prefs.MinSelect, prefs.MaxSelect);
				}
				else
				{
					NDeckEnchantSelectScreen nDeckEnchantSelectScreen = NDeckEnchantSelectScreen.ShowScreen(list, enchantment, amount, prefs);
					enumerable = await nDeckEnchantSelectScreen.CardsSelected();
					RunManager.Instance.PlayerChoiceSynchronizer.SyncLocalChoice(player, choiceId, PlayerChoiceResult.FromMutableDeckCards(enumerable));
				}
			}
			else
			{
				enumerable = (await RunManager.Instance.PlayerChoiceSynchronizer.WaitForRemoteChoice(player, choiceId)).AsDeckCards();
			}
		}
		if (cards.Count > 0)
		{
			LogChoice(cards[0].Owner, enumerable);
		}
		return enumerable;
	}

	public static Task<IEnumerable<CardModel>> FromDeckForRemoval(Player player, CardSelectorPrefs prefs, Func<CardModel, bool>? filter = null)
	{
		List<CardModel> deck = PileType.Deck.GetPile(player).Cards.ToList();
		return FromDeckGeneric(player, prefs, (CardModel c) => c.IsRemovable && (filter == null || filter(c)), (CardModel c) => (c.Type != CardType.Curse) ? deck.IndexOf(c) : (-999999999));
	}

	public static async Task<IEnumerable<CardModel>> FromDeckGeneric(Player player, CardSelectorPrefs prefs, Func<CardModel, bool>? filter = null, Func<CardModel, int>? sortingOrder = null)
	{
		List<CardModel> source = PileType.Deck.GetPile(player).Cards.ToList();
		List<CardModel> list = ((filter == null) ? source.ToList() : source.Where(filter).ToList());
		if (player.Creature.IsDead)
		{
			return Array.Empty<CardModel>();
		}
		if (sortingOrder != null)
		{
			list = list.OrderBy(sortingOrder).ToList();
		}
		if (list.Count == 0)
		{
			return Array.Empty<CardModel>();
		}
		ICardSelector selector = RunManager.Instance.CardSelectorStack.GetSelector(player);
		IEnumerable<CardModel> enumerable;
		if (!prefs.RequireManualConfirmation && list.Count <= prefs.MinSelect)
		{
			enumerable = list;
		}
		else if (selector != null)
		{
			enumerable = await selector.GetSelectedCards(list, prefs.MinSelect, prefs.MaxSelect);
		}
		else
		{
			uint choiceId = RunManager.Instance.PlayerChoiceSynchronizer.ReserveChoiceId(player);
			if (ShouldSelectLocalCard(player))
			{
				ICardSelector localSelector = RunManager.Instance.CardSelectorStack.LocalSelector;
				if (localSelector != null)
				{
					enumerable = await localSelector.GetSelectedCards(list, prefs.MinSelect, prefs.MaxSelect);
				}
				else
				{
					NDeckCardSelectScreen nDeckCardSelectScreen = NDeckCardSelectScreen.Create(list, prefs);
					NOverlayStack.Instance.Push(nDeckCardSelectScreen);
					enumerable = await nDeckCardSelectScreen.CardsSelected();
					RunManager.Instance.PlayerChoiceSynchronizer.SyncLocalChoice(player, choiceId, PlayerChoiceResult.FromMutableDeckCards(enumerable));
				}
			}
			else
			{
				enumerable = (await RunManager.Instance.PlayerChoiceSynchronizer.WaitForRemoteChoice(player, choiceId)).AsDeckCards();
			}
		}
		LogChoice(player, enumerable);
		return enumerable;
	}

	public static Task<IEnumerable<CardModel>> FromHand(PlayerChoiceContext context, Player player, CardSelectorPrefs prefs, Func<CardModel, bool>? filter, AbstractModel source)
    {
        return FromHandInternal(context, player, prefs, filter, source, preserveEndTurn: false);
    }

    // End-of-turn retain choices must not re-enable card play or withdraw multiplayer readiness.
    public static Task<IEnumerable<CardModel>> FromHandForRetain(PlayerChoiceContext context, Player player, CardSelectorPrefs prefs, AbstractModel source)
    {
        return FromHandInternal(context, player, prefs, c => !c.ShouldRetainThisTurn, source, preserveEndTurn: true);
    }

    private static async Task<IEnumerable<CardModel>> FromHandInternal(PlayerChoiceContext context, Player player, CardSelectorPrefs prefs, Func<CardModel, bool>? filter, AbstractModel source, bool preserveEndTurn)
	{
		if (CombatManager.Instance.IsOverOrEnding)
		{
			return Array.Empty<CardModel>();
		}
		if (player.Creature.IsDead)
		{
			return Array.Empty<CardModel>();
		}
		if (!preserveEndTurn) UndoEndTurnIfNecessary(player);
		uint? choiceId = null;
		ICardSelector selector = RunManager.Instance.CardSelectorStack.GetSelector(player);
		if (selector == null)
		{
			choiceId = RunManager.Instance.PlayerChoiceSynchronizer.ReserveChoiceId(player);
			await context.SignalPlayerChoiceBegun(player, PlayerChoiceOptions.CancelPlayCardActions);
		}
		List<CardModel> list = PileType.Hand.GetPile(player).Cards.Where(filter ?? ((Func<CardModel, bool>)((CardModel _) => true))).ToList();
		IEnumerable<CardModel> result;
		if (list.Count == 0)
		{
			result = list;
		}
		else if (!prefs.RequireManualConfirmation && list.Count <= prefs.MinSelect)
		{
			result = list;
		}
		else if (selector != null)
		{
			result = await selector.GetSelectedCards(list, prefs.MinSelect, prefs.MaxSelect);
		}
		else if (ShouldSelectLocalCard(player))
		{
			ICardSelector localSelector = RunManager.Instance.CardSelectorStack.LocalSelector;
			if (localSelector != null)
			{
				result = await localSelector.GetSelectedCards(list, prefs.MinSelect, prefs.MaxSelect);
			}
			else
			{
				result = await NCombatRoom.Instance.Ui.Hand.SelectCards(prefs, filter, source);
				RunManager.Instance.PlayerChoiceSynchronizer.SyncLocalChoice(player, choiceId.Value, PlayerChoiceResult.FromMutableCombatCards(result));
			}
		}
		else
		{
			result = (await RunManager.Instance.PlayerChoiceSynchronizer.WaitForRemoteChoice(player, choiceId.Value)).AsCombatCards();
		}
		if (choiceId.HasValue)
		{
			await context.SignalPlayerChoiceEnded();
		}
		LogChoice(player, result);
		return result;
	}

	public static async Task<IEnumerable<CardModel>> FromHandForDiscard(PlayerChoiceContext context, Player player, CardSelectorPrefs prefs, Func<CardModel, bool>? filter, AbstractModel source)
	{
		prefs.ShouldGlowGold = (CardModel c) =>
		{
			if (!c.IsSlyThisTurn)
			{
				return false;
			}
			UnplayableReason reason;
			AbstractModel preventer;
			return c.CanPlay(out reason, out preventer) || reason.HasResourceCostReason();
		};
		return await FromHand(context, player, prefs, filter, source);
	}

	public static async Task<CardModel?> FromHandForUpgrade(PlayerChoiceContext context, Player player, AbstractModel source)
	{
		if (CombatManager.Instance.IsOverOrEnding)
		{
			return null;
		}
		UndoEndTurnIfNecessary(player);
		uint? choiceId = null;
		ICardSelector selector = RunManager.Instance.CardSelectorStack.GetSelector(player);
		if (selector == null)
		{
			choiceId = RunManager.Instance.PlayerChoiceSynchronizer.ReserveChoiceId(player);
			await context.SignalPlayerChoiceBegun(player, PlayerChoiceOptions.CancelPlayCardActions);
		}
		List<CardModel> list = PileType.Hand.GetPile(player).Cards.Where((CardModel c) => c.IsUpgradable).ToList();
		CardModel result;
		if (list.Count <= 1)
		{
			result = list.FirstOrDefault();
		}
		else if (selector != null)
		{
			result = (await selector.GetSelectedCards(list, 1, 1)).FirstOrDefault();
		}
		else if (ShouldSelectLocalCard(player))
		{
			ICardSelector localSelector = RunManager.Instance.CardSelectorStack.LocalSelector;
			if (localSelector != null)
			{
				result = (await localSelector.GetSelectedCards(list, 1, 1)).FirstOrDefault();
			}
			else
			{
				result = (await NCombatRoom.Instance.Ui.Hand.SelectCards(new CardSelectorPrefs(new LocString("gameplay_ui", "CHOOSE_CARD_UPGRADE_HEADER"), 1), (CardModel c) => c.IsUpgradable, source, NPlayerHand.Mode.UpgradeSelect)).FirstOrDefault();
				RunManager.Instance.PlayerChoiceSynchronizer.SyncLocalChoice(player, choiceId.Value, PlayerChoiceResult.FromMutableCombatCard(result));
			}
		}
		else
		{
			result = (await RunManager.Instance.PlayerChoiceSynchronizer.WaitForRemoteChoice(player, choiceId.Value)).AsCombatCards().FirstOrDefault();
		}
		if (choiceId.HasValue)
		{
			await context.SignalPlayerChoiceEnded();
		}
		LogChoice(player, new _003C_003Ez__ReadOnlySingleElementList<CardModel>(result));
		return result;
	}

	public static async Task<IEnumerable<CardModel>> FromChooseABundleScreen(Player player, IReadOnlyList<IReadOnlyList<CardModel>> bundles, bool canSkip = false)
	{
		if (CombatManager.Instance.IsEnding)
		{
			return Array.Empty<CardModel>();
		}
		if (bundles.Count == 0)
		{
			ReportSoftlock();
			return Array.Empty<CardModel>();
		}
		uint choiceId = RunManager.Instance.PlayerChoiceSynchronizer.ReserveChoiceId(player);
		IReadOnlyList<CardModel> readOnlyList;
		if (TestMode.IsOn)
		{
			readOnlyList = bundles[0];
		}
		else if (ShouldSelectLocalCard(player))
		{
			NChooseABundleSelectionScreen nChooseABundleSelectionScreen = NChooseABundleSelectionScreen.ShowScreen(bundles, canSkip);
			readOnlyList = (await nChooseABundleSelectionScreen.CardsSelected()).FirstOrDefault() ?? Array.Empty<CardModel>();
			RunManager.Instance.PlayerChoiceSynchronizer.SyncLocalChoice(player, choiceId, PlayerChoiceResult.FromIndex(bundles.IndexOf(readOnlyList)));
		}
		else
		{
			int num = (await RunManager.Instance.PlayerChoiceSynchronizer.WaitForRemoteChoice(player, choiceId)).AsIndex();
			IReadOnlyList<CardModel> readOnlyList2;
			if (num >= 0)
			{
				readOnlyList2 = bundles[num];
			}
			else
			{
				IReadOnlyList<CardModel> readOnlyList3 = Array.Empty<CardModel>();
				readOnlyList2 = readOnlyList3;
			}
			readOnlyList = readOnlyList2;
		}
		LogChoice(player, readOnlyList);
		return readOnlyList;
	}

	private static void LogChoice(Player player, IEnumerable<CardModel?> cards)
	{
		string value = string.Join(",", from c in cards.OfType<CardModel>()
			select c.Id.Entry);
		Log.Info($"Player {player.NetId} chose cards [{value}]");
	}

	private static void UndoEndTurnIfNecessary(Player player)
	{
		if (CombatManager.Instance.IsPlayerReadyToEndTurn(player) && player.Creature.CombatState != null && player.Creature.CombatState.CurrentSide == CombatSide.Player)
		{
			CombatManager.Instance.TryUndoReadyToEndTurn(player);
		}
	}
}
