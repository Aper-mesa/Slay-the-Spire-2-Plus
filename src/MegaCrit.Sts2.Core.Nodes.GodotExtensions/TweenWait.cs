using System.Threading;
using System.Threading.Tasks;
using Godot;

namespace MegaCrit.Sts2.Core.Nodes.GodotExtensions;

/// <summary>
/// One wait on a tween's <c>Finished</c> signal, with the early stops that <see cref="M:MegaCrit.Sts2.Core.Nodes.GodotExtensions.TweenHelper.AwaitFinished(Godot.Tween,Godot.Node,System.Threading.CancellationToken)" />
/// documents. The wait can also be resolved from outside through <see cref="M:MegaCrit.Sts2.Core.Nodes.GodotExtensions.TweenWait.Resolve(MegaCrit.Sts2.Core.Nodes.GodotExtensions.TweenOutcome)" />, for an owner that replaces or
/// kills the tween before it finishes.
/// </summary>
internal sealed class TweenWait
{
	private readonly Tween _tween;

	private readonly Node _owner;

	private readonly SceneTree? _tree;

	private readonly bool _finishInline;

	private readonly TaskCompletionSource<TweenOutcome> _tcs;

	private readonly CancellationTokenRegistration _ctr;

	private int _resolved;

	internal Task<TweenOutcome> Task => _tcs.Task;

	/// <summary>
	/// Runs the entry checks. If one fails, <see cref="P:MegaCrit.Sts2.Core.Nodes.GodotExtensions.TweenWait.Task" /> is already completed, nothing is subscribed, and a
	/// later <see cref="M:MegaCrit.Sts2.Core.Nodes.GodotExtensions.TweenWait.Resolve(MegaCrit.Sts2.Core.Nodes.GodotExtensions.TweenOutcome)" /> is a no-op. Otherwise subscribes to <c>Finished</c>, the owner's
	/// <c>TreeExiting</c>, the tree's <c>ProcessFrame</c> and the token, then checks again.
	/// </summary>
	internal TweenWait(Tween tween, Node owner, CancellationToken ct)
	{
		_tween = tween;
		_owner = owner;
		_finishInline = ct.CanBeCanceled;
		_tcs = new TaskCompletionSource<TweenOutcome>((!_finishInline) ? TaskCreationOptions.RunContinuationsAsynchronously : TaskCreationOptions.None);
		if (!tween.IsValid() || !tween.IsRunning())
		{
			CompleteAtEntry(TweenOutcome.Finished);
			return;
		}
		if (ct.IsCancellationRequested)
		{
			CompleteAtEntry(TweenOutcome.Cancelled);
			return;
		}
		if (!GodotObject.IsInstanceValid(owner))
		{
			CompleteAtEntry(TweenOutcome.OwnerFreed);
			return;
		}
		if (!owner.IsInsideTree())
		{
			CompleteAtEntry(TweenOutcome.OwnerExited);
			return;
		}
		_tree = owner.GetTree();
		tween.Finished += OnFinished;
		owner.TreeExiting += OnExiting;
		if (_tree != null)
		{
			_tree.ProcessFrame += OnProcessFrame;
		}
		if (ct.CanBeCanceled)
		{
			_ctr = ct.Register(OnCancelled);
			if (Volatile.Read(in _resolved) != 0)
			{
				_ctr.Dispose();
			}
		}
		if (!tween.IsValid() || !tween.IsRunning())
		{
			OnFinished();
		}
		else if (!owner.IsInsideTree())
		{
			OnExiting();
		}
	}

	/// <summary>
	/// Ends the wait with <paramref name="outcome" />. The first call wins; later calls, and any call on a wait that
	/// ended at its entry checks, do nothing. With a token, <see cref="F:MegaCrit.Sts2.Core.Nodes.GodotExtensions.TweenOutcome.Finished" /> completes inline and
	/// every other outcome completes through a deferred call. Without a token every outcome completes through the
	/// source's asynchronous continuations.
	///
	/// Call this from the main thread. Two paths already leave that to their caller: the token registration runs on
	/// whichever thread calls <c>CancellationTokenSource.Cancel</c>, and the constructor's recheck runs on whichever
	/// thread started the wait.
	/// </summary>
	internal void Resolve(TweenOutcome outcome)
	{
		if (Interlocked.Exchange(ref _resolved, 1) != 0)
		{
			return;
		}
		Unsubscribe();
		if (_finishInline && outcome != TweenOutcome.Finished)
		{
			Callable.From(() => _tcs.TrySetResult(outcome)).CallDeferred();
		}
		else
		{
			_tcs.TrySetResult(outcome);
		}
	}

	private void CompleteAtEntry(TweenOutcome outcome)
	{
		_resolved = 1;
		_tcs.TrySetResult(outcome);
	}

	private void OnProcessFrame()
	{
		if (Volatile.Read(in _resolved) == 0 && !_tween.IsValid())
		{
			OnStopped();
		}
	}

	private void Unsubscribe()
	{
		if (GodotObject.IsInstanceValid(_tween))
		{
			_tween.Finished -= OnFinished;
		}
		if (GodotObject.IsInstanceValid(_owner))
		{
			_owner.TreeExiting -= OnExiting;
		}
		if (_tree != null && GodotObject.IsInstanceValid(_tree))
		{
			_tree.ProcessFrame -= OnProcessFrame;
		}
		_ctr.Dispose();
	}

	private void OnFinished()
	{
		Resolve(TweenOutcome.Finished);
	}

	private void OnStopped()
	{
		Resolve(TweenOutcome.Killed);
	}

	private void OnExiting()
	{
		Resolve(TweenOutcome.OwnerExited);
	}

	private void OnCancelled()
	{
		Resolve(TweenOutcome.Cancelled);
	}
}
