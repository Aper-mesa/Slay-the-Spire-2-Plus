using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Debug;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Save;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Multiplayer.Game;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Platform.Steam;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Runs.Metrics;
using MegaCrit.Sts2.Core.Saves.Managers;
using MegaCrit.Sts2.Core.Saves.Migrations;
using MegaCrit.Sts2.Core.Saves.Test;
using MegaCrit.Sts2.Core.TestSupport;
using MegaCrit.Sts2.Core.Timeline;
using MegaCrit.Sts2.Core.Timeline.Epochs;
using MegaCrit.Sts2.Core.Unlocks;
using Steamworks;

namespace MegaCrit.Sts2.Core.Saves;

public class SaveManager : IProfileIdProvider
{
	private static SaveManager? _mockInstance;

	private static SaveManager? _instance;

	private readonly SettingsSaveManager _settingsSaveManager;

	private readonly ProgressSaveManager _progressSaveManager;

	private readonly RunSaveManager _runSaveManager;

	private readonly RunHistorySaveManager _runHistorySaveManager;

	private readonly PrefsSaveManager _prefsSaveManager;

	private readonly ProfileSaveManager _profileSaveManager;

	private readonly ISaveStore _saveStore;

	private readonly MigrationManager _migrationManager;

	private int? _currentProfileId;

	public static SaveManager Instance
	{
		get
		{
			if (_mockInstance != null)
			{
				return _mockInstance;
			}
			if (_instance == null)
			{
				_instance = ConstructDefault();
			}
			return _instance;
		}
	}

	public static int TotalAgnosticUnlocks => EpochModel.AgnosticUnlockOrder.Count;

	public SettingsSave SettingsSave => _settingsSaveManager.Settings;

	public PrefsSave PrefsSave => _prefsSaveManager.Prefs;

	public bool IsPrefsLoaded => _prefsSaveManager.IsLoaded;

	public ProgressState Progress
	{
		get
		{
			return _progressSaveManager.Progress;
		}
		set
		{
			_progressSaveManager.Progress = value;
		}
	}

	public bool HasRunSave => _runSaveManager.HasRunSave;

	public bool HasMultiplayerRunSave => _runSaveManager.HasMultiplayerRunSave;

	public bool IsProfileInitialized => _currentProfileId.HasValue;

	public int CurrentProfileId => _currentProfileId ?? throw new InvalidOperationException("InitProfileId must be called on SaveManager!");

	public Task? CurrentRunSaveTask { get; private set; }

	public event Action? Saved
	{
		add
		{
			_runSaveManager.Saved += value;
		}
		remove
		{
			_runSaveManager.Saved -= value;
		}
	}

	public event Action<int>? ProfileIdChanged;

	public static void MockInstanceForTesting(SaveManager saveManager)
	{
		_mockInstance = saveManager;
	}

	public static void ClearInstanceForTesting()
	{
		_mockInstance = null;
	}

	public SaveManager(ISaveStore saveStore, bool forceSynchronous = false)
		: this(saveStore, saveStore, forceSynchronous)
	{
	}

	public SaveManager(ISaveStore saveStore, ISaveStore localOnlyStore, bool forceSynchronous = false)
	{
		_saveStore = saveStore;
		_migrationManager = new MigrationManager(saveStore);
		_settingsSaveManager = new SettingsSaveManager(localOnlyStore, _migrationManager);
		_profileSaveManager = new ProfileSaveManager(saveStore, _migrationManager);
		_prefsSaveManager = new PrefsSaveManager(saveStore, _migrationManager, this);
		_progressSaveManager = new ProgressSaveManager(saveStore, _migrationManager, this);
		_runSaveManager = new RunSaveManager(saveStore, _migrationManager, this, forceSynchronous);
		_runHistorySaveManager = new RunHistorySaveManager(saveStore, _migrationManager, this);
	}

