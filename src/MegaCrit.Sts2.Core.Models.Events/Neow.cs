using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Godot;
using MegaCrit.Sts2.Core.Entities.Ancients;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models.Blitz;
using MegaCrit.Sts2.Core.Models.Characters;
using MegaCrit.Sts2.Core.Models.Relics;

namespace MegaCrit.Sts2.Core.Models.Events;

public class Neow : AncientEventModel
{
	private const string _sfxSleepy = "event:/sfx/npcs/neow/neow_sleepy";

	private const string _sfxWelcome = "event:/sfx/npcs/neow/neow_welcome";

	private const string _sfxCurious = "event:/sfx/npcs/neow/neow_curious";

	private List<EventOption>? _modifierOptions;

	public override FoolPronounGuess FoolPronounGuess => FoolPronounGuess.It;

	public override string AmbientBgm => "event:/sfx/ambience/act1_neow";

	public override Color ButtonColor => new Color(0f, 0.1f, 0.2f, 0.5f);

	public override Color DialogueColor => new Color("28454F");

	public override LocString InitialDescription
	{
		get
		{
			Player? owner = Owner;
			if (owner != null && owner.RunState.Modifiers.Count <= 0)
			{
				return base.InitialDescription;
			}
			return L10NLookup(Id.Entry + ".EVENT.description");
		}
	}

	public override IEnumerable<EventOption> AllPossibleOptions
	{
		get
		{
			List<EventOption> list = new List<EventOption>();
			list.AddRange(CurseOptions);
			list.AddRange(PositiveOptions);
			list.Add(LavaRockOption);
			list.Add(NeowsTalismanOption);
			list.Add(NutritiousOysterOption);
			list.Add(PomanderOption);
			list.Add(SmallCapsuleOption);
			list.Add(StoneHumidifierOption);
			return new _003C_003Ez__ReadOnlyList<EventOption>(list);
		}
	}

	private IEnumerable<EventOption> PositiveOptions => new _003C_003Ez__ReadOnlyArray<EventOption>(new EventOption[14]
	{
		RelicOption<ArcaneScroll>(),
		RelicOption<BoomingConch>(),
		RelicOption<FishingRod>(),
		RelicOption<GoldenPearl>(),
		RelicOption<Kaleidoscope>(),
		RelicOption<LeadPaperweight>(),
		RelicOption<LostCoffer>(),
		RelicOption<MassiveScroll>(),
		RelicOption<NeowsTorment>(),
		RelicOption<NewLeaf>(),
		RelicOption<PhialHolster>(),
		RelicOption<PreciseScissors>(),
		RelicOption<ScrollBoxes>(),
		RelicOption<WingedBoots>()
	});

	private EventOption LavaRockOption => RelicOption<LavaRock>();

	private EventOption NeowsTalismanOption => RelicOption<NeowsTalisman>();

	private EventOption NutritiousOysterOption => RelicOption<NutritiousOyster>();

	private EventOption PomanderOption => RelicOption<Pomander>();

	private EventOption SmallCapsuleOption => RelicOption<SmallCapsule>();

	private EventOption StoneHumidifierOption => RelicOption<StoneHumidifier>();

	private IEnumerable<EventOption> CurseOptions => new _003C_003Ez__ReadOnlyArray<EventOption>(new EventOption[10]
	{
		RelicOption<CursedPearl>(),
		RelicOption<DowsingRod>(),
		RelicOption<HeftyTablet>(),
		RelicOption<LargeCapsule>(),
		RelicOption<LeafyPoultice>(),
		RelicOption<NeowsBones>(),
		RelicOption<NeowsSacrifice>(),
		RelicOption<PrecariousShears>(),
		RelicOption<SilkenTress>(),
		RelicOption<SilverCrucible>()
	});

	private List<EventOption> ModifierOptions
	{
		get
		{
			AssertMutable();
			if (_modifierOptions == null)
			{
				_modifierOptions = new List<EventOption>();
			}
			return _modifierOptions;
		}
	}

