using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Entities.Ascension;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.HoverTips;
using MegaCrit.Sts2.Core.Platform;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Saves;
using MegaCrit.Sts2.addons.mega_text;

namespace MegaCrit.Sts2.Core.Nodes.Screens.RunHistoryScreen;

[ScriptPath("res://src/Core/Nodes/Screens/RunHistoryScreen/NRunHistoryPlayerIcon.cs")]
public class NRunHistoryPlayerIcon : NButton
{
	/// <summary>
	/// Cached StringNames for the methods contained in this class, for fast lookup.
	/// </summary>
	public new class MethodName : NButton.MethodName
	{
		/// <summary>
		/// Cached name for the '_Ready' method.
		/// </summary>
		public new static readonly StringName _Ready = "_Ready";

		/// <summary>
		/// Cached name for the 'SetFireBlue' method.
		/// </summary>
		public static readonly StringName SetFireBlue = "SetFireBlue";

		/// <summary>
		/// Cached name for the 'SetFireRed' method.
		/// </summary>
		public static readonly StringName SetFireRed = "SetFireRed";

		/// <summary>
		/// Cached name for the 'Select' method.
		/// </summary>
		public static readonly StringName Select = "Select";

		/// <summary>
		/// Cached name for the 'Deselect' method.
		/// </summary>
		public static readonly StringName Deselect = "Deselect";

		/// <summary>
		/// Cached name for the 'HideAscensionVisuals' method.
		/// </summary>
		public static readonly StringName HideAscensionVisuals = "HideAscensionVisuals";

		/// <summary>
		/// Cached name for the 'OnFocus' method.
		/// </summary>
		public new static readonly StringName OnFocus = "OnFocus";

		/// <summary>
		/// Cached name for the 'OnUnfocus' method.
		/// </summary>
		public new static readonly StringName OnUnfocus = "OnUnfocus";
	}

	/// <summary>
	/// Cached StringNames for the properties and fields contained in this class, for fast lookup.
	/// </summary>
	public new class PropertyName : NButton.PropertyName
	{
		/// <summary>
		/// Cached name for the '_achievementLock' field.
		/// </summary>
		public static readonly StringName _achievementLock = "_achievementLock";

		/// <summary>
		/// Cached name for the '_ascensionIconStandard' field.
		/// </summary>
		public static readonly StringName _ascensionIconStandard = "_ascensionIconStandard";

		/// <summary>
		/// Cached name for the '_ascensionIconDread' field.
		/// </summary>
		public static readonly StringName _ascensionIconDread = "_ascensionIconDread";

		/// <summary>
		/// Cached name for the '_ascensionIconBlitz' field.
		/// </summary>
		public static readonly StringName _ascensionIconBlitz = "_ascensionIconBlitz";

		/// <summary>
		/// Cached name for the '_ascensionLabel' field.
		/// </summary>
		public static readonly StringName _ascensionLabel = "_ascensionLabel";

		/// <summary>
		/// Cached name for the '_selectionReticle' field.
		/// </summary>
		public static readonly StringName _selectionReticle = "_selectionReticle";

		/// <summary>
		/// Cached name for the '_hsv' field.
		/// </summary>
		public static readonly StringName _hsv = "_hsv";

		/// <summary>
		/// Cached name for the '_icon' field.
		/// </summary>
		public static readonly StringName _icon = "_icon";

		/// <summary>
		/// Cached name for the '_gameMode' field.
		/// </summary>
		public static readonly StringName _gameMode = "_gameMode";

		/// <summary>
		/// Cached name for the '_tween' field.
		/// </summary>
		public static readonly StringName _tween = "_tween";
	}

	/// <summary>
	/// Cached StringNames for the signals contained in this class, for fast lookup.
	/// </summary>
	public new class SignalName : NButton.SignalName
	{
	}

	private static readonly StringName _fontOutlineTheme = "font_outline_color";

	private static readonly Color _redLabelOutline = new Color("593400");

	private static readonly Color _blueLabelOutline = new Color("004759");

	private static readonly StringName _h = new StringName("h");

	private static readonly StringName _v = new StringName("v");

	private static readonly StringName _s = new StringName("s");

	public static readonly string scenePath = SceneHelper.GetScenePath("screens/run_history_screen/run_history_player_icon");

	private Control _achievementLock;

	private TextureRect _ascensionIconStandard;

	private TextureRect _ascensionIconDread;

	private TextureRect _ascensionIconBlitz;

