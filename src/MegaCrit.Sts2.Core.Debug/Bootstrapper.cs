using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Map;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Debug;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Saves;

namespace MegaCrit.Sts2.Core.Debug;

/// <summary>
/// Utility for quickly starting up a game.
/// </summary>
public class Bootstrapper
{
	private Action<RunState, bool> _runManagerSetup;

	private List<Player>? _players;

	private List<ActModel>? _acts;

	private string? _seed;

	private RunState? _runState;

	public static IBootstrapSettings? Settings { get; }

	static Bootstrapper()
	{
		Type type = BootstrapSettingsUtil.Get();
		if (!(type == null))
		{
			Settings = (IBootstrapSettings)Activator.CreateInstance(type);
		}
	}

	public Bootstrapper()
	{
		_runManagerSetup = DefaultRunManagerSetup;
	}

	public Bootstrapper WithPlayers(List<Player> players)
	{
		_players = players;
		return this;
	}

	public Bootstrapper WithRunManagerSetup(Action<RunState, bool> action)
	{
		_runManagerSetup = action;
		return this;
	}

	public Bootstrapper WithActs(List<ActModel> acts)
	{
		_acts = acts;
		return this;
	}

	public Bootstrapper WithSeed(string seed)
	{
		_seed = seed;
		return this;
	}

	public async Task<RunState> SetupRun()
	{
		if (Settings == null)
		{
			throw new InvalidOperationException("No type implementing IBootstrapSettings found in the project! To use the bootstrap scene, copy src/Core/Nodes/Debug/BootstrapSettings.cs.template and rename it to BootstrapSettings.cs");
		}
		if (Settings.Language != null)
		{
			LocManager.Instance.SetLanguage(Settings.Language);
		}
		PreloadManager.Enabled = Settings.DoPreloading;
		string seed = _seed ?? Settings.Seed ?? SeedHelper.GetRandomSeed();
		if (_acts == null)
		{
			_acts = ActModel.GetDefaultList().ToList();
		}
		_acts[0] = Settings.Act;
		if (_players == null)
		{
			_players = new List<Player>(1) { Player.CreateForNewRun(Settings.Character, SaveManager.Instance.GenerateUnlockStateFromProgress(), 1uL) };
		}
		int val = ((Settings.GameMode == GameMode.Blitz) ? 5 : 10);
		int ascensionLevel = Math.Min(Settings.Ascension, val);
		_runState = RunState.CreateForNewRun(_players, _acts.Select((ActModel a) => a.ToMutable()).ToList(), Settings.Modifiers, Settings.GameMode, isCustom: false, ascensionLevel, seed);
		_runManagerSetup(_runState, Settings.SaveRunHistory);
		if (Settings.ForcedBoss != null)
		{
			RoomSet roomsDebug = _runState.Acts[0].GetRoomsDebug();
			if (roomsDebug.HasSecondBoss)
			{
				roomsDebug.SecondBoss = Settings.ForcedBoss;
			}
			else
			{
				roomsDebug.Boss = Settings.ForcedBoss;
			}
		}
		await PreloadManager.LoadRunAssets(_runState.Players.Select((Player p) => p.Character));
		await RunManager.Instance.FinalizeStartingRelics();
		RunManager.Instance.Launch();
		NGame.Instance.RootSceneContainer.SetCurrentScene(NRun.Create(_runState));
		await RunManager.Instance.SetActInternal(0);
		await SaveManager.Instance.SaveRun(null);
		RunManager.Instance.RunLocationTargetedBuffer.OnLocationChanged(_runState.RunLocation);
		RunManager.Instance.MapSelectionSynchronizer.OnLocationChanged(_runState.MapLocation);
		await Settings.Setup(_runState.Players[0]);
		return _runState;
	}

	public async Task EnterFirstRoom()
	{
		if (_runState == null)
		{
			throw new InvalidOperationException("Must call SetupRun first!");
		}
		if (Settings == null)
		{
			throw new InvalidOperationException("No type implementing IBootstrapSettings found in the project! To use the bootstrap scene, copy src/Core/Nodes/Debug/BootstrapSettings.cs.template and rename it to BootstrapSettings.cs");
		}
		switch (Settings.RoomType)
		{
		case RoomType.Unassigned:
			await RunManager.Instance.EnterAct(0);
			break;
		case RoomType.Treasure:
		case RoomType.Shop:
		case RoomType.RestSite:
			await RunManager.Instance.EnterRoomDebug(Settings.RoomType);
			RunManager.Instance.ActionExecutor.Unpause();
			break;
		case RoomType.Event:
		{
			AbstractRoom abstractRoom = await RunManager.Instance.EnterRoomDebug(Settings.RoomType, MapPointType.Unassigned, Settings.Event);
			if (abstractRoom != null && abstractRoom.IsVictoryRoom)
			{
				_runState.CurrentActIndex = _runState.Acts.Count - 1;
			}
			break;
		}
		default:
			await RunManager.Instance.EnterRoomDebug(Settings.RoomType, MapPointType.Unassigned, Settings.RoomType.IsCombatRoom() ? Settings.Encounter.ToMutable() : null);
			break;
		}
	}

	private void DefaultRunManagerSetup(RunState runState, bool saveRunHistory)
	{
		RunManager.Instance.SetUpNewSingleplayer(runState, saveRunHistory);
	}
}
