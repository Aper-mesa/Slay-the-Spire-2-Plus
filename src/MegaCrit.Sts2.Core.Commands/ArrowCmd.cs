using System;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.FoolArrows;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.Nodes.Vfx.Utilities;
using MegaCrit.Sts2.Core.Random;

namespace MegaCrit.Sts2.Core.Commands;

public static class ArrowCmd
{
	/// <summary>
	/// Adds an arrow slot to the player creature.
	/// Right not it is implemented as a Power, but implementation may change as the character develops.
	/// Right now, we limit it to one arrow, but that may also change as the character develops.
	/// </summary>
	/// <param name="player">Player gaining the arrow.</param>
	public static void GainArrowSlot(Player player)
	{
		player.PlayerCombatState?.GainArrow();
	}

	private static void RetrieveArrow(ArrowSlot arrowSlot)
	{
		if (arrowSlot.Status == ArrowStatus.Destroyed)
		{
			throw new InvalidOperationException("Destroyed arrows cannot be retrieved");
		}
		if (arrowSlot.Status == ArrowStatus.Retrieved)
		{
			throw new InvalidOperationException("This arrow has already been retrieved");
		}
		arrowSlot.Status = ArrowStatus.Retrieved;
		arrowSlot.DestroyAfterImpaled = false;
	}

	/// <summary>
	/// Checks if the player creature has any Arrow.
	/// Right not it is implemented as a Power, but implementation may change as the character develops.
	/// </summary>
	/// <param name="player">Player gaining the arrow.</param>
	public static bool PlayerHasAnyArrows(Player player)
	{
		return TryGetNextArrowToShoot(player) != null;
	}

	/// <summary>
	/// Number of Arrows the player currently has available to shoot. 0 outside of combat.
	/// </summary>
	public static int GetAvailableArrowCount(Player player)
	{
		return player.PlayerCombatState?.AvailableArrowCount ?? 0;
	}

	private static ArrowSlot? TryGetNextArrowToShoot(Player player)
	{
		if (player.PlayerCombatState == null)
		{
			return null;
		}
		return player.PlayerCombatState.Arrows.LastOrDefault((ArrowSlot a) => a.IsAvailable);
	}

	public static bool CreatureIsImpaledByArrow(Creature creature)
	{
		return creature.HasPower<ImpaledPower>();
	}

	public static bool CreatureIsImpaledByPlayersArrow(Creature creature, Player player)
	{
		if (!CreatureIsImpaledByArrow(creature))
		{
			return false;
		}
		return creature.GetPowerInstances<ImpaledPower>().Any((ImpaledPower p) => p.Applier == player.Creature);
	}

	/// <summary>
	/// Shoots the Arrow of a player at a creature.
	/// If the Player doesn't have an arrow, this does nothing.
	/// The shot creature gains the power <see cref="T:MegaCrit.Sts2.Core.Models.Powers.ImpaledPower" />.
	/// TODO: Consider case in MP when multiple folks shoot arrows
	/// </summary>
	/// <param name="target">Target creature that is getting shot.</param>
	/// <param name="player">Player that is shooting the arrow.</param>
	/// <param name="context">Current player context.</param>
	/// <param name="card">Card that is shooting the arrow.</param>
	/// <param name="actionToPerform">Action to perform when the arrow is shot</param>
	public static async Task<bool> ShootArrowAtTarget(Creature target, Player player, PlayerChoiceContext context, CardModel? card, Func<Task>? actionToPerform = null)
	{
		if (target.IsDead)
		{
			return false;
		}
		ArrowSlot arrow = await TryShootArrow(player, context, card);
		if (arrow != null)
		{
			await ImpaleTarget(target, arrow, actionToPerform, context, card);
		}
		return arrow != null;
	}