	private static SaveManager ConstructDefault()
	{
		ISaveStore saveStore;
		if (TestMode.IsOn)
		{
			saveStore = new MockGodotFileIo("user://test");
			return new SaveManager(saveStore);
		}
		ISaveStore saveStore2 = new GodotFileIo(UserDataPathProvider.GetAccountScopedBasePath(null));
		saveStore = saveStore2;
		if (SteamInitializer.Initialized)
		{
			Log.Info($"Steam is enabled, we will write saves to steam storage. Enabled for account: {SteamRemoteStorage.IsCloudEnabledForAccount()}, app: {SteamRemoteStorage.IsCloudEnabledForApp()} ");
			SteamRemoteSaveStore cloudStore = new SteamRemoteSaveStore();
			CloudSaveStore cloudSaveStore = new CloudSaveStore(saveStore2, cloudStore);
			saveStore = cloudSaveStore;
		}
		return new SaveManager(saveStore, saveStore2);
	}

	public void InitProfileId(int? profileId = null)
	{
		CleanupTemporaryFiles();
		if (!profileId.HasValue)
		{
			_profileSaveManager.LoadProfile();
			_currentProfileId = _profileSaveManager.Profile.LastProfileId;
		}
		else
		{
			_currentProfileId = profileId.Value;
		}
		string profileScopedBasePath = UserDataPathProvider.GetProfileScopedBasePath(CurrentProfileId);
		Log.Info("Profile-scoped data path initialized: " + profileScopedBasePath);
		_runHistorySaveManager.CreateRunHistoryDirectory();
	}

	public int GetLatestSchemaVersion<T>()
	{
		return _migrationManager.GetLatestVersion<T>();
	}

	public string GetProfileScopedPath(string userData)
	{
		return _saveStore.GetFullPath(Path.Combine(UserDataPathProvider.GetProfileDir(CurrentProfileId), userData));
	}

	public void SwitchProfileId(int profileId)
	{
		Log.Info($"Switching save profiles to {profileId}");
		_currentProfileId = profileId;
		_profileSaveManager.Profile.LastProfileId = profileId;
		SaveProfile();
		_runHistorySaveManager.CreateRunHistoryDirectory();
		ProfileIdChanged?.Invoke(profileId);
	}

	public SaveBatchScope BeginSaveBatch()
	{
		if (_saveStore is CloudSaveStore cloudSaveStore)
		{
			cloudSaveStore.BeginSaveBatch();
		}
		return new SaveBatchScope(this);
	}

	public void EndSaveBatch()
	{
		if (_saveStore is CloudSaveStore cloudSaveStore)
		{
			cloudSaveStore.EndSaveBatch();
		}
	}

	public async Task SaveRun(AbstractRoom? preFinishedRoom, bool saveProgress = true)
	{
		if (CurrentRunSaveTask != null)
		{
			await CurrentRunSaveTask;
		}
		using (BeginSaveBatch())
		{
			if (saveProgress)
			{
				SaveProgressFile();
			}
			try
			{
				CurrentRunSaveTask = _runSaveManager.SaveRun(preFinishedRoom);
				await CurrentRunSaveTask;
			}
			catch (Exception ex)
			{
				Log.Error($"Failed to save run: {ex}");
				SentryService.CaptureException(ex);
			}
			finally
			{
				CurrentRunSaveTask = null;
			}
		}
	}

	public async Task IncrementNumReloads(SerializableRun save, NetGameType type, bool forceInTest = false)
	{
		save.NumReloads++;
		bool flag = forceInTest || TestMode.IsOff;
		bool flag2 = flag;
		if (flag2)
		{
			bool flag3 = (uint)(type - 1) <= 1u;
			flag2 = flag3;
		}
		if (!flag2)
		{
			return;
		}
		if (CurrentRunSaveTask != null)
		{
			await CurrentRunSaveTask;
		}
		using (BeginSaveBatch())
		{
			_ = 1;
			try
			{
				CurrentRunSaveTask = _runSaveManager.SaveRun(save, type.IsMultiplayer());
				await CurrentRunSaveTask;
			}
			finally
			{
				CurrentRunSaveTask = null;
			}
		}
	}

	public PostRunProgressUpdateResult UpdateProgressWithRunData(SerializableRun serializableRun, bool victory)
	{
		return _progressSaveManager.UpdateWithRunData(serializableRun, victory);
	}