	private MegaLabel _ascensionLabel;

	private NSelectionReticle _selectionReticle;

	private ShaderMaterial _hsv;

	private TextureRect _icon;

	private readonly List<IHoverTip> _hoverTips = new List<IHoverTip>();

	private GameMode _gameMode;

	private Tween? _tween;

	private static readonly Vector2 _enabledScale = Vector2.One * 1.1f;

	private static readonly Vector2 _disabledScale = Vector2.One * 0.95f;

	public RunHistoryPlayer Player { get; private set; }

	public override void _Ready()
	{
		ConnectSignals();
		_achievementLock = GetNode<Control>("%AchievementLock");
		_ascensionIconStandard = GetNode<TextureRect>("%AscensionIconStandard");
		_ascensionIconDread = GetNode<TextureRect>("%AscensionIconDread");
		_ascensionIconBlitz = GetNode<TextureRect>("%AscensionIconBlitz");
		_ascensionLabel = GetNode<MegaLabel>("%AscensionLabel");
		_selectionReticle = GetNode<NSelectionReticle>("%SelectionReticle");
		_icon = GetNode<TextureRect>("%Icon");
		_hsv = (ShaderMaterial)_icon.GetMaterial();
		Deselect();
	}

	public void LoadRun(RunHistoryPlayer player, RunHistory history)
	{
		Player = player;
		_gameMode = history.GameMode;
		int num = AscensionManager.ClampToMode(history.GameMode, history.Ascension);
		CharacterModel characterModel = SaveUtil.CharacterOrDeprecated(player.Character);
		_icon.Texture = characterModel.IconTexture;
		LocString locString = new LocString("ascension", "PORTRAIT_TITLE");
		locString.Add("character", characterModel.Title);
		locString.Add("ascension", num);
		LocString locString2 = new LocString("ascension", "PORTRAIT_DESCRIPTION");
		List<string> list = new List<string>();
		for (int i = 1; i <= num; i++)
		{
			list.Add(AscensionHelper.GetTitle(i, history.GameMode).GetFormattedText());
		}
		locString2.Add("ascensions", list);
		_achievementLock.Visible = GameModeHelper.AreAchievementsAndEpochsLockedForHistory(history);
		_ascensionLabel.SetTextAutoSize((num > 0) ? num.ToString() : string.Empty);
		LocString locString3 = new LocString("run_history", "PLAYER_HOVER");
		if (history.Players.Count > 1)
		{
			locString3.Add("PlayerName", PlatformUtil.GetPlayerName(history.PlatformType, player.Id));
			locString3.Add("CharacterName", characterModel.Title.GetFormattedText());
			SetFireBlue();
		}
		else
		{
			locString3.Add("PlayerName", characterModel.Title.GetFormattedText());
			locString3.Add("CharacterName", string.Empty);
			SetFireRed();
		}
		if (num > 0 || GameModeHelper.AreAchievementsAndEpochsLockedForHistory(history))
		{
			_hoverTips.Add(AscensionHelper.GetHoverTip(characterModel, num, history.GameMode, history.IsCustomGame));
		}
		else
		{
			_hoverTips.Add(new HoverTip(locString3));
		}
	}

	private void SetFireBlue()
	{
		ShaderMaterial shaderMaterial = (ShaderMaterial)_ascensionIconStandard.Material;
		shaderMaterial.SetShaderParameter(_h, 0.52f);
		shaderMaterial.SetShaderParameter(_v, 1.2f);
		_ascensionLabel.AddThemeColorOverride(_fontOutlineTheme, _blueLabelOutline);
	}

	private void SetFireRed()
	{
		ShaderMaterial shaderMaterial = (ShaderMaterial)_ascensionIconStandard.Material;
		shaderMaterial.SetShaderParameter(_h, 1f);
		shaderMaterial.SetShaderParameter(_v, 1f);
		_ascensionLabel.AddThemeColorOverride(_fontOutlineTheme, _redLabelOutline);
	}

