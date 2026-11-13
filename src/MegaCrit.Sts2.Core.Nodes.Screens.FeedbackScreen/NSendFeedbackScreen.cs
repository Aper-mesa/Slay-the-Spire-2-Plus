using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Debug;
using MegaCrit.Sts2.Core.DevConsole.ConsoleCommands;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Screens.ScreenContext;
using MegaCrit.Sts2.Core.Platform;
using MegaCrit.Sts2.Core.Saves;
using MegaCrit.Sts2.Core.TestSupport;
using MegaCrit.Sts2.addons.mega_text;
using Sentry.Godot;

namespace MegaCrit.Sts2.Core.Nodes.Screens.FeedbackScreen;

[ScriptPath("res://src/Core/Nodes/Screens/FeedbackScreen/NSendFeedbackScreen.cs")]
public class NSendFeedbackScreen : Control, IScreenContext
{
	/// <summary>
	/// Cached StringNames for the methods contained in this class, for fast lookup.
	/// </summary>
	public new class MethodName : Control.MethodName
	{
		/// <summary>
		/// Cached name for the 'Create' method.
		/// </summary>
		public static readonly StringName Create = "Create";

		/// <summary>
		/// Cached name for the '_Ready' method.
		/// </summary>
		public new static readonly StringName _Ready = "_Ready";

		/// <summary>
		/// Cached name for the 'Relocalize' method.
		/// </summary>
		public static readonly StringName Relocalize = "Relocalize";

		/// <summary>
		/// Cached name for the 'OnDescriptionChanged' method.
		/// </summary>
		public static readonly StringName OnDescriptionChanged = "OnDescriptionChanged";

		/// <summary>
		/// Cached name for the 'SetScreenshot' method.
		/// </summary>
		public static readonly StringName SetScreenshot = "SetScreenshot";

		/// <summary>
		/// Cached name for the 'GetUploadedScreenshotSize' method.
		/// </summary>
		public static readonly StringName GetUploadedScreenshotSize = "GetUploadedScreenshotSize";

		/// <summary>
		/// Cached name for the 'EmojiButtonSelected' method.
		/// </summary>
		public static readonly StringName EmojiButtonSelected = "EmojiButtonSelected";

		/// <summary>
		/// Cached name for the 'SendButtonFocused' method.
		/// </summary>
		public static readonly StringName SendButtonFocused = "SendButtonFocused";

		/// <summary>
		/// Cached name for the 'SendButtonUnfocused' method.
		/// </summary>
		public static readonly StringName SendButtonUnfocused = "SendButtonUnfocused";

		/// <summary>
		/// Cached name for the 'Open' method.
		/// </summary>
		public static readonly StringName Open = "Open";

		/// <summary>
		/// Cached name for the 'Close' method.
		/// </summary>
		public static readonly StringName Close = "Close";

		/// <summary>
		/// Cached name for the 'ClearInput' method.
		/// </summary>
		public static readonly StringName ClearInput = "ClearInput";

		/// <summary>
		/// Cached name for the 'SetSelectedEmoji' method.
		/// </summary>
		public static readonly StringName SetSelectedEmoji = "SetSelectedEmoji";

		/// <summary>
		/// Cached name for the 'SendButtonSelected' method.
		/// </summary>
		public static readonly StringName SendButtonSelected = "SendButtonSelected";

		/// <summary>
		/// Cached name for the 'ReturnToGameSelected' method.
		/// </summary>
		public static readonly StringName ReturnToGameSelected = "ReturnToGameSelected";

		/// <summary>
		/// Cached name for the 'ReturnToGameFocused' method.
		/// </summary>
		public static readonly StringName ReturnToGameFocused = "ReturnToGameFocused";

		/// <summary>
		/// Cached name for the 'ReturnToGameUnfocused' method.
		/// </summary>
		public static readonly StringName ReturnToGameUnfocused = "ReturnToGameUnfocused";

		/// <summary>
		/// Cached name for the 'WiggleCartoons1' method.
		/// </summary>
		public static readonly StringName WiggleCartoons1 = "WiggleCartoons1";

		/// <summary>
		/// Cached name for the 'WiggleCartoons2' method.
		/// </summary>
		public static readonly StringName WiggleCartoons2 = "WiggleCartoons2";
	}

	/// <summary>
	/// Cached StringNames for the properties and fields contained in this class, for fast lookup.
	/// </summary>
	public new class PropertyName : Control.PropertyName
	{
		/// <summary>
		/// Cached name for the 'DefaultFocusedControl' property.
		/// </summary>
		public static readonly StringName DefaultFocusedControl = "DefaultFocusedControl";

		/// <summary>
		/// Cached name for the '_backButton' field.
		/// </summary>
		public static readonly StringName _backButton = "_backButton";

		/// <summary>
		/// Cached name for the '_mainPanel' field.
		/// </summary>
		public static readonly StringName _mainPanel = "_mainPanel";

		/// <summary>
		/// Cached name for the '_descriptionInput' field.
		/// </summary>
		public static readonly StringName _descriptionInput = "_descriptionInput";

		/// <summary>
		/// Cached name for the '_emojiLabel' field.
		/// </summary>
		public static readonly StringName _emojiLabel = "_emojiLabel";

		/// <summary>
		/// Cached name for the '_sendButton' field.
		/// </summary>
		public static readonly StringName _sendButton = "_sendButton";

		/// <summary>
		/// Cached name for the '_sendLabel' field.
		/// </summary>
		public static readonly StringName _sendLabel = "_sendLabel";

		/// <summary>
		/// Cached name for the '_categoryLabel' field.
		/// </summary>
		public static readonly StringName _categoryLabel = "_categoryLabel";

		/// <summary>
		/// Cached name for the '_returnToGameButton' field.
		/// </summary>
		public static readonly StringName _returnToGameButton = "_returnToGameButton";

		/// <summary>
		/// Cached name for the '_returnToGameLabel' field.
		/// </summary>
		public static readonly StringName _returnToGameLabel = "_returnToGameLabel";

		/// <summary>
		/// Cached name for the '_returnToGameHoverLabel' field.
		/// </summary>
		public static readonly StringName _returnToGameHoverLabel = "_returnToGameHoverLabel";

		/// <summary>
		/// Cached name for the '_categoryDropdown' field.
		/// </summary>
		public static readonly StringName _categoryDropdown = "_categoryDropdown";

		/// <summary>
		/// Cached name for the '_sendBackstop' field.
		/// </summary>
		public static readonly StringName _sendBackstop = "_sendBackstop";

		/// <summary>
		/// Cached name for the '_sendPanel' field.
		/// </summary>
		public static readonly StringName _sendPanel = "_sendPanel";

		/// <summary>
		/// Cached name for the '_successLabel' field.
		/// </summary>
		public static readonly StringName _successLabel = "_successLabel";

		/// <summary>
		/// Cached name for the '_failedLabel' field.
		/// </summary>
		public static readonly StringName _failedLabel = "_failedLabel";

		/// <summary>
		/// Cached name for the '_sendingLabel' field.
		/// </summary>
		public static readonly StringName _sendingLabel = "_sendingLabel";

		/// <summary>
		/// Cached name for the '_flower' field.
		/// </summary>
		public static readonly StringName _flower = "_flower";

		/// <summary>
		/// Cached name for the '_selectedEmoteButton' field.
		/// </summary>
		public static readonly StringName _selectedEmoteButton = "_selectedEmoteButton";

		/// <summary>
		/// Cached name for the '_screenshotBytes' field.
		/// </summary>
		public static readonly StringName _screenshotBytes = "_screenshotBytes";

		/// <summary>
		/// Cached name for the '_originalSuccessPosition' field.
		/// </summary>
		public static readonly StringName _originalSuccessPosition = "_originalSuccessPosition";

		/// <summary>
		/// Cached name for the '_lastClosedMsec' field.
		/// </summary>
		public static readonly StringName _lastClosedMsec = "_lastClosedMsec";

		/// <summary>
		/// Cached name for the '_descriptionText' field.
		/// </summary>
		public static readonly StringName _descriptionText = "_descriptionText";

		/// <summary>
		/// Cached name for the '_descriptionCaretLine' field.
		/// </summary>
		public static readonly StringName _descriptionCaretLine = "_descriptionCaretLine";

		/// <summary>
		/// Cached name for the '_descriptionCaretColumn' field.
		/// </summary>
		public static readonly StringName _descriptionCaretColumn = "_descriptionCaretColumn";

		/// <summary>
		/// Cached name for the '_wiggleTween' field.
		/// </summary>
		public static readonly StringName _wiggleTween = "_wiggleTween";
	}

