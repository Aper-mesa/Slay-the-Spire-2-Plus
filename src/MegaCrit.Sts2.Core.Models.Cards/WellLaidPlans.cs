using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace MegaCrit.Sts2.Core.Models.Cards;

public sealed class WellLaidPlans : CardModel
{
	public WellLaidPlans()
		: base(2, CardType.Power, CardRarity.Rare, TargetType.Self)
	{
	}

	protected override void OnUpgrade()
	{
		EnergyCost.UpgradeBy(-1);
	}

	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		await CreatureCmd.TriggerAnim(Owner.Creature, "PowerUp", Owner.Character.PowerUpAnimDelay);
		await PowerCmd.Apply<WellLaidPlansPower>(choiceContext, Owner.Creature, 1m, Owner.Creature, this);
	}
}
