using System;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.ValueProps;

namespace MegaCrit.Sts2.Core.Models.Relics;

public sealed class ClassicIroncladPaperPhrog : ClassicIroncladRelicModel
{
    public override RelicRarity Rarity => RelicRarity.Uncommon;
    public decimal ModifyVulnerableMultiplier(decimal amount) => amount + 0.25m;
}

public sealed class ClassicIroncladChampionsBelt : ClassicIroncladRelicModel
{
    public override RelicRarity Rarity => RelicRarity.Rare;
    public override async Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if (power is VulnerablePower && amount > 0 && applier == Owner.Creature && power.Owner.Side != Owner.Creature.Side)
        {
            Flash();
            await PowerCmd.Apply<WeakPower>(choiceContext, power.Owner, 1, Owner.Creature, null);
        }
    }
}

public sealed class ClassicIroncladMagicFlower : ClassicIroncladRelicModel
{
    public override RelicRarity Rarity => RelicRarity.Rare;
    public decimal ModifyHealing(decimal amount)
    {
        // Victory healing runs after IsInProgress clears but before the combat room is marked finished.
        if (amount <= 0 || Owner.RunState.CurrentRoom is not CombatRoom { IsPreFinished: false }) return amount;
        Flash();
        return Math.Round(amount * 1.5m, MidpointRounding.AwayFromZero);
    }
}

// Classic boss relics use the rare reward pool, as with the other classic characters.
public sealed class ClassicIroncladBlackBlood : ClassicIroncladRelicModel
{
    public override RelicRarity Rarity => RelicRarity.Rare;
    public override bool HasUponPickupEffect => true;
    public override async Task AfterObtained()
    {
        var starter = Owner.GetRelic<ClassicIroncladBurningBlood>();
        if (starter != null) await RelicCmd.Remove(starter);
    }
    public override async Task AfterCombatVictory(CombatRoom room)
    {
        if (Owner.Creature.IsAlive) await CreatureCmd.Heal(Owner.Creature, 12);
    }
}

public sealed class ClassicIroncladMarkOfPain : ClassicIroncladRelicModel
{
    public override RelicRarity Rarity => RelicRarity.Rare;
    public override decimal ModifyMaxEnergy(Player player, decimal amount) => player == Owner ? amount + 1 : amount;
    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext choiceContext, ICombatState combatState)
    {
        if (player != Owner || player.PlayerCombatState.TurnNumber != 1) return;
        Flash();
        for (int i = 0; i < 2; i++)
            await CardPileCmd.AddGeneratedCardToCombat(combatState.CreateCard<Wound>(Owner), PileType.Draw, Owner, CardPilePosition.Random);
    }
}

public sealed class ClassicIroncladRunicCube : ClassicIroncladRelicModel
{
    public override RelicRarity Rarity => RelicRarity.Rare;
    public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (target == Owner.Creature && result.UnblockedDamage > 0)
        {
            Flash();
            await CardPileCmd.Draw(choiceContext, 1, Owner);
        }
    }
}
