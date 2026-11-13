using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace MegaCrit.Sts2.Core.Models.Cards;

public sealed class SelfDeprecate : CardModel
{
	private const string _joyKey = "Joy";

	private const string _sadnessKey = "Sadness";

	public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;

	protected override IEnumerable<IHoverTip> ExtraHoverTips => new _003C_003Ez__ReadOnlyArray<IHoverTip>(new IHoverTip[2]
	{
		HoverTipFactory.FromCard<Joy>(),
		HoverTipFactory.FromCard<Sadness>()
	});

	protected override IEnumerable<DynamicVar> CanonicalVars => new _003C_003Ez__ReadOnlyArray<DynamicVar>(new DynamicVar[2]
	{
		new DynamicVar("Joy", 2m),
		new DynamicVar("Sadness", 1m)
	});

	public SelfDeprecate()
		: base(1, CardType.Skill, CardRarity.Rare, TargetType.AllAllies)
	{
	}

	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		List<CardModel> list = new List<CardModel>();
		for (int i = 0; (decimal)i < DynamicVars["Sadness"].BaseValue; i++)
		{
			list.Add(CombatState.CreateCard<Sadness>(Owner));
		}
		await CardPileCmd.AddGeneratedCardsToCombat(list, PileType.Hand, Owner);
		SfxCmd.Play("event:/sfx/characters/fugitive/fugitive_joy");
		if (CombatState == null)
		{
			return;
		}
		IEnumerable<Creature> enumerable = from c in CombatState.GetTeammatesOf(Owner.Creature)
			where c != null && c.IsAlive && c.IsPlayer && c.Player != Owner
			select c;
		foreach (Creature item in enumerable)
		{
			List<CardModel> list2 = new List<CardModel>();
			for (int num = 0; (decimal)num < DynamicVars["Joy"].BaseValue; num++)
			{
				list2.Add(CombatState.CreateCard<Joy>(item.Player));
			}
			await CardPileCmd.AddGeneratedCardsToCombat(list2, PileType.Hand, Owner);
		}
	}

	protected override void OnUpgrade()
	{
		DynamicVars["Joy"].UpgradeValueBy(1m);
	}
}
