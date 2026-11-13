using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Debug;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Save;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Map;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Characters;
using MegaCrit.Sts2.Core.Models.Events;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.Platform;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Runs.History;
using MegaCrit.Sts2.Core.Saves.Migrations;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.Saves.Validation;
using MegaCrit.Sts2.Core.TestSupport;
using MegaCrit.Sts2.Core.Timeline;
using MegaCrit.Sts2.Core.Timeline.Epochs;
using MegaCrit.Sts2.Core.Unlocks;
using Sentry;

namespace MegaCrit.Sts2.Core.Saves.Managers;

public class ProgressSaveManager
{
	public const string fileName = "progress.save";

	private readonly ISaveStore _saveStore;

	private readonly MigrationManager _migrationManager;

	private readonly IProfileIdProvider _profileIdProvider;

	private static HashSet<ModelId>? _eliteEncounters;

	public ProgressState Progress { get; set; } = ProgressState.CreateDefault();

	public ProgressSaveManager(int profileId, ISaveStore saveStore, MigrationManager migrationManager)
		: this(saveStore, migrationManager, new StaticProfileIdProvider(profileId))
	{
	}

	public ProgressSaveManager(ISaveStore saveStore, MigrationManager migrationManager, IProfileIdProvider profileIdProvider)
	{
		_saveStore = saveStore;
		_migrationManager = migrationManager;
		_profileIdProvider = profileIdProvider;
	}

	public static string GetProgressPathForProfile(int profileId, bool? forceModState = null)
	{
		return UserDataPathProvider.GetProfileDir(profileId, forceModState).PathJoin(UserDataPathProvider.SavesDir).PathJoin("progress.save");
	}

	public void SaveProgress()
	{
		try
		{
			SerializableProgress serializableProgress = Progress.ToSerializable();
			serializableProgress.SchemaVersion = _migrationManager.GetLatestVersion<SerializableProgress>();
			string content = JsonSerializationUtility.ToJson(serializableProgress);
			_saveStore.WriteFile(GetProgressPathForProfile(_profileIdProvider.CurrentProfileId), content);
		}
		catch (Exception ex)
		{
			Log.Error($"Failed to save progress: {ex}");
			SentryService.CaptureException(ex);
		}
	}

	public ReadSaveResult<SerializableProgress> LoadProgress()
	{
		ReadSaveResult<SerializableProgress> readSaveResult = _migrationManager.LoadSave<SerializableProgress>(GetProgressPathForProfile(_profileIdProvider.CurrentProfileId));
		if (!readSaveResult.Success || readSaveResult.SaveData == null)
		{
			Progress = ProgressState.CreateDefault();
			Ironclad ironclad = ModelDb.Character<Ironclad>();
			foreach (CardModel item in ironclad.StartingDeck)
			{
				Progress.MarkCardAsSeen(item.Id);
			}
			foreach (RelicModel startingRelic in ironclad.StartingRelics)
			{
				Progress.MarkRelicAsSeen(startingRelic.Id);
			}
			SaveProgress();
		}
		else
		{
			DeserializationContext deserializationContext = new DeserializationContext();
			Progress = ProgressState.FromSerializable(readSaveResult.SaveData, deserializationContext);
			foreach (ValidationError error in deserializationContext.Errors)
			{
				Log.Warn($"Progress parse: {error}");
			}
		}
		return readSaveResult;
	}

	public UnlockState GenerateUnlockState()
	{
		return new UnlockState(Progress);
	}

	private void IncrementEncounterLoss(ModelId characterId, ModelId encounterId)
	{
		EncounterStats orCreateEncounterStats = Progress.GetOrCreateEncounterStats(encounterId);
		FightStats fightStats = orCreateEncounterStats.FightStats.FirstOrDefault((FightStats f) => f.Character == characterId);
		if (fightStats == null)
		{
			Log.Info($"{characterId} fought {encounterId} for the first time and LOST :(");
			FightStats item = new FightStats
			{
				Character = characterId,
				Wins = 0,
				Losses = 1
			};
			orCreateEncounterStats.FightStats.Add(item);
		}
		else
		{
			orCreateEncounterStats.IncrementLoss(characterId);
		}
	}

	private void IncrementEnemyFightLoss(ModelId characterId, ModelId monster)
	{
		EnemyStats orCreateEnemyStats = Progress.GetOrCreateEnemyStats(monster);
		FightStats fightStats = orCreateEnemyStats.FightStats.FirstOrDefault((FightStats f) => f.Character == characterId);
		if (fightStats == null)
		{
			Log.Info($"{characterId} fought {monster} for the first time and LOST >:(");
			FightStats item = new FightStats
			{
				Character = characterId,
				Wins = 0,
				Losses = 1
			};
			orCreateEnemyStats.FightStats.Add(item);
		}
		else
		{
			orCreateEnemyStats.IncrementLoss(characterId);
		}
	}

