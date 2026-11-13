using System.Collections.Generic;
using System.Linq;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Dread;
using MegaCrit.Sts2.Core.Runs;

namespace MegaCrit.Sts2.Core.HoverTips;

public static class HoverTipFactory
{
	private static readonly Dictionary<CardKeyword, HoverTip> _keywordHoverTips = new Dictionary<CardKeyword, HoverTip>();

	private static readonly Dictionary<ModelId, HoverTip> _potionHoverTips = new Dictionary<ModelId, HoverTip>();

	public static IEnumerable<IHoverTip> FromEnchantment<T>(int amount = 1) where T : EnchantmentModel
	{
		EnchantmentModel enchantment = ModelDb.Enchantment<T>();
		return FromEnchantment(enchantment, amount);
	}

	public static IEnumerable<IHoverTip> FromEnchantment(EnchantmentModel enchantment, int amount = 1)
	{
		enchantment.AssertCanonical();
		EnchantmentModel enchantmentModel = enchantment.ToMutable();
		enchantmentModel.Amount = amount;
		enchantmentModel.RecalculateValues();
		return enchantmentModel.HoverTips;
	}

	public static IEnumerable<IHoverTip> FromAffliction<T>(int amount = 1) where T : AfflictionModel
	{
		AfflictionModel afflictionModel = ModelDb.Affliction<T>().ToMutable();
		afflictionModel.Amount = amount;
		return afflictionModel.HoverTips;
	}

	public static IHoverTip FromKeyword(CardKeyword keyword)
	{
		if (!_keywordHoverTips.ContainsKey(keyword))
		{
			_keywordHoverTips[keyword] = new HoverTip(keyword.GetTitle(), keyword.GetDescription());
		}
		return _keywordHoverTips[keyword];
	}

	public static IHoverTip FromPower<T>(int? amount = null) where T : PowerModel
	{
		PowerModel model = ModelDb.Power<T>();
		return FromPower(model, amount);
	}

	public static IHoverTip FromPower(PowerModel model, int? amount = null)
	{
		return model.GetDumbHoverTip(amount);
	}

	/// <summary>
	/// Generate a HoverTip-sized preview of the specified power along with the power's hovertips.
	/// </summary>
	public static IEnumerable<IHoverTip> FromPowerWithPowerHoverTips<T>(int? amount = null) where T : PowerModel
	{
		return new IHoverTip[1] { FromPower<T>(amount) }.Concat(ModelDb.Power<T>().GetExtraHoverTips());
	}

	public static IHoverTip FromPotion<T>() where T : PotionModel
	{
		PotionModel model = ModelDb.Potion<T>();
		return FromPotion(model);
	}

	public static IHoverTip FromPotion(PotionModel model)
	{
		_potionHoverTips.TryAdd(model.Id, model.HoverTip);
		return _potionHoverTips[model.Id];
	}

	public static IHoverTip FromOrb<T>() where T : OrbModel
	{
		return ModelDb.Orb<T>().DumbHoverTip;
	}

	public static IEnumerable<IHoverTip> FromRelic<T>() where T : RelicModel
	{
		RelicModel relic = ModelDb.Relic<T>();
		return FromRelic(relic);
	}

	public static IEnumerable<IHoverTip> FromRelicExcludingItself<T>() where T : RelicModel
	{
		RelicModel relic = ModelDb.Relic<T>();
		return FromRelicExcludingItself(relic);
	}

	public static IEnumerable<IHoverTip> FromRelic(RelicModel relic)
	{
		return relic.HoverTips;
	}

	public static IEnumerable<IHoverTip> FromRelicExcludingItself(RelicModel relic)
	{
		return relic.HoverTipsExcludingRelic;
	}

	/// <summary>
	/// Generate a HoverTip-sized preview of the specified card along with the card's hovertips.
	/// </summary>
	/// <typeparam name="T">CardModel subclass</typeparam>
	/// <returns>HoverTip card preview</returns>
	public static IEnumerable<IHoverTip> FromCardWithCardHoverTips<T>(bool upgrade = false) where T : CardModel
	{
		return new IHoverTip[1] { FromCard<T>(upgrade) }.Concat(ModelDb.Card<T>().HoverTips);
	}

	/// <summary>
	/// Same as FromCardWithHoverTips but allow you to omit some keywords.
	/// Use when there are TOO MANY TIPS. Example: CeremonialBeastDominion
	/// </summary>
	public static IEnumerable<IHoverTip> FromCardWithCardHoverTips<T>(IEnumerable<CardKeyword> excludedKeywords, bool upgrade = false) where T : CardModel
	{
		HashSet<string> excludedIds = excludedKeywords.Select((CardKeyword keyword) => FromKeyword(keyword).Id).ToHashSet();
		return from tip in FromCardWithCardHoverTips<T>(upgrade)
			where !excludedIds.Contains(tip.Id)
			select tip;
	}

