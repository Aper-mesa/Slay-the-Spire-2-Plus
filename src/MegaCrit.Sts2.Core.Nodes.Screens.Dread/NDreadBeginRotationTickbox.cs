using System;
using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.HoverTips;
using MegaCrit.Sts2.addons.mega_text;

namespace MegaCrit.Sts2.Core.Nodes.Screens.Dread;

[ScriptPath("res://src/Core/Nodes/Screens/Dread/NDreadBeginRotationTickbox.cs")]
public class NDreadBeginRotationTickbox : NTickbox
{
	/// <summary>
	/// Cached StringNames for the methods contained in this class, for fast lookup.
	/// </summary>
	public new class MethodName : NTickbox.MethodName
	{
		/// <summary>
		/// Cached name for the '_Ready' method.
		/// </summary>
		public new static readonly StringName _Ready = "_Ready";

		/// <summary>
		/// Cached name for the 'InitTickedState' method.
		/// </summary>
		public static readonly StringName InitTickedState = "InitTickedState";

		/// <summary>
		/// Cached name for the 'OnTick' method.
		/// </summary>
		public new static readonly StringName OnTick = "OnTick";

		/// <summary>
		/// Cached name for the 'OnUntick' method.
		/// </summary>
		public new static readonly StringName OnUntick = "OnUntick";

		/// <summary>
		/// Cached name for the 'OnFocus' method.
		/// </summary>
		public new static readonly StringName OnFocus = "OnFocus";

		/// <summary>
		/// Cached name for the 'OnUnfocus' method.
		/// </summary>
		public new static readonly StringName OnUnfocus = "OnUnfocus";

		/// <summary>
		/// Cached name for the 'UpdateRotatingImage' method.
		/// </summary>
		public static readonly StringName UpdateRotatingImage = "UpdateRotatingImage";
	}

	/// <summary>
	/// Cached StringNames for the properties and fields contained in this class, for fast lookup.
	/// </summary>
	public new class PropertyName : NTickbox.PropertyName
	{
		/// <summary>
		/// Cached name for the '_rotatingImage' field.
		/// </summary>
		public static readonly StringName _rotatingImage = "_rotatingImage";

		/// <summary>
		/// Cached name for the '_rotatingExplanation' field.
		/// </summary>
		public static readonly StringName _rotatingExplanation = "_rotatingExplanation";

		/// <summary>
		/// Cached name for the '_dreadScreen' field.
		/// </summary>
		public static readonly StringName _dreadScreen = "_dreadScreen";

		/// <summary>
		/// Cached name for the '_rotatingTween' field.
		/// </summary>
		public static readonly StringName _rotatingTween = "_rotatingTween";
	}

	/// <summary>
	/// Cached StringNames for the signals contained in this class, for fast lookup.
	/// </summary>
	public new class SignalName : NTickbox.SignalName
	{
	}

	private HoverTip _hoverTip;

	private Control _rotatingImage;

	private MegaRichTextLabel _rotatingExplanation;

	private NDreadScreen _dreadScreen;

	private Tween? _rotatingTween;

	public override void _Ready()
	{
		ConnectSignals();
		_rotatingImage = GetNode<Control>("%TickboxRotatingImage");
		_rotatingExplanation = GetNode<MegaRichTextLabel>("%RotatingExplanation");
		_dreadScreen = this.GetAncestorOfType<NDreadScreen>();
		_hoverTip = new HoverTip(new LocString("static_hover_tips", "DREAD_ROTATION.title"), new LocString("static_hover_tips", "DREAD_ROTATION.description"));
	}

	/// <summary>
	/// Setting <see cref="P:MegaCrit.Sts2.Core.Nodes.CommonUi.NTickbox.IsTicked" /> doesn't update the rotating image, so use this instead.
	/// </summary>
	public void InitTickedState(bool isTicked)
	{
		IsTicked = isTicked;
		UpdateRotatingImage();
	}

	protected override void OnTick()
	{
		UpdateRotatingImage();
		_dreadScreen.BeginRotationTickboxChanged(beginRotation: true);
	}

	protected override void OnUntick()
	{
		UpdateRotatingImage();
		_dreadScreen.BeginRotationTickboxChanged(beginRotation: false);
	}

	protected override void OnFocus()
	{
		base.OnFocus();
		NHoverTipSet nHoverTipSet = NHoverTipSet.CreateAndShow(this, _hoverTip);
		if (nHoverTipSet != null)
		{
			nHoverTipSet.GlobalPosition = GlobalPosition + Vector2.Right * (Size.X + 10f);
		}
	}

	protected override void OnUnfocus()
	{
		base.OnUnfocus();
		NHoverTipSet.Remove(this);
	}

