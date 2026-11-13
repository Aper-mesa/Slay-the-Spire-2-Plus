namespace MegaCrit.Sts2.Core.DevConsole.ConsoleCommands;

/// <summary>
/// A text entry of a feedback zip.
/// </summary>
public sealed record FeedbackTextEntry(string EntryName, byte[] Content);
