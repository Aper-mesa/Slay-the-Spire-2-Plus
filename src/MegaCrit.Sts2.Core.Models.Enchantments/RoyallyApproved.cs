using System.Collections.Generic;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.HoverTips;

namespace MegaCrit.Sts2.Core.Models.Enchantments;

public sealed class RoyallyApproved : EnchantmentModel
{
	protected override IEnumerable<IHoverTip> ExtraHoverTips => new _003C_003Ez__ReadOnlyArray<IHoverTip>(new IHoverTip[2]
	{
		HoverTipFactory.FromKeyword(CardKeyword.Innate),
		HoverTipFactory.FromKeyword(CardKeyword.Retain)
	});

	public override bool CanEnchantCardType(CardType cardType)
	{
		if ((uint)(cardType - 1) <= 1u)
		{
			return true;
		}
		return false;
	}

	protected override void OnEnchant()
	{
		Card.AddKeyword(CardKeyword.Innate);
		Card.AddKeyword(CardKeyword.Retain);
	}
}
