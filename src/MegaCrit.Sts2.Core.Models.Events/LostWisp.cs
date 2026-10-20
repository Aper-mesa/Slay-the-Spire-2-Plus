using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Relics;

namespace MegaCrit.Sts2.Core.Models.Events;

public sealed class LostWisp : EventModel
{
	private const string _relicKey = "Relic";

	private const string _curseKey = "Curse";

	private const int _baseGold = 60;

	private const int _goldVariance = 15;

	protected override IEnumerable<DynamicVar> CanonicalVars => new _003C_003Ez__ReadOnlyArray<DynamicVar>(new DynamicVar[3]
	{
		new GoldVar(60),
		new StringVar("Relic", ModelDb.Relic<MegaCrit.Sts2.Core.Models.Relics.LostWisp>().Title.GetFormattedText()),
		new StringVar("Curse", ModelDb.Card<Decay>().Title)
	});

	protected override IReadOnlyList<EventOption> GenerateInitialOptions()
	{
		EventOption[] array = new EventOption[2];
		Func<Task> onChosen = Claim;
		List<IHoverTip> list = new List<IHoverTip>();
		list.AddRange(HoverTipFactory.FromRelic<MegaCrit.Sts2.Core.Models.Relics.LostWisp>());
		list.AddRange(HoverTipFactory.FromCardWithCardHoverTips<Decay>());
		array[0] = new EventOption(this, onChosen, "LOST_WISP.pages.INITIAL.options.CLAIM", list.ToArray());
		array[1] = new EventOption(this, Search, "LOST_WISP.pages.INITIAL.options.SEARCH");
		return new _003C_003Ez__ReadOnlyArray<EventOption>(array);
	}

	public override void CalculateVars()
	{
		DynamicVars.Gold.BaseValue += (decimal)Rng.NextInt(-15, 16);
	}

	private async Task Claim()
	{
		await CardPileCmd.AddCursesToDeck(new _003C_003Ez__ReadOnlySingleElementList<CardModel>(ModelDb.Card<Decay>()), Owner);
		await RelicCmd.Obtain<MegaCrit.Sts2.Core.Models.Relics.LostWisp>(Owner);
		SetEventFinished(L10NLookup("LOST_WISP.pages.CLAIM.description"));
	}

	private async Task Search()
	{
		await PlayerCmd.GainGold(DynamicVars.Gold.IntValue, Owner);
		SetEventFinished(L10NLookup("LOST_WISP.pages.SEARCH.description"));
	}
}
