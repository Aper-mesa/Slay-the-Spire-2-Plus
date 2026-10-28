using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace MegaCrit.Sts2.Core.Models.Cards;

public sealed class ClassicSilentBlur : ClassicSilentCardModel
{
	private const string _blurKey = "ClassicSilentBlur";

	public override bool GainsBlock => true;

	protected override IEnumerable<DynamicVar> CanonicalVars => new _003C_003Ez__ReadOnlyArray<DynamicVar>(new DynamicVar[2]
	{
		new BlockVar(5m, ValueProp.Move),
		new DynamicVar("ClassicSilentBlur", 1m)
	});

	public ClassicSilentBlur()
		: base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
	{
	}

	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
		await PowerCmd.Apply<BlurPower>(choiceContext, Owner.Creature, DynamicVars["ClassicSilentBlur"].BaseValue, Owner.Creature, this);
	}

	protected override void OnUpgrade()
	{
		DynamicVars.Block.UpgradeValueBy(3m);
	}
}
