using System.Collections.Generic;
using System.Linq;
using Godot;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Timeline.Epochs;
using MegaCrit.Sts2.Core.Unlocks;

namespace MegaCrit.Sts2.Core.Models.CardPools;

public sealed class ClassicWatcherCardPool : CardPoolModel
{
	public override string Title => "necrobinder";

	public override string EnergyColorName => "necrobinder";

	public override string CardFrameMaterialPath => "card_frame_pink";

	public override Color DeckEntryCardColor => new Color("CD4EED");

	public override Color EnergyOutlineColor => new Color("803367");

	public override bool IsColorless => false;

    protected override CardModel[] GenerateAllCards() => [
        ModelDb.Card<ClassicWatcherAlpha>(),
        ModelDb.Card<ClassicWatcherBattleHymn>(),
        ModelDb.Card<ClassicWatcherBlasphemy>(),
        ModelDb.Card<ClassicWatcherBowlingBash>(),
        ModelDb.Card<ClassicWatcherBrilliance>(),
        ModelDb.Card<ClassicWatcherCarveReality>(),
        ModelDb.Card<ClassicWatcherCollect>(),
        ModelDb.Card<ClassicWatcherConclude>(),
        ModelDb.Card<ClassicWatcherConjureBlade>(),
        ModelDb.Card<ClassicWatcherConsecrate>(),
        ModelDb.Card<ClassicWatcherCrescendo>(),
        ModelDb.Card<ClassicWatcherCrushJoints>(),
        ModelDb.Card<ClassicWatcherCutThroughFate>(),
        ModelDb.Card<ClassicWatcherDeceiveReality>(),
        ModelDb.Card<ClassicWatcherDefend>(),
        ModelDb.Card<ClassicWatcherDeusExMachina>(),
        ModelDb.Card<ClassicWatcherDevaForm>(),
        ModelDb.Card<ClassicWatcherDevotion>(),
        ModelDb.Card<ClassicWatcherEmptyBody>(),
        ModelDb.Card<ClassicWatcherEmptyFist>(),
        ModelDb.Card<ClassicWatcherEmptyMind>(),
        ModelDb.Card<ClassicWatcherEruption>(),
        ModelDb.Card<ClassicWatcherEstablishment>(),
        ModelDb.Card<ClassicWatcherEvaluate>(),
        ModelDb.Card<ClassicWatcherFasting>(),
        ModelDb.Card<ClassicWatcherFearNoEvil>(),
        ModelDb.Card<ClassicWatcherFlurryOfBlows>(),
        ModelDb.Card<ClassicWatcherFlyingSleeves>(),
        ModelDb.Card<ClassicWatcherFollowUp>(),
        ModelDb.Card<ClassicWatcherForeignInfluence>(),
        ModelDb.Card<ClassicWatcherForesight>(),
        ModelDb.Card<ClassicWatcherHalt>(),
        ModelDb.Card<ClassicWatcherIndignation>(),
        ModelDb.Card<ClassicWatcherInnerPeace>(),
        ModelDb.Card<ClassicWatcherJudgement>(),
        ModelDb.Card<ClassicWatcherJustLucky>(),
        ModelDb.Card<ClassicWatcherLessonLearned>(),
        ModelDb.Card<ClassicWatcherLikeWater>(),
        ModelDb.Card<ClassicWatcherMasterReality>(),
        ModelDb.Card<ClassicWatcherMeditate>(),
        ModelDb.Card<ClassicWatcherMentalFortress>(),
        ModelDb.Card<ClassicWatcherNirvana>(),
        ModelDb.Card<ClassicWatcherOmniscience>(),
        ModelDb.Card<ClassicWatcherPerseverance>(),
        ModelDb.Card<ClassicWatcherPray>(),
        ModelDb.Card<ClassicWatcherPressurePoints>(),
        ModelDb.Card<ClassicWatcherProstrate>(),
        ModelDb.Card<ClassicWatcherProtect>(),
        ModelDb.Card<ClassicWatcherRagnarok>(),
        ModelDb.Card<ClassicWatcherReachHeaven>(),
        ModelDb.Card<ClassicWatcherRushdown>(),
        ModelDb.Card<ClassicWatcherSanctity>(),
        ModelDb.Card<ClassicWatcherSandsOfTime>(),
        ModelDb.Card<ClassicWatcherSashWhip>(),
        ModelDb.Card<ClassicWatcherScrawl>(),
        ModelDb.Card<ClassicWatcherSignatureMove>(),
        ModelDb.Card<ClassicWatcherSimmeringFury>(),
        ModelDb.Card<ClassicWatcherSpiritShield>(),
        ModelDb.Card<ClassicWatcherStrike>(),
        ModelDb.Card<ClassicWatcherStudy>(),
        ModelDb.Card<ClassicWatcherSwivel>(),
        ModelDb.Card<ClassicWatcherTalkToTheHand>(),
        ModelDb.Card<ClassicWatcherTantrum>(),
        ModelDb.Card<ClassicWatcherThirdEye>(),
        ModelDb.Card<ClassicWatcherTranquility>(),
        ModelDb.Card<ClassicWatcherVault>(),
        ModelDb.Card<ClassicWatcherVigilance>(),
        ModelDb.Card<ClassicWatcherWallop>(),
        ModelDb.Card<ClassicWatcherWaveOfTheHand>(),
        ModelDb.Card<ClassicWatcherWeave>(),
        ModelDb.Card<ClassicWatcherWheelKick>(),
        ModelDb.Card<ClassicWatcherWindmillStrike>(),
        ModelDb.Card<ClassicWatcherWish>(),
        ModelDb.Card<ClassicWatcherWorship>(),
        ModelDb.Card<ClassicWatcherWreathOfFlame>(),
        ModelDb.Card<ClassicWatcherMiracle>(),
        ModelDb.Card<ClassicWatcherSmite>(),
        ModelDb.Card<ClassicWatcherSafety>(),
        ModelDb.Card<ClassicWatcherInsight>(),
        ModelDb.Card<ClassicWatcherBeta>(),
        ModelDb.Card<ClassicWatcherOmega>(),
        ModelDb.Card<ClassicWatcherExpunger>(),
        ModelDb.Card<ClassicWatcherThroughViolence>(),
        ModelDb.Card<ClassicWatcherBecomeAlmighty>(),
        ModelDb.Card<ClassicWatcherFameAndFortune>(),
        ModelDb.Card<ClassicWatcherLiveForever>(),
    ];
}
