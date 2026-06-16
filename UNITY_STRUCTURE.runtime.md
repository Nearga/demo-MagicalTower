# Unity Structure: Runtime

## When to read

Read this before implementing gameplay, scene objects, enemies, spells, projectiles, tower health, status effects, spawning, targeting, damage, game over, or prototype scene composition.

## Primary paths

- Live project-owned runtime paths: none yet under `Assets/`.
- Task brief: parent `Test Task.md`.
- Proposed scene root: `Assets/Scenes/`.
- Proposed runtime source root: `Assets/Scripts/Runtime/`.
- Proposed prefab root: `Assets/Prefabs/`.

## Runtime/source owners

- No live runtime owners exist yet.
- Task-derived owners to create as focused collaborators:
  - Tower health/combat target: `Tower`, `TowerHealth`, or equivalent owner.
  - Game/session flow: `GameSession`, `GameOverController`, or equivalent thin state owner.
  - Enemy lifecycle: enemy component plus enemy movement/combat collaborators.
  - Enemy spawning: spawn scheduler that reads spawn period data and instantiates enemy definitions.
  - Target registry: active enemy registry/runtime set for spell targeting.
  - Spell casting: tower spell scheduler using configurable spell definitions and cooldowns.
  - Projectiles: separate fireball and barrage projectile behavior owners if their movement/impact rules differ.
  - Damage/status: damage service or local damage receiver plus burning damage-over-time owner.
  - Scene bootstrap/composition: one gameplay scene owner that wires data, tower, spawner, camera, and UI.

## Data/config owners

- Runtime should read tuning from content definitions rather than hardcoded branches where practical.
- Expected data inputs from `UNITY_STRUCTURE.content.md`: `EnemyDefinition`, `SpellDefinition`, `ProjectileDefinition`, `SpawnScheduleDefinition`, `BurningStatusEffectDefinition`, `TowerDefinition`, and `EnemyPoolConfig` assets.

## Cross-module routes

- Runtime systems should communicate through small interfaces/events where this prevents sibling feature coupling.
- Enemy, tower, spell, projectile, and UI code should not directly own each other's internals.
- Damage numbers and health display should receive events or presentation DTOs from runtime owners, not poll global state from unrelated scripts.
- Keep scene composition/bootstrap responsible for wiring references; avoid static global managers unless a task proves they are needed.

## Validation hints

- After adding runtime code, validate Unity compile/import with Unity `6000.4.8f1`.
- For gameplay proof, use Play Mode with a scene containing tower, camera, spawner, enemy definitions, spell definitions, and UI.
- For architecture proof, list owner chain for each core feature: scene/bootstrap -> system owner -> content definition -> runtime behavior.

## Do-not-touch

- Do not put all spawning, targeting, damage, spell cooldowns, projectile motion, UI, and game over into one large manager.
- Do not hardcode all enemy/spell/spawn tuning in behavior classes when the task explicitly asks for expandable/configurable systems.
- Do not make runtime gameplay systems depend on UI implementation details.
- Do not edit `Library/PackageCache/` or generated Unity cache folders.

## Open gaps

- Gameplay foundation scene exists at `Assets/Scenes/MagicalTowerPrototype.unity`; runtime components are not wired yet.
- Scene hierarchy is grouped under `GameRoot`: `GameplayRoot` owns tower/spawn/pool/projectile/VFX roots, `UIRoot` owns canvases, `CameraRoot` owns `Main Camera`, and `LightingRoot` owns lights.
- No tower, enemy, spell, projectile, damage, status, spawn, targeting, game-over, or bootstrap owners exist yet.
- Content definitions have compiled/imported, but no Play Mode runtime validation has been run because gameplay runtime source does not exist yet.
