using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace MegaCrit.Sts2.Core.Models.Relics;

public sealed class CharonsAshes : RelicModel
{
	public override RelicRarity Rarity => RelicRarity.Rare;

	protected override IEnumerable<DynamicVar> CanonicalVars => new _003C_003Ez__ReadOnlySingleElementList<DynamicVar>(new DamageVar(3m, ValueProp.Unpowered));

	public override async Task AfterCardExhausted(PlayerChoiceContext choiceContext, CardModel card, bool _)
	{
		if (card.Owner == Owner)
		{
			Flash();
			DamageVar damage = DynamicVars.Damage;
			await CreatureCmd.Damage(choiceContext, Owner.Creature.CombatState.HittableEnemies, damage.BaseValue, damage.Props, Owner.Creature);
		}
	}
}
