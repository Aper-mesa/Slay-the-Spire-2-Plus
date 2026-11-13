using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Godot;
using MegaCrit.Sts2.Core.AutoSlay.Helpers;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.DevConsole;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization.Fonts;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Characters;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Screens.ScreenContext;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Saves;
using MegaCrit.Sts2.Core.Settings;
using MegaCrit.Sts2.Core.Timeline;
using MegaCrit.Sts2.Core.Timeline.Epochs;
using MegaCrit.Sts2.Core.Unlocks;
using MegaCrit.Sts2.addons.mega_text;

namespace MegaCrit.Sts2.Core.AutoSlay.Scenarios;

/// <summary>
/// Drives a scenario script from boot to exit: reach a run at a pinned seed, execute the script,
/// emit one sentinel, quit. A sibling to <see cref="T:MegaCrit.Sts2.Core.AutoSlay.AutoSlayer" /> that never marks itself non
/// interactive, so command waits actually wait and there are animation frames worth observing.
/// </summary>
public sealed class ScenarioRunner
{
	/// <summary>A scenario-level failure carrying the message the sentinel reports.</summary>
	private sealed class ScenarioFailureException : Exception
	{
		public ScenarioFailureException(string message)
			: base(message)
		{
		}
	}

	private readonly string _scenarioPath;

	private readonly string _scenarioName;

	private readonly string _seed;

	private readonly FastModeType _fastMode;

	private static readonly TimeSpan _startupTimeout = TimeSpan.FromSeconds(60L);

	private static readonly TimeSpan _runStartTimeout = TimeSpan.FromSeconds(120L);

	private static readonly TimeSpan _consoleCommandTimeout = TimeSpan.FromSeconds(45L);

	private readonly CancellationTokenSource _cts = new CancellationTokenSource();

	private IDisposable? _watchdogScope;

	private MegaCrit.Sts2.Core.DevConsole.DevConsole? _console;

	/// <summary>The turn number observed when `endturn` ran, so `turn-started` keys off that, not off when it began.</summary>
	private int? _endTurnTurnNumber;

	/// <summary>The combat that <see cref="F:MegaCrit.Sts2.Core.AutoSlay.Scenarios.ScenarioRunner._endTurnTurnNumber" /> was observed in, so it cannot be mistaken for a turn of the same number in a later combat.</summary>
	private CombatId? _endTurnCombatId;

	private ScenarioOutcome _outcome;

	private string _failureReason = string.Empty;

	private ScenarioRunner(string scenarioPath, string seed, FastModeType fastMode)
	{
		_scenarioPath = scenarioPath;
		_scenarioName = Path.GetFileNameWithoutExtension(scenarioPath);
		_seed = seed;
		_fastMode = fastMode;
	}

	/// <summary>
	/// Builds a runner from the command line and starts it. Never awaited: the caller is
	/// <see cref="M:MegaCrit.Sts2.Core.Nodes.NGame.GameStartup" />, which the runner itself waits on.
	/// </summary>
	public static void StartFromCommandLine()
	{
		try
		{
			Start();
		}
		catch (Exception ex)
		{
			FailBeforeStart(ex.GetType().Name + ": " + ex.Message);
		}
	}

	private static void Start()
	{
		string value = CommandLineHelper.GetValue("log-file");
		if (value != null)
		{
			ScenarioLog.OpenLogFile(value);
		}
		string value2 = CommandLineHelper.GetValue("scenario");
		if (string.IsNullOrWhiteSpace(value2))
		{
			FailBeforeStart("--scenario needs a path, as --scenario=<path>.");
			return;
		}
		if (CommandLineHelper.HasArg("autoslay"))
		{
			FailBeforeStart("--scenario and --autoslay are mutually exclusive.");
			return;
		}
		string value3 = CommandLineHelper.GetValue("fast-mode");
		if (!TryParseFastMode(value3, out var fastMode))
		{
			FailBeforeStart($"--fast-mode='{value3}' is not one of {string.Join(", ", Enum.GetNames<FastModeType>())}.");
		}
		else
		{
			string seed = CommandLineHelper.GetValue("seed") ?? SeedHelper.GetRandomSeed();
			ScenarioRunner scenarioRunner = new ScenarioRunner(value2, seed, fastMode);
			TaskHelper.RunSafely(scenarioRunner.RunAsync());
		}
	}

