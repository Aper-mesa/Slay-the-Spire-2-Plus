using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Debug;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Exceptions;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Logging;

namespace MegaCrit.Sts2.Core.Models.Relics;

public sealed class ChoicesParadox : RelicModel
{
	public override RelicRarity Rarity => RelicRarity.Ancient;

	protected override IEnumerable<DynamicVar> CanonicalVars => new _003C_003Ez__ReadOnlySingleElementList<DynamicVar>(new CardsVar(5));

	protected override IEnumerable<IHoverTip> ExtraHoverTips => new _003C_003Ez__ReadOnlySingleElementList<IHoverTip>(HoverTipFactory.FromKeyword(CardKeyword.Retain));

	public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
	{
		if (player != Owner || Owner.PlayerCombatState.TurnNumber != 1)
		{
			return;
		}
		Flash();
		List<CardModel> list = CardFactory.GetDistinctForCombat(Owner, Owner.Character.CardPool.GetUnlockedCards(Owner.UnlockState, Owner.RunState.CardMultiplayerConstraint), DynamicVars.Cards.IntValue, Owner.RunState.Rng.CombatCardGeneration).ToList();
		if (list.Count == 0)
		{
			string text = "ChoicesParadox generated no cards for selection. Returning early to prevent softlock.";
			Log.Error(text);
			SentryService.CaptureException(new SoftlockException(text));
			return;
		}
		foreach (CardModel item in list)
		{
			CardCmd.ApplyKeyword(item, CardKeyword.Retain);
		}
		foreach (CardModel item2 in await CardSelectCmd.FromSimpleGrid(choiceContext, list, Owner, new CardSelectorPrefs(RelicModel.L10NLookup("CHOICES_PARADOX.selectionScreenPrompt"), 1)))
		{
			await CardPileCmd.AddGeneratedCardToCombat(item2, PileType.Hand, Owner);
		}
	}
}
