using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace MegaCrit.Sts2.Core.Models.Cards;

public sealed class TheLovers : CardModel
{
	public override IEnumerable<CardKeyword> CanonicalKeywords => new _003C_003Ez__ReadOnlySingleElementList<CardKeyword>(CardKeyword.Exhaust);

	public TheLovers()
		: base(0, CardType.Skill, CardRarity.Token, TargetType.Self)
	{
	}

	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		List<CardPoolModel> list = Owner.UnlockState.CharacterCardPools.ToList();
		if (list.Count > 1)
		{
			list.Remove(Owner.Character.CardPool);
		}
		IEnumerable<CardModel> cards = list.SelectMany((CardPoolModel c) => c.GetUnlockedCards(Owner.UnlockState, Owner.RunState.CardMultiplayerConstraint));
		int count = CardPile.MaxCardsInHand - CardPile.GetCards(Owner, PileType.Hand).Count();
		IEnumerable<CardModel> forCombat = CardFactory.GetForCombat(Owner, cards, count, Owner.RunState.Rng.CombatCardGeneration);
		if (IsUpgraded)
		{
			foreach (CardModel item in forCombat)
			{
				if (IsUpgraded)
				{
					CardCmd.Upgrade(item);
				}
			}
		}
		await CardPileCmd.AddGeneratedCardsToCombat(forCombat, PileType.Hand, Owner);
	}
}
