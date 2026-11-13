using Godot;
using MegaCrit.Sts2.Core.Helpers;

namespace MegaCrit.Sts2.Core.Nodes.TopBar;

public class BlitzIconConfig : TopBarAscensionIconConfig
{
	protected override string TexturePath => ImageHelper.GetImagePath("packed/character_select/blitz_top_bar_icon.png");

	public override Color FontColor => new Color(0.498039f, 1f, 0f);

	public override Color FontOutlineColor => new Color(0.0186f, 0.31f, 0.13516003f);
}
