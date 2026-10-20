using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace MegaCrit.Sts2.Core.Models.Powers;

public sealed class TrashToTreasurePower : PowerModel
{
	public override PowerType Type => PowerType.Buff;

	public override PowerStackType StackType => PowerStackType.Counter;

	public override async Task AfterCardGeneratedForCombat(CardModel card, Player? creator)
	{
		if (card.Type == CardType.Status && creator != null && creator.Creature == Owner)
		{
			Flash();
			for (int i = 0; i < Amount; i++)
			{
				OrbModel orb = OrbModel.GetRandomOrb(Owner.Player.RunState.Rng.CombatOrbGeneration).ToMutable();
				await OrbCmd.Channel(new ThrowingPlayerChoiceContext(), orb, Owner.Player);
			}
		}
	}
}