	private void UpdateRotatingImage()
	{
		if (IsTicked)
		{
			_rotatingImage.Modulate = Colors.White;
			if (_rotatingTween == null)
			{
				_rotatingTween = CreateTween();
				_rotatingTween.TweenProperty(_rotatingImage, "rotation", (float)Math.PI * 2f, 10.0).From(0f).SetTrans(Tween.TransitionType.Linear);
				_rotatingTween.SetLoops();
			}
			else
			{
				_rotatingTween.Play();
			}
		}
		else if (!IsTicked)
		{
			_rotatingTween?.Pause();
			_rotatingImage.Modulate = StsColors.gray;
		}
	}

	/// <summary>
	/// Get the method information for all the methods declared in this class.
	/// This method is used by Godot to register the available methods in the editor.
	/// Do not call this method.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Never)]
	internal new static List<MethodInfo> GetGodotMethodList()
	{
		List<MethodInfo> list = new List<MethodInfo>(7);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.InitTickedState, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.Bool, "isTicked", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
		}, null));
		list.Add(new MethodInfo(MethodName.OnTick, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.OnUntick, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.OnFocus, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.OnUnfocus, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.UpdateRotatingImage, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
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
		if (method == MethodName.InitTickedState && args.Count == 1)
		{
			InitTickedState(VariantUtils.ConvertTo<bool>(in args[0]));
			ret = default;
			return true;
		}
		if (method == MethodName.OnTick && args.Count == 0)
		{
			OnTick();
			ret = default;
			return true;
		}
		if (method == MethodName.OnUntick && args.Count == 0)
		{
			OnUntick();
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
		if (method == MethodName.UpdateRotatingImage && args.Count == 0)
		{
			UpdateRotatingImage();
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
		if (method == MethodName.InitTickedState)
		{
			return true;
		}
		if (method == MethodName.OnTick)
		{
			return true;
		}
		if (method == MethodName.OnUntick)
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
		if (method == MethodName.UpdateRotatingImage)
		{
			return true;
		}
		return base.HasGodotClassMethod(in method);
	}

	/// <inheritdoc />
	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if (name == PropertyName._rotatingImage)
		{
			_rotatingImage = VariantUtils.ConvertTo<Control>(in value);
			return true;
		}
		if (name == PropertyName._rotatingExplanation)
		{
			_rotatingExplanation = VariantUtils.ConvertTo<MegaRichTextLabel>(in value);
			return true;
		}
		if (name == PropertyName._dreadScreen)
		{
			_dreadScreen = VariantUtils.ConvertTo<NDreadScreen>(in value);
			return true;
		}
		if (name == PropertyName._rotatingTween)
		{
			_rotatingTween = VariantUtils.ConvertTo<Tween>(in value);
			return true;
		}
		return base.SetGodotClassPropertyValue(in name, in value);
	}

	/// <inheritdoc />
	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool GetGodotClassPropertyValue(in godot_string_name name, out godot_variant value)
	{
		if (name == PropertyName._rotatingImage)
		{
			value = VariantUtils.CreateFrom(in _rotatingImage);
			return true;
		}
		if (name == PropertyName._rotatingExplanation)
		{
			value = VariantUtils.CreateFrom(in _rotatingExplanation);
			return true;
		}
		if (name == PropertyName._dreadScreen)
		{
			value = VariantUtils.CreateFrom(in _dreadScreen);
			return true;
		}
		if (name == PropertyName._rotatingTween)
		{
			value = VariantUtils.CreateFrom(in _rotatingTween);
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
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._rotatingImage, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._rotatingExplanation, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._dreadScreen, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._rotatingTween, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		return list;
	}

	/// <inheritdoc />
	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		base.SaveGodotObjectData(info);
		info.AddProperty(PropertyName._rotatingImage, Variant.From(in _rotatingImage));
		info.AddProperty(PropertyName._rotatingExplanation, Variant.From(in _rotatingExplanation));
		info.AddProperty(PropertyName._dreadScreen, Variant.From(in _dreadScreen));
		info.AddProperty(PropertyName._rotatingTween, Variant.From(in _rotatingTween));
	}

	/// <inheritdoc />
	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		if (info.TryGetProperty(PropertyName._rotatingImage, out var value))
		{
			_rotatingImage = value.As<Control>();
		}
		if (info.TryGetProperty(PropertyName._rotatingExplanation, out var value2))
		{
			_rotatingExplanation = value2.As<MegaRichTextLabel>();
		}
		if (info.TryGetProperty(PropertyName._dreadScreen, out var value3))
		{
			_dreadScreen = value3.As<NDreadScreen>();
		}
		if (info.TryGetProperty(PropertyName._rotatingTween, out var value4))
		{
			_rotatingTween = value4.As<Tween>();
		}
	}
}
