# Magical Tower Roadmap

## Summary

Build a designer-wired 3D Magical Tower prototype where code provides focused reusable components, while objects, properties, content definitions, prefabs, and references are configured in the Unity editor. Runtime construction should be minimal; scene and prefab composition are the source of truth.

## Phase 1: Project Foundation

- Add TextMeshPro to `Packages/manifest.json`.
- Create project folders:
  - `Assets/Scenes`
  - `Assets/Scripts/Runtime`
  - `Assets/Scripts/Runtime/Pooling`
  - `Assets/Scripts/UI`
  - `Assets/Content`
  - `Assets/Prefabs`
  - `Assets/Materials`
  - `Assets/Art/Generated/Textures`
- Create `Assets/Scenes/MagicalTowerPrototype.unity`.
- Add grouped scene hierarchy:
  - `GameRoot`
  - `GameRoot/GameplayRoot/Tower`
  - `GameRoot/GameplayRoot/EnemySpawnRoot`
  - `GameRoot/GameplayRoot/EnemyPoolRoot`
  - `GameRoot/GameplayRoot/ProjectileRoot`
  - `GameRoot/GameplayRoot/VfxRoot`
  - `GameRoot/UIRoot/WorldCanvas`
  - `GameRoot/UIRoot/HudCanvas`
  - `GameRoot/CameraRoot/Main Camera`
  - `GameRoot/LightingRoot/Directional Light`
  - `GameRoot/LightingRoot/Fill Light`
- Keep Phase 1 free of gameplay implementation code.

## Phase 2: Designer-Configurable Data

- Create ScriptableObject definitions:
  - `TowerDefinition`
  - `EnemyDefinition`
  - `SpellDefinition`
  - `ProjectileDefinition`
  - `BurningStatusEffectDefinition`
  - `SpawnScheduleDefinition`
  - optional `EnemyPoolConfig`
- Store content under:
  - `Assets/Content/Tower`
  - `Assets/Content/Enemies`
  - `Assets/Content/Spells`
  - `Assets/Content/Projectiles`
  - `Assets/Content/StatusEffects`
  - `Assets/Content/Spawning`
  - `Assets/Content/Pooling`
- Enemy variants are data assets, not subclasses: Default Enemy, Fast Enemy, Big Slow Enemy.
- Spell variants are data assets plus behavior components: Fireball Spell, Barrage Spell.

## Phase 3: Runtime Components

- Implement focused MonoBehaviours with serialized Inspector fields:
  - `GameSession`
  - `TowerHealth`
  - `EnemyAgent`
  - `EnemyMovementController`
  - `EnemyAttackController`
  - `EnemySpawner`
  - `EnemyPool`
  - `ActiveEnemyRegistry`
  - `TowerSpellScheduler`
  - `LinearExplosiveProjectile`
  - `ArcTargetProjectile`
  - `StatusEffectController`
- Use assigned scene and prefab references. Avoid `FindObjectOfType` and global singletons.
- Use composition over inheritance:
  - one enemy prefab with swappable `EnemyDefinition`
  - separate movement, attack, health, status, and visual components
  - spell behavior selected through configured definitions/prefab references.

## Phase 4: Prefab And Scene Wiring

- Create prefabs:
  - `Tower.prefab`
  - `EnemyAgent.prefab`
  - `FireballProjectile.prefab`
  - `BarrageProjectile.prefab`
  - `DamageNumber.prefab`
  - optional primitive VFX prefabs.
- Wire references in the editor:
  - `GameSession` references tower/session UI.
  - `EnemySpawner` references spawn schedule, enemy pool, target tower, enemy registry, and `EnemySpawnRoot`.
  - `EnemyPool` references enemy prefab, initial capacity, max capacity, and `EnemyPoolRoot`.
  - `EnemyAgent` returns itself to `EnemyPool` on death or despawn.
  - `TowerSpellScheduler` references spell definitions, projectile roots, and enemy registry.
  - Projectile prefabs reference projectile definitions and damage/status settings.
  - UI presenters subscribe to assigned runtime components/events.
- The scene acts as the designer-editable composition root; code should not construct the whole scene procedurally.

## Phase 5: UI And Feedback

- Add TextMeshPro HUD:
  - tower health bar/value
  - elapsed time
  - game-over panel.
- Add world/screen damage numbers:
  - enemy damage
  - tower damage
  - burning ticks.
- Damage UI is presentation-only; confirmed runtime damage emits the data.
- Use Inspector fields for colors, lifetime, motion, scale, and text prefab references.

## Phase 6: Balance And Polish

- Configure spawn schedule time bands in `SpawnScheduleDefinition`.
- Tune enemy health, speed, scale, and contact damage through `EnemyDefinition`.
- Tune spells through `SpellDefinition` and projectile definitions.
- Apply generated textures to materials:
  - tower rune/stone
  - arena floor
  - enemy surface
  - fireball glow.
- Keep primitive meshes and particles as the default visual style.

## Phase 7: Validation

- Compile/import validation:
  - Unity batchmode compile/import with Unity `6000.4.8f1`.
  - Check logs for missing package, script, prefab, and serialized reference errors.
- Editor wiring validation:
  - open `MagicalTowerPrototype.unity`
  - inspect all root objects for missing references
  - inspect all prefabs for missing definitions/materials.
- Play Mode checklist:
  - enemies spawn from the pool outside view and move toward tower
  - all three enemy definitions behave differently
  - tower takes damage and game over triggers at zero health
  - Fireball explodes, area damages, and applies burning
  - Barrage fires one arcing projectile per visible enemy
  - damage numbers appear for enemies and tower
  - spawn pressure increases over time.

## Phase 1 Status

- Status: in progress.
- Runtime code: intentionally not started.
- Enemy pooling: required for Phase 3 and Phase 4; not implemented in Phase 1.
