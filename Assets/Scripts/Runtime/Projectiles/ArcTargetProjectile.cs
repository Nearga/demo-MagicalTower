using MagicalTower.Content;
using UnityEngine;

namespace MagicalTower.Runtime
{
    public sealed class ArcTargetProjectile : MonoBehaviour
    {
        [SerializeField] private ProjectileDefinition definition;
        [SerializeField] private EnemyAgent target;
        [SerializeField] private RuntimeMessageBus messageBus;

        private Vector3 startPosition;
        private float travelTime;
        private float elapsed;
        private bool hit;

        public void Configure(ProjectileDefinition projectileDefinition, EnemyAgent targetEnemy, RuntimeMessageBus bus)
        {
            definition = projectileDefinition;
            target = targetEnemy;
            messageBus = bus;
            startPosition = transform.position;
            elapsed = 0f;
            hit = false;

            var distance = target != null ? Vector3.Distance(startPosition, target.transform.position) : 0f;
            var speed = definition != null ? Mathf.Max(0.1f, definition.Speed) : 1f;
            travelTime = Mathf.Max(0.1f, distance / speed);
        }

        private void Update()
        {
            if (definition == null || target == null || hit)
            {
                Destroy(gameObject);
                return;
            }

            if (!target.IsAlive)
            {
                Destroy(gameObject);
                return;
            }

            elapsed += Time.deltaTime;
            var t = Mathf.Clamp01(elapsed / travelTime);
            var endPosition = target.transform.position;
            var arcOffset = Vector3.up * (Mathf.Sin(t * Mathf.PI) * definition.ArcHeight);
            transform.position = Vector3.Lerp(startPosition, endPosition, t) + arcOffset;

            if (t >= 1f)
            {
                HitTarget();
            }
        }

        private void HitTarget()
        {
            if (hit || target == null)
            {
                return;
            }

            hit = true;
            var report = target.TakeDamage(new DamageRequest(definition.Damage, gameObject, target.transform.position));
            GameLog.Info(
                LogChannel.Damage,
                $"Barrage dealt {report.Amount} damage to {target.Definition.DisplayName}. Fatal: {report.WasFatal}.",
                this);
            Destroy(gameObject);
        }
    }
}
