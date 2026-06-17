# Unity Structure: UI

## When to read

Read this before implementing tower health bars, damage number text, game-over display, HUD canvas, world-space UI, screen-space UI, or visual feedback tied to runtime damage.

## Primary paths

- Live project-owned UI scripts: `Assets/Scripts/UI/`
  - `HudPresenter.cs`
  - `GameOverPresenter.cs`
  - `DamageNumberSpawner.cs`
  - `DamageNumber.cs`
- Live UI prefab root: `Assets/Prefabs/UI/`
  - `DamageNumber.prefab` — RectTransform + CanvasGroup + TMP_Text child + DamageNumber script
- Proposed scene root: `Assets/Scenes/MagicalTowerPrototype.unity`

## Runtime/source owners

- `HudPresenter` — subscribes to `PlayersTowerChangedMessage`; polls `GameSession.ElapsedTime` in `Update`.
  - Drives a Unity `Slider` (fill-only) and a `TMP_Text` label for health.
  - Drives a `TMP_Text` label for elapsed time (frozen on game over).
- `GameOverPresenter` — subscribes to `TowerDestroyedMessage`; enables the game-over panel GameObject.
  - Panel contains static TMP text ("TOWER DESTROYED") authored in the scene.
  - Restart button and scene reload are Phase 8 work.
- `DamageNumberSpawner` — subscribes to `DamageDealtMessage` and `BurningTickMessage`.
  - Converts `report.WorldPosition` to screen space via `Camera.WorldToScreenPoint`.
  - Distinguishes tower vs enemy hits by checking `report.Target is PlayersTower`.
  - Burning ticks use the separate `BurningTickMessage` for distinct coloring.
  - Instantiates `DamageNumber` prefab as a child under `DamageCanvas`.
- `DamageNumber` — self-timed rise+fade driven by `Update`; destroys itself at end of lifetime.

## Canvas setup (scene)

Both canvases live under `GameRoot/UIRoot` in `MagicalTowerPrototype.unity`:

- **`HudCanvas`** — Screen Space Overlay, `CanvasScaler` scale-with-screen (1920×1080 reference)
  - `HealthBar` (Slider, fill-only) + `HealthLabel` (TMP_Text)
  - `ElapsedTimeLabel` (TMP_Text, top-right)
  - `GameOverPanel` (disabled by default) — dark overlay + TMP_Text "TOWER DESTROYED"
  - `HudPresenter` and `GameOverPresenter` components attached here
- **`DamageCanvas`** — Screen Space Overlay (same camera), `DamageNumberSpawner` component attached

## Data/config owners

- All style settings (colors, lifetime, riseSpeed, fadeStartFraction) are `[SerializeField]` fields on `DamageNumberSpawner`.
- No separate UI config asset is needed for Phase 5.

## Cross-module routes

- UI subscribes to `RuntimeMessageBus` events via `Configure()`; it does not own damage resolution.
- `GameplayCompositionRoot` calls `Configure` on all three presenters in `ConfigureRuntime()`.
- `BurningTickMessage` is published by `StatusEffectController` (not via `DamageDealtMessage`) so the spawner can apply a distinct orange color without re-inspecting the report.

## Validation hints

- Verify health bar fills correctly at game start and decreases on enemy contact damage.
- Verify elapsed time label increments each second and freezes on game over.
- Verify game-over panel appears when tower health reaches zero (and not before).
- Verify damage numbers appear at the correct screen position for enemy hits and at the tower for tower hits.
- Verify burning tick numbers appear in the distinct orange color.
- Verify no NullReferenceExceptions in the console during a full run.

## Do-not-touch

- Do not combine UI presentation, enemy damage rules, tower death, spawn scheduling, and spell cooldowns in one presenter.
- Do not use screenshot-only positions as runtime truth for damage text placement.
- Do not make UI implementation a dependency of core runtime systems.

## Open gaps

- Restart button and scene reload live in Phase 8 (Replayability).
- `DamageNumber` uses instantiate/destroy; pooling can be added later if profiling shows it matters.
