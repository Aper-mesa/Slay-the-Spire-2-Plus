using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using MegaCrit.Sts2.Core.Audio.Debug;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.Ftue;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.Nodes.Vfx.Cards;
using MegaCrit.Sts2.Core.Random;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Saves;
using MegaCrit.Sts2.Core.Settings;
using MegaCrit.Sts2.Core.TestSupport;

namespace MegaCrit.Sts2.Core.Commands;

public static class CardPileCmd
{
	public static async Task RemoveFromDeck(CardModel card, bool showPreview = true)
	{
		await RemoveFromDeck(new _003C_003Ez__ReadOnlySingleElementList<CardModel>(card), showPreview);
	}

	public static async Task RemoveFromDeck(IReadOnlyList<CardModel> cards, bool showPreview = true)
	{
		foreach (CardModel card in cards)
		{
			if (card.Pile.Type != PileType.Deck)
			{
				throw new InvalidOperationException("You cannot remove a card that is not in the deck.");
			}
			card.Owner.RunState.CurrentMapPointHistoryEntry?.GetEntry(card.Owner.NetId).CardsRemoved.Add(card.ToSerializable());
			await Hook.BeforeCardRemoved(card.Owner.RunState, card);
			card.RemoveFromCurrentPile();
			if (showPreview && LocalContext.IsMine(card))
			{
				NCard cardNode = NCard.Create(card);
				if (cardNode != null)
				{
					NRun.Instance.GlobalUi.CardPreviewContainer.AddChildSafely(cardNode);
					cardNode.UpdateVisuals(PileType.None, CardPreviewMode.Normal);
					Tween tween = cardNode.CreateTween();
					tween.TweenProperty(cardNode, "scale", Vector2.One * 1f, 0.25).From(Vector2.Zero).SetEase(Tween.EaseType.Out)
						.SetTrans(Tween.TransitionType.Cubic);
					if (!TestMode.IsOn)
					{
						tween.TweenInterval(0.25);
						tween.TweenCallback(Callable.From(() =>
						{
							NCardRemoveVfx child = NCardRemoveVfx.Create(cardNode);
							NRun.Instance.GlobalUi.AboveTopBarVfxContainer.AddChildSafely(child);
						}));
						tween.TweenInterval(0.4000000059604645);
					}
					tween.TweenCallback(Callable.From(cardNode.QueueFreeSafely));
				}
			}
			card.RemoveFromState();
		}
	}

	public static async Task RemoveFromCombat(CardModel card, bool skipVisuals = false)
	{
		await RemoveFromCombat(new _003C_003Ez__ReadOnlySingleElementList<CardModel>(card), skipVisuals);
	}

	public static async Task RemoveFromCombat(IEnumerable<CardModel> cards, bool skipVisuals = false)
	{
		if (!cards.Any())
		{
			return;
		}
		ICombatState combatState = cards.First().CombatState;
		IRunState runState = cards.First().Owner.RunState;
		List<NCard> list = new List<NCard>();
		Dictionary<CardModel, CardPile> oldPiles = new Dictionary<CardModel, CardPile>();
		CardPile value;
		foreach (CardModel card in cards)
		{
			value = card.Pile;
			if (value == null || !value.IsCombatPile)
			{
				throw new InvalidOperationException("Card must be in a combat pile for it to be removed");
			}
			if ((card.Pile.Type != PileType.Play || card.Type != CardType.Power) && !skipVisuals)
			{
				NCard nCard = NCard.FindOnTable(card);
				if (nCard != null)
				{
					list.Add(nCard);
				}
			}
			oldPiles.Add(card, card.Pile);
			card.RemoveFromCurrentPile();
		}
		if (list.Count != 0)
		{
			NPlayerHand nPlayerHand = NCombatRoom.Instance?.Ui.Hand;
			NCardPlayQueue nCardPlayQueue = NCombatRoom.Instance?.Ui.PlayQueue;
			Tween tween = null;
			for (int i = 0; i < list.Count; i++)
			{
				NCard node = list[i];
				Vector2 globalPosition = node.GlobalPosition;
				CardModel model = node.Model;
				CardPile cardPile = oldPiles[model];
				bool isInPlayQueue = nCardPlayQueue?.IsAncestorOf(node) ?? false;
				if (isInPlayQueue)
				{
					nCardPlayQueue.RemoveCardFromQueueForCancellation(node);
				}
				if (nPlayerHand != null && nPlayerHand.IsAncestorOf(node))
				{
					nPlayerHand.Remove(model);
				}
				else
				{
					node.GetParent()?.RemoveChildSafely(node);
				}
				NCombatRoom.Instance?.Ui.AddChildSafely(node);
				node.GlobalPosition = globalPosition;
				if (tween == null)
				{
					tween = NCombatRoom.Instance?.CreateTween();
					tween?.SetParallel();
				}
				model.Pile?.InvokeCardAddFinished();
				if (cardPile.Type != PileType.Hand && cardPile.Type != PileType.Play)
				{
					AppendPileLerpTween(tween, node, PileType.Play, cardPile.Type);
				}
				tween?.Chain().TweenCallback(Callable.From(() =>
				{
					NCombatRoom instance = NCombatRoom.Instance;
					NCardExhaustVfx nCardExhaustVfx = ((instance != null) ? NCardExhaustVfx.Create(node) : null);
					if (nCardExhaustVfx != null)
					{
						instance?.Ui.AddChildSafely(nCardExhaustVfx);
						NDebugAudioManager.Instance?.Play("card_exhaust.mp3");
						TaskHelper.RunSafely(nCardExhaustVfx.PlayAnimation());
					}
					else if (!isInPlayQueue)
					{
						node.QueueFreeSafely();
					}
				}));
			}
			if (tween != null)
			{
				tween.Play();
				if (NCombatRoom.Instance != null)
				{
					await tween.AwaitFinished(NCombatRoom.Instance);
				}
			}
		}
		foreach (KeyValuePair<CardModel, CardPile> item in oldPiles)
		{
			item.Deconstruct(out var key, out value);
			CardModel oldCard = key;
			CardPile cardPile2 = value;
			await Hook.AfterCardChangedPiles(runState, combatState, oldCard, cardPile2.Type, null);
			oldCard.RemoveFromState();
		}
	}

