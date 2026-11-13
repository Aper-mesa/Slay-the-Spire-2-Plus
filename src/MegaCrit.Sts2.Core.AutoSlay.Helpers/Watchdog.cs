using System;
using System.Text;
using Godot;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Screens.Overlays;
using MegaCrit.Sts2.Core.Runs;

namespace MegaCrit.Sts2.Core.AutoSlay.Helpers;

/// <summary>
/// Watchdog that detects when an automated driver gets stuck and dumps diagnostic state.
/// </summary>
public class Watchdog
{
	private sealed class WatchdogScope : IDisposable
	{
		private readonly Watchdog? _previous;

		private bool _disposed;

		public WatchdogScope(Watchdog watchdog)
		{
			_previous = Current;
			Current = watchdog;
		}

		public void Dispose()
		{
			if (!_disposed)
			{
				_disposed = true;
				Current = _previous;
			}
		}
	}

	private readonly Action<string> _logInfo;

	private readonly Action<string> _logError;

	private readonly TimeSpan _timeout;

	private readonly TimeSpan _logInterval;

	private readonly Func<DateTime> _clock;

	private DateTime _lastProgressTime;

	private DateTime _lastLogTime;

	private string _lastActivity = "Starting";

	/// <summary>
	/// The watchdog the currently running driver registered, for <see cref="T:MegaCrit.Sts2.Core.AutoSlay.Helpers.WaitHelper" /> integration.
	/// Set only through <see cref="M:MegaCrit.Sts2.Core.AutoSlay.Helpers.Watchdog.Use(MegaCrit.Sts2.Core.AutoSlay.Helpers.Watchdog)" />, which restores the previous watchdog on dispose.
	/// </summary>
	public static Watchdog? Current { get; private set; }

	/// <summary>
	/// The <paramref name="clock" /> is injected so stall behaviour is testable without sleeping.
	/// </summary>
	public Watchdog(Action<string> logInfo, Action<string> logError, TimeSpan? timeout = null, TimeSpan? logInterval = null, Func<DateTime>? clock = null)
	{
		_logInfo = logInfo;
		_logError = logError;
		_timeout = timeout ?? AutoSlayConfig.watchdogTimeout;
		_logInterval = logInterval ?? AutoSlayConfig.watchdogLogInterval;
		_clock = clock ?? ((Func<DateTime>)(() => DateTime.UtcNow));
		Reset("Initialized");
	}

	/// <summary>
	/// Installs <paramref name="watchdog" /> as <see cref="P:MegaCrit.Sts2.Core.AutoSlay.Helpers.Watchdog.Current" /> for the lifetime of the
	/// returned scope, restoring whatever was there before on dispose.
	/// </summary>
	public static IDisposable Use(Watchdog watchdog)
	{
		return new WatchdogScope(watchdog);
	}

	/// <summary>
	/// Resets the watchdog timer. Call this whenever meaningful progress is made.
	/// </summary>
	public void Reset(string activity)
	{
		_lastProgressTime = _clock();
		_lastActivity = activity;
	}

	/// <summary>
	/// Checks if the watchdog has timed out. Call this periodically (e.g., in WaitHelper).
	/// Throws TimeoutException if no progress for too long.
	/// </summary>
	public void Check()
	{
		DateTime dateTime = _clock();
		TimeSpan timeSpan = dateTime - _lastProgressTime;
		if (timeSpan > _logInterval && dateTime - _lastLogTime > _logInterval)
		{
			_lastLogTime = dateTime;
			_logInfo($"[Watchdog] Still on '{_lastActivity}' after {timeSpan.TotalSeconds:F1}s (stalls at {_timeout.TotalSeconds:F0}s)");
		}
		if (timeSpan > _timeout)
		{
			string value = DumpState();
			_logError($"[Watchdog] Stuck detected! No progress for {timeSpan.TotalSeconds:F1}s\nLast activity: {_lastActivity}\n{value}");
			throw new DriverTimeoutException($"Watchdog timeout: No progress for {timeSpan.TotalSeconds:F1}s. Last activity: {_lastActivity}");
		}
	}

