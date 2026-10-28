using System;
using System.Collections.Generic;
using System.Linq;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;

namespace MegaCrit.Sts2.Core.ClassicContent;

/// <summary>
/// [apermesa] Registers the classic Defect models at startup without touching the generated
/// AbstractModelSubtypes list, then injects the classic character's localization.
/// </summary>
public static class ClassicDefectContent
{
	private static bool _initialized;

	public static void InjectAll()
	{
		if (_initialized)
		{
			return;
		}
		_initialized = true;

		foreach (Type type in typeof(ClassicDefectContent).Assembly.GetTypes())
		{
			if (!type.IsAbstract
				&& typeof(AbstractModel).IsAssignableFrom(type)
				&& type.Name.StartsWith("Classic", StringComparison.Ordinal))
			{
				ModelDb.Inject(type);
			}
		}

		RegisterLocStrings();
		ClassicDefectLocale.Register();
		ClassicSilentLocale.Register();
		ClassicIroncladLocale.Register();
	}

	private static void RegisterLocStrings()
	{
		LocManager.RegisterRuntimeStrings("zhs", "characters", new Dictionary<string, string>
		{
			["CLASSIC_DEFECT.title"] = "故障机器人（经典）",
			["CLASSIC_DEFECT.titleObject"] = "故障机器人（经典）",
			["CLASSIC_DEFECT.description"] = "一具不停改造自己来生存下去的自动机械。\n在不得不战斗时，它会使用充能球科技来作战。",
			["CLASSIC_DEFECT.unlockText"] = "用[pink]{Prerequisite}[/pink]进行一局游戏来解锁这个角色。",
			["CLASSIC_DEFECT.pronounSubject"] = "它",
			["CLASSIC_DEFECT.pronounObject"] = "它",
			["CLASSIC_DEFECT.pronounPossessive"] = "它的",
			["CLASSIC_DEFECT.possessiveAdjective"] = "它的",
			["CLASSIC_DEFECT.banter.alive.endTurnPing"] = "[jitter]0x54494D45 0x204C494D 0x49542045 0x58434545 0x00444544[/jitter]",
			["CLASSIC_DEFECT.banter.dead.endTurnPing"] = "……",
			["CLASSIC_DEFECT.eventDeathPrevention"] = "为了我的朋友们，我必须活下去！",
			["CLASSIC_DEFECT.goldMonologue"] = "[sine]这些金币可以用来交换零件……[/sine]",
			["CLASSIC_DEFECT.bestiaryQuote"] = "<分析需要首先制服这个对象>",
			["CLASSIC_DEFECT.cardsModifierTitle"] = "故障机器人（经典）卡牌",
			["CLASSIC_DEFECT.cardsModifierDescription"] = "故障机器人（经典）的卡牌现在会出现在奖励和商店中。"
		});
		LocManager.RegisterRuntimeStrings("eng", "characters", new Dictionary<string, string>
		{
			["CLASSIC_DEFECT.title"] = "Defect (Classic)",
			["CLASSIC_DEFECT.titleObject"] = "Defect (Classic)",
			["CLASSIC_DEFECT.description"] = "An automaton that keeps remaking itself to survive.\nWhen forced to fight, it uses orb technology.",
			["CLASSIC_DEFECT.unlockText"] = "Play a run as [pink]{Prerequisite}[/pink] to unlock this character.",
			["CLASSIC_DEFECT.pronounSubject"] = "it",
			["CLASSIC_DEFECT.pronounObject"] = "it",
			["CLASSIC_DEFECT.pronounPossessive"] = "its",
			["CLASSIC_DEFECT.possessiveAdjective"] = "its",
			["CLASSIC_DEFECT.banter.alive.endTurnPing"] = "[jitter]0x54494D45 0x204C494D 0x49542045 0x58434545 0x00444544[/jitter]",
			["CLASSIC_DEFECT.banter.dead.endTurnPing"] = "...",
			["CLASSIC_DEFECT.eventDeathPrevention"] = "I must survive, for my friends!",
			["CLASSIC_DEFECT.goldMonologue"] = "[sine]These coins can be exchanged for parts...[/sine]",
			["CLASSIC_DEFECT.bestiaryQuote"] = "<Analysis requires subjugation of this object first>",
			["CLASSIC_DEFECT.cardsModifierTitle"] = "Defect (Classic) Cards",
			["CLASSIC_DEFECT.cardsModifierDescription"] = "Defect (Classic) cards will now appear in rewards and shops."
		});

		LocManager.RegisterRuntimeStrings("zhs", "cards", new Dictionary<string, string>
		{
			["CLASSIC_STRIKE.title"] = "打击",
			["CLASSIC_STRIKE.description"] = "造成{Damage:diff()}点伤害。",
			["CLASSIC_DEFEND.title"] = "防御",
			["CLASSIC_DEFEND.description"] = "获得{Block:diff()}点[gold]格挡[/gold]。",
			["CLASSIC_ZAP.title"] = "电击",
			["CLASSIC_ZAP.description"] = "[gold]生成[/gold]1个[gold]闪电[/gold]充能球。",
			["CLASSIC_DUALCAST.title"] = "双重释放",
			["CLASSIC_DUALCAST.description"] = "[gold]激发[/gold]你最右侧的充能球2次。"
		});
		LocManager.RegisterRuntimeStrings("eng", "cards", new Dictionary<string, string>
		{
			["CLASSIC_STRIKE.title"] = "Strike",
			["CLASSIC_STRIKE.description"] = "Deal {Damage:diff()} damage.",
			["CLASSIC_DEFEND.title"] = "Defend",
			["CLASSIC_DEFEND.description"] = "Gain {Block:diff()} [gold]Block[/gold].",
			["CLASSIC_ZAP.title"] = "Zap",
			["CLASSIC_ZAP.description"] = "[gold]Channel[/gold] 1 [gold]Lightning[/gold].",
			["CLASSIC_DUALCAST.title"] = "Dualcast",
			["CLASSIC_DUALCAST.description"] = "[gold]Evoke[/gold] your rightmost orb twice."
		});

		LocManager.RegisterRuntimeStrings("zhs", "relics", new Dictionary<string, string>
		{
			["CLASSIC_CRACKED_CORE.title"] = "破损核心",
			["CLASSIC_CRACKED_CORE.description"] = "战斗开始时，[gold]生成[/gold]1个[gold]闪电[/gold]充能球。",
			["CLASSIC_CRACKED_CORE.flavor"] = "这个核心裂开了，但还在运转。"
		});
		LocManager.RegisterRuntimeStrings("eng", "relics", new Dictionary<string, string>
		{
			["CLASSIC_CRACKED_CORE.title"] = "Cracked Core",
			["CLASSIC_CRACKED_CORE.description"] = "At the start of combat, [gold]Channel[/gold] 1 [gold]Lightning[/gold].",
			["CLASSIC_CRACKED_CORE.flavor"] = "The core is cracked, but still running."
		});
	}
}
