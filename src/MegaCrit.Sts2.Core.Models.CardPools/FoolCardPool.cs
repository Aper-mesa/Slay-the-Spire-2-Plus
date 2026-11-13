using System.Collections.Generic;
using System.Linq;
using Godot;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Timeline.Epochs;
using MegaCrit.Sts2.Core.Unlocks;

namespace MegaCrit.Sts2.Core.Models.CardPools;

public sealed class FoolCardPool : CardPoolModel
{
	public override string Title => "fool";

	public override string EnergyColorName => "fool";

	public override string CardFrameMaterialPath => "card_frame_yellow";

	public override Color DeckEntryCardColor => new Color("D62000");

	public override Color EnergyOutlineColor => new Color("5a4411");

	public override bool IsColorless => false;

	protected override CardModel[] GenerateAllCards()
	{
		return new CardModel[90]
		{
			ModelDb.Card<AllIsWell>(),
			ModelDb.Card<Anguish>(),
			ModelDb.Card<Archery>(),
			ModelDb.Card<ArrowStab>(),
			ModelDb.Card<BlossomShot>(),
			ModelDb.Card<BlotOutTheSky>(),
			ModelDb.Card<Bounce>(),
			ModelDb.Card<BowSmack>(),
			ModelDb.Card<Brace>(),
			ModelDb.Card<BrightenUp>(),
			ModelDb.Card<ChoBarbs>(),
			ModelDb.Card<ChoChorus>(),
			ModelDb.Card<ChoFirm>(),
			ModelDb.Card<ChoKi>(),
			ModelDb.Card<ChoLimbs>(),
			ModelDb.Card<ChoReady>(),
			ModelDb.Card<Coda>(),
			ModelDb.Card<Compost>(),
			ModelDb.Card<CoveringFire>(),
			ModelDb.Card<DeepStretch>(),
			ModelDb.Card<DefendFool>(),
			ModelDb.Card<DislikeYou>(),
			ModelDb.Card<DivineShot>(),
			ModelDb.Card<Ecstasy>(),
			ModelDb.Card<Encore>(),
			ModelDb.Card<Ennui>(),
			ModelDb.Card<Enrapture>(),
			ModelDb.Card<Ensemble>(),
			ModelDb.Card<EpicRetrieval>(),
			ModelDb.Card<ExplosiveArrows>(),
			ModelDb.Card<Florescence>(),
			ModelDb.Card<Growth>(),
			ModelDb.Card<HarmonicShield>(),
			ModelDb.Card<HeavyShot>(),
			ModelDb.Card<HiddenRoots>(),
			ModelDb.Card<HobblingShot>(),
			ModelDb.Card<Honing>(),
			ModelDb.Card<Jest>(),
			ModelDb.Card<KiChant>(),
			ModelDb.Card<KiHop>(),
			ModelDb.Card<KiRest>(),
			ModelDb.Card<KiVerse>(),
			ModelDb.Card<KiWhip>(),
			ModelDb.Card<LockedIn>(),
			ModelDb.Card<FoolishShot>(),
			ModelDb.Card<LostLute>(),
			ModelDb.Card<Lullaby>(),
			ModelDb.Card<MelodicShot>(),
			ModelDb.Card<Motif>(),
			ModelDb.Card<MourningBlow>(),
			ModelDb.Card<MythicForm>(),
			ModelDb.Card<Nail>(),
			ModelDb.Card<Nock>(),
			ModelDb.Card<PunchOfJoy>(),
			ModelDb.Card<QuickShot>(),
			ModelDb.Card<Quietus>(),
			ModelDb.Card<Quiver>(),
			ModelDb.Card<RainShot>(),
			ModelDb.Card<RapidShot>(),
			ModelDb.Card<Recall>(),
			ModelDb.Card<RepeatShot>(),
			ModelDb.Card<RhythmStrike>(),
			ModelDb.Card<Rummage>(),
			ModelDb.Card<SelfDeprecate>(),
			ModelDb.Card<SerratedArrows>(),
			ModelDb.Card<ShowOff>(),
			ModelDb.Card<Simpleton>(),
			ModelDb.Card<SkyShot>(),
			ModelDb.Card<SongPath>(),
			ModelDb.Card<SorrowfulStrike>(),
			ModelDb.Card<SteadyCadence>(),
			ModelDb.Card<StrikeFool>(),
			ModelDb.Card<Timber>(),
			ModelDb.Card<TrickShot>(),
			ModelDb.Card<Tune>(),
			ModelDb.Card<Uproot>(),
			ModelDb.Card<ViolentChord>(),
			ModelDb.Card<Vitriol>(),
			ModelDb.Card<WahBlast>(),
			ModelDb.Card<WahCho>(),
			ModelDb.Card<WahChoKi>(),
			ModelDb.Card<WahHammer>(),
			ModelDb.Card<WahKi>(),
			ModelDb.Card<WahSymphony>(),
			ModelDb.Card<WahSweep>(),
			ModelDb.Card<WahTouch>(),
			ModelDb.Card<WhistlingArrows>(),
			ModelDb.Card<Wrest>(),
			ModelDb.Card<Yah>(),
			ModelDb.Card<Yoga>()
		};
	}

	protected override IEnumerable<CardModel> FilterThroughEpochs(UnlockState unlockState, IEnumerable<CardModel> cards)
	{
		List<CardModel> list = cards.ToList();
		if (!unlockState.IsEpochRevealed<Fool2Epoch>())
		{
			list.RemoveAll((CardModel c) => Fool2Epoch.Cards.Any((CardModel card) => card.Id == c.Id));
		}
		if (!unlockState.IsEpochRevealed<Fool5Epoch>())
		{
			list.RemoveAll((CardModel c) => Fool5Epoch.Cards.Any((CardModel card) => card.Id == c.Id));
		}
		if (!unlockState.IsEpochRevealed<Fool7Epoch>())
		{
			list.RemoveAll((CardModel c) => Fool7Epoch.Cards.Any((CardModel card) => card.Id == c.Id));
		}
		return list;
	}
}
