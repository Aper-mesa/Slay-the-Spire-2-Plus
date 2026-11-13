using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace MegaCrit.Sts2.Core.Models.Potions;

public sealed class BottledSong : PotionModel
{
	public override PotionRarity Rarity => PotionRarity.Uncommon;

	public override PotionUsage Usage => PotionUsage.CombatOnly;

	public override TargetType TargetType => TargetType.AnyPlayer;

	protected override async Task OnUse(PlayerChoiceContext choiceContext, Creature? target)
	{
		PotionModel.AssertValidForTargetedPotion(target);
		Player targetPlayer = target.Player;
		CardTag[] melodyTags = new CardTag[3]
		{
			CardTag.Wah,
			CardTag.Cho,
			CardTag.Ki
		};
		List<CardPoolModel> source = targetPlayer.UnlockState.CharacterCardPools.ToList();
		IEnumerable<CardModel> source2 = from c in source.SelectMany((CardPoolModel c) => c.GetUnlockedCards(targetPlayer.UnlockState, targetPlayer.RunState.CardMultiplayerConstraint))
			where c.Tags.Any(((IEnumerable<CardTag>)melodyTags).Contains<CardTag>)
			select c;
		List<CardModel> list = new List<CardModel>();
		CardTag[] array = melodyTags;
		foreach (CardTag melodyTag in array)
		{
			CardModel cardModel = CardFactory.GetDistinctForCombat(targetPlayer, source2.Where((CardModel c) => c.Tags.Contains(melodyTag)), 1, targetPlayer.RunState.Rng.CombatCardGeneration).FirstOrDefault();
			if (cardModel != null)
			{
				cardModel.SetToFreeThisTurn();
				list.Add(cardModel);
			}
		}
		await CardPileCmd.AddGeneratedCardsToCombat(list, PileType.Hand, Owner);
	}
}
