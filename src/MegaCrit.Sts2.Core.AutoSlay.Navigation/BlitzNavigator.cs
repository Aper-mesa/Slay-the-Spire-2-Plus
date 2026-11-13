using System.Threading;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.AutoSlay.Helpers;
using MegaCrit.Sts2.Core.Map;
using MegaCrit.Sts2.Core.Random;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;

namespace MegaCrit.Sts2.Core.AutoSlay.Navigation;

/// <summary>
/// Blitz maps are a single line, and pressing a room's proceed button moves the run to the next node
/// (<see cref="M:MegaCrit.Sts2.Core.Runs.RunManager.Proceed" />), so there is nothing to pick: wait for the next node to be entered.
/// </summary>
public class BlitzNavigator : IRoomNavigator
{
	public async Task AdvanceAsync(RunState runState, RunPosition from, Rng random, CancellationToken ct)
	{
		await WaitHelper.Until(() =>
		{
			if (RunPosition.Of(runState) != from)
			{
				AbstractRoom currentRoom = runState.CurrentRoom;
				if (currentRoom != null)
				{
					return currentRoom.RoomType != RoomType.Unassigned;
				}
				return false;
			}
			return false;
		}, ct, AutoSlayConfig.roomEnterTimeout, () => $"Blitz did not advance from act {from.ActIndex + 1} node {from.Coord}");
		AutoSlayLog.Action($"Blitz advanced to {runState.CurrentRoom.RoomType} ({DescribeNode(runState)})");
	}

	/// <summary>
	/// Nothing to wait for. A Blitz run starts every act on its ancient without opening the map, and the caller has
	/// already waited for the boss room to be replaced, which in Blitz is that ancient.
	/// </summary>
	public Task WaitForNextActAsync(RunState runState, CancellationToken ct)
	{
		return Task.CompletedTask;
	}

	private static string DescribeNode(RunState runState)
	{
		if (runState.Map is BlitzActMap blitzActMap)
		{
			MapPoint currentMapPoint = runState.CurrentMapPoint;
			if (currentMapPoint != null)
			{
				int num = blitzActMap.GetMapPoints().IndexOf(currentMapPoint);
				return $"act {runState.CurrentActIndex + 1} node {num + 1} of {blitzActMap.GetMapPoints().Count}";
			}
		}
		return "off the map";
	}
}
