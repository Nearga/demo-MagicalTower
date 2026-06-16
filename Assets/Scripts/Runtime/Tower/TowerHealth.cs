using System;
using MagicalTower.Content;
using UnityEngine;

namespace MagicalTower.Runtime
{
    public sealed class TowerHealth : MonoBehaviour, IDamageReceiver
    {
        [SerializeField] private TowerDefinition definition;
        [SerializeField] private RuntimeMessageBus messageBus;
        [SerializeField] private GameSession gameSession;

        private bool initialized;

        public event Action<int, int> HealthChanged;
        public event Action Destroyed;

        public int CurrentHealth { get; private set; }
        public int MaxHealth => definition != null ? definition.MaxHealth : 0;
        public bool IsAlive => initialized && CurrentHealth > 0;
        public Transform Transform => transform;

        public void Configure(TowerDefinition towerDefinition, RuntimeMessageBus bus, GameSession session)
        {
            definition = towerDefinition;
            messageBus = bus;
            gameSession = session;
            InitializeHealth();
        }

        private void Awake()
        {
            InitializeHealth();
        }

        public DamageReport TakeDamage(DamageRequest request)
        {
            if (!IsAlive)
            {
                return new DamageReport(this, 0, transform.position, false);
            }

            var amount = Mathf.Max(0, request.Amount);
            CurrentHealth = Mathf.Max(0, CurrentHealth - amount);
            var position = request.WorldPosition ?? transform.position;
            var wasFatal = CurrentHealth == 0;
            var report = new DamageReport(this, amount, position, wasFatal);

            HealthChanged?.Invoke(CurrentHealth, MaxHealth);
            messageBus?.Publish(new TowerHealthChangedMessage(this, CurrentHealth, MaxHealth));
            messageBus?.Publish(new DamageDealtMessage(report));

            if (wasFatal)
            {
                Destroyed?.Invoke();
                gameSession?.EndGame(this);
            }

            return report;
        }

        private void InitializeHealth()
        {
            if (definition == null)
            {
                return;
            }

            initialized = true;
            CurrentHealth = definition.MaxHealth;
            HealthChanged?.Invoke(CurrentHealth, MaxHealth);
            messageBus?.Publish(new TowerHealthChangedMessage(this, CurrentHealth, MaxHealth));
        }
    }
}
