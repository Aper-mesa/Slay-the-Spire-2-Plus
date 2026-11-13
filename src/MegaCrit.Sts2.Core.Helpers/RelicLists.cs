using System.Collections.Generic;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;

namespace MegaCrit.Sts2.Core.Helpers;

/// <summary>
/// Relics that a game mode or a run stage excludes. Keyed by <see cref="T:MegaCrit.Sts2.Core.Models.ModelId" /> rather than by model
/// instance, so the sets are static data with no initialization step to sequence after <c>ModelDb.Init</c>.
/// Instance keys also go stale without failing loudly when <c>ModelDb.ResetForTest</c> rebuilds the
/// canonical models.
/// </summary>
public static class RelicLists
{
	public static readonly HashSet<ModelId> notAllowedInBlitz = new HashSet<ModelId>
	{
		ModelDb.GetId<AmethystAubergine>(),
		ModelDb.GetId<BlackStar>(),
		ModelDb.GetId<BookOfFiveRings>(),
		ModelDb.GetId<BowlerHat>(),
		ModelDb.GetId<CursedPearl>(),
		ModelDb.GetId<DowsingRod>(),
		ModelDb.GetId<DragonFruit>(),
		ModelDb.GetId<DreamCatcher>(),
		ModelDb.GetId<Driftwood>(),
		ModelDb.GetId<Ectoplasm>(),
		ModelDb.GetId<EternalFeather>(),
		ModelDb.GetId<FishingRod>(),
		ModelDb.GetId<FurCoat>(),
		ModelDb.GetId<Girya>(),
		ModelDb.GetId<GoldenCompass>(),
		ModelDb.GetId<GoldenPearl>(),
		ModelDb.GetId<JuzuBracelet>(),
		ModelDb.GetId<LastingCandy>(),
		ModelDb.GetId<LordsParasol>(),
		ModelDb.GetId<LuckyFysh>(),
		ModelDb.GetId<MawBank>(),
		ModelDb.GetId<MealTicket>(),
		ModelDb.GetId<MeatCleaver>(),
		ModelDb.GetId<MembershipCard>(),
		ModelDb.GetId<MiniatureTent>(),
		ModelDb.GetId<OldCoin>(),
		ModelDb.GetId<PaelsClaw>(),
		ModelDb.GetId<PaelsGrowth>(),
		ModelDb.GetId<PaelsWing>(),
		ModelDb.GetId<Planisphere>(),
		ModelDb.GetId<PrayerWheel>(),
		ModelDb.GetId<PumpkinCandle>(),
		ModelDb.GetId<RegalPillow>(),
		ModelDb.GetId<SealOfGold>(),
		ModelDb.GetId<Shovel>(),
		ModelDb.GetId<SignetRing>(),
		ModelDb.GetId<SilkenTress>(),
		ModelDb.GetId<StoneHumidifier>(),
		ModelDb.GetId<TheCourier>(),
		ModelDb.GetId<TinyMailbox>(),
		ModelDb.GetId<ToyBox>(),
		ModelDb.GetId<VenerableTeaSet>(),
		ModelDb.GetId<WhiteBeastStatue>(),
		ModelDb.GetId<WingedBoots>()
	};

	public static readonly HashSet<ModelId> notAllowedLateInAct3 = new HashSet<ModelId>
	{
		ModelDb.GetId<AmethystAubergine>(),
		ModelDb.GetId<BookOfFiveRings>(),
		ModelDb.GetId<BowlerHat>(),
		ModelDb.GetId<DragonFruit>(),
		ModelDb.GetId<FrozenEgg>(),
		ModelDb.GetId<Girya>(),
		ModelDb.GetId<JuzuBracelet>(),
		ModelDb.GetId<LastingCandy>(),
		ModelDb.GetId<LuckyFysh>(),
		ModelDb.GetId<MealTicket>(),
		ModelDb.GetId<MoltenEgg>(),
		ModelDb.GetId<OldCoin>(),
		ModelDb.GetId<Planisphere>(),
		ModelDb.GetId<Shovel>(),
		ModelDb.GetId<ToxicEgg>(),
		ModelDb.GetId<WhiteBeastStatue>(),
		ModelDb.GetId<WhiteStar>()
	};
}
