using System;
using MegaCrit.Sts2.Core.Entities.Players;

namespace MegaCrit.Sts2.Core.Entities.FoolArrows;

public class ArrowSlot
{
	public readonly Player owner;

	private ArrowStatus _status;

	public bool IsAvailable => Status == ArrowStatus.Retrieved;

	public ArrowStatus Status
	{
		get
		{
			return _status;
		}
		set
		{
			_status = value;
			ArrowStatusUpdated?.Invoke();
		}
	}

	/// <summary>
	/// This is important to get Quietus to work correctly.
	/// </summary>
	public bool DestroyAfterImpaled { get; set; }

	public event Action? ArrowStatusUpdated;

	public ArrowSlot(Player owner)
	{
		this.owner = owner;
	}
}
