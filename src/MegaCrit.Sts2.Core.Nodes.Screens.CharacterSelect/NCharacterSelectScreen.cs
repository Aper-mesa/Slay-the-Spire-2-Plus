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
using MegaCrit.Sts2.Core.Entities.UI;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Multiplayer;
using MegaCrit.Sts2.Core.Multiplayer.Game;
using MegaCrit.Sts2.Core.Multiplayer.Game.Lobby;
using MegaCrit.Sts2.Core.Multiplayer.Messages.Lobby;
using MegaCrit.Sts2.Core.Nodes.Audio;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.Ftue;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Multiplayer;
using MegaCrit.Sts2.Core.Nodes.Screens.MainMenu;
using MegaCrit.Sts2.Core.Nodes.Vfx.Ui;
using MegaCrit.Sts2.Core.Nodes.Vfx.Utilities;
using MegaCrit.Sts2.Core.Platform;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Saves;
using MegaCrit.Sts2.Core.TestSupport;
using MegaCrit.Sts2.Core.Unlocks;
using MegaCrit.Sts2.addons.mega_text;

namespace MegaCrit.Sts2.Core.Nodes.Screens.CharacterSelect;

[ScriptPath("res://src/Core/Nodes/Screens/CharacterSelect/NCharacterSelectScreen.cs")]
public class NCharacterSelectScreen : NSubmenu, IStartRunLobbyListener, ICharacterSelectButtonDelegate
{
	public new class MethodName : NSubmenu.MethodName
	{
		public new static readonly StringName _Ready = "_Ready";

		public static readonly StringName CheckForMultiplayerAscensionPopup = "CheckForMultiplayerAscensionPopup";

		public static readonly StringName InitializeSingleplayer = "InitializeSingleplayer";

		public static readonly StringName InitCharacterButtons = "InitCharacterButtons";

		public static readonly StringName UpdateRandomCharacterVisibility = "UpdateRandomCharacterVisibility";

		public new static readonly StringName _Input = "_Input";

		public static readonly StringName DebugUnlockAllCharacters = "DebugUnlockAllCharacters";

		public new static readonly StringName OnSubmenuOpened = "OnSubmenuOpened";

		public static readonly StringName GetPreferredCharacterButton = "GetPreferredCharacterButton";

		public static readonly StringName UpdatePreferredStandardCharacter = "UpdatePreferredStandardCharacter";

		public new static readonly StringName OnSubmenuClosed = "OnSubmenuClosed";

		public static readonly StringName OnEmbarkPressed = "OnEmbarkPressed";

		public new static readonly StringName _Process = "_Process";

		public static readonly StringName CleanUpLobby = "CleanUpLobby";

		public static readonly StringName SelectCharacter = "SelectCharacter";

		public static readonly StringName OnAscensionPanelLevelChanged = "OnAscensionPanelLevelChanged";

		public static readonly StringName OnUnreadyPressed = "OnUnreadyPressed";

		public static readonly StringName UpdateRichPresence = "UpdateRichPresence";

		public static readonly StringName MaxAscensionChanged = "MaxAscensionChanged";

		public static readonly StringName AscensionChanged = "AscensionChanged";

		public static readonly StringName SeedChanged = "SeedChanged";

		public static readonly StringName ModifiersChanged = "ModifiersChanged";

		public static readonly StringName GameModeChanged = "GameModeChanged";

		public static readonly StringName AfterInitialized = "AfterInitialized";

		public static readonly StringName TryShowAscensionSinglePlayerFtue = "TryShowAscensionSinglePlayerFtue";
	}

	public new class PropertyName : NSubmenu.PropertyName
	{
		public new static readonly StringName InitialFocusedControl = "InitialFocusedControl";

		public static readonly StringName ShouldShowActDropdown = "ShouldShowActDropdown";

		public static readonly StringName _name = "_name";

		public static readonly StringName _infoPanel = "_infoPanel";

		public static readonly StringName _description = "_description";

		public static readonly StringName _hp = "_hp";

		public static readonly StringName _gold = "_gold";

		public static readonly StringName _relicTitle = "_relicTitle";

		public static readonly StringName _relicDescription = "_relicDescription";

		public static readonly StringName _relicIcon = "_relicIcon";

		public static readonly StringName _relicIconOutline = "_relicIconOutline";

		public static readonly StringName _selectedButton = "_selectedButton";

		public static readonly StringName _charButtonContainer = "_charButtonContainer";

		public static readonly StringName _bgContainer = "_bgContainer";

		public static readonly StringName _readyAndWaitingContainer = "_readyAndWaitingContainer";

		public new static readonly StringName _backButton = "_backButton";

		public static readonly StringName _unreadyButton = "_unreadyButton";

		public static readonly StringName _embarkButton = "_embarkButton";

		public static readonly StringName _ascensionPanel = "_ascensionPanel";

		public static readonly StringName _actDropdown = "_actDropdown";

		public static readonly StringName _actDropdownLabel = "_actDropdownLabel";

		public static readonly StringName _remotePlayerContainer = "_remotePlayerContainer";

		public static readonly StringName _characterUnlockAnimationBackstop = "_characterUnlockAnimationBackstop";

		public static readonly StringName _randomCharacterButton = "_randomCharacterButton";

		public static readonly StringName _infoPanelTween = "_infoPanelTween";

		public static readonly StringName _infoPanelPosFinalVal = "_infoPanelPosFinalVal";

		public static readonly StringName _charSelectButtonScene = "_charSelectButtonScene";
	}

	public new class SignalName : NSubmenu.SignalName
	{
	}

	private static readonly string _scenePath = SceneHelper.GetScenePath("screens/character_select_screen");

	private MegaLabel _name;

	private Control _infoPanel;

	private MegaRichTextLabel _description;

	private MegaLabel _hp;

	private MegaLabel _gold;

	private MegaRichTextLabel _relicTitle;

	private MegaRichTextLabel _relicDescription;

	private TextureRect _relicIcon;

	private TextureRect _relicIconOutline;

	private NCharacterSelectButton? _selectedButton;

	private Control _charButtonContainer;

	private Control _bgContainer;

	private Control _readyAndWaitingContainer;

	private NBackButton _backButton;

	private NBackButton _unreadyButton;

	private NConfirmButton _embarkButton;

	private NAscensionPanel _ascensionPanel;

	private NActDropdown _actDropdown;

	private MegaRichTextLabel _actDropdownLabel;

	private NRemoteLobbyPlayerContainer _remotePlayerContainer;

	private Control _characterUnlockAnimationBackstop;

	private NCharacterSelectButton _randomCharacterButton;

	private Tween? _infoPanelTween;

	private Vector2 _infoPanelPosFinalVal;

	private const string _sceneCharSelectButtonPath = "res://scenes/screens/char_select/char_select_button.tscn";

	[Export(PropertyHint.None, "")]
	private PackedScene _charSelectButtonScene;

	private StartRunLobby _lobby;

