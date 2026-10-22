using System.Collections.Generic;
using System.Linq;
using Godot;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Unlocks;

namespace MegaCrit.Sts2.Core.Models.CardPools;

/// <summary>
/// [apermesa] Card pool for the classic (Slay the Spire 1) Defect.
/// The title stays "defect" so card art and energy icons reuse the Defect assets.
/// </summary>
public sealed class ClassicDefectCardPool : CardPoolModel
{
	public override string Title => "defect";

	public override string EnergyColorName => "defect";

	public override string CardFrameMaterialPath => "card_frame_blue";

	public override Color DeckEntryCardColor => new Color("3EB3ED");

	public override Color EnergyOutlineColor => new Color("1D5673");

	public override bool IsColorless => false;

	protected override CardModel[] GenerateAllCards()
	{
		return new CardModel[75]
		{
			ModelDb.Card<ClassicStrike>(),
			ModelDb.Card<ClassicDefend>(),
			ModelDb.Card<ClassicZap>(),
			ModelDb.Card<ClassicDualcast>(),
			ModelDb.Card<ClassicAggregate>(),
			ModelDb.Card<ClassicAllForOne>(),
			ModelDb.Card<ClassicAmplify>(),
			ModelDb.Card<ClassicAutoShields>(),
			ModelDb.Card<ClassicBallLightning>(),
			ModelDb.Card<ClassicBarrage>(),
			ModelDb.Card<ClassicBeamCell>(),
			ModelDb.Card<ClassicBiasedCognition>(),
			ModelDb.Card<ClassicBlizzard>(),
			ModelDb.Card<ClassicBootSequence>(),
			ModelDb.Card<ClassicBuffer>(),
			ModelDb.Card<ClassicCapacitor>(),
			ModelDb.Card<ClassicChaos>(),
			ModelDb.Card<ClassicChill>(),
			ModelDb.Card<ClassicColdSnap>(),
			ModelDb.Card<ClassicCompileDriver>(),
			ModelDb.Card<ClassicConserveBattery>(),
			ModelDb.Card<ClassicConsume>(),
			ModelDb.Card<ClassicCoolheaded>(),
			ModelDb.Card<ClassicCoreSurge>(),
			ModelDb.Card<ClassicCreativeAi>(),
			ModelDb.Card<ClassicDarkness>(),
			ModelDb.Card<ClassicDefragment>(),
			ModelDb.Card<ClassicDoomAndGloom>(),
			ModelDb.Card<ClassicDoubleEnergy>(),
			ModelDb.Card<ClassicEchoForm>(),
			ModelDb.Card<ClassicElectrodynamics>(),
			ModelDb.Card<ClassicEquilibrium>(),
			ModelDb.Card<ClassicFission>(),
			ModelDb.Card<ClassicForceField>(),
			ModelDb.Card<ClassicFtl>(),
			ModelDb.Card<ClassicFusion>(),
			ModelDb.Card<ClassicGash>(),
			ModelDb.Card<ClassicGeneticAlgorithm>(),
			ModelDb.Card<ClassicGlacier>(),
			ModelDb.Card<ClassicGoForTheEyes>(),
			ModelDb.Card<ClassicHeatsinks>(),
			ModelDb.Card<ClassicHelloWorld>(),
			ModelDb.Card<ClassicHologram>(),
			ModelDb.Card<ClassicHyperbeam>(),
			ModelDb.Card<ClassicLeap>(),
			ModelDb.Card<ClassicLockOn>(),
			ModelDb.Card<ClassicLoop>(),
			ModelDb.Card<ClassicMachineLearning>(),
			ModelDb.Card<ClassicMelter>(),
			ModelDb.Card<ClassicMeteorStrike>(),
			ModelDb.Card<ClassicMultiCast>(),
			ModelDb.Card<ClassicRainbow>(),
			ModelDb.Card<ClassicReboot>(),
			ModelDb.Card<ClassicRebound>(),
			ModelDb.Card<ClassicRecursion>(),
			ModelDb.Card<ClassicRecycle>(),
			ModelDb.Card<ClassicReinforcedBody>(),
			ModelDb.Card<ClassicReprogram>(),
			ModelDb.Card<ClassicRipAndTear>(),
			ModelDb.Card<ClassicScrape>(),
			ModelDb.Card<ClassicSeek>(),
			ModelDb.Card<ClassicSelfRepair>(),
			ModelDb.Card<ClassicSkim>(),
			ModelDb.Card<ClassicStack>(),
			ModelDb.Card<ClassicStaticDischarge>(),
			ModelDb.Card<ClassicSteamBarrier>(),
			ModelDb.Card<ClassicSteamPower>(),
			ModelDb.Card<ClassicStorm>(),
			ModelDb.Card<ClassicStreamline>(),
			ModelDb.Card<ClassicSunder>(),
			ModelDb.Card<ClassicSweepingBeam>(),
			ModelDb.Card<ClassicTempest>(),
			ModelDb.Card<ClassicThunderStrike>(),
			ModelDb.Card<ClassicTurbo>(),
			ModelDb.Card<ClassicWhiteNoise>()
		};
	}
}
