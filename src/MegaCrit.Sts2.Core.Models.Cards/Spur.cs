using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Characters;

namespace MegaCrit.Sts2.Core.Models.Cards;

public sealed class Spur : CardModel
{
	public override IEnumerable<CardKeyword> CanonicalKeywords => new _003C_003Ez__ReadOnlySingleElementList<CardKeyword>(CardKeyword.Retain);

	protected override IEnumerable<IHoverTip> ExtraHoverTips => new _003C_003Ez__ReadOnlySingleElementList<IHoverTip>(HoverTipFactory.Static(StaticHoverTip.SummonDynamic, DynamicVars.Summon));

	protected override IEnumerable<DynamicVar> CanonicalVars => new _003C_003Ez__ReadOnlyArray<DynamicVar>(new DynamicVar[2]
	{
		new SummonVar(3m),
		new HealVar(5m)
	});

	public Spur()
		: base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
	{
	}

	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		await CreatureCmd.TriggerAnim(Owner.Creature, Necrobinder.GetSummonAnimIfApplicable(Owner.Character), Necrobinder.GetSummonDelayIfApplicable(Owner.Character));
		await OstyCmd.Summon(choiceContext, Owner, DynamicVars.Summon.BaseValue, this);
		await CreatureCmd.Heal(Owner.Osty, DynamicVars.Heal.BaseValue);
	}

	protected override void OnUpgrade()
	{
		DynamicVars.Summon.UpgradeValueBy(2m);
		DynamicVars.Heal.UpgradeValueBy(2m);
	}
}
