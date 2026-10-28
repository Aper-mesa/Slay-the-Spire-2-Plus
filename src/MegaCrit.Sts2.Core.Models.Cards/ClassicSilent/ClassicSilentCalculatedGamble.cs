using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace MegaCrit.Sts2.Core.Models.Cards;

public sealed class ClassicSilentCalculatedGamble : ClassicSilentCardModel
{
	public override IEnumerable<CardKeyword> CanonicalKeywords => new _003C_003Ez__ReadOnlySingleElementList<CardKeyword>(CardKeyword.Exhaust);

	public ClassicSilentCalculatedGamble()
		: base(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
	{
	}

	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		IEnumerable<CardModel> cards = PileType.Hand.GetPile(Owner).Cards;
		int cardsToDraw = cards.Count();
		await CardCmd.DiscardAndDraw(choiceContext, cards, cardsToDraw);
	}

	protected override void OnUpgrade()
	{
		RemoveKeyword(CardKeyword.Exhaust);
	}
}
