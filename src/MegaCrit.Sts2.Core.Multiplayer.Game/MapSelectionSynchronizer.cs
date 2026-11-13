using System;
using System.Collections.Generic;
using System.Linq;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Map;
using MegaCrit.Sts2.Core.Random;
using MegaCrit.Sts2.Core.Runs;

namespace MegaCrit.Sts2.Core.Multiplayer.Game;

public class MapSelectionSynchronizer
{
	private readonly INetGameService _netService;

	private readonly ActionQueueSynchronizer _actionQueueSynchronizer;

	private readonly IRunState _runState;

	private MapLocation _acceptingVotesFromSource;

	private readonly List<MapVote?> _votes = new List<MapVote?>();

	/// <summary>
	/// This is set when the local player votes for a map point.
	/// When the local player clicks a map point, we want to immediately show that they voted for it so it looks
	/// responsive. However, if the final vote is cast before the roundtrip for that action comes back, then we need to
	/// invalidate the vote that the player cast and return to their previous one.
	/// </summary>
	private MapVote? _predictedVote;

	/// <summary>
	/// This is needed because null is a valid value for <see cref="F:MegaCrit.Sts2.Core.Multiplayer.Game.MapSelectionSynchronizer._predictedVote" /> (meaning "no vote").
	/// </summary>
	private bool _predictedVoteSet;

	private readonly Logger _logger = new Logger("MapSelectionSynchronizer", LogType.GameSync);

	private readonly Rng _multiplayerMapPointSelection;

	/// <summary>
	/// Each time we generate a map, this counter goes up. It is used to keep track of multiplayer map votes and to
	/// discard map votes that are for old maps.
	/// </summary>
	public int MapGenerationCount { get; private set; }

	/// <summary>
	/// This is called when the vote changes for any player.
	/// Note that it is called immediately when the local player votes for a map coordinate, and NOT when the vote is
	/// confirmed.
	/// The second argument is the previously voted for map coord (if any). The third is newly voted-for map coord.
	/// </summary>
	public event Action<Player, MapVote?, MapVote?>? PlayerVoteChanged;

	public event Action<Player, MapVote?>? PlayerVoteCancelled;

	public event Action? PlayerVotesCleared;

	public MapSelectionSynchronizer(INetGameService netService, ActionQueueSynchronizer actionQueueSynchronizer, IRunState runState)
	{
		_netService = netService;
		_actionQueueSynchronizer = actionQueueSynchronizer;
		_runState = runState;
		_multiplayerMapPointSelection = new Rng(_runState.Rng.Seed, "map_point_selection");
		OnLocationChanged(_runState.MapLocation);
	}

	public void OnLocalPlayerVotedFor(MapCoord coord)
	{
		MapLocation source = new MapLocation(_runState.CurrentMapCoord, _runState.CurrentActIndex);
		MapVote value = new MapVote
		{
			coord = coord,
			mapGenerationCount = MapGenerationCount
		};
		Player me = LocalContext.GetMe(_runState);
		int playerSlotIndex = _runState.GetPlayerSlotIndex(me);
		MapVote? arg = (_predictedVoteSet ? _predictedVote : _votes[playerSlotIndex]);
		_predictedVote = value;
		_predictedVoteSet = true;
		PlayerVoteChanged?.Invoke(me, arg, value);
		VoteForMapCoordAction action = new VoteForMapCoordAction(LocalContext.GetMe(_runState), source, value);
		_actionQueueSynchronizer.RequestEnqueue(action);
	}

