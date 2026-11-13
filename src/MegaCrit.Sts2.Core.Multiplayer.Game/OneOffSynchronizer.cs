using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Ascension;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Events.Custom.CrystalSphereEvent;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Map;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Events;
using MegaCrit.Sts2.Core.Multiplayer.Messages.Game;
using MegaCrit.Sts2.Core.Random;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Runs;

namespace MegaCrit.Sts2.Core.Multiplayer.Game;

/// <summary>
/// A synchronizer for a few one-off scenarios that don't fit in any other synchronizer.
/// This should be used pretty rarely, and we should be on the lookout for patterns that emerge with new content.
/// </summary>
public class OneOffSynchronizer : IDisposable
{
	private readonly RunLocationTargetedMessageBuffer _messageBuffer;

	private readonly INetGameService _gameService;

	private readonly IPlayerCollection _playerCollection;

	private readonly ulong _localPlayerId;

	private Player LocalPlayer => _playerCollection.GetPlayer(_localPlayerId);

	public OneOffSynchronizer(RunLocationTargetedMessageBuffer messageBuffer, INetGameService gameService, IPlayerCollection playerCollection, ulong localPlayerId)
	{
		_playerCollection = playerCollection;
		_localPlayerId = localPlayerId;
		_gameService = gameService;
		_messageBuffer = messageBuffer;
		messageBuffer.RegisterMessageHandler<TreasureChestOpenedMessage>(HandleTreasureChestOpenedMessage);
		messageBuffer.RegisterMessageHandler<CrystalSphereRewardsMessage>(HandleCrystalSphereRewardsMessage);
	}

	public void Dispose()
	{
		_messageBuffer.UnregisterMessageHandler<TreasureChestOpenedMessage>(HandleTreasureChestOpenedMessage);
		_messageBuffer.UnregisterMessageHandler<CrystalSphereRewardsMessage>(HandleCrystalSphereRewardsMessage);
	}

	/// <summary>
	/// Should be called after a player opens the chest in the treasure room to synchronize gold gain and other effects.
	/// </summary>
	/// <returns>The amount of gold given to the player.</returns>
	public Task<int> DoLocalTreasureRoomRewards()
	{
		TreasureChestOpenedMessage message = new TreasureChestOpenedMessage
		{
			Location = _messageBuffer.CurrentLocation
		};
		_gameService.SendMessage(message);
		return DoTreasureRoomRewards(LocalPlayer);
	}

	private void HandleTreasureChestOpenedMessage(TreasureChestOpenedMessage message, ulong senderId)
	{
		Player player = _playerCollection.GetPlayer(senderId);
		if (player == LocalPlayer)
		{
			throw new InvalidOperationException("TreasureChestOpenedMessage should not be sent to the player who opened the treasure chest!");
		}
		TaskHelper.RunSafely(DoTreasureRoomRewards(player));
	}

	private async Task<int> DoTreasureRoomRewards(Player player)
	{
		if (!Hook.ShouldGenerateTreasure(player.RunState, player))
		{
			return 0;
		}
		decimal gold = 0m;
		if (player.RunState.GameMode != GameMode.Blitz)
		{
			gold = player.PlayerRng.Rewards.NextInt(42, 53);
			if (AscensionHelper.HasAscension(AscensionLevel.Poverty))
			{
				gold *= 0.75m;
			}
			await PlayerCmd.GainGold((int)gold, player);
			decimal num = gold;
			gold = num + (decimal)(await TryHandleSpoilsMap(player));
		}
		return (int)gold;
	}

	/// <summary>
	/// Special one-off logic for Spoils Map. If more stuff ends up going here, make this a hook
	/// </summary>
	/// <returns>The amount of gold given by spoils map, if any.</returns>
	private async Task<int> TryHandleSpoilsMap(Player player)
	{
		MapPoint mapPoint = (player.RunState.CurrentMapCoord.HasValue ? player.RunState.Map.GetPoint(player.RunState.CurrentMapCoord.Value) : null);
		if (mapPoint == null)
		{
			return 0;
		}
		if (!mapPoint.Quests.Any((AbstractModel q) => q is SpoilsMap))
		{
			return 0;
		}
		List<SpoilsMap> list = player.Deck.Cards.OfType<SpoilsMap>().ToList();
		int num = 0;
		foreach (SpoilsMap item in list)
		{
			int num2 = num;
			num = num2 + await item.OnQuestComplete();
		}
		return num;
	}

	/// <summary>
	/// Special one-off logic for presenting crystal sphere rewards to the player, and notifying other players that
	/// crystal sphere rewards are beginning for the local player.
	/// </summary>
	public async Task DoLocalCrystalSphereRewards(Player owner, Rng rng, List<CrystalSphereItem> revealed)
	{
		if (owner != LocalContext.GetMe(_playerCollection))
		{
			throw new InvalidOperationException($"Trying to sync crystal sphere rewards for non-local player {owner.NetId}!");
		}
		List<SerializableCrystalSphereItem> rewards = revealed.Select((CrystalSphereItem r) => r.ToSerializable()).ToList();
		CrystalSphereRewardsMessage message = new CrystalSphereRewardsMessage
		{
			Location = _messageBuffer.CurrentLocation,
			rewards = rewards
		};
		_gameService.SendMessage(message);
		await OfferCrystalSphereRewards(owner, revealed, rng);
	}

	private void HandleCrystalSphereRewardsMessage(CrystalSphereRewardsMessage message, ulong senderId)
	{
		Player player = _playerCollection.GetPlayer(senderId);
		if (player == LocalPlayer)
		{
			throw new InvalidOperationException("CrystalSphereRewardsMessage should not be sent to the player who completed the event!");
		}
		EventModel eventForPlayer = RunManager.Instance.EventSynchronizer.GetEventForPlayer(player);
		if (!(eventForPlayer is CrystalSphere crystalSphere))
		{
			throw new InvalidOperationException($"Received {"CrystalSphereRewardsMessage"} for player {player.NetId} while the player was in event {eventForPlayer.Id}!");
		}
		List<CrystalSphereItem> revealed = message.rewards.Select((SerializableCrystalSphereItem r) => CrystalSphereItem.FromSerializable(r, player)).ToList();
		TaskHelper.RunSafely(OfferCrystalSphereRewards(player, revealed, crystalSphere.Rng));
	}

	private async Task OfferCrystalSphereRewards(Player owner, List<CrystalSphereItem> revealed, Rng rng)
	{
		List<Reward> rewards = revealed.Select((CrystalSphereItem r) => r.ToReward(owner, rng)).OfType<Reward>().ToList();
		await RewardsCmd.OfferCustom(owner, rewards);
	}
}
