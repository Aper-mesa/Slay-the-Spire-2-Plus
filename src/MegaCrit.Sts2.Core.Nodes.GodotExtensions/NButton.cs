using System;
using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Nodes.CommonUi;

namespace MegaCrit.Sts2.Core.Nodes.GodotExtensions;

/// <summary>
/// A Base Button class which doesn't rely on Godot's Button class.
/// </summary>
[ScriptPath("res://src/Core/Nodes/GodotExtensions/NButton.cs")]
public class NButton : NClickableControl
{
	/// <summary>
	/// Cached StringNames for the methods contained in this class, for fast lookup.
	/// </summary>
	public new class MethodName : NClickableControl.MethodName
	{
		/// <summary>
		/// Cached name for the '_Ready' method.
		/// </summary>
		public new static readonly StringName _Ready = "_Ready";

		/// <summary>
		/// Cached name for the 'ConnectSignals' method.
		/// </summary>
		public new static readonly StringName ConnectSignals = "ConnectSignals";

		/// <summary>
		/// Cached name for the 'GetControllerIconNode' method.
		/// </summary>
		public static readonly StringName GetControllerIconNode = "GetControllerIconNode";

		/// <summary>
		/// Cached name for the '_Input' method.
		/// </summary>
		public new static readonly StringName _Input = "_Input";

		/// <summary>
		/// Cached name for the 'OnPress' method.
		/// </summary>
		public new static readonly StringName OnPress = "OnPress";

		/// <summary>
		/// Cached name for the 'OnFocus' method.
		/// </summary>
		public new static readonly StringName OnFocus = "OnFocus";

		/// <summary>
		/// Cached name for the 'OnEnable' method.
		/// </summary>
		public new static readonly StringName OnEnable = "OnEnable";

		/// <summary>
		/// Cached name for the 'OnDisable' method.
		/// </summary>
		public new static readonly StringName OnDisable = "OnDisable";

		/// <summary>
		/// Cached name for the 'UpdateControllerButton' method.
		/// </summary>
		public static readonly StringName UpdateControllerButton = "UpdateControllerButton";

		/// <summary>
		/// Cached name for the 'RegisterHotkeys' method.
		/// </summary>
		public static readonly StringName RegisterHotkeys = "RegisterHotkeys";

		/// <summary>
		/// Cached name for the 'UnregisterHotkeys' method.
		/// </summary>
		public static readonly StringName UnregisterHotkeys = "UnregisterHotkeys";

		/// <summary>
		/// Cached name for the 'ForgetRemovedPriorities' method.
		/// </summary>
		public static readonly StringName ForgetRemovedPriorities = "ForgetRemovedPriorities";

		/// <summary>
		/// Cached name for the '_EnterTree' method.
		/// </summary>
		public new static readonly StringName _EnterTree = "_EnterTree";

		/// <summary>
		/// Cached name for the '_ExitTree' method.
		/// </summary>
		public new static readonly StringName _ExitTree = "_ExitTree";
	}

	/// <summary>
	/// Cached StringNames for the properties and fields contained in this class, for fast lookup.
	/// </summary>
	public new class PropertyName : NClickableControl.PropertyName
	{
		/// <summary>
		/// Cached name for the 'ClickedSfx' property.
		/// </summary>
		public static readonly StringName ClickedSfx = "ClickedSfx";

		/// <summary>
		/// Cached name for the 'HoveredSfx' property.
		/// </summary>
		public static readonly StringName HoveredSfx = "HoveredSfx";

		/// <summary>
		/// Cached name for the 'Hotkeys' property.
		/// </summary>
		public static readonly StringName Hotkeys = "Hotkeys";

		/// <summary>
		/// Cached name for the 'ControllerIconHotkey' property.
		/// </summary>
		public static readonly StringName ControllerIconHotkey = "ControllerIconHotkey";

		/// <summary>
		/// Cached name for the 'HasControllerHotkey' property.
		/// </summary>
		public static readonly StringName HasControllerHotkey = "HasControllerHotkey";

		/// <summary>
		/// Cached name for the '_hotkeyIcon' field.
		/// </summary>
		public static readonly StringName _hotkeyIcon = "_hotkeyIcon";

		/// <summary>
		/// Cached name for the '_removedHotkeys' field.
		/// </summary>
		public static readonly StringName _removedHotkeys = "_removedHotkeys";

		/// <summary>
		/// Cached name for the '_removedPressedSequences' field.
		/// </summary>
		public static readonly StringName _removedPressedSequences = "_removedPressedSequences";

		/// <summary>
		/// Cached name for the '_removedReleasedSequences' field.
		/// </summary>
		public static readonly StringName _removedReleasedSequences = "_removedReleasedSequences";
	}

