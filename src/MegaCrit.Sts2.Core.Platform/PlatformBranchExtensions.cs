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
		};
	}

	public static PlatformBranch? FromName(string name)
	{
		return name switch
		{
			"dev-test" => (PlatformBranch?)PlatformBranch.DevTest, 
			"private-beta" => PlatformBranch.PrivateBeta, 
			"public-beta" => PlatformBranch.PublicBeta, 
			"public" => PlatformBranch.Production, 
			_ => null, 
		};
	}
}
