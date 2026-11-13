using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MegaCrit.Sts2.Core.Logging;

namespace MegaCrit.Sts2.Core.DevConsole.ConsoleCommands;

/// <summary>
/// The files for a feedback upload. They are gathered once, on the main thread, so that every attempt includes the
/// same text and a retry can write the zip again with fewer attachments. Writing the zip then only reads the attachment
/// files and needs no game state.
/// </summary>
public sealed class FeedbackLogFiles : IDisposable
{
	private readonly string? _temporaryFile;

	/// <summary>
	/// Logs, saves and other text, already read and sanitized. It compresses well, so it is always uploaded.
	/// </summary>
	public IReadOnlyList<FeedbackTextEntry> Text { get; }

	/// <summary>
	/// Logs from before the previous session, already read and sanitized. They are sent until an attempt without any
	/// attachments fails, in case the text on its own is too large.
	/// </summary>
	public IReadOnlyList<FeedbackTextEntry> OlderLogs { get; }

	/// <summary>
	/// Large binary files, smallest first. An attachment that does not fit in the zip budget is left out, and after
	/// each failed upload attempt, the next attempt leaves out the largest one that the failed attempt included.
	/// </summary>
	public IReadOnlyList<FeedbackAttachment> Attachments { get; }

	/// <summary>
	/// When the files were gathered. Every zip of them stores this as its entry times, so that two attempts with the
	/// same attachments send the same bytes and the server keeps only one report.
	/// </summary>
	public DateTimeOffset GatheredAt { get; } = DateTimeOffset.Now;

	/// <param name="text">See <see cref="P:MegaCrit.Sts2.Core.DevConsole.ConsoleCommands.FeedbackLogFiles.Text" />.</param>
	/// <param name="attachments">See <see cref="P:MegaCrit.Sts2.Core.DevConsole.ConsoleCommands.FeedbackLogFiles.Attachments" />. Any order; they are sorted by size.</param>
	/// <param name="temporaryFile">A file that only this upload uses, such as a copy of a core dump. It is deleted on
	/// dispose.</param>
	/// <param name="olderLogs">See <see cref="P:MegaCrit.Sts2.Core.DevConsole.ConsoleCommands.FeedbackLogFiles.OlderLogs" />.</param>
	public FeedbackLogFiles(IReadOnlyList<FeedbackTextEntry> text, IReadOnlyList<FeedbackAttachment> attachments, string? temporaryFile = null, IReadOnlyList<FeedbackTextEntry>? olderLogs = null)
	{
		Text = text;
		OlderLogs = olderLogs ?? Array.Empty<FeedbackTextEntry>();
		Attachments = attachments.OrderBy((FeedbackAttachment a) => GetSize(a.Path)).ToList();
		_temporaryFile = temporaryFile;
	}

	private static long GetSize(string path)
	{
		try
		{
			return new FileInfo(path).Length;
		}
		catch (Exception ex) when ((ex is IOException || ex is UnauthorizedAccessException) ? true : false)
		{
			return 0L;
		}
	}

	public void Dispose()
	{
		if (_temporaryFile == null)
		{
			return;
		}
		try
		{
			File.Delete(_temporaryFile);
		}
		catch (Exception ex)
		{
			Log.Warn("Could not delete " + _temporaryFile + ": " + ex.Message);
		}
	}
}
