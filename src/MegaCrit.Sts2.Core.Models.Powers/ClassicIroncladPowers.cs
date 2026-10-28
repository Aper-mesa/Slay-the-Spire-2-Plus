using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace MegaCrit.Sts2.Core.Models.Powers;

public abstract class ClassicIroncladPowerModel : PowerModel
{
    protected abstract PowerModel ArtPower { get; }
    public override Texture2D Icon => ArtPower.Icon;
    public override Texture2D BigIcon => ArtPower.BigIcon;
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
}

public sealed class ClassicIroncladBerserkPower : ClassicIroncladPowerModel
{
    protected override PowerModel ArtPower => ModelDb.Power<EnergyNextTurnPower>();
    public override async Task AfterEnergyReset(Player player)
    {
        if (player == Owner.Player) await PlayerCmd.GainEnergy(Amount, player);
    }
}

public sealed class ClassicIroncladBrutalityPower : ClassicIroncladPowerModel
{
    protected override PowerModel ArtPower => ModelDb.Power<RupturePower>();
    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner.Player) return;
        await CreatureCmd.Damage(choiceContext, Owner, Amount, ValueProp.Unblockable | ValueProp.Unpowered, Owner);
        if (Owner.IsAlive) await CardPileCmd.Draw(choiceContext, Amount, player);
    }
}

public sealed class ClassicIroncladCombustPower : ClassicIroncladPowerModel
{
    protected override PowerModel ArtPower => ModelDb.Power<FlameBarrierPower>();
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("HpLoss", 0)];

    public override Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if (power == this && amount > 0) DynamicVars["HpLoss"].BaseValue++;
        return Task.CompletedTask;
    }

    public override async Task BeforeSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (!participants.Contains(Owner) || !CombatState.HittableEnemies.Any()) return;
        await CreatureCmd.Damage(choiceContext, Owner, DynamicVars["HpLoss"].BaseValue, ValueProp.Unblockable | ValueProp.Unpowered, Owner);
        if (Owner.IsAlive)
            await CreatureCmd.Damage(choiceContext, CombatState.HittableEnemies.ToList(), Amount, ValueProp.Unpowered, Owner);
    }
}

public sealed class ClassicIroncladDoubleTapPower : ClassicIroncladPowerModel
{
    protected override PowerModel ArtPower => ModelDb.Power<BurstPower>();
    public override int ModifyCardPlayCount(CardModel card, Creature? target, int playCount)
        => card.Owner == Owner.Player && card.Type == CardType.Attack ? playCount + 1 : playCount;
    public override async Task AfterModifyingCardPlayCount(CardModel card) => await PowerCmd.Decrement(this);
    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (participants.Contains(Owner)) await PowerCmd.Remove(this);
    }
}

public sealed class ClassicIroncladEvolvePower : ClassicIroncladPowerModel
{
    protected override PowerModel ArtPower => ModelDb.Power<DarkEmbracePower>();
    public override async Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)
    {
        if (card.Owner == Owner.Player && card.Type == CardType.Status)
            await CardPileCmd.Draw(choiceContext, Amount, Owner.Player);
    }
}

public sealed class ClassicIroncladFireBreathingPower : ClassicIroncladPowerModel
{
    protected override PowerModel ArtPower => ModelDb.Power<JuggernautPower>();
    public override async Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)
    {
        if (card.Owner == Owner.Player && (card.Type == CardType.Status || card.Type == CardType.Curse))
            await CreatureCmd.Damage(choiceContext, CombatState.HittableEnemies.ToList(), Amount, ValueProp.Unpowered, Owner);
    }
}

public sealed class ClassicIroncladLoseStrengthPower : ClassicIroncladPowerModel
{
    protected override PowerModel ArtPower => ModelDb.Power<StrengthPower>();
    public override PowerType Type => PowerType.Debuff;
    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (!participants.Contains(Owner)) return;
        await PowerCmd.Apply<StrengthPower>(choiceContext, Owner, -Amount, Owner, null);
        await PowerCmd.Remove(this);
    }
}

public sealed class ClassicIroncladMetallicizePower : ClassicIroncladPowerModel
{
    protected override PowerModel ArtPower => ModelDb.Power<FeelNoPainPower>();
    public override async Task BeforeSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (participants.Contains(Owner)) await CreatureCmd.GainBlock(Owner, Amount, ValueProp.Unpowered, null);
    }
}

public sealed class ClassicIroncladRupturePower : ClassicIroncladPowerModel
{
    protected override PowerModel ArtPower => ModelDb.Power<RupturePower>();
    public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (target == Owner && dealer == Owner && result.UnblockedDamage > 0)
            await PowerCmd.Apply<StrengthPower>(choiceContext, Owner, Amount, Owner, null);
    }
}