	public void UpdateProgressAfterCombatWon(Player localPlayer, CombatRoom combatRoom)
	{
		_progressSaveManager.UpdateAfterCombatWon(localPlayer, combatRoom);
	}

	public void DeleteCurrentRun()
	{
		_runSaveManager.DeleteCurrentRun();
	}

	public void DeleteCurrentMultiplayerRun()
	{
		_runSaveManager.DeleteCurrentMultiplayerRun();
	}

	public void DeleteProfile(int profileId)
	{
		string profileScopedBasePath = UserDataPathProvider.GetProfileScopedBasePath(profileId);
		Log.Info($"DELETING the profile id {profileId} at path {profileScopedBasePath}!!");
		DeleteDirectoryRecursive(UserDataPathProvider.GetProfileDir(profileId));
	}

	public void DeleteDirectoryRecursive(string directory)
	{
		DeleteInDirectoryRecursive(directory);
		Log.Info("Deleting directory at " + directory);
		_saveStore.DeleteDirectory(directory);
	}

	private void DeleteInDirectoryRecursive(string directory)
	{
		string[] directoriesInDirectory = _saveStore.GetDirectoriesInDirectory(directory);
		foreach (string text in directoriesInDirectory)
		{
			string text2 = directory + "/" + text;
			DeleteInDirectoryRecursive(text2);
			Log.Info("Deleting directory at " + text2);
			_saveStore.DeleteDirectory(text2);
		}
		string[] filesInDirectory = _saveStore.GetFilesInDirectory(directory);
		foreach (string text3 in filesInDirectory)
		{
			string text4 = directory + "/" + text3;
			Log.Info("Deleting file at " + text4);
			_saveStore.DeleteFile(text4);
		}
		if (_saveStore is CloudSaveStore cloudSaveStore)
		{
			string[] filesInDirectory2 = cloudSaveStore.CloudStore.GetFilesInDirectory(directory);
			foreach (string text5 in filesInDirectory2)
			{
				string text6 = directory + "/" + text5;
				Log.Info("Deleting cloud-only file at " + text6);
				cloudSaveStore.CloudStore.DeleteFile(text6);
			}
		}
	}

	public void SaveSettings()
	{
		try
		{
			_settingsSaveManager.SaveSettings();
		}
		catch (Exception ex)
		{
			Log.Error($"Failed to save settings: {ex}");
			SentryService.CaptureException(ex);
		}
	}

	public void SaveProfile()
	{
		try
		{
			_profileSaveManager.SaveProfile();
		}
		catch (Exception ex)
		{
			Log.Error($"Failed to save profile: {ex}");
			SentryService.CaptureException(ex);
		}
	}

	public ReadSaveResult<SettingsSave> InitSettingsDataForTest()
	{
		_settingsSaveManager.Settings = new SettingsSave();
		return new ReadSaveResult<SettingsSave>(_settingsSaveManager.Settings);
	}

	public ReadSaveResult<PrefsSave> InitPrefsDataForTest()
	{
		_prefsSaveManager.Prefs = new PrefsSave();
		return new ReadSaveResult<PrefsSave>(_prefsSaveManager.Prefs);
	}

	public ReadSaveResult<SettingsSave> InitSettingsData()
	{
		return _settingsSaveManager.LoadSettings();
	}

	public ReadSaveResult<PrefsSave> InitPrefsData()
	{
		return _prefsSaveManager.LoadPrefs();
	}

	public ReadSaveResult<SerializableProgress> InitProgressData()
	{
		return _progressSaveManager.LoadProgress();
	}

	public async Task SyncCloudToLocal()
	{
		if (!(_saveStore is CloudSaveStore cloudStore))
		{
			return;
		}
		Log.Info("Syncing cloud save files to the local save directory");
		DeleteSettingsFromCloud(cloudStore);
		List<Task> tasks = new List<Task>();
		foreach (Task item in EnumerateCloudSyncTasks(cloudStore))
		{
			tasks.Add(item);
			if (tasks.Count >= 8)
			{
				await Task.WhenAll(tasks);
				tasks.Clear();
			}
		}
		if (tasks.Count > 0)
		{
			await Task.WhenAll(tasks);
		}
		CleanupStaleCurrentRunSaves();
	}