	public void MarkPotionAsSeen(PotionModel potion)
	{
		if (Progress.MarkPotionAsSeen(potion.Id) && LocalContext.IsMine(potion))
		{
			potion.Owner.DiscoveredPotions.Add(potion.Id);
		}
	}

	public void MarkCardAsSeen(CardModel card)
	{
		if (Progress.MarkCardAsSeen(card.Id) && LocalContext.IsMine(card))
		{
			card.Owner.DiscoveredCards.Add(card.Id);
		}
	}

	public void MarkRelicAsSeen(RelicModel relic)
	{
		if (Progress.MarkRelicAsSeen(relic.Id) && LocalContext.IsMine(relic))
		{
			relic.Owner.DiscoveredRelics.Add(relic.Id);
		}
	}

	public PostRunProgressUpdateResult UpdateWithRunData(SerializableRun serializableRun, bool victory)
	{
		bool flag = serializableRun.Players.Count == 1;
		bool flag2 = serializableRun.GameMode == GameMode.Daily;
		bool isCustomGame = serializableRun.IsCustomGame;
		bool flag3 = serializableRun.GameMode == GameMode.Dread;
		PostRunProgressUpdateResult result = new PostRunProgressUpdateResult
		{
			isSingleplayer = flag,
			run = serializableRun
		};
		SerializablePlayer serializablePlayer;
		if (flag)
		{
			serializablePlayer = serializableRun.Players.First();
		}
		else
		{
			ulong playerId = PlatformUtil.GetLocalPlayerId(serializableRun.PlatformType);
			serializablePlayer = serializableRun.Players.FirstOrDefault((SerializablePlayer p) => p.NetId == playerId);
			if (serializablePlayer == null)
			{
				Log.Warn($"Local player with net id {playerId} not found in run! Progress will not be updated");
				result.success = false;
				return result;
			}
		}
		for (int num = 0; num < serializableRun.MapPointHistory.Count; num++)
		{
			if (num >= serializableRun.Acts.Count)
			{
				Log.Warn($"There are {serializableRun.MapPointHistory.Count} acts in the map point history, but {serializableRun.Acts.Count} acts in the act array! This is unexpected");
			}
			else
			{
				ModelId id = serializableRun.Acts[num].Id;
				Progress.MarkActAsSeen(id);
			}
		}
		List<MapPointHistoryEntry> list = serializableRun.MapPointHistory.SelectMany((List<MapPointHistoryEntry> act) => act).ToList();
		Progress.TotalPlaytime += ((serializableRun.WinTime > 0) ? serializableRun.WinTime : serializableRun.RunTime);
		Progress.WongoPoints += serializablePlayer.ExtraFields.WongoPoints;
		Progress.TestSubjectKills += serializableRun.ExtraFields.TestSubjectKills;
		Progress.FloorsClimbed += list.Count;
		CharacterStats orCreateCharacterStats = Progress.GetOrCreateCharacterStats(serializablePlayer.CharacterId);
		orCreateCharacterStats.Playtime += ((serializableRun.WinTime > 0) ? serializableRun.WinTime : serializableRun.RunTime);
		if (victory)
		{
			int num2 = ScoreUtility.CalculateScore(serializableRun, victory);
			Progress.ArchitectDamage += num2;
			if (!flag2 && !isCustomGame)
			{
				if (serializableRun.GameMode == GameMode.Blitz)
				{
					result.oldMaxAscension = Progress.MaxBlitzAscension;
					IncrementBlitzAscension(serializableRun);
					result.newMaxAscension = Progress.MaxBlitzAscension;
				}
				else if (flag)
				{
					result.oldMaxAscension = orCreateCharacterStats.MaxAscension;
					IncrementSingleplayerAscension(serializableRun, orCreateCharacterStats);
					result.newMaxAscension = orCreateCharacterStats.MaxAscension;
				}
				else
				{
					result.oldMaxAscension = Progress.MaxMultiplayerAscension;
					IncrementMultiplayerAscension(serializableRun);
					result.newMaxAscension = Progress.MaxMultiplayerAscension;
				}
				orCreateCharacterStats.TotalWins++;
				orCreateCharacterStats.CurrentWinStreak++;
				orCreateCharacterStats.BestWinStreak = Math.Max(orCreateCharacterStats.BestWinStreak, orCreateCharacterStats.CurrentWinStreak);
				if (flag3 && !isCustomGame)
				{
					CharacterStats orCreateCharacterStats2 = Progress.GetOrCreateCharacterStats(serializablePlayer.ExtraFields.IsPlayingRotatingCharacter ? ModelDb.GetId<RotatingCharacter>() : serializablePlayer.CharacterId);
					int previousStreak = (flag ? orCreateCharacterStats2.CurrentSingleplayerDreadWinStreak : orCreateCharacterStats2.CurrentMultiplayerDreadWinStreak);
					ModelId previousStartingRotationCharacter = ModelId.none;
					if (serializablePlayer.ExtraFields.IsPlayingRotatingCharacter)
					{
						previousStartingRotationCharacter = (flag ? Progress.CurrentSingleplayerDreadStartingCharacterForRotation : Progress.CurrentMultiplayerDreadStartingCharacterForRotation);
					}
					UpdateDreadWinStreakOnVictory(orCreateCharacterStats2, flag);
					SetPendingDreadWinStreakResult(orCreateCharacterStats2.Id, flag, wasReset: false, previousStartingRotationCharacter, previousStreak, 1);
				}
				if (orCreateCharacterStats.FastestWinTime < 0 || orCreateCharacterStats.FastestWinTime > serializableRun.RunTime)
				{
					orCreateCharacterStats.FastestWinTime = serializableRun.RunTime;
				}
			}
		}
		else
		{
			if (!flag2 && !isCustomGame)
			{
				orCreateCharacterStats.CurrentWinStreak = 0L;
				orCreateCharacterStats.TotalLosses++;
				if (flag3)
				{
					CharacterStats orCreateCharacterStats3 = Progress.GetOrCreateCharacterStats(serializablePlayer.ExtraFields.IsPlayingRotatingCharacter ? ModelDb.GetId<RotatingCharacter>() : serializablePlayer.CharacterId);
					ModelId previousStartingRotationCharacter2 = ModelId.none;
					int previousStreak2;
					if (flag)
					{
						previousStreak2 = orCreateCharacterStats3.CurrentSingleplayerDreadWinStreak;
						orCreateCharacterStats3.CurrentSingleplayerDreadWinStreak = 0;
						if (serializablePlayer.ExtraFields.IsPlayingRotatingCharacter)
						{
							previousStartingRotationCharacter2 = Progress.CurrentSingleplayerDreadStartingCharacterForRotation;
							Progress.CurrentSingleplayerDreadStartingCharacterForRotation = ModelId.none;
						}
					}
					else
					{
						previousStreak2 = orCreateCharacterStats3.CurrentMultiplayerDreadWinStreak;
						orCreateCharacterStats3.CurrentMultiplayerDreadWinStreak = 0;
						if (serializablePlayer.ExtraFields.IsPlayingRotatingCharacter)
						{
							previousStartingRotationCharacter2 = Progress.CurrentMultiplayerDreadStartingCharacterForRotation;
							Progress.CurrentMultiplayerDreadStartingCharacterForRotation = ModelId.none;
						}
					}
					SetPendingDreadWinStreakResult(orCreateCharacterStats3.Id, flag, wasReset: true, previousStartingRotationCharacter2, previousStreak2, 0);
				}
			}
			MapPointHistoryEntry mapPointHistoryEntry = serializableRun.MapPointHistory.LastOrDefault()?.LastOrDefault();
			if (mapPointHistoryEntry != null)
			{
				RoomType roomType = mapPointHistoryEntry.Rooms.Last().RoomType;
				if (roomType.IsCombatRoom())
				{
					ModelId modelId = mapPointHistoryEntry.Rooms.Last().ModelId;
					ModelId characterId = serializablePlayer.CharacterId;
					IncrementEncounterLoss(characterId, modelId);
					foreach (ModelId monsterId in mapPointHistoryEntry.Rooms.Last().MonsterIds)
					{
						IncrementEnemyFightLoss(characterId, monsterId);
					}
				}
			}
		}
		foreach (MapPointHistoryEntry item in list)
		{
			MapPointRoomHistoryEntry mapPointRoomHistoryEntry = item.FirstRoomOfType(RoomType.Event);
			if (mapPointRoomHistoryEntry != null)
			{
				Progress.MarkEventAsSeen(mapPointRoomHistoryEntry.ModelId);
			}
		}
		if (!flag2 && !isCustomGame)
		{
			HashSet<ModelId> hashSet = serializablePlayer.Deck.Select((SerializableCard c) => c.Id).ToHashSet();
			foreach (ModelId item2 in hashSet)
			{
				CardStats orCreateCardStats = Progress.GetOrCreateCardStats(item2);
				if (victory)
				{
					orCreateCardStats.TimesWon++;
				}
				else
				{
					orCreateCardStats.TimesLost++;
				}
			}
			foreach (MapPointHistoryEntry item3 in list)
			{
				PlayerMapPointHistoryEntry entry = item3.GetEntry(serializablePlayer.NetId);
				foreach (CardChoiceHistoryEntry cardChoice in entry.CardChoices)
				{
					CardStats orCreateCardStats2 = Progress.GetOrCreateCardStats(cardChoice.Card.Id);
					if (cardChoice.wasPicked)
					{
						orCreateCardStats2.TimesPicked++;
					}
					else
					{
						orCreateCardStats2.TimesSkipped++;
					}
				}
				if (item3.MapPointType == MapPointType.Ancient)
				{
					MapPointRoomHistoryEntry mapPointRoomHistoryEntry2 = item3.FirstRoomOfType(RoomType.Event);
					AncientStats orCreateAncientStats = Progress.GetOrCreateAncientStats(mapPointRoomHistoryEntry2.ModelId);
					if (victory)
					{
						orCreateAncientStats.IncrementWin(serializablePlayer.CharacterId);
					}
					else
					{
						orCreateAncientStats.IncrementLoss(serializablePlayer.CharacterId);
					}
				}
			}
		}
		UpdateEpochsPostRun(serializablePlayer, serializableRun, victory);
		SaveProgress();
		result.success = true;
		return result;
	}

