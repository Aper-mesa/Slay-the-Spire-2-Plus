using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace MegaCrit.Sts2.Core.Models.Relics;

public sealed class FakeBloodVial : RelicModel
{
	public override RelicRarity Rarity => RelicRarity.Event;

	public override int MerchantCost => 50;

	protected override IEnumerable<DynamicVar> CanonicalVars => new _003C_003Ez__ReadOnlySingleElementList<DynamicVar>(new HealVar(1m));

	public override async Task AfterPlayerTurnStartLate(PlayerChoiceContext choiceContext, Player player)
	{
		if (player == Owner && Owner.PlayerCombatState.TurnNumber <= 1)
		{
			await CreatureCmd.Heal(Owner.Creature, DynamicVars.Heal.IntValue);
		}
	}
}
