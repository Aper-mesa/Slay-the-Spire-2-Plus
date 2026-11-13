using System;
using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.ControllerInput;
using MegaCrit.Sts2.Core.Entities.UI;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Saves;
using MegaCrit.Sts2.addons.mega_text;

namespace MegaCrit.Sts2.Core.Nodes.Screens.CharacterSelect;

[ScriptPath("res://src/Core/Nodes/Screens/CharacterSelect/NAscensionPanel.cs")]
public class NAscensionPanel : Control
{
	[Signal]
	public delegate void AscensionLevelChangedEventHandler();

	/// <summary>
	/// Cached StringNames for the methods contained in this class, for fast lookup.
	/// </summary>
	public new class MethodName : Control.MethodName
	{
		/// <summary>
		/// Cached name for the '_Ready' method.
		/// </summary>
		public new static readonly StringName _Ready = "_Ready";

		/// <summary>
		/// Cached name for the 'Initialize' method.
		/// </summary>
		public static readonly StringName Initialize = "Initialize";

		/// <summary>
		/// Cached name for the 'SwitchGameMode' method.
		/// </summary>
		public static readonly StringName SwitchGameMode = "SwitchGameMode";

		/// <summary>
		/// Cached name for the 'SetFireBlue' method.
		/// </summary>
		public static readonly StringName SetFireBlue = "SetFireBlue";

		/// <summary>
		/// Cached name for the 'SetFireRed' method.
		/// </summary>
		public static readonly StringName SetFireRed = "SetFireRed";

		/// <summary>
		/// Cached name for the 'Cleanup' method.
		/// </summary>
		public static readonly StringName Cleanup = "Cleanup";

		/// <summary>
		/// Cached name for the 'SetAscensionLevel' method.
		/// </summary>
		public static readonly StringName SetAscensionLevel = "SetAscensionLevel";

		/// <summary>
		/// Cached name for the 'IncrementAscension' method.
		/// </summary>
		public static readonly StringName IncrementAscension = "IncrementAscension";

		/// <summary>
		/// Cached name for the 'DecrementAscension' method.
		/// </summary>
		public static readonly StringName DecrementAscension = "DecrementAscension";

		/// <summary>
		/// Cached name for the 'RefreshArrowVisibility' method.
		/// </summary>
		public static readonly StringName RefreshArrowVisibility = "RefreshArrowVisibility";

		/// <summary>
		/// Cached name for the 'SetMaxAscension' method.
		/// </summary>
		public static readonly StringName SetMaxAscension = "SetMaxAscension";

		/// <summary>
		/// Cached name for the 'RefreshAscensionText' method.
		/// </summary>
		public static readonly StringName RefreshAscensionText = "RefreshAscensionText";

		/// <summary>
		/// Cached name for the 'AnimIn' method.
		/// </summary>
		public static readonly StringName AnimIn = "AnimIn";

		/// <summary>
		/// Cached name for the 'UpdateControllerButton' method.
		/// </summary>
		public static readonly StringName UpdateControllerButton = "UpdateControllerButton";

		/// <summary>
		/// Cached name for the 'Disable' method.
		/// </summary>
		public static readonly StringName Disable = "Disable";
	}

