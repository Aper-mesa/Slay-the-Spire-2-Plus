using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Godot;
using MegaCrit.Sts2.Core.AutoSlay.Handlers;
using MegaCrit.Sts2.Core.AutoSlay.Handlers.Rooms;
using MegaCrit.Sts2.Core.AutoSlay.Handlers.Screens;
using MegaCrit.Sts2.Core.AutoSlay.Helpers;
using MegaCrit.Sts2.Core.AutoSlay.Launchers;
using MegaCrit.Sts2.Core.AutoSlay.Navigation;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Debug;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization.Fonts;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.Events;
using MegaCrit.Sts2.Core.Nodes.Events.Custom.CrystalSphere;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Screens;
using MegaCrit.Sts2.Core.Nodes.Screens.CardSelection;
using MegaCrit.Sts2.Core.Nodes.Screens.GameOverScreen;
using MegaCrit.Sts2.Core.Nodes.Screens.MainMenu;
using MegaCrit.Sts2.Core.Nodes.Screens.Map;
using MegaCrit.Sts2.Core.Nodes.Screens.Overlays;
using MegaCrit.Sts2.Core.Nodes.Screens.PauseMenu;
using MegaCrit.Sts2.Core.Nodes.TopBar;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.Random;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Saves;
using MegaCrit.Sts2.Core.Settings;
using MegaCrit.Sts2.addons.mega_text;

namespace MegaCrit.Sts2.Core.AutoSlay;

/// <summary>
/// Main orchestrator for AutoSlay. Runs the game automatically for smoke testing.
/// </summary>
public class AutoSlayer
{
	private readonly Dictionary<RoomType, IRoomHandler> _roomHandlers;

	private readonly Dictionary<Type, IScreenHandler> _screenHandlers;

	private CancellationTokenSource? _cts;

	private string? _seed;

	private Rng? _random;

	private IDisposable? _cardSelectorScope;

	private IDisposable? _watchdogScope;

	private static int _exitCode;

	/// <summary>How often to re-check a pending task while draining the screens it opens.</summary>
	private const int _drainPollIntervalMs = 50;

	/// <summary>Static flag indicating if AutoSlay is currently running.</summary>
	public static bool IsActive { get; private set; }

	static AutoSlayer()
	{
		NonInteractiveMode.AutoSlayerCheck = () => IsActive;
	}

	public AutoSlayer()
	{
		CombatRoomHandler value = new CombatRoomHandler();
		_roomHandlers = new Dictionary<RoomType, IRoomHandler>
		{
			[RoomType.Monster] = value,
			[RoomType.Elite] = value,
			[RoomType.Boss] = value,
			[RoomType.Event] = new EventRoomHandler(),
			[RoomType.Shop] = new ShopRoomHandler(DrainOverlayScreensUntilAsync),
			[RoomType.Treasure] = new TreasureRoomHandler(),
			[RoomType.RestSite] = new RestSiteRoomHandler()
		};
		_screenHandlers = new Dictionary<Type, IScreenHandler>
		{
			[typeof(NRewardsScreen)] = new RewardsScreenHandler(),
			[typeof(NCardRewardSelectionScreen)] = new CardRewardScreenHandler(),
			[typeof(NDeckUpgradeSelectScreen)] = new DeckUpgradeScreenHandler(),
			[typeof(NDeckTransformSelectScreen)] = new DeckTransformScreenHandler(),
			[typeof(NDeckEnchantSelectScreen)] = new DeckEnchantScreenHandler(),
			[typeof(NDeckCardSelectScreen)] = new DeckCardSelectScreenHandler(),
			[typeof(NSimpleCardSelectScreen)] = new SimpleCardSelectScreenHandler(),
			[typeof(NChooseACardSelectionScreen)] = new ChooseACardScreenHandler(),
			[typeof(NChooseABundleSelectionScreen)] = new ChooseABundleScreenHandler(),
			[typeof(NChooseARelicSelection)] = new ChooseARelicScreenHandler(),
			[typeof(NGameOverScreen)] = new GameOverScreenHandler(),
			[typeof(NCrystalSphereScreen)] = new CrystalSphereScreenHandler(),
			[typeof(NByrdEvolutionVfx)] = new ByrdEvolutionScreenHandler()
		};
	}

