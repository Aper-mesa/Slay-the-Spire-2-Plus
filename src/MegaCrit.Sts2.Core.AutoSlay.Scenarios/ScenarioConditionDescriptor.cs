using System;

namespace MegaCrit.Sts2.Core.AutoSlay.Scenarios;

/// <summary>
/// What a named `wait` condition means to the runner: how long it may block, and what to say when
/// it does not come true. A descriptor rather than a predicate, so all of it is assertable without a
/// running game. The predicate is looked up separately at execution time.
/// </summary>
/// <param name="Condition">The condition this describes.</param>
/// <param name="Name">The name scenario authors write.</param>
/// <param name="Timeout">How long the wait may block before it is a stall.</param>
/// <param name="FailureMessage">What to report on timeout.</param>
public sealed record ScenarioConditionDescriptor(ScenarioWaitCondition Condition, string Name, TimeSpan Timeout, string FailureMessage);
