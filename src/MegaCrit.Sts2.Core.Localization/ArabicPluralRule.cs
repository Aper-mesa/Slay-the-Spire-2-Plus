using System;
using SmartFormat.Utilities;

namespace MegaCrit.Sts2.Core.Localization;

/// <summary>
/// SmartFormat's built-in Arabic rule follows CLDR and only accepts exactly six forms (zero|one|two|few|many|other).
/// Our Arabic loc mostly uses the simpler singular|plural shape (and occasionally singular|dual|plural), which the
/// built-in rule rejects with "Invalid number of plural parameters" (or worse, picks the plural form for 1).
/// This rule chooses an index based on how many forms the string actually provides.
/// </summary>
public static class ArabicPluralRule
{
	private const string _isoCode = "ar";

	/// <summary>
	/// Replaces SmartFormat's global Arabic plural rule. Safe to call more than once.
	/// </summary>
	public static void Register()
	{
		PluralRules.IsoLangToDelegate["ar"] = Select;
	}

	public static int Select(decimal value, int pluralWordsCount)
	{
		decimal num = Math.Abs(value);
		int result;
		switch (pluralWordsCount)
		{
		case 2:
			return (!(num == 1m)) ? 1 : 0;
		case 3:
			return (!(num == 1m)) ? ((num == 2m) ? 1 : 2) : 0;
		case 6:
			return SelectCldr(num);
		default:
			result = ((!(num == 1m)) ? (pluralWordsCount - 1) : 0);
			break;
		case 1:
			result = 0;
			break;
		}
		return result;
	}

	private static int SelectCldr(decimal n)
	{
		if (n == 0m)
		{
			return 0;
		}
		if (n == 1m)
		{
			return 1;
		}
		if (n == 2m)
		{
			return 2;
		}
		if (n == decimal.Truncate(n))
		{
			decimal num = n % 100m;
			if (num >= 3m && num <= 10m)
			{
				return 3;
			}
			if (num >= 11m && num <= 99m)
			{
				return 4;
			}
		}
		return 5;
	}
}
