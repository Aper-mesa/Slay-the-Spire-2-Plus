using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace MegaCrit.Sts2.Core.Models.Relics;

public sealed class Byrdine : RelicModel
{
	private string _hair = HairOptions[0];

	public override bool AddsPet => true;

	public override RelicRarity Rarity => RelicRarity.Event;

	public override bool HasUponPickupEffect => true;

	public override bool SpawnsPets => true;

	protected override IEnumerable<IHoverTip> ExtraHoverTips
	{
		get
		{
			List<IHoverTip> list = new List<IHoverTip>();
			list.AddRange(HoverTipFactory.FromCardWithCardHoverTips<ByrdSwoop>());
			list.AddRange(HoverTipFactory.FromCardWithCardHoverTips<GodByrd>());
			return new _003C_003Ez__ReadOnlyList<IHoverTip>(list);
		}
	}

	public static string[] HairOptions => new string[4] { "hair/v1", "hair/v2", "hair/v3", "hair/v4" };

	[SavedProperty]
	public string Hair
	{
		get
		{
			return _hair;
		}
		set
		{
			AssertMutable();
			_hair = value;
		}
	}

	public override async Task AfterObtained()
	{
		List<CardModel> list = PileType.Deck.GetPile(Owner).Cards.Where((CardModel c) => c is ByrdSwoop).ToList();
		if (CombatManager.Instance.IsInProgress)
		{
			list.AddRange(Owner.PlayerCombatState.AllCards.Where((CardModel c) => c is ByrdSwoop));
		}
		foreach (CardModel item in list)
		{
			CardModel cardModel = item.CardScope.CreateCard<GodByrd>(item.Owner);
			if (item.IsUpgraded)
			{
				CardCmd.Upgrade(cardModel);
			}
			await CardCmd.Transform(item, cardModel);
		}
		if (CombatManager.Instance.IsInProgress)
		{
			await SummonPet();
		}
	}

	public override async Task BeforeCombatStart()
	{
		await SummonPet();
	}

	private async Task SummonPet()
	{
		await PlayerCmd.AddPet<ByrdineMonster>(Owner);
	}
}
