using System;

namespace MegaCrit.Sts2.Core.Saves;

/// <summary>
/// Disposable scope returned by <see cref="M:MegaCrit.Sts2.Core.Saves.SaveManager.BeginSaveBatch" />.
/// Calls <see cref="M:MegaCrit.Sts2.Core.Saves.SaveManager.EndSaveBatch" /> on dispose, allowing batch usage with <c>using</c>.
/// </summary>
public readonly struct SaveBatchScope(SaveManager saveManager) : IDisposable
{
	public void Dispose()
	{
		saveManager.EndSaveBatch();
	}
}
