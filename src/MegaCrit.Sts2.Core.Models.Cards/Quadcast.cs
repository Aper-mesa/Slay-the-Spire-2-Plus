using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace MegaCrit.Sts2.Core.Models.Cards;

public sealed class Quadcast : CardModel
{
	public override OrbEvokeType OrbEvokeType => OrbEvokeType.Front;

	protected override IEnumerable<DynamicVar> CanonicalVars => new _003C_003Ez__ReadOnlySingleElementList<DynamicVar>(new RepeatVar(4));

	public Quadcast()
		: base(1, CardType.Skill, CardRarity.Ancient, TargetType.Self)
	{
	}

	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		if (Owner.PlayerCombatState.OrbQueue.Orbs.Count <= 0)
		{
			return;
		}
		await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
		for (int i = 0; i < DynamicVars.Repeat.IntValue; i++)
		{
			await OrbCmd.EvokeNext(choiceContext, Owner, i == DynamicVars.Repeat.IntValue - 1);
			if (i != DynamicVars.Repeat.IntValue - 1)
			{
				await Cmd.CustomScaledWait(0.15f, 0.25f);
			}
		}
	}

	protected override void OnUpgrade()
	{
		EnergyCost.UpgradeBy(-1);
	}
}
