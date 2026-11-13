using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Godot;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Nodes.Debug;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Saves;
using MegaCrit.Sts2.Core.Saves.Managers;

namespace MegaCrit.Sts2.Core.DevConsole.ConsoleCommands;

public class GetLogsConsoleCmd : AbstractConsoleCmd
{
	private const string _dateTimeSpecifier = "yyyy_MM_dd_HH_mm_ss";

	private const long _maxLogFileBytes = 8388608L;

	private const long _maxFeedbackZipBytes = 41943040L;

	private const long _maxCoreDumpBytes = 209715200L;

	public override string CmdName => "getlogs";

	public override string Args => "[test-feedback] <name:string>";

	public override string Description => "Gathers logs, automatically zips them to a file containing 'name', and opens the directory containing the zip file.";

	public override bool IsNetworked => false;

	public override bool DebugOnly => false;

	public override CmdResult Process(Player? issuingPlayer, string[] args)
	{
		string extraName = "";
		bool flag = false;
		if (args.Length != 0)
		{
			if (args[0] == "test-feedback")
			{
				flag = true;
			}
			else
			{
				extraName = "-" + args[0];
			}
		}
		string bugReportPath = GetBugReportPath(extraName);
		if (flag)
		{
			using FileAccessStream outputStream = new FileAccessStream(bugReportPath, Godot.FileAccess.ModeFlags.Write);
			ZipFeedbackLogs(outputStream, SaveManager.Instance.CurrentProfileId);
		}
		else
		{
			TaskHelper.RunSafely(GrabLogs(bugReportPath));
		}
		return new CmdResult(success: true, "Zipping files to '" + bugReportPath + "'...");
	}

	public static string GetBugReportPath(string extraName = "")
	{
		string userDataDir = OS.GetUserDataDir();
		if (userDataDir == null)
		{
			throw new InvalidOperationException("Unable to open the user data directory.");
		}
		string value = DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss");
		return Path.Combine(userDataDir, $"BugReport{extraName}-{value}.zip");
	}

	public static async Task GrabLogs(string bugReportPath)
	{
		if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
		{
			bugReportPath = bugReportPath.Replace('/', '\\');
		}
		bool consoleVisible = NDevConsole.Instance.Visible;
		if (consoleVisible)
		{
			NDevConsole.Instance.HideConsole();
			await NDevConsole.Instance.AwaitProcessFrame();
			await NDevConsole.Instance.AwaitProcessFrame();
		}
		Image image = NDevConsole.Instance.GetViewport().GetTexture().GetImage();
		if (consoleVisible)
		{
			NDevConsole.Instance.ShowConsole();
		}
		await using (FileStream outputStream = new FileStream(bugReportPath, FileMode.CreateNew))
		{
			ZipFiles(outputStream, image.SavePngToBuffer());
		}
		Error error = OS.ShellShowInFileManager(ProjectSettings.GlobalizePath(bugReportPath));
		if (error != Error.Ok)
		{
			Log.Error($"Error {error}: Cannot open OS file manager. Files zipped to '{bugReportPath}'");
		}
		else
		{
			Log.Info("Files zipped to '" + bugReportPath + "'");
		}
	}

