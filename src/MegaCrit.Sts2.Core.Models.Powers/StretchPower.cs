using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.ValueProps;

namespace MegaCrit.Sts2.Core.Models.Powers;

public sealed class StretchPower : PowerModel
{
	private class Data
	{
		public CardPlay? cardPlay;

		public int amountWhenCardStarted;

		public bool gaveBonusBlock;
	}

	public override PowerType Type => PowerType.Buff;

	public override PowerStackType StackType => PowerStackType.Counter;

	protected override IEnumerable<IHoverTip> ExtraHoverTips => new _003C_003Ez__ReadOnlySingleElementList<IHoverTip>(HoverTipFactory.Static(StaticHoverTip.Block));

	protected override object InitInternalData()
	{
		return new Data();
	}

	public override Task BeforeCardPlayed(CardPlay cardPlay)
	{
		if (cardPlay.Player.Creature != Owner)
		{
			return Task.CompletedTask;
		}
		if (!cardPlay.Card.GainsBlock)
		{
			return Task.CompletedTask;
		}
		Data internalData = GetInternalData<Data>();
		if (internalData.cardPlay != null)
		{
			return Task.CompletedTask;
		}
		internalData.cardPlay = cardPlay;
		internalData.amountWhenCardStarted = Amount;
		return Task.CompletedTask;
	}

	public override decimal ModifyBlockAdditive(Creature target, decimal block, ValueProp props, CardModel? cardSource, CardPlay? cardPlay)
	{
		if (cardSource == null)
		{
			return 0m;
		}
		if (cardSource.Owner != Owner.Player)
		{
			return 0m;
		}
		Data internalData = GetInternalData<Data>();
		if (internalData.cardPlay != null && internalData.cardPlay != cardPlay)
		{
			return 0m;
		}
		if (!props.IsPoweredCardOrMonsterMoveBlock())
		{
			return 0m;
		}
		if (cardPlay == null)
		{
			return Amount;
		}
		if (internalData.cardPlay == null)
		{
			return 0m;
		}
		if (internalData.cardPlay.Player != cardSource.Owner)
		{
			return 0m;
		}
		internalData.gaveBonusBlock = true;
		return Amount;
	}

	public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		Data internalData = GetInternalData<Data>();
		if (cardPlay == internalData.cardPlay)
		{
			internalData.cardPlay = null;
			if (internalData.gaveBonusBlock)
			{
				internalData.gaveBonusBlock = false;
				await PowerCmd.ModifyAmount(choiceContext, this, -internalData.amountWhenCardStarted, null, null);
			}
		}
	}
}
