using System.Collections.Generic;

namespace MegaCrit.Sts2.Core.DevConsole;

public class FixedSizedQueue<T> : List<T>
{
	private readonly int _limit;

	public FixedSizedQueue(int limit)
	{
		_limit = limit;
	}

	public void Enqueue(T obj)
	{
		if (Count >= _limit)
		{
			RemoveAt(Count - 1);
		}
		Insert(0, obj);
	}
}
