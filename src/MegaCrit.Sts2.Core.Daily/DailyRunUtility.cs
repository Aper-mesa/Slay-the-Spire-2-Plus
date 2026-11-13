using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Leaderboard;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Platform;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace MegaCrit.Sts2.Core.Daily;

/// <summary>
/// A little helper for uploading the score at the end of a daily run.
/// </summary>
public static class DailyRunUtility
{
	/// <summary>
	/// Format of the date that a daily run seed starts with. A daily seed is this date followed by "_Np", where N is
	/// the number of players in the run.
	/// </summary>
	private const string _dailySeedDateFormat = "dd_MM_yyyy";

	/// <summary>
	/// Uploads a score to the daily leaderboard for the given time.
	/// </summary>
	public static async Task UploadScore(DateTimeOffset time, int score, List<SerializablePlayer> players)
	{
		List<ulong> playerIdsInRun = players.Select((SerializablePlayer p) => p.NetId).ToList();
		if (playerIdsInRun != null && playerIdsInRun.Count == 1 && playerIdsInRun[0] == 1)
		{
			playerIdsInRun[0] = PlatformUtil.GetLocalPlayerId(LeaderboardManager.CurrentPlatform);
		}
		string leaderboardName = GetLeaderboardName(time, players.Count);
		if (!(await ShouldUploadScore(await LeaderboardManager.GetLeaderboard(leaderboardName), playerIdsInRun)))
		{
			Log.Info($"Player already uploaded score for daily {time}, ignoring new score");
			return;
		}
		await LeaderboardManager.UploadLocalScore(await LeaderboardManager.GetOrCreateLeaderboard(leaderboardName), score, playerIdsInRun);
		Log.Info($"Uploaded score of {score} for daily {time} to leaderboard {leaderboardName}");
	}

	/// <summary>
	/// Figures out whether we should upload a score to the passed leaderboard.
	/// If any player in the run has already submitted a score, then this returns false.
	/// </summary>
	public static async Task<bool> ShouldUploadScore(ILeaderboardHandle? handle, List<ulong> playerIdsInRun, CancellationToken cancelToken = default(CancellationToken))
	{
		if (handle == null)
		{
			return true;
		}
		if (playerIdsInRun != null && playerIdsInRun.Count == 1 && playerIdsInRun[0] == 1)
		{
			playerIdsInRun[0] = PlatformUtil.GetLocalPlayerId(LeaderboardManager.CurrentPlatform);
		}
		return (await LeaderboardManager.QueryLeaderboardForUsers(handle, playerIdsInRun, cancelToken)).Count <= 0;
	}

	/// <summary>
	/// Builds the seed for the daily run of the given day and player count.
	/// </summary>
	public static string GetDailySeed(DateTimeOffset dayServerTime, int playerCount)
	{
		return SeedHelper.CanonicalizeSeed($"{GetDailySeedDate(dayServerTime)}_{playerCount}p");
	}

	/// <summary>
	/// Returns the date part of the daily seed, which seeds everything that does not depend on player count.
	/// </summary>
	public static string GetDailySeedDate(DateTimeOffset dayServerTime)
	{
		return SeedHelper.CanonicalizeSeed(dayServerTime.ToString("dd_MM_yyyy", CultureInfo.InvariantCulture));
	}

	/// <summary>
	/// Recovers the day that a daily seed was built for, as midnight UTC. Returns false if the seed is not a daily
	/// seed (a random seed, or NGame.DebugSeedOverride).
	/// </summary>
	public static bool TryGetDailySeedDate(string? seed, out DateTimeOffset day)
	{
		day = default;
		if (seed == null || seed.Length < "dd_MM_yyyy".Length)
		{
			return false;
		}
		return DateTimeOffset.TryParseExact(seed.Substring(0, "dd_MM_yyyy".Length), "dd_MM_yyyy", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal, out day);
	}

