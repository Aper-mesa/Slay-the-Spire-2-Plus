using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Ascension;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Characters;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Saves;

namespace MegaCrit.Sts2.Core.Helpers;

public static class AscensionHelper
{
	public const decimal povertyAscensionGoldMultiplier = 0.75m;

	/// <summary>
	/// Return a different value depending on whether the given ascension is higher or lower than the current
	/// ascension of the run.
	/// </summary>
	/// <param name="level">The value that we use for ascensions below the ascensionThreshold</param>
	/// <param name="ascensionValue">The value that we use for ascensions above the ascensionThreshold</param>
	/// <param name="fallbackValue">The value that we use for ascensions below the ascensionThreshold</param>
	public static int GetValueIfAscension(AscensionLevel level, int ascensionValue, int fallbackValue)
	{
		if (!HasAscension(level))
		{
			return fallbackValue;
		}
		return ascensionValue;
	}

	public static float GetValueIfAscension(AscensionLevel level, float ascensionValue, float fallbackValue)
	{
		if (!HasAscension(level))
		{
			return fallbackValue;
		}
		return ascensionValue;
	}

	public static decimal GetValueIfAscension(AscensionLevel level, decimal ascensionValue, decimal fallbackValue)
	{
		if (!HasAscension(level))
		{
			return fallbackValue;
		}
		return ascensionValue;
	}

	public static bool HasAscension(AscensionLevel level)
	{
		return RunManager.Instance.HasAscension(level);
	}

	public static bool IsDread()
	{
		return RunManager.Instance.IsDread();
	}

	public static AscensionLevel GetLevelForBlitz(int blitzLevel)
	{
		return blitzLevel switch
		{
			0 => AscensionLevel.None,
			1 => AscensionLevel.WearyTraveler,
			2 => AscensionLevel.TightBelt,
			3 => AscensionLevel.ToughEnemies,
			4 => AscensionLevel.DeadlyEnemies,
			5 => AscensionLevel.AscendersBane,
			_ => throw new InvalidOperationException($"Invalid blitz ascension level {blitzLevel}"),
		};
	}

	public static LocString GetTitle(int level, GameMode gameMode)
	{
		return new LocString("ascension", "LEVEL_" + GetKey(level, gameMode) + ".title");
	}

	public static LocString GetDescription(int level, GameMode gameMode)
	{
		return new LocString("ascension", "LEVEL_" + GetKey(level, gameMode) + ".description");
	}

	private static string GetKey(int level, GameMode gameMode)
	{
		return ((gameMode != GameMode.Blitz) ? level : ((int)GetLevelForBlitz(level))).ToString("D2");
	}

	public static HoverTip GetHoverTip(CharacterModel character, int level, GameMode gameMode, bool isCustomGame)
	{
		LocString locString;
		if (level > 0)
		{
			locString = new LocString("ascension", "PORTRAIT_TITLE");
			locString.Add("character", character.Title);
			locString.Add("ascension", level);
		}
		else
		{
			locString = new LocString("ascension", "PORTRAIT_TITLE_NO_ASCENSION");
			locString.Add("character", character.Title);
		}
		LocString locString2 = new LocString("ascension", "PORTRAIT_DESCRIPTION");
		List<string> list = new List<string>();
		for (int i = 1; i <= level; i++)
		{
			list.Add(GetTitle(i, gameMode).GetFormattedText());
		}
		locString2.Add("ascensions", list);
		if (GameModeHelper.AreAchievementsAndEpochsLocked(gameMode, isCustomGame))
		{
			if (level == 0)
			{
				LocString locString3 = new LocString("gameplay_ui", "ACHIEVEMENTS_LOCKED");
				return new HoverTip(locString, locString2.GetFormattedText() + locString3.GetFormattedText())
				{
					ShouldOverrideTextOverflow = true
				};
			}
			LocString locString4 = new LocString("gameplay_ui", "ACHIEVEMENTS_LOCKED");
			return new HoverTip(locString, locString2.GetFormattedText() + "\n" + locString4.GetFormattedText())
			{
				ShouldOverrideTextOverflow = true
			};
		}
		return new HoverTip(locString, locString2)
		{
			ShouldOverrideTextOverflow = true
		};
	}

