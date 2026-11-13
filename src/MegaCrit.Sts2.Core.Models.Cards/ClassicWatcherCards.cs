using MegaCrit.Sts2.Core.Entities.Cards;

namespace MegaCrit.Sts2.Core.Models.Cards;

public sealed class ClassicWatcherAlpha : ClassicWatcherCardModel
{
    public ClassicWatcherAlpha() : base(1, CardType.Skill, CardRarity.Rare, TargetType.Self, 0, 0, 0, 0, 0, 0, 0, CardKeyword.Exhaust) { }
}

public sealed class ClassicWatcherBattleHymn : ClassicWatcherCardModel
{
    public ClassicWatcherBattleHymn() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self, 0, 0, 1, 0, 0, 0, 0) { }
}

public sealed class ClassicWatcherBlasphemy : ClassicWatcherCardModel
{
    public ClassicWatcherBlasphemy() : base(1, CardType.Skill, CardRarity.Rare, TargetType.Self, 0, 0, 0, 0, 0, 0, 0, CardKeyword.Exhaust) { }
}

public sealed class ClassicWatcherBowlingBash : ClassicWatcherCardModel
{
    public ClassicWatcherBowlingBash() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, 7, 0, 0, 3, 0, 0, 0) { }
}

public sealed class ClassicWatcherBrilliance : ClassicWatcherCardModel
{
    public ClassicWatcherBrilliance() : base(1, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy, 12, 0, 0, 4, 0, 0, 0) { }
}

public sealed class ClassicWatcherCarveReality : ClassicWatcherCardModel
{
    public ClassicWatcherCarveReality() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy, 6, 0, 0, 4, 0, 0, 0) { }
}

public sealed class ClassicWatcherCollect : ClassicWatcherCardModel
{
    public ClassicWatcherCollect() : base(-1, CardType.Skill, CardRarity.Uncommon, TargetType.Self, 0, 0, 0, 0, 0, 0, 0, CardKeyword.Exhaust) { }
}

public sealed class ClassicWatcherConclude : ClassicWatcherCardModel
{
    public ClassicWatcherConclude() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AllEnemies, 12, 0, 0, 4, 0, 0, 0) { }
}

public sealed class ClassicWatcherConjureBlade : ClassicWatcherCardModel
{
    public ClassicWatcherConjureBlade() : base(-1, CardType.Skill, CardRarity.Rare, TargetType.Self, 0, 0, 0, 0, 0, 0, 0, CardKeyword.Exhaust) { }
}

public sealed class ClassicWatcherConsecrate : ClassicWatcherCardModel
{
    public ClassicWatcherConsecrate() : base(0, CardType.Attack, CardRarity.Common, TargetType.AllEnemies, 5, 0, 0, 3, 0, 0, 0) { }
}

public sealed class ClassicWatcherCrescendo : ClassicWatcherCardModel
{
    public ClassicWatcherCrescendo() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self, 0, 0, 0, 0, 0, 0, -1, CardKeyword.Exhaust, CardKeyword.Retain) { }
}

public sealed class ClassicWatcherCrushJoints : ClassicWatcherCardModel
{
    public ClassicWatcherCrushJoints() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, 8, 0, 1, 2, 0, 1, 0) { }
}

public sealed class ClassicWatcherCutThroughFate : ClassicWatcherCardModel
{
    public ClassicWatcherCutThroughFate() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, 7, 0, 2, 2, 0, 1, 0) { }
}

public sealed class ClassicWatcherDeceiveReality : ClassicWatcherCardModel
{
    public ClassicWatcherDeceiveReality() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self, 0, 4, 0, 0, 3, 0, 0) { }
}

public sealed class ClassicWatcherDefend : ClassicWatcherCardModel
{
    public ClassicWatcherDefend() : base(1, CardType.Skill, CardRarity.Basic, TargetType.Self, 0, 5, 0, 0, 3, 0, 0) { }
}

public sealed class ClassicWatcherDeusExMachina : ClassicWatcherCardModel
{
    public ClassicWatcherDeusExMachina() : base(-2, CardType.Skill, CardRarity.Rare, TargetType.Self, 0, 0, 2, 0, 0, 1, 0, CardKeyword.Unplayable) { }
}

