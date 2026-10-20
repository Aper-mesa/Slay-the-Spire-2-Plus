using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Multiplayer.Game;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Multiplayer;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Screens.Overlays;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace MegaCrit.Sts2.Core.Transfer;

/// <summary>
/// [apermesa] Multiplayer card/relic transfer.
///
/// A player may spend 1 energy once per turn to send a hand card to a teammate, and may spend
/// 1 max HP once per turn to lend a relic to a teammate. Loaned relics return at the start of the
/// giver's next turn or when combat ends.
///
/// The card hand-cap rule is inherited from CardPileCmd: a full hand sends the card to the discard pile.
/// </summary>
public static class TransferState
{
	private const int _cardEnergyCost = 1;

	private const int _relicMaxHpCost = 1;

	private sealed class RelicLoan
	{
		public required Player Giver;

		public required Player Receiver;

		public required RelicModel Relic;

		public required SerializableRelic Snapshot;

		public required int ReturnTurn;
	}

	private static readonly Dictionary<ulong, int> _cardTransferTurn = new Dictionary<ulong, int>();

	private static readonly Dictionary<ulong, int> _relicTransferTurn = new Dictionary<ulong, int>();

	private static readonly List<RelicLoan> _loans = new List<RelicLoan>();

	private static bool _initialized;

	public static void Initialize()
	{
		if (_initialized)
		{
			return;
		}
		_initialized = true;
		RegisterLocStrings();
		CombatManager.Instance.CombatSetUp += OnCombatSetUp;
		CombatManager.Instance.TurnStarted += OnTurnStarted;
		CombatManager.Instance.CombatEnded += OnCombatEnded;
	}

	private static void RegisterLocStrings()
	{
		LocManager.RegisterRuntimeStrings("zhs", "gameplay_ui", new Dictionary<string, string>
		{
			["APERMESA_TRANSFER_CARD_BUTTON"] = "传牌",
			["APERMESA_TRANSFER_RELIC_BUTTON"] = "传遗物",
			["APERMESA_TRANSFER_CARD_TOOLTIP"] = "花费 1 点能量，将一张手牌传给队友（每回合一次）",
			["APERMESA_TRANSFER_RELIC_TOOLTIP"] = "花费 1 点最大生命值，将一件遗物借给队友，下个回合开始或战斗结束时归还（每回合一次）",
			["APERMESA_TRANSFER_NOT_PLAY_PHASE"] = "现在不能传递",
			["APERMESA_TRANSFER_NO_TARGET"] = "没有可传递的队友",
			["APERMESA_TRANSFER_NO_ENERGY"] = "能量不足：传牌需要多于 1 点能量",
			["APERMESA_TRANSFER_NO_HP"] = "最大生命值不足：传遗物需要多于 1 点最大生命值",
			["APERMESA_TRANSFER_NO_CARDS"] = "手牌中没有可传递的牌",
			["APERMESA_TRANSFER_NO_RELICS"] = "没有可传递的遗物",
			["APERMESA_TRANSFER_ALREADY_USED_CARD"] = "本回合已经传过一次牌了",
			["APERMESA_TRANSFER_ALREADY_USED_RELIC"] = "本回合已经传过一次遗物了"
		});
		LocManager.RegisterRuntimeStrings("eng", "gameplay_ui", new Dictionary<string, string>
		{
			["APERMESA_TRANSFER_CARD_BUTTON"] = "Send Card",
			["APERMESA_TRANSFER_RELIC_BUTTON"] = "Send Relic",
			["APERMESA_TRANSFER_CARD_TOOLTIP"] = "Spend 1 Energy to send a card from your hand to a teammate (once per turn).",
			["APERMESA_TRANSFER_RELIC_TOOLTIP"] = "Spend 1 Max HP to lend a relic to a teammate. It returns at the start of your next turn or when combat ends (once per turn).",
			["APERMESA_TRANSFER_NOT_PLAY_PHASE"] = "You cannot transfer right now.",
			["APERMESA_TRANSFER_NO_TARGET"] = "No teammate available.",
			["APERMESA_TRANSFER_NO_ENERGY"] = "Not enough Energy: sending a card requires more than 1 Energy.",
			["APERMESA_TRANSFER_NO_HP"] = "Not enough Max HP: sending a relic requires more than 1 Max HP.",
			["APERMESA_TRANSFER_NO_CARDS"] = "You have no cards in hand.",
			["APERMESA_TRANSFER_NO_RELICS"] = "You have no transferable relics.",
			["APERMESA_TRANSFER_ALREADY_USED_CARD"] = "You already sent a card this turn.",
			["APERMESA_TRANSFER_ALREADY_USED_RELIC"] = "You already sent a relic this turn."
		});
		LocManager.RegisterRuntimeStrings("zhs", "card_selection", new Dictionary<string, string>
		{
			["APERMESA_TRANSFER_CARD_PROMPT"] = "选择一张手牌传给队友"
		});
		LocManager.RegisterRuntimeStrings("eng", "card_selection", new Dictionary<string, string>
		{
			["APERMESA_TRANSFER_CARD_PROMPT"] = "Choose a card to send to your teammate"
		});
	}

