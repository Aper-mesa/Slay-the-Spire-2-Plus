using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Nodes.Vfx;

namespace MegaCrit.Sts2.Core.Models.Cards;

public sealed class KiChant : CardModel
{
	protected override HashSet<CardTag> CanonicalTags => new HashSet<CardTag> { CardTag.Ki };

	protected override IEnumerable<DynamicVar> CanonicalVars => new _003C_003Ez__ReadOnlySingleElementList<DynamicVar>(new CardsVar(3));

	protected override IEnumerable<string> ExtraRunAssetPaths
	{
		get
		{
			List<string> list = new List<string>();
			list.AddRange(NKiVfx.AssetPaths);
			return new _003C_003Ez__ReadOnlyList<string>(list);
		}
	}

	public KiChant()
		: base(1, CardType.Skill, CardRarity.Common, TargetType.Self)
	{
	}

	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		await MelodyCmd.PlayMelodyVfx(Owner.Creature, CardTag.Ki);
		int count = DynamicVars.Cards.IntValue;
		CardPile drawPile = PileType.Draw.GetPile(Owner);
		if (drawPile.Cards.Count == 0)
		{
			await CardPileCmd.ShuffleIfNecessary(choiceContext, Owner);
		}
		else if (drawPile.Cards.Count < count && PileType.Discard.GetPile(Owner).Cards.Any())
		{
			List<CardModel> alreadyInDraw = drawPile.Cards.ToList();
			await CardPileCmd.Shuffle(choiceContext, Owner);
			await CardPileCmd.Add(alreadyInDraw.Where((CardModel c) => c.Pile == drawPile).Reverse(), drawPile, CardPilePosition.Top, null, skipVisuals: true);
		}
		List<CardModel> topCards = drawPile.Cards.Take(count).ToList();
		if (topCards.Count == 0)
		{
			return;
		}
		List<CardModel> picked = (await CardSelectCmd.FromCombatPile(choiceContext, drawPile, Owner, new CardSelectorPrefs(SelectionScreenPrompt, 1), topCards.Contains)).ToList();
		foreach (CardModel item in picked)
		{
			await CardPileCmd.Add(item, PileType.Hand);
		}
		await CardCmd.Discard(choiceContext, from c in topCards.Except(picked)
			where c.Pile == drawPile
			select c);
	}

	protected override void OnUpgrade()
	{
		DynamicVars.Cards.UpgradeValueBy(1m);
	}
}