	/// <summary>
	/// Same as FromCardWithHoverTips but allows you to pass a specific card rather than the canonical one.
	/// </summary>
	public static IEnumerable<IHoverTip> FromCardWithCardHoverTips(CardModel card, bool upgrade = false)
	{
		return new IHoverTip[1] { FromCard(card, upgrade) }.Concat(card.HoverTips);
	}

	/// <summary>
	/// Generate a HoverTip-sized preview of the specified card.
	/// </summary>
	/// <param name="upgrade">Whether the card should appear upgraded.</param>
	/// <typeparam name="T">CardModel subclass</typeparam>
	/// <returns>HoverTip card preview</returns>
	public static IHoverTip FromCard<T>(bool upgrade = false) where T : CardModel
	{
		return FromCard(ModelDb.Card<T>(), upgrade);
	}

	/// <summary>
	/// Generate a HoverTip-sized preview of the specified card.
	/// Note: You should usually use the generic version of this method. Only use this version if you need to modify
	/// the base version of the card (like to show an upgrade or something).
	/// </summary>
	/// <param name="card">CardModel to preview</param>
	/// <param name="upgrade">Whether the card should appear upgraded.</param>
	/// <returns>HoverTip card preview</returns>
	public static IHoverTip FromCard(CardModel card, bool upgrade = false)
	{
		if (upgrade)
		{
			card = (CardModel)card.MutableClone();
			card.UpgradeInternal();
			card.FinalizeUpgradeInternal();
		}
		return new CardHoverTip(card);
	}

	public static IHoverTip Static(StaticHoverTip tip, params DynamicVar[] vars)
	{
		string text = StringHelper.Slugify(tip.ToString());
		LocString locString = L10NStatic(text + ".title");
		LocString locString2 = L10NStatic(text + ".description");
		foreach (DynamicVar dynamicVar in vars)
		{
			locString.Add(dynamicVar);
			locString2.Add(dynamicVar);
		}
		return new HoverTip(locString, locString2);
	}

	public static IHoverTip ForEnergy(CardModel card)
	{
		return ForEnergyWithIconPath(EnergyIconHelper.GetPath(card));
	}

	public static IHoverTip ForEnergy(PotionModel potion)
	{
		return ForEnergyWithIconPath(EnergyIconHelper.GetPath(potion));
	}

	public static IHoverTip ForEnergy(PowerModel power)
	{
		return ForEnergyWithIconPath(EnergyIconHelper.GetPath(power));
	}

	public static IHoverTip ForEnergy(Player player)
	{
		return ForEnergyWithIconPath(EnergyIconHelper.GetPath(player.Character.CardPool));
	}

	public static IHoverTip ForEnergy(RelicModel relic)
	{
		return ForEnergyWithIconPath(EnergyIconHelper.GetPath(relic));
	}

	public static IHoverTip ForEnergy(DominionModel dominion)
	{
		return ForEnergyWithIconPath(EnergyIconHelper.GetPath(dominion));
	}

	private static IHoverTip ForEnergyWithIconPath(string path)
	{
		string localCharacterEnergyIconPrefix = RunManager.Instance.GetLocalCharacterEnergyIconPrefix();
		return new HoverTip(L10NStatic("ENERGY.title"), L10NStatic("ENERGY.description"), PreloadManager.Cache.GetTexture2D((localCharacterEnergyIconPrefix != null) ? EnergyIconHelper.GetPath(localCharacterEnergyIconPrefix) : path));
	}

	/// <summary>
	/// Special hovertip logic for the Fool's arrows.
	/// Handles multiplayer, arrow count, no arrows, and when hovering the empty arrow slot.
	/// </summary>
	public static IHoverTip ForArrow(Player? player, bool isImpaled = false)
	{
		bool flag = CombatManager.Instance.IsInProgress && player?.PlayerCombatState != null && LocalContext.IsMe(player);
		LocString locString = L10NStatic(isImpaled ? "ARROW.missing.description" : "ARROW.description");
		locString.Add("ShowArrowCount", flag);
		if (flag)
		{
			LocString locString2 = L10NStatic("ARROW.count");
			locString2.Add("Arrows", ArrowCmd.GetAvailableArrowCount(player));
			locString.Add("ArrowCount", locString2);
		}
		else
		{
			locString.Add("ArrowCount", string.Empty);
		}
		return new HoverTip(L10NStatic("ARROW.title"), locString);
	}

	public static IEnumerable<IHoverTip> FromForge()
	{
		List<IHoverTip> list = new List<IHoverTip>();
		list.Add(Static(StaticHoverTip.Forge));
		list.AddRange(FromCardWithCardHoverTips<SovereignBlade>());
		return new _003C_003Ez__ReadOnlyList<IHoverTip>(list);
	}

	private static LocString L10NStatic(string entry)
	{
		return new LocString("static_hover_tips", entry);
	}
}
