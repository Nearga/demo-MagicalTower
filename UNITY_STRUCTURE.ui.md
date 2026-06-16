# Unity Structure: UI

## When to read

Read this before implementing tower health bars, damage number text, game-over display, HUD canvas, world-space UI, screen-space UI, or visual feedback tied to runtime damage.

## Primary paths

- Live project-owned UI paths: none yet under `Assets/`.
- Task brief: parent `Test Task.md`.
- Proposed UI source root: `Assets/Scripts/UI/` or a focused UI folder under the owning runtime feature if the implementation stays small.
- Proposed UI prefab root: `Assets/Prefabs/UI/`.
- Proposed scene root: `Assets/Scenes/`.

## Runtime/source owners

- No live UI owners exist yet.
- Task-derived UI owners to create:
  - Tower health bar presenter bound to the tower health owner.
  - Damage number spawner/presenter for enemy and tower damage.
  - Game-over presenter triggered when tower health reaches zero.
  - HUD canvas or world-space canvas owner depending on the first scene composition.

## Data/config owners

- UI display settings can start as serialized fields on focused presenters.
- Damage number style/timing can later move to a small UI config asset if reused across enemies and tower.

## Cross-module routes

- UI should subscribe to runtime events or receive calls from scene composition/presenters; it should not own damage resolution.
- Damage number spawning should be a presentation effect created after damage is confirmed by runtime owners.
- Tower health bar should read/display tower health state, not decide game-over rules.

## Validation hints

- Validate health bar changes when the tower takes damage.
- Validate damage numbers spawn for both enemies and tower damage.
- Validate game-over display appears when tower health reaches zero.
- If world-space damage numbers are used, validate camera/canvas conversion and final drawn positions in Play Mode.

## Do-not-touch

- Do not combine UI presentation, enemy damage rules, tower death, spawn scheduling, and spell cooldowns in one presenter.
- Do not use screenshot-only positions as runtime truth for damage text placement.
- Do not make UI implementation a dependency of core runtime systems.

## Open gaps

- No Canvas, TMP/TextMeshPro package dependency, UI prefabs, presenters, or runtime event path exist yet.
- The project manifest currently does not include TextMeshPro as an explicit package; Unity may provide it via built-in UI workflows, but verify before depending on TMP-specific code.
