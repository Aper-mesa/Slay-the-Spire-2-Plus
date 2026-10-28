using System.Collections.Generic;
using MegaCrit.Sts2.Core.Models.CardPools;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace MegaCrit.Sts2.Core.Models.Cards;

public sealed class ClassicIroncladInfernalBlade : ClassicIroncladCardModel
{
	public override IEnumerable<CardKeyword> CanonicalKeywords => new _003C_003Ez__ReadOnlySingleElementList<CardKeyword>(CardKeyword.Exhaust);

	public ClassicIroncladInfernalBlade()
		: base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
	{
	}

	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		CardModel cardModel = CardFactory.GetDistinctForCombat(Owner, from c in ModelDb.CardPool<ClassicIroncladCardPool>().GetUnlockedCards(Owner.UnlockState, Owner.RunState.CardMultiplayerConstraint)
			where c.Type == CardType.Attack
			select c, 1, Owner.RunState.Rng.CombatCardGeneration).FirstOrDefault();
		if (cardModel != null)
		{
			cardModel.SetToFreeThisTurn();
			await CardPileCmd.AddGeneratedCardToCombat(cardModel, PileType.Hand, Owner);
		}
	}

	protected override void OnUpgrade()
	{
		EnergyCost.UpgradeBy(-1);
	}
}
