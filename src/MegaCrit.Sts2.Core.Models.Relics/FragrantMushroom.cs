using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.ValueProps;

namespace MegaCrit.Sts2.Core.Models.Relics;

public sealed class FragrantMushroom : RelicModel
{
	public const int hpLoss = 15;

	public override RelicRarity Rarity => RelicRarity.Event;

	public override bool HasUponPickupEffect => true;

	protected override IEnumerable<DynamicVar> CanonicalVars => new _003C_003Ez__ReadOnlyArray<DynamicVar>(new DynamicVar[2]
	{
		new HpLossVar(15m),
		new CardsVar(2)
	});

	public override async Task AfterObtained()
	{
		await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), Owner.Creature, DynamicVars.HpLoss.BaseValue, ValueProp.Unblockable | ValueProp.Unpowered, null, null);
		IEnumerable<CardModel> enumerable = PileType.Deck.GetPile(Owner).Cards.Where((CardModel c) => c?.IsUpgradable ?? false).ToList().StableShuffle(Owner.RunState.Rng.Niche)
			.Take(DynamicVars.Cards.IntValue);
		foreach (CardModel item in enumerable)
		{
			CardCmd.Upgrade(item, CardPreviewStyle.MessyLayout);
		}
	}
}
