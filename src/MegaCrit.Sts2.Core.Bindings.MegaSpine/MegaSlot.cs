using System;
using System.Collections.Generic;
using Godot;

namespace MegaCrit.Sts2.Core.Bindings.MegaSpine;

public class MegaSlot : MegaSpineBinding
{
	protected override string SpineClassName => "SpineSlot";

	protected override IEnumerable<string> SpineMethods => Array.Empty<string>();

	public MegaSlot(Variant native)
		: base(native)
	{
	}
}
