using System.Collections.Generic;
using System.Linq;
using Godot;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Timeline.Epochs;
using MegaCrit.Sts2.Core.Unlocks;

namespace MegaCrit.Sts2.Core.Models.CardPools;

public sealed class ClassicSilentCardPool : CardPoolModel
{
	public override string Title => "silent";

	public override string EnergyColorName => "silent";

	public override string CardFrameMaterialPath => "card_frame_green";

	public override Color DeckEntryCardColor => new Color("5EBD00");

	public override Color EnergyOutlineColor => new Color("1A6625");

	public override bool IsColorless => false;

	protected override CardModel[] GenerateAllCards() =>
	[
		ModelDb.Card<ClassicSilentAccuracy>(),
		ModelDb.Card<ClassicSilentAcrobatics>(),
		ModelDb.Card<ClassicSilentAdrenaline>(),
		ModelDb.Card<ClassicSilentAfterimage>(),
		ModelDb.Card<ClassicSilentAlchemize>(),
		ModelDb.Card<ClassicSilentAllOutAttack>(),
		ModelDb.Card<ClassicSilentAThousandCuts>(),
		ModelDb.Card<ClassicSilentBackflip>(),
		ModelDb.Card<ClassicSilentBackstab>(),
		ModelDb.Card<ClassicSilentBane>(),
		ModelDb.Card<ClassicSilentBladeDance>(),
		ModelDb.Card<ClassicSilentBlur>(),
		ModelDb.Card<ClassicSilentBouncingFlask>(),
		ModelDb.Card<ClassicSilentBulletTime>(),
		ModelDb.Card<ClassicSilentBurst>(),
		ModelDb.Card<ClassicSilentCalculatedGamble>(),
		ModelDb.Card<ClassicSilentCaltrops>(),
		ModelDb.Card<ClassicSilentCatalyst>(),
		ModelDb.Card<ClassicSilentChoke>(),
		ModelDb.Card<ClassicSilentCloakAndDagger>(),
		ModelDb.Card<ClassicSilentConcentrate>(),
		ModelDb.Card<ClassicSilentCorpseExplosion>(),
		ModelDb.Card<ClassicSilentCripplingPoison>(),
		ModelDb.Card<ClassicSilentDaggerSpray>(),
		ModelDb.Card<ClassicSilentDaggerThrow>(),
		ModelDb.Card<ClassicSilentDash>(),
		ModelDb.Card<ClassicSilentDeadlyPoison>(),
		ModelDb.Card<ClassicSilentDefendSilent>(),
		ModelDb.Card<ClassicSilentDeflect>(),
		ModelDb.Card<ClassicSilentDieDieDie>(),
		ModelDb.Card<ClassicSilentDistraction>(),
		ModelDb.Card<ClassicSilentDodgeAndRoll>(),
		ModelDb.Card<ClassicSilentDoppelganger>(),
		ModelDb.Card<ClassicSilentEndlessAgony>(),
		ModelDb.Card<ClassicSilentEnvenom>(),
		ModelDb.Card<ClassicSilentEscapePlan>(),
		ModelDb.Card<ClassicSilentEviscerate>(),
		ModelDb.Card<ClassicSilentExpertise>(),
		ModelDb.Card<ClassicSilentFinisher>(),
		ModelDb.Card<ClassicSilentFlechettes>(),
		ModelDb.Card<ClassicSilentFlyingKnee>(),
		ModelDb.Card<ClassicSilentFootwork>(),
		ModelDb.Card<ClassicSilentGlassKnife>(),
		ModelDb.Card<ClassicSilentGrandFinale>(),
		ModelDb.Card<ClassicSilentHeelHook>(),
		ModelDb.Card<ClassicSilentInfiniteBlades>(),
		ModelDb.Card<ClassicSilentLegSweep>(),
		ModelDb.Card<ClassicSilentMalaise>(),
		ModelDb.Card<ClassicSilentMasterfulStab>(),
		ModelDb.Card<ClassicSilentNeutralize>(),
		ModelDb.Card<ClassicSilentNightmare>(),
		ModelDb.Card<ClassicSilentNoxiousFumes>(),
		ModelDb.Card<ClassicSilentOutmaneuver>(),
		ModelDb.Card<ClassicSilentPhantasmalKiller>(),
		ModelDb.Card<ClassicSilentPiercingWail>(),
		ModelDb.Card<ClassicSilentPoisonedStab>(),
		ModelDb.Card<ClassicSilentPredator>(),
		ModelDb.Card<ClassicSilentPrepared>(),
		ModelDb.Card<ClassicSilentQuickSlash>(),
		ModelDb.Card<ClassicSilentReflex>(),
		ModelDb.Card<ClassicSilentRiddleWithHoles>(),
		ModelDb.Card<ClassicSilentSetup>(),
		ModelDb.Card<ClassicSilentSkewer>(),
		ModelDb.Card<ClassicSilentSlice>(),
		ModelDb.Card<ClassicSilentSneakyStrike>(),
		ModelDb.Card<ClassicSilentStormOfSteel>(),
		ModelDb.Card<ClassicSilentStrikeSilent>(),
		ModelDb.Card<ClassicSilentSuckerPunch>(),
		ModelDb.Card<ClassicSilentSurvivor>(),
		ModelDb.Card<ClassicSilentTactician>(),
		ModelDb.Card<ClassicSilentTerror>(),
		ModelDb.Card<ClassicSilentToolsOfTheTrade>(),
		ModelDb.Card<ClassicSilentUnload>(),
		ModelDb.Card<ClassicSilentWellLaidPlans>(),
		ModelDb.Card<ClassicSilentWraithForm>(),
	];
}
