using System.Collections;
using MagicalTower.Content;
using UnityEngine;

namespace MagicalTower.Runtime
{
    public sealed class StatusEffectController : MonoBehaviour
    {
        [SerializeField] private MonoBehaviour damageReceiverSource;
        [SerializeField] private BurningStatusVisual burningVisualPrefab;

        private IDamageReceiver damageReceiver;
        private RuntimeMessageBus messageBus;
        private Coroutine burningRoutine;
        private BurningStatusVisual activeBurningVisual;

        private void Awake()
        {
            damageReceiver = damageReceiverSource as IDamageReceiver;
        }

        private void OnDisable()
        {
            burningRoutine = null;
            StopBurningVisual();
        }

        public void Configure(IDamageReceiver receiver, RuntimeMessageBus bus)
        {
            damageReceiver        = receiver;
            damageReceiverSource  = receiver as MonoBehaviour;
            messageBus            = bus;
        }

        public void ApplyBurning(BurningStatusEffectDefinition definition, GameObject source)
        {
            if (definition == null || damageReceiver == null)
            {
                return;
            }

            if (definition.StackPolicy == StatusStackPolicy.RefreshDuration && burningRoutine != null)
            {
                StopCoroutine(burningRoutine);
                burningRoutine = null;
            }

            PlayBurningVisual();
            burningRoutine = StartCoroutine(BurningRoutine(definition, source));
        }

        private IEnumerator BurningRoutine(BurningStatusEffectDefinition definition, GameObject source)
        {
            var elapsed = 0f;
            while (elapsed < definition.Duration && damageReceiver.IsAlive)
            {
                yield return new WaitForSeconds(definition.TickInterval);
                elapsed += definition.TickInterval;

                if (damageReceiver.IsAlive)
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

            StopBurning();
        }

        private void PlayBurningVisual()
        {
            if (activeBurningVisual == null && burningVisualPrefab != null)
            {
                activeBurningVisual = Instantiate(burningVisualPrefab, transform);
            }

            activeBurningVisual?.Play();
        }

        private void StopBurning()
        {
            if (burningRoutine != null)
            {
                StopCoroutine(burningRoutine);
                burningRoutine = null;
            }

            if (activeBurningVisual != null)
            {
                StopBurningVisual();
            }
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
