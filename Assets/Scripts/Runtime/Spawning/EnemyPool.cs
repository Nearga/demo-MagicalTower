using System;
using MagicalTower.Content;
using UnityEngine;

namespace MagicalTower.Runtime
{
    public sealed class EnemyPool : MonoBehaviour
    {
        [SerializeField] private EnemyPoolConfig config;
        [SerializeField] private EnemyAgent enemyPrefab;
        [SerializeField] private Transform poolRoot;
        [SerializeField] private GenericObjectPool objectPool;
        [SerializeField] private GenericObjectPool burningVisualPool;

        public GenericObjectPool BurningVisualPool => burningVisualPool;

        private void Awake()
        {
            if (poolRoot == null)
            {
                poolRoot = transform;
            }

            if (objectPool == null)
            {
                objectPool = GetComponent<GenericObjectPool>();
            }

            enemyPrefab = enemyPrefab != null ? enemyPrefab : ResolvePrefabFromConfig();
        }

        private void Start()
        {
            PreparePools(prewarmEnemyPool: true);
        }

        public EnemyAgent Spawn(EnemyDefinition definition, Vector3 position, Quaternion rotation)
        {
            if (definition == null)
            {
                return null;
            }

            var enemy = RentEnemy(position, rotation);
            if (enemy == null)
            {
                return null;
            }

            enemy.transform.SetPositionAndRotation(position, rotation);
            enemy.Configure(definition, this);
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

            var pooledObject = enemy.GetComponent<PooledObject>();
            if (objectPool != null && pooledObject != null && pooledObject.Owner == objectPool)
            {
                objectPool.Return(enemy.gameObject);
            }
            else
            {
                enemy.gameObject.SetActive(false);
                enemy.transform.SetParent(poolRoot, true);
            }

            GameLog.Info(LogChannel.Pooling, "Returned enemy to pool.", enemy);
        }

        private EnemyAgent RentEnemy(Vector3 position, Quaternion rotation)
        {
            PreparePools(prewarmEnemyPool: true);
            RequireObjectPool();
            return objectPool.RentInactive<EnemyAgent>(position, rotation);
        }

        private void PreparePools(bool prewarmEnemyPool)
        {
            if (objectPool == null)
            {
                objectPool = GetComponent<GenericObjectPool>();
            }

            if (objectPool != null)
            {
                if (prewarmEnemyPool)
                {
                    objectPool.Prewarm();
                }
            }
        }

        private void RequireObjectPool()
        {
            if (objectPool != null)
            {
                return;
            }

            throw new InvalidOperationException(
                "EnemyPool is misconfigured: objectPool is not assigned and no GenericObjectPool exists on the same GameObject. " +
                $"EnemyPool='{name}', GameObjectPath='{GetHierarchyPath(transform)}', " +
                $"Config='{(config != null ? config.name : "null")}', " +
                $"EnemyPrefab='{(enemyPrefab != null ? enemyPrefab.name : "null")}', " +
                $"PoolRoot='{(poolRoot != null ? GetHierarchyPath(poolRoot) : "null")}', " +
                $"BurningVisualPool='{(burningVisualPool != null ? burningVisualPool.name : "null")}'.");
        }

        private EnemyAgent ResolvePrefabFromConfig()
        {
            if (config == null || config.EnemyPrefab == null)
            {
                return null;
            }

            return config.EnemyPrefab.GetComponent<EnemyAgent>();
        }

        private static string GetHierarchyPath(Transform target)
        {
            if (target == null)
            {
                return "null";
            }

            var path = target.name;
            var current = target.parent;
            while (current != null)
            {
                path = $"{current.name}/{path}";
                current = current.parent;
            }

            return path;
        }
    }
}