	/// <summary>
	/// Parses `--fast-mode`. A null argument means the flag was absent, which is Normal.
	/// </summary>
	/// <remarks>
	/// Enum.TryParse accepts any numeric string, so `--fast-mode=7` would parse to an undefined
	/// value and reach PrefsSave. None is rejected too: the enum documents it as never to be used.
	/// </remarks>
	public static bool TryParseFastMode(string? argument, out FastModeType fastMode)
	{
		if (argument == null)
		{
			fastMode = FastModeType.Normal;
			return true;
		}
		if (Enum.TryParse<FastModeType>(argument, ignoreCase: true, out fastMode) && Enum.IsDefined(fastMode) && fastMode != FastModeType.None)
		{
			return true;
		}
		fastMode = FastModeType.Normal;
		return false;
	}

	/// <summary>
	/// Separate from the install site so a test can pin the timeout; the constructor argument is
	/// optional and falls back to AutoSlay's 30s. The test covers this factory, not the `Use` call.
	/// </summary>
	public static Watchdog CreateWatchdog(Func<DateTime>? clock = null)
	{
		return new Watchdog(ScenarioLog.Info, ScenarioLog.Error, ScenarioConditions.watchdogTimeout, ScenarioConditions.watchdogLogInterval, clock);
	}

	private static void FailBeforeStart(string reason)
	{
		try
		{
			ScenarioLog.Failed(reason);
		}
		finally
		{
			QuitGame(ScenarioExitCode.For(ScenarioOutcome.Failed));
			ScenarioLog.CloseLogFile();
		}
	}

	private async Task RunAsync()
	{
		ScenarioLog.Info("Starting scenario '" + _scenarioName + "' from " + _scenarioPath);
		try
		{
			await WithTimeout(NGame.Instance.GameStartupComplete, _startupTimeout, "Game startup");
			ScenarioScript script = ParseOrFail();
			ApplyPreflight();
			await StartRunAsync();
			await ExecuteAsync(script);
		}
		catch (ScenarioFailureException ex)
		{
			Fail(ScenarioOutcome.Failed, ex.Message);
		}
		catch (DriverTimeoutException ex2)
		{
			Fail(ScenarioOutcome.Stalled, ex2.Message);
		}
		catch (Exception ex3)
		{
			Fail(ScenarioOutcome.InternalError, ex3.GetType().Name + ": " + ex3.Message);
			ScenarioLog.Error("Unhandled scenario exception", ex3);
		}
		finally
		{
			if (_outcome == ScenarioOutcome.Completed)
			{
				ScenarioLog.Completed(_scenarioName);
			}
			else
			{
				ScenarioLog.Failed(_failureReason);
			}
			_watchdogScope?.Dispose();
			_watchdogScope = null;
			_cts.Dispose();
			QuitGame(ScenarioExitCode.For(_outcome));
			ScenarioLog.CloseLogFile();
		}
	}

	/// <summary>Bounds a task that runs outside the watchdog's reach.</summary>
	private static async Task WithTimeout(Task task, TimeSpan timeout, string what)
	{
		try
		{
			await task.WaitAsync(timeout);
		}
		catch (TimeoutException)
		{
			throw new DriverTimeoutException($"{what} did not complete within {timeout.TotalSeconds}s.");
		}
	}

