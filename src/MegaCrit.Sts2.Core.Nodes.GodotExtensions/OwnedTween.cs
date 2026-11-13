using System;
using System.Threading;
using System.Threading.Tasks;
using Godot;

namespace MegaCrit.Sts2.Core.Nodes.GodotExtensions;

/// <summary>
/// A tween on a node that must not play on top of itself, and the wait on it. Every <see cref="M:MegaCrit.Sts2.Core.Nodes.GodotExtensions.OwnedTween.Play(System.Action{Godot.Tween},System.Threading.CancellationToken)" /> stops the
/// playback before it, and the wait reports why it ended instead of hanging or returning a bare bool. It replaces the
/// "kill the field, reassign it, await it" pattern.
/// </summary>
/// <remarks>
/// Construct one per animation that must not overlap itself, in the owner's constructor, with the owner as the
/// argument. Two animations that may play at the same time get two instances.
///
/// It is connected to the owner's <c>TreeExiting</c> only while a playback is in flight: <see cref="M:MegaCrit.Sts2.Core.Nodes.GodotExtensions.OwnedTween.Play(System.Action{Godot.Tween},System.Threading.CancellationToken)" /> connects,
/// and the connection is removed when the latest playback's caller resumes. A node between playbacks carries no
/// connection, so the owner can be a pooled node or sit below one. Returning a node to the pool removes every
/// connection on it and on every node below it (<c>NodePool.DisconnectIncomingAndOutgoingSignals</c>), and
/// <c>QueueFreeSafely</c> takes the node out of the tree first, which ends any playback.
///
/// A <see cref="M:MegaCrit.Sts2.Core.Nodes.GodotExtensions.OwnedTween.Play(System.Action{Godot.Tween},System.Threading.CancellationToken)" /> started from a handler of the owner's own <c>TreeExiting</c> is not ended by that exit: the
/// engine has already chosen the handlers it will call. A playback started from <c>_ExitTree</c> is, because
/// <c>_ExitTree</c> runs before the signal.
///
/// For an animation whose end frees the owner, such as a fade out, use an <see cref="T:MegaCrit.Sts2.Core.Nodes.GodotExtensions.OwnerFreeingTween" />. This class
/// never frees the owner.
/// </remarks>
public sealed class OwnedTween
{
	private readonly Node _owner;

	private Tween? _tween;

	private TweenWait? _wait;

	private int _playNumber;

	private TweenOutcome _lastStopReason;

	private readonly Callable _onOwnerExiting;

	/// <summary>
	/// True while the latest playback has a tween that was not stopped. Set when <see cref="M:MegaCrit.Sts2.Core.Nodes.GodotExtensions.OwnedTween.Play(System.Action{Godot.Tween},System.Threading.CancellationToken)" /> creates the tween,
	/// and cleared when the playback is stopped, when its <c>build</c> throws, or when its caller resumes. False for a
	/// playback that was stopped and whose caller has not resumed yet.
	/// </summary>
	internal bool IsPlaying => _tween != null;

	public OwnedTween(Node owner)
	{
		_owner = owner;
		_onOwnerExiting = Callable.From(OnOwnerExiting);
	}