	public void UpdateDreadWinStreakOnVictory(CharacterStats characterStats, bool isSingleplayer)
	{
		string gameVersion = NGame.GetGameVersion();
		if (isSingleplayer)
		{
			var (val, bestSingleplayerDreadWinStreakOldVersion, bestSingleplayerDreadWinStreakOldVersionTag) = ReconcileDreadWinStreakVersion(gameVersion, characterStats.BestSingleplayerDreadWinStreakThisVersion, characterStats.BestSingleplayerDreadWinStreakThisVersionTag, characterStats.BestSingleplayerDreadWinStreakOldVersion, characterStats.BestSingleplayerDreadWinStreakOldVersionTag);
			characterStats.CurrentSingleplayerDreadWinStreak++;
			characterStats.BestSingleplayerDreadWinStreakThisVersion = Math.Max(val, characterStats.CurrentSingleplayerDreadWinStreak);
			characterStats.BestSingleplayerDreadWinStreakThisVersionTag = gameVersion;
			characterStats.BestSingleplayerDreadWinStreakOldVersion = bestSingleplayerDreadWinStreakOldVersion;
			characterStats.BestSingleplayerDreadWinStreakOldVersionTag = bestSingleplayerDreadWinStreakOldVersionTag;
		}
		else
		{
			var (val2, bestMultiplayerDreadWinStreakOldVersion, bestMultiplayerDreadWinStreakOldVersionTag) = ReconcileDreadWinStreakVersion(gameVersion, characterStats.BestMultiplayerDreadWinStreakThisVersion, characterStats.BestMultiplayerDreadWinStreakThisVersionTag, characterStats.BestMultiplayerDreadWinStreakOldVersion, characterStats.BestMultiplayerDreadWinStreakOldVersionTag);
			characterStats.CurrentMultiplayerDreadWinStreak++;
			characterStats.BestMultiplayerDreadWinStreakThisVersion = Math.Max(val2, characterStats.CurrentMultiplayerDreadWinStreak);
			characterStats.BestMultiplayerDreadWinStreakThisVersionTag = gameVersion;
			characterStats.BestMultiplayerDreadWinStreakOldVersion = bestMultiplayerDreadWinStreakOldVersion;
			characterStats.BestMultiplayerDreadWinStreakOldVersionTag = bestMultiplayerDreadWinStreakOldVersionTag;
		}
	}

