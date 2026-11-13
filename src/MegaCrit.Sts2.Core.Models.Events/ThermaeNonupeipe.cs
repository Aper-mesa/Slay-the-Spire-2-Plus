using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.ValueProps;

namespace MegaCrit.Sts2.Core.Models.Events;

public sealed class ThermaeNonupeipe : EventModel
{
	private const string _thermalBathsCostKey = "ThermalBathsCost";

	private const string _bathTowelDescriptionKey = "BathTowelDescription";

	private const string _bathRobesDescriptionKey = "BathRobesDescription";

	private const string _drinkHpCostKey = "DrinkHpLoss";

	private const string _drinkRelicNameKey = "DrinkRelicName";

	private const string _drinkGuiltyCountKey = "DrinkGuiltyCount";

	private RelicModel? _drinkRelic;

	protected override IEnumerable<DynamicVar> CanonicalVars => new _003C_003Ez__ReadOnlyArray<DynamicVar>(new DynamicVar[6]
	{
		new GoldVar("ThermalBathsCost", 88),
		new HpLossVar("DrinkHpLoss", 31m),
		new CardsVar("DrinkGuiltyCount", 3),
		new StringVar("BathTowelDescription"),
		new StringVar("BathRobesDescription"),
		new StringVar("DrinkRelicName")
	});

	public RelicModel DrinkRelic
	{
		get
		{
			return _drinkRelic ?? throw new InvalidOperationException();
		}
		set
		{
			AssertMutable();
			_drinkRelic = value;
		}
	}

	public override bool IsAllowed(IRunState runState)
	{
		if (runState.Players.All((Player p) => p.Gold > 175 && (decimal)p.Creature.CurrentHp <= (decimal)p.Creature.MaxHp * 0.75m))
		{
			return !(runState.Act.Ancient is Nonupeipe);
		}
		return false;
	}

	public override void CalculateVars()
	{
		Nonupeipe nonupeipe = ModelDb.AncientEvent<Nonupeipe>();
		IEnumerable<EventOption> items = nonupeipe.AllPossibleOptions.Where((EventOption o) => o.Relic != null && !(o.Relic is LoomingFruit));
		DrinkRelic = Rng.NextItem(items).Relic;
		((StringVar)DynamicVars["BathTowelDescription"]).StringValue = ModelDb.Relic<BathTowel>().DynamicEventDescription.GetFormattedText();
		((StringVar)DynamicVars["BathRobesDescription"]).StringValue = ModelDb.Relic<BathRobes>().DynamicEventDescription.GetFormattedText();
		((StringVar)DynamicVars["DrinkRelicName"]).StringValue = DrinkRelic.Title.GetFormattedText();
	}

	protected override IReadOnlyList<EventOption> GenerateInitialOptions()
	{
		EventOption[] array = new EventOption[3]
		{
			new EventOption(this, ThermalBaths, "THERMAE_NONUPEIPE.pages.INITIAL.options.THERMAL_BATHS"),
			new EventOption(this, PrivateOasis, "THERMAE_NONUPEIPE.pages.INITIAL.options.PRIVATE_OASIS"),
			null
		};
		Func<Task> onChosen = DrinkWaters;
		List<IHoverTip> list = new List<IHoverTip>();
		list.AddRange(HoverTipFactory.FromRelic(DrinkRelic));
		list.Add(HoverTipFactory.FromCard<Guilty>());
		array[2] = new EventOption(this, onChosen, "THERMAE_NONUPEIPE.pages.INITIAL.options.DRINK_WATERS", list.ToArray()).ThatDoesDamage(DynamicVars["DrinkHpLoss"].BaseValue);
		return new _003C_003Ez__ReadOnlyArray<EventOption>(array);
	}

	private async Task ThermalBaths()
	{
		await PlayerCmd.LoseGold(DynamicVars["ThermalBathsCost"].BaseValue, Owner);
		await RelicCmd.Obtain<BathTowel>(Owner);
		SetEventFinished(L10NLookup("THERMAE_NONUPEIPE.pages.THERMAL_BATHS.description"));
	}

	private async Task PrivateOasis()
	{
		await PlayerCmd.LoseGold(Owner.Gold, Owner);
		await RelicCmd.Obtain<BathRobes>(Owner);
		SetEventFinished(L10NLookup("THERMAE_NONUPEIPE.pages.PRIVATE_OASIS.description"));
	}

	private async Task DrinkWaters()
	{
		await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), Owner.Creature, DynamicVars["DrinkHpLoss"].BaseValue, ValueProp.Unblockable | ValueProp.Unpowered, null, null);
		List<CardModel> list = new List<CardModel>();
		for (int i = 0; (decimal)i < DynamicVars["DrinkGuiltyCount"].BaseValue; i++)
		{
			list.Add(ModelDb.Card<Guilty>());
		}
		await CardPileCmd.AddCursesToDeck(list, Owner);
		await RelicCmd.Obtain(DrinkRelic, Owner);
		SetEventFinished(L10NLookup("THERMAE_NONUPEIPE.pages.DRINK_WATERS.description"));
	}
}
