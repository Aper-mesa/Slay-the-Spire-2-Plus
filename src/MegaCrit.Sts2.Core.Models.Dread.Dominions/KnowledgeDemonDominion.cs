using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Monsters;

namespace MegaCrit.Sts2.Core.Models.Dread.Dominions;

public class KnowledgeDemonDominion : DominionModel
{
	public override IEnumerable<IHoverTip> ExtraHoverTips
	{
		get
		{
			List<IHoverTip> list = new List<IHoverTip>();
			list.AddRange(HoverTipFactory.FromCardWithCardHoverTips<Sloth>());
			list.AddRange(HoverTipFactory.FromCardWithCardHoverTips<Amnesia>());
			return new _003C_003Ez__ReadOnlyList<IHoverTip>(list);
		}
	}

	public override async Task BeforeHandDraw(Player player, PlayerChoiceContext choiceContext, ICombatState combatState)
	{
		if (player.PlayerCombatState.TurnNumber <= 1 && !player.Creature.IsDead)
		{
			List<CardModel> cards = new List<CardModel>(2)
			{
				combatState.CreateCard(ModelDb.Card<Sloth>(), player),
				combatState.CreateCard(ModelDb.Card<Amnesia>(), player)
			};
			CardModel cardModel = await CardSelectCmd.FromChooseACardScreen(choiceContext, cards, player);
			if (cardModel != null)
			{
				await ((KnowledgeDemon.IChoosable)cardModel).OnChosen();
			}
		}
	}
}
