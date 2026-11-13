using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;

namespace MegaCrit.Sts2.Core.Localization.DynamicVars;

public class CalculatedHitCountVar : CalculatedVar
{
	public CalculatedHitCountVar(string name)
		: base(name)
	{
	}

	public override void UpdateCardPreview(CardModel card, CardPreviewMode previewMode, Creature? target, bool runGlobalHooks)
	{
		PreviewValue = HitCountVar.GetPreviewValue(card, (int)Calculate(target), runGlobalHooks);
	}
}
