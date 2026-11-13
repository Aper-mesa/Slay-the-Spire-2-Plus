using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.ControllerInput;
using MegaCrit.Sts2.Core.Debug;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Characters;
using MegaCrit.Sts2.Core.Multiplayer;
using MegaCrit.Sts2.Core.Multiplayer.Game;
using MegaCrit.Sts2.Core.Multiplayer.Game.Lobby;
using MegaCrit.Sts2.Core.Multiplayer.Messages.Lobby;
using MegaCrit.Sts2.Core.Nodes.Audio;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.Debug;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Multiplayer;
using MegaCrit.Sts2.Core.Nodes.Screens.CharacterSelect;
using MegaCrit.Sts2.Core.Nodes.Screens.MainMenu;
using MegaCrit.Sts2.Core.Nodes.Vfx.Ui;
using MegaCrit.Sts2.Core.Nodes.Vfx.Utilities;
using MegaCrit.Sts2.Core.Platform;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Saves;
using MegaCrit.Sts2.Core.Saves.Managers;
using MegaCrit.Sts2.Core.TestSupport;
using MegaCrit.Sts2.Core.Unlocks;
using MegaCrit.Sts2.addons.mega_text;

namespace MegaCrit.Sts2.Core.Nodes.Screens.Dread;

[ScriptPath("res://src/Core/Nodes/Screens/Dread/NDreadScreen.cs")]
public class NDreadScreen : NSubmenu, IStartRunLobbyListener, ICharacterSelectButtonDelegate
{
	public new class MethodName : NSubmenu.MethodName
	{
		public new static readonly StringName _Ready = "_Ready";

		public static readonly StringName InitializeSingleplayer = "InitializeSingleplayer";

		public static readonly StringName InitCharacterButtons = "InitCharacterButtons";

		public new static readonly StringName _Process = "_Process";

		public static readonly StringName AscensionChanged = "AscensionChanged";

		public static readonly StringName SeedChanged = "SeedChanged";

		public static readonly StringName ModifiersChanged = "ModifiersChanged";

		public static readonly StringName GameModeChanged = "GameModeChanged";

		public static readonly StringName MaxAscensionChanged = "MaxAscensionChanged";

		public static readonly StringName UpdatePreferredDreadCharacter = "UpdatePreferredDreadCharacter";

		public static readonly StringName IsBeginningRotation = "IsBeginningRotation";

		public new static readonly StringName OnSubmenuClosed = "OnSubmenuClosed";

		public static readonly StringName SelectCharacter = "SelectCharacter";

		public static readonly StringName BeginRotationTickboxChanged = "BeginRotationTickboxChanged";

		public static readonly StringName OnEmbarkPressed = "OnEmbarkPressed";

		public static readonly StringName AfterInitialized = "AfterInitialized";

		public static readonly StringName InitRotatingCharacterButton = "InitRotatingCharacterButton";

		public static readonly StringName OnUnreadyPressed = "OnUnreadyPressed";

		public static readonly StringName OpenDreadInfo = "OpenDreadInfo";

		public static readonly StringName UpdateRichPresence = "UpdateRichPresence";

		public new static readonly StringName OnSubmenuOpened = "OnSubmenuOpened";

		public static readonly StringName RefreshDreadRecordsUi = "RefreshDreadRecordsUi";

		public static readonly StringName CleanUpLobby = "CleanUpLobby";
	}

	public new class PropertyName : NSubmenu.PropertyName
	{
		public new static readonly StringName InitialFocusedControl = "InitialFocusedControl";

		public new static readonly StringName _backButton = "_backButton";

		public static readonly StringName _embarkButton = "_embarkButton";

		public static readonly StringName _underConstruction = "_underConstruction";

		public static readonly StringName _nameText = "_nameText";

		public static readonly StringName _selectedButton = "_selectedButton";

		public static readonly StringName _chainVisualization = "_chainVisualization";

		public static readonly StringName _charButtonContainer = "_charButtonContainer";

		public static readonly StringName _readyAndWaitingContainer = "_readyAndWaitingContainer";

		public static readonly StringName _remotePlayerContainer = "_remotePlayerContainer";

		public static readonly StringName _rotatingCharacterButton = "_rotatingCharacterButton";

		public static readonly StringName _rotatingImage = "_rotatingImage";

		public static readonly StringName _startRotationTickbox = "_startRotationTickbox";

		public static readonly StringName _startRotationLabel = "_startRotationLabel";

		public static readonly StringName _unreadyButton = "_unreadyButton";

		public static readonly StringName _dreadInfoButton = "_dreadInfoButton";

		public static readonly StringName _bgContainer = "_bgContainer";

		public static readonly StringName _grayscaleBg = "_grayscaleBg";

		public static readonly StringName _scoreContainer = "_scoreContainer";

		public static readonly StringName _scoreLabels = "_scoreLabels";

		public static readonly StringName _scoreValues = "_scoreValues";

		public static readonly StringName _skipNextCharSelectLogic = "_skipNextCharSelectLogic";

		public static readonly StringName _doingChainAnimation = "_doingChainAnimation";

		public static readonly StringName _overriddenFocusedControl = "_overriddenFocusedControl";
	}

	public new class SignalName : NSubmenu.SignalName
	{
	}

	public static readonly StringName shaderParameterBrightness = new StringName("brightness");

	public static readonly StringName shaderParameterContrast = new StringName("contrast");

	private NBackButton _backButton;

	private NConfirmButton _embarkButton;

	private Control _underConstruction;

	private NDreadNameText _nameText;

	private NCharacterSelectButton? _selectedButton;

	private CharacterModel? _rotatingDisplayCharacterOverride;

	private CharacterModel? _rotatingPostAnimationCharacterSelect;

	private CharacterModel? _rotatingCharacter;

	private NChainVisualization _chainVisualization;

	private Control _charButtonContainer;

	private Control _readyAndWaitingContainer;

	private NRemoteLobbyPlayerContainer _remotePlayerContainer;

	private NCharacterSelectButton _rotatingCharacterButton;

	private Control _rotatingImage;

	private NDreadBeginRotationTickbox _startRotationTickbox;

	private MegaRichTextLabel _startRotationLabel;

	private NBackButton _unreadyButton;

	private const string _sceneCharSelectButtonPath = "screens/char_select/char_select_button";

	private NDreadInfoButton _dreadInfoButton;

	private Control _bgContainer;

	private ShaderMaterial _grayscaleBg;

	private Control _scoreContainer;

	private MegaRichTextLabel _scoreLabels;

	private MegaRichTextLabel _scoreValues;

	private IBootstrapSettings? _settings;

	private StartRunLobby _lobby;

	private List<NCharacterSelectButton> _charSelectButtons = new List<NCharacterSelectButton>();

	private bool _skipNextCharSelectLogic;

	private bool _doingChainAnimation;

	private bool? _lastOpenWasSingleplayer;

	private Control? _overriddenFocusedControl;

	private const int _ascension = 10;

	public StartRunLobby Lobby => _lobby;

	protected override Control? InitialFocusedControl => _overriddenFocusedControl ?? _charButtonContainer.GetChild<Control>(0);

