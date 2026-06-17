using System;
using System.Collections.Generic;
using UnityEngine;

namespace MagicalTower.Runtime
{
    public sealed class RuntimeMessageBus : MonoBehaviour
    {
        private readonly Dictionary<Type, List<Delegate>> subscribers = new Dictionary<Type, List<Delegate>>();

        public IDisposable Subscribe<TMessage>(Action<TMessage> handler)
        {
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            var messageType = typeof(TMessage);
            if (!subscribers.TryGetValue(messageType, out var handlers))
            {
                handlers = new List<Delegate>();
                subscribers.Add(messageType, handlers);
            }

            handlers.Add(handler);
            return new Subscription(() => Unsubscribe(messageType, handler));
        }

        public void Publish<TMessage>(TMessage message)
        {
            if (!subscribers.TryGetValue(typeof(TMessage), out var handlers))
            {
                return;
            }

            for (var i = handlers.Count - 1; i >= 0; i--)
            {
                if (i < handlers.Count && handlers[i] is Action<TMessage> handler)
                {
                    handler.Invoke(message);
                }
            }
        }

        private void Unsubscribe(Type messageType, Delegate handler)
        {
            if (!subscribers.TryGetValue(messageType, out var handlers))
            {
                return;
            }

            handlers.Remove(handler);
            if (handlers.Count == 0)
            {
                subscribers.Remove(messageType);
            }
        }

        private sealed class Subscription : IDisposable
        {
            private Action dispose;

            public Subscription(Action dispose)
            {
                this.dispose = dispose;
            }

            public void Dispose()
            {
                dispose?.Invoke();
                dispose = null;
            }
        }
    }
}
