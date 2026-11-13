using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.TestSupport;

namespace MegaCrit.Sts2.Core.Nodes.Vfx;

[ScriptPath("res://src/Core/Nodes/Vfx/NByrdpipReadyToEvolveVfx.cs")]
public class NByrdpipReadyToEvolveVfx : Node2D
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
		/// Cached name for the 'OnStarted' method.
		/// </summary>
		public static readonly StringName OnStarted = "OnStarted";

		/// <summary>
		/// Cached name for the 'OnCompleted' method.
		/// </summary>
		public static readonly StringName OnCompleted = "OnCompleted";

		/// <summary>
		/// Cached name for the 'OnInterrupted' method.
		/// </summary>
		public static readonly StringName OnInterrupted = "OnInterrupted";

		/// <summary>
		/// Cached name for the 'OnEnded' method.
		/// </summary>
		public static readonly StringName OnEnded = "OnEnded";

		/// <summary>
		/// Cached name for the 'IsAttackAnimation' method.
		/// </summary>
		public static readonly StringName IsAttackAnimation = "IsAttackAnimation";

		/// <summary>
		/// Cached name for the 'Finish' method.
		/// </summary>
		public static readonly StringName Finish = "Finish";

		/// <summary>
		/// Cached name for the '_ExitTree' method.
		/// </summary>
		public new static readonly StringName _ExitTree = "_ExitTree";
	}

	/// <summary>
	/// Cached StringNames for the properties and fields contained in this class, for fast lookup.
	/// </summary>
	public new class PropertyName : Node2D.PropertyName
	{
		/// <summary>
		/// Cached name for the '_finished' field.
		/// </summary>
		public static readonly StringName _finished = "_finished";
	}

	/// <summary>
	/// Cached StringNames for the signals contained in this class, for fast lookup.
	/// </summary>
	public new class SignalName : Node2D.SignalName
	{
	}

	private Creature _byrdpip;

	private MegaSprite _sprite;

	private CombatId? _combatId;

	private MegaTrackEntry? _attackEntry;

	private bool _finished;

	public static NByrdpipReadyToEvolveVfx? Create(Player player)
	{
		if (TestMode.IsOn || NonInteractiveMode.IsActive)
		{
			return null;
		}
		MegaCrit.Sts2.Core.Models.Relics.Byrdpip relic = player.GetRelic<MegaCrit.Sts2.Core.Models.Relics.Byrdpip>();
		if (relic == null || relic.SwoopsPlayed != 5)
		{
			return null;
		}
		Creature creature = player.PlayerCombatState?.GetPet<MegaCrit.Sts2.Core.Models.Monsters.Byrdpip>();
		MegaSprite megaSprite = creature?.GetCreatureNode()?.Visuals.SpineBody;
		if (creature == null || megaSprite == null)
		{
			return null;
		}
		return new NByrdpipReadyToEvolveVfx
		{
			_byrdpip = creature,
			_sprite = megaSprite,
			_combatId = CombatManager.Instance.CurrentCombatId
		};
	}

	public override void _Ready()
	{
		_sprite.ConnectAnimationStarted(Callable.From<GodotObject, GodotObject, GodotObject>(OnStarted));
		_sprite.ConnectAnimationCompleted(Callable.From<GodotObject, GodotObject, GodotObject>(OnCompleted));
		_sprite.ConnectAnimationInterrupted(Callable.From<GodotObject, GodotObject, GodotObject>(OnInterrupted));
		_sprite.ConnectAnimationEnded(Callable.From<GodotObject, GodotObject, GodotObject>(OnEnded));
	}

	private void OnStarted(GodotObject _, GodotObject __, GodotObject trackEntry)
	{
		if (!_finished && IsAttackAnimation(trackEntry))
		{
			if (_attackEntry != null)
			{
				Finish();
			}
			else
			{
				_attackEntry = _sprite.GetAnimationState().GetCurrent(0);
			}
		}
	}

	private void OnCompleted(GodotObject _, GodotObject __, GodotObject trackEntry)
	{
		if (!_finished && _attackEntry != null && IsAttackAnimation(trackEntry) && _attackEntry.IsComplete())
		{
			Finish();
			if (CombatManager.Instance.IsCurrentLiveCombat(_combatId) && !CombatManager.Instance.IsEnding)
			{
				TalkCmd.Play(new LocString("relics", "BYRDPIP.readyToEvolve"), _byrdpip, VfxColor.Green);
			}
		}
	}

	private void OnInterrupted(GodotObject _, GodotObject __, GodotObject trackEntry)
	{
		if (_attackEntry != null && !_finished && IsAttackAnimation(trackEntry) && !new MegaTrackEntry(trackEntry).IsComplete())
		{
			Finish();
		}
	}

	private void OnEnded(GodotObject _, GodotObject __, GodotObject trackEntry)
	{
		if (!_finished && _attackEntry != null && IsAttackAnimation(trackEntry) && !(_sprite.GetAnimationState().GetCurrentAnimationName() == "attack"))
		{
			Finish();
		}
	}

	private static bool IsAttackAnimation(GodotObject trackEntry)
	{
		return new MegaTrackEntry(trackEntry).GetAnimationName() == "attack";
	}

	private void Finish()
	{
		_finished = true;
		this.QueueFreeSafely();
	}

	public override void _ExitTree()
	{
		_finished = true;
		_attackEntry?.Dispose();
		_attackEntry = null;
		if (GodotObject.IsInstanceValid(_sprite.BoundObject))
		{
			_sprite.DisconnectAnimationStarted(Callable.From<GodotObject, GodotObject, GodotObject>(OnStarted));
			_sprite.DisconnectAnimationCompleted(Callable.From<GodotObject, GodotObject, GodotObject>(OnCompleted));
			_sprite.DisconnectAnimationInterrupted(Callable.From<GodotObject, GodotObject, GodotObject>(OnInterrupted));
			_sprite.DisconnectAnimationEnded(Callable.From<GodotObject, GodotObject, GodotObject>(OnEnded));
		}
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
		list.Add(new MethodInfo(MethodName.OnStarted, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.Object, "_", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Object"), exported: false),
			new PropertyInfo(Variant.Type.Object, "__", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Object"), exported: false),
			new PropertyInfo(Variant.Type.Object, "trackEntry", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Object"), exported: false)
		}, null));
		list.Add(new MethodInfo(MethodName.OnCompleted, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.Object, "_", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Object"), exported: false),
			new PropertyInfo(Variant.Type.Object, "__", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Object"), exported: false),
			new PropertyInfo(Variant.Type.Object, "trackEntry", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Object"), exported: false)
		}, null));
		list.Add(new MethodInfo(MethodName.OnInterrupted, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.Object, "_", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Object"), exported: false),
			new PropertyInfo(Variant.Type.Object, "__", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Object"), exported: false),
			new PropertyInfo(Variant.Type.Object, "trackEntry", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Object"), exported: false)
		}, null));
		list.Add(new MethodInfo(MethodName.OnEnded, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.Object, "_", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Object"), exported: false),
			new PropertyInfo(Variant.Type.Object, "__", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Object"), exported: false),
			new PropertyInfo(Variant.Type.Object, "trackEntry", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Object"), exported: false)
		}, null));
		list.Add(new MethodInfo(MethodName.IsAttackAnimation, new PropertyInfo(Variant.Type.Bool, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal | MethodFlags.Static, new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.Object, "trackEntry", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Object"), exported: false)
		}, null));
		list.Add(new MethodInfo(MethodName.Finish, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null));
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
		if (method == MethodName.OnStarted && args.Count == 3)
		{
			OnStarted(VariantUtils.ConvertTo<GodotObject>(in args[0]), VariantUtils.ConvertTo<GodotObject>(in args[1]), VariantUtils.ConvertTo<GodotObject>(in args[2]));
			ret = default;
			return true;
		}
		if (method == MethodName.OnCompleted && args.Count == 3)
		{
			OnCompleted(VariantUtils.ConvertTo<GodotObject>(in args[0]), VariantUtils.ConvertTo<GodotObject>(in args[1]), VariantUtils.ConvertTo<GodotObject>(in args[2]));
			ret = default;
			return true;
		}
		if (method == MethodName.OnInterrupted && args.Count == 3)
		{
			OnInterrupted(VariantUtils.ConvertTo<GodotObject>(in args[0]), VariantUtils.ConvertTo<GodotObject>(in args[1]), VariantUtils.ConvertTo<GodotObject>(in args[2]));
			ret = default;
			return true;
		}
		if (method == MethodName.OnEnded && args.Count == 3)
		{
			OnEnded(VariantUtils.ConvertTo<GodotObject>(in args[0]), VariantUtils.ConvertTo<GodotObject>(in args[1]), VariantUtils.ConvertTo<GodotObject>(in args[2]));
			ret = default;
			return true;
		}
		if (method == MethodName.IsAttackAnimation && args.Count == 1)
		{
			ret = VariantUtils.CreateFrom<bool>(IsAttackAnimation(VariantUtils.ConvertTo<GodotObject>(in args[0])));
			return true;
		}
		if (method == MethodName.Finish && args.Count == 0)
		{
			Finish();
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

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static bool InvokeGodotClassStaticMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		if (method == MethodName.IsAttackAnimation && args.Count == 1)
		{
			ret = VariantUtils.CreateFrom<bool>(IsAttackAnimation(VariantUtils.ConvertTo<GodotObject>(in args[0])));
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
		if (method == MethodName.OnStarted)
		{
			return true;
		}
		if (method == MethodName.OnCompleted)
		{
			return true;
		}
		if (method == MethodName.OnInterrupted)
		{
			return true;
		}
		if (method == MethodName.OnEnded)
		{
			return true;
		}
		if (method == MethodName.IsAttackAnimation)
		{
			return true;
		}
		if (method == MethodName.Finish)
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
		if (name == PropertyName._finished)
		{
			_finished = VariantUtils.ConvertTo<bool>(in value);
			return true;
		}
		return base.SetGodotClassPropertyValue(in name, in value);
	}

	/// <inheritdoc />
	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool GetGodotClassPropertyValue(in godot_string_name name, out godot_variant value)
	{
		if (name == PropertyName._finished)
		{
			value = VariantUtils.CreateFrom(in _finished);
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
		list.Add(new PropertyInfo(Variant.Type.Bool, PropertyName._finished, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false));
		return list;
	}

	/// <inheritdoc />
	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		base.SaveGodotObjectData(info);
		info.AddProperty(PropertyName._finished, Variant.From(in _finished));
	}

	/// <inheritdoc />
	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		if (info.TryGetProperty(PropertyName._finished, out var value))
		{
			_finished = value.As<bool>();
		}
	}
}