	/// <summary>
	/// Cached StringNames for the signals contained in this class, for fast lookup.
	/// </summary>
	public new class SignalName : Control.SignalName
	{
	}

	private static readonly string _scenePath = SceneHelper.GetScenePath("screens/feedback_screen/feedback_screen");

	private const float _superWiggleTime = 0.25f;

	private const string _defaultUrl = "https://feedback.sts2.megacrit.com/feedback";

	private static readonly string _url = System.Environment.GetEnvironmentVariable("STS2_FEEDBACK_URL") ?? "https://feedback.sts2.megacrit.com/feedback";

	private const int _maxDescriptionChars = 8000;

	private NBackButton _backButton;

	private Control _mainPanel;

	private NMegaTextEdit _descriptionInput;

	private MegaLabel _emojiLabel;

	private NButton _sendButton;

	private MegaLabel _sendLabel;

	private MegaLabel _categoryLabel;

	private NButton _returnToGameButton;

	private MegaLabel _returnToGameLabel;

	private MegaLabel _returnToGameHoverLabel;

	private NFeedbackCategoryDropdown _categoryDropdown;

	private Control _sendBackstop;

	private Control _sendPanel;

	private MegaLabel _successLabel;

	private MegaLabel _failedLabel;

	private MegaRichTextLabel _sendingLabel;

	private List<NSendFeedbackCartoon> _cartoons = new List<NSendFeedbackCartoon>();

	private NSendFeedbackFlower _flower;

	private CancellationTokenSource? _screenClosedCancelToken;

	private TaskCompletionSource? _runInBackgroundTaskSource;

	private readonly List<NSendFeedbackEmojiButton> _emojiButtons = new List<NSendFeedbackEmojiButton>();

	private NSendFeedbackEmojiButton? _selectedEmoteButton;

	private byte[]? _screenshotBytes;

	private Vector2 _originalSuccessPosition;

	private ulong _lastClosedMsec;

	private string _descriptionText = string.Empty;

	private int _descriptionCaretLine;

	private int _descriptionCaretColumn;

	private Tween? _wiggleTween;

	public Control DefaultFocusedControl => _descriptionInput;

	public static NSendFeedbackScreen? Create()
	{
		if (TestMode.IsOn)
		{
			return null;
		}
		return PreloadManager.Cache.GetScene(_scenePath).Instantiate<NSendFeedbackScreen>(PackedScene.GenEditState.Disabled);
	}

	public override void _Ready()
	{
		_mainPanel = GetNode<Control>("%MainPanel");
		_descriptionInput = GetNode<NMegaTextEdit>("%DescriptionInput");
		_emojiLabel = GetNode<MegaLabel>("%EmojiLabel");
		_sendButton = GetNode<NButton>("%SendButton");
		_returnToGameButton = GetNode<NButton>("%ReturnToGameButton");
		_returnToGameLabel = _returnToGameButton.GetNode<MegaLabel>("Label");
		_returnToGameHoverLabel = _returnToGameButton.GetNode<MegaLabel>("%ReturnToGameHoverLabel");
		_sendLabel = _sendButton.GetNode<MegaLabel>("Label");
		_categoryLabel = GetNode<MegaLabel>("%CategoryLabel");
		_categoryDropdown = GetNode<NFeedbackCategoryDropdown>("%CategoryDropdown");
		_sendBackstop = GetNode<Control>("%SendBackstop");
		_sendPanel = GetNode<Control>("%SendPanel");
		_successLabel = GetNode<MegaLabel>("%SuccessLabel");
		_failedLabel = GetNode<MegaLabel>("%FailedLabel");
		_sendingLabel = GetNode<MegaRichTextLabel>("%SendingLabel");
		_backButton = GetNode<NBackButton>("BackButton");
		_successLabel.SetTextAutoSize(new LocString("settings_ui", "FEEDBACK_SEND_SUCCESS_LABEL").GetFormattedText());
		_failedLabel.SetTextAutoSize(new LocString("settings_ui", "FEEDBACK_SEND_FAILED_LABEL").GetFormattedText());
		_sendingLabel.SetTextAutoSize(new LocString("settings_ui", "FEEDBACK_SENDING_LABEL").GetFormattedText());
		_returnToGameLabel.SetTextAutoSize(new LocString("settings_ui", "FEEDBACK_RUN_IN_BACKGROUND_LABEL").GetFormattedText());
		_returnToGameHoverLabel.SetTextAutoSize(new LocString("settings_ui", "FEEDBACK_RUN_IN_BACKGROUND_HOVER_LABEL").GetFormattedText());
		_originalSuccessPosition = _sendPanel.Position;
		int num = 3;
		List<NSendFeedbackCartoon> list = new List<NSendFeedbackCartoon>(num);
		CollectionsMarshal.SetCount(list, num);
		Span<NSendFeedbackCartoon> span = CollectionsMarshal.AsSpan(list);
		int num2 = 0;
		span[num2] = GetNode<NSendFeedbackCartoon>("Sun");
		num2++;
		span[num2] = GetNode<NSendFeedbackCartoon>("Cupcake");
		num2++;
		span[num2] = GetNode<NSendFeedbackCartoon>("FlowerContainer/Flower");
		_cartoons = list;
		_flower = GetNode<NSendFeedbackFlower>("FlowerContainer");
		foreach (Node child in GetNode("%EmojiButtonContainer").GetChildren())
		{
			if (child is NSendFeedbackEmojiButton nSendFeedbackEmojiButton)
			{
				_emojiButtons.Add(nSendFeedbackEmojiButton);
				nSendFeedbackEmojiButton.PivotOffset = nSendFeedbackEmojiButton.Size * 0.5f;
				nSendFeedbackEmojiButton.Connect(NClickableControl.SignalName.Released, Callable.From<NButton>(EmojiButtonSelected));
			}
		}
		_sendButton.Connect(NClickableControl.SignalName.Released, Callable.From<NButton>(SendButtonSelected));
		_sendButton.Connect(NClickableControl.SignalName.Focused, Callable.From<NClickableControl>(SendButtonFocused));
		_sendButton.Connect(NClickableControl.SignalName.Unfocused, Callable.From<NClickableControl>(SendButtonUnfocused));
		_returnToGameButton.Connect(NClickableControl.SignalName.Released, Callable.From<NButton>(ReturnToGameSelected));
		_returnToGameButton.Connect(NClickableControl.SignalName.Focused, Callable.From<NClickableControl>(ReturnToGameFocused));
		_returnToGameButton.Connect(NClickableControl.SignalName.Unfocused, Callable.From<NClickableControl>(ReturnToGameUnfocused));
		_descriptionInput.TextEdit.Connect(TextEdit.SignalName.TextChanged, Callable.From(OnDescriptionChanged));
		_returnToGameHoverLabel.Visible = false;
		_sendButton.FocusNeighborTop = _categoryDropdown.GetPath();
		_sendButton.FocusNeighborLeft = _emojiButtons.Last().GetPath();
		_sendButton.FocusNeighborBottom = _sendButton.GetPath();
		_sendButton.FocusNeighborRight = _sendButton.GetPath();
		_emojiButtons.Last().FocusNeighborRight = _sendButton.GetPath();
		foreach (NSendFeedbackEmojiButton emojiButton in _emojiButtons)
		{
			emojiButton.FocusNeighborTop = _categoryDropdown.GetPath();
			emojiButton.FocusNeighborBottom = emojiButton.GetPath();
		}
		_categoryDropdown.FocusNeighborRight = _sendButton.GetPath();
		_categoryDropdown.FocusNeighborBottom = _emojiButtons.First().GetPath();
		_categoryDropdown.FocusNeighborTop = _descriptionInput.GetPath();
		_descriptionInput.FocusNeighborTop = _descriptionInput.GetPath();
		_descriptionInput.FocusNeighborLeft = _descriptionInput.GetPath();
		_descriptionInput.FocusNeighborRight = _descriptionInput.GetPath();
		_descriptionInput.FocusNeighborBottom = _categoryDropdown.GetPath();
		_backButton.Connect(NClickableControl.SignalName.Released, Callable.From((NButton _) =>
		{
			Close();
		}));
		Visible = false;
		MouseFilter = MouseFilterEnum.Ignore;
		_backButton.Disable();
	}