	/// <summary>
	/// Cached StringNames for the properties and fields contained in this class, for fast lookup.
	/// </summary>
	public new class PropertyName : Control.PropertyName
	{
		/// <summary>
		/// Cached name for the 'Ascension' property.
		/// </summary>
		public static readonly StringName Ascension = "Ascension";

		/// <summary>
		/// Cached name for the '_maxAscension' field.
		/// </summary>
		public static readonly StringName _maxAscension = "_maxAscension";

		/// <summary>
		/// Cached name for the '_leftArrow' field.
		/// </summary>
		public static readonly StringName _leftArrow = "_leftArrow";

		/// <summary>
		/// Cached name for the '_rightArrow' field.
		/// </summary>
		public static readonly StringName _rightArrow = "_rightArrow";

		/// <summary>
		/// Cached name for the '_ascensionLevel' field.
		/// </summary>
		public static readonly StringName _ascensionLevel = "_ascensionLevel";

		/// <summary>
		/// Cached name for the '_info' field.
		/// </summary>
		public static readonly StringName _info = "_info";

		/// <summary>
		/// Cached name for the '_leftTabIcon' field.
		/// </summary>
		public static readonly StringName _leftTabIcon = "_leftTabIcon";

		/// <summary>
		/// Cached name for the '_rightTabIcon' field.
		/// </summary>
		public static readonly StringName _rightTabIcon = "_rightTabIcon";

		/// <summary>
		/// Cached name for the '_standardIcon' field.
		/// </summary>
		public static readonly StringName _standardIcon = "_standardIcon";

		/// <summary>
		/// Cached name for the '_blitzIcon' field.
		/// </summary>
		public static readonly StringName _blitzIcon = "_blitzIcon";

		/// <summary>
		/// Cached name for the '_standardIconHsv' field.
		/// </summary>
		public static readonly StringName _standardIconHsv = "_standardIconHsv";

		/// <summary>
		/// Cached name for the '_arrowsVisible' field.
		/// </summary>
		public static readonly StringName _arrowsVisible = "_arrowsVisible";

		/// <summary>
		/// Cached name for the '_gameMode' field.
		/// </summary>
		public static readonly StringName _gameMode = "_gameMode";

		/// <summary>
		/// Cached name for the '_uiMode' field.
		/// </summary>
		public static readonly StringName _uiMode = "_uiMode";

		/// <summary>
		/// Cached name for the '_tween' field.
		/// </summary>
		public static readonly StringName _tween = "_tween";
	}

	/// <summary>
	/// Cached StringNames for the signals contained in this class, for fast lookup.
	/// </summary>
	public new class SignalName : Control.SignalName
	{
		/// <summary>
		/// Cached name for the 'AscensionLevelChanged' signal.
		/// </summary>
		public static readonly StringName AscensionLevelChanged = "AscensionLevelChanged";
	}

	private static readonly StringName _tabLeftHotkey = MegaInput.viewDeckAndTabLeft;

	private static readonly StringName _tabRightHotkey = MegaInput.viewExhaustPileAndTabRight;

	private static readonly StringName _fontOutlineTheme = "font_outline_color";

	private static readonly StringName _h = new StringName("h");

	private static readonly StringName _v = new StringName("v");

	private static readonly Color _redLabelOutline = new Color("593400");

	private static readonly Color _blueLabelOutline = new Color("004759");

	private int _maxAscension;

	private NButton _leftArrow;

	private NButton _rightArrow;

	private MegaLabel _ascensionLevel;

	private MegaRichTextLabel _info;

	private NHotkeyIcon _leftTabIcon;

	private NHotkeyIcon _rightTabIcon;

	private TextureRect _standardIcon;

	private TextureRect _blitzIcon;

	private ShaderMaterial _standardIconHsv;

	private bool _arrowsVisible = true;

	private GameMode _gameMode = GameMode.Standard;

	private MultiplayerUiMode _uiMode = MultiplayerUiMode.Singleplayer;

	private Tween? _tween;

	private AscensionLevelChangedEventHandler backing_AscensionLevelChanged;

	public int Ascension { get; private set; }

	/// <inheritdoc cref="T:MegaCrit.Sts2.Core.Nodes.Screens.CharacterSelect.NAscensionPanel.AscensionLevelChangedEventHandler" />
	public event AscensionLevelChangedEventHandler AscensionLevelChanged
	{
		add
		{
			backing_AscensionLevelChanged = (AscensionLevelChangedEventHandler)Delegate.Combine(backing_AscensionLevelChanged, value);
		}
		remove
		{
			backing_AscensionLevelChanged = (AscensionLevelChangedEventHandler)Delegate.Remove(backing_AscensionLevelChanged, value);
		}
	}