	/// <summary>
	/// Cached StringNames for the signals contained in this class, for fast lookup.
	/// </summary>
	public new class SignalName : NClickableControl.SignalName
	{
	}

	protected NHotkeyIcon? _hotkeyIcon;

	private string[]? _removedHotkeys;

	private long[]? _removedPressedSequences;

	private long[]? _removedReleasedSequences;

	protected virtual string? ClickedSfx => "event:/sfx/ui/clicks/ui_click";

	protected virtual string? HoveredSfx => "event:/sfx/ui/clicks/ui_hover";

	protected virtual string[] Hotkeys => Array.Empty<string>();

	protected virtual string? ControllerIconHotkey
	{
		get
		{
			if (Hotkeys.Length == 0)
			{
				return null;
			}
			return Hotkeys[0];
		}
	}

	private bool HasControllerHotkey => Hotkeys.Length != 0;

	public override void _Ready()
	{
		if (GetType() != typeof(NButton))
		{
			Log.Error($"{GetType()}");
			throw new InvalidOperationException("Don't call base._Ready()! Call ConnectSignals() instead.");
		}
		ConnectSignals();
	}

	protected override void ConnectSignals()
	{
		base.ConnectSignals();
		if (HasControllerHotkey)
		{
			RegisterHotkeys();
		}
		GetControllerIconNode();
		UpdateControllerButton();
	}

	protected virtual void GetControllerIconNode()
	{
		_hotkeyIcon = GetNodeOrNull<NHotkeyIcon>("%HotkeyIcon");
	}

	/// <summary>
	/// Detects input actions
	/// </summary>
	/// <param name="inputEvent"></param>
	public override void _Input(InputEvent inputEvent)
	{
		CheckMouseDragThreshold(inputEvent);
	}

	protected override void OnPress()
	{
		if (ClickedSfx != null)
		{
			SfxCmd.Play(ClickedSfx);
		}
	}

	protected override void OnFocus()
	{
		if (HoveredSfx != null)
		{
			SfxCmd.Play(HoveredSfx);
		}
	}

	protected override void OnEnable()
	{
		RegisterHotkeys();
		UpdateControllerButton();
	}

	protected override void OnDisable()
	{
		UnregisterHotkeys();
		UpdateControllerButton();
	}

	protected void UpdateControllerButton()
	{
		NControllerManager instance = NControllerManager.Instance;
		if (instance != null && _hotkeyIcon != null)
		{
			_hotkeyIcon.Visible = instance.IsUsingDirectionalNavigation && _isEnabled;
			if (ControllerIconHotkey != null)
			{
				_hotkeyIcon.UpdateInput(ControllerIconHotkey);
			}
		}
	}

	protected void RegisterHotkeys()
	{
		RegisterHotkeys(restorePriority: false);
	}

	private void RegisterHotkeys(bool restorePriority)
	{
		if (HasControllerHotkey && _isEnabled)
		{
			string[] hotkeys = Hotkeys;
			string[] array = (restorePriority ? _removedHotkeys : null);
			for (int i = 0; i < hotkeys.Length; i++)
			{
				int num = ((array != null) ? Array.IndexOf(array, hotkeys[i]) : (-1));
				long sequence = ((num >= 0) ? _removedPressedSequences[num] : 0);
				long sequence2 = ((num >= 0) ? _removedReleasedSequences[num] : 0);
				NHotkeyManager.Instance.PushHotkeyPressedBinding(hotkeys[i], OnPressHandler, sequence);
				NHotkeyManager.Instance.PushHotkeyReleasedBinding(hotkeys[i], OnReleaseHandler, sequence2);
			}
		}
		ForgetRemovedPriorities();
	}

	protected void UnregisterHotkeys()
	{
		ForgetRemovedPriorities();
		if (HasControllerHotkey)
		{
			string[] hotkeys = Hotkeys;
			long[] array = new long[hotkeys.Length];
			long[] array2 = new long[hotkeys.Length];
			for (int i = 0; i < hotkeys.Length; i++)
			{
				array[i] = NHotkeyManager.Instance.RemoveHotkeyPressedBinding(hotkeys[i], OnPressHandler);
				array2[i] = NHotkeyManager.Instance.RemoveHotkeyReleasedBinding(hotkeys[i], OnReleaseHandler);
			}
			_removedHotkeys = hotkeys;
			_removedPressedSequences = array;
			_removedReleasedSequences = array2;
		}
	}

	private void ForgetRemovedPriorities()
	{
		_removedHotkeys = null;
		_removedPressedSequences = null;
		_removedReleasedSequences = null;
	}

