# Unity Structure: Runtime

## When to read

Read this before implementing gameplay, scene objects, enemies, spells, projectiles, tower health, status effects, spawning, targeting, damage, game over, or prototype scene composition.

## Primary paths

- Live project-owned runtime paths:
  - `Assets/Scripts/Runtime/Infrastructure/`
  - `Assets/Scripts/Runtime/Combat/`
  - `Assets/Scripts/Runtime/Session/`
  - `Assets/Scripts/Runtime/Tower/`
  - `Assets/Scripts/Runtime/Enemies/`
  - `Assets/Scripts/Runtime/Spawning/`
  - `Assets/Scripts/Runtime/Spells/`
  - `Assets/Scripts/Runtime/Projectiles/`
  - `Assets/Scripts/Runtime/Status/`
- Task brief: parent `Test Task.md`.
- Proposed scene root: `Assets/Scenes/`.
- Proposed runtime source root: `Assets/Scripts/Runtime/`.
- Proposed prefab root: `Assets/Prefabs/`.
- Live gameplay prefab root: `Assets/Prefabs/Gameplay/`.

## Runtime/source owners

- Live runtime owners:
  - Scene composition/internal services: `GameplayCompositionRoot`, `RuntimeServiceRegistry`, `RuntimeMessageBus`.
  - Game/session flow: `GameSession`.
  - Tower health/combat target: `TowerHealth`.
  - Enemy lifecycle: `EnemyAgent`, `EnemyMovementController`, `EnemyAttackController`.
  - Enemy spawning/pooling: `EnemySpawner`, `EnemyPool`.
  - Target registry: `ActiveEnemyRegistry`.
  - Spell casting: `TowerSpellScheduler`.
  - Projectiles: `LinearExplosiveProjectile`, `ArcTargetProjectile`.
  - Damage/status: `IDamageReceiver`, `DamageRequest`, `DamageReport`, `StatusEffectController`.
- Live prefab owners:
  - `Assets/Prefabs/Gameplay/Tower.prefab`
  - `Assets/Prefabs/Gameplay/EnemyAgent.prefab`
  - `Assets/Prefabs/Gameplay/FireballProjectile.prefab`
  - `Assets/Prefabs/Gameplay/BarrageProjectile.prefab`

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

- Gameplay foundation scene exists at `Assets/Scenes/MagicalTowerPrototype.unity`; Phase 4 explicitly wires runtime components and content references through `GameplayCompositionRoot` and scene component fields.
- Scene hierarchy is grouped under `GameRoot`: `GameplayRoot` owns tower/spawn/pool/projectile/VFX roots, `UIRoot` owns canvases, `CameraRoot` owns `Main Camera`, and `LightingRoot` owns lights.
- Runtime owners and gameplay prefabs are wired for a runnable prototype slice.
- UI presenters, HUD, damage numbers, and final visual feedback remain future work.
