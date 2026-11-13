using Godot;

namespace MegaCrit.Sts2.Core.Nodes.TopBar;

/// <summary>
/// Visual config for the top bar's AscensionIcon. The icon is shared between Ascension, Dread, and Blitz runs (only
/// one is ever shown), so each mode supplies its own texture, label colors, and HSV tint through a subclass of this.
/// </summary>
public abstract class TopBarAscensionIconConfig
{
	protected abstract string TexturePath { get; }

	public abstract Color FontColor { get; }

	public abstract Color FontOutlineColor { get; }

	public virtual float Hue => 1f;

	public virtual float Saturation => 1f;

	public virtual float Value => 1f;

	public Texture2D Texture => ResourceLoader.Load<Texture2D>(TexturePath, null, ResourceLoader.CacheMode.Reuse);
}
