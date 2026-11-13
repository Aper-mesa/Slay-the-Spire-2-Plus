using System;
using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.ControllerInput;
using MegaCrit.Sts2.Core.Nodes.Debug;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;

namespace MegaCrit.Sts2.Core.Nodes.CommonUi;

/// <summary>
/// Class that manages Hotkeys/shortcuts for various buttons and UI in the game.
/// Manages which buttons get priority if multiple of them share the same Hotkey
/// (typically the last one added gets priority)
/// </summary>
[ScriptPath("res://src/Core/Nodes/CommonUi/NHotkeyManager.cs")]
public class NHotkeyManager : Node
{
	/// <summary>
	/// A registered binding. <see cref="P:MegaCrit.Sts2.Core.Nodes.CommonUi.NHotkeyManager.Binding.Sequence" /> is what orders it against the other bindings on
	/// the same hotkey, highest wins. Handing the same sequence back to Push after a Remove puts the
	/// binding back exactly where it was, whatever else registered or unregistered meanwhile.
	/// </summary>
	private readonly record struct Binding(long Sequence, Action Action);

	/// <summary>
	/// Cached StringNames for the methods contained in this class, for fast lookup.
	/// </summary>
	public new class MethodName : Node.MethodName
	{
		/// <summary>
		/// Cached name for the 'ClearHotkeys' method.
		/// </summary>
		public static readonly StringName ClearHotkeys = "ClearHotkeys";

		/// <summary>
		/// Cached name for the 'AddBlockingScreen' method.
		/// </summary>
		public static readonly StringName AddBlockingScreen = "AddBlockingScreen";

		/// <summary>
		/// Cached name for the 'RemoveBlockingScreen' method.
		/// </summary>
		public static readonly StringName RemoveBlockingScreen = "RemoveBlockingScreen";

		/// <summary>
		/// Cached name for the '_UnhandledInput' method.
		/// </summary>
		public new static readonly StringName _UnhandledInput = "_UnhandledInput";
	}

	/// <summary>
	/// Cached StringNames for the properties and fields contained in this class, for fast lookup.
	/// </summary>
	public new class PropertyName : Node.PropertyName
	{
		/// <summary>
		/// Cached name for the '_lastSequence' field.
		/// </summary>
		public static readonly StringName _lastSequence = "_lastSequence";
	}

	/// <summary>
	/// Cached StringNames for the signals contained in this class, for fast lookup.
	/// </summary>
	public new class SignalName : Node.SignalName
	{
	}

	private readonly Dictionary<StringName, List<Binding>> _hotkeyPressedBindings = new Dictionary<StringName, List<Binding>>();

	private readonly Dictionary<StringName, List<Binding>> _hotkeyReleasedBindings = new Dictionary<StringName, List<Binding>>();

	private readonly Dictionary<Node, Action> _blockingScreens = new Dictionary<Node, Action>();

	/// <summary>Last sequence handed out. Only ever increases, so a fresh binding always outranks.</summary>
	private long _lastSequence;

	/// <summary>Sequence value meaning "no remembered position, give this top priority".</summary>
	public const long noSequence = 0L;

	public static NHotkeyManager? Instance
	{
		get
		{
			NHotkeyManager nHotkeyManager = InstanceForTest;
			if (nHotkeyManager == null)
			{
				if (NGame.Instance == null)
				{
					return null;
				}
				nHotkeyManager = NGame.Instance.HotkeyManager;
			}
			return nHotkeyManager;
		}
	}

	/// <summary>
	/// Replaces the game's manager in tests, which run without an <see cref="T:MegaCrit.Sts2.Core.Nodes.NGame" />. Null in the game.
	/// </summary>
	internal static NHotkeyManager? InstanceForTest { get; set; }

	public void ClearHotkeys()
	{
		_hotkeyPressedBindings.Clear();
		_hotkeyReleasedBindings.Clear();
		_blockingScreens.Clear();
	}

	/// <summary>
	/// Registers a binding at top priority. Pass a <paramref name="sequence" /> from
	/// <see cref="M:MegaCrit.Sts2.Core.Nodes.CommonUi.NHotkeyManager.RemoveHotkeyPressedBinding(System.String,System.Action)" /> to put a binding back at the priority it held.
	/// </summary>
	public void PushHotkeyPressedBinding(string hotkey, Action action, long sequence = 0L)
	{
		Insert(_hotkeyPressedBindings, hotkey, action, sequence, ref _lastSequence);
	}

	/// <returns>
	/// The sequence the binding held, or <see cref="F:MegaCrit.Sts2.Core.Nodes.CommonUi.NHotkeyManager.noSequence" /> if it was not registered. Pass it
	/// back to <see cref="M:MegaCrit.Sts2.Core.Nodes.CommonUi.NHotkeyManager.PushHotkeyPressedBinding(System.String,System.Action,System.Int64)" /> to restore the binding at its original priority.
	/// </returns>
	public long RemoveHotkeyPressedBinding(string hotkey, Action action)
	{
		return Remove(_hotkeyPressedBindings, hotkey, action);
	}

