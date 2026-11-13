using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace MegaCrit.Sts2.Core.Models.Relics;

public sealed class CaptainsWheel : RelicModel
{
	public override RelicRarity Rarity => RelicRarity.Rare;

	protected override IEnumerable<DynamicVar> CanonicalVars => new _003C_003Ez__ReadOnlySingleElementList<DynamicVar>(new BlockVar(18m, ValueProp.Unpowered));

	protected override IEnumerable<IHoverTip> ExtraHoverTips => new _003C_003Ez__ReadOnlySingleElementList<IHoverTip>(HoverTipFactory.Static(StaticHoverTip.Block));

	/// <summary>
	/// The relic does not gray out after it triggers. Nothing is consumed and it triggers again on turn 3 of every
	/// combat, so it never sets <see cref="P:MegaCrit.Sts2.Core.Models.RelicModel.Status" />. Permafrost is the same. Closed as designed in
	/// PRG-5964, PRG-5712 and PRG-5628.
	/// </summary>
	public override async Task AfterBlockCleared(Creature creature)
	{
		if (creature == Owner.Creature && Owner.PlayerCombatState.TurnNumber == 3)
		{
			Flash();
			await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, null);
		}
	}
}
