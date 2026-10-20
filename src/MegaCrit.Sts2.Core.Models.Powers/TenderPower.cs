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

public sealed class TenderPower : PowerModel
{
	private int _cardsPlayedThisTurn;

	public override PowerType Type => PowerType.Debuff;

	public override PowerStackType StackType => PowerStackType.Counter;

	public override int DisplayAmount => CardsPlayedThisTurn;

	protected override IEnumerable<IHoverTip> ExtraHoverTips => new _003C_003Ez__ReadOnlyArray<IHoverTip>(new IHoverTip[2]
	{
		HoverTipFactory.FromPower<StrengthPower>(),
		HoverTipFactory.FromPower<DexterityPower>()
	});

	private int CardsPlayedThisTurn
	{
		get
		{
			return _cardsPlayedThisTurn;
		}
		set
		{
			AssertMutable();
			_cardsPlayedThisTurn = value;
			InvokeDisplayAmountChanged();
		}
	}

	public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		if (cardPlay.Card.Owner == Owner.Player)
		{
			CardsPlayedThisTurn++;
			Flash();
			await PowerCmd.Apply<StrengthPower>(choiceContext, Owner, -1m, Applier, null, silent: true);
			await PowerCmd.Apply<DexterityPower>(choiceContext, Owner, -1m, Applier, null, silent: true);
		}
	}

	public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
	{
		if (participants.Contains(Owner))
		{
			await PowerCmd.Apply<StrengthPower>(choiceContext, Owner, CardsPlayedThisTurn, Applier, null, silent: true);
			await PowerCmd.Apply<DexterityPower>(choiceContext, Owner, CardsPlayedThisTurn, Applier, null, silent: true);
			CardsPlayedThisTurn = 0;
		}
	}
}
