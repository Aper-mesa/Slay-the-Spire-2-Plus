using System.Collections.Generic;
using System.Linq;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace MegaCrit.Sts2.Core.Map;

public sealed class BlitzActMap : ActMap
{
	public static readonly MapPointType[] pointTypes = new MapPointType[6]
	{
		MapPointType.Monster,
		MapPointType.Monster,
		MapPointType.Shop,
		MapPointType.Elite,
		MapPointType.Treasure,
		MapPointType.RestSite
	};

	private const int _width = 7;

	private const int _middle = 3;

	public override MapPoint BossMapPoint { get; }

	public override MapPoint StartingMapPoint { get; }

	public override MapPoint? SecondBossMapPoint { get; }

	protected override MapPoint?[,] Grid { get; }

	public BlitzActMap(IRunState state)
	{
		int num = pointTypes.Length;
		Grid = new MapPoint[7, num + 1];
		BossMapPoint = new MapPoint(3, num + 1)
		{
			PointType = MapPointType.Boss
		};
		StartingMapPoint = new MapPoint(3, 0)
		{
			PointType = MapPointType.Ancient
		};
		if (state.Act.HasSecondBoss)
		{
			SecondBossMapPoint = new MapPoint(3, num + 2)
			{
				PointType = MapPointType.Boss
			};
		}
		ConnectChildren();
	}

	public BlitzActMap(SerializableActMap saved, IRunState state)
	{
		Grid = new MapPoint[saved.GridWidth, saved.GridHeight];
		Dictionary<MapCoord, MapPoint> dictionary = new Dictionary<MapCoord, MapPoint>();
		foreach (SerializableMapPoint point in saved.Points)
		{
			MapPoint mapPoint = MapPoint.FromSerializable(point);
			Grid[point.Coord.col, point.Coord.row] = mapPoint;
			dictionary[point.Coord] = mapPoint;
		}
		BossMapPoint = MapPoint.FromSerializable(saved.BossPoint);
		dictionary[saved.BossPoint.Coord] = BossMapPoint;
		StartingMapPoint = MapPoint.FromSerializable(saved.StartingPoint);
		dictionary[saved.StartingPoint.Coord] = StartingMapPoint;
		if (saved.SecondBossPoint != null && state.Act.HasSecondBoss)
		{
			SecondBossMapPoint = MapPoint.FromSerializable(saved.SecondBossPoint);
			dictionary[saved.SecondBossPoint.Coord] = SecondBossMapPoint;
		}
		ActMap.ConnectChildren(saved.Points, dictionary);
		ActMap.ConnectChildren(saved.BossPoint, dictionary);
		ActMap.ConnectChildren(saved.StartingPoint, dictionary);
		if (SecondBossMapPoint != null)
		{
			ActMap.ConnectChildren(saved.SecondBossPoint, dictionary);
		}
		RestoreStartMapPoints(saved, dictionary);
	}

	/// <summary>
	/// Since the lightning map is a single line, this returns the map points as a single list.
	/// It follows the links from the starting point, so a map loaded from a save keeps the layout it was saved with.
	/// </summary>
	public List<MapPoint> GetMapPoints()
	{
		List<MapPoint> list = new List<MapPoint>();
		for (MapPoint mapPoint = StartingMapPoint; mapPoint != null; mapPoint = mapPoint.Children.FirstOrDefault())
		{
			list.Add(mapPoint);
		}
		return list;
	}

	private void ConnectChildren()
	{
		for (int i = 0; i < pointTypes.Length; i++)
		{
			MapPoint mapPoint = new MapPoint(3, i + 1);
			Grid[3, i + 1] = mapPoint;
			mapPoint.PointType = pointTypes[i];
			if (i > 0)
			{
				Grid[3, i]?.AddChildPoint(mapPoint);
			}
		}
		Grid[3, pointTypes.Length]?.AddChildPoint(BossMapPoint);
		StartingMapPoint.AddChildPoint(Grid[3, 1]);
		if (SecondBossMapPoint != null)
		{
			BossMapPoint.AddChildPoint(SecondBossMapPoint);
		}
	}
}