	/// <inheritdoc cref="M:MegaCrit.Sts2.Core.AutoSlay.Scenarios.ScenarioRunner.WithTimeout(System.Threading.Tasks.Task,System.TimeSpan,System.String)" />
	private static async Task<T> WithTimeout<T>(Task<T> task, TimeSpan timeout, string what)
	{
		try
		{
			return await task.WaitAsync(timeout);
		}
		catch (TimeoutException)
		{
			throw new DriverTimeoutException($"{what} did not complete within {timeout.TotalSeconds}s.");
		}
	}

	private void Fail(ScenarioOutcome outcome, string reason)
	{
		_outcome = outcome;
		_failureReason = reason;
	}

	private ScenarioScript ParseOrFail()
	{
		MegaCrit.Sts2.Core.DevConsole.DevConsole devConsole = new MegaCrit.Sts2.Core.DevConsole.DevConsole(shouldAllowDebugCommands: true);
		HashSet<string> reservedNames = new HashSet<string>(devConsole.GetCompletionResults(string.Empty).Candidates, StringComparer.OrdinalIgnoreCase) { "help" };
		ScenarioParseResult scenarioParseResult = ScenarioScript.ParseFile(_scenarioPath, reservedNames);
		if (!(scenarioParseResult is ScenarioParseResult.Success success))
		{
			throw new ScenarioFailureException(scenarioParseResult.Describe());
		}
		_console = devConsole;
		ScenarioLog.Info($"Parsed {success.Script.Statements.Count} statements from {_scenarioPath}");
		return success.Script;
	}

	/// <summary>
	/// AutoSlay's pre-flight minus FastMode. Mutates the real progress save, as AutoSlay's does.
	/// </summary>
	private void ApplyPreflight()
	{
		if (NonInteractiveMode.IsActive)
		{
			throw new ScenarioFailureException("NonInteractiveMode.IsActive is true, so every command wait would be skipped and no animation would play.");
		}
		SaveManager.Instance.PrefsSave.FastMode = _fastMode;
		ScenarioLog.Info($"FastMode={_fastMode}");
		SaveManager.Instance.SetFtuesEnabled(enabled: false);
		SaveManager.Instance.MarkFtueAsComplete("ascension_singleplayer_ftue");
		SaveManager.Instance.ObtainEpochOverride(EpochModel.GetId<Silent1Epoch>(), EpochState.Revealed);
		SaveManager.Instance.ObtainEpochOverride(EpochModel.GetId<Regent1Epoch>(), EpochState.Revealed);
		SaveManager.Instance.ObtainEpochOverride(EpochModel.GetId<Defect1Epoch>(), EpochState.Revealed);
		SaveManager.Instance.ObtainEpochOverride(EpochModel.GetId<Necrobinder1Epoch>(), EpochState.Revealed);
		_watchdogScope = Watchdog.Use(CreateWatchdog());
	}

	private async Task StartRunAsync()
	{
		Watchdog.Current?.Reset("Starting run");
		ScenarioLog.Info("Starting run with seed=" + _seed);
		Player player = Player.CreateForNewRun<Ironclad>(new UnlockState(SaveManager.Instance.Progress), 1uL);
		RunState runState = await WithTimeout(NGame.Instance.StartNewSingleplayerRun(player, shouldSave: false, ActModel.GetDefaultList(), Array.Empty<ModifierModel>(), _seed, GameMode.Standard, isCustomGame: false), _runStartTimeout, "Starting the run");
		Watchdog.Current?.Reset("Run started");
		if (!RunManager.Instance.IsSingleplayerOrFakeMultiplayer)
		{
			throw new ScenarioFailureException("Scenarios are singleplayer only, and this run is not singleplayer.");
		}
		await WaitHelper.Until(() => runState.CurrentRoom != null && runState.CurrentRoom.RoomType != RoomType.Unassigned, _cts.Token, AutoSlayConfig.nodeWaitTimeout, "Room type not assigned");
		ScenarioLog.Info("Run started");
	}

	private async Task ExecuteAsync(ScenarioScript script)
	{
		foreach (ScenarioStatement statement in script.Statements)
		{
			Watchdog.Current?.Reset($"Line {statement.LineNumber}");
			await ExecuteAsync(statement);
		}
	}

