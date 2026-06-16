# Unity Structure: Content

## When to read

Read this before adding or tuning enemies, spells, projectile parameters, burning status, spawn schedules, tower health, balance values, or ScriptableObject/config assets.

## Primary paths

- Live project-owned content paths:
  - `Assets/Scripts/Content/`
  - `Assets/Content/Tower/`
  - `Assets/Content/Enemies/`
  - `Assets/Content/Spells/`
  - `Assets/Content/Projectiles/`
  - `Assets/Content/StatusEffects/`
  - `Assets/Content/Spawning/`
  - `Assets/Content/Pooling/`
- Task brief: parent `Test Task.md`.
- Proposed content root: `Assets/Content/`.
- Definition source root: `Assets/Scripts/Content/`.

## Runtime/source owners

- Live content definition types:
  - `TowerDefinition`
  - `EnemyDefinition`
  - `SpellDefinition`
  - `ProjectileDefinition`
  - `BurningStatusEffectDefinition`
  - `SpawnScheduleDefinition`
  - `EnemyPoolConfig`
- Live content helper enums/structs:
  - `SpellTargetMode`
  - `StatusStackPolicy`
  - `SpawnTimeBand`
  - `WeightedEnemyEntry`
- No live runtime bridge exists yet; Phase 3 runtime systems should consume these definitions.

## Data/config owners

- ScriptableObject assets are the current source of truth for enemy/spell/projectile/spawn/tower/status/pooling tuning.
- Asset locations:
  - `Assets/Content/Enemies/`
  - `Assets/Content/Spells/`
  - `Assets/Content/Projectiles/`
  - `Assets/Content/Spawning/`
  - `Assets/Content/Tower/`
  - `Assets/Content/StatusEffects/`
  - `Assets/Content/Pooling/`
- Primitive materials/particles used as source visuals can live under `Assets/Materials/` and `Assets/VFX/` unless a better project convention appears.

## Cross-module routes

- Runtime systems should consume content through definitions/config objects.
- A behavior key or strategy enum in a definition is acceptable for a 2-3 hour prototype, but adding a new enemy/spell should not require editing a central switch in many places.
- Spawn schedule data should not directly own enemy movement/combat logic; it should select definitions and timing only.

## Validation hints

- Before gameplay phases, validate every definition has runtime consumers before treating the content path as complete.
- Balance changes should report exact asset/value changes.
- For expandable-system proof, add at least one path where a new enemy or spell can be configured by creating data plus one focused behavior owner when needed.

## Do-not-touch

- Do not bury all balance values in scene-only serialized fields if the task asks for configurable systems.
- Do not let visual primitive choices decide content architecture.
- Do not duplicate the same spell/enemy tuning across prefabs and scripts without a clear source of truth.

## Open gaps

- No runtime consumers exist yet for the content definitions.
- No materials, particles, or prefabs exist yet.
- Initial balance values exist in Phase 2 assets and can be tuned after Play Mode validation.
