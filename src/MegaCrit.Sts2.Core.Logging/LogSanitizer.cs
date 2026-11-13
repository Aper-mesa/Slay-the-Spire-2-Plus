using System;
using System.CodeDom.Compiler;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Text.RegularExpressions.Generated;

namespace MegaCrit.Sts2.Core.Logging;

public static class LogSanitizer
{
	private static readonly string _homeReplacement = (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "%USERPROFILE%" : "~");

	[GeneratedCode("System.Text.RegularExpressions.Generator", "9.0.12.31616")]
	private static Regex SteamIdRegex()
	{
		return _003CRegexGenerator_g_003EFACC081AAF3D765EFF87A82C4FBB77F6FD3EA759AA2D03D993988F88E97CC0B5B__SteamIdRegex_4.Instance;
	}

	public static string Sanitize(string text)
	{
		string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
		if (!string.IsNullOrEmpty(folderPath))
		{
			text = text.Replace(folderPath, _homeReplacement);
			string text2 = folderPath.Replace('\\', '/');
			if (text2 != folderPath)
			{
				text = text.Replace(text2, _homeReplacement);
			}
		}
		text = SteamIdRegex().Replace(text, ReplaceSteamId);
		return text;
	}

	public static byte[] AnonymizeSteamIds(byte[] bytes)
	{
		string input = Encoding.Latin1.GetString(bytes);
		input = SteamIdRegex().Replace(input, (Match m) => IdAnonymizer.Anonymize(ulong.Parse(m.Value)).ToString());
		return Encoding.Latin1.GetBytes(input);
	}

	public static string ReplaceSteamId(Match m)
	{
		ulong id = ulong.Parse(m.Value);
		string text = IdAnonymizer.Anonymize(id).ToString();
		return "A" + text;
	}
}