public sealed class ClassicWatcherDevaForm : ClassicWatcherCardModel
{
    public ClassicWatcherDevaForm() : base(3, CardType.Power, CardRarity.Rare, TargetType.Self, 0, 0, 1, 0, 0, 0, 0, CardKeyword.Ethereal) { }
}

public sealed class ClassicWatcherDevotion : ClassicWatcherCardModel
{
    public ClassicWatcherDevotion() : base(1, CardType.Power, CardRarity.Rare, TargetType.Self, 0, 0, 2, 0, 0, 1, 0) { }
}

public sealed class ClassicWatcherEmptyBody : ClassicWatcherCardModel
{
    public ClassicWatcherEmptyBody() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self, 0, 7, 0, 0, 3, 0, 0) { }
}

public sealed class ClassicWatcherEmptyFist : ClassicWatcherCardModel
{
    public ClassicWatcherEmptyFist() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, 9, 0, 0, 5, 0, 0, 0) { }
}

public sealed class ClassicWatcherEmptyMind : ClassicWatcherCardModel
{
    public ClassicWatcherEmptyMind() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self, 0, 0, 2, 0, 0, 1, 0) { }
}

public sealed class ClassicWatcherEruption : ClassicWatcherCardModel
{
    public ClassicWatcherEruption() : base(2, CardType.Attack, CardRarity.Basic, TargetType.AnyEnemy, 9, 0, 0, 0, 0, 0, -1) { }
}

public sealed class ClassicWatcherEstablishment : ClassicWatcherCardModel
{
    public ClassicWatcherEstablishment() : base(1, CardType.Power, CardRarity.Rare, TargetType.Self, 0, 0, 1, 0, 0, 0, 0) { }
}

public sealed class ClassicWatcherEvaluate : ClassicWatcherCardModel
{
    public ClassicWatcherEvaluate() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self, 0, 6, 0, 0, 4, 0, 0) { }
}

public sealed class ClassicWatcherFasting : ClassicWatcherCardModel
{
    public ClassicWatcherFasting() : base(2, CardType.Power, CardRarity.Uncommon, TargetType.Self, 0, 0, 3, 0, 0, 1, 0) { }
}

public sealed class ClassicWatcherFearNoEvil : ClassicWatcherCardModel
{
    public ClassicWatcherFearNoEvil() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy, 8, 0, 0, 3, 0, 0, 0) { }
}

public sealed class ClassicWatcherFlurryOfBlows : ClassicWatcherCardModel
{
    public ClassicWatcherFlurryOfBlows() : base(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, 4, 0, 0, 2, 0, 0, 0) { }
}

public sealed class ClassicWatcherFlyingSleeves : ClassicWatcherCardModel
{
    public ClassicWatcherFlyingSleeves() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, 4, 0, 0, 2, 0, 0, 0, CardKeyword.Retain) { }
}

public sealed class ClassicWatcherFollowUp : ClassicWatcherCardModel
{
    public ClassicWatcherFollowUp() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, 7, 0, 0, 4, 0, 0, 0) { }
}

public sealed class ClassicWatcherForeignInfluence : ClassicWatcherCardModel
{
    public ClassicWatcherForeignInfluence() : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self, 0, 0, 0, 0, 0, 0, 0, CardKeyword.Exhaust) { }
}

public sealed class ClassicWatcherForesight : ClassicWatcherCardModel
{
    public ClassicWatcherForesight() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self, 0, 0, 3, 0, 0, 1, 0) { }
}

public sealed class ClassicWatcherHalt : ClassicWatcherCardModel
{
    public ClassicWatcherHalt() : base(0, CardType.Skill, CardRarity.Common, TargetType.Self, 0, 3, 9, 0, 1, 5, 0) { }
}

public sealed class ClassicWatcherIndignation : ClassicWatcherCardModel
{
    public ClassicWatcherIndignation() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self, 0, 0, 3, 0, 0, 2, 0) { }
}

public sealed class ClassicWatcherInnerPeace : ClassicWatcherCardModel
{
    public ClassicWatcherInnerPeace() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self, 0, 0, 3, 0, 0, 1, 0) { }
}

