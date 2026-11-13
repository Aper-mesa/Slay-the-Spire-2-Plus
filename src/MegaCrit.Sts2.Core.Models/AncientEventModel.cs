using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Godot;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Ancients;
using MegaCrit.Sts2.Core.Entities.Ascension;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Map;
using MegaCrit.Sts2.Core.Models.Events;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Runs.History;

namespace MegaCrit.Sts2.Core.Models;

public abstract class AncientEventModel : EventModel
{
	private AncientDialogueSet? _dialogueSet;

	private List<EventOption>? _generatedOptions;

	private string? _debugOption;

	public override string LocTable => "ancients";

	public LocString Epithet => L10NLookup(Id.Entry + ".epithet");

	public AncientDialogueSet DialogueSet
	{
		get
		{
			if (_dialogueSet == null)
			{
				_dialogueSet = DefineDialogues();
				_dialogueSet.PopulateLocKeys(Id.Entry);
			}
			return _dialogueSet;
		}
	}

	/// <summary>
	/// The characters for whom the "any character" dialogue options shouldn't be shown.
	/// For example, <see cref="T:MegaCrit.Sts2.Core.Models.Events.Tezcatara" />'s "any character" dialogue is friendly, but they don't trust the
	/// <see cref="T:MegaCrit.Sts2.Core.Models.Characters.Defect" />, so we blacklist them so we don't show these friendly dialogue options for them.
	/// </summary>
	public virtual IEnumerable<CharacterModel> AnyCharacterDialogueBlacklist => Array.Empty<CharacterModel>();

	public override Color ButtonColor => new Color(0f, 0f, 0f, 0.35f);

	/// <summary>
	/// The color of the speech bubble in the Ancient events during Ancient Dialogue.
	/// </summary>
	public virtual Color DialogueColor { get; } = new Color("28454f");

	public string? DebugOption
	{
		get
		{
			return _debugOption;
		}
		set
		{
			AssertMutable();
			_debugOption = value;
		}
	}

	/// <summary>
	/// The initial options generated when entering an instance of this Ancient event.
	/// Will always be null in canonical events.
	/// </summary>
	private List<EventOption>? GeneratedOptions
	{
		get
		{
			return _generatedOptions;
		}
		set
		{
			AssertMutable();
			_generatedOptions = value;
		}
	}

	public override EventLayoutType LayoutType => EventLayoutType.Ancient;

	private string MapIconPath => ImageHelper.GetImagePath("packed/map/ancients/ancient_node_" + Id.Entry.ToLowerInvariant() + ".png");

	private string MapIconOutlinePath => ImageHelper.GetImagePath("packed/map/ancients/ancient_node_" + Id.Entry.ToLowerInvariant() + "_outline.png");

	public Texture2D MapIcon => PreloadManager.Cache.GetCompressedTexture2D(MapIconPath);

	public Texture2D MapIconOutline => PreloadManager.Cache.GetCompressedTexture2D(MapIconOutlinePath);

	public IEnumerable<string> MapNodeAssetPaths => new _003C_003Ez__ReadOnlyArray<string>(new string[2] { MapIconPath, MapIconOutlinePath });

	public virtual string AmbientBgm => "";

	public bool HasAmbientBgm => AmbientBgm != "";

	/// <summary>
	/// The icon used for this Ancient in the Run History screen.
	/// Also used in other spots, like <see cref="T:MegaCrit.Sts2.Core.Nodes.Events.NAncientDialogueLine" />.
	/// </summary>
	public Texture2D RunHistoryIcon => PreloadManager.Cache.GetCompressedTexture2D(ImageHelper.GetRoomIconPath(MapPointType.Ancient, RoomType.Event, Id));

	public Texture2D RunHistoryIconOutline => PreloadManager.Cache.GetCompressedTexture2D(RunHistoryIconOutlinePath);

	private string RunHistoryIconOutlinePath => ImageHelper.GetImagePath("ui/run_history/" + Id.Entry.ToLowerInvariant() + "_outline.png");

	/// <summary>
	/// The amount of health the player healed upon entering this ancient.
	/// </summary>
	public int HealedAmount { get; private set; }

	public abstract IEnumerable<EventOption> AllPossibleOptions { get; }

	protected virtual Color EventButtonColor { get; set; } = new Color("00000059");

