using System;
using System.Collections.Generic;
using MegaCrit.Sts2.Core.Saves;
using MegaCrit.Sts2.Core.Timeline;
using MegaCrit.Sts2.Core.Timeline.Epochs;

namespace MegaCrit.Sts2.Core.AutoSlay;

/// <summary>
/// Progress AutoSlay forces onto its profile before touching the main menu. CI starts every run from an empty
/// profile, so anything gated behind an epoch that is not listed here is never played.
/// </summary>
public static class AutoSlayUnlocks
{
	private static IReadOnlyList<string> CharacterEpochIds => new _003C_003Ez__ReadOnlyArray<string>(new string[5]
	{
		EpochModel.GetId<Silent1Epoch>(),
		EpochModel.GetId<Regent1Epoch>(),
		EpochModel.GetId<Defect1Epoch>(),
		EpochModel.GetId<Necrobinder1Epoch>(),
		EpochModel.GetId<Fool1Epoch>()
	});

	/// <summary>
	/// One-time popups to mark seen. <c>SetFtuesEnabled(false)</c> does not suppress these, and one left open
	/// blocks the screen under it.
	/// </summary>
	private static IReadOnlyList<string> SeenPopupIds => new _003C_003Ez__ReadOnlyArray<string>(new string[3] { "ascension_singleplayer_ftue", "blitz_disclaimer", "dread_disclaimer" });

	/// <summary>
	/// Epochs revealed at startup for <paramref name="mode" />. <c>AutoSlayUnlocksTest</c> fails if a character in
	/// <see cref="P:MegaCrit.Sts2.Core.Models.ModelDb.AllCharacters" /> stays locked under any mode's list.
	/// </summary>
	/// <remarks>
	/// Blitz and Dread also get <see cref="T:MegaCrit.Sts2.Core.Timeline.Epochs.NeowEpoch" />: a player can only open either mode after finishing a run,
	/// which reveals it, and act 1 has no other ancient for Blitz to start on. Standard's list does not include it, so
	/// its CI runs keep starting like a first run.
	/// </remarks>
	public static IReadOnlyList<string> EpochIdsFor(AutoSlayMode mode)
	{
		switch (mode)
		{
		case AutoSlayMode.Standard:
			return CharacterEpochIds;
		case AutoSlayMode.Blitz:
		{
			IReadOnlyList<string> characterEpochIds2 = CharacterEpochIds;
			int num2 = 0;
			string[] array2 = new string[2 + characterEpochIds2.Count];
			foreach (string item in characterEpochIds2)
			{
				array2[num2] = item;
				num2++;
			}
			array2[num2] = EpochModel.GetId<NeowEpoch>();
			num2++;
			array2[num2] = EpochModel.GetId<BlitzEpoch>();
			return new _003C_003Ez__ReadOnlyArray<string>(array2);
		}
		case AutoSlayMode.Dread:
		{
			IReadOnlyList<string> characterEpochIds = CharacterEpochIds;
			int num = 0;
			string[] array = new string[2 + characterEpochIds.Count];
			foreach (string item2 in characterEpochIds)
			{
				array[num] = item2;
				num++;
			}
			array[num] = EpochModel.GetId<NeowEpoch>();
			num++;
			array[num] = EpochModel.GetId<DreadEpoch>();
			return new _003C_003Ez__ReadOnlyArray<string>(array);
		}
		default:
			throw new ArgumentOutOfRangeException("mode", mode, null);
		}
	}

	/// <summary>Reveals every epoch <paramref name="mode" /> needs and marks the blocking popups seen.</summary>
	public static void Apply(ProgressState progress, AutoSlayMode mode)
	{
		foreach (string item in EpochIdsFor(mode))
		{
			progress.ObtainEpochOverride(item, EpochState.Revealed);
		}
		foreach (string seenPopupId in SeenPopupIds)
		{
			progress.MarkFtueAsComplete(seenPopupId);
		}
	}
}