	public static async Task GiveToAnotherPlayer(CardModel card, Player player, PileType pileType, CardPilePosition position = CardPilePosition.Bottom, AbstractModel? clonedBy = null)
	{
		if ((pileType.IsCombatPile() && CombatManager.Instance.IsOverOrEnding) || player.Creature.IsDead)
		{
			return;
		}
		NCard cardNode = NCard.FindOnTable(card);
		PileType? oldPileType = card.Pile?.Type;
		card.RemoveFromCurrentPile(silent: true);
		card.GiveToAnotherPlayer(player);
		bool islocalPlayerTheReceivingPlayer = LocalContext.IsMine(card);
		await Add(new _003C_003Ez__ReadOnlySingleElementList<CardModel>(card), pileType.GetPile(player), position, clonedBy, skipVisuals: true, isChangingOwners: true);
		if (islocalPlayerTheReceivingPlayer && card.Pile?.Type == PileType.Hand)
		{
			// [apermesa] skipVisuals skips node creation, and the card has no node on the receiver's table.
			NCard? receivedCardNode = NCard.Create(card);
			if (receivedCardNode != null)
			{
				NPlayerHand.Instance?.Add(receivedCardNode);
			}
		}
		if (cardNode == null || !cardNode.IsValid())
		{
			return;
		}
		Node vfxContainer = card.Owner.Creature.GetVfxContainer();
		cardNode.Reparent(vfxContainer);
		if (islocalPlayerTheReceivingPlayer)
		{
			Tween tweenForCardsChangingPiles = GetTweenForCardsChangingPiles(new _003C_003Ez__ReadOnlySingleElementList<(NCard, PileType?)>((cardNode, oldPileType)));
			if (tweenForCardsChangingPiles != null)
			{
				tweenForCardsChangingPiles.Play();
				await tweenForCardsChangingPiles.AwaitFinished(NCombatRoom.Instance);
			}
		}
		else
		{
			NCardFlyVfx child = NCardFlyVfx.Create(cardNode, player.Creature, card.Owner.Character.TrailPath);
			vfxContainer?.AddChildSafely(child);
		}
	}

	public static async Task<CardPileAddResult> AddGeneratedCardToCombat(CardModel card, PileType newPileType, Player? creator, CardPilePosition position = CardPilePosition.Bottom)
	{
		return (await AddGeneratedCardsToCombat(new _003C_003Ez__ReadOnlySingleElementList<CardModel>(card), newPileType, creator, position))[0];
	}

	public static async Task<IReadOnlyList<CardPileAddResult>> AddGeneratedCardsToCombat(IEnumerable<CardModel> cards, PileType newPileType, Player? creator, CardPilePosition position = CardPilePosition.Bottom)
	{
		List<CardModel> list = cards.ToList();
		if (list.Count == 0)
		{
			return Array.Empty<CardPileAddResult>();
		}
		if (!CombatManager.Instance.IsInProgress)
		{
			return Array.Empty<CardPileAddResult>();
		}
		if (list.Any((CardModel c) => c.Pile != null))
		{
			throw new InvalidOperationException("You are not allowed to generate cards that already have a pile");
		}
		if (!newPileType.IsCombatPile())
		{
			throw new InvalidOperationException("You are not allowed to added generated cards to a non combat pile");
		}
		ICombatState combatState = list[0].Owner.Creature.CombatState;
		if (combatState == null)
		{
			return Array.Empty<CardPileAddResult>();
		}
		List<CardPileAddResult> results = new List<CardPileAddResult>();
		foreach (CardModel card in list)
		{
			CombatManager.Instance.History.CardGenerated(combatState, card, creator);
			List<CardPileAddResult> list2 = results;
			list2.Add(await Add(card, newPileType.GetPile(card.Owner), position));
			await Hook.AfterCardGeneratedForCombat(combatState, card, creator);
		}
		return results;
	}

	public static async Task<CardPileAddResult> Add(CardModel card, PileType newPileType, CardPilePosition position = CardPilePosition.Bottom, AbstractModel? clonedBy = null, bool skipVisuals = false)
	{
		if (card.Owner == null)
		{
			throw new InvalidOperationException($"Attempted to add card {card} to pile, but it has no owner!");
		}
		return await Add(card, newPileType.GetPile(card.Owner), position, clonedBy, skipVisuals);
	}

	public static async Task<CardPileAddResult> Add(CardModel card, CardPile newPile, CardPilePosition position = CardPilePosition.Bottom, AbstractModel? clonedBy = null, bool skipVisuals = false)
	{
		return (await Add(new _003C_003Ez__ReadOnlySingleElementList<CardModel>(card), newPile, position, clonedBy, skipVisuals))[0];
	}