	public void Select()
	{
		_hsv.SetShaderParameter(_s, 1f);
		_hsv.SetShaderParameter(_v, 1f);
		HideAscensionVisuals();
		_ascensionLabel.Visible = true;
		if (_ascensionLabel.Text == string.Empty)
		{
			if (_gameMode == GameMode.Dread)
			{
				_ascensionIconDread.Visible = true;
			}
		}
		else
		{
			_ascensionIconStandard.Visible = _gameMode != GameMode.Blitz && _gameMode != GameMode.Dread;
			_ascensionIconDread.Visible = _gameMode == GameMode.Dread;
			_ascensionIconStandard.Visible = _gameMode != GameMode.Blitz && _gameMode != GameMode.Dread;
			_ascensionIconBlitz.Visible = _gameMode == GameMode.Blitz;
		}
		_tween?.Kill();
		_tween = CreateTween().SetParallel();
		_tween.TweenProperty(_icon, "scale", _enabledScale, 0.05);
	}

	public void Deselect()
	{
		_hsv.SetShaderParameter(_s, 0.3f);
		_hsv.SetShaderParameter(_v, 0.55f);
		HideAscensionVisuals();
		_tween?.Kill();
		_tween = CreateTween().SetParallel();
		_tween.TweenProperty(_icon, "scale", _disabledScale, 0.05);
	}

	private void HideAscensionVisuals()
	{
		_ascensionIconStandard.Visible = false;
		_ascensionIconDread.Visible = false;
		_ascensionIconBlitz.Visible = false;
		_ascensionLabel.Visible = false;
	}

	protected override void OnFocus()
	{
		NHoverTipSet.CreateAndShow(this, _hoverTips)?.SetGlobalPosition(GlobalPosition + new Vector2(0f, Size.Y + 20f));
		if (NControllerManager.Instance.IsUsingDirectionalNavigation)
		{
			_selectionReticle.OnSelect();
		}
	}

	protected override void OnUnfocus()
	{
		_selectionReticle.OnDeselect();
		NHoverTipSet.Remove(this);
	}

