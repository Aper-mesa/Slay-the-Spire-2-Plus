using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;

namespace MegaCrit.Sts2.Core.Models.Powers;

public sealed class PhantomBladesPower : PowerModel
{
	public override PowerType Type => PowerType.Buff;

	public override PowerStackType StackType => PowerStackType.Counter;

	protected override IEnumerable<IHoverTip> ExtraHoverTips => new _003C_003Ez__ReadOnlyArray<IHoverTip>(new IHoverTip[2]
	{
		HoverTipFactory.FromCard<Shiv>(),
		HoverTipFactory.FromKeyword(CardKeyword.Retain)
	});

	/// <summary>
	/// Retain is granted to the owner's Shivs for as long as this power exists. When this power is removed, it stops
	/// contributing here automatically, so any Retain from another source (e.g. <see cref="T:MegaCrit.Sts2.Core.Models.Cards.Snap" />) is left untouched.
	/// </summary>
	public override bool TryModifyKeywordsInCombat(CardModel card, ISet<CardKeyword> keywords)
	{
		if (card.Owner != Owner.Player)
		{
			return false;
		}
		if (!card.Tags.Contains(CardTag.Shiv))
		{
			return false;
		}
		return keywords.Add(CardKeyword.Retain);
	}

	public override Task AfterApplied(Creature? applier, CardModel? cardSource)
	{
		InvokeKeywordsChangedOnShivs(Owner.Player);
		return Task.CompletedTask;
	}

	public override Task AfterRemoved(Creature oldOwner)
	{
		InvokeKeywordsChangedOnShivs(oldOwner.Player);
		return Task.CompletedTask;
	}

	/// <summary>
	/// Retain from the hook above raises no card event, so the Shivs already in combat are told here to flash.
	/// </summary>
	private static void InvokeKeywordsChangedOnShivs(Player player)
	{
		foreach (CardModel item in player.PlayerCombatState.AllCards.Where((CardModel c) => c.Tags.Contains(CardTag.Shiv)))
		{
			item.InvokeKeywordsChanged();
		}
	}

	public override decimal ModifyDamageAdditive(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource, CardPlay? cardPlay)
	{
		if (!props.IsPoweredAttack())
		{
			return 0m;
		}
		if (cardSource == null || !cardSource.Tags.Contains(CardTag.Shiv))
		{
			return 0m;
		}
		if (dealer != Owner)
		{
			return 0m;
		}
		int num = CombatManager.Instance.History.CardPlaysFinished.Count((CardPlayFinishedEntry e) => e.HappenedThisTurn(CombatState) && e.CardPlay.Card.Tags.Contains(CardTag.Shiv) && e.CardPlay.Player == Owner.Player);
		if (num > 0)
		{
			return 0m;
		}
		return Amount;
	}
}
