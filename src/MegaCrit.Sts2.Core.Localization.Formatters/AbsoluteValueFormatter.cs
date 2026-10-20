using System;
using System.Globalization;
using SmartFormat.Core.Extensions;

namespace MegaCrit.Sts2.Core.Localization.Formatters;

public class AbsoluteValueFormatter : IFormatter
{
	public string Name
	{
		get
		{
			return "abs";
		}
		set
		{
			throw new NotImplementedException();
		}
	}

	public bool CanAutoDetect { get; set; }

	private static CultureInfo Culture => LocManager.Instance.CultureInfo;

	public bool TryEvaluateFormat(IFormattingInfo formattingInfo)
	{
		object currentValue = formattingInfo.CurrentValue;
		string text;
		if (currentValue is decimal value)
		{
			text = Math.Abs(value).ToString(Culture);
		}
		else if (currentValue is double value2)
		{
			text = Math.Abs(value2).ToString(Culture);
		}
		else if (currentValue is float value3)
		{
			text = Math.Abs(value3).ToString(Culture);
		}
		else if (currentValue is int value4)
		{
			text = Math.Abs(value4).ToString();
		}
		else if (currentValue is long value5)
		{
			text = Math.Abs(value5).ToString();
		}
		else
		{
			text = ((!(currentValue is short value6)) ? null : Math.Abs(value6).ToString());
		}
		string text2 = text;
		if (text2 == null)
		{
			return false;
		}
		formattingInfo.Write(text2);
		return true;
	}
}