	/// <summary>
	/// Shoots the Arrow of a player at a target creature.
	/// If the Player doesn't have an arrow, this does nothing.
	/// Is set up for Attack Commands so that the timing with the arrow Vfx is correct
	/// The shot creature gains the power <see cref="T:MegaCrit.Sts2.Core.Models.Powers.ImpaledPower" />.
	/// TODO: Consider case in MP when multiple folks shoot arrows
	/// </summary>
	/// <param name="target">Target creature that is getting shot.</param>
	/// <param name="player">Player that is shooting the arrow.</param>
	/// <param name="attackCommand">The attack command that is being performed along with this arrow getting shot.</param>
	/// <param name="context">Current player context.</param>
	public static async Task<bool> ShootArrowAtTarget(Creature target, Player player, AttackCommand attackCommand, PlayerChoiceContext context)
	{
		if (target.IsDead)
		{
			return false;
		}
		ArrowSlot arrow = await TryShootArrow(player, context, attackCommand.CardPlay.Card);
		if (arrow != null)
		{
			await ImpaleTarget(target, arrow, attackCommand, context);
		}
		return arrow != null;
	}

	/// <summary>
	/// Shoots the Arrow of a player.
	/// If the Player doesn't have an arrow, this does nothing.
	/// WARNING: Please make sure you know what you are doing when you are using this.
	/// In most cases, you should be using ShootArrowAtTarget.
	/// </summary>
	public static async Task<ArrowSlot?> TryShootArrow(Player player, PlayerChoiceContext context, CardModel? source)
	{
		ICombatState combatState = player.Creature.CombatState;
		if (combatState == null)
		{
			return null;
		}
		ArrowSlot arrowSlot = TryGetNextArrowToShoot(player);
		if (arrowSlot != null)
		{
			arrowSlot.Status = ArrowStatus.Away;
			CombatManager.Instance.History.ArrowShot(combatState, arrowSlot);
		}
		await Hook.AfterArrowShot(combatState, player, arrowSlot, context, source);
		return arrowSlot;
	}

	/// <summary>
	/// Impales a target creature with an Arrow from a player.
	/// WARNING: Please make sure you know what you are doing when you are using this.
	/// In most cases, you should be using ShootArrowAtTarget.
	/// </summary>
	private static async Task ImpaleTarget(Creature target, ArrowSlot arrowSlot, AttackCommand attackCommand, PlayerChoiceContext context)
	{
		if (target.IsDead)
		{
			return;
		}
		ICombatState combatState = arrowSlot.owner.Creature.CombatState;
		attackCommand.AfterAttackerAnim(() =>
		{
			NGame.Instance?.ScreenShake(ShakeStrength.Medium, ShakeDuration.Normal, 180f + Rng.Chaotic.NextFloat(-10f, 10f));
			NCreature nCreature = NCombatRoom.Instance?.GetCreatureNode(target);
			if (nCreature != null)
			{
				NImpaledArrowVfx nImpaledArrowVfx = NImpaledArrowVfx.Create(target, arrowSlot);
				if (nImpaledArrowVfx != null)
				{
					nCreature.Visuals.AddChildSafely(nImpaledArrowVfx);
				}
			}
			return Task.CompletedTask;
		});
		await attackCommand.Execute(context);
		ImpaledPower impaledInstance = target.GetPowerInstances<ImpaledPower>().FirstOrDefault((ImpaledPower power) => power.Applier == arrowSlot.owner.Creature);
		if (impaledInstance == null)
		{
			impaledInstance = (ImpaledPower)ModelDb.Power<ImpaledPower>().ToMutable();
			await PowerCmd.Apply(context, impaledInstance, target, 1m, arrowSlot.owner.Creature, attackCommand.CardPlay.Card);
		}
		impaledInstance.AddArrow(arrowSlot);
		await Hook.AfterArrowImpaled(combatState, impaledInstance, arrowSlot, context, null);
		if (arrowSlot.DestroyAfterImpaled)
		{
			await DestroyArrow(target, impaledInstance, arrowSlot);
		}
		else if (target.IsDead && impaledInstance.Arrows.Contains(arrowSlot))
		{
			await DislodgeArrow(combatState, impaledInstance, context, attackCommand.CardPlay.Card);
		}
	}