	public StartRunLobby Lobby => _lobby;

	public static IEnumerable<string> AssetPaths
	{
		get
		{
			List<string> list = new List<string>();
			list.Add(_scenePath);
			list.Add("res://scenes/screens/char_select/char_select_button.tscn");
			list.AddRange(NCharacterSelectButton.AssetPaths);
			return new _003C_003Ez__ReadOnlyList<string>(list);
		}
	}

	protected override Control InitialFocusedControl => _selectedButton ?? _charButtonContainer.GetChild<Control>(0);

	private bool ShouldShowActDropdown => false;

	public override void _Ready()
	{
		ConnectSignals();
		_infoPanel = GetNode<Control>("InfoPanel");
		_name = GetNode<MegaLabel>("InfoPanel/VBoxContainer/Name");
		_description = GetNode<MegaRichTextLabel>("InfoPanel/VBoxContainer/DescriptionLabel");
		_hp = GetNode<MegaLabel>("InfoPanel/VBoxContainer/HpGoldSpacer/HpGold/Hp/Label");
		_gold = GetNode<MegaLabel>("InfoPanel/VBoxContainer/HpGoldSpacer/HpGold/Gold/Label");
		_relicTitle = GetNode<MegaRichTextLabel>("InfoPanel/VBoxContainer/Relic/Name/RichTextLabel");
		_relicDescription = GetNode<MegaRichTextLabel>("InfoPanel/VBoxContainer/Relic/Description");
		_relicIcon = GetNode<TextureRect>("InfoPanel/VBoxContainer/Relic/Icon");
		_relicIconOutline = GetNode<TextureRect>("InfoPanel/VBoxContainer/Relic/Icon/Outline");
		_bgContainer = GetNode<Control>("AnimatedBg");
		_charButtonContainer = GetNode<Control>("CharSelectButtons/ButtonContainer");
		_ascensionPanel = GetNode<NAscensionPanel>("%AscensionPanel");
		_actDropdown = GetNode<NActDropdown>("%ActDropdown");
		_actDropdownLabel = GetNode<MegaRichTextLabel>("ActLabel");
		_remotePlayerContainer = GetNode<NRemoteLobbyPlayerContainer>("RemotePlayerContainer");
		_readyAndWaitingContainer = GetNode<Control>("ReadyAndWaitingPanel");
		GetNode<MegaRichTextLabel>("%WaitingForPlayers").Text = new LocString("main_menu_ui", "CHARACTER_SELECT.waitingForPlayers").GetFormattedText();
		_characterUnlockAnimationBackstop = GetNode<Control>("%CharacterUnlockAnimationBackstop");
		_backButton = GetNode<NBackButton>("BackButton");
		_unreadyButton = GetNode<NBackButton>("UnreadyButton");
		_embarkButton = GetNode<NConfirmButton>("ConfirmButton");
		_embarkButton.OverrideHotkeys(new string[1] { MegaInput.select });
		_embarkButton.Connect(NClickableControl.SignalName.Released, Callable.From<NButton>(OnEmbarkPressed));
		_ascensionPanel.Connect(NAscensionPanel.SignalName.AscensionLevelChanged, Callable.From(OnAscensionPanelLevelChanged));
		_unreadyButton.Connect(NClickableControl.SignalName.Released, Callable.From<NButton>(OnUnreadyPressed));
		_unreadyButton.Disable();
		InitCharacterButtons();
	}

	public void InitializeMultiplayerAsHost(INetGameService gameService, int maxPlayers)
	{
		if (gameService.Type != NetGameType.Host)
		{
			throw new InvalidOperationException($"Initialized character select screen with GameService of type {gameService.Type} when hosting!");
		}
		_lobby = new StartRunLobby(GameMode.Standard, isCustom: false, gameService, this, maxPlayers);
		_ascensionPanel.Initialize(MultiplayerUiMode.Host, GameMode.Standard);
		_lobby.AddLocalHostPlayer(new UnlockState(SaveManager.Instance.Progress), SaveManager.Instance.Progress.MaxMultiplayerAscension, SaveManager.Instance.Progress.MaxBlitzAscension);
		OnAscensionPanelLevelChanged();
		CheckForMultiplayerAscensionPopup();
		AfterInitialized();
	}

	public void InitializeMultiplayerAsClient(INetGameService gameService, ClientLobbyJoinResponseMessage message)
	{
		if (gameService.Type != NetGameType.Client)
		{
			throw new InvalidOperationException($"Initialized character select screen with GameService of type {gameService.Type} when joining!");
		}
		_lobby = new StartRunLobby(GameMode.Standard, isCustom: false, gameService, this, -1);
		_ascensionPanel.Initialize(MultiplayerUiMode.Client, GameMode.Standard);
		_lobby.InitializeFromMessage(message);
		CheckForMultiplayerAscensionPopup();
		AfterInitialized();
	}

	private void CheckForMultiplayerAscensionPopup()
	{
		if (SaveManager.Instance.Progress.MaxMultiplayerAscension > 0 && !SaveManager.Instance.SeenPopup("ascension_multiplayer_ftue"))
		{
			NAscensionMultiplayerFtue nAscensionMultiplayerFtue = NAscensionMultiplayerFtue.Create();
			if (nAscensionMultiplayerFtue != null)
			{
				NModalContainer.Instance.Add(nAscensionMultiplayerFtue);
			}
		}
	}

	public void InitializeSingleplayer()
	{
		_lobby = new StartRunLobby(GameMode.Standard, isCustom: false, new NetSingleplayerGameService(), this, 1);
		_ascensionPanel.Initialize(MultiplayerUiMode.Singleplayer, GameMode.Standard);
		_lobby.AddLocalHostPlayer(new UnlockState(SaveManager.Instance.Progress), 0, SaveManager.Instance.Progress.MaxBlitzAscension);
		AfterInitialized();
	}

