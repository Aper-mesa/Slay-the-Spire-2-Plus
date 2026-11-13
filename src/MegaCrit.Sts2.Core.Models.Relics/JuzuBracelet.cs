using System.Collections.Generic;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Rooms;

namespace MegaCrit.Sts2.Core.Models.Relics;

public sealed class JuzuBracelet : RelicModel
{
	public override RelicRarity Rarity => RelicRarity.Common;

	public override IReadOnlySet<RoomType> ModifyUnknownMapPointRoomTypes(IReadOnlySet<RoomType> roomTypes)
	{
		HashSet<RoomType> hashSet = new HashSet<RoomType>();
		foreach (RoomType roomType in roomTypes)
		{
			hashSet.Add(roomType);
		}
		HashSet<RoomType> hashSet2 = hashSet;
		hashSet2.Remove(RoomType.Monster);
		hashSet2.Remove(RoomType.Elite);
		return hashSet2;
	}
}
