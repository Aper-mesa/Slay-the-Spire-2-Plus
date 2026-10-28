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

public sealed class ClassicIroncladPowerThrough : ClassicIroncladCardModel
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromCard<Wound>()];

	protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(15m, ValueProp.Move)];
	public ClassicIroncladPowerThrough() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }
	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		await CardPileCmd.AddGeneratedCardToCombat(CombatState.CreateCard<Wound>(Owner), PileType.Hand, Owner);
		await CardPileCmd.AddGeneratedCardToCombat(CombatState.CreateCard<Wound>(Owner), PileType.Hand, Owner);
		await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
	}
	protected override void OnUpgrade()
	{
		DynamicVars.Block.UpgradeValueBy(5m);
	}
    public override bool GainsBlock => true;
}