public sealed class ClassicWatcherJudgement : ClassicWatcherCardModel
{
    public ClassicWatcherJudgement() : base(1, CardType.Skill, CardRarity.Rare, TargetType.AnyEnemy, 0, 0, 30, 0, 0, 10, 0) { }
}

public sealed class ClassicWatcherJustLucky : ClassicWatcherCardModel
{
    public ClassicWatcherJustLucky() : base(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, 3, 2, 1, 1, 1, 1, 0) { }
}

public sealed class ClassicWatcherLessonLearned : ClassicWatcherCardModel
{
    public ClassicWatcherLessonLearned() : base(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy, 10, 0, 0, 3, 0, 0, 0, CardKeyword.Exhaust) { }
}

public sealed class ClassicWatcherLikeWater : ClassicWatcherCardModel
{
    public ClassicWatcherLikeWater() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self, 0, 0, 5, 0, 0, 2, 0) { }
}

public sealed class ClassicWatcherMasterReality : ClassicWatcherCardModel
{
    public ClassicWatcherMasterReality() : base(1, CardType.Power, CardRarity.Rare, TargetType.Self, 0, 0, 0, 0, 0, 0, -1) { }
}

public sealed class ClassicWatcherMeditate : ClassicWatcherCardModel
{
    public ClassicWatcherMeditate() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self, 0, 0, 1, 0, 0, 1, 0) { }
}

public sealed class ClassicWatcherMentalFortress : ClassicWatcherCardModel
{
    public ClassicWatcherMentalFortress() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self, 0, 0, 4, 0, 0, 2, 0) { }
}

public sealed class ClassicWatcherNirvana : ClassicWatcherCardModel
{
    public ClassicWatcherNirvana() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self, 0, 0, 3, 0, 0, 1, 0) { }
}

public sealed class ClassicWatcherOmniscience : ClassicWatcherCardModel
{
    public ClassicWatcherOmniscience() : base(4, CardType.Skill, CardRarity.Rare, TargetType.Self, 0, 0, 2, 0, 0, 0, -1, CardKeyword.Exhaust) { }
}

public sealed class ClassicWatcherPerseverance : ClassicWatcherCardModel
{
    public ClassicWatcherPerseverance() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self, 0, 5, 2, 0, 2, 1, 0, CardKeyword.Retain) { }
}

public sealed class ClassicWatcherPray : ClassicWatcherCardModel
{
    public ClassicWatcherPray() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self, 0, 0, 3, 0, 0, 1, 0) { }
}

public sealed class ClassicWatcherPressurePoints : ClassicWatcherCardModel
{
    public ClassicWatcherPressurePoints() : base(1, CardType.Skill, CardRarity.Common, TargetType.AnyEnemy, 0, 0, 8, 0, 0, 3, 0) { }
}

public sealed class ClassicWatcherProstrate : ClassicWatcherCardModel
{
    public ClassicWatcherProstrate() : base(0, CardType.Skill, CardRarity.Common, TargetType.Self, 0, 4, 2, 0, 0, 1, 0) { }
}

public sealed class ClassicWatcherProtect : ClassicWatcherCardModel
{
    public ClassicWatcherProtect() : base(2, CardType.Skill, CardRarity.Common, TargetType.Self, 0, 12, 0, 0, 4, 0, 0, CardKeyword.Retain) { }
}

public sealed class ClassicWatcherRagnarok : ClassicWatcherCardModel
{
    public ClassicWatcherRagnarok() : base(3, CardType.Attack, CardRarity.Rare, TargetType.RandomEnemy, 5, 0, 5, 1, 0, 1, 0) { }
}

public sealed class ClassicWatcherReachHeaven : ClassicWatcherCardModel
{
    public ClassicWatcherReachHeaven() : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy, 10, 0, 0, 5, 0, 0, 0) { }
}

public sealed class ClassicWatcherRushdown : ClassicWatcherCardModel
{
    public ClassicWatcherRushdown() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self, 0, 0, 2, 0, 0, 0, -1) { }
}

public sealed class ClassicWatcherSanctity : ClassicWatcherCardModel
{
    public ClassicWatcherSanctity() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self, 0, 6, 2, 0, 3, 0, 0) { }
}

public sealed class ClassicWatcherSandsOfTime : ClassicWatcherCardModel
{
    public ClassicWatcherSandsOfTime() : base(4, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy, 20, 0, 0, 6, 0, 0, 0, CardKeyword.Retain) { }
}

