using System;
using System.Collections.Generic;
using UnityEngine;

namespace MagicalTower.Runtime
{
    public sealed class RuntimeServiceRegistry : MonoBehaviour
    {
        private readonly Dictionary<Type, object> services = new Dictionary<Type, object>();

        public void Register<T>(T service) where T : class
        {
            if (service == null)
            {
                Debug.LogWarning($"Cannot register null service for {typeof(T).Name}.", this);
                return;
            }

            services[typeof(T)] = service;
        }

        public bool TryGet<T>(out T service) where T : class
        {
            if (services.TryGetValue(typeof(T), out var value) && value is T typed)
            {
                service = typed;
                return true;
            }

            service = null;
            return false;
        }

        public T Get<T>() where T : class
        {
            if (TryGet<T>(out var service))
            {
                return service;
            }

            throw new InvalidOperationException($"Service is not registered: {typeof(T).Name}");
        }

        public void Clear()
        {
            services.Clear();
        }
    }
}
