using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.PotionPools;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.ValueProps;

namespace MegaCrit.Sts2.Core.Models.Events;

public sealed class WuppyInTrouble : EventModel
{
	private const string _cautiousHpKey = "CautiousWuppyHp";

	private const string _immediateHpKey = "ImmediateWuppyHp";

	private static readonly HoverTip _wuppyHoverTip = new HoverTip(new LocString("relics", "WUPPY.title"), new LocString("relics", "WUPPY.eventDescription"));

	public override bool IsShared => true;

	public override LocString InitialDescription
	{
		get
		{
			LocString initialDescription = base.InitialDescription;
			Player? owner = Owner;
			initialDescription.Add("IsMultiplayer", owner != null && owner.RunState.Players.Count > 1);
			return initialDescription;
		}
	}

	protected override IEnumerable<DynamicVar> CanonicalVars => new _003C_003Ez__ReadOnlyArray<DynamicVar>(new DynamicVar[3]
	{
		new DynamicVar("ImmediateWuppyHp", 60m),
		new DynamicVar("CautiousWuppyHp", 30m),
		new HpLossVar(0m)
	});

	public override bool IsAllowed(IRunState runState)
	{
		foreach (Player player in runState.Players)
		{
			if ((decimal)player.Creature.CurrentHp <= GetHpLoss(player))
			{
				return false;
			}
			if (player.HasEventPet())
			{
				return false;
			}
		}
		return true;
	}

	protected override IReadOnlyList<EventOption> GenerateInitialOptions()
	{
		EventOption[] array = new EventOption[3];
		Func<Task> onChosen = HelpImmediately;
		List<IHoverTip> list = new List<IHoverTip>();
		list.Add(_wuppyHoverTip);
		list.AddRange(HoverTipFactory.FromCardWithCardHoverTips<Injury>());
		array[0] = new EventOption(this, onChosen, "WUPPY_IN_TROUBLE.pages.INITIAL.options.HELP_IMMEDIATELY", list.ToArray());
		array[1] = new EventOption(this, BeCautious, "WUPPY_IN_TROUBLE.pages.INITIAL.options.BE_CAUTIOUS", _wuppyHoverTip).ThatDoesDamage(DynamicVars.HpLoss.BaseValue);
		array[2] = new EventOption(this, Ignore, "WUPPY_IN_TROUBLE.pages.INITIAL.options.IGNORE");
		return new _003C_003Ez__ReadOnlyArray<EventOption>(array);
	}

	private static decimal GetHpLoss(Player player)
	{
		return (decimal)player.Creature.CurrentHp * 0.05m;
	}

	public override void CalculateVars()
	{
		DynamicVars.HpLoss.BaseValue = GetHpLoss(Owner);
	}

	private async Task HelpImmediately()
	{
		CardModel card = Owner.RunState.CreateCard(ModelDb.Card<Injury>(), Owner);
		CardCmd.PreviewCardPileAdd(await CardPileCmd.Add(card, PileType.Deck));
		Wuppy wuppy = (Wuppy)ModelDb.Relic<Wuppy>().ToMutable();
		wuppy.MaxHp = DynamicVars["ImmediateWuppyHp"].IntValue;
		await RelicCmd.Obtain(wuppy, Owner);
		SetEventFinished(L10NLookup("WUPPY_IN_TROUBLE.pages.HELP_IMMEDIATELY.description"));
	}

	private async Task BeCautious()
	{
		await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), Owner.Creature, DynamicVars.HpLoss.BaseValue, ValueProp.Unblockable | ValueProp.Unpowered, null, null);
		Wuppy wuppy = (Wuppy)ModelDb.Relic<Wuppy>().ToMutable();
		wuppy.MaxHp = DynamicVars["CautiousWuppyHp"].IntValue;
		await RelicCmd.Obtain(wuppy, Owner);
		SetEventFinished(L10NLookup("WUPPY_IN_TROUBLE.pages.BE_CAUTIOUS.description"));
	}

	private async Task Ignore()
	{
		IEnumerable<PotionModel> items = Owner.Character.PotionPool.GetUnlockedPotions(Owner.UnlockState).Concat(ModelDb.PotionPool<SharedPotionPool>().GetUnlockedPotions(Owner.UnlockState));
		PotionModel potionModel = Owner.PlayerRng.Rewards.NextItem(items);
		if (potionModel != null)
		{
			await RewardsCmd.OfferCustom(Owner, new List<Reward>(1)
			{
				new PotionReward(potionModel.ToMutable(), Owner)
			});
		}
		SetEventFinished(L10NLookup("WUPPY_IN_TROUBLE.pages.IGNORE.description"));
	}
}
