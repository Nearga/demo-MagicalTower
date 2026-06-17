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
  - `Assets/Scripts/Runtime/Pooling/`
  - `Assets/Scripts/Runtime/Utility/`
- Task brief: parent `Test Task.md`.
- Proposed scene root: `Assets/Scenes/`.
- Proposed runtime source root: `Assets/Scripts/Runtime/`.
- Proposed prefab root: `Assets/Prefabs/`.
- Live gameplay prefab root: `Assets/Prefabs/Gameplay/`.

## Runtime/source owners

- Live runtime owners:
  - Scene composition/internal services: `GameplayCompositionRoot` as a VContainer `LifetimeScope`, `RuntimeMessageBus`.
  - Game/session flow: `GameSession`, `SceneLoader`.
  - Player tower health/combat target: `PlayersTower`.
  - Enemy lifecycle: `EnemyAgent`, `EnemyMovementController`, `EnemyAttackController`.
  - Generic visual pooling: `GenericObjectPool`, `PooledObject`.
  - Enemy spawning/pooling: `EnemySpawner`, `EnemyPool` as a typed facade over `GenericObjectPool`.
  - Target registry: `ActiveEnemyRegistry`.
  - Spell casting: `TowerSpellScheduler`.
  - Projectiles: `LinearExplosiveProjectile`, `ArcTargetProjectile`, `FireNovaEffect`.
  - Damage/status: `IDamageReceiver`, `DamageRequest`, `DamageReport`, `StatusEffectController`, `BurningStatusVisual`.
  - Editor-only diagnostics: `GameLog`, `LogChannel`.
- Live prefab owners:
  - `Assets/Prefabs/Gameplay/Tower.prefab`
  - `Assets/Prefabs/Gameplay/EnemyAgent.prefab`
  - `Assets/Prefabs/Gameplay/FireballProjectile.prefab`
  - `Assets/Prefabs/Gameplay/BarrageProjectile.prefab`
  - `Assets/Prefabs/Gameplay/FireNovaEffect.prefab`
  - `Assets/Prefabs/Gameplay/BurningEnemyEffect.prefab`

## Data/config owners

- Runtime should read tuning from content definitions rather than hardcoded branches where practical.
- Expected data inputs from `UNITY_STRUCTURE.content.md`: `EnemyDefinition`, `SpellDefinition`, `ProjectileDefinition`, `SpawnScheduleDefinition`, `BurningStatusEffectDefinition`, `TowerDefinition`, and `EnemyPoolConfig` assets.
- Phase 6 tower HP ownership:
  - Enemy contact damage is collision-gated in `EnemyAttackController` using enemy/tower colliders plus `contactTolerance`; center-distance `attackRange` is no longer the tower damage gate.
  - `EnemyMovementController` follows the same collider-contact semantics before stopping; missing-collider fallback is intentionally small.
  - `LinearExplosiveProjectile` ignores `PlayersTower` colliders/receivers so tower-cast fireballs cannot damage the tower.
- Phase 7 VFX ownership:
  - `LinearExplosiveProjectile` spawns `FireNovaEffect` at impact using `ProjectileDefinition.ImpactRadius` as the visual radius.
  - `TowerSpellScheduler` owns the scene `VfxRoot` reference for transient explosion parenting.
  - `StatusEffectController` owns burning visual lifecycle, instantiating one `BurningStatusVisual` while `BurningRoutine` is active and cleaning it up on refresh, expiry, disable, death, or pool return.
- Visual pooling ownership:
  - `GenericObjectPool` is a scene-visible, Inspector-tuned MonoBehaviour pool for enemies, projectiles, and VFX.
  - `GameplayCompositionRoot` injects all scene `GenericObjectPool` components on container build so pooled prefab instances are created through VContainer.
  - The prototype scene wires enemy pooling on `EnemiesRoot` and projectile/fire nova/burning visual pools on `Tower`.
  - `PooledObject` owns return-to-pool behavior for runtime instances and falls back to destroy only when no owner pool exists.
