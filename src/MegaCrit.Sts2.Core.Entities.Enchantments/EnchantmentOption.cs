using MegaCrit.Sts2.Core.Models;

namespace MegaCrit.Sts2.Core.Entities.Enchantments;

/// <summary>
/// Utility object representing a random enchantment that may be applied to a card.
/// </summary>
public struct EnchantmentOption(EnchantmentModel enchantment, int minAmount, int maxAmount)
{
	public readonly EnchantmentModel enchantment = enchantment;

	public readonly int minAmount = minAmount;

	public readonly int maxAmount = maxAmount;
}
