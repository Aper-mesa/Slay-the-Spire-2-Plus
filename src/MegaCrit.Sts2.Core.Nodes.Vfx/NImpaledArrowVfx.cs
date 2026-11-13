using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.FoolArrows;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Random;
using MegaCrit.Sts2.Core.TestSupport;

namespace MegaCrit.Sts2.Core.Nodes.Vfx;

[ScriptPath("res://src/Core/Nodes/Vfx/NImpaledArrowVfx.cs")]
public class NImpaledArrowVfx : Node2D
{
	/// <summary>
	/// Cached StringNames for the methods contained in this class, for fast lookup.
	/// </summary>
	public new class MethodName : Node2D.MethodName
	{
		/// <summary>
		/// Cached name for the '_Ready' method.
		/// </summary>
		public new static readonly StringName _Ready = "_Ready";

		/// <summary>
		/// Cached name for the 'ImpaleArrowIntoSpineSlots' method.
		/// </summary>
		public static readonly StringName ImpaleArrowIntoSpineSlots = "ImpaleArrowIntoSpineSlots";

		/// <summary>
		/// Cached name for the 'ImpaleArrowIntoCreature' method.
		/// </summary>
		public static readonly StringName ImpaleArrowIntoCreature = "ImpaleArrowIntoCreature";

		/// <summary>
		/// Cached name for the '_EnterTree' method.
		/// </summary>
		public new static readonly StringName _EnterTree = "_EnterTree";

		/// <summary>
		/// Cached name for the '_ExitTree' method.
		/// </summary>
		public new static readonly StringName _ExitTree = "_ExitTree";

		/// <summary>
		/// Cached name for the 'OnArrowStatusUpdated' method.
		/// </summary>
		public static readonly StringName OnArrowStatusUpdated = "OnArrowStatusUpdated";

		/// <summary>
		/// Cached name for the 'RetrieveAndFree' method.
		/// </summary>
		public static readonly StringName RetrieveAndFree = "RetrieveAndFree";

		/// <summary>
		/// Cached name for the 'OnRetrieveFinished' method.
		/// </summary>
		public static readonly StringName OnRetrieveFinished = "OnRetrieveFinished";
	}

	/// <summary>
	/// Cached StringNames for the properties and fields contained in this class, for fast lookup.
	/// </summary>
	public new class PropertyName : Node2D.PropertyName
	{
		/// <summary>
		/// Cached name for the '_arrowRotationOffsets' field.
		/// </summary>
		public static readonly StringName _arrowRotationOffsets = "_arrowRotationOffsets";

		/// <summary>
		/// Cached name for the '_arrowPositionOffsets' field.
		/// </summary>
		public static readonly StringName _arrowPositionOffsets = "_arrowPositionOffsets";

		/// <summary>
		/// Cached name for the '_frontArrowNode' field.
		/// </summary>
		public static readonly StringName _frontArrowNode = "_frontArrowNode";

		/// <summary>
		/// Cached name for the '_backArrowNode' field.
		/// </summary>
		public static readonly StringName _backArrowNode = "_backArrowNode";

		/// <summary>
		/// Cached name for the '_frontArrowVisual' field.
		/// </summary>
		public static readonly StringName _frontArrowVisual = "_frontArrowVisual";

		/// <summary>
		/// Cached name for the '_backArrowVisual' field.
		/// </summary>
		public static readonly StringName _backArrowVisual = "_backArrowVisual";

		/// <summary>
		/// Cached name for the '_frontSlotNode' field.
		/// </summary>
		public static readonly StringName _frontSlotNode = "_frontSlotNode";

		/// <summary>
		/// Cached name for the '_backSlotNode' field.
		/// </summary>
		public static readonly StringName _backSlotNode = "_backSlotNode";

		/// <summary>
		/// Cached name for the '_retrieveTween' field.
		/// </summary>
		public static readonly StringName _retrieveTween = "_retrieveTween";
	}

	/// <summary>
	/// Cached StringNames for the signals contained in this class, for fast lookup.
	/// </summary>
	public new class SignalName : Node2D.SignalName
	{
	}

	public const string frontArrowSlot = "ARROW_HIT_FRONT";

	public const string backArrowSlot = "ARROW_HIT_BACK";

	private static readonly string _scenePath = SceneHelper.GetScenePath("vfx/impaled_arrow_vfx");

	private float[] _arrowRotationOffsets = new float[3] { 0f, -20f, 20f };

	private Vector2[] _arrowPositionOffsets = new Vector2[3]
	{
		new Vector2(0f, 0f),
		new Vector2(15f, 5f),
		new Vector2(-15f, 5f)
	};

	private static readonly StringName _fadingName = "ImpaledArrowVfxFading";

	private Node2D _frontArrowNode;

	private Node2D _backArrowNode;