	/// <summary>Starts an AutoSlay run with the given seed.</summary>
	/// <param name="seed">The run seed, also used for AutoSlay's own decisions.</param>
	/// <param name="mode">The raw <c>--mode</c> value, or null for a standard run. Parsed inside the run, so a
	/// bad value fails it with the usual verdict line instead of crashing before the log is open.</param>
	/// <param name="logFile">Where to write the AutoSlay log, or null for the Godot log only.</param>
	public void Start(string seed, string? mode, string? logFile = null)
	{
		if (logFile != null)
		{
			AutoSlayLog.OpenLogFile(logFile);
		}
		SentryService.SetTag("autoslay", "true");
		SentryService.SetTag("autoslay.seed", seed);
		SentryService.SetTag("autoslay.mode", mode ?? "standard");
		IsActive = true;
		_cts = new CancellationTokenSource();
		Task task = RunAsync(seed, mode, _cts.Token);
		TaskHelper.RunSafely(task);
	}

	/// <summary>Stops the current AutoSlay run.</summary>
	public void Stop()
	{
		IsActive = false;
		_cts?.Cancel();
		_cts = null;
	}

	/// <summary>Gets the current overlay screen cast to the expected type.</summary>
	/// <exception cref="T:System.InvalidOperationException">
	/// The top of the overlay stack is not a <typeparamref name="T" />, or there is no stack to peek.
	/// </exception>
	public static T GetCurrentScreen<T>() where T : Node
	{
		IOverlayScreen overlayScreen = NOverlayStack.Instance?.Peek();
		if (!(overlayScreen is T result))
		{
			throw new InvalidOperationException($"Expected {typeof(T).Name} on top of the overlay stack, found {overlayScreen?.GetType().Name ?? "nothing"}.");
		}
		return result;
	}

	private async Task RunAsync(string seed, string? modeArg, CancellationToken ct)
	{
		AutoSlayLog.RunStarted(seed);
		try
		{
			AutoSlayMode mode = AutoSlayModes.Parse(modeArg);
			AutoSlayLog.Info($"Mode: {mode}");
			await WaitHelper.WithTimeout((CancellationToken token) => PlayRunAsync(seed, mode, token), AutoSlayConfig.runTimeout, ct);
			AutoSlayLog.RunCompleted(seed);
		}
		catch (Exception ex)
		{
			_exitCode = 1;
			AutoSlayLog.RunFailed(seed, ex);
			throw;
		}
		finally
		{
			IsActive = false;
			_watchdogScope?.Dispose();
			_watchdogScope = null;
			_cardSelectorScope?.Dispose();
			_cardSelectorScope = null;
			MemoryProfiler.Reset();
			AutoSlayLog.CloseLogFile();
			QuitGame(_exitCode);
		}
	}