	public static bool HasUsedCardTransfer(Player player, int turnNumber)
	{
		return _cardTransferTurn.TryGetValue(player.NetId, out int value) && value == turnNumber;
	}

	public static bool HasUsedRelicTransfer(Player player, int turnNumber)
	{
		return _relicTransferTurn.TryGetValue(player.NetId, out int value) && value == turnNumber;
	}

	public static void MarkCardTransfer(Player player, int turnNumber)
	{
		_cardTransferTurn[player.NetId] = turnNumber;
	}

	public static void MarkRelicTransfer(Player player, int turnNumber)
	{
		_relicTransferTurn[player.NetId] = turnNumber;
	}

	public static List<Player> GetTeammates(Player me)
	{
		ICombatState? combatState = me.Creature.CombatState;
		if (combatState == null)
		{
			return new List<Player>();
		}
		return (from c in combatState.GetTeammatesOf(me.Creature)
			where c != null && c.IsAlive && c.IsPlayer && c.Player != me
			select c.Player).OfType<Player>().ToList();
	}

	/// <summary>
	/// [apermesa] Relics that may be lent. This is a whitelist built from exclusion rules:
	/// - no on-pickup effects (max HP, card grants, enchants, etc.),
	/// - no combat-start-only effects, which would never trigger mid-combat,
	/// - no used-up/melted/pet-summoning relics,
	/// - no starter relics.
	/// Pael's Eye is the explicit exception: its AfterObtained only initializes per-combat state.
	/// </summary>
	public static bool IsTransferableRelic(RelicModel relic)
	{
		if (relic == null)
		{
			return false;
		}
		if (relic.IsUsedUp || relic.IsMelted || relic.SpawnsPets)
		{
			return false;
		}
		if (relic.Rarity == RelicRarity.None || relic.Rarity == RelicRarity.Starter || relic.Rarity == RelicRarity.Event)
		{
			return false;
		}
		if (_loans.Any((RelicLoan loan) => object.ReferenceEquals(loan.Relic, relic)))
		{
			return false;
		}
		if (relic is PaelsEye)
		{
			return true;
		}
		if (relic.HasUponPickupEffect)
		{
			return false;
		}
		Type type = relic.GetType();
		if (Declares(type, "AfterObtained"))
		{
			return false;
		}
		if (Declares(type, "BeforeCombatStart") || Declares(type, "BeforeCombatStartLate"))
		{
			return false;
		}
		return true;
	}

	public static List<RelicModel> GetTransferableRelics(Player player, Player? target = null)
	{
		return player.Relics.Where(delegate(RelicModel relic)
		{
			if (!IsTransferableRelic(relic))
			{
				return false;
			}
			if (target != null)
			{
				return !target.Relics.Any((RelicModel owned) => owned.Id == relic.Id);
			}
			return true;
		}).ToList();
	}

	private static bool Declares(Type type, string methodName)
	{
		MethodInfo? methodInfo = type.GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		return methodInfo != null && methodInfo.DeclaringType == type;
	}

