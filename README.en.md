<div align="center">

# Slay the Spire 2 Plus

**Decompiled Slay the Spire 2 source — plus a set of Plus changes that make co-op and characters better**

![Game](https://img.shields.io/badge/game-v0.111.0-8B5CF6?style=flat-square)
![Engine](https://img.shields.io/badge/Godot-4.5.1-478CBF?style=flat-square&logo=godotengine&logoColor=white)
![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?style=flat-square&logo=dotnet&logoColor=white)
![Language](https://img.shields.io/badge/C%23-14-239120?style=flat-square&logo=csharp&logoColor=white)
![Decompiler](https://img.shields.io/badge/ILSpy-11.1.0-6B7280?style=flat-square)
![Build](https://img.shields.io/badge/build-0%20errors-22C55E?style=flat-square)

<sub>**3,537** source files · **6,641** classes · **223** namespaces · **~491,700** lines</sub>

[中文](README.md) · **English**

</div>

---

> ### ⚠️ Disclaimer
> This repository contains decompiled source code for **research, interoperability and modding**.
> Slay the Spire 2 and its code are © **Mega Crit Games**. Apart from the small set of Slay the Spire 1
> card / power images under `src/ClassicDefectArt/` and `src/ClassicPowerIcons/` used to build the
> classic Defect character, no game assets, binaries, save data or Steam Workshop content are
> included. Those images are © Mega Crit Games as well and may be deleted (missing card art then
> falls back to the placeholder). This project is **not affiliated with or endorsed by** Mega Crit.
> Do not use this repository to distribute the original game or its assets.
> Rights holders may open an issue for removal.

---

## Contents

- [What this is](#what-this-is)
- [Target](#target)
- [Repository map](#repository-map)
- [Build](#build)
- [Plus changes](#plus-changes)
  - [Shared saves](#shared-saves)
  - [Multiplayer build fingerprint](#multiplayer-build-fingerprint)
  - [Multiplayer transfer: cards & relics](#multiplayer-transfer-cards--relics)
  - [Classic Defect](#classic-defect-slay-the-spire-1)
- [Notes](#notes)
- [References](#references)

## What this is

A **source tree that actually compiles**:

- A C# project decompiled from `sts2.dll` with **ILSpy 11.1.0** (`src/`) — namespaces, types and members are intact and not obfuscated;
- 56 GDScript files (`gdscript_tree/`) recovered from **GDSC v101** bytecode, keeping their original project paths;
- A set of **Plus changes** layered on top: shared saves, a multiplayer build fingerprint, card / relic transfer, and the STS1 Defect ported wholesale into STS2.

Getting from scratch to a clean build takes **15 small fixes**; the result is **0 errors** with the same type count as the original assembly.

## Target

| Item | Value |
| --- | --- |
| Game version | `v0.111.0` (`release_info.json`, commit `41cef1ea`) |
| Engine | **Godot 4.5.1 + .NET 9** |
| Original assembly | `data_sts2_windows_x86_64/sts2.dll` |
| Size | 9,757,184 bytes |
| SHA-256 | `0861bfa1df347538d932f22d580e75420f08082792eb914e53b4882764acdbe9` |
| Format | PE32+ / x86_64 / IL-only / not ReadyToRun / not strong-named |

## Repository map

| Path | Contents |
| --- | --- |
| `src/` | ILSpy-decompiled C# project: 223 namespaces, 3,537 files, ~491,700 lines |
| `src/ClassicDefectArt/` | 23 STS1 card images, embedded into the DLL for the classic Defect |
| `src/ClassicPowerIcons/` | 6 STS1 power icons (48 / 128), embedded into the DLL |
| `gdscript_tree/` | 56 GDScript files recovered from GDSC v101 bytecode, original project paths |
| `types.txt` | Class inventory (6,641 classes) |
| `original_source_paths.txt` | Original C# source paths recorded in the PCK (3,441 entries, paths only) |

**Not included**: PCK, original game DLL, tool binaries, local save data. The only game assets in the repository are the classic Defect images listed above.

## Build

> **Requires**: .NET SDK 9+ (this tree was built with SDK 10.0.402 targeting `net9.0`)

1. Install the .NET SDK.
2. `src/sts2.csproj` references `GodotSharp`, `Sentry`, `Steamworks.NET`, etc. through `HintPath`s pointing at the **game installation**. Adjust these paths for your machine.
3. Build:

```bash
dotnet build src/sts2.csproj -c Release
```

4. The output `sts2.dll` is a drop-in replacement for `data_sts2_windows_x86_64/sts2.dll`.

> 🔖 **Back up the original DLL first.**

### Decompilation fixes

The decompiled tree needed 15 small fixes to compile:

| Count | Issue | Fix |
| :---: | --- | --- |
| 8 | `[GeneratedRegex]` attributes | Removed — the generated bodies are already present in the decompiled source, keeping them makes the C# generator emit **duplicates** |
| 5 | `Container` ambiguity (`System.ComponentModel.Container` vs `Godot.Container`) | `using Container = Godot.Container;` |
| 2 | `base._002Ector();` artifacts | Removed |

Result: **0 errors**, 6,641 types (same type count as the original assembly).

## Plus changes

| Change | In one line |
| --- | --- |
| 🗂️ **Shared saves** | The modded / unmodded save split is gone — one save set for everything |
| 🔑 **Build fingerprint** | Co-op peers must run the **same DLL**, or the connection is blocked |
| 🃏 **Card / relic transfer** | Once per turn, hand a card or lend a relic to a teammate at a cost |
| 🤖 **Classic Defect** | The STS1 Defect ported in whole, with all 75 cards |

### Shared saves

The original modded/unmodded save isolation is removed:

- `UserDataPathProvider.GetAccountDir()` always returns `""`.
- `ModManager.CopyUnmoddedSaveFilesIfNeeded()` and its call were removed.
- All saves now use `user://<platform>/<userid>/profileN/saves/...`.

### Multiplayer build fingerprint

`ModManager.GetGameplayRelevantModNameList()` always appends:

```text
apermesa-build-<AssemblyHasher.GetMainAssemblyHash()>
```

| Case | Result |
| --- | --- |
| Same build | Same fingerprint → handshake succeeds |
| Different build (including the unmodified game) | `ModMismatch` popup and the connection is blocked |

> ⚠️ Every rebuild changes the fingerprint, so **all peers must use the same DLL.**

### Multiplayer transfer: cards & relics

- **Send Card**: once per turn, spend 1 Energy to send a hand card to a teammate — 1 Energy is enough. Cards locked by the Queen (and any other combat afflictions) cannot be sent. If the teammate's hand is full, the card goes to their discard pile.
- **Send Relic**: once per turn, with more than 1 Max HP, spend 1 Max HP to lend a relic to a teammate. The relic returns at the start of the giver's next turn or when combat ends.
- The Send Card / Send Relic buttons sit in the **top-right corner** of the combat UI; the card/relic picker has a **Cancel** button that aborts the transfer without spending Energy or Max HP.
- The **relic whitelist** is rule-generated: relics with on-pickup effects (max HP, card/curse grants, enchantments), combat-start-only relics, used-up / melted / pet-summoning relics, starter relics and event relics are excluded; **Pael's Eye** is allowed as an explicit "first time each combat" exception.
- Multiplayer sync uses the new `TransferCardAction` / `TransferRelicAction` through the action queue; **all peers must run the same build.**

### Classic Defect (Slay the Spire 1)

The Slay the Spire 1 Defect is ported as a **separate character** that coexists with the redesigned STS2 Defect:

**Scale** — 75 STS1 cards, 9 STS1 relics and 7 classic-only powers with their own card and relic pools; the character has its own entry in character select.

**Numbers follow the STS1 source**, e.g.:

| Card | STS1 value / mechanic |
| --- | --- |
| Barrage | 4 |
| Biased Cognition | 4 Focus |
| Claw | +2 damage on upgrade |
| Fusion | 2-cost, no Exhaust |
| Glacier | 7 Block |
| Hyperbeam | 26 damage, permanent 3 Focus loss |
| Meteor Strike | 24 damage |
| Chill | gains Innate on upgrade |
| Darkness | only triggers Dark passives when upgraded |
| Tempest | Exhausts |
| Storm | gains Innate on upgrade |
| Genetic Algorithm | +2 per play |
| All For One | returns every 0-cost card from the discard pile |

**Mechanics in detail**

- Lock-On's 50% damage bonus applies to Lightning passives / evokes and Dark evokes, and the debuff is visible;
- All six classic powers (Amplify, Electrodynamics, Heatsinks, Lock-On, Static Discharge, Self Repair) display STS1 icons in the power bar;
- STS1 card art and power icons are **embedded into the DLL** as resources (`src/ClassicDefectArt/`, `src/ClassicPowerIcons/`), so the mod is still a single `sts2.dll`;
- Character visuals / Spine / SFX currently reuse the STS2 Defect; ancient and Architect dialogue also reuse the STS2 Defect lines and animations so the post-Act-3 ending completes normally;
- The card library's Defect filter lists both STS2 Defect and classic Defect cards. A dedicated filter button would require overriding `card_library.tscn` through a mod PCK.

## Notes

- Decompiled with **ILSpy 11.1.0** (ICSharpCode.Decompiler 11.1.0), **C# 14**.
- XML documentation from the shipped `sts2.xml` was merged into the source.
- No PDB was shipped, so **local variable names are reconstructed**; type / member names are intact and not obfuscated.
- Compiler-generated code is present. **Do not re-run Godot source generators** against this tree; build with plain `Microsoft.NET.Sdk` (as provided) or disable the generators.
- The game's own mod system (`mods/`, `ModHelper`, `Harmony`) is under `src/MegaCrit.Sts2.Core.Modding/`.
- `src/ClassicDefectArt/*.png` and `src/ClassicPowerIcons/*.png` are compiled into the DLL as `EmbeddedResource`; apart from those, no scenes, audio or FMOD banks are included.

## References

- [Godot RE Tools (gdsdecomp) v2.7.4](https://github.com/GDRETools/gdsdecomp/releases/tag/v2.7.4) — PCK / GDScript extraction
- [ILSpy](https://github.com/icsharpcode/ILSpy) — assembly decompilation