	public void PlayerVotedForMapCoord(Player player, MapLocation source, MapVote? destination)
	{
		if (_acceptingVotesFromSource != source)
		{
			_logger.Warn($"Received map vote from player {player.NetId} for source {source}, but we're currently accepting votes for {_acceptingVotesFromSource}");
			return;
		}
		if (destination?.mapGenerationCount < MapGenerationCount)
		{
			_logger.Warn($"Received map vote from player {player.NetId} for destination {destination}, but the map generation count is lower than our current: {MapGenerationCount}");
			return;
		}
		int playerSlotIndex = _runState.GetPlayerSlotIndex(player);
		MapVote? arg = _votes[playerSlotIndex];
		_votes[playerSlotIndex] = destination;
		if (LocalContext.IsMe(player) && _predictedVoteSet && _predictedVote == destination)
		{
			_predictedVote = null;
			_predictedVoteSet = false;
		}
		else
		{
			PlayerVoteChanged?.Invoke(player, arg, destination);
		}
		if (destination.HasValue)
		{
			_logger.Debug($"Received vote to move to {destination} from player {player.NetId} (slot {playerSlotIndex})");
		}
		else
		{
			_logger.Debug($"Received cancellation of vote from player {player.NetId} (slot {playerSlotIndex})");
		}
		if (_votes.All((MapVote? p) => p.HasValue && p.Value.mapGenerationCount == MapGenerationCount) && _netService.Type != NetGameType.Client)
		{
			_logger.Debug("All votes received and we are host, choosing coordinate");
			MoveToMapCoord();
		}
	}

	public MapVote? GetVote(Player player)
	{
		if (LocalContext.IsMe(player) && _predictedVoteSet)
		{
			return _predictedVote;
		}
		return _votes[_runState.GetPlayerSlotIndex(player)];
	}

	private void MoveToMapCoord()
	{
		if (_netService.Type == NetGameType.Client)
		{
			throw new InvalidOperationException("Only host should be moving to new map point!");
		}
		MapCoord coord = _multiplayerMapPointSelection.NextItem(_votes).Value.coord;
		_acceptingVotesFromSource.coord = coord;
		_logger.Debug($"Moving to coordinate {coord}");
		MoveToMapCoordAction action = new MoveToMapCoordAction(LocalContext.GetMe(_runState), coord);
		_actionQueueSynchronizer.RequestEnqueue(action);
	}

	/// <summary>
	/// Called when moving to a new location.
	/// This differs from <see cref="M:MegaCrit.Sts2.Core.Multiplayer.Game.MapSelectionSynchronizer.BeforeBeginningMoveToNewMapCoord" /> because it is called when the movement actually
	/// occurs.
	/// </summary>
	/// <param name="location"></param>
	public void OnLocationChanged(MapLocation location)
	{
		_acceptingVotesFromSource = location;
		_votes.Clear();
		for (int i = 0; i < _runState.Players.Count; i++)
		{
			_votes.Add(null);
		}
		_predictedVoteSet = false;
		_predictedVote = null;
		PlayerVotesCleared?.Invoke();
	}

	public void BeforeMapGenerated()
	{
		MapGenerationCount++;
		for (int i = 0; i < _votes.Count; i++)
		{
			MapVote? mapVote = _votes[i];
			if (mapVote.HasValue && mapVote.Value.mapGenerationCount < MapGenerationCount)
			{
				Player player = _runState.Players[i];
				_logger.Debug($"Cancelling map vote for player {player.NetId} because the map has re-generated and their vote is old");
				MapVote? arg = _votes[i];
				_votes[i] = null;
				PlayerVoteCancelled?.Invoke(player, arg);
			}
		}
		if (_predictedVoteSet && _predictedVote.HasValue && _predictedVote.Value.mapGenerationCount < MapGenerationCount)
		{
			MapVote? predictedVote = _predictedVote;
			_predictedVoteSet = false;
			_predictedVote = null;
			PlayerVoteCancelled?.Invoke(LocalContext.GetMe(_runState), predictedVote);
		}
	}

	/// <summary>
	/// This is called before any animation begins for moving to the new map coord.
	/// It is used for clearing the predicted local vote, in case it was not accepted by the host.
	/// </summary>
	public void BeforeBeginningMoveToNewMapCoord()
	{
		if (_predictedVoteSet)
		{
			MapVote? predictedVote = _predictedVote;
			_predictedVote = null;
			_predictedVoteSet = false;
			Player me = LocalContext.GetMe(_runState);
			PlayerVoteChanged?.Invoke(me, predictedVote, _votes[_runState.GetPlayerSlotIndex(me)]);
		}
	}
}
