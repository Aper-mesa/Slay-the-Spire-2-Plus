using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.HoverTips;

namespace MegaCrit.Sts2.Core.Models.Powers;

public sealed class CorruptionPower : PowerModel
{
	public override PowerType Type => PowerType.Buff;

	public override PowerStackType StackType => PowerStackType.Single;

	protected override IEnumerable<IHoverTip> ExtraHoverTips => new _003C_003Ez__ReadOnlySingleElementList<IHoverTip>(HoverTipFactory.FromKeyword(CardKeyword.Exhaust));

	public override bool TryModifyEnergyCostInCombatLate(CardModel card, decimal originalCost, out decimal modifiedCost)
	{
		if (card.Owner.Creature != Owner || card.Type != CardType.Skill)
		{
			modifiedCost = originalCost;
			return false;
		}
		modifiedCost = 0m;
		return true;
	}

	public override CardLocation ModifyCardPlayResultLocation(CardModel card, bool isAutoPlay, ResourceInfo resources, CardLocation location)
	{
		if (card.Owner.Creature != Owner)
		{
			return location;
		}
		if (card.Type != CardType.Skill)
		{
			return location;
		}
		location.pileType = PileType.Exhaust;
		return location;
	}

	public override Task AfterModifyingCardPlayResultLocation(CardModel card, CardLocation cardLocation)
	{
		Flash();
		return Task.CompletedTask;
	}
}