	public static async Task<IReadOnlyList<CardPileAddResult>> Add(IEnumerable<CardModel> cards, PileType newPileType, CardPilePosition position = CardPilePosition.Bottom, AbstractModel? clonedBy = null, bool skipVisuals = false)
	{
		if (!cards.Any())
		{
			return Array.Empty<CardPileAddResult>();
		}
		return await Add(cards, newPileType.GetPile(cards.First().Owner), position, clonedBy, skipVisuals);
	}

	public static async Task<IReadOnlyList<CardPileAddResult>> Add(IEnumerable<CardModel> cards, CardPile newPile, CardPilePosition position = CardPilePosition.Bottom, AbstractModel? clonedBy = null, bool skipVisuals = false, bool isChangingOwners = false)
	{
		if (!cards.Any())
		{
			return Array.Empty<CardPileAddResult>();
		}
		if (newPile.IsCombatPile && CombatManager.Instance.IsEnding)
		{
			return cards.Select((CardModel c) => new CardPileAddResult
			{
				cardAdded = c,
				success = false
			}).ToList();
		}
		Player player = null;
		List<CardPileAddResult> results = new List<CardPileAddResult>();
		foreach (CardModel card3 in cards)
		{
			if (card3.Owner == null)
			{
				throw new InvalidOperationException(card3.Id.Entry + " has no owner.");
			}
			Creature creature = card3.Owner.Creature;
			if (card3.HasBeenRemovedFromState || creature.IsDead || (card3.IsInCombat && creature.CombatState == null))
			{
				CardPileAddResult item = new CardPileAddResult
				{
					success = false,
					cardAdded = card3,
					oldPile = card3.Pile,
					targetPile = newPile.Type,
					modifyingModels = null
				};
				results.Add(item);
				continue;
			}
			if (newPile.Type == PileType.Deck)
			{
				if (!card3.Owner.RunState.ContainsCard(card3))
				{
					if (card3.Owner.RunState is NullRunState)
					{
						throw new InvalidOperationException("Tried to add card " + card3.Id.Entry + " to deck for an owner with a NullRunState!");
					}
					throw new InvalidOperationException(card3.Id.Entry + " must be added to a RunState before adding it to your deck.");
				}
			}
			else if (card3.IsInCombat && creature.CombatState != null && !creature.CombatState.ContainsCard(card3))
			{
				throw new InvalidOperationException(card3.Id.Entry + " must be added to a CombatState before adding it to this pile.");
			}
			if (card3.UpgradePreviewType.IsPreview())
			{
				throw new InvalidOperationException("A card preview cannot be added to a pile.");
			}
			CardPileAddResult item2 = new CardPileAddResult
			{
				success = true,
				cardAdded = card3,
				oldPile = card3.Pile,
				targetPile = newPile.Type,
				modifyingModels = null
			};
			results.Add(item2);
			if (player == null)
			{
				player = card3.Owner;
			}
			if (player == card3.Owner)
			{
				continue;
			}
			throw new InvalidOperationException("Tried to add cards with different owners to the same pile!");
		}
		if (newPile.Type == PileType.Deck)
		{
			for (int i = 0; i < results.Count; i++)
			{
				CardPileAddResult result = results[i];
				IRunState runState = result.cardAdded.RunState;
				if (Hook.ShouldAddToDeck(runState, result.cardAdded, out AbstractModel preventer))
				{
					runState.CurrentMapPointHistoryEntry?.GetEntry(result.cardAdded.Owner.NetId).CardsGained.Add(result.cardAdded.ToSerializable());
					result.cardAdded.FloorAddedToDeck = runState.TotalFloor;
				}
				else
				{
					await preventer.AfterAddToDeckPrevented(result.cardAdded);
					result.success = false;
					results[i] = result;
				}
			}
		}
		if (newPile.IsCombatPile && !CombatManager.Instance.IsInProgress)
		{
			return results;
		}
		if (!results.Any((CardPileAddResult r) => r.success))
		{
			return results;
		}
		for (int i = 0; i < results.Count; i++)
		{
			CardPileAddResult value = results[i];
			if (!value.success)
			{
				continue;
			}
			CardPile oldPile = value.oldPile;
			CardModel card = value.cardAdded;
			CardPile cardPile = newPile;
			bool isFullHandAdd = cardPile.Type == PileType.Hand && cardPile.Cards.Count >= CardPile.MaxCardsInHand;
			if (isFullHandAdd)
			{
				cardPile = CardPile.Get(PileType.Discard, card.Owner);
			}
			CardModel card2 = card;
			if (oldPile != null)
			{
				card.RemoveFromCurrentPile(skipVisuals);
			}
			else if (cardPile.Type == PileType.Deck)
			{
				CardModel cardModel = Hook.ModifyCardBeingAddedToDeck(card.Owner.RunState, card, out List<AbstractModel> modifyingModels);
				card2 = cardModel;
				if (modifyingModels != null && modifyingModels.Count > 0)
				{
					value.cardAdded = cardModel;
					value.modifyingModels = modifyingModels;
					results[i] = value;
				}
			}
			cardPile.AddInternal(card2, position switch
			{
				CardPilePosition.Bottom => -1,
				CardPilePosition.Top => 0,
				CardPilePosition.Random => card.Owner.RunState.Rng.Shuffle.NextInt(cardPile.Cards.Count + 1),
				_ => throw new ArgumentOutOfRangeException("position", position, null),
			});
			if (oldPile == null && cardPile.IsCombatPile && !isChangingOwners)
			{
				await Hook.AfterCardEnteredCombat(card.CombatState, card);
			}
			if (isFullHandAdd && LocalContext.IsMe(card.Owner))
			{
				ThinkCmd.Play(new LocString("combat_messages", "HAND_FULL"), card.Owner.Creature, 2.0);
			}
		}
		if (!skipVisuals)
		{
			Tween item3 = GetTweenForCardsChangingPiles(results, fromSilentAdd: false).Item1;
			if (item3 != null)
			{
				item3.Play();
				if (await item3.AwaitFinished(NCombatRoom.Instance) != TweenOutcome.Finished)
				{
					return results;
				}
			}
		}
		foreach (CardPileAddResult item4 in results)
		{
			if (item4.success)
			{
				CardModel cardAdded = item4.cardAdded;
				if (item4.oldPile == null || item4.oldPile.Type != cardAdded.Pile?.Type)
				{
					await Hook.AfterCardChangedPiles(cardAdded.Owner.RunState, cardAdded.CombatState, cardAdded, item4.oldPile?.Type ?? PileType.None, clonedBy);
				}
			}
		}
		return results;
	}