	/// <summary>
	/// Registers a binding at top priority. Pass a <paramref name="sequence" /> from
	/// <see cref="M:MegaCrit.Sts2.Core.Nodes.CommonUi.NHotkeyManager.RemoveHotkeyReleasedBinding(System.String,System.Action)" /> to put a binding back at the priority it held.
	/// </summary>
	public void PushHotkeyReleasedBinding(string hotkey, Action action, long sequence = 0L)
	{
		Insert(_hotkeyReleasedBindings, hotkey, action, sequence, ref _lastSequence);
	}

	/// <returns>
	/// The sequence the binding held, or <see cref="F:MegaCrit.Sts2.Core.Nodes.CommonUi.NHotkeyManager.noSequence" /> if it was not registered. Pass it
	/// back to <see cref="M:MegaCrit.Sts2.Core.Nodes.CommonUi.NHotkeyManager.PushHotkeyReleasedBinding(System.String,System.Action,System.Int64)" /> to restore the binding at its original priority.
	/// </returns>
	public long RemoveHotkeyReleasedBinding(string hotkey, Action action)
	{
		return Remove(_hotkeyReleasedBindings, hotkey, action);
	}

	/// <summary>
	/// The binding that currently wins <paramref name="hotkey" />, or null if nothing is registered.
	/// </summary>
	public Action? GetPrioritizedPressedBinding(string hotkey)
	{
		return GetPrioritized(_hotkeyPressedBindings, hotkey);
	}

	/// <summary>
	/// The binding that currently wins the release of <paramref name="hotkey" />, or null if nothing
	/// is registered.
	/// </summary>
	public Action? GetPrioritizedReleasedBinding(string hotkey)
	{
		return GetPrioritized(_hotkeyReleasedBindings, hotkey);
	}

	private static Action? GetPrioritized(Dictionary<StringName, List<Binding>> bindings, string hotkey)
	{
		if (!bindings.TryGetValue(hotkey, out List<Binding> value) || value.Count <= 0)
		{
			return null;
		}
		List<Binding> list = value;
		return list[list.Count - 1].Action;
	}

	private static void Insert(Dictionary<StringName, List<Binding>> bindings, string hotkey, Action action, long sequence, ref long lastSequence)
	{
		if (!bindings.TryGetValue(hotkey, out List<Binding> value))
		{
			value = new List<Binding>();
			bindings.Add(hotkey, value);
		}
		if (value.FindIndex((Binding binding) => binding.Action == action) < 0)
		{
			Binding item = new Binding((sequence == 0L) ? (++lastSequence) : sequence, action);
			int num = value.Count;
			while (num > 0 && value[num - 1].Sequence > item.Sequence)
			{
				num--;
			}
			value.Insert(num, item);
		}
	}

	private static long Remove(Dictionary<StringName, List<Binding>> bindings, string hotkey, Action action)
	{
		if (!bindings.TryGetValue(hotkey, out List<Binding> value))
		{
			return 0L;
		}
		long result = 0L;
		int num = value.FindIndex((Binding binding) => binding.Action == action);
		if (num >= 0)
		{
			result = value[num].Sequence;
			value.RemoveAt(num);
		}
		if (value.Count == 0)
		{
			bindings.Remove(hotkey);
		}
		return result;
	}

	/// <summary>
	/// we want to block all previous hotkeys while this screen is open
	/// New hotkeys can be added on top though.
	/// ex: we don't want to be able to hotkey to view our draw pile if the pause menu is open
	/// </summary>
	/// <param name="screen">Screen that is block all previous hotkey input</param>
	public void AddBlockingScreen(Node screen)
	{
		Action action = () =>
		{
		};
		string[] allInputs = MegaInput.AllInputs;
		foreach (string hotkey in allInputs)
		{
			PushHotkeyPressedBinding(hotkey, action, 0L);
		}
		string[] allInputs2 = MegaInput.AllInputs;
		foreach (string hotkey2 in allInputs2)
		{
			PushHotkeyReleasedBinding(hotkey2, action, 0L);
		}
		_blockingScreens.Add(screen, action);
	}

	/// <summary>
	/// Removes the all hotkey blockers related to a screen.
	/// </summary>
	/// <param name="screen">Screen that want to remove the hotkey blockage from</param>
	public void RemoveBlockingScreen(Node screen)
	{
		if (_blockingScreens.TryGetValue(screen, out Action value))
		{
			string[] allInputs = MegaInput.AllInputs;
			foreach (string hotkey in allInputs)
			{
				RemoveHotkeyPressedBinding(hotkey, value);
			}
			string[] allInputs2 = MegaInput.AllInputs;
			foreach (string hotkey2 in allInputs2)
			{
				RemoveHotkeyReleasedBinding(hotkey2, value);
			}
			_blockingScreens.Remove(screen);
		}
	}

