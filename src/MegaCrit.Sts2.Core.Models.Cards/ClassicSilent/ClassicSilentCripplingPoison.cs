using System;
using MegaCrit.Sts2.Core.HoverTips;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace MegaCrit.Sts2.Core.Models.Cards;

public sealed class ClassicSilentCripplingPoison : ClassicSilentCardModel
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<PoisonPower>(), HoverTipFactory.FromPower<WeakPower>()];

	protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<PoisonPower>("MagicNumber", 4m)];
	public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
	public ClassicSilentCripplingPoison() : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.AllEnemies) { }
	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		foreach (Creature enemy in CombatState.HittableEnemies.ToList())
		{
		await PowerCmd.Apply<PoisonPower>(choiceContext, enemy, DynamicVars["MagicNumber"].BaseValue, Owner.Creature, this);
		await PowerCmd.Apply<WeakPower>(choiceContext, enemy, 2, Owner.Creature, this);
		}
	}
	protected override void OnUpgrade()
	{
		DynamicVars["MagicNumber"].UpgradeValueBy(3m);
	}
}
