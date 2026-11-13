using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Characters;
using MegaCrit.Sts2.Core.Nodes.Screens.Timeline;
using MegaCrit.Sts2.Core.Saves;

namespace MegaCrit.Sts2.Core.Timeline.Epochs;

/// <summary>
/// Escape Artist
/// </summary>
public class Fool1Epoch : EpochModel
{
	public override string Id => "FOOL1_EPOCH";

	public override EpochEra Era => EpochEra.Invitation0;

	public override int EraPosition => 2;

	public override string StoryId => "Fool";

	public override EpochModel[] GetTimelineExpansion()
	{
		return new EpochModel[6]
		{
			EpochModel.Get(EpochModel.GetId<Fool2Epoch>()),
			EpochModel.Get(EpochModel.GetId<Fool3Epoch>()),
			EpochModel.Get(EpochModel.GetId<Fool4Epoch>()),
			EpochModel.Get(EpochModel.GetId<Fool5Epoch>()),
			EpochModel.Get(EpochModel.GetId<Fool6Epoch>()),
			EpochModel.Get(EpochModel.GetId<Fool7Epoch>())
		};
	}

	public override void QueueUnlocks()
	{
		NTimelineScreen.Instance.QueueCharacterUnlock<Fool>(this);
		SaveManager.Instance.Progress.PendingCharacterUnlock = ModelDb.Character<Fool>().Id;
		EpochModel.QueueTimelineExpansion(GetTimelineExpansion());
	}
}
