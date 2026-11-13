using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Godot;
using MegaCrit.Sts2.Core.Entities.Ancients;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Models.Characters;
using MegaCrit.Sts2.Core.Models.Relics;

namespace MegaCrit.Sts2.Core.Models.Events;

public class Tezcatara : AncientEventModel
{
	private const string _sfxLaugh = "event:/sfx/npcs/tezcatara/tezcatara_laugh";

	private const string _sfxGreeting = "event:/sfx/npcs/tezcatara/tezcatara_greeting";

	private const string _sfxCondescending = "event:/sfx/npcs/tezcatara/tezcatara_condescending";

	private const string _sfxInquisitive = "event:/sfx/npcs/tezcatara/tezcatara_inquisitive";

	public override FoolPronounGuess FoolPronounGuess => FoolPronounGuess.Masculine;

	public override IEnumerable<CharacterModel> AnyCharacterDialogueBlacklist => new _003C_003Ez__ReadOnlyArray<CharacterModel>(new CharacterModel[2]
	{
		ModelDb.Character<Defect>(),
		ModelDb.Character<ClassicDefect>()
	});

	public override string AmbientBgm => "event:/sfx/ambience/act2_ambience_tezcatara";

	public override Color ButtonColor => new Color(0.08f, 0.04f, 0f, 0.75f);

	public override Color DialogueColor => new Color("33251E");

	public override IEnumerable<EventOption> AllPossibleOptions
	{
		get
		{
			List<EventOption>[] array = new List<EventOption>[4] { OptionPool1, OptionPool2, OptionPool3, null };
			int num = 1;
			List<EventOption> list = new List<EventOption>(num);
			CollectionsMarshal.SetCount(list, num);
			Span<EventOption> span = CollectionsMarshal.AsSpan(list);
			int index = 0;
			span[index] = NutritiousSoupOption;
			array[3] = list;
			return array.SelectMany((List<EventOption> x) => x);
		}
	}

	private EventOption NutritiousSoupOption => RelicOption<NutritiousSoup>();

	private List<EventOption> OptionPool1
	{
		get
		{
			int num = 2;
			List<EventOption> list = new List<EventOption>(num);
			CollectionsMarshal.SetCount(list, num);
			Span<EventOption> span = CollectionsMarshal.AsSpan(list);
			int num2 = 0;
			span[num2] = RelicOption<VeryHotCocoa>();
			num2++;
			span[num2] = RelicOption<YummyCookie>();
			return list;
		}
	}

	private List<EventOption> OptionPool2
	{
		get
		{
			int num = 3;
			List<EventOption> list = new List<EventOption>(num);
			CollectionsMarshal.SetCount(list, num);
			Span<EventOption> span = CollectionsMarshal.AsSpan(list);
			int num2 = 0;
			span[num2] = RelicOption<BiiigHug>();
			num2++;
			span[num2] = RelicOption<Storybook>();
			num2++;
			span[num2] = RelicOption<ToastyMittens>();
			return list;
		}
	}