	public override void _Ready()
	{
		_leftTabIcon = GetNode<NHotkeyIcon>("%LeftTabIcon");
		_rightTabIcon = GetNode<NHotkeyIcon>("%RightTabIcon");
		_leftArrow = GetNode<NButton>("HBoxContainer/LeftArrowContainer/LeftArrow");
		_rightArrow = GetNode<NButton>("HBoxContainer/RightArrowContainer/RightArrow");
		_ascensionLevel = GetNode<MegaLabel>("HBoxContainer/AscensionContainer/AscensionLevel");
		_info = GetNode<MegaRichTextLabel>("HBoxContainer/AscensionDescription/Description");
		_standardIcon = GetNode<TextureRect>("%AscensionIconStandard");
		_blitzIcon = GetNode<TextureRect>("%AscensionIconBlitz");
		_standardIconHsv = (ShaderMaterial)_standardIcon.Material;
		_leftArrow.Connect(NClickableControl.SignalName.Released, Callable.From((NButton _) =>
		{
			DecrementAscension();
		}));
		_rightArrow.Connect(NClickableControl.SignalName.Released, Callable.From((NButton _) =>
		{
			IncrementAscension();
		}));
		NControllerManager.Instance.Connect(NControllerManager.SignalName.MouseDetected, Callable.From(UpdateControllerButton));
		NControllerManager.Instance.Connect(NControllerManager.SignalName.ControllerDetected, Callable.From(UpdateControllerButton));
		NInputManager.Instance.Connect(NInputManager.SignalName.InputRebound, Callable.From(UpdateControllerButton));
		UpdateControllerButton();
	}

	public void Initialize(MultiplayerUiMode mode, GameMode gameMode)
	{
		_uiMode = mode;
		SwitchGameMode(gameMode);
		if (_uiMode == MultiplayerUiMode.Host)
		{
			SetFireBlue();
			_arrowsVisible = true;
			if (_gameMode != GameMode.Blitz)
			{
				SetMaxAscension(SaveManager.Instance.Progress.MaxMultiplayerAscension);
				SetAscensionLevel(Math.Min(_maxAscension, SaveManager.Instance.Progress.PreferredMultiplayerAscension));
			}
			else
			{
				SetMaxAscension(SaveManager.Instance.Progress.MaxBlitzAscension);
				SetAscensionLevel(Math.Min(_maxAscension, SaveManager.Instance.Progress.PreferredBlitzAscension));
			}
			NHotkeyManager.Instance.PushHotkeyPressedBinding(_tabLeftHotkey, DecrementAscension, 0L);
			NHotkeyManager.Instance.PushHotkeyPressedBinding(_tabRightHotkey, IncrementAscension, 0L);
		}
		else if (_uiMode == MultiplayerUiMode.Singleplayer)
		{
			SetFireRed();
			_arrowsVisible = true;
			if (_gameMode != GameMode.Blitz)
			{
				SetMaxAscension(0);
				SetAscensionLevel(0);
			}
			else
			{
				SetMaxAscension(SaveManager.Instance.Progress.MaxBlitzAscension);
				SetAscensionLevel(Math.Min(_maxAscension, SaveManager.Instance.Progress.PreferredBlitzAscension));
			}
			NHotkeyManager.Instance.PushHotkeyPressedBinding(_tabLeftHotkey, DecrementAscension, 0L);
			NHotkeyManager.Instance.PushHotkeyPressedBinding(_tabRightHotkey, IncrementAscension, 0L);
		}
		else
		{
			MultiplayerUiMode uiMode = _uiMode;
			if ((uint)(uiMode - 3) <= 1u)
			{
				SetFireBlue();
				_arrowsVisible = false;
				SetMaxAscension(0);
			}
		}
	}

