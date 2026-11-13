using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;

namespace MegaCrit.Sts2.Core.Models.Potions;

public sealed class CrystallineEuphoria : PotionModel
{
	public override PotionRarity Rarity => PotionRarity.Rare;

	public override PotionUsage Usage => PotionUsage.CombatOnly;

	public override TargetType TargetType => TargetType.AnyPlayer;

	protected override IEnumerable<DynamicVar> CanonicalVars => new _003C_003Ez__ReadOnlySingleElementList<DynamicVar>(new CardsVar(3));

	public override IEnumerable<IHoverTip> ExtraHoverTips => new _003C_003Ez__ReadOnlyArray<IHoverTip>(new IHoverTip[2]
	{
		HoverTipFactory.FromCard<Joy>(),
		HoverTipFactory.FromCard<Sadness>()
	});

	protected override async Task OnUse(PlayerChoiceContext choiceContext, Creature? target)
	{
		PotionModel.AssertValidForTargetedPotion(target);
		Player targetPlayer = target.Player;
		List<CardModel> list = new List<CardModel>();
		for (int i = 0; (decimal)i < DynamicVars.Cards.BaseValue; i++)
		{
			list.Add(targetPlayer.Creature.CombatState.CreateCard<Joy>(targetPlayer));
		}
		await CardPileCmd.AddGeneratedCardsToCombat(list, PileType.Hand, Owner);
		int num = CardPile.MaxCardsInHand - CardPile.GetCards(targetPlayer, PileType.Hand).Count();
		List<CardModel> list2 = new List<CardModel>();
		for (int j = 0; j < num; j++)
		{
			list2.Add(targetPlayer.Creature.CombatState.CreateCard<Sadness>(targetPlayer));
		}
		await CardPileCmd.AddGeneratedCardsToCombat(list2, PileType.Hand, Owner);
	}
}
