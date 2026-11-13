using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using MegaCrit.Sts2.Core.ClassicContent;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;

namespace MegaCrit.Sts2.Core.Models.Powers;

public abstract class ClassicWatcherPowerModel : PowerModel
{
    protected string Kind => GetType().Name.Substring(14).Replace("Power", "");
    public override PowerType Type => Kind is "Mark" or "TalkToTheHand" or "Fasting" or "LoseDexterity" ? PowerType.Debuff : PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    protected override bool IsVisibleInternal => Kind is not ("MantraHistory" or "Omniscience") && (Kind != "Stance" || Amount > 1);
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Energy",0)];
    public override Texture2D Icon => ClassicWatcherAssets.Texture("power48_" + Art);
    public override Texture2D BigIcon => ClassicWatcherAssets.Texture("power128_" + Art);
    private string Art => Kind switch
    {
        "Devotion" => "devotion", "Mantra" or "MantraHistory" or "Stance" => "mantra",
        "DevaForm" => "deva", "Fasting" => "fasting", "Establishment" => "establishment",
        "MentalFortress" => "mental_fortress", "Nirvana" => "nirvana", "Rushdown" => "rushdown",
        "MasterReality" => "master_reality", "Omega" => "omega", "WaveOfTheHand" => "wave_of_the_hand",
        "PlatedArmor" => "platedarmor", "BattleHymn" => "hymn", "Collect" => "energized_blue",
        "Foresight" => "wireheading", "LikeWater" => "like_water", "Study" => "draw",
        "WrathNextTurn" => "anger", "Blasphemy" => "end_turn_death", "Mark" => "pressure_points",
        "TalkToTheHand" => "talk_to_hand", _ => "strength"
    };
    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext c, ICombatState combatState)
    {
        if (player != Owner.Player) return;
        switch (Kind)
        {

            case "BattleHymn": for(int i=0;i<Amount;i++) await ClassicWatcherMechanics.Generate<ClassicWatcherSmite>(player); break;
            case "Collect": await ClassicWatcherMechanics.Generate<ClassicWatcherMiracle>(player,upgrade:true); await PowerCmd.Decrement(this); break;
            case "Devotion": await ClassicWatcherMechanics.Mantra(c,player,Amount); break;
            case "Foresight": await CardPileCmd.ShuffleIfNecessary(c,player); await ClassicWatcherMechanics.Scry(c,player,Amount); break;
            case "WrathNextTurn": await ClassicWatcherMechanics.ChangeStance(c,player,ClassicWatcherStance.Wrath); await PowerCmd.Remove(this); break;
            case "Blasphemy": await CreatureCmd.Damage(c,Owner,99999,ValueProp.Unblockable | ValueProp.Unpowered,Owner); await PowerCmd.Remove(this); break;
        }
    }
    public override async Task AfterEnergyReset(Player player)
    {
        if (Kind == "DevaForm" && player == Owner.Player)
        {
            DynamicVars["Energy"].BaseValue += Amount;
            await PlayerCmd.GainEnergy(DynamicVars["Energy"].BaseValue,player);
        }
    }
    public override decimal ModifyMaxEnergy(Player player, decimal amount) => Kind == "Fasting" && player == Owner.Player ? amount - Amount : amount;
    public override async Task BeforeSideTurnEnd(PlayerChoiceContext c, CombatSide side, IEnumerable<Creature> participants)
    {
        if (!participants.Contains(Owner)) return;
        if (Kind == "LikeWater" && ClassicWatcherMechanics.Stance(Owner.Player) == ClassicWatcherStance.Calm || Kind == "PlatedArmor")
            await CreatureCmd.GainBlock(Owner,Amount,ValueProp.Unpowered,null);
        if (Kind == "Omega") await CreatureCmd.Damage(c,CombatState.GetOpponentsOf(Owner).Where(x=>x.IsAlive).ToList(),Amount,ValueProp.Unpowered,Owner);
        if (Kind == "Study") for(int i=0;i<Amount;i++) await ClassicWatcherMechanics.Generate<ClassicWatcherInsight>(Owner.Player,PileType.Draw);
    }
    public override async Task BeforeSideTurnStart(PlayerChoiceContext c, CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (Kind == "Stance" && Amount == (int)ClassicWatcherStance.Divinity && participants.Contains(Owner))
            await ClassicWatcherMechanics.ChangeStance(c,Owner.Player,ClassicWatcherStance.Neutral);
    }
    public override async Task AfterSideTurnEnd(PlayerChoiceContext c, CombatSide side, IEnumerable<Creature> participants)
    {
        if (Kind == "WaveOfTheHand" && participants.Contains(Owner)) await PowerCmd.Remove(this);
        if (Kind == "LoseDexterity" && participants.Contains(Owner))
        { await PowerCmd.Apply<DexterityPower>(c,Owner,-Amount,Owner,null); await PowerCmd.Remove(this); }
    }
    public override async Task AfterBlockGained(Creature creature, decimal amount, ValueProp props, CardModel cardSource)
    {
        if (Kind == "WaveOfTheHand" && creature == Owner && amount > 0)
            await PowerCmd.Apply<WeakPower>(new ThrowingPlayerChoiceContext(),CombatState.GetOpponentsOf(Owner).Where(x=>x.IsAlive).ToList(),Amount,Owner,null);
    }
    public override async Task AfterDamageReceived(PlayerChoiceContext c, Creature target, DamageResult result, ValueProp props, Creature dealer, CardModel cardSource)
    {
        if (Kind == "PlatedArmor" && target == Owner && dealer != Owner && props.IsPoweredAttack() && result.UnblockedDamage > 0)
            await PowerCmd.Decrement(this);
    }
    public override async Task AfterDamageGiven(PlayerChoiceContext c, Creature dealer, DamageResult result, ValueProp props, Creature target, CardModel cardSource)
    {
        if (Kind == "TalkToTheHand" && target == Owner && dealer?.Player != null && dealer.Side != Owner.Side && props.IsPoweredAttack())
            await CreatureCmd.GainBlock(dealer,Amount,ValueProp.Unpowered,null);
    }
    public override Task AfterCardGeneratedForCombat(CardModel card, Player creator)
    {
        if (Kind == "MasterReality" && card.Owner == Owner.Player && card.IsUpgradable) CardCmd.Upgrade(card);
        return Task.CompletedTask;
    }
    public override Task AfterFlush(PlayerChoiceContext c, Player player, IReadOnlyCollection<CardModel> flushed, IReadOnlyCollection<CardModel> retained)
    {
        if (Kind == "Establishment" && player == Owner.Player)
            foreach(var card in retained.Where(x=>x.ShouldRetainThisTurn && !x.EnergyCost.CostsX)) card.EnergyCost.AddThisCombat(-Amount);
        return Task.CompletedTask;
    }
}