	public static (int ThisVersion, int OldVersion, string? OldVersionTag) ReconcileDreadWinStreakVersion(string currentVersion, int thisVersion, string? thisVersionTag, int oldVersion, string? oldVersionTag)
	{
		if (thisVersionTag == currentVersion)
		{
			return (ThisVersion: thisVersion, OldVersion: oldVersion, OldVersionTag: oldVersionTag);
		}
		if (thisVersion <= oldVersion)
		{
			return (ThisVersion: 0, OldVersion: oldVersion, OldVersionTag: oldVersionTag);
		}
		return (ThisVersion: 0, OldVersion: thisVersion, OldVersionTag: thisVersionTag);
	}

	private void SetPendingDreadWinStreakResult(ModelId characterId, bool isSingleplayer, bool wasReset, ModelId previousStartingRotationCharacter, int previousStreak, int streakIncrease)
	{
		PendingDreadWinStreakResult pendingDreadWinStreakResult = new PendingDreadWinStreakResult
		{
			CharacterId = characterId,
			WasReset = wasReset,
			InitialRotatingCharacterBeforeReset = previousStartingRotationCharacter,
			WinStreakBeforeReset = previousStreak,
			WinStreakIncrease = streakIncrease
		};
		if (isSingleplayer)
		{
			Progress.PendingSingleplayerDreadWinStreakResult = pendingDreadWinStreakResult;
		}
		else
		{
			Progress.PendingMultiplayerDreadWinStreakResult = pendingDreadWinStreakResult;
		}
	}

