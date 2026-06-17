# Magical Tower

A 3D survivor-style prototype built in Unity where a magical tower stands firm in the center of the arena against an onslaught of enemies. Designed with a robust, highly modular, and designer-friendly architecture leveraging dependency injection, event-driven communication, object pooling, and fully configurable ScriptableObjects.

---

## 🎮 Core Gameplay Features

1. **Magical Tower**: Centered target with active health management. The game ends immediately when the tower's health reaches zero.
2. **Enemy Onslaught**: Designer-tuned enemy variants that spawn outside the screen boundaries and advance on the tower:
   - **Default Enemy**: Standard speed, health, and size.
   - **Fast Enemy**: High movement speed, low health.
   - **Big and Slow Enemy**: Visually large, high health pool, slow movement.
3. **Magic Spells**: The tower automatically schedules and casts powerful defensive spells:
   - **Fireball Spell**: Launches a single fireball towards a random enemy. Explodes on impact (AoE damage) and applies a burning damage-over-time (DoT) status effect.
   - **Barrage Spell**: Launches arcing, parabolic projectiles targeting every visible enemy simultaneously.
4. **Enemy Spawning**: A time-banded spawner increases game difficulty over time, shifting the spawn rate and enemy weight distribution.
5. **Damage UI**: Spawn dynamic rise-and-fade damage numbers in world space. Styles are distinct for enemy hits, tower hits, and burning status ticks.
6. **VFX & Polish**: DOTween-driven visual effects including fire nova explosions and active burning visual cues on afflicted enemies.
7. **Replayability**: Game over screen with a functional `PLAY AGAIN` button which performs a clean scene reload.

---

## 🏗️ Architectural Highlights

- **Dependency Injection (VContainer)**: Uses `GameplayCompositionRoot` as the scene's `LifetimeScope` installer. decodes direct references, wires dependencies cleanly, and registers global services.
- **Event Bus (`RuntimeMessageBus`)**: Communication between sibling systems is entirely decoupled using a custom pub-sub message bus. It is highly optimized to use **zero-allocation direct backward iteration** on publish.
- **Composition over Inheritance**: Game entities are assembled via modular components rather than rigid inheritance hierarchies. For instance, the enemy prefab is generic and binds `EnemyMovementController`, `EnemyAttackController`, and `StatusEffectController` dynamically.
- **Data-Driven Configuration**: All gameplay properties, balances, spawn rates, and visual variables are defined inside `ScriptableObjects` (located in `Assets/Content/`), allowing designers to modify balance without touching code.
- **Object Pooling**: Highly efficient generic reuse system (`GenericObjectPool` & `PooledObject`) for managing enemies, projectiles, and status visuals without runtime heap churn.
- **Granular Editor Logging**: Features a custom static `GameLog` tool with conditional compilation `[Conditional("UNITY_EDITOR")]` and togglable channel filters (`LogChannel.Damage`, `LogChannel.Spawning`, etc.). Silenced in production builds with zero performance footprint.

---

## 📁 Repository Structure

```
Assets/
 ├── Art/                     # Generated visual textures
 ├── Content/                 # Designer ScriptableObject configuration assets
 ├── Materials/               # Visual shaders and materials
 ├── Plugins/                 # Third-party assets (DOTween, VContainer)
 ├── Prefabs/                 # Main game and UI prefabs
 ├── Scenes/                  # MagicalTowerPrototype.unity
 ├── Screenshots~/            # Phase verification screenshots
 └── Scripts/
      ├── Content/            # ScriptableObject definition classes
      ├── Runtime/            # Core gameplay logic
      │    ├── Combat/        # Damage receivers and models
      │    ├── Enemies/       # Enemy movement, attacks, and registration
      │    ├── Infrastructure/# RuntimeMessageBus and DI binders
      │    ├── Pooling/       # Generic object pool and facade
      │    ├── Projectiles/   # Projectile physics, movement, and effects
      │    ├── Session/       # Game session and scene loader helpers
      │    ├── Spawning/      # Enemy wave spawner
      │    ├── Spells/        # Tower spell scheduler
      │    ├── Status/        # Status effects (burning)
      │    └── Utility/       # Conditional log system
      └── UI/                 # HUD and game over presenters
```

---

## 🚀 Getting Started

### Prerequisites
- **Unity Editor**: Developed and tested using Unity version `6000.4.8f1`.

### Playing and Testing
1. Clone this repository and open the project root in Unity.
2. Open the main prototype scene: `Assets/Scenes/MagicalTowerPrototype.unity`.
3. Press **Play** in the Unity Editor.
4. **Debug Tip**: During Play Mode, you can click on the elapsed time label in the top-right corner of the HUD (wired to the `TimerGameOverButton` script) to instantly trigger game over and test the replay loop.
