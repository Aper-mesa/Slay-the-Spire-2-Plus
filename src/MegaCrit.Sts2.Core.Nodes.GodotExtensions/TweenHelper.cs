using System.Threading;
using System.Threading.Tasks;
using Godot;

namespace MegaCrit.Sts2.Core.Nodes.GodotExtensions;

public static class TweenHelper
{
	public static void FastForwardToCompletion(this Tween t)
	{
		t.CustomStep(999999999.0);
	}

	/// <summary>
	/// Awaits the tween's <see cref="F:Godot.Tween.SignalName.Finished" /> signal, returning early when the tween can no longer
	/// finish or when <paramref name="owner" /> leaves the tree. Unlike <c>ToSignal(tween, Tween.SignalName.Finished)</c>,
	/// this will not hang when the tween stops without finishing.
	/// </summary>
	/// <remarks>
	/// The engine emits <c>finished</c> from one place only, at the end of <c>Tween::step</c>
	/// (<c>scene/animation/tween.cpp</c>). Four things stop a tween before that point and emit nothing:
	/// <c>Tween.Kill()</c>, the bound node being freed, the "started with no Tweeners" guard, and the infinite-loop
	/// guard (debug builds only, it is inside <c>DEBUG_ENABLED</c>). All four leave <c>valid == false</c>. Either
	/// <c>kill()</c> sets it, or the <c>clear()</c> that <c>SceneTree::process_tweens</c> runs when <c>step</c>
	/// returns false sets it. Polling <see cref="M:Godot.Tween.IsValid" /> once a frame therefore detects all four, one frame
	/// late, as long as the engine still steps the tween. It does not step a tween whose bound node cannot process,
	/// or a tween that pauses with a paused tree (<c>Tween::can_process</c>), so a node freed while the tree is
	/// paused is not cleared until the tree resumes. <paramref name="owner" />'s <c>TreeExiting</c> covers that, since
	/// freeing a node removes it from the tree first. Do NOT add <see cref="M:Godot.Tween.IsRunning" /> to that poll.
	/// <c>stop()</c> and <c>pause()</c> clear it on a tween that can still resume, and it stays true after the bound
	/// node is freed.
	///
	/// A tween that finishes normally never satisfies that poll. The step that finishes a tween emits
	/// <c>Finished</c> while <c>step</c> still returns true, so <c>clear()</c> runs on a later step, and
	/// <c>IsValid()</c> is still true when the signal arrives. The signal completes the await and removes the poll at
	/// once. This holds for idle tweens, which <c>SceneTree::process</c> steps after it emits <c>process_frame</c>,
	/// and for physics tweens, which <c>SceneTree::physics_process</c> steps.
	///
	/// A tween that stops part way through returns <see cref="F:MegaCrit.Sts2.Core.Nodes.GodotExtensions.TweenOutcome.Killed" />, not
	/// <see cref="F:MegaCrit.Sts2.Core.Nodes.GodotExtensions.TweenOutcome.Finished" />. Of the four stops above,
	/// <c>Kill()</c> is the one game code does on purpose: callers kill a tween part way through to start a
	/// different animation. The caller that was waiting on the old animation must stop, not continue while the new one
	/// runs. A tween started with no Tweeners is an engine error; fix that where the tween is created.
	///
	/// The <c>IsValid</c> poll does not detect one case: a bound node that leaves the tree but is not freed. The tween
	/// then waits, still valid, until the node returns. <paramref name="owner" />'s <c>TreeExiting</c> covers that
	/// case, so the owner must be the node the tween is bound to.
	///
	/// Awaiting the signal directly through <c>ToSignal</c> also leaks: the engine removes a one-shot connection only
	/// when it emits the signal (<c>object.cpp</c>), so the awaiter, its async state machine and the tween stay
	/// reachable from the tween's signal map for the rest of the process.
	/// </remarks>
	/// <param name="tween">The tween to await.</param>
	/// <param name="owner">
	/// The node the tween is bound to, which is the node whose <c>CreateTween()</c> made it.
	/// </param>
	/// <param name="ct">
	/// An optional second stop condition. A cancelled token returns <see cref="F:MegaCrit.Sts2.Core.Nodes.GodotExtensions.TweenOutcome.Cancelled" /> instead of
	/// throwing, so it behaves like a freed owner and the caller needs only one early return. A caller that passes a
	/// token also resumes inside the <c>Finished</c> emission, in the same frame, where a caller without one resumes
	/// after the frame draws.
	/// </param>
	/// <returns>
	/// <see cref="F:MegaCrit.Sts2.Core.Nodes.GodotExtensions.TweenOutcome.Finished" /> if the tween finished normally and the owner is still alive and inside the
	/// tree when the caller resumes. Otherwise the reason the wait ended: <see cref="F:MegaCrit.Sts2.Core.Nodes.GodotExtensions.TweenOutcome.Killed" /> if the
	/// tween stopped without finishing, <see cref="F:MegaCrit.Sts2.Core.Nodes.GodotExtensions.TweenOutcome.OwnerExited" /> if the owner left the tree,
	/// <see cref="F:MegaCrit.Sts2.Core.Nodes.GodotExtensions.TweenOutcome.Cancelled" /> if <paramref name="ct" /> was cancelled, and
	/// <see cref="F:MegaCrit.Sts2.Core.Nodes.GodotExtensions.TweenOutcome.OwnerFreed" /> if the owner was freed, at entry or between the <c>Finished</c> signal
	/// and the caller resuming. A caller that gets anything other than <see cref="F:MegaCrit.Sts2.Core.Nodes.GodotExtensions.TweenOutcome.Finished" /> must
	/// return without doing its remaining work.
	///
	/// A tween that is already invalid when it is passed in returns <see cref="F:MegaCrit.Sts2.Core.Nodes.GodotExtensions.TweenOutcome.Finished" />, because an
	/// invalid tween has almost always finished normally before the caller reached the await.
	/// </returns>
	public static async Task<TweenOutcome> AwaitFinished(this Tween tween, Node owner, CancellationToken ct = default(CancellationToken))
	{
		return AfterResume(owner, await new TweenWait(tween, owner, ct).Task);
	}

	/// <summary>
	/// Checks the owner again once the caller resumes, so that the outcome reflects the owner's state at that point.
	/// </summary>
	internal static TweenOutcome AfterResume(Node owner, TweenOutcome outcome)
	{
		if (!GodotObject.IsInstanceValid(owner))
		{
			return TweenOutcome.OwnerFreed;
		}
		if (outcome == TweenOutcome.Finished && !owner.IsInsideTree())
		{
			return TweenOutcome.OwnerExited;
		}
		return outcome;
	}
}
