using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.FoolArrows;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace MegaCrit.Sts2.Core.Models.Cards;

public sealed class EpicRetrieval : CardModel
{
	protected override IEnumerable<IHoverTip> ExtraHoverTips => new _003C_003Ez__ReadOnlyArray<IHoverTip>(new IHoverTip[2] { ArrowHoverTip, EnergyHoverTip });

	protected override bool ShouldGlowGoldInternal => CombatState?.HittableEnemies.Any(NextHitWillDislodgeArrowFrom) ?? false;

	protected override IEnumerable<DynamicVar> CanonicalVars => new _003C_003Ez__ReadOnlyArray<DynamicVar>(new DynamicVar[2]
	{
		new DamageVar(15m, ValueProp.Move),
		new EnergyVar(3)
	});

	public EpicRetrieval()
		: base(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
	{
	}

	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
		await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this, cardPlay).Targeting(cardPlay.Target)
			.WithHitFx("vfx/vfx_attack_slash")
			.Execute(choiceContext);
	}

	public override async Task AfterArrowDislodged(ImpaledPower power, ArrowSlot arrow, PlayerChoiceContext context, CardModel? source)
	{
		if (power.Applier == Owner.Creature && source == this)
		{
			await PlayerCmd.GainEnergy(DynamicVars.Energy.BaseValue, Owner);
		}
	}

	protected override void OnUpgrade()
	{
		DynamicVars.Damage.UpgradeValueBy(5m);
		DynamicVars.Energy.UpgradeValueBy(1m);
	}

	private bool NextHitWillDislodgeArrowFrom(Creature creature)
	{
		ImpaledPower impaledPower = creature.GetPowerInstances<ImpaledPower>().FirstOrDefault((ImpaledPower p) => p.Applier == Owner.Creature);
		if (impaledPower == null)
		{
			return false;
		}
		return impaledPower.HitCounter == 1;
	}
}
