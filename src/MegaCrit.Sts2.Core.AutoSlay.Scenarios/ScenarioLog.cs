using System;
using System.IO;
using MegaCrit.Sts2.Core.Logging;

namespace MegaCrit.Sts2.Core.AutoSlay.Scenarios;

/// <summary>
/// Structured logging for scenario runs, with its own writer and its own prefix.
/// </summary>
public static class ScenarioLog
{
	private const string _prefix = "[Scenario]";

	private static readonly string[] _newlineSeparators = new string[3] { "\r\n", "\n", "\r" };

	private static StreamWriter? _fileWriter;

	private static readonly object _lock = new object();

	public static void OpenLogFile(string path)
	{
		string directoryName = Path.GetDirectoryName(Path.GetFullPath(path));
		if (!string.IsNullOrEmpty(directoryName))
		{
			Directory.CreateDirectory(directoryName);
		}
		lock (_lock)
		{
			_fileWriter?.Dispose();
			_fileWriter = new StreamWriter(path, append: false)
			{
				AutoFlush = true
			};
		}
	}

	public static void CloseLogFile()
	{
		lock (_lock)
		{
			_fileWriter?.Dispose();
			_fileWriter = null;
		}
	}

	public static void Info(string message)
	{
		string text = "[Scenario] " + message;
		Log.Info(text);
		WriteToFile("INFO", text);
	}

	public static void Warn(string message)
	{
		string text = "[Scenario] " + message;
		Log.Warn(text);
		WriteToFile("WARN", text);
	}

	public static void Error(string message)
	{
		string text = "[Scenario] " + message;
		Log.Error(text);
		WriteToFile("ERROR", text);
	}

	public static void Error(string message, Exception ex)
	{
		string text = $"{"[Scenario]"} {message}: {ex.Message}\n{ex.StackTrace}";
		Log.Error(text);
		WriteToFile("ERROR", text);
	}

	/// <summary>The one sentinel a successful run emits. Harnesses judge on this, not the exit code.</summary>
	public static void Completed(string scenarioName)
	{
		Info("Scenario completed successfully: " + scenarioName);
	}

	public static void Failed(string reason)
	{
		Error("Scenario failed: " + CollapseNewlines(reason));
	}

	/// <summary>
	/// Folds newlines into ' | ' so a multi-line reason stays one prefixed record. DevConsole's
	/// "command does not exist" message is multi-line and would otherwise emit unprefixed lines.
	/// </summary>
	public static string CollapseNewlines(string text)
	{
		if (string.IsNullOrEmpty(text))
		{
			return text;
		}
		string[] array = text.Split(_newlineSeparators, StringSplitOptions.RemoveEmptyEntries);
		return string.Join(" | ", Array.ConvertAll(array, (string part) => part.Trim()));
	}

	private static void WriteToFile(string level, string message)
	{
		lock (_lock)
		{
			_fileWriter?.WriteLine($"{DateTime.Now:HH:mm:ss.fff} [{level}] {message}");
		}
	}
}
