using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Monsters;

namespace MegaCrit.Sts2.Core.Models.Powers;

public sealed class RavenousPower : PowerModel
{
	public override PowerType Type => PowerType.Buff;

	public override PowerStackType StackType => PowerStackType.Counter;

	protected override IEnumerable<IHoverTip> ExtraHoverTips => new _003C_003Ez__ReadOnlyArray<IHoverTip>(new IHoverTip[2]
	{
		HoverTipFactory.FromPower<StrengthPower>(),
		HoverTipFactory.Static(StaticHoverTip.Stun)
	});

	public override async Task AfterDeath(PlayerChoiceContext choiceContext, Creature target, bool wasRemovalPrevented, float deathAnimLength)
	{
		if (!wasRemovalPrevented && target != Owner && target.Side == Owner.Side && !Owner.IsDead)
		{
			Flash();
			SfxCmd.Play("event:/sfx/enemy/enemy_attacks/corpse_slugs/corpse_slugs_ravenous");
			await CreatureCmd.TriggerAnim(Owner, "DevourStartTrigger", 0.5f);
			((CorpseSlug)Owner.Monster).IsRavenous = true;
			await CreatureCmd.Stun(Owner, StunnedMove);
			await PowerCmd.Apply<StrengthPower>(choiceContext, Owner, Amount, Owner, null);
		}
	}

	private async Task StunnedMove(IReadOnlyList<Creature> targets)
	{
		SfxCmd.Play("event:/sfx/enemy/enemy_attacks/corpse_slugs/corpse_slugs_ravenous_up_double");
		await CreatureCmd.TriggerAnim(Owner, "DevourEndkTrigger", 0.5f);
		((CorpseSlug)Owner.Monster).IsRavenous = false;
	}
}
