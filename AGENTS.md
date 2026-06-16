# Repository Guidelines

## First Source of Truth

Before planning, editing, or verifying Unity work, read `UNITY_STRUCTURE.md` and the focused map that matches the task:

- `UNITY_STRUCTURE.runtime.md` for runtime/gameplay systems.
- `UNITY_STRUCTURE.content.md` for content, tuning, spawning, enemies, spells, and progression data.
- `UNITY_STRUCTURE.ui.md` for HUD, damage numbers, menus, canvases, TMP, and visual UI.
- `UNITY_STRUCTURE.assemblies.md` for assembly/module routing.
- `UNITY_STRUCTURE.cleanup.md` for generated files, cache boundaries, and deletion safety.

Refresh the relevant map when a task changes the project structure or invalidates its routing notes.

## Project Structure & Module Organization

This repository is the active Unity project root. `Assets/` contains project-owned scenes, scripts, prefabs, content assets, materials, VFX, and UI assets as they are added. `Packages/` contains Unity package dependencies. `ProjectSettings/` contains Unity project settings.

Generated and local editor folders such as `Library/`, `Logs/`, `Temp/`, and `UserSettings/` are not project-owned source. Do not edit or rely on package cache files under `Library/PackageCache/` as the owner of project behavior.

The current project started from an empty `Assets/` tree. Add new files only when a concrete feature owner exists, and prefer focused feature/data/UI/runtime folders over broad architecture scaffolding.

## Build, Test, and Development Commands

No repo-local build or test scripts are currently defined. For Unity validation, open this directory in Unity Editor version `6000.4.8f1` and let packages import/compile.

When package dependencies change, validate `Packages/manifest.json` and `Packages/packages-lock.json` as JSON and allow Unity to resolve the lock file on next editor launch.

## Coding Style & Naming Conventions

Use standard C# conventions in Unity code: 4-space indentation, `PascalCase` for types and public members, and `camelCase` for locals and private fields unless a surrounding file establishes a different pattern.

Keep runtime behavior, content definitions, UI assembly, and editor tooling in focused owners. Do not grow central hubs when a smaller service, data object, component, event, bridge, or feature-owned class can own the responsibility.

## Unity MCP Workflow

Use Unity MCP for Unity Editor work. Before changing scenes, prefabs, UI layout, GameObjects, components, or Unity scripts, inspect the live editor state with MCP resources/tools, including relevant scene hierarchy, selected or persistent objects, component values, console errors, and screenshots when visual fidelity matters.

If Unity MCP exposes an issue that can be fixed safely, fix it and re-check the editor state or console afterward. If Unity MCP is unavailable, disconnected, stale, blocked by compilation, or otherwise reporting something genuinely wrong, stop and report exactly what must be opened, connected, compiled, or clarified before proceeding.

For dynamic or runtime-instantiated UI and gameplay, verify in active Play Mode when making layout, alignment, or visible behavior claims. Check the Unity console immediately after C# logic or UI changes and during or after Play Mode. Fix compiler errors, unassigned references, and runtime exceptions before finishing.

Save Unity MCP verification screenshots under `Assets/Screenshots~/`. Do not save generated verification screenshots under `Assets/Screenshots`, build output folders, or cache folders.

Avoid code-created UI components and child hierarchies for visual layout work unless the user explicitly asks for generated UI. Reusable UI should usually be prefab-owned, while single-use UI should be laid out in the scene or prefab hierarchy. Runtime code may bind behavior, repair small non-visual settings, or populate data into existing slots.

## Testing Guidelines

No automated test suite is currently defined. For Unity work, validate the affected scene, prefab, or system in the editor, check console errors, and use Play Mode for runtime behavior. For visible changes, capture a Unity MCP screenshot and inspect the actual rendered result before finishing.

## Git Hygiene

Preserve unrelated dirty files. Do not revert user changes unless explicitly asked. Keep changes scoped to the requested system and avoid mixing source edits with generated cache churn.

Commit messages, when requested, should be concise and imperative, for example `Add Unity MCP package` or `Create tower gameplay scene`.

## Security & Configuration Tips

Do not commit credentials, private API responses, local machine secrets, generated cache contents, or device-specific editor state. Keep package and project settings changes intentional and reviewable.
