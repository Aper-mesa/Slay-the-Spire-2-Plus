using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;

namespace MegaCrit.Sts2.Core.Models.Cards;

public sealed class ClassicSilentWraithForm : ClassicSilentCardModel
{
	protected override IEnumerable<DynamicVar> CanonicalVars => new _003C_003Ez__ReadOnlyArray<DynamicVar>(new DynamicVar[2]
	{
		new PowerVar<IntangiblePower>(2m),
		new PowerVar<ClassicSilentWraithFormPower>(1m)
	});

	protected override IEnumerable<IHoverTip> ExtraHoverTips => new _003C_003Ez__ReadOnlyArray<IHoverTip>(new IHoverTip[2]
	{
		HoverTipFactory.FromPower<IntangiblePower>(),
		HoverTipFactory.FromPower<DexterityPower>()
	});

	public ClassicSilentWraithForm()
		: base(3, CardType.Power, CardRarity.Rare, TargetType.Self)
	{
	}

	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		await CreatureCmd.TriggerAnim(Owner.Creature, "PowerUp", Owner.Character.PowerUpAnimDelay);
		await PowerCmd.Apply<IntangiblePower>(choiceContext, Owner.Creature, DynamicVars["IntangiblePower"].BaseValue, Owner.Creature, this);
		await PowerCmd.Apply<ClassicSilentWraithFormPower>(choiceContext, Owner.Creature, DynamicVars["ClassicSilentWraithFormPower"].BaseValue, Owner.Creature, this);
	}

	protected override void OnUpgrade()
	{
		DynamicVars["IntangiblePower"].UpgradeValueBy(1m);
	}
}
