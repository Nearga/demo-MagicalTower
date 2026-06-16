# Unity Structure

## Project Identity

- Project root: `D:\Local\Projects\Unity\Magical Tower\Magical Tower`
- Unity version: `6000.4.8f1` from `ProjectSettings/ProjectVersion.txt`
- Git status: parent root `D:\Local\Projects\Unity\Magical Tower` is a Git worktree; from the nested Unity project root, `git status --short` reports `?? ./` while the whole nested project is untracked.
- Repo instructions: no `AGENTS.md` found at the project root during Teach discovery.

## Known Structure Maps

- `UNITY_STRUCTURE.runtime.md`: task-derived runtime owner routing for the Magical Tower prototype.
- `UNITY_STRUCTURE.content.md`: task-derived configurable data/content routing for enemies, spells, spawning, and tuning.
- `UNITY_STRUCTURE.ui.md`: task-derived UI routing for tower health and damage numbers.
- `UNITY_STRUCTURE.assemblies.md`: assembly/module routing for this project.
- `UNITY_STRUCTURE.cleanup.md`: generated/cache/source boundaries and cleanup notes.

## Current Project Shape

- Project-owned source root: `Assets/` exists but is currently empty.
- Package/dependency root: `Packages/`
- Unity settings root: `ProjectSettings/`
- Generated/cache roots: `Library/`, `Logs/`, `UserSettings/`
- Source-control ignore root: parent `.gitignore` at `D:\Local\Projects\Unity\Magical Tower\.gitignore`
- Build scenes: none listed in `ProjectSettings/EditorBuildSettings.asset`.
- Project-owned scene: `Assets/Scenes/MagicalTowerPrototype.unity`.
- Scene composition root: `GameRoot` with `GameplayRoot`, `UIRoot`, `CameraRoot`, and `LightingRoot` children.
- Project-owned editor tooling: `Assets/Editor/Phase1FoundationBuilder.cs`, `Assets/Editor/Phase2ContentBuilder.cs`.
- Project-owned content definition scripts: `Assets/Scripts/Content/`.
- Project-owned runtime scripts: `Assets/Scripts/Runtime/`.
- Project-owned content assets: `Assets/Content/`.
- Project-owned gameplay prefabs: `Assets/Prefabs/Gameplay/Tower.prefab`, `EnemyAgent.prefab`, `FireballProjectile.prefab`, and `BarrageProjectile.prefab`.
- Project-owned asmdefs: none currently found under `Assets/`.
- Product/task brief: parent `Test Task.md` describes a 3D Magical Tower survivor-style prototype with enemies, configurable spawning difficulty, spells/projectiles/status effects, tower health/game over, damage numbers, and architecture/expandability as the main review focus.
- Established source roots: `Assets/Scripts/Content/`, `Assets/Scenes/`, `Assets/Content/`, `Assets/Materials/`, `Assets/Prefabs/`, `Assets/Art/Generated/Textures/`.

## Do-Not-Touch Areas

- Do not edit `Library/`, `Logs/`, or `UserSettings/` for source changes.
- Do not treat package cache files under `Library/PackageCache/` as project-owned code.
- Do not create broad architecture folders until a concrete feature owner or project convention exists.

## Validation Notes

- Unity editor path observed in local command approvals: `C:\Program Files\Unity\Hub\Editor\6000.4.8f1-x86_64\Editor\Unity.exe` may be the installed editor path, but validate before relying on it.
- No repo-local validation scripts were found during this Teach pass.
- Basic future validation should start with Unity editor compile/import after adding the first source assets.

## Open Gaps

- Content ownership now exists through ScriptableObject definition types in `Assets/Scripts/Content/` and initial tuning assets in `Assets/Content/`.
- Runtime gameplay ownership now exists under `Assets/Scripts/Runtime/`; Phase 4 gameplay prefabs and explicit scene wiring exist. UI presenters and assembly ownership still need first concrete owners.
- Future tasks that add features should route new files beside the first concrete owner they create and refresh the relevant focused map afterward.
