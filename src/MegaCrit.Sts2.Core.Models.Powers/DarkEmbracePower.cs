using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;

namespace MegaCrit.Sts2.Core.Models.Powers;

public sealed class DarkEmbracePower : PowerModel
{
	private class Data
	{
		/// <summary>
		/// If Ethereal cards are exhausted at turn end, we want to give the resulting cards to the player after the
		/// flush occurs. In STS1 this is handled because the card draws are put on the queue, but here we have to
		/// manually defer.
		/// </summary>
		public int etherealCount;
	}

	public override PowerType Type => PowerType.Buff;

	public override PowerStackType StackType => PowerStackType.Counter;

	protected override IEnumerable<IHoverTip> ExtraHoverTips => new _003C_003Ez__ReadOnlySingleElementList<IHoverTip>(HoverTipFactory.FromKeyword(CardKeyword.Exhaust));

	protected override object InitInternalData()
	{
		return new Data();
	}

	public override async Task AfterCardExhausted(PlayerChoiceContext choiceContext, CardModel card, bool causedByEthereal)
	{
		if (card.Owner.Creature == Owner)
		{
			if (causedByEthereal)
			{
				GetInternalData<Data>().etherealCount++;
			}
			else
			{
				await CardPileCmd.Draw(choiceContext, Amount, Owner.Player);
			}
		}
	}

	public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
	{
		if (participants.Contains(Owner))
		{
			Data data = GetInternalData<Data>();
			await CardPileCmd.Draw(choiceContext, Amount * data.etherealCount, Owner.Player);
			data.etherealCount = 0;
		}
	}
}
