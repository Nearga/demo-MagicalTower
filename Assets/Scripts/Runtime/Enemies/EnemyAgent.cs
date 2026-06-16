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
        [SerializeField] private EnemyMovementController movementController;
        [SerializeField] private EnemyAttackController attackController;
        [SerializeField] private StatusEffectController statusEffectController;

        private RuntimeMessageBus messageBus;

        public event Action<EnemyAgent> Defeated;

        public EnemyDefinition Definition => definition;
        public int CurrentHealth { get; private set; }
        public bool IsAlive => gameObject.activeInHierarchy && CurrentHealth > 0;
        public Transform Transform => transform;

        public void Configure(
            EnemyDefinition enemyDefinition,
            TowerHealth targetTower,
            ActiveEnemyRegistry activeRegistry,
            EnemyPool pool)
        {
            definition = enemyDefinition;
            registry = activeRegistry;
            owningPool = pool;

            EnsureCollaborators();
            ResetFromDefinition();
            movementController?.Configure(this, targetTower != null ? targetTower.transform : null);
            attackController?.Configure(this, targetTower);
            statusEffectController?.Configure(this);
        }

        [Inject]
        public void Construct(RuntimeMessageBus bus)
        {
            messageBus = bus;
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
            if (movementController == null)
            {
                movementController = GetComponent<EnemyMovementController>();
            }

            if (attackController == null)
            {
                attackController = GetComponent<EnemyAttackController>();
            }

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
