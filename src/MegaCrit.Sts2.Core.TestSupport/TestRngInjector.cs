using System;
using System.Collections.Generic;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models;

namespace MegaCrit.Sts2.Core.TestSupport;

public static class TestRngInjector
{
	private static readonly List<RelicModel> _relicOverrides = new List<RelicModel>();

	private static RelicRarity? _relicRarityOverride;

	private static Action<List<CardModel>>? _initialShuffleOverride;

	private static List<CardModel>? _combatCardGenerationOverride;

	public static void AddRelicOverride<T>() where T : RelicModel
	{
		_relicOverrides.Add(ModelDb.Relic<T>());
	}

	public static RelicModel? ConsumeRelicOverride()
	{
		if (_relicOverrides.Count > 0)
		{
			List<RelicModel> relicOverrides = _relicOverrides;
			RelicModel result = relicOverrides[relicOverrides.Count - 1];
			_relicOverrides.RemoveAt(_relicOverrides.Count - 1);
			return result;
		}
		return null;
	}

	public static void SetRelicRarityOverride(RelicRarity relicRarity)
	{
		_relicRarityOverride = relicRarity;
	}

	public static RelicRarity? GetRelicRarityOverride()
	{
		return _relicRarityOverride;
	}

	/// <summary>
	/// Force specific cards to show up for CardFactory.GetDistinctForCombat.
	/// </summary>
	public static void SetCombatCardGenerationOverride(List<CardModel> cards)
	{
		_combatCardGenerationOverride = cards;
	}

	/// <summary>
	/// Consume the combat cards generation override and clear it out.
	/// </summary>
	public static List<CardModel>? ConsumeCombatCardGenerationOverride()
	{
		List<CardModel> combatCardGenerationOverride = _combatCardGenerationOverride;
		_combatCardGenerationOverride = null;
		return combatCardGenerationOverride;
	}

	/// <summary>
	/// Force a specific order for the initial shuffle.
	/// </summary>
	public static void SetInitialShuffleOverride(Action<List<CardModel>> reorder)
	{
		_initialShuffleOverride = reorder;
	}

	/// <summary>
	/// Consume the initial shuffle override and clear it out.
	/// </summary>
	public static Action<List<CardModel>>? ConsumeInitialShuffleOverride()
	{
		Action<List<CardModel>> initialShuffleOverride = _initialShuffleOverride;
		_initialShuffleOverride = null;
		return initialShuffleOverride;
	}

	public static void Cleanup()
	{
		_relicOverrides.Clear();
		_relicRarityOverride = null;
		_initialShuffleOverride = null;
		_combatCardGenerationOverride = null;
	}
}
