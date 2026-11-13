using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Map;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Exceptions;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;

namespace MegaCrit.Sts2.Core.DevConsole.ConsoleCommands;

public class FightConsoleCmd : AbstractConsoleCmd
{
	/// <summary>Parsed arguments. A null <see cref="P:MegaCrit.Sts2.Core.DevConsole.ConsoleCommands.FightConsoleCmd.FightArgs.RngSeed" /> means randomize, which is the default.</summary>
	public readonly record struct FightArgs(string EncounterId, ulong? RngSeed);

	public override string CmdName => "fight";

	public override string Args => "<id:string> [rng-seed:ulong]";

	public override string Description => "Jumps a player to a specific encounter.";

	public override bool IsNetworked => true;

	/// <summary>Pure argument handling for <see cref="M:MegaCrit.Sts2.Core.DevConsole.ConsoleCommands.FightConsoleCmd.Process(MegaCrit.Sts2.Core.Entities.Players.Player,System.String[])" />, so it can be tested without a run.</summary>
	public static bool TryParseFightArgs(string[] args, out FightArgs parsed, out string error)
	{
		parsed = default;
		if (args.Length == 0)
		{
			error = "No encounter name specified.";
			return false;
		}
		if (args.Length > 2)
		{
			error = "Too many arguments, expected '<id> [rng-seed]' but got '" + string.Join(' ', args) + "'.";
			return false;
		}
		ulong? rngSeed = null;
		if (args.Length == 2)
		{
			if (!ulong.TryParse(args[1], out var result))
			{
				error = $"'{args[1]}' is not a valid rng seed. Expected a whole number from 0 to {ulong.MaxValue}.";
				return false;
			}
			rngSeed = result;
		}
		parsed = new FightArgs(args[0], rngSeed);
		error = "";
		return true;
	}

	public override CmdResult Process(Player? issuingPlayer, string[] args)
	{
		if (!TryParseFightArgs(args, out FightArgs parsed, out string error))
		{
			return new CmdResult(success: false, error);
		}
		if (!RunManager.Instance.IsInProgress)
		{
			return new CmdResult(success: false, "A run is currently not in progress!");
		}
		ModelId modelId = new ModelId(ModelId.SlugifyCategory<EncounterModel>(), parsed.EncounterId.ToUpperInvariant());
		EncounterModel encounterModel;
		try
		{
			encounterModel = ModelDb.GetById<EncounterModel>(modelId).ToMutable();
		}
		catch (ModelNotFoundException)
		{
			return new CmdResult(success: false, "Encounter '" + modelId.Entry + "' not found");
		}
		ulong? rngSeed = parsed.RngSeed;
		ulong value;
		if (rngSeed.HasValue)
		{
			ulong valueOrDefault = rngSeed.GetValueOrDefault();
			encounterModel.DebugSetRng(valueOrDefault);
			value = valueOrDefault;
		}
		else
		{
			value = encounterModel.DebugRandomizeRng();
		}
		Task task = RunManager.Instance.EnterRoomDebug(RoomType.Monster, MapPointType.Unassigned, encounterModel);
		return new CmdResult(task, success: true, $"Jumped to encounter: '{encounterModel.Id.Entry}' (rng seed {value})");
	}

	public override CompletionResult GetArgumentCompletions(Player? player, string[] args)
	{
		if (args.Length <= 1)
		{
			List<string> candidates = ModelDb.AllEncounters.Select((EncounterModel e) => e.Id.Entry).ToList();
			return CompleteArgument(candidates, Array.Empty<string>(), args.FirstOrDefault() ?? "");
		}
		return new CompletionResult
		{
			Type = CompletionType.Argument,
			ArgumentContext = CmdName
		};
	}
}
