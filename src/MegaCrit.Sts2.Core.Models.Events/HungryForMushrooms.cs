using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Models.Relics;

namespace MegaCrit.Sts2.Core.Models.Events;

public sealed class HungryForMushrooms : EventModel
{
	protected override IReadOnlyList<EventOption> GenerateInitialOptions()
	{
		return new _003C_003Ez__ReadOnlyArray<EventOption>(new EventOption[2]
		{
			RelicOption<BigMushroom>(BigMushroom),
			RelicOption<FragrantMushroom>(FragrantMushroom).ThatDoesDamage(15m)
		});
	}

	private async Task BigMushroom()
	{
		await RelicCmd.Obtain<BigMushroom>(Owner);
		SetEventFinished(L10NLookup("HUNGRY_FOR_MUSHROOMS.pages.BIG_MUSHROOM.description"));
	}

	private async Task FragrantMushroom()
	{
		await RelicCmd.Obtain<FragrantMushroom>(Owner);
		SetEventFinished(L10NLookup("HUNGRY_FOR_MUSHROOMS.pages.FRAGRANT_MUSHROOM.description"));
	}
}