	private List<EventOption> OptionPool3
	{
		get
		{
			int num = 4;
			List<EventOption> list = new List<EventOption>(num);
			CollectionsMarshal.SetCount(list, num);
			Span<EventOption> span = CollectionsMarshal.AsSpan(list);
			int num2 = 0;
			span[num2] = RelicOption<GoldenCompass>();
			num2++;
			span[num2] = RelicOption<PumpkinCandle>();
			num2++;
			span[num2] = RelicOption<ToyBox>();
			num2++;
			span[num2] = RelicOption<SealOfGold>();
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
					new AncientDialogue("event:/sfx/npcs/tezcatara/tezcatara_greeting")
					{
						VisitIndex = 0
					},
					new AncientDialogue("event:/sfx/npcs/tezcatara/tezcatara_laugh")
					{
						VisitIndex = 1
					},
					new AncientDialogue("event:/sfx/npcs/tezcatara/tezcatara_inquisitive")
					{
						VisitIndex = 4
					}
				}),
				[AncientEventModel.CharKey<Silent>()] = new _003C_003Ez__ReadOnlyArray<AncientDialogue>(new AncientDialogue[3]
				{
					new AncientDialogue("event:/sfx/npcs/tezcatara/tezcatara_inquisitive", "", "")
					{
						VisitIndex = 0
					},
					new AncientDialogue("event:/sfx/npcs/tezcatara/tezcatara_laugh")
					{
						VisitIndex = 1
					},
					new AncientDialogue("event:/sfx/npcs/tezcatara/tezcatara_laugh", "", "event:/sfx/npcs/tezcatara/tezcatara_inquisitive")
					{
						VisitIndex = 4
					}
				}),
				[AncientEventModel.CharKey<Defect>()] = new _003C_003Ez__ReadOnlyArray<AncientDialogue>(new AncientDialogue[3]
				{
					new AncientDialogue("event:/sfx/npcs/tezcatara/tezcatara_laugh", "event:/sfx/npcs/tezcatara/tezcatara_inquisitive")
					{
						VisitIndex = 0
					},
					new AncientDialogue("event:/sfx/npcs/tezcatara/tezcatara_laugh")
					{
						VisitIndex = 1
					},
					new AncientDialogue("event:/sfx/npcs/tezcatara/tezcatara_inquisitive", "event:/sfx/npcs/tezcatara/tezcatara_greeting", "event:/sfx/npcs/tezcatara/tezcatara_condescending")
					{
						VisitIndex = 4
					}
				}),
				[AncientEventModel.CharKey<Necrobinder>()] = new _003C_003Ez__ReadOnlyArray<AncientDialogue>(new AncientDialogue[3]
				{
					new AncientDialogue("event:/sfx/npcs/tezcatara/tezcatara_inquisitive", "", "event:/sfx/npcs/tezcatara/tezcatara_laugh")
					{
						VisitIndex = 0
					},
					new AncientDialogue("event:/sfx/npcs/tezcatara/tezcatara_greeting")
					{
						VisitIndex = 1
					},
					new AncientDialogue("event:/sfx/npcs/tezcatara/tezcatara_inquisitive", "", "event:/sfx/npcs/tezcatara/tezcatara_laugh")
					{
						VisitIndex = 4
					}
				}),
				[AncientEventModel.CharKey<Regent>()] = new _003C_003Ez__ReadOnlyArray<AncientDialogue>(new AncientDialogue[3]
				{
					new AncientDialogue("event:/sfx/npcs/tezcatara/tezcatara_condescending", "", "event:/sfx/npcs/tezcatara/tezcatara_laugh")
					{
						VisitIndex = 0
					},
					new AncientDialogue("event:/sfx/npcs/tezcatara/tezcatara_greeting")
					{
						VisitIndex = 1
					},
					new AncientDialogue("event:/sfx/npcs/tezcatara/tezcatara_greeting", "", "event:/sfx/npcs/tezcatara/tezcatara_laugh")
					{
						VisitIndex = 4
					}
				})
			},
			AgnosticDialogues = new _003C_003Ez__ReadOnlyArray<AncientDialogue>(new AncientDialogue[2]
			{
				new AncientDialogue("event:/sfx/npcs/tezcatara/tezcatara_greeting"),
				new AncientDialogue("event:/sfx/npcs/tezcatara/tezcatara_greeting")
			})
		};
	}

	protected override IReadOnlyList<EventOption> GenerateInitialOptions()
	{
		List<EventOption> list = OptionPool1.ToList();
		if (Owner.Deck.Cards.Any((CardModel c) => c.Tags.Contains(CardTag.Strike) && c.Rarity == CardRarity.Basic))
		{
			list.Add(NutritiousSoupOption);
		}
		return GetOptionsFromThreePools(list, OptionPool2, OptionPool3);
	}
}
