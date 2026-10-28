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

public sealed class ClassicSilentAThousandCuts : ClassicSilentCardModel
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<ClassicSilentThousandCutsPower>()];

	protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<ClassicSilentThousandCutsPower>("MagicNumber", 1m)];
	public ClassicSilentAThousandCuts() : base(2, CardType.Power, CardRarity.Rare, TargetType.Self) { }
	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		await PowerCmd.Apply<ClassicSilentThousandCutsPower>(choiceContext, Owner.Creature, DynamicVars["MagicNumber"].BaseValue, Owner.Creature, this);
	}
	protected override void OnUpgrade()
	{
		DynamicVars["MagicNumber"].UpgradeValueBy(1m);
	}
}