	public override LocString InitialDescription
	{
		get
		{
			if (!RunManager.Instance.IsInProgress || Hook.ShouldAllowAncient(Owner.RunState, Owner, this))
			{
				return base.InitialDescription;
			}
			return new LocString("relics", "WAX_CHOKER.blockMessage");
		}
	}

	/// <summary>
	/// Define the dialogue structure for this ancient. Each subclass must implement this
	/// to specify dialogues for all categories (first meeting, repeating, etc.).
	/// </summary>
	protected abstract AncientDialogueSet DefineDialogues();

	/// <summary>
	/// Convenience method to get a character's dictionary key from its type.
	/// Usage: CharKey&lt;Ironclad&gt;() returns "IRONCLAD".
	/// </summary>
	protected static string CharKey<T>() where T : CharacterModel
	{
		return ModelDb.Character<T>().Id.Entry;
	}

	/// <summary>
	/// Called after the background scene is instantiated, before it's added to the tree.
	/// Override to customize the background (e.g. swap art based on player progress).
	/// </summary>
	public virtual void ConfigureBackgroundScene(Control backgroundScene)
	{
	}

	protected override async Task BeforeEventStarted(bool isPreFinished)
	{
		if (!isPreFinished)
		{
			if (this is Neow)
			{
				Owner.Creature.SetCurrentHpInternal(0m);
			}
			int oldHp = Owner.Creature.CurrentHp;
			decimal amount = Owner.Creature.MaxHp - Owner.Creature.CurrentHp;
			if (RunManager.Instance.HasAscension(AscensionLevel.WearyTraveler))
			{
				amount *= 0.8m;
			}
			await CreatureCmd.Heal(Owner.Creature, amount, playAnim: false);
			if (NRun.Instance != null && this is Neow)
			{
				TaskHelper.RunSafely(NRun.Instance.GlobalUi.TopBar.Hp.LerpAtNeow());
			}
			HealedAmount = Owner.Creature.CurrentHp - oldHp;
		}
	}

	protected sealed override IReadOnlyList<EventOption> GenerateInitialOptionsWrapper()
	{
		if (Hook.ShouldAllowAncient(Owner.RunState, Owner, this))
		{
			GeneratedOptions = GenerateInitialOptions().ToList();
			if (DebugOption != null)
			{
				GeneratedOptions.RemoveAt(0);
				GeneratedOptions.Insert(0, AllPossibleOptions.First((EventOption c) => c.TextKey.Contains(DebugOption)));
			}
			ReplaceNullOptions(GeneratedOptions);
			return GeneratedOptions;
		}
		return new _003C_003Ez__ReadOnlySingleElementList<EventOption>(new EventOption(this, NEventRoom.Proceed, "PROCEED", false, true));
	}

	protected override void SetInitialEventState(bool isPreFinished)
	{
		IReadOnlyList<EventOption> readOnlyList = GenerateInitialOptionsWrapper();
		if ((readOnlyList.Count == 0) | isPreFinished)
		{
			StartPreFinished();
		}
		else
		{
			SetEventState(InitialDescription, readOnlyList);
		}
	}

	private void UpdateRunHistory()
	{
		if (!RunManager.Instance.IsInProgress)
		{
			return;
		}
		foreach (EventOption generatedOption in GeneratedOptions)
		{
			AncientChoiceHistoryEntry item = new AncientChoiceHistoryEntry(generatedOption.Title, generatedOption.WasChosen);
			Owner.RunState.CurrentMapPointHistoryEntry?.GetEntry(Owner.NetId).AncientChoices.Add(item);
		}
	}

	public void StartPreFinished()
	{
		SetEventFinished(L10NLookup(Id.Entry + ".pages.DONE.description"));
	}

	protected void Done()
	{
		UpdateRunHistory();
		SetEventFinished(L10NLookup(Id.Entry + ".pages.DONE.description"));
	}

	/// <summary>
	/// Generate an event option from a relic.
	/// This overload also generates an onChosen callback that makes the player obtain the specified relic and then
	/// finishes the event.
	/// </summary>
	/// <param name="pageName">Name of the page that this option is in, for metrics/gameinfo and loc. INITIAL by default.</param>
	/// <typeparam name="T">Relic whose title, description, and HoverTips should be used for the event option.</typeparam>
	protected EventOption RelicOption<T>(string pageName = "INITIAL") where T : RelicModel
	{
		return RelicOption(ModelDb.Relic<T>().ToMutable(), pageName);
	}

