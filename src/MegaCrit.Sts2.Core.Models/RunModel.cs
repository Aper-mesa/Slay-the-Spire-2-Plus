using System;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Runs;

namespace MegaCrit.Sts2.Core.Models;

/// <summary>
/// A model which is instantiated once for an entire run.
/// Models which inherit this are intended to be created directly by RunManager for different modes. There's not much
/// difference in functionality between this and a modifier; mainly semantics.
/// </summary>
public abstract class RunModel : AbstractModel
{
	/// <summary>
	/// The <see cref="P:MegaCrit.Sts2.Core.Models.RunModel.RunState" /> that this model is modifying.
	/// Should always be set when creating a run, so we mark it as non-nullable.
	/// </summary>
	private RunState _runState;

	/// <summary>
	/// The CombatState that this model is modifying.
	/// Only set when we are in combat.
	/// </summary>
	private CombatState? _combatState;

	public override bool ShouldReceiveCombatHooks => true;

	/// <summary>
	/// The <see cref="P:MegaCrit.Sts2.Core.Models.RunModel.RunState" /> that this model is modifying.
	/// </summary>
	protected RunState RunState
	{
		get
		{
			return _runState;
		}
		set
		{
			AssertMutable();
			_runState = value;
		}
	}

	protected CombatState? CombatState
	{
		get
		{
			return _combatState;
		}
		set
		{
			AssertMutable();
			_combatState = value;
		}
	}

	/// <summary>
	/// Called when a run is created with this model in it.
	/// </summary>
	public void OnRunCreated(RunState state)
	{
		if (_runState != null)
		{
			throw new InvalidOperationException("Already initialized");
		}
		_runState = state;
		AfterRunCreated();
	}

	/// <summary>
	/// Called when a run is loaded with this model in it.
	/// </summary>
	public void OnRunLoaded(RunState state)
	{
		if (_runState != null)
		{
			throw new InvalidOperationException("Already initialized");
		}
		_runState = state;
		AfterRunLoaded();
	}

	public virtual void AfterRunCreated()
	{
	}

	public virtual void AfterRunLoaded()
	{
	}

	public void OnCombatEntered(CombatState combatState)
	{
		_combatState = combatState;
	}

	public void OnCombatFinished()
	{
		_combatState = null;
	}
}
