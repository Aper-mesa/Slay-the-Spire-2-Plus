using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Map;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace MegaCrit.Sts2.Core.Models.Cards;

public sealed class Dowsing : CardModel
{
	public const int maxRooms = 4;

	private const string _roomsKey = "Rooms";

	private int _roomsEntered;

	public override int MaxUpgradeLevel => 0;

	protected override IEnumerable<DynamicVar> CanonicalVars => new _003C_003Ez__ReadOnlySingleElementList<DynamicVar>(new DynamicVar("Rooms", 4m));

	public override IEnumerable<CardKeyword> CanonicalKeywords => new _003C_003Ez__ReadOnlySingleElementList<CardKeyword>(CardKeyword.Unplayable);

	protected override IEnumerable<IHoverTip> ExtraHoverTips => new _003C_003Ez__ReadOnlySingleElementList<IHoverTip>(HoverTipFactory.FromCard<Abundance>());

	[SavedProperty]
	public int RoomsEntered
	{
		get
		{
			return _roomsEntered;
		}
		set
		{
			AssertMutable();
			_roomsEntered = value;
			DynamicVars["Rooms"].BaseValue = Math.Max(1, 4 - RoomsEntered);
		}
	}

	public Dowsing()
		: base(-1, CardType.Quest, CardRarity.Quest, TargetType.None)
	{
	}

	public override async Task BeforeRoomEntered(AbstractRoom room)
	{
		CardPile? pile = Pile;
		if (pile == null || pile.Type != PileType.Deck || Owner.RunState.CurrentRoomCount > 1)
		{
			return;
		}
		MapPoint? currentMapPoint = Owner.RunState.CurrentMapPoint;
		if (currentMapPoint != null && currentMapPoint.PointType == MapPointType.Unknown)
		{
			RoomsEntered++;
			if (RoomsEntered >= 4)
			{
				PlayerCmd.CompleteQuest(this);
				await CardCmd.TransformTo<Abundance>(this);
			}
		}
	}

	public override IReadOnlySet<RoomType> ModifyUnknownMapPointRoomTypes(IReadOnlySet<RoomType> roomTypes)
	{
		HashSet<RoomType> hashSet = new HashSet<RoomType>();
		foreach (RoomType roomType in roomTypes)
		{
			hashSet.Add(roomType);
		}
		HashSet<RoomType> hashSet2 = hashSet;
		if (RoomsEntered >= 3)
		{
			hashSet2.Remove(RoomType.Monster);
			hashSet2.Remove(RoomType.Elite);
		}
		return hashSet2;
	}
}
