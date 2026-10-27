using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Actions;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;

namespace MegaCrit.Sts2.Core.Transfer;

/// <summary>
/// [apermesa] Spend 1 energy to send one card from the owner's hand to a teammate.
/// </summary>
public sealed class TransferCardAction : GameAction
{
	private const int _energyCost = 1;

	public override ulong OwnerId => Player.NetId;

	public override GameActionType ActionType => GameActionType.CombatPlayPhaseOnly;

	public Player Player { get; }

	public ulong TargetPlayerId { get; }

	public TransferCardAction(Player player, ulong targetPlayerId)
	{
		Player = player;
		TargetPlayerId = targetPlayerId;
	}

	protected override async Task ExecuteAction()
	{
		if (!CombatManager.Instance.IsInProgress || CombatManager.Instance.IsOverOrEnding)
		{
			Cancel();
			return;
		}
		PlayerCombatState? playerCombatState = Player.PlayerCombatState;
		if (playerCombatState == null || playerCombatState.Phase != PlayerTurnPhase.Play)
		{
			Cancel();
			return;
		}
		Player? player = Player.RunState.GetPlayer(TargetPlayerId);
		if (player == null || player == Player || player.Creature == null || player.Creature.IsDead)
		{
			Cancel();
			return;
		}
		if (TransferState.HasUsedCardTransfer(Player, playerCombatState.TurnNumber))
		{
			Cancel();
			return;
		}
		if (playerCombatState.Energy < _energyCost || TransferState.GetTransferableCards(Player).Count == 0)
		{
			Cancel();
			return;
		}
		PlayerChoiceContext context = new GameActionPlayerChoiceContext(this);
		CardSelectorPrefs prefs = new CardSelectorPrefs(new LocString("card_selection", "APERMESA_TRANSFER_CARD_PROMPT"), 1)
		{
			Cancelable = true,
			RequireManualConfirmation = true
		};
		IEnumerable<CardModel> enumerable;
		try
		{
			enumerable = await CardSelectCmd.FromHand(context, Player, prefs, TransferState.IsTransferableCard, null);
		}
		catch (OperationCanceledException)
		{
			if (State == GameActionState.GatheringPlayerChoice)
			{
				await context.SignalPlayerChoiceEnded();
			}
			Cancel();
			return;
		}
		CardModel? cardModel = enumerable.FirstOrDefault();
		if (cardModel == null || cardModel.Pile == null || cardModel.Pile.Type != PileType.Hand)
		{
			Cancel();
			return;
		}
		if (playerCombatState.Energy < _energyCost)
		{
			Cancel();
			return;
		}
		playerCombatState.LoseEnergy(_energyCost);
		if (Player.Creature.CombatState != null)
		{
			CombatManager.Instance.History.EnergySpent(Player.Creature.CombatState, _energyCost, Player);
		}
		await CardPileCmd.GiveToAnotherPlayer(cardModel, player, PileType.Hand);
		TransferState.MarkCardTransfer(Player, playerCombatState.TurnNumber);
	}

	public override INetAction ToNetAction()
	{
		return new NetTransferCardAction
		{
			giverNetId = Player.NetId,
			targetPlayerId = TargetPlayerId
		};
	}

	public override string ToString()
	{
		return $"TransferCardAction giver: {Player.NetId} target: {TargetPlayerId}";
	}
}