	public override void _Ready()
	{
		ConnectSignals();
		_underConstruction = GetNode<Control>("%UnderConstruction");
		_underConstruction.OffsetLeft = 24f;
		_underConstruction.OffsetTop = 82f;
		_nameText = GetNode<NDreadNameText>("%DreadNameText");
		_rotatingImage = GetNode<Control>("%RotatingImage");
		_startRotationTickbox = GetNode<NDreadBeginRotationTickbox>("%RotatingTickbox");
		_startRotationLabel = GetNode<MegaRichTextLabel>("%RotatingExplanation");
		_bgContainer = GetNode<Control>("%AnimatedBg");
		_grayscaleBg = (ShaderMaterial)GetNode<Control>("%GrayScaler").Material;
		_charButtonContainer = GetNode<Control>("%ButtonContainer");
		_remotePlayerContainer = GetNode<NRemoteLobbyPlayerContainer>("%RemotePlayerContainer");
		_readyAndWaitingContainer = GetNode<Control>("%ReadyAndWaitingPanel");
		GetNode<MegaRichTextLabel>("%WaitingForPlayers").Text = new LocString("main_menu_ui", "CHARACTER_SELECT.waitingForPlayers").GetFormattedText();
		_backButton = GetNode<NBackButton>("%BackButton");
		_unreadyButton = GetNode<NBackButton>("%UnreadyButton");
		_embarkButton = GetNode<NConfirmButton>("%ConfirmButton");
		_embarkButton.OverrideHotkeys(new string[1] { MegaInput.select });
		_embarkButton.Connect(NClickableControl.SignalName.Released, Callable.From<NButton>(OnEmbarkPressed));
		_unreadyButton.Connect(NClickableControl.SignalName.Released, Callable.From<NButton>(OnUnreadyPressed));
		_unreadyButton.Disable();
		_dreadInfoButton = GetNode<NDreadInfoButton>("%DreadInfoButton");
		_dreadInfoButton.Connect(NClickableControl.SignalName.Released, Callable.From<NButton>(OpenDreadInfo));
		_chainVisualization = GetNode<NChainVisualization>("%DreadWinStreak");
		_scoreLabels = GetNode<MegaRichTextLabel>("%ScoreLabels");
		_scoreValues = GetNode<MegaRichTextLabel>("%ScoreValues");
		Tween tween = CreateTween();
		tween.TweenProperty(_rotatingImage, "rotation", (float)Math.PI * 2f, 10.0).From(0f).SetTrans(Tween.TransitionType.Linear);
		tween.SetLoops();
		_rotatingImage.Visible = false;
		InitCharacterButtons();
		Type type = BootstrapSettingsUtil.Get();
		if (type != null)
		{
			_settings = (IBootstrapSettings)Activator.CreateInstance(type);
			PreloadManager.Enabled = _settings.DoPreloading;
		}
	}

	public void InitializeSingleplayer()
	{
		_lobby = new StartRunLobby(GameMode.Dread, isCustom: false, new NetSingleplayerGameService(), this, 1);
		_lobby.AddLocalHostPlayer(new UnlockState(SaveManager.Instance.Progress), 0, SaveManager.Instance.Progress.MaxBlitzAscension);
		AfterInitialized();
	}

	public void InitializeMultiplayerAsHost(INetGameService gameService, int maxPlayers)
	{
		if (gameService.Type != NetGameType.Host)
		{
			throw new InvalidOperationException($"Initialized Dread with GameService of type {gameService.Type} when hosting!");
		}
		_lobby = new StartRunLobby(GameMode.Dread, isCustom: false, gameService, this, maxPlayers);
		_lobby.AddLocalHostPlayer(new UnlockState(SaveManager.Instance.Progress), 10, SaveManager.Instance.Progress.MaxBlitzAscension);
		AfterInitialized();
	}

	public void InitializeMultiplayerAsClient(INetGameService gameService, ClientLobbyJoinResponseMessage message)
	{
		if (gameService.Type != NetGameType.Client)
		{
			throw new InvalidOperationException($"Initialized character select screen with GameService of type {gameService.Type} when joining!");
		}
		_lobby = new StartRunLobby(GameMode.Dread, isCustom: false, gameService, this, -1);
		_lobby.InitializeFromMessage(message);
		AfterInitialized();
	}

	private void InitCharacterButtons()
	{
		foreach (CharacterModel allCharacter in ModelDb.AllCharacters)
		{
			NCharacterSelectButton nCharacterSelectButton = SceneHelper.Instantiate<NCharacterSelectButton>("screens/char_select/char_select_button");
			_charButtonContainer.AddChildSafely(nCharacterSelectButton);
			nCharacterSelectButton.Init(allCharacter, this);
			_charSelectButtons.Add(nCharacterSelectButton);
		}
		_rotatingCharacterButton = SceneHelper.Instantiate<NCharacterSelectButton>("screens/char_select/char_select_button");
		_charButtonContainer.AddChildSafely(_rotatingCharacterButton);
		_charSelectButtons.Add(_rotatingCharacterButton);
		List<NCharacterSelectButton> list = _charSelectButtons.Where((NCharacterSelectButton c) => c.Visible).ToList();
		for (int num = 0; num < list.Count; num++)
		{
			list[num].FocusNeighborTop = list[num].GetPath();
			list[num].FocusNeighborBottom = list[num].GetPath();
			NCharacterSelectButton nCharacterSelectButton2 = list[num];
			NodePath path;
			if (num <= 0)
			{
				path = list[list.Count - 1].GetPath();
			}
			else
			{
				path = list[num - 1].GetPath();
			}
			nCharacterSelectButton2.FocusNeighborLeft = path;
			list[num].FocusNeighborRight = ((num < list.Count - 1) ? list[num + 1].GetPath() : list[0].GetPath());
		}
	}

	public override void _Process(double delta)
	{
		if (_lobby != null && _lobby.NetService.IsConnected)
		{
			_lobby.NetService.Update();
		}
	}

	public void PlayerConnected(StartRunLobbyPlayer player)
	{
		_remotePlayerContainer.OnPlayerConnected(player);
		RefreshButtonSelectionForPlayer(player);
		UpdateRichPresence();
	}

	private void RefreshButtonSelectionForPlayer(StartRunLobbyPlayer player)
	{
		if (player.id == _lobby.LocalPlayer.id)
		{
			return;
		}
		CharacterModel character = player.character;
		foreach (NCharacterSelectButton charSelectButton in _charSelectButtons)
		{
			if (charSelectButton.RemoteSelectedPlayers.Contains(player.id) && character != charSelectButton.Character)
			{
				charSelectButton.OnRemotePlayerDeselected(player.id);
			}
			else if (character == charSelectButton.Character)
			{
				charSelectButton.OnRemotePlayerSelected(player.id);
			}
		}
	}

	public void PlayerChanged(StartRunLobbyPlayer player)
	{
		_remotePlayerContainer.OnPlayerChanged(player);
		RefreshButtonSelectionForPlayer(player);
	}

	public void AscensionChanged()
	{
	}

	public void SeedChanged()
	{
		throw new NotImplementedException("Seed should not be changed in Dread mode!");
	}

	public void ModifiersChanged()
	{
		throw new NotImplementedException("Modifiers should not be changed in Dread mode!");
	}

	public void GameModeChanged()
	{
		throw new NotImplementedException("Game mode should not be changed in Dread mode!");
	}

	public void MaxAscensionChanged()
	{
	}

	public void RemotePlayerDisconnected(StartRunLobbyPlayer player)
	{
		_remotePlayerContainer.OnPlayerDisconnected(player);
		foreach (NCharacterSelectButton charSelectButton in _charSelectButtons)
		{
			if (charSelectButton.RemoteSelectedPlayers.Contains(player.id) && player.character == charSelectButton.Character)
			{
				charSelectButton.OnRemotePlayerDeselected(player.id);
			}
		}
		UpdateRichPresence();
	}

	public void BeginRun(string seed, List<ActModel> acts, IReadOnlyList<ModifierModel> modifiers)
	{
		if (modifiers.Count > 0)
		{
			Log.Error("Modifiers list is not empty while starting a Dread run, ignoring!");
		}
		NAudioManager.Instance?.StopMusic();
		_embarkButton.Disable();
		_unreadyButton.Disable();
		_startRotationTickbox.Disable();
		if (_lobby.NetService.Type == NetGameType.Singleplayer)
		{
			TaskHelper.RunSafely(StartNewSingleplayerRun(seed, acts));
		}
		else
		{
			TaskHelper.RunSafely(StartNewMultiplayerRun(seed, acts));
		}
	}

