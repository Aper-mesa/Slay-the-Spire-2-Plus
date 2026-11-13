namespace MegaCrit.Sts2.Core.Nodes.GodotExtensions;

/// <summary>
/// Why a wait on a tween ended. <see cref="M:MegaCrit.Sts2.Core.Nodes.GodotExtensions.TweenHelper.AwaitFinished(Godot.Tween,Godot.Node,System.Threading.CancellationToken)" /> returns one of these.
/// </summary>
public enum TweenOutcome
{
	/// <summary>The tween ran to its end, and the owner is alive and inside the tree at resume.</summary>
	Finished,
	/// <summary>A later OwnedTween.Play on the same OwnedTween started before the caller resumed.</summary>
	Replaced,
	/// <summary>The tween stopped without finishing: Kill(), OwnedTween.Kill(), or an engine clear.</summary>
	Killed,
	/// <summary>The owner left the tree during the wait, or was outside it at entry or at resume.</summary>
	OwnerExited,
	/// <summary>The owner was freed: at entry, or by the time the caller resumed.</summary>
	OwnerFreed,
	/// <summary>The token was cancelled.</summary>
	Cancelled
}
