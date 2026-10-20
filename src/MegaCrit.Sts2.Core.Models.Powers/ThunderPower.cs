using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Orbs;
using MegaCrit.Sts2.Core.ValueProps;

namespace MegaCrit.Sts2.Core.Models.Powers;

public sealed class ThunderPower : PowerModel
{
	public override PowerType Type => PowerType.Buff;

	public override PowerStackType StackType => PowerStackType.Counter;

	protected override IEnumerable<IHoverTip> ExtraHoverTips => new _003C_003Ez__ReadOnlyArray<IHoverTip>(new IHoverTip[2]
	{
		HoverTipFactory.Static(StaticHoverTip.Evoke),
		HoverTipFactory.FromOrb<LightningOrb>()
	});

	public override async Task AfterOrbEvoked(PlayerChoiceContext choiceContext, OrbModel orb, IEnumerable<Creature> targets)
	{
		if (orb.Owner == Owner.Player && orb is LightningOrb)
		{
			List<Creature> livingTargets = targets.Where((Creature c) => c.IsAlive).ToList();
			Flash();
			SfxCmd.Play("slash_attack.mp3");
			VfxCmd.PlayOnCreatureCenters(livingTargets, "vfx/vfx_attack_slash");
			await CreatureCmd.TriggerAnim(orb.Owner.Creature, "Attack", Owner.Player.Character.AttackAnimDelay);
			await CreatureCmd.Damage(choiceContext, livingTargets, Amount, ValueProp.Unpowered, Owner, null, null);
		}
	}
}
