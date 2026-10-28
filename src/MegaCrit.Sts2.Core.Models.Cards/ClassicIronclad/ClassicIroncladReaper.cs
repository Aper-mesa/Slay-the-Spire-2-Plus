using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace MegaCrit.Sts2.Core.Models.Cards;

public sealed class ClassicIroncladReaper : ClassicIroncladCardModel
{
	protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(4m, ValueProp.Move)];
	public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
	public ClassicIroncladReaper() : base(2, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies) { }
	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		var attack = await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this, cardPlay)
		    .TargetingAllOpponents(CombatState).WithHitFx("vfx/vfx_attack_slash").Execute(choiceContext);
		int healing = attack.Results.SelectMany(r => r).Sum(r => r.UnblockedDamage - r.OverkillDamage);
		if (healing > 0 && Owner.Creature.IsAlive) await CreatureCmd.Heal(Owner.Creature, healing);
	}
	protected override void OnUpgrade()
	{
		DynamicVars.Damage.UpgradeValueBy(1m);
	}
    public override bool CanBeGeneratedInCombat => false;
}
