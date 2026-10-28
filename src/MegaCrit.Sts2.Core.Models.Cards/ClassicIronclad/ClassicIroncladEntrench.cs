using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.ValueProps;

namespace MegaCrit.Sts2.Core.Models.Cards;

public sealed class ClassicIroncladEntrench : ClassicIroncladCardModel
{
	public override CardPoolModel VisualCardPool => ModelDb.CardPool<IroncladCardPool>();

	public override bool GainsBlock => true;

	public ClassicIroncladEntrench()
		: base(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
	{
	}

	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		await CreatureCmd.GainBlock(Owner.Creature, Owner.Creature.Block, ValueProp.Unpowered | ValueProp.Move, cardPlay);
	}

	protected override void OnUpgrade()
	{
		EnergyCost.UpgradeBy(-1);
	}
}
