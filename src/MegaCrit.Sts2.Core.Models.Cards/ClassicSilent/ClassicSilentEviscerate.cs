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

public sealed class ClassicSilentEviscerate : ClassicSilentCardModel
{
	protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(7m, ValueProp.Move)];
	public ClassicSilentEviscerate() : base(3, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy) { }
	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this, cardPlay).WithHitCount(3).Targeting(cardPlay.Target).WithHitFx("vfx/vfx_attack_slash").Execute(choiceContext);
	}
	protected override void OnUpgrade()
	{
		DynamicVars.Damage.UpgradeValueBy(2m);
	}

    public override bool TryModifyEnergyCostInCombat(CardModel card, decimal originalCost, out decimal modifiedCost)
    {
        modifiedCost = originalCost;
        if (card != this) return false;
        int discards = CombatManager.Instance.History.Entries.OfType<CardDiscardedEntry>()
            .Count(e => e.Actor.Player == Owner && e.HappenedThisTurn(CombatState));
        modifiedCost = Math.Max(0, originalCost - discards);
        return true;
    }
}
