using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;

namespace MegaCrit.Sts2.Core.Models.Events;

public sealed class SunkenTreasury : EventModel
{
	private const string _smallChestGoldKey = "SmallChestGold";

	private const string _largeChestGoldKey = "LargeChestGold";

	protected override IEnumerable<DynamicVar> CanonicalVars => new _003C_003Ez__ReadOnlyArray<DynamicVar>(new DynamicVar[2]
	{
		new DynamicVar("SmallChestGold", 60m),
		new DynamicVar("LargeChestGold", 333m)
	});

	protected override IReadOnlyList<EventOption> GenerateInitialOptions()
	{
		return new _003C_003Ez__ReadOnlyArray<EventOption>(new EventOption[2]
		{
			new EventOption(this, FirstChest, "SUNKEN_TREASURY.pages.INITIAL.options.FIRST_CHEST"),
			new EventOption(this, SecondChest, "SUNKEN_TREASURY.pages.INITIAL.options.SECOND_CHEST", HoverTipFactory.FromCardWithCardHoverTips<Greed>())
		});
	}

	public override void CalculateVars()
	{
		DynamicVars["SmallChestGold"].BaseValue += (decimal)(Rng.NextInt(16) - 8);
		DynamicVars["LargeChestGold"].BaseValue += (decimal)(Rng.NextInt(61) - 30);
	}

	private async Task FirstChest()
	{
		await PlayerCmd.GainGold(DynamicVars["SmallChestGold"].BaseValue, Owner);
		SetEventFinished(L10NLookup("SUNKEN_TREASURY.pages.FIRST_CHEST.description"));
	}

	private async Task SecondChest()
	{
		await PlayerCmd.GainGold(DynamicVars["LargeChestGold"].BaseValue, Owner);
		await CardPileCmd.AddCurseToDeck<Greed>(Owner);
		LocString locString = L10NLookup("SUNKEN_TREASURY.pages.SECOND_CHEST.description");
		locString.Add("Monologue", new LocString("characters", Owner.Character.Id.Entry + ".goldMonologue"));
		SetEventFinished(locString);
	}
}
