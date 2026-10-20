using System.Collections.Generic;
using System.Linq;
using Godot;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Ancients;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Characters;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Saves;

namespace MegaCrit.Sts2.Core.Models.Events;

public class Vakuu : AncientEventModel
{
	private const string _visitsKey = "Visits";

	public override Color ButtonColor => new Color(0.05f, 0.06f, 0.12f, 0.8f);

	public override Color DialogueColor => new Color("3C1931");

	protected override IEnumerable<DynamicVar> CanonicalVars => new _003C_003Ez__ReadOnlySingleElementList<DynamicVar>(new DynamicVar("Visits", 0m));

	public override IEnumerable<EventOption> AllPossibleOptions => Pool1.Concat(Pool2).Concat(Pool3);

	private IEnumerable<EventOption> Pool1 => new _003C_003Ez__ReadOnlyArray<EventOption>(new EventOption[3]
	{
		RelicOption<BloodSoakedRose>(),
		RelicOption<WhisperingEarring>(),
		RelicOption<Fiddle>()
	});

	private IEnumerable<EventOption> Pool2 => new _003C_003Ez__ReadOnlyArray<EventOption>(new EventOption[3]
	{
		RelicOption<PreservedFog>(),
		RelicOption<SereTalon>().ThatDecreasesMaxHp(9m),
		RelicOption<DistinguishedCape>()
	});

	private IEnumerable<EventOption> Pool3 => new _003C_003Ez__ReadOnlyArray<EventOption>(new EventOption[4]
	{
		RelicOption<ChoicesParadox>(),
		RelicOption<MusicBox>(),
		RelicOption<LordsParasol>(),
		RelicOption<JeweledMask>()
	});

	public override void CalculateVars()
	{
		base.CalculateVars();
		if (LocalContext.IsMe(Owner))
		{
			int num = SaveManager.Instance.Progress.AncientStats.GetValueOrDefault(Id)?.GetVisitsAs(Owner.Character.Id) ?? 0;
			DynamicVars["Visits"].BaseValue = num + 1;
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
				[AncientEventModel.CharKey<Silent>()] = new _003C_003Ez__ReadOnlyArray<AncientDialogue>(new AncientDialogue[3]
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
			AgnosticDialogues = new _003C_003Ez__ReadOnlyArray<AncientDialogue>(new AncientDialogue[3]
			{
				new AncientDialogue(""),
				new AncientDialogue(""),
				new AncientDialogue("")
			})
		};
	}

	protected override IReadOnlyList<EventOption> GenerateInitialOptions()
	{
		List<EventOption> list = Pool1.ToList();
		List<EventOption> list2 = Pool2.ToList();
		List<EventOption> list3 = Pool3.ToList();
		list.UnstableShuffle(Rng);
		list2.UnstableShuffle(Rng);
		list3.UnstableShuffle(Rng);
		return new _003C_003Ez__ReadOnlyArray<EventOption>(new EventOption[3]
		{
			list[0],
			list2[0],
			list3[0]
		});
	}
}