	private void UpdateEpochsPostRun(SerializablePlayer serializablePlayer, SerializableRun serializableRun, bool victory)
	{
		TryObtainEpochPostRun(EpochModel.Get<NeowEpoch>(), serializablePlayer, serializableRun);
		PostRunUnlockCharacterEpochCheck(serializablePlayer, serializableRun);
		PostRunFoolBackfillCheck(serializablePlayer, serializableRun);
		PostRunCharacterEpochChecks(serializablePlayer, serializableRun, victory);
		List<ModelId> allCharacterIds = ModelDb.AllCharacters.Where((CharacterModel c) => c is not ClassicDefect && c is not ClassicSilent && c is not ClassicIronclad && c is not ClassicWatcher).Select((CharacterModel c) => c.Id).ToList();
		int count = allCharacterIds.Count;
		List<CharacterStats> list = Progress.CharacterStats.Values.Where((CharacterStats s) => s.Id != null && allCharacterIds.Contains(s.Id)).ToList();
		int count2 = list.Count;
		if (victory && count2 >= count)
		{
			TryObtainEpochPostRun(EpochModel.Get<DailyRunEpoch>(), serializablePlayer, serializableRun);
		}
		if (count2 >= count && list.All((CharacterStats stats) => stats.MaxAscension >= 10) && !GameModeHelper.AreAchievementsAndEpochsLocked(serializableRun.GameMode, serializableRun.IsCustomGame) && TryObtainEpochPostRun(EpochModel.Get<DreadEpoch>(), serializablePlayer, serializableRun))
		{
			Progress.ObtainEpochOverride(EpochModel.GetId<DreadEpoch>(), EpochState.Obtained);
		}
		if (count2 >= count)
		{
			TryObtainEpochPostRun(EpochModel.Get<OrobasEpoch>(), serializablePlayer, serializableRun);
		}
		if (ModelDb.AllAncients.All((AncientEventModel a) => Progress.AncientStats.ContainsKey(a.Id) || a is Darv))
		{
			TryObtainEpochPostRun(EpochModel.Get<DarvEpoch>(), serializablePlayer, serializableRun);
		}
		if (serializableRun.Ascension <= 0)
		{
			return;
		}
		if (!Progress.IsEmptyEpochSlotAvailable(EpochModel.Get<BlitzEpoch>().Id))
		{
			if (!GameModeHelper.AreAchievementsAndEpochsLockedForSerializableRun(serializableRun) && TryObtainEpochPostRun(EpochModel.Get<BlitzEpoch>(), serializablePlayer, serializableRun))
			{
				Progress.ObtainEpochOverride(EpochModel.GetId<BlitzEpoch>(), EpochState.Obtained);
			}
		}
		else
		{
			TryObtainEpochPostRun(EpochModel.Get<BlitzEpoch>(), serializablePlayer, serializableRun);
		}
	}

	private void PostRunUnlockCharacterEpochCheck(SerializablePlayer serializablePlayer, SerializableRun serializableRun)
	{
		CharacterModel byId = ModelDb.GetById<CharacterModel>(serializablePlayer.CharacterId);
		string text;
		if (byId is Silent)
		{
			text = EpochModel.GetId<Regent1Epoch>();
		}
		else if (byId is Regent)
		{
			text = EpochModel.GetId<Necrobinder1Epoch>();
		}
		else if (byId is Necrobinder)
		{
			text = EpochModel.GetId<Defect1Epoch>();
		}
		else
		{
			text = ((!(byId is Defect)) ? null : EpochModel.GetId<Fool1Epoch>());
		}
		string text2 = text;
		if (text2 != null && TryObtainEpochPostRun(EpochModel.Get(text2), serializablePlayer, serializableRun))
		{
			Log.Info($"Epoch obtained for playing a run as {serializablePlayer.CharacterId}");
		}
	}

	private void PostRunFoolBackfillCheck(SerializablePlayer serializablePlayer, SerializableRun serializableRun)
	{
		if (HasCompletedDefectRun() && TryObtainEpochPostRun(EpochModel.Get<Fool1Epoch>(), serializablePlayer, serializableRun))
		{
			Log.Info("Fool1Epoch obtained post-run for a player who has already completed a Defect run");
		}
	}

	public void ObtainFoolEpochIfDefectRunCompleted()
	{
		string id = EpochModel.GetId<Fool1Epoch>();
		if (HasCompletedDefectRun() && !Progress.IsEpochObtained(id))
		{
			Log.Info("Player has already completed a Defect run, obtaining Fool1Epoch");
			Progress.ObtainEpoch(id);
			SaveProgress();
		}
	}

	private bool HasCompletedDefectRun()
	{
		CharacterStats statsForCharacter = Progress.GetStatsForCharacter(ModelDb.GetId<Defect>());
		if (statsForCharacter != null)
		{
			return statsForCharacter.TotalWins + statsForCharacter.TotalLosses > 0;
		}
		return false;
	}

	private void PostRunCharacterEpochChecks(SerializablePlayer serializablePlayer, SerializableRun serializableRun, bool victory)
	{
		if (victory)
		{
			CheckAscensionOneCompleted(serializablePlayer, serializableRun);
			string id = EpochModel.GetId<CustomAndSeedsEpoch>();
			if (Progress.Wins >= 3)
			{
				TryObtainEpochPostRun(EpochModel.Get(id), serializablePlayer, serializableRun);
			}
		}
	}

