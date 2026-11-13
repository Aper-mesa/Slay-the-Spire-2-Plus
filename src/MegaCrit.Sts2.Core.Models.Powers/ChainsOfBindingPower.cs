using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Afflictions;
using MegaCrit.Sts2.Core.Nodes.CommonUi;

namespace MegaCrit.Sts2.Core.Models.Powers;

public sealed class ChainsOfBindingPower : PowerModel
{
	private class Data
	{
		public bool boundCardPlayed;
	}

	public override PowerType Type => PowerType.Debuff;

	public override PowerStackType StackType => PowerStackType.Counter;

	protected override IEnumerable<IHoverTip> ExtraHoverTips => HoverTipFactory.FromAffliction<Bound>();

	protected override object InitInternalData()
	{
		return new Data();
	}

	/// <summary>
	/// Bound is applied here. A card played from AfterCardDrawnEarly is played before this runs, so it is unafflicted
	/// at play time: it skips <see cref="M:MegaCrit.Sts2.Core.Models.Powers.ChainsOfBindingPower.ShouldPlay(MegaCrit.Sts2.Core.Models.CardModel,MegaCrit.Sts2.Core.Entities.Cards.AutoPlayType)" /> and is not counted by <see cref="M:MegaCrit.Sts2.Core.Models.Powers.ChainsOfBindingPower.BeforeCardPlayed(MegaCrit.Sts2.Core.Entities.Cards.CardPlay)" />. It still
	/// becomes Bound straight afterwards. Hellraiser relies on this; closed as designed in PRG-6544.
	/// </summary>
	public override async Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)
	{
		if (card.Owner == Owner.Player && CombatState.CurrentSide == Owner.Side && ModelDb.Affliction<Bound>().CanAfflict(card))
		{
			int num = CombatManager.Instance.History.Entries.OfType<CardAfflictedEntry>().Count((CardAfflictedEntry e) => e.HappenedThisTurn(CombatState) && e.Actor == Owner && e.Affliction is Bound);
			if (num < Amount)
			{
				await CardCmd.AfflictAndPreview<Bound>(new _003C_003Ez__ReadOnlySingleElementList<CardModel>(card), Amount, CardPreviewStyle.None);
			}
		}
	}

	/// <summary>
	/// This runs for automatic plays as well as manual ones, so a Bound card played by Sly uses up the player's one
	/// Bound card for the turn. Confirmed as designed in PRG-2503.
	/// </summary>
	public override Task BeforeCardPlayed(CardPlay cardPlay)
	{
		CardModel card = cardPlay.Card;
		if (card.IsDupe)
		{
			return Task.CompletedTask;
		}
		if (card.Owner.Creature != Owner)
		{
			return Task.CompletedTask;
		}
		if (!(card.Affliction is Bound))
		{
			return Task.CompletedTask;
		}
		GetInternalData<Data>().boundCardPlayed = true;
		return Task.CompletedTask;
	}

	public override bool ShouldPlay(CardModel card, AutoPlayType autoPlayType)
	{
		if (card.Owner.Creature != Owner)
		{
			return true;
		}
		if (!(card.Affliction is Bound))
		{
			return true;
		}
		return !GetInternalData<Data>().boundCardPlayed;
	}

	public override Task BeforeSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
	{
		if (!participants.Contains(Owner))
		{
			return Task.CompletedTask;
		}
		GetInternalData<Data>().boundCardPlayed = false;
		IEnumerable<CardModel> enumerable = Owner.Player?.PlayerCombatState?.AllCards ?? Array.Empty<CardModel>();
		foreach (CardModel item in enumerable)
		{
			if (item.Affliction is Bound)
			{
				CardCmd.ClearAffliction(item);
			}
		}
		return Task.CompletedTask;
	}
}
