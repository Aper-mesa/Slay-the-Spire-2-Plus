using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace MegaCrit.Sts2.Core.Models.Enchantments;

public sealed class SlumberingEssence : EnchantmentModel
{
	public override Task BeforeFlush(PlayerChoiceContext choiceContext, Player player)
	{
		if (player != Card.Owner)
		{
			return Task.CompletedTask;
		}
		CardPile? pile = Card.Pile;
		if (pile == null || pile.Type != PileType.Hand)
		{
			return Task.CompletedTask;
		}
		Card.EnergyCost.AddUntilPlayed(-1);
		return Task.CompletedTask;
	}
}
