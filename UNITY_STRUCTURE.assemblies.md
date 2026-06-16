# Unity Structure: Assemblies

## When to read

Read this before adding new C# files, asmdefs, namespaces, modules, package references, or cross-module dependencies.

## Primary paths

- `Assets/`: project-owned source root.
- `Assets/Scripts/Content/`: first project-owned runtime-facing content definition scripts.
- `Assets/Editor/`: project-owned editor builders for Phase 1 and Phase 2 setup.
- `Assets/Scenes/`, `Assets/Prefabs/`, `Assets/Content/`, `Assets/Materials/`, `Assets/Art/Generated/Textures/`: established project folders.
- `Packages/manifest.json`: package dependency manifest.
- `Packages/packages-lock.json`: resolved package lock.
- `ProjectSettings/`: Unity project settings.

## Runtime/source owners

- Project-owned content definition scripts exist under `Assets/Scripts/Content/` using namespace `MagicalTower.Content`.
- Project-owned editor scripts exist under `Assets/Editor/`.
- Package cache scripts under `Library/PackageCache/` are dependency code, not project-owned owners.

## Data/config owners

- `Packages/manifest.json` owns package dependencies.
- `ProjectSettings/ProjectVersion.txt` owns Unity editor version metadata.
- `ProjectSettings/EditorBuildSettings.asset` currently has an empty `m_Scenes` list.

## Cross-module routes

- No project-owned `.asmdef` files were found.
- Project-owned namespace: `MagicalTower.Content`.
- Current fallback for new code: create the smallest feature-specific folder under `Assets/` that matches the concrete runtime owner being added. Do not introduce shared modules until there is a real shared caller boundary.
- For the Magical Tower prototype, a pragmatic initial routing is a single runtime assembly or `Assembly-CSharp` with clear folders first; add asmdefs only if they reduce real coupling during implementation.
- If asmdefs are introduced, prefer boundaries that match responsibility, for example `MagicalTower.Core`, `MagicalTower.Runtime`, `MagicalTower.Content`, and `MagicalTower.UI`. Do not add these until source files exist and the dependency direction can be validated.
- Dependency direction to preserve if assemblies are added: core primitives/contracts -> runtime systems/content definitions -> UI presenters/scene composition. Runtime gameplay systems should not depend on UI implementation.

## Validation hints

- After adding scripts or asmdefs, validate Unity import/compile with the project editor version.
- If asmdefs are introduced, refresh this map with assembly names, references, and dependency direction.

## Do-not-touch

- Do not edit `Library/PackageCache/` to change dependency behavior.
- Do not add scripts directly under broad roots when a narrower feature folder is known.
- Do not add direct sibling feature references without first establishing a contract, event, gateway, or local owner boundary.

## Open gaps

- First project-owned content scripts established the `MagicalTower.Content` namespace and `Assets/Scripts/Content/` folder convention.
- First asmdef will establish assembly routing; until then, assembly boundaries are absent.
- Suggested root namespace, if no stronger project convention appears: `MagicalTower`.
