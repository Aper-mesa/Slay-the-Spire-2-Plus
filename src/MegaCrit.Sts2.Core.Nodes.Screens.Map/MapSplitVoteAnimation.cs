using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using MegaCrit.Sts2.Core.Audio.Debug;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Map;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Random;
using MegaCrit.Sts2.Core.Runs;

namespace MegaCrit.Sts2.Core.Nodes.Screens.Map;

/// <summary>
/// Represents the animation which plays when players don't all vote for the same map point.
/// The amount of state to keep track of was getting a little unwieldy to put directly into the map screen.
/// </summary>
public class MapSplitVoteAnimation
{
	private readonly NMapScreen _mapScreen;

	private readonly RunState _runState;

	private readonly Dictionary<MapCoord, NMapPoint> _mapPointDictionary;

	private int _ticks;

	private Player _winner;

	private List<Player> _sortedPlayers;

	private Player? _currentlyHighlightedPlayer;

	public MapSplitVoteAnimation(NMapScreen mapScreen, RunState runState, Dictionary<MapCoord, NMapPoint> mapPointDictionary)
	{
		_mapScreen = mapScreen;
		_runState = runState;
		_mapPointDictionary = mapPointDictionary;
	}

	/// <summary>
	/// Plays the animation if necessary.
	/// The animation is not played if all players voted for the same coordinate.
	/// </summary>
	public async Task TryPlay(MapCoord selectedCoord)
	{
		MapCoord? mapCoord = null;
		bool flag = true;
		List<Player> list = new List<Player>();
		foreach (Player player in _runState.Players)
		{
			MapCoord? mapCoord2 = RunManager.Instance.MapSelectionSynchronizer.GetVote(player)?.coord;
			if (!mapCoord.HasValue)
			{
				mapCoord = mapCoord2;
			}
			else if (mapCoord2 != mapCoord)
			{
				flag = false;
			}
			if (mapCoord2 == selectedCoord)
			{
				list.Add(player);
			}
		}
		if (!flag)
		{
			Rng rng = new Rng(_runState.Rng.Seed + (ulong)_runState.ActFloor);
			_ticks = rng.NextInt(12, 18);
			float num = rng.NextFloat(0.05f, 0.3f);
			_winner = rng.NextItem(list);
			_sortedPlayers = (from p in (from player in _runState.Players
					select (player: player, coord: RunManager.Instance.MapSelectionSynchronizer.GetVote(player)?.coord) into p
					where p.coord.HasValue
					select p).OrderBy(((Player player, MapCoord? coord) p) => p.coord, Comparer<MapCoord?>.Create(MapCoordComparer)).ThenBy(((Player player, MapCoord? coord) p) => _mapPointDictionary[p.coord.Value].VoteContainer.GetVoteIndex(p.player))
				select p.player).ToList();
			Tween tween = _mapScreen.CreateTween();
			tween.TweenMethod(Callable.From<float>(TickSplitVoteAnimation), 0f, 1f, 1.2000000476837158).SetTrans(Tween.TransitionType.Sine).SetEase(Tween.EaseType.Out);
			tween.TweenInterval(num);
			if (await tween.AwaitFinished(_mapScreen) == TweenOutcome.Finished)
			{
				_mapPointDictionary[selectedCoord].VoteContainer.BouncePlayers();
			}
		}
	}

	private int MapCoordComparer(MapCoord? a, MapCoord? b)
	{
		if (a == b)
		{
			return 0;
		}
		if (!a.HasValue)
		{
			return 1;
		}
		if (!b.HasValue)
		{
			return -1;
		}
		if (a.Value.col != b.Value.col)
		{
			return a.Value.col.CompareTo(b.Value.col);
		}
		if (a.Value.row != b.Value.row)
		{
			return a.Value.row.CompareTo(b.Value.row);
		}
		return 0;
	}

	private void TickSplitVoteAnimation(float value)
	{
		int num = Mathf.RoundToInt(value * (float)_ticks);
		int num2 = _sortedPlayers.IndexOf(_winner) - (_ticks - num);
		int index = (num2 % _sortedPlayers.Count + _sortedPlayers.Count) % _sortedPlayers.Count;
		Player player = _sortedPlayers[index];
		if (_currentlyHighlightedPlayer != player)
		{
			HighlightPlayer(_sortedPlayers[index]);
			NDebugAudioManager.Instance.Play("map_split_tick.mp3", 0.15f, PitchVariance.Small);
		}
	}

	private void HighlightPlayer(Player? player)
	{
		if (_currentlyHighlightedPlayer != player)
		{
			if (_currentlyHighlightedPlayer != null)
			{
				MapCoord coord = RunManager.Instance.MapSelectionSynchronizer.GetVote(_currentlyHighlightedPlayer).Value.coord;
				_mapPointDictionary[coord].VoteContainer.SetPlayerHighlighted(_currentlyHighlightedPlayer, isHighlighted: false);
			}
			_currentlyHighlightedPlayer = player;
			if (player != null)
			{
				MapCoord coord2 = RunManager.Instance.MapSelectionSynchronizer.GetVote(player).Value.coord;
				_mapPointDictionary[coord2].VoteContainer.SetPlayerHighlighted(player, isHighlighted: true);
			}
		}
	}
}