	private async Task StartNewSingleplayerRun(string seed, List<ActModel> acts)
	{
		Log.Info("Embarking on a solo Dread run " + _lobby.LocalPlayer.character.Id.Entry + ". Seed: " + seed);
		Player player = _lobby.GeneratePlayers(seed)[0];
		player.ExtraFields.IsPlayingRotatingCharacter = _lobby.LocalPlayer.isRotatingDread;
		UpdateProgressForRotation(player.Character);
		try
		{
			SfxCmd.Play(player.Character.CharacterTransitionSfx);
			await NGame.Instance.Transition.FadeOut(0.8f, player.Character.CharacterSelectTransitionPath);
			await NGame.Instance.StartNewSingleplayerRun(player, shouldSave: true, acts, Array.Empty<ModifierModel>(), seed, GameMode.Dread, isCustomGame: false, 10);
		}
		catch (Exception ex)
		{
			Log.Error($"Exception starting a solo Dread run: {ex}");
			CleanUpLobby(disconnectSession: true, NetError.InternalError);
			await NGame.Instance.ReturnToMainMenuWithInternalError(ex);
			return;
		}
		CleanUpLobby(disconnectSession: false);
	}

	private async Task StartNewMultiplayerRun(string seed, List<ActModel> acts)
	{
		Log.Info("Embarking on a multiplayer Dread run. Players: " + string.Join(",", _lobby.Players) + ". Seed: " + seed);
		List<Player> players = _lobby.GeneratePlayers(seed);
		Player player = players.First((Player p) => p.NetId == _lobby.LocalPlayer.id);
		UpdateProgressForRotation(player.Character);
		SfxCmd.Play(player.Character.CharacterTransitionSfx);
		await NGame.Instance.Transition.FadeOut(0.8f, player.Character.CharacterSelectTransitionPath);
		if (Bootstrapper.Settings?.BootstrapInMultiplayer ?? false)
		{
			using (new NetLoadingHandle(_lobby.NetService))
			{
				Bootstrapper bootstrapper = new Bootstrapper().WithActs(acts).WithPlayers(players).WithRunManagerSetup((RunState r, bool s) =>
				{
					RunManager.Instance.SetUpNewMultiplayer(r, _lobby, s);
				})
					.WithSeed(seed);
				try
				{
					await bootstrapper.SetupRun();
				}
				catch (Exception ex)
				{
					Log.Error($"Exception starting bootstrap multiplayer run : {ex}");
					CleanUpLobby(disconnectSession: true, NetError.InternalError);
					await NGame.Instance.ReturnToMainMenuWithInternalError(ex);
					return;
				}
				CleanUpLobby(disconnectSession: false);
				await bootstrapper.EnterFirstRoom();
			}
		}
		else
		{
			try
			{
				await NGame.Instance.StartNewMultiplayerRun(players, _lobby, shouldSave: true, acts, Array.Empty<ModifierModel>(), seed, 10);
			}
			catch (Exception ex2)
			{
				Log.Error($"Exception starting multiplayer run : {ex2}");
				CleanUpLobby(disconnectSession: true, NetError.InternalError);
				await NGame.Instance.ReturnToMainMenuWithInternalError(ex2);
				return;
			}
			CleanUpLobby(disconnectSession: false);
		}
	}

	private void UpdateProgressForRotation(CharacterModel embarkingCharacter)
	{
		bool flag = !_lobby.NetService.Type.IsMultiplayer();
		bool flag2 = IsBeginningRotation();
		UpdatePreferredDreadCharacter();
		if (flag)
		{
			SaveManager.Instance.Progress.PreferredDreadRotatingTickboxOnSingleplayer = flag2 || _rotatingCharacterButton.IsSelected;
			if (flag2)
			{
				SaveManager.Instance.Progress.CurrentSingleplayerDreadStartingCharacterForRotation = embarkingCharacter.Id;
				SaveManager.Instance.Progress.PreferredDreadCharacter = ModelDb.GetId<RotatingCharacter>();
			}
		}
		else
		{
			SaveManager.Instance.Progress.PreferredDreadRotatingTickboxOnMultiplayer = flag2 || _rotatingCharacterButton.IsSelected;
			if (flag2)
			{
				SaveManager.Instance.Progress.CurrentMultiplayerDreadStartingCharacterForRotation = embarkingCharacter.Id;
				SaveManager.Instance.Progress.PreferredMultiplayerDreadCharacter = ModelDb.GetId<RotatingCharacter>();
			}
		}
		SaveManager.Instance.SaveProgressFile();
	}

	private void UpdatePreferredDreadCharacter()
	{
		if (_selectedButton == null)
		{
			return;
		}
		ModelId id = _selectedButton.Character.Id;
		ProgressState progress = SaveManager.Instance.Progress;
		if (_lobby.NetService.Type == NetGameType.Singleplayer)
		{
			if (!(progress.PreferredDreadCharacter == id))
			{
				Log.Info($"Setting preferred Dread character to {id}");
				progress.PreferredDreadCharacter = id;
			}
		}
		else if (!(progress.PreferredMultiplayerDreadCharacter == id))
		{
			Log.Info($"Setting preferred multiplayer Dread character to {id}");
			progress.PreferredMultiplayerDreadCharacter = id;
		}
	}

	private bool IsBeginningRotation()
	{
		bool flag = _lobby.NetService.Type.IsMultiplayer();
		int currentRotatingDreadWinStreak = AscensionHelper.GetCurrentRotatingDreadWinStreak(flag);
		if (!flag && currentRotatingDreadWinStreak == 0)
		{
			return _startRotationTickbox.IsTicked;
		}
		return false;
	}

	public override void OnSubmenuClosed()
	{
		base.OnSubmenuClosed();
		_embarkButton.Disable();
		_remotePlayerContainer.Cleanup();
		if (_lobby.NetService.Type.IsMultiplayer())
		{
			PlatformUtil.SetRichPresence("MAIN_MENU", null, null);
		}
		CleanUpLobby(disconnectSession: true);
	}

	public void LocalPlayerDisconnected(NetErrorInfo info)
	{
		if ((info.SelfInitiated && info.GetReason() == NetError.Quit) || !this.IsValid() || _stack == null)
		{
			return;
		}
		if (_stack.Peek() == this)
		{
			_stack.Pop();
		}
		if (TestMode.IsOff)
		{
			NErrorPopup nErrorPopup = NErrorPopup.Create(info);
			if (nErrorPopup != null)
			{
				NModalContainer.Instance.Add(nErrorPopup);
			}
		}
	}

	public void SelectCharacter(NCharacterSelectButton charSelectButton)
	{
		_selectedButton = charSelectButton;
		if (_skipNextCharSelectLogic)
		{
			_skipNextCharSelectLogic = false;
			return;
		}
		CharacterModel character = charSelectButton.Character;
		SetSelectedCharacterVisuals(character);
		InitChainForCharacter(character);
		RefreshDreadRecordsUi();
		foreach (NCharacterSelectButton charSelectButton2 in _charSelectButtons)
		{
			if (charSelectButton2 != _selectedButton)
			{
				charSelectButton2.Deselect();
			}
		}
		if (charSelectButton.IsLocked)
		{
			_embarkButton.Disable();
		}
		else if (!_doingChainAnimation)
		{
			_embarkButton.Enable();
		}
		bool flag = _lobby.NetService.Type == NetGameType.Singleplayer;
		bool isRotatingDread = character is RotatingCharacter || IsBeginningRotation();
		CharacterModel character2;
		if (!(character is RotatingCharacter))
		{
			character2 = character;
		}
		else
		{
			character2 = ((_rotatingPostAnimationCharacterSelect == null) ? AscensionHelper.GetNextRotatingDreadCharacter(!flag) : _rotatingPostAnimationCharacterSelect);
		}
		_lobby.SetLocalCharacter(character2, isRotatingDread);
	}

