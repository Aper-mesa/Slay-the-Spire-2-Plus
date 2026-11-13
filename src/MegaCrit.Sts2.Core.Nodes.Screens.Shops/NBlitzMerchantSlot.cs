using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Entities.Merchant;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Nodes.HoverTips;

namespace MegaCrit.Sts2.Core.Nodes.Screens.Shops;

/// <summary>
/// UI wrapper class for MerchantEntry.
/// </summary>
[ScriptPath("res://src/Core/Nodes/Screens/Shops/NBlitzMerchantSlot.cs")]
public class NBlitzMerchantSlot : NMerchantSlot
{
	/// <summary>
	/// Cached StringNames for the methods contained in this class, for fast lookup.
	/// </summary>
	public new class MethodName : NMerchantSlot.MethodName
	{
		/// <summary>
		/// Cached name for the '_Ready' method.
		/// </summary>
		public new static readonly StringName _Ready = "_Ready";

		/// <summary>
		/// Cached name for the 'UpdateVisual' method.
		/// </summary>
		public new static readonly StringName UpdateVisual = "UpdateVisual";

		/// <summary>
		/// Cached name for the 'CreateHoverTip' method.
		/// </summary>
		public new static readonly StringName CreateHoverTip = "CreateHoverTip";

		/// <summary>
		/// Cached name for the '_ExitTree' method.
		/// </summary>
		public new static readonly StringName _ExitTree = "_ExitTree";
	}

	/// <summary>
	/// Cached StringNames for the properties and fields contained in this class, for fast lookup.
	/// </summary>
	public new class PropertyName : NMerchantSlot.PropertyName
	{
		/// <summary>
		/// Cached name for the 'Visual' property.
		/// </summary>
		public new static readonly StringName Visual = "Visual";

		/// <summary>
		/// Cached name for the 'HoverScale' property.
		/// </summary>
		public new static readonly StringName HoverScale = "HoverScale";

		/// <summary>
		/// Cached name for the 'SmallScale' property.
		/// </summary>
		public new static readonly StringName SmallScale = "SmallScale";

		/// <summary>
		/// Cached name for the 'MerchantHandOffset' property.
		/// </summary>
		public new static readonly StringName MerchantHandOffset = "MerchantHandOffset";

		/// <summary>
		/// Cached name for the '_icon' field.
		/// </summary>
		public static readonly StringName _icon = "_icon";

		/// <summary>
		/// Cached name for the '_shadow' field.
		/// </summary>
		public static readonly StringName _shadow = "_shadow";

		/// <summary>
		/// Cached name for the '_rareVfx' field.
		/// </summary>
		public static readonly StringName _rareVfx = "_rareVfx";

		/// <summary>
		/// Cached name for the '_cursedVfx' field.
		/// </summary>
		public static readonly StringName _cursedVfx = "_cursedVfx";

		/// <summary>
		/// Cached name for the '_alreadyDisabled' field.
		/// </summary>
		public static readonly StringName _alreadyDisabled = "_alreadyDisabled";
	}

	/// <summary>
	/// Cached StringNames for the signals contained in this class, for fast lookup.
	/// </summary>
	public new class SignalName : NMerchantSlot.SignalName
	{
	}

	private BlitzMerchantEntry _blitzEntry;

	private TextureRect _icon;

	private TextureRect _shadow;

	private GpuParticles2D _rareVfx;

	private GpuParticles2D _cursedVfx;

	private bool _alreadyDisabled;

	public override MerchantEntry Entry => _blitzEntry;

	protected override CanvasItem Visual => _icon;

	protected override Vector2 HoverScale => Vector2.One * 1.1f;

	protected override Vector2 SmallScale => Vector2.One;

	public override Vector2 MerchantHandOffset => base.MerchantHandOffset + Vector2.Left * 18f;

	public override void _Ready()
	{
		ConnectSignals();
		_icon = GetNode<TextureRect>("%Icon");
		_shadow = GetNode<TextureRect>("%Shadow");
		_rareVfx = GetNode<GpuParticles2D>("%RareVfx");
		_cursedVfx = GetNode<GpuParticles2D>("%CursedVfx");
	}

	public void FillSlot(BlitzMerchantEntry entry)
	{
		_blitzEntry = entry;
		_blitzEntry.EntryUpdated += UpdateVisual;
		_blitzEntry.PurchaseFailed += OnPurchaseFailed;
		_blitzEntry.PurchaseCompleted += OnSuccessfulPurchase;
		Texture2D icon = _blitzEntry.Icon;
		_icon.Texture = icon;
		_shadow.Texture = icon;
		_rareVfx.Visible = _blitzEntry.IsRare;
		_cursedVfx.Visible = _blitzEntry.IsCursed;
		UpdateVisual();
	}

	protected override void UpdateVisual()
	{
		base.UpdateVisual();
		if (_blitzEntry.Status != BlitzMerchantEntry.BlitzEntryStatus.Available)
		{
			if (!_alreadyDisabled)
			{
				_hitbox.MouseFilter = MouseFilterEnum.Ignore;
				_alreadyDisabled = true;
				FocusMode = FocusModeEnum.None;
				if (_blitzEntry.Status == BlitzMerchantEntry.BlitzEntryStatus.Used)
				{
					_icon.SetVisible(visible: false);
					_rareVfx.SetVisible(visible: false);
					_cursedVfx.SetVisible(visible: false);
				}
				else
				{
					Modulate = Colors.DarkGray;
				}
			}
		}
		else
		{
			MouseFilter = MouseFilterEnum.Stop;
			_costLabel.Visible = true;
			FocusMode = FocusModeEnum.All;
		}
		ClearHoverTip();
	}