	/// <summary>
	/// Builds the hover tip shown on the top bar character portrait. Always contains the character name and game mode,
	/// followed by any ascension details and the achievements-locked warning.
	/// </summary>
	public static HoverTip GetPortraitHoverTip(IRunState runState)
	{
		Player me = LocalContext.GetMe(runState);
		CharacterModel character = me.Character;
		bool isMultiplayer = runState.Players.Count > 1;
		int ascensionLevel = runState.AscensionLevel;
		LocString locString = new LocString("ascension", IsModded(character) ? "PORTRAIT_TITLE_MODDED" : "PORTRAIT_TITLE_NO_ASCENSION");
		locString.Add("character", character.Title);
		int num = 1;
		List<string> list = new List<string>(num);
		CollectionsMarshal.SetCount(list, num);
		Span<string> span = CollectionsMarshal.AsSpan(list);
		int index = 0;
		span[index] = GetGameModeLine(runState.GameMode, isMultiplayer).GetFormattedText();
		List<string> list2 = list;
		if (runState.GameMode == GameMode.Dread)
		{
			list2.Add(GetDreadDescription(character, isMultiplayer, me.ExtraFields.IsPlayingRotatingCharacter));
		}
		else if (ascensionLevel > 0)
		{
			LocString locString2 = new LocString("ascension", "PORTRAIT_ASCENSION_HEADER");
			locString2.Add("ascension", ascensionLevel);
			LocString locString3 = new LocString("ascension", "PORTRAIT_DESCRIPTION");
			List<string> list3 = new List<string>();
			for (int i = 1; i <= ascensionLevel; i++)
			{
				list3.Add(GetTitle(i, runState.GameMode).GetFormattedText());
			}
			locString3.Add("ascensions", list3);
			list2.Add(locString2.GetFormattedText() + "\n" + locString3.GetFormattedText());
		}
		if (GameModeHelper.AreAchievementsAndEpochsLockedForRun(runState))
		{
			list2.Add(new LocString("gameplay_ui", "ACHIEVEMENTS_LOCKED").GetFormattedText());
		}
		return new HoverTip(locString, string.Join("\n\n", list2))
		{
			ShouldOverrideTextOverflow = true
		};
	}

	private static LocString GetGameModeLine(GameMode gameMode, bool isMultiplayer)
	{
		bool flag = !Enum.IsDefined(gameMode);
		LocString variable = new LocString("ascension", gameMode switch
		{
			GameMode.Standard => "PORTRAIT_GAME_MODE.standard",
			GameMode.Daily => "PORTRAIT_GAME_MODE.daily",
			GameMode.Dread => "PORTRAIT_GAME_MODE.dread",
			GameMode.Blitz => "PORTRAIT_GAME_MODE.blitz",
			_ => "PORTRAIT_GAME_MODE.unknown",
		});
		if (flag)
		{
			LocString locString = new LocString("ascension", "PORTRAIT_GAME_MODE_MODDED");
			locString.Add("gameMode", variable);
			variable = locString;
		}
		if (isMultiplayer)
		{
			LocString locString2 = new LocString("ascension", "PORTRAIT_GAME_MODE_MULTIPLAYER");
			locString2.Add("gameMode", variable);
			variable = locString2;
		}
		LocString locString3 = new LocString("ascension", "PORTRAIT_GAME_MODE");
		locString3.Add("gameMode", variable);
		return locString3;
	}

	public static int GetDreadStreak(IRunState runState)
	{
		Player me = LocalContext.GetMe(runState);
		return GetDreadStreak(me.Character, runState.Players.Count > 1, me.ExtraFields.IsPlayingRotatingCharacter);
	}