	public static (Tween?, bool) GetTweenForCardsChangingPiles(IEnumerable<CardPileAddResult> results, bool fromSilentAdd)
	{
		if (TestMode.IsOn)
		{
			return (null, false);
		}
		List<NCard> list = new List<NCard>();
		List<CardModel> list2 = new List<CardModel>();
		foreach (CardPileAddResult result in results)
		{
			if (!result.success)
			{
				continue;
			}
			if (fromSilentAdd)
			{
				result.oldPile?.InvokeCardRemoved(result.cardAdded);
				result.oldPile?.InvokeCardRemoveFinished();
				result.oldPile?.InvokeContentsChanged();
			}
			CardModel cardAdded = result.cardAdded;
			PileType? pileType = cardAdded.Pile?.Type;
			PileType? pileType2 = result.oldPile?.Type;
			bool flag = LocalContext.IsMe(result.cardAdded.Owner);
			if (!flag && pileType != PileType.Play && pileType2 != PileType.Play)
			{
				continue;
			}
			NCard nCard = NCard.FindOnTable(cardAdded, pileType2);
			int num;
			if (result.targetPile == PileType.Hand)
			{
				CardPile? pile = result.cardAdded.Pile;
				num = ((pile != null && pile.Type == PileType.Discard) ? 1 : 0);
			}
			else
			{
				num = 0;
			}
			bool flag2 = (byte)num != 0;
			bool flag3 = nCard == null && pileType.HasValue && pileType.Value.IsCombatPile() && (flag2 || pileType2.HasValue || pileType == PileType.Hand);
			bool flag4 = nCard == null;
			bool flag5 = flag4;
			if (flag5)
			{
				bool flag6;
				switch (pileType2)
				{
				case PileType.Draw:
				case PileType.Discard:
				case PileType.Exhaust:
				case PileType.Deck:
					flag6 = true;
					break;
				default:
					flag6 = false;
					break;
				}
				flag5 = flag6;
			}
			bool flag7 = flag5;
			if (flag7)
			{
				bool flag6;
				switch (pileType)
				{
				case PileType.Draw:
				case PileType.Discard:
				case PileType.Deck:
					flag6 = true;
					break;
				default:
					flag6 = false;
					break;
				}
				flag7 = flag6;
			}
			if (flag7)
			{
				list2.Add(cardAdded);
			}
			else if (flag3)
			{
				nCard = CreateCardNodeAndUpdateVisuals(cardAdded, pileType2, pileType.Value, flag);
			}
			if (pileType.HasValue && (pileType2 != PileType.Play || pileType == PileType.Hand || cardAdded.IsDupe))
			{
				nCard?.UpdateVisuals(pileType.Value, CardPreviewMode.Normal);
			}
			if (nCard != null)
			{
				list.Add(nCard);
			}
		}
		IEnumerable<PileType?> second = list.Select((NCard c) => results.First((CardPileAddResult r) => r.cardAdded == c.Model).oldPile?.Type);
		Tween tweenForCardsChangingPiles = GetTweenForCardsChangingPiles(list.Zip(second));
		if (list2.Count != 0)
		{
			foreach (CardModel card in list2)
			{
				CardPile oldPile = results.First((CardPileAddResult r) => r.cardAdded == card).oldPile;
				CardPile targetPile = card.Pile;
				string trailPath = card.Owner.Character.TrailPath;
				Node vfxContainer = ((targetPile.Type != PileType.Deck) ? card.Owner.Creature.GetVfxContainer() : NRun.Instance.GlobalUi.TopBar.TrailContainer);
				if (tweenForCardsChangingPiles != null)
				{
					tweenForCardsChangingPiles.TweenCallback(Callable.From(() =>
					{
						NCardFlyShuffleVfx child2 = NCardFlyShuffleVfx.Create(oldPile, targetPile, trailPath);
						vfxContainer?.AddChildSafely(child2);
					}));
				}
				else
				{
					NCardFlyShuffleVfx child = NCardFlyShuffleVfx.Create(oldPile, targetPile, trailPath);
					vfxContainer?.AddChildSafely(child);
				}
			}
		}
		return (tweenForCardsChangingPiles, list2.Count > 0);
	}

