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

[ScriptPath("res://src/Core/Nodes/Rooms/NStandardMerchantRoom.cs")]
public class NStandardMerchantRoom : NMerchantRoom
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
	}

	/// <summary>
	/// Cached StringNames for the signals contained in this class, for fast lookup.
	/// </summary>
	public new class SignalName : NMerchantRoom.SignalName
	{
	}

	private static readonly string _scenePath = SceneHelper.GetScenePath("rooms/standard_merchant_room");

	private NMerchantButton _merchantButton;

	public static IEnumerable<string> AssetPaths => new _003C_003Ez__ReadOnlySingleElementList<string>(_scenePath);

	public override NMerchantButton MerchantButton => _merchantButton;

	public static NStandardMerchantRoom? Create(IReadOnlyList<Player> players)
	{
		if (TestMode.IsOn)
		{
			return null;
		}
		NStandardMerchantRoom nStandardMerchantRoom = PreloadManager.Cache.GetScene(_scenePath).Instantiate<NStandardMerchantRoom>(PackedScene.GenEditState.Disabled);
		nStandardMerchantRoom._players.AddRange(players);
		nStandardMerchantRoom._dialogue = MerchantRoom.Dialogue;
		return nStandardMerchantRoom;
	}

	public override void _Ready()
	{
		base._Ready();
		Player me = LocalContext.GetMe(_players);
		_merchantButton = GetNode<NMerchantButton>("%MerchantButton");
		_merchantButton.IsLocalPlayerDead = me.Creature.IsDead;
		_merchantButton.PlayerDeadLines = _dialogue.PlayerDeadLines;
		_merchantButton.Connect(NMerchantButton.SignalName.MerchantOpened, Callable.From<NMerchantButton>(OnMerchantOpened));
		NMapScreen.Instance.SetTravelEnabled(enabled: true);
		NGame.Instance.SetScreenShakeTarget(this);
	}

	public override void OpenInventory()
	{
		if (!Inventory.IsOpen)
		{
			ProceedButton.Disable();
			Inventory.Open();
			MerchantButton.Disable();
			Inventory.Connect(NMerchantInventory.SignalName.InventoryClosed, Callable.From(() =>
			{
				MerchantButton.Enable();
				ProceedButton.Enable();
				ProceedButton.SetPulseState(isPulsing: true);
			}), 4u);
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
			_merchantButton = VariantUtils.ConvertTo<NMerchantButton>(in value);
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
		return list;
	}

	/// <inheritdoc />
	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		base.SaveGodotObjectData(info);
		info.AddProperty(PropertyName._merchantButton, Variant.From(in _merchantButton));
	}

	/// <inheritdoc />
	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		if (info.TryGetProperty(PropertyName._merchantButton, out var value))
		{
			_merchantButton = value.As<NMerchantButton>();
		}
	}
}
