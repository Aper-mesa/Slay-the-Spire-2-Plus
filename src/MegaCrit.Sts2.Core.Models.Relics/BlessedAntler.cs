using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;

namespace MegaCrit.Sts2.Core.Models.Relics;

public sealed class BlessedAntler : RelicModel
{
	public override RelicRarity Rarity => RelicRarity.Ancient;

	protected override IEnumerable<DynamicVar> CanonicalVars => new _003C_003Ez__ReadOnlyArray<DynamicVar>(new DynamicVar[2]
	{
		new EnergyVar(1),
		new CardsVar(3)
	});

	protected override IEnumerable<IHoverTip> ExtraHoverTips
	{
		get
		{
			List<IHoverTip> list = new List<IHoverTip>();
			list.Add(HoverTipFactory.ForEnergy(this));
			list.AddRange(HoverTipFactory.FromCardWithCardHoverTips<Dazed>());
			return new _003C_003Ez__ReadOnlyList<IHoverTip>(list);
		}
	}

	public override decimal ModifyMaxEnergy(Player player, decimal amount)
	{
		if (player != Owner)
		{
			return amount;
		}
		return amount + (decimal)DynamicVars.Energy.IntValue;
	}

	public override async Task BeforeHandDraw(Player player, PlayerChoiceContext choiceContext, ICombatState combatState)
	{
		if (player == Owner && Owner.PlayerCombatState.TurnNumber == 1)
		{
			Flash();
			List<CardModel> list = new List<CardModel>();
			for (int i = 0; i < DynamicVars.Cards.IntValue; i++)
			{
				list.Add(combatState.CreateCard<Dazed>(Owner));
			}
			CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardsToCombat(list, PileType.Draw, Owner, CardPilePosition.Random));
			await Cmd.Wait(3f);
		}
	}
}