	public static int GetDreadStreak(CharacterModel character, bool isMultiplayer, bool isRotating)
	{
		CharacterStats statsForCharacter = SaveManager.Instance.Progress.GetStatsForCharacter(isRotating ? ModelDb.GetId<RotatingCharacter>() : character.Id);
		return ((!isMultiplayer) ? statsForCharacter?.CurrentSingleplayerDreadWinStreak : statsForCharacter?.CurrentMultiplayerDreadWinStreak).GetValueOrDefault();
	}

	private static string GetDreadDescription(CharacterModel character, bool isMultiplayer, bool isRotating)
	{
		int dreadStreak = GetDreadStreak(character, isMultiplayer, isRotating);
		LocString locString = new LocString("ascension", "DREAD_PORTRAIT_DESCRIPTION");
		int num = 4;
		List<string> list = new List<string>(num);
		CollectionsMarshal.SetCount(list, num);
		Span<string> span = CollectionsMarshal.AsSpan(list);
		int num2 = 0;
		span[num2] = new LocString("ascension", "DREAD_RULE_01").GetFormattedText();
		num2++;
		span[num2] = new LocString("ascension", "DREAD_RULE_02").GetFormattedText();
		num2++;
		span[num2] = new LocString("ascension", "DREAD_RULE_03").GetFormattedText();
		num2++;
		span[num2] = new LocString("ascension", "DREAD_RULE_04").GetFormattedText();
		List<string> variable = list;
		locString.Add("dreadRules", variable);
		if (dreadStreak <= 0)
		{
			return locString.GetFormattedText().TrimStart('\n');
		}
		LocString locString2 = new LocString("ascension", "PORTRAIT_DREAD_STREAK_HEADER");
		locString2.Add("streak", dreadStreak);
		return locString2.GetFormattedText() + locString.GetFormattedText();
	}

	/// <summary>
	/// Whether the character was added by a mod rather than shipped with the base game.
	/// </summary>
	private static bool IsModded(CharacterModel character)
	{
		return character.GetType().Assembly != typeof(CharacterModel).Assembly;
	}

	public static ModelId GetNextRotatingDreadCharacterId(bool isMultiplayer)
	{
		return GetRotatingCharacterIdForStreak(GetCurrentRotatingDreadStartingCharacter(isMultiplayer), GetCurrentRotatingDreadWinStreak(isMultiplayer));
	}

	public static CharacterModel GetNextRotatingDreadCharacter(bool isMultiplayer)
	{
		return ModelDb.GetById<CharacterModel>(GetNextRotatingDreadCharacterId(isMultiplayer));
	}

	public static int GetCurrentRotatingDreadWinStreak(bool isMultiplayer)
	{
		CharacterStats orCreateCharacterStats = SaveManager.Instance.Progress.GetOrCreateCharacterStats(ModelDb.GetId<RotatingCharacter>());
		if (!isMultiplayer)
		{
			return orCreateCharacterStats.CurrentSingleplayerDreadWinStreak;
		}
		return orCreateCharacterStats.CurrentMultiplayerDreadWinStreak;
	}

	public static ModelId GetCurrentRotatingDreadStartingCharacter(bool isMultiplayer)
	{
		ModelId modelId = (isMultiplayer ? SaveManager.Instance.Progress.CurrentMultiplayerDreadStartingCharacterForRotation : SaveManager.Instance.Progress.CurrentSingleplayerDreadStartingCharacterForRotation);
		if (modelId == ModelId.none)
		{
			return ModelDb.GetId<Ironclad>();
		}
		return modelId;
	}

	public static ModelId GetRotatingCharacterIdForStreak(ModelId startingCharacter, int streak)
	{
		if (startingCharacter == ModelId.none)
		{
			return ModelId.none;
		}
		List<CharacterModel> list = ModelDb.AllCharacters.ToList();
		int num = list.FindIndex((CharacterModel c) => c.Id == startingCharacter);
		if (num < 0)
		{
			Log.Warn($"Couldn't find starting character for dread rotation {startingCharacter} in character list. Defaulting to index 0");
			num = 0;
		}
		return list[(num + streak) % list.Count].Id;
	}
}