	private IEnumerable<Task> EnumerateCloudSyncTasks(CloudSaveStore cloudStore)
	{
		yield return cloudStore.SyncCloudToLocal(ProfileSaveManager.GetProfileSavePath());
		for (int i = 1; i <= 3; i++)
		{
			yield return cloudStore.SyncCloudToLocal(ProgressSaveManager.GetProgressPathForProfile(i));
			yield return cloudStore.SyncCloudToLocal(RunSaveManager.GetRunSavePath(i, "current_run.save"));
			yield return cloudStore.SyncCloudToLocal(RunSaveManager.GetRunSavePath(i, "current_run_mp.save"));
			yield return cloudStore.SyncCloudToLocal(PrefsSaveManager.GetPrefsPath(i));
			foreach (Task item in cloudStore.SyncCloudToLocalDirectory(RunHistorySaveManager.GetHistoryPath(i)))
			{
				yield return item;
			}
		}
	}

	private static void DeleteSettingsFromCloud(CloudSaveStore cloudStore)
	{
		try
		{
			if (cloudStore.CloudStore.FileExists("settings.save"))
			{
				Log.Info("Deleting stale settings.save from cloud storage");
				cloudStore.CloudStore.DeleteFile("settings.save");
			}
		}
		catch (Exception ex)
		{
			Log.Warn("Failed to delete settings from cloud: " + ex.Message);
		}
	}

	private void CleanupStaleCurrentRunSaves()
	{
		for (int i = 1; i <= 3; i++)
		{
			CleanupStaleCurrentRunSaveForProfile(i, "current_run.save");
			CleanupStaleCurrentRunSaveForProfile(i, "current_run_mp.save");
		}
	}

	private void CleanupStaleCurrentRunSaveForProfile(int profileId, string runSaveFileName)
	{
		string runSavePath = RunSaveManager.GetRunSavePath(profileId, runSaveFileName);
		string text = runSavePath + ".backup";
		string text2 = null;
		if (_saveStore.FileExists(runSavePath))
		{
			text2 = runSavePath;
		}
		else if (_saveStore.FileExists(text))
		{
			text2 = text;
		}
		if (text2 == null)
		{
			return;
		}
		try
		{
			string text3 = _saveStore.ReadFile(text2);
			if (text3 == null)
			{
				Log.Warn("Could not read " + text2 + ", skipping staleness check");
				return;
			}
			long? value = ExtractStartTimeFromRunSave(text3);
			if (!value.HasValue)
			{
				Log.Warn("Could not extract start_time from " + text2 + ", skipping staleness check");
				return;
			}
			string text4 = Path.Combine(RunHistorySaveManager.GetHistoryPath(profileId), $"{value}.run");
			if (_saveStore.FileExists(text4))
			{
				Log.Warn($"Deleting stale {runSaveFileName} for profile {profileId}: run with StartTime {value} already exists in history at {text4}");
				_saveStore.DeleteFile(runSavePath);
				_saveStore.DeleteFile(runSavePath + ".backup");
			}
		}
		catch (Exception ex)
		{
			Log.Warn($"Error checking for stale current_run.save in profile {profileId}: {ex.Message}");
		}
	}

	private void CleanupTemporaryFiles()
	{
		_saveStore.DeleteTemporaryFiles("");
		for (int i = 1; i <= 3; i++)
		{
			string directoryPath = Path.Combine(UserDataPathProvider.GetProfileDir(i), UserDataPathProvider.SavesDir);
			_saveStore.DeleteTemporaryFiles(directoryPath);
			_saveStore.DeleteTemporaryFiles(RunHistorySaveManager.GetHistoryPath(i));
		}
	}

	private static long? ExtractStartTimeFromRunSave(string json)
	{
		try
		{
			using JsonDocument jsonDocument = JsonDocument.Parse(json);
			if (jsonDocument.RootElement.TryGetProperty("start_time", out var value))
			{
				return value.GetInt64();
			}
		}
		catch
		{
		}
		return null;
	}

