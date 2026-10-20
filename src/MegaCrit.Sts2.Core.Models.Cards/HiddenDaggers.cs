using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace MegaCrit.Sts2.Core.Models.Cards;

public sealed class HiddenDaggers : CardModel
{
	private const string _shivKey = "Shivs";

	protected override IEnumerable<DynamicVar> CanonicalVars => new _003C_003Ez__ReadOnlyArray<DynamicVar>(new DynamicVar[2]
	{
		new CardsVar(2),
		new DynamicVar("Shivs", 2m)
	});

	protected override IEnumerable<IHoverTip> ExtraHoverTips => new _003C_003Ez__ReadOnlySingleElementList<IHoverTip>(HoverTipFactory.FromCard<Shiv>(IsUpgraded));

	public HiddenDaggers()
		: base(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
	{
	}

	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		await CardCmd.Discard(choiceContext, await CardSelectCmd.FromHandForDiscard(prefs: new CardSelectorPrefs(CardSelectorPrefs.DiscardSelectionPrompt, DynamicVars.Cards.IntValue), context: choiceContext, player: Owner, filter: null, source: this));
		IEnumerable<CardModel> enumerable = await Shiv.CreateInHand(Owner, DynamicVars["Shivs"].IntValue, CombatState);
		if (!IsUpgraded)
		{
			return;
		}
		foreach (CardModel item in enumerable)
		{
			CardCmd.Upgrade(item);
		}
	}
}