	private async Task ExecuteAsync(ScenarioStatement statement)
	{
		if (!(statement is ScenarioStatement.ConsoleLine statement2))
		{
			if (!(statement is ScenarioStatement.WaitForCondition statement3))
			{
				if (!(statement is ScenarioStatement.WaitForDuration statement4))
				{
					if (!(statement is ScenarioStatement.EndTurn statement5))
					{
						throw new ScenarioFailureException($"Line {statement.LineNumber}: unhandled statement type {statement.GetType().Name}.");
					}
					await ExecuteEndTurnAsync(statement5);
				}
				else
				{
					await ExecuteWaitForDurationAsync(statement4);
				}
			}
			else
			{
				await ExecuteWaitForConditionAsync(statement3);
			}
		}
		else
		{
			await ExecuteConsoleLineAsync(statement2);
		}
	}

	private async Task ExecuteConsoleLineAsync(ScenarioStatement.ConsoleLine statement)
	{
		ScenarioLog.Info($"Line {statement.LineNumber}: {statement.Text}");
		Player me = LocalContext.GetMe(RunManager.Instance.DebugOnlyGetState());
		if (me == null)
		{
			throw new ScenarioFailureException($"Line {statement.LineNumber}: '{statement.Text}' has no local player to act as.");
		}
		CmdResult cmdResult = _console.ProcessNetCommand(me, statement.Text);
		if (!cmdResult.success)
		{
			throw new ScenarioFailureException($"Line {statement.LineNumber}: '{statement.Text}' failed: {cmdResult.msg}");
		}
		if (!string.IsNullOrWhiteSpace(cmdResult.msg))
		{
			ScenarioLog.Info($"Line {statement.LineNumber}: {cmdResult.msg}");
		}
		if (cmdResult.task != null)
		{
			try
			{
				await WaitHelper.ForTask(cmdResult.task, _cts.Token, _consoleCommandTimeout, $"Did not complete within {_consoleCommandTimeout.TotalSeconds}s.");
			}
			catch (DriverTimeoutException ex)
			{
				throw new DriverTimeoutException($"Line {statement.LineNumber}: '{statement.Text}' stalled. {ex.Message}");
			}
			catch (Exception ex2)
			{
				throw new ScenarioFailureException($"Line {statement.LineNumber}: '{statement.Text}' threw {ex2.GetType().Name}: {ex2.Message}");
			}
		}
		await ReadinessBarrierAsync();
	}

	/// <summary>
	/// The turn `endturn` last recorded, and only if it was recorded in the combat running now. It
	/// is cleared by a successful `wait turn-started`, so it otherwise outlives its own combat.
	/// </summary>
	private int? EndTurnTurnNumberForCurrentCombat()
	{
		if (!(_endTurnCombatId == CombatManager.Instance.CurrentCombatId))
		{
			return null;
		}
		return _endTurnTurnNumber;
	}