	/// <summary>
	/// Get the method information for all the methods declared in this class.
	/// This method is used by Godot to register the available methods in the editor.
	/// Do not call this method.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Never)]
	internal new static List<MethodInfo> GetGodotMethodList()
	{
		List<MethodInfo> list = new List<MethodInfo>(8);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.SetFireBlue, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.SetFireRed, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.Select, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.Deselect, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.HideAscensionVisuals, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.OnFocus, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.OnUnfocus, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
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
		if (method == MethodName.Select && args.Count == 0)
		{
			Select();
			ret = default;
			return true;
		}
		if (method == MethodName.Deselect && args.Count == 0)
		{
			Deselect();
			ret = default;
			return true;
		}
		if (method == MethodName.HideAscensionVisuals && args.Count == 0)
		{
			HideAscensionVisuals();
			ret = default;
			return true;
		}
		if (method == MethodName.OnFocus && args.Count == 0)
		{
			OnFocus();
			ret = default;
			return true;
		}
		if (method == MethodName.OnUnfocus && args.Count == 0)
		{
			OnUnfocus();
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
		if (method == MethodName.SetFireBlue)
		{
			return true;
		}
		if (method == MethodName.SetFireRed)
		{
			return true;
		}
		if (method == MethodName.Select)
		{
			return true;
		}
		if (method == MethodName.Deselect)
		{
			return true;
		}
		if (method == MethodName.HideAscensionVisuals)
		{
			return true;
		}
		if (method == MethodName.OnFocus)
		{
			return true;
		}
		if (method == MethodName.OnUnfocus)
		{
			return true;
		}
		return base.HasGodotClassMethod(in method);
	}

	/// <inheritdoc />
	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if (name == PropertyName._achievementLock)
		{
			_achievementLock = VariantUtils.ConvertTo<Control>(in value);
			return true;
		}
		if (name == PropertyName._ascensionIconStandard)
		{
			_ascensionIconStandard = VariantUtils.ConvertTo<TextureRect>(in value);
			return true;
		}
		if (name == PropertyName._ascensionIconDread)
		{
			_ascensionIconDread = VariantUtils.ConvertTo<TextureRect>(in value);
			return true;
		}
		if (name == PropertyName._ascensionIconBlitz)
		{
			_ascensionIconBlitz = VariantUtils.ConvertTo<TextureRect>(in value);
			return true;
		}
		if (name == PropertyName._ascensionLabel)
		{
			_ascensionLabel = VariantUtils.ConvertTo<MegaLabel>(in value);
			return true;
		}
		if (name == PropertyName._selectionReticle)
		{
			_selectionReticle = VariantUtils.ConvertTo<NSelectionReticle>(in value);
			return true;
		}
		if (name == PropertyName._hsv)
		{
			_hsv = VariantUtils.ConvertTo<ShaderMaterial>(in value);
			return true;
		}
		if (name == PropertyName._icon)
		{
			_icon = VariantUtils.ConvertTo<TextureRect>(in value);
			return true;
		}
		if (name == PropertyName._gameMode)
		{
			_gameMode = VariantUtils.ConvertTo<GameMode>(in value);
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
		if (name == PropertyName._achievementLock)
		{
			value = VariantUtils.CreateFrom(in _achievementLock);
			return true;
		}
		if (name == PropertyName._ascensionIconStandard)
		{
			value = VariantUtils.CreateFrom(in _ascensionIconStandard);
			return true;
		}
		if (name == PropertyName._ascensionIconDread)
		{
			value = VariantUtils.CreateFrom(in _ascensionIconDread);
			return true;
		}
		if (name == PropertyName._ascensionIconBlitz)
		{
			value = VariantUtils.CreateFrom(in _ascensionIconBlitz);
			return true;
		}
		if (name == PropertyName._ascensionLabel)
		{
			value = VariantUtils.CreateFrom(in _ascensionLabel);
			return true;
		}
		if (name == PropertyName._selectionReticle)
		{
			value = VariantUtils.CreateFrom(in _selectionReticle);
			return true;
		}
		if (name == PropertyName._hsv)
		{
			value = VariantUtils.CreateFrom(in _hsv);
			return true;
		}
		if (name == PropertyName._icon)
		{
			value = VariantUtils.CreateFrom(in _icon);
			return true;
		}
		if (name == PropertyName._gameMode)
		{
			value = VariantUtils.CreateFrom(in _gameMode);
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
	internal new static List<PropertyInfo> GetGodotPropertyList()
	{
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._achievementLock, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._ascensionIconStandard, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._ascensionIconDread, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._ascensionIconBlitz, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._ascensionLabel, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._selectionReticle, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._hsv, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._icon, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Int, PropertyName._gameMode, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._tween, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		return list;
	}

	/// <inheritdoc />
	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		base.SaveGodotObjectData(info);
		info.AddProperty(PropertyName._achievementLock, Variant.From(in _achievementLock));
		info.AddProperty(PropertyName._ascensionIconStandard, Variant.From(in _ascensionIconStandard));
		info.AddProperty(PropertyName._ascensionIconDread, Variant.From(in _ascensionIconDread));
		info.AddProperty(PropertyName._ascensionIconBlitz, Variant.From(in _ascensionIconBlitz));
		info.AddProperty(PropertyName._ascensionLabel, Variant.From(in _ascensionLabel));
		info.AddProperty(PropertyName._selectionReticle, Variant.From(in _selectionReticle));
		info.AddProperty(PropertyName._hsv, Variant.From(in _hsv));
		info.AddProperty(PropertyName._icon, Variant.From(in _icon));
		info.AddProperty(PropertyName._gameMode, Variant.From(in _gameMode));
		info.AddProperty(PropertyName._tween, Variant.From(in _tween));
	}

	/// <inheritdoc />
	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		if (info.TryGetProperty(PropertyName._achievementLock, out var value))
		{
			_achievementLock = value.As<Control>();
		}
		if (info.TryGetProperty(PropertyName._ascensionIconStandard, out var value2))
		{
			_ascensionIconStandard = value2.As<TextureRect>();
		}
		if (info.TryGetProperty(PropertyName._ascensionIconDread, out var value3))
		{
			_ascensionIconDread = value3.As<TextureRect>();
		}
		if (info.TryGetProperty(PropertyName._ascensionIconBlitz, out var value4))
		{
			_ascensionIconBlitz = value4.As<TextureRect>();
		}
		if (info.TryGetProperty(PropertyName._ascensionLabel, out var value5))
		{
			_ascensionLabel = value5.As<MegaLabel>();
		}
		if (info.TryGetProperty(PropertyName._selectionReticle, out var value6))
		{
			_selectionReticle = value6.As<NSelectionReticle>();
		}
		if (info.TryGetProperty(PropertyName._hsv, out var value7))
		{
			_hsv = value7.As<ShaderMaterial>();
		}
		if (info.TryGetProperty(PropertyName._icon, out var value8))
		{
			_icon = value8.As<TextureRect>();
		}
		if (info.TryGetProperty(PropertyName._gameMode, out var value9))
		{
			_gameMode = value9.As<GameMode>();
		}
		if (info.TryGetProperty(PropertyName._tween, out var value10))
		{
			_tween = value10.As<Tween>();
		}
	}
}
