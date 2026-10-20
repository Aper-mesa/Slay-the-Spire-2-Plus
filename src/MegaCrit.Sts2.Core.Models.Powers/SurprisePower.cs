using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Encounters;
using MegaCrit.Sts2.Core.Models.Monsters;

namespace MegaCrit.Sts2.Core.Models.Powers;

public sealed class SurprisePower : PowerModel
{
	public override PowerType Type => PowerType.Buff;

	public override PowerStackType StackType => PowerStackType.Single;

	public override async Task AfterDeath(PlayerChoiceContext choiceContext, Creature target, bool wasRemovalPrevented, float deathAnimLength)
	{
		if (wasRemovalPrevented || Owner != target)
		{
			return;
		}
		Creature fatGremlin = CombatState.CreateCreature(ModelDb.Monster<FatGremlin>().ToMutable(), CombatSide.Enemy, "fat");
		int totalStolen = 0;
		foreach (ThieveryPower powerInstance in Owner.GetPowerInstances<ThieveryPower>())
		{
			int intValue = powerInstance.DynamicVars.Gold.IntValue;
			totalStolen += intValue;
			HeistPower heistPower = (HeistPower)ModelDb.Power<HeistPower>().ToMutable();
			heistPower.Target = powerInstance.Target;
			await PowerCmd.Apply(choiceContext, heistPower, fatGremlin, intValue, Owner, null);
		}
		await CreatureCmd.Add<SneakyGremlin>(CombatState, "sneaky");
		await CreatureCmd.Add(fatGremlin);
		if (totalStolen > 0 && CombatState.Encounter is GremlinMercNormal gremlinMercNormal)
		{
			gremlinMercNormal.MarkGoldStolen();
		}
	}

	public override bool ShouldStopCombatFromEnding()
	{
		return true;
	}
}