	private void SetSelectedCharacterVisuals(CharacterModel characterModel)
	{
		bool flag = _lobby.NetService.Type == NetGameType.Singleplayer;
		int currentRotatingDreadWinStreak = AscensionHelper.GetCurrentRotatingDreadWinStreak(!flag);
		bool flag2 = characterModel is RotatingCharacter;
		_startRotationTickbox.Visible = flag && !flag2 && currentRotatingDreadWinStreak <= 0;
		if (_startRotationTickbox.Visible && !_doingChainAnimation)
		{
			_startRotationTickbox.Enable();
		}
		else
		{
			_startRotationTickbox.Disable();
		}
		_rotatingImage.Visible = flag2;
		_chainVisualization.Visible = !_startRotationTickbox.Visible || !_startRotationTickbox.IsTicked || _chainVisualization.IsBreaking;
		NGame.Instance?.ScreenShake(ShakeStrength.Weak, ShakeDuration.Short, 90f);
		foreach (Node child2 in _bgContainer.GetChildren())
		{
			_bgContainer.RemoveChildSafely(child2);
			child2.QueueFreeSafely();
		}
		CharacterModel characterModel2;
		if (characterModel is RotatingCharacter)
		{
			characterModel2 = _rotatingDisplayCharacterOverride ?? _rotatingCharacter ?? characterModel;
			LocString locString = new LocString("main_menu_ui", "DREAD_ROTATION_CHARACTER");
			locString.Add("CharacterName", characterModel2.ShortTitle);
			_nameText.SetNameText(locString, characterModel2.DreadNameColor);
		}
		else
		{
			characterModel2 = characterModel;
			_nameText.SetNameText(characterModel2);
		}
		SfxCmd.Play(characterModel2.CharacterSelectSfx);
		Control child = characterModel2.CreateCharacterSelectBackground();
		_bgContainer.AddChildSafely(child);
		_grayscaleBg.SetShaderParameter(shaderParameterBrightness, characterModel2.DreadBgBrightness);
		_grayscaleBg.SetShaderParameter(shaderParameterContrast, characterModel2.DreadBgContrast);
	}

	public void BeginRotationTickboxChanged(bool beginRotation)
	{
		bool flag = !_lobby.NetService.Type.IsMultiplayer();
		if (!beginRotation && _selectedButton != null)
		{
			CharacterStats statsForCharacter = SaveManager.Instance.Progress.GetStatsForCharacter(_selectedButton.Character.Id);
			int count = ((!flag) ? (statsForCharacter?.CurrentMultiplayerDreadWinStreak ?? 0) : (statsForCharacter?.CurrentSingleplayerDreadWinStreak ?? 0));
			_chainVisualization.Initialize(count);
		}
		RefreshDreadRecordsUi();
		_chainVisualization.Visible = !beginRotation || _chainVisualization.IsBreaking;
		if (_selectedButton != null && _lobby.LocalPlayer.isRotatingDread != beginRotation)
		{
			_lobby.SetLocalCharacter(_selectedButton.Character, beginRotation);
		}
	}

	private void OnEmbarkPressed(NButton _)
	{
		_embarkButton.Disable();
		_startRotationTickbox.Disable();
		_lobby.SetReady(ready: true);
		foreach (NCharacterSelectButton charSelectButton in _charSelectButtons)
		{
			charSelectButton.Disable();
		}
		_backButton.Disable();
		if (_lobby.NetService.Type.IsMultiplayer() && !_lobby.IsAboutToBeginGame())
		{
			_readyAndWaitingContainer.Visible = true;
			_unreadyButton.Enable();
		}
	}

	private void AfterInitialized()
	{
		NGame.Instance.RemoteCursorContainer.Initialize(_lobby.InputSynchronizer, _lobby.Players.Select((StartRunLobbyPlayer p) => p.id));
		NGame.Instance.ReactionContainer.InitializeNetworking(_lobby.NetService);
		NGame.Instance.TimeoutOverlay.Initialize(_lobby.NetService, isGameLevel: true);
		_lobby.PlayerFailedToConnect += RemoteClientFailedToConnectToLocalHost;
		UpdateRichPresence();
		MegaCrit.Sts2.Core.Logging.Logger.logLevelTypeMap[LogType.Network] = ((_lobby.NetService.Type == NetGameType.Singleplayer) ? LogLevel.Info : LogLevel.Debug);
		MegaCrit.Sts2.Core.Logging.Logger.logLevelTypeMap[LogType.Actions] = ((_lobby.NetService.Type == NetGameType.Singleplayer) ? LogLevel.Info : LogLevel.VeryDebug);
		MegaCrit.Sts2.Core.Logging.Logger.logLevelTypeMap[LogType.GameSync] = ((_lobby.NetService.Type == NetGameType.Singleplayer) ? LogLevel.Info : LogLevel.VeryDebug);
		if (_lobby.NetService.Type != NetGameType.Singleplayer)
		{
			IBootstrapSettings? settings = _settings;
			if (settings != null && settings.BootstrapInMultiplayer)
			{
				NGame.Instance.DebugSeedOverride = _settings.Seed;
				return;
			}
		}
		NGame.Instance.DebugSeedOverride = null;
	}

	private void InitRotatingCharacterButton()
	{
		if (_lobby.NetService.Type.IsMultiplayer())
		{
			_rotatingCharacterButton.Visible = false;
		}
		else if (_rotatingDisplayCharacterOverride != null)
		{
			_rotatingCharacterButton.Visible = true;
			_rotatingCharacterButton.InitAsRotating(_rotatingDisplayCharacterOverride, this);
		}
		else if (_rotatingCharacter != null)
		{
			_rotatingCharacterButton.Visible = true;
			_rotatingCharacterButton.InitAsRotating(_rotatingCharacter, this);
		}
		else
		{
			_rotatingCharacterButton.Visible = false;
		}
	}

	private void OnUnreadyPressed(NButton _)
	{
		_lobby.SetReady(ready: false);
		foreach (NCharacterSelectButton charSelectButton in _charSelectButtons)
		{
			charSelectButton.Enable();
		}
		_selectedButton?.TryGrabFocus();
		_readyAndWaitingContainer.Visible = false;
		_embarkButton.Enable();
		_backButton.Enable();
		_startRotationTickbox.Enable();
		_unreadyButton.Disable();
	}

	private void OpenDreadInfo(NButton _)
	{
		this.AddChildSafely(NDreadInfoDisclaimer.Create(isDelayedOpen: false));
	}

	private void UpdateRichPresence()
	{
		if (_lobby.NetService.Type.IsMultiplayer())
		{
			PlatformUtil.SetRichPresence("DREAD_MP_LOBBY", _lobby.NetService.GetRawLobbyIdentifier(), _lobby.Players.Count);
		}
	}

