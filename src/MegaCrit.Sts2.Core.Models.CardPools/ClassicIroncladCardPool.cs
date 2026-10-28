using System.Collections.Generic;
using System.Linq;
using Godot;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Timeline.Epochs;
using MegaCrit.Sts2.Core.Unlocks;

namespace MegaCrit.Sts2.Core.Models.CardPools;

public sealed class ClassicIroncladCardPool : CardPoolModel
{
	public override string Title => "ironclad";

	public override string EnergyColorName => "ironclad";

	public override string CardFrameMaterialPath => "card_frame_red";

	public override Color DeckEntryCardColor => new Color("D62000");

	public override Color EnergyOutlineColor => new Color("802020");

	public override bool IsColorless => false;

	protected override CardModel[] GenerateAllCards() =>
	[
		ModelDb.Card<ClassicIroncladAnger>(),
		ModelDb.Card<ClassicIroncladArmaments>(),
		ModelDb.Card<ClassicIroncladBarricade>(),
		ModelDb.Card<ClassicIroncladBash>(),
		ModelDb.Card<ClassicIroncladBattleTrance>(),
		ModelDb.Card<ClassicIroncladBerserk>(),
		ModelDb.Card<ClassicIroncladBloodForBlood>(),
		ModelDb.Card<ClassicIroncladBloodletting>(),
		ModelDb.Card<ClassicIroncladBludgeon>(),
		ModelDb.Card<ClassicIroncladBodySlam>(),
		ModelDb.Card<ClassicIroncladBrutality>(),
		ModelDb.Card<ClassicIroncladBurningPact>(),
		ModelDb.Card<ClassicIroncladCarnage>(),
		ModelDb.Card<ClassicIroncladClash>(),
		ModelDb.Card<ClassicIroncladCleave>(),
		ModelDb.Card<ClassicIroncladClothesline>(),
		ModelDb.Card<ClassicIroncladCombust>(),
		ModelDb.Card<ClassicIroncladCorruption>(),
		ModelDb.Card<ClassicIroncladDarkEmbrace>(),
		ModelDb.Card<ClassicIroncladDefendIronclad>(),
		ModelDb.Card<ClassicIroncladDemonForm>(),
		ModelDb.Card<ClassicIroncladDisarm>(),
		ModelDb.Card<ClassicIroncladDoubleTap>(),
		ModelDb.Card<ClassicIroncladDropkick>(),
		ModelDb.Card<ClassicIroncladDualWield>(),
		ModelDb.Card<ClassicIroncladEntrench>(),
		ModelDb.Card<ClassicIroncladEvolve>(),
		ModelDb.Card<ClassicIroncladExhume>(),
		ModelDb.Card<ClassicIroncladFeed>(),
		ModelDb.Card<ClassicIroncladFeelNoPain>(),
		ModelDb.Card<ClassicIroncladFiendFire>(),
		ModelDb.Card<ClassicIroncladFireBreathing>(),
		ModelDb.Card<ClassicIroncladFlameBarrier>(),
		ModelDb.Card<ClassicIroncladFlex>(),
		ModelDb.Card<ClassicIroncladGhostlyArmor>(),
		ModelDb.Card<ClassicIroncladHavoc>(),
		ModelDb.Card<ClassicIroncladHeadbutt>(),
		ModelDb.Card<ClassicIroncladHeavyBlade>(),
		ModelDb.Card<ClassicIroncladHemokinesis>(),
		ModelDb.Card<ClassicIroncladImmolate>(),
		ModelDb.Card<ClassicIroncladImpervious>(),
		ModelDb.Card<ClassicIroncladInfernalBlade>(),
		ModelDb.Card<ClassicIroncladInflame>(),
		ModelDb.Card<ClassicIroncladIntimidate>(),
		ModelDb.Card<ClassicIroncladIronWave>(),
		ModelDb.Card<ClassicIroncladJuggernaut>(),
		ModelDb.Card<ClassicIroncladLimitBreak>(),
		ModelDb.Card<ClassicIroncladMetallicize>(),
		ModelDb.Card<ClassicIroncladOffering>(),
		ModelDb.Card<ClassicIroncladPerfectedStrike>(),
		ModelDb.Card<ClassicIroncladPommelStrike>(),
		ModelDb.Card<ClassicIroncladPowerThrough>(),
		ModelDb.Card<ClassicIroncladPummel>(),
		ModelDb.Card<ClassicIroncladRage>(),
		ModelDb.Card<ClassicIroncladRampage>(),
		ModelDb.Card<ClassicIroncladReaper>(),
		ModelDb.Card<ClassicIroncladRecklessCharge>(),
		ModelDb.Card<ClassicIroncladRupture>(),
		ModelDb.Card<ClassicIroncladSearingBlow>(),
		ModelDb.Card<ClassicIroncladSecondWind>(),
		ModelDb.Card<ClassicIroncladSeeingRed>(),
		ModelDb.Card<ClassicIroncladSentinel>(),
		ModelDb.Card<ClassicIroncladSeverSoul>(),
		ModelDb.Card<ClassicIroncladShockwave>(),
		ModelDb.Card<ClassicIroncladShrugItOff>(),
		ModelDb.Card<ClassicIroncladSpotWeakness>(),
		ModelDb.Card<ClassicIroncladStrikeIronclad>(),
		ModelDb.Card<ClassicIroncladSwordBoomerang>(),
		ModelDb.Card<ClassicIroncladThunderclap>(),
		ModelDb.Card<ClassicIroncladTrueGrit>(),
		ModelDb.Card<ClassicIroncladTwinStrike>(),
		ModelDb.Card<ClassicIroncladUppercut>(),
		ModelDb.Card<ClassicIroncladWarcry>(),
		ModelDb.Card<ClassicIroncladWhirlwind>(),
		ModelDb.Card<ClassicIroncladWildStrike>(),
	];
}
