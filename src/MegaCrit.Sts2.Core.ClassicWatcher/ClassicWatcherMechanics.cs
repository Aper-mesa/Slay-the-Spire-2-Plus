using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.ValueProps;

namespace MegaCrit.Sts2.Core.ClassicContent;

public enum ClassicWatcherStance { Neutral = 1, Calm, Wrath, Divinity }

public static class ClassicWatcherMechanics
{
    public static ClassicWatcherStance Stance(Player player) => (ClassicWatcherStance)(player.Creature.GetPower<ClassicWatcherStancePower>()?.Amount ?? 1);

    public static async Task ChangeStance(PlayerChoiceContext context, Player player, ClassicWatcherStance next, CardModel source = null)
    {
        var previous = Stance(player);
        if (previous == next || !player.Creature.IsAlive) return;
        var state = player.Creature.GetPower<ClassicWatcherStancePower>() ?? await PowerCmd.Apply<ClassicWatcherStancePower>(context, player.Creature, 1, player.Creature, source);
        if (state == null) return;
        state.SetAmount((int)next);
        if (previous == ClassicWatcherStance.Calm)
            await PlayerCmd.GainEnergy(player.GetRelic<ClassicWatcherVioletLotus>() is { IsMelted: false } ? 3 : 2, player);
        if (next == ClassicWatcherStance.Divinity) await PlayerCmd.GainEnergy(3, player);
        int block = player.Creature.GetPowerAmount<ClassicWatcherMentalFortressPower>();
        if (block > 0) await CreatureCmd.GainBlock(player.Creature, block, ValueProp.Unpowered, null);
        foreach (var card in PileType.Discard.GetPile(player).Cards.OfType<ClassicWatcherFlurryOfBlows>().ToList())
            await CardPileCmd.Add(card, PileType.Hand);
        if (next == ClassicWatcherStance.Wrath)
        {
            int draw = player.Creature.GetPowerAmount<ClassicWatcherRushdownPower>();
            if (draw > 0) await CardPileCmd.Draw(context, draw, player);
        }
    }

    public static async Task Mantra(PlayerChoiceContext context, Player player, decimal amount, CardModel source = null)
    {
        if (amount <= 0) return;
        await PowerCmd.Apply<ClassicWatcherMantraHistoryPower>(context, player.Creature, amount, player.Creature, source);
        var power = await PowerCmd.Apply<ClassicWatcherMantraPower>(context, player.Creature, amount, player.Creature, source);
        if (power == null) return;
        while (power.Amount >= 10)
        {
            await PowerCmd.ModifyAmount(context, power, -10, player.Creature, source);
            await ChangeStance(context, player, ClassicWatcherStance.Divinity, source);
        }
    }

    public static async Task Scry(PlayerChoiceContext context, Player player, int count)
    {
        if (player.GetRelic<ClassicWatcherGoldenEye>() is { IsMelted: false }) count += 2;
        int block = player.Creature.GetPowerAmount<ClassicWatcherNirvanaPower>();
        if (block > 0) await CreatureCmd.GainBlock(player.Creature, block, ValueProp.Unpowered, null);
        var top = PileType.Draw.GetPile(player).Cards.Take(count).ToList();
        if (top.Count == 0) return;
        var prefs = new CardSelectorPrefs(new LocString("cards", "CLASSIC_WATCHER.scryPrompt"), 0, top.Count);
        var selected = (await CardSelectCmd.FromSimpleGrid(context, top, player, prefs)).ToList();
        // Scry moves cards without firing manual-discard effects.
        if (selected.Count > 0) await CardPileCmd.Add(selected, PileType.Discard);
        foreach (var card in PileType.Discard.GetPile(player).Cards.OfType<ClassicWatcherWeave>().ToList())
            await CardPileCmd.Add(card, PileType.Hand);
    }

    public static async Task<T> Generate<T>(Player player, PileType pile = PileType.Hand, bool upgrade = false) where T : CardModel
    {
        var card = player.Creature.CombatState.CreateCard<T>(player);
        if (upgrade && card.IsUpgradable) CardCmd.Upgrade(card);
        await CardPileCmd.AddGeneratedCardToCombat(card, pile, player, pile == PileType.Draw ? CardPilePosition.Random : CardPilePosition.Bottom);
        return card;
    }
}
