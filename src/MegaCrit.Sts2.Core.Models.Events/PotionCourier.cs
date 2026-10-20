using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.PotionPools;
using MegaCrit.Sts2.Core.Models.Potions;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Runs;

namespace MegaCrit.Sts2.Core.Models.Events;

public sealed class PotionCourier : EventModel
{
	private const string _foulPotionsKey = "FoulPotions";

	protected override IEnumerable<DynamicVar> CanonicalVars => new _003C_003Ez__ReadOnlySingleElementList<DynamicVar>(new DynamicVar("FoulPotions", 3m));

	protected override IReadOnlyList<EventOption> GenerateInitialOptions()
	{
		return new _003C_003Ez__ReadOnlyArray<EventOption>(new EventOption[2]
		{
			new EventOption(this, GrabPotions, "POTION_COURIER.pages.INITIAL.options.GRAB_POTIONS", HoverTipFactory.FromPotion<FoulPotion>()),
			new EventOption(this, Ransack, "POTION_COURIER.pages.INITIAL.options.RANSACK")
		});
	}

	public override bool IsAllowed(IRunState runState)
	{
		return runState.CurrentActIndex > 0;
	}

	private async Task GrabPotions()
	{
		List<Reward> list = new List<Reward>();
		for (int i = 0; i < DynamicVars["FoulPotions"].IntValue; i++)
		{
			list.Add(new PotionReward(ModelDb.Potion<FoulPotion>().ToMutable(), Owner));
		}
		await RewardsCmd.OfferCustom(Owner, list);
		SetEventFinished(L10NLookup("POTION_COURIER.pages.GRAB_POTIONS.description"));
	}

	private async Task Ransack()
	{
		IEnumerable<PotionModel> items = from p in Owner.Character.PotionPool.GetUnlockedPotions(Owner.UnlockState).Concat(ModelDb.PotionPool<SharedPotionPool>().GetUnlockedPotions(Owner.UnlockState))
			where p.Rarity == PotionRarity.Uncommon
			select p;
		PotionModel potionModel = Owner.PlayerRng.Rewards.NextItem(items);
		if (potionModel != null)
		{
			await RewardsCmd.OfferCustom(Owner, new List<Reward>(1)
			{
				new PotionReward(potionModel.ToMutable(), Owner)
			});
		}
		SetEventFinished(L10NLookup("POTION_COURIER.pages.RANSACK.description"));
	}
}
