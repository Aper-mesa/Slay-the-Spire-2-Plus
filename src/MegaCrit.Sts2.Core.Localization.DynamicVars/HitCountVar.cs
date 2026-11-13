using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Models;

namespace MegaCrit.Sts2.Core.Localization.DynamicVars;

public class HitCountVar : DynamicVar
{
	public const string defaultName = "HitCount";

	public HitCountVar(int hits)
		: base("HitCount", hits)
	{
	}

	public HitCountVar(string name, int hits)
		: base(name, hits)
	{
	}

	public override void UpdateCardPreview(CardModel card, CardPreviewMode previewMode, Creature? target, bool runGlobalHooks)
	{
		PreviewValue = GetPreviewValue(card, IntValue, runGlobalHooks);
	}

	internal static decimal GetPreviewValue(CardModel card, int hits, bool runGlobalHooks)
	{
		if (!runGlobalHooks)
		{
			return hits;
		}
		if (card.CombatState == null)
		{
			return hits;
		}
		return Hook.ModifyAttackHitCount(card.CombatState, DamageCmd.Attack(0m).FromCard(card, null), hits);
	}
}