	private void OnSuccessfulPurchase(PurchaseStatus _, MerchantEntry __)
	{
		TriggerMerchantHandToPointHere();
		UpdateVisual();
	}

	protected override void CreateHoverTip()
	{
		HoverTipAlignment alignment = ((GetIndex() % 3 == 2) ? HoverTipAlignment.Left : HoverTipAlignment.Right);
		NHoverTipSet.CreateAndShow(this, _blitzEntry.HoverTips)?.SetAlignment(_hitbox, alignment);
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		if (IsInitialized)
		{
			_blitzEntry.EntryUpdated -= UpdateVisual;
			_blitzEntry.PurchaseFailed -= OnPurchaseFailed;
			_blitzEntry.PurchaseCompleted -= OnSuccessfulPurchase;
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
		List<MethodInfo> list = new List<MethodInfo>(4);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.UpdateVisual, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.CreateHoverTip, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
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
		if (method == MethodName.UpdateVisual && args.Count == 0)
		{
			UpdateVisual();
			ret = default;
			return true;
		}
		if (method == MethodName.CreateHoverTip && args.Count == 0)
		{
			CreateHoverTip();
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
		if (method == MethodName.UpdateVisual)
		{
			return true;
		}
		if (method == MethodName.CreateHoverTip)
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
		if (name == PropertyName._icon)
		{
			_icon = VariantUtils.ConvertTo<TextureRect>(in value);
			return true;
		}
		if (name == PropertyName._shadow)
		{
			_shadow = VariantUtils.ConvertTo<TextureRect>(in value);
			return true;
		}
		if (name == PropertyName._rareVfx)
		{
			_rareVfx = VariantUtils.ConvertTo<GpuParticles2D>(in value);
			return true;
		}
		if (name == PropertyName._cursedVfx)
		{
			_cursedVfx = VariantUtils.ConvertTo<GpuParticles2D>(in value);
			return true;
		}
		if (name == PropertyName._alreadyDisabled)
		{
			_alreadyDisabled = VariantUtils.ConvertTo<bool>(in value);
			return true;
		}
		return base.SetGodotClassPropertyValue(in name, in value);
	}

	/// <inheritdoc />
	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool GetGodotClassPropertyValue(in godot_string_name name, out godot_variant value)
	{
		if (name == PropertyName.Visual)
		{
			value = VariantUtils.CreateFrom<CanvasItem>(Visual);
			return true;
		}
		Vector2 from;
		if (name == PropertyName.HoverScale)
		{
			from = HoverScale;
			value = VariantUtils.CreateFrom(in from);
			return true;
		}
		if (name == PropertyName.SmallScale)
		{
			from = SmallScale;
			value = VariantUtils.CreateFrom(in from);
			return true;
		}
		if (name == PropertyName.MerchantHandOffset)
		{
			from = MerchantHandOffset;
			value = VariantUtils.CreateFrom(in from);
			return true;
		}
		if (name == PropertyName._icon)
		{
			value = VariantUtils.CreateFrom(in _icon);
			return true;
		}
		if (name == PropertyName._shadow)
		{
			value = VariantUtils.CreateFrom(in _shadow);
			return true;
		}
		if (name == PropertyName._rareVfx)
		{
			value = VariantUtils.CreateFrom(in _rareVfx);
			return true;
		}
		if (name == PropertyName._cursedVfx)
		{
			value = VariantUtils.CreateFrom(in _cursedVfx);
			return true;
		}
		if (name == PropertyName._alreadyDisabled)
		{
			value = VariantUtils.CreateFrom(in _alreadyDisabled);
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
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._shadow, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._rareVfx, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._cursedVfx, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Bool, PropertyName._alreadyDisabled, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName.Visual, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Vector2, PropertyName.HoverScale, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Vector2, PropertyName.SmallScale, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Vector2, PropertyName.MerchantHandOffset, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		return list;
	}

	/// <inheritdoc />
	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		base.SaveGodotObjectData(info);
		info.AddProperty(PropertyName._icon, Variant.From(in _icon));
		info.AddProperty(PropertyName._shadow, Variant.From(in _shadow));
		info.AddProperty(PropertyName._rareVfx, Variant.From(in _rareVfx));
		info.AddProperty(PropertyName._cursedVfx, Variant.From(in _cursedVfx));
		info.AddProperty(PropertyName._alreadyDisabled, Variant.From(in _alreadyDisabled));
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
		if (info.TryGetProperty(PropertyName._shadow, out var value2))
		{
			_shadow = value2.As<TextureRect>();
		}
		if (info.TryGetProperty(PropertyName._rareVfx, out var value3))
		{
			_rareVfx = value3.As<GpuParticles2D>();
		}
		if (info.TryGetProperty(PropertyName._cursedVfx, out var value4))
		{
			_cursedVfx = value4.As<GpuParticles2D>();
		}
		if (info.TryGetProperty(PropertyName._alreadyDisabled, out var value5))
		{
			_alreadyDisabled = value5.As<bool>();
		}
	}
}
