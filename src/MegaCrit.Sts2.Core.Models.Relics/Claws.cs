using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;

namespace MegaCrit.Sts2.Core.Models.Relics;

public sealed class Claws : RelicModel
{
	public override RelicRarity Rarity => RelicRarity.Ancient;

	protected override IEnumerable<DynamicVar> CanonicalVars => new _003C_003Ez__ReadOnlySingleElementList<DynamicVar>(new CardsVar(6));

	protected override IEnumerable<IHoverTip> ExtraHoverTips => HoverTipFactory.FromCardWithCardHoverTips<Maul>();

	public override async Task AfterObtained()
	{
		CardSelectorPrefs prefs = new CardSelectorPrefs(SelectionScreenPrompt, 0, DynamicVars.Cards.IntValue)
		{
			Cancelable = false,
			RequireManualConfirmation = true
		};
		List<CardTransformation> transformations = (await CardSelectCmd.FromDeckForTransformation(Owner, prefs, (CardModel c) => new CardTransformation(c, CreateMaulFromOriginal(c, forPreview: true)))).Select((CardModel original) => new CardTransformation(original, CreateMaulFromOriginal(original, forPreview: false))).ToList();
		await CardCmd.Transform(transformations, Owner.PlayerRng.Transformations);
	}

	private CardModel CreateMaulFromOriginal(CardModel original, bool forPreview)
	{
		CardModel cardModel = (forPreview ? ModelDb.Card<Maul>().ToMutable() : Owner.RunState.CreateCard<Maul>(Owner));
		if (original.IsUpgraded && cardModel.IsUpgradable)
		{
			if (forPreview)
			{
				cardModel.UpgradeInternal();
			}
			else
			{
				CardCmd.Upgrade(cardModel);
			}
		}
		if (original.Enchantment != null)
		{
			EnchantmentModel enchantmentModel = (EnchantmentModel)original.Enchantment.MutableClone();
			if (enchantmentModel.CanEnchant(cardModel))
			{
				if (forPreview)
				{
					cardModel.EnchantInternal(enchantmentModel, enchantmentModel.Amount);
					enchantmentModel.ModifyCard();
				}
				else
				{
					CardCmd.Enchant(enchantmentModel, cardModel, enchantmentModel.Amount);
				}
			}
		}
		return cardModel;
	}
}
