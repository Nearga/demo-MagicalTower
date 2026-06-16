using System.Collections;
using MagicalTower.Content;
using UnityEngine;

namespace MagicalTower.Runtime
{
    public sealed class StatusEffectController : MonoBehaviour
    {
        [SerializeField] private MonoBehaviour damageReceiverSource;

        private IDamageReceiver damageReceiver;
        private Coroutine burningRoutine;

        private void Awake()
        {
            damageReceiver = damageReceiverSource as IDamageReceiver;
        }

        public void Configure(IDamageReceiver receiver)
        {
            damageReceiver = receiver;
            damageReceiverSource = receiver as MonoBehaviour;
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
            }

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
                    damageReceiver.TakeDamage(new DamageRequest(definition.DamagePerTick, source, transform.position));
                }
            }

            burningRoutine = null;
        }
    }
}