	/// <summary>
	/// Dumps the current game state for debugging.
	/// </summary>
	public static string DumpState()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("=== Driver State Dump ===");
		try
		{
			RunState runState = RunManager.Instance.DebugOnlyGetState();
			if (runState != null)
			{
				stringBuilder.AppendLine("Run State:");
				StringBuilder stringBuilder2 = stringBuilder;
				StringBuilder stringBuilder3 = stringBuilder2;
				StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(24, 3, stringBuilder2);
				handler.AppendLiteral("  Floor: ");
				handler.AppendFormatted(runState.TotalFloor);
				handler.AppendLiteral(" (Act ");
				handler.AppendFormatted(runState.CurrentActIndex + 1);
				handler.AppendLiteral(", Floor ");
				handler.AppendFormatted(runState.ActFloor);
				handler.AppendLiteral(")");
				stringBuilder3.AppendLine(ref handler);
				stringBuilder2 = stringBuilder;
				StringBuilder stringBuilder4 = stringBuilder2;
				handler = new StringBuilder.AppendInterpolatedStringHandler(8, 1, stringBuilder2);
				handler.AppendLiteral("  Room: ");
				handler.AppendFormatted(runState.CurrentRoom?.RoomType.ToString() ?? "null");
				stringBuilder4.AppendLine(ref handler);
			}
			else
			{
				stringBuilder.AppendLine("Run State: Not initialized");
			}
			if (NOverlayStack.Instance != null)
			{
				StringBuilder stringBuilder2 = stringBuilder;
				StringBuilder stringBuilder5 = stringBuilder2;
				StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(23, 1, stringBuilder2);
				handler.AppendLiteral("Overlay Stack: ");
				handler.AppendFormatted(NOverlayStack.Instance.ScreenCount);
				handler.AppendLiteral(" screens");
				stringBuilder5.AppendLine(ref handler);
				IOverlayScreen overlayScreen = NOverlayStack.Instance.Peek();
				if (overlayScreen != null)
				{
					stringBuilder2 = stringBuilder;
					StringBuilder stringBuilder6 = stringBuilder2;
					handler = new StringBuilder.AppendInterpolatedStringHandler(11, 1, stringBuilder2);
					handler.AppendLiteral("  Current: ");
					handler.AppendFormatted(overlayScreen.GetType().Name);
					stringBuilder6.AppendLine(ref handler);
				}
			}
			Node instance = NGame.Instance;
			if (instance != null)
			{
				stringBuilder.AppendLine("Scene Container:");
				Node nodeOrNull = instance.GetNodeOrNull("RootSceneContainer");
				if (nodeOrNull != null)
				{
					foreach (Node child in nodeOrNull.GetChildren())
					{
						string value;
						if (child is Control control)
						{
							value = (control.Visible ? "visible" : "hidden");
						}
						else
						{
							value = "";
						}
						StringBuilder stringBuilder2 = stringBuilder;
						StringBuilder stringBuilder7 = stringBuilder2;
						StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(10, 3, stringBuilder2);
						handler.AppendLiteral("  - ");
						handler.AppendFormatted(child.Name);
						handler.AppendLiteral(" (");
						handler.AppendFormatted(child.GetType().Name);
						handler.AppendLiteral(") [");
						handler.AppendFormatted(value);
						handler.AppendLiteral("]");
						stringBuilder7.AppendLine(ref handler);
					}
				}
			}
		}
		catch (Exception ex)
		{
			StringBuilder stringBuilder2 = stringBuilder;
			StringBuilder stringBuilder8 = stringBuilder2;
			StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(21, 1, stringBuilder2);
			handler.AppendLiteral("Error dumping state: ");
			handler.AppendFormatted(ex.Message);
			stringBuilder8.AppendLine(ref handler);
		}
		stringBuilder.AppendLine("=== End State Dump ===");
		return stringBuilder.ToString();
	}
}