	private void CheckAscensionOneCompleted(SerializablePlayer serializablePlayer, SerializableRun serializableRun)
	{
		if (serializableRun.Ascension == 1)
		{
			CharacterModel byId = ModelDb.GetById<CharacterModel>(serializablePlayer.CharacterId);
			string text;
			if (byId is Ironclad)
			{
				text = EpochModel.GetId<Ironclad7Epoch>();
			}
			else if (byId is Silent)
			{
				text = EpochModel.GetId<Silent7Epoch>();
			}
			else if (byId is Regent)
			{
				text = EpochModel.GetId<Regent7Epoch>();
			}
			else if (byId is Defect)
			{
				text = EpochModel.GetId<Defect7Epoch>();
			}
			else if (byId is Necrobinder)
			{
				text = EpochModel.GetId<Necrobinder7Epoch>();
			}
			else
			{
				text = ((!(byId is Fool)) ? null : EpochModel.GetId<Fool7Epoch>());
			}
			string text2 = text;
			if (text2 != null)
			{
				TryObtainEpochPostRun(EpochModel.Get(text2), serializablePlayer, serializableRun);
			}
		}
	}

	private void CheckFifteenElitesDefeatedEpoch(Player localPlayer)
	{
		CharacterModel character = localPlayer.Character;
		EpochModel epochModel;
		if (!(character is Ironclad))
		{
			if (!(character is Silent))
			{
				if (!(character is Regent))
				{
					if (!(character is Defect))
					{
						if (!(character is Necrobinder))
						{
							if (!(character is Fool))
							{
								if (!(character is Deprived) && !(character is ClassicDefect) && !(character is ClassicSilent) && !(character is ClassicIronclad) && !(character is ClassicWatcher))
								{
									throw new ArgumentOutOfRangeException("character", character, null);
								}
								epochModel = null;
							}
							else
							{
								epochModel = EpochModel.Get(EpochModel.GetId<Fool5Epoch>());
							}
						}
						else
						{
							epochModel = EpochModel.Get(EpochModel.GetId<Necrobinder5Epoch>());
						}
					}
					else
					{
						epochModel = EpochModel.Get(EpochModel.GetId<Defect5Epoch>());
					}
				}
				else
				{
					epochModel = EpochModel.Get(EpochModel.GetId<Regent5Epoch>());
				}
			}
			else
			{
				epochModel = EpochModel.Get(EpochModel.GetId<Silent5Epoch>());
			}
		}
		else
		{
			epochModel = EpochModel.Get(EpochModel.GetId<Ironclad5Epoch>());
		}
		EpochModel epochModel2 = epochModel;
		if (epochModel2 == null)
		{
			return;
		}
		HashSet<ModelId> eliteEncounters = GetEliteEncounters();
		int num = 0;
		foreach (EncounterStats value in Progress.EncounterStats.Values)
		{
			if (!eliteEncounters.Contains(value.Id))
			{
				continue;
			}
			foreach (FightStats fightStat in value.FightStats)
			{
				if (fightStat.Character == character.Id)
				{
					num += fightStat.Wins;
					break;
				}
			}
		}
		Log.Info($"Elites Defeated: {num}/{eliteEncounters.Count}");
		if (num >= 15)
		{
			TryObtainEpochMidRun(epochModel2, localPlayer);
		}
	}

	private void CheckFifteenBossesDefeatedEpoch(Player localPlayer)
	{
		CharacterModel character = localPlayer.Character;
		EpochModel epochModel;
		if (!(character is Ironclad))
		{
			if (!(character is Silent))
			{
				if (!(character is Regent))
				{
					if (!(character is Defect))
					{
						if (!(character is Necrobinder))
						{
							if (!(character is Deprived) && !(character is ClassicDefect) && !(character is ClassicSilent) && !(character is ClassicIronclad) && !(character is ClassicWatcher))
							{
								if (!(character is Fool))
								{
									throw new ArgumentOutOfRangeException("character", character, null);
								}
								epochModel = EpochModel.Get(EpochModel.GetId<Fool6Epoch>());
							}
							else
							{
								epochModel = null;
							}
						}
						else
						{
							epochModel = EpochModel.Get(EpochModel.GetId<Necrobinder6Epoch>());
						}
					}
					else
					{
						epochModel = EpochModel.Get(EpochModel.GetId<Defect6Epoch>());
					}
				}
				else
				{
					epochModel = EpochModel.Get(EpochModel.GetId<Regent6Epoch>());
				}
			}
			else
			{
				epochModel = EpochModel.Get(EpochModel.GetId<Silent6Epoch>());
			}
		}
		else
		{
			epochModel = EpochModel.Get(EpochModel.GetId<Ironclad6Epoch>());
		}
		EpochModel epochModel2 = epochModel;
		if (epochModel2 == null)
		{
			return;
		}
		HashSet<ModelId> hashSet = ModelDb.Acts.SelectMany((ActModel a) => a.AllBossEncounters.Select((EncounterModel e) => e.Id)).ToHashSet();
		int num = 0;
		foreach (EncounterStats value in Progress.EncounterStats.Values)
		{
			if (!hashSet.Contains(value.Id))
			{
				continue;
			}
			foreach (FightStats fightStat in value.FightStats)
			{
				if (fightStat.Character == character.Id)
				{
					num += fightStat.Wins;
					break;
				}
			}
		}
		if (num >= 15)
		{
			TryObtainEpochMidRun(epochModel2, localPlayer);
		}
	}

