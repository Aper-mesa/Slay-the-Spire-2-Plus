using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;

namespace MegaCrit.Sts2.Core.Models.Relics;

public sealed class Thumbscrew : RelicModel
{
	public override RelicRarity Rarity => RelicRarity.Event;

	public override int ModifyAttackHitCount(AttackCommand attack, int hitCount)
	{
		if (hitCount <= 1)
		{
			return hitCount;
		}
		if (!(attack.ModelSource is CardModel { Type: CardType.Attack } cardModel))
		{
			return hitCount;
		}
		if (cardModel.Owner != Owner)
		{
			return hitCount;
		}
		return hitCount + 1;
	}
}
