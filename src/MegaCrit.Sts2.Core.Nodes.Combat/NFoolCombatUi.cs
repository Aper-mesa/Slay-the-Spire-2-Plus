using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.FoolArrows;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models.Characters;

namespace MegaCrit.Sts2.Core.Nodes.Combat;

/// <summary>
/// Special UI specifically for the Fool that tracks Arrows and Symbols for the Fool.
/// </summary>
[ScriptPath("res://src/Core/Nodes/Combat/NFoolCombatUi.cs")]
public class NFoolCombatUi : Control
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
		/// Cached name for the '_EnterTree' method.
		/// </summary>
		public new static readonly StringName _EnterTree = "_EnterTree";

		/// <summary>
		/// Cached name for the '_ExitTree' method.
		/// </summary>
		public new static readonly StringName _ExitTree = "_ExitTree";

		/// <summary>
		/// Cached name for the 'ConnectArrowsAndSymbolsChangedSignal' method.
		/// </summary>
		public static readonly StringName ConnectArrowsAndSymbolsChangedSignal = "ConnectArrowsAndSymbolsChangedSignal";

		/// <summary>
		/// Cached name for the 'RefreshVisibility' method.
		/// </summary>
		public static readonly StringName RefreshVisibility = "RefreshVisibility";

		/// <summary>
		/// Cached name for the 'UpdateArrows' method.
		/// </summary>
		public static readonly StringName UpdateArrows = "UpdateArrows";

		/// <summary>
		/// Cached name for the 'SetMelodyAmounts' method.
		/// </summary>
		public static readonly StringName SetMelodyAmounts = "SetMelodyAmounts";

		/// <summary>
		/// Cached name for the 'Place' method.
		/// </summary>
		public static readonly StringName Place = "Place";

		/// <summary>
		/// Cached name for the 'HasMelody' method.
		/// </summary>
		public static readonly StringName HasMelody = "HasMelody";
	}

	/// <summary>
	/// Cached StringNames for the properties and fields contained in this class, for fast lookup.
	/// </summary>
	public new class PropertyName : Control.PropertyName
	{
		/// <summary>
		/// Cached name for the '_symbolContainer' field.
		/// </summary>
		public static readonly StringName _symbolContainer = "_symbolContainer";

		/// <summary>
		/// Cached name for the '_symbolPos0' field.
		/// </summary>
		public static readonly StringName _symbolPos0 = "_symbolPos0";

		/// <summary>
		/// Cached name for the '_symbolPos1' field.
		/// </summary>
		public static readonly StringName _symbolPos1 = "_symbolPos1";

		/// <summary>
		/// Cached name for the '_symbolPos2' field.
		/// </summary>
		public static readonly StringName _symbolPos2 = "_symbolPos2";

		/// <summary>
		/// Cached name for the '_symbolPos3' field.
		/// </summary>
		public static readonly StringName _symbolPos3 = "_symbolPos3";

		/// <summary>
		/// Cached name for the '_symbolPos4' field.
		/// </summary>
		public static readonly StringName _symbolPos4 = "_symbolPos4";

		/// <summary>
		/// Cached name for the '_arrowContainer' field.
		/// </summary>
		public static readonly StringName _arrowContainer = "_arrowContainer";

		/// <summary>
		/// Cached name for the '_isListeningToCombatState' field.
		/// </summary>
		public static readonly StringName _isListeningToCombatState = "_isListeningToCombatState";
	}

	/// <summary>
	/// Cached StringNames for the signals contained in this class, for fast lookup.
	/// </summary>
	public new class SignalName : Control.SignalName
	{
	}

	private Player? _player;

	private Control _symbolContainer;

	private Node2D _symbolPos0;

	private Node2D _symbolPos1;

	private Node2D _symbolPos2;

	private Node2D _symbolPos3;

	private Node2D _symbolPos4;

	private Control _arrowContainer;

	private static readonly Vector2 _symbolOffset = new Vector2(-42f, -42f);

	private bool _isListeningToCombatState;

	private List<CardTag> MelodyTags => new List<CardTag>
	{
		CardTag.Wah,
		CardTag.Cho,
		CardTag.Ki
	};

	public override void _Ready()
	{
		_symbolContainer = GetNode<Control>("%SymbolContainer");
		_symbolPos0 = GetNode<Node2D>("%Pos0");
		_symbolPos1 = GetNode<Node2D>("%Pos1");
		_symbolPos2 = GetNode<Node2D>("%Pos2");
		_symbolPos3 = GetNode<Node2D>("%Pos3");
		_symbolPos4 = GetNode<Node2D>("%Pos4");
		_arrowContainer = GetNode<Control>("%ArrowContainer");
		Visible = false;
	}

	public void Initialize(Player player)
	{
		_player = player;
		ConnectArrowsAndSymbolsChangedSignal();
		RefreshVisibility();
		UpdateArrows();
	}

	public override void _EnterTree()
	{
		base._EnterTree();
		ConnectArrowsAndSymbolsChangedSignal();
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		if (_player != null && _isListeningToCombatState)
		{
			_player.PlayerCombatState.ArrowsChanged -= UpdateArrows;
			_player.PlayerCombatState.ArrowsChanged -= RefreshVisibility;
			CombatManager.Instance.StateTracker.CombatStateChanged -= OnSymbolsChanged;
			CombatManager.Instance.StateTracker.CombatStateChanged -= RefreshVisibility;
			_isListeningToCombatState = false;
		}
	}

	private void ConnectArrowsAndSymbolsChangedSignal()
	{
		if (_player != null && !_isListeningToCombatState)
		{
			_player.PlayerCombatState.ArrowsChanged += UpdateArrows;
			_player.PlayerCombatState.ArrowsChanged += RefreshVisibility;
			CombatManager.Instance.StateTracker.CombatStateChanged += OnSymbolsChanged;
			CombatManager.Instance.StateTracker.CombatStateChanged += RefreshVisibility;
			_isListeningToCombatState = true;
		}
	}

	private void RefreshVisibility(CombatState? _)
	{
		RefreshVisibility();
	}

	private void RefreshVisibility()
	{
		if (!Visible)
		{
			if (_player == null)
			{
				Visible = false;
			}
			else if (_player.Character is Fool || _player.PlayerCombatState.Arrows.Count > 0 || GetSymbols().Count > 0)
			{
				Visible = true;
			}
		}
	}

	private void UpdateArrows()
	{
		IReadOnlyList<ArrowSlot> arrows = _player.PlayerCombatState.Arrows;
		HashSet<ArrowSlot> hashSet = new HashSet<ArrowSlot>();
		foreach (NFoolArrowUi item in _arrowContainer.GetChildren().OfType<NFoolArrowUi>())
		{
			if (arrows.Contains(item.ArrowSlot))
			{
				hashSet.Add(item.ArrowSlot);
				continue;
			}
			_arrowContainer.RemoveChildSafely(item);
			item.QueueFreeSafely();
		}
		for (int i = 0; i < arrows.Count; i++)
		{
			ArrowSlot arrowSlot = arrows[i];
			if (!hashSet.Contains(arrowSlot))
			{
				NFoolArrowUi nFoolArrowUi = NFoolArrowUi.Create(arrowSlot, i);
				_arrowContainer.AddChildSafely(nFoolArrowUi);
				nFoolArrowUi.SetVisible(arrowSlot.Status != ArrowStatus.Destroyed);
			}
		}
	}

	private void OnSymbolsChanged(CombatState? _)
	{
		if (_player != null)
		{
			SetMelodyAmounts();
		}
	}

	private void SetMelodyAmounts()
	{
		foreach (CardTag melodyTag in MelodyTags)
		{
			bool flag = MelodyCmd.PlayedThisTurn(_player, melodyTag);
			NFoolCombatSymbol nFoolCombatSymbol = HasMelody(melodyTag);
			if ((nFoolCombatSymbol == null) & flag)
			{
				nFoolCombatSymbol = NFoolCombatSymbol.Create(melodyTag);
				_symbolContainer.AddChildSafely(nFoolCombatSymbol);
				nFoolCombatSymbol.SetGlobalPosition(_symbolPos2.GlobalPosition + _symbolOffset);
			}
			else if (nFoolCombatSymbol != null && !flag)
			{
				nFoolCombatSymbol.SetVisible(visible: false);
				nFoolCombatSymbol.QueueFreeSafely();
			}
		}
		List<NFoolCombatSymbol> symbols = GetSymbols();
		switch (symbols.Count)
		{
		case 1:
			symbols[0].SetGlobalPosition(_symbolPos2.GlobalPosition + _symbolOffset);
			break;
		case 2:
			Place(symbols[0], _symbolPos1);
			Place(symbols[1], _symbolPos3);
			break;
		case 3:
			Place(symbols[0], _symbolPos0);
			Place(symbols[1], _symbolPos2);
			Place(symbols[2], _symbolPos4);
			break;
		}
	}

	/// <summary>
	/// Helper function for weirdo symbol placement UX.
	/// </summary>
	/// <param name="symbol"></param>
	/// <param name="slot"></param>
	private static void Place(NFoolCombatSymbol symbol, Node2D slot)
	{
		Vector2 targetPosition = slot.GlobalPosition + _symbolOffset;
		symbol.MoveTo(targetPosition);
	}

	private List<NFoolCombatSymbol> GetSymbols()
	{
		List<NFoolCombatSymbol> list = new List<NFoolCombatSymbol>();
		foreach (NFoolCombatSymbol item in from c in _symbolContainer.GetChildren().OfType<NFoolCombatSymbol>()
			where c.Visible
			select c)
		{
			list.Add(item);
		}
		list.Sort((NFoolCombatSymbol a, NFoolCombatSymbol b) => a.symbolType.CompareTo(b.symbolType));
		return list;
	}

	private NFoolCombatSymbol? HasMelody(CardTag symbolType)
	{
		foreach (NFoolCombatSymbol item in _symbolContainer.GetChildren().OfType<NFoolCombatSymbol>())
		{
			if (item.symbolType == symbolType)
			{
				return item;
			}
		}
		return null;
	}

	/// <summary>
	/// Get the method information for all the methods declared in this class.
	/// This method is used by Godot to register the available methods in the editor.
	/// Do not call this method.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		List<MethodInfo> list = new List<MethodInfo>(9);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName._EnterTree, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName._ExitTree, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.ConnectArrowsAndSymbolsChangedSignal, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.RefreshVisibility, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.UpdateArrows, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.SetMelodyAmounts, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.Place, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal | MethodFlags.Static, new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.Object, "symbol", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Control"), exported: false),
			new PropertyInfo(Variant.Type.Object, "slot", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Node2D"), exported: false)
		}, null));
		list.Add(new MethodInfo(MethodName.HasMelody, new PropertyInfo(Variant.Type.Object, "", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Control"), exported: false), MethodFlags.Normal, new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.Int, "symbolType", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
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
		if (method == MethodName.ConnectArrowsAndSymbolsChangedSignal && args.Count == 0)
		{
			ConnectArrowsAndSymbolsChangedSignal();
			ret = default;
			return true;
		}
		if (method == MethodName.RefreshVisibility && args.Count == 0)
		{
			RefreshVisibility();
			ret = default;
			return true;
		}
		if (method == MethodName.UpdateArrows && args.Count == 0)
		{
			UpdateArrows();
			ret = default;
			return true;
		}
		if (method == MethodName.SetMelodyAmounts && args.Count == 0)
		{
			SetMelodyAmounts();
			ret = default;
			return true;
		}
		if (method == MethodName.Place && args.Count == 2)
		{
			Place(VariantUtils.ConvertTo<NFoolCombatSymbol>(in args[0]), VariantUtils.ConvertTo<Node2D>(in args[1]));
			ret = default;
			return true;
		}
		if (method == MethodName.HasMelody && args.Count == 1)
		{
			ret = VariantUtils.CreateFrom<NFoolCombatSymbol>(HasMelody(VariantUtils.ConvertTo<CardTag>(in args[0])));
			return true;
		}
		return base.InvokeGodotClassMethod(in method, args, out ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static bool InvokeGodotClassStaticMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		if (method == MethodName.Place && args.Count == 2)
		{
			Place(VariantUtils.ConvertTo<NFoolCombatSymbol>(in args[0]), VariantUtils.ConvertTo<Node2D>(in args[1]));
			ret = default;
			return true;
		}
		ret = default;
		return false;
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
		if (method == MethodName.ConnectArrowsAndSymbolsChangedSignal)
		{
			return true;
		}
		if (method == MethodName.RefreshVisibility)
		{
			return true;
		}
		if (method == MethodName.UpdateArrows)
		{
			return true;
		}
		if (method == MethodName.SetMelodyAmounts)
		{
			return true;
		}
		if (method == MethodName.Place)
		{
			return true;
		}
		if (method == MethodName.HasMelody)
		{
			return true;
		}
		return base.HasGodotClassMethod(in method);
	}

	/// <inheritdoc />
	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if (name == PropertyName._symbolContainer)
		{
			_symbolContainer = VariantUtils.ConvertTo<Control>(in value);
			return true;
		}
		if (name == PropertyName._symbolPos0)
		{
			_symbolPos0 = VariantUtils.ConvertTo<Node2D>(in value);
			return true;
		}
		if (name == PropertyName._symbolPos1)
		{
			_symbolPos1 = VariantUtils.ConvertTo<Node2D>(in value);
			return true;
		}
		if (name == PropertyName._symbolPos2)
		{
			_symbolPos2 = VariantUtils.ConvertTo<Node2D>(in value);
			return true;
		}
		if (name == PropertyName._symbolPos3)
		{
			_symbolPos3 = VariantUtils.ConvertTo<Node2D>(in value);
			return true;
		}
		if (name == PropertyName._symbolPos4)
		{
			_symbolPos4 = VariantUtils.ConvertTo<Node2D>(in value);
			return true;
		}
		if (name == PropertyName._arrowContainer)
		{
			_arrowContainer = VariantUtils.ConvertTo<Control>(in value);
			return true;
		}
		if (name == PropertyName._isListeningToCombatState)
		{
			_isListeningToCombatState = VariantUtils.ConvertTo<bool>(in value);
			return true;
		}
		return base.SetGodotClassPropertyValue(in name, in value);
	}

	/// <inheritdoc />
	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool GetGodotClassPropertyValue(in godot_string_name name, out godot_variant value)
	{
		if (name == PropertyName._symbolContainer)
		{
			value = VariantUtils.CreateFrom(in _symbolContainer);
			return true;
		}
		if (name == PropertyName._symbolPos0)
		{
			value = VariantUtils.CreateFrom(in _symbolPos0);
			return true;
		}
		if (name == PropertyName._symbolPos1)
		{
			value = VariantUtils.CreateFrom(in _symbolPos1);
			return true;
		}
		if (name == PropertyName._symbolPos2)
		{
			value = VariantUtils.CreateFrom(in _symbolPos2);
			return true;
		}
		if (name == PropertyName._symbolPos3)
		{
			value = VariantUtils.CreateFrom(in _symbolPos3);
			return true;
		}
		if (name == PropertyName._symbolPos4)
		{
			value = VariantUtils.CreateFrom(in _symbolPos4);
			return true;
		}
		if (name == PropertyName._arrowContainer)
		{
			value = VariantUtils.CreateFrom(in _arrowContainer);
			return true;
		}
		if (name == PropertyName._isListeningToCombatState)
		{
			value = VariantUtils.CreateFrom(in _isListeningToCombatState);
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
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._symbolContainer, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._symbolPos0, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._symbolPos1, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._symbolPos2, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._symbolPos3, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._symbolPos4, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._arrowContainer, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Bool, PropertyName._isListeningToCombatState, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		return list;
	}

	/// <inheritdoc />
	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		base.SaveGodotObjectData(info);
		info.AddProperty(PropertyName._symbolContainer, Variant.From(in _symbolContainer));
		info.AddProperty(PropertyName._symbolPos0, Variant.From(in _symbolPos0));
		info.AddProperty(PropertyName._symbolPos1, Variant.From(in _symbolPos1));
		info.AddProperty(PropertyName._symbolPos2, Variant.From(in _symbolPos2));
		info.AddProperty(PropertyName._symbolPos3, Variant.From(in _symbolPos3));
		info.AddProperty(PropertyName._symbolPos4, Variant.From(in _symbolPos4));
		info.AddProperty(PropertyName._arrowContainer, Variant.From(in _arrowContainer));
		info.AddProperty(PropertyName._isListeningToCombatState, Variant.From(in _isListeningToCombatState));
	}

	/// <inheritdoc />
	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		if (info.TryGetProperty(PropertyName._symbolContainer, out var value))
		{
			_symbolContainer = value.As<Control>();
		}
		if (info.TryGetProperty(PropertyName._symbolPos0, out var value2))
		{
			_symbolPos0 = value2.As<Node2D>();
		}
		if (info.TryGetProperty(PropertyName._symbolPos1, out var value3))
		{
			_symbolPos1 = value3.As<Node2D>();
		}
		if (info.TryGetProperty(PropertyName._symbolPos2, out var value4))
		{
			_symbolPos2 = value4.As<Node2D>();
		}
		if (info.TryGetProperty(PropertyName._symbolPos3, out var value5))
		{
			_symbolPos3 = value5.As<Node2D>();
		}
		if (info.TryGetProperty(PropertyName._symbolPos4, out var value6))
		{
			_symbolPos4 = value6.As<Node2D>();
		}
		if (info.TryGetProperty(PropertyName._arrowContainer, out var value7))
		{
			_arrowContainer = value7.As<Control>();
		}
		if (info.TryGetProperty(PropertyName._isListeningToCombatState, out var value8))
		{
			_isListeningToCombatState = value8.As<bool>();
		}
	}
}
