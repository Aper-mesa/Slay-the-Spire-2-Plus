using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;

namespace MegaCrit.Sts2.Core.Models.Powers;

public sealed class EnnuiPower : PowerModel
{
	public override PowerType Type => PowerType.Buff;

	public override PowerStackType StackType => PowerStackType.Counter;

	protected override IEnumerable<DynamicVar> CanonicalVars => new _003C_003Ez__ReadOnlySingleElementList<DynamicVar>(new CardsVar(0));

	public override async Task BeforeHandDraw(Player player, PlayerChoiceContext choiceContext, ICombatState combatState)
	{
		if (player == Owner.Player)
		{
			Flash();
			List<CardModel> list = new List<CardModel>();
			for (int i = 0; (decimal)i < DynamicVars.Cards.BaseValue; i++)
			{
				list.Add(CombatState.CreateCard<Sadness>(Owner.Player));
			}
			await CardPileCmd.AddGeneratedCardsToCombat(list, PileType.Hand, player);
		}
	}

	public override decimal ModifyHandDraw(Player player, decimal count)
	{
		if (player != Owner.Player)
		{
			return count;
		}
		return count + (decimal)Amount;
	}

	public void IncrementSadnessAmount()
	{
		AssertMutable();
		DynamicVars.Cards.BaseValue++;
	}
}
