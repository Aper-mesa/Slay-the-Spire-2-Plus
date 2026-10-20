# Slay the Spire 2 Plus

Decompiled C# / GDScript source of **Slay the Spire 2 v0.111.0** (Godot 4.5.1 + .NET 9), plus the "Plus" modifications made in this repository.

> **Disclaimer**
> This repository contains decompiled source code for research, interoperability and modding.
> Slay the Spire 2 and its code are © Mega Crit Games. No game assets, binaries, save data or
> Steam Workshop content are included. This project is not affiliated with or endorsed by Mega Crit.
> Do not use this repository to distribute the original game or its assets.
> Rights holders may open an issue for removal.

## Target

- Game version: `v0.111.0` (`release_info.json`, commit `41cef1ea`)
- Engine: **Godot 4.5.1 + .NET 9**
- Original assembly: `data_sts2_windows_x86_64/sts2.dll`
  - 9,757,184 bytes
  - SHA-256 `0861bfa1df347538d932f22d580e75420f08082792eb914e53b4882764acdbe9`
  - PE32+ / x86_64 / IL-only / not ReadyToRun / not strong-named

## Repository contents

| Path | Contents |
| --- | --- |
| `src/` | ILSpy-decompiled C# project: 223 namespaces, 3,537 files, ~491,700 lines |
| `gdscript_tree/` | 56 GDScript files recovered from GDSC v101 bytecode, original project paths |
| `types.txt` | Class inventory (6,641 classes) |
| `original_source_paths.txt` | Original C# source paths recorded in the PCK (3,441 entries, paths only) |

Not included: game assets, PCK, original game DLL, tool binaries, local save data.

## Build

1. Install .NET SDK 9+ (this tree was built with SDK 10.0.402 targeting `net9.0`).
2. `src/sts2.csproj` references `GodotSharp`, `Sentry`, `Steamworks.NET`, etc. through HintPaths
   pointing at the game installation. Adjust these paths for your machine.
3. Build:

```text
dotnet build src/sts2.csproj -c Release
```

4. The output `sts2.dll` is a drop-in replacement for
   `data_sts2_windows_x86_64/sts2.dll`. Back up the original DLL first.

### Decompilation fixes

The decompiled tree needed 15 small fixes to compile:

- 8 × `[GeneratedRegex]` attributes removed (the generated bodies are already present in the
  decompiled source; keeping the attributes makes the C# generator emit duplicates).
- 5 × `Container` ambiguity (`System.ComponentModel.Container` vs `Godot.Container`) resolved with
  `using Container = Godot.Container;`.
- 2 × `base._002Ector();` artifacts removed.

Result: **0 errors**, 6,641 types (same type count as the original assembly).

## Implemented changes (Plus)

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

- Same build → same fingerprint → handshake succeeds.
- A different build (including the unmodified game) → `ModMismatch` popup and connection blocked.
- Every rebuild changes the fingerprint, so all players must use the same DLL.

## Notes

- Decompiled with ILSpy 11.1.0 (ICSharpCode.Decompiler 11.1.0), C# 14.
- XML documentation from the shipped `sts2.xml` was merged into the source.
- No PDB was shipped, so local variable names are reconstructed; type/member names are intact and
  not obfuscated.
- Compiler-generated code is present. Do not re-run Godot source generators against this tree;
  build with plain `Microsoft.NET.Sdk` (as provided) or disable the generators.
- The game's own mod system (`mods/`, `ModHelper`, `Harmony`) is under
  `src/MegaCrit.Sts2.Core.Modding/`.
- Assets, scenes, audio and FMOD banks are not included.

## References

- Godot RE Tools (gdsdecomp) v2.7.4: https://github.com/GDRETools/gdsdecomp/releases/tag/v2.7.4
- ILSpy: https://github.com/icsharpcode/ILSpy
