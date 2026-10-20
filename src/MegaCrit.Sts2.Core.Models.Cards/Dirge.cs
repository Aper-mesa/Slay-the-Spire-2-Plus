using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace MegaCrit.Sts2.Core.Models.Cards;

public sealed class Dirge : CardModel
{
	protected override bool HasEnergyCostX => true;

	public override IEnumerable<CardKeyword> CanonicalKeywords => new _003C_003Ez__ReadOnlySingleElementList<CardKeyword>(CardKeyword.Exhaust);

	protected override IEnumerable<DynamicVar> CanonicalVars => new _003C_003Ez__ReadOnlySingleElementList<DynamicVar>(new SummonVar(3m));

	protected override IEnumerable<IHoverTip> ExtraHoverTips => new _003C_003Ez__ReadOnlyArray<IHoverTip>(new IHoverTip[2]
	{
		HoverTipFactory.Static(StaticHoverTip.SummonDynamic, DynamicVars.Summon),
		HoverTipFactory.FromCard<Soul>(IsUpgraded)
	});

	public Dirge()
		: base(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
	{
	}

	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
		int xValue = ResolveEnergyXValue();
		for (int i = 0; i < xValue; i++)
		{
			await OstyCmd.Summon(choiceContext, Owner, DynamicVars.Summon.BaseValue, this);
		}
		List<Soul> list = Soul.Create(Owner, xValue, CombatState).ToList();
		if (IsUpgraded)
		{
			foreach (Soul item in list)
			{
				CardCmd.Upgrade(item);
			}
		}
		CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardsToCombat(list, PileType.Draw, Owner, CardPilePosition.Random));
	}

	protected override void OnUpgrade()
	{
		DynamicVars.Summon.UpgradeValueBy(1m);
	}
}