	private static Tween? GetTweenForCardsChangingPiles(IEnumerable<(NCard, PileType?)> cards)
	{
		if (!cards.Any())
		{
			return null;
		}
		NPlayerHand handNode = NCombatRoom.Instance?.Ui.Hand;
		Tween tween = null;
		foreach (var card2 in cards)
		{
			NCard cardNode = card2.Item1;
			PileType? item = card2.Item2;
			CardModel card = cardNode.Model;
			PileType? pileType = card.Pile?.Type;
			if (pileType.HasValue)
			{
				PileType newPileType = pileType.GetValueOrDefault();
				MoveCardNodeToNewPileBeforeTween(cardNode, newPileType);
				bool flag = !LocalContext.IsMe(card.Owner);
				bool flag2 = flag;
				if (flag2)
				{
					PileType pileType2 = newPileType;
					bool flag3 = (((uint)(pileType2 - 1) <= 2u || pileType2 == PileType.Deck) ? true : false);
					flag2 = flag3;
				}
				if (flag2)
				{
					GetTween()?.Parallel().TweenProperty(cardNode, "position", cardNode.Position + Vector2.Down * 25f, (SaveManager.Instance.PrefsSave.FastMode == FastModeType.Fast) ? 0.2f : 0.3f);
					GetTween()?.Parallel().TweenProperty(cardNode, "modulate", StsColors.exhaustGray, (SaveManager.Instance.PrefsSave.FastMode == FastModeType.Fast) ? 0.2f : 0.3f);
					GetTween()?.Chain().TweenCallback(Callable.From(cardNode.QueueFreeSafely));
					continue;
				}
				switch (newPileType)
				{
				case PileType.Exhaust:
					card.Pile?.InvokeCardAddFinished();
					if (item.HasValue && item != PileType.Hand && item != PileType.Play)
					{
						AppendPileLerpTween(GetTween(), cardNode, PileType.Play, item);
						FastModeType fastMode = SaveManager.Instance.PrefsSave.FastMode;
						GetTween()?.Chain().TweenInterval(fastMode switch
						{
							FastModeType.Instant => 0.01f,
							FastModeType.Fast => 0.2f,
							_ => 0.5f,
						});
					}
					if (item == PileType.Hand)
					{
						GetTween()?.Chain().TweenCallback(Callable.From(() =>
						{
							NCardExhaustQuickVfx nCardExhaustQuickVfx = NCardExhaustQuickVfx.Create(cardNode);
							if (nCardExhaustQuickVfx != null)
							{
								NDebugAudioManager.Instance?.Play("card_exhaust.mp3");
								TaskHelper.RunSafely(nCardExhaustQuickVfx.PlayAnimation());
							}
							else
							{
								cardNode.QueueFreeSafely();
							}
						}));
						continue;
					}
					GetTween()?.Chain().TweenCallback(Callable.From(() =>
					{
						NCombatRoom instance = NCombatRoom.Instance;
						NCardExhaustVfx nCardExhaustVfx = ((instance != null) ? NCardExhaustVfx.Create(cardNode) : null);
						if (nCardExhaustVfx != null)
						{
							instance.Ui.AddChildSafely(nCardExhaustVfx);
							NDebugAudioManager.Instance?.Play("card_exhaust.mp3");
							TaskHelper.RunSafely(nCardExhaustVfx.PlayAnimation());
						}
						else
						{
							cardNode.QueueFreeSafely();
						}
					}));
					continue;
				case PileType.Hand:
					if (item.HasValue)
					{
						AppendPileLerpTween(GetTween(), cardNode, PileType.Hand, item);
						GetTween()?.Parallel().TweenCallback(Callable.From(() =>
						{
							handNode?.Add(cardNode);
						}));
					}
					else
					{
						GetTween()?.Chain().TweenCallback(Callable.From(() =>
						{
							handNode?.Add(cardNode);
						}));
					}
					continue;
				case PileType.Play:
					AppendPlayPileLerpTween(GetTween(), cardNode, item);
					continue;
				}
				string trailPath = card.Owner.Character.TrailPath;
				GetTween()?.TweenCallback(Callable.From(() =>
				{
					if (newPileType.IsCombatPile() && !CombatManager.Instance.IsInProgress)
					{
						cardNode.QueueFreeSafely();
					}
					else
					{
						Node node = ((newPileType != PileType.Deck) ? card.Owner.Creature.GetVfxContainer() : NRun.Instance?.GlobalUi.TopBar.TrailContainer);
						if (node == null)
						{
							cardNode.QueueFreeSafely();
						}
						else
						{
							cardNode.Reparent(node);
							NCardFlyVfx nCardFlyVfx = NCardFlyVfx.Create(cardNode, newPileType, isAddingToPile: true, trailPath);
							if (nCardFlyVfx == null)
							{
								cardNode.QueueFreeSafely();
							}
							else
							{
								node.AddChildSafely(nCardFlyVfx);
							}
						}
					}
				}));
			}
			else
			{
				GetTween()?.TweenCallback(Callable.From(cardNode.QueueFreeSafely));
			}
		}
		return tween;
		Tween? GetTween()
		{
			return tween ?? (tween = NCombatRoom.Instance?.CreateTween().SetParallel());
		}
	}

