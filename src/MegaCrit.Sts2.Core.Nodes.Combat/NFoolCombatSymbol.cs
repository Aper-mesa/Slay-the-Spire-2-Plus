using System;
using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.HoverTips;
using MegaCrit.Sts2.Core.Random;

namespace MegaCrit.Sts2.Core.Nodes.Combat;

/// <summary>
/// Represents one of the symbols the Fool can create during combat.
/// Is part of a special UI specifically for the Fool.
/// </summary>
[ScriptPath("res://src/Core/Nodes/Combat/NFoolCombatSymbol.cs")]
public class NFoolCombatSymbol : NClickableControl
{
	/// <summary>
	/// Cached StringNames for the methods contained in this class, for fast lookup.
	/// </summary>
	public new class MethodName : NClickableControl.MethodName
	{
		/// <summary>
		/// Cached name for the 'Create' method.
		/// </summary>
		public static readonly StringName Create = "Create";

		/// <summary>
		/// Cached name for the '_Ready' method.
		/// </summary>
		public new static readonly StringName _Ready = "_Ready";

		/// <summary>
		/// Cached name for the 'SetVisualsAndHoverTip' method.
		/// </summary>
		public static readonly StringName SetVisualsAndHoverTip = "SetVisualsAndHoverTip";

		/// <summary>
		/// Cached name for the 'OnFocus' method.
		/// </summary>
		public new static readonly StringName OnFocus = "OnFocus";

		/// <summary>
		/// Cached name for the 'OnUnfocus' method.
		/// </summary>
		public new static readonly StringName OnUnfocus = "OnUnfocus";

		/// <summary>
		/// Cached name for the '_Process' method.
		/// </summary>
		public new static readonly StringName _Process = "_Process";

		/// <summary>
		/// Cached name for the 'MoveTo' method.
		/// </summary>
		public static readonly StringName MoveTo = "MoveTo";
	}

	/// <summary>
	/// Cached StringNames for the properties and fields contained in this class, for fast lookup.
	/// </summary>
	public new class PropertyName : NClickableControl.PropertyName
	{
		/// <summary>
		/// Cached name for the '_symbol' field.
		/// </summary>
		public static readonly StringName _symbol = "_symbol";

		/// <summary>
		/// Cached name for the '_glow' field.
		/// </summary>
		public static readonly StringName _glow = "_glow";

		/// <summary>
		/// Cached name for the 'symbolType' field.
		/// </summary>
		public static readonly StringName symbolType = "symbolType";

		/// <summary>
		/// Cached name for the '_originalPosition' field.
		/// </summary>
		public static readonly StringName _originalPosition = "_originalPosition";

		/// <summary>
		/// Cached name for the '_timeOffset' field.
		/// </summary>
		public static readonly StringName _timeOffset = "_timeOffset";

		/// <summary>
		/// Cached name for the '_spawnTween' field.
		/// </summary>
		public static readonly StringName _spawnTween = "_spawnTween";

		/// <summary>
		/// Cached name for the '_moveTween' field.
		/// </summary>
		public static readonly StringName _moveTween = "_moveTween";
	}

	/// <summary>
	/// Cached StringNames for the signals contained in this class, for fast lookup.
	/// </summary>
	public new class SignalName : NClickableControl.SignalName
	{
	}

	private static readonly string _scenePath = SceneHelper.GetScenePath("combat/fool_combat_symbol");

	private TextureRect _symbol;

	private TextureRect _glow;

	public CardTag symbolType;

	private Vector2 _originalPosition;

	private float _timeOffset;

	private const float _wobbleRadius = 2f;

	private const float _wobbleSpeed = 1f;

	private Tween? _spawnTween;

	private Tween? _moveTween;

	private LocString Title => new LocString("static_hover_tips", symbolType.ToString().ToUpperInvariant() + "_MELODY.title");

	private LocString Description => new LocString("static_hover_tips", symbolType.ToString().ToUpperInvariant() + "_MELODY.description");

	public static NFoolCombatSymbol Create(CardTag melodyTag)
	{
		NFoolCombatSymbol nFoolCombatSymbol = PreloadManager.Cache.GetScene(_scenePath).Instantiate<NFoolCombatSymbol>(PackedScene.GenEditState.Disabled);
		nFoolCombatSymbol.symbolType = melodyTag;
		return nFoolCombatSymbol;
	}