public sealed class ClassicWatcherSashWhip : ClassicWatcherCardModel
{
    public ClassicWatcherSashWhip() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, 8, 0, 1, 2, 0, 1, 0) { }
}

public sealed class ClassicWatcherScrawl : ClassicWatcherCardModel
{
    public ClassicWatcherScrawl() : base(1, CardType.Skill, CardRarity.Rare, TargetType.Self, 0, 0, 0, 0, 0, 0, -1, CardKeyword.Exhaust) { }
}

public sealed class ClassicWatcherSignatureMove : ClassicWatcherCardModel
{
    public ClassicWatcherSignatureMove() : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy, 30, 0, 0, 10, 0, 0, 0) { }
}

public sealed class ClassicWatcherSimmeringFury : ClassicWatcherCardModel
{
    public ClassicWatcherSimmeringFury() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self, 0, 0, 2, 0, 0, 1, 0) { }
}

public sealed class ClassicWatcherSpiritShield : ClassicWatcherCardModel
{
    public ClassicWatcherSpiritShield() : base(2, CardType.Skill, CardRarity.Rare, TargetType.Self, 0, 0, 3, 0, 0, 1, 0) { }
}

public sealed class ClassicWatcherStrike : ClassicWatcherCardModel
{
    public ClassicWatcherStrike() : base(1, CardType.Attack, CardRarity.Basic, TargetType.AnyEnemy, 6, 0, 0, 3, 0, 0, 0) { }
}

public sealed class ClassicWatcherStudy : ClassicWatcherCardModel
{
    public ClassicWatcherStudy() : base(2, CardType.Power, CardRarity.Uncommon, TargetType.Self, 0, 0, 1, 0, 0, 0, -1) { }
}

public sealed class ClassicWatcherSwivel : ClassicWatcherCardModel
{
    public ClassicWatcherSwivel() : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self, 0, 8, 0, 0, 3, 0, 0) { }
}

public sealed class ClassicWatcherTalkToTheHand : ClassicWatcherCardModel
{
    public ClassicWatcherTalkToTheHand() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy, 5, 0, 2, 2, 0, 1, 0, CardKeyword.Exhaust) { }
}

public sealed class ClassicWatcherTantrum : ClassicWatcherCardModel
{
    public ClassicWatcherTantrum() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy, 3, 0, 3, 0, 0, 1, 0) { }
}

public sealed class ClassicWatcherThirdEye : ClassicWatcherCardModel
{
    public ClassicWatcherThirdEye() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self, 0, 7, 3, 0, 2, 2, 0) { }
}

public sealed class ClassicWatcherTranquility : ClassicWatcherCardModel
{
    public ClassicWatcherTranquility() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self, 0, 0, 0, 0, 0, 0, -1, CardKeyword.Exhaust, CardKeyword.Retain) { }
}

public sealed class ClassicWatcherVault : ClassicWatcherCardModel
{
    public ClassicWatcherVault() : base(3, CardType.Skill, CardRarity.Rare, TargetType.Self, 0, 0, 0, 0, 0, 0, -1, CardKeyword.Exhaust) { }
}

public sealed class ClassicWatcherVigilance : ClassicWatcherCardModel
{
    public ClassicWatcherVigilance() : base(2, CardType.Skill, CardRarity.Basic, TargetType.Self, 0, 8, 0, 0, 4, 0, 0) { }
}

public sealed class ClassicWatcherWallop : ClassicWatcherCardModel
{
    public ClassicWatcherWallop() : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy, 9, 0, 0, 3, 0, 0, 0) { }
}

public sealed class ClassicWatcherWaveOfTheHand : ClassicWatcherCardModel
{
    public ClassicWatcherWaveOfTheHand() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self, 0, 0, 1, 0, 0, 1, 0) { }
}

public sealed class ClassicWatcherWeave : ClassicWatcherCardModel
{
    public ClassicWatcherWeave() : base(0, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy, 4, 0, 0, 2, 0, 0, 0) { }
}

public sealed class ClassicWatcherWheelKick : ClassicWatcherCardModel
{
    public ClassicWatcherWheelKick() : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy, 15, 0, 2, 5, 0, 0, 0) { }
}

