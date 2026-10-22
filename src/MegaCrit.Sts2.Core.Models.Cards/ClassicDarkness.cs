using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Orbs;

namespace MegaCrit.Sts2.Core.Models.Cards;

public sealed class ClassicDarkness : ClassicCardModel
{
	protected override IEnumerable<IHoverTip> ExtraHoverTips => new _003C_003Ez__ReadOnlyArray<IHoverTip>(new IHoverTip[2]
	{
		HoverTipFactory.Static(StaticHoverTip.Channeling),
		HoverTipFactory.FromOrb<DarkOrb>()
	});

	public ClassicDarkness()
		: base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
	{
	}

	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
		await OrbCmd.Channel<DarkOrb>(choiceContext, Owner);
		if (!IsUpgraded)
		{
			return;
		}
		IEnumerable<OrbModel> darkOrbs = Owner.PlayerCombatState.OrbQueue.Orbs.Where((OrbModel orb) => orb is DarkOrb).ToList();
		foreach (OrbModel darknessOrb in darkOrbs)
		{
			await OrbCmd.Passive(choiceContext, darknessOrb, null);
		}
	}
}
