using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace MegaCrit.Sts2.Core.Models.Cards;

public sealed class Enlightenment : CardModel
{
	public Enlightenment()
		: base(0, CardType.Skill, CardRarity.Event, TargetType.Self)
	{
	}

	protected override Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		foreach (CardModel card in PileType.Hand.GetPile(Owner).Cards)
		{
			if (IsUpgraded)
			{
				card.EnergyCost.SetThisCombat(1, reduceOnly: true);
			}
			else
			{
				card.EnergyCost.SetThisTurnOrUntilPlayed(1, reduceOnly: true);
			}
		}
		return Task.CompletedTask;
	}
}