public sealed class ClassicWatcherWindmillStrike : ClassicWatcherCardModel
{
    public ClassicWatcherWindmillStrike() : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy, 7, 0, 4, 3, 0, 1, 0, CardKeyword.Retain) { }
}

public sealed class ClassicWatcherWish : ClassicWatcherCardModel
{
    public ClassicWatcherWish() : base(3, CardType.Skill, CardRarity.Rare, TargetType.Self, 3, 6, 25, 1, 2, 5, 0, CardKeyword.Exhaust) { }
}

public sealed class ClassicWatcherWorship : ClassicWatcherCardModel
{
    public ClassicWatcherWorship() : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self, 0, 0, 5, 0, 0, 0, 0) { }
}

public sealed class ClassicWatcherWreathOfFlame : ClassicWatcherCardModel
{
    public ClassicWatcherWreathOfFlame() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self, 0, 0, 5, 0, 0, 3, 0) { }
}

public sealed class ClassicWatcherMiracle : ClassicWatcherCardModel
{
    public ClassicWatcherMiracle() : base(0, CardType.Skill, CardRarity.Token, TargetType.Self, 0, 0, 0, 0, 0, 0, 0, CardKeyword.Exhaust, CardKeyword.Retain) { }
}

public sealed class ClassicWatcherSmite : ClassicWatcherCardModel
{
    public ClassicWatcherSmite() : base(1, CardType.Attack, CardRarity.Token, TargetType.AnyEnemy, 12, 0, 0, 4, 0, 0, 0, CardKeyword.Exhaust, CardKeyword.Retain) { }
}

public sealed class ClassicWatcherSafety : ClassicWatcherCardModel
{
    public ClassicWatcherSafety() : base(1, CardType.Skill, CardRarity.Token, TargetType.Self, 0, 12, 0, 0, 4, 0, 0, CardKeyword.Exhaust, CardKeyword.Retain) { }
}

public sealed class ClassicWatcherInsight : ClassicWatcherCardModel
{
    public ClassicWatcherInsight() : base(0, CardType.Skill, CardRarity.Token, TargetType.Self, 0, 0, 2, 0, 0, 1, 0, CardKeyword.Exhaust, CardKeyword.Retain) { }
}

public sealed class ClassicWatcherBeta : ClassicWatcherCardModel
{
    public ClassicWatcherBeta() : base(2, CardType.Skill, CardRarity.Token, TargetType.Self, 0, 0, 0, 0, 0, 0, -1, CardKeyword.Exhaust) { }
}

public sealed class ClassicWatcherOmega : ClassicWatcherCardModel
{
    public ClassicWatcherOmega() : base(3, CardType.Power, CardRarity.Token, TargetType.Self, 0, 0, 50, 0, 0, 10, 0) { }
}

public sealed class ClassicWatcherExpunger : ClassicWatcherCardModel
{
    public ClassicWatcherExpunger() : base(1, CardType.Attack, CardRarity.Token, TargetType.AnyEnemy, 9, 0, 0, 6, 0, 0, 0) { }
}

public sealed class ClassicWatcherThroughViolence : ClassicWatcherCardModel
{
    public ClassicWatcherThroughViolence() : base(0, CardType.Attack, CardRarity.Token, TargetType.AnyEnemy, 20, 0, 0, 10, 0, 0, 0, CardKeyword.Exhaust, CardKeyword.Retain) { }
}

public sealed class ClassicWatcherBecomeAlmighty : ClassicWatcherCardModel
{
    public ClassicWatcherBecomeAlmighty() : base(-2, CardType.Power, CardRarity.Token, TargetType.Self, 0, 0, 3, 0, 0, 1, 0) { }
}

public sealed class ClassicWatcherFameAndFortune : ClassicWatcherCardModel
{
    public ClassicWatcherFameAndFortune() : base(-2, CardType.Skill, CardRarity.Token, TargetType.Self, 0, 0, 25, 0, 0, 5, 0) { }
}

public sealed class ClassicWatcherLiveForever : ClassicWatcherCardModel
{
    public ClassicWatcherLiveForever() : base(-2, CardType.Power, CardRarity.Token, TargetType.Self, 0, 0, 6, 0, 0, 2, 0) { }
}
