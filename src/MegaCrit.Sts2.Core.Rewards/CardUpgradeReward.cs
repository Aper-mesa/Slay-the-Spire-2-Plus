using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.Vfx;

namespace MegaCrit.Sts2.Core.Rewards;

public class CardUpgradeReward : Reward
{
	private static string RewardIcon => ImageHelper.GetImagePath("ui/reward_screen/reward_icon_card_upgrade.png");

	protected override RewardType RewardType => RewardType.UpgradeCard;

	public override int RewardsSetIndex => 8;

	protected override string IconPath => RewardIcon;

	public static IEnumerable<string> AssetPaths => new _003C_003Ez__ReadOnlySingleElementList<string>(RewardIcon);

	public override bool IsPopulated => true;

	public override LocString Description => new LocString("gameplay_ui", "COMBAT_REWARD_CARD_UPGRADE");

	public CardUpgradeReward(Player player)
		: base(player)
	{
	}

	public override void Populate()
	{
	}

	protected override async Task<bool> OnSelect()
	{
		Log.Info($"Player {Player.NetId} obtained card upgrade from reward");
		CardSelectorPrefs prefs = new CardSelectorPrefs(CardSelectorPrefs.UpgradeSelectionPrompt, 1)
		{
			Cancelable = true,
			RequireManualConfirmation = true
		};
		CardModel[] array = (await CardSelectCmd.FromDeckForUpgrade(Player, prefs)).ToArray();
		if (!array.Any())
		{
			return false;
		}
		CardModel[] array2 = array;
		foreach (CardModel card in array2)
		{
			CardCmd.Upgrade(card, CardPreviewStyle.None);
		}
		NRun.Instance?.GlobalUi.CardPreviewContainer.AddChildSafely(NCardSmithVfx.Create(array));
		return true;
	}

	public override void MarkContentAsSeen()
	{
	}
}
