namespace MegaCrit.Sts2.Core.DevConsole.ConsoleCommands;

/// <summary>
/// A binary file in a feedback zip, copied byte for byte.
/// </summary>
public sealed record FeedbackAttachment(string EntryName, string Path);
