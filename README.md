<div align="center">

# Slay the Spire 2 Plus

**Slay the Spire 2 反编译源码 · 以及一套让联机与角色更好玩的 Plus 改动**

![Game](https://img.shields.io/badge/game-v0.111.0-8B5CF6?style=flat-square)
![Engine](https://img.shields.io/badge/Godot-4.5.1-478CBF?style=flat-square&logo=godotengine&logoColor=white)
![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?style=flat-square&logo=dotnet&logoColor=white)
![Language](https://img.shields.io/badge/C%23-14-239120?style=flat-square&logo=csharp&logoColor=white)
![Decompiler](https://img.shields.io/badge/ILSpy-11.1.0-6B7280?style=flat-square)
![Build](https://img.shields.io/badge/build-0%20errors-22C55E?style=flat-square)

<sub>**3,537** 个源文件 · **6,641** 个类 · **223** 个命名空间 · **约 491,700** 行</sub>

**中文** · [English](README.en.md)

</div>

---

> ### ⚠️ 免责声明
> 本仓库包含为**研究、互操作和 Mod 开发**而反编译的源码。
> Slay the Spire 2 及其代码版权归 **Mega Crit Games** 所有。
> 除 `src/ClassicDefectArt/`、`src/ClassicPowerIcons/` 中用于构建「经典故障机器人」的少量 1 代卡图 / 能力图标外，不包含其他游戏资产、二进制、存档或 Steam Workshop 内容。
> 上述图片版权同属 Mega Crit Games，仅用于该角色构建；删除后经典角色的缺图卡会回退到占位图。
> 本项目与 Mega Crit **无关联**，也未获其背书。请勿使用本仓库分发原版游戏或其资产。
> 权利人可提交 issue 要求删除相关内容。

---

## 目录

- [这是什么](#这是什么)
- [目标版本](#目标版本)
- [仓库地图](#仓库地图)
- [构建](#构建)
- [Plus 改动](#plus-改动)
  - [共享存档](#共享存档)
  - [多人构建指纹](#多人构建指纹)
  - [多人传递：传牌 / 传遗物](#多人传递传牌--传遗物)
  - [经典故障机器人](#经典故障机器人1-代-defect)
- [反编译说明](#反编译说明)
- [参考](#参考)

---

## 这是什么

一棵**能直接编译**的 Slay the Spire 2 源码树：

- 用 **ILSpy 11.1.0** 从 `sts2.dll` 反编译出的 C# 工程（`src/`），命名空间、类型、成员名完整且未混淆；
- 从 **GDSC v101** 字节码还原的 56 个 GDScript（`gdscript_tree/`），保留原始项目路径；
- 在此基础上实施的一组 **Plus 改动**：共享存档、联机构建指纹、传牌 / 传遗物，以及把 1 代故障机器人整个搬进 2 代。

从头构建只差 **15 处小修复**，结果 **0 error**，类型数量与原程序集一致。

## 目标版本

| 项目 | 值 |
| --- | --- |
| 游戏版本 | `v0.111.0`（`release_info.json`，commit `41cef1ea`） |
| 引擎 | **Godot 4.5.1 + .NET 9** |
| 原程序集 | `data_sts2_windows_x86_64/sts2.dll` |
| 大小 | 9,757,184 字节 |
| SHA-256 | `0861bfa1df347538d932f22d580e75420f08082792eb914e53b4882764acdbe9` |
| 格式 | PE32+ / x86_64 / IL-only / 非 ReadyToRun / 无强名称 |

## 仓库地图

| 路径 | 内容 |
| --- | --- |
| `src/` | ILSpy 反编译的 C# 工程：223 个命名空间，3,537 个文件，约 491,700 行 |
| `src/ClassicDefectArt/` | 23 张 1 代卡图 PNG，作为嵌入资源编入 DLL（经典 Defect 用） |
| `src/ClassicPowerIcons/` | 6 个 1 代能力图标（48 / 128 两套），作为嵌入资源编入 DLL |
| `gdscript_tree/` | 从 GDSC v101 字节码还原的 56 个 GDScript，保留原始项目路径 |
| `types.txt` | 类清单（6,641 个类） |
| `original_source_paths.txt` | PCK 中记录的原始 C# 源文件路径（3,441 条，仅路径） |

**不包含**：PCK、原版游戏 DLL、工具二进制、本地存档；游戏资产仅包含上述经典角色图片。

## 构建

> **依赖**：.NET SDK 9+（本仓库使用 SDK 10.0.402 构建，目标 `net9.0`）

1. 安装 .NET SDK。
2. `src/sts2.csproj` 通过 `HintPath` 引用 `GodotSharp`、`Sentry`、`Steamworks.NET` 等，路径指向**游戏安装目录**，请按本机调整。
3. 构建：

```bash
dotnet build src/sts2.csproj -c Release
```

4. 输出的 `sts2.dll` 可直接替换 `data_sts2_windows_x86_64/sts2.dll`。

> 🔖 **替换前请备份原 DLL。**

### 反编译修复

反编译代码需要 15 处小修复才能编译：

| 数量 | 问题 | 处理 |
| :---: | --- | --- |
| 8 | `[GeneratedRegex]` 属性 | 删除属性 —— 生成器展开后的方法体已在反编译源码里，保留会导致**重复生成** |
| 5 | `Container` 歧义（`System.ComponentModel.Container` vs `Godot.Container`） | `using Container = Godot.Container;` |
| 2 | `base._002Ector();` 残留 | 删除 |

结果：**0 error**，6,641 个类型（与原程序集一致）。

## Plus 改动

| 改动 | 一句话 |
| --- | --- |
| 🗂️ **共享存档** | 移除原版的 modded / unmodded 存档隔离，存档只此一份 |
| 🔑 **构建指纹** | 联机双方必须使用**同一份 DLL**，否则直接拦截 |
| 🃏 **传牌 / 传遗物** | 每回合一次，用能量 / 生命上限把牌和遗物借给队友 |
| 🤖 **经典故障机器人** | 把 1 代 Defect 连同 75 张卡完整搬进 2 代 |

### 共享存档

移除原版的 modded/unmodded 存档隔离：

- `UserDataPathProvider.GetAccountDir()` 恒返回 `""`。
- 删除 `ModManager.CopyUnmoddedSaveFilesIfNeeded()` 及其调用。
- 所有存档统一走 `user://<platform>/<userid>/profileN/saves/...`。

### 多人构建指纹

`ModManager.GetGameplayRelevantModNameList()` 始终追加：

```text
apermesa-build-<AssemblyHasher.GetMainAssemblyHash()>
```

| 情况 | 结果 |
| --- | --- |
| 同一构建 | 指纹一致 → 握手成功 |
| 不同构建（含未修改的原版） | `ModMismatch` 弹窗并拦截连接 |

> ⚠️ 每次重新编译都会改变指纹，**所有联机玩家必须使用同一份 DLL**。

### 多人传递：传牌 / 传遗物

- **传牌**：每回合一次，花费 1 点能量把一张手牌传给队友（剩 1 点能量也能传）；被女王锁住的牌以及其它带有战斗负面状态的牌不能传递；队友手牌已满时自动进入弃牌堆。
- **传遗物**：每回合一次，最大生命值多于 1 点时，花费 1 点最大生命值把一件遗物借给队友；在给予者的下个回合开始或战斗结束时归还。
- 传牌 / 传遗物按钮位于战斗界面**右上角**；选择卡牌或遗物时可点「取消」放弃本次操作，不消耗能量或生命值。
- **遗物白名单**按规则生成：排除拾取时立即生效的遗物（提升生命上限、加入卡牌/诅咒、附魔等）、仅战斗开始时生效的遗物、已耗尽/融化/召唤宠物的遗物、初始与事件遗物；**佩尔之眼**作为「每场战斗第一次」类遗物特例允许传递。
- 联机通过新增的 `TransferCardAction` / `TransferRelicAction` 走行动队列同步，**所有端必须使用同一构建**。

### 经典故障机器人（1 代 Defect）

把 1 代（Slay the Spire）的故障机器人作为**独立角色**搬进 2 代，与 2 代重制的 Defect 并存：

**规模** —— 75 张 1 代卡牌、9 件 1 代遗物、7 个 1 代专属能力，拥有独立的卡池与遗物池，在角色选择中单独出现。

**数值按 1 代源码校对**，例如：

| 卡牌 | 1 代数值 / 机制 |
| --- | --- |
| 弹幕齐射 | 4 点 |
| 偏差认知 | 4 集中 |
| 爪击 | 升级 +2 伤害 |
| 聚变 | 2 费，不消耗 |
| 冰川 | 7 格挡 |
| 超能光束 | 26 伤害，永久失去 3 集中 |
| 陨石打击 | 24 伤害 |
| 冰寒 | 升级获得固有 |
| 漆黑 | 仅升级后触发黑暗被动 |
| 暴风雨 | 带消耗 |
| 雷暴 | 升级获得固有 |
| 遗传算法 | 每次 +2 |
| 万物一心 | 取回弃牌堆全部 0 费牌 |

**机制细节**

- 跟踪锁定的 50% 增伤对闪电（被动 / 激发）与黑暗球激发都生效，并显示减益图标；
- 6 个专属能力（增幅 / 电动力学 / 散热片 / 跟踪锁定 / 静电释放 / 自我修复）都会在状态栏显示 1 代图标；
- 1 代卡图与能力图标以**嵌入资源**编入 DLL（`src/ClassicDefectArt/`、`src/ClassicPowerIcons/`），分发时只需要一个 `sts2.dll`；
- 角色立绘 / Spine / 音效暂复用 2 代 Defect；古代与建筑师对话也复用 2 代 Defect 的文本和演出，保证第三层后的结算流程正常；
- 图鉴的「故障」筛选同时列出 2 代 Defect 与经典 Defect 的卡牌。受场景限制暂不新增独立筛选按钮，若需要应通过 mod PCK 覆盖 `card_library.tscn`。

## 反编译说明

- 使用 **ILSpy 11.1.0**（ICSharpCode.Decompiler 11.1.0）、**C# 14** 反编译。
- 发行版自带的 `sts2.xml` XML 文档已合并进源码。
- 游戏未发布 PDB，**局部变量名是重建的**；类型 / 成员名完整且未混淆。
- 源码中包含编译器生成代码。**不要对这棵树重新运行 Godot 源生成器**；请使用仓库提供的普通 `Microsoft.NET.Sdk` 构建，或关闭生成器。
- 游戏自带的 mod 系统（`mods/`、`ModHelper`、`Harmony`）位于 `src/MegaCrit.Sts2.Core.Modding/`。
- 构建时 `src/ClassicDefectArt/*.png` 与 `src/ClassicPowerIcons/*.png` 会作为 `EmbeddedResource` 编入 DLL；除此之外不包含场景、音频和 FMOD bank。

## 参考

- [Godot RE Tools (gdsdecomp) v2.7.4](https://github.com/GDRETools/gdsdecomp/releases/tag/v2.7.4) —— PCK / GDScript 提取
- [ILSpy](https://github.com/icsharpcode/ILSpy) —— 程序集反编译
