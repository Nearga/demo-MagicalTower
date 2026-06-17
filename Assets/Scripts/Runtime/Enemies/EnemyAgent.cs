using System;
using MagicalTower.Content;
using UnityEngine;
using VContainer;

namespace MagicalTower.Runtime
{
    public sealed class EnemyAgent : MonoBehaviour, IDamageReceiver
    {
        [SerializeField] private EnemyDefinition definition;
        [SerializeField] private ActiveEnemyRegistry registry;
        [SerializeField] private EnemyPool owningPool;
        [SerializeField] private StatusEffectController statusEffectController;

        [field: SerializeField] public int CurrentHealth { get; private set; }

        private RuntimeMessageBus messageBus;

        public event Action<EnemyAgent> Defeated;

        public EnemyDefinition Definition => definition;
        
        public bool IsAlive => gameObject.activeInHierarchy && CurrentHealth > 0;
        public Transform Transform => transform;

        public void Configure(EnemyDefinition enemyDefinition, EnemyPool pool)
        {
            definition = enemyDefinition;
            owningPool = pool;

            EnsureCollaborators();
            ResetFromDefinition();
        }

        [Inject]
        public void Construct(RuntimeMessageBus bus, ActiveEnemyRegistry activeRegistry)
        {
            messageBus = bus;
            registry = activeRegistry;
        }

        private void Awake()
        {
            EnsureCollaborators();
        }

        private void OnEnable()
        {
            if (definition == null)
            {
                return;
            }

            ResetFromDefinition();
            registry?.Register(this);
            messageBus?.Publish(new EnemySpawnedMessage(this));
        }

        private void OnDisable()
        {
            registry?.Unregister(this);
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

            messageBus?.Publish(new DamageDealtMessage(report));
            if (request.BurningStatusEffect != null && statusEffectController != null)
            {
                statusEffectController.ApplyBurning(request.BurningStatusEffect, request.Source);
            }

            if (wasFatal)
            {
                Defeated?.Invoke(this);
                messageBus?.Publish(new EnemyDefeatedMessage(this, request.Source));
                Despawn();
            }

            return report;
        }

        public void Despawn()
        {
            statusEffectController?.ResetForPool();
            registry?.Unregister(this);

            if (owningPool != null)
            {
                owningPool.Release(this);
                return;
            }

            gameObject.SetActive(false);
        }

        private void EnsureCollaborators()
        {
            if (statusEffectController == null)
            {
                statusEffectController = GetComponent<StatusEffectController>();
            }
        }

        private void ResetFromDefinition()
        {
            if (definition == null)
            {
                return;
            }

            CurrentHealth = definition.MaxHealth;
            transform.localScale = Vector3.one * definition.VisualScale;
        }
    }
}