	public override void OnSubmenuOpened()
	{
		base.OnSubmenuOpened();
		foreach (NCharacterSelectButton charSelectButton in _charSelectButtons)
		{
			if (!charSelectButton.IsLocked)
			{
				charSelectButton.Enable();
			}
			else
			{
				charSelectButton.UnlockIfPossible();
			}
			charSelectButton.Reset();
		}
		_rotatingCharacter = null;
		_selectedButton = null;
		_rotatingDisplayCharacterOverride = null;
		_rotatingPostAnimationCharacterSelect = null;
		_embarkButton.Enable();
		bool flag = _lobby.NetService.Type == NetGameType.Singleplayer;
		ModelId preferredCharacterId = (flag ? SaveManager.Instance.Progress.PreferredDreadCharacter : SaveManager.Instance.Progress.PreferredMultiplayerDreadCharacter);
		PendingDreadWinStreakResult pendingDreadWinStreakResult = (flag ? SaveManager.Instance.Progress.PendingSingleplayerDreadWinStreakResult : SaveManager.Instance.Progress.PendingMultiplayerDreadWinStreakResult);
		bool isTicked = (flag ? SaveManager.Instance.Progress.PreferredDreadRotatingTickboxOnSingleplayer : SaveManager.Instance.Progress.PreferredDreadRotatingTickboxOnMultiplayer);
		int currentRotatingDreadWinStreak = AscensionHelper.GetCurrentRotatingDreadWinStreak(!flag);
		if (pendingDreadWinStreakResult?.CharacterId == ModelDb.GetId<RotatingCharacter>())
		{
			ModelId startingCharacter = ((pendingDreadWinStreakResult.InitialRotatingCharacterBeforeReset == ModelId.none) ? ModelDb.GetId<Ironclad>() : pendingDreadWinStreakResult.InitialRotatingCharacterBeforeReset);
			ModelId rotatingCharacterIdForStreak = AscensionHelper.GetRotatingCharacterIdForStreak(startingCharacter, pendingDreadWinStreakResult.WinStreakBeforeReset + 1);
			if (pendingDreadWinStreakResult.WasReset && pendingDreadWinStreakResult.WinStreakBeforeReset == 0)
			{
				preferredCharacterId = rotatingCharacterIdForStreak;
				if (flag)
				{
					SaveManager.Instance.Progress.PreferredDreadCharacter = rotatingCharacterIdForStreak;
				}
				else
				{
					SaveManager.Instance.Progress.PreferredMultiplayerDreadCharacter = rotatingCharacterIdForStreak;
				}
				SaveManager.Instance.SaveProgressFile();
			}
			else
			{
				_rotatingDisplayCharacterOverride = ModelDb.GetById<CharacterModel>(AscensionHelper.GetRotatingCharacterIdForStreak(startingCharacter, pendingDreadWinStreakResult.WinStreakBeforeReset));
				_rotatingPostAnimationCharacterSelect = ModelDb.GetById<CharacterModel>(rotatingCharacterIdForStreak);
			}
		}
		if (currentRotatingDreadWinStreak > 0)
		{
			ModelId nextRotatingDreadCharacterId = AscensionHelper.GetNextRotatingDreadCharacterId(!flag);
			_rotatingCharacter = ModelDb.GetById<CharacterModel>(nextRotatingDreadCharacterId);
		}
		if (flag != _lastOpenWasSingleplayer)
		{
			_startRotationTickbox.InitTickedState(isTicked);
		}
		_startRotationTickbox.Enable();
		InitRotatingCharacterButton();
		NCharacterSelectButton overriddenFocusedControl = _charSelectButtons.FirstOrDefault((NCharacterSelectButton button) => !button.IsLocked && button.Visible && button.Character.Id == preferredCharacterId);
		_overriddenFocusedControl = overriddenFocusedControl;
		_remotePlayerContainer.Visible = _lobby.NetService.Type != NetGameType.Singleplayer;
		_remotePlayerContainer.Initialize(_lobby, displayLocalPlayer: true);
		_readyAndWaitingContainer.Visible = false;
		_underConstruction.Visible = !_remotePlayerContainer.Visible;
		if (!SaveManager.Instance.SeenPopup("dread_disclaimer"))
		{
			this.AddChildSafely(NDreadInfoDisclaimer.Create(isDelayedOpen: true));
		}
		foreach (StartRunLobbyPlayer player in _lobby.Players)
		{
			RefreshButtonSelectionForPlayer(player);
		}
		NControllerManager? instance = NControllerManager.Instance;
		if (instance != null && !instance.IsUsingDirectionalNavigation)
		{
			Callable.From(() =>
			{
				(InitialFocusedControl as NCharacterSelectButton)?.Select();
			}).CallDeferred();
		}
		_lastOpenWasSingleplayer = flag;
	}

	private async Task PlayDreadWinStreakResultAnimation(int currentStreak, PendingDreadWinStreakResult winStreakResult, bool isSingleplayer)
	{
		_embarkButton.Disable();
		_backButton.Disable();
		foreach (NCharacterSelectButton charSelectButton in _charSelectButtons)
		{
			charSelectButton.Disable();
		}
		try
		{
			_doingChainAnimation = true;
			if (winStreakResult.WasReset)
			{
				_chainVisualization.Initialize(winStreakResult.WinStreakBeforeReset);
				await _chainVisualization.Break(0.5f);
			}
			else if (winStreakResult.WinStreakIncrease > 0)
			{
				_chainVisualization.Initialize(currentStreak);
				TaskCompletionSource taskCompletionSource = new TaskCompletionSource();
				TaskHelper.RunSafely(_chainVisualization.AnimateInLastLink(0.5f, taskCompletionSource));
				await taskCompletionSource.Task;
			}
		}
		finally
		{
			_doingChainAnimation = false;
			if (isSingleplayer)
			{
				SaveManager.Instance.Progress.PendingSingleplayerDreadWinStreakResult = null;
			}
			else
			{
				SaveManager.Instance.Progress.PendingMultiplayerDreadWinStreakResult = null;
			}
			if (winStreakResult.CharacterId == ModelDb.GetId<RotatingCharacter>())
			{
				_rotatingDisplayCharacterOverride = null;
				CharacterModel nextCharacter = _rotatingPostAnimationCharacterSelect ?? ModelDb.Character<Ironclad>();
				if (winStreakResult.WasReset)
				{
					_skipNextCharSelectLogic = true;
					_charSelectButtons.FirstOrDefault((NCharacterSelectButton b) => b.Character == nextCharacter)?.Select();
					_rotatingCharacterButton.Deselect();
					if (isSingleplayer)
					{
						SaveManager.Instance.Progress.PreferredDreadCharacter = nextCharacter.Id;
					}
					else
					{
						SaveManager.Instance.Progress.PreferredMultiplayerDreadCharacter = nextCharacter.Id;
					}
					SetSelectedCharacterVisuals(nextCharacter);
				}
				else
				{
					SetSelectedCharacterVisuals(ModelDb.Character<RotatingCharacter>());
				}
				_rotatingPostAnimationCharacterSelect = null;
				InitRotatingCharacterButton();
			}
			SaveManager.Instance.SaveProgressFile();
			foreach (NCharacterSelectButton charSelectButton2 in _charSelectButtons)
			{
				charSelectButton2.Enable();
			}
			if (_startRotationTickbox.Visible)
			{
				_startRotationTickbox.Enable();
			}
			await Cmd.Wait(0.5f);
			_embarkButton.Enable();
			_backButton.Enable();
		}
	}

	private void InitChainForCharacter(CharacterModel characterModel)
	{
		bool flag = _lobby.NetService.Type == NetGameType.Singleplayer;
		ProgressState progress = SaveManager.Instance.Progress;
		CharacterStats statsForCharacter = progress.GetStatsForCharacter(characterModel.Id);
		int num = ((!flag) ? (statsForCharacter?.CurrentMultiplayerDreadWinStreak ?? 0) : (statsForCharacter?.CurrentSingleplayerDreadWinStreak ?? 0));
		PendingDreadWinStreakResult pendingDreadWinStreakResult = (flag ? progress.PendingSingleplayerDreadWinStreakResult : progress.PendingMultiplayerDreadWinStreakResult);
		if (pendingDreadWinStreakResult != null && pendingDreadWinStreakResult.WasReset && pendingDreadWinStreakResult.WinStreakBeforeReset == 0)
		{
			if (flag)
			{
				SaveManager.Instance.Progress.PendingSingleplayerDreadWinStreakResult = null;
			}
			else
			{
				SaveManager.Instance.Progress.PendingMultiplayerDreadWinStreakResult = null;
			}
			pendingDreadWinStreakResult = null;
		}
		if (pendingDreadWinStreakResult == null || pendingDreadWinStreakResult.CharacterId != characterModel.Id)
		{
			_chainVisualization.Initialize(num);
			_chainVisualization.Visible = !_startRotationTickbox.Visible || !_startRotationTickbox.IsTicked || _chainVisualization.IsBreaking;
		}
		else
		{
			TaskHelper.RunSafely(PlayDreadWinStreakResultAnimation(num, pendingDreadWinStreakResult, flag));
		}
	}

