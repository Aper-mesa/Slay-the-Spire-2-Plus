using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Nodes.CommonUi;

namespace MegaCrit.Sts2.Core.Models.Relics;

public sealed class Bellows : RelicModel
{
	public override RelicRarity Rarity => RelicRarity.Rare;

	public override Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
	{
		if (player != Owner)
		{
			return Task.CompletedTask;
		}
		if (Owner.PlayerCombatState.TurnNumber > 1)
		{
			return Task.CompletedTask;
		}
		Flash();
		CardCmd.Upgrade(PileType.Hand.GetPile(Owner).Cards, CardPreviewStyle.HorizontalLayout);
		return Task.CompletedTask;
	}
}
