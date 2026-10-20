using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Godot;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.RestSite;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;

namespace MegaCrit.Sts2.Core.Entities.RestSite;

public sealed class SmithRestSiteOption : RestSiteOption
{
	private IEnumerable<CardModel>? _selection;

	public override string OptionId => "SMITH";

	public override IEnumerable<string> AssetPaths => base.AssetPaths.Concat(NCardSmithVfx.AssetPaths);

	public int SmithCount { get; set; } = 1;

	public override LocString Description
	{
		get
		{
			LocString locString;
			if (IsEnabled)
			{
				locString = new LocString("rest_site_ui", "OPTION_" + OptionId + ".description");
				locString.Add("Count", SmithCount);
			}
			else
			{
				locString = new LocString("rest_site_ui", "OPTION_" + OptionId + ".descriptionDisabled");
			}
			return locString;
		}
	}

	public override bool IsEnabled => Owner.Deck.UpgradableCardCount != 0;

	public SmithRestSiteOption(Player owner)
		: base(owner)
	{
	}

	public override async Task<bool> OnSelect()
	{
		CardSelectorPrefs prefs = new CardSelectorPrefs(CardSelectorPrefs.UpgradeSelectionPrompt, SmithCount)
		{
			Cancelable = true,
			RequireManualConfirmation = true
		};
		_selection = await CardSelectCmd.FromDeckForUpgrade(Owner, prefs);
		if (!_selection.Any())
		{
			return false;
		}
		foreach (CardModel item in _selection)
		{
			CardCmd.Upgrade(item, CardPreviewStyle.None);
		}
		await Hook.AfterRestSiteSmith(Owner.RunState, Owner);
		return true;
	}

	public override async Task DoLocalPostSelectVfx(CancellationToken ct = default(CancellationToken))
	{
		NRun.Instance?.GlobalUi.CardPreviewContainer.AddChildSafely(NCardSmithVfx.Create(_selection.ToArray()));
		await Cmd.CustomScaledWait(1f, 2f, ignoreCombatEnd: false, ct);
	}

	public override Task DoRemotePostSelectVfx()
	{
		NRestSiteCharacter nRestSiteCharacter = NRestSiteRoom.Instance?.Characters.First((NRestSiteCharacter c) => c.Player == Owner);
		NCardSmithVfx nCardSmithVfx = NCardSmithVfx.Create();
		if (nCardSmithVfx == null)
		{
			return Task.CompletedTask;
		}
		nRestSiteCharacter?.AddChildSafely(nCardSmithVfx);
		nCardSmithVfx.Position = Vector2.Zero;
		return Task.CompletedTask;
	}
}
