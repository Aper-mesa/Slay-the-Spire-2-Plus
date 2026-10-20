using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Gold;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.ValueProps;

namespace MegaCrit.Sts2.Core.Models.Events;

public sealed class WhisperingHollow : EventModel
{
	private const int _baseGold = 35;

	private const int _goldVariance = 9;

	protected override IEnumerable<DynamicVar> CanonicalVars => new _003C_003Ez__ReadOnlyArray<DynamicVar>(new DynamicVar[2]
	{
		new GoldVar(35),
		new HpLossVar(9m)
	});

	public override bool IsAllowed(IRunState runState)
	{
		return runState.Players.All((Player p) => p.Gold >= 44);
	}

	public override void CalculateVars()
	{
		DynamicVars.Gold.BaseValue += (decimal)Rng.NextInt(-9, 10);
	}

	protected override IReadOnlyList<EventOption> GenerateInitialOptions()
	{
		return new _003C_003Ez__ReadOnlyArray<EventOption>(new EventOption[2]
		{
			new EventOption(this, Gold, "WHISPERING_HOLLOW.pages.INITIAL.options.GOLD"),
			new EventOption(this, Hug, "WHISPERING_HOLLOW.pages.INITIAL.options.HUG", HoverTipFactory.Static(StaticHoverTip.Transform)).ThatDoesDamage(DynamicVars.HpLoss.IntValue)
		});
	}

	private async Task Gold()
	{
		await PlayerCmd.LoseGold(DynamicVars.Gold.IntValue, Owner, GoldLossType.Spent);
		await RewardsCmd.OfferCustom(Owner, new List<Reward>(2)
		{
			new PotionReward(Owner),
			new PotionReward(Owner)
		});
		SetEventFinished(L10NLookup("WHISPERING_HOLLOW.pages.GOLD.description"));
	}

	private async Task Hug()
	{
		List<CardModel> list = (await CardSelectCmd.FromDeckForTransformation(prefs: new CardSelectorPrefs(CardSelectorPrefs.TransformSelectionPrompt, 1), player: Owner)).ToList();
		foreach (CardModel item in list)
		{
			await CardCmd.TransformToRandom(item, Rng, CardPreviewStyle.EventLayout);
		}
		await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), Owner.Creature, DynamicVars.HpLoss.BaseValue, ValueProp.Unblockable | ValueProp.Unpowered, null, null, null);
		SetEventFinished(L10NLookup("WHISPERING_HOLLOW.pages.HUG.description"));
	}
}
