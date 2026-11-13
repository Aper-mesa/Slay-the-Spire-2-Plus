namespace MegaCrit.Sts2.Core.Entities.Creatures;

public record struct DamageTargetData(Creature target)
{
	public Creature target = target;

	public string? hitVfx = null;

	public bool playVfxOnCreatureCenter = false;
}