	protected override AncientDialogueSet DefineDialogues()
	{
		return new AncientDialogueSet
		{
			FirstVisitEverDialogue = new AncientDialogue("event:/sfx/npcs/neow/neow_welcome"),
			CharacterDialogues = new Dictionary<string, IReadOnlyList<AncientDialogue>>
			{
				[AncientEventModel.CharKey<Ironclad>()] = new _003C_003Ez__ReadOnlyArray<AncientDialogue>(new AncientDialogue[3]
				{
					new AncientDialogue("event:/sfx/npcs/neow/neow_welcome")
					{
						VisitIndex = 0
					},
					new AncientDialogue("event:/sfx/npcs/neow/neow_curious")
					{
						VisitIndex = 1
					},
					new AncientDialogue("event:/sfx/npcs/neow/neow_sleepy", "event:/sfx/npcs/neow/neow_sleepy", "event:/sfx/npcs/neow/neow_sleepy")
					{
						VisitIndex = 4
					}
				}),
				[AncientEventModel.CharKey<Silent>()] = new _003C_003Ez__ReadOnlyArray<AncientDialogue>(new AncientDialogue[3]
				{
					new AncientDialogue("event:/sfx/npcs/neow/neow_curious")
					{
						VisitIndex = 0
					},
					new AncientDialogue("event:/sfx/npcs/neow/neow_sleepy")
					{
						VisitIndex = 1
					},
					new AncientDialogue("event:/sfx/npcs/neow/neow_sleepy", "", "")
					{
						VisitIndex = 4
					}
				}),
				[AncientEventModel.CharKey<Defect>()] = new _003C_003Ez__ReadOnlyArray<AncientDialogue>(new AncientDialogue[3]
				{
					new AncientDialogue("event:/sfx/npcs/neow/neow_curious")
					{
						VisitIndex = 0
					},
					new AncientDialogue("event:/sfx/npcs/neow/neow_sleepy")
					{
						VisitIndex = 1
					},
					new AncientDialogue("event:/sfx/npcs/neow/neow_sleepy", "", "event:/sfx/npcs/neow/neow_sleepy")
					{
						VisitIndex = 4
					}
				}),
				[AncientEventModel.CharKey<Necrobinder>()] = new _003C_003Ez__ReadOnlyArray<AncientDialogue>(new AncientDialogue[3]
				{
					new AncientDialogue("event:/sfx/npcs/neow/neow_welcome", "", "event:/sfx/npcs/neow/neow_sleepy")
					{
						VisitIndex = 0
					},
					new AncientDialogue("event:/sfx/npcs/neow/neow_sleepy")
					{
						VisitIndex = 1
					},
					new AncientDialogue("", "event:/sfx/npcs/neow/neow_sleepy")
					{
						VisitIndex = 4
					}
				}),
				[AncientEventModel.CharKey<Regent>()] = new _003C_003Ez__ReadOnlyArray<AncientDialogue>(new AncientDialogue[3]
				{
					new AncientDialogue("", "event:/sfx/npcs/neow/neow_sleepy")
					{
						VisitIndex = 0
					},
					new AncientDialogue("event:/sfx/npcs/neow/neow_curious")
					{
						VisitIndex = 1
					},
					new AncientDialogue("", "event:/sfx/npcs/neow/neow_sleepy", "")
					{
						VisitIndex = 4
					}
				})
			},
			AgnosticDialogues = new _003C_003Ez__ReadOnlyArray<AncientDialogue>(new AncientDialogue[5]
			{
				new AncientDialogue("event:/sfx/npcs/neow/neow_welcome"),
				new AncientDialogue("event:/sfx/npcs/neow/neow_welcome"),
				new AncientDialogue("event:/sfx/npcs/neow/neow_welcome"),
				new AncientDialogue("event:/sfx/npcs/neow/neow_welcome"),
				new AncientDialogue("")
			})
		};
	}

