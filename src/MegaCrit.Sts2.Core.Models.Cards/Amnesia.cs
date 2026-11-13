using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Models.Powers;

namespace MegaCrit.Sts2.Core.Models.Cards;

public sealed class Amnesia : CardModel, KnowledgeDemon.IChoosable
{
	public override int MaxUpgradeLevel => 0;

	public override bool CanBeGeneratedInCombat => false;

	protected override IEnumerable<DynamicVar> CanonicalVars => new _003C_003Ez__ReadOnlyArray<DynamicVar>(new DynamicVar[3]
	{
		new PowerVar<AmnesiaPower>(1m),
		new CardsVar(2),
		new EnergyVar(1)
	});

	public Amnesia()
		: base(-1, CardType.Status, CardRarity.Status, TargetType.None)
	{
	}

	public async Task OnChosen()
	{
		await PowerCmd.Apply<AmnesiaPower>(new ThrowingPlayerChoiceContext(), Owner.Creature, DynamicVars["AmnesiaPower"].IntValue, Owner.Creature, this);
	}
}