	private void InitCharacterButtons()
	{
		foreach (CharacterModel allCharacter in ModelDb.AllCharacters)
		{
			NCharacterSelectButton nCharacterSelectButton = _charSelectButtonScene.Instantiate<NCharacterSelectButton>(PackedScene.GenEditState.Disabled);
			_charButtonContainer.AddChildSafely(nCharacterSelectButton);
			nCharacterSelectButton.Init(allCharacter, this);
		}
		_randomCharacterButton = _charSelectButtonScene.Instantiate<NCharacterSelectButton>(PackedScene.GenEditState.Disabled);
		_charButtonContainer.AddChildSafely(_randomCharacterButton);
		_randomCharacterButton.InitAsRandom(this);
		UpdateRandomCharacterVisibility();
		List<NCharacterSelectButton> list = (from c in _charButtonContainer.GetChildren().OfType<NCharacterSelectButton>()
			where c.Visible
			select c).ToList();
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

	private void UpdateRandomCharacterVisibility()
	{
		if (_lobby == null)
		{
			return;
		}
		bool visible = false;
		foreach (StartRunLobbyPlayer player in _lobby.Players)
		{
			UnlockState unlockState = UnlockState.FromSerializable(player.unlockState);
			bool flag = true;
			foreach (CharacterModel allCharacter in ModelDb.AllCharacters)
			{
				if (!unlockState.Characters.Contains(allCharacter))
				{
					flag = false;
					break;
				}
			}
			if (flag)
			{
				visible = true;
				break;
			}
		}
		_randomCharacterButton.Visible = visible;
	}

	public override void _Input(InputEvent inputEvent)
	{
		if (inputEvent.IsActionReleased(DebugHotkey.unlockCharacters))
		{
			DebugUnlockAllCharacters();
		}
	}

	private void DebugUnlockAllCharacters()
	{
		foreach (NCharacterSelectButton item in _charButtonContainer.GetChildren().OfType<NCharacterSelectButton>())
		{
			item.DebugUnlock();
		}
	}

	public override void OnSubmenuOpened()
	{
		base.OnSubmenuOpened();
		foreach (NCharacterSelectButton item in _charButtonContainer.GetChildren().OfType<NCharacterSelectButton>())
		{
			if (!item.IsLocked)
			{
				item.Enable();
			}
			else
			{
				item.UnlockIfPossible();
			}
			item.Reset();
		}
		_embarkButton.Enable();
		if (SaveManager.Instance.Progress.PendingCharacterUnlock == ModelId.none)
		{
			(GetPreferredCharacterButton() ?? _charButtonContainer.GetChild<NCharacterSelectButton>(0)).Select();
		}
		else
		{
			TaskHelper.RunSafely(PlayUnlockCharacterAnimation(SaveManager.Instance.Progress.PendingCharacterUnlock));
		}
		_remotePlayerContainer.Visible = _lobby.NetService.Type != NetGameType.Singleplayer;
		_remotePlayerContainer.Initialize(_lobby, displayLocalPlayer: true);
		if (_lobby.NetService.Type == NetGameType.Client)
		{
			_ascensionPanel.SetAscensionLevel(_lobby.Ascension);
		}
		_actDropdown.Visible = ShouldShowActDropdown;
		_actDropdownLabel.Visible = _actDropdown.Visible;
		_readyAndWaitingContainer.Visible = false;
		foreach (StartRunLobbyPlayer player in _lobby.Players)
		{
			RefreshButtonSelectionForPlayer(player);
		}
	}

	private async Task PlayUnlockCharacterAnimation(ModelId character)
	{
		await this.AwaitProcessFrame();
		_backButton.Disable();
		_embarkButton.Disable();
		_infoPanel.Visible = false;
		_characterUnlockAnimationBackstop.Visible = true;
		foreach (NCharacterSelectButton button in _charButtonContainer.GetChildren().OfType<NCharacterSelectButton>())
		{
			if (button.Character.Id == character)
			{
				button.LockForAnimation();
				await Cmd.Wait(0.3f);
				await button.AnimateUnlock();
				button.Select();
			}
		}
		_infoPanel.Visible = true;
		_characterUnlockAnimationBackstop.Visible = false;
		_backButton.Enable();
		_embarkButton.Enable();
		SaveManager.Instance.Progress.PendingCharacterUnlock = ModelId.none;
		TryShowAscensionSinglePlayerFtue();
	}

	private NCharacterSelectButton? GetPreferredCharacterButton()
	{
		ModelId preferredCharacterId = ((_lobby.NetService.Type == NetGameType.Singleplayer) ? SaveManager.Instance.Progress.PreferredStandardCharacter : SaveManager.Instance.Progress.PreferredMultiplayerStandardCharacter);
		if (preferredCharacterId == ModelId.none)
		{
			return null;
		}
		return _charButtonContainer.GetChildren().OfType<NCharacterSelectButton>().FirstOrDefault((NCharacterSelectButton button) => !button.IsLocked && button.Visible && button.Character.Id == preferredCharacterId);
	}

	private void UpdatePreferredStandardCharacter()
	{
		if (_selectedButton == null)
		{
			return;
		}
		ModelId id = _selectedButton.Character.Id;
		ProgressState progress = SaveManager.Instance.Progress;
		if (_lobby.NetService.Type == NetGameType.Singleplayer)
		{
			if (progress.PreferredStandardCharacter == id)
			{
				return;
			}
			Log.Info($"Setting preferred Standard character to {id}");
			progress.PreferredStandardCharacter = id;
		}
		else
		{
			if (progress.PreferredMultiplayerStandardCharacter == id)
			{
				return;
			}
			Log.Info($"Setting preferred multiplayer Standard character to {id}");
			progress.PreferredMultiplayerStandardCharacter = id;
		}
		SaveManager.Instance.SaveProgressFile();
	}

	public override void OnSubmenuClosed()
	{
		base.OnSubmenuClosed();
		_embarkButton.Disable();
		_remotePlayerContainer.Cleanup();
		_ascensionPanel.Cleanup();
		if (_lobby.NetService.Type.IsMultiplayer())
		{
			PlatformUtil.SetRichPresence("MAIN_MENU", null, null);
		}
		CleanUpLobby(disconnectSession: true);
	}

	private void OnEmbarkPressed(NButton _)
	{
		_embarkButton.Disable();
		if (!SaveManager.Instance.SeenFtue("accept_tutorials_ftue"))
		{
			NModalContainer.Instance.Add(NAcceptTutorialsFtue.Create(this, () =>
			{
				OnEmbarkPressed(null);
			}));
			return;
		}
		NetGameType type = _lobby.NetService.Type;
		if ((uint)(type - 1) <= 1u)
		{
			_lobby.Act1 = _actDropdown.CurrentOption;
		}
		_lobby.SetReady(ready: true);
		foreach (NCharacterSelectButton item in _charButtonContainer.GetChildren().OfType<NCharacterSelectButton>())
		{
			item.Disable();
		}
		_backButton.Disable();
		if (_lobby.NetService.Type.IsMultiplayer() && !_lobby.IsAboutToBeginGame())
		{
			_readyAndWaitingContainer.Visible = true;
			_unreadyButton.Enable();
		}
	}

	public override void _Process(double delta)
	{
		if (_lobby != null && _lobby.NetService.IsConnected)
		{
			_lobby.NetService.Update();
		}
	}

	private void CleanUpLobby(bool disconnectSession, NetError error = NetError.Quit)
	{
		_lobby.PlayerFailedToConnect -= RemoteClientFailedToConnectToLocalHost;
		_lobby.CleanUp(disconnectSession, error);
		_lobby = null;
	}

	private async Task DoRandomCharacterChange(Player player)
	{
		NGame.Instance?.ScreenShake(ShakeStrength.Weak, ShakeDuration.Short, 90f);
		SfxCmd.Play(player.Character.CharacterSelectSfx);
		Control child = player.Character.CreateCharacterSelectBackground();
		_bgContainer.AddChildSafely(child);
		await Cmd.Wait(1f);
	}

	private async Task StartNewSingleplayerRun(string seed, List<ActModel> acts)
	{
		Log.Info($"Embarking on a singleplayer {_lobby.LocalPlayer.character.Id.Entry} run. Ascension: {_lobby.Ascension} Seed: {seed}");
		Player player = _lobby.GeneratePlayers(seed)[0];
		int ascensionToEmbark = _lobby.Ascension;
		if (_randomCharacterButton.IsSelected)
		{
			await DoRandomCharacterChange(player);
		}
		try
		{
			SfxCmd.Play(player.Character.CharacterTransitionSfx);
			await NGame.Instance.Transition.FadeOut(0.8f, player.Character.CharacterSelectTransitionPath);
			await NGame.Instance.StartNewSingleplayerRun(player, shouldSave: true, acts, Array.Empty<ModifierModel>(), seed, GameMode.Standard, isCustomGame: false, ascensionToEmbark);
		}
		catch (Exception ex)
		{
			Log.Error($"Exception starting singleplayer run : {ex}");
			CleanUpLobby(disconnectSession: true, NetError.InternalError);
			await NGame.Instance.ReturnToMainMenuWithInternalError(ex);
			return;
		}
		CleanUpLobby(disconnectSession: false);
	}

	private async Task StartNewMultiplayerRun(string seed, List<ActModel> acts)
	{
		Log.Info($"Embarking on a multiplayer run. Players: {string.Join(",", _lobby.Players)}. Ascension: {_lobby.Ascension} Seed: {seed}");
		List<Player> players = _lobby.GeneratePlayers(seed);
		Player localPlayer = players.First((Player p) => p.NetId == _lobby.LocalPlayer.id);
		if (_randomCharacterButton.IsSelected)
		{
			await DoRandomCharacterChange(localPlayer);
		}
		SfxCmd.Play(localPlayer.Character.CharacterTransitionSfx);
		await NGame.Instance.Transition.FadeOut(0.8f, localPlayer.Character.CharacterSelectTransitionPath);
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
				await NGame.Instance.StartNewMultiplayerRun(players, _lobby, shouldSave: true, acts, Array.Empty<ModifierModel>(), seed, _lobby.Ascension);
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

	private void RemoteClientFailedToConnectToLocalHost(ulong sender, NetErrorInfo info)
	{
		string formattedText = NErrorPopup.LocStringFromNetError(info, out var _).GetFormattedText();
		LocString locString = new LocString("main_menu_ui", "NETWORK_ERROR.HOST.PREFIX.body");
		locString.Add("playerName", PlatformUtil.GetPlayerName(_lobby.NetService.Platform, sender));
		locString.Add("info", formattedText);
		this.AddChildSafely(NFailedJoinVfx.Create(locString.GetFormattedText()));
	}

	public void SelectCharacter(NCharacterSelectButton charSelectButton)
	{
		CharacterModel character = charSelectButton.Character;
		if (!charSelectButton.IsRandom)
		{
			SfxCmd.Play(character.CharacterSelectSfx);
		}
		NGame.Instance?.ScreenShake(ShakeStrength.Weak, ShakeDuration.Short, 90f);
		if (_infoPanelTween != null)
		{
			_infoPanel.Position = _infoPanelPosFinalVal;
		}
		_infoPanelPosFinalVal = _infoPanel.Position;
		_infoPanelTween?.Kill();
		_infoPanelTween = CreateTween().SetParallel();
		_infoPanelTween.TweenProperty(_infoPanel, "position", _infoPanel.Position, 0.5).SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Expo)
			.From(_infoPanel.Position - new Vector2(300f, 0f));
		foreach (Node child in _bgContainer.GetChildren())
		{
			_bgContainer.RemoveChildSafely(child);
			child.QueueFreeSafely();
		}
		_selectedButton = charSelectButton;
		if (!charSelectButton.IsLocked)
		{
			_embarkButton.Enable();
			Control control = character.CreateCharacterSelectBackground();
			control.Name = character.Id.Entry + "_bg";
			_bgContainer.AddChildSafely(control);
			string formattedText = new LocString("characters", character.CharacterSelectTitle).GetFormattedText();
			_name.SetTextAutoSize(formattedText);
			_description.Text = new LocString("characters", character.CharacterSelectDesc).GetFormattedText();
			if (!_selectedButton.IsRandom)
			{
				_hp.SetTextAutoSize($"{character.StartingHp}/{character.StartingHp}");
				_gold.SetTextAutoSize($"{character.StartingGold}");
				RelicModel relicModel = character.StartingRelics[0];
				_relicTitle.Text = relicModel.Title.GetFormattedText();
				_relicDescription.Text = relicModel.DynamicDescription.GetFormattedText();
				_relicIcon.Texture = relicModel.Icon;
				_relicIconOutline.Texture = relicModel.IconOutline;
				_relicIcon.SelfModulate = Colors.White;
				_relicIconOutline.SelfModulate = StsColors.halfTransparentBlack;
			}
			else
			{
				_hp.SetTextAutoSize("??/??");
				_gold.SetTextAutoSize("???");
				_relicIcon.SelfModulate = StsColors.transparentBlack;
				_relicIconOutline.SelfModulate = StsColors.transparentBlack;
				_relicTitle.Text = string.Empty;
				_relicDescription.Text = string.Empty;
			}
			_lobby.SetLocalCharacter(character);
			if (!_lobby.NetService.Type.IsMultiplayer())
			{
				_ascensionPanel.AnimIn();
				if (SaveManager.Instance.Progress.PendingCharacterUnlock == ModelId.none)
				{
					TryShowAscensionSinglePlayerFtue();
				}
			}
		}
		else
		{
			_embarkButton.Disable();
			string formattedText2 = new LocString("main_menu_ui", "CHARACTER_SELECT.locked.title").GetFormattedText();
			_name.SetTextAutoSize(formattedText2);
			_description.Text = character.GetUnlockText().GetFormattedText();
			_hp.SetTextAutoSize("??/??");
			_gold.SetTextAutoSize("???");
			if (!_selectedButton.IsRandom)
			{
				RelicModel relicModel2 = character.StartingRelics[0];
				_relicTitle.Text = new LocString("main_menu_ui", "CHARACTER_SELECT.lockedRelic.title").GetFormattedText();
				_relicDescription.Text = new LocString("main_menu_ui", "CHARACTER_SELECT.lockedRelic.description").GetFormattedText();
				_relicIcon.Texture = relicModel2.Icon;
				_relicIconOutline.Texture = relicModel2.IconOutline;
				_relicIcon.SelfModulate = StsColors.ninetyPercentBlack;
				_relicIconOutline.SelfModulate = StsColors.halfTransparentWhite;
			}
			else
			{
				_relicIcon.SelfModulate = StsColors.transparentBlack;
				_relicIconOutline.SelfModulate = StsColors.transparentBlack;
				_relicTitle.Text = string.Empty;
				_relicDescription.Text = string.Empty;
			}
			_ascensionPanel.Visible = false;
		}
		foreach (NCharacterSelectButton item in _charButtonContainer.GetChildren().OfType<NCharacterSelectButton>())
		{
			if (item != _selectedButton)
			{
				item.Deselect();
			}
		}
	}

	private void OnAscensionPanelLevelChanged()
	{
		if (_lobby.NetService.Type != NetGameType.Client && _lobby.Ascension != _ascensionPanel.Ascension)
		{
			_lobby.SyncAscensionChange(_ascensionPanel.Ascension);
		}
	}

	private void OnUnreadyPressed(NButton _)
	{
		_lobby.SetReady(ready: false);
		foreach (NCharacterSelectButton item in _charButtonContainer.GetChildren().OfType<NCharacterSelectButton>())
		{
			item.Enable();
		}
		_selectedButton?.TryGrabFocus();
		_readyAndWaitingContainer.Visible = false;
		_embarkButton.Enable();
		_backButton.Enable();
		_unreadyButton.Disable();
	}

	private void UpdateRichPresence()
	{
		if (_lobby.NetService.Type.IsMultiplayer())
		{
			PlatformUtil.SetRichPresence("STANDARD_MP_LOBBY", _lobby.NetService.GetRawLobbyIdentifier(), _lobby.Players.Count);
		}
	}

	public void MaxAscensionChanged()
	{
		_ascensionPanel.SetMaxAscension(_lobby.MaxAscension);
	}

	public void PlayerConnected(StartRunLobbyPlayer player)
	{
		_remotePlayerContainer.OnPlayerConnected(player);
		RefreshButtonSelectionForPlayer(player);
		UpdateRichPresence();
		UpdateRandomCharacterVisibility();
	}

	public void PlayerChanged(StartRunLobbyPlayer player)
	{
		_remotePlayerContainer.OnPlayerChanged(player);
		RefreshButtonSelectionForPlayer(player);
	}

	private void RefreshButtonSelectionForPlayer(StartRunLobbyPlayer player)
	{
		if (player.id == _lobby.LocalPlayer.id)
		{
			return;
		}
		foreach (NCharacterSelectButton item in _charButtonContainer.GetChildren().OfType<NCharacterSelectButton>())
		{
			if (item.RemoteSelectedPlayers.Contains(player.id) && player.character != item.Character)
			{
				item.OnRemotePlayerDeselected(player.id);
			}
			else if (player.character == item.Character)
			{
				item.OnRemotePlayerSelected(player.id);
			}
		}
	}

	public void AscensionChanged()
	{
		_ascensionPanel.SetAscensionLevel(_lobby.Ascension);
	}

	public void SeedChanged()
	{
		throw new NotImplementedException("Seed should not be changed in standard mode!");
	}

	public void ModifiersChanged()
	{
		throw new NotImplementedException("Modifiers should not be changed in standard mode!");
	}

	public void GameModeChanged()
	{
		throw new NotImplementedException("Game mode should not be changed in standard mode!");
	}

	public void RemotePlayerDisconnected(StartRunLobbyPlayer player)
	{
		_remotePlayerContainer.OnPlayerDisconnected(player);
		foreach (NCharacterSelectButton item in _charButtonContainer.GetChildren().OfType<NCharacterSelectButton>())
		{
			if (item.RemoteSelectedPlayers.Contains(player.id) && player.character == item.Character)
			{
				item.OnRemotePlayerDeselected(player.id);
			}
		}
		UpdateRichPresence();
		UpdateRandomCharacterVisibility();
	}

	public void BeginRun(string seed, List<ActModel> acts, IReadOnlyList<ModifierModel> modifiers)
	{
		if (modifiers.Count > 0)
		{
			Log.Error("Modifiers list is not empty while starting a standard run, ignoring!");
		}
		NAudioManager.Instance?.StopMusic();
		_ascensionPanel.Cleanup();
		_ascensionPanel.Disable();
		_embarkButton.Disable();
		_unreadyButton.Disable();
		UpdatePreferredStandardCharacter();
		if (_lobby.NetService.Type == NetGameType.Singleplayer)
		{
			TaskHelper.RunSafely(StartNewSingleplayerRun(seed, acts));
		}
		else
		{
			TaskHelper.RunSafely(StartNewMultiplayerRun(seed, acts));
		}
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

	private void AfterInitialized()
	{
		NGame.Instance.RemoteCursorContainer.Initialize(_lobby.InputSynchronizer, _lobby.Players.Select((StartRunLobbyPlayer p) => p.id));
		NGame.Instance.ReactionContainer.InitializeNetworking(_lobby.NetService);
		NGame.Instance.TimeoutOverlay.Initialize(_lobby.NetService, isGameLevel: true);
		_lobby.PlayerFailedToConnect += RemoteClientFailedToConnectToLocalHost;
		UpdateRichPresence();
		UpdateRandomCharacterVisibility();
		MegaCrit.Sts2.Core.Logging.Logger.logLevelTypeMap[LogType.Network] = ((_lobby.NetService.Type == NetGameType.Singleplayer) ? LogLevel.Info : LogLevel.Debug);
		MegaCrit.Sts2.Core.Logging.Logger.logLevelTypeMap[LogType.Actions] = ((_lobby.NetService.Type == NetGameType.Singleplayer) ? LogLevel.Info : LogLevel.VeryDebug);
		MegaCrit.Sts2.Core.Logging.Logger.logLevelTypeMap[LogType.GameSync] = ((_lobby.NetService.Type == NetGameType.Singleplayer) ? LogLevel.Info : LogLevel.VeryDebug);
		NGame.Instance.InitDebugSeedFromBootstrap(_lobby.NetService.Type != NetGameType.Singleplayer);
	}

	public void TryShowAscensionSinglePlayerFtue()
	{
		if (_lobby.GameMode == GameMode.Standard && _lobby.GetMaxAscensionAcrossAllCharacters() > 0 && !SaveManager.Instance.SeenPopup("ascension_singleplayer_ftue") && !_lobby.NetService.Type.IsMultiplayer())
		{
			NAscensionSingleplayerFtue nAscensionSingleplayerFtue = NAscensionSingleplayerFtue.Create();
			if (nAscensionSingleplayerFtue != null)
			{
				NModalContainer.Instance.Add(nAscensionSingleplayerFtue);
			}
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal new static List<MethodInfo> GetGodotMethodList()
	{
		List<MethodInfo> list = new List<MethodInfo>(25);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.CheckForMultiplayerAscensionPopup, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.InitializeSingleplayer, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.InitCharacterButtons, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.UpdateRandomCharacterVisibility, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName._Input, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.Object, "inputEvent", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("InputEvent"), exported: false)
		}, null));
		list.Add(new MethodInfo(MethodName.DebugUnlockAllCharacters, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.OnSubmenuOpened, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.GetPreferredCharacterButton, new PropertyInfo(Variant.Type.Object, "", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Control"), exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.UpdatePreferredStandardCharacter, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.OnSubmenuClosed, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.OnEmbarkPressed, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.Object, "_", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Control"), exported: false)
		}, null));
		list.Add(new MethodInfo(MethodName._Process, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.Float, "delta", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
		}, null));
		list.Add(new MethodInfo(MethodName.CleanUpLobby, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.Bool, "disconnectSession", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false),
			new PropertyInfo(Variant.Type.Int, "error", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
		}, null));
		list.Add(new MethodInfo(MethodName.SelectCharacter, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.Object, "charSelectButton", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Control"), exported: false)
		}, null));
		list.Add(new MethodInfo(MethodName.OnAscensionPanelLevelChanged, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.OnUnreadyPressed, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.Object, "_", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Control"), exported: false)
		}, null));
		list.Add(new MethodInfo(MethodName.UpdateRichPresence, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.MaxAscensionChanged, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.AscensionChanged, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.SeedChanged, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.ModifiersChanged, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.GameModeChanged, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.AfterInitialized, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.TryShowAscensionSinglePlayerFtue, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
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
		if (method == MethodName.CheckForMultiplayerAscensionPopup && args.Count == 0)
		{
			CheckForMultiplayerAscensionPopup();
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
		if (method == MethodName.UpdateRandomCharacterVisibility && args.Count == 0)
		{
			UpdateRandomCharacterVisibility();
			ret = default;
			return true;
		}
		if (method == MethodName._Input && args.Count == 1)
		{
			_Input(VariantUtils.ConvertTo<InputEvent>(in args[0]));
			ret = default;
			return true;
		}
		if (method == MethodName.DebugUnlockAllCharacters && args.Count == 0)
		{
			DebugUnlockAllCharacters();
			ret = default;
			return true;
		}
		if (method == MethodName.OnSubmenuOpened && args.Count == 0)
		{
			OnSubmenuOpened();
			ret = default;
			return true;
		}
		if (method == MethodName.GetPreferredCharacterButton && args.Count == 0)
		{
			ret = VariantUtils.CreateFrom<NCharacterSelectButton>(GetPreferredCharacterButton());
			return true;
		}
		if (method == MethodName.UpdatePreferredStandardCharacter && args.Count == 0)
		{
			UpdatePreferredStandardCharacter();
			ret = default;
			return true;
		}
		if (method == MethodName.OnSubmenuClosed && args.Count == 0)
		{
			OnSubmenuClosed();
			ret = default;
			return true;
		}
		if (method == MethodName.OnEmbarkPressed && args.Count == 1)
		{
			OnEmbarkPressed(VariantUtils.ConvertTo<NButton>(in args[0]));
			ret = default;
			return true;
		}
		if (method == MethodName._Process && args.Count == 1)
		{
			_Process(VariantUtils.ConvertTo<double>(in args[0]));
			ret = default;
			return true;
		}
		if (method == MethodName.CleanUpLobby && args.Count == 2)
		{
			CleanUpLobby(VariantUtils.ConvertTo<bool>(in args[0]), VariantUtils.ConvertTo<NetError>(in args[1]));
			ret = default;
			return true;
		}
		if (method == MethodName.SelectCharacter && args.Count == 1)
		{
			SelectCharacter(VariantUtils.ConvertTo<NCharacterSelectButton>(in args[0]));
			ret = default;
			return true;
		}
		if (method == MethodName.OnAscensionPanelLevelChanged && args.Count == 0)
		{
			OnAscensionPanelLevelChanged();
			ret = default;
			return true;
		}
		if (method == MethodName.OnUnreadyPressed && args.Count == 1)
		{
			OnUnreadyPressed(VariantUtils.ConvertTo<NButton>(in args[0]));
			ret = default;
			return true;
		}
		if (method == MethodName.UpdateRichPresence && args.Count == 0)
		{
			UpdateRichPresence();
			ret = default;
			return true;
		}
		if (method == MethodName.MaxAscensionChanged && args.Count == 0)
		{
			MaxAscensionChanged();
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
		if (method == MethodName.AfterInitialized && args.Count == 0)
		{
			AfterInitialized();
			ret = default;
			return true;
		}
		if (method == MethodName.TryShowAscensionSinglePlayerFtue && args.Count == 0)
		{
			TryShowAscensionSinglePlayerFtue();
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
		if (method == MethodName.CheckForMultiplayerAscensionPopup)
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
		if (method == MethodName.UpdateRandomCharacterVisibility)
		{
			return true;
		}
		if (method == MethodName._Input)
		{
			return true;
		}
		if (method == MethodName.DebugUnlockAllCharacters)
		{
			return true;
		}
		if (method == MethodName.OnSubmenuOpened)
		{
			return true;
		}
		if (method == MethodName.GetPreferredCharacterButton)
		{
			return true;
		}
		if (method == MethodName.UpdatePreferredStandardCharacter)
		{
			return true;
		}
		if (method == MethodName.OnSubmenuClosed)
		{
			return true;
		}
		if (method == MethodName.OnEmbarkPressed)
		{
			return true;
		}
		if (method == MethodName._Process)
		{
			return true;
		}
		if (method == MethodName.CleanUpLobby)
		{
			return true;
		}
		if (method == MethodName.SelectCharacter)
		{
			return true;
		}
		if (method == MethodName.OnAscensionPanelLevelChanged)
		{
			return true;
		}
		if (method == MethodName.OnUnreadyPressed)
		{
			return true;
		}
		if (method == MethodName.UpdateRichPresence)
		{
			return true;
		}
		if (method == MethodName.MaxAscensionChanged)
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
		if (method == MethodName.AfterInitialized)
		{
			return true;
		}
		if (method == MethodName.TryShowAscensionSinglePlayerFtue)
		{
			return true;
		}
		return base.HasGodotClassMethod(in method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if (name == PropertyName._name)
		{
			_name = VariantUtils.ConvertTo<MegaLabel>(in value);
			return true;
		}
		if (name == PropertyName._infoPanel)
		{
			_infoPanel = VariantUtils.ConvertTo<Control>(in value);
			return true;
		}
		if (name == PropertyName._description)
		{
			_description = VariantUtils.ConvertTo<MegaRichTextLabel>(in value);
			return true;
		}
		if (name == PropertyName._hp)
		{
			_hp = VariantUtils.ConvertTo<MegaLabel>(in value);
			return true;
		}
		if (name == PropertyName._gold)
		{
			_gold = VariantUtils.ConvertTo<MegaLabel>(in value);
			return true;
		}
		if (name == PropertyName._relicTitle)
		{
			_relicTitle = VariantUtils.ConvertTo<MegaRichTextLabel>(in value);
			return true;
		}
		if (name == PropertyName._relicDescription)
		{
			_relicDescription = VariantUtils.ConvertTo<MegaRichTextLabel>(in value);
			return true;
		}
		if (name == PropertyName._relicIcon)
		{
			_relicIcon = VariantUtils.ConvertTo<TextureRect>(in value);
			return true;
		}
		if (name == PropertyName._relicIconOutline)
		{
			_relicIconOutline = VariantUtils.ConvertTo<TextureRect>(in value);
			return true;
		}
		if (name == PropertyName._selectedButton)
		{
			_selectedButton = VariantUtils.ConvertTo<NCharacterSelectButton>(in value);
			return true;
		}
		if (name == PropertyName._charButtonContainer)
		{
			_charButtonContainer = VariantUtils.ConvertTo<Control>(in value);
			return true;
		}
		if (name == PropertyName._bgContainer)
		{
			_bgContainer = VariantUtils.ConvertTo<Control>(in value);
			return true;
		}
		if (name == PropertyName._readyAndWaitingContainer)
		{
			_readyAndWaitingContainer = VariantUtils.ConvertTo<Control>(in value);
			return true;
		}
		if (name == PropertyName._backButton)
		{
			_backButton = VariantUtils.ConvertTo<NBackButton>(in value);
			return true;
		}
		if (name == PropertyName._unreadyButton)
		{
			_unreadyButton = VariantUtils.ConvertTo<NBackButton>(in value);
			return true;
		}
		if (name == PropertyName._embarkButton)
		{
			_embarkButton = VariantUtils.ConvertTo<NConfirmButton>(in value);
			return true;
		}
		if (name == PropertyName._ascensionPanel)
		{
			_ascensionPanel = VariantUtils.ConvertTo<NAscensionPanel>(in value);
			return true;
		}
		if (name == PropertyName._actDropdown)
		{
			_actDropdown = VariantUtils.ConvertTo<NActDropdown>(in value);
			return true;
		}
		if (name == PropertyName._actDropdownLabel)
		{
			_actDropdownLabel = VariantUtils.ConvertTo<MegaRichTextLabel>(in value);
			return true;
		}
		if (name == PropertyName._remotePlayerContainer)
		{
			_remotePlayerContainer = VariantUtils.ConvertTo<NRemoteLobbyPlayerContainer>(in value);
			return true;
		}
		if (name == PropertyName._characterUnlockAnimationBackstop)
		{
			_characterUnlockAnimationBackstop = VariantUtils.ConvertTo<Control>(in value);
			return true;
		}
		if (name == PropertyName._randomCharacterButton)
		{
			_randomCharacterButton = VariantUtils.ConvertTo<NCharacterSelectButton>(in value);
			return true;
		}
		if (name == PropertyName._infoPanelTween)
		{
			_infoPanelTween = VariantUtils.ConvertTo<Tween>(in value);
			return true;
		}
		if (name == PropertyName._infoPanelPosFinalVal)
		{
			_infoPanelPosFinalVal = VariantUtils.ConvertTo<Vector2>(in value);
			return true;
		}
		if (name == PropertyName._charSelectButtonScene)
		{
			_charSelectButtonScene = VariantUtils.ConvertTo<PackedScene>(in value);
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
		if (name == PropertyName.ShouldShowActDropdown)
		{
			value = VariantUtils.CreateFrom<bool>(ShouldShowActDropdown);
			return true;
		}
		if (name == PropertyName._name)
		{
			value = VariantUtils.CreateFrom(in _name);
			return true;
		}
		if (name == PropertyName._infoPanel)
		{
			value = VariantUtils.CreateFrom(in _infoPanel);
			return true;
		}
		if (name == PropertyName._description)
		{
			value = VariantUtils.CreateFrom(in _description);
			return true;
		}
		if (name == PropertyName._hp)
		{
			value = VariantUtils.CreateFrom(in _hp);
			return true;
		}
		if (name == PropertyName._gold)
		{
			value = VariantUtils.CreateFrom(in _gold);
			return true;
		}
		if (name == PropertyName._relicTitle)
		{
			value = VariantUtils.CreateFrom(in _relicTitle);
			return true;
		}
		if (name == PropertyName._relicDescription)
		{
			value = VariantUtils.CreateFrom(in _relicDescription);
			return true;
		}
		if (name == PropertyName._relicIcon)
		{
			value = VariantUtils.CreateFrom(in _relicIcon);
			return true;
		}
		if (name == PropertyName._relicIconOutline)
		{
			value = VariantUtils.CreateFrom(in _relicIconOutline);
			return true;
		}
		if (name == PropertyName._selectedButton)
		{
			value = VariantUtils.CreateFrom(in _selectedButton);
			return true;
		}
		if (name == PropertyName._charButtonContainer)
		{
			value = VariantUtils.CreateFrom(in _charButtonContainer);
			return true;
		}
		if (name == PropertyName._bgContainer)
		{
			value = VariantUtils.CreateFrom(in _bgContainer);
			return true;
		}
		if (name == PropertyName._readyAndWaitingContainer)
		{
			value = VariantUtils.CreateFrom(in _readyAndWaitingContainer);
			return true;
		}
		if (name == PropertyName._backButton)
		{
			value = VariantUtils.CreateFrom(in _backButton);
			return true;
		}
		if (name == PropertyName._unreadyButton)
		{
			value = VariantUtils.CreateFrom(in _unreadyButton);
			return true;
		}
		if (name == PropertyName._embarkButton)
		{
			value = VariantUtils.CreateFrom(in _embarkButton);
			return true;
		}
		if (name == PropertyName._ascensionPanel)
		{
			value = VariantUtils.CreateFrom(in _ascensionPanel);
			return true;
		}
		if (name == PropertyName._actDropdown)
		{
			value = VariantUtils.CreateFrom(in _actDropdown);
			return true;
		}
		if (name == PropertyName._actDropdownLabel)
		{
			value = VariantUtils.CreateFrom(in _actDropdownLabel);
			return true;
		}
		if (name == PropertyName._remotePlayerContainer)
		{
			value = VariantUtils.CreateFrom(in _remotePlayerContainer);
			return true;
		}
		if (name == PropertyName._characterUnlockAnimationBackstop)
		{
			value = VariantUtils.CreateFrom(in _characterUnlockAnimationBackstop);
			return true;
		}
		if (name == PropertyName._randomCharacterButton)
		{
			value = VariantUtils.CreateFrom(in _randomCharacterButton);
			return true;
		}
		if (name == PropertyName._infoPanelTween)
		{
			value = VariantUtils.CreateFrom(in _infoPanelTween);
			return true;
		}
		if (name == PropertyName._infoPanelPosFinalVal)
		{
			value = VariantUtils.CreateFrom(in _infoPanelPosFinalVal);
			return true;
		}
		if (name == PropertyName._charSelectButtonScene)
		{
			value = VariantUtils.CreateFrom(in _charSelectButtonScene);
			return true;
		}
		return base.GetGodotClassPropertyValue(in name, out value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal new static List<PropertyInfo> GetGodotPropertyList()
	{
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._name, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._infoPanel, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._description, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._hp, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._gold, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._relicTitle, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._relicDescription, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._relicIcon, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._relicIconOutline, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._selectedButton, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._charButtonContainer, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._bgContainer, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._readyAndWaitingContainer, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._backButton, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._unreadyButton, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._embarkButton, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._ascensionPanel, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._actDropdown, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._actDropdownLabel, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._remotePlayerContainer, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._characterUnlockAnimationBackstop, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._randomCharacterButton, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._infoPanelTween, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Vector2, PropertyName._infoPanelPosFinalVal, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._charSelectButtonScene, PropertyHint.ResourceType, "PackedScene", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName.InitialFocusedControl, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Bool, PropertyName.ShouldShowActDropdown, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		base.SaveGodotObjectData(info);
		info.AddProperty(PropertyName._name, Variant.From(in _name));
		info.AddProperty(PropertyName._infoPanel, Variant.From(in _infoPanel));
		info.AddProperty(PropertyName._description, Variant.From(in _description));
		info.AddProperty(PropertyName._hp, Variant.From(in _hp));
		info.AddProperty(PropertyName._gold, Variant.From(in _gold));
		info.AddProperty(PropertyName._relicTitle, Variant.From(in _relicTitle));
		info.AddProperty(PropertyName._relicDescription, Variant.From(in _relicDescription));
		info.AddProperty(PropertyName._relicIcon, Variant.From(in _relicIcon));
		info.AddProperty(PropertyName._relicIconOutline, Variant.From(in _relicIconOutline));
		info.AddProperty(PropertyName._selectedButton, Variant.From(in _selectedButton));
		info.AddProperty(PropertyName._charButtonContainer, Variant.From(in _charButtonContainer));
		info.AddProperty(PropertyName._bgContainer, Variant.From(in _bgContainer));
		info.AddProperty(PropertyName._readyAndWaitingContainer, Variant.From(in _readyAndWaitingContainer));
		info.AddProperty(PropertyName._backButton, Variant.From(in _backButton));
		info.AddProperty(PropertyName._unreadyButton, Variant.From(in _unreadyButton));
		info.AddProperty(PropertyName._embarkButton, Variant.From(in _embarkButton));
		info.AddProperty(PropertyName._ascensionPanel, Variant.From(in _ascensionPanel));
		info.AddProperty(PropertyName._actDropdown, Variant.From(in _actDropdown));
		info.AddProperty(PropertyName._actDropdownLabel, Variant.From(in _actDropdownLabel));
		info.AddProperty(PropertyName._remotePlayerContainer, Variant.From(in _remotePlayerContainer));
		info.AddProperty(PropertyName._characterUnlockAnimationBackstop, Variant.From(in _characterUnlockAnimationBackstop));
		info.AddProperty(PropertyName._randomCharacterButton, Variant.From(in _randomCharacterButton));
		info.AddProperty(PropertyName._infoPanelTween, Variant.From(in _infoPanelTween));
		info.AddProperty(PropertyName._infoPanelPosFinalVal, Variant.From(in _infoPanelPosFinalVal));
		info.AddProperty(PropertyName._charSelectButtonScene, Variant.From(in _charSelectButtonScene));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		if (info.TryGetProperty(PropertyName._name, out var value))
		{
			_name = value.As<MegaLabel>();
		}
		if (info.TryGetProperty(PropertyName._infoPanel, out var value2))
		{
			_infoPanel = value2.As<Control>();
		}
		if (info.TryGetProperty(PropertyName._description, out var value3))
		{
			_description = value3.As<MegaRichTextLabel>();
		}
		if (info.TryGetProperty(PropertyName._hp, out var value4))
		{
			_hp = value4.As<MegaLabel>();
		}
		if (info.TryGetProperty(PropertyName._gold, out var value5))
		{
			_gold = value5.As<MegaLabel>();
		}
		if (info.TryGetProperty(PropertyName._relicTitle, out var value6))
		{
			_relicTitle = value6.As<MegaRichTextLabel>();
		}
		if (info.TryGetProperty(PropertyName._relicDescription, out var value7))
		{
			_relicDescription = value7.As<MegaRichTextLabel>();
		}
		if (info.TryGetProperty(PropertyName._relicIcon, out var value8))
		{
			_relicIcon = value8.As<TextureRect>();
		}
		if (info.TryGetProperty(PropertyName._relicIconOutline, out var value9))
		{
			_relicIconOutline = value9.As<TextureRect>();
		}
		if (info.TryGetProperty(PropertyName._selectedButton, out var value10))
		{
			_selectedButton = value10.As<NCharacterSelectButton>();
		}
		if (info.TryGetProperty(PropertyName._charButtonContainer, out var value11))
		{
			_charButtonContainer = value11.As<Control>();
		}
		if (info.TryGetProperty(PropertyName._bgContainer, out var value12))
		{
			_bgContainer = value12.As<Control>();
		}
		if (info.TryGetProperty(PropertyName._readyAndWaitingContainer, out var value13))
		{
			_readyAndWaitingContainer = value13.As<Control>();
		}
		if (info.TryGetProperty(PropertyName._backButton, out var value14))
		{
			_backButton = value14.As<NBackButton>();
		}
		if (info.TryGetProperty(PropertyName._unreadyButton, out var value15))
		{
			_unreadyButton = value15.As<NBackButton>();
		}
		if (info.TryGetProperty(PropertyName._embarkButton, out var value16))
		{
			_embarkButton = value16.As<NConfirmButton>();
		}
		if (info.TryGetProperty(PropertyName._ascensionPanel, out var value17))
		{
			_ascensionPanel = value17.As<NAscensionPanel>();
		}
		if (info.TryGetProperty(PropertyName._actDropdown, out var value18))
		{
			_actDropdown = value18.As<NActDropdown>();
		}
		if (info.TryGetProperty(PropertyName._actDropdownLabel, out var value19))
		{
			_actDropdownLabel = value19.As<MegaRichTextLabel>();
		}
		if (info.TryGetProperty(PropertyName._remotePlayerContainer, out var value20))
		{
			_remotePlayerContainer = value20.As<NRemoteLobbyPlayerContainer>();
		}
		if (info.TryGetProperty(PropertyName._characterUnlockAnimationBackstop, out var value21))
		{
			_characterUnlockAnimationBackstop = value21.As<Control>();
		}
		if (info.TryGetProperty(PropertyName._randomCharacterButton, out var value22))
		{
			_randomCharacterButton = value22.As<NCharacterSelectButton>();
		}
		if (info.TryGetProperty(PropertyName._infoPanelTween, out var value23))
		{
			_infoPanelTween = value23.As<Tween>();
		}
		if (info.TryGetProperty(PropertyName._infoPanelPosFinalVal, out var value24))
		{
			_infoPanelPosFinalVal = value24.As<Vector2>();
		}
		if (info.TryGetProperty(PropertyName._charSelectButtonScene, out var value25))
		{
			_charSelectButtonScene = value25.As<PackedScene>();
		}
	}
}