- Replay ownership:
  - `SceneLoader` lives on scene `GameRoot` and owns active-scene reload for the `PLAY AGAIN` button.
  - Replay is a full scene reload, so `GameSession`, `PlayersTower`, pools, spawner timers, and spell scheduling reset through normal scene/prefab initialization rather than custom reset methods.
  - `TimerGameOverButton` on `ElapsedTimeLabel` ends the active round through `GameSession.EndGame(playersTower)` for a direct clickable timer debug path.

## Cross-module routes

- Runtime systems should communicate through small interfaces/events where this prevents sibling feature coupling.
- Enemy, tower, spell, projectile, and UI code should not directly own each other's internals.
- Damage numbers and health display should receive events or presentation DTOs from runtime owners, not poll global state from unrelated scripts.
- Keep scene composition/bootstrap responsible for wiring references; use VContainer for obvious shared runtime services and avoid static global managers unless a task proves they are needed.

## Validation hints

- After adding runtime code, validate Unity compile/import with Unity `6000.4.8f1`.
- For gameplay proof, use Play Mode with a scene containing tower, camera, spawner, enemy definitions, spell definitions, and UI.
- Phase 6 validation proof captured contact logs showing tower HP stayed at 100 until enemy contact; screenshot saved to `Assets/Screenshots~/phase6-polish-validation.png`.
- Phase 7 validation captured DOTween fire nova/burning VFX proof with screenshot `Assets/Screenshots~/phase7-fire-nova-burning.png`.
- For architecture proof, list owner chain for each core feature: scene/bootstrap -> system owner -> content definition -> runtime behavior.

## Do-not-touch

- Do not put all spawning, targeting, damage, spell cooldowns, projectile motion, UI, and game over into one large manager.
- Do not hardcode all enemy/spell/spawn tuning in behavior classes when the task explicitly asks for expandable/configurable systems.
- Do not make runtime gameplay systems depend on UI implementation details.
- Do not edit `Library/PackageCache/` or generated Unity cache folders.

## Open gaps

- Gameplay foundation scene exists at `Assets/Scenes/MagicalTowerPrototype.unity`; Phase 4 explicitly wires runtime components and content references through scene component fields. `GameplayCompositionRoot` is now a VContainer `LifetimeScope` installer only; runtime components own their local content/root/camera fields and shared dependencies are injected. `RuntimeMessageBus` consumers receive it through VContainer injection only; do not reintroduce serialized bus fields or manual bus parameters.
- Enemy movement, attack, and status controllers use VContainer/local component lookup for stable dependencies; `EnemyAgent.Configure` remains only for per-spawn enemy definition and owning pool payload, and projectile `Configure` methods remain per-cast payload.
- Scene hierarchy is grouped under `GameRoot`: `GameplayRoot` owns `Tower`, `EnemiesRoot`, `ProjectileRoot`, `VfxRoot`, and `ArenaFloor`; `EnemiesRoot` owns `ActiveEnemyRegistry`, `EnemySpawner`, and `EnemyPool`, with `EnemySpawnRoot` and `EnemyPoolRoot` as child container transforms; `UIRoot` owns canvases; `CameraRoot` owns `Main Camera` and lights.
- Runtime owners and gameplay prefabs are wired for a runnable prototype slice.
- Phase 5 UI presenters, HUD, game-over panel, and damage numbers are wired through `GameplayCompositionRoot`.
- Phase 6 balance/polish is implemented: generated materials are assigned to tower/enemy/fireball prefabs, `ArenaFloor` is scene-owned and visual-only, and tower HP loss is limited to collision-gated enemy contact.
- Phase 8 replayability is implemented: `SceneLoader` on `GameRoot` reloads the build-indexed active scene from the game-over `PLAY AGAIN` button.