	private void ObtainCharUnlockEpoch(Player localPlayer, int act)
	{
		if (localPlayer.Character.IsPlayable && !(localPlayer.Character is ClassicDefect) && !(localPlayer.Character is ClassicSilent) && !(localPlayer.Character is ClassicIronclad) && !(localPlayer.Character is ClassicWatcher))
		{
			string text = localPlayer.Character.Id.Entry.ToUpperInvariant();
			EpochModel epochModel = null;
			switch (act)
			{
			case 0:
				epochModel = EpochModel.Get(text + "2_EPOCH");
				break;
			case 1:
				epochModel = EpochModel.Get(text + "3_EPOCH");
				break;
			case 2:
				epochModel = EpochModel.Get(text + "4_EPOCH");
				break;
			case 3:
				Log.Error($"Act {act + 1} is not yet implemented.");
				break;
			default:
				Log.Error($"Unsupported Act: {act}");
				break;
			}
			if (epochModel == null)
			{
				Log.Error("EpochModel was not found :(");
			}
			else if (TryObtainEpochMidRun(epochModel, localPlayer))
			{
				Log.Info($"Epoch obtained for completing Act {act + 1}");
			}
		}
	}

	private bool TryObtainEpochMidRun(EpochModel epoch, Player localPlayer)
	{
		if (GameModeHelper.AreAchievementsAndEpochsLockedForRun(localPlayer.RunState))
		{
			return false;
		}
		if (localPlayer.RunState.IsCustomGame)
		{
			return false;
		}
		if (!TryObtainEpochInternal(epoch))
		{
			return false;
		}
		localPlayer.DiscoveredEpochs.Add(epoch.Id);
		return true;
	}

	private bool TryObtainEpochPostRun(EpochModel epoch, SerializablePlayer serializablePlayer, SerializableRun serializableRun)
	{
		if (GameModeHelper.AreAchievementsAndEpochsLockedForSerializableRun(serializableRun))
		{
			return false;
		}
		if (serializableRun.IsCustomGame)
		{
			return false;
		}
		if (!TryObtainEpochInternal(epoch))
		{
			return false;
		}
		serializablePlayer.DiscoveredEpochs.Add(epoch.Id);
		return true;
	}

	private bool TryObtainEpochInternal(EpochModel epoch)
	{
		if (Progress.IsEpochObtained(epoch.Id))
		{
			Log.Info("Player already has Epoch: " + epoch.Id);
			return false;
		}
		Progress.ObtainEpoch(epoch.Id);
		NGame.Instance?.AddChildSafely(NGainEpochVfx.Create(epoch));
		if (GetRevealableEpochs().All((SerializableEpoch e) => e.Id != epoch.Id))
		{
			string text = "Epoch " + epoch.Id + " was obtained, but is not yet revealable by the player!";
			Log.Warn(text);
			SentryService.CaptureMessage(text, SentryLevel.Info, (Scope scope) =>
			{
				string text2 = JsonSerializer.Serialize(Progress.Epochs, JsonSerializationUtility.GetTypeInfo<List<SerializableEpoch>>());
				scope.AddCompressedAttachment(text2, "epochs.txt.gz");
			});
		}
		return true;
	}

	public IEnumerable<SerializableEpoch> GetRevealableEpochs()
	{
		HashSet<string> satisfiedEpochIds = new HashSet<string>(from e in Progress.Epochs.Where((SerializableEpoch e) =>
			{
				EpochState state2 = e.State;
				return (uint)(state2 - 3) <= 1u;
			})
			select e.Id);
		HashSet<string> reachableSet = new HashSet<string>();
		Queue<string> queue = new Queue<string>();
		string id = EpochModel.Get<NeowEpoch>().Id;
		reachableSet.Add(id);
		queue.Enqueue(id);
		foreach (SerializableEpoch epoch in Progress.Epochs)
		{
			bool flag = epoch.Id == id;
			bool flag2 = flag;
			if (!flag2)
			{
				EpochState state = epoch.State;
				bool flag3 = ((state == EpochState.None || (uint)(state - 2) <= 1u) ? true : false);
				flag2 = flag3;
			}
			if (!flag2)
			{
				reachableSet.Add(epoch.Id);
				queue.Enqueue(epoch.Id);
			}
		}
		while (queue.Count > 0)
		{
			string id2 = queue.Dequeue();
			EpochModel[] timelineExpansion = EpochModel.Get(id2).GetTimelineExpansion();
			foreach (EpochModel epochModel in timelineExpansion)
			{
				if (!reachableSet.Contains(epochModel.Id))
				{
					reachableSet.Add(epochModel.Id);
					if (satisfiedEpochIds.Contains(epochModel.Id))
					{
						queue.Enqueue(epochModel.Id);
					}
				}
			}
		}
		return Progress.Epochs.Where((SerializableEpoch e) => satisfiedEpochIds.Contains(e.Id) && reachableSet.Contains(e.Id));
	}