	public override void _EnterTree()
	{
		if (NControllerManager.Instance != null)
		{
			NControllerManager.Instance.Connect(NControllerManager.SignalName.MouseDetected, Callable.From(UpdateControllerButton));
			NControllerManager.Instance.Connect(NControllerManager.SignalName.ControllerDetected, Callable.From(UpdateControllerButton));
		}
		if (NInputManager.Instance != null)
		{
			NInputManager.Instance.Connect(NInputManager.SignalName.InputRebound, Callable.From(UpdateControllerButton));
		}
		RegisterHotkeys(restorePriority: true);
	}

	public override void _ExitTree()
	{
		if (NControllerManager.Instance != null)
		{
			NControllerManager.Instance.Disconnect(NControllerManager.SignalName.ControllerDetected, Callable.From(UpdateControllerButton));
			NControllerManager.Instance.Disconnect(NControllerManager.SignalName.MouseDetected, Callable.From(UpdateControllerButton));
		}
		if (NInputManager.Instance != null)
		{
			NInputManager.Instance.Disconnect(NInputManager.SignalName.InputRebound, Callable.From(UpdateControllerButton));
		}
		UnregisterHotkeys();
	}

	/// <summary>
	/// Get the method information for all the methods declared in this class.
	/// This method is used by Godot to register the available methods in the editor.
	/// Do not call this method.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Never)]
	internal new static List<MethodInfo> GetGodotMethodList()
	{
		List<MethodInfo> list = new List<MethodInfo>(15);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.ConnectSignals, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.GetControllerIconNode, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName._Input, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.Object, "inputEvent", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("InputEvent"), exported: false)
		}, null));
		list.Add(new MethodInfo(MethodName.OnPress, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.OnFocus, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.OnEnable, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.OnDisable, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.UpdateControllerButton, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.RegisterHotkeys, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.RegisterHotkeys, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.Bool, "restorePriority", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
		}, null));
		list.Add(new MethodInfo(MethodName.UnregisterHotkeys, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.ForgetRemovedPriorities, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName._EnterTree, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName._ExitTree, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
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
		if (method == MethodName.ConnectSignals && args.Count == 0)
		{
			ConnectSignals();
			ret = default;
			return true;
		}
		if (method == MethodName.GetControllerIconNode && args.Count == 0)
		{
			GetControllerIconNode();
			ret = default;
			return true;
		}
		if (method == MethodName._Input && args.Count == 1)
		{
			_Input(VariantUtils.ConvertTo<InputEvent>(in args[0]));
			ret = default;
			return true;
		}
		if (method == MethodName.OnPress && args.Count == 0)
		{
			OnPress();
			ret = default;
			return true;
		}
		if (method == MethodName.OnFocus && args.Count == 0)
		{
			OnFocus();
			ret = default;
			return true;
		}
		if (method == MethodName.OnEnable && args.Count == 0)
		{
			OnEnable();
			ret = default;
			return true;
		}
		if (method == MethodName.OnDisable && args.Count == 0)
		{
			OnDisable();
			ret = default;
			return true;
		}
		if (method == MethodName.UpdateControllerButton && args.Count == 0)
		{
			UpdateControllerButton();
			ret = default;
			return true;
		}
		if (method == MethodName.RegisterHotkeys && args.Count == 0)
		{
			RegisterHotkeys();
			ret = default;
			return true;
		}
		if (method == MethodName.RegisterHotkeys && args.Count == 1)
		{
			RegisterHotkeys(VariantUtils.ConvertTo<bool>(in args[0]));
			ret = default;
			return true;
		}
		if (method == MethodName.UnregisterHotkeys && args.Count == 0)
		{
			UnregisterHotkeys();
			ret = default;
			return true;
		}
		if (method == MethodName.ForgetRemovedPriorities && args.Count == 0)
		{
			ForgetRemovedPriorities();
			ret = default;
			return true;
		}
		if (method == MethodName._EnterTree && args.Count == 0)
		{
			_EnterTree();
			ret = default;
			return true;
		}
		if (method == MethodName._ExitTree && args.Count == 0)
		{
			_ExitTree();
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
		if (method == MethodName.ConnectSignals)
		{
			return true;
		}
		if (method == MethodName.GetControllerIconNode)
		{
			return true;
		}
		if (method == MethodName._Input)
		{
			return true;
		}
		if (method == MethodName.OnPress)
		{
			return true;
		}
		if (method == MethodName.OnFocus)
		{
			return true;
		}
		if (method == MethodName.OnEnable)
		{
			return true;
		}
		if (method == MethodName.OnDisable)
		{
			return true;
		}
		if (method == MethodName.UpdateControllerButton)
		{
			return true;
		}
		if (method == MethodName.RegisterHotkeys)
		{
			return true;
		}
		if (method == MethodName.UnregisterHotkeys)
		{
			return true;
		}
		if (method == MethodName.ForgetRemovedPriorities)
		{
			return true;
		}
		if (method == MethodName._EnterTree)
		{
			return true;
		}
		if (method == MethodName._ExitTree)
		{
			return true;
		}
		return base.HasGodotClassMethod(in method);
	}

	/// <inheritdoc />
	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if (name == PropertyName._hotkeyIcon)
		{
			_hotkeyIcon = VariantUtils.ConvertTo<NHotkeyIcon>(in value);
			return true;
		}
		if (name == PropertyName._removedHotkeys)
		{
			_removedHotkeys = VariantUtils.ConvertTo<string[]>(in value);
			return true;
		}
		if (name == PropertyName._removedPressedSequences)
		{
			_removedPressedSequences = VariantUtils.ConvertTo<long[]>(in value);
			return true;
		}
		if (name == PropertyName._removedReleasedSequences)
		{
			_removedReleasedSequences = VariantUtils.ConvertTo<long[]>(in value);
			return true;
		}
		return base.SetGodotClassPropertyValue(in name, in value);
	}

	/// <inheritdoc />
	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool GetGodotClassPropertyValue(in godot_string_name name, out godot_variant value)
	{
		string from;
		if (name == PropertyName.ClickedSfx)
		{
			from = ClickedSfx;
			value = VariantUtils.CreateFrom(in from);
			return true;
		}
		if (name == PropertyName.HoveredSfx)
		{
			from = HoveredSfx;
			value = VariantUtils.CreateFrom(in from);
			return true;
		}
		if (name == PropertyName.Hotkeys)
		{
			value = VariantUtils.CreateFrom<string[]>(Hotkeys);
			return true;
		}
		if (name == PropertyName.ControllerIconHotkey)
		{
			from = ControllerIconHotkey;
			value = VariantUtils.CreateFrom(in from);
			return true;
		}
		if (name == PropertyName.HasControllerHotkey)
		{
			value = VariantUtils.CreateFrom<bool>(HasControllerHotkey);
			return true;
		}
		if (name == PropertyName._hotkeyIcon)
		{
			value = VariantUtils.CreateFrom(in _hotkeyIcon);
			return true;
		}
		if (name == PropertyName._removedHotkeys)
		{
			value = VariantUtils.CreateFrom(in _removedHotkeys);
			return true;
		}
		if (name == PropertyName._removedPressedSequences)
		{
			value = VariantUtils.CreateFrom(in _removedPressedSequences);
			return true;
		}
		if (name == PropertyName._removedReleasedSequences)
		{
			value = VariantUtils.CreateFrom(in _removedReleasedSequences);
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
		list.Add(new PropertyInfo(Variant.Type.String, PropertyName.ClickedSfx, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.String, PropertyName.HoveredSfx, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.PackedStringArray, PropertyName.Hotkeys, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.String, PropertyName.ControllerIconHotkey, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Bool, PropertyName.HasControllerHotkey, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._hotkeyIcon, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.PackedStringArray, PropertyName._removedHotkeys, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.PackedInt64Array, PropertyName._removedPressedSequences, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.PackedInt64Array, PropertyName._removedReleasedSequences, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		return list;
	}

	/// <inheritdoc />
	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		base.SaveGodotObjectData(info);
		info.AddProperty(PropertyName._hotkeyIcon, Variant.From(in _hotkeyIcon));
		info.AddProperty(PropertyName._removedHotkeys, Variant.From(in _removedHotkeys));
		info.AddProperty(PropertyName._removedPressedSequences, Variant.From(in _removedPressedSequences));
		info.AddProperty(PropertyName._removedReleasedSequences, Variant.From(in _removedReleasedSequences));
	}

	/// <inheritdoc />
	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		if (info.TryGetProperty(PropertyName._hotkeyIcon, out var value))
		{
			_hotkeyIcon = value.As<NHotkeyIcon>();
		}
		if (info.TryGetProperty(PropertyName._removedHotkeys, out var value2))
		{
			_removedHotkeys = value2.As<string[]>();
		}
		if (info.TryGetProperty(PropertyName._removedPressedSequences, out var value3))
		{
			_removedPressedSequences = value3.As<long[]>();
		}
		if (info.TryGetProperty(PropertyName._removedReleasedSequences, out var value4))
		{
			_removedReleasedSequences = value4.As<long[]>();
		}
	}
}
