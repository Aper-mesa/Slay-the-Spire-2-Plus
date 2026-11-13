using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;

namespace MegaCrit.Sts2.Core.DevConsole.ConsoleCommands;

/// <summary>
/// Console command which causes the player to win the combat or the entire run
/// </summary>
public class WinConsoleCmd : AbstractConsoleCmd
{
	public override string CmdName => "win";

	public override string Args => "[run]";

	public override string Description => "You win the combat or the entire run";

	public override bool IsNetworked => true;

	public override CmdResult Process(Player? issuingPlayer, string[] args)
	{
		if (args.Length != 0)
		{
			if (args[0] == "run")
			{
				Task task = RunManager.Instance.WinRun();
				return new CmdResult(task, success: true, "Won the run");
			}
			return new CmdResult(success: false, "Invalid arg " + args[0]);
		}
		if (!CombatManager.Instance.IsInProgress)
		{
			return new CmdResult(success: false, "This doesn't appear to be a combat!");
		}
		List<Creature> list = CombatManager.Instance.DebugOnlyGetState().Enemies.ToList();
		Task task2 = KillEnemies(list);
		IEnumerable<string> values = from c in list
			select c.Monster into m
			where m != null
			select m.Id.Entry.ToString();
		return new CmdResult(task2, success: true, "Killed: [" + string.Join(",", values) + "]");
	}

	private async Task KillEnemies(List<Creature> creatures)
	{
		foreach (Creature creature in creatures)
		{
			creature.RemoveAllPowersInternalExcept();
			await CreatureCmd.Kill(creature);
		}
		await CombatManager.Instance.CheckWinCondition();
	}
}
