using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;

namespace MegaCrit.Sts2.Core.Nodes.Vfx;

[GlobalClass]
[ScriptPath("res://src/Core/Nodes/Vfx/NFoolVfx.cs")]
public class NFoolVfx : Node2D
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
		/// Cached name for the 'OnAnimationEvent' method.
		/// </summary>
		public static readonly StringName OnAnimationEvent = "OnAnimationEvent";

		/// <summary>
		/// Cached name for the 'OnAnimationStart' method.
		/// </summary>
		public static readonly StringName OnAnimationStart = "OnAnimationStart";

		/// <summary>
		/// Cached name for the 'ResetVfx' method.
		/// </summary>
		public static readonly StringName ResetVfx = "ResetVfx";

		/// <summary>
		/// Cached name for the 'StartLeafCannon' method.
		/// </summary>
		public static readonly StringName StartLeafCannon = "StartLeafCannon";

		/// <summary>
		/// Cached name for the 'EndLeafCannon' method.
		/// </summary>
		public static readonly StringName EndLeafCannon = "EndLeafCannon";

		/// <summary>
		/// Cached name for the 'StartLeafWaterfall' method.
		/// </summary>
		public static readonly StringName StartLeafWaterfall = "StartLeafWaterfall";

		/// <summary>
		/// Cached name for the 'EndLeafWaterfall' method.
		/// </summary>
		public static readonly StringName EndLeafWaterfall = "EndLeafWaterfall";

		/// <summary>
		/// Cached name for the 'ShootMagicLeaves' method.
		/// </summary>
		public static readonly StringName ShootMagicLeaves = "ShootMagicLeaves";

		/// <summary>
		/// Cached name for the 'ShootMusic' method.
		/// </summary>
		public static readonly StringName ShootMusic = "ShootMusic";

		/// <summary>
		/// Cached name for the 'ShootStars' method.
		/// </summary>
		public static readonly StringName ShootStars = "ShootStars";
	}

	/// <summary>
	/// Cached StringNames for the properties and fields contained in this class, for fast lookup.
	/// </summary>
	public new class PropertyName : Node2D.PropertyName
	{
		/// <summary>
		/// Cached name for the '_leafCannonParticles' field.
		/// </summary>
		public static readonly StringName _leafCannonParticles = "_leafCannonParticles";

		/// <summary>
		/// Cached name for the '_leafWaterfallParticles' field.
		/// </summary>
		public static readonly StringName _leafWaterfallParticles = "_leafWaterfallParticles";

		/// <summary>
		/// Cached name for the '_magicLeafParticles' field.
		/// </summary>
		public static readonly StringName _magicLeafParticles = "_magicLeafParticles";

		/// <summary>
		/// Cached name for the '_musicParticles' field.
		/// </summary>
		public static readonly StringName _musicParticles = "_musicParticles";

		/// <summary>
		/// Cached name for the '_starParticles' field.
		/// </summary>
		public static readonly StringName _starParticles = "_starParticles";

		/// <summary>
		/// Cached name for the '_impactParticle' field.
		/// </summary>
		public static readonly StringName _impactParticle = "_impactParticle";

		/// <summary>
		/// Cached name for the '_parent' field.
		/// </summary>
		public static readonly StringName _parent = "_parent";
	}

	/// <summary>
	/// Cached StringNames for the signals contained in this class, for fast lookup.
	/// </summary>
	public new class SignalName : Node2D.SignalName
	{
	}

	private GpuParticles2D _leafCannonParticles;

	private GpuParticles2D _leafWaterfallParticles;

	private GpuParticles2D _magicLeafParticles;

	private GpuParticles2D _musicParticles;

	private GpuParticles2D _starParticles;

	private GpuParticles2D _impactParticle;

	private Node2D _parent;

	private MegaSprite _animController;

	public override void _Ready()
	{
		_parent = GetParent<Node2D>();
		_animController = new MegaSprite(_parent);
		_animController.ConnectAnimationEvent(Callable.From<GodotObject, GodotObject, GodotObject, GodotObject>(OnAnimationEvent));
		_animController.ConnectAnimationStarted(Callable.From<GodotObject, GodotObject, GodotObject>(OnAnimationStart));
		_leafCannonParticles = _parent.GetNode<GpuParticles2D>("ParticleCannonSlot/LeafCannonParticles");
		_leafWaterfallParticles = _parent.GetNode<GpuParticles2D>("LeafWaterfallSlot/LeafWaterfallParticles");
		_musicParticles = _parent.GetNode<GpuParticles2D>("ParticleCannonSlot/MusicParticles");
		_magicLeafParticles = _parent.GetNode<GpuParticles2D>("ParticleCannonSlot/MagicLeafParticles");
		_starParticles = _parent.GetNode<GpuParticles2D>("ParticleCannonSlot/StarParticles");
		_impactParticle = _parent.GetNode<GpuParticles2D>("ParticleCannonSlot/ImpactParticle");
		_magicLeafParticles.OneShot = true;
		_musicParticles.OneShot = true;
		_starParticles.OneShot = true;
		_impactParticle.OneShot = true;
		ResetVfx();
	}

	private void OnAnimationEvent(GodotObject _, GodotObject __, GodotObject ___, GodotObject spineEvent)
	{
		string eventName = new MegaEvent(spineEvent).GetData().GetEventName();
		if (eventName == null)
		{
			return;
		}
		switch (eventName.Length)
		{
		case 17:
			if (eventName == "leaf_cannon_start")
			{
				StartLeafCannon();
			}
			break;
		case 15:
			if (eventName == "leaf_cannon_end")
			{
				EndLeafCannon();
			}
			break;
		case 20:
			if (eventName == "leaf_waterfall_start")
			{
				StartLeafWaterfall();
			}
			break;
		case 18:
			if (eventName == "leaf_waterfall_end")
			{
				EndLeafWaterfall();
			}
			break;
		case 16:
			if (eventName == "magic_leaf_shoot")
			{
				ShootMagicLeaves();
			}
			break;
		case 21:
			if (eventName == "music_particles_shoot")
			{
				ShootMusic();
			}
			break;
		case 11:
			if (eventName == "stars_shoot")
			{
				ShootStars();
			}
			break;
		case 12:
		case 13:
		case 14:
		case 19:
			break;
		}
	}

	private void OnAnimationStart(GodotObject spineSprite, GodotObject animationState, GodotObject trackEntry)
	{
		ResetVfx();
	}

	private void ResetVfx()
	{
		_leafCannonParticles.Restart();
		_leafCannonParticles.Emitting = false;
		_magicLeafParticles.Emitting = false;
		_musicParticles.Emitting = false;
		_leafWaterfallParticles.Restart();
		_leafWaterfallParticles.Emitting = false;
		_starParticles.Emitting = false;
		_impactParticle.Emitting = false;
	}

	private void StartLeafCannon()
	{
		_leafCannonParticles.Restart();
	}

	private void EndLeafCannon()
	{
		_leafCannonParticles.Emitting = false;
	}

	private void StartLeafWaterfall()
	{
		_leafWaterfallParticles.Restart();
	}

	private void EndLeafWaterfall()
	{
		_leafWaterfallParticles.Restart();
		_leafWaterfallParticles.Emitting = false;
	}

	private void ShootMagicLeaves()
	{
		_magicLeafParticles.Restart();
	}

	private void ShootMusic()
	{
		_musicParticles.Restart();
	}

	private void ShootStars()
	{
		_starParticles.Restart();
		_impactParticle.Restart();
	}

	/// <summary>
	/// Get the method information for all the methods declared in this class.
	/// This method is used by Godot to register the available methods in the editor.
	/// Do not call this method.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		List<MethodInfo> list = new List<MethodInfo>(11);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.OnAnimationEvent, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.Object, "_", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Object"), exported: false),
			new PropertyInfo(Variant.Type.Object, "__", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Object"), exported: false),
			new PropertyInfo(Variant.Type.Object, "___", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Object"), exported: false),
			new PropertyInfo(Variant.Type.Object, "spineEvent", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Object"), exported: false)
		}, null));
		list.Add(new MethodInfo(MethodName.OnAnimationStart, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.Object, "spineSprite", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Object"), exported: false),
			new PropertyInfo(Variant.Type.Object, "animationState", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Object"), exported: false),
			new PropertyInfo(Variant.Type.Object, "trackEntry", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Object"), exported: false)
		}, null));
		list.Add(new MethodInfo(MethodName.ResetVfx, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.StartLeafCannon, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.EndLeafCannon, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.StartLeafWaterfall, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.EndLeafWaterfall, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.ShootMagicLeaves, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.ShootMusic, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
		list.Add(new MethodInfo(MethodName.ShootStars, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
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
		if (method == MethodName.OnAnimationStart && args.Count == 3)
		{
			OnAnimationStart(VariantUtils.ConvertTo<GodotObject>(in args[0]), VariantUtils.ConvertTo<GodotObject>(in args[1]), VariantUtils.ConvertTo<GodotObject>(in args[2]));
			ret = default;
			return true;
		}
		if (method == MethodName.ResetVfx && args.Count == 0)
		{
			ResetVfx();
			ret = default;
			return true;
		}
		if (method == MethodName.StartLeafCannon && args.Count == 0)
		{
			StartLeafCannon();
			ret = default;
			return true;
		}
		if (method == MethodName.EndLeafCannon && args.Count == 0)
		{
			EndLeafCannon();
			ret = default;
			return true;
		}
		if (method == MethodName.StartLeafWaterfall && args.Count == 0)
		{
			StartLeafWaterfall();
			ret = default;
			return true;
		}
		if (method == MethodName.EndLeafWaterfall && args.Count == 0)
		{
			EndLeafWaterfall();
			ret = default;
			return true;
		}
		if (method == MethodName.ShootMagicLeaves && args.Count == 0)
		{
			ShootMagicLeaves();
			ret = default;
			return true;
		}
		if (method == MethodName.ShootMusic && args.Count == 0)
		{
			ShootMusic();
			ret = default;
			return true;
		}
		if (method == MethodName.ShootStars && args.Count == 0)
		{
			ShootStars();
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
		if (method == MethodName.OnAnimationStart)
		{
			return true;
		}
		if (method == MethodName.ResetVfx)
		{
			return true;
		}
		if (method == MethodName.StartLeafCannon)
		{
			return true;
		}
		if (method == MethodName.EndLeafCannon)
		{
			return true;
		}
		if (method == MethodName.StartLeafWaterfall)
		{
			return true;
		}
		if (method == MethodName.EndLeafWaterfall)
		{
			return true;
		}
		if (method == MethodName.ShootMagicLeaves)
		{
			return true;
		}
		if (method == MethodName.ShootMusic)
		{
			return true;
		}
		if (method == MethodName.ShootStars)
		{
			return true;
		}
		return base.HasGodotClassMethod(in method);
	}

	/// <inheritdoc />
	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if (name == PropertyName._leafCannonParticles)
		{
			_leafCannonParticles = VariantUtils.ConvertTo<GpuParticles2D>(in value);
			return true;
		}
		if (name == PropertyName._leafWaterfallParticles)
		{
			_leafWaterfallParticles = VariantUtils.ConvertTo<GpuParticles2D>(in value);
			return true;
		}
		if (name == PropertyName._magicLeafParticles)
		{
			_magicLeafParticles = VariantUtils.ConvertTo<GpuParticles2D>(in value);
			return true;
		}
		if (name == PropertyName._musicParticles)
		{
			_musicParticles = VariantUtils.ConvertTo<GpuParticles2D>(in value);
			return true;
		}
		if (name == PropertyName._starParticles)
		{
			_starParticles = VariantUtils.ConvertTo<GpuParticles2D>(in value);
			return true;
		}
		if (name == PropertyName._impactParticle)
		{
			_impactParticle = VariantUtils.ConvertTo<GpuParticles2D>(in value);
			return true;
		}
		if (name == PropertyName._parent)
		{
			_parent = VariantUtils.ConvertTo<Node2D>(in value);
			return true;
		}
		return base.SetGodotClassPropertyValue(in name, in value);
	}

	/// <inheritdoc />
	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool GetGodotClassPropertyValue(in godot_string_name name, out godot_variant value)
	{
		if (name == PropertyName._leafCannonParticles)
		{
			value = VariantUtils.CreateFrom(in _leafCannonParticles);
			return true;
		}
		if (name == PropertyName._leafWaterfallParticles)
		{
			value = VariantUtils.CreateFrom(in _leafWaterfallParticles);
			return true;
		}
		if (name == PropertyName._magicLeafParticles)
		{
			value = VariantUtils.CreateFrom(in _magicLeafParticles);
			return true;
		}
		if (name == PropertyName._musicParticles)
		{
			value = VariantUtils.CreateFrom(in _musicParticles);
			return true;
		}
		if (name == PropertyName._starParticles)
		{
			value = VariantUtils.CreateFrom(in _starParticles);
			return true;
		}
		if (name == PropertyName._impactParticle)
		{
			value = VariantUtils.CreateFrom(in _impactParticle);
			return true;
		}
		if (name == PropertyName._parent)
		{
			value = VariantUtils.CreateFrom(in _parent);
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
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._leafCannonParticles, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._leafWaterfallParticles, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._magicLeafParticles, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._musicParticles, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._starParticles, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._impactParticle, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		list.Add(new PropertyInfo(Variant.Type.Object, PropertyName._parent, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		return list;
	}

	/// <inheritdoc />
	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		base.SaveGodotObjectData(info);
		info.AddProperty(PropertyName._leafCannonParticles, Variant.From(in _leafCannonParticles));
		info.AddProperty(PropertyName._leafWaterfallParticles, Variant.From(in _leafWaterfallParticles));
		info.AddProperty(PropertyName._magicLeafParticles, Variant.From(in _magicLeafParticles));
		info.AddProperty(PropertyName._musicParticles, Variant.From(in _musicParticles));
		info.AddProperty(PropertyName._starParticles, Variant.From(in _starParticles));
		info.AddProperty(PropertyName._impactParticle, Variant.From(in _impactParticle));
		info.AddProperty(PropertyName._parent, Variant.From(in _parent));
	}

	/// <inheritdoc />
	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		if (info.TryGetProperty(PropertyName._leafCannonParticles, out var value))
		{
			_leafCannonParticles = value.As<GpuParticles2D>();
		}
		if (info.TryGetProperty(PropertyName._leafWaterfallParticles, out var value2))
		{
			_leafWaterfallParticles = value2.As<GpuParticles2D>();
		}
		if (info.TryGetProperty(PropertyName._magicLeafParticles, out var value3))
		{
			_magicLeafParticles = value3.As<GpuParticles2D>();
		}
		if (info.TryGetProperty(PropertyName._musicParticles, out var value4))
		{
			_musicParticles = value4.As<GpuParticles2D>();
		}
		if (info.TryGetProperty(PropertyName._starParticles, out var value5))
		{
			_starParticles = value5.As<GpuParticles2D>();
		}
		if (info.TryGetProperty(PropertyName._impactParticle, out var value6))
		{
			_impactParticle = value6.As<GpuParticles2D>();
		}
		if (info.TryGetProperty(PropertyName._parent, out var value7))
		{
			_parent = value7.As<Node2D>();
		}
	}
}