	private void RefreshDreadRecordsUi()
	{
		if (_selectedButton == null)
		{
			return;
		}
		Log.Info("Updating Dread Records ui...");
		CharacterModel characterModel = (IsBeginningRotation() ? ModelDb.Character<RotatingCharacter>() : _selectedButton.Character);
		bool flag = _lobby.NetService.Type == NetGameType.Singleplayer;
		CharacterStats statsForCharacter = SaveManager.Instance.Progress.GetStatsForCharacter(characterModel.Id);
		string gameVersion = NGame.GetGameVersion();
		int num;
		int num2;
		string variable;
		if (flag)
		{
			(num, num2, variable) = ProgressSaveManager.ReconcileDreadWinStreakVersion(gameVersion, statsForCharacter?.BestSingleplayerDreadWinStreakThisVersion ?? 0, statsForCharacter?.BestSingleplayerDreadWinStreakThisVersionTag, statsForCharacter?.BestSingleplayerDreadWinStreakOldVersion ?? 0, statsForCharacter?.BestSingleplayerDreadWinStreakOldVersionTag);
		}
		else
		{
			(num, num2, variable) = ProgressSaveManager.ReconcileDreadWinStreakVersion(gameVersion, statsForCharacter?.BestMultiplayerDreadWinStreakThisVersion ?? 0, statsForCharacter?.BestMultiplayerDreadWinStreakThisVersionTag, statsForCharacter?.BestMultiplayerDreadWinStreakOldVersion ?? 0, statsForCharacter?.BestMultiplayerDreadWinStreakOldVersionTag);
		}
		if (num == 0 && num2 == 0)
		{
			_scoreLabels.Text = "";
			_scoreValues.Text = "";
			return;
		}
		List<string> list = new List<string>();
		List<string> list2 = new List<string>();
		if (num > 0)
		{
			LocString locString = new LocString("main_menu_ui", (characterModel is RotatingCharacter) ? "DREAD_BEST_ROTATING_STREAK" : "DREAD_BEST_STREAK");
			list.Add(locString.GetFormattedText());
			list2.Add(num.ToString());
		}
		if (num2 > num)
		{
			LocString locString2 = new LocString("main_menu_ui", (characterModel is RotatingCharacter) ? "DREAD_BEST_ROTATING_STREAK_OLD" : "DREAD_BEST_STREAK_OLD");
			locString2.Add("Version", variable);
			list.Add(locString2.GetFormattedText());
			list2.Add(num2.ToString());
		}
		_scoreLabels.Text = string.Join("\n", list);
		_scoreValues.Text = string.Join("\n", list2);
	}

	private void CleanUpLobby(bool disconnectSession, NetError error = NetError.Quit)
	{
		_lobby.PlayerFailedToConnect -= RemoteClientFailedToConnectToLocalHost;
		_lobby.CleanUp(disconnectSession, error);
		_lobby = null;
	}