public sealed class ClassicWatcherStancePower : ClassicWatcherPowerModel
{
    public override LocString Title => new("powers", "CLASSIC_WATCHER_STANCE_POWER." + Amount + ".title");
    public override LocString Description => new("powers", "CLASSIC_WATCHER_STANCE_POWER." + Amount + ".description");
    public override decimal ModifyDamageMultiplicative(Creature target, decimal amount, ValueProp props, Creature dealer, CardModel cardSource, CardPlay cardPlay)
    {
        if (!props.IsPoweredAttack()) return 1;
        decimal factor = 1;
        if (dealer == Owner) factor = Amount == 3 ? 2 : Amount == 4 ? 3 : 1;
        if (target == Owner && Amount == 3) factor *= 2;
        return factor;
    }
}
public sealed class ClassicWatcherOmnisciencePower : ClassicWatcherPowerModel
{
    public CardModel Chosen { get; set; }
    public override int ModifyCardPlayCount(CardModel card, Creature target, int count) => card == Chosen ? count + 1 : count;
    public override async Task AfterModifyingCardPlayCount(CardModel card) { if (card == Chosen) await PowerCmd.Remove(this); }
}
public sealed class ClassicWatcherBattleHymnPower : ClassicWatcherPowerModel { }
public sealed class ClassicWatcherCollectPower : ClassicWatcherPowerModel { }
public sealed class ClassicWatcherDevotionPower : ClassicWatcherPowerModel { }
public sealed class ClassicWatcherForesightPower : ClassicWatcherPowerModel { }
public sealed class ClassicWatcherWrathNextTurnPower : ClassicWatcherPowerModel { }
public sealed class ClassicWatcherBlasphemyPower : ClassicWatcherPowerModel { }
public sealed class ClassicWatcherDevaFormPower : ClassicWatcherPowerModel { }
public sealed class ClassicWatcherFastingPower : ClassicWatcherPowerModel { }
public sealed class ClassicWatcherLikeWaterPower : ClassicWatcherPowerModel { }
public sealed class ClassicWatcherOmegaPower : ClassicWatcherPowerModel { }
public sealed class ClassicWatcherStudyPower : ClassicWatcherPowerModel { }
public sealed class ClassicWatcherWaveOfTheHandPower : ClassicWatcherPowerModel { }
public sealed class ClassicWatcherPlatedArmorPower : ClassicWatcherPowerModel { }
public sealed class ClassicWatcherTalkToTheHandPower : ClassicWatcherPowerModel { }
public sealed class ClassicWatcherMasterRealityPower : ClassicWatcherPowerModel { }
public sealed class ClassicWatcherEstablishmentPower : ClassicWatcherPowerModel { }
public sealed class ClassicWatcherMentalFortressPower : ClassicWatcherPowerModel { }
public sealed class ClassicWatcherNirvanaPower : ClassicWatcherPowerModel { }
public sealed class ClassicWatcherRushdownPower : ClassicWatcherPowerModel { }
public sealed class ClassicWatcherMantraPower : ClassicWatcherPowerModel { }
public sealed class ClassicWatcherMantraHistoryPower : ClassicWatcherPowerModel { }
public sealed class ClassicWatcherMarkPower : ClassicWatcherPowerModel { }
public sealed class ClassicWatcherLoseDexterityPower : ClassicWatcherPowerModel { }
