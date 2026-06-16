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
            if (!ValidateRequiredReferences())
            {
                enabled = false;
                return;
            }

            RegisterServices();
            ConfigureRuntime();
        }

        private bool ValidateRequiredReferences()
        {
            var isValid = true;

            // Infrastructure
            if (serviceRegistry == null) { Debug.LogError($"[{nameof(GameplayCompositionRoot)}] '{nameof(serviceRegistry)}' is not assigned.", this); isValid = false; }
            if (messageBus     == null) { Debug.LogError($"[{nameof(GameplayCompositionRoot)}] '{nameof(messageBus)}' is not assigned.", this);     isValid = false; }

            // Scene Runtime
            if (gameSession     == null) { Debug.LogError($"[{nameof(GameplayCompositionRoot)}] '{nameof(gameSession)}' is not assigned.", this);     isValid = false; }
            if (towerHealth     == null) { Debug.LogError($"[{nameof(GameplayCompositionRoot)}] '{nameof(towerHealth)}' is not assigned.", this);     isValid = false; }
            if (enemyRegistry   == null) { Debug.LogError($"[{nameof(GameplayCompositionRoot)}] '{nameof(enemyRegistry)}' is not assigned.", this);   isValid = false; }
            if (enemyPool       == null) { Debug.LogError($"[{nameof(GameplayCompositionRoot)}] '{nameof(enemyPool)}' is not assigned.", this);       isValid = false; }
            if (enemySpawner    == null) { Debug.LogError($"[{nameof(GameplayCompositionRoot)}] '{nameof(enemySpawner)}' is not assigned.", this);    isValid = false; }
            if (spellScheduler  == null) { Debug.LogError($"[{nameof(GameplayCompositionRoot)}] '{nameof(spellScheduler)}' is not assigned.", this);  isValid = false; }

            // Scene Roots
            if (enemySpawnRoot  == null) { Debug.LogError($"[{nameof(GameplayCompositionRoot)}] '{nameof(enemySpawnRoot)}' is not assigned.", this);  isValid = false; }
            if (enemyPoolRoot   == null) { Debug.LogError($"[{nameof(GameplayCompositionRoot)}] '{nameof(enemyPoolRoot)}' is not assigned.", this);   isValid = false; }
            if (projectileRoot  == null) { Debug.LogError($"[{nameof(GameplayCompositionRoot)}] '{nameof(projectileRoot)}' is not assigned.", this);  isValid = false; }
            if (viewCamera      == null) { Debug.LogError($"[{nameof(GameplayCompositionRoot)}] '{nameof(viewCamera)}' is not assigned.", this);      isValid = false; }

            // Content
            if (towerDefinition  == null) { Debug.LogError($"[{nameof(GameplayCompositionRoot)}] '{nameof(towerDefinition)}' is not assigned.", this);  isValid = false; }
            if (enemyPoolConfig  == null) { Debug.LogError($"[{nameof(GameplayCompositionRoot)}] '{nameof(enemyPoolConfig)}' is not assigned.", this);  isValid = false; }
            if (spawnSchedule    == null) { Debug.LogError($"[{nameof(GameplayCompositionRoot)}] '{nameof(spawnSchedule)}' is not assigned.", this);    isValid = false; }
            if (enemyPrefab      == null) { Debug.LogError($"[{nameof(GameplayCompositionRoot)}] '{nameof(enemyPrefab)}' is not assigned.", this);      isValid = false; }

            if (spellBindings == null || spellBindings.Length == 0)
            {
                Debug.LogError($"[{nameof(GameplayCompositionRoot)}] '{nameof(spellBindings)}' is not assigned or empty.", this);
                isValid = false;
            }

            return isValid;
        }

        private void RegisterServices()
        {
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
