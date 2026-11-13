using System.Collections.Generic;
using Godot;
using Godot.Collections;

namespace MegaCrit.Sts2.Core.Bindings.MegaSpine;

/// <summary>
/// C# bindings for SpineSkeletonDataResource.
/// </summary>
public class MegaSkeletonDataResource : MegaSpineBinding
{
	protected override string SpineClassName => "SpineSkeletonDataResource";

	protected override IEnumerable<string> SpineMethods => new _003C_003Ez__ReadOnlyArray<string>(new string[4] { "find_animation", "find_skin", "get_animations", "get_skins" });

	public MegaSkeletonDataResource(Variant native)
		: base(native)
	{
	}

	public MegaSkin? FindSkin(string skinName)
	{
		Variant native = Call("find_skin", skinName);
		if (native.AsGodotObject() == null)
		{
			return null;
		}
		return new MegaSkin(native);
	}

	/// <summary>
	/// Whether this skeleton defines an animation with the given name. Returns a bool rather than the
	/// animation so no transient <see cref="T:MegaCrit.Sts2.Core.Bindings.MegaSpine.MegaAnimation" /> wrapper escapes to callers, which would
	/// invite the GC-unsafe chain this binding is hardened against (PRG-6985).
	/// </summary>
	public bool HasAnimation(string animName)
	{
		using Variant variant = Call("find_animation", animName);
		using GodotObject godotObject = variant.AsGodotObject();
		return godotObject != null;
	}

	/// <summary>
	/// Names of every animation this skeleton defines. Returns the names rather than the wrappers so no
	/// transient <see cref="T:MegaCrit.Sts2.Core.Bindings.MegaSpine.MegaAnimation" /> escapes to callers, keeping the GC-unsafe read chain
	/// unrepresentable through this API (PRG-6985).
	/// </summary>
	public IReadOnlyList<string> GetAnimationNames()
	{
		using Variant variant = Call("get_animations");
		using Array array = variant.AsGodotArray();
		List<string> list = new List<string>(array.Count);
		for (int i = 0; i < array.Count; i++)
		{
			using Variant native = array[i];
			using MegaAnimation megaAnimation = new MegaAnimation(native);
			list.Add(megaAnimation.GetName());
		}
		return list;
	}

	public Array<GodotObject> GetSkins()
	{
		return (Array<GodotObject>)Call("get_skins");
	}
}
