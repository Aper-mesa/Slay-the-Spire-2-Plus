using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Nodes.Vfx.Utilities;
using MegaCrit.Sts2.Core.ValueProps;

namespace MegaCrit.Sts2.Core.Models.Powers;

public sealed class GodByrdPower : PowerModel
{
	private const string _attackSfx = "event:/sfx/byrdpip/byrd_2_attack";

	public override PowerType Type => PowerType.Buff;

	public override PowerStackType StackType => PowerStackType.Counter;

	public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
	{
		if (player == Owner.Player && AmountOnTurnStart != 0)
		{
			Creature byrdine = Owner.Player.PlayerCombatState.GetPet<ByrdineMonster>();
			if (byrdine != null)
			{
				await CreatureCmd.TriggerAnim(byrdine, "Attack", 0f);
				await Cmd.Wait(0.6f);
				SfxCmd.Play("event:/sfx/byrdpip/byrd_2_attack");
				NByrdineAttackVfx.Create(byrdine);
				await Cmd.Wait(0.15f);
			}
			await CreatureCmd.Damage(choiceContext, CombatState.HittableEnemies, Amount, ValueProp.Unpowered, Owner);
			await PowerCmd.Remove(this);
		}
	}
}
