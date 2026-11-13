using System;
using System.Collections.Generic;

namespace MegaCrit.Sts2.Core.AutoSlay.Scenarios;

/// <summary>
/// The named `wait` conditions and their per-condition timeouts.
/// </summary>
public static class ScenarioConditions
{
	/// <summary>
	/// The watchdog timeout a scenario run installs, in place of AutoSlay's 30s.
	/// </summary>
	/// <remarks>
	/// A one-monster enemy turn measures ~6s at FastMode.Normal with animations playing, and a boss
	/// turn is a multiple of that. AutoSlay's 30s is tuned for FastMode with animations suppressed.
	/// </remarks>
	public static readonly TimeSpan watchdogTimeout = TimeSpan.FromSeconds(90L);

	/// <summary>
	/// How often a wait in progress reports itself. Scaled with <see cref="F:MegaCrit.Sts2.Core.AutoSlay.Scenarios.ScenarioConditions.watchdogTimeout" /> rather
	/// than inherited: AutoSlay's 5s is sized for its 30s budget, and at Normal speed a healthy enemy
	/// turn already outlasts it, so every green scenario log would carry progress lines.
	/// </summary>
	public static readonly TimeSpan watchdogLogInterval = TimeSpan.FromSeconds(15L);

	private static readonly IReadOnlyDictionary<ScenarioWaitCondition, ScenarioConditionDescriptor> _descriptors = new Dictionary<ScenarioWaitCondition, ScenarioConditionDescriptor>
	{
		[ScenarioWaitCondition.CombatStarted] = new ScenarioConditionDescriptor(ScenarioWaitCondition.CombatStarted, "combat-started", TimeSpan.FromSeconds(45L), "Combat did not reach the local player's play phase"),
		[ScenarioWaitCondition.TurnStarted] = new ScenarioConditionDescriptor(ScenarioWaitCondition.TurnStarted, "turn-started", TimeSpan.FromSeconds(60L), "The player's next turn did not start")
	};

	/// <summary>
	/// The descriptor for a condition.
	/// </summary>
	/// <exception cref="T:System.ArgumentOutOfRangeException">The condition has no descriptor.</exception>
	public static ScenarioConditionDescriptor For(ScenarioWaitCondition condition)
	{
		if (!_descriptors.TryGetValue(condition, out ScenarioConditionDescriptor value))
		{
			throw new ArgumentOutOfRangeException("condition", condition, "No descriptor for this wait condition.");
		}
		return value;
	}

	/// <summary>
	/// Resolves the name a scenario author writes. Returns null for an unknown name.
	/// </summary>
	public static ScenarioConditionDescriptor? Resolve(string name)
	{
		if (!ScenarioDirectives.waitConditions.TryGetValue(name, out var value))
		{
			return null;
		}
		return For(value);
	}
}
