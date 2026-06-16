using System.Collections.Generic;
using MagicalTower.Content;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace MagicalTower.Runtime
{
    public sealed class EnemyPool : MonoBehaviour
    {
        [SerializeField] private EnemyPoolConfig config;
        [SerializeField] private EnemyAgent enemyPrefab;
        [SerializeField] private Transform poolRoot;

        private readonly Queue<EnemyAgent> inactiveEnemies = new Queue<EnemyAgent>();
        private IObjectResolver objectResolver;
        private ActiveEnemyRegistry registry;
        private TowerHealth targetTower;
        private int createdCount;
        private bool warnedMissingPrefab;

        private int InitialCapacity => config != null ? config.InitialCapacity : 0;
        private int MaxCapacity => config != null ? config.MaxCapacity : 50;

        [Inject]
        public void Construct(IObjectResolver resolver, ActiveEnemyRegistry activeRegistry, TowerHealth tower)
        {
            objectResolver = resolver;
            registry = activeRegistry;
            targetTower = tower;
            enemyPrefab = enemyPrefab != null ? enemyPrefab : ResolvePrefabFromConfig();
            Prewarm();
        }

        private void Awake()
        {
            if (poolRoot == null)
            {
                poolRoot = transform;
            }
        }

        public EnemyAgent Spawn(EnemyDefinition definition, Vector3 position, Quaternion rotation)
        {
            if (definition == null)
            {
                return null;
            }

            var enemy = GetOrCreate();
            if (enemy == null)
            {
                return null;
            }

            enemy.transform.SetParent(poolRoot, true);
            enemy.transform.SetPositionAndRotation(position, rotation);
            enemy.Configure(definition, targetTower, registry, this);
            enemy.gameObject.SetActive(true);
            GameLog.Info(LogChannel.Pooling, $"Activated pooled {definition.DisplayName}.", enemy);
            return enemy;
        }

        public void Release(EnemyAgent enemy)
        {
            if (enemy == null)
            {
                return;
            }

            enemy.gameObject.SetActive(false);
            enemy.transform.SetParent(poolRoot, true);
            inactiveEnemies.Enqueue(enemy);
            GameLog.Info(LogChannel.Pooling, "Returned enemy to pool.", enemy);
        }

        private void Prewarm()
        {
            for (var i = createdCount; i < InitialCapacity; i++)
            {
                var enemy = CreateEnemy();
                if (enemy == null)
                {
                    return;
                }

                Release(enemy);
            }
        }

        private EnemyAgent GetOrCreate()
        {
            while (inactiveEnemies.Count > 0)
            {
                var enemy = inactiveEnemies.Dequeue();
                if (enemy != null)
                {
                    return enemy;
                }
            }

            if (createdCount >= MaxCapacity)
            {
                return null;
            }

            return CreateEnemy();
        }

        private EnemyAgent CreateEnemy()
        {
            enemyPrefab = enemyPrefab != null ? enemyPrefab : ResolvePrefabFromConfig();
            if (enemyPrefab == null)
            {
                if (!warnedMissingPrefab)
                {
                    Debug.LogWarning("EnemyPool has no enemy prefab assigned.", this);
                    warnedMissingPrefab = true;
                }

                return null;
            }

            var enemy = objectResolver != null
                ? objectResolver.Instantiate(enemyPrefab, poolRoot)
                : Instantiate(enemyPrefab, poolRoot);
            enemy.gameObject.SetActive(false);
            createdCount++;
            GameLog.Info(LogChannel.Pooling, $"Created enemy pool instance {createdCount}/{MaxCapacity}.", enemy);
            return enemy;
        }

        private EnemyAgent ResolvePrefabFromConfig()
        {
            if (config == null || config.EnemyPrefab == null)
            {
                return null;
            }

            return config.EnemyPrefab.GetComponent<EnemyAgent>();
        }
    }
}