	public void Relocalize()
	{
		_descriptionInput.TextEdit.PlaceholderText = new LocString("settings_ui", "FEEDBACK_DESCRIPTION_PLACEHOLDER").GetFormattedText();
		_categoryLabel.SetTextAutoSize(new LocString("settings_ui", "FEEDBACK_CATEGORY_LABEL").GetFormattedText());
		_emojiLabel.SetTextAutoSize(new LocString("settings_ui", "FEEDBACK_EMOJI_LABEL").GetFormattedText());
		_sendLabel.SetTextAutoSize(new LocString("settings_ui", "FEEDBACK_SEND_BUTTON_LABEL").GetFormattedText());
		_descriptionInput.RefreshFont();
		_categoryLabel.RefreshFont();
		_emojiLabel.RefreshFont();
		_sendLabel.RefreshFont();
	}

	/// <summary>
	/// Used to limit the number of characters that can be typed into the description box.
	/// </summary>
	private void OnDescriptionChanged()
	{
		if (_descriptionInput.TextEdit.Text.Length > 8000)
		{
			_descriptionInput.TextEdit.Text = _descriptionText;
			_descriptionInput.TextEdit.SetCaretLine(_descriptionCaretLine);
			_descriptionInput.TextEdit.SetCaretColumn(_descriptionCaretColumn);
		}
		else
		{
			_descriptionText = _descriptionInput.TextEdit.Text;
			_descriptionCaretLine = _descriptionInput.TextEdit.GetCaretLine();
			_descriptionCaretColumn = _descriptionInput.TextEdit.GetCaretColumn();
		}
	}

	/// <summary>
	/// Set the screenshot that will be uploaded with the feedback. The screenshot will be automatically scaled to a
	/// smaller size so that the upload isn't massive.
	/// </summary>
	/// <param name="screenshot"></param>
	public void SetScreenshot(Image screenshot)
	{
		Vector2I uploadedScreenshotSize = GetUploadedScreenshotSize(screenshot.GetWidth(), screenshot.GetHeight());
		if (uploadedScreenshotSize != screenshot.GetSize())
		{
			screenshot.Resize(uploadedScreenshotSize.X, uploadedScreenshotSize.Y, Image.Interpolation.Bilinear);
		}
		_screenshotBytes = screenshot.SavePngToBuffer();
	}

	/// <summary>
	/// Returns the size that a screenshot of the given size is scaled to before it is uploaded.
	/// </summary>
	public static Vector2I GetUploadedScreenshotSize(int width, int height)
	{
		float num = Mathf.Min(1f, Mathf.Min(1280f / (float)width, 720f / (float)height));
		return new Vector2I(Mathf.Max(1, Mathf.RoundToInt((float)width * num)), Mathf.Max(1, Mathf.RoundToInt((float)height * num)));
	}

	private void EmojiButtonSelected(NButton button)
	{
		SetSelectedEmoji((NSendFeedbackEmojiButton)button);
	}

	private void SendButtonFocused(NClickableControl _)
	{
		_flower.SetState(NSendFeedbackFlower.State.Anticipation);
	}

	private void SendButtonUnfocused(NClickableControl _)
	{
		if (_flower.MyState == NSendFeedbackFlower.State.Anticipation && !_sendBackstop.Visible)
		{
			_flower.SetState(NSendFeedbackFlower.State.None);
		}
	}

	public void Open()
	{
		if (!Visible)
		{
			Log.Info("Feedback screen opened");
			if (Time.GetTicksMsec() - _lastClosedMsec > 60000)
			{
				ClearInput();
			}
			_screenClosedCancelToken = new CancellationTokenSource();
			Visible = true;
			_flower.SetState(NSendFeedbackFlower.State.None);
			_sendBackstop.Visible = false;
			_sendButton.Enable();
			MouseFilter = MouseFilterEnum.Stop;
			NHotkeyManager.Instance.AddBlockingScreen(this);
			ActiveScreenContext.Instance.Update();
			_backButton.Enable();
		}
	}

	private void Close()
	{
		Log.Info("Feedback screen closed");
		_flower.SetState(NSendFeedbackFlower.State.None);
		_sendBackstop.Visible = false;
		_mainPanel.Modulate = Colors.White;
		_wiggleTween?.Kill();
		Visible = false;
		_lastClosedMsec = Time.GetTicksMsec();
		_screenClosedCancelToken?.Cancel();
		MouseFilter = MouseFilterEnum.Ignore;
		_backButton.Disable();
		NHotkeyManager.Instance.RemoveBlockingScreen(this);
		ActiveScreenContext.Instance.Update();
	}

	private void ClearInput()
	{
		_descriptionInput.TextEdit.Text = string.Empty;
		_descriptionText = string.Empty;
		SetSelectedEmoji(null);
	}

	private void SetSelectedEmoji(NSendFeedbackEmojiButton? button)
	{
		NSendFeedbackEmojiButton selectedEmoteButton = _selectedEmoteButton;
		_selectedEmoteButton?.SetSelected(isSelected: false);
		if (selectedEmoteButton != button)
		{
			_selectedEmoteButton = button;
			_selectedEmoteButton?.SetSelected(isSelected: true);
		}
	}

	private void SendButtonSelected(NButton _)
	{
		TaskHelper.RunSafely(SendFeedbackWrapper());
		_sendButton.Disable();
	}

	private void ReturnToGameSelected(NButton _)
	{
		_runInBackgroundTaskSource?.TrySetResult();
	}

	private void ReturnToGameFocused(NClickableControl _)
	{
		_returnToGameHoverLabel.Visible = true;
	}

	private void ReturnToGameUnfocused(NClickableControl _)
	{
		_returnToGameHoverLabel.Visible = false;
	}

