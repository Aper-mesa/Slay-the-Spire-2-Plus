using System;
using System.Collections.Generic;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.TestSupport;

namespace MegaCrit.Sts2.Core.CardSelection;

public class CardSelectorStack
{
	private sealed class StackedSelectorScope : IDisposable
	{
		private readonly ICardSelector _selector;

		private readonly Stack<ICardSelector> _stack;

		private bool _disposed;

		public StackedSelectorScope(Stack<ICardSelector> stack, ICardSelector selector)
		{
			_stack = stack;
			_selector = selector;
		}

		public void Dispose()
		{
			if (!_disposed)
			{
				_disposed = true;
				if (_stack.Count > 0 && _stack.Peek() == _selector)
				{
					_stack.Pop();
				}
			}
		}
	}

	/// <summary>
	/// Used by <see cref="M:MegaCrit.Sts2.Core.CardSelection.CardSelectorStack.SuspendTestSelector" /> to temporarily suspend the current card selector so it can be
	/// restored later.
	/// </summary>
	private sealed class RestoreTestSelectorScope : IDisposable
	{
		private readonly ICardSelector _saved;

		private bool _disposed;

		public RestoreTestSelectorScope(ICardSelector saved)
		{
			_saved = saved;
		}

		public void Dispose()
		{
			if (!_disposed)
			{
				_disposed = true;
				SetTestSelector(_saved);
			}
		}
	}

	private sealed class SelectorScope : IDisposable
	{
		private readonly Stack<ICardSelector> _stack;

		private bool _disposed;

		public SelectorScope(Stack<ICardSelector> stack)
		{
			_stack = stack;
		}

		public void Dispose()
		{
			if (!_disposed)
			{
				_disposed = true;
				_stack.Clear();
			}
		}
	}

	private sealed class TestSelectorScope : IDisposable
	{
		private bool _disposed;

		public void Dispose()
		{
			if (!_disposed)
			{
				_disposed = true;
				_testSelector = null;
			}
		}
	}

	private sealed class LocalSelectorScope : IDisposable
	{
		private readonly CardSelectorStack _stack;

		private bool _disposed;

		public LocalSelectorScope(CardSelectorStack stack)
		{
			_stack = stack;
		}

		public void Dispose()
		{
			if (!_disposed)
			{
				_disposed = true;
				_stack._localSelector = null;
			}
		}
	}

	/// <summary>
	/// A selector applied to all players, for testing.
	/// This acts as a non-local selector that is applied to the bottom of all player selector stacks.
	/// </summary>
	private static ICardSelector? _testSelector;

	/// <summary>
	/// "Local" selector, also for test purposes.
	/// This selector operates during local card selection only. It kicks in only when the local player is asked to make
	/// a choice, and the choice is propagated to other players via the PlayerChoiceSynchronizer. This is in opposition
	/// to the normal selector stacks, which automatically select deterministically across all peers.
	/// </summary>
	private ICardSelector? _localSelector;

	/// <summary>
	/// Selector stacks.
	/// There is one per player, indexed by player slot index.
	/// </summary>
	private readonly List<Stack<ICardSelector>> _selectorStacks = new List<Stack<ICardSelector>>();

	private readonly IPlayerCollection _playerCollection;

	/// <summary>
	/// The currently active automated card selector used in tests.
	/// This selector kicks in only when the local player is asked to make a choice, and the choice is propagated to
	/// other players via the normal player choice route. This is in opposition to the normal selector stacks, which
	/// automatically select deterministically across all peers.
	/// This is normally reserved for testing purposes.
	/// </summary>
	public ICardSelector? LocalSelector => _localSelector;

	public CardSelectorStack(IPlayerCollection playerCollection)
	{
		_playerCollection = playerCollection;
		EnsureSelectorStacksExist();
	}

