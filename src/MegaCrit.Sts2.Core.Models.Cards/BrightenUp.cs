using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace MegaCrit.Sts2.Core.Models.Cards;

public sealed class BrightenUp : CardModel
{
	private const string _joyKey = "Joy";

	private const string _sadnessKey = "Sadness";

	public override IEnumerable<CardKeyword> CanonicalKeywords => new _003C_003Ez__ReadOnlySingleElementList<CardKeyword>(CardKeyword.Exhaust);

	protected override IEnumerable<IHoverTip> ExtraHoverTips => new _003C_003Ez__ReadOnlyArray<IHoverTip>(new IHoverTip[2]
	{
		HoverTipFactory.FromCard<Joy>(),
		HoverTipFactory.FromCard<Sadness>()
	});

	protected override IEnumerable<DynamicVar> CanonicalVars => new _003C_003Ez__ReadOnlyArray<DynamicVar>(new DynamicVar[2]
	{
		new DynamicVar("Joy", 2m),
		new DynamicVar("Sadness", 2m)
	});

	public BrightenUp()
		: base(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
	{
	}

	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
		List<CardModel> list = new List<CardModel>();
		for (int i = 0; (decimal)i < DynamicVars["Sadness"].BaseValue; i++)
		{
			list.Add(CombatState.CreateCard<Sadness>(Owner));
		}
		await CardPileCmd.AddGeneratedCardsToCombat(list, PileType.Hand, Owner);
		SfxCmd.Play("event:/sfx/characters/fugitive/fugitive_joy");
		List<CardModel> list2 = new List<CardModel>();
		for (int j = 0; (decimal)j < DynamicVars["Joy"].BaseValue; j++)
		{
			list2.Add(CombatState.CreateCard<Joy>(Owner));
		}
		await CardPileCmd.AddGeneratedCardsToCombat(list2, PileType.Hand, Owner);
	}

	protected override void OnUpgrade()
	{
		DynamicVars["Joy"].UpgradeValueBy(1m);
	}
}
