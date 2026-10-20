using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Nodes.CommonUi;

namespace MegaCrit.Sts2.Core.Models.Cards;

public sealed class Stoke : CardModel
{
	public Stoke()
		: base(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
	{
	}

	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
		List<CardModel> list = PileType.Hand.GetPile(Owner).Cards.ToList();
		int exhaustCount = list.Count;
		foreach (CardModel item in list)
		{
			await CardCmd.Exhaust(choiceContext, item);
		}
		List<CardModel> cards = CardFactory.GetForCombat(Owner, Owner.Character.CardPool.GetUnlockedCards(Owner.UnlockState, Owner.RunState.CardMultiplayerConstraint), exhaustCount, Owner.RunState.Rng.CombatCardGeneration).ToList();
		if (IsUpgraded)
		{
			CardCmd.Upgrade(cards, CardPreviewStyle.None);
		}
		await CardPileCmd.AddGeneratedCardsToCombat(cards, PileType.Hand, Owner);
	}
}
