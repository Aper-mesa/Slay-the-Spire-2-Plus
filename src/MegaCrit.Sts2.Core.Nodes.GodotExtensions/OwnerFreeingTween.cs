using System;
using System.Threading;
using System.Threading.Tasks;
using Godot;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes.Pooling;

namespace MegaCrit.Sts2.Core.Nodes.GodotExtensions;

/// <summary>
/// An <see cref="T:MegaCrit.Sts2.Core.Nodes.GodotExtensions.OwnedTween" /> for an animation whose end removes the node, such as a fade out. Every
/// <see cref="M:MegaCrit.Sts2.Core.Nodes.GodotExtensions.OwnerFreeingTween.Play(System.Action{Godot.Tween},System.Threading.CancellationToken)" /> queues the owner for deletion when that playback's end is what should free it.
/// </summary>
/// <remarks>
/// This is a separate type, and not a second method on <see cref="T:MegaCrit.Sts2.Core.Nodes.GodotExtensions.OwnedTween" />, because a playback that reports
/// <see cref="F:MegaCrit.Sts2.Core.Nodes.GodotExtensions.TweenOutcome.Replaced" /> never frees: the replacement's playback frees instead. If one field could start
/// both a playback that frees and a playback that does not, a playback that does not free could replace one that does,
/// and the owner would stay alive. Here every playback frees, so that cannot be written.
///
/// Construct one per animation that must not overlap itself, in the owner's constructor, with the owner as the
/// argument, as for an <see cref="T:MegaCrit.Sts2.Core.Nodes.GodotExtensions.OwnedTween" />.
///
/// The owner must not be an <c>IPoolable</c>. <see cref="M:MegaCrit.Sts2.Core.Nodes.GodotExtensions.OwnerFreeingTween.Play(System.Action{Godot.Tween},System.Threading.CancellationToken)" /> does not queue-free a pooled node:
/// <c>QueueFreeSafely</c> takes its <c>IPoolable</c> branch, so the node goes back to the pool. When something else
/// returns the node to the pool during a playback, the node leaves the tree, the playback ends, and this class would
/// return the node a second time. The constructor throws for a pooled owner. A node below a pooled node is allowed: it
/// is queue-freed out of its parent like any other node.
///
/// Only the latest playback of one instance frees. Two <see cref="T:MegaCrit.Sts2.Core.Nodes.GodotExtensions.OwnerFreeingTween" /> fields on one node each free the
/// node once. That is harmless for the plain nodes this class supports:
/// <c>SceneTree::queue_delete</c> pushes the id twice and <c>_flush_delete_queue</c> skips the second.
/// </remarks>
public sealed class OwnerFreeingTween
{
	private readonly Node _owner;

	private readonly OwnedTween _tween;

	/// <exception cref="T:System.ArgumentException"><paramref name="owner" /> is an <c>IPoolable</c>.</exception>
	public OwnerFreeingTween(Node owner)
	{
		if (owner is IPoolable)
		{
			throw new ArgumentException($"{owner.GetType().Name} is pooled, and an {"OwnerFreeingTween"} would return it to the pool a second time. Use an {"OwnedTween"} and return the node to the pool yourself.", "owner");
		}
		_owner = owner;
		_tween = new OwnedTween(owner);
	}

	/// <summary>
	/// <see cref="M:MegaCrit.Sts2.Core.Nodes.GodotExtensions.OwnedTween.Play(System.Action{Godot.Tween},System.Threading.CancellationToken)" />, then queues the owner for deletion when this playback's end is what should free
	/// it.
	/// </summary>
	/// <remarks>
	/// The owner is freed on <see cref="F:MegaCrit.Sts2.Core.Nodes.GodotExtensions.TweenOutcome.Finished" />, <see cref="F:MegaCrit.Sts2.Core.Nodes.GodotExtensions.TweenOutcome.Killed" />,
	/// <see cref="F:MegaCrit.Sts2.Core.Nodes.GodotExtensions.TweenOutcome.OwnerExited" /> and <see cref="F:MegaCrit.Sts2.Core.Nodes.GodotExtensions.TweenOutcome.Cancelled" />. A
	/// <see cref="F:MegaCrit.Sts2.Core.Nodes.GodotExtensions.TweenOutcome.Replaced" /> playback does not free: the replacement's playback does. Every playback that
	/// is not the latest one reports <see cref="F:MegaCrit.Sts2.Core.Nodes.GodotExtensions.TweenOutcome.Replaced" />, so only the latest playback can free.
	/// A <see cref="F:MegaCrit.Sts2.Core.Nodes.GodotExtensions.TweenOutcome.OwnerFreed" /> playback has nothing left to free. A
	/// <see cref="F:MegaCrit.Sts2.Core.Nodes.GodotExtensions.TweenOutcome.Cancelled" /> playback does free, because a token passed here is the caller's own stop
	/// for an animation whose end is the only thing that frees this node; without the free the node would stay in the
	/// tree, invisible, with its tween killed.
	///
	/// The owner is also freed when <paramref name="build" /> throws, and the exception is rethrown. If a playback that
	/// <paramref name="build" /> started is still playing at the throw, that playback frees the owner when it ends.
	///
	/// The table is a switch expression with no discard arm on purpose. A new <see cref="T:MegaCrit.Sts2.Core.Nodes.GodotExtensions.TweenOutcome" /> member is
	/// then a build error here, not a silent leak.
	/// </remarks>
	/// <param name="build">As in <see cref="M:MegaCrit.Sts2.Core.Nodes.GodotExtensions.OwnedTween.Play(System.Action{Godot.Tween},System.Threading.CancellationToken)" />.</param>
	/// <param name="ct">As in <see cref="M:MegaCrit.Sts2.Core.Nodes.GodotExtensions.OwnedTween.Play(System.Action{Godot.Tween},System.Threading.CancellationToken)" />.</param>
	public async Task Play(Action<Tween> build, CancellationToken ct = default(CancellationToken))
	{
		TweenOutcome tweenOutcome;
		try
		{
			tweenOutcome = await _tween.Play(build, ct);
		}
		catch
		{
			if (!_tween.IsPlaying)
			{
				_owner.QueueFreeSafely();
			}
			throw;
		}
		if (tweenOutcome switch
		{
			TweenOutcome.Finished => true,
			TweenOutcome.Killed => true,
			TweenOutcome.OwnerExited => true,
			TweenOutcome.Cancelled => true,
			TweenOutcome.Replaced => false,
			TweenOutcome.OwnerFreed => false,
		})
		{
			_owner.QueueFreeSafely();
		}
	}

	/// <summary>
	/// Ends the current playback with <see cref="F:MegaCrit.Sts2.Core.Nodes.GodotExtensions.TweenOutcome.Killed" /> and kills its tween. That playback then frees
	/// the owner when it resumes, unless a newer playback has started. A no-op when no playback is pending.
	/// </summary>
	public void Kill()
	{
		_tween.Kill();
	}
}
