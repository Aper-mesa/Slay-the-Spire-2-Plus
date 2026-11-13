using System.Collections.Generic;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace MegaCrit.Sts2.Core.Map;

/// <summary>
/// An ActMap that is reconstructed from saved data rather than generated from RNG.
/// This ensures map topology is preserved even if map generation code changes.
///
/// This single class handles all map types (Standard, Spoils, GoldenPath) because
/// the concrete type only matters during generation. After generation, all maps
/// are simply grids of MapPoints with connections. The serialization captures
/// the complete topology, making the original generation algorithm irrelevant
/// for restoration.
/// </summary>
public sealed class SavedActMap : ActMap
{
	public override MapPoint BossMapPoint { get; }

	public override MapPoint StartingMapPoint { get; }

	public override MapPoint? SecondBossMapPoint { get; }

	protected override MapPoint?[,] Grid { get; }

	public SavedActMap(SerializableActMap saved)
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
		if (saved.SecondBossPoint != null)
		{
			SecondBossMapPoint = MapPoint.FromSerializable(saved.SecondBossPoint);
			dictionary[saved.SecondBossPoint.Coord] = SecondBossMapPoint;
		}
		ActMap.ConnectChildren(saved.Points, dictionary);
		ActMap.ConnectChildren(saved.BossPoint, dictionary);
		ActMap.ConnectChildren(saved.StartingPoint, dictionary);
		if (saved.SecondBossPoint != null)
		{
			ActMap.ConnectChildren(saved.SecondBossPoint, dictionary);
		}
		RestoreStartMapPoints(saved, dictionary);
	}
}
