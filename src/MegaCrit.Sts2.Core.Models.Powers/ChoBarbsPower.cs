using MegaCrit.Sts2.Core.Models.Cards;

namespace MegaCrit.Sts2.Core.Models.Powers;

public class ChoBarbsPower : TemporaryThornsPower
{
	public override AbstractModel OriginModel => ModelDb.Card<ChoBarbs>();
}
