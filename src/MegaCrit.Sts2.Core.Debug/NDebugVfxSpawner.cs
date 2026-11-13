using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes.Vfx.Ui;

namespace MegaCrit.Sts2.Core.Debug;

/// <summary>
/// A helper script to spawn vfx in whole_screen_vfx_playground
/// </summary>
[ScriptPath("res://src/Core/Debug/NDebugVfxSpawner.cs")]
public class NDebugVfxSpawner : Control
{
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
		/// Cached name for the '_Input' method.
		/// </summary>
		public new static readonly StringName _Input = "_Input";
	}

	/// <summary>
	/// Cached StringNames for the properties and fields contained in this class, for fast lookup.
	/// </summary>
	public new class PropertyName : Control.PropertyName
	{
		/// <summary>
		/// Cached name for the '_links' field.
		/// </summary>
		public static readonly StringName _links = "_links";

		/// <summary>
		/// Cached name for the '_chainVisualization' field.
		/// </summary>
		public static readonly StringName _chainVisualization = "_chainVisualization";
	}

	/// <summary>
	/// Cached StringNames for the signals contained in this class, for fast lookup.
	/// </summary>
	public new class SignalName : Control.SignalName
	{
	}

	private int _links = 10;

	private NChainVisualization _chainVisualization;

	public override void _Ready()
	{
		_chainVisualization.Initialize(_links);
	}

	public override void _Input(InputEvent ev)
	{
		if (ev is InputEventKey { Pressed: not false } inputEventKey)
		{
			if (inputEventKey.Keycode == Key.B)
			{
				TaskHelper.RunSafely(_chainVisualization.Break(0f));
			}
			else if (inputEventKey.Keycode == Key.R)
			{
				_chainVisualization.Reset();
			}
			else if (inputEventKey.Keycode == Key.A)
			{
				_links++;
				_chainVisualization.Initialize(_links);
				TaskHelper.RunSafely(_chainVisualization.AnimateInLastLink(0f, null));
			}
			else if (inputEventKey.Keycode == Key.Minus)
			{
				_links--;
				_chainVisualization.Initialize(_links);
			}
			else if (inputEventKey.Keycode == Key.Equal)
			{
				_links++;
				_chainVisualization.Initialize(_links);
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
		List<MethodInfo> list = new List<MethodInfo>(2);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName._Input, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.Object, "ev", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("InputEvent"), exported: false)
		}, null));
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
		if (method == MethodName._Input && args.Count == 1)
		{
			_Input(VariantUtils.ConvertTo<InputEvent>(in args[0]));
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
		if (method == MethodName._Input)
		{
			return true;
		}
		return base.HasGodotClassMethod(in method);
	}

	/// <inheritdoc />
	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if (name == PropertyName._links)
		{
			_links = VariantUtils.ConvertTo<int>(in value);
			return true;
		}
		if (name == PropertyName._chainVisualization)
		{
			_chainVisualization = VariantUtils.ConvertTo<NChainVisualization>(in value);
			return true;
		}
		return base.SetGodotClassPropertyValue(in name, in value);
	}

	/// <inheritdoc />
	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool GetGodotClassPropertyValue(in godot_string_name name, out godot_variant value)
	{
		if (name == PropertyName._links)
		{
			value = VariantUtils.CreateFrom(in _links);
			return true;
		}
		if (name == PropertyName._chainVisualization)
		{
			value = VariantUtils.CreateFrom(in _chainVisualization);
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
		list.Add(new PropertyInfo(Variant.Type.Int, PropertyName._links, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._chainVisualization, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		return list;
	}

	/// <inheritdoc />
	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		base.SaveGodotObjectData(info);
		info.AddProperty(PropertyName._links, Variant.From(in _links));
		info.AddProperty(PropertyName._chainVisualization, Variant.From(in _chainVisualization));
	}

	/// <inheritdoc />
	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		if (info.TryGetProperty(PropertyName._links, out var value))
		{
			_links = value.As<int>();
		}
		if (info.TryGetProperty(PropertyName._chainVisualization, out var value2))
		{
			_chainVisualization = value2.As<NChainVisualization>();
		}
	}
}
