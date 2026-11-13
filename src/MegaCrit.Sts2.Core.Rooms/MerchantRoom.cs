using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Entities.Merchant;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.TestSupport;

namespace MegaCrit.Sts2.Core.Rooms;

public class MerchantRoom : AbstractRoom
{
	private static MerchantDialogueSet? _dialogue;

	public override RoomType RoomType => RoomType.Shop;

	public override ModelId? ModelId => null;

	public static MerchantDialogueSet Dialogue
	{
		get
		{
			if (_dialogue != null)
			{
				return _dialogue;
			}
			LocTable table = LocManager.Instance.GetTable("merchant_room");
			IReadOnlyList<LocString> locStringsWithPrefix = table.GetLocStringsWithPrefix("MERCHANT.talk.");
			_dialogue = MerchantDialogueSet.CreateFromLocStrings(locStringsWithPrefix);
			return _dialogue;
		}
	}

	public override async Task EnterInternal(IRunState? runState, bool isRestoringRoomStackBase)
	{
		if (isRestoringRoomStackBase)
		{
			throw new InvalidOperationException("MerchantRoom does not support room stack reconstruction.");
		}
		RunManager.Instance.MerchantSynchronizer.OnMerchantEntered();
		if (runState != null && runState.GameMode == GameMode.Blitz)
		{
			if (!TestMode.IsOn)
			{
				BackgroundAssets backgroundAssets = runState.Act.GenerateBackgroundAssets(NCombatRoom.GenerateBackgroundRngForCurrentPoint(runState));
				IReadOnlyList<BlitzMerchantEntry> blitzEntries = RunManager.Instance.MerchantSynchronizer.GetLocalInventory().BlitzEntries;
				await PreloadManager.LoadRoomBlitzMerchantAssets(backgroundAssets, blitzEntries);
				NRun.Instance?.SetCurrentRoom(NBlitzMerchantRoom.Create(runState.Players, backgroundAssets));
			}
		}
		else
		{
			await PreloadManager.LoadRoomMerchantAssets();
			NRun.Instance?.SetCurrentRoom(NStandardMerchantRoom.Create(runState?.Players ?? Array.Empty<Player>()));
		}
		if (runState != null)
		{
			await Hook.AfterRoomEntered(runState, this);
		}
	}

	public override Task Exit(IRunState? runState)
	{
		if (TestMode.IsOn)
		{
			return Task.CompletedTask;
		}
		RunManager.Instance.MerchantSynchronizer.OnMerchantExited();
		return Task.CompletedTask;
	}

	public override Task Resume(AbstractRoom _, IRunState? runState)
	{
		throw new NotImplementedException();
	}
}
