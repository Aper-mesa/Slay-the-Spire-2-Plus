using System;
using System.Collections.Generic;
using System.Linq;

namespace MegaCrit.Sts2.Core.AutoSlay.Scenarios;

/// <summary>
/// The closed directive set. The bar for adding one: it is about the runner or about time, never
/// about game state. Anything that mutates game state becomes a real console command instead.
/// </summary>
public static class ScenarioDirectives
{
	public const string wait = "wait";

	public const string endTurn = "endturn";

	/// <summary>
	/// Every directive name. Directives shadow console commands, so this set must stay disjoint
	/// from the console registry.
	/// </summary>
	public static readonly IReadOnlySet<string> all = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "wait", "endturn" };

	/// <summary>
	/// The `wait` conditions, keyed by the name scenario authors write.
	/// </summary>
	public static readonly IReadOnlyDictionary<string, ScenarioWaitCondition> waitConditions = new Dictionary<string, ScenarioWaitCondition>(StringComparer.OrdinalIgnoreCase)
	{
		["combat-started"] = ScenarioWaitCondition.CombatStarted,
		["turn-started"] = ScenarioWaitCondition.TurnStarted
	};

	/// <summary>
	/// Directive names that are also registered console command names, sorted for a stable message.
	/// </summary>
	public static IReadOnlyList<string> FindCollisions(IReadOnlySet<string> reservedNames)
	{
		HashSet<string> hashSet = new HashSet<string>(reservedNames, StringComparer.OrdinalIgnoreCase);
		return all.Where(hashSet.Contains).OrderBy((string name) => name, StringComparer.Ordinal).ToList();
	}
}