	public static async Task AddDuringManualCardPlay(CardModel card)
	{
		if (CombatManager.Instance.IsOverOrEnding)
		{
			return;
		}
		ICombatState combatState = card.Owner.Creature.CombatState;
		if (combatState == null || !combatState.ContainsCard(card))
		{
			throw new InvalidOperationException(card.Id.Entry + " must be added to a CombatState before playing it.");
		}
		bool owningPlayerIsLocal = LocalContext.IsMe(card.Owner);
		CardPile oldPile = card.Pile;
		NCard nCard = null;
		if (TestMode.IsOff)
		{
			nCard = NCard.FindOnTable(card);
			if (nCard == null)
			{
				nCard = CreateCardNodeAndUpdateVisuals(card, card.Pile?.Type, PileType.Play, owningPlayerIsLocal);
			}
		}
		card.RemoveFromCurrentPile();
		PileType.Play.GetPile(card.Owner).AddInternal(card);
		if (nCard != null)
		{
			MoveCardNodeToNewPileBeforeTween(nCard, PileType.Play);
			Tween tween = NCombatRoom.Instance.CreateTween().SetParallel();
			AppendPlayPileLerpTween(tween, nCard, oldPile?.Type);
			nCard.PlayPileTween = tween;
			tween.Play();
			if (card.Type == CardType.Power && await tween.AwaitFinished(NCombatRoom.Instance) != TweenOutcome.Finished)
			{
				return;
			}
		}
		await Hook.AfterCardChangedPiles(card.Owner.RunState, card.CombatState, card, oldPile?.Type ?? PileType.None, null);
	}

	private static NCard CreateCardNodeAndUpdateVisuals(CardModel card, PileType? oldPileType, PileType targetPileType, bool owningPlayerIsLocal)
	{
		NCard nCard = NCard.Create(card);
		NCombatRoom.Instance.Ui.AddChildSafely(nCard);
		nCard.UpdateVisuals(targetPileType, CardPreviewMode.Normal);
		if (!owningPlayerIsLocal)
		{
			nCard.Position = NCombatRoom.Instance.GetCreatureNode(card.Owner.Creature).IntentContainer.GlobalPosition;
		}
		else if (oldPileType.HasValue)
		{
			nCard.Position = oldPileType.Value.GetTargetPosition(nCard);
		}
		else
		{
			nCard.Position = targetPileType.GetTargetPosition(nCard);
		}
		return nCard;
	}

	private static void MoveCardNodeToNewPileBeforeTween(NCard cardNode, PileType newPileType)
	{
		NPlayerHand hand = NCombatRoom.Instance.Ui.Hand;
		NCardPlayQueue playQueue = NCombatRoom.Instance.Ui.PlayQueue;
		Control playContainer = NCombatRoom.Instance.Ui.PlayContainer;
		Vector2 globalPosition = cardNode.GlobalPosition;
		CardModel model = cardNode.Model;
		if (playQueue.IsAncestorOf(cardNode))
		{
			playQueue.RemoveCardFromQueueForExecution(model);
		}
		if (hand.IsAncestorOf(cardNode))
		{
			hand.Remove(model);
		}
		else
		{
			cardNode.GetParent()?.RemoveChildSafely(cardNode);
		}
		if (newPileType == PileType.Play)
		{
			playContainer.AddChildSafely(cardNode);
			if (NCombatUi.IsDebugHidingPlayContainer)
			{
				cardNode.Visible = false;
			}
		}
		else
		{
			NCombatRoom.Instance.Ui.AddChildSafely(cardNode);
		}
		cardNode.GlobalPosition = globalPosition;
		cardNode.PlayPileTween?.Kill();
		cardNode.PlayPileTween = null;
	}

	private static void AppendPlayPileLerpTween(Tween? tween, NCard cardNode, PileType? oldPile)
	{
		AppendPileLerpTween(tween, cardNode, cardNode.Model.Pile.Type, oldPile);
		tween?.Parallel().TweenCallback(Callable.From(() =>
		{
			NCombatRoom.Instance.Ui.AddToPlayContainer(cardNode);
		}));
	}