	/// <summary>
	/// Generate an event option from a relic.
	/// This overload also generates an onChosen callback that makes the player obtain the specified relic and then
	/// finishes the event.
	/// See <see cref="M:MegaCrit.Sts2.Core.Models.AncientEventModel.RelicOption``1(System.String)" /> for more details.
	/// </summary>
	protected EventOption RelicOption(RelicModel relic, string pageName = "INITIAL")
	{
		return RelicOption(relic, OnChosen, pageName);
		async Task OnChosen()
		{
			await RelicCmd.Obtain(relic, Owner);
			Done();
		}
	}

	/// <summary>
	/// Filters an option pool down to the options whose relic is allowed in the current run.
	/// Ancient events pick their options from their pools by hand, so every generator that offers relics has to apply
	/// this. A relic that a game mode bans is otherwise still reachable through the event that offers it.
	/// </summary>
	protected List<EventOption> AllowedOptions(IEnumerable<EventOption> options)
	{
		return options.Where((EventOption o) => o.Relic?.IsAllowed(Owner.RunState) ?? true).ToList();
	}

	/// <summary>
	/// Chooses a random allowed option, or returns null when a game mode bans every relic in the pool.
	/// </summary>
	private EventOption? TryChooseRandomAllowedOption(IEnumerable<EventOption> options)
	{
		List<EventOption> list = AllowedOptions(options);
		if (list.Count != 0)
		{
			return Rng.NextItem(list);
		}
		return null;
	}

	/// <summary>
	/// A helper to choose three options, each from a pool.
	/// This is a common method to choose from ancient event options involving relics.
	/// This automatically excludes relic options if they are not allowed, e.g. because of the game mode or because of
	/// multiplayer. If a pool ends up containing no options, then a random option from another pool is chosen instead.
	/// If there are not enough options to generate three, then less than three relics may be returned.
	/// </summary>
	protected List<EventOption> GetOptionsFromThreePools(List<EventOption> pool1, List<EventOption> pool2, List<EventOption> pool3)
	{
		List<EventOption> list = new List<EventOption>();
		EventOption eventOption = TryChooseRandomAllowedOption(pool1);
		EventOption eventOption2 = TryChooseRandomAllowedOption(pool2);
		EventOption eventOption3 = TryChooseRandomAllowedOption(pool3);
		if (eventOption != null)
		{
			list.Add(eventOption);
		}
		if (eventOption2 != null)
		{
			list.Add(eventOption2);
		}
		if (eventOption3 != null)
		{
			list.Add(eventOption3);
		}
		if (list.Count < 3)
		{
			int num = pool1.Count + pool2.Count + pool3.Count;
			List<EventOption> list2 = new List<EventOption>(num);
			CollectionsMarshal.SetCount(list2, num);
			Span<EventOption> span = CollectionsMarshal.AsSpan(list2);
			int num2 = 0;
			Span<EventOption> span2 = CollectionsMarshal.AsSpan(pool1);
			span2.CopyTo(span.Slice(num2, span2.Length));
			num2 += span2.Length;
			Span<EventOption> span3 = CollectionsMarshal.AsSpan(pool2);
			span3.CopyTo(span.Slice(num2, span3.Length));
			num2 += span3.Length;
			Span<EventOption> span4 = CollectionsMarshal.AsSpan(pool3);
			span4.CopyTo(span.Slice(num2, span4.Length));
			num2 += span4.Length;
			List<EventOption> list3 = list2;
			foreach (EventOption item in list)
			{
				list3.Remove(item);
			}
			while (list.Count < 3)
			{
				EventOption eventOption4 = TryChooseRandomAllowedOption(list3);
				if (eventOption4 == null)
				{
					break;
				}
				list.Add(eventOption4);
				list3.Remove(eventOption4);
			}
		}
		if (list.Count == 0)
		{
			Log.Error("Event " + Id.Entry + " has no allowed options in this run!");
		}
		else if (list.Count < 3)
		{
			Log.Error("Event " + Id.Entry + " could not generate three options in this run!");
		}
		return list;
	}
}
