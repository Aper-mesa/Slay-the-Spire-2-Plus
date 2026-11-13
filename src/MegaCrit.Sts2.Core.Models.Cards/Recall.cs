using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Powers;

namespace MegaCrit.Sts2.Core.Models.Cards;

public sealed class Recall : CardModel
{
	protected override bool ShouldGlowGoldInternal => CombatState?.HittableEnemies.Any((Creature c) => ArrowCmd.CreatureIsImpaledByPlayersArrow(c, Owner)) ?? false;

	protected override IEnumerable<IHoverTip> ExtraHoverTips => new _003C_003Ez__ReadOnlySingleElementList<IHoverTip>(HoverTipFactory.FromPower<ImpaledPower>());

	public Recall()
		: base(1, CardType.Skill, CardRarity.Common, TargetType.AnyEnemy)
	{
	}

	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
		if (ArrowCmd.CreatureIsImpaledByPlayersArrow(cardPlay.Target, Owner))
		{
			while (ArrowCmd.CreatureIsImpaledByPlayersArrow(cardPlay.Target, Owner))
			{
				await ArrowCmd.DislodgeArrow(cardPlay.Target, cardPlay.Player, choiceContext, this);
			}
			CardModel cardModel = (await CardSelectCmd.FromCombatPile(prefs: new CardSelectorPrefs(SelectionScreenPrompt, 1), context: choiceContext, pile: PileType.Discard.GetPile(Owner), player: Owner)).FirstOrDefault();
			if (cardModel != null)
			{
				await CardPileCmd.Add(cardModel, PileType.Draw, CardPilePosition.Top);
			}
		}
	}

	protected override void OnUpgrade()
	{
		EnergyCost.UpgradeBy(-1);
	}
}
