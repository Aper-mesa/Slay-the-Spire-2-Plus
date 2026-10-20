using System;

namespace MegaCrit.Sts2.Core.Runs.Metrics;

public struct EncounterMetric(string id, int damage, int turns)
{
	public readonly string id = id;

	public readonly int damage = Math.Clamp(damage, 0, 100);

	public readonly int turns = turns;
}
