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

public sealed class ClassicSilentCatalyst : ClassicSilentCardModel
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<PoisonPower>()];

	protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("MagicNumber", 2m)];
	public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
	public ClassicSilentCatalyst() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy) { }
	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		int poison = cardPlay.Target.GetPowerAmount<PoisonPower>();
		if (poison > 0)
		{
		await PowerCmd.Apply<PoisonPower>(choiceContext, cardPlay.Target, poison * (DynamicVars["MagicNumber"].IntValue - 1), Owner.Creature, this);
		}
	}
	protected override void OnUpgrade()
	{
		DynamicVars["MagicNumber"].UpgradeValueBy(1m);
	}
}
