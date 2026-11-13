using System.Collections.Generic;
using Godot;
using MegaCrit.Sts2.Core.Helpers;

namespace MegaCrit.Sts2.Core.Bindings.MegaSpine;

/// <summary>
/// C# bindings for SpineSlotNode.
/// </summary>
public class MegaSlotNode : MegaSpineBinding
{
	public const string spineClassName = "SpineSlotNode";

	private static readonly StringName _slotNameProperty = "slot_name";

	protected override string SpineClassName => "SpineSlotNode";

	protected override IEnumerable<string> SpineMethods => new _003C_003Ez__ReadOnlySingleElementList<string>("get_normal_material");

	public MegaSlotNode(Variant native)
		: base(native)
	{
	}

	public Material? GetNormalMaterial()
	{
		return CallNullable("get_normal_material")?.As<Material>();
	}

	/// <summary>
	/// Creates a SpineSlotNode under the given SpineSprite that follows the given slot. Creating it here rather than
	/// in a scene avoids the warning a slot node logs whenever its parent isn't a SpineSprite.
	/// </summary>
	public static Node2D Create(Node2D spineSprite, string slotName)
	{
		Node2D node2D = ClassDB.Instantiate("SpineSlotNode").As<Node2D>();
		spineSprite.AddChildSafely(node2D);
		node2D.Set(_slotNameProperty, slotName);
		return node2D;
	}
}
