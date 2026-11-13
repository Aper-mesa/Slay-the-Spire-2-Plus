using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;

namespace MegaCrit.Sts2.Core.Models.Cards;

public sealed class ChoChorus : CardModel
{
	public override bool GainsBlock => true;

	protected override HashSet<CardTag> CanonicalTags => new HashSet<CardTag> { CardTag.Cho };

	protected override IEnumerable<DynamicVar> CanonicalVars => new _003C_003Ez__ReadOnlySingleElementList<DynamicVar>(new BlockVar(12m, ValueProp.Move));

	protected override IEnumerable<string> ExtraRunAssetPaths => NChoVfx.AssetPaths;

	public ChoChorus()
		: base(3, CardType.Skill, CardRarity.Rare, TargetType.Self)
	{
	}

	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		await CreatureCmd.TriggerAnim(Owner.Creature, "PowerUp", Owner.Character.PowerUpAnimDelay);
		await MelodyCmd.PlayMelodyVfx(Owner.Creature, CardTag.Cho);
		await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
		CardPile pile = PileType.Hand.GetPile(Owner);
		List<CardModel> list = pile.Cards.Where((CardModel c) => c.Tags.Contains(CardTag.Wah) || c.Tags.Contains(CardTag.Ki)).ToList();
		foreach (CardModel item in list)
		{
			await CardCmd.AutoPlay(choiceContext, item, null);
		}
	}

	protected override void OnUpgrade()
	{
		DynamicVars.Block.UpgradeValueBy(3m);
	}
}
