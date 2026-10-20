using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;

namespace MegaCrit.Sts2.Core.Models.Powers;

public sealed class InfernoPower : PowerModel
{
	private const string _selfDamageKey = "SelfDamage";

	public override PowerType Type => PowerType.Buff;

	public override PowerStackType StackType => PowerStackType.Counter;

	protected override IEnumerable<DynamicVar> CanonicalVars => new _003C_003Ez__ReadOnlySingleElementList<DynamicVar>(new DamageVar("SelfDamage", 0m, ValueProp.Unblockable | ValueProp.Unpowered));

	public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
	{
		if (player == Owner.Player)
		{
			NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(NFireSmokePuffVfx.Create(Owner));
			await Cmd.CustomScaledWait(0.2f, 0.4f);
			DamageVar damageVar = (DamageVar)DynamicVars["SelfDamage"];
			await CreatureCmd.Damage(choiceContext, Owner, damageVar.BaseValue, damageVar.Props, Owner);
		}
	}

	public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
	{
		if (target != Owner || result.UnblockedDamage <= 0 || Owner.CombatState.CurrentSide != Owner.Side)
		{
			return;
		}
		foreach (Creature hittableEnemy in CombatState.HittableEnemies)
		{
			NFireBurstVfx child = NFireBurstVfx.Create(hittableEnemy, 0.75f);
			NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(child);
		}
		await CreatureCmd.Damage(choiceContext, CombatState.HittableEnemies, Amount, ValueProp.Unpowered, Owner);
	}

	public void IncrementSelfDamage()
	{
		AssertMutable();
		DynamicVars["SelfDamage"].BaseValue++;
	}
}
