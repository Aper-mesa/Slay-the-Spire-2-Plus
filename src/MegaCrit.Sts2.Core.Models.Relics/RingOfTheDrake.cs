using System.Collections.Generic;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace MegaCrit.Sts2.Core.Models.Relics;

public sealed class RingOfTheDrake : RelicModel
{
	private const string _turnsKey = "Turns";

	public override RelicRarity Rarity => RelicRarity.Starter;

	protected override IEnumerable<DynamicVar> CanonicalVars => new _003C_003Ez__ReadOnlyArray<DynamicVar>(new DynamicVar[2]
	{
		new CardsVar(2),
		new DynamicVar("Turns", 3m)
	});

	public override decimal ModifyHandDraw(Player player, decimal count)
	{
		if (player != Owner)
		{
			return count;
		}
		if ((decimal)Owner.PlayerCombatState.TurnNumber > DynamicVars["Turns"].BaseValue)
		{
			return count;
		}
		return count + DynamicVars.Cards.BaseValue;
	}
}