	private async Task PlayRunAsync(string seed, AutoSlayMode mode, CancellationToken ct)
	{
		await WaitHelper.Until(() => NGame.Instance != null, ct, AutoSlayConfig.gameInitTimeout, "Game instance not initialized");
		_seed = seed;
		NGame.Instance.DebugSeedOverride = seed;
		SaveManager.Instance.PrefsSave.FastMode = FastModeType.Fast;
		SaveManager.Instance.SetFtuesEnabled(enabled: false);
		AutoSlayUnlocks.Apply(SaveManager.Instance.Progress, mode);
		_random = new Rng(StringHelper.GetDeterministicHashCode(seed));
		_cardSelectorScope = CardSelectorStack.SetTestSelector(new AutoSlayCardSelector(_random));
		_watchdogScope = Watchdog.Use(new Watchdog(AutoSlayLog.Info, AutoSlayLog.Error));
		Watchdog.Current?.Reset("Playing main menu");
		await PlayMainMenuAsync(RunLaunchers.For(mode), ct);
		await WaitHelper.Until(() => RunManager.Instance.DebugOnlyGetState() != null, ct, AutoSlayConfig.runStateTimeout, "Run state not initialized");
		RunState runState = RunManager.Instance.DebugOnlyGetState();
		LogRunStartedAndCheckMode(runState, mode);
		IRoomNavigator navigator = RoomNavigators.For(mode);
		MemoryProfiler.SetBaseline();
		await WaitHelper.Until(() => runState.CurrentRoom != null && runState.CurrentRoom.RoomType != RoomType.Unassigned, ct, AutoSlayConfig.nodeWaitTimeout, "Room type not assigned");
		int checkedActIndex = -1;
		while (runState.TotalFloor < 60)
		{
			ct.ThrowIfCancellationRequested();
			if (runState.CurrentActIndex != checkedActIndex)
			{
				checkedActIndex = runState.CurrentActIndex;
				await ActEntryCheck.RunAsync(runState, mode, ct);
			}
			RoomType roomType = runState.CurrentRoom.RoomType;
			RunPosition position = RunPosition.Of(runState);
			bool isFirstBossInDoubleBossMode = roomType == RoomType.Boss && runState.Map.SecondBossMapPoint != null && runState.CurrentMapCoord == runState.Map.BossMapPoint.coord;
			Watchdog.Current?.Reset($"Entering {roomType} room (Act {runState.CurrentActIndex + 1}, Floor {runState.ActFloor})");
			AutoSlayLog.EnterRoom(roomType, runState.CurrentActIndex, runState.ActFloor);
			string roomLabel = $"{roomType}:Act{runState.CurrentActIndex + 1}:F{runState.ActFloor}";
			MemoryProfiler.LogSnapshot("pre-room:" + roomLabel);
			await HandleRoomAsync(roomType, ct);
			if ((uint)(roomType - 1) > 2u)
			{
				await Task.Delay(500, ct);
			}
			else
			{
				await WaitForRewardsScreenAsync(runState, position, ct);
			}
			await DrainOverlayScreensAsync(ct);
			if (roomType == RoomType.RestSite)
			{
				await ClickRestSiteProceedIfNeeded(runState, position, ct);
			}
			if (roomType == RoomType.Event)
			{
				await ClickEventProceedIfNeeded(runState, position, ct);
			}
			MemoryProfiler.LogSnapshot("post-room:" + roomLabel);
			if (roomType == RoomType.Boss && !isFirstBossInDoubleBossMode)
			{
				Watchdog.Current?.Reset("Waiting for act transition after boss");
				RoomType postBossRoomType = RoomType.Boss;
				await WaitHelper.Until(() =>
				{
					AbstractRoom currentRoom = runState.CurrentRoom;
					if (currentRoom == null)
					{
						return false;
					}
					postBossRoomType = currentRoom.RoomType;
					return postBossRoomType != RoomType.Boss;
				}, ct, TimeSpan.FromSeconds(10L), "Act transition did not start after boss");
				AutoSlayLog.Info($"Post-boss transition: room type is now {postBossRoomType}");
				bool flag = position.ActIndex >= runState.Acts.Count - 1;
				if ((postBossRoomType == RoomType.Event) & flag)
				{
					Watchdog.Current?.Reset($"Entering {postBossRoomType} room (Act {runState.CurrentActIndex + 1}, Floor {runState.ActFloor})");
					AutoSlayLog.EnterRoom(postBossRoomType, runState.CurrentActIndex, runState.ActFloor);
					await HandleRoomAsync(postBossRoomType, ct);
					await WaitForGameOverScreenAsync(ct);
					await DrainOverlayScreensAsync(ct);
					Watchdog.Current?.Reset("Waiting for main menu after victory");
					await WaitForMainMenuAsync(ct);
					AutoSlayLog.Action("Victory! Run completed and returned to main menu");
					LogRunSummary(runState, "victory");
					return;
				}
				await navigator.WaitForNextActAsync(runState, ct);
				MemoryProfiler.LogSnapshot($"act-transition:Act{runState.CurrentActIndex + 1}");
			}
			Watchdog.Current?.Reset("Moving to the next room");
			await navigator.AdvanceAsync(runState, position, _random, ct);
		}
		AutoSlayLog.Action("Run completed (max floor reached). Abandoning");
		LogRunSummary(runState, "max floor reached");
		await AbandonRunAsync(ct);
	}

