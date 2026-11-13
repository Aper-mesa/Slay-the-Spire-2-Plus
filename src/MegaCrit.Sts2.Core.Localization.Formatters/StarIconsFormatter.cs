using System;
using System.Linq;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using SmartFormat.Core.Extensions;

namespace MegaCrit.Sts2.Core.Localization.Formatters;

public class StarIconsFormatter : IFormatter
{
	private const string _starIconPath = "res://images/packed/sprite_fonts/star_icon.png";

	public const string starIconSprite = "[img]res://images/packed/sprite_fonts/star_icon.png[/img]";

	public string Name
	{
		get
		{
			return "starIcons";
		}
		set
		{
			throw new NotImplementedException();
		}
	}

	public bool CanAutoDetect { get; set; }

	public bool TryEvaluateFormat(IFormattingInfo formattingInfo)
	{
		object currentValue = formattingInfo.CurrentValue;
		int num3;
		if (!(currentValue is DynamicVar dynamicVar))
		{
			if (!(currentValue is decimal num))
			{
				if (!(currentValue is int num2))
				{
					throw new LocException($"Unknown value='{formattingInfo.CurrentValue}' type={formattingInfo.CurrentValue?.GetType()}");
				}
				num3 = num2;
			}
			else
			{
				num3 = (int)num;
			}
		}
		else
		{
			num3 = (int)dynamicVar.PreviewValue;
		}
		int num4 = num3;
		string text;
		if (num4 > 0 && num4 < 5)
		{
			text = string.Concat(Enumerable.Repeat("[img]res://images/packed/sprite_fonts/star_icon.png[/img]", num4));
		}
		else
		{
			text = ((!(formattingInfo.CurrentValue is DynamicVar dynamicVar2)) ? $"{num4}{"[img]res://images/packed/sprite_fonts/star_icon.png[/img]"}" : (dynamicVar2.ToHighlightedString(inverse: false) + "[img]res://images/packed/sprite_fonts/star_icon.png[/img]"));
		}
		formattingInfo.Write(text);
		return true;
	}
}
