using MagicalTower.Content;
using UnityEngine;

namespace MagicalTower.Runtime
{
    public sealed class GameplayCompositionRoot : MonoBehaviour
    {
        [Header("Infrastructure")]
        [SerializeField] private RuntimeServiceRegistry serviceRegistry;
        [SerializeField] private RuntimeMessageBus messageBus;

        [Header("Scene Runtime")]
        [SerializeField] private GameSession gameSession;
        [SerializeField] private TowerHealth towerHealth;
        [SerializeField] private ActiveEnemyRegistry enemyRegistry;
        [SerializeField] private EnemyPool enemyPool;
        [SerializeField] private EnemySpawner enemySpawner;
        [SerializeField] private TowerSpellScheduler spellScheduler;

        [Header("Scene Roots")]
        [SerializeField] private Transform enemySpawnRoot;
        [SerializeField] private Transform enemyPoolRoot;
        [SerializeField] private Transform projectileRoot;
        [SerializeField] private Camera viewCamera;

        [Header("Content")]
        [SerializeField] private TowerDefinition towerDefinition;
        [SerializeField] private EnemyPoolConfig enemyPoolConfig;
        [SerializeField] private SpawnScheduleDefinition spawnSchedule;
        [SerializeField] private EnemyAgent enemyPrefab;
        [SerializeField] private SpellCastBinding[] spellBindings;

        private void Awake()
        {
            ResolveLocalFallbacks();
            RegisterServices();
            ConfigureRuntime();
        }

        private void ResolveLocalFallbacks()
        {
            if (serviceRegistry == null)
            {
                serviceRegistry = GetComponentInChildren<RuntimeServiceRegistry>(true);
            }

            if (messageBus == null)
            {
                messageBus = GetComponentInChildren<RuntimeMessageBus>(true);
            }

            if (gameSession == null)
            {
                gameSession = GetComponentInChildren<GameSession>(true);
            }

            if (towerHealth == null)
            {
                towerHealth = GetComponentInChildren<TowerHealth>(true);
            }

            if (enemyRegistry == null)
            {
                enemyRegistry = GetComponentInChildren<ActiveEnemyRegistry>(true);
            }

            if (enemyPool == null)
            {
                enemyPool = GetComponentInChildren<EnemyPool>(true);
            }

            if (enemySpawner == null)
            {
                enemySpawner = GetComponentInChildren<EnemySpawner>(true);
            }

            if (spellScheduler == null)
            {
                spellScheduler = GetComponentInChildren<TowerSpellScheduler>(true);
            }

            if (viewCamera == null)
            {
                viewCamera = GetComponentInChildren<Camera>(true);
            }
        }

        private void RegisterServices()
        {
            if (serviceRegistry == null)
            {
                Debug.LogWarning("GameplayCompositionRoot has no RuntimeServiceRegistry assigned.", this);
                return;
            }

            serviceRegistry.Clear();
            serviceRegistry.Register(serviceRegistry);
            RegisterIfAssigned(messageBus);
            RegisterIfAssigned(gameSession);
            RegisterIfAssigned(towerHealth);
            RegisterIfAssigned(enemyRegistry);
            RegisterIfAssigned(enemyPool);
            RegisterIfAssigned(enemySpawner);
            RegisterIfAssigned(spellScheduler);
        }

        private void ConfigureRuntime()
        {
            gameSession?.Configure(messageBus);
            towerHealth?.Configure(towerDefinition, messageBus, gameSession);
            enemyPool?.Configure(enemyPoolConfig, enemyPrefab, enemyPoolRoot, enemyRegistry, messageBus, towerHealth);
            enemySpawner?.Configure(spawnSchedule, enemyPool, gameSession, enemySpawnRoot, towerHealth, viewCamera);
            spellScheduler?.Configure(spellBindings, enemyRegistry, projectileRoot, messageBus, viewCamera);
        }

        private void RegisterIfAssigned<T>(T service) where T : class
        {
            if (service != null)
            {
                serviceRegistry.Register(service);
            }
        }
    }
}
