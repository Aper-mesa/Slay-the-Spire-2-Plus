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

public sealed class ClassicSilentCorpseExplosion : ClassicSilentCardModel
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<PoisonPower>(), HoverTipFactory.FromPower<ClassicSilentCorpseExplosionPower>()];

	protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<PoisonPower>("MagicNumber", 6m)];
	public ClassicSilentCorpseExplosion() : base(2, CardType.Skill, CardRarity.Rare, TargetType.AnyEnemy) { }
	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		await PowerCmd.Apply<PoisonPower>(choiceContext, cardPlay.Target, DynamicVars["MagicNumber"].BaseValue, Owner.Creature, this);
		await PowerCmd.Apply<ClassicSilentCorpseExplosionPower>(choiceContext, cardPlay.Target, 1, Owner.Creature, this);
	}
	protected override void OnUpgrade()
	{
		DynamicVars["MagicNumber"].UpgradeValueBy(3m);
	}
}