	public bool ShouldOverwriteCloudWithLocal()
	{
		if (!(_saveStore is CloudSaveStore cloudSaveStore))
		{
			return false;
		}
		if (!cloudSaveStore.HasUserEnabledCloudSync())
		{
			return true;
		}
		if (!_saveStore.FileExists(ProfileSaveManager.GetProfileSavePath()))
		{
			return false;
		}
		if (cloudSaveStore.HasCloudFiles())
		{
			return false;
		}
		return true;
	}

	public async Task OverwriteCloudWithLocal()
	{
		if (!(_saveStore is CloudSaveStore cloudStore))
		{
			return;
		}
		Log.Info("OVERWRITING cloud saves with local saves.");
		List<Task> tasks = new List<Task>();
		foreach (Task item in EnumerateOverwriteCloudWithLocalTasks(cloudStore))
		{
			tasks.Add(item);
			if (tasks.Count >= 8)
			{
				await Task.WhenAll(tasks);
				tasks.Clear();
			}
		}
		if (tasks.Count > 0)
		{
			await Task.WhenAll(tasks);
		}
	}

	private IEnumerable<Task> EnumerateOverwriteCloudWithLocalTasks(CloudSaveStore cloudStore)
	{
		yield return cloudStore.OverwriteCloudWithLocal(ProfileSaveManager.GetProfileSavePath());
		for (int i = 1; i <= 3; i++)
		{
			yield return cloudStore.OverwriteCloudWithLocal(ProgressSaveManager.GetProgressPathForProfile(i));
			yield return cloudStore.OverwriteCloudWithLocal(RunSaveManager.GetRunSavePath(i, "current_run.save"));
			yield return cloudStore.OverwriteCloudWithLocal(RunSaveManager.GetRunSavePath(i, "current_run_mp.save"));
			yield return cloudStore.OverwriteCloudWithLocal(PrefsSaveManager.GetPrefsPath(i));
			foreach (Task item in cloudStore.OverwriteCloudWithLocalDirectory(RunHistorySaveManager.GetHistoryPath(i), 5242880, 100))
			{
				yield return item;
			}
		}
	}

	public ReadSaveResult<SerializableRun> LoadRunSave()
	{
		return _runSaveManager.LoadRunSave();
	}

	public ReadSaveResult<SerializableRun> LoadAndCanonicalizeMultiplayerRunSave(ulong localPlayerId)
	{
		return _runSaveManager.LoadAndCanonicalizeMultiplayerRunSave(localPlayerId);
	}

	public void SaveRunHistory(RunHistory history)
	{
		try
		{
			_runHistorySaveManager.SaveHistory(history);
		}
		catch (Exception ex)
		{
			Log.Error($"Failed to save run history: {ex}");
			SentryService.CaptureException(ex);
		}
	}

	public int GetRunHistoryCount()
	{
		return _runHistorySaveManager.GetHistoryCount();
	}

	public List<string> GetAllRunHistoryNames()
	{
		return _runHistorySaveManager.LoadAllRunHistoryNames();
	}

	public ReadSaveResult<RunHistory> LoadRunHistory(string fileName)
	{
		return _runHistorySaveManager.LoadHistory(fileName);
	}