	public override void _Ready()
	{
		ConnectSignals();
		_symbol = GetNode<TextureRect>("%Symbol");
		_glow = GetNode<TextureRect>("%Glow");
		_originalPosition = _symbol.Position;
		_timeOffset = Rng.Chaotic.NextFloat(0f, (float)Math.PI * 2f);
		SetVisualsAndHoverTip();
		_symbol.Modulate = StsColors.transparentWhite;
		_spawnTween = CreateTween().SetParallel();
		_spawnTween.TweenProperty(_symbol, "modulate:a", 1f, 0.3);
		_spawnTween.TweenProperty(_symbol, "scale", Vector2.One, 0.3).SetTrans(Tween.TransitionType.Back).SetEase(Tween.EaseType.Out)
			.From(Vector2.One * 0.5f);
	}

	private void SetVisualsAndHoverTip()
	{
		switch (symbolType)
		{
		case CardTag.Wah:
			_symbol.Texture = GD.Load<Texture2D>("res://images/packed/fool/symbol_wah.png");
			_glow.Modulate = new Color("FF009980");
			break;
		case CardTag.Cho:
			_symbol.Texture = GD.Load<Texture2D>("res://images/packed/fool/symbol_cho.png");
			_glow.Modulate = new Color("0048FFC0");
			break;
		case CardTag.Ki:
			_symbol.Texture = GD.Load<Texture2D>("res://images/packed/fool/symbol_ki.png");
			_glow.Modulate = new Color("FF770080");
			break;
		default:
			throw new InvalidOperationException("Invalid Card Tag for the fool song symbol");
		}
	}

	protected override void OnFocus()
	{
		LocString description = Description;
		NHoverTipSet.CreateAndShow(this, new HoverTip(Title, description))?.SetGlobalPosition(GlobalPosition + new Vector2(100f, 0f));
	}

	protected override void OnUnfocus()
	{
		NHoverTipSet.Remove(this);
	}

	public override void _Process(double delta)
	{
		float num = (float)Time.GetTicksMsec() * 0.001f * 1f + _timeOffset;
		float x = Mathf.Sin(num) * 2f;
		float y = Mathf.Sin(num * 0.7f + 1.3f) * 2f;
		_symbol.Position = _originalPosition + new Vector2(x, y);
	}

