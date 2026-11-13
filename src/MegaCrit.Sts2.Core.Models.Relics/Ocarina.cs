using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;

namespace MegaCrit.Sts2.Core.Models.Relics;

public sealed class Ocarina : RelicModel
{
	private static readonly CardTag[] _melodyTags = new CardTag[3]
	{
		CardTag.Wah,
		CardTag.Cho,
		CardTag.Ki
	};

	public override RelicRarity Rarity => RelicRarity.Uncommon;

	protected override IEnumerable<IHoverTip> ExtraHoverTips => new _003C_003Ez__ReadOnlyArray<IHoverTip>(new IHoverTip[3]
	{
		HoverTipFactory.Static(StaticHoverTip.WahMelody),
		HoverTipFactory.Static(StaticHoverTip.ChoMelody),
		HoverTipFactory.Static(StaticHoverTip.KiMelody)
	});

	public override async Task BeforeHandDraw(Player player, PlayerChoiceContext choiceContext, ICombatState combatState)
	{
		if (player == Owner && Owner.PlayerCombatState.TurnNumber <= 1)
		{
			List<CardModel> list = (from c in Owner.UnlockState.CharacterCardPools.SelectMany((CardPoolModel p) => p.GetUnlockedCards(Owner.UnlockState, Owner.RunState.CardMultiplayerConstraint))
				where c.Tags.Any(((IEnumerable<CardTag>)_melodyTags).Contains<CardTag>)
				select c).ToList();
			if (list.Count > 0)
			{
				List<CardModel> cards = CardFactory.GetDistinctForCombat(Owner, list, 1, Owner.RunState.Rng.CombatCardGeneration).ToList();
				Flash();
				await CardPileCmd.AddGeneratedCardsToCombat(cards, PileType.Hand, Owner);
			}
		}
	}
}
