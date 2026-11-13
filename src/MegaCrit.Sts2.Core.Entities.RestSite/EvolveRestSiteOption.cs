using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.RestSite;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Screens.Map;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.TestSupport;

namespace MegaCrit.Sts2.Core.Entities.RestSite;

public class EvolveRestSiteOption : RestSiteOption
{
	public override string OptionId => "EVOLVE";

	public override IEnumerable<string> AssetPaths
	{
		get
		{
			List<string> list = new List<string>();
			list.AddRange(base.AssetPaths);
			list.AddRange(NByrdEvolutionVfx.AssetPaths);
			return new _003C_003Ez__ReadOnlyList<string>(list);
		}
	}

	public EvolveRestSiteOption(Player owner)
		: base(owner)
	{
	}

	public override async Task<bool> OnSelect()
	{
		Byrdpip byrdpip = Owner.GetRelic<Byrdpip>();
		if (byrdpip == null)
		{
			return false;
		}
		string skin = byrdpip.Skin;
		skin = skin.Replace("version", "hair/v");
		Byrdine byrdine = (Byrdine)ModelDb.Relic<Byrdine>().ToMutable();
		byrdine.Hair = skin;
		if (TestMode.IsOff && LocalContext.IsMe(Owner))
		{
			bool travelWasEnabled = NMapScreen.Instance?.IsTravelEnabled ?? false;
			bool mapButtonWasEnabled = NRun.Instance?.GlobalUi.TopBar.Map.IsEnabled ?? false;
			bool proceedWasEnabled = NRestSiteRoom.Instance?.ProceedButton.IsEnabled ?? false;
			try
			{
				NRun.Instance?.GlobalUi.TopBar.Map.Disable();
				NMapScreen.Instance?.SetTravelEnabled(enabled: false);
				NRestSiteRoom.Instance?.ProceedButton.Disable();
				NByrdEvolutionVfx nByrdEvolutionVfx = NByrdEvolutionVfx.ShowScreen(byrdpip, byrdine);
				await nByrdEvolutionVfx.PlayAnimation();
				await Cmd.Wait(0.25f);
			}
			finally
			{
				if (mapButtonWasEnabled)
				{
					NRun.Instance?.GlobalUi.TopBar.Map.Enable();
				}
				if (travelWasEnabled)
				{
					NMapScreen.Instance?.SetTravelEnabled(enabled: true);
				}
				if (proceedWasEnabled)
				{
					NRestSiteRoom.Instance?.ProceedButton.Enable();
				}
			}
		}
		await RelicCmd.Replace(byrdpip, byrdine);
		return true;
	}

	public override Task DoLocalPostSelectVfx(CancellationToken ct = default(CancellationToken))
	{
		SfxCmd.Play("event:/sfx/byrdpip/byrdpip_attack");
		return Task.CompletedTask;
	}

	public override Task DoRemotePostSelectVfx()
	{
		SfxCmd.Play("event:/sfx/byrdpip/byrdpip_attack");
		NRestSiteCharacter nRestSiteCharacter = NRestSiteRoom.Instance?.Characters.First((NRestSiteCharacter c) => c.Player == Owner);
		NRelicFlashVfx nRelicFlashVfx = NRelicFlashVfx.Create(ModelDb.Relic<Byrdine>());
		if (nRelicFlashVfx == null)
		{
			return Task.CompletedTask;
		}
		nRestSiteCharacter?.AddChildSafely(nRelicFlashVfx);
		nRelicFlashVfx.Position = Vector2.Zero;
		return Task.CompletedTask;
	}
}