	public void UpdateAfterCombatWon(Player localPlayer, CombatRoom room)
	{
		CombatState combatState = room.CombatState;
		IRunState runState = combatState.RunState;
		CharacterModel character = localPlayer.Character;
		ModelId id = combatState.Encounter.Id;
		EncounterStats orCreateEncounterStats = Progress.GetOrCreateEncounterStats(id);
		FightStats fightStats = orCreateEncounterStats.FightStats.FirstOrDefault((FightStats f) => f.Character == character.Id);
		if (fightStats == null)
		{
			Log.Info($"{character.Id} fought {id} for the first time and WON >:)");
			FightStats item = new FightStats
			{
				Character = character.Id,
				Wins = 1,
				Losses = 0
			};
			orCreateEncounterStats.FightStats.Add(item);
		}
		else
		{
			orCreateEncounterStats.IncrementWin(character.Id);
		}
		if (room.RoomType == RoomType.Boss)
		{
			ObtainCharUnlockEpoch(localPlayer, runState.CurrentActIndex);
			CheckFifteenBossesDefeatedEpoch(localPlayer);
		}
		else if (room.RoomType == RoomType.Elite)
		{
			CheckFifteenElitesDefeatedEpoch(localPlayer);
		}
		foreach (MonsterModel spawnedEnemy in room.Encounter.SpawnedEnemies)
		{
			EnemyStats orCreateEnemyStats = Progress.GetOrCreateEnemyStats(spawnedEnemy.Id);
			bool flag = orCreateEnemyStats.FightStats.Count == 0;
			FightStats fightStats2 = orCreateEnemyStats.FightStats.FirstOrDefault((FightStats f) => f.Character == character.Id);
			if (fightStats2 == null)
			{
				Log.Info($"{character.Id} fought {spawnedEnemy.Id} for the first time and WON >:(");
				FightStats item2 = new FightStats
				{
					Character = character.Id,
					Wins = 1,
					Losses = 0
				};
				orCreateEnemyStats.FightStats.Add(item2);
				if (flag)
				{
					localPlayer.DiscoveredEnemies.Add(spawnedEnemy.Id);
				}
			}
			else
			{
				orCreateEnemyStats.IncrementWin(character.Id);
			}
		}
	}

	private static void IncrementSingleplayerAscension(SerializableRun run, CharacterStats charStats)
	{
		if (run.Ascension == charStats.MaxAscension)
		{
			if (charStats.MaxAscension < 10)
			{
				charStats.MaxAscension++;
				charStats.PreferredAscension = charStats.MaxAscension;
			}
		}
		else
		{
			Log.Info($"Not playing on max singleplayer ascension ({charStats.MaxAscension})");
		}
	}

	private void IncrementMultiplayerAscension(SerializableRun run)
	{
		if (run.Ascension == Progress.MaxMultiplayerAscension)
		{
			if (Progress.MaxMultiplayerAscension < 10)
			{
				Progress.MaxMultiplayerAscension++;
				Progress.PreferredMultiplayerAscension = Progress.MaxMultiplayerAscension;
			}
		}
		else
		{
			Log.Info($"Not playing on max multiplayer ascension ({Progress.MaxMultiplayerAscension})");
		}
	}

	private void IncrementBlitzAscension(SerializableRun run)
	{
		if (run.Ascension == Progress.MaxBlitzAscension)
		{
			if (Progress.MaxBlitzAscension < 5)
			{
				Progress.MaxBlitzAscension++;
				Progress.PreferredBlitzAscension = Progress.MaxBlitzAscension;
			}
		}
		else
		{
			Log.Info($"Not playing on max blitz ascension ({Progress.MaxBlitzAscension})");
		}
	}

	public bool SeenFtue(string ftueKey)
	{
		if (!Progress.EnableFtues)
		{
			return true;
		}
		return Progress.FtueCompleted.Contains(ftueKey);
	}

	public bool SeenPopup(string popupKey)
	{
		if (TestMode.IsOn)
		{
			return true;
		}
		return Progress.FtueCompleted.Contains(popupKey);
	}

	public void MarkFtueAsComplete(string ftueId)
	{
		if (Progress.MarkFtueAsComplete(ftueId))
		{
			Log.Info("Player has seen ftue " + ftueId + "!");
			SaveProgress();
		}
	}

	public void SetFtuesEnabled(bool enabled)
	{
		if (Progress.EnableFtues != enabled)
		{
			Log.Info($"Player has set FTUEs enabled: {enabled}");
			Progress.EnableFtues = enabled;
			SaveProgress();
		}
	}

	public void ResetFtues()
	{
		Log.Info("Player has reset FTUEs to enabled");
		Progress.ResetFtues();
		SaveProgress();
	}

	private static HashSet<ModelId> GetEliteEncounters()
	{
		return _eliteEncounters ?? (_eliteEncounters = (from e in ModelDb.AllEncounters
			where e.RoomType == RoomType.Elite
			select e.Id).Distinct().ToHashSet());
	}
}
