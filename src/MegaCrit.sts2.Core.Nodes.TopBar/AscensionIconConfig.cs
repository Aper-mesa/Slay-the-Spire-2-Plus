using Godot;
using MegaCrit.Sts2.Core.Helpers;

namespace MegaCrit.Sts2.Core.Nodes.TopBar;

/// <summary>
/// Standard Ascension runs. Red in singleplayer, blue in multiplayer.
/// </summary>
public class AscensionIconConfig : TopBarAscensionIconConfig
{
	private readonly bool _isMultiplayer;

	protected override string TexturePath => ImageHelper.GetImagePath("atlases/ui_atlas.sprites/top_bar/top_bar_ascension.tres");

	public override Color FontColor => new Color(1f, 0.964706f, 0.886275f);

	public override Color FontOutlineColor
	{
		get
		{
			if (!_isMultiplayer)
			{
				return new Color("593400");
			}
			return new Color("004759");
		}
	}

	public override float Hue
	{
		get
		{
			if (!_isMultiplayer)
			{
				return 1f;
			}
			return 0.52f;
		}
	}

	public override float Value
	{
		get
		{
			if (!_isMultiplayer)
			{
				return 1f;
			}
			return 1.2f;
		}
	}

	public AscensionIconConfig(bool isMultiplayer)
	{
		_isMultiplayer = isMultiplayer;
	}
}