	public static string ToJson<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods)] T>(T obj) where T : ISaveSchema
	{
		return JsonSerializationUtility.ToJson(obj);
	}

	public static ReadSaveResult<T> FromJson<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods)] T>(string json) where T : ISaveSchema, new()
	{
		return JsonSerializationUtility.FromJson<T>(json);
	}

	public bool SeenFtue(string ftueKey)
	{
		return _progressSaveManager.SeenFtue(ftueKey);
	}

	public bool SeenPopup(string popupKey)
	{
		return _progressSaveManager.SeenPopup(popupKey);
	}

	public void SaveProgressFile()
	{
		_progressSaveManager.SaveProgress();
	}

	public UnlockState GenerateUnlockStateFromProgress()
	{
		return _progressSaveManager.GenerateUnlockState();
	}

	public void SavePrefsFile()
	{
		try
		{
			_prefsSaveManager.SavePrefs();
		}
		catch (Exception ex)
		{
			Log.Error($"Failed to save prefs: {ex}");
			SentryService.CaptureException(ex);
		}
	}

	public void MarkFtueAsComplete(string ftueId)
	{
		_progressSaveManager.MarkFtueAsComplete(ftueId);
	}

	public void SetFtuesEnabled(bool enabled)
	{
		_progressSaveManager.SetFtuesEnabled(enabled);
	}

	public void ResetFtues()
	{
		_progressSaveManager.ResetFtues();
	}

	public void MarkPotionAsSeen(PotionModel potion)
	{
		_progressSaveManager.MarkPotionAsSeen(potion);
	}

	public void MarkCardAsSeen(CardModel card)
	{
		_progressSaveManager.MarkCardAsSeen(card);
	}

	public void MarkRelicAsSeen(RelicModel relic)
	{
		_progressSaveManager.MarkRelicAsSeen(relic);
	}

	public bool IsRelicSeen(RelicModel relic)
	{
		return Progress.DiscoveredRelics.Contains(relic.Id);
	}

	public void UnlockSlot(string epochId)
	{
		Progress.UnlockSlot(epochId);
	}

	public void ObtainEpoch(string epochId)
	{
		Progress.ObtainEpoch(epochId);
	}

	public void ObtainEpochOverride(string epochId, EpochState state)
	{
		Progress.ObtainEpochOverride(epochId, state);
	}

	public void ObtainFoolEpochIfDefectRunCompleted()
	{
		_progressSaveManager.ObtainFoolEpochIfDefectRunCompleted();
	}

	public void RevealEpoch(string epochId, bool isDebug = false)
	{
		Progress.RevealEpoch(epochId);
		if (!isDebug)
		{
			MetricUtilities.UploadEpochMetric(epochId);
		}
	}

	public void ResetTimelineProgress()
	{
		Progress.ResetEpochs();
		ObtainEpochOverride(EpochModel.GetId<NeowEpoch>(), EpochState.Obtained);
		SaveProgressFile();
	}

	public bool IsEpochRevealed<T>() where T : EpochModel
	{
		return Progress.IsEpochRevealed(EpochModel.GetId<T>());
	}

	public bool IsEpochRevealed(string id)
	{
		return Progress.IsEpochRevealed(id);
	}

	public int GetTotalUnlockedCards()
	{
		return GetCardUnlockEpochIds().Count(IsEpochRevealed) * 3;
	}

	public static int GetUnlockableCardCount()
	{
		return GetCardUnlockEpochIds().Length * 3;
	}

	private static string[] GetCardUnlockEpochIds()
	{
		return new string[23]
		{
			EpochModel.GetId<Colorless1Epoch>(),
			EpochModel.GetId<Colorless2Epoch>(),
			EpochModel.GetId<Colorless3Epoch>(),
			EpochModel.GetId<Colorless4Epoch>(),
			EpochModel.GetId<Colorless5Epoch>(),
			EpochModel.GetId<Ironclad2Epoch>(),
			EpochModel.GetId<Ironclad5Epoch>(),
			EpochModel.GetId<Ironclad7Epoch>(),
			EpochModel.GetId<Silent2Epoch>(),
			EpochModel.GetId<Silent5Epoch>(),
			EpochModel.GetId<Silent7Epoch>(),
			EpochModel.GetId<Regent2Epoch>(),
			EpochModel.GetId<Regent5Epoch>(),
			EpochModel.GetId<Regent7Epoch>(),
			EpochModel.GetId<Defect2Epoch>(),
			EpochModel.GetId<Defect5Epoch>(),
			EpochModel.GetId<Defect7Epoch>(),
			EpochModel.GetId<Necrobinder2Epoch>(),
			EpochModel.GetId<Necrobinder5Epoch>(),
			EpochModel.GetId<Necrobinder7Epoch>(),
			EpochModel.GetId<Fool2Epoch>(),
			EpochModel.GetId<Fool5Epoch>(),
			EpochModel.GetId<Fool7Epoch>()
		};
	}

	public int GetTotalUnlockedRelics()
	{
		return GetRelicUnlockEpochIds().Count(IsEpochRevealed) * 3;
	}

	public static int GetUnlockableRelicCount()
	{
		return GetRelicUnlockEpochIds().Length * 3;
	}

	private static string[] GetRelicUnlockEpochIds()
	{
		return new string[17]
		{
			EpochModel.GetId<Relic1Epoch>(),
			EpochModel.GetId<Relic2Epoch>(),
			EpochModel.GetId<Relic3Epoch>(),
			EpochModel.GetId<Relic4Epoch>(),
			EpochModel.GetId<Relic5Epoch>(),
			EpochModel.GetId<Ironclad3Epoch>(),
			EpochModel.GetId<Ironclad6Epoch>(),
			EpochModel.GetId<Silent3Epoch>(),
			EpochModel.GetId<Silent6Epoch>(),
			EpochModel.GetId<Regent3Epoch>(),
			EpochModel.GetId<Regent6Epoch>(),
			EpochModel.GetId<Defect3Epoch>(),
			EpochModel.GetId<Defect6Epoch>(),
			EpochModel.GetId<Necrobinder3Epoch>(),
			EpochModel.GetId<Necrobinder6Epoch>(),
			EpochModel.GetId<Fool3Epoch>(),
			EpochModel.GetId<Fool6Epoch>()
		};
	}

	public int GetTotalUnlockedPotions()
	{
		return GetPotionUnlockEpochIds().Count(IsEpochRevealed) * 3;
	}

	public static int GetUnlockablePotionCount()
	{
		return GetPotionUnlockEpochIds().Length * 3;
	}

	private static string[] GetPotionUnlockEpochIds()
	{
		return new string[8]
		{
			EpochModel.GetId<Potion1Epoch>(),
			EpochModel.GetId<Potion2Epoch>(),
			EpochModel.GetId<Ironclad4Epoch>(),
			EpochModel.GetId<Silent4Epoch>(),
			EpochModel.GetId<Regent4Epoch>(),
			EpochModel.GetId<Defect4Epoch>(),
			EpochModel.GetId<Necrobinder4Epoch>(),
			EpochModel.GetId<Fool4Epoch>()
		};
	}

	public int GetAggregateAscensionProgress()
	{
		return Progress.CharacterStats.Values.Sum((CharacterStats stat) => stat.MaxAscension);
	}

	public static int GetAggregateAscensionCount()
	{
		return ModelDb.AllCharacters.Count((CharacterModel c) => !(c is MegaCrit.Sts2.Core.Models.Characters.ClassicDefect) && !(c is MegaCrit.Sts2.Core.Models.Characters.ClassicSilent) && !(c is MegaCrit.Sts2.Core.Models.Characters.ClassicIronclad) && !(c is MegaCrit.Sts2.Core.Models.Characters.ClassicWatcher)) * 10;
	}

	public int GetTotalKills()
	{
		return Progress.EnemyStats.Values.Sum((EnemyStats enemy) => enemy.TotalWins);
	}

	public IEnumerable<SerializableEpoch> GetRevealableEpochs()
	{
		return _progressSaveManager.GetRevealableEpochs();
	}

	public int GetDiscoveredEpochCount()
	{
		return GetRevealableEpochs().Count();
	}

	public bool IsNeowDiscovered()
	{
		SerializableEpoch serializableEpoch = Progress.Epochs.FirstOrDefault((SerializableEpoch e) => e.Id == EpochModel.GetId<NeowEpoch>());
		if (serializableEpoch == null)
		{
			return false;
		}
		return serializableEpoch.State != EpochState.Revealed;
	}

	public int GetUnlocksRemaining()
	{
		return TotalAgnosticUnlocks - Progress.TotalUnlocks;
	}

	public int GetCurrentScore()
	{
		return Progress.CurrentScore;
	}

	public EpochModel? GrantNextUnlock()
	{
		string text = Progress.GrantNextUnlock();
		if (text != null)
		{
			return EpochModel.Get(text);
		}
		return null;
	}

	public bool IsCompendiumAvailable()
	{
		if (Progress.NumberOfRuns <= 0)
		{
			return !NGame.IsReleaseGame();
		}
		return true;
	}
}
