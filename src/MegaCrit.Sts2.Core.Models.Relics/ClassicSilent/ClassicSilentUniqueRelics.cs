using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace MegaCrit.Sts2.Core.Models.Relics;

public sealed class ClassicSilentPaperKrane : ClassicSilentRelicModel
{
    public override RelicRarity Rarity => RelicRarity.Uncommon;

    public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource, CardPlay? cardPlay)
    {
        // Weak already contributes 0.75; bring it to 0.60 without changing the sequel's Paper Krane.
        if (target == Owner.Creature && props.IsPoweredAttack() && dealer?.HasPower<WeakPower>() == true
            && Owner.GetRelic<PaperKrane>() == null && !dealer.HasPower<DebilitatePower>())
            return 0.8m;
        return 1m;
    }
}

public sealed class ClassicSilentTheSpecimen : ClassicSilentRelicModel
{
    public override RelicRarity Rarity => RelicRarity.Rare;

    public override async Task AfterDeath(PlayerChoiceContext choiceContext, Creature creature, bool wasRemovalPrevented, float deathAnimLength)
    {
        if (creature.Side == Owner.Creature.Side || !creature.IsDead) return;
        int poison = creature.GetPowerAmount<PoisonPower>();
        if (poison <= 0) return;
        Creature target = Owner.RunState.Rng.CombatTargets.NextItem(Owner.Creature.CombatState.HittableEnemies.Where(c => c != creature));
        if (target != null)
        {
            Flash();
            await PowerCmd.Apply<PoisonPower>(choiceContext, target, poison, Owner.Creature, null);
        }
    }
}

// STS2 has no boss-relic reward tier; classic boss relics follow ClassicDefect's rare-pool convention.
public sealed class ClassicSilentRingOfTheSerpent : ClassicSilentRelicModel
{
    public override RelicRarity Rarity => RelicRarity.Rare;
    public override bool HasUponPickupEffect => true;

    public override async Task AfterObtained()
    {
        var starter = Owner.GetRelic<ClassicSilentRingOfTheSnake>();
        if (starter != null) await RelicCmd.Remove(starter);
    }

    public override decimal ModifyHandDraw(Player player, decimal count) => player == Owner ? count + 1 : count;
}

public sealed class ClassicSilentWristBlade : ClassicSilentRelicModel
{
    public override RelicRarity Rarity => RelicRarity.Rare;

    public override decimal ModifyDamageAdditive(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource, CardPlay? cardPlay)
    {
        if (dealer == Owner.Creature && cardSource?.Type == CardType.Attack && props.IsPoweredAttack()
            && !cardSource.EnergyCost.CostsX && cardSource.EnergyCost.GetWithModifiers(CostModifiers.All) == 0)
            return 4;
        return 0;
    }
}

public sealed class ClassicSilentHoveringKite : ClassicSilentRelicModel
{
    public override RelicRarity Rarity => RelicRarity.Rare;

    public override async Task AfterCardDiscarded(PlayerChoiceContext choiceContext, CardModel card)
    {
        if (card.Owner != Owner) return;
        int discards = CombatManager.Instance.History.Entries.OfType<CardDiscardedEntry>()
            .Count(e => e.Actor.Player == Owner && e.HappenedThisTurn(Owner.Creature.CombatState));
        if (discards == 1)
        {
            Flash();
            await PlayerCmd.GainEnergy(1, Owner);
        }
    }
}
