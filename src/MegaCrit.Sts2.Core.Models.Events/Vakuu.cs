using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Godot;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Ancients;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Characters;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Random;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Saves;
using MegaCrit.Sts2.Core.TestSupport;
using MegaCrit.Sts2.Core.Timeline.Epochs;

namespace MegaCrit.Sts2.Core.Models.Events;

public class Vakuu : AncientEventModel
{
	private const string _visitsKey = "Visits";

	private const string _angryBgSeenKey = "VAKUU_ANGRY_BG_SEEN";

	public override FoolPronounGuess FoolPronounGuess => FoolPronounGuess.Neutral;

	public override Color ButtonColor => new Color(0.05f, 0.06f, 0.12f, 0.8f);

	public override Color DialogueColor => new Color("3C1931");

	protected override IEnumerable<DynamicVar> CanonicalVars => new _003C_003Ez__ReadOnlySingleElementList<DynamicVar>(new DynamicVar("Visits", 0m));

	private static string NormalBgPath => ImageHelper.GetImagePath("ancients/vakuu_placeholder.png");

	private static string AngryBgPath => ImageHelper.GetImagePath("ancients/vakuu_angry.png");

	public override IEnumerable<EventOption> AllPossibleOptions => Pool1.Concat(Pool2).Concat(Pool3);

	private List<EventOption> Pool1
	{
		get
		{
			int num = 3;
			List<EventOption> list = new List<EventOption>(num);
			CollectionsMarshal.SetCount(list, num);
			Span<EventOption> span = CollectionsMarshal.AsSpan(list);
			int num2 = 0;
			span[num2] = RelicOption<BloodSoakedRose>();
			num2++;
			span[num2] = RelicOption<WhisperingEarring>();
			num2++;
			span[num2] = RelicOption<Fiddle>();
			return list;
		}
	}

	private List<EventOption> Pool2
	{
		get
		{
			int num = 3;
			List<EventOption> list = new List<EventOption>(num);
			CollectionsMarshal.SetCount(list, num);
			Span<EventOption> span = CollectionsMarshal.AsSpan(list);
			int num2 = 0;
			span[num2] = RelicOption<PreservedFog>();
			num2++;
			span[num2] = RelicOption<SereTalon>().ThatDecreasesMaxHp(9m);
			num2++;
			span[num2] = RelicOption<DistinguishedCape>();
			return list;
		}
	}

	private List<EventOption> Pool3
	{
		get
		{
			int num = 4;
			List<EventOption> list = new List<EventOption>(num);
			CollectionsMarshal.SetCount(list, num);
			Span<EventOption> span = CollectionsMarshal.AsSpan(list);
			int num2 = 0;
			span[num2] = RelicOption<ChoicesParadox>();
			num2++;
			span[num2] = RelicOption<MusicBox>();
			num2++;
			span[num2] = RelicOption<LordsParasol>();
			num2++;
			span[num2] = RelicOption<JeweledMask>();
			return list;
		}
	}

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

	public override IEnumerable<string> GetAssetPaths(IRunState runState)
	{
		IEnumerable<string> assetPaths = base.GetAssetPaths(runState);
		if (TestMode.IsOn)
		{
			return assetPaths;
		}
		return assetPaths.Concat(new _003C_003Ez__ReadOnlyArray<string>(new string[2] { NormalBgPath, AngryBgPath }));
	}

	public override void ConfigureBackgroundScene(Control backgroundScene)
	{
		backgroundScene.GetNode<TextureRect>("TextureRect").Texture = PreloadManager.Cache.GetTexture2D(ChooseBgPath());
	}

	/// <summary>
	/// After the "My Face" epoch (Ironclad7Epoch) is revealed, the next singleplayer Vakuu visit is guaranteed to show
	/// the angry art. Otherwise it's a 50/50 between angry and normal, rolled from the run seed so that every player
	/// in a multiplayer run sees the same art.
	/// </summary>
	private string ChooseBgPath()
	{
		IRunState runState = Owner?.RunState;
		if (runState == null || !runState.UnlockState.IsEpochRevealed<Ironclad7Epoch>())
		{
			return NormalBgPath;
		}
		string text;
		if (runState.Players.Count == 1 && !SaveManager.Instance.Progress.FtueCompleted.Contains("VAKUU_ANGRY_BG_SEEN"))
		{
			text = AngryBgPath;
		}
		else
		{
			Rng rng = new Rng(runState.Rng.Seed + (ulong)runState.CurrentActIndex, "VAKUU_ANGRY_BG");
			text = (rng.NextBool() ? AngryBgPath : NormalBgPath);
		}
		if (text == AngryBgPath)
		{
			SaveManager.Instance.MarkFtueAsComplete("VAKUU_ANGRY_BG_SEEN");
		}
		return text;
	}

	protected override IReadOnlyList<EventOption> GenerateInitialOptions()
	{
		return GetOptionsFromThreePools(Pool1, Pool2, Pool3);
	}
}
