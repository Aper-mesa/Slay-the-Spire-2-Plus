using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;

namespace MegaCrit.Sts2.Core.Nodes.Vfx;

[ScriptPath("res://src/Core/Nodes/Vfx/NInfestedPrismVfx.cs")]
public class NInfestedPrismVfx : Node
{
	/// <summary>
	/// Cached StringNames for the methods contained in this class, for fast lookup.
	/// </summary>
	public new class MethodName : Node.MethodName
	{
		/// <summary>
		/// Cached name for the '_Ready' method.
		/// </summary>
		public new static readonly StringName _Ready = "_Ready";

		/// <summary>
		/// Cached name for the 'OnAnimationEvent' method.
		/// </summary>
		public static readonly StringName OnAnimationEvent = "OnAnimationEvent";

		/// <summary>
		/// Cached name for the 'DissolveSoul' method.
		/// </summary>
		public static readonly StringName DissolveSoul = "DissolveSoul";

		/// <summary>
		/// Cached name for the 'Explode' method.
		/// </summary>
		public static readonly StringName Explode = "Explode";

		/// <summary>
		/// Cached name for the 'OnAnimationStart' method.
		/// </summary>
		public static readonly StringName OnAnimationStart = "OnAnimationStart";

		/// <summary>
		/// Cached name for the '_ExitTree' method.
		/// </summary>
		public new static readonly StringName _ExitTree = "_ExitTree";
	}

	/// <summary>
	/// Cached StringNames for the properties and fields contained in this class, for fast lookup.
	/// </summary>
	public new class PropertyName : Node.PropertyName
	{
		/// <summary>
		/// Cached name for the '_parent' field.
		/// </summary>
		public static readonly StringName _parent = "_parent";

		/// <summary>
		/// Cached name for the '_innerTween' field.
		/// </summary>
		public static readonly StringName _innerTween = "_innerTween";

		/// <summary>
		/// Cached name for the '_soulMaterial' field.
		/// </summary>
		public static readonly StringName _soulMaterial = "_soulMaterial";

		/// <summary>
		/// Cached name for the '_dissolveTargStep' field.
		/// </summary>
		public static readonly StringName _dissolveTargStep = "_dissolveTargStep";

		/// <summary>
		/// Cached name for the '_dissolveBaseStep' field.
		/// </summary>
		public static readonly StringName _dissolveBaseStep = "_dissolveBaseStep";

		/// <summary>
		/// Cached name for the '_deathPArticles' field.
		/// </summary>
		public static readonly StringName _deathPArticles = "_deathPArticles";

		/// <summary>
		/// Cached name for the '_deathSprinkles' field.
		/// </summary>
		public static readonly StringName _deathSprinkles = "_deathSprinkles";
	}

	/// <summary>
	/// Cached StringNames for the signals contained in this class, for fast lookup.
	/// </summary>
	public new class SignalName : Node.SignalName
	{
	}

	private static readonly StringName _outerStep = new StringName("OuterStep");

	private MegaSprite _megaSprite;

	private Node _parent;

	private Tween? _innerTween;

	private MegaSlotNode? _soulSlot;

	private ShaderMaterial? _soulMaterial;

	private Vector2 _dissolveTargStep = new Vector2(0.36f, 0.46f);

	private Vector2 _dissolveBaseStep;

	private GpuParticles2D _deathPArticles;

	private GpuParticles2D _deathSprinkles;

	public override void _Ready()
	{
		_parent = GetParent<Node2D>();
		_megaSprite = new MegaSprite(_parent);
		_megaSprite.ConnectAnimationEvent(Callable.From<GodotObject, GodotObject, GodotObject, GodotObject>(OnAnimationEvent));
		_megaSprite.ConnectAnimationStarted(Callable.From<GodotObject, GodotObject, GodotObject>(OnAnimationStart));
		_soulSlot = new MegaSlotNode(GetNode<Node2D>("../SoulSlot"));
		_soulMaterial = _soulSlot.GetNormalMaterial() as ShaderMaterial;
		_dissolveBaseStep = _soulMaterial.GetShaderParameter(_outerStep).AsVector2();
		_deathPArticles = _parent.GetNode<GpuParticles2D>("DeathParticles");
		_deathSprinkles = _parent.GetNode<GpuParticles2D>("DeathSprinkles");
		_deathPArticles.Emitting = false;
		_deathSprinkles.Emitting = false;
		_deathPArticles.OneShot = true;
		_deathSprinkles.OneShot = true;
	}

