using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Entities.RestSite;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Random;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace MegaCrit.Sts2.Core.Models.Relics;

public sealed class Byrdpip : RelicModel
{
	public const int playsToEvolve = 6;

	private string _skin = SkinOptions[0];

	private int _swoopsPlayed;

	public override bool AddsPet => true;

	public override RelicRarity Rarity => RelicRarity.Event;

	public override bool HasUponPickupEffect => true;

	public override bool SpawnsPets => true;

	public override bool ShowCounter => true;

	public override int DisplayAmount => Math.Min(SwoopsPlayed, 6);

	public static string[] SkinOptions => new string[4] { "version1", "version2", "version3", "version4" };

	[SavedProperty]
	public string Skin
	{
		get
		{
			return _skin;
		}
		set
		{
			AssertMutable();
			_skin = value;
		}
	}

	[SavedProperty]
	public int SwoopsPlayed
	{
		get
		{
			return _swoopsPlayed;
		}
		set
		{
			AssertMutable();
			_swoopsPlayed = value;
			UpdateDisplay();
		}
	}

	protected override IEnumerable<DynamicVar> CanonicalVars => new _003C_003Ez__ReadOnlySingleElementList<DynamicVar>(new RepeatVar(6));

	protected override IEnumerable<IHoverTip> ExtraHoverTips => HoverTipFactory.FromCardWithCardHoverTips<ByrdSwoop>();

	public override async Task AfterObtained()
	{
		Skin = new Rng(Owner, Id, 0uL).NextItem(SkinOptions);
		List<CardModel> list = PileType.Deck.GetPile(Owner).Cards.Where((CardModel c) => c is ByrdonisEgg).ToList();
		if (CombatManager.Instance.IsInProgress)
		{
			list.AddRange(Owner.PlayerCombatState.AllCards.Where((CardModel c) => c is ByrdonisEgg));
		}
		foreach (CardModel item in list)
		{
			await CardCmd.TransformTo<ByrdSwoop>(item);
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

	public override Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		if (!(cardPlay.Card is ByrdSwoop))
		{
			return Task.CompletedTask;
		}
		if (cardPlay.Card.Owner != Owner)
		{
			return Task.CompletedTask;
		}
		if (SwoopsPlayed >= 6)
		{
			return Task.CompletedTask;
		}
		SwoopsPlayed++;
		return Task.CompletedTask;
	}

	public override bool TryModifyRestSiteOptions(Player player, ICollection<RestSiteOption> options)
	{
		if (player != Owner)
		{
			return false;
		}
		if (SwoopsPlayed < 6)
		{
			return false;
		}
		options.Add(new EvolveRestSiteOption(player));
		return true;
	}

	private void UpdateDisplay()
	{
		Status = ((SwoopsPlayed >= 6) ? RelicStatus.Active : RelicStatus.Normal);
		InvokeDisplayAmountChanged();
	}

	private async Task SummonPet()
	{
		await PlayerCmd.AddPet<MegaCrit.Sts2.Core.Models.Monsters.Byrdpip>(Owner);
	}
}
