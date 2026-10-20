using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Map;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.SourceGeneration;

namespace MegaCrit.Sts2.Core.Nodes.Debug;

[GenerateSubtypes(DynamicallyAccessedMemberTypes = DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
public interface IBootstrapSettings
{
	CharacterModel Character { get; }

	RoomType RoomType { get; }

	EncounterModel Encounter { get; }

	EventModel Event { get; }

	ActModel Act { get; }

	int Ascension { get; }

	bool SaveRunHistory { get; }

	string? Seed { get; }

	bool DoPreloading { get; }

	bool BootstrapInMultiplayer { get; }

	List<ModifierModel> Modifiers { get; }

	string? Language => null;

	int? ReplayPlayerIndex => null;

	MapPointType MapPointType
	{
		get
		{
			RoomType roomType = RoomType;
			return roomType switch
			{
				RoomType.Monster => MapPointType.Monster, 
				RoomType.Elite => MapPointType.Elite, 
				RoomType.Boss => MapPointType.Boss, 
				RoomType.Treasure => MapPointType.Treasure, 
				RoomType.Shop => MapPointType.Shop, 
				RoomType.Event => MapPointType.Unknown, 
				RoomType.RestSite => MapPointType.RestSite, 
				RoomType.Map => MapPointType.Unknown, 
				RoomType.Unassigned => throw new ArgumentOutOfRangeException(), 
			};
		}
	}

	Task Setup(Player localPlayer);
}