	public static string? ValidateCardTransfer(Player me)
	{
		if (!CombatManager.Instance.IsInProgress || CombatManager.Instance.IsOverOrEnding)
		{
			return "APERMESA_TRANSFER_NOT_PLAY_PHASE";
		}
		PlayerCombatState? playerCombatState = me.PlayerCombatState;
		if (playerCombatState == null || playerCombatState.Phase != PlayerTurnPhase.Play)
		{
			return "APERMESA_TRANSFER_NOT_PLAY_PHASE";
		}
		if (GetTeammates(me).Count == 0)
		{
			return "APERMESA_TRANSFER_NO_TARGET";
		}
		if (playerCombatState.Energy <= _cardEnergyCost)
		{
			return "APERMESA_TRANSFER_NO_ENERGY";
		}
		if (playerCombatState.Hand.Cards.Count == 0)
		{
			return "APERMESA_TRANSFER_NO_CARDS";
		}
		if (HasUsedCardTransfer(me, playerCombatState.TurnNumber))
		{
			return "APERMESA_TRANSFER_ALREADY_USED_CARD";
		}
		return null;
	}

	public static string? ValidateRelicTransfer(Player me)
	{
		if (!CombatManager.Instance.IsInProgress || CombatManager.Instance.IsOverOrEnding)
		{
			return "APERMESA_TRANSFER_NOT_PLAY_PHASE";
		}
		PlayerCombatState? playerCombatState = me.PlayerCombatState;
		if (playerCombatState == null || playerCombatState.Phase != PlayerTurnPhase.Play)
		{
			return "APERMESA_TRANSFER_NOT_PLAY_PHASE";
		}
		if (GetTeammates(me).Count == 0)
		{
			return "APERMESA_TRANSFER_NO_TARGET";
		}
		if (me.Creature.MaxHp <= _relicMaxHpCost)
		{
			return "APERMESA_TRANSFER_NO_HP";
		}
		if (GetTransferableRelics(me).Count == 0)
		{
			return "APERMESA_TRANSFER_NO_RELICS";
		}
		if (HasUsedRelicTransfer(me, playerCombatState.TurnNumber))
		{
			return "APERMESA_TRANSFER_ALREADY_USED_RELIC";
		}
		return null;
	}

	public static void BeginCardTransfer(Player me, Control source)
	{
		TaskHelper.RunSafely(BeginCardTransferAsync(me, source));
	}

	public static void BeginRelicTransfer(Player me, Control source)
	{
		TaskHelper.RunSafely(BeginRelicTransferAsync(me, source));
	}

	private static async Task BeginCardTransferAsync(Player me, Control source)
	{
		string? error = ValidateCardTransfer(me);
		if (error != null)
		{
			ShowMessage(me, error);
			return;
		}
		Player? target = await SelectTeammateAsync(me, source);
		if (target == null)
		{
			return;
		}
		RunManager.Instance.ActionQueueSynchronizer.RequestEnqueue(new TransferCardAction(me, target.NetId));
	}

	private static async Task BeginRelicTransferAsync(Player me, Control source)
	{
		string? error = ValidateRelicTransfer(me);
		if (error != null)
		{
			ShowMessage(me, error);
			return;
		}
		Player? target = await SelectTeammateAsync(me, source);
		if (target == null)
		{
			return;
		}
		RunManager.Instance.ActionQueueSynchronizer.RequestEnqueue(new TransferRelicAction(me, target.NetId, returnMode: false));
	}

	public static void ShowMessage(Player me, string locKey)
	{
		if (me?.Creature != null && LocalContext.IsMe(me))
		{
			ThinkCmd.Play(new LocString("gameplay_ui", locKey), me.Creature, 2.0);
		}
	}

