using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.ValueProps;

namespace MegaCrit.Sts2.Core.Models.Events;

public sealed class BrainLeech : EventModel
{
	private const string _ripHpLossKey = "RipHpLoss";

	private const string _rewardCountKey = "RewardCount";

	private const string _cardChoiceCountKey = "CardChoiceCount";

	private const string _fromCardChoiceCountKey = "FromCardChoiceCount";

	protected override IEnumerable<DynamicVar> CanonicalVars => new _003C_003Ez__ReadOnlyArray<DynamicVar>(new DynamicVar[4]
	{
		new DamageVar("RipHpLoss", 5m, ValueProp.Unblockable | ValueProp.Unpowered),
		new IntVar("RewardCount", 1m),
		new IntVar("CardChoiceCount", 1m),
		new IntVar("FromCardChoiceCount", 5m)
	});

	public override bool IsAllowed(IRunState runState)
	{
		return runState.CurrentActIndex < 2;
	}

	protected override IReadOnlyList<EventOption> GenerateInitialOptions()
	{
		return new _003C_003Ez__ReadOnlyArray<EventOption>(new EventOption[2]
		{
			new EventOption(this, ShareKnowledge, "BRAIN_LEECH.pages.INITIAL.options.SHARE_KNOWLEDGE"),
			new EventOption(this, Rip, "BRAIN_LEECH.pages.INITIAL.options.RIP").ThatDoesDamage(DynamicVars["RipHpLoss"].BaseValue)
		});
	}

	private async Task Rip()
	{
		await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), Owner.Creature, (DamageVar)DynamicVars["RipHpLoss"], null, null, null);
		for (int i = 0; i < DynamicVars["RewardCount"].IntValue; i++)
		{
			CardCreationOptions options = CardCreationOptions.ForNonCombatWithDefaultOdds(new _003C_003Ez__ReadOnlySingleElementList<CardPoolModel>(ModelDb.CardPool<ColorlessCardPool>())).WithFlags(CardCreationFlags.NoRarityModification | CardCreationFlags.NoCardPoolModifications);
			CardReward item = new CardReward(options, 3, Owner);
			await RewardsCmd.OfferCustom(Owner, new List<Reward>(1) { item });
		}
		SetEventFinished(L10NLookup("BRAIN_LEECH.pages.RIP.description"));
	}

	private async Task ShareKnowledge()
	{
		Player owner = Owner;
		List<CardCreationResult> cards = CardFactory.CreateForReward(owner, DynamicVars["FromCardChoiceCount"].IntValue, CardCreationOptions.ForNonCombatWithDefaultOdds(new _003C_003Ez__ReadOnlySingleElementList<CardPoolModel>(owner.Character.CardPool))).ToList();
		CardSelectorPrefs prefs = new CardSelectorPrefs(L10NLookup("BRAIN_LEECH.pages.SHARE_KNOWLEDGE.selectionScreenPrompt"), 1)
		{
			Cancelable = false
		};
		await SelectCardsToAddToDeckFromGrid(cards, prefs);
		SetEventFinished(L10NLookup("BRAIN_LEECH.pages.SHARE_KNOWLEDGE.description"));
	}
}