	/// <summary>
	/// Send feedback, allowing the player to exit the menu if they want to get back to playing.
	/// </summary>
	private async Task SendFeedbackWrapper()
	{
		if (string.IsNullOrEmpty(_descriptionText))
		{
			return;
		}
		Log.Info("Beginning asynchronous feedback send at " + Log.Timestamp + ": " + _descriptionText);
		ReleaseInfo releaseInfo = ReleaseInfoManager.Instance.ReleaseInfo;
		string text = releaseInfo?.Commit ?? GitHelper.ShortCommitId;
		FeedbackData data = new FeedbackData
		{
			description = _descriptionText,
			category = _categoryDropdown.CurrentCategory,
			gameVersion = (releaseInfo?.Version ?? "v0.0.1"),
			uniqueId = SaveManager.Instance.Progress.UniqueId,
			commit = (text ?? "unknown"),
			platformBranch = PlatformUtil.GetPlatformBranch().ToName(),
			sessionId = SentryService.SessionId,
			isModded = (ModManager.IsRunningModded() || ModManager.HasHarmonyPatches()),
			isFullConsole = SaveManager.Instance.SettingsSave.FullConsole,
			lang = LocManager.Instance.Language
		};
		FeedbackLogFiles logFiles = GetLogsConsoleCmd.CollectFeedbackLogFiles(SaveManager.Instance.CurrentProfileId);
		_runInBackgroundTaskSource = new TaskCompletionSource();
		Task<bool> sendTask = SendFeedback(data, _screenshotBytes, logFiles, PutWithNewClient);
		_sendBackstop.Visible = true;
		_sendingLabel.Visible = true;
		_failedLabel.Visible = false;
		_successLabel.Visible = false;
		_sendPanel.Modulate = Colors.Transparent;
		Control sendPanel = _sendPanel;
		Vector2 position = _sendPanel.Position;
		position.Y = _originalSuccessPosition.Y + 20f;
		sendPanel.Position = position;
		Tween tween = GetTree().CreateTween().Parallel();
		tween.TweenProperty(_mainPanel, "modulate", new Color(0.1f, 0.1f, 0.1f), 0.15000000596046448);
		tween.TweenProperty(_sendPanel, "modulate", Colors.White, 0.15000000596046448);
		tween.TweenProperty(_sendPanel, "position:y", _originalSuccessPosition.Y, 0.15000000596046448).SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Quad);
		tween.Chain().TweenProperty(_successLabel, "position:y", _successLabel.Position.Y - 10f, 0.10000000149011612).SetEase(Tween.EaseType.Out)
			.SetTrans(Tween.TransitionType.Quad);
		await TaskHelper.WhenAny(sendTask, _runInBackgroundTaskSource.Task);
		if (this.IsValid())
		{
			if (_runInBackgroundTaskSource.Task.IsCompleted)
			{
				Close();
			}
			else if (await sendTask)
			{
				await OnFeedbackSuccess();
			}
			else
			{
				await OnFeedbackFailed();
			}
		}
	}

	/// <summary>
	/// Send feedback and log after close.
	/// This is purposefully static so that we ensure that nothing from the screen is used, as it can be deallocated
	/// before the method completes.
	/// After a 413 or a timeout, the next zip is written without the largest attachment that the failed attempt
	/// included. Any other failure retries the same zip. The attachment budget keeps a zip with attachments under the
	/// server's current limit, so a 413 with attachments only happens if that limit drops or a proxy in front of the
	/// server has a lower one. Over HTTP/1.1 a body that is too large fails as a network error, because the server
	/// closes the connection without reading it, so after an attempt without attachments fails in any way, the next
	/// zip is also written without the older logs.
	/// </summary>
	/// <param name="data">The report payload.</param>
	/// <param name="screenshotBytes">The PNG screenshot.</param>
	/// <param name="logFiles">Disposed when the send is done.</param>
	/// <param name="put">Sends one attempt.</param>
	/// <param name="retryDelaysMs">The wait before each retry. The last value is used for any further retries.</param>
	public static async Task<bool> SendFeedback(FeedbackData data, byte[] screenshotBytes, FeedbackLogFiles logFiles, Func<HttpContent, Task<HttpResponseMessage>> put, IReadOnlyList<int>? retryDelaysMs = null)
	{
		using (logFiles)
		{
			if (retryDelaysMs == null)
			{
				retryDelaysMs = new _003C_003Ez__ReadOnlyArray<int>(new int[3] { 500, 1000, 2000 });
			}
			int maxAttempts = Math.Max(3, logFiles.Attachments.Count + 1 + ((logFiles.OlderLogs.Count > 0) ? 1 : 0));
			int maxAttachments = logFiles.Attachments.Count;
			bool includeOlderLogs = true;
			string sentryMessage = null;
			for (int attempt = 0; attempt < maxAttempts; attempt++)
			{
				MemoryStream logsStream = new MemoryStream();
				try
				{
					int attachmentLimit = maxAttachments;
					bool withOlderLogs = includeOlderLogs;
					List<int> written = await Task.Run(() => GetLogsConsoleCmd.WriteFeedbackZip(logsStream, logFiles, attachmentLimit, withOlderLogs, 41943040L));
					logsStream.Seek(0L, SeekOrigin.Begin);
					using MemoryStream screenshotStream = new MemoryStream(screenshotBytes);
					using MultipartFormDataContent formContent = BuildMultipartContent(data, screenshotStream, logsStream);
					Log.Info($"Feedback attempt {attempt + 1}/{maxAttempts} is {formContent.Headers.ContentLength} bytes (logs {logsStream.Length}, screenshot {screenshotStream.Length}), with attachments: {string.Join(", ", written.Select((int i) => logFiles.Attachments[i].EntryName))}{(withOlderLogs ? "" : ", without older logs")}");
					bool flag;
					try
					{
						using HttpResponseMessage response = await put(formContent);
						if (response.IsSuccessStatusCode)
						{
							Log.Info("Feedback successfully posted!");
							return true;
						}
						int statusCode = (int)response.StatusCode;
						if (statusCode >= 400 && statusCode < 500 && statusCode != 429 && statusCode != 413)
						{
							string value = await response.Content.ReadAsStringAsync();
							Log.Warn($"Feedback rejected ({response.StatusCode}): {value}");
							SentrySdk.CaptureMessage($"Feedback rejected: Response status code {response.StatusCode}");
							return false;
						}
						flag = statusCode == 413;
						sentryMessage = $"Response status code {response.StatusCode}";
						Log.Warn($"Feedback attempt {attempt + 1}/{maxAttempts} failed: {response.StatusCode} over HTTP/{response.Version}");
					}
					catch (Exception ex) when ((ex is HttpRequestException || ex is TaskCanceledException) ? true : false)
					{
						flag = ex is TaskCanceledException;
						string text = $"Feedback attempt {attempt + 1}/{maxAttempts} network error: {ex}\n Inner messages: {ExceptionMessageWithInner(ex)}";
						if (ex is HttpRequestException { HttpRequestError: not HttpRequestError.NameResolutionError })
						{
							sentryMessage = ex.GetType().Name + ": " + ExceptionMessageWithInner(ex);
						}
						Log.Warn(text);
					}
					if (written.Count == 0)
					{
						includeOlderLogs = false;
					}
					else if (flag)
					{
						maxAttachments = written[written.Count - 1];
					}
					if (attempt < maxAttempts - 1)
					{
						await Task.Delay(retryDelaysMs[Math.Min(attempt, retryDelaysMs.Count - 1)]);
					}
				}
				finally
				{
					if (logsStream != null)
					{
						((IDisposable)logsStream).Dispose();
					}
				}
			}
			Log.Warn("Feedback send failed after all retry attempts");
			if (sentryMessage != null)
			{
				SentrySdk.CaptureMessage("Feedback failed to send: " + sentryMessage);
			}
			return false;
		}
	}

	/// <summary>
	/// Sends one upload attempt on a new HTTP client.
	/// Over HTTP/1.1 the server's 413 for a body that is too large reaches the game as a broken connection while it is
	/// still sending. Over HTTP/2 the game reads the 413. Some proxies only allow HTTP/1.1, so fall back to it. Over
	/// plain http://, such as a local STS2_FEEDBACK_URL, this client uses HTTP/1.1.
	/// Each attempt gets its own client because a request that times out over HTTP/2 only cancels its stream: a
	/// connection that stopped passing data would stay in a shared client's pool, and every retry would wait on it
	/// until the timeout.
	/// </summary>
	private static async Task<HttpResponseMessage> PutWithNewClient(HttpContent content)
	{
		using System.Net.Http.HttpClient client = new System.Net.Http.HttpClient
		{
			Timeout = TimeSpan.FromSeconds(10L),
			DefaultRequestVersion = HttpVersion.Version20,
			DefaultVersionPolicy = HttpVersionPolicy.RequestVersionOrLower
		};
		return await client.PutAsync(_url, content);
	}

	private static string ExceptionMessageWithInner(Exception ex)
	{
		if (ex.InnerException == null)
		{
			return ex.Message;
		}
		return ex.Message + " | " + ExceptionMessageWithInner(ex.InnerException);
	}

	private static MultipartFormDataContent BuildMultipartContent(FeedbackData data, Stream screenshotStream, Stream logsStream)
	{
		string content = JsonSerializer.Serialize(data, JsonSerializationUtility.GetTypeInfo<FeedbackData>());
		MultipartFormDataContent multipartFormDataContent = new MultipartFormDataContent();
		StringContent stringContent = new StringContent(content);
		stringContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
		stringContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
		{
			Name = "payload_json"
		};
		multipartFormDataContent.Add(stringContent);
		StreamContent streamContent = new StreamContent(logsStream);
		streamContent.Headers.ContentType = new MediaTypeHeaderValue("application/zip");
		streamContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
		{
			Name = "logs"
		};
		multipartFormDataContent.Add(streamContent);
		StreamContent streamContent2 = new StreamContent(screenshotStream);
		streamContent2.Headers.ContentType = new MediaTypeHeaderValue("image/png");
		streamContent2.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
		{
			Name = "screenshot"
		};
		multipartFormDataContent.Add(streamContent2);
		return multipartFormDataContent;
	}

	private async Task OnFeedbackFailed()
	{
		_sendingLabel.Visible = false;
		_failedLabel.Visible = true;
		_successLabel.Visible = false;
		Tween tween = GetTree().CreateTween().Chain();
		tween.TweenProperty(_failedLabel, "position:x", _successLabel.Position.X - 10f, 0.02500000037252903).SetEase(Tween.EaseType.In).SetTrans(Tween.TransitionType.Quad);
		tween.TweenProperty(_failedLabel, "position:x", _successLabel.Position.X + 10f, 0.05000000074505806).SetEase(Tween.EaseType.InOut).SetTrans(Tween.TransitionType.Quad);
		tween.TweenProperty(_failedLabel, "position:x", _successLabel.Position.X - 10f, 0.05000000074505806).SetEase(Tween.EaseType.InOut).SetTrans(Tween.TransitionType.Quad);
		tween.TweenProperty(_failedLabel, "position:x", _successLabel.Position.X + 10f, 0.05000000074505806).SetEase(Tween.EaseType.InOut).SetTrans(Tween.TransitionType.Quad);
		tween.TweenProperty(_failedLabel, "position:x", _successLabel.Position.X, 0.02500000037252903).SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Quad);
		_wiggleTween?.Kill();
		_flower.SetState(NSendFeedbackFlower.State.None);
		await Task.Delay(2000, _screenClosedCancelToken?.Token ?? default(CancellationToken));
		_sendBackstop.Visible = false;
		_mainPanel.Modulate = Colors.White;
	}

	private async Task OnFeedbackSuccess()
	{
		ClearInput();
		_screenshotBytes = null;
		_sendingLabel.Visible = false;
		_failedLabel.Visible = false;
		_successLabel.Visible = true;
		MegaLabel successLabel = _successLabel;
		Color modulate = _successLabel.Modulate;
		modulate.A = 0f;
		successLabel.Modulate = modulate;
		Tween tween = GetTree().CreateTween().Parallel();
		tween.TweenProperty(_successLabel, "modulate:a", 1f, 0.15000000596046448);
		tween.TweenProperty(_successLabel, "position:y", _successLabel.Position.Y - 10f, 0.10000000149011612).SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Quad);
		tween.Chain().TweenProperty(_successLabel, "position:y", _successLabel.Position.Y, 0.10000000149011612).SetEase(Tween.EaseType.In)
			.SetTrans(Tween.TransitionType.Quad);
		_wiggleTween?.Kill();
		_wiggleTween = CreateTween();
		_wiggleTween.TweenCallback(Callable.From(WiggleCartoons1));
		_wiggleTween.TweenInterval(0.25);
		_wiggleTween.TweenCallback(Callable.From(WiggleCartoons2));
		_wiggleTween.TweenInterval(0.25);
		_wiggleTween.SetLoops();
		string scenePath = SceneHelper.GetScenePath("vfx/vfx_dramatic_entrance_fullscreen");
		Node2D node2D = PreloadManager.Cache.GetScene(scenePath).Instantiate<Node2D>(PackedScene.GenEditState.Disabled);
		this.AddChildSafely(node2D);
		this.MoveChildSafely(node2D, 1);
		node2D.GlobalPosition = NGame.Instance.GetViewportRect().Size * 0.5f;
		_flower.SetState(NSendFeedbackFlower.State.NoddingFast);
		await Task.Delay(2000, _screenClosedCancelToken?.Token ?? default(CancellationToken));
		Close();
	}

	private void WiggleCartoons1()
	{
		foreach (NSendFeedbackCartoon cartoon in _cartoons)
		{
			if (_flower.MyState == NSendFeedbackFlower.State.None || cartoon != _flower.Cartoon)
			{
				cartoon.SetRotation1();
			}
		}
	}

	private void WiggleCartoons2()
	{
		foreach (NSendFeedbackCartoon cartoon in _cartoons)
		{
			if (_flower.MyState == NSendFeedbackFlower.State.None || cartoon != _flower.Cartoon)
			{
				cartoon.SetRotation2();
			}
		}
	}

	/// <summary>
	/// Get the method information for all the methods declared in this class.
	/// This method is used by Godot to register the available methods in the editor.
	/// Do not call this method.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		List<MethodInfo> list = new List<MethodInfo>(19);
		list.Add(new MethodInfo(MethodName.Create, new PropertyInfo(Variant.Type.Object, "", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Control"), exported: false), MethodFlags.Normal | MethodFlags.Static, null, null));
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.Relocalize, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.OnDescriptionChanged, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.SetScreenshot, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.Object, "screenshot", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Image"), exported: false)
		}, null));
		list.Add(new MethodInfo(MethodName.GetUploadedScreenshotSize, new PropertyInfo(Variant.Type.Vector2I, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal | MethodFlags.Static, new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.Int, "width", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false),
			new PropertyInfo(Variant.Type.Int, "height", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
		}, null));
		list.Add(new MethodInfo(MethodName.EmojiButtonSelected, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.Object, "button", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Control"), exported: false)
		}, null));
		list.Add(new MethodInfo(MethodName.SendButtonFocused, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.Object, "_", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Control"), exported: false)
		}, null));
		list.Add(new MethodInfo(MethodName.SendButtonUnfocused, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.Object, "_", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Control"), exported: false)
		}, null));
		list.Add(new MethodInfo(MethodName.Open, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.Close, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.ClearInput, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.SetSelectedEmoji, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.Object, "button", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Control"), exported: false)
		}, null));
		list.Add(new MethodInfo(MethodName.SendButtonSelected, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.Object, "_", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Control"), exported: false)
		}, null));
		list.Add(new MethodInfo(MethodName.ReturnToGameSelected, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.Object, "_", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Control"), exported: false)
		}, null));
		list.Add(new MethodInfo(MethodName.ReturnToGameFocused, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.Object, "_", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Control"), exported: false)
		}, null));
		list.Add(new MethodInfo(MethodName.ReturnToGameUnfocused, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.Object, "_", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Control"), exported: false)
		}, null));
		list.Add(new MethodInfo(MethodName.WiggleCartoons1, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.WiggleCartoons2, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		return list;
	}

	/// <inheritdoc />
	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		if (method == MethodName.Create && args.Count == 0)
		{
			ret = VariantUtils.CreateFrom<NSendFeedbackScreen>(Create());
			return true;
		}
		if (method == MethodName._Ready && args.Count == 0)
		{
			_Ready();
			ret = default;
			return true;
		}
		if (method == MethodName.Relocalize && args.Count == 0)
		{
			Relocalize();
			ret = default;
			return true;
		}
		if (method == MethodName.OnDescriptionChanged && args.Count == 0)
		{
			OnDescriptionChanged();
			ret = default;
			return true;
		}
		if (method == MethodName.SetScreenshot && args.Count == 1)
		{
			SetScreenshot(VariantUtils.ConvertTo<Image>(in args[0]));
			ret = default;
			return true;
		}
		if (method == MethodName.GetUploadedScreenshotSize && args.Count == 2)
		{
			ret = VariantUtils.CreateFrom<Vector2I>(GetUploadedScreenshotSize(VariantUtils.ConvertTo<int>(in args[0]), VariantUtils.ConvertTo<int>(in args[1])));
			return true;
		}
		if (method == MethodName.EmojiButtonSelected && args.Count == 1)
		{
			EmojiButtonSelected(VariantUtils.ConvertTo<NButton>(in args[0]));
			ret = default;
			return true;
		}
		if (method == MethodName.SendButtonFocused && args.Count == 1)
		{
			SendButtonFocused(VariantUtils.ConvertTo<NClickableControl>(in args[0]));
			ret = default;
			return true;
		}
		if (method == MethodName.SendButtonUnfocused && args.Count == 1)
		{
			SendButtonUnfocused(VariantUtils.ConvertTo<NClickableControl>(in args[0]));
			ret = default;
			return true;
		}
		if (method == MethodName.Open && args.Count == 0)
		{
			Open();
			ret = default;
			return true;
		}
		if (method == MethodName.Close && args.Count == 0)
		{
			Close();
			ret = default;
			return true;
		}
		if (method == MethodName.ClearInput && args.Count == 0)
		{
			ClearInput();
			ret = default;
			return true;
		}
		if (method == MethodName.SetSelectedEmoji && args.Count == 1)
		{
			SetSelectedEmoji(VariantUtils.ConvertTo<NSendFeedbackEmojiButton>(in args[0]));
			ret = default;
			return true;
		}
		if (method == MethodName.SendButtonSelected && args.Count == 1)
		{
			SendButtonSelected(VariantUtils.ConvertTo<NButton>(in args[0]));
			ret = default;
			return true;
		}
		if (method == MethodName.ReturnToGameSelected && args.Count == 1)
		{
			ReturnToGameSelected(VariantUtils.ConvertTo<NButton>(in args[0]));
			ret = default;
			return true;
		}
		if (method == MethodName.ReturnToGameFocused && args.Count == 1)
		{
			ReturnToGameFocused(VariantUtils.ConvertTo<NClickableControl>(in args[0]));
			ret = default;
			return true;
		}
		if (method == MethodName.ReturnToGameUnfocused && args.Count == 1)
		{
			ReturnToGameUnfocused(VariantUtils.ConvertTo<NClickableControl>(in args[0]));
			ret = default;
			return true;
		}
		if (method == MethodName.WiggleCartoons1 && args.Count == 0)
		{
			WiggleCartoons1();
			ret = default;
			return true;
		}
		if (method == MethodName.WiggleCartoons2 && args.Count == 0)
		{
			WiggleCartoons2();
			ret = default;
			return true;
		}
		return base.InvokeGodotClassMethod(in method, args, out ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static bool InvokeGodotClassStaticMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		if (method == MethodName.Create && args.Count == 0)
		{
			ret = VariantUtils.CreateFrom<NSendFeedbackScreen>(Create());
			return true;
		}
		if (method == MethodName.GetUploadedScreenshotSize && args.Count == 2)
		{
			ret = VariantUtils.CreateFrom<Vector2I>(GetUploadedScreenshotSize(VariantUtils.ConvertTo<int>(in args[0]), VariantUtils.ConvertTo<int>(in args[1])));
			return true;
		}
		ret = default;
		return false;
	}

	/// <inheritdoc />
	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassMethod(in godot_string_name method)
	{
		if (method == MethodName.Create)
		{
			return true;
		}
		if (method == MethodName._Ready)
		{
			return true;
		}
		if (method == MethodName.Relocalize)
		{
			return true;
		}
		if (method == MethodName.OnDescriptionChanged)
		{
			return true;
		}
		if (method == MethodName.SetScreenshot)
		{
			return true;
		}
		if (method == MethodName.GetUploadedScreenshotSize)
		{
			return true;
		}
		if (method == MethodName.EmojiButtonSelected)
		{
			return true;
		}
		if (method == MethodName.SendButtonFocused)
		{
			return true;
		}
		if (method == MethodName.SendButtonUnfocused)
		{
			return true;
		}
		if (method == MethodName.Open)
		{
			return true;
		}
		if (method == MethodName.Close)
		{
			return true;
		}
		if (method == MethodName.ClearInput)
		{
			return true;
		}
		if (method == MethodName.SetSelectedEmoji)
		{
			return true;
		}
		if (method == MethodName.SendButtonSelected)
		{
			return true;
		}
		if (method == MethodName.ReturnToGameSelected)
		{
			return true;
		}
		if (method == MethodName.ReturnToGameFocused)
		{
			return true;
		}
		if (method == MethodName.ReturnToGameUnfocused)
		{
			return true;
		}
		if (method == MethodName.WiggleCartoons1)
		{
			return true;
		}
		if (method == MethodName.WiggleCartoons2)
		{
			return true;
		}
		return base.HasGodotClassMethod(in method);
	}

	/// <inheritdoc />
	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if (name == PropertyName._backButton)
		{
			_backButton = VariantUtils.ConvertTo<NBackButton>(in value);
			return true;
		}
		if (name == PropertyName._mainPanel)
		{
			_mainPanel = VariantUtils.ConvertTo<Control>(in value);
			return true;
		}
		if (name == PropertyName._descriptionInput)
		{
			_descriptionInput = VariantUtils.ConvertTo<NMegaTextEdit>(in value);
			return true;
		}
		if (name == PropertyName._emojiLabel)
		{
			_emojiLabel = VariantUtils.ConvertTo<MegaLabel>(in value);
			return true;
		}
		if (name == PropertyName._sendButton)
		{
			_sendButton = VariantUtils.ConvertTo<NButton>(in value);
			return true;
		}
		if (name == PropertyName._sendLabel)
		{
			_sendLabel = VariantUtils.ConvertTo<MegaLabel>(in value);
			return true;
		}
		if (name == PropertyName._categoryLabel)
		{
			_categoryLabel = VariantUtils.ConvertTo<MegaLabel>(in value);
			return true;
		}
		if (name == PropertyName._returnToGameButton)
		{
			_returnToGameButton = VariantUtils.ConvertTo<NButton>(in value);
			return true;
		}
		if (name == PropertyName._returnToGameLabel)
		{
			_returnToGameLabel = VariantUtils.ConvertTo<MegaLabel>(in value);
			return true;
		}
		if (name == PropertyName._returnToGameHoverLabel)
		{
			_returnToGameHoverLabel = VariantUtils.ConvertTo<MegaLabel>(in value);
			return true;
		}
		if (name == PropertyName._categoryDropdown)
		{
			_categoryDropdown = VariantUtils.ConvertTo<NFeedbackCategoryDropdown>(in value);
			return true;
		}
		if (name == PropertyName._sendBackstop)
		{
			_sendBackstop = VariantUtils.ConvertTo<Control>(in value);
			return true;
		}
		if (name == PropertyName._sendPanel)
		{
			_sendPanel = VariantUtils.ConvertTo<Control>(in value);
			return true;
		}
		if (name == PropertyName._successLabel)
		{
			_successLabel = VariantUtils.ConvertTo<MegaLabel>(in value);
			return true;
		}
		if (name == PropertyName._failedLabel)
		{
			_failedLabel = VariantUtils.ConvertTo<MegaLabel>(in value);
			return true;
		}
		if (name == PropertyName._sendingLabel)
		{
			_sendingLabel = VariantUtils.ConvertTo<MegaRichTextLabel>(in value);
			return true;
		}
		if (name == PropertyName._flower)
		{
			_flower = VariantUtils.ConvertTo<NSendFeedbackFlower>(in value);
			return true;
		}
		if (name == PropertyName._selectedEmoteButton)
		{
			_selectedEmoteButton = VariantUtils.ConvertTo<NSendFeedbackEmojiButton>(in value);
			return true;
		}
		if (name == PropertyName._screenshotBytes)
		{
			_screenshotBytes = VariantUtils.ConvertTo<byte[]>(in value);
			return true;
		}
		if (name == PropertyName._originalSuccessPosition)
		{
			_originalSuccessPosition = VariantUtils.ConvertTo<Vector2>(in value);
			return true;
		}
		if (name == PropertyName._lastClosedMsec)
		{
			_lastClosedMsec = VariantUtils.ConvertTo<ulong>(in value);
			return true;
		}
		if (name == PropertyName._descriptionText)
		{
			_descriptionText = VariantUtils.ConvertTo<string>(in value);
			return true;
		}
		if (name == PropertyName._descriptionCaretLine)
		{
			_descriptionCaretLine = VariantUtils.ConvertTo<int>(in value);
			return true;
		}
		if (name == PropertyName._descriptionCaretColumn)
		{
			_descriptionCaretColumn = VariantUtils.ConvertTo<int>(in value);
			return true;
		}
		if (name == PropertyName._wiggleTween)
		{
			_wiggleTween = VariantUtils.ConvertTo<Tween>(in value);
			return true;
		}
		return base.SetGodotClassPropertyValue(in name, in value);
	}

	/// <inheritdoc />
	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool GetGodotClassPropertyValue(in godot_string_name name, out godot_variant value)
	{
		if (name == PropertyName.DefaultFocusedControl)
		{
			value = VariantUtils.CreateFrom<Control>(DefaultFocusedControl);
			return true;
		}
		if (name == PropertyName._backButton)
		{
			value = VariantUtils.CreateFrom(in _backButton);
			return true;
		}
		if (name == PropertyName._mainPanel)
		{
			value = VariantUtils.CreateFrom(in _mainPanel);
			return true;
		}
		if (name == PropertyName._descriptionInput)
		{
			value = VariantUtils.CreateFrom(in _descriptionInput);
			return true;
		}
		if (name == PropertyName._emojiLabel)
		{
			value = VariantUtils.CreateFrom(in _emojiLabel);
			return true;
		}
		if (name == PropertyName._sendButton)
		{
			value = VariantUtils.CreateFrom(in _sendButton);
			return true;
		}
		if (name == PropertyName._sendLabel)
		{
			value = VariantUtils.CreateFrom(in _sendLabel);
			return true;
		}
		if (name == PropertyName._categoryLabel)
		{
			value = VariantUtils.CreateFrom(in _categoryLabel);
			return true;
		}
		if (name == PropertyName._returnToGameButton)
		{
			value = VariantUtils.CreateFrom(in _returnToGameButton);
			return true;
		}
		if (name == PropertyName._returnToGameLabel)
		{
			value = VariantUtils.CreateFrom(in _returnToGameLabel);
			return true;
		}
		if (name == PropertyName._returnToGameHoverLabel)
		{
			value = VariantUtils.CreateFrom(in _returnToGameHoverLabel);
			return true;
		}
		if (name == PropertyName._categoryDropdown)
		{
			value = VariantUtils.CreateFrom(in _categoryDropdown);
			return true;
		}
		if (name == PropertyName._sendBackstop)
		{
			value = VariantUtils.CreateFrom(in _sendBackstop);
			return true;
		}
		if (name == PropertyName._sendPanel)
		{
			value = VariantUtils.CreateFrom(in _sendPanel);
			return true;
		}
		if (name == PropertyName._successLabel)
		{
			value = VariantUtils.CreateFrom(in _successLabel);
			return true;
		}
		if (name == PropertyName._failedLabel)
		{
			value = VariantUtils.CreateFrom(in _failedLabel);
			return true;
		}
		if (name == PropertyName._sendingLabel)
		{
			value = VariantUtils.CreateFrom(in _sendingLabel);
			return true;
		}
		if (name == PropertyName._flower)
		{
			value = VariantUtils.CreateFrom(in _flower);
			return true;
		}
		if (name == PropertyName._selectedEmoteButton)
		{
			value = VariantUtils.CreateFrom(in _selectedEmoteButton);
			return true;
		}
		if (name == PropertyName._screenshotBytes)
		{
			value = VariantUtils.CreateFrom(in _screenshotBytes);
			return true;
		}
		if (name == PropertyName._originalSuccessPosition)
		{
			value = VariantUtils.CreateFrom(in _originalSuccessPosition);
			return true;
		}
		if (name == PropertyName._lastClosedMsec)
		{
			value = VariantUtils.CreateFrom(in _lastClosedMsec);
			return true;
		}
		if (name == PropertyName._descriptionText)
		{
			value = VariantUtils.CreateFrom(in _descriptionText);
			return true;
		}
		if (name == PropertyName._descriptionCaretLine)
		{
			value = VariantUtils.CreateFrom(in _descriptionCaretLine);
			return true;
		}
		if (name == PropertyName._descriptionCaretColumn)
		{
			value = VariantUtils.CreateFrom(in _descriptionCaretColumn);
			return true;
		}
		if (name == PropertyName._wiggleTween)
		{
			value = VariantUtils.CreateFrom(in _wiggleTween);
			return true;
		}
		return base.GetGodotClassPropertyValue(in name, out value);
	}

	/// <summary>
	/// Get the property information for all the properties declared in this class.
	/// This method is used by Godot to register the available properties in the editor.
	/// Do not call this method.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<PropertyInfo> GetGodotPropertyList()
	{
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._backButton, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._mainPanel, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._descriptionInput, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._emojiLabel, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._sendButton, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._sendLabel, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._categoryLabel, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._returnToGameButton, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._returnToGameLabel, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._returnToGameHoverLabel, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._categoryDropdown, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._sendBackstop, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._sendPanel, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._successLabel, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._failedLabel, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._sendingLabel, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._flower, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._selectedEmoteButton, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.PackedByteArray, PropertyName._screenshotBytes, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Vector2, PropertyName._originalSuccessPosition, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Int, PropertyName._lastClosedMsec, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.String, PropertyName._descriptionText, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Int, PropertyName._descriptionCaretLine, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Int, PropertyName._descriptionCaretColumn, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._wiggleTween, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName.DefaultFocusedControl, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		return list;
	}

	/// <inheritdoc />
	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		base.SaveGodotObjectData(info);
		info.AddProperty(PropertyName._backButton, Variant.From(in _backButton));
		info.AddProperty(PropertyName._mainPanel, Variant.From(in _mainPanel));
		info.AddProperty(PropertyName._descriptionInput, Variant.From(in _descriptionInput));
		info.AddProperty(PropertyName._emojiLabel, Variant.From(in _emojiLabel));
		info.AddProperty(PropertyName._sendButton, Variant.From(in _sendButton));
		info.AddProperty(PropertyName._sendLabel, Variant.From(in _sendLabel));
		info.AddProperty(PropertyName._categoryLabel, Variant.From(in _categoryLabel));
		info.AddProperty(PropertyName._returnToGameButton, Variant.From(in _returnToGameButton));
		info.AddProperty(PropertyName._returnToGameLabel, Variant.From(in _returnToGameLabel));
		info.AddProperty(PropertyName._returnToGameHoverLabel, Variant.From(in _returnToGameHoverLabel));
		info.AddProperty(PropertyName._categoryDropdown, Variant.From(in _categoryDropdown));
		info.AddProperty(PropertyName._sendBackstop, Variant.From(in _sendBackstop));
		info.AddProperty(PropertyName._sendPanel, Variant.From(in _sendPanel));
		info.AddProperty(PropertyName._successLabel, Variant.From(in _successLabel));
		info.AddProperty(PropertyName._failedLabel, Variant.From(in _failedLabel));
		info.AddProperty(PropertyName._sendingLabel, Variant.From(in _sendingLabel));
		info.AddProperty(PropertyName._flower, Variant.From(in _flower));
		info.AddProperty(PropertyName._selectedEmoteButton, Variant.From(in _selectedEmoteButton));
		info.AddProperty(PropertyName._screenshotBytes, Variant.From(in _screenshotBytes));
		info.AddProperty(PropertyName._originalSuccessPosition, Variant.From(in _originalSuccessPosition));
		info.AddProperty(PropertyName._lastClosedMsec, Variant.From(in _lastClosedMsec));
		info.AddProperty(PropertyName._descriptionText, Variant.From(in _descriptionText));
		info.AddProperty(PropertyName._descriptionCaretLine, Variant.From(in _descriptionCaretLine));
		info.AddProperty(PropertyName._descriptionCaretColumn, Variant.From(in _descriptionCaretColumn));
		info.AddProperty(PropertyName._wiggleTween, Variant.From(in _wiggleTween));
	}

	/// <inheritdoc />
	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		if (info.TryGetProperty(PropertyName._backButton, out var value))
		{
			_backButton = value.As<NBackButton>();
		}
		if (info.TryGetProperty(PropertyName._mainPanel, out var value2))
		{
			_mainPanel = value2.As<Control>();
		}
		if (info.TryGetProperty(PropertyName._descriptionInput, out var value3))
		{
			_descriptionInput = value3.As<NMegaTextEdit>();
		}
		if (info.TryGetProperty(PropertyName._emojiLabel, out var value4))
		{
			_emojiLabel = value4.As<MegaLabel>();
		}
		if (info.TryGetProperty(PropertyName._sendButton, out var value5))
		{
			_sendButton = value5.As<NButton>();
		}
		if (info.TryGetProperty(PropertyName._sendLabel, out var value6))
		{
			_sendLabel = value6.As<MegaLabel>();
		}
		if (info.TryGetProperty(PropertyName._categoryLabel, out var value7))
		{
			_categoryLabel = value7.As<MegaLabel>();
		}
		if (info.TryGetProperty(PropertyName._returnToGameButton, out var value8))
		{
			_returnToGameButton = value8.As<NButton>();
		}
		if (info.TryGetProperty(PropertyName._returnToGameLabel, out var value9))
		{
			_returnToGameLabel = value9.As<MegaLabel>();
		}
		if (info.TryGetProperty(PropertyName._returnToGameHoverLabel, out var value10))
		{
			_returnToGameHoverLabel = value10.As<MegaLabel>();
		}
		if (info.TryGetProperty(PropertyName._categoryDropdown, out var value11))
		{
			_categoryDropdown = value11.As<NFeedbackCategoryDropdown>();
		}
		if (info.TryGetProperty(PropertyName._sendBackstop, out var value12))
		{
			_sendBackstop = value12.As<Control>();
		}
		if (info.TryGetProperty(PropertyName._sendPanel, out var value13))
		{
			_sendPanel = value13.As<Control>();
		}
		if (info.TryGetProperty(PropertyName._successLabel, out var value14))
		{
			_successLabel = value14.As<MegaLabel>();
		}
		if (info.TryGetProperty(PropertyName._failedLabel, out var value15))
		{
			_failedLabel = value15.As<MegaLabel>();
		}
		if (info.TryGetProperty(PropertyName._sendingLabel, out var value16))
		{
			_sendingLabel = value16.As<MegaRichTextLabel>();
		}
		if (info.TryGetProperty(PropertyName._flower, out var value17))
		{
			_flower = value17.As<NSendFeedbackFlower>();
		}
		if (info.TryGetProperty(PropertyName._selectedEmoteButton, out var value18))
		{
			_selectedEmoteButton = value18.As<NSendFeedbackEmojiButton>();
		}
		if (info.TryGetProperty(PropertyName._screenshotBytes, out var value19))
		{
			_screenshotBytes = value19.As<byte[]>();
		}
		if (info.TryGetProperty(PropertyName._originalSuccessPosition, out var value20))
		{
			_originalSuccessPosition = value20.As<Vector2>();
		}
		if (info.TryGetProperty(PropertyName._lastClosedMsec, out var value21))
		{
			_lastClosedMsec = value21.As<ulong>();
		}
		if (info.TryGetProperty(PropertyName._descriptionText, out var value22))
		{
			_descriptionText = value22.As<string>();
		}
		if (info.TryGetProperty(PropertyName._descriptionCaretLine, out var value23))
		{
			_descriptionCaretLine = value23.As<int>();
		}
		if (info.TryGetProperty(PropertyName._descriptionCaretColumn, out var value24))
		{
			_descriptionCaretColumn = value24.As<int>();
		}
		if (info.TryGetProperty(PropertyName._wiggleTween, out var value25))
		{
			_wiggleTween = value25.As<Tween>();
		}
	}
}
