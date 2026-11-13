using MegaCrit.Sts2.Core.Map;
using MegaCrit.Sts2.Core.Nodes.Screens.Map;
using MegaCrit.Sts2.Core.Runs;

namespace MegaCrit.Sts2.Core.AutoSlay.Navigation;

/// <summary>The act and map node the run is at.</summary>
/// <remarks>
/// Compared by node rather than by room object, because an event that starts a fight pushes a combat room on top
/// of the event room without moving the run anywhere.
/// </remarks>
public readonly record struct RunPosition(int ActIndex, MapCoord? Coord)
{
	public static RunPosition Of(RunState runState)
	{
		return new RunPosition(runState.CurrentActIndex, runState.CurrentMapCoord);
	}

	/// <summary>
	/// Whether the run has finished with the room at <paramref name="from" />: the map is open to pick the next room
	/// (standard and Dread), or the run has already moved to another node (Blitz, which never opens the map).
	/// </summary>
	public static bool HasLeft(RunState runState, RunPosition from)
	{
		NMapScreen? instance = NMapScreen.Instance;
		if (instance == null || !instance.IsOpen)
		{
			return Of(runState) != from;
		}
		return true;
	}
}
