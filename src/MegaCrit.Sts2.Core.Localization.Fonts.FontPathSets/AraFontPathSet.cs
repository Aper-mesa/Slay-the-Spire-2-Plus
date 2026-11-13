using System;

namespace MegaCrit.Sts2.Core.Localization.Fonts.FontPathSets;

public class AraFontPathSet : FontPathSet
{
	private const string _regular = "res://themes/fonts/ara/alyamama_regular_shared.tres";

	private const string _bold = "res://themes/fonts/ara/alyamama_bold_shared.tres";

	public override string GetPath(FontType type)
	{
		return type switch
		{
			FontType.Regular => "res://themes/fonts/ara/alyamama_regular_shared.tres",
			FontType.Bold => "res://themes/fonts/ara/alyamama_bold_shared.tres",
			FontType.Italic => "res://themes/fonts/ara/alyamama_regular_shared.tres",
			_ => throw new ArgumentOutOfRangeException("type", type, null),
		};
	}
}
