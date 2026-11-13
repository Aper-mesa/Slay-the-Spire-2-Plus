namespace MegaCrit.Sts2.Core.Models.Enchantments;

public sealed class Nimble : EnchantmentModel
{
	public override bool ShowAmount => true;

	/// <summary>
	/// The card itself has to gain Block. A card that applies a power which later grants Block, such as Child of the
	/// Stars, does not qualify and cannot take this enchantment. Closed as designed in PRG-4534.
	/// </summary>
	public override bool CanEnchant(CardModel card)
	{
		if (base.CanEnchant(card))
		{
			return card.GainsBlock;
		}
		return false;
	}

	public override decimal EnchantBlockAdditive(decimal originalBlock)
	{
		return Amount;
	}
}
