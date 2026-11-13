using MegaCrit.Sts2.Core.Timeline.Epochs;

namespace MegaCrit.Sts2.Core.Timeline.Stories;

public sealed class FoolStory : StoryModel
{
	protected override string Id => "FOOL";

	public override EpochModel[] Epochs => new EpochModel[7]
	{
		EpochModel.Get<Fool5Epoch>(),
		EpochModel.Get<Fool4Epoch>(),
		EpochModel.Get<Fool6Epoch>(),
		EpochModel.Get<Fool1Epoch>(),
		EpochModel.Get<Fool2Epoch>(),
		EpochModel.Get<Fool3Epoch>(),
		EpochModel.Get<Fool7Epoch>()
	};
}
