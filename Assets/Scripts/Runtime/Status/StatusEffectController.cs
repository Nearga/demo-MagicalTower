using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using MagicalTower.Content;
using UnityEngine;
using VContainer;

namespace MagicalTower.Runtime
{
    public sealed class StatusEffectController : MonoBehaviour
    {
        [SerializeField] private MonoBehaviour damageReceiverSource;
        [SerializeField] private BurningStatusVisual burningVisualPrefab;
        [SerializeField] private GenericObjectPool burningVisualPool;

        private IDamageReceiver damageReceiver;
        private RuntimeMessageBus messageBus;
        private CancellationTokenSource burningCts;
        private CancellationTokenSource linkedBurningCts;
        private BurningStatusVisual activeBurningVisual;
        private int burningRunId;

        private void Awake()
        {
            damageReceiver = damageReceiverSource as IDamageReceiver;
            if (damageReceiver == null)
            {
                damageReceiver = GetComponent<IDamageReceiver>();
                damageReceiverSource = damageReceiver as MonoBehaviour;
            }
        }

        private void OnDisable()
        {
            ResetForPool();
        }

        [Inject]
        public void Construct(RuntimeMessageBus bus, EnemyPool enemyPool)
        {
            messageBus = bus;
            if (burningVisualPool == null && enemyPool != null)
            {
                burningVisualPool = enemyPool.BurningVisualPool;
            }
        }

        public void ApplyBurning(BurningStatusEffectDefinition definition, GameObject source)
        {
            if (definition == null || damageReceiver == null)
            {
                return;
            }

            CancelBurningTask();
            var runId = ++burningRunId;

            PlayBurningVisual();

            burningCts = new CancellationTokenSource();
            var destroyToken = this.GetCancellationTokenOnDestroy();
            linkedBurningCts = CancellationTokenSource.CreateLinkedTokenSource(burningCts.Token, destroyToken);

            BurningRoutineAsync(definition, source, runId, linkedBurningCts.Token).Forget();
        }

        public void ResetForPool()
        {
            burningRunId++;
            CancelBurningTask();
            StopBurningVisual();
        }

        private async UniTaskVoid BurningRoutineAsync(
            BurningStatusEffectDefinition definition,
            GameObject source,
            int runId,
            CancellationToken cancellationToken)
        {
            var elapsed = 0f;
            while (elapsed < definition.Duration && damageReceiver != null && damageReceiver.IsAlive)
            {
                var isCancelled = await UniTask.Delay(TimeSpan.FromSeconds(definition.TickInterval), cancellationToken: cancellationToken).SuppressCancellationThrow();
                if (isCancelled)
                {
                    return;
                }

                elapsed += definition.TickInterval;

                if (damageReceiver != null && damageReceiver.IsAlive)
                {
                    var tickPosition = (damageReceiverSource != null)
                        ? damageReceiverSource.transform.position
                        : Vector3.zero;

                    damageReceiver.TakeDamage(new DamageRequest(definition.DamagePerTick, source, tickPosition));
                    GameLog.Info(
                        LogChannel.Damage,
                        $"Burning tick dealt {definition.DamagePerTick} damage to {damageReceiver.GetType().Name}.",
                        this);

                    // Separate message so UI can color burning ticks distinctly
                    if (messageBus != null && damageReceiver is EnemyAgent enemy)
                    {
                        messageBus.Publish(new BurningTickMessage(enemy, definition.DamagePerTick, tickPosition));
                    }
                }
            }

            StopBurning(runId);
        }

        private void PlayBurningVisual()
        {
            if (activeBurningVisual == null && (burningVisualPool != null || burningVisualPrefab != null))
            {
                activeBurningVisual = burningVisualPool != null
                    ? burningVisualPool.Rent<BurningStatusVisual>(transform.position, transform.rotation)
                    : Instantiate(burningVisualPrefab, transform);

                if (activeBurningVisual != null)
                {
                    activeBurningVisual.transform.SetParent(transform, true);
                }
            }

            activeBurningVisual?.Play();
        }

        private void StopBurning(int runId)
        {
            if (runId != burningRunId)
            {
                return;
            }

            CancelBurningTask();

            if (activeBurningVisual != null)
            {
                StopBurningVisual();
            }
        }

        private void CancelBurningTask()
        {
            if (burningCts != null)
            {
                burningCts.Cancel();
            }

            linkedBurningCts?.Dispose();
            linkedBurningCts = null;

            burningCts?.Dispose();
            burningCts = null;
        }

        private void StopBurningVisual()
        {
            if (activeBurningVisual == null)
            {
                return;
            }

            activeBurningVisual.StopAndDestroy();
            activeBurningVisual = null;
        }
    }
}
