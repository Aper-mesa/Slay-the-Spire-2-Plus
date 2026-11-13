using System;
using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Runs;

namespace MegaCrit.Sts2.Core.Nodes.Screens.CustomRun;

/// <summary>
/// The display mode dropdown in the SettingsScreen.
/// </summary>
[ScriptPath("res://src/Core/Nodes/Screens/CustomRun/NCustomGameModeDropdown.cs")]
public class NCustomGameModeDropdown : NDropdown
{
	[Signal]
	public delegate void GameModeUpdatedEventHandler();

	/// <summary>
	/// Cached StringNames for the methods contained in this class, for fast lookup.
	/// </summary>
	public new class MethodName : NDropdown.MethodName
	{
		/// <summary>
		/// Cached name for the '_Ready' method.
		/// </summary>
		public new static readonly StringName _Ready = "_Ready";

		/// <summary>
		/// Cached name for the 'AddDropdownItem' method.
		/// </summary>
		public static readonly StringName AddDropdownItem = "AddDropdownItem";

		/// <summary>
		/// Cached name for the 'OnDropdownItemSelected' method.
		/// </summary>
		public static readonly StringName OnDropdownItemSelected = "OnDropdownItemSelected";

		/// <summary>
		/// Cached name for the 'SetSelectedGameMode' method.
		/// </summary>
		public static readonly StringName SetSelectedGameMode = "SetSelectedGameMode";

		/// <summary>
		/// Cached name for the 'OnDisable' method.
		/// </summary>
		public new static readonly StringName OnDisable = "OnDisable";

		/// <summary>
		/// Cached name for the 'OnEnable' method.
		/// </summary>
		public new static readonly StringName OnEnable = "OnEnable";

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
	public new class PropertyName : NDropdown.PropertyName
	{
		/// <summary>
		/// Cached name for the 'SelectedGameMode' property.
		/// </summary>
		public static readonly StringName SelectedGameMode = "SelectedGameMode";

		/// <summary>
		/// Cached name for the '_selectionReticle' field.
		/// </summary>
		public static readonly StringName _selectionReticle = "_selectionReticle";

		/// <summary>
		/// Cached name for the '_dropdownContainer' field.
		/// </summary>
		public new static readonly StringName _dropdownContainer = "_dropdownContainer";
	}

	/// <summary>
	/// Cached StringNames for the signals contained in this class, for fast lookup.
	/// </summary>
	public new class SignalName : NDropdown.SignalName
	{
		/// <summary>
		/// Cached name for the 'GameModeUpdated' signal.
		/// </summary>
		public static readonly StringName GameModeUpdated = "GameModeUpdated";
	}

	private NSelectionReticle _selectionReticle;

	private NDropdownContainer _dropdownContainer;

	private GameModeUpdatedEventHandler backing_GameModeUpdated;

	private static string DropdownItemScenePath => SceneHelper.GetScenePath("screens/custom_run/custom_game_mode_dropdown_item");

	public static IEnumerable<string> AssetPaths => new _003C_003Ez__ReadOnlySingleElementList<string>(DropdownItemScenePath);

	public GameMode SelectedGameMode { get; private set; }

	/// <inheritdoc cref="T:MegaCrit.Sts2.Core.Nodes.Screens.CustomRun.NCustomGameModeDropdown.GameModeUpdatedEventHandler" />
	public event GameModeUpdatedEventHandler GameModeUpdated
	{
		add
		{
			backing_GameModeUpdated = (GameModeUpdatedEventHandler)Delegate.Combine(backing_GameModeUpdated, value);
		}
		remove
		{
			backing_GameModeUpdated = (GameModeUpdatedEventHandler)Delegate.Remove(backing_GameModeUpdated, value);
		}
	}

	public override void _Ready()
	{
		ConnectSignals();
		_dropdownContainer = GetNode<NDropdownContainer>("Container/DropdownContainer");
		_currentOptionLabel.SetTextAutoSize(GetGameModeLoc(GameMode.Standard).GetFormattedText());
		_selectionReticle = GetNode<NSelectionReticle>("SelectionReticle");
	}

	public void SetOptions(List<GameMode> gameModes)
	{
		ClearDropdownItems();
		foreach (GameMode gameMode in gameModes)
		{
			AddDropdownItem(gameMode);
		}
		if (!gameModes.Contains(SelectedGameMode))
		{
			SetSelectedGameMode(GameMode.Standard);
		}
		_dropdownContainer.RefreshLayout();
	}

	private void AddDropdownItem(GameMode gameMode)
	{
		NCustomGameModeDropdownItem nCustomGameModeDropdownItem = PreloadManager.Cache.GetScene(DropdownItemScenePath).Instantiate<NCustomGameModeDropdownItem>(PackedScene.GenEditState.Disabled);
		_dropdownItems.AddChildSafely(nCustomGameModeDropdownItem);
		nCustomGameModeDropdownItem.Connect(NDropdownItem.SignalName.Selected, Callable.From<NCustomGameModeDropdownItem>(OnDropdownItemSelected));
		nCustomGameModeDropdownItem.Init(gameMode);
	}

	private void OnDropdownItemSelected(NCustomGameModeDropdownItem item)
	{
		CloseDropdown();
		SetSelectedGameMode(item.GameMode);
	}

	public void SetSelectedGameMode(GameMode gameMode)
	{
		if (SelectedGameMode != gameMode)
		{
			SelectedGameMode = gameMode;
			_currentOptionLabel.SetTextAutoSize(GetGameModeLoc(gameMode).GetFormattedText());
			EmitSignalGameModeUpdated();
		}
	}

	protected override void OnDisable()
	{
		Modulate = StsColors.gray;
	}

	protected override void OnEnable()
	{
		Modulate = Colors.White;
	}

	public static LocString GetGameModeLoc(GameMode gameMode)
	{
		return new LocString("main_menu_ui", gameMode.ToString().ToUpperInvariant() + ".title");
	}

	protected override void OnFocus()
	{
		base.OnFocus();
		_currentOptionHighlight.Modulate = new Color("3C5B6B");
		if (NControllerManager.Instance.IsUsingDirectionalNavigation)
		{
			_selectionReticle.OnSelect();
		}
	}

	protected override void OnUnfocus()
	{
		_selectionReticle.OnDeselect();
		_currentOptionHighlight.Modulate = new Color("2C434F");
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
		list.Add(new MethodInfo(MethodName.AddDropdownItem, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.Int, "gameMode", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
		}, null));
		list.Add(new MethodInfo(MethodName.OnDropdownItemSelected, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.Object, "item", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Control"), exported: false)
		}, null));
		list.Add(new MethodInfo(MethodName.SetSelectedGameMode, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.Int, "gameMode", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
		}, null));
		list.Add(new MethodInfo(MethodName.OnDisable, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.OnEnable, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
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
		if (method == MethodName.AddDropdownItem && args.Count == 1)
		{
			AddDropdownItem(VariantUtils.ConvertTo<GameMode>(in args[0]));
			ret = default;
			return true;
		}
		if (method == MethodName.OnDropdownItemSelected && args.Count == 1)
		{
			OnDropdownItemSelected(VariantUtils.ConvertTo<NCustomGameModeDropdownItem>(in args[0]));
			ret = default;
			return true;
		}
		if (method == MethodName.SetSelectedGameMode && args.Count == 1)
		{
			SetSelectedGameMode(VariantUtils.ConvertTo<GameMode>(in args[0]));
			ret = default;
			return true;
		}
		if (method == MethodName.OnDisable && args.Count == 0)
		{
			OnDisable();
			ret = default;
			return true;
		}
		if (method == MethodName.OnEnable && args.Count == 0)
		{
			OnEnable();
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
		if (method == MethodName.AddDropdownItem)
		{
			return true;
		}
		if (method == MethodName.OnDropdownItemSelected)
		{
			return true;
		}
		if (method == MethodName.SetSelectedGameMode)
		{
			return true;
		}
		if (method == MethodName.OnDisable)
		{
			return true;
		}
		if (method == MethodName.OnEnable)
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
		if (name == PropertyName.SelectedGameMode)
		{
			SelectedGameMode = VariantUtils.ConvertTo<GameMode>(in value);
			return true;
		}
		if (name == PropertyName._selectionReticle)
		{
			_selectionReticle = VariantUtils.ConvertTo<NSelectionReticle>(in value);
			return true;
		}
		if (name == PropertyName._dropdownContainer)
		{
			_dropdownContainer = VariantUtils.ConvertTo<NDropdownContainer>(in value);
			return true;
		}
		return base.SetGodotClassPropertyValue(in name, in value);
	}

	/// <inheritdoc />
	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool GetGodotClassPropertyValue(in godot_string_name name, out godot_variant value)
	{
		if (name == PropertyName.SelectedGameMode)
		{
			value = VariantUtils.CreateFrom<GameMode>(SelectedGameMode);
			return true;
		}
		if (name == PropertyName._selectionReticle)
		{
			value = VariantUtils.CreateFrom(in _selectionReticle);
			return true;
		}
		if (name == PropertyName._dropdownContainer)
		{
			value = VariantUtils.CreateFrom(in _dropdownContainer);
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
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._selectionReticle, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._dropdownContainer, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Int, PropertyName.SelectedGameMode, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		return list;
	}

	/// <inheritdoc />
	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		base.SaveGodotObjectData(info);
		info.AddProperty(PropertyName.SelectedGameMode, Variant.From<GameMode>(SelectedGameMode));
		info.AddProperty(PropertyName._selectionReticle, Variant.From(in _selectionReticle));
		info.AddProperty(PropertyName._dropdownContainer, Variant.From(in _dropdownContainer));
		info.AddSignalEventDelegate(SignalName.GameModeUpdated, backing_GameModeUpdated);
	}

	/// <inheritdoc />
	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		if (info.TryGetProperty(PropertyName.SelectedGameMode, out var value))
		{
			SelectedGameMode = value.As<GameMode>();
		}
		if (info.TryGetProperty(PropertyName._selectionReticle, out var value2))
		{
			_selectionReticle = value2.As<NSelectionReticle>();
		}
		if (info.TryGetProperty(PropertyName._dropdownContainer, out var value3))
		{
			_dropdownContainer = value3.As<NDropdownContainer>();
		}
		if (info.TryGetSignalEventDelegate<GameModeUpdatedEventHandler>(SignalName.GameModeUpdated, out var value4))
		{
			backing_GameModeUpdated = value4;
		}
	}

	/// <summary>
	/// Get the signal information for all the signals declared in this class.
	/// This method is used by Godot to register the available signals in the editor.
	/// Do not call this method.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Never)]
	internal new static List<MethodInfo> GetGodotSignalList()
	{
		List<MethodInfo> list = new List<MethodInfo>(1);
		list.Add(new MethodInfo(SignalName.GameModeUpdated, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		return list;
	}

	protected void EmitSignalGameModeUpdated()
	{
		EmitSignal(SignalName.GameModeUpdated);
	}

	/// <inheritdoc />
	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RaiseGodotClassSignalCallbacks(in godot_string_name signal, NativeVariantPtrArgs args)
	{
		if (signal == SignalName.GameModeUpdated && args.Count == 0)
		{
			backing_GameModeUpdated?.Invoke();
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
		if (signal == SignalName.GameModeUpdated)
		{
			return true;
		}
		return base.HasGodotClassSignal(in signal);
	}
}