	/// <param name="dateTime">Date to retrieve the leaderboard for. All fields other than year/month/day are ignored.</param>
	/// <param name="playerCount">Count of players to retrieve the leaderboard for.</param>
	/// <returns>The name of the leaderboard used for the passed date and player count.</returns>
	public static string GetLeaderboardName(DateTimeOffset dateTime, int playerCount)
	{
		PlatformBranch platformBranch = PlatformUtil.GetPlatformBranch();
		bool flag = (uint)(platformBranch - 2) <= 2u;
		if (!flag && NGame.IsReleaseGame())
		{
			return $"{dateTime.Year}_{dateTime.Month:D2}_{dateTime.Day:D2}_{playerCount}p";
		}
		return $"{dateTime.Year}_{dateTime.Month:D2}_{dateTime.Day:D2}_{playerCount}p_BETA";
	}

	/// <summary>
	/// Returns the leaderboard day <paramref name="days" /> away from <paramref name="day" />, or null if that day
	/// falls outside the representable <see cref="T:System.DateTimeOffset" /> range (e.g. the day before
	/// <see cref="F:System.DateTimeOffset.MinValue" />). The daily leaderboard probes its neighbouring days and is paged one
	/// day at a time, so the boundary arithmetic must not throw <see cref="T:System.ArgumentOutOfRangeException" />.
	/// </summary>
	public static DateTimeOffset? AddLeaderboardDays(DateTimeOffset day, int days)
	{
		try
		{
			return day + TimeSpan.FromDays(days);
		}
		catch (ArgumentOutOfRangeException)
		{
			return null;
		}
	}

	/// <summary>
	/// Returns true when <paramref name="day" /> is a later calendar day than <paramref name="currentTime" />. A
	/// leaderboard covers a whole day (see <see cref="M:MegaCrit.Sts2.Core.Daily.DailyRunUtility.GetLeaderboardName(System.DateTimeOffset,System.Int32)" />), so this compares dates, not instants.
	/// </summary>
	public static bool IsFutureLeaderboardDay(DateTimeOffset day, DateTimeOffset currentTime)
	{
		return day.Date > currentTime.Date;
	}

	/// <summary>
	/// Returns true when <paramref name="day" /> is the same calendar day as <paramref name="currentTime" />, on the
	/// same whole-day basis as <see cref="M:MegaCrit.Sts2.Core.Daily.DailyRunUtility.IsFutureLeaderboardDay(System.DateTimeOffset,System.DateTimeOffset)" />. The two days carry the time of day they were
	/// sampled at, so comparing them as instants reports today's leaderboard as a different day.
	/// </summary>
	public static bool IsCurrentLeaderboardDay(DateTimeOffset day, DateTimeOffset currentTime)
	{
		return day.Date == currentTime.Date;
	}

	/// <summary>
	/// Attempt to get the time from the time server.
	/// If the most recent request succeeded, this reuses its result. Otherwise it requests a new time, which replaces
	/// any request still in flight. Falls back to local time if the server is unreachable.
	/// </summary>
	public static async Task<TimeServerResult> GetTimeServerTime()
	{
		TimeServerResult? result = null;
		if (TimeServer.RequestTimeTask?.IsCompleted ?? false)
		{
			if (TimeServer.RequestTimeTask.IsCompletedSuccessfully)
			{
				result = await TimeServer.RequestTimeTask;
			}
			if (!result.HasValue)
			{
				try
				{
					result = await TimeServer.FetchDailyTime();
				}
				catch (Exception ex) when ((ex is HttpRequestException || ex is TaskCanceledException) ? true : false)
				{
					Log.Error(ex.ToString());
				}
			}
		}
		else
		{
			try
			{
				result = await TimeServer.FetchDailyTime();
			}
			catch (Exception ex2) when ((ex2 is HttpRequestException || ex2 is TaskCanceledException) ? true : false)
			{
				Log.Error(ex2.ToString());
			}
		}
		if (!result.HasValue)
		{
			Log.Info("Couldn't retrieve time from time server, using local time");
			result = new TimeServerResult
			{
				serverTime = DateTimeOffset.UtcNow,
				localReceivedTime = DateTimeOffset.UtcNow
			};
		}
		return result.Value;
	}
}
