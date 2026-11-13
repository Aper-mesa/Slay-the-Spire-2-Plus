using System;
using System.Collections.Generic;
using System.Linq;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Runs;
using SmartFormat.Core.Extensions;
using SmartFormat.Core.Formatting;

namespace MegaCrit.Sts2.Core.Localization.Formatters;

public class EnergyIconsFormatter : IFormatter
{
	private const string _fallbackIconPrefix = "colorless";

	public string Name
	{
		get
		{
			return "energyIcons";
		}
		set
		{
			throw new NotImplementedException();
		}
	}

	public bool CanAutoDetect { get; set; }

	public bool TryEvaluateFormat(IFormattingInfo formattingInfo)
	{
		string text = null;
		object currentValue = formattingInfo.CurrentValue;
		int result;
		if (!(currentValue is EnergyVar energyVar))
		{
			if (!(currentValue is CalculatedVar calculatedVar))
			{
				if (!(currentValue is decimal num))
				{
					if (!(currentValue is int num2))
					{
						if (!(currentValue is string text2))
						{
							throw new LocException($"Unknown value='{formattingInfo.CurrentValue}' type={formattingInfo.CurrentValue?.GetType()}");
						}
						if (!int.TryParse(formattingInfo.FormatterOptions, out result))
						{
							return false;
						}
						text = text2;
					}
					else
					{
						result = num2;
					}
				}
				else
				{
					result = (int)num;
				}
			}
			else
			{
				result = Convert.ToInt32(calculatedVar.Calculate(null));
			}
		}
		else
		{
			result = Convert.ToInt32(energyVar.PreviewValue);
			if (!string.IsNullOrEmpty(energyVar.ColorPrefix))
			{
				text = energyVar.ColorPrefix;
			}
		}
		if ((string.IsNullOrEmpty(text) || text == "colorless") && formattingInfo is FormattingInfo info)
		{
			FormattingInfo rootInfo = GetRootInfo(info);
			if (rootInfo.CurrentValue is Dictionary<string, object> dictionary && dictionary.TryGetValue("energyPrefix", out var value) && value is string text3)
			{
				text = text3;
			}
		}
		if (string.IsNullOrEmpty(text) || text == "colorless")
		{
			text = RunManager.Instance.GetLocalCharacterEnergyIconPrefix();
		}
		if (text == null)
		{
			Log.Warn("No energy prefix found for EnergyIconsFormatter! Using colorless as a fallback.");
			if (text == null)
			{
				text = "colorless";
			}
		}
		string text4 = "[img]res://images/packed/sprite_fonts/" + text + "_energy_icon.png[/img]";
		string text5;
		if (result > 0 && result < 5)
		{
			text5 = string.Concat(Enumerable.Repeat(text4, result));
		}
		else
		{
			text5 = ((!(formattingInfo.CurrentValue is DynamicVar dynamicVar)) ? $"{result}{text4}" : (dynamicVar.ToHighlightedString(inverse: false) + text4));
		}
		formattingInfo.Write(text5);
		return true;
	}

	private FormattingInfo GetRootInfo(FormattingInfo info)
	{
		while (info.Parent != null)
		{
			info = info.Parent;
		}
		return info;
	}
}
