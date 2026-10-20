using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace MegaCrit.Sts2.Core.Models.Cards;

public sealed class BigBang : CardModel
{
	public override IEnumerable<CardKeyword> CanonicalKeywords => new _003C_003Ez__ReadOnlySingleElementList<CardKeyword>(CardKeyword.Exhaust);

	protected override IEnumerable<DynamicVar> CanonicalVars => new _003C_003Ez__ReadOnlyArray<DynamicVar>(new DynamicVar[4]
	{
		new CardsVar(1),
		new EnergyVar(1),
		new StarsVar(1),
		new ForgeVar(5)
	});

	protected override IEnumerable<IHoverTip> ExtraHoverTips
	{
		get
		{
			List<IHoverTip> list = new List<IHoverTip>();
			list.Add(EnergyHoverTip);
			list.AddRange(HoverTipFactory.FromForge());
			return new _003C_003Ez__ReadOnlyList<IHoverTip>(list);
		}
	}

	public BigBang()
		: base(0, CardType.Skill, CardRarity.Rare, TargetType.Self)
	{
	}

	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
		await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.BaseValue, Owner);
		await PlayerCmd.GainStars(DynamicVars.Stars.BaseValue, Owner);
		await PlayerCmd.GainEnergy(DynamicVars.Energy.IntValue, Owner);
		await ForgeCmd.Forge(DynamicVars.Forge.IntValue, Owner, this);
	}

	protected override void OnUpgrade()
	{
		AddKeyword(CardKeyword.Innate);
	}
}
