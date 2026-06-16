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
- `Assets/Content/Pooling/EnemyPool_Default.asset` now references `Assets/Prefabs/Gameplay/EnemyAgent.prefab`.
- Asset locations:
  - `Assets/Content/Enemies/`
  - `Assets/Content/Spells/`
  - `Assets/Content/Projectiles/`
  - `Assets/Content/Spawning/`
  - `Assets/Content/Tower/`
  - `Assets/Content/StatusEffects/`
  - `Assets/Content/Pooling/`
- Primitive materials/particles used as source visuals can live under `Assets/Materials/` and `Assets/VFX/` unless a better project convention appears.
- Phase 6 generated texture assets:
  - `Assets/Art/Generated/Textures/Tower_RuneStone.png`
  - `Assets/Art/Generated/Textures/Arena_BasaltFloor.png`
  - `Assets/Art/Generated/Textures/Enemy_ObsidianHide.png`
  - `Assets/Art/Generated/Textures/Fireball_Glow.png`
- Phase 6 material assets:
  - `Assets/Materials/Tower_RuneStone.mat`
  - `Assets/Materials/Arena_BasaltFloor.mat`
  - `Assets/Materials/Enemy_ObsidianHide.mat`
  - `Assets/Materials/Fireball_Glow.mat`
- Phase 7 VFX material assets:
  - `Assets/Materials/FireNova_Flame.mat`
  - `Assets/Materials/BurningEnemy_Flame.mat`
- Phase 7 VFX prefab assets:
  - `Assets/Prefabs/Gameplay/FireNovaEffect.prefab`
  - `Assets/Prefabs/Gameplay/BurningEnemyEffect.prefab`

## Cross-module routes

- Runtime systems should consume content through definitions/config objects.
- A behavior key or strategy enum in a definition is acceptable for a 2-3 hour prototype, but adding a new enemy/spell should not require editing a central switch in many places.
- Spawn schedule data should not directly own enemy movement/combat logic; it should select definitions and timing only.

## Validation hints

- Before gameplay phases, validate every definition has runtime consumers before treating the content path as complete.
- Balance changes should report exact asset/value changes.
- Phase 6 tuning changed enemy contact damage/interval/speed, spawn intervals, and fireball/barrage cooldown/projectile values in existing content assets.
- For expandable-system proof, add at least one path where a new enemy or spell can be configured by creating data plus one focused behavior owner when needed.

## Do-not-touch

- Do not bury all balance values in scene-only serialized fields if the task asks for configurable systems.
- Do not let visual primitive choices decide content architecture.
- Do not duplicate the same spell/enemy tuning across prefabs and scripts without a clear source of truth.

## Open gaps

- Runtime consumers exist under `Assets/Scripts/Runtime/` and Phase 4 gameplay prefabs consume the configured content assets.
- Phase 6 final prototype materials now exist for tower, arena floor, enemy, and fireball glow.
- Initial balance values were tuned in Phase 6 after Play Mode validation exposed projectile self-damage and later confirmed contact-driven tower damage.
