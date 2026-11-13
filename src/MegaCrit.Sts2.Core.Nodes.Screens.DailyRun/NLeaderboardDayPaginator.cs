using System;
using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Daily;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.addons.mega_text;

namespace MegaCrit.Sts2.Core.Nodes.Screens.DailyRun;

/// <summary>
/// This is pretty close to NPaginator, but NPaginator only supports a static list of options whereas this needs to
/// - Set its text based on a DateTimeOffset that is (effectively) infinite
/// - Dynamically enable/disable arrows based on whether the query says there is a leaderboard
/// </summary>
[ScriptPath("res://src/Core/Nodes/Screens/DailyRun/NLeaderboardDayPaginator.cs")]
public class NLeaderboardDayPaginator : Control
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
		/// Cached name for the 'PageLeft' method.
		/// </summary>
		public static readonly StringName PageLeft = "PageLeft";

		/// <summary>
		/// Cached name for the 'PageRight' method.
		/// </summary>
		public static readonly StringName PageRight = "PageRight";

		/// <summary>
		/// Cached name for the 'DayChangeHelper' method.
		/// </summary>
		public static readonly StringName DayChangeHelper = "DayChangeHelper";

		/// <summary>
		/// Cached name for the 'OnDayChanged' method.
		/// </summary>
		public static readonly StringName OnDayChanged = "OnDayChanged";

		/// <summary>
		/// Cached name for the 'SetEnabled' method.
		/// </summary>
		public static readonly StringName SetEnabled = "SetEnabled";

		/// <summary>
		/// Cached name for the 'OnFocus' method.
		/// </summary>
		public static readonly StringName OnFocus = "OnFocus";

		/// <summary>
		/// Cached name for the 'OnUnfocus' method.
		/// </summary>
		public static readonly StringName OnUnfocus = "OnUnfocus";
	}

	/// <summary>
	/// Cached StringNames for the properties and fields contained in this class, for fast lookup.
	/// </summary>
	public new class PropertyName : Control.PropertyName
	{
		/// <summary>
		/// Cached name for the '_label' field.
		/// </summary>
		public static readonly StringName _label = "_label";

		/// <summary>
		/// Cached name for the '_vfxLabel' field.
		/// </summary>
		public static readonly StringName _vfxLabel = "_vfxLabel";

		/// <summary>
		/// Cached name for the '_leftArrow' field.
		/// </summary>
		public static readonly StringName _leftArrow = "_leftArrow";

		/// <summary>
		/// Cached name for the '_rightArrow' field.
		/// </summary>
		public static readonly StringName _rightArrow = "_rightArrow";

		/// <summary>
		/// Cached name for the '_selectionReticle' field.
		/// </summary>
		public static readonly StringName _selectionReticle = "_selectionReticle";

		/// <summary>
		/// Cached name for the '_tween' field.
		/// </summary>
		public static readonly StringName _tween = "_tween";

		/// <summary>
		/// Cached name for the '_leaderboard' field.
		/// </summary>
		public static readonly StringName _leaderboard = "_leaderboard";

		/// <summary>
		/// Cached name for the '_canPageLeft' field.
		/// </summary>
		public static readonly StringName _canPageLeft = "_canPageLeft";

		/// <summary>
		/// Cached name for the '_canPageRight' field.
		/// </summary>
		public static readonly StringName _canPageRight = "_canPageRight";

		/// <summary>
		/// Cached name for the '_paginationAllowed' field.
		/// </summary>
		public static readonly StringName _paginationAllowed = "_paginationAllowed";
	}

	/// <summary>
	/// Cached StringNames for the signals contained in this class, for fast lookup.
	/// </summary>
	public new class SignalName : Control.SignalName
	{
	}

	protected MegaLabel _label;

	private MegaLabel _vfxLabel;

	private NLeaderboardDayArrow _leftArrow;

	private NLeaderboardDayArrow _rightArrow;

	private NSelectionReticle _selectionReticle;

	private Tween? _tween;

	private const double _animDuration = 0.25;

	private const float _animDistance = 90f;

	private DateTimeOffset _currentDay;

	private NDailyRunLeaderboard? _leaderboard;

	private bool _canPageLeft;

	private bool _canPageRight;

	private bool _paginationAllowed = true;

	public override void _Ready()
	{
		_label = GetNode<MegaLabel>("LabelContainer/Mask/Label");
		_vfxLabel = GetNode<MegaLabel>("LabelContainer/Mask/VfxLabel");
		_selectionReticle = GetNode<NSelectionReticle>("SelectionReticle");
		_leftArrow = GetNode<NLeaderboardDayArrow>("LeftArrow");
		_rightArrow = GetNode<NLeaderboardDayArrow>("RightArrow");
		Connect(Control.SignalName.FocusEntered, Callable.From(OnFocus));
		Connect(Control.SignalName.FocusExited, Callable.From(OnUnfocus));
		_leftArrow.Connect(PageLeft);
		_rightArrow.Connect(PageRight);
		SetEnabled(leftArrowEnabled: false, rightArrowEnabled: false);
	}

	public void Initialize(NDailyRunLeaderboard leaderboard, DateTimeOffset dateTime, bool allowPagination)
	{
		_currentDay = dateTime;
		_leaderboard = leaderboard;
		_paginationAllowed = allowPagination;
		OnDayChanged(changeLeaderboardDay: false);
		_leftArrow.Visible = allowPagination;
		_rightArrow.Visible = allowPagination;
		FocusMode = (FocusModeEnum)(allowPagination ? 2 : 0);
	}

	private void PageLeft()
	{
		if (_paginationAllowed && _canPageLeft && _leaderboard != null)
		{
			DateTimeOffset? dateTimeOffset = DailyRunUtility.AddLeaderboardDays(_currentDay, -1);
			if (dateTimeOffset.HasValue)
			{
				DateTimeOffset valueOrDefault = dateTimeOffset.GetValueOrDefault();
				_currentDay = valueOrDefault;
				DayChangeHelper(pagedLeft: true);
			}
		}
	}

	private void PageRight()
	{
		if (_paginationAllowed && _canPageRight && _leaderboard != null)
		{
			DateTimeOffset? dateTimeOffset = DailyRunUtility.AddLeaderboardDays(_currentDay, 1);
			if (dateTimeOffset.HasValue)
			{
				DateTimeOffset valueOrDefault = dateTimeOffset.GetValueOrDefault();
				_currentDay = valueOrDefault;
				DayChangeHelper(pagedLeft: false);
			}
		}
	}

	private void DayChangeHelper(bool pagedLeft)
	{
		_vfxLabel.SetTextAutoSize(_label.Text);
		_vfxLabel.Modulate = _label.Modulate;
		OnDayChanged(changeLeaderboardDay: true);
		_tween?.Kill();
		_tween = CreateTween().SetParallel();
		_tween.TweenProperty(_label, "position:x", 0f, 0.25).SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Cubic)
			.From(pagedLeft ? (-90f) : 90f);
		_tween.TweenProperty(_label, "modulate:a", 1f, 0.25).From(0.75f);
		_tween.TweenProperty(_vfxLabel, "position:x", pagedLeft ? 90f : (-90f), 0.25).SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Cubic)
			.From(0f);
		_tween.TweenProperty(_vfxLabel, "modulate", StsColors.transparentBlack, 0.25);
	}

	private void OnDayChanged(bool changeLeaderboardDay)
	{
		_label.SetTextAutoSize(_currentDay.ToString(NDailyRunScreen.dateFormat));
		if (changeLeaderboardDay)
		{
			_leaderboard.SetDay(_currentDay);
		}
	}

	public void SetEnabled(bool leftArrowEnabled, bool rightArrowEnabled)
	{
		_canPageLeft = leftArrowEnabled && _paginationAllowed;
		_canPageRight = rightArrowEnabled && _paginationAllowed;
		_leftArrow.SetEnabled(_canPageLeft);
		_rightArrow.SetEnabled(_canPageRight);
	}

	private void OnFocus()
	{
		if (NControllerManager.Instance.IsUsingDirectionalNavigation)
		{
			_selectionReticle.OnSelect();
		}
	}

	private void OnUnfocus()
	{
		_selectionReticle.OnDeselect();
	}

	/// <summary>
	/// Get the method information for all the methods declared in this class.
	/// This method is used by Godot to register the available methods in the editor.
	/// Do not call this method.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		List<MethodInfo> list = new List<MethodInfo>(8);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.PageLeft, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.PageRight, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.DayChangeHelper, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.Bool, "pagedLeft", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
		}, null));
		list.Add(new MethodInfo(MethodName.OnDayChanged, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.Bool, "changeLeaderboardDay", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
		}, null));
		list.Add(new MethodInfo(MethodName.SetEnabled, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.Bool, "leftArrowEnabled", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false),
			new PropertyInfo(Variant.Type.Bool, "rightArrowEnabled", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
		}, null));
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
		if (method == MethodName.PageLeft && args.Count == 0)
		{
			PageLeft();
			ret = default;
			return true;
		}
		if (method == MethodName.PageRight && args.Count == 0)
		{
			PageRight();
			ret = default;
			return true;
		}
		if (method == MethodName.DayChangeHelper && args.Count == 1)
		{
			DayChangeHelper(VariantUtils.ConvertTo<bool>(in args[0]));
			ret = default;
			return true;
		}
		if (method == MethodName.OnDayChanged && args.Count == 1)
		{
			OnDayChanged(VariantUtils.ConvertTo<bool>(in args[0]));
			ret = default;
			return true;
		}
		if (method == MethodName.SetEnabled && args.Count == 2)
		{
			SetEnabled(VariantUtils.ConvertTo<bool>(in args[0]), VariantUtils.ConvertTo<bool>(in args[1]));
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
		if (method == MethodName.PageLeft)
		{
			return true;
		}
		if (method == MethodName.PageRight)
		{
			return true;
		}
		if (method == MethodName.DayChangeHelper)
		{
			return true;
		}
		if (method == MethodName.OnDayChanged)
		{
			return true;
		}
		if (method == MethodName.SetEnabled)
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
		if (name == PropertyName._label)
		{
			_label = VariantUtils.ConvertTo<MegaLabel>(in value);
			return true;
		}
		if (name == PropertyName._vfxLabel)
		{
			_vfxLabel = VariantUtils.ConvertTo<MegaLabel>(in value);
			return true;
		}
		if (name == PropertyName._leftArrow)
		{
			_leftArrow = VariantUtils.ConvertTo<NLeaderboardDayArrow>(in value);
			return true;
		}
		if (name == PropertyName._rightArrow)
		{
			_rightArrow = VariantUtils.ConvertTo<NLeaderboardDayArrow>(in value);
			return true;
		}
		if (name == PropertyName._selectionReticle)
		{
			_selectionReticle = VariantUtils.ConvertTo<NSelectionReticle>(in value);
			return true;
		}
		if (name == PropertyName._tween)
		{
			_tween = VariantUtils.ConvertTo<Tween>(in value);
			return true;
		}
		if (name == PropertyName._leaderboard)
		{
			_leaderboard = VariantUtils.ConvertTo<NDailyRunLeaderboard>(in value);
			return true;
		}
		if (name == PropertyName._canPageLeft)
		{
			_canPageLeft = VariantUtils.ConvertTo<bool>(in value);
			return true;
		}
		if (name == PropertyName._canPageRight)
		{
			_canPageRight = VariantUtils.ConvertTo<bool>(in value);
			return true;
		}
		if (name == PropertyName._paginationAllowed)
		{
			_paginationAllowed = VariantUtils.ConvertTo<bool>(in value);
			return true;
		}
		return base.SetGodotClassPropertyValue(in name, in value);
	}

	/// <inheritdoc />
	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool GetGodotClassPropertyValue(in godot_string_name name, out godot_variant value)
	{
		if (name == PropertyName._label)
		{
			value = VariantUtils.CreateFrom(in _label);
			return true;
		}
		if (name == PropertyName._vfxLabel)
		{
			value = VariantUtils.CreateFrom(in _vfxLabel);
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
		if (name == PropertyName._selectionReticle)
		{
			value = VariantUtils.CreateFrom(in _selectionReticle);
			return true;
		}
		if (name == PropertyName._tween)
		{
			value = VariantUtils.CreateFrom(in _tween);
			return true;
		}
		if (name == PropertyName._leaderboard)
		{
			value = VariantUtils.CreateFrom(in _leaderboard);
			return true;
		}
		if (name == PropertyName._canPageLeft)
		{
			value = VariantUtils.CreateFrom(in _canPageLeft);
			return true;
		}
		if (name == PropertyName._canPageRight)
		{
			value = VariantUtils.CreateFrom(in _canPageRight);
			return true;
		}
		if (name == PropertyName._paginationAllowed)
		{
			value = VariantUtils.CreateFrom(in _paginationAllowed);
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
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._label, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._vfxLabel, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._leftArrow, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._rightArrow, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._selectionReticle, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._tween, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._leaderboard, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Bool, PropertyName._canPageLeft, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Bool, PropertyName._canPageRight, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Bool, PropertyName._paginationAllowed, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		return list;
	}

	/// <inheritdoc />
	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		base.SaveGodotObjectData(info);
		info.AddProperty(PropertyName._label, Variant.From(in _label));
		info.AddProperty(PropertyName._vfxLabel, Variant.From(in _vfxLabel));
		info.AddProperty(PropertyName._leftArrow, Variant.From(in _leftArrow));
		info.AddProperty(PropertyName._rightArrow, Variant.From(in _rightArrow));
		info.AddProperty(PropertyName._selectionReticle, Variant.From(in _selectionReticle));
		info.AddProperty(PropertyName._tween, Variant.From(in _tween));
		info.AddProperty(PropertyName._leaderboard, Variant.From(in _leaderboard));
		info.AddProperty(PropertyName._canPageLeft, Variant.From(in _canPageLeft));
		info.AddProperty(PropertyName._canPageRight, Variant.From(in _canPageRight));
		info.AddProperty(PropertyName._paginationAllowed, Variant.From(in _paginationAllowed));
	}

	/// <inheritdoc />
	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		if (info.TryGetProperty(PropertyName._label, out var value))
		{
			_label = value.As<MegaLabel>();
		}
		if (info.TryGetProperty(PropertyName._vfxLabel, out var value2))
		{
			_vfxLabel = value2.As<MegaLabel>();
		}
		if (info.TryGetProperty(PropertyName._leftArrow, out var value3))
		{
			_leftArrow = value3.As<NLeaderboardDayArrow>();
		}
		if (info.TryGetProperty(PropertyName._rightArrow, out var value4))
		{
			_rightArrow = value4.As<NLeaderboardDayArrow>();
		}
		if (info.TryGetProperty(PropertyName._selectionReticle, out var value5))
		{
			_selectionReticle = value5.As<NSelectionReticle>();
		}
		if (info.TryGetProperty(PropertyName._tween, out var value6))
		{
			_tween = value6.As<Tween>();
		}
		if (info.TryGetProperty(PropertyName._leaderboard, out var value7))
		{
			_leaderboard = value7.As<NDailyRunLeaderboard>();
		}
		if (info.TryGetProperty(PropertyName._canPageLeft, out var value8))
		{
			_canPageLeft = value8.As<bool>();
		}
		if (info.TryGetProperty(PropertyName._canPageRight, out var value9))
		{
			_canPageRight = value9.As<bool>();
		}
		if (info.TryGetProperty(PropertyName._paginationAllowed, out var value10))
		{
			_paginationAllowed = value10.As<bool>();
		}
	}
}
