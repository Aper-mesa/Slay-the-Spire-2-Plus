using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace MegaCrit.Sts2.Core.Models.Cards;

public sealed class TheDevil : CardModel
{
	public override IEnumerable<CardKeyword> CanonicalKeywords => new _003C_003Ez__ReadOnlySingleElementList<CardKeyword>(CardKeyword.Exhaust);

	public TheDevil()
		: base(0, CardType.Skill, CardRarity.Token, TargetType.Self)
	{
	}

	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		List<CardModel> cards = PileType.Hand.GetPile(Owner).Cards.ToList();
		foreach (CardModel item in cards)
		{
			await CardCmd.Exhaust(choiceContext, item);
		}
		await CardPileCmd.AutoPlayFromDrawPile(choiceContext, Owner, cards.Count, CardPilePosition.Top, forceExhaust: false, (CardModel c) =>
		{
			if (IsUpgraded)
			{
				CardCmd.Upgrade(c);
			}
		});
	}

	protected override void OnUpgrade()
	{
	}
}