	protected override IReadOnlyList<EventOption> GenerateInitialOptions()
	{
		if (Owner.RunState.RunModels.OfType<BlitzRunModel>().Any())
		{
			return new _003C_003Ez__ReadOnlySingleElementList<EventOption>(new EventOption(this, OnBlitzOptionSelected, new LocString("ascension", "BLITZ.NEOW.title"), new LocString("ascension", "BLITZ.NEOW.description"), "BLITZ", Array.Empty<IHoverTip>()));
		}
		if (Owner.RunState.Modifiers.Count > 0)
		{
			return GenerateNeowModifiers();
		}
		List<EventOption> list = CurseOptions.ToList();
		list.RemoveAll((EventOption r) =>
		{
			RelicModel? relic = r.Relic;
			return relic != null && !relic.IsAllowedAtNeow(Owner);
		});
		EventOption eventOption = Rng.NextItem(list);
		List<EventOption> list2 = PositiveOptions.ToList();
		if (eventOption.Relic is CursedPearl)
		{
			list2.RemoveAll((EventOption o) => o.Relic is GoldenPearl);
		}
		if (eventOption.Relic is HeftyTablet)
		{
			list2.RemoveAll((EventOption o) => o.Relic is ArcaneScroll);
		}
		if (eventOption.Relic is LeafyPoultice)
		{
			list2.RemoveAll((EventOption o) => o.Relic is NewLeaf);
		}
		if (eventOption.Relic is PrecariousShears)
		{
			list2.RemoveAll((EventOption o) => o.Relic is PreciseScissors);
		}
		if (eventOption.Relic is NeowsSacrifice)
		{
			list2.RemoveAll((EventOption o) => o.Relic is PhialHolster);
			list2.RemoveAll((EventOption o) => o.Relic is LostCoffer);
		}
		if (!(eventOption.Relic is LargeCapsule))
		{
			if (Rng.NextBool())
			{
				list2.Add(LavaRockOption);
			}
			else
			{
				list2.Add(SmallCapsuleOption);
			}
		}
		if (Rng.NextBool())
		{
			list2.Add(NutritiousOysterOption);
		}
		else
		{
			list2.Add(StoneHumidifierOption);
		}
		if (Rng.NextBool())
		{
			list2.Add(NeowsTalismanOption);
		}
		else
		{
			list2.Add(PomanderOption);
		}
		list2.RemoveAll((EventOption r) =>
		{
			RelicModel? relic = r.Relic;
			return relic != null && !relic.IsAllowedAtNeow(Owner);
		});
		List<EventOption> list3 = new List<EventOption>();
		list3.AddRange(list2.ToList().UnstableShuffle(Rng).Take(2));
		list3.Add(eventOption);
		return new _003C_003Ez__ReadOnlyList<EventOption>(list3);
	}

	private async Task OnBlitzOptionSelected()
	{
		await BlitzRunModel.GiveNeowRewards(Owner);
		List<EventOption> list = GenerateNeowModifiers();
		if (list.Count > 0)
		{
			SetEventState(InitialDescription, new _003C_003Ez__ReadOnlySingleElementList<EventOption>(ModifierOptions[0]));
		}
		else
		{
			SetEventFinished(L10NLookup(Id.Entry + ".pages.DONE.description"));
		}
	}

	private List<EventOption> GenerateNeowModifiers()
	{
		foreach (ModifierModel modifier in Owner.RunState.Modifiers)
		{
			Func<Task> neowOption = modifier.GenerateNeowOption(this);
			if (neowOption != null)
			{
				int optionIndex = ModifierOptions.Count;
				ModifierOptions.Add(new EventOption(this, () => OnModifierOptionSelected(neowOption, optionIndex), modifier.NeowOptionTitle, modifier.NeowOptionDescription, modifier.Id.Entry, modifier.HoverTips.ToArray()));
			}
		}
		if (ModifierOptions.Count > 0)
		{
			int num = 1;
			List<EventOption> list = new List<EventOption>(num);
			CollectionsMarshal.SetCount(list, num);
			Span<EventOption> span = CollectionsMarshal.AsSpan(list);
			int index = 0;
			span[index] = ModifierOptions[0];
			return list;
		}
		return new List<EventOption>();
	}

	private async Task OnModifierOptionSelected(Func<Task> modifierFunc, int index)
	{
		await modifierFunc();
		if (index + 1 >= ModifierOptions.Count)
		{
			SetEventFinished(L10NLookup(Id.Entry + ".pages.DONE.description"));
		}
		else
		{
			SetEventState(InitialDescription, new _003C_003Ez__ReadOnlySingleElementList<EventOption>(ModifierOptions[index + 1]));
		}
	}
}