	private static async Task<Player?> SelectTeammateAsync(Player me, Control source)
	{
		List<Player> teammates = GetTeammates(me);
		if (teammates.Count == 0)
		{
			return null;
		}
		if (teammates.Count == 1)
		{
			return teammates[0];
		}
		NTargetManager? targetManager = NTargetManager.Instance;
		if (targetManager == null)
		{
			return null;
		}
		bool isUsingDirectionalNavigation = NControllerManager.Instance != null && NControllerManager.Instance.IsUsingDirectionalNavigation;
		targetManager.StartTargeting(TargetType.AnyAlly, source, isUsingDirectionalNavigation ? TargetMode.Controller : TargetMode.ClickMouseToTarget, delegate
		{
			return !GodotObject.IsInstanceValid(source) || NOverlayStack.Instance.ScreenCount > 0;
		}, null);
		if (isUsingDirectionalNavigation && NCombatRoom.Instance != null)
		{
			List<Control> list = (from p in teammates
				where p.Creature != null && p.Creature.IsAlive
				select NCombatRoom.Instance.GetCreatureNode(p.Creature)).OfType<NCreature>().Select((NCreature c) => c.Hitbox).ToList();
			NCombatRoom.Instance.RestrictControllerNavigation(list);
			list.FirstOrDefault()?.TryGrabFocus();
		}
		try
		{
			Node? node = await targetManager.SelectionFinished();
			if (node is NCreature nCreature)
			{
				return nCreature.Entity.Player;
			}
			if (node is NMultiplayerPlayerState nMultiplayerPlayerState)
			{
				return nMultiplayerPlayerState.Player;
			}
			return null;
		}
		finally
		{
			NCombatRoom.Instance?.EnableControllerNavigation();
		}
	}

	public static async Task GiveRelicAsync(Player giver, Player receiver, RelicModel relic)
	{
		SerializableRelic snapshot = relic.ToSerializable();
		await RelicCmd.Remove(relic);
		RelicModel relicModel = RelicModel.FromSerializable(snapshot);
		await RelicCmd.Obtain(relicModel, receiver);
		PlayerCombatState? playerCombatState = giver.PlayerCombatState;
		int returnTurn = ((playerCombatState != null) ? (playerCombatState.TurnNumber + 1) : 1);
		_loans.Add(new RelicLoan
		{
			Giver = giver,
			Receiver = receiver,
			Relic = relicModel,
			Snapshot = snapshot,
			ReturnTurn = returnTurn
		});
	}

	public static async Task ReturnDueLoansAsync(Player giver)
	{
		int turnNumber = giver.PlayerCombatState?.TurnNumber ?? 0;
		List<RelicLoan> list = _loans.Where((RelicLoan loan) => loan.Giver == giver && turnNumber >= loan.ReturnTurn).ToList();
		foreach (RelicLoan item in list)
		{
			await ReturnLoanAsync(item);
			_loans.Remove(item);
		}
	}

	private static async Task ReturnAllLoansAsync()
	{
		List<RelicLoan> list = _loans.ToList();
		foreach (RelicLoan item in list)
		{
			await ReturnLoanAsync(item);
			_loans.Remove(item);
		}
	}

	private static async Task ReturnLoanAsync(RelicLoan loan)
	{
		RelicModel? relicModel = loan.Receiver.Relics.FirstOrDefault((RelicModel relic) => object.ReferenceEquals(relic, loan.Relic));
		if (relicModel != null)
		{
			await RelicCmd.Remove(relicModel);
		}
		await RelicCmd.Obtain(RelicModel.FromSerializable(loan.Snapshot), loan.Giver);
	}

	private static void OnCombatSetUp(CombatState state)
	{
		_cardTransferTurn.Clear();
		_relicTransferTurn.Clear();
		_loans.Clear();
	}

	private static void OnCombatEnded(CombatRoom room)
	{
		_cardTransferTurn.Clear();
		_relicTransferTurn.Clear();
		TaskHelper.RunSafely(ReturnAllLoansAsync());
	}

	private static void OnTurnStarted(CombatState state)
	{
		if (state.CurrentSide != CombatSide.Player)
		{
			return;
		}
		NetGameType type = RunManager.Instance.NetService.Type;
		if (type != NetGameType.Singleplayer && type != NetGameType.Host)
		{
			return;
		}
		foreach (Player player in state.Players)
		{
			int turnNumber = player.PlayerCombatState?.TurnNumber ?? 0;
			if (_loans.Any((RelicLoan loan) => loan.Giver == player && turnNumber >= loan.ReturnTurn))
			{
				RunManager.Instance.ActionQueueSynchronizer.RequestEnqueue(new TransferRelicAction(player, 0uL, returnMode: true));
			}
		}
	}
}
