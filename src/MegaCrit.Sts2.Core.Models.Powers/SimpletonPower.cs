using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace MegaCrit.Sts2.Core.Models.Powers;

public sealed class SimpletonPower : PowerModel
{
	public override PowerType Type => PowerType.Buff;

	public override PowerStackType StackType => PowerStackType.Counter;

	public override async Task AfterAutoPostPlayPhaseEntered(PlayerChoiceContext choiceContext, Player player)
	{
		if (player != Owner.Player)
		{
			return;
		}
		CardPile hand = PileType.Hand.GetPile(Owner.Player);
		for (int i = 0; i < Amount; i++)
		{
			IGrouping<int, CardModel> grouping = (from c in hand.Cards
				where !c.Keywords.Contains(CardKeyword.Unplayable)
				group c by (!c.EnergyCost.CostsX) ? c.EnergyCost.GetResolved() : 999999999 into g
				orderby g.Key
				select g).FirstOrDefault();
			if (grouping != null)
			{
				CardModel cardModel = Owner.Player.RunState.Rng.CombatCardSelection.NextItem(grouping);
				if (cardModel != null)
				{
					await CardCmd.AutoPlay(choiceContext, cardModel, null);
				}
			}
		}
	}
}