	private async Task ExecuteEndTurnAsync(ScenarioStatement.EndTurn statement)
	{
		ScenarioLog.Info($"Line {statement.LineNumber}: endturn");
		if (!CombatManager.Instance.IsInProgress)
		{
			throw new ScenarioFailureException($"Line {statement.LineNumber}: 'endturn' needs combat in progress.");
		}
		Player me = LocalContext.GetMe(RunManager.Instance.DebugOnlyGetState());
		if (me?.PlayerCombatState == null)
		{
			throw new ScenarioFailureException($"Line {statement.LineNumber}: 'endturn' has no local player to act as.");
		}
		int turnNumber = me.PlayerCombatState.TurnNumber;
		if (EndTurnTurnNumberForCurrentCombat() == turnNumber)
		{
			throw new ScenarioFailureException($"Line {statement.LineNumber}: 'endturn' ran twice for turn {turnNumber}. " + "Put a 'wait turn-started' between two 'endturn' statements.");
		}
		if (CombatManager.Instance.IsPlayerReadyToEndTurn(me))
		{
			throw new ScenarioFailureException($"Line {statement.LineNumber}: 'endturn' ran while the player was already ready to end turn. " + "Put a 'wait turn-started' between two 'endturn' statements.");
		}
		if (me.PlayerCombatState.Phase != PlayerTurnPhase.Play)
		{
			throw new ScenarioFailureException($"Line {statement.LineNumber}: 'endturn' needs the local player's play phase, but the phase is {me.PlayerCombatState.Phase}. Put a 'wait combat-started' or 'wait turn-started' before it.");
		}
		NCombatRoom instance = NCombatRoom.Instance;
		if (instance == null)
		{
			throw new ScenarioFailureException($"Line {statement.LineNumber}: 'endturn' has no combat room to act on.");
		}
		NPlayerHand hand = instance.Ui.Hand;
		if (hand.InCardPlay || hand.CurrentMode != NPlayerHand.Mode.Play)
		{
			throw new ScenarioFailureException($"Line {statement.LineNumber}: 'endturn' cannot end the turn while the hand is busy (InCardPlay={hand.InCardPlay}, mode={hand.CurrentMode}).");
		}
		if (instance.Mode != CombatRoomMode.ActiveCombat)
		{
			throw new ScenarioFailureException($"Line {statement.LineNumber}: 'endturn' needs the combat room in active combat, but it is {instance.Mode}.");
		}
		if (!ActiveScreenContext.Instance.IsCurrent(instance))
		{
			throw new ScenarioFailureException($"Line {statement.LineNumber}: 'endturn' cannot end the turn while something else owns the " + "screen. Close the overlay the previous line opened first.");
		}
		RunManager.Instance.ActionQueueSynchronizer.RequestEnqueue(new EndPlayerTurnAction(me, turnNumber));
		_endTurnTurnNumber = turnNumber;
		_endTurnCombatId = CombatManager.Instance.CurrentCombatId;
		await ReadinessBarrierAsync();
	}

	private async Task ExecuteWaitForConditionAsync(ScenarioStatement.WaitForCondition statement)
	{
		ScenarioConditionDescriptor scenarioConditionDescriptor = ScenarioConditions.For(statement.Condition);
		string action = $"wait {scenarioConditionDescriptor.Name} (line {statement.LineNumber})";
		ScenarioLog.Info($"Line {statement.LineNumber}: wait {scenarioConditionDescriptor.Name}");
		Watchdog.Current?.Reset(action);
		await WaitHelper.Until(BuildPredicate(statement.Condition, scenarioConditionDescriptor, statement.LineNumber), _cts.Token, scenarioConditionDescriptor.Timeout, $"Line {statement.LineNumber}: {scenarioConditionDescriptor.FailureMessage}");
		if (statement.Condition == ScenarioWaitCondition.TurnStarted)
		{
			_endTurnTurnNumber = null;
			_endTurnCombatId = null;
		}
		Watchdog.Current?.Reset(action);
	}

	private async Task ExecuteWaitForDurationAsync(ScenarioStatement.WaitForDuration statement)
	{
		string action = $"wait {statement.Seconds}s (line {statement.LineNumber})";
		ScenarioLog.Info($"Line {statement.LineNumber}: wait {statement.Seconds}s");
		DateTime deadline = DateTime.UtcNow + TimeSpan.FromSeconds(statement.Seconds);
		while (DateTime.UtcNow < deadline)
		{
			Watchdog.Current?.Reset(action);
			TimeSpan timeSpan = deadline - DateTime.UtcNow;
			if (timeSpan < TimeSpan.Zero)
			{
				timeSpan = TimeSpan.Zero;
			}
			await Task.Delay((timeSpan < AutoSlayConfig.pollingInterval) ? timeSpan : AutoSlayConfig.pollingInterval, _cts.Token);
		}
		Watchdog.Current?.Reset(action);
	}

