using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Actions;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Multiplayer.Game;
using MegaCrit.Sts2.Core.Nodes.Screens;
using MegaCrit.Sts2.Core.Runs;

namespace MegaCrit.Sts2.Core.Transfer;

/// <summary>
/// [apermesa] Spend 1 max HP to lend a relic to a teammate. The relic returns at the start of the
/// giver's next turn or when combat ends. The same action type is also used to return due loans.
/// </summary>
public sealed class TransferRelicAction : GameAction
{
	private const int _maxHpCost = 1;

	public override ulong OwnerId => Player.NetId;

	public override GameActionType ActionType => GameActionType.CombatPlayPhaseOnly;

	public Player Player { get; }

	public ulong TargetPlayerId { get; }

	public bool ReturnMode { get; }

	public TransferRelicAction(Player player, ulong targetPlayerId, bool returnMode)
	{
		Player = player;
		TargetPlayerId = targetPlayerId;
		ReturnMode = returnMode;
	}

	protected override async Task ExecuteAction()
	{
		if (ReturnMode)
		{
			await TransferState.ReturnDueLoansAsync(Player);
			return;
		}
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
		Player? target = Player.RunState.GetPlayer(TargetPlayerId);
		if (target == null || target == Player || target.Creature == null || target.Creature.IsDead)
		{
			Cancel();
			return;
		}
		if (Player.Creature.MaxHp <= _maxHpCost)
		{
			Cancel();
			return;
		}
		if (TransferState.HasUsedRelicTransfer(Player, playerCombatState.TurnNumber))
		{
			Cancel();
			return;
		}
		List<RelicModel> eligible = TransferState.GetTransferableRelics(Player, target);
		if (eligible.Count == 0)
		{
			Cancel();
			return;
		}
		PlayerChoiceContext context = new GameActionPlayerChoiceContext(this);
		uint choiceId = RunManager.Instance.PlayerChoiceSynchronizer.ReserveChoiceId(Player);
		RelicModel? relic = null;
		await context.SignalPlayerChoiceBegun(Player, PlayerChoiceOptions.CancelPlayCardActions);
		try
		{
			if (LocalContext.IsMe(Player) && RunManager.Instance.NetService.Type != NetGameType.Replay)
			{
				NChooseARelicSelection? nChooseARelicSelection = NChooseARelicSelection.ShowScreen(eligible, new LocString("gameplay_ui", "APERMESA_TRANSFER_CANCEL_BUTTON").GetFormattedText());
				relic = ((nChooseARelicSelection == null) ? null : (await nChooseARelicSelection.RelicsSelected()).FirstOrDefault());
				int index = ((relic == null) ? (-1) : eligible.IndexOf(relic));
				RunManager.Instance.PlayerChoiceSynchronizer.SyncLocalChoice(Player, choiceId, PlayerChoiceResult.FromIndex(index));
			}
			else
			{
				int num = (await RunManager.Instance.PlayerChoiceSynchronizer.WaitForRemoteChoice(Player, choiceId)).AsIndex();
				relic = ((num >= 0 && num < eligible.Count) ? eligible[num] : null);
			}
		}
		catch (OperationCanceledException)
		{
			relic = null;
		}
		finally
		{
			if (State == GameActionState.GatheringPlayerChoice)
			{
				await context.SignalPlayerChoiceEnded();
			}
		}
		if (relic == null || !Player.Relics.Contains(relic))
		{
			Cancel();
			return;
		}
		if (TransferState.HasUsedRelicTransfer(Player, playerCombatState.TurnNumber))
		{
			Cancel();
			return;
		}
		await CreatureCmd.LoseMaxHp(context, Player.Creature, _maxHpCost, isFromCard: false);
		await TransferState.GiveRelicAsync(Player, target, relic);
		TransferState.MarkRelicTransfer(Player, playerCombatState.TurnNumber);
	}

	public override INetAction ToNetAction()
	{
		return new NetTransferRelicAction
		{
			giverNetId = Player.NetId,
			targetPlayerId = TargetPlayerId,
			returnMode = ReturnMode
		};
	}

	public override string ToString()
	{
		return $"TransferRelicAction giver: {Player.NetId} target: {TargetPlayerId} return: {ReturnMode}";
	}
}
