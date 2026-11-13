using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;

namespace MegaCrit.Sts2.Core.Models.Powers;

/// <summary>
/// Gives Thorns until the owner's next turn. Like <see cref="T:MegaCrit.Sts2.Core.Models.Powers.TemporaryDexterityPower" />, except it lasts through the
/// opponent's turn so the Thorns can trigger on their attacks, and it is positive only since Thorns has no negative
/// form the way Dexterity does.
/// </summary>
public abstract class TemporaryThornsPower : PowerModel, ITemporaryPower
{
	public override PowerType Type => PowerType.Buff;

	public override PowerStackType StackType => PowerStackType.Counter;

	/// <summary>
	/// The canonical model that applies this power. For example, <see cref="T:MegaCrit.Sts2.Core.Models.Cards.ChoBarbs" />.
	/// </summary>
	public abstract AbstractModel OriginModel { get; }

	public PowerModel InternallyAppliedPower => ModelDb.Power<ThornsPower>();

	public override LocString Title
	{
		get
		{
			AbstractModel originModel = OriginModel;
			if (!(originModel is CardModel cardModel))
			{
				if (!(originModel is PotionModel potionModel))
				{
					if (originModel is RelicModel relicModel)
					{
						return relicModel.Title;
					}
					throw new InvalidOperationException();
				}
				return potionModel.Title;
			}
			return cardModel.TitleLocString;
		}
	}

	public override LocString Description => new LocString("powers", "TEMPORARY_THORNS_POWER.description");

	protected override string SmartDescriptionLocKey => "TEMPORARY_THORNS_POWER.smartDescription";

	protected override IEnumerable<IHoverTip> ExtraHoverTips
	{
		get
		{
			List<IHoverTip> list = new List<IHoverTip>();
			List<IHoverTip> list2 = list;
			AbstractModel originModel = OriginModel;
			IEnumerable<IHoverTip> collection;
			if (!(originModel is CardModel card))
			{
				if (!(originModel is PotionModel))
				{
					if (!(originModel is RelicModel relic))
					{
						throw new InvalidOperationException();
					}
					collection = HoverTipFactory.FromRelic(relic);
				}
				else
				{
					collection = Array.Empty<IHoverTip>();
				}
			}
			else
			{
				collection = new _003C_003Ez__ReadOnlySingleElementList<IHoverTip>(HoverTipFactory.FromCard(card));
			}
			list2.AddRange(collection);
			list.Add(HoverTipFactory.FromPower<ThornsPower>());
			return new _003C_003Ez__ReadOnlyList<IHoverTip>(list);
		}
	}

	public override async Task BeforeApplied(Creature target, decimal amount, Creature? applier, CardModel? cardSource)
	{
		await PowerCmd.Apply<ThornsPower>(new ThrowingPlayerChoiceContext(), target, amount, applier, cardSource, silent: true);
	}

	public override async Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
	{
		if (power == this && !(amount == (decimal)Amount))
		{
			await PowerCmd.Apply<ThornsPower>(choiceContext, Owner, amount, applier, cardSource, silent: true);
		}
	}

	public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
	{
		if (Owner.Side != side)
		{
			Flash();
			await PowerCmd.Remove(this);
			ThornsPower power = Owner.GetPower<ThornsPower>();
			if (power != null)
			{
				await PowerCmd.ModifyAmount(choiceContext, power, -Amount, Owner, null);
			}
		}
	}
}