	/// <summary>
	/// Built per wait, because `turn-started` is relative to the turn number when the wait began.
	/// </summary>
	private Func<bool> BuildPredicate(ScenarioWaitCondition condition, ScenarioConditionDescriptor descriptor, int lineNumber)
	{
		switch (condition)
		{
		case ScenarioWaitCondition.CombatStarted:
		{
			Func<bool> abort2 = BuildCombatEndedAbort(descriptor, lineNumber, sawCombat: false);
			return () =>
			{
				if (!abort2() && CombatManager.Instance.IsInProgress)
				{
					PlayerCombatState playerCombatState = LocalContext.GetMe(RunManager.Instance.DebugOnlyGetState())?.PlayerCombatState;
					if (playerCombatState != null)
					{
						return playerCombatState.Phase == PlayerTurnPhase.Play;
					}
					return false;
				}
				return false;
			};
		}
		case ScenarioWaitCondition.TurnStarted:
		{
			Func<bool> abort = BuildCombatEndedAbort(descriptor, lineNumber, sawCombat: true);
			Player player = LocalContext.GetMe(RunManager.Instance.DebugOnlyGetState());
			int baseline = EndTurnTurnNumberForCurrentCombat() ?? (player?.PlayerCombatState?.TurnNumber).GetValueOrDefault();
			return () =>
			{
				if (!abort())
				{
					PlayerCombatState playerCombatState = player?.PlayerCombatState;
					if (playerCombatState != null && playerCombatState.Phase == PlayerTurnPhase.Play)
					{
						return playerCombatState.TurnNumber > baseline;
					}
				}
				return false;
			};
		}
		default:
			throw new ScenarioFailureException($"No predicate for wait condition {condition}.");
		}
	}

	/// <summary>
	/// Fails a combat-dependent wait once its combat is over. Always returns false, so it composes
	/// into a predicate. Latched, because combat is not in progress yet when `combat-started` begins.
	/// </summary>
	private static Func<bool> BuildCombatEndedAbort(ScenarioConditionDescriptor descriptor, int lineNumber, bool sawCombat)
	{
		return () =>
		{
			bool flag = CombatManager.Instance.IsInProgress || CombatManager.Instance.IsStarting;
			sawCombat |= flag;
			if (sawCombat && !flag)
			{
				throw new ScenarioFailureException($"Line {lineNumber}: combat ended before '{descriptor.Name}' came true.");
			}
			return false;
		};
	}

	/// <summary>
	/// Two process frames after each line, the same pair the get-logs command uses. The await resumes
	/// inside SceneTree's process_frame emission, which runs before the deferred queue is flushed, so
	/// a line that finished on the C# pump needs the second frame for its call_deferred work to land.
	/// Does not await overlay quiet: awaiting an interactive overlay is a known deadlock source.
	/// </summary>
	private static async Task ReadinessBarrierAsync()
	{
		await NGame.Instance.AwaitProcessFrame();
		await NGame.Instance.AwaitProcessFrame();
	}

	private static void QuitGame(int exitCode)
	{
		ScenarioLog.Info($"Quitting game with exit code {exitCode}");
		try
		{
			MegaLabel.DisposeCachedParagraph();
			MegaRichTextLabel.DisposeCachedParagraph();
			FontManager.ClearCache();
		}
		catch (Exception ex)
		{
			ScenarioLog.Error("Cache teardown failed", ex);
		}
		finally
		{
			SceneTree sceneTree = NGame.Instance?.GetTree() ?? (Engine.GetMainLoop() as SceneTree);
			if (sceneTree != null)
			{
				sceneTree.Quit(exitCode);
			}
			else
			{
				ScenarioLog.Error("No scene tree to quit through; exiting the process directly.");
				ScenarioLog.CloseLogFile();
				System.Environment.Exit(exitCode);
			}
		}
	}
}
