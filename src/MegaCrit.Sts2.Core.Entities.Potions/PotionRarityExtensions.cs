using System;
using MegaCrit.Sts2.Core.Localization;

namespace MegaCrit.Sts2.Core.Entities.Potions;

public static class PotionRarityExtensions
{
	public static LocString ToLocString(this PotionRarity potionRarity)
	{
		return potionRarity switch
		{
			PotionRarity.Common => new LocString("gameplay_ui", "POTION_RARITY.COMMON"), 
			PotionRarity.Uncommon => new LocString("gameplay_ui", "POTION_RARITY.UNCOMMON"), 
			PotionRarity.Rare => new LocString("gameplay_ui", "POTION_RARITY.RARE"), 
			PotionRarity.Event => new LocString("gameplay_ui", "POTION_RARITY.EVENT"), 
			PotionRarity.Token => new LocString("gameplay_ui", "POTION_RARITY.TOKEN"), 
			PotionRarity.None => throw new ArgumentOutOfRangeException("potionRarity", potionRarity, null), 
		};
	}
}