	private Node2D _frontArrowVisual;

	private Node2D _backArrowVisual;

	private Node2D? _frontSlotNode;

	private Node2D? _backSlotNode;

	private ArrowSlot? _arrowSlot;

	private Creature _creature;

	private Tween? _retrieveTween;

	public static IEnumerable<string> AssetPaths => new _003C_003Ez__ReadOnlySingleElementList<string>(_scenePath);

	public static NImpaledArrowVfx? Create(Creature creature, ArrowSlot arrowSlot)
	{
		if (TestMode.IsOn)
		{
			return null;
		}
		NImpaledArrowVfx nImpaledArrowVfx = PreloadManager.Cache.GetScene(_scenePath).Instantiate<NImpaledArrowVfx>(PackedScene.GenEditState.Disabled);
		nImpaledArrowVfx._creature = creature;
		nImpaledArrowVfx._arrowSlot = arrowSlot;
		return nImpaledArrowVfx;
	}

	public override void _Ready()
	{
		NCreature nCreature = NCombatRoom.Instance?.GetCreatureNode(_creature);
		_frontArrowNode = GetNode<Node2D>("FrontArrow");
		_backArrowNode = GetNode<Node2D>("BackArrow");
		_frontArrowVisual = _frontArrowNode.GetNode<Node2D>("ArrowFg");
		_backArrowVisual = _backArrowNode.GetNode<Node2D>("ArrowBg");
		if (nCreature != null)
		{
			if (nCreature.HasSpineArrowSlots)
			{
				ImpaleArrowIntoSpineSlots(nCreature);
			}
			else
			{
				ImpaleArrowIntoCreature(nCreature.Visuals);
			}
		}
	}

	private void ImpaleArrowIntoSpineSlots(NCreature creature)
	{
		_frontSlotNode = MegaSlotNode.Create(creature.Visuals.Body, "ARROW_HIT_FRONT");
		_backSlotNode = MegaSlotNode.Create(creature.Visuals.Body, "ARROW_HIT_BACK");
		_backSlotNode.ShowBehindParent = true;
		_frontArrowVisual.Reparent(_frontSlotNode, keepGlobalTransform: false);
		_backArrowVisual.Reparent(_backSlotNode, keepGlobalTransform: false);
		float num = Rng.Chaotic.NextFloat(0.9f, 1.1f);
		_frontArrowVisual.Scale *= 1f / creature.Visuals.Body.Scale.X * num;
		_backArrowVisual.Scale *= 1f / creature.Visuals.Body.Scale.X * num;
		ImpaledPower power = creature.Entity.GetPower<ImpaledPower>();
		if (power != null)
		{
			int count = power.Arrows.Count;
			float num2 = _arrowRotationOffsets[count % _arrowRotationOffsets.Length];
			Vector2 vector = _arrowPositionOffsets[count % _arrowRotationOffsets.Length];
			_frontArrowVisual.RotationDegrees += num2;
			_backArrowVisual.RotationDegrees += num2;
			_frontArrowVisual.Position += vector;
			_backArrowVisual.Position += vector;
		}
	}

	private void ImpaleArrowIntoCreature(NCreatureVisuals visuals)
	{
		Marker2D nodeOrNull = visuals.GetNodeOrNull<Marker2D>("ArrowPosition");
		if (nodeOrNull != null)
		{
			Position = nodeOrNull.Position;
		}
		else
		{
			Position = visuals.VfxSpawnPosition.Position;
		}
		RotationDegrees += Rng.Chaotic.NextFloat(-15f, 15f);
		_backArrowNode.Reparent(visuals);
	}

	public override void _EnterTree()
	{
		if (_arrowSlot != null)
		{
			_arrowSlot.ArrowStatusUpdated += OnArrowStatusUpdated;
		}
	}

	public override void _ExitTree()
	{
		if (_retrieveTween != null)
		{
			_retrieveTween.Finished -= OnRetrieveFinished;
			_retrieveTween.Kill();
		}
		if (_arrowSlot != null)
		{
			_arrowSlot.ArrowStatusUpdated -= OnArrowStatusUpdated;
		}
		_backArrowNode.QueueFreeSafely();
		_frontSlotNode?.QueueFreeSafely();
		_backSlotNode?.QueueFreeSafely();
	}

	private void OnArrowStatusUpdated()
	{
		if (_arrowSlot.Status == ArrowStatus.Destroyed)
		{
			this.QueueFreeSafely();
		}
		else
		{
			RetrieveAndFree();
		}
	}

