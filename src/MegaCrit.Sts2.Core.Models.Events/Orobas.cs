using System.Collections.Generic;
using System.Linq;
using Godot;
using MegaCrit.Sts2.Core.Entities.Ancients;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Models.Characters;
using MegaCrit.Sts2.Core.Models.Relics;

namespace MegaCrit.Sts2.Core.Models.Events;

public class Orobas : AncientEventModel
{
	private const float _prismaticOdds = 0.3333333f;

	public override Color ButtonColor => new Color(0.05f, 0f, 0.1f, 0.35f);

	public override Color DialogueColor => new Color("5C5F7A");

	public override IEnumerable<EventOption> AllPossibleOptions => new IEnumerable<EventOption>[5]
	{
		OptionPool1,
		OptionPool2,
		OptionPool3,
		SeaGlassOptions,
		new _003C_003Ez__ReadOnlySingleElementList<EventOption>(PrismaticGemOption)
	}.SelectMany((IEnumerable<EventOption> x) => x);

	private IEnumerable<EventOption> OptionPool1 => new _003C_003Ez__ReadOnlyArray<EventOption>(new EventOption[2]
	{
		RelicOption<ElectricShrymp>(),
		RelicOption<GlassEye>()
	});

	private IEnumerable<EventOption> OptionPool2 => new _003C_003Ez__ReadOnlyArray<EventOption>(new EventOption[4]
	{
		RelicOption<AlchemicalCoffer>(),
		RelicOption<Driftwood>(),
		RelicOption<RadiantPearl>(),
		RelicOption<SandCastle>()
	});

	private IEnumerable<EventOption> SeaGlassOptions
	{
		get
		{
			List<EventOption> list = new List<EventOption>();
			foreach (CharacterModel allCharacter in ModelDb.AllCharacters)
			{
				SeaGlass seaGlass = (SeaGlass)ModelDb.Relic<SeaGlass>().ToMutable();
				seaGlass.CharacterId = allCharacter.Id;
				list.Add(RelicOption(seaGlass));
			}
			return list;
		}
	}

	private EventOption PrismaticGemOption => RelicOption<PrismaticGem>();

	private IEnumerable<EventOption> OptionPool3
	{
		get
		{
			List<EventOption> list = new List<EventOption>();
			TouchOfOrobas touchOfOrobas = (TouchOfOrobas)ModelDb.Relic<TouchOfOrobas>().ToMutable();
			if (Owner != null)
			{
				if (touchOfOrobas.SetupForPlayer(Owner))
				{
					list.Add(RelicOption(touchOfOrobas));
				}
			}
			else
			{
				list.Add(RelicOption(touchOfOrobas));
			}
			ArchaicTooth archaicTooth = (ArchaicTooth)ModelDb.Relic<ArchaicTooth>().ToMutable();
			if (Owner != null)
			{
				if (archaicTooth.SetupForPlayer(Owner))
				{
					list.Add(RelicOption(archaicTooth));
				}
			}
			else
			{
				list.Add(RelicOption(archaicTooth));
			}
			if (list.Count == 0)
			{
				list.Add(new EventOption(this, null, "OROBAS.pages.INITIAL.options.OPTION_POOL_3_LOCKED"));
			}
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
					new AncientDialogue("", "")
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
					new AncientDialogue("", "", "")
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
		CharacterModel character = Owner.Character;
		CharacterModel characterModel = Rng.NextItem(Owner.UnlockState.Characters.Where((CharacterModel c) => c.Id != character.Id));
		if (characterModel == null)
		{
			characterModel = character;
		}
		List<EventOption> list = OptionPool1.ToList();
		EventOption item;
		if (Rng.NextFloat() < 0.3333333f)
		{
			item = PrismaticGemOption;
		}
		else
		{
			SeaGlass seaGlass = (SeaGlass)ModelDb.Relic<SeaGlass>().ToMutable();
			seaGlass.CharacterId = characterModel.Id;
			item = RelicOption(seaGlass);
		}
		list.Add(item);
		return new _003C_003Ez__ReadOnlyArray<EventOption>(new EventOption[3]
		{
			Rng.NextItem(list),
			Rng.NextItem(OptionPool2),
			Rng.NextItem(OptionPool3)
		});
	}
}