	/// <summary>
	/// Stops the playback before this one, then starts a new tween on the owner and waits for it.
	/// </summary>
	/// <param name="build">
	/// Adds the tweeners to the new tween. Not called when the owner is freed or outside the tree: those playbacks end
	/// at once with <see cref="F:MegaCrit.Sts2.Core.Nodes.GodotExtensions.TweenOutcome.OwnerFreed" /> or <see cref="F:MegaCrit.Sts2.Core.Nodes.GodotExtensions.TweenOutcome.OwnerExited" />.
	/// </param>
	/// <param name="ct">
	/// An optional second stop condition. A cancelled token ends the playback with <see cref="F:MegaCrit.Sts2.Core.Nodes.GodotExtensions.TweenOutcome.Cancelled" />
	/// and kills the tween. The resume mode follows the token as in <see cref="M:MegaCrit.Sts2.Core.Nodes.GodotExtensions.TweenHelper.AwaitFinished(Godot.Tween,Godot.Node,System.Threading.CancellationToken)" />: a caller
	/// that passes a token resumes inside the <c>Finished</c> emission, in the same frame, and a caller without one
	/// resumes after the frame draws.
	/// </param>
	/// <returns>
	/// <see cref="F:MegaCrit.Sts2.Core.Nodes.GodotExtensions.TweenOutcome.Finished" /> if the tween reached its end and the owner is alive and inside the tree at
	/// resume; the caller continues. Otherwise why the playback ended, and the caller returns without its remaining
	/// work: <see cref="F:MegaCrit.Sts2.Core.Nodes.GodotExtensions.TweenOutcome.Replaced" /> if a later <see cref="M:MegaCrit.Sts2.Core.Nodes.GodotExtensions.OwnedTween.Play(System.Action{Godot.Tween},System.Threading.CancellationToken)" /> started,
	/// <see cref="F:MegaCrit.Sts2.Core.Nodes.GodotExtensions.TweenOutcome.Killed" /> if <see cref="M:MegaCrit.Sts2.Core.Nodes.GodotExtensions.OwnedTween.Kill" /> stopped it or the engine cleared the tween,
	/// <see cref="F:MegaCrit.Sts2.Core.Nodes.GodotExtensions.TweenOutcome.OwnerExited" /> if the owner left the tree, <see cref="F:MegaCrit.Sts2.Core.Nodes.GodotExtensions.TweenOutcome.OwnerFreed" /> if it
	/// was freed, and <see cref="F:MegaCrit.Sts2.Core.Nodes.GodotExtensions.TweenOutcome.Cancelled" /> if <paramref name="ct" /> was cancelled.
	///
	/// A later <see cref="M:MegaCrit.Sts2.Core.Nodes.GodotExtensions.OwnedTween.Play(System.Action{Godot.Tween},System.Threading.CancellationToken)" /> gives <see cref="F:MegaCrit.Sts2.Core.Nodes.GodotExtensions.TweenOutcome.Replaced" /> whatever else happened, up to the moment
	/// the caller resumes. That includes a playback started from inside <paramref name="build" />, and a playback
	/// started after the tween finished and before a caller without a token resumed.
	/// </returns>
	public Task<TweenOutcome> Play(Action<Tween> build, CancellationToken ct = default(CancellationToken))
	{
		int num = ++_playNumber;
		Stop(TweenOutcome.Replaced);
		if (!GodotObject.IsInstanceValid(_owner))
		{
			return Task.FromResult(TweenOutcome.OwnerFreed);
		}
		if (!_owner.IsInsideTree())
		{
			DisconnectFromOwnerExit();
			return Task.FromResult(TweenOutcome.OwnerExited);
		}
		ConnectToOwnerExit();
		Tween tween = (_tween = _owner.CreateTween());
		try
		{
			build(tween);
		}
		catch
		{
			if (_playNumber == num)
			{
				_tween = null;
				if (GodotObject.IsInstanceValid(tween))
				{
					tween.Kill();
				}
				DisconnectFromOwnerExit();
			}
			throw;
		}
		if (_playNumber != num)
		{
			return Task.FromResult(TweenOutcome.Replaced);
		}
		if (_tween != tween)
		{
			DisconnectFromOwnerExit();
			return Task.FromResult(TweenHelper.AfterResume(_owner, _lastStopReason));
		}
		return Await(_wait = new TweenWait(tween, _owner, ct), tween, num);
	}

	private async Task<TweenOutcome> Await(TweenWait wait, Tween tween, int playback)
	{
		Node owner = _owner;
		TweenOutcome tweenOutcome = TweenHelper.AfterResume(owner, await wait.Task);
		bool flag = _wait == wait;
		if (flag)
		{
			_wait = null;
			_tween = null;
			if (tweenOutcome != TweenOutcome.Finished && GodotObject.IsInstanceValid(tween))
			{
				tween.Kill();
			}
		}
		if (_playNumber != playback)
		{
			return TweenOutcome.Replaced;
		}
		DisconnectFromOwnerExit();
		if (!flag && tweenOutcome == TweenOutcome.Finished)
		{
			return _lastStopReason;
		}
		return tweenOutcome;
	}

	private void ConnectToOwnerExit()
	{
		if (!_owner.IsConnected(Node.SignalName.TreeExiting, _onOwnerExiting))
		{
			_owner.Connect(Node.SignalName.TreeExiting, _onOwnerExiting);
		}
	}

	private void DisconnectFromOwnerExit()
	{
		if (GodotObject.IsInstanceValid(_owner) && _owner.IsConnected(Node.SignalName.TreeExiting, _onOwnerExiting))
		{
			_owner.Disconnect(Node.SignalName.TreeExiting, _onOwnerExiting);
		}
	}

	/// <summary>
	/// Ends the current playback with <see cref="F:MegaCrit.Sts2.Core.Nodes.GodotExtensions.TweenOutcome.Killed" /> and kills its tween. A no-op when no playback
	/// is pending.
	/// </summary>
	public void Kill()
	{
		Stop(TweenOutcome.Killed);
	}

	private void OnOwnerExiting()
	{
		Stop(TweenOutcome.OwnerExited);
	}

	private void Stop(TweenOutcome reason)
	{
		TweenWait wait = _wait;
		Tween tween = _tween;
		_wait = null;
		_tween = null;
		if (wait != null || tween != null)
		{
			_lastStopReason = reason;
		}
		wait?.Resolve(reason);
		if (tween != null && GodotObject.IsInstanceValid(tween))
		{
			tween.Kill();
		}
	}
}