	public void MoveTo(Vector2 targetPosition)
	{
		_moveTween?.Kill();
		_moveTween = CreateTween().SetParallel();
		_moveTween.TweenProperty(this, "global_position", targetPosition, 0.5).SetTrans(Tween.TransitionType.Back).SetEase(Tween.EaseType.Out);
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
		list.Add(new MethodInfo(MethodName.Create, new PropertyInfo(Variant.Type.Object, "", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Control"), exported: false), MethodFlags.Normal | MethodFlags.Static, new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.Int, "melodyTag", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
		}, null));
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.SetVisualsAndHoverTip, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.OnFocus, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.OnUnfocus, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName._Process, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.Float, "delta", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
		}, null));
		list.Add(new MethodInfo(MethodName.MoveTo, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.Vector2, "targetPosition", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
		}, null));
		return list;
	}

	/// <inheritdoc />
	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		if (method == MethodName.Create && args.Count == 1)
		{
			ret = VariantUtils.CreateFrom<NFoolCombatSymbol>(Create(VariantUtils.ConvertTo<CardTag>(in args[0])));
			return true;
		}
		if (method == MethodName._Ready && args.Count == 0)
		{
			_Ready();
			ret = default;
			return true;
		}
		if (method == MethodName.SetVisualsAndHoverTip && args.Count == 0)
		{
			SetVisualsAndHoverTip();
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
		if (method == MethodName._Process && args.Count == 1)
		{
			_Process(VariantUtils.ConvertTo<double>(in args[0]));
			ret = default;
			return true;
		}
		if (method == MethodName.MoveTo && args.Count == 1)
		{
			MoveTo(VariantUtils.ConvertTo<Vector2>(in args[0]));
			ret = default;
			return true;
		}
		return base.InvokeGodotClassMethod(in method, args, out ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static bool InvokeGodotClassStaticMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		if (method == MethodName.Create && args.Count == 1)
		{
			ret = VariantUtils.CreateFrom<NFoolCombatSymbol>(Create(VariantUtils.ConvertTo<CardTag>(in args[0])));
			return true;
		}
		ret = default;
		return false;
	}

	/// <inheritdoc />
	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassMethod(in godot_string_name method)
	{
		if (method == MethodName.Create)
		{
			return true;
		}
		if (method == MethodName._Ready)
		{
			return true;
		}
		if (method == MethodName.SetVisualsAndHoverTip)
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
		if (method == MethodName._Process)
		{
			return true;
		}
		if (method == MethodName.MoveTo)
		{
			return true;
		}
		return base.HasGodotClassMethod(in method);
	}

	/// <inheritdoc />
	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if (name == PropertyName._symbol)
		{
			_symbol = VariantUtils.ConvertTo<TextureRect>(in value);
			return true;
		}
		if (name == PropertyName._glow)
		{
			_glow = VariantUtils.ConvertTo<TextureRect>(in value);
			return true;
		}
		if (name == PropertyName.symbolType)
		{
			symbolType = VariantUtils.ConvertTo<CardTag>(in value);
			return true;
		}
		if (name == PropertyName._originalPosition)
		{
			_originalPosition = VariantUtils.ConvertTo<Vector2>(in value);
			return true;
		}
		if (name == PropertyName._timeOffset)
		{
			_timeOffset = VariantUtils.ConvertTo<float>(in value);
			return true;
		}
		if (name == PropertyName._spawnTween)
		{
			_spawnTween = VariantUtils.ConvertTo<Tween>(in value);
			return true;
		}
		if (name == PropertyName._moveTween)
		{
			_moveTween = VariantUtils.ConvertTo<Tween>(in value);
			return true;
		}
		return base.SetGodotClassPropertyValue(in name, in value);
	}

	/// <inheritdoc />
	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool GetGodotClassPropertyValue(in godot_string_name name, out godot_variant value)
	{
		if (name == PropertyName._symbol)
		{
			value = VariantUtils.CreateFrom(in _symbol);
			return true;
		}
		if (name == PropertyName._glow)
		{
			value = VariantUtils.CreateFrom(in _glow);
			return true;
		}
		if (name == PropertyName.symbolType)
		{
			value = VariantUtils.CreateFrom(in symbolType);
			return true;
		}
		if (name == PropertyName._originalPosition)
		{
			value = VariantUtils.CreateFrom(in _originalPosition);
			return true;
		}
		if (name == PropertyName._timeOffset)
		{
			value = VariantUtils.CreateFrom(in _timeOffset);
			return true;
		}
		if (name == PropertyName._spawnTween)
		{
			value = VariantUtils.CreateFrom(in _spawnTween);
			return true;
		}
		if (name == PropertyName._moveTween)
		{
			value = VariantUtils.CreateFrom(in _moveTween);
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
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._symbol, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._glow, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Int, PropertyName.symbolType, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Vector2, PropertyName._originalPosition, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Float, PropertyName._timeOffset, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._spawnTween, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._moveTween, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		return list;
	}

	/// <inheritdoc />
	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		base.SaveGodotObjectData(info);
		info.AddProperty(PropertyName._symbol, Variant.From(in _symbol));
		info.AddProperty(PropertyName._glow, Variant.From(in _glow));
		info.AddProperty(PropertyName.symbolType, Variant.From(in symbolType));
		info.AddProperty(PropertyName._originalPosition, Variant.From(in _originalPosition));
		info.AddProperty(PropertyName._timeOffset, Variant.From(in _timeOffset));
		info.AddProperty(PropertyName._spawnTween, Variant.From(in _spawnTween));
		info.AddProperty(PropertyName._moveTween, Variant.From(in _moveTween));
	}

	/// <inheritdoc />
	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		if (info.TryGetProperty(PropertyName._symbol, out var value))
		{
			_symbol = value.As<TextureRect>();
		}
		if (info.TryGetProperty(PropertyName._glow, out var value2))
		{
			_glow = value2.As<TextureRect>();
		}
		if (info.TryGetProperty(PropertyName.symbolType, out var value3))
		{
			symbolType = value3.As<CardTag>();
		}
		if (info.TryGetProperty(PropertyName._originalPosition, out var value4))
		{
			_originalPosition = value4.As<Vector2>();
		}
		if (info.TryGetProperty(PropertyName._timeOffset, out var value5))
		{
			_timeOffset = value5.As<float>();
		}
		if (info.TryGetProperty(PropertyName._spawnTween, out var value6))
		{
			_spawnTween = value6.As<Tween>();
		}
		if (info.TryGetProperty(PropertyName._moveTween, out var value7))
		{
			_moveTween = value7.As<Tween>();
		}
	}
}
