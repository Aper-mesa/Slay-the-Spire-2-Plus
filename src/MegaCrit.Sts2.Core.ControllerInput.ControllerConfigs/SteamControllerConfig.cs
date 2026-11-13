using MegaCrit.Sts2.Core.Helpers;

namespace MegaCrit.Sts2.Core.ControllerInput.ControllerConfigs;

public class SteamControllerConfig : ControllerConfig
{
	private const string _rightStickFolderPath = "atlases/controller_atlas.sprites/ps4";

	protected override string FolderPath => "atlases/controller_atlas.sprites/steam";

	protected override string RightJoystickNorth => ImageHelper.GetImagePath("atlases/controller_atlas.sprites/ps4/rs_up.tres");

	protected override string RightJoystickSouth => ImageHelper.GetImagePath("atlases/controller_atlas.sprites/ps4/rs_down.tres");

	protected override string RightJoystickEast => ImageHelper.GetImagePath("atlases/controller_atlas.sprites/ps4/rs_right.tres");

	protected override string RightJoystickWest => ImageHelper.GetImagePath("atlases/controller_atlas.sprites/ps4/rs_left.tres");
}
