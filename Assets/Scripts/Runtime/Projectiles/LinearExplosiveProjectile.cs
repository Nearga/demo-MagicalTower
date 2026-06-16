using MagicalTower.Content;
using UnityEngine;

namespace MagicalTower.Runtime
{
    public sealed class LinearExplosiveProjectile : MonoBehaviour
    {
        [SerializeField] private ProjectileDefinition definition;
        [SerializeField] private RuntimeMessageBus messageBus;
        [SerializeField] private FireNovaEffect explosionEffectPrefab;
        [SerializeField] private LayerMask damageMask = ~0;
        [SerializeField] private float lifetime = 5f;

        private Vector3 direction = Vector3.forward;
        private Transform vfxRoot;
        private float age;
        private bool exploded;

        public void Configure(
            ProjectileDefinition projectileDefinition,
            Vector3 flyDirection,
            RuntimeMessageBus bus,
            Transform effectsRoot)
        {
            definition = projectileDefinition;
            direction = flyDirection.sqrMagnitude > 0.001f ? flyDirection.normalized : transform.forward;
            messageBus = bus;
            vfxRoot = effectsRoot;
            age = 0f;
            exploded = false;
        }

        private void Update()
        {
            if (definition == null || exploded)
            {
                return;
            }

            age += Time.deltaTime;
            if (age >= lifetime)
            {
                Explode(transform.position);
                return;
            }

            transform.position += direction * definition.Speed * Time.deltaTime;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (IsTowerCollider(collision.collider))
            {
                return;
            }

            Explode(collision.GetContact(0).point);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (IsTowerCollider(other))
            {
                return;
            }

            Explode(other.ClosestPoint(transform.position));
        }

        private void Explode(Vector3 position)
        {
            if (definition == null || exploded)
            {
                return;
            }

            exploded = true;
            PlayExplosionEffect(position);

            if (definition.ImpactRadius > 0f)
            {
                DamageArea(position);
            }

            Destroy(gameObject);
        }

        private void PlayExplosionEffect(Vector3 position)
        {
            if (explosionEffectPrefab == null)
            {
                return;
            }

            var instance = Instantiate(explosionEffectPrefab, position, Quaternion.identity, vfxRoot);
            var radius = definition != null ? definition.ImpactRadius : 0f;
            instance.Play(radius);
        }

        private void DamageArea(Vector3 position)
        {
            var hits = Physics.OverlapSphere(position, definition.ImpactRadius, damageMask, QueryTriggerInteraction.Collide);
            for (var i = 0; i < hits.Length; i++)
            {
                var receiver = DamageReceiverLookup.FromCollider(hits[i]);
                if (receiver == null || !receiver.IsAlive)
                {
                    continue;
                }

                if (receiver is TowerHealth)
                {
                    continue;
                }

                var report = receiver.TakeDamage(new DamageRequest(
                    definition.Damage,
                    gameObject,
                    position,
                    definition.BurningStatusEffect));
                GameLog.Info(
                    LogChannel.Damage,
                    $"Fireball dealt {report.Amount} area damage to {receiver.GetType().Name}. Fatal: {report.WasFatal}.",
                    this);
            }
        }

        private static bool IsTowerCollider(Collider collider)
        {
            return collider != null && DamageReceiverLookup.FromCollider(collider) is TowerHealth;
        }
    }
}
