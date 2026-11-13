using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Entities.FoolArrows;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.HoverTips;

namespace MegaCrit.Sts2.Core.Nodes.Combat;

/// <summary>
/// Represents a single arrow in the fool's bow and arrow UI.
/// </summary>
[ScriptPath("res://src/Core/Nodes/Combat/NFoolArrowUi.cs")]
public class NFoolArrowUi : NClickableControl
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
		/// Cached name for the '_EnterTree' method.
		/// </summary>
		public new static readonly StringName _EnterTree = "_EnterTree";

		/// <summary>
		/// Cached name for the '_ExitTree' method.
		/// </summary>
		public new static readonly StringName _ExitTree = "_ExitTree";

		/// <summary>
		/// Cached name for the 'UpdateVisuals' method.
		/// </summary>
		public static readonly StringName UpdateVisuals = "UpdateVisuals";

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
	public new class PropertyName : NClickableControl.PropertyName
	{
		/// <summary>
		/// Cached name for the '_visual' field.
		/// </summary>
		public static readonly StringName _visual = "_visual";

		/// <summary>
		/// Cached name for the '_outline' field.
		/// </summary>
		public static readonly StringName _outline = "_outline";

		/// <summary>
		/// Cached name for the '_hasArrow' field.
		/// </summary>
		public static readonly StringName _hasArrow = "_hasArrow";

		/// <summary>
		/// Cached name for the '_createTween' field.
		/// </summary>
		public static readonly StringName _createTween = "_createTween";

		/// <summary>
		/// Cached name for the '_shootTween' field.
		/// </summary>
		public static readonly StringName _shootTween = "_shootTween";

		/// <summary>
		/// Cached name for the '_arrowIndex' field.
		/// </summary>
		public static readonly StringName _arrowIndex = "_arrowIndex";
	}

	/// <summary>
	/// Cached StringNames for the signals contained in this class, for fast lookup.
	/// </summary>
	public new class SignalName : NClickableControl.SignalName
	{
	}

	private ArrowSlot _arrowSlot;

	private TextureRect _visual;

	private TextureRect _outline;

	private bool _hasArrow;

	private static readonly string _path = SceneHelper.GetScenePath("combat/fool_arrow");

	private Tween? _createTween;

	private Tween? _shootTween;

	private int _arrowIndex;

	private const double _fadeDuration = 0.25;

	private static readonly Vector2 _shotArrowScale = Vector2.One * 1.4f;

	public ArrowSlot ArrowSlot => _arrowSlot;

	public static NFoolArrowUi Create(ArrowSlot arrowSlot, int index)
	{
		NFoolArrowUi nFoolArrowUi = PreloadManager.Cache.GetScene(_path).Instantiate<NFoolArrowUi>(PackedScene.GenEditState.Disabled);
		nFoolArrowUi._arrowSlot = arrowSlot;
		nFoolArrowUi._arrowIndex = index;
		return nFoolArrowUi;
	}

	public override void _Ready()
	{
		_visual = GetNode<TextureRect>("%Visual");
		_outline = GetNode<TextureRect>("%Stroke");
		bool flag = _arrowSlot.Status == ArrowStatus.Retrieved;
		Visible = _arrowSlot.Status != ArrowStatus.Destroyed;
		_visual.Modulate = (flag ? Colors.White : StsColors.transparentBlack);
		_visual.Scale = (flag ? Vector2.One : _shotArrowScale);
		ConnectSignals();
		_createTween = CreateTween().SetParallel();
		_createTween.TweenProperty(this, "modulate:a", 1f, 0.25);
		if (_arrowIndex % 2 == 0)
		{
			_visual.FlipV = false;
			_outline.FlipV = false;
			_createTween.TweenProperty(_visual, "position:y", 0f, 0.25).SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Back)
				.From(16f);
			_createTween.TweenProperty(_outline, "position:y", 0f, 0.25).SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Back)
				.From(16f);
		}
		else
		{
			_visual.FlipV = true;
			_outline.FlipV = true;
			_createTween.TweenProperty(_visual, "position:y", -8f, 0.25).SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Back)
				.From(-24f);
			_createTween.TweenProperty(_outline, "position:y", -8f, 0.25).SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Back)
				.From(-24f);
		}
	}

	public override void _EnterTree()
	{
		base._EnterTree();
		_arrowSlot.ArrowStatusUpdated += UpdateVisuals;
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		_arrowSlot.ArrowStatusUpdated -= UpdateVisuals;
		_createTween?.Kill();
		_shootTween?.Kill();
	}

	private void UpdateVisuals()
	{
		_shootTween?.Kill();
		Visible = _arrowSlot.Status != ArrowStatus.Destroyed;
		if (Visible)
		{
			_shootTween = CreateTween().SetParallel();
			_shootTween.TweenProperty(_visual, "modulate", (_arrowSlot.Status == ArrowStatus.Retrieved) ? Colors.White : StsColors.transparentBlack, 0.25);
			if (_arrowSlot.Status != ArrowStatus.Retrieved)
			{
				_shootTween.TweenProperty(_visual, "scale", _shotArrowScale, 0.5).SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Expo);
			}
			else
			{
				_shootTween.TweenProperty(_visual, "scale", Vector2.One, 0.5).SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Back);
			}
		}
	}

	protected override void OnFocus()
	{
		NHoverTipSet.CreateAndShow(this, HoverTipFactory.ForArrow(_arrowSlot.owner, _arrowSlot.Status == ArrowStatus.Away))?.SetGlobalPosition(GlobalPosition + new Vector2(50f, 0f));
	}

	protected override void OnUnfocus()
	{
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
		List<MethodInfo> list = new List<MethodInfo>(6);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName._EnterTree, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName._ExitTree, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.UpdateVisuals, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
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
		if (method == MethodName.UpdateVisuals && args.Count == 0)
		{
			UpdateVisuals();
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
		if (method == MethodName._EnterTree)
		{
			return true;
		}
		if (method == MethodName._ExitTree)
		{
			return true;
		}
		if (method == MethodName.UpdateVisuals)
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
		if (name == PropertyName._visual)
		{
			_visual = VariantUtils.ConvertTo<TextureRect>(in value);
			return true;
		}
		if (name == PropertyName._outline)
		{
			_outline = VariantUtils.ConvertTo<TextureRect>(in value);
			return true;
		}
		if (name == PropertyName._hasArrow)
		{
			_hasArrow = VariantUtils.ConvertTo<bool>(in value);
			return true;
		}
		if (name == PropertyName._createTween)
		{
			_createTween = VariantUtils.ConvertTo<Tween>(in value);
			return true;
		}
		if (name == PropertyName._shootTween)
		{
			_shootTween = VariantUtils.ConvertTo<Tween>(in value);
			return true;
		}
		if (name == PropertyName._arrowIndex)
		{
			_arrowIndex = VariantUtils.ConvertTo<int>(in value);
			return true;
		}
		return base.SetGodotClassPropertyValue(in name, in value);
	}

	/// <inheritdoc />
	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool GetGodotClassPropertyValue(in godot_string_name name, out godot_variant value)
	{
		if (name == PropertyName._visual)
		{
			value = VariantUtils.CreateFrom(in _visual);
			return true;
		}
		if (name == PropertyName._outline)
		{
			value = VariantUtils.CreateFrom(in _outline);
			return true;
		}
		if (name == PropertyName._hasArrow)
		{
			value = VariantUtils.CreateFrom(in _hasArrow);
			return true;
		}
		if (name == PropertyName._createTween)
		{
			value = VariantUtils.CreateFrom(in _createTween);
			return true;
		}
		if (name == PropertyName._shootTween)
		{
			value = VariantUtils.CreateFrom(in _shootTween);
			return true;
		}
		if (name == PropertyName._arrowIndex)
		{
			value = VariantUtils.CreateFrom(in _arrowIndex);
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
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._visual, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._outline, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Bool, PropertyName._hasArrow, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._createTween, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._shootTween, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Int, PropertyName._arrowIndex, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		return list;
	}

	/// <inheritdoc />
	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		base.SaveGodotObjectData(info);
		info.AddProperty(PropertyName._visual, Variant.From(in _visual));
		info.AddProperty(PropertyName._outline, Variant.From(in _outline));
		info.AddProperty(PropertyName._hasArrow, Variant.From(in _hasArrow));
		info.AddProperty(PropertyName._createTween, Variant.From(in _createTween));
		info.AddProperty(PropertyName._shootTween, Variant.From(in _shootTween));
		info.AddProperty(PropertyName._arrowIndex, Variant.From(in _arrowIndex));
	}

	/// <inheritdoc />
	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		if (info.TryGetProperty(PropertyName._visual, out var value))
		{
			_visual = value.As<TextureRect>();
		}
		if (info.TryGetProperty(PropertyName._outline, out var value2))
		{
			_outline = value2.As<TextureRect>();
		}
		if (info.TryGetProperty(PropertyName._hasArrow, out var value3))
		{
			_hasArrow = value3.As<bool>();
		}
		if (info.TryGetProperty(PropertyName._createTween, out var value4))
		{
			_createTween = value4.As<Tween>();
		}
		if (info.TryGetProperty(PropertyName._shootTween, out var value5))
		{
			_shootTween = value5.As<Tween>();
		}
		if (info.TryGetProperty(PropertyName._arrowIndex, out var value6))
		{
			_arrowIndex = value6.As<int>();
		}
	}
}
