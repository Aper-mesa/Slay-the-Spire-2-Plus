using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Characters;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Runs.History;

namespace MegaCrit.Sts2.Core.Entities.Merchant;

public sealed class BlitzMerchantPrismaticCardRewardEntry : BlitzMerchantEntry
{
	private readonly CharacterModel? _character;

	public override BlitzMerchantEntryType MerchantEntryType => BlitzMerchantEntryType.PrismaticCardReward;

	public override LocString Title
	{
		get
		{
			LocString title = base.Title;
			title.Add("Character", _character.ShortTitle);
			return title;
		}
	}

	public override LocString Description
	{
		get
		{
			LocString description = base.Description;
			description.Add("Character", _character.ShortTitle);
			return description;
		}
	}

	public BlitzMerchantPrismaticCardRewardEntry(Player player)
		: base(player)
	{
		_character = player.PlayerRng.Shops.NextItem(player.UnlockState.Characters.Where((CharacterModel c) => c != player.Character));
		if (_character == null)
		{
			_character = ModelDb.Character<Ironclad>();
		}
	}

	protected override async Task<bool> OnTrySelect()
	{
		CardCreationOptions options = new CardCreationOptions(new _003C_003Ez__ReadOnlySingleElementList<CardPoolModel>(_character.CardPool), CardCreationSource.Other, CardRarityOddsType.Blitz).WithRngOverride(_player.PlayerRng.Shops);
		await RewardsCmd.OfferCustom(_player, new List<Reward>(1)
		{
			new CardReward(options, 3, _player)
		});
		return true;
	}

	public override BlitzMerchantHistoryEntry GenerateHistoryEntry(bool wasPicked)
	{
		BlitzMerchantHistoryEntry result = new BlitzMerchantHistoryEntry(BlitzMerchantEntryType.PrismaticCardReward, wasPicked);
		result.modelId = _character?.Id;
		return result;
	}
}
