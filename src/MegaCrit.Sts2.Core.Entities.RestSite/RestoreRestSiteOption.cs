using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Godot;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Nodes.RestSite;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;

namespace MegaCrit.Sts2.Core.Entities.RestSite;

public class RestoreRestSiteOption : RestSiteOption
{
	public override string OptionId => "RESTORE";

	public override bool IsEnabled
	{
		get
		{
			Wuppy wuppy = Owner.Relics.OfType<Wuppy>().FirstOrDefault();
			if (wuppy == null)
			{
				return false;
			}
			return wuppy.CurrentHp < wuppy.MaxHp;
		}
	}

	public RestoreRestSiteOption(Player owner)
		: base(owner)
	{
	}

	public override Task<bool> OnSelect()
	{
		foreach (RelicModel relic in Owner.Relics)
		{
			if (relic is Wuppy wuppy)
			{
				wuppy.CurrentHp = wuppy.MaxHp;
			}
		}
		return Task.FromResult(result: true);
	}

	public override Task DoLocalPostSelectVfx(CancellationToken ct = default(CancellationToken))
	{
		Owner.Relics.OfType<Wuppy>().FirstOrDefault()?.Flash();
		return Task.CompletedTask;
	}

	public override Task DoRemotePostSelectVfx()
	{
		NRestSiteCharacter nRestSiteCharacter = NRestSiteRoom.Instance?.Characters.First((NRestSiteCharacter c) => c.Player == Owner);
		NRelicFlashVfx nRelicFlashVfx = NRelicFlashVfx.Create(ModelDb.Relic<Wuppy>());
		if (nRelicFlashVfx == null)
		{
			return Task.CompletedTask;
		}
		nRestSiteCharacter?.AddChildSafely(nRelicFlashVfx);
		nRelicFlashVfx.Position = Vector2.Zero;
		return Task.CompletedTask;
	}
}
