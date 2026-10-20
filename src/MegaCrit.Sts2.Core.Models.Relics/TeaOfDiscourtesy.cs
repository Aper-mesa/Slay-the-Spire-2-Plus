using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace MegaCrit.Sts2.Core.Models.Relics;

public sealed class TeaOfDiscourtesy : RelicModel
{
	private const string _combatsKey = "Combats";

	private const string _dazedCountKey = "DazedCount";

	private int _combatsLeft = 1;

	public override RelicRarity Rarity => RelicRarity.Event;

	public override bool IsUsedUp => CombatsLeft <= 0;

	public override bool ShowCounter => false;

	protected override IEnumerable<DynamicVar> CanonicalVars => new _003C_003Ez__ReadOnlyArray<DynamicVar>(new DynamicVar[3]
	{
		new HealVar(1m),
		new DynamicVar("Combats", CombatsLeft),
		new DynamicVar("DazedCount", 2m)
	});

	protected override IEnumerable<IHoverTip> ExtraHoverTips => HoverTipFactory.FromCardWithCardHoverTips<Dazed>();

	[SavedProperty]
	private int CombatsLeft
	{
		get
		{
			return _combatsLeft;
		}
		set
		{
			AssertMutable();
			_combatsLeft = value;
			DynamicVars["Combats"].BaseValue = _combatsLeft;
			InvokeDisplayAmountChanged();
			if (IsUsedUp)
			{
				Status = RelicStatus.Disabled;
			}
		}
	}

	public override async Task BeforeCombatStart()
	{
		if (CombatsLeft > 0)
		{
			await CardPileCmd.AddToCombatAndPreview<Dazed>(Owner.Creature, PileType.Draw, DynamicVars["DazedCount"].IntValue, Owner, CardPilePosition.Random);
			CombatsLeft--;
			Flash();
		}
	}
}
