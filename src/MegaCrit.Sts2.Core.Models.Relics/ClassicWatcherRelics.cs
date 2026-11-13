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
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace MegaCrit.Sts2.Core.Models.Relics;

public abstract class ClassicWatcherRelicModel : RelicModel
{
    private string Kind => GetType().Name.Substring(14);
    public override Texture2D Icon => ClassicWatcherAssets.Texture("relic_" + Kind);
    public override Texture2D IconOutline => ClassicWatcherAssets.Texture("outline_" + Kind);
    public override Texture2D BigIcon => ClassicWatcherAssets.Texture("large_" + Kind);
    protected override string IconBaseName => "ring_of_the_snake";
    public override bool HasUponPickupEffect => Kind == "HolyWater";
    public override async Task AfterObtained()
    {
        if (Kind == "HolyWater" && Owner.GetRelic<ClassicWatcherPureWater>() is { } pure) await RelicCmd.Remove(pure);
    }
    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext c, ICombatState combatState)
    {
        if (player != Owner) return;
        if (Kind == "Damaru") await ClassicWatcherMechanics.Mantra(c,player,1);
        if (player.PlayerCombatState.TurnNumber != 1) return;
        if (Kind is "PureWater" or "HolyWater")
            for(int i=0;i<(Kind == "HolyWater"?3:1);i++) await ClassicWatcherMechanics.Generate<ClassicWatcherMiracle>(player);
        if (Kind == "TeardropLocket") await ClassicWatcherMechanics.ChangeStance(c,player,ClassicWatcherStance.Calm);
    }
    public override async Task BeforeCardPlayed(CardPlay play)
    {
        if (Kind == "Duality" && play.Player == Owner && play.Card.Type == CardType.Attack)
        {
            var c = new ThrowingPlayerChoiceContext();
            await PowerCmd.Apply<DexterityPower>(c,Owner.Creature,1,Owner.Creature,null);
            await PowerCmd.Apply<ClassicWatcherLoseDexterityPower>(c,Owner.Creature,1,Owner.Creature,null);
        }
    }
    public override async Task BeforeSideTurnEnd(PlayerChoiceContext c, CombatSide side, IEnumerable<Creature> participants)
    {
        if (Kind == "CloakClasp" && participants.Contains(Owner.Creature))
            await CreatureCmd.GainBlock(Owner.Creature,PileType.Hand.GetPile(Owner).Cards.Count,ValueProp.Unpowered,null);
    }
    public override async Task AfterShuffle(PlayerChoiceContext c, Player player)
    {
        if (Kind == "Melange" && player == Owner) await ClassicWatcherMechanics.Scry(c,player,3);
    }
}
public sealed class ClassicWatcherPureWater : ClassicWatcherRelicModel { public override RelicRarity Rarity => RelicRarity.Starter; }
public sealed class ClassicWatcherHolyWater : ClassicWatcherRelicModel { public override RelicRarity Rarity => RelicRarity.Rare; }
public sealed class ClassicWatcherDamaru : ClassicWatcherRelicModel { public override RelicRarity Rarity => RelicRarity.Common; }
public sealed class ClassicWatcherTeardropLocket : ClassicWatcherRelicModel { public override RelicRarity Rarity => RelicRarity.Uncommon; }
public sealed class ClassicWatcherDuality : ClassicWatcherRelicModel { public override RelicRarity Rarity => RelicRarity.Uncommon; }
public sealed class ClassicWatcherGoldenEye : ClassicWatcherRelicModel { public override RelicRarity Rarity => RelicRarity.Rare; }
public sealed class ClassicWatcherVioletLotus : ClassicWatcherRelicModel { public override RelicRarity Rarity => RelicRarity.Rare; }
public sealed class ClassicWatcherMelange : ClassicWatcherRelicModel { public override RelicRarity Rarity => RelicRarity.Shop; }
public sealed class ClassicWatcherCloakClasp : ClassicWatcherRelicModel { public override RelicRarity Rarity => RelicRarity.Rare; }