	private void RemoteClientFailedToConnectToLocalHost(ulong sender, NetErrorInfo info)
	{
		string formattedText = NErrorPopup.LocStringFromNetError(info, out var _).GetFormattedText();
		LocString locString = new LocString("main_menu_ui", "NETWORK_ERROR.HOST.PREFIX.body");
		locString.Add("playerName", PlatformUtil.GetPlayerName(_lobby.NetService.Platform, sender));
		locString.Add("info", formattedText);
		this.AddChildSafely(NFailedJoinVfx.Create(locString.GetFormattedText()));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal new static List<MethodInfo> GetGodotMethodList()
	{
		List<MethodInfo> list = new List<MethodInfo>(23);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.InitializeSingleplayer, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.InitCharacterButtons, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName._Process, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.Float, "delta", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
		}, null));
		list.Add(new MethodInfo(MethodName.AscensionChanged, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.SeedChanged, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.ModifiersChanged, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.GameModeChanged, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.MaxAscensionChanged, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.UpdatePreferredDreadCharacter, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.IsBeginningRotation, new PropertyInfo(Variant.Type.Bool, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.OnSubmenuClosed, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.SelectCharacter, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.Object, "charSelectButton", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Control"), exported: false)
		}, null));
		list.Add(new MethodInfo(MethodName.BeginRotationTickboxChanged, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.Bool, "beginRotation", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
		}, null));
		list.Add(new MethodInfo(MethodName.OnEmbarkPressed, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.Object, "_", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Control"), exported: false)
		}, null));
		list.Add(new MethodInfo(MethodName.AfterInitialized, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.InitRotatingCharacterButton, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.OnUnreadyPressed, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.Object, "_", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Control"), exported: false)
		}, null));
		list.Add(new MethodInfo(MethodName.OpenDreadInfo, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.Object, "_", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Control"), exported: false)
		}, null));
		list.Add(new MethodInfo(MethodName.UpdateRichPresence, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.OnSubmenuOpened, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.RefreshDreadRecordsUi, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.CleanUpLobby, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.Bool, "disconnectSession", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false),
			new PropertyInfo(Variant.Type.Int, "error", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
		}, null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		if (method == MethodName._Ready && args.Count == 0)
		{
			_Ready();
			ret = default;
			return true;
		}
		if (method == MethodName.InitializeSingleplayer && args.Count == 0)
		{
			InitializeSingleplayer();
			ret = default;
			return true;
		}
		if (method == MethodName.InitCharacterButtons && args.Count == 0)
		{
			InitCharacterButtons();
			ret = default;
			return true;
		}
		if (method == MethodName._Process && args.Count == 1)
		{
			_Process(VariantUtils.ConvertTo<double>(in args[0]));
			ret = default;
			return true;
		}
		if (method == MethodName.AscensionChanged && args.Count == 0)
		{
			AscensionChanged();
			ret = default;
			return true;
		}
		if (method == MethodName.SeedChanged && args.Count == 0)
		{
			SeedChanged();
			ret = default;
			return true;
		}
		if (method == MethodName.ModifiersChanged && args.Count == 0)
		{
			ModifiersChanged();
			ret = default;
			return true;
		}
		if (method == MethodName.GameModeChanged && args.Count == 0)
		{
			GameModeChanged();
			ret = default;
			return true;
		}
		if (method == MethodName.MaxAscensionChanged && args.Count == 0)
		{
			MaxAscensionChanged();
			ret = default;
			return true;
		}
		if (method == MethodName.UpdatePreferredDreadCharacter && args.Count == 0)
		{
			UpdatePreferredDreadCharacter();
			ret = default;
			return true;
		}
		if (method == MethodName.IsBeginningRotation && args.Count == 0)
		{
			ret = VariantUtils.CreateFrom<bool>(IsBeginningRotation());
			return true;
		}
		if (method == MethodName.OnSubmenuClosed && args.Count == 0)
		{
			OnSubmenuClosed();
			ret = default;
			return true;
		}
		if (method == MethodName.SelectCharacter && args.Count == 1)
		{
			SelectCharacter(VariantUtils.ConvertTo<NCharacterSelectButton>(in args[0]));
			ret = default;
			return true;
		}
		if (method == MethodName.BeginRotationTickboxChanged && args.Count == 1)
		{
			BeginRotationTickboxChanged(VariantUtils.ConvertTo<bool>(in args[0]));
			ret = default;
			return true;
		}
		if (method == MethodName.OnEmbarkPressed && args.Count == 1)
		{
			OnEmbarkPressed(VariantUtils.ConvertTo<NButton>(in args[0]));
			ret = default;
			return true;
		}
		if (method == MethodName.AfterInitialized && args.Count == 0)
		{
			AfterInitialized();
			ret = default;
			return true;
		}
		if (method == MethodName.InitRotatingCharacterButton && args.Count == 0)
		{
			InitRotatingCharacterButton();
			ret = default;
			return true;
		}
		if (method == MethodName.OnUnreadyPressed && args.Count == 1)
		{
			OnUnreadyPressed(VariantUtils.ConvertTo<NButton>(in args[0]));
			ret = default;
			return true;
		}
		if (method == MethodName.OpenDreadInfo && args.Count == 1)
		{
			OpenDreadInfo(VariantUtils.ConvertTo<NButton>(in args[0]));
			ret = default;
			return true;
		}
		if (method == MethodName.UpdateRichPresence && args.Count == 0)
		{
			UpdateRichPresence();
			ret = default;
			return true;
		}
		if (method == MethodName.OnSubmenuOpened && args.Count == 0)
		{
			OnSubmenuOpened();
			ret = default;
			return true;
		}
		if (method == MethodName.RefreshDreadRecordsUi && args.Count == 0)
		{
			RefreshDreadRecordsUi();
			ret = default;
			return true;
		}
		if (method == MethodName.CleanUpLobby && args.Count == 2)
		{
			CleanUpLobby(VariantUtils.ConvertTo<bool>(in args[0]), VariantUtils.ConvertTo<NetError>(in args[1]));
			ret = default;
			return true;
		}
		return base.InvokeGodotClassMethod(in method, args, out ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassMethod(in godot_string_name method)
	{
		if (method == MethodName._Ready)
		{
			return true;
		}
		if (method == MethodName.InitializeSingleplayer)
		{
			return true;
		}
		if (method == MethodName.InitCharacterButtons)
		{
			return true;
		}
		if (method == MethodName._Process)
		{
			return true;
		}
		if (method == MethodName.AscensionChanged)
		{
			return true;
		}
		if (method == MethodName.SeedChanged)
		{
			return true;
		}
		if (method == MethodName.ModifiersChanged)
		{
			return true;
		}
		if (method == MethodName.GameModeChanged)
		{
			return true;
		}
		if (method == MethodName.MaxAscensionChanged)
		{
			return true;
		}
		if (method == MethodName.UpdatePreferredDreadCharacter)
		{
			return true;
		}
		if (method == MethodName.IsBeginningRotation)
		{
			return true;
		}
		if (method == MethodName.OnSubmenuClosed)
		{
			return true;
		}
		if (method == MethodName.SelectCharacter)
		{
			return true;
		}
		if (method == MethodName.BeginRotationTickboxChanged)
		{
			return true;
		}
		if (method == MethodName.OnEmbarkPressed)
		{
			return true;
		}
		if (method == MethodName.AfterInitialized)
		{
			return true;
		}
		if (method == MethodName.InitRotatingCharacterButton)
		{
			return true;
		}
		if (method == MethodName.OnUnreadyPressed)
		{
			return true;
		}
		if (method == MethodName.OpenDreadInfo)
		{
			return true;
		}
		if (method == MethodName.UpdateRichPresence)
		{
			return true;
		}
		if (method == MethodName.OnSubmenuOpened)
		{
			return true;
		}
		if (method == MethodName.RefreshDreadRecordsUi)
		{
			return true;
		}
		if (method == MethodName.CleanUpLobby)
		{
			return true;
		}
		return base.HasGodotClassMethod(in method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if (name == PropertyName._backButton)
		{
			_backButton = VariantUtils.ConvertTo<NBackButton>(in value);
			return true;
		}
		if (name == PropertyName._embarkButton)
		{
			_embarkButton = VariantUtils.ConvertTo<NConfirmButton>(in value);
			return true;
		}
		if (name == PropertyName._underConstruction)
		{
			_underConstruction = VariantUtils.ConvertTo<Control>(in value);
			return true;
		}
		if (name == PropertyName._nameText)
		{
			_nameText = VariantUtils.ConvertTo<NDreadNameText>(in value);
			return true;
		}
		if (name == PropertyName._selectedButton)
		{
			_selectedButton = VariantUtils.ConvertTo<NCharacterSelectButton>(in value);
			return true;
		}
		if (name == PropertyName._chainVisualization)
		{
			_chainVisualization = VariantUtils.ConvertTo<NChainVisualization>(in value);
			return true;
		}
		if (name == PropertyName._charButtonContainer)
		{
			_charButtonContainer = VariantUtils.ConvertTo<Control>(in value);
			return true;
		}
		if (name == PropertyName._readyAndWaitingContainer)
		{
			_readyAndWaitingContainer = VariantUtils.ConvertTo<Control>(in value);
			return true;
		}
		if (name == PropertyName._remotePlayerContainer)
		{
			_remotePlayerContainer = VariantUtils.ConvertTo<NRemoteLobbyPlayerContainer>(in value);
			return true;
		}
		if (name == PropertyName._rotatingCharacterButton)
		{
			_rotatingCharacterButton = VariantUtils.ConvertTo<NCharacterSelectButton>(in value);
			return true;
		}
		if (name == PropertyName._rotatingImage)
		{
			_rotatingImage = VariantUtils.ConvertTo<Control>(in value);
			return true;
		}
		if (name == PropertyName._startRotationTickbox)
		{
			_startRotationTickbox = VariantUtils.ConvertTo<NDreadBeginRotationTickbox>(in value);
			return true;
		}
		if (name == PropertyName._startRotationLabel)
		{
			_startRotationLabel = VariantUtils.ConvertTo<MegaRichTextLabel>(in value);
			return true;
		}
		if (name == PropertyName._unreadyButton)
		{
			_unreadyButton = VariantUtils.ConvertTo<NBackButton>(in value);
			return true;
		}
		if (name == PropertyName._dreadInfoButton)
		{
			_dreadInfoButton = VariantUtils.ConvertTo<NDreadInfoButton>(in value);
			return true;
		}
		if (name == PropertyName._bgContainer)
		{
			_bgContainer = VariantUtils.ConvertTo<Control>(in value);
			return true;
		}
		if (name == PropertyName._grayscaleBg)
		{
			_grayscaleBg = VariantUtils.ConvertTo<ShaderMaterial>(in value);
			return true;
		}
		if (name == PropertyName._scoreContainer)
		{
			_scoreContainer = VariantUtils.ConvertTo<Control>(in value);
			return true;
		}
		if (name == PropertyName._scoreLabels)
		{
			_scoreLabels = VariantUtils.ConvertTo<MegaRichTextLabel>(in value);
			return true;
		}
		if (name == PropertyName._scoreValues)
		{
			_scoreValues = VariantUtils.ConvertTo<MegaRichTextLabel>(in value);
			return true;
		}
		if (name == PropertyName._skipNextCharSelectLogic)
		{
			_skipNextCharSelectLogic = VariantUtils.ConvertTo<bool>(in value);
			return true;
		}
		if (name == PropertyName._doingChainAnimation)
		{
			_doingChainAnimation = VariantUtils.ConvertTo<bool>(in value);
			return true;
		}
		if (name == PropertyName._overriddenFocusedControl)
		{
			_overriddenFocusedControl = VariantUtils.ConvertTo<Control>(in value);
			return true;
		}
		return base.SetGodotClassPropertyValue(in name, in value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool GetGodotClassPropertyValue(in godot_string_name name, out godot_variant value)
	{
		if (name == PropertyName.InitialFocusedControl)
		{
			value = VariantUtils.CreateFrom<Control>(InitialFocusedControl);
			return true;
		}
		if (name == PropertyName._backButton)
		{
			value = VariantUtils.CreateFrom(in _backButton);
			return true;
		}
		if (name == PropertyName._embarkButton)
		{
			value = VariantUtils.CreateFrom(in _embarkButton);
			return true;
		}
		if (name == PropertyName._underConstruction)
		{
			value = VariantUtils.CreateFrom(in _underConstruction);
			return true;
		}
		if (name == PropertyName._nameText)
		{
			value = VariantUtils.CreateFrom(in _nameText);
			return true;
		}
		if (name == PropertyName._selectedButton)
		{
			value = VariantUtils.CreateFrom(in _selectedButton);
			return true;
		}
		if (name == PropertyName._chainVisualization)
		{
			value = VariantUtils.CreateFrom(in _chainVisualization);
			return true;
		}
		if (name == PropertyName._charButtonContainer)
		{
			value = VariantUtils.CreateFrom(in _charButtonContainer);
			return true;
		}
		if (name == PropertyName._readyAndWaitingContainer)
		{
			value = VariantUtils.CreateFrom(in _readyAndWaitingContainer);
			return true;
		}
		if (name == PropertyName._remotePlayerContainer)
		{
			value = VariantUtils.CreateFrom(in _remotePlayerContainer);
			return true;
		}
		if (name == PropertyName._rotatingCharacterButton)
		{
			value = VariantUtils.CreateFrom(in _rotatingCharacterButton);
			return true;
		}
		if (name == PropertyName._rotatingImage)
		{
			value = VariantUtils.CreateFrom(in _rotatingImage);
			return true;
		}
		if (name == PropertyName._startRotationTickbox)
		{
			value = VariantUtils.CreateFrom(in _startRotationTickbox);
			return true;
		}
		if (name == PropertyName._startRotationLabel)
		{
			value = VariantUtils.CreateFrom(in _startRotationLabel);
			return true;
		}
		if (name == PropertyName._unreadyButton)
		{
			value = VariantUtils.CreateFrom(in _unreadyButton);
			return true;
		}
		if (name == PropertyName._dreadInfoButton)
		{
			value = VariantUtils.CreateFrom(in _dreadInfoButton);
			return true;
		}
		if (name == PropertyName._bgContainer)
		{
			value = VariantUtils.CreateFrom(in _bgContainer);
			return true;
		}
		if (name == PropertyName._grayscaleBg)
		{
			value = VariantUtils.CreateFrom(in _grayscaleBg);
			return true;
		}
		if (name == PropertyName._scoreContainer)
		{
			value = VariantUtils.CreateFrom(in _scoreContainer);
			return true;
		}
		if (name == PropertyName._scoreLabels)
		{
			value = VariantUtils.CreateFrom(in _scoreLabels);
			return true;
		}
		if (name == PropertyName._scoreValues)
		{
			value = VariantUtils.CreateFrom(in _scoreValues);
			return true;
		}
		if (name == PropertyName._skipNextCharSelectLogic)
		{
			value = VariantUtils.CreateFrom(in _skipNextCharSelectLogic);
			return true;
		}
		if (name == PropertyName._doingChainAnimation)
		{
			value = VariantUtils.CreateFrom(in _doingChainAnimation);
			return true;
		}
		if (name == PropertyName._overriddenFocusedControl)
		{
			value = VariantUtils.CreateFrom(in _overriddenFocusedControl);
			return true;
		}
		return base.GetGodotClassPropertyValue(in name, out value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal new static List<PropertyInfo> GetGodotPropertyList()
	{
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._backButton, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._embarkButton, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._underConstruction, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._nameText, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._selectedButton, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._chainVisualization, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._charButtonContainer, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._readyAndWaitingContainer, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._remotePlayerContainer, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._rotatingCharacterButton, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._rotatingImage, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._startRotationTickbox, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._startRotationLabel, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._unreadyButton, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._dreadInfoButton, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._bgContainer, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._grayscaleBg, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._scoreContainer, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._scoreLabels, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._scoreValues, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Bool, PropertyName._skipNextCharSelectLogic, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Bool, PropertyName._doingChainAnimation, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._overriddenFocusedControl, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName.InitialFocusedControl, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		base.SaveGodotObjectData(info);
		info.AddProperty(PropertyName._backButton, Variant.From(in _backButton));
		info.AddProperty(PropertyName._embarkButton, Variant.From(in _embarkButton));
		info.AddProperty(PropertyName._underConstruction, Variant.From(in _underConstruction));
		info.AddProperty(PropertyName._nameText, Variant.From(in _nameText));
		info.AddProperty(PropertyName._selectedButton, Variant.From(in _selectedButton));
		info.AddProperty(PropertyName._chainVisualization, Variant.From(in _chainVisualization));
		info.AddProperty(PropertyName._charButtonContainer, Variant.From(in _charButtonContainer));
		info.AddProperty(PropertyName._readyAndWaitingContainer, Variant.From(in _readyAndWaitingContainer));
		info.AddProperty(PropertyName._remotePlayerContainer, Variant.From(in _remotePlayerContainer));
		info.AddProperty(PropertyName._rotatingCharacterButton, Variant.From(in _rotatingCharacterButton));
		info.AddProperty(PropertyName._rotatingImage, Variant.From(in _rotatingImage));
		info.AddProperty(PropertyName._startRotationTickbox, Variant.From(in _startRotationTickbox));
		info.AddProperty(PropertyName._startRotationLabel, Variant.From(in _startRotationLabel));
		info.AddProperty(PropertyName._unreadyButton, Variant.From(in _unreadyButton));
		info.AddProperty(PropertyName._dreadInfoButton, Variant.From(in _dreadInfoButton));
		info.AddProperty(PropertyName._bgContainer, Variant.From(in _bgContainer));
		info.AddProperty(PropertyName._grayscaleBg, Variant.From(in _grayscaleBg));
		info.AddProperty(PropertyName._scoreContainer, Variant.From(in _scoreContainer));
		info.AddProperty(PropertyName._scoreLabels, Variant.From(in _scoreLabels));
		info.AddProperty(PropertyName._scoreValues, Variant.From(in _scoreValues));
		info.AddProperty(PropertyName._skipNextCharSelectLogic, Variant.From(in _skipNextCharSelectLogic));
		info.AddProperty(PropertyName._doingChainAnimation, Variant.From(in _doingChainAnimation));
		info.AddProperty(PropertyName._overriddenFocusedControl, Variant.From(in _overriddenFocusedControl));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		if (info.TryGetProperty(PropertyName._backButton, out var value))
		{
			_backButton = value.As<NBackButton>();
		}
		if (info.TryGetProperty(PropertyName._embarkButton, out var value2))
		{
			_embarkButton = value2.As<NConfirmButton>();
		}
		if (info.TryGetProperty(PropertyName._underConstruction, out var value3))
		{
			_underConstruction = value3.As<Control>();
		}
		if (info.TryGetProperty(PropertyName._nameText, out var value4))
		{
			_nameText = value4.As<NDreadNameText>();
		}
		if (info.TryGetProperty(PropertyName._selectedButton, out var value5))
		{
			_selectedButton = value5.As<NCharacterSelectButton>();
		}
		if (info.TryGetProperty(PropertyName._chainVisualization, out var value6))
		{
			_chainVisualization = value6.As<NChainVisualization>();
		}
		if (info.TryGetProperty(PropertyName._charButtonContainer, out var value7))
		{
			_charButtonContainer = value7.As<Control>();
		}
		if (info.TryGetProperty(PropertyName._readyAndWaitingContainer, out var value8))
		{
			_readyAndWaitingContainer = value8.As<Control>();
		}
		if (info.TryGetProperty(PropertyName._remotePlayerContainer, out var value9))
		{
			_remotePlayerContainer = value9.As<NRemoteLobbyPlayerContainer>();
		}
		if (info.TryGetProperty(PropertyName._rotatingCharacterButton, out var value10))
		{
			_rotatingCharacterButton = value10.As<NCharacterSelectButton>();
		}
		if (info.TryGetProperty(PropertyName._rotatingImage, out var value11))
		{
			_rotatingImage = value11.As<Control>();
		}
		if (info.TryGetProperty(PropertyName._startRotationTickbox, out var value12))
		{
			_startRotationTickbox = value12.As<NDreadBeginRotationTickbox>();
		}
		if (info.TryGetProperty(PropertyName._startRotationLabel, out var value13))
		{
			_startRotationLabel = value13.As<MegaRichTextLabel>();
		}
		if (info.TryGetProperty(PropertyName._unreadyButton, out var value14))
		{
			_unreadyButton = value14.As<NBackButton>();
		}
		if (info.TryGetProperty(PropertyName._dreadInfoButton, out var value15))
		{
			_dreadInfoButton = value15.As<NDreadInfoButton>();
		}
		if (info.TryGetProperty(PropertyName._bgContainer, out var value16))
		{
			_bgContainer = value16.As<Control>();
		}
		if (info.TryGetProperty(PropertyName._grayscaleBg, out var value17))
		{
			_grayscaleBg = value17.As<ShaderMaterial>();
		}
		if (info.TryGetProperty(PropertyName._scoreContainer, out var value18))
		{
			_scoreContainer = value18.As<Control>();
		}
		if (info.TryGetProperty(PropertyName._scoreLabels, out var value19))
		{
			_scoreLabels = value19.As<MegaRichTextLabel>();
		}
		if (info.TryGetProperty(PropertyName._scoreValues, out var value20))
		{
			_scoreValues = value20.As<MegaRichTextLabel>();
		}
		if (info.TryGetProperty(PropertyName._skipNextCharSelectLogic, out var value21))
		{
			_skipNextCharSelectLogic = value21.As<bool>();
		}
		if (info.TryGetProperty(PropertyName._doingChainAnimation, out var value22))
		{
			_doingChainAnimation = value22.As<bool>();
		}
		if (info.TryGetProperty(PropertyName._overriddenFocusedControl, out var value23))
		{
			_overriddenFocusedControl = value23.As<Control>();
		}
	}
}
