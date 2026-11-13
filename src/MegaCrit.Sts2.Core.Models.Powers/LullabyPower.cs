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
using MegaCrit.Sts2.Core.ValueProps;

namespace MegaCrit.Sts2.Core.Models.Powers;

public sealed class LullabyPower : PowerModel
{
	private bool _proccdThisTurn;

	public override PowerType Type => PowerType.Buff;

	public override PowerStackType StackType => PowerStackType.Counter;

	protected override IEnumerable<IHoverTip> ExtraHoverTips => new _003C_003Ez__ReadOnlyArray<IHoverTip>(new IHoverTip[4]
	{
		HoverTipFactory.Static(StaticHoverTip.Block),
		HoverTipFactory.Static(StaticHoverTip.WahMelody),
		HoverTipFactory.Static(StaticHoverTip.ChoMelody),
		HoverTipFactory.Static(StaticHoverTip.KiMelody)
	});

	private bool ProccdThisTurn
	{
		get
		{
			return _proccdThisTurn;
		}
		set
		{
			AssertMutable();
			_proccdThisTurn = value;
		}
	}

	public override Task AfterApplied(Creature? applier, CardModel? cardSource)
	{
		if (MelodyCmd.PlayedChoThisTurn(Owner.Player) && MelodyCmd.PlayedWahThisTurn(Owner.Player) && MelodyCmd.PlayedKiThisTurn(Owner.Player))
		{
			ProccdThisTurn = true;
		}
		return Task.CompletedTask;
	}

	public override Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
	{
		if (!participants.Contains(Owner))
		{
			return Task.CompletedTask;
		}
		ProccdThisTurn = false;
		return Task.CompletedTask;
	}

	public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		if (cardPlay.Card.Owner.Creature == Owner && CombatManager.Instance.IsInProgress && !ProccdThisTurn && MelodyCmd.PlayedChoThisTurn(Owner.Player) && MelodyCmd.PlayedWahThisTurn(Owner.Player) && MelodyCmd.PlayedKiThisTurn(Owner.Player))
		{
			Flash();
			ProccdThisTurn = true;
			await CreatureCmd.GainBlock(Owner, Amount, ValueProp.Unpowered, null, fast: true);
		}
	}
}
