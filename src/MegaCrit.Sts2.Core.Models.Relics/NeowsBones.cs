using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Events;
using MegaCrit.Sts2.Core.Rewards;

namespace MegaCrit.Sts2.Core.Models.Relics;

public sealed class NeowsBones : RelicModel
{
	private const string _relicCountKey = "Relics";

	private const string _cursesCountKey = "Curses";

	public override RelicRarity Rarity => RelicRarity.Ancient;

	protected override IEnumerable<DynamicVar> CanonicalVars => new _003C_003Ez__ReadOnlyArray<DynamicVar>(new DynamicVar[2]
	{
		new DynamicVar("Relics", 2m),
		new DynamicVar("Curses", 1m)
	});

	private static IEnumerable<RelicModel> GetValidRelics(Player owner)
	{
		return (from o in ModelDb.Event<Neow>().AllPossibleOptions
			where o.Relic != null && o.Relic.IsAllowedAtNeow(owner) && !(o.Relic is NeowsBones)
			select o.Relic).OfType<RelicModel>();
	}

	public override async Task AfterObtained()
	{
		List<RelicModel> list = GetValidRelics(Owner).ToList();
		Owner.PlayerRng.Rewards.Shuffle(list);
		List<Reward> rewards = list.Take(DynamicVars["Relics"].IntValue).Select((Func<RelicModel, Reward>)((RelicModel r) => new RelicReward(r, Owner))).ToList();
		await new RewardsSet(Owner).WithCustomRewards(rewards).WithSkippingDisallowed().Offer();
		List<CardModel> availableCurses = (from c in ModelDb.CardPool<CurseCardPool>().GetUnlockedCards(Owner.UnlockState, Owner.RunState.CardMultiplayerConstraint)
			where c.CanBeGeneratedByModifiers
			orderby c.Id
			select c).ToList();
		List<CardPileAddResult> curseResults = new List<CardPileAddResult>();
		for (int i = 0; i < DynamicVars["Curses"].IntValue; i++)
		{
			CardModel cardModel = Owner.RunState.Rng.Niche.NextItem(availableCurses);
			availableCurses.Remove(cardModel);
			CardModel card = Owner.RunState.CreateCard(cardModel, Owner);
			curseResults.Add(await CardPileCmd.Add(card, PileType.Deck));
		}
		CardCmd.PreviewCardPileAdd(curseResults, 2f);
		await Cmd.Wait(0.75f);
	}
}
