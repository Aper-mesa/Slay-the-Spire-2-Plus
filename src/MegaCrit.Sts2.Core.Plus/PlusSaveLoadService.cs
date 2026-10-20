using System;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.ControllerInput;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Multiplayer;
using MegaCrit.Sts2.Core.Multiplayer.Game;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Audio;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Saves;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace MegaCrit.Sts2.Core.Plus;

/// <summary>[apermesa] Built-in Save &amp; Load: reload the latest autosaved checkpoint.</summary>
public static class PlusSaveLoadService
{
	public const string FadeTransitionPath = "res://materials/transitions/fade_transition_mat.tres";

	private static bool _running;

	private static INetGameService? _registeredService;

	public static bool IsRunning => _running;

	internal static bool TryEnter()
	{
		if (_running)
		{
			return false;
		}
		_running = true;
		return true;
	}

	internal static void Exit()
	{
		_running = false;
	}

	public static void Request()
	{
		if (!TryEnter())
		{
			return;
		}
		TaskHelper.RunSafely(RunAsync());
	}

	/// <summary>Called from NRun._Ready so the hotkey and net handlers follow the run lifecycle.</summary>
	public static void OnRunStarted()
	{
		PlusSaveLoadRuntimeLoc.EnsureRegistered();
		NHotkeyManager.Instance?.PushHotkeyPressedBinding(MegaInput.saveAndLoad, Request);
		EnsureHandlersRegistered();
	}

	private static async Task RunAsync()
	{
		try
		{
			NGame? game = NGame.Instance;
			if (game == null || game.Transition.InTransition)
			{
				return;
			}
			RunManager runManager = RunManager.Instance;
			if (!runManager.IsInProgress || runManager.IsCleaningUp)
			{
				return;
			}
			switch (runManager.NetService.Type)
			{
			case NetGameType.Singleplayer:
				await RunSingleplayerAsync(game, runManager);
				break;
			case NetGameType.Host:
				await PlusSaveLoadHost.RunAsync(game, runManager);
				break;
			case NetGameType.Client:
				await PlusSaveLoadClient.RequestAsync(game, runManager);
				break;
			}
		}
		catch (Exception ex)
		{
			Log.Error($"[PlusSL] Save & Load failed: {ex}");
		}
		finally
		{
			Exit();
		}
	}

	private static async Task RunSingleplayerAsync(NGame game, RunManager runManager)
	{
		bool fadedOut = false;
		bool cleanedUp = false;
		try
		{
			SaveManager saveManager = SaveManager.Instance;
			Task? currentSaveTask = saveManager.CurrentRunSaveTask;
			if (currentSaveTask != null)
			{
				await currentSaveTask;
			}
			if (!saveManager.HasRunSave)
			{
				Log.Warn("[PlusSL] No singleplayer run save found.");
				return;
			}
			ReadSaveResult<SerializableRun> readResult = saveManager.LoadRunSave();
			if (!readResult.Success || readResult.SaveData == null)
			{
				Log.Warn($"[PlusSL] Failed to read run save: {readResult.Status}");
				return;
			}
			SerializableRun save = readResult.SaveData;
			RunState state = RunState.FromSerializable(save);

			runManager.ActionExecutor.Cancel();
			runManager.ActionQueueSet.Reset();
			NRunMusicController.Instance?.StopMusic();

			await game.Transition.FadeOut(0.8f, FadeTransitionPath);
			fadedOut = true;
			runManager.CleanUp(graceful: true, disconnect: true);
			cleanedUp = true;
			await runManager.SetUpSavedSingleplayer(state, save);
			game.ReactionContainer.InitializeNetworking(new NetSingleplayerGameService());
			await game.LoadRun(state, save.PreFinishedRoom);
			await game.Transition.FadeIn(0.8f, FadeTransitionPath);
			fadedOut = false;
			Log.Info("[PlusSL] Singleplayer reload complete.");
		}
		catch (Exception ex)
		{
			Log.Error($"[PlusSL] Singleplayer reload failed: {ex}");
			await RecoverAsync(game, fadedOut, cleanedUp);
		}
	}

	internal static async Task RecoverAsync(NGame game, bool fadedOut, bool cleanedUp)
	{
		try
		{
			if (cleanedUp)
			{
				await game.ReturnToMainMenu();
			}
			else if (fadedOut)
			{
				await game.Transition.FadeIn(0.8f, FadeTransitionPath);
			}
		}
		catch (Exception ex)
		{
			Log.Error($"[PlusSL] Recovery failed: {ex}");
		}
	}

	internal static void Notify(string locKey)
	{
		NGame? game = NGame.Instance;
		if (game == null)
		{
			return;
		}
		try
		{
			string text = new LocString("gameplay_ui", locKey).GetFormattedText();
			NFullscreenTextVfx? vfx = NFullscreenTextVfx.Create(text);
			if (vfx != null)
			{
				game.AddChildSafely(vfx);
			}
		}
		catch (Exception ex)
		{
			Log.Warn($"[PlusSL] Failed to show notice {locKey}: {ex}");
		}
	}

	private static void EnsureHandlersRegistered()
	{
		INetGameService netService = RunManager.Instance.NetService;
		if (netService.Type == NetGameType.Singleplayer)
		{
			return;
		}
		if (ReferenceEquals(_registeredService, netService))
		{
			return;
		}
		_registeredService = netService;
		netService.RegisterMessageHandler<PlusSlInitiateMessage>(PlusSaveLoadHost.HandleInitiate);
		netService.RegisterMessageHandler<PlusSlInitiateResponseMessage>(PlusSaveLoadClient.HandleInitiateResponse);
		netService.RegisterMessageHandler<PlusSlExecuteMessage>(PlusSaveLoadClient.HandleExecute);
		netService.RegisterMessageHandler<PlusSlSetupReadyMessage>(PlusSaveLoadHost.HandleSetupReady);
		netService.RegisterMessageHandler<PlusSlRunBeginMessage>(PlusSaveLoadClient.HandleRunBegin);
		netService.RegisterMessageHandler<PlusSlCancelMessage>(PlusSaveLoadClient.HandleCancel);
		netService.Disconnected += OnDisconnected;
	}

	private static void OnDisconnected(NetErrorInfo info)
	{
		_registeredService = null;
		PlusSaveLoadHost.Reset();
		PlusSaveLoadClient.Reset();
	}
}
