using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace MegaCrit.Sts2.Core.Models.Cards;

public sealed class Restlessness : CardModel
{
	protected override IEnumerable<DynamicVar> CanonicalVars => new _003C_003Ez__ReadOnlyArray<DynamicVar>(new DynamicVar[2]
	{
		new CardsVar(2),
		new EnergyVar(2)
	});

	protected override IEnumerable<IHoverTip> ExtraHoverTips => new _003C_003Ez__ReadOnlySingleElementList<IHoverTip>(EnergyHoverTip);

	public override IEnumerable<CardKeyword> CanonicalKeywords => new _003C_003Ez__ReadOnlySingleElementList<CardKeyword>(CardKeyword.Retain);

	protected override bool ShouldGlowGoldInternal => IsOnlyCardInHand;

	private bool IsOnlyCardInHand => !PileType.Hand.GetPile(Owner).Cards.Except(new _003C_003Ez__ReadOnlySingleElementList<CardModel>(this)).Any();

	public Restlessness()
		: base(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
	{
	}

	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		if (IsOnlyCardInHand)
		{
			for (int i = 0; i < DynamicVars.Cards.IntValue; i++)
			{
				await CardPileCmd.Draw(choiceContext, Owner);
			}
			await PlayerCmd.GainEnergy(DynamicVars.Energy.IntValue, Owner);
		}
	}

	protected override void OnUpgrade()
	{
		DynamicVars.Cards.UpgradeValueBy(1m);
		DynamicVars.Energy.UpgradeValueBy(1m);
	}
}