	public void SwitchGameMode(GameMode gameMode)
	{
		_gameMode = gameMode;
		_blitzIcon.Visible = gameMode == GameMode.Blitz;
		_standardIcon.Visible = gameMode != GameMode.Blitz;
	}

	private void SetFireBlue()
	{
		_standardIconHsv.SetShaderParameter(_h, 0.52f);
		_standardIconHsv.SetShaderParameter(_v, 1.2f);
		_ascensionLevel.AddThemeColorOverride(_fontOutlineTheme, _blueLabelOutline);
	}

	private void SetFireRed()
	{
		_standardIconHsv.SetShaderParameter(_h, 1f);
		_standardIconHsv.SetShaderParameter(_v, 1f);
		_ascensionLevel.AddThemeColorOverride(_fontOutlineTheme, _redLabelOutline);
	}

	public void Cleanup()
	{
		MultiplayerUiMode uiMode = _uiMode;
		if ((uint)(uiMode - 1) <= 1u)
		{
			NHotkeyManager.Instance.RemoveHotkeyPressedBinding(_tabLeftHotkey, DecrementAscension);
			NHotkeyManager.Instance.RemoveHotkeyPressedBinding(_tabRightHotkey, IncrementAscension);
		}
	}

	public void SetAscensionLevel(int ascension)
	{
		if (Ascension != ascension)
		{
			Ascension = ascension;
			EmitSignal(SignalName.AscensionLevelChanged);
		}
		RefreshAscensionText();
		RefreshArrowVisibility();
	}

	private void IncrementAscension()
	{
		if (Ascension < _maxAscension)
		{
			SetAscensionLevel(Ascension + 1);
		}
	}

	private void DecrementAscension()
	{
		if (Ascension > 0)
		{
			SetAscensionLevel(Ascension - 1);
		}
	}

	private void RefreshArrowVisibility()
	{
		_leftArrow.Visible = _arrowsVisible && Ascension != 0 && _gameMode != GameMode.Dread;
		_rightArrow.Visible = _arrowsVisible && Ascension != _maxAscension && _gameMode != GameMode.Dread;
	}

	public void SetMaxAscension(int maxAscension)
	{
		Log.Info($"Max ascension changed to {maxAscension}");
		_maxAscension = maxAscension;
		if (Ascension >= _maxAscension)
		{
			SetAscensionLevel(_maxAscension);
		}
		Visible = _maxAscension > 0;
		RefreshArrowVisibility();
	}

	private void RefreshAscensionText()
	{
		_ascensionLevel.SetTextAutoSize(Ascension.ToString());
		string formattedText = AscensionHelper.GetTitle(Ascension, _gameMode).GetFormattedText();
		string formattedText2 = AscensionHelper.GetDescription(Ascension, _gameMode).GetFormattedText();
		_info.Text = "[b][gold]" + formattedText + "[/gold][/b]\n" + formattedText2;
	}

	public void AnimIn()
	{
		if (Visible)
		{
			Color modulate = Modulate;
			modulate.A = 0f;
			Modulate = modulate;
			_tween?.FastForwardToCompletion();
			_tween = CreateTween().SetParallel();
			_tween.TweenProperty(this, "modulate:a", 1f, 0.2);
			_tween.TweenProperty(this, "position:y", Position.Y, 0.3).From(Position.Y + 30f).SetEase(Tween.EaseType.Out)
				.SetTrans(Tween.TransitionType.Back);
		}
	}

	private void UpdateControllerButton()
	{
		MultiplayerUiMode uiMode = _uiMode;
		if ((uint)(uiMode - 1) <= 1u)
		{
			_leftTabIcon.Visible = NControllerManager.Instance.IsUsingDirectionalNavigation;
			_rightTabIcon.Visible = NControllerManager.Instance.IsUsingDirectionalNavigation;
			_leftTabIcon.UpdateInput(MegaInput.viewDeckAndTabLeft);
			_rightTabIcon.UpdateInput(MegaInput.viewExhaustPileAndTabRight);
		}
		else
		{
			_leftTabIcon.Visible = false;
			_rightTabIcon.Visible = false;
		}
	}