	/// <summary>
	/// Animates the arrow back to its owner, for when it has been pulled back out of the creature.
	/// </summary>
	private void RetrieveAndFree()
	{
		if (_retrieveTween == null)
		{
			SfxCmd.Play("event:/sfx/characters/fool/fool_dislodge");
			if (_frontSlotNode != null)
			{
				GlobalPosition = _frontSlotNode.GlobalPosition;
			}
			_frontArrowVisual.Reparent(_frontArrowNode);
			_backArrowVisual.Reparent(_backArrowNode);
			_backArrowNode.Reparent(this);
			NCreature nCreature = NCombatRoom.Instance?.GetCreatureNode(_arrowSlot.owner.Creature);
			Name = _fadingName;
			if (nCreature == null)
			{
				_retrieveTween = CreateTween();
				_retrieveTween.TweenProperty(this, "modulate:a", 0f, 0.20000000298023224);
			}
			else
			{
				_retrieveTween = CreateTween();
				_retrieveTween.TweenProperty(this, "global_position:x", nCreature.VfxSpawnPosition.X, 0.5);
				_retrieveTween.Parallel().TweenProperty(this, "rotation_degrees", -540f, 0.5);
				_retrieveTween.Parallel().TweenProperty(this, "global_position:y", nCreature.VfxSpawnPosition.Y - 500f, 0.25).SetEase(Tween.EaseType.Out)
					.SetTrans(Tween.TransitionType.Sine);
				_retrieveTween.Parallel().TweenProperty(this, "global_position:y", nCreature.VfxSpawnPosition.Y, 0.17499999701976776).SetDelay(0.25)
					.SetEase(Tween.EaseType.In)
					.SetTrans(Tween.TransitionType.Sine);
				_retrieveTween.Parallel().TweenProperty(this, "modulate:a", 0f, 0.10000000149011612).SetDelay(0.4000000059604645);
			}
			_retrieveTween.Finished += OnRetrieveFinished;
		}
	}

