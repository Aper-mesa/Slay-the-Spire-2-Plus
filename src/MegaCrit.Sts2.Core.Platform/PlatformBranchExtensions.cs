using System;

namespace MegaCrit.Sts2.Core.Platform;

public static class PlatformBranchExtensions
{
	/// <summary>
	/// Prefer over ToString as this gives the actual branch name as displayed in Steam or other platform
	/// </summary>
	public static string ToName(this PlatformBranch branch)
	{
		return branch switch
		{
			PlatformBranch.None => "none",
			PlatformBranch.DevTest => "dev-test",
			PlatformBranch.PrivateBeta => "private-beta",
			PlatformBranch.PublicBeta => "public-beta",
			PlatformBranch.Production => "public",
			_ => "unknown",
		};
	}

	public static PlatformBranch? FromName(string name)
	{
		if (name == "dev-test")
		{
			return PlatformBranch.DevTest;
		}
		if (name.StartsWith("private-", StringComparison.Ordinal))
		{
			return PlatformBranch.PrivateBeta;
		}
		if (name == "public-beta")
		{
			return PlatformBranch.PublicBeta;
		}
		if (name == "public")
		{
			return PlatformBranch.Production;
		}
		return null;
	}
}