	private async Task HandleRoomAsync(RoomType roomType, CancellationToken ct)
	{
		if (!_roomHandlers.TryGetValue(roomType, out IRoomHandler handler))
		{
			AutoSlayLog.Warn($"No handler for room type: {roomType}");
		}
		else
		{
			await WaitHelper.WithTimeout((CancellationToken token) => handler.HandleAsync(_random, token), handler.Timeout, ct);
			AutoSlayLog.ExitRoom(roomType);
		}
	}

	/// <summary>
	/// Drains overlay screens until <paramref name="pending" /> completes.
	/// </summary>
	/// <remarks>
	/// A room handler that awaits something which opens a screen cannot rely on the drain
	/// between rooms: the run loop does not reach it until the room finishes, and the room is
	/// blocked on that task. Buying Orrery or Cauldron does exactly this, since both await
	/// <c>RewardsCmd.OfferCustom</c> when obtained. Driving the drain alongside the task breaks
	/// the cycle. The drain runs in fail-when-stuck mode here, because a screen it cannot close
	/// will never be closed by anyone else from this call site.
	/// </remarks>
	private async Task DrainOverlayScreensUntilAsync(Task pending, CancellationToken ct)
	{
		_ = 2;
		try
		{
			while (!pending.IsCompleted)
			{
				ct.ThrowIfCancellationRequested();
				NOverlayStack? instance = NOverlayStack.Instance;
				if (instance != null && instance.ScreenCount > 0)
				{
					await DrainOverlayScreensAsync(ct, failWhenStuck: true);
				}
				await Task.Delay(50, ct);
			}
			await pending;
		}
		finally
		{
			if (!pending.IsCompleted)
			{
				pending.ContinueWith((Task t) => t.Exception, TaskContinuationOptions.OnlyOnFaulted);
			}
		}
	}

	/// <param name="ct">Cancels the drain.</param>
	/// <param name="failWhenStuck">
	/// Throw instead of returning when the drain cannot close the screen on top. Callers that
	/// are waiting on that screen have no way to recover, so returning would spin them until an
	/// outer timeout fires with the real reason buried.
	/// </param>
	private async Task DrainOverlayScreensAsync(CancellationToken ct, bool failWhenStuck = false)
	{
		if (NOverlayStack.Instance == null)
		{
			await WaitHelper.Until(() => NOverlayStack.Instance != null, ct, AutoSlayConfig.nodeWaitTimeout, "Overlay stack not initialized");
		}
		int consecutiveNoProgress = 0;
		while (true)
		{
			NOverlayStack instance = NOverlayStack.Instance;
			if (instance == null || instance.ScreenCount == 0)
			{
				break;
			}
			ct.ThrowIfCancellationRequested();
			IOverlayScreen currentOverlay = instance.Peek();
			if (currentOverlay == null)
			{
				break;
			}
			Node node = (Node)currentOverlay;
			Type screenType = node.GetType();
			if (!_screenHandlers.TryGetValue(screenType, out IScreenHandler handler))
			{
				AutoSlayLog.Warn("No handler for screen type: " + screenType.Name);
				if (failWhenStuck)
				{
					throw new InvalidOperationException("No handler for screen " + screenType.Name + ", which is blocking a pending action");
				}
				break;
			}
			Watchdog.Current?.Reset("Handling screen: " + screenType.Name);
			int screenCountBefore = instance.ScreenCount;
			await WaitHelper.WithTimeout((CancellationToken token) => handler.HandleAsync(_random, token), handler.Timeout, ct);
			if (currentOverlay is NRewardsScreen && (NMapScreen.Instance?.IsOpen ?? false))
			{
				AutoSlayLog.Info("Rewards screen handled and map is open, exiting drain loop");
				if (failWhenStuck)
				{
					throw new InvalidOperationException("Map opened while a pending action was still waiting on the rewards screen");
				}
				break;
			}
			NOverlayStack instance2 = NOverlayStack.Instance;
			if (instance2 == null || instance2.ScreenCount != screenCountBefore || instance2.Peek() != currentOverlay)
			{
				consecutiveNoProgress = 0;
			}
			else
			{
				consecutiveNoProgress++;
				if (consecutiveNoProgress >= 3)
				{
					AutoSlayLog.Error($"Infinite loop detected: screen {screenType.Name} left the overlay stack unchanged after {3} handler attempts");
					throw new InvalidOperationException("Screen " + screenType.Name + " not closing after being handled");
				}
				AutoSlayLog.Warn($"Screen {screenType.Name} left the overlay stack unchanged after handling (attempt {consecutiveNoProgress})");
			}
			await Task.Delay(100, ct);
		}
	}

