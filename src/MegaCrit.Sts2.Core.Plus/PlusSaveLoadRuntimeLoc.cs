using System.Collections.Generic;
using MegaCrit.Sts2.Core.Localization;

namespace MegaCrit.Sts2.Core.Plus;

/// <summary>[apermesa] Runtime localization for the built-in Save &amp; Load feature.</summary>
public static class PlusSaveLoadRuntimeLoc
{
	private static bool _registered;

	public static void EnsureRegistered()
	{
		if (_registered)
		{
			return;
		}
		_registered = true;
		LocManager.RegisterRuntimeStrings("eng", "settings_ui", new Dictionary<string, string>
		{
			["INPUT_SETTINGS.INPUT_TITLE.saveAndLoad"] = "Save & Load"
		});
		LocManager.RegisterRuntimeStrings("zhs", "settings_ui", new Dictionary<string, string>
		{
			["INPUT_SETTINGS.INPUT_TITLE.saveAndLoad"] = "存档/读档"
		});
		LocManager.RegisterRuntimeStrings("eng", "gameplay_ui", new Dictionary<string, string>
		{
			["PAUSE_MENU.SAVE_AND_LOAD"] = "Save & Load",
			["PLUS_SL.HOST_CONFIRM.title"] = "Save & Load",
			["PLUS_SL.HOST_CONFIRM.body"] = "A teammate wants to reload the latest checkpoint. Continue?",
			["PLUS_SL.HOST_CONFIRM.confirm"] = "Reload",
			["PLUS_SL.HOST_CONFIRM.cancel"] = "Cancel",
			["PLUS_SL.WAITING.title"] = "Save & Load",
			["PLUS_SL.WAITING.body"] = "Waiting for the host...",
			["PLUS_SL.FAILED.title"] = "Save & Load",
			["PLUS_SL.FAILED.body"] = "Could not reload the run.",
			["PLUS_SL.REJECTED.body"] = "The host declined the request.",
			["PLUS_SL.TIMEOUT.body"] = "The request timed out.",
			["PLUS_SL.NO_PEERS.body"] = "No teammates are connected."
		});
		LocManager.RegisterRuntimeStrings("zhs", "gameplay_ui", new Dictionary<string, string>
		{
			["PAUSE_MENU.SAVE_AND_LOAD"] = "存档/读档",
			["PLUS_SL.HOST_CONFIRM.title"] = "存档/读档",
			["PLUS_SL.HOST_CONFIRM.body"] = "队友请求回到最近的存档点，是否继续？",
			["PLUS_SL.HOST_CONFIRM.confirm"] = "读档",
			["PLUS_SL.HOST_CONFIRM.cancel"] = "取消",
			["PLUS_SL.WAITING.title"] = "存档/读档",
			["PLUS_SL.WAITING.body"] = "正在等待主机...",
			["PLUS_SL.FAILED.title"] = "存档/读档",
			["PLUS_SL.FAILED.body"] = "读档失败。",
			["PLUS_SL.REJECTED.body"] = "主机拒绝了请求。",
			["PLUS_SL.TIMEOUT.body"] = "请求超时。",
			["PLUS_SL.NO_PEERS.body"] = "当前没有已连接的队友。"
		});
	}
}
