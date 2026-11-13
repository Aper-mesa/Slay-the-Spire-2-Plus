using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;

namespace MegaCrit.Sts2.Core.Models.Cards;

public sealed class Uproot : CardModel
{
	protected override bool ShouldGlowGoldInternal
	{
		get
		{
			if (CombatState == null)
			{
				return false;
			}
			return CombatState.HittableEnemies.Any((Creature c) => ArrowCmd.CreatureIsImpaledByPlayersArrow(c, Owner));
		}
	}

	protected override IEnumerable<IHoverTip> ExtraHoverTips => new _003C_003Ez__ReadOnlyArray<IHoverTip>(new IHoverTip[2]
	{
		HoverTipFactory.FromPower<ImpaledPower>(),
		HoverTipFactory.FromPower<VulnerablePower>()
	});

	protected override IEnumerable<DynamicVar> CanonicalVars => new _003C_003Ez__ReadOnlySingleElementList<DynamicVar>(new PowerVar<VulnerablePower>(3m));

	public Uproot()
		: base(1, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy)
	{
	}

	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
		if (ArrowCmd.CreatureIsImpaledByPlayersArrow(cardPlay.Target, Owner))
		{
			while (ArrowCmd.CreatureIsImpaledByPlayersArrow(cardPlay.Target, Owner))
			{
				await ArrowCmd.DislodgeArrow(cardPlay.Target, cardPlay.Player, choiceContext, this);
			}
			await PowerCmd.Apply<VulnerablePower>(choiceContext, cardPlay.Target, DynamicVars.Vulnerable.BaseValue, Owner.Creature, this);
		}
	}

	protected override void OnUpgrade()
	{
		DynamicVars.Vulnerable.UpgradeValueBy(1m);
	}
}