	/// <summary>
	/// Get the currently active automated card selector for the passed player.
	/// Used by AutoSlay and gameplay effects that auto-play cards (e.g., WhisperingEarring).
	/// When set, card selection UI is bypassed and cards are selected automatically.
	/// </summary>
	/// <returns>The top of the stack of selectors. If no selector is set, then the currently active test selector is
	/// returned.</returns>
	public ICardSelector? GetSelector(Player player)
	{
		Stack<ICardSelector> selectorStackForPlayer = GetSelectorStackForPlayer(player);
		if (selectorStackForPlayer.Count == 0)
		{
			return _testSelector;
		}
		return selectorStackForPlayer.Peek();
	}

	/// <summary>
	/// Set the test selector.
	/// This is static so that it carries through the entire game session, regardless of run.
	/// </summary>
	public static IDisposable SetTestSelector(ICardSelector selector)
	{
		if (_testSelector != null)
		{
			throw new InvalidOperationException($"A test card selector {_testSelector} is already set!");
		}
		_testSelector = selector;
		return new TestSelectorScope();
	}

	/// <summary>
	/// Suspends the current test selector.
	/// It will be reinstalled when the returned disposable is disposed.
	/// </summary>
	public static IDisposable SuspendTestSelector()
	{
		if (_testSelector == null)
		{
			throw new InvalidOperationException("There is no test selector to suspend!");
		}
		ICardSelector testSelector = _testSelector;
		_testSelector = null;
		return new RestoreTestSelectorScope(testSelector);
	}

	/// <summary>
	/// Sets the automated card selector for the given player.
	/// When the returned scope is disposed, all selectors are cleared.
	/// Throws if a selector is already active (use <see cref="M:MegaCrit.Sts2.Core.CardSelection.CardSelectorStack.PushSelector(MegaCrit.Sts2.Core.Entities.Players.Player,MegaCrit.Sts2.Core.TestSupport.ICardSelector)" /> for stacking behavior).
	/// </summary>
	public IDisposable UseSelector(Player player, ICardSelector selector)
	{
		Stack<ICardSelector> selectorStackForPlayer = GetSelectorStackForPlayer(player);
		if (selectorStackForPlayer.Count > 0)
		{
			throw new InvalidOperationException("A card selector is already active.");
		}
		selectorStackForPlayer.Push(selector);
		return new SelectorScope(selectorStackForPlayer);
	}

	/// <summary>
	/// Pushes a new selector onto the stack. When disposed, the previous selector is restored.
	/// Use this when you need temporary selector behavior (e.g., WhisperingEarring's auto-play).
	/// </summary>
	public IDisposable PushSelector(Player player, ICardSelector selector)
	{
		Stack<ICardSelector> selectorStackForPlayer = GetSelectorStackForPlayer(player);
		selectorStackForPlayer.Push(selector);
		return new StackedSelectorScope(selectorStackForPlayer, selector);
	}

	/// <summary>
	/// Installs the passed selector for testing. See <see cref="P:MegaCrit.Sts2.Core.CardSelection.CardSelectorStack.LocalSelector" />.
	/// </summary>
	public IDisposable UseLocalSelector(ICardSelector selector)
	{
		if (_localSelector != null)
		{
			throw new InvalidOperationException("A card selector is already active.");
		}
		_localSelector = selector;
		return new LocalSelectorScope(this);
	}

	private Stack<ICardSelector> GetSelectorStackForPlayer(Player player)
	{
		EnsureSelectorStacksExist();
		int playerSlotIndex = _playerCollection.GetPlayerSlotIndex(player);
		if (playerSlotIndex < 0 || playerSlotIndex >= _selectorStacks.Count)
		{
			throw new InvalidOperationException($"Couldn't get selector stack for player {player.NetId}!");
		}
		return _selectorStacks[playerSlotIndex];
	}

	private void EnsureSelectorStacksExist()
	{
		for (int i = 0; i < _playerCollection.Players.Count; i++)
		{
			if (i >= _selectorStacks.Count)
			{
				_selectorStacks.Add(new Stack<ICardSelector>());
			}
		}
	}
}