	public override void _UnhandledInput(InputEvent inputEvent)
	{
		if (!NGame.IsGameFocusedWindow() || NDevConsole.IsConsoleVisible)
		{
			return;
		}
		Viewport viewport = GetViewport();
		Control control = viewport?.GuiGetFocusOwner();
		if (control != null && ((control is LineEdit lineEdit && lineEdit.IsEditing()) || (control.GetParent() is NMegaTextEdit nMegaTextEdit && nMegaTextEdit.IsEditing())))
		{
			return;
		}
		foreach (KeyValuePair<StringName, List<Binding>> hotkeyPressedBinding in _hotkeyPressedBindings)
		{
			if (inputEvent.IsActionPressed(hotkeyPressedBinding.Key) && !inputEvent.IsEcho() && hotkeyPressedBinding.Value.Count > 0)
			{
				List<Binding> value = hotkeyPressedBinding.Value;
				Callable.From(value[value.Count - 1].Action.Invoke).CallDeferred();
				viewport?.SetInputAsHandled();
			}
		}
		foreach (KeyValuePair<StringName, List<Binding>> hotkeyReleasedBinding in _hotkeyReleasedBindings)
		{
			if (inputEvent.IsActionReleased(hotkeyReleasedBinding.Key) && hotkeyReleasedBinding.Value.Count > 0)
			{
				List<Binding> value2 = hotkeyReleasedBinding.Value;
				Callable.From(value2[value2.Count - 1].Action.Invoke).CallDeferred();
				viewport?.SetInputAsHandled();
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
		List<MethodInfo> list = new List<MethodInfo>(4);
		list.Add(new MethodInfo(MethodName.ClearHotkeys, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.AddBlockingScreen, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.Object, "screen", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Node"), exported: false)
		}, null));
		list.Add(new MethodInfo(MethodName.RemoveBlockingScreen, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.Object, "screen", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Node"), exported: false)
		}, null));
		list.Add(new MethodInfo(MethodName._UnhandledInput, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.Object, "inputEvent", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("InputEvent"), exported: false)
		}, null));
		return list;
	}

	/// <inheritdoc />
	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		if (method == MethodName.ClearHotkeys && args.Count == 0)
		{
			ClearHotkeys();
			ret = default;
			return true;
		}
		if (method == MethodName.AddBlockingScreen && args.Count == 1)
		{
			AddBlockingScreen(VariantUtils.ConvertTo<Node>(in args[0]));
			ret = default;
			return true;
		}
		if (method == MethodName.RemoveBlockingScreen && args.Count == 1)
		{
			RemoveBlockingScreen(VariantUtils.ConvertTo<Node>(in args[0]));
			ret = default;
			return true;
		}
		if (method == MethodName._UnhandledInput && args.Count == 1)
		{
			_UnhandledInput(VariantUtils.ConvertTo<InputEvent>(in args[0]));
			ret = default;
			return true;
		}
		return base.InvokeGodotClassMethod(in method, args, out ret);
	}

	/// <inheritdoc />
	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassMethod(in godot_string_name method)
	{
		if (method == MethodName.ClearHotkeys)
		{
			return true;
		}
		if (method == MethodName.AddBlockingScreen)
		{
			return true;
		}
		if (method == MethodName.RemoveBlockingScreen)
		{
			return true;
		}
		if (method == MethodName._UnhandledInput)
		{
			return true;
		}
		return base.HasGodotClassMethod(in method);
	}

	/// <inheritdoc />
	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if (name == PropertyName._lastSequence)
		{
			_lastSequence = VariantUtils.ConvertTo<long>(in value);
			return true;
		}
		return base.SetGodotClassPropertyValue(in name, in value);
	}

	/// <inheritdoc />
	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool GetGodotClassPropertyValue(in godot_string_name name, out godot_variant value)
	{
		if (name == PropertyName._lastSequence)
		{
			value = VariantUtils.CreateFrom(in _lastSequence);
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
		list.Add(new PropertyInfo(Variant.Type.Int, PropertyName._lastSequence, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		return list;
	}

	/// <inheritdoc />
	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		base.SaveGodotObjectData(info);
		info.AddProperty(PropertyName._lastSequence, Variant.From(in _lastSequence));
	}

	/// <inheritdoc />
	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		if (info.TryGetProperty(PropertyName._lastSequence, out var value))
		{
			_lastSequence = value.As<long>();
		}
	}
}
