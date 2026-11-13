using System.Collections.Generic;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace MegaCrit.Sts2.Core.Map;

/// <summary>
/// Handles the creation of map points for a map.
/// </summary>
public abstract class ActMap
{
	public readonly HashSet<MapPoint> startMapPoints = new HashSet<MapPoint>();

	public abstract MapPoint BossMapPoint { get; }

	public abstract MapPoint StartingMapPoint { get; }

	/// <summary>
	/// The second boss map point for Double Boss mode (Ascension 10+).
	/// Null if not in double boss mode.
	/// </summary>
	public virtual MapPoint? SecondBossMapPoint => null;

	protected abstract MapPoint?[,] Grid { get; }

	/// <summary>
	/// Connects parent and child points from saved data rather than from generation code, so a map restored
	/// from a save keeps the exact topology it was saved with. Children absent from the lookup are skipped.
	/// </summary>
	protected static void ConnectChildren(IEnumerable<SerializableMapPoint> points, Dictionary<MapCoord, MapPoint> lookup)
	{
		foreach (SerializableMapPoint point in points)
		{
			ConnectChildren(point, lookup);
		}
	}

	/// <inheritdoc cref="M:MegaCrit.Sts2.Core.Map.ActMap.ConnectChildren(System.Collections.Generic.IEnumerable{MegaCrit.Sts2.Core.Saves.Runs.SerializableMapPoint},System.Collections.Generic.Dictionary{MegaCrit.Sts2.Core.Map.MapCoord,MegaCrit.Sts2.Core.Map.MapPoint})" />
	protected static void ConnectChildren(SerializableMapPoint savedPoint, Dictionary<MapCoord, MapPoint> lookup)
	{
		if (savedPoint.ChildCoords == null)
		{
			return;
		}
		MapPoint mapPoint = lookup[savedPoint.Coord];
		foreach (MapCoord childCoord in savedPoint.ChildCoords)
		{
			if (lookup.TryGetValue(childCoord, out MapPoint value))
			{
				mapPoint.AddChildPoint(value);
			}
		}
	}

	/// <summary>
	/// Restores the start points a saved map recorded. Coords with no point in the lookup are skipped.
	/// </summary>
	protected void RestoreStartMapPoints(SerializableActMap saved, Dictionary<MapCoord, MapPoint> lookup)
	{
		if (saved.StartMapPointCoords == null)
		{
			return;
		}
		foreach (MapCoord startMapPointCoord in saved.StartMapPointCoords)
		{
			if (lookup.TryGetValue(startMapPointCoord, out MapPoint value))
			{
				startMapPoints.Add(value);
			}
		}
	}

	public int GetColumnCount()
	{
		return Grid.GetLength(0);
	}

	public int GetRowCount()
	{
		return Grid.GetLength(1);
	}

	public IEnumerable<MapPoint> GetAllMapPoints()
	{
		for (int c = 0; c < GetColumnCount(); c++)
		{
			for (int r = 0; r < Grid.GetLength(1); r++)
			{
				MapPoint mapPoint = Grid[c, r];
				if (mapPoint != null)
				{
					yield return mapPoint;
				}
			}
		}
	}

	public IEnumerable<MapPoint> GetPointsInRow(int row)
	{
		if (row < 0 || row >= Grid.GetLength(1))
		{
			yield break;
		}
		for (int c = 0; c < GetColumnCount(); c++)
		{
			MapPoint mapPoint = Grid[c, row];
			if (mapPoint != null)
			{
				yield return mapPoint;
			}
		}
	}

	public virtual MapPoint? GetPoint(MapCoord coord)
	{
		return GetPoint(coord.col, coord.row);
	}

	public MapPoint? GetPoint(int col, int row)
	{
		if (col == BossMapPoint.coord.col && row == BossMapPoint.coord.row)
		{
			return BossMapPoint;
		}
		if (SecondBossMapPoint != null && col == SecondBossMapPoint.coord.col && row == SecondBossMapPoint.coord.row)
		{
			return SecondBossMapPoint;
		}
		if (col == StartingMapPoint.coord.col && row == StartingMapPoint.coord.row)
		{
			return StartingMapPoint;
		}
		if (col >= 0 && col < Grid.GetLength(0) && row >= 0 && row < Grid.GetLength(1))
		{
			return Grid[col, row];
		}
		return null;
	}

	public bool IsInMap(MapPoint mapPoint)
	{
		if (mapPoint.PointType == MapPointType.Ancient || mapPoint.PointType == MapPointType.Boss)
		{
			return true;
		}
		int col = mapPoint.coord.col;
		int row = mapPoint.coord.row;
		if (col < 0 || col >= Grid.GetLength(0) || row < 0 || row >= Grid.GetLength(1))
		{
			return false;
		}
		return Grid[col, row] != null;
	}

	public bool HasPoint(MapCoord coord)
	{
		if (coord.col == BossMapPoint.coord.col && coord.row == BossMapPoint.coord.row)
		{
			return true;
		}
		if (SecondBossMapPoint != null && coord.col == SecondBossMapPoint.coord.col && coord.row == SecondBossMapPoint.coord.row)
		{
			return true;
		}
		if (coord.col == StartingMapPoint.coord.col && coord.row == StartingMapPoint.coord.row)
		{
			return true;
		}
		if (coord.col < 0 || coord.col >= Grid.GetLength(0))
		{
			return false;
		}
		if (coord.row < 0 || coord.row >= Grid.GetLength(1))
		{
			return false;
		}
		return Grid[coord.col, coord.row] != null;
	}
}
