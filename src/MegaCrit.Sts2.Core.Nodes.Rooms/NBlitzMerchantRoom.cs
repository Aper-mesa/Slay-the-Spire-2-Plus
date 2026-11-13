using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes.Screens.Map;
using MegaCrit.Sts2.Core.Nodes.Screens.Shops;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.TestSupport;

namespace MegaCrit.Sts2.Core.Nodes.Rooms;

[ScriptPath("res://src/Core/Nodes/Rooms/NBlitzMerchantRoom.cs")]
public class NBlitzMerchantRoom : NMerchantRoom
{
	/// <summary>
	/// Cached StringNames for the methods contained in this class, for fast lookup.
	/// </summary>
	public new class MethodName : NMerchantRoom.MethodName
	{
		/// <summary>
		/// Cached name for the '_Ready' method.
		/// </summary>
		public new static readonly StringName _Ready = "_Ready";

		/// <summary>
		/// Cached name for the 'OpenInventory' method.
		/// </summary>
		public new static readonly StringName OpenInventory = "OpenInventory";
	}

	/// <summary>
	/// Cached StringNames for the properties and fields contained in this class, for fast lookup.
	/// </summary>
	public new class PropertyName : NMerchantRoom.PropertyName
	{
		/// <summary>
		/// Cached name for the 'MerchantButton' property.
		/// </summary>
		public new static readonly StringName MerchantButton = "MerchantButton";

		/// <summary>
		/// Cached name for the '_merchantButton' field.
		/// </summary>
		public static readonly StringName _merchantButton = "_merchantButton";

		/// <summary>
		/// Cached name for the '_bgContainer' field.
		/// </summary>
		public static readonly StringName _bgContainer = "_bgContainer";
	}

	/// <summary>
	/// Cached StringNames for the signals contained in this class, for fast lookup.
	/// </summary>
	public new class SignalName : NMerchantRoom.SignalName
	{
	}

	private static readonly string _scenePath = SceneHelper.GetScenePath("rooms/blitz_merchant_room");

	private NBlitzMerchantButton _merchantButton;

	private Control _bgContainer;

	private BackgroundAssets _backgroundAssets;

	public static IEnumerable<string> AssetPaths => new _003C_003Ez__ReadOnlySingleElementList<string>(_scenePath);

	public override NMerchantButton MerchantButton => _merchantButton;

	public static NBlitzMerchantRoom? Create(IReadOnlyList<Player> players, BackgroundAssets backgroundAssets)
	{
		if (TestMode.IsOn)
		{
			return null;
		}
		NBlitzMerchantRoom nBlitzMerchantRoom = PreloadManager.Cache.GetScene(_scenePath).Instantiate<NBlitzMerchantRoom>(PackedScene.GenEditState.Disabled);
		nBlitzMerchantRoom._players.AddRange(players);
		nBlitzMerchantRoom._dialogue = MerchantRoom.Dialogue;
		nBlitzMerchantRoom._backgroundAssets = backgroundAssets;
		return nBlitzMerchantRoom;
	}

	public override void _Ready()
	{
		base._Ready();
		Player me = LocalContext.GetMe(_players);
		_merchantButton = GetNode<NBlitzMerchantButton>("%MerchantButton");
		_merchantButton.IsLocalPlayerDead = me.Creature.IsDead;
		_merchantButton.PlayerDeadLines = _dialogue.PlayerDeadLines;
		_merchantButton.Connect(NMerchantButton.SignalName.MerchantOpened, Callable.From<NMerchantButton>(OnMerchantOpened));
		_bgContainer = GetNode<Control>("%BgContainer");
		NCombatBackground child = NCombatBackground.Create(_backgroundAssets);
		_bgContainer.AddChildSafely(child);
		_merchantButton.Setup(Inventory.Inventory);
		NMapScreen.Instance.SetTravelEnabled(enabled: true);
		NGame.Instance.SetScreenShakeTarget(this);
	}

	public override void OpenInventory()
	{
		if (Inventory.IsOpen)
		{
			return;
		}
		ProceedButton.Disable();
		Inventory.Open();
		MerchantButton.Disable();
		Inventory.Connect(NMerchantInventory.SignalName.InventoryClosed, Callable.From(() =>
		{
			MerchantButton.Enable();
			ProceedButton.Enable();
			ProceedButton.SetPulseState(isPulsing: true);
			if (Inventory.Inventory.IsBlitzShop && Inventory.Inventory.BlitzChoicesRemaining <= 0)
			{
				_merchantButton.CompleteShop();
			}
		}), 4u);
	}

	/// <summary>
	/// Get the method information for all the methods declared in this class.
	/// This method is used by Godot to register the available methods in the editor.
	/// Do not call this method.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Never)]
	internal new static List<MethodInfo> GetGodotMethodList()
	{
		List<MethodInfo> list = new List<MethodInfo>(2);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.OpenInventory, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
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
		if (method == MethodName.OpenInventory && args.Count == 0)
		{
			OpenInventory();
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
		if (method == MethodName.OpenInventory)
		{
			return true;
		}
		return base.HasGodotClassMethod(in method);
	}

	/// <inheritdoc />
	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if (name == PropertyName._merchantButton)
		{
			_merchantButton = VariantUtils.ConvertTo<NBlitzMerchantButton>(in value);
			return true;
		}
		if (name == PropertyName._bgContainer)
		{
			_bgContainer = VariantUtils.ConvertTo<Control>(in value);
			return true;
		}
		return base.SetGodotClassPropertyValue(in name, in value);
	}

	/// <inheritdoc />
	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool GetGodotClassPropertyValue(in godot_string_name name, out godot_variant value)
	{
		if (name == PropertyName.MerchantButton)
		{
			value = VariantUtils.CreateFrom<NMerchantButton>(MerchantButton);
			return true;
		}
		if (name == PropertyName._merchantButton)
		{
			value = VariantUtils.CreateFrom(in _merchantButton);
			return true;
		}
		if (name == PropertyName._bgContainer)
		{
			value = VariantUtils.CreateFrom(in _bgContainer);
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
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName.MerchantButton, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._merchantButton, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._bgContainer, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		return list;
	}

	/// <inheritdoc />
	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		base.SaveGodotObjectData(info);
		info.AddProperty(PropertyName._merchantButton, Variant.From(in _merchantButton));
		info.AddProperty(PropertyName._bgContainer, Variant.From(in _bgContainer));
	}

	/// <inheritdoc />
	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		if (info.TryGetProperty(PropertyName._merchantButton, out var value))
		{
			_merchantButton = value.As<NBlitzMerchantButton>();
		}
		if (info.TryGetProperty(PropertyName._bgContainer, out var value2))
		{
			_bgContainer = value2.As<Control>();
		}
	}
}
