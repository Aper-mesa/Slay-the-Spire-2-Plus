using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Cards;

namespace MegaCrit.Sts2.Core.Models.Relics;

public sealed class DistinguishedCape : RelicModel
{
	private const string _cursesKey = "Curses";

	public override RelicRarity Rarity => RelicRarity.Ancient;

	public override bool HasUponPickupEffect => true;

	protected override IEnumerable<IHoverTip> ExtraHoverTips => HoverTipFactory.FromCardWithCardHoverTips<Apparition>();

	protected override IEnumerable<DynamicVar> CanonicalVars => new _003C_003Ez__ReadOnlyArray<DynamicVar>(new DynamicVar[2]
	{
		new DynamicVar("Curses", 2m),
		new CardsVar(3)
	});

	public override async Task AfterObtained()
	{
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
		List<CardPileAddResult> results = new List<CardPileAddResult>();
		for (int i = 0; i < DynamicVars.Cards.IntValue; i++)
		{
			CardModel card2 = Owner.RunState.CreateCard<Apparition>(Owner);
			List<CardPileAddResult> list = results;
			list.Add(await CardPileCmd.Add(card2, PileType.Deck));
		}
		CardCmd.PreviewCardPileAdd(results, 2f);
	}
}
