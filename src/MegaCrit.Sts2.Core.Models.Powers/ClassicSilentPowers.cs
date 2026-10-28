using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.ValueProps;

namespace MegaCrit.Sts2.Core.Models.Powers;

public abstract class ClassicSilentPowerModel : PowerModel
{
    protected abstract PowerModel ArtPower { get; }
    public override Texture2D Icon => ArtPower.Icon;
    public override Texture2D BigIcon => ArtPower.BigIcon;
    public override PowerStackType StackType => PowerStackType.Counter;
}

public sealed class ClassicSilentThousandCutsPower : ClassicSilentPowerModel
{
    private class Data
    {
        public readonly Dictionary<CardModel, int> Played = new();
    }

    protected override PowerModel ArtPower => ModelDb.Power<AfterimagePower>();
    public override PowerType Type => PowerType.Buff;
    protected override object InitInternalData() => new Data();

    public override Task BeforeCardPlayed(CardPlay cardPlay)
    {
        if (cardPlay.Player == Owner.Player)
            GetInternalData<Data>().Played[cardPlay.Card] = Amount;
        return Task.CompletedTask;
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (GetInternalData<Data>().Played.Remove(cardPlay.Card, out int amount))
            await CreatureCmd.Damage(choiceContext, Owner.CombatState.HittableEnemies.ToList(), amount, ValueProp.Unpowered, Owner);
    }
}

public sealed class ClassicSilentChokePower : ClassicSilentPowerModel
{
    private class Data
    {
        public readonly Dictionary<CardModel, int> Played = new();
    }

    protected override PowerModel ArtPower => ModelDb.Power<StranglePower>();
    public override PowerType Type => PowerType.Debuff;
    public override PowerInstanceType InstanceType => PowerInstanceType.InstancedPerApplier;
    protected override object InitInternalData() => new Data();

    public override Task BeforeCardPlayed(CardPlay cardPlay)
    {
        if (Applier?.Player != null && cardPlay.Player == Applier.Player)
            GetInternalData<Data>().Played[cardPlay.Card] = Amount;
        return Task.CompletedTask;
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (Owner.IsAlive && GetInternalData<Data>().Played.Remove(cardPlay.Card, out int amount))
            await CreatureCmd.Damage(choiceContext, Owner, amount, ValueProp.Unblockable | ValueProp.Unpowered, Applier);
    }

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (side == Applier?.Side) await PowerCmd.Remove(this);
    }
}

public sealed class ClassicSilentCorpseExplosionPower : ClassicSilentPowerModel
{
    protected override PowerModel ArtPower => ModelDb.Power<PoisonPower>();
    public override PowerType Type => PowerType.Debuff;

    public override async Task AfterDeath(PlayerChoiceContext choiceContext, Creature creature, bool wasRemovalPrevented, float deathAnimLength)
    {
        if (creature != Owner || !creature.IsDead) return;
        var targets = Owner.CombatState.Creatures.Where(c => c.Side == Owner.Side).Where(c => c != Owner && c.IsAlive).ToList();
        await CreatureCmd.Damage(choiceContext, targets, Owner.MaxHp * Amount, ValueProp.Unpowered, Applier);
    }
}

public sealed class ClassicSilentPhantasmalPower : ClassicSilentPowerModel
{
    protected override PowerModel ArtPower => ModelDb.Power<DoubleDamagePower>();
    public override PowerType Type => PowerType.Buff;

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner.Player) return;
        await PowerCmd.Apply<DoubleDamagePower>(choiceContext, Owner, 1, Owner, null);
        await PowerCmd.Decrement(this);
    }
}

public sealed class ClassicSilentWellLaidPlansPower : ClassicSilentPowerModel
{
    protected override PowerModel ArtPower => ModelDb.Power<WellLaidPlansPower>();
    public override PowerType Type => PowerType.Buff;

    public override async Task BeforeFlush(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner.Player || Owner.HasPower<WellLaidPlansPower>() || Owner.HasPower<RetainHandPower>()) return;
        var prefs = new CardSelectorPrefs(new LocString("cards", "CLASSIC_SILENT_WELL_LAID_PLANS.selectionScreenPrompt"), 0, Amount);
        foreach (CardModel card in await CardSelectCmd.FromHandForRetain(choiceContext, player, prefs, this))
            CardCmd.ApplySingleTurnRetain(card);
    }
}

public sealed class ClassicSilentWraithFormPower : ClassicSilentPowerModel
{
    protected override PowerModel ArtPower => ModelDb.Power<WraithFormPower>();
    public override PowerType Type => PowerType.Debuff;

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (participants.Contains(Owner))
            await PowerCmd.Apply<DexterityPower>(choiceContext, Owner, -Amount, Owner, null);
    }
}
