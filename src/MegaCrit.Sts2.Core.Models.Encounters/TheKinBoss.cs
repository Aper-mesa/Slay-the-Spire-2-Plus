using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Godot;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models.Dread;
using MegaCrit.Sts2.Core.Models.Dread.Dominions;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Rooms;

namespace MegaCrit.Sts2.Core.Models.Encounters;

public sealed class TheKinBoss : EncounterModel
{
	public override RoomType RoomType => RoomType.Boss;

	public override bool HasScene => true;

	protected override bool HasCustomBackground => true;

	public override string CustomBgm => "event:/music/act1_boss_the_kin";

	public override string BossNodePath => "res://images/map/placeholder/" + Id.Entry.ToLowerInvariant() + "_icon";

	public override MegaSkeletonDataResource? BossNodeSpineResource => null;

	public override IReadOnlyList<string> Slots
	{
		get
		{
			int num = 2;
			List<string> list = new List<string>(num);
			CollectionsMarshal.SetCount(list, num);
			Span<string> span = CollectionsMarshal.AsSpan(list);
			int num2 = 0;
			span[num2] = "slot1";
			num2++;
			span[num2] = "slot2";
			List<string> list2 = list;
			if (AscensionHelper.IsDread())
			{
				list2.Add("slot3");
			}
			list2.Add("leaderSlot");
			return list2;
		}
	}

	protected override string ScenePath => SceneHelper.GetScenePath("encounters/" + Id.Entry.ToLowerInvariant() + (AscensionHelper.IsDread() ? "_dread" : ""));

	public override IEnumerable<MonsterModel> AllPossibleMonsters => new _003C_003Ez__ReadOnlyArray<MonsterModel>(new MonsterModel[2]
	{
		ModelDb.Monster<KinFollower>(),
		ModelDb.Monster<KinPriest>()
	});

	public override float GetCameraScaling()
	{
		if (!AscensionHelper.IsDread())
		{
			return 0.85f;
		}
		return 0.8f;
	}

	public override Vector2 GetCameraOffset()
	{
		return Vector2.Down * 50f;
	}

	protected override IReadOnlyList<(MonsterModel, string?)> GenerateMonsters()
	{
		List<(MonsterModel, string)> list2;
		if (AscensionHelper.IsDread())
		{
			KinFollower kinFollower = (KinFollower)ModelDb.Monster<KinFollower>().ToMutable();
			KinFollower kinFollower2 = (KinFollower)ModelDb.Monster<KinFollower>().ToMutable();
			KinFollower kinFollower3 = (KinFollower)ModelDb.Monster<KinFollower>().ToMutable();
			kinFollower.Index = 0;
			kinFollower2.Index = 1;
			kinFollower3.Index = 2;
			int num = 3;
			List<(MonsterModel, string)> list = new List<(MonsterModel, string)>(num);
			CollectionsMarshal.SetCount(list, num);
			Span<(MonsterModel, string)> span = CollectionsMarshal.AsSpan(list);
			int num2 = 0;
			span[num2] = (kinFollower, "slot1");
			num2++;
			span[num2] = (kinFollower2, "slot2");
			num2++;
			span[num2] = (kinFollower3, "slot3");
			list2 = list;
		}
		else
		{
			KinFollower kinFollower4 = (KinFollower)ModelDb.Monster<KinFollower>().ToMutable();
			KinFollower kinFollower5 = (KinFollower)ModelDb.Monster<KinFollower>().ToMutable();
			kinFollower4.Index = 0;
			kinFollower5.Index = 1;
			int num2 = 2;
			List<(MonsterModel, string)> list3 = new List<(MonsterModel, string)>(num2);
			CollectionsMarshal.SetCount(list3, num2);
			Span<(MonsterModel, string)> span = CollectionsMarshal.AsSpan(list3);
			int num = 0;
			span[num] = (kinFollower4, "slot1");
			num++;
			span[num] = (kinFollower5, "slot2");
			list2 = list3;
		}
		list2.Add((ModelDb.Monster<KinPriest>().ToMutable(), "leaderSlot"));
		return list2;
	}

	public override DominionModel GetDominion()
	{
		return ModelDb.Dominion<KinDominion>();
	}
}
