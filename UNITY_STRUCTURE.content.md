# Unity Structure: Content

## When to read

Read this before adding or tuning enemies, spells, projectile parameters, burning status, spawn schedules, tower health, balance values, or ScriptableObject/config assets.

## Primary paths

- Live project-owned content paths: none yet under `Assets/`.
- Task brief: parent `Test Task.md`.
- Proposed content root: `Assets/Content/`.
- Proposed definition source root, if using C#: `Assets/Scripts/Content/` or a focused definitions folder beside runtime source.

## Runtime/source owners

- No live content/runtime bridge exists yet.
- Task-derived configurable content surfaces to create:
  - Enemy definitions: Default Enemy, Fast Enemy, Big and Slow Enemy.
  - Spell definitions: Fireball Spell and Barrage Spell.
  - Spawn schedule: time-period spawn-rate and enemy mix data for gradually increasing difficulty.
  - Tower tuning: max health and damage intake values.
  - Status effect tuning: Burning Effect damage per tick, duration, tick interval, stacking/refresh policy.
  - Projectile tuning: damage, speed, area radius, cooldown, parabolic arc settings where relevant.

## Data/config owners

- Prefer ScriptableObjects or serialized profile assets for enemy/spell/spawn/tower/status tuning.
- Suggested asset locations once created:
  - `Assets/Content/Enemies/`
  - `Assets/Content/Spells/`
  - `Assets/Content/Spawning/`
  - `Assets/Content/Tower/`
  - `Assets/Content/StatusEffects/`
- Primitive materials/particles used as source visuals can live under `Assets/Materials/` and `Assets/VFX/` unless a better project convention appears.

## Cross-module routes

- Runtime systems should consume content through definitions/config objects.
- A behavior key or strategy enum in a definition is acceptable for a 2-3 hour prototype, but adding a new enemy/spell should not require editing a central switch in many places.
- Spawn schedule data should not directly own enemy movement/combat logic; it should select definitions and timing only.

## Validation hints

- Validate every definition has runtime consumers before treating the content path as complete.
- Balance changes should report exact asset/value changes.
- For expandable-system proof, add at least one path where a new enemy or spell can be configured by creating data plus one focused behavior owner when needed.

## Do-not-touch

- Do not bury all balance values in scene-only serialized fields if the task asks for configurable systems.
- Do not let visual primitive choices decide content architecture.
- Do not duplicate the same spell/enemy tuning across prefabs and scripts without a clear source of truth.

## Open gaps

- No ScriptableObjects, config assets, materials, particles, prefabs, or definitions exist yet.
- Exact balance values are intentionally undecided; `Test Task.md` allows balancing as needed.