	public void Disable()
	{
		_leftArrow.Disable();
		_rightArrow.Disable();
		_leftTabIcon.Visible = false;
		_rightTabIcon.Visible = false;
	}

	/// <summary>
	/// Get the method information for all the methods declared in this class.
	/// This method is used by Godot to register the available methods in the editor.
	/// Do not call this method.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		List<MethodInfo> list = new List<MethodInfo>(15);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.Initialize, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.Int, "mode", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false),
			new PropertyInfo(Variant.Type.Int, "gameMode", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
		}, null));
		list.Add(new MethodInfo(MethodName.SwitchGameMode, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.Int, "gameMode", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
		}, null));
		list.Add(new MethodInfo(MethodName.SetFireBlue, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.SetFireRed, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.Cleanup, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.SetAscensionLevel, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.Int, "ascension", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
		}, null));
		list.Add(new MethodInfo(MethodName.IncrementAscension, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.DecrementAscension, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.RefreshArrowVisibility, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.SetMaxAscension, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.Int, "maxAscension", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
		}, null));
		list.Add(new MethodInfo(MethodName.RefreshAscensionText, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.AnimIn, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.UpdateControllerButton, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.Disable, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		return list;
	}

	/// <inheritdoc />
	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		if (method == MethodName._Ready && args.Count == 0)
		{
			_Ready();
			ret = default;
			return true;
		}
		if (method == MethodName.Initialize && args.Count == 2)
		{
			Initialize(VariantUtils.ConvertTo<MultiplayerUiMode>(in args[0]), VariantUtils.ConvertTo<GameMode>(in args[1]));
			ret = default;
			return true;
		}
		if (method == MethodName.SwitchGameMode && args.Count == 1)
		{
			SwitchGameMode(VariantUtils.ConvertTo<GameMode>(in args[0]));
			ret = default;
			return true;
		}
		if (method == MethodName.SetFireBlue && args.Count == 0)
		{
			SetFireBlue();
			ret = default;
			return true;
		}
		if (method == MethodName.SetFireRed && args.Count == 0)
		{
			SetFireRed();
			ret = default;
			return true;
		}
		if (method == MethodName.Cleanup && args.Count == 0)
		{
			Cleanup();
			ret = default;
			return true;
		}
		if (method == MethodName.SetAscensionLevel && args.Count == 1)
		{
			SetAscensionLevel(VariantUtils.ConvertTo<int>(in args[0]));
			ret = default;
			return true;
		}
		if (method == MethodName.IncrementAscension && args.Count == 0)
		{
			IncrementAscension();
			ret = default;
			return true;
		}
		if (method == MethodName.DecrementAscension && args.Count == 0)
		{
			DecrementAscension();
			ret = default;
			return true;
		}
		if (method == MethodName.RefreshArrowVisibility && args.Count == 0)
		{
			RefreshArrowVisibility();
			ret = default;
			return true;
		}
		if (method == MethodName.SetMaxAscension && args.Count == 1)
		{
			SetMaxAscension(VariantUtils.ConvertTo<int>(in args[0]));
			ret = default;
			return true;
		}
		if (method == MethodName.RefreshAscensionText && args.Count == 0)
		{
			RefreshAscensionText();
			ret = default;
			return true;
		}
		if (method == MethodName.AnimIn && args.Count == 0)
		{
			AnimIn();
			ret = default;
			return true;
		}
		if (method == MethodName.UpdateControllerButton && args.Count == 0)
		{
			UpdateControllerButton();
			ret = default;
			return true;
		}
		if (method == MethodName.Disable && args.Count == 0)
		{
			Disable();
			ret = default;
			return true;
		}
		return base.InvokeGodotClassMethod(in method, args, out ret);
	}

	/// <inheritdoc />
	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassMethod(in godot_string_name method)
	{
		if (method == MethodName._Ready)
		{
			return true;
		}
		if (method == MethodName.Initialize)
		{
			return true;
		}
		if (method == MethodName.SwitchGameMode)
		{
			return true;
		}
		if (method == MethodName.SetFireBlue)
		{
			return true;
		}
		if (method == MethodName.SetFireRed)
		{
			return true;
		}
		if (method == MethodName.Cleanup)
		{
			return true;
		}
		if (method == MethodName.SetAscensionLevel)
		{
			return true;
		}
		if (method == MethodName.IncrementAscension)
		{
			return true;
		}
		if (method == MethodName.DecrementAscension)
		{
			return true;
		}
		if (method == MethodName.RefreshArrowVisibility)
		{
			return true;
		}
		if (method == MethodName.SetMaxAscension)
		{
			return true;
		}
		if (method == MethodName.RefreshAscensionText)
		{
			return true;
		}
		if (method == MethodName.AnimIn)
		{
			return true;
		}
		if (method == MethodName.UpdateControllerButton)
		{
			return true;
		}
		if (method == MethodName.Disable)
		{
			return true;
		}
		return base.HasGodotClassMethod(in method);
	}

	/// <inheritdoc />
	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if (name == PropertyName.Ascension)
		{
			Ascension = VariantUtils.ConvertTo<int>(in value);
			return true;
		}
		if (name == PropertyName._maxAscension)
		{
			_maxAscension = VariantUtils.ConvertTo<int>(in value);
			return true;
		}
		if (name == PropertyName._leftArrow)
		{
			_leftArrow = VariantUtils.ConvertTo<NButton>(in value);
			return true;
		}
		if (name == PropertyName._rightArrow)
		{
			_rightArrow = VariantUtils.ConvertTo<NButton>(in value);
			return true;
		}
		if (name == PropertyName._ascensionLevel)
		{
			_ascensionLevel = VariantUtils.ConvertTo<MegaLabel>(in value);
			return true;
		}
		if (name == PropertyName._info)
		{
			_info = VariantUtils.ConvertTo<MegaRichTextLabel>(in value);
			return true;
		}
		if (name == PropertyName._leftTabIcon)
		{
			_leftTabIcon = VariantUtils.ConvertTo<NHotkeyIcon>(in value);
			return true;
		}
		if (name == PropertyName._rightTabIcon)
		{
			_rightTabIcon = VariantUtils.ConvertTo<NHotkeyIcon>(in value);
			return true;
		}
		if (name == PropertyName._standardIcon)
		{
			_standardIcon = VariantUtils.ConvertTo<TextureRect>(in value);
			return true;
		}
		if (name == PropertyName._blitzIcon)
		{
			_blitzIcon = VariantUtils.ConvertTo<TextureRect>(in value);
			return true;
		}
		if (name == PropertyName._standardIconHsv)
		{
			_standardIconHsv = VariantUtils.ConvertTo<ShaderMaterial>(in value);
			return true;
		}
		if (name == PropertyName._arrowsVisible)
		{
			_arrowsVisible = VariantUtils.ConvertTo<bool>(in value);
			return true;
		}
		if (name == PropertyName._gameMode)
		{
			_gameMode = VariantUtils.ConvertTo<GameMode>(in value);
			return true;
		}
		if (name == PropertyName._uiMode)
		{
			_uiMode = VariantUtils.ConvertTo<MultiplayerUiMode>(in value);
			return true;
		}
		if (name == PropertyName._tween)
		{
			_tween = VariantUtils.ConvertTo<Tween>(in value);
			return true;
		}
		return base.SetGodotClassPropertyValue(in name, in value);
	}

	/// <inheritdoc />
	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool GetGodotClassPropertyValue(in godot_string_name name, out godot_variant value)
	{
		if (name == PropertyName.Ascension)
		{
			value = VariantUtils.CreateFrom<int>(Ascension);
			return true;
		}
		if (name == PropertyName._maxAscension)
		{
			value = VariantUtils.CreateFrom(in _maxAscension);
			return true;
		}
		if (name == PropertyName._leftArrow)
		{
			value = VariantUtils.CreateFrom(in _leftArrow);
			return true;
		}
		if (name == PropertyName._rightArrow)
		{
			value = VariantUtils.CreateFrom(in _rightArrow);
			return true;
		}
		if (name == PropertyName._ascensionLevel)
		{
			value = VariantUtils.CreateFrom(in _ascensionLevel);
			return true;
		}
		if (name == PropertyName._info)
		{
			value = VariantUtils.CreateFrom(in _info);
			return true;
		}
		if (name == PropertyName._leftTabIcon)
		{
			value = VariantUtils.CreateFrom(in _leftTabIcon);
			return true;
		}
		if (name == PropertyName._rightTabIcon)
		{
			value = VariantUtils.CreateFrom(in _rightTabIcon);
			return true;
		}
		if (name == PropertyName._standardIcon)
		{
			value = VariantUtils.CreateFrom(in _standardIcon);
			return true;
		}
		if (name == PropertyName._blitzIcon)
		{
			value = VariantUtils.CreateFrom(in _blitzIcon);
			return true;
		}
		if (name == PropertyName._standardIconHsv)
		{
			value = VariantUtils.CreateFrom(in _standardIconHsv);
			return true;
		}
		if (name == PropertyName._arrowsVisible)
		{
			value = VariantUtils.CreateFrom(in _arrowsVisible);
			return true;
		}
		if (name == PropertyName._gameMode)
		{
			value = VariantUtils.CreateFrom(in _gameMode);
			return true;
		}
		if (name == PropertyName._uiMode)
		{
			value = VariantUtils.CreateFrom(in _uiMode);
			return true;
		}
		if (name == PropertyName._tween)
		{
			value = VariantUtils.CreateFrom(in _tween);
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
		list.Add(new PropertyInfo(Variant.Type.Int, PropertyName.Ascension, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Int, PropertyName._maxAscension, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._leftArrow, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._rightArrow, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._ascensionLevel, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._info, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._leftTabIcon, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._rightTabIcon, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._standardIcon, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._blitzIcon, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._standardIconHsv, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Bool, PropertyName._arrowsVisible, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Int, PropertyName._gameMode, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Int, PropertyName._uiMode, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._tween, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		return list;
	}

	/// <inheritdoc />
	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		base.SaveGodotObjectData(info);
		info.AddProperty(PropertyName.Ascension, Variant.From<int>(Ascension));
		info.AddProperty(PropertyName._maxAscension, Variant.From(in _maxAscension));
		info.AddProperty(PropertyName._leftArrow, Variant.From(in _leftArrow));
		info.AddProperty(PropertyName._rightArrow, Variant.From(in _rightArrow));
		info.AddProperty(PropertyName._ascensionLevel, Variant.From(in _ascensionLevel));
		info.AddProperty(PropertyName._info, Variant.From(in _info));
		info.AddProperty(PropertyName._leftTabIcon, Variant.From(in _leftTabIcon));
		info.AddProperty(PropertyName._rightTabIcon, Variant.From(in _rightTabIcon));
		info.AddProperty(PropertyName._standardIcon, Variant.From(in _standardIcon));
		info.AddProperty(PropertyName._blitzIcon, Variant.From(in _blitzIcon));
		info.AddProperty(PropertyName._standardIconHsv, Variant.From(in _standardIconHsv));
		info.AddProperty(PropertyName._arrowsVisible, Variant.From(in _arrowsVisible));
		info.AddProperty(PropertyName._gameMode, Variant.From(in _gameMode));
		info.AddProperty(PropertyName._uiMode, Variant.From(in _uiMode));
		info.AddProperty(PropertyName._tween, Variant.From(in _tween));
		info.AddSignalEventDelegate(SignalName.AscensionLevelChanged, backing_AscensionLevelChanged);
	}

	/// <inheritdoc />
	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		if (info.TryGetProperty(PropertyName.Ascension, out var value))
		{
			Ascension = value.As<int>();
		}
		if (info.TryGetProperty(PropertyName._maxAscension, out var value2))
		{
			_maxAscension = value2.As<int>();
		}
		if (info.TryGetProperty(PropertyName._leftArrow, out var value3))
		{
			_leftArrow = value3.As<NButton>();
		}
		if (info.TryGetProperty(PropertyName._rightArrow, out var value4))
		{
			_rightArrow = value4.As<NButton>();
		}
		if (info.TryGetProperty(PropertyName._ascensionLevel, out var value5))
		{
			_ascensionLevel = value5.As<MegaLabel>();
		}
		if (info.TryGetProperty(PropertyName._info, out var value6))
		{
			_info = value6.As<MegaRichTextLabel>();
		}
		if (info.TryGetProperty(PropertyName._leftTabIcon, out var value7))
		{
			_leftTabIcon = value7.As<NHotkeyIcon>();
		}
		if (info.TryGetProperty(PropertyName._rightTabIcon, out var value8))
		{
			_rightTabIcon = value8.As<NHotkeyIcon>();
		}
		if (info.TryGetProperty(PropertyName._standardIcon, out var value9))
		{
			_standardIcon = value9.As<TextureRect>();
		}
		if (info.TryGetProperty(PropertyName._blitzIcon, out var value10))
		{
			_blitzIcon = value10.As<TextureRect>();
		}
		if (info.TryGetProperty(PropertyName._standardIconHsv, out var value11))
		{
			_standardIconHsv = value11.As<ShaderMaterial>();
		}
		if (info.TryGetProperty(PropertyName._arrowsVisible, out var value12))
		{
			_arrowsVisible = value12.As<bool>();
		}
		if (info.TryGetProperty(PropertyName._gameMode, out var value13))
		{
			_gameMode = value13.As<GameMode>();
		}
		if (info.TryGetProperty(PropertyName._uiMode, out var value14))
		{
			_uiMode = value14.As<MultiplayerUiMode>();
		}
		if (info.TryGetProperty(PropertyName._tween, out var value15))
		{
			_tween = value15.As<Tween>();
		}
		if (info.TryGetSignalEventDelegate<AscensionLevelChangedEventHandler>(SignalName.AscensionLevelChanged, out var value16))
		{
			backing_AscensionLevelChanged = value16;
		}
	}

	/// <summary>
	/// Get the signal information for all the signals declared in this class.
	/// This method is used by Godot to register the available signals in the editor.
	/// Do not call this method.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotSignalList()
	{
		List<MethodInfo> list = new List<MethodInfo>(1);
		list.Add(new MethodInfo(SignalName.AscensionLevelChanged, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		return list;
	}

	protected void EmitSignalAscensionLevelChanged()
	{
		EmitSignal(SignalName.AscensionLevelChanged);
	}

	/// <inheritdoc />
	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RaiseGodotClassSignalCallbacks(in godot_string_name signal, NativeVariantPtrArgs args)
	{
		if (signal == SignalName.AscensionLevelChanged && args.Count == 0)
		{
			backing_AscensionLevelChanged?.Invoke();
		}
		else
		{
			base.RaiseGodotClassSignalCallbacks(in signal, args);
		}
	}

	/// <inheritdoc />
	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassSignal(in godot_string_name signal)
	{
		if (signal == SignalName.AscensionLevelChanged)
		{
			return true;
		}
		return base.HasGodotClassSignal(in signal);
	}
}
