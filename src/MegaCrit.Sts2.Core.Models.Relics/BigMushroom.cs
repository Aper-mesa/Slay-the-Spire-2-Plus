using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Rooms;

namespace MegaCrit.Sts2.Core.Models.Relics;

public sealed class BigMushroom : RelicModel
{
	public override RelicRarity Rarity => RelicRarity.Event;

	public override bool HasUponPickupEffect => true;

	protected override IEnumerable<DynamicVar> CanonicalVars => new _003C_003Ez__ReadOnlyArray<DynamicVar>(new DynamicVar[2]
	{
		new MaxHpVar(20m),
		new CardsVar(2)
	});

	public override async Task AfterObtained()
	{
		await CreatureCmd.GainMaxHp(Owner.Creature, DynamicVars.MaxHp.BaseValue);
		Grow();
	}

	public override Task AfterRoomEntered(AbstractRoom _)
	{
		Grow();
		return Task.CompletedTask;
	}

	public override decimal ModifyHandDraw(Player player, decimal cardsToDraw)
	{
		if (player != Owner)
		{
			return cardsToDraw;
		}
		if (Owner.PlayerCombatState.TurnNumber != 1)
		{
			return cardsToDraw;
		}
		return cardsToDraw - (decimal)DynamicVars.Cards.IntValue;
	}

	private void Grow()
	{
		NCombatRoom.Instance?.GetCreatureNode(Owner.Creature)?.ScaleTo(1.5f, 0.0);
	}
}
