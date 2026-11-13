using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Godot;
using MegaCrit.Sts2.Core.Entities.Ancients;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Models.Characters;
using MegaCrit.Sts2.Core.Models.Enchantments;
using MegaCrit.Sts2.Core.Models.Relics;

namespace MegaCrit.Sts2.Core.Models.Events;

public class Pael : AncientEventModel
{
	public override FoolPronounGuess FoolPronounGuess => FoolPronounGuess.Neutral;

	public override IEnumerable<EventOption> AllPossibleOptions => OptionPool1.Concat(OptionPool2).Concat(OptionPool3).Concat(new _003C_003Ez__ReadOnlyArray<EventOption>(new EventOption[4] { PaelsClawOption, PaelsToothOption, PaelsLegionOption, PaelsGrowthOption }));

	public override Color ButtonColor => new Color(0.03f, 0.08f, 0f, 0.75f);

	public override Color DialogueColor => new Color("332C29");

	private EventOption PaelsClawOption => RelicOption<PaelsClaw>();

	private EventOption PaelsToothOption => RelicOption<PaelsTooth>();

	private EventOption PaelsGrowthOption => RelicOption<PaelsGrowth>();

	private EventOption PaelsLegionOption => RelicOption<PaelsLegion>();

	private List<EventOption> OptionPool1
	{
		get
		{
			int num = 3;
			List<EventOption> list = new List<EventOption>(num);
			CollectionsMarshal.SetCount(list, num);
			Span<EventOption> span = CollectionsMarshal.AsSpan(list);
			int num2 = 0;
			span[num2] = RelicOption<PaelsFlesh>();
			num2++;
			span[num2] = RelicOption<PaelsHorn>();
			num2++;
			span[num2] = RelicOption<PaelsTears>();
			return list;
		}
	}

	private List<EventOption> OptionPool2
	{
		get
		{
			int num = 1;
			List<EventOption> list = new List<EventOption>(num);
			CollectionsMarshal.SetCount(list, num);
			Span<EventOption> span = CollectionsMarshal.AsSpan(list);
			int index = 0;
			span[index] = RelicOption<PaelsWing>();
			return list;
		}
	}

	private List<EventOption> OptionPool3
	{
		get
		{
			int num = 2;
			List<EventOption> list = new List<EventOption>(num);
			CollectionsMarshal.SetCount(list, num);
			Span<EventOption> span = CollectionsMarshal.AsSpan(list);
			int num2 = 0;
			span[num2] = RelicOption<PaelsEye>();
			num2++;
			span[num2] = RelicOption<PaelsBlood>();
			return list;
		}
	}

	protected override AncientDialogueSet DefineDialogues()
	{
		return new AncientDialogueSet
		{
			FirstVisitEverDialogue = new AncientDialogue(""),
			CharacterDialogues = new Dictionary<string, IReadOnlyList<AncientDialogue>>
			{
				[AncientEventModel.CharKey<Ironclad>()] = new _003C_003Ez__ReadOnlyArray<AncientDialogue>(new AncientDialogue[3]
				{
					new AncientDialogue("")
					{
						VisitIndex = 0
					},
					new AncientDialogue("")
					{
						VisitIndex = 1
					},
					new AncientDialogue("")
					{
						VisitIndex = 4
					}
				}),
				[AncientEventModel.CharKey<Silent>()] = new _003C_003Ez__ReadOnlyArray<AncientDialogue>(new AncientDialogue[3]
				{
					new AncientDialogue("")
					{
						VisitIndex = 0
					},
					new AncientDialogue("")
					{
						VisitIndex = 1
					},
					new AncientDialogue("")
					{
						VisitIndex = 4
					}
				}),
				[AncientEventModel.CharKey<Defect>()] = new _003C_003Ez__ReadOnlyArray<AncientDialogue>(new AncientDialogue[3]
				{
					new AncientDialogue("")
					{
						VisitIndex = 0
					},
					new AncientDialogue("")
					{
						VisitIndex = 1
					},
					new AncientDialogue("")
					{
						VisitIndex = 4
					}
				}),
				[AncientEventModel.CharKey<Necrobinder>()] = new _003C_003Ez__ReadOnlyArray<AncientDialogue>(new AncientDialogue[3]
				{
					new AncientDialogue("", "", "")
					{
						VisitIndex = 0
					},
					new AncientDialogue("")
					{
						VisitIndex = 1
					},
					new AncientDialogue("", "", "")
					{
						VisitIndex = 4
					}
				}),
				[AncientEventModel.CharKey<Regent>()] = new _003C_003Ez__ReadOnlyArray<AncientDialogue>(new AncientDialogue[3]
				{
					new AncientDialogue("", "")
					{
						VisitIndex = 0
					},
					new AncientDialogue("")
					{
						VisitIndex = 1
					},
					new AncientDialogue("", "", "")
					{
						VisitIndex = 4
					}
				})
			},
			AgnosticDialogues = new _003C_003Ez__ReadOnlyArray<AncientDialogue>(new AncientDialogue[2]
			{
				new AncientDialogue(""),
				new AncientDialogue("")
			})
		};
	}

	protected override IReadOnlyList<EventOption> GenerateInitialOptions()
	{
		List<EventOption> optionPool = OptionPool2;
		IReadOnlyList<CardModel> cards = Owner.Deck.Cards;
		if (cards.Count((CardModel c) => ModelDb.Enchantment<Goopy>().CanEnchant(c)) >= 3)
		{
			optionPool.Add(PaelsClawOption);
		}
		if (cards.Count((CardModel c) => c.IsRemovable) >= 5)
		{
			optionPool.Add(PaelsToothOption);
		}
		optionPool.AddRange(optionPool);
		optionPool.Add(PaelsGrowthOption);
		List<EventOption> optionPool2 = OptionPool3;
		if (!Owner.HasEventPet())
		{
			optionPool2.Add(PaelsLegionOption);
		}
		return GetOptionsFromThreePools(OptionPool1, optionPool, optionPool2);
	}
}
