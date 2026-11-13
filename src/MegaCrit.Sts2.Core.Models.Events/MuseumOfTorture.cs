using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Runs;

namespace MegaCrit.Sts2.Core.Models.Events;

public sealed class MuseumOfTorture : EventModel
{
	private RelicModel _offeredRelic;

	private RelicModel OfferedRelic
	{
		get
		{
			return _offeredRelic;
		}
		set
		{
			AssertMutable();
			_offeredRelic = value;
		}
	}

	protected override IEnumerable<DynamicVar> CanonicalVars => new _003C_003Ez__ReadOnlySingleElementList<DynamicVar>(new MaxHpVar(10m));

	public override bool IsAllowed(IRunState runState)
	{
		return runState.CurrentActIndex == 1;
	}

	private static string GetPortraitPath(int screen)
	{
		return ImageHelper.GetImagePath($"events/museum_of_torture_0{screen}.png");
	}

	public override IEnumerable<string> GetAssetPaths(IRunState runState)
	{
		List<string> list = new List<string>();
		list.AddRange(base.GetAssetPaths(runState));
		list.Add(GetPortraitPath(2));
		list.Add(GetPortraitPath(3));
		return new _003C_003Ez__ReadOnlyList<string>(list);
	}

	public override void CalculateVars()
	{
		OfferedRelic = Rng.NextItem(new _003C_003Ez__ReadOnlyArray<RelicModel>(new RelicModel[3]
		{
			ModelDb.Relic<ObsidianVice>(),
			ModelDb.Relic<Thumbscrew>(),
			ModelDb.Relic<TormentousCollar>()
		}));
	}

	protected override IReadOnlyList<EventOption> GenerateInitialOptions()
	{
		return new _003C_003Ez__ReadOnlySingleElementList<EventOption>(new EventOption(this, EnterMuseum, "MUSEUM_OF_TORTURE.pages.INITIAL.options.CONTINUE"));
	}

	private Task EnterMuseum()
	{
		SetPortrait(2);
		SetEventState(L10NLookup("MUSEUM_OF_TORTURE.pages.LOCKED.description"), new _003C_003Ez__ReadOnlySingleElementList<EventOption>(new EventOption(this, ViewDisplay, "MUSEUM_OF_TORTURE.pages.LOCKED.options.CONTINUE")));
		return Task.CompletedTask;
	}

	private Task ViewDisplay()
	{
		SetPortrait(3);
		LocString locString = L10NLookup("MUSEUM_OF_TORTURE.pages.DISPLAY.description");
		locString.Add("Relic", OfferedRelic.Title.GetFormattedText());
		EventOption eventOption = new EventOption(this, TakeRelic, "MUSEUM_OF_TORTURE.pages.DISPLAY.options.TAKE", HoverTipFactory.FromRelic(OfferedRelic));
		eventOption.Description.Add("Relic", OfferedRelic.Title.GetFormattedText());
		SetEventState(locString, new _003C_003Ez__ReadOnlyArray<EventOption>(new EventOption[2]
		{
			eventOption,
			new EventOption(this, BreakOut, "MUSEUM_OF_TORTURE.pages.DISPLAY.options.BREAK_OUT")
		}));
		return Task.CompletedTask;
	}

	private async Task TakeRelic()
	{
		await RelicCmd.Obtain(OfferedRelic.ToMutable(), Owner);
		List<CardModel> items = (from c in ModelDb.CardPool<CurseCardPool>().GetUnlockedCards(Owner.UnlockState, Owner.RunState.CardMultiplayerConstraint)
			where c.CanBeGeneratedByModifiers
			orderby c.Id
			select c).ToList();
		CardModel canonicalCard = Owner.PlayerRng.NonCombatGeneration.NextItem(items);
		CardModel card = Owner.RunState.CreateCard(canonicalCard, Owner);
		CardCmd.PreviewCardPileAdd(await CardPileCmd.Add(card, PileType.Deck));
		SetEventFinished(L10NLookup("MUSEUM_OF_TORTURE.pages.TAKE.description"));
	}

	private async Task BreakOut()
	{
		await CreatureCmd.LoseMaxHp(new ThrowingPlayerChoiceContext(), Owner.Creature, DynamicVars.MaxHp.BaseValue, null);
		SetEventFinished(L10NLookup("MUSEUM_OF_TORTURE.pages.BREAK_OUT.description"));
	}

	private void SetPortrait(int screen)
	{
		if (LocalContext.IsMe(Owner) && NEventRoom.Instance != null)
		{
			NEventRoom.Instance.SetPortrait(PreloadManager.Cache.GetTexture2D(GetPortraitPath(screen)));
		}
	}
}