	private void OnRetrieveFinished()
	{
		this.QueueFreeSafely();
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
		list.Add(new MethodInfo(MethodName.ImpaleArrowIntoSpineSlots, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.Object, "creature", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Control"), exported: false)
		}, null));
		list.Add(new MethodInfo(MethodName.ImpaleArrowIntoCreature, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.Object, "visuals", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Node2D"), exported: false)
		}, null));
		list.Add(new MethodInfo(MethodName._EnterTree, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName._ExitTree, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.OnArrowStatusUpdated, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.RetrieveAndFree, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.OnRetrieveFinished, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
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
		if (method == MethodName.ImpaleArrowIntoSpineSlots && args.Count == 1)
		{
			ImpaleArrowIntoSpineSlots(VariantUtils.ConvertTo<NCreature>(in args[0]));
			ret = default;
			return true;
		}
		if (method == MethodName.ImpaleArrowIntoCreature && args.Count == 1)
		{
			ImpaleArrowIntoCreature(VariantUtils.ConvertTo<NCreatureVisuals>(in args[0]));
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
		if (method == MethodName.OnArrowStatusUpdated && args.Count == 0)
		{
			OnArrowStatusUpdated();
			ret = default;
			return true;
		}
		if (method == MethodName.RetrieveAndFree && args.Count == 0)
		{
			RetrieveAndFree();
			ret = default;
			return true;
		}
		if (method == MethodName.OnRetrieveFinished && args.Count == 0)
		{
			OnRetrieveFinished();
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
		if (method == MethodName.ImpaleArrowIntoSpineSlots)
		{
			return true;
		}
		if (method == MethodName.ImpaleArrowIntoCreature)
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
		if (method == MethodName.OnArrowStatusUpdated)
		{
			return true;
		}
		if (method == MethodName.RetrieveAndFree)
		{
			return true;
		}
		if (method == MethodName.OnRetrieveFinished)
		{
			return true;
		}
		return base.HasGodotClassMethod(in method);
	}

	/// <inheritdoc />
	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if (name == PropertyName._arrowRotationOffsets)
		{
			_arrowRotationOffsets = VariantUtils.ConvertTo<float[]>(in value);
			return true;
		}
		if (name == PropertyName._arrowPositionOffsets)
		{
			_arrowPositionOffsets = VariantUtils.ConvertTo<Vector2[]>(in value);
			return true;
		}
		if (name == PropertyName._frontArrowNode)
		{
			_frontArrowNode = VariantUtils.ConvertTo<Node2D>(in value);
			return true;
		}
		if (name == PropertyName._backArrowNode)
		{
			_backArrowNode = VariantUtils.ConvertTo<Node2D>(in value);
			return true;
		}
		if (name == PropertyName._frontArrowVisual)
		{
			_frontArrowVisual = VariantUtils.ConvertTo<Node2D>(in value);
			return true;
		}
		if (name == PropertyName._backArrowVisual)
		{
			_backArrowVisual = VariantUtils.ConvertTo<Node2D>(in value);
			return true;
		}
		if (name == PropertyName._frontSlotNode)
		{
			_frontSlotNode = VariantUtils.ConvertTo<Node2D>(in value);
			return true;
		}
		if (name == PropertyName._backSlotNode)
		{
			_backSlotNode = VariantUtils.ConvertTo<Node2D>(in value);
			return true;
		}
		if (name == PropertyName._retrieveTween)
		{
			_retrieveTween = VariantUtils.ConvertTo<Tween>(in value);
			return true;
		}
		return base.SetGodotClassPropertyValue(in name, in value);
	}

	/// <inheritdoc />
	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool GetGodotClassPropertyValue(in godot_string_name name, out godot_variant value)
	{
		if (name == PropertyName._arrowRotationOffsets)
		{
			value = VariantUtils.CreateFrom(in _arrowRotationOffsets);
			return true;
		}
		if (name == PropertyName._arrowPositionOffsets)
		{
			value = VariantUtils.CreateFrom(in _arrowPositionOffsets);
			return true;
		}
		if (name == PropertyName._frontArrowNode)
		{
			value = VariantUtils.CreateFrom(in _frontArrowNode);
			return true;
		}
		if (name == PropertyName._backArrowNode)
		{
			value = VariantUtils.CreateFrom(in _backArrowNode);
			return true;
		}
		if (name == PropertyName._frontArrowVisual)
		{
			value = VariantUtils.CreateFrom(in _frontArrowVisual);
			return true;
		}
		if (name == PropertyName._backArrowVisual)
		{
			value = VariantUtils.CreateFrom(in _backArrowVisual);
			return true;
		}
		if (name == PropertyName._frontSlotNode)
		{
			value = VariantUtils.CreateFrom(in _frontSlotNode);
			return true;
		}
		if (name == PropertyName._backSlotNode)
		{
			value = VariantUtils.CreateFrom(in _backSlotNode);
			return true;
		}
		if (name == PropertyName._retrieveTween)
		{
			value = VariantUtils.CreateFrom(in _retrieveTween);
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
		list.Add(new PropertyInfo(Variant.Type.PackedFloat32Array, PropertyName._arrowRotationOffsets, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.PackedVector2Array, PropertyName._arrowPositionOffsets, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._frontArrowNode, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._backArrowNode, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._frontArrowVisual, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._backArrowVisual, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._frontSlotNode, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._backSlotNode, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._retrieveTween, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		return list;
	}

	/// <inheritdoc />
	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		base.SaveGodotObjectData(info);
		info.AddProperty(PropertyName._arrowRotationOffsets, Variant.From(in _arrowRotationOffsets));
		info.AddProperty(PropertyName._arrowPositionOffsets, Variant.From(in _arrowPositionOffsets));
		info.AddProperty(PropertyName._frontArrowNode, Variant.From(in _frontArrowNode));
		info.AddProperty(PropertyName._backArrowNode, Variant.From(in _backArrowNode));
		info.AddProperty(PropertyName._frontArrowVisual, Variant.From(in _frontArrowVisual));
		info.AddProperty(PropertyName._backArrowVisual, Variant.From(in _backArrowVisual));
		info.AddProperty(PropertyName._frontSlotNode, Variant.From(in _frontSlotNode));
		info.AddProperty(PropertyName._backSlotNode, Variant.From(in _backSlotNode));
		info.AddProperty(PropertyName._retrieveTween, Variant.From(in _retrieveTween));
	}

	/// <inheritdoc />
	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		if (info.TryGetProperty(PropertyName._arrowRotationOffsets, out var value))
		{
			_arrowRotationOffsets = value.As<float[]>();
		}
		if (info.TryGetProperty(PropertyName._arrowPositionOffsets, out var value2))
		{
			_arrowPositionOffsets = value2.As<Vector2[]>();
		}
		if (info.TryGetProperty(PropertyName._frontArrowNode, out var value3))
		{
			_frontArrowNode = value3.As<Node2D>();
		}
		if (info.TryGetProperty(PropertyName._backArrowNode, out var value4))
		{
			_backArrowNode = value4.As<Node2D>();
		}
		if (info.TryGetProperty(PropertyName._frontArrowVisual, out var value5))
		{
			_frontArrowVisual = value5.As<Node2D>();
		}
		if (info.TryGetProperty(PropertyName._backArrowVisual, out var value6))
		{
			_backArrowVisual = value6.As<Node2D>();
		}
		if (info.TryGetProperty(PropertyName._frontSlotNode, out var value7))
		{
			_frontSlotNode = value7.As<Node2D>();
		}
		if (info.TryGetProperty(PropertyName._backSlotNode, out var value8))
		{
			_backSlotNode = value8.As<Node2D>();
		}
		if (info.TryGetProperty(PropertyName._retrieveTween, out var value9))
		{
			_retrieveTween = value9.As<Tween>();
		}
	}
}
