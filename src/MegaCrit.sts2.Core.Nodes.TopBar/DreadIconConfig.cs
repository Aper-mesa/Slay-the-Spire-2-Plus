using Godot;
using MegaCrit.Sts2.Core.Helpers;

namespace MegaCrit.Sts2.Core.Nodes.TopBar;

public class DreadIconConfig : TopBarAscensionIconConfig
{
	protected override string TexturePath => ImageHelper.GetImagePath("packed/character_select/dread_top_bar_icon.png");

	public override Color FontColor => new Color(135f / 255f, 206f / 255f, 235f / 255f);

	public override Color FontOutlineColor => new Color(0.025337905f, 0.199606f, 0.3713474f);
}