	private async Task ClickRestSiteProceedIfNeeded(RunState runState, RunPosition position, CancellationToken ct)
	{
		if (!RunPosition.HasLeft(runState, position))
		{
			Node root = ((SceneTree)Engine.GetMainLoop()).Root;
			NProceedButton nodeOrNull = root.GetNodeOrNull<NProceedButton>("/root/Game/RootSceneContainer/Run/RoomContainer/RestSiteRoom/ProceedButton");
			if (nodeOrNull != null && nodeOrNull.IsEnabled)
			{
				AutoSlayLog.Action("Clicking rest site proceed button");
				await UiHelper.Click(nodeOrNull);
			}
		}
	}

	private async Task ClickEventProceedIfNeeded(RunState runState, RunPosition position, CancellationToken ct)
	{
		Node root = ((SceneTree)Engine.GetMainLoop()).Root;
		Node eventRoom = root.GetNodeOrNull("/root/Game/RootSceneContainer/Run/RoomContainer/EventRoom");
		if (eventRoom == null)
		{
			AutoSlayLog.Info("Event room not found for proceed check");
			return;
		}
		NEventOptionButton proceedOption = null;
		await WaitHelper.Until(() =>
		{
			if (RunPosition.HasLeft(runState, position))
			{
				return true;
			}
			List<NEventOptionButton> list = (from o in UiHelper.FindAll<NEventOptionButton>(eventRoom)
				where !o.Option.IsLocked && o.Option.IsProceed
				select o).ToList();
			if (list.Count > 0)
			{
				proceedOption = list[0];
				return true;
			}
			return false;
		}, ct, TimeSpan.FromSeconds(5L), "Event proceed option did not appear and the run did not leave the event");
		if (proceedOption != null)
		{
			AutoSlayLog.Action("Clicking event proceed option");
			await UiHelper.Click(proceedOption);
		}
		else
		{
			AutoSlayLog.Info("Run already left the event, no proceed needed");
		}
	}

	private async Task WaitForRewardsScreenAsync(RunState runState, RunPosition position, CancellationToken ct)
	{
		AutoSlayLog.Action("Waiting for rewards screen");
		await WaitHelper.Until(() => NOverlayStack.Instance?.Peek() is NRewardsScreen || RunPosition.HasLeft(runState, position), ct, TimeSpan.FromSeconds(10L), "Rewards screen did not appear after combat");
	}

	private async Task WaitForGameOverScreenAsync(CancellationToken ct)
	{
		AutoSlayLog.Action("Waiting for game over screen");
		await WaitHelper.Until(() => NOverlayStack.Instance?.Peek() is NGameOverScreen, ct, TimeSpan.FromSeconds(10L), "Game over screen did not appear");
	}

	private async Task WaitForMainMenuAsync(CancellationToken ct)
	{
		AutoSlayLog.Action("Waiting for main menu");
		Node root = ((SceneTree)Engine.GetMainLoop()).Root;
		await WaitHelper.Until(() => root.GetNodeOrNull<Control>("/root/Game/RootSceneContainer/MainMenu")?.IsVisibleInTree() ?? false, ct, TimeSpan.FromSeconds(30L), "Main menu did not appear after game over");
		AutoSlayLog.Action("Main menu appeared");
	}

