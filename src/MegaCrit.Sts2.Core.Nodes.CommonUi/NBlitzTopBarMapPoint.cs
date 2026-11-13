using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Map;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.HoverTips;
using MegaCrit.Sts2.Core.Nodes.Screens.Map;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Runs.History;

namespace MegaCrit.Sts2.Core.Nodes.CommonUi;

/// <summary>
/// A node in the blitz map shown in the top bar.
/// Since it is not a real map point (i.e. not shown in NMapScreen), it doesn't inherit from NMapPoint.
/// </summary>
[ScriptPath("res://src/Core/Nodes/CommonUi/NBlitzTopBarMapPoint.cs")]
public class NBlitzTopBarMapPoint : NClickableControl
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
		/// Cached name for the 'UpdateIcon' method.
		/// </summary>
		public static readonly StringName UpdateIcon = "UpdateIcon";

		/// <summary>
		/// Cached name for the 'ShowCircleVfx' method.
		/// </summary>
		public static readonly StringName ShowCircleVfx = "ShowCircleVfx";

		/// <summary>
		/// Cached name for the 'HideCircleVfx' method.
		/// </summary>
		public static readonly StringName HideCircleVfx = "HideCircleVfx";

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
		/// Cached name for the '_icon' field.
		/// </summary>
		public static readonly StringName _icon = "_icon";

		/// <summary>
		/// Cached name for the '_outline' field.
		/// </summary>
		public static readonly StringName _outline = "_outline";

		/// <summary>
		/// Cached name for the '_controllerSelectionReticle' field.
		/// </summary>
		public static readonly StringName _controllerSelectionReticle = "_controllerSelectionReticle";

		/// <summary>
		/// Cached name for the '_circleVfx' field.
		/// </summary>
		public static readonly StringName _circleVfx = "_circleVfx";
	}

	/// <summary>
	/// Cached StringNames for the signals contained in this class, for fast lookup.
	/// </summary>
	public new class SignalName : NClickableControl.SignalName
	{
	}

	private static readonly string _scenePath = SceneHelper.GetScenePath("/ui/blitz_map_point");

	private const float _minCircleScale = 0.65f;

	private const float _maxCircleScale = 0.7f;

	private TextureRect _icon;

	private TextureRect _outline;

	private NSelectionReticle _controllerSelectionReticle;

	private MapPoint _mapPoint;

	private IRunState _runState;

	private NMapCircleVfx? _circleVfx;

	public MapPoint Point => _mapPoint;

	public static NBlitzTopBarMapPoint Create(MapPoint point, IRunState runState)
	{
		NBlitzTopBarMapPoint nBlitzTopBarMapPoint = PreloadManager.Cache.GetScene(_scenePath).Instantiate<NBlitzTopBarMapPoint>(PackedScene.GenEditState.Disabled);
		nBlitzTopBarMapPoint._mapPoint = point;
		nBlitzTopBarMapPoint._runState = runState;
		return nBlitzTopBarMapPoint;
	}

	public override void _Ready()
	{
		ConnectSignals();
		_icon = GetNode<TextureRect>("%Icon");
		_outline = GetNode<TextureRect>("%Outline");
		_controllerSelectionReticle = GetNode<NSelectionReticle>("%SelectionReticle");
		MapPointType pointType = _mapPoint.PointType;
		if ((uint)(pointType - 7) <= 1u)
		{
			_icon.SetAnchorAndOffset(Side.Left, 0f, -10f);
			_icon.SetAnchorAndOffset(Side.Top, 0f, -10f);
			_icon.SetAnchorAndOffset(Side.Right, 1f, 10f);
			_icon.SetAnchorAndOffset(Side.Bottom, 1f, 10f);
		}
		UpdateIcon();
	}

	private void UpdateIcon()
	{
		MapPointType pointType = _mapPoint.PointType;
		if ((uint)(pointType - 7) <= 1u)
		{
			string roomIconPath;
			string roomIconOutlinePath;
			if (_mapPoint.PointType == MapPointType.Ancient)
			{
				roomIconPath = ImageHelper.GetRoomIconPath(_mapPoint.PointType, RoomType.Event, _runState.Act.Ancient.Id);
				roomIconOutlinePath = ImageHelper.GetRoomIconOutlinePath(_mapPoint.PointType, RoomType.Event, _runState.Act.Ancient.Id);
			}
			else
			{
				roomIconPath = ImageHelper.GetRoomIconPath(_mapPoint.PointType, RoomType.Boss, _runState.Act.BossEncounter.Id);
				roomIconOutlinePath = ImageHelper.GetRoomIconOutlinePath(_mapPoint.PointType, RoomType.Boss, _runState.Act.BossEncounter.Id);
			}
			_icon.Texture = ((roomIconPath == null) ? null : ResourceLoader.Load<Texture2D>(roomIconPath, null, ResourceLoader.CacheMode.Reuse));
			_outline.Texture = ((roomIconOutlinePath == null) ? null : ResourceLoader.Load<Texture2D>(roomIconOutlinePath, null, ResourceLoader.CacheMode.Reuse));
		}
		else if (_mapPoint.PointType != MapPointType.Unknown)
		{
			string filename = NNormalMapPoint.IconName(_mapPoint.PointType);
			string path = NNormalMapPoint.IconPath(filename);
			_icon.Texture = ResourceLoader.Load<Texture2D>(path, null, ResourceLoader.CacheMode.Reuse);
			string path2 = NNormalMapPoint.OutlinePath(NNormalMapPoint.IconName(_mapPoint.PointType));
			_outline.Texture = ResourceLoader.Load<Texture2D>(path2, null, ResourceLoader.CacheMode.Reuse);
		}
		else if (_runState.MapPointHistory.Count > _runState.CurrentActIndex)
		{
			IReadOnlyList<MapPointHistoryEntry> readOnlyList = _runState.MapPointHistory[_runState.CurrentActIndex];
			if (readOnlyList.Count > _mapPoint.coord.row)
			{
				RoomType roomType = readOnlyList[_mapPoint.coord.row].Rooms.First().RoomType;
				_icon.Texture = ResourceLoader.Load<Texture2D>(NNormalMapPoint.UnknownIconPath(roomType), null, ResourceLoader.CacheMode.Reuse);
				_outline.Texture = ResourceLoader.Load<Texture2D>(NNormalMapPoint.UnknownOutlinePath(roomType), null, ResourceLoader.CacheMode.Reuse);
			}
		}
	}

	public void ShowCircleVfx(bool playAnim)
	{
		if (_circleVfx == null)
		{
			_circleVfx = NMapCircleVfx.Create(_runState, _mapPoint.coord, playAnim, 0.65f, 0.7f);
			this.AddChildSafely(_circleVfx);
			_circleVfx.Position += PivotOffset;
		}
	}

	public void HideCircleVfx()
	{
		_circleVfx?.QueueFreeSafely();
		_circleVfx = null;
	}

	protected override void OnFocus()
	{
		if (_isEnabled && NControllerManager.Instance.IsUsingDirectionalNavigation)
		{
			_controllerSelectionReticle.OnSelect();
		}
		MapPointHistoryEntry historyEntryFor = _runState.GetHistoryEntryFor(new MapLocation(Point.coord, _runState.CurrentActIndex));
		NHoverTipSet nHoverTipSet;
		if (historyEntryFor != null && _runState.MapLocation.coord != Point.coord)
		{
			int num = Point.coord.row + 1;
			for (int i = 0; i < _runState.MapPointHistory.Count - 1; i++)
			{
				num += _runState.MapPointHistory[i].Count;
			}
			nHoverTipSet = NHoverTipSet.CreateAndShowMapPointHistory(this, NMapPointHistoryHoverTip.Create(num, LocalContext.NetId.Value, historyEntryFor, _runState.GameMode));
		}
		else
		{
			MapPointType pointType = Point.PointType;
			LocString locString = pointType switch
			{
				MapPointType.Unassigned => throw new NotImplementedException(),
				MapPointType.Unknown => new LocString("map", "LEGEND_UNKNOWN"),
				MapPointType.Shop => new LocString("map", "LEGEND_MERCHANT"),
				MapPointType.Treasure => new LocString("map", "LEGEND_TREASURE"),
				MapPointType.RestSite => new LocString("map", "LEGEND_REST"),
				MapPointType.Monster => new LocString("map", "LEGEND_ENEMY"),
				MapPointType.Elite => new LocString("map", "LEGEND_ELITE"),
				MapPointType.Ancient => new LocString("map", "LEGEND_ANCIENT"),
				MapPointType.Boss => new LocString("map", "LEGEND_BOSS"),
			};
			LocString title = new LocString(locString.LocTable, locString.LocEntryKey + ".hoverTip.title");
			LocString locString2 = new LocString(locString.LocTable, locString.LocEntryKey + ".hoverTip.description");
			HoverTip hoverTip;
			if (Point.PointType == MapPointType.Boss)
			{
				LocString locString3 = new LocString("static_hover_tips", "BOSS.description");
				locString3.Add("BossName", _runState.Act.BossEncounter.Title);
				hoverTip = new HoverTip(title, locString2.GetFormattedText() + "\n\n" + locString3.GetFormattedText());
			}
			else
			{
				hoverTip = new HoverTip(title, locString2);
			}
			nHoverTipSet = NHoverTipSet.CreateAndShow(this, hoverTip);
		}
		if (nHoverTipSet != null)
		{
			nHoverTipSet.GlobalPosition = new Vector2(GlobalPosition.X + Size.X * 0.5f - nHoverTipSet.Size.X * 0.5f, GlobalPosition.Y + Size.Y + 20f);
		}
	}

	protected override void OnUnfocus()
	{
		_controllerSelectionReticle.OnDeselect();
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
		list.Add(new MethodInfo(MethodName.UpdateIcon, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.ShowCircleVfx, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.Bool, "playAnim", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
		}, null));
		list.Add(new MethodInfo(MethodName.HideCircleVfx, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
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
		if (method == MethodName.UpdateIcon && args.Count == 0)
		{
			UpdateIcon();
			ret = default;
			return true;
		}
		if (method == MethodName.ShowCircleVfx && args.Count == 1)
		{
			ShowCircleVfx(VariantUtils.ConvertTo<bool>(in args[0]));
			ret = default;
			return true;
		}
		if (method == MethodName.HideCircleVfx && args.Count == 0)
		{
			HideCircleVfx();
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
		if (method == MethodName.UpdateIcon)
		{
			return true;
		}
		if (method == MethodName.ShowCircleVfx)
		{
			return true;
		}
		if (method == MethodName.HideCircleVfx)
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
		if (name == PropertyName._icon)
		{
			_icon = VariantUtils.ConvertTo<TextureRect>(in value);
			return true;
		}
		if (name == PropertyName._outline)
		{
			_outline = VariantUtils.ConvertTo<TextureRect>(in value);
			return true;
		}
		if (name == PropertyName._controllerSelectionReticle)
		{
			_controllerSelectionReticle = VariantUtils.ConvertTo<NSelectionReticle>(in value);
			return true;
		}
		if (name == PropertyName._circleVfx)
		{
			_circleVfx = VariantUtils.ConvertTo<NMapCircleVfx>(in value);
			return true;
		}
		return base.SetGodotClassPropertyValue(in name, in value);
	}

	/// <inheritdoc />
	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool GetGodotClassPropertyValue(in godot_string_name name, out godot_variant value)
	{
		if (name == PropertyName._icon)
		{
			value = VariantUtils.CreateFrom(in _icon);
			return true;
		}
		if (name == PropertyName._outline)
		{
			value = VariantUtils.CreateFrom(in _outline);
			return true;
		}
		if (name == PropertyName._controllerSelectionReticle)
		{
			value = VariantUtils.CreateFrom(in _controllerSelectionReticle);
			return true;
		}
		if (name == PropertyName._circleVfx)
		{
			value = VariantUtils.CreateFrom(in _circleVfx);
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
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._icon, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._outline, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._controllerSelectionReticle, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._circleVfx, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		return list;
	}

	/// <inheritdoc />
	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		base.SaveGodotObjectData(info);
		info.AddProperty(PropertyName._icon, Variant.From(in _icon));
		info.AddProperty(PropertyName._outline, Variant.From(in _outline));
		info.AddProperty(PropertyName._controllerSelectionReticle, Variant.From(in _controllerSelectionReticle));
		info.AddProperty(PropertyName._circleVfx, Variant.From(in _circleVfx));
	}

	/// <inheritdoc />
	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		if (info.TryGetProperty(PropertyName._icon, out var value))
		{
			_icon = value.As<TextureRect>();
		}
		if (info.TryGetProperty(PropertyName._outline, out var value2))
		{
			_outline = value2.As<TextureRect>();
		}
		if (info.TryGetProperty(PropertyName._controllerSelectionReticle, out var value3))
		{
			_controllerSelectionReticle = value3.As<NSelectionReticle>();
		}
		if (info.TryGetProperty(PropertyName._circleVfx, out var value4))
		{
			_circleVfx = value4.As<NMapCircleVfx>();
		}
	}
}
