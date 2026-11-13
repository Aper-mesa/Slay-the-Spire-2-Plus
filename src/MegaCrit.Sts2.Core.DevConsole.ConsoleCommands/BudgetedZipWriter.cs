using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using MegaCrit.Sts2.Core.Logging;

namespace MegaCrit.Sts2.Core.DevConsole.ConsoleCommands;

/// <summary>
/// Writes a zip archive in which some entries are limited by a byte budget.
/// <see cref="M:MegaCrit.Sts2.Core.DevConsole.ConsoleCommands.BudgetedZipWriter.AddBytes(System.String,System.Byte[])" /> always writes its entry. Use it for text, which compresses well.
/// <see cref="M:MegaCrit.Sts2.Core.DevConsole.ConsoleCommands.BudgetedZipWriter.TryAddFile(System.String,System.String)" /> writes its entry only if it fits: before the entry is written, its uncompressed size is
/// added to the archive bytes already written and to the central directory still to be written when the archive is
/// closed, and if the sum is over the budget, the entry is skipped. Its compressed size is not known until it is
/// written, so counting the uncompressed size keeps the archive within the budget whatever the entry compresses to, as
/// long as the entries that are always written fit.
/// Add the always-written entries first, then the budgeted ones in priority order: when the budget runs out, the
/// entries added last are the ones dropped.
/// </summary>
public sealed class BudgetedZipWriter : IDisposable
{
	private sealed class CountingWriteStream(Stream inner) : Stream
	{
		public long BytesWritten { get; private set; }

		public override bool CanRead => false;

		public override bool CanSeek => false;

		public override bool CanWrite => true;

		public override long Length
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		public override long Position
		{
			get
			{
				throw new NotSupportedException();
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			inner.Write(buffer, offset, count);
			BytesWritten += count;
		}

		public override void Write(ReadOnlySpan<byte> buffer)
		{
			inner.Write(buffer);
			BytesWritten += buffer.Length;
		}

		public override void Flush()
		{
			inner.Flush();
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			throw new NotSupportedException();
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotSupportedException();
		}

		public override void SetLength(long value)
		{
			throw new NotSupportedException();
		}
	}

	private const long _localOverheadBytes = 256L;

	private const long _centralRecordOverheadBytes = 128L;

	private const long _endRecordBytes = 128L;

	private readonly CountingWriteStream _output;

	private readonly ZipArchive _archive;

	private readonly long _budgetBytes;

	private readonly DateTimeOffset? _entryTime;

	private long _pendingCentralDirectoryBytes = 128L;

	/// <param name="output">The stream to write the archive to.</param>
	/// <param name="budgetBytes">The budget for the entries added with <see cref="M:MegaCrit.Sts2.Core.DevConsole.ConsoleCommands.BudgetedZipWriter.TryAddFile(System.String,System.String)" />.</param>
	/// <param name="entryTime">The modified time stored for every entry. If null, each entry gets the time it is
	/// written, so the same files written twice give different bytes.</param>
	public BudgetedZipWriter(Stream output, long budgetBytes, DateTimeOffset? entryTime = null)
	{
		ArgumentOutOfRangeException.ThrowIfLessThan(budgetBytes, 128L, "budgetBytes");
		_budgetBytes = budgetBytes;
		_entryTime = entryTime;
		_output = new CountingWriteStream(output);
		_archive = new ZipArchive(_output, ZipArchiveMode.Create, leaveOpen: true);
	}

	/// <summary>
	/// Adds a file byte for byte if it fits in the budget. Returns false if it does not fit or can not be read.
	/// </summary>
	public bool TryAddFile(string file, string entryName)
	{
		entryName = CanonicalizeEntryName(entryName);
		try
		{
			using FileStream fileStream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
			if (!Fits(entryName, fileStream.Length))
			{
				return false;
			}
			using Stream destination = CreateEntry(entryName).Open();
			fileStream.CopyTo(destination);
			return true;
		}
		catch (Exception ex) when ((ex is IOException || ex is UnauthorizedAccessException) ? true : false)
		{
			Log.Warn("Could not read " + file + " for zipping: " + ex.Message);
			return false;
		}
	}

	/// <summary>
	/// Adds bytes from memory, whatever the budget.
	/// </summary>
	public void AddBytes(string entryName, byte[] bytes)
	{
		entryName = CanonicalizeEntryName(entryName);
		_pendingCentralDirectoryBytes += GetCentralRecordBytes(entryName);
		using Stream stream = CreateEntry(entryName).Open();
		stream.Write(bytes);
	}

	public void Dispose()
	{
		_archive.Dispose();
	}

	private ZipArchiveEntry CreateEntry(string entryName)
	{
		ZipArchiveEntry zipArchiveEntry = _archive.CreateEntry(entryName);
		DateTimeOffset? entryTime = _entryTime;
		if (entryTime.HasValue)
		{
			DateTimeOffset valueOrDefault = entryTime.GetValueOrDefault();
			zipArchiveEntry.LastWriteTime = valueOrDefault;
		}
		return zipArchiveEntry;
	}

	private static long GetCentralRecordBytes(string entryName)
	{
		return 128L + (long)Encoding.UTF8.GetByteCount(entryName);
	}

	private bool Fits(string entryName, long uncompressedBytes)
	{
		long num = Encoding.UTF8.GetByteCount(entryName);
		long centralRecordBytes = GetCentralRecordBytes(entryName);
		long num2 = uncompressedBytes + uncompressedBytes / 1000 + 256 + num + centralRecordBytes;
		if (_output.BytesWritten + _pendingCentralDirectoryBytes + num2 > _budgetBytes)
		{
			Log.Warn($"Skipping {entryName} ({uncompressedBytes} bytes): it would take the zip past {_budgetBytes} bytes, {_output.BytesWritten} bytes are already written");
			return false;
		}
		_pendingCentralDirectoryBytes += centralRecordBytes;
		return true;
	}

	private static string CanonicalizeEntryName(string entryName)
	{
		return entryName.Replace("\\", "/");
	}
}