	public static void ZipFiles(Stream outputStream, byte[] screenshotBytes)
	{
		if (RunManager.Instance.IsInProgress && RunManager.Instance.CombatReplayWriter.IsRecordingReplay)
		{
			RunManager.Instance.WriteReplay(stopRecording: false);
		}
		string baseDir = OS.GetExecutablePath().GetBaseDir();
		string text = ProjectSettings.GlobalizePath(UserDataPathProvider.GetAccountScopedBasePath(""));
		string text2 = ProjectSettings.GlobalizePath("user://");
		string path = Path.Combine(text2, "logs");
		string text3 = Path.Combine(baseDir, "release_info.json");
		string path2 = Path.Combine(text2, "sentry", "reports");
		ZipArchive archive = new ZipArchive(outputStream, ZipArchiveMode.Create, leaveOpen: true);
		try
		{
			if (Directory.Exists(path))
			{
				foreach (string item in Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories))
				{
					string relativePath = Path.GetRelativePath(text2, item);
					ArchiveLogFile(item, archive, relativePath, 8388608L);
				}
			}
			List<string> list = new List<string>();
			if (Directory.Exists(path2))
			{
				list.AddRange(Directory.GetFiles(path2));
			}
			if (OS.GetName() == "Windows")
			{
				string folderPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData);
				string path3 = Path.Combine(folderPath, "CrashDumps");
				if (Directory.Exists(path3))
				{
					string exeName = Path.GetFileNameWithoutExtension(OS.GetExecutablePath());
					list.AddRange(from f in Directory.GetFiles(path3)
						where Path.GetFileName(f).StartsWith(exeName, StringComparison.OrdinalIgnoreCase)
						select f);
				}
			}
			if (list.Count > 0)
			{
				string text4 = list.OrderByDescending(File.GetLastWriteTime).First();
				DateTimeOffset dateTimeOffset = File.GetLastWriteTime(text4);
				string text5 = "yyyy-MM-dd_HH-mm-ss";
				ArchiveFile(text4, archive, "crashes/crash_" + dateTimeOffset.ToString(text5) + Path.GetExtension(text4));
			}
			TryCollectLinuxCoreDump((string info) =>
			{
				ArchiveText(info, archive, "crashes/coredump_info.txt");
			}, (string coreFile) =>
			{
				ArchiveFile(coreFile, archive, "crashes/coredump.core");
				return false;
			});
			foreach (string allSaveFile in GetAllSaveFiles(text))
			{
				ArchiveSaveFile(allSaveFile, archive, "saves/" + Path.GetRelativePath(text, allSaveFile));
			}
			if (File.Exists(text3))
			{
				ArchiveFile(text3, archive, "release_info.json");
			}
			ArchiveBytes(screenshotBytes, archive, "screenshot.png");
		}
		finally
		{
			if (archive != null)
			{
				((IDisposable)archive).Dispose();
			}
		}
	}

	/// <summary>
	/// Builds a ZIP of logs and saves tailored for the feedback upload path, with every attachment that fits. See
	/// <see cref="M:MegaCrit.Sts2.Core.DevConsole.ConsoleCommands.GetLogsConsoleCmd.CollectFeedbackLogFiles(System.Int32)" /> and <see cref="M:MegaCrit.Sts2.Core.DevConsole.ConsoleCommands.GetLogsConsoleCmd.WriteFeedbackZip(System.IO.Stream,MegaCrit.Sts2.Core.DevConsole.ConsoleCommands.FeedbackLogFiles,System.Int32,System.Boolean,System.Int64)" />.
	/// </summary>
	public static void ZipFeedbackLogs(Stream outputStream, int profileId)
	{
		using FeedbackLogFiles feedbackLogFiles = CollectFeedbackLogFiles(profileId);
		WriteFeedbackZip(outputStream, feedbackLogFiles, feedbackLogFiles.Attachments.Count, includeOlderLogs: true, 41943040L);
	}

	/// <summary>
	/// Gathers the files for a feedback upload.
	/// Unlike <see cref="M:MegaCrit.Sts2.Core.DevConsole.ConsoleCommands.GetLogsConsoleCmd.ZipFiles(System.IO.Stream,System.Byte[])" />, this excludes the screenshot (already sent as a separate multipart part)
	/// and inactive profiles. Only the most recent run history file is included.
	/// </summary>
	public static FeedbackLogFiles CollectFeedbackLogFiles(int profileId)
	{
		WriteUnwrittenReplay();
		string baseDir = OS.GetExecutablePath().GetBaseDir();
		string text = ProjectSettings.GlobalizePath(UserDataPathProvider.GetAccountScopedBasePath(""));
		string text2 = ProjectSettings.GlobalizePath("user://");
		string path = Path.Combine(text2, "logs");
		string path2 = Path.Combine(baseDir, "release_info.json");
		List<FeedbackTextEntry> text3 = new List<FeedbackTextEntry>();
		List<FeedbackAttachment> attachments = new List<FeedbackAttachment>();
		string coreFile = null;
		if (File.Exists(path2))
		{
			AddFeedbackText(text3, "release_info.json", path2, 0L, shouldSanitize: false);
		}
		foreach (var (path3, text4) in GetFeedbackSaveFiles(text, profileId))
		{
			AddFeedbackSave(text3, "saves/" + text4, path3);
		}
		List<FeedbackTextEntry> list = new List<FeedbackTextEntry>();
		if (Directory.Exists(path))
		{
			List<string> list2 = Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories).OrderByDescending(File.GetLastWriteTime).ToList();
			for (int i = 0; i < list2.Count; i++)
			{
				AddFeedbackText((i < 2) ? text3 : list, Path.GetRelativePath(text2, list2[i]), list2[i], 8388608L, shouldSanitize: true);
			}
		}
		string text5 = FindNewestCrashDump(text2);
		if (text5 != null)
		{
			attachments.Add(new FeedbackAttachment(GetCrashDumpEntryName(text5), text5));
		}
		string path4 = Path.Combine(text, UserDataPathProvider.GetProfileDir(profileId), "replays/latest.mcr");
		if (File.Exists(path4))
		{
			attachments.Add(new FeedbackAttachment("saves/" + Path.GetRelativePath(text, path4), path4));
		}
		TryCollectLinuxCoreDump((string info) =>
		{
			text3.Add(new FeedbackTextEntry("crashes/coredump_info.txt", Encoding.UTF8.GetBytes(info)));
		}, (string text6) =>
		{
			long length = new FileInfo(text6).Length;
			if (length > 41943040)
			{
				Log.Warn($"Core dump is {length / 1048576} MB, more than the {40L} MB feedback zip budget, skipping");
				return false;
			}
			coreFile = text6;
			attachments.Add(new FeedbackAttachment("crashes/coredump.core", text6));
			return true;
		});
		return new FeedbackLogFiles(text3, attachments, coreFile, list);
	}

	/// <summary>
	/// Writes a feedback ZIP: all of the text, the older logs if <paramref name="includeOlderLogs" />, then the first
	/// <paramref name="maxAttachments" /> attachments that fit in <paramref name="budgetBytes" />.
	/// </summary>
	/// <returns>The indexes in <see cref="P:MegaCrit.Sts2.Core.DevConsole.ConsoleCommands.FeedbackLogFiles.Attachments" /> of the attachments written, in order. Pass
	/// the last of them as <paramref name="maxAttachments" /> to drop the largest attachment that this ZIP
	/// contains.</returns>
	public static List<int> WriteFeedbackZip(Stream outputStream, FeedbackLogFiles files, int maxAttachments, bool includeOlderLogs = true, long budgetBytes = 41943040L)
	{
		using BudgetedZipWriter budgetedZipWriter = new BudgetedZipWriter(outputStream, budgetBytes, files.GatheredAt);
		IEnumerable<FeedbackTextEntry> enumerable;
		if (!includeOlderLogs)
		{
			IEnumerable<FeedbackTextEntry> text = files.Text;
			enumerable = text;
		}
		else
		{
			enumerable = files.Text.Concat(files.OlderLogs);
		}
		foreach (FeedbackTextEntry item in enumerable)
		{
			budgetedZipWriter.AddBytes(item.EntryName, item.Content);
		}
		List<int> list = new List<int>();
		for (int i = 0; i < Math.Min(maxAttachments, files.Attachments.Count); i++)
		{
			FeedbackAttachment feedbackAttachment = files.Attachments[i];
			if (budgetedZipWriter.TryAddFile(feedbackAttachment.Path, feedbackAttachment.EntryName))
			{
				list.Add(i);
			}
		}
		return list;
	}

	/// <summary>
	/// Reads a file into a feedback text entry, or logs a warning and adds nothing if it can not be read.
	/// </summary>
	/// <param name="text">The list to add the entry to.</param>
	/// <param name="entryName">The path of the entry in the ZIP.</param>
	/// <param name="path">The file to read.</param>
	/// <param name="maxBytes">Keep only this many bytes from the end of the file, 0 for no limit (see
	/// <see cref="M:MegaCrit.Sts2.Core.DevConsole.ConsoleCommands.GetLogsConsoleCmd.ReadTailText(System.IO.Stream,System.Int64)" />). Only applies when <paramref name="shouldSanitize" /> is set.</param>
	/// <param name="shouldSanitize">If set, the file is read as UTF-8 text and sanitized. If not, it is copied byte for
	/// byte.</param>
	private static void AddFeedbackText(List<FeedbackTextEntry> text, string entryName, string path, long maxBytes, bool shouldSanitize)
	{
		try
		{
			using FileStream fileStream = new FileStream(path, FileMode.Open, System.IO.FileAccess.Read, FileShare.ReadWrite);
			byte[] content;
			if (shouldSanitize)
			{
				content = Encoding.UTF8.GetBytes(LogSanitizer.Sanitize(ReadTailText(fileStream, maxBytes)));
			}
			else
			{
				using MemoryStream memoryStream = new MemoryStream();
				fileStream.CopyTo(memoryStream);
				content = memoryStream.ToArray();
			}
			text.Add(new FeedbackTextEntry(entryName, content));
		}
		catch (Exception ex) when ((ex is IOException || ex is UnauthorizedAccessException) ? true : false)
		{
			Log.Warn("Could not read " + path + " for zipping: " + ex.Message);
		}
	}

	/// <summary>
	/// Reads a save into a feedback text entry with its Steam IDs anonymized, or logs a warning and adds nothing if it
	/// can not be read.
	/// </summary>
	private static void AddFeedbackSave(List<FeedbackTextEntry> text, string entryName, string path)
	{
		try
		{
			text.Add(new FeedbackTextEntry(entryName, LogSanitizer.AnonymizeSteamIds(ReadAllBytesShared(path))));
		}
		catch (Exception ex) when ((ex is IOException || ex is UnauthorizedAccessException) ? true : false)
		{
			Log.Warn("Could not read " + path + " for zipping: " + ex.Message);
		}
	}

	private static byte[] ReadAllBytesShared(string path)
	{
		using FileStream fileStream = new FileStream(path, FileMode.Open, System.IO.FileAccess.Read, FileShare.ReadWrite);
		using MemoryStream memoryStream = new MemoryStream();
		fileStream.CopyTo(memoryStream);
		return memoryStream.ToArray();
	}

	/// <summary>
	/// If we're in a run and we have an unwritten replay, write it now. We only do this at the end of combat otherwise.
	/// </summary>
	private static void WriteUnwrittenReplay()
	{
		if (RunManager.Instance.IsInProgress && RunManager.Instance.CombatReplayWriter.IsRecordingReplay)
		{
			RunManager.Instance.WriteReplay(stopRecording: false);
		}
	}

	/// <summary>
	/// Returns the save files that go in a feedback upload, as full paths and paths relative to the account directory.
	/// The replay is not one of them. It is an attachment.
	/// </summary>
	private static IEnumerable<(string fullPath, string relativePath)> GetFeedbackSaveFiles(string accountBasePath, int profileId)
	{
		int num = 6;
		List<string> list = new List<string>(num);
		CollectionsMarshal.SetCount(list, num);
		Span<string> span = CollectionsMarshal.AsSpan(list);
		int num2 = 0;
		span[num2] = ProfileSaveManager.GetProfileSavePath();
		num2++;
		span[num2] = "settings.save";
		num2++;
		span[num2] = ProgressSaveManager.GetProgressPathForProfile(profileId);
		num2++;
		span[num2] = RunSaveManager.GetRunSavePath(profileId, "current_run.save");
		num2++;
		span[num2] = RunSaveManager.GetRunSavePath(profileId, "current_run_mp.save");
		num2++;
		span[num2] = PrefsSaveManager.GetPrefsPath(profileId);
		List<string> list2 = list;
		string path = Path.Combine(accountBasePath, RunHistorySaveManager.GetHistoryPath(profileId));
		if (Directory.Exists(path))
		{
			string text = Directory.EnumerateFiles(path, "*", SearchOption.TopDirectoryOnly).OrderByDescending(File.GetLastWriteTime).FirstOrDefault();
			if (text != null)
			{
				list2.Add(Path.GetRelativePath(accountBasePath, text));
			}
		}
		foreach (string allSaveFile in GetAllSaveFiles(accountBasePath))
		{
			if (allSaveFile.EndsWith(".corrupt") && DateTimeOffset.Now - File.GetLastWriteTime(allSaveFile) < TimeSpan.FromDays(1))
			{
				list2.Add(Path.GetRelativePath(accountBasePath, allSaveFile));
			}
		}
		foreach (string item in list2)
		{
			string text2 = Path.Combine(accountBasePath, item);
			if (File.Exists(text2))
			{
				yield return (fullPath: text2, relativePath: item);
			}
		}
	}

	/// <summary>
	/// Returns the most recently written crash dump from Sentry's reports and, on Windows, the OS crash dump folder.
	/// </summary>
	private static string? FindNewestCrashDump(string userDirectory)
	{
		List<string> list = new List<string>();
		string path = Path.Combine(userDirectory, "sentry", "reports");
		if (Directory.Exists(path))
		{
			list.AddRange(Directory.GetFiles(path));
		}
		if (OS.GetName() == "Windows")
		{
			string folderPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData);
			string path2 = Path.Combine(folderPath, "CrashDumps");
			if (Directory.Exists(path2))
			{
				string exeName = Path.GetFileNameWithoutExtension(OS.GetExecutablePath());
				list.AddRange(from f in Directory.GetFiles(path2)
					where Path.GetFileName(f).StartsWith(exeName, StringComparison.OrdinalIgnoreCase)
					select f);
			}
		}
		return list.OrderByDescending(File.GetLastWriteTime).FirstOrDefault();
	}

	private static string GetCrashDumpEntryName(string dmpFile)
	{
		return "crashes/crash_" + ((DateTimeOffset)File.GetLastWriteTime(dmpFile)).ToString("yyyy-MM-dd_HH-mm-ss") + Path.GetExtension(dmpFile);
	}

	/// <summary>
	/// Best-effort collection of Linux core dumps via systemd-coredump.
	/// Silently does nothing if coredumpctl is unavailable or no dumps are found.
	/// </summary>
	/// <param name="addInfo">Receives the coredumpctl info text.</param>
	/// <param name="addCoreFile">Receives the path of a temporary copy of the core. Return true to keep the file and
	/// delete it yourself, or false to have it deleted when this method returns.</param>
	private static void TryCollectLinuxCoreDump(Action<string> addInfo, Func<string, bool> addCoreFile)
	{
		if (OS.GetName() != "Linux")
		{
			return;
		}
		string fileName = Path.GetFileName(OS.GetExecutablePath());
		try
		{
			using Process process = new Process();
			process.StartInfo = new ProcessStartInfo
			{
				FileName = "coredumpctl",
				Arguments = "info -1 --no-pager " + fileName,
				RedirectStandardOutput = true,
				UseShellExecute = false
			};
			process.Start();
			Task<string> task = process.StandardOutput.ReadToEndAsync();
			if (!task.Wait(10000))
			{
				process.Kill();
				process.WaitForExit(5000);
				Log.Warn("coredumpctl info timed out after 10s");
			}
			else
			{
				string result = task.Result;
				process.WaitForExit();
				if (process.ExitCode == 0 && !string.IsNullOrWhiteSpace(result))
				{
					addInfo(result);
				}
			}
		}
		catch (Exception ex)
		{
			Log.Warn("Could not collect coredumpctl info: " + ex.Message);
		}
		string text = Path.Combine(Path.GetTempPath(), $"sts2_coredump_{Guid.NewGuid()}.core");
		bool flag = false;
		try
		{
			using Process process2 = new Process();
			process2.StartInfo = new ProcessStartInfo
			{
				FileName = "coredumpctl",
				Arguments = "dump -1 --no-pager -o \"" + text + "\" " + fileName,
				UseShellExecute = false
			};
			process2.Start();
			if (!process2.WaitForExit(10000))
			{
				Log.Warn("coredumpctl dump timed out after 10s, killing process");
				process2.Kill();
				process2.WaitForExit(5000);
			}
			else if (process2.ExitCode != 0)
			{
				Log.Warn($"coredumpctl dump exited with code {process2.ExitCode}");
			}
			if (File.Exists(text))
			{
				long length = new FileInfo(text).Length;
				if (length > 209715200)
				{
					Log.Warn($"Core dump is {length / 1048576} MB which exceeds the {200L} MB limit, skipping");
				}
				else if (length > 0)
				{
					flag = addCoreFile(text);
				}
			}
		}
		catch (Exception ex2)
		{
			Log.Warn("Could not collect core dump: " + ex2.Message);
		}
		finally
		{
			if (!flag)
			{
				try
				{
					File.Delete(text);
				}
				catch
				{
				}
			}
		}
	}

	/// <summary>
	/// Returns all save files in the account directory.
	/// Includes everything to ensure we capture info that might give clues for debugging,
	/// including old migrations or other unexpected files.
	/// </summary>
	private static IEnumerable<string> GetAllSaveFiles(string accountBasePath)
	{
		if (!Directory.Exists(accountBasePath))
		{
			yield break;
		}
		foreach (string item in Directory.EnumerateFiles(accountBasePath, "*", SearchOption.AllDirectories))
		{
			yield return item;
		}
	}

	/// <summary>
	/// Reads text from a stream, truncating to the tail if it exceeds <paramref name="maxBytes" />.
	/// When truncated, skips past any partial UTF-8 character and partial line at the seek point,
	/// then prepends a truncation marker.
	/// </summary>
	/// <param name="stream">A readable, seekable stream containing UTF-8 text.</param>
	/// <param name="maxBytes">Maximum bytes to read from the tail. 0 means no limit.</param>
	/// <returns>The full or truncated text content.</returns>
	public static string ReadTailText(Stream stream, long maxBytes)
	{
		bool flag = false;
		if (maxBytes > 0 && stream.Length > maxBytes)
		{
			stream.Seek(stream.Length - maxBytes, SeekOrigin.Begin);
			while (stream.Position < stream.Length)
			{
				int num = stream.ReadByte();
				if (num == -1)
				{
					break;
				}
				if ((num & 0xC0) != 128)
				{
					stream.Seek(-1L, SeekOrigin.Current);
					break;
				}
			}
			flag = true;
		}
		using StreamReader streamReader = new StreamReader(stream, null, detectEncodingFromByteOrderMarks: true, -1, leaveOpen: true);
		string text = streamReader.ReadToEnd();
		if (flag)
		{
			int num2 = text.IndexOf('\n');
			if (num2 >= 0)
			{
				string text2 = text;
				int num3 = num2 + 1;
				text = text2.Substring(num3, text2.Length - num3);
			}
			text = $"[...truncated, showing last ~{maxBytes / 1048576} MB...]\n" + text;
		}
		return text;
	}

	private static void ArchiveLogFile(string file, ZipArchive archive, string entryName, long maxBytes)
	{
		entryName = entryName.Replace("\\", "/");
		ZipArchiveEntry zipArchiveEntry = archive.CreateEntry(entryName);
		try
		{
			using FileStream stream = new FileStream(file, FileMode.Open, System.IO.FileAccess.Read, FileShare.ReadWrite);
			string text = ReadTailText(stream, maxBytes);
			string value = LogSanitizer.Sanitize(text);
			using StreamWriter streamWriter = new StreamWriter(zipArchiveEntry.Open());
			streamWriter.Write(value);
		}
		catch (FileNotFoundException)
		{
			Log.Error("Could not find file for zipping: " + file);
		}
	}

	private static void ArchiveFile(string file, ZipArchive archive, string entryName)
	{
		entryName = entryName.Replace("\\", "/");
		ZipArchiveEntry zipArchiveEntry = archive.CreateEntry(entryName);
		try
		{
			using FileStream fileStream = new FileStream(file, FileMode.Open, System.IO.FileAccess.Read, FileShare.ReadWrite);
			using Stream destination = zipArchiveEntry.Open();
			fileStream.CopyTo(destination);
		}
		catch (FileNotFoundException)
		{
			Log.Error("Could not find file for zipping: " + file);
		}
	}

	private static void ArchiveSaveFile(string file, ZipArchive archive, string entryName)
	{
		try
		{
			byte[] array = LogSanitizer.AnonymizeSteamIds(ReadAllBytesShared(file));
			using Stream stream = archive.CreateEntry(entryName.Replace("\\", "/")).Open();
			stream.Write(array);
		}
		catch (FileNotFoundException)
		{
			Log.Error("Could not find file for zipping: " + file);
		}
	}

	private static void ArchiveText(string text, ZipArchive archive, string entryName)
	{
		ZipArchiveEntry zipArchiveEntry = archive.CreateEntry(entryName);
		using StreamWriter streamWriter = new StreamWriter(zipArchiveEntry.Open());
		streamWriter.Write(text);
	}

	private static void ArchiveBytes(byte[] bytes, ZipArchive archive, string entryName)
	{
		ZipArchiveEntry zipArchiveEntry = archive.CreateEntry(entryName);
		using Stream stream = zipArchiveEntry.Open();
		stream.Write(bytes);
	}
}
