using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace MegaCrit.Sts2.Core.Models.Powers;

public sealed class StampedePower : PowerModel
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
			List<CardModel> items = hand.Cards.Where((CardModel c) => c.Type == CardType.Attack && !c.Keywords.Contains(CardKeyword.Unplayable)).ToList();
			CardModel cardModel = Owner.Player.RunState.Rng.Shuffle.NextItem(items);
			if (cardModel != null)
			{
				await CardCmd.AutoPlay(choiceContext, cardModel, null);
			}
		}
	}
}