	private static void AppendPileLerpTween(Tween? tween, NCard cardNode, PileType typePile, PileType? oldPile)
	{
		if (tween != null)
		{
			Vector2 targetPosition = typePile.GetTargetPosition(cardNode);
			float num = SaveManager.Instance.PrefsSave.FastMode switch
			{
				FastModeType.Instant => 0.01f,
				FastModeType.Fast => 0.1f,
				_ => 0.25f,
			};
			if (typePile != PileType.Hand)
			{
				tween.TweenProperty(cardNode, "position", targetPosition, num).SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Cubic);
			}
			if (typePile == PileType.Play)
			{
				tween.TweenProperty(cardNode, "scale", Vector2.One * 0.8f, 0.25).SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Cubic);
			}
			else if (!oldPile.HasValue)
			{
				tween.TweenProperty(cardNode, "scale", Vector2.One, num).SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Cubic)
					.From(Vector2.Zero);
			}
			else
			{
				tween.Parallel().TweenProperty(cardNode, "scale", Vector2.One, num).SetEase(Tween.EaseType.Out)
					.SetTrans(Tween.TransitionType.Cubic);
			}
		}
	}

	public static async Task<CardModel?> Draw(PlayerChoiceContext choiceContext, Player player)
	{
		return (await Draw(choiceContext, 1m, player)).FirstOrDefault();
	}

	public static Task<IEnumerable<CardModel>> Draw(PlayerChoiceContext choiceContext, decimal count, Player player, bool fromHandDraw = false)
	{
		return DrawInternal(choiceContext, count, player, fromHandDraw);
	}

	public static Task DrawWithoutBlockingOnOtherPlayers(PlayerChoiceContext choiceContext, decimal count, Player player, CardModel source, bool fromHandDraw = false)
	{
		BranchingPlayerChoiceContext branchingPlayerChoiceContext = new BranchingPlayerChoiceContext(source, LocalContext.NetId.Value, choiceContext);
		Task<IEnumerable<CardModel>> task = Draw(branchingPlayerChoiceContext, count, player, fromHandDraw);
		return branchingPlayerChoiceContext.AssignTaskAndWaitForPauseOrCompletion(task);
	}

	private static async Task<IEnumerable<CardModel>> DrawInternal(PlayerChoiceContext choiceContext, decimal count, Player player, bool fromHandDraw = false)
	{
		if (CombatManager.Instance.IsOverOrEnding)
		{
			return Array.Empty<CardModel>();
		}
		if (!Hook.ShouldDraw(player.Creature.CombatState, player, fromHandDraw, out AbstractModel modifier))
		{
			await Hook.AfterPreventingDraw(player.Creature.CombatState, modifier);
			return Array.Empty<CardModel>();
		}
		ICombatState combatState = player.Creature.CombatState;
		List<CardModel> result = new List<CardModel>();
		CardPile hand = PileType.Hand.GetPile(player);
		CardPile drawPile = PileType.Draw.GetPile(player);
		int drawsRequested = ((count > 0m) ? ((int)Math.Ceiling(count)) : 0);
		if (drawsRequested == 0)
		{
			return result;
		}
		int num = Math.Max(0, CardPile.MaxCardsInHand - hand.Cards.Count);
		if (num == 0)
		{
			CheckIfDrawIsPossibleAndShowThoughtBubbleIfNot(player);
			return result;
		}
		for (int i = 0; i < drawsRequested; i++)
		{
			if (num <= 0)
			{
				break;
			}
			if (CombatManager.Instance.IsOverOrEnding)
			{
				break;
			}
			if (!CheckIfDrawIsPossibleAndShowThoughtBubbleIfNot(player))
			{
				break;
			}
			await ShuffleIfNecessary(choiceContext, player);
			if (!CheckIfDrawIsPossibleAndShowThoughtBubbleIfNot(player))
			{
				break;
			}
			CardModel card = drawPile.Cards.FirstOrDefault();
			if (card == null || hand.Cards.Count >= CardPile.MaxCardsInHand)
			{
				break;
			}
			result.Add(card);
			await Add(card, hand);
			CombatManager.Instance.History.CardDrawn(combatState, card, fromHandDraw);
			await Hook.AfterCardDrawn(combatState, choiceContext, card, fromHandDraw);
			card.InvokeDrawn();
			NDebugAudioManager.Instance?.Play("card_deal.mp3", 0.25f, PitchVariance.Small);
			num = Math.Max(0, CardPile.MaxCardsInHand - hand.Cards.Count);
		}
		return result;
	}

	public static async Task Shuffle(PlayerChoiceContext choiceContext, Player player)
	{
		if (CombatManager.Instance.IsOverOrEnding)
		{
			return;
		}
		CardPile pile = PileType.Draw.GetPile(player);
		List<CardModel> list = PileType.Discard.GetPile(player).Cards.ToList();
		float timeBetweenCardAdds = Mathf.Min(0.045f, 0.8f / (float)list.Count);
		float randomTimeBetweenCardAdds = 1.11f * timeBetweenCardAdds;
		HashSet<CardModel> drawPileCards = pile.Cards.ToHashSet();
		list.AddRange(drawPileCards);
		list.StableShuffle(player.RunState.Rng.Shuffle);
		Hook.ModifyShuffleOrder(player.Creature.CombatState, player, list, isInitialShuffle: false);
		if (CombatManager.Instance.DebugForcedTopCardOnNextShuffle != null)
		{
			if (!list.Remove(CombatManager.Instance.DebugForcedTopCardOnNextShuffle))
			{
				throw new InvalidOperationException("Could not find card " + CombatManager.Instance.DebugForcedTopCardOnNextShuffle.Id.Entry + " in discard pile.");
			}
			list.Insert(0, CombatManager.Instance.DebugForcedTopCardOnNextShuffle);
			CombatManager.Instance.DebugClearForcedTopCardOnNextShuffle();
		}
		float waitTimeAccumulator = 0f;
		IReadOnlyList<CardPileAddResult> readOnlyList = await Add(list, pile, CardPilePosition.Bottom, null, skipVisuals: true);
		List<Tween> tweens = new List<Tween>();
		foreach (CardPileAddResult item in readOnlyList)
		{
			if (drawPileCards.Contains(item.cardAdded))
			{
				continue;
			}
			var (tween, flag) = GetTweenForCardsChangingPiles(new _003C_003Ez__ReadOnlySingleElementList<CardPileAddResult>(item), fromSilentAdd: true);
			if (tween != null)
			{
				tweens.Add(tween);
			}
			if (tween != null || flag)
			{
				float num = timeBetweenCardAdds + Rng.Chaotic.NextFloat((0f - randomTimeBetweenCardAdds) * 0.5f, randomTimeBetweenCardAdds * 0.5f);
				waitTimeAccumulator += num;
				if ((double)waitTimeAccumulator >= ((SceneTree)Engine.GetMainLoop()).Root.GetProcessDeltaTime())
				{
					await Cmd.Wait(num);
					waitTimeAccumulator = 0f;
				}
			}
		}
		foreach (Tween item2 in tweens)
		{
			if (item2.IsRunning())
			{
				await item2.AwaitFinished(NCombatRoom.Instance);
			}
		}
		await Cmd.CustomScaledWait(0.2f, 0.5f);
		if (!CombatManager.Instance.IsOverOrEnding)
		{
			await Hook.AfterShuffle(player.Creature.CombatState, choiceContext, player);
		}
	}

	public static async Task AutoPlayFromDrawPile(PlayerChoiceContext choiceContext, Player player, int count, CardPilePosition position, bool forceExhaust)
	{
		await AutoPlayFromDrawPile(choiceContext, player, count, position, forceExhaust, (CardModel _) =>
		{
		});
	}

	public static async Task AutoPlayFromDrawPile(PlayerChoiceContext choiceContext, Player player, int count, CardPilePosition position, bool forceExhaust, Action<CardModel> cardModification)
	{
		if (CombatManager.Instance.IsOverOrEnding)
		{
			return;
		}
		List<CardModel> cards = new List<CardModel>(count);
		CardPile drawPile = PileType.Draw.GetPile(player);
		for (int i = 0; i < count; i++)
		{
			await ShuffleIfNecessary(choiceContext, player);
			CardModel cardModel = position switch
			{
				CardPilePosition.Bottom => drawPile.Cards.LastOrDefault(),
				CardPilePosition.Top => drawPile.Cards.FirstOrDefault(),
				CardPilePosition.Random => player.RunState.Rng.CombatCardSelection.NextItem(drawPile.Cards),
				_ => throw new ArgumentOutOfRangeException("position", position, null),
			};
			if (cardModel == null)
			{
				break;
			}
			cardModification(cardModel);
			cards.Add(cardModel);
			await Add(cardModel, PileType.Play);
		}
		foreach (CardModel item in cards)
		{
			if (!item.Owner.Creature.IsDead)
			{
				item.ExhaustOnNextPlay = forceExhaust;
				await CardCmd.AutoPlay(choiceContext, item, null);
				continue;
			}
			break;
		}
	}

	public static async Task ShuffleIfNecessary(PlayerChoiceContext choiceContext, Player player)
	{
		CardPile pile = PileType.Draw.GetPile(player);
		CardPile pile2 = PileType.Discard.GetPile(player);
		if (!pile.Cards.Any() && pile2.Cards.Any())
		{
			await ShuffleFtueCheck();
			await Shuffle(choiceContext, player);
		}
	}

	private static async Task ShuffleFtueCheck()
	{
		if (!SaveManager.Instance.SeenFtue("shuffle_ftue") && NModalContainer.Instance != null)
		{
			NShuffleFtue nShuffleFtue = NShuffleFtue.Create();
			NModalContainer.Instance.Add(nShuffleFtue);
			SaveManager.Instance.MarkFtueAsComplete("shuffle_ftue");
			await nShuffleFtue.WaitForPlayerToConfirm();
		}
	}

	public static async Task AddToCombatAndPreview<T>(IEnumerable<Creature> targets, PileType pileType, int count, Player? creator, CardPilePosition position = CardPilePosition.Bottom) where T : CardModel
	{
		foreach (Creature target in targets)
		{
			await AddToCombatAndPreview<T>(target, pileType, count, creator, position);
		}
	}

	public static async Task AddToCombatAndPreview<T>(Creature target, PileType pileType, int count, Player? creator, CardPilePosition position = CardPilePosition.Bottom) where T : CardModel
	{
		Player player = target.Player ?? target.PetOwner;
		if (player.Creature.IsDead)
		{
			return;
		}
		CardPileAddResult[] statusCards = new CardPileAddResult[count];
		for (int i = 0; i < count; i++)
		{
			ICombatState? combatState = target.CombatState;
			CardModel cardModel = ((combatState != null) ? combatState.CreateCard<T>(player) : null);
			if (cardModel != null)
			{
				CardPileAddResult[] array = statusCards;
				int num = i;
				array[num] = await AddGeneratedCardToCombat(cardModel, pileType, creator, position);
			}
		}
		if (LocalContext.IsMe(player))
		{
			if (pileType == PileType.Hand)
			{
				await Cmd.Wait(0.1f);
				return;
			}
			CardPreviewStyle style = ((statusCards.Length <= 5) ? CardPreviewStyle.HorizontalLayout : CardPreviewStyle.MessyLayout);
			CardCmd.PreviewCardPileAdd(statusCards, 1.2f, style);
			await Cmd.Wait(1f);
		}
	}

	public static async Task<CardModel?> AddCurseToDeck<T>(Player owner) where T : CardModel
	{
		return (await AddCursesToDeck(new _003C_003Ez__ReadOnlySingleElementList<CardModel>(ModelDb.Card<T>()), owner)).FirstOrDefault().cardAdded;
	}

	public static async Task<IEnumerable<CardPileAddResult>> AddCursesToDeck(IEnumerable<CardModel> curses, Player owner)
	{
		List<CardPileAddResult> results = new List<CardPileAddResult>();
		foreach (CardModel curse in curses)
		{
			if (curse.Type != CardType.Curse)
			{
				throw new ArgumentException(curse.Id.Entry + " is not a curse");
			}
			CardModel card = owner.RunState.CreateCard(curse, owner);
			results.Add(await Add(card, PileType.Deck));
		}
		CardCmd.PreviewCardPileAdd(results, 2f);
		return results;
	}

	private static bool CheckIfDrawIsPossibleAndShowThoughtBubbleIfNot(Player player)
	{
		if (PileType.Draw.GetPile(player).Cards.Count + PileType.Discard.GetPile(player).Cards.Count == 0)
		{
			ThinkCmd.Play(new LocString("combat_messages", "NO_DRAW"), player.Creature, 2.0);
			return false;
		}
		if (PileType.Hand.GetPile(player).Cards.Count >= CardPile.MaxCardsInHand)
		{
			ThinkCmd.Play(new LocString("combat_messages", "HAND_FULL"), player.Creature, 2.0);
			return false;
		}
		return true;
	}
}
