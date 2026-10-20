using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;

namespace MegaCrit.Sts2.Core.Models.Cards;

public sealed class ImitationLearning : CardModel
{
	public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;

	public override IEnumerable<CardKeyword> CanonicalKeywords => new _003C_003Ez__ReadOnlySingleElementList<CardKeyword>(CardKeyword.Exhaust);

	protected override IEnumerable<DynamicVar> CanonicalVars => new _003C_003Ez__ReadOnlySingleElementList<DynamicVar>(new PowerVar<ImitationLearningPower>(2m));

	public ImitationLearning()
		: base(1, CardType.Skill, CardRarity.Rare, TargetType.AnyAlly)
	{
	}

	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
		await CreatureCmd.TriggerAnim(Owner.Creature, "PowerUp", Owner.Character.PowerUpAnimDelay);
		ImitationLearningPower imitationLearningPower = Owner.Creature.Powers.OfType<ImitationLearningPower>().FirstOrDefault((ImitationLearningPower s) => s.PlayerTarget == cardPlay.Target.Player);
		decimal baseValue = DynamicVars["ImitationLearningPower"].BaseValue;
		if (imitationLearningPower != null)
		{
			await PowerCmd.ModifyAmount(choiceContext, imitationLearningPower, baseValue, Owner.Creature, this);
			return;
		}
		imitationLearningPower = await PowerCmd.Apply<ImitationLearningPower>(choiceContext, Owner.Creature, baseValue, Owner.Creature, this);
		if (imitationLearningPower != null)
		{
			imitationLearningPower.PlayerTarget = cardPlay.Target.Player;
		}
	}

	protected override void OnUpgrade()
	{
		DynamicVars["ImitationLearningPower"].UpgradeValueBy(1m);
	}
}