	/// <summary>
	/// Impales a target creature with an Arrow from a player.
	/// WARNING: Please make sure you know what you are doing when you are using this.
	/// In most cases, you should be using ShootArrowAtTarget.
	/// </summary>
	public static async Task ImpaleTarget(Creature target, ArrowSlot arrowSlot, Func<Task>? actionToPerform, PlayerChoiceContext context, CardModel? card)
	{
		if (target.IsDead)
		{
			return;
		}
		ICombatState combatState = arrowSlot.owner.Creature.CombatState;
		NGame.Instance?.ScreenShake(ShakeStrength.Medium, ShakeDuration.Normal, 180f + Rng.Chaotic.NextFloat(-10f, 10f));
		NCreature nCreature = NCombatRoom.Instance?.GetCreatureNode(target);
		if (nCreature != null)
		{
			NImpaledArrowVfx nImpaledArrowVfx = NImpaledArrowVfx.Create(target, arrowSlot);
			if (nImpaledArrowVfx != null)
			{
				nCreature.Visuals.AddChildSafely(nImpaledArrowVfx);
			}
		}
		if (actionToPerform != null)
		{
			await actionToPerform();
		}
		ImpaledPower impaledInstance = target.GetPowerInstances<ImpaledPower>().FirstOrDefault((ImpaledPower power) => power.Applier == arrowSlot.owner.Creature);
		if (impaledInstance == null)
		{
			impaledInstance = (ImpaledPower)ModelDb.Power<ImpaledPower>().ToMutable();
			await PowerCmd.Apply(context, impaledInstance, target, 1m, arrowSlot.owner.Creature, card);
		}
		impaledInstance.AddArrow(arrowSlot);
		await Hook.AfterArrowImpaled(combatState, impaledInstance, arrowSlot, context, card);
		if (arrowSlot.DestroyAfterImpaled)
		{
			await DestroyArrow(target, impaledInstance, arrowSlot);
		}
		else if (target.IsDead && impaledInstance.Arrows.Contains(arrowSlot))
		{
			await DislodgeArrow(combatState, impaledInstance, context, card);
		}
	}

	public static async Task DislodgeArrow(Creature target, Player player, PlayerChoiceContext context, CardModel? source)
	{
		if (!CreatureIsImpaledByArrow(target))
		{
			throw new InvalidOperationException("This creature has no arrow to dislodge");
		}
		ImpaledPower power = target.GetPowerInstances<ImpaledPower>().First((ImpaledPower impaledPower) => impaledPower.Applier == player.Creature);
		await DislodgeArrow(target.CombatState, power, context, source);
		await power.RemoveIfNoArrows();
	}

	/// <summary>
	/// Removes the arrow from the <see cref="T:MegaCrit.Sts2.Core.Models.Powers.ImpaledPower" /> enemy enemy and returns it to the player.
	/// </summary>
	/// <param name="combatState"></param>
	/// <param name="impaledArrow">Instance of impale taht we are dislodging.</param>
	/// <param name="context">Current player context.</param>
	/// <param name="source">The CardModel that caused the dislodge.</param>
	/// TODO: Consider case in MP when multiple folks have arrows shot into the same enemy
	public static async Task DislodgeArrow(ICombatState combatState, ImpaledPower impaledArrow, PlayerChoiceContext context, CardModel? source)
	{
		ArrowSlot arrow = impaledArrow.RemoveFirstArrow();
		if (arrow.DestroyAfterImpaled)
		{
			arrow.Status = ArrowStatus.Destroyed;
			return;
		}
		await Hook.AfterArrowDislodged(combatState, impaledArrow, arrow, context, source);
		RetrieveArrow(arrow);
		await Cmd.CustomScaledWait(0.1f, 0.25f);
	}

	private static async Task DestroyArrow(Creature target, ImpaledPower impaledInstance, ArrowSlot arrow)
	{
		if (arrow.Status != ArrowStatus.Destroyed)
		{
			if (target.IsAlive)
			{
				await impaledInstance.ForceRemoveArrow(arrow);
			}
			arrow.Status = ArrowStatus.Destroyed;
		}
	}
}
