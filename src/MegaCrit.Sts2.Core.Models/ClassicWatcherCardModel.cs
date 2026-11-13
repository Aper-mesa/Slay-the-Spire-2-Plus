using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.ClassicContent;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace MegaCrit.Sts2.Core.Models;

public abstract class ClassicWatcherCardModel : CardModel
{
    private readonly decimal _damage, _block, _magic;
    private readonly int _ud, _ub, _um, _uc;
    private readonly CardKeyword[] _keywords;
    protected string Key => GetType().Name.Substring("ClassicWatcher".Length);
    protected ClassicWatcherCardModel(int cost, CardType type, CardRarity rarity, TargetType target, int damage, int block, int magic, int ud, int ub, int um, int uc, params CardKeyword[] keywords)
        : base(Math.Max(-1, cost), type, rarity, target)
    { _damage = damage; _block = block; _magic = magic; _ud = ud; _ub = ub; _um = um; _uc = uc; _keywords = keywords; }
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(_damage, ValueProp.Move), new BlockVar(_block, ValueProp.Move), new DynamicVar("MagicNumber", _magic)];
    public override IEnumerable<CardKeyword> CanonicalKeywords => _keywords;
    protected override IEnumerable<IHoverTip> ExtraHoverTips
    {
        get
        {
            if (Key is "Eruption" or "Vigilance" or "Tranquility" or "Crescendo" or "Tantrum" or "FearNoEvil" or "InnerPeace" or "Indignation" or "Meditate" or "Blasphemy" or "EmptyFist" or "EmptyBody" or "EmptyMind" or "Halt" or "Rushdown" or "LikeWater" or "MentalFortress" or "SimmeringFury")
                yield return HoverTipFactory.FromPower<ClassicWatcherStancePower>();
            if (Key is "CutThroughFate" or "JustLucky" or "ThirdEye" or "Foresight" or "Weave" or "Nirvana")
                yield return new HoverTip(new LocString("cards","CLASSIC_WATCHER.scryTitle"),new LocString("cards","CLASSIC_WATCHER.scryDescription"));
            if (Key is "Devotion" or "Worship" or "Pray" or "Prostrate" or "Brilliance")
                yield return HoverTipFactory.FromPower<ClassicWatcherMantraPower>();
            if (Key == "Alpha") yield return HoverTipFactory.FromCard<ClassicWatcherBeta>();
            if (Key == "Beta") yield return HoverTipFactory.FromCard<ClassicWatcherOmega>();
            if (Key == "CarveReality") yield return HoverTipFactory.FromCard<ClassicWatcherSmite>();
            if (Key == "BattleHymn") yield return HoverTipFactory.FromCard<ClassicWatcherSmite>();
            if (Key == "DeceiveReality") yield return HoverTipFactory.FromCard<ClassicWatcherSafety>();
            if (Key == "Evaluate") yield return HoverTipFactory.FromCard<ClassicWatcherInsight>();
            if (Key == "Study") yield return HoverTipFactory.FromCard<ClassicWatcherInsight>();
            if (Key == "Pray") yield return HoverTipFactory.FromCard<ClassicWatcherInsight>();
            if (Key == "ReachHeaven") yield return HoverTipFactory.FromCard<ClassicWatcherThroughViolence>();
            if (Key == "Collect") yield return HoverTipFactory.FromCard<ClassicWatcherMiracle>();
            if (Key == "DeusExMachina") yield return HoverTipFactory.FromCard<ClassicWatcherMiracle>();
            if (Key == "ConjureBlade") yield return HoverTipFactory.FromCard<ClassicWatcherExpunger>();
            if (Key == "PressurePoints") yield return HoverTipFactory.FromPower<ClassicWatcherMarkPower>();
            if (Key == "TalkToTheHand") yield return HoverTipFactory.FromPower<ClassicWatcherTalkToTheHandPower>();
            if (Key == "Wish") yield return HoverTipFactory.FromPower<ClassicWatcherPlatedArmorPower>();
        }
    }
    public override Texture2D Portrait => ClassicWatcherAssets.Texture(Key);
    public override string PortraitPath => MissingPortraitPath;
    public override string BetaPortraitPath => MissingPortraitPath;
    protected override string PortraitPngPath => MissingPortraitPath;
    public override bool GainsBlock => _block > 0 || Key is "SpiritShield" or "Wallop";
    protected override bool HasEnergyCostX => Key is "Collect" or "ConjureBlade";
    public override bool CanBeGeneratedInCombat => Rarity != CardRarity.Token && Key is not ("Wish" or "LessonLearned");
    protected override HashSet<CardTag> CanonicalTags => Key == "Strike" ? [CardTag.Strike] : Key == "Defend" ? [CardTag.Defend] : [];
    protected override bool IsPlayable => Key != "SignatureMove" || !PileType.Hand.GetPile(Owner).Cards.Any(c => c != this && c.Type == CardType.Attack);
    private decimal D => DynamicVars.Damage.BaseValue;
    private decimal B => DynamicVars.Block.BaseValue;
    private int M => DynamicVars["MagicNumber"].IntValue;
    private bool In(ClassicWatcherStance stance) => ClassicWatcherMechanics.Stance(Owner) == stance;
    private Task Stance(PlayerChoiceContext c, ClassicWatcherStance s) => ClassicWatcherMechanics.ChangeStance(c, Owner, s, this);
    private Task Mantra(PlayerChoiceContext c, int n) => ClassicWatcherMechanics.Mantra(c, Owner, n, this);
    private Task Scry(PlayerChoiceContext c) => ClassicWatcherMechanics.Scry(c, Owner, M);
    private Task Draw(PlayerChoiceContext c, int n) => CardPileCmd.Draw(c, n, Owner);
    private Task Block(CardPlay play, decimal? amount = null) => CreatureCmd.GainBlock(Owner.Creature, amount ?? B, ValueProp.Move, play);
    private Task Buff<T>(PlayerChoiceContext c, decimal n, Creature target = null) where T : PowerModel => PowerCmd.Apply<T>(c, target ?? Owner.Creature, n, Owner.Creature, this);
    private Task Token<T>(PileType pile = PileType.Hand, bool upgrade = false) where T : CardModel => ClassicWatcherMechanics.Generate<T>(Owner, pile, upgrade);
    private Task Attack(PlayerChoiceContext c, CardPlay play, int hits = 1, bool all = false, bool random = false)
    {
        var attack = DamageCmd.Attack(D).FromCard(this, play).WithHitCount(hits).WithHitFx("vfx/vfx_attack_blunt");
        if (all) attack.TargetingAllOpponents(CombatState);
        else if (random) attack.TargetingRandomOpponents(CombatState);
        else attack.Targeting(play.Target);
        return attack.Execute(c);
    }
    private bool Previous(CardType type) => CombatManager.Instance.History.CardPlaysFinished.LastOrDefault(e => e.CardPlay.Player == Owner)?.CardPlay.Card.Type == type;
    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(_ud);
        DynamicVars.Block.UpgradeValueBy(_ub);
        DynamicVars["MagicNumber"].UpgradeValueBy(_um);
        if (_uc != 0) EnergyCost.UpgradeBy(_uc);
        if (Key is "Alpha" or "BattleHymn" or "Brutality" or "Establishment") AddKeyword(CardKeyword.Innate);
        if (Key is "Blasphemy" or "Worship") AddKeyword(CardKeyword.Retain);
        if (Key == "DevaForm") RemoveKeyword(CardKeyword.Ethereal);
    }
    public override async Task AfterCardDrawn(PlayerChoiceContext c, CardModel card, bool fromHandDraw)
    {
        if (card == this && Key == "DeusExMachina")
        {
            await CardCmd.Exhaust(c, this);
            for (int i = 0; i < M; i++) await Token<ClassicWatcherMiracle>();
        }
    }
    public override Task AfterFlush(PlayerChoiceContext c, Player player, IReadOnlyCollection<CardModel> flushed, IReadOnlyCollection<CardModel> retained)
    {
        if (player == Owner && retained.Contains(this) && ShouldRetainThisTurn)
        {
            if (Key == "Perseverance") DynamicVars.Block.BaseValue += M;
            if (Key == "WindmillStrike") DynamicVars.Damage.BaseValue += M;
            if (Key == "SandsOfTime") EnergyCost.AddThisCombat(-1);
        }
        return Task.CompletedTask;
    }
    public override decimal ModifyDamageAdditive(Creature target, decimal amount, ValueProp props, Creature dealer, CardModel cardSource, CardPlay cardPlay)
        => Key == "Brilliance" && cardSource == this && props.IsPoweredAttack() ? Owner.Creature.GetPowerAmount<ClassicWatcherMantraHistoryPower>() : 0;
    public override CardLocation ModifyCardPlayResultLocation(CardModel card, bool isAutoPlay, ResourceInfo resources, CardLocation location)
    {
        if (card == this && Key == "Tantrum" && location.pileType == PileType.Discard)
        { location.pileType = PileType.Draw; location.position = CardPilePosition.Random; }
        return location;
    }
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        switch (Key)
        {
            case "Alpha":
            {
                await Token<ClassicWatcherBeta>(PileType.Draw);
                break;
            }
            case "BattleHymn":
            {
                await Buff<ClassicWatcherBattleHymnPower>(c,M);
                break;
            }
            case "Blasphemy":
            {
                await Stance(c,ClassicWatcherStance.Divinity); await Buff<ClassicWatcherBlasphemyPower>(c,1);
                break;
            }
            case "BowlingBash":
            {
                await Attack(c,p,CombatState.HittableEnemies.Count);
                break;
            }
            case "Brilliance":
            {
                await Attack(c, p);
                break;
            }
            case "CarveReality":
            {
                await Attack(c,p); await Token<ClassicWatcherSmite>();
                break;
            }
            case "Collect":
            {
                await Buff<ClassicWatcherCollectPower>(c,ResolveEnergyXValue() + (IsUpgraded ? 1 : 0));
                break;
            }
            case "Conclude":
            {
                await Attack(c,p,all:true); PlayerCmd.EndTurn(Owner,canBackOut:false);
                break;
            }
            case "ConjureBlade":
            {
                var card = CombatState.CreateCard<ClassicWatcherExpunger>(Owner); card.DynamicVars["MagicNumber"].BaseValue = ResolveEnergyXValue() + (IsUpgraded ? 1 : 0); await CardPileCmd.AddGeneratedCardToCombat(card,PileType.Draw,Owner,CardPilePosition.Random);
                break;
            }
            case "Consecrate":
            {
                await Attack(c,p,all:true);
                break;
            }
            case "Crescendo":
            {
                await Stance(c,ClassicWatcherStance.Wrath);
                break;
            }
            case "CrushJoints":
            {
                bool skill = Previous(CardType.Skill); await Attack(c,p); if (skill) await Buff<VulnerablePower>(c,M,p.Target);
                break;
            }
            case "CutThroughFate":
            {
                await Attack(c,p); await Scry(c); await Draw(c,1);
                break;
            }
            case "DeceiveReality":
            {
                await Block(p); await Token<ClassicWatcherSafety>();
                break;
            }
            case "Defend":
            {
                await Block(p);
                break;
            }
            case "DeusExMachina":
            {
                break;
            }
            case "DevaForm":
            {
                await Buff<ClassicWatcherDevaFormPower>(c,1);
                break;
            }
            case "Devotion":
            {
                await Buff<ClassicWatcherDevotionPower>(c,M);
                break;
            }
            case "EmptyBody":
            {
                await Block(p); await Stance(c,ClassicWatcherStance.Neutral);
                break;
            }
            case "EmptyFist":
            {
                await Attack(c, p);
                await Stance(c, ClassicWatcherStance.Neutral);
                break;
            }
            case "EmptyMind":
            {
                await Draw(c,M); await Stance(c,ClassicWatcherStance.Neutral);
                break;
            }
            case "Eruption":
            {
                await Attack(c,p); await Stance(c,ClassicWatcherStance.Wrath);
                break;
            }
            case "Establishment":
            {
                await Buff<ClassicWatcherEstablishmentPower>(c,M);
                break;
            }
            case "Evaluate":
            {
                await Block(p); await Token<ClassicWatcherInsight>(PileType.Draw);
                break;
            }
            case "Fasting":
            {
                await Buff<StrengthPower>(c,M); await Buff<DexterityPower>(c,M); await Buff<ClassicWatcherFastingPower>(c,1);
                break;
            }
            case "FearNoEvil":
            {
                bool attacking = p.Target.Monster?.IntendsToAttack == true; await Attack(c,p); if (attacking) await Stance(c,ClassicWatcherStance.Calm);
                break;
            }
            case "FlurryOfBlows":
            {
                await Attack(c, p);
                break;
            }
            case "FlyingSleeves":
            {
                await Attack(c,p,2);
                break;
            }
            case "FollowUp":
            {
                bool attack = Previous(CardType.Attack); await Attack(c,p); if (attack) await PlayerCmd.GainEnergy(1,Owner);
                break;
            }
            case "ForeignInfluence":
            {
                var pool = ModelDb.CardPool<ClassicIroncladCardPool>().AllCards.Concat(ModelDb.CardPool<ClassicSilentCardPool>().AllCards).Concat(ModelDb.CardPool<ClassicDefectCardPool>().AllCards).Concat(ModelDb.CardPool<ClassicWatcherCardPool>().AllCards);
                var choices = CardFactory.GetDistinctForCombat(Owner,pool.Where(x=>x.Type == CardType.Attack),3,Owner.RunState.Rng.CombatCardGeneration).ToList();
                var chosen = await CardSelectCmd.FromChooseACardScreen(c,choices,Owner,canSkip:false);
                if (chosen != null) { if(IsUpgraded) chosen.SetToFreeThisTurn(); await CardPileCmd.AddGeneratedCardToCombat(chosen,PileType.Hand,Owner); }
                break;
            }
            case "Foresight":
            {
                await Buff<ClassicWatcherForesightPower>(c,M);
                break;
            }
            case "Halt":
            {
                if (!In(ClassicWatcherStance.Wrath)) await Block(p);
                else
                {
                    var first = MegaCrit.Sts2.Core.Hooks.Hook.ModifyBlock(CombatState,Owner.Creature,B,ValueProp.Move,this,p,out _);
                    var extra = MegaCrit.Sts2.Core.Hooks.Hook.ModifyBlock(CombatState,Owner.Creature,M,ValueProp.Move,this,p,out _);
                    await CreatureCmd.GainBlock(Owner.Creature,Math.Max(0,Math.Floor(first))+Math.Max(0,Math.Floor(extra)),ValueProp.Unpowered,p);
                }
                break;
            }
            case "Indignation":
            {
                if (In(ClassicWatcherStance.Wrath)) { foreach (var enemy in CombatState.HittableEnemies.ToList()) await Buff<VulnerablePower>(c,M,enemy); } else await Stance(c,ClassicWatcherStance.Wrath);
                break;
            }
            case "InnerPeace":
            {
                if (In(ClassicWatcherStance.Calm)) await Draw(c,M); else await Stance(c,ClassicWatcherStance.Calm);
                break;
            }
            case "Judgement":
            {
                if (p.Target.CurrentHp <= M) await CreatureCmd.Kill(p.Target);
                break;
            }
            case "JustLucky":
            {
                await Scry(c); await Block(p); await Attack(c,p);
                break;
            }
            case "LessonLearned":
            {
                bool fatalAllowed = p.Target.Powers.All(x=>x.ShouldOwnerDeathTriggerFatal());
                var hit = await DamageCmd.Attack(D).FromCard(this,p).Targeting(p.Target).WithHitFx("vfx/vfx_attack_blunt").Execute(c);
                if (fatalAllowed && hit.Results.SelectMany(x=>x).Any(x=>x.WasTargetKilled))
                {
                    var chosen = Owner.RunState.Rng.CombatCardSelection.NextItem(PileType.Deck.GetPile(Owner).Cards.Where(x=>x.IsUpgradable));
                    if (chosen != null) CardCmd.Upgrade(chosen);
                }
                break;
            }
            case "LikeWater":
            {
                await Buff<ClassicWatcherLikeWaterPower>(c,M);
                break;
            }
            case "MasterReality":
            {
                await Buff<ClassicWatcherMasterRealityPower>(c,1);
                break;
            }
            case "Meditate":
            {
                var cards = await CardSelectCmd.FromCombatPile(c,PileType.Discard.GetPile(Owner),Owner,new CardSelectorPrefs(SelectionScreenPrompt,Math.Min(M,PileType.Discard.GetPile(Owner).Cards.Count)));
                foreach (var card in cards) { CardCmd.ApplySingleTurnRetain(card); await CardPileCmd.Add(card,PileType.Hand); }
                await Stance(c,ClassicWatcherStance.Calm); PlayerCmd.EndTurn(Owner,canBackOut:false);
                break;
            }
            case "MentalFortress":
            {
                await Buff<ClassicWatcherMentalFortressPower>(c,M);
                break;
            }
            case "Nirvana":
            {
                await Buff<ClassicWatcherNirvanaPower>(c,M);
                break;
            }
            case "Omniscience":
            {
                var chosen = (await CardSelectCmd.FromCombatPile(c,PileType.Draw.GetPile(Owner),Owner,new CardSelectorPrefs(SelectionScreenPrompt,1))).FirstOrDefault();
                if (chosen != null)
                {
                    var repeat = await PowerCmd.Apply<ClassicWatcherOmnisciencePower>(c,Owner.Creature,1,Owner.Creature,this);
                    if (repeat != null) repeat.Chosen = chosen;
                    chosen.AddKeyword(CardKeyword.Exhaust);
                    await CardCmd.AutoPlay(c,chosen,null);
                    if (repeat != null && Owner.Creature.HasPower<ClassicWatcherOmnisciencePower>()) await PowerCmd.Remove(repeat);
                }
                break;
            }
            case "Perseverance":
            {
                await Block(p);
                break;
            }
            case "Pray":
            {
                await Mantra(c,M); await Token<ClassicWatcherInsight>(PileType.Draw);
                break;
            }
            case "PressurePoints":
            {
                await Buff<ClassicWatcherMarkPower>(c,M,p.Target);
                foreach (var enemy in CombatState.HittableEnemies.ToList())
                    if (enemy.GetPower<ClassicWatcherMarkPower>() is { } mark)
                        await CreatureCmd.Damage(c,enemy,mark.Amount,ValueProp.Unblockable | ValueProp.Unpowered,Owner.Creature);
                break;
            }
            case "Prostrate":
            {
                await Mantra(c,M); await Block(p);
                break;
            }
            case "Protect":
            {
                await Block(p);
                break;
            }
            case "Ragnarok":
            {
                await Attack(c,p,M,random:true);
                break;
            }
            case "ReachHeaven":
            {
                await Attack(c,p); await Token<ClassicWatcherThroughViolence>(PileType.Draw);
                break;
            }
            case "Rushdown":
            {
                await Buff<ClassicWatcherRushdownPower>(c,M);
                break;
            }
            case "Sanctity":
            {
                bool skill = Previous(CardType.Skill); await Block(p); if (skill) await Draw(c,M);
                break;
            }
            case "SandsOfTime":
            {
                await Attack(c, p);
                break;
            }
            case "SashWhip":
            {
                bool attack = Previous(CardType.Attack); await Attack(c,p); if (attack) await Buff<WeakPower>(c,M,p.Target);
                break;
            }
            case "Scrawl":
            {
                await Draw(c,Math.Max(0,CardPile.MaxCardsInHand - PileType.Hand.GetPile(Owner).Cards.Count));
                break;
            }
            case "SignatureMove":
            {
                await Attack(c, p);
                break;
            }
            case "SimmeringFury":
            {
                await Buff<ClassicWatcherWrathNextTurnPower>(c,1); await Buff<DrawCardsNextTurnPower>(c,M);
                break;
            }
            case "SpiritShield":
            {
                await Block(p, PileType.Hand.GetPile(Owner).Cards.Count * M);
                break;
            }
            case "Strike":
            {
                await Attack(c, p);
                break;
            }
            case "Study":
            {
                await Buff<ClassicWatcherStudyPower>(c,M);
                break;
            }
            case "Swivel":
            {
                await Block(p); await Buff<FreeAttackPower>(c,1);
                break;
            }
            case "TalkToTheHand":
            {
                await Attack(c,p); await Buff<ClassicWatcherTalkToTheHandPower>(c,M,p.Target);
                break;
            }
            case "Tantrum":
            {
                await Attack(c,p,M); await Stance(c,ClassicWatcherStance.Wrath);
                break;
            }
            case "ThirdEye":
            {
                await Block(p); await Scry(c);
                break;
            }
            case "Tranquility":
            {
                await Stance(c,ClassicWatcherStance.Calm);
                break;
            }
            case "Vault":
            {
                await Buff<AmbergrisPower>(c,1); PlayerCmd.EndTurn(Owner,canBackOut:false);
                break;
            }
            case "Vigilance":
            {
                await Block(p); await Stance(c,ClassicWatcherStance.Calm);
                break;
            }
            case "Wallop":
            {
                var hit = await DamageCmd.Attack(D).FromCard(this,p).Targeting(p.Target).WithHitFx("vfx/vfx_attack_blunt").Execute(c);
                int block = hit.Results.SelectMany(x=>x).Sum(x=>x.UnblockedDamage - x.OverkillDamage);
                if (block > 0) await CreatureCmd.GainBlock(Owner.Creature,block,ValueProp.Unpowered,p);
                break;
            }
            case "WaveOfTheHand":
            {
                await Buff<ClassicWatcherWaveOfTheHandPower>(c,M);
                break;
            }
            case "Weave":
            {
                await Attack(c, p);
                break;
            }
            case "WheelKick":
            {
                await Attack(c,p); await Draw(c,M);
                break;
            }
            case "WindmillStrike":
            {
                await Attack(c, p);
                break;
            }
            case "Wish":
            {
                var choices = new List<CardModel> { CombatState.CreateCard<ClassicWatcherBecomeAlmighty>(Owner), CombatState.CreateCard<ClassicWatcherFameAndFortune>(Owner), CombatState.CreateCard<ClassicWatcherLiveForever>(Owner) };
                if (IsUpgraded) foreach (var card in choices) CardCmd.Upgrade(card);
                var chosen = await CardSelectCmd.FromChooseACardScreen(c,choices,Owner,canSkip:false);
                if (chosen is ClassicWatcherBecomeAlmighty) await Buff<StrengthPower>(c,IsUpgraded?4:3);
                else if (chosen is ClassicWatcherFameAndFortune) await PlayerCmd.GainGold(IsUpgraded?30:25,Owner);
                else if (chosen != null) await Buff<ClassicWatcherPlatedArmorPower>(c,IsUpgraded?8:6);
                break;
            }
            case "Worship":
            {
                await Mantra(c,M);
                break;
            }
            case "WreathOfFlame":
            {
                await Buff<VigorPower>(c,M);
                break;
            }
            case "Miracle":
            {
                await PlayerCmd.GainEnergy(IsUpgraded ? 2 : 1,Owner);
                break;
            }
            case "Smite":
            {
                await Attack(c, p);
                break;
            }
            case "Safety":
            {
                await Block(p);
                break;
            }
            case "Insight":
            {
                await Draw(c,M);
                break;
            }
            case "Beta":
            {
                await Token<ClassicWatcherOmega>(PileType.Draw);
                break;
            }
            case "Omega":
            {
                await Buff<ClassicWatcherOmegaPower>(c,M);
                break;
            }
            case "Expunger":
            {
                await Attack(c,p,M);
                break;
            }
            case "ThroughViolence":
            {
                await Attack(c, p);
                break;
            }
            case "BecomeAlmighty":
            {
                await Buff<StrengthPower>(c,M);
                break;
            }
            case "FameAndFortune":
            {
                await PlayerCmd.GainGold(M,Owner);
                break;
            }
            case "LiveForever":
            {
                await Buff<ClassicWatcherPlatedArmorPower>(c,M);
                break;
            }
            default: throw new InvalidOperationException(Key);
        }
    }
}