	private void OnAnimationEvent(GodotObject _, GodotObject __, GodotObject ___, GodotObject spineEvent)
	{
		string eventName = new MegaEvent(spineEvent).GetData().GetEventName();
		if (!(eventName == "dissolve_soul"))
		{
			if (eventName == "explode")
			{
				Explode();
			}
		}
		else
		{
			DissolveSoul();
		}
	}

	private void DissolveSoul()
	{
		_innerTween = CreateTween();
		_innerTween.TweenProperty(_soulMaterial, "shader_parameter/OuterStep", _dissolveTargStep, 0.75);
		_deathSprinkles.Restart();
	}

	private void Explode()
	{
		_deathPArticles.Restart();
	}

	private void OnAnimationStart(GodotObject spineSprite, GodotObject animationState, GodotObject trackEntry)
	{
		_soulMaterial.SetShaderParameter(_outerStep, _dissolveBaseStep);
	}

	public override void _ExitTree()
	{
		_innerTween?.Kill();
	}

	/// <summary>
	/// Get the method information for all the methods declared in this class.
	/// This method is used by Godot to register the available methods in the editor.
	/// Do not call this method.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		List<MethodInfo> list = new List<MethodInfo>(6);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.OnAnimationEvent, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.Object, "_", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Object"), exported: false),
			new PropertyInfo(Variant.Type.Object, "__", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Object"), exported: false),
			new PropertyInfo(Variant.Type.Object, "___", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Object"), exported: false),
			new PropertyInfo(Variant.Type.Object, "spineEvent", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Object"), exported: false)
		}, null));
		list.Add(new MethodInfo(MethodName.DissolveSoul, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.Explode, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.OnAnimationStart, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.Object, "spineSprite", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Object"), exported: false),
			new PropertyInfo(Variant.Type.Object, "animationState", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Object"), exported: false),
			new PropertyInfo(Variant.Type.Object, "trackEntry", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Object"), exported: false)
		}, null));
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
		if (method == MethodName.OnAnimationEvent && args.Count == 4)
		{
			OnAnimationEvent(VariantUtils.ConvertTo<GodotObject>(in args[0]), VariantUtils.ConvertTo<GodotObject>(in args[1]), VariantUtils.ConvertTo<GodotObject>(in args[2]), VariantUtils.ConvertTo<GodotObject>(in args[3]));
			ret = default;
			return true;
		}
		if (method == MethodName.DissolveSoul && args.Count == 0)
		{
			DissolveSoul();
			ret = default;
			return true;
		}
		if (method == MethodName.Explode && args.Count == 0)
		{
			Explode();
			ret = default;
			return true;
		}
		if (method == MethodName.OnAnimationStart && args.Count == 3)
		{
			OnAnimationStart(VariantUtils.ConvertTo<GodotObject>(in args[0]), VariantUtils.ConvertTo<GodotObject>(in args[1]), VariantUtils.ConvertTo<GodotObject>(in args[2]));
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
		if (method == MethodName.OnAnimationEvent)
		{
			return true;
		}
		if (method == MethodName.DissolveSoul)
		{
			return true;
		}
		if (method == MethodName.Explode)
		{
			return true;
		}
		if (method == MethodName.OnAnimationStart)
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
		if (name == PropertyName._parent)
		{
			_parent = VariantUtils.ConvertTo<Node>(in value);
			return true;
		}
		if (name == PropertyName._innerTween)
		{
			_innerTween = VariantUtils.ConvertTo<Tween>(in value);
			return true;
		}
		if (name == PropertyName._soulMaterial)
		{
			_soulMaterial = VariantUtils.ConvertTo<ShaderMaterial>(in value);
			return true;
		}
		if (name == PropertyName._dissolveTargStep)
		{
			_dissolveTargStep = VariantUtils.ConvertTo<Vector2>(in value);
			return true;
		}
		if (name == PropertyName._dissolveBaseStep)
		{
			_dissolveBaseStep = VariantUtils.ConvertTo<Vector2>(in value);
			return true;
		}
		if (name == PropertyName._deathPArticles)
		{
			_deathPArticles = VariantUtils.ConvertTo<GpuParticles2D>(in value);
			return true;
		}
		if (name == PropertyName._deathSprinkles)
		{
			_deathSprinkles = VariantUtils.ConvertTo<GpuParticles2D>(in value);
			return true;
		}
		return base.SetGodotClassPropertyValue(in name, in value);
	}

	/// <inheritdoc />
	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool GetGodotClassPropertyValue(in godot_string_name name, out godot_variant value)
	{
		if (name == PropertyName._parent)
		{
			value = VariantUtils.CreateFrom(in _parent);
			return true;
		}
		if (name == PropertyName._innerTween)
		{
			value = VariantUtils.CreateFrom(in _innerTween);
			return true;
		}
		if (name == PropertyName._soulMaterial)
		{
			value = VariantUtils.CreateFrom(in _soulMaterial);
			return true;
		}
		if (name == PropertyName._dissolveTargStep)
		{
			value = VariantUtils.CreateFrom(in _dissolveTargStep);
			return true;
		}
		if (name == PropertyName._dissolveBaseStep)
		{
			value = VariantUtils.CreateFrom(in _dissolveBaseStep);
			return true;
		}
		if (name == PropertyName._deathPArticles)
		{
			value = VariantUtils.CreateFrom(in _deathPArticles);
			return true;
		}
		if (name == PropertyName._deathSprinkles)
		{
			value = VariantUtils.CreateFrom(in _deathSprinkles);
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
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._parent, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._innerTween, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._soulMaterial, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Vector2, PropertyName._dissolveTargStep, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Vector2, PropertyName._dissolveBaseStep, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._deathPArticles, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._deathSprinkles, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		return list;
	}

	/// <inheritdoc />
	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		base.SaveGodotObjectData(info);
		info.AddProperty(PropertyName._parent, Variant.From(in _parent));
		info.AddProperty(PropertyName._innerTween, Variant.From(in _innerTween));
		info.AddProperty(PropertyName._soulMaterial, Variant.From(in _soulMaterial));
		info.AddProperty(PropertyName._dissolveTargStep, Variant.From(in _dissolveTargStep));
		info.AddProperty(PropertyName._dissolveBaseStep, Variant.From(in _dissolveBaseStep));
		info.AddProperty(PropertyName._deathPArticles, Variant.From(in _deathPArticles));
		info.AddProperty(PropertyName._deathSprinkles, Variant.From(in _deathSprinkles));
	}

	/// <inheritdoc />
	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		if (info.TryGetProperty(PropertyName._parent, out var value))
		{
			_parent = value.As<Node>();
		}
		if (info.TryGetProperty(PropertyName._innerTween, out var value2))
		{
			_innerTween = value2.As<Tween>();
		}
		if (info.TryGetProperty(PropertyName._soulMaterial, out var value3))
		{
			_soulMaterial = value3.As<ShaderMaterial>();
		}
		if (info.TryGetProperty(PropertyName._dissolveTargStep, out var value4))
		{
			_dissolveTargStep = value4.As<Vector2>();
		}
		if (info.TryGetProperty(PropertyName._dissolveBaseStep, out var value5))
		{
			_dissolveBaseStep = value5.As<Vector2>();
		}
		if (info.TryGetProperty(PropertyName._deathPArticles, out var value6))
		{
			_deathPArticles = value6.As<GpuParticles2D>();
		}
		if (info.TryGetProperty(PropertyName._deathSprinkles, out var value7))
		{
			_deathSprinkles = value7.As<GpuParticles2D>();
		}
	}
}
