using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Gold;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Runs;

namespace MegaCrit.Sts2.Core.Models.Events;

public sealed class ZenWeaver : EventModel
{
	private const string _breathingTechniquesCostKey = "BreathingTechniquesCost";

	private const string _emotionalAwarenessCostKey = "EmotionalAwarenessCost";

	private const string _arachnidAcupunctureCostKey = "ArachnidAcupunctureCost";

	protected override IEnumerable<DynamicVar> CanonicalVars => new _003C_003Ez__ReadOnlyArray<DynamicVar>(new DynamicVar[3]
	{
		new DynamicVar("BreathingTechniquesCost", 50m),
		new DynamicVar("EmotionalAwarenessCost", 125m),
		new DynamicVar("ArachnidAcupunctureCost", 250m)
	});

	public override bool IsAllowed(IRunState runState)
	{
		return runState.Players.All((Player p) => (decimal)p.Gold >= DynamicVars["EmotionalAwarenessCost"].BaseValue);
	}

	protected override IReadOnlyList<EventOption> GenerateInitialOptions()
	{
		int gold = Owner.Gold;
		EventOption eventOption = new EventOption(this, BreathingTechniques, "ZEN_WEAVER.pages.INITIAL.options.BREATHING_TECHNIQUES", HoverTipFactory.FromCardWithCardHoverTips<Enlightenment>());
		EventOption eventOption2 = ((gold < DynamicVars["EmotionalAwarenessCost"].IntValue) ? CreateLockedOption() : new EventOption(this, EmotionalAwareness, "ZEN_WEAVER.pages.INITIAL.options.EMOTIONAL_AWARENESS"));
		EventOption eventOption3 = ((gold < DynamicVars["ArachnidAcupunctureCost"].IntValue) ? CreateLockedOption() : new EventOption(this, ArachnidAcupuncture, "ZEN_WEAVER.pages.INITIAL.options.ARACHNID_ACUPUNCTURE"));
		return new _003C_003Ez__ReadOnlyArray<EventOption>(new EventOption[3] { eventOption, eventOption2, eventOption3 });
	}

	private async Task BreathingTechniques()
	{
		await PlayerCmd.LoseGold(DynamicVars["BreathingTechniquesCost"].IntValue, Owner, GoldLossType.Spent);
		CardModel[] array = new CardModel[2];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = Owner.RunState.CreateCard<Enlightenment>(Owner);
		}
		CardCmd.PreviewCardPileAdd(await CardPileCmd.Add(array, PileType.Deck));
		SetEventFinished(L10NLookup("ZEN_WEAVER.pages.BREATHING_TECHNIQUES.description"));
	}

	private async Task EmotionalAwareness()
	{
		await RemoveCardsAndProceed(DynamicVars["EmotionalAwarenessCost"].IntValue, 1);
		SetEventFinished(L10NLookup("ZEN_WEAVER.pages.EMOTIONAL_AWARENESS.description"));
	}

	private async Task ArachnidAcupuncture()
	{
		await RemoveCardsAndProceed(DynamicVars["ArachnidAcupunctureCost"].IntValue, 2);
		SetEventFinished(L10NLookup("ZEN_WEAVER.pages.ARACHNID_ACUPUNCTURE.description"));
	}

	private async Task RemoveCardsAndProceed(int cost, int count)
	{
		await CardPileCmd.RemoveFromDeck((await CardSelectCmd.FromDeckForRemoval(Owner, new CardSelectorPrefs(CardSelectorPrefs.RemoveSelectionPrompt, count))).ToList());
		await PlayerCmd.LoseGold(cost, Owner, GoldLossType.Spent);
	}

	private EventOption CreateLockedOption()
	{
		return new EventOption(this, null, "ZEN_WEAVER.pages.INITIAL.options.LOCKED");
	}
}
