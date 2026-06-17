using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace MagicalTower.Runtime
{
    public sealed class GenericObjectPool : MonoBehaviour
    {
        [SerializeField] private GameObject prefab;
        [SerializeField] private int initialSize = 8;
        [SerializeField] private int maxSize = 32;
        [SerializeField] private Transform inactiveContainer;
        [SerializeField] private bool prewarmOnAwake = true;
        [SerializeField] private bool allowExpansionUntilMax = true;

        private readonly Queue<GameObject> inactiveInstances = new Queue<GameObject>();
        private readonly HashSet<GameObject> inactiveLookup = new HashSet<GameObject>();
        private IObjectResolver objectResolver;
        private int createdCount;
        private bool prewarmed;
        private bool awakeCompleted;

        public GameObject Prefab => prefab;
        public int InitialSize => Mathf.Max(0, initialSize);
        public int MaxSize => Mathf.Max(0, maxSize);
        public int CreatedCount => createdCount;
        public int InactiveCount => inactiveInstances.Count;
        public string Diagnostics =>
            $"Pool='{name}', GameObjectPath='{GetHierarchyPath(transform)}', " +
            $"Prefab='{(prefab != null ? prefab.name : "null")}', InitialSize={InitialSize}, " +
            $"MaxSize={MaxSize}, CreatedCount={createdCount}, InactiveCount={inactiveInstances.Count}, " +
            $"AllowExpansionUntilMax={allowExpansionUntilMax}, PrewarmOnAwake={prewarmOnAwake}, " +
            $"InactiveContainer='{(inactiveContainer != null ? GetHierarchyPath(inactiveContainer) : "null")}'";

        [Inject]
        public void Construct(IObjectResolver resolver)
        {
            objectResolver = resolver;
            if (awakeCompleted && prewarmOnAwake)
            {
                Prewarm();
            }
        }

        private void Awake()
        {
            if (inactiveContainer == null)
            {
                inactiveContainer = transform;
            }

            awakeCompleted = true;
            if (prewarmOnAwake && objectResolver != null)
            {
                Prewarm();
            }
        }

        private void Start()
        {
            if (prewarmOnAwake)
            {
                Prewarm();
            }
        }

        public void Prewarm()
        {
            if (prewarmed)
            {
                return;
            }

            prewarmed = true;
            var targetCount = Mathf.Min(InitialSize, MaxSize);
            while (createdCount < targetCount)
            {
                var instance = CreateInstance();
                if (instance == null)
                {
                    return;
                }

                Return(instance);
            }
        }

        public GameObject Rent(Vector3 position, Quaternion rotation)
        {
            var instance = RentInactive(position, rotation);
            if (instance != null)
            {
                instance.SetActive(true);
            }

            return instance;
        }

        public GameObject RentInactive(Vector3 position, Quaternion rotation)
        {
            Prewarm();

            var instance = GetOrCreate();
            if (instance == null)
            {
                return null;
            }

            inactiveLookup.Remove(instance);
            instance.transform.SetParent(transform, true);
            instance.transform.SetPositionAndRotation(position, rotation);
            return instance;
        }

        public T Rent<T>(Vector3 position, Quaternion rotation) where T : Component
        {
            var instance = Rent(position, rotation);
            if (instance != null && instance.TryGetComponent<T>(out var component))
            {
                return component;
            }

            Return(instance);
            throw new InvalidOperationException(
                $"GenericObjectPool rented an instance that does not contain required component '{typeof(T).Name}'. " +
                $"{Diagnostics}. Instance='{(instance != null ? instance.name : "null")}'.");
        }

        public T RentInactive<T>(Vector3 position, Quaternion rotation) where T : Component
        {
            var instance = RentInactive(position, rotation);
            if (instance != null && instance.TryGetComponent<T>(out var component))
            {
                return component;
            }

            Return(instance);
            throw new InvalidOperationException(
                $"GenericObjectPool rented an inactive instance that does not contain required component '{typeof(T).Name}'. " +
                $"{Diagnostics}. Instance='{(instance != null ? instance.name : "null")}'.");
        }

        public void Return(GameObject instance)
        {
            if (instance == null)
            {
                return;
            }

            if (inactiveLookup.Contains(instance))
            {
                return;
            }

            var pooledObject = EnsurePooledObject(instance);
            pooledObject.SetOwner(this);

            instance.SetActive(false);
            instance.transform.SetParent(inactiveContainer != null ? inactiveContainer : transform, true);
            inactiveInstances.Enqueue(instance);
            inactiveLookup.Add(instance);
        }

        private GameObject GetOrCreate()
        {
            while (inactiveInstances.Count > 0)
            {
                var instance = inactiveInstances.Dequeue();
                if (instance == null)
                {
                    continue;
                }

                inactiveLookup.Remove(instance);
                return instance;
            }

            if (!allowExpansionUntilMax || createdCount >= MaxSize)
            {
                throw new InvalidOperationException(
                    $"GenericObjectPool cannot rent because it reached capacity or expansion is disabled. {Diagnostics}.");
            }

            return CreateInstance();
        }

        private GameObject CreateInstance()
        {
            if (prefab == null)
            {
                throw new InvalidOperationException(
                    $"GenericObjectPool is misconfigured: prefab is not assigned. {Diagnostics}.");
            }

            var parent = inactiveContainer != null ? inactiveContainer : transform;
            var instance = objectResolver != null
                ? objectResolver.Instantiate(prefab, parent)
                : Instantiate(prefab, parent);

            EnsurePooledObject(instance).SetOwner(this);
            instance.SetActive(false);
            createdCount++;
            return instance;
        }

        private static PooledObject EnsurePooledObject(GameObject instance)
        {
            if (!instance.TryGetComponent<PooledObject>(out var pooledObject))
            {
                pooledObject = instance.AddComponent<PooledObject>();
            }

            return pooledObject;
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