	private async Task PlayMainMenuAsync(IRunLauncher launcher, CancellationToken ct)
	{
		AutoSlayLog.Action("Playing main menu");
		Node root = ((SceneTree)Engine.GetMainLoop()).Root;
		NMainMenu mainMenu = await WaitHelper.ForNode<NMainMenu>(root, "/root/Game/RootSceneContainer/MainMenu", ct, TimeSpan.FromSeconds(30L));
		NButton node = mainMenu.GetNode<NButton>("MainMenuTextButtons/AbandonRunButton");
		if (node.Visible)
		{
			AutoSlayLog.Action("Abandoning existing run");
			await UiHelper.Click(node);
			await WaitHelper.Until(() => NModalContainer.Instance?.OpenModal != null, ct, AutoSlayConfig.nodeWaitTimeout, "Abandon run confirmation popup did not appear");
			Node node2 = (Node)NModalContainer.Instance.OpenModal;
			NButton node3 = node2.GetNode<NButton>("VerticalPopup/YesButton");
			AutoSlayLog.Action("Confirming abandon");
			await UiHelper.Click(node3);
			await WaitHelper.Until(() => NModalContainer.Instance.OpenModal == null, ct, AutoSlayConfig.nodeWaitTimeout, "Abandon run confirmation popup did not close");
		}
		await launcher.LaunchAsync(mainMenu, _random, _seed, ct);
	}

	/// <summary>
	/// Logs what the run actually started as, read from the run state rather than from what was clicked, and fails
	/// the run if the game did not start the requested mode.
	/// </summary>
	private static void LogRunStartedAndCheckMode(RunState runState, AutoSlayMode mode)
	{
		Player player = runState.Players[0];
		AutoSlayLog.Info($"Run started: mode={runState.GameMode}, character={player.Character.Id}, ascension={runState.AscensionLevel}, seed={runState.Rng.StringSeed}, acts=[{string.Join(", ", runState.Acts.Select((ActModel a) => a.Id))}], rotatingCharacter={player.ExtraFields.IsPlayingRotatingCharacter}");
		if (runState.GameMode != mode.ToGameMode())
		{
			throw new InvalidOperationException($"Asked for a {mode} run, but the game started a {runState.GameMode} run");
		}
	}

	private static void LogRunSummary(RunState runState, string outcome)
	{
		AutoSlayLog.Info($"Run summary: outcome={outcome}, mode={runState.GameMode}, character={runState.Players[0].Character.Id}, act={runState.CurrentActIndex + 1}/{runState.Acts.Count}, floor={runState.TotalFloor}");
	}

	private async Task AbandonRunAsync(CancellationToken ct)
	{
		Node root = ((SceneTree)Engine.GetMainLoop()).Root;
		await Task.Delay(1000, ct);
		await UiHelper.Click(await WaitHelper.ForNode<NTopBarPauseButton>(root, "/root/Game/RootSceneContainer/Run/GlobalUi/TopBar/RightAlignedStuff/PauseButton", ct));
		NPauseMenu pauseMenu = null;
		await WaitHelper.Until(() => (pauseMenu = UiHelper.FindFirst<NPauseMenu>(root)) != null && pauseMenu.IsVisibleInTree(), ct, null, "Pause menu did not open");
		NPauseMenuButton node = pauseMenu.GetNode<Control>("%ButtonContainer").GetNode<NPauseMenuButton>("GiveUp");
		await UiHelper.Click(node);
		NAbandonRunConfirmPopup confirmPopup = null;
		await WaitHelper.Until(() => (confirmPopup = UiHelper.FindFirst<NAbandonRunConfirmPopup>(root)) != null, ct, null, "Abandon confirm popup did not appear");
		NVerticalPopup node2 = confirmPopup.GetNode<NVerticalPopup>("VerticalPopup");
		await UiHelper.Click(node2.YesButton);
		await WaitForGameOverScreenAsync(ct);
		await DrainOverlayScreensAsync(ct);
		await WaitForMainMenuAsync(ct);
	}

	private static void QuitGame(int exitCode)
	{
		AutoSlayLog.Action($"Quitting game with exit code {exitCode}");
		MegaLabel.DisposeCachedParagraph();
		MegaRichTextLabel.DisposeCachedParagraph();
		FontManager.ClearCache();
		NGame.Instance?.GetTree().Quit(exitCode);
	}
}
