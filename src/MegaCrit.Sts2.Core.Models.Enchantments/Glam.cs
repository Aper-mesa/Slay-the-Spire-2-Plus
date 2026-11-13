using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Enchantments;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace MegaCrit.Sts2.Core.Models.Enchantments;

public sealed class Glam : EnchantmentModel
{
	private const string _timesKey = "Times";

	protected override IEnumerable<DynamicVar> CanonicalVars => new _003C_003Ez__ReadOnlySingleElementList<DynamicVar>(new DynamicVar("Times", 1m));

	protected override IEnumerable<IHoverTip> ExtraHoverTips => new _003C_003Ez__ReadOnlySingleElementList<IHoverTip>(HoverTipFactory.Static(StaticHoverTip.ReplayDynamic, DynamicVars["Times"]));

	/// <summary>
	/// Glam adds its extra plays only to the first play of the card in a combat, not to every play. An effect that
	/// returns the card to hand and lets the player play it again does not get the bonus a second time. Closed as
	/// designed in PRG-6937.
	/// </summary>
	public override int EnchantPlayCount(int originalPlayCount)
	{
		if (Status == EnchantmentStatus.Disabled)
		{
			return originalPlayCount;
		}
		return originalPlayCount + DynamicVars["Times"].IntValue;
	}

	public override Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		if (cardPlay.Card != Card)
		{
			return Task.CompletedTask;
		}
		Status = EnchantmentStatus.Disabled;
		return Task.CompletedTask;
	}
}
