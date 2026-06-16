using MagicalTower.Content;
using UnityEngine;

namespace MagicalTower.Runtime
{
    public sealed class LinearExplosiveProjectile : MonoBehaviour
    {
        [SerializeField] private ProjectileDefinition definition;
        [SerializeField] private RuntimeMessageBus messageBus;
        [SerializeField] private LayerMask damageMask = ~0;
        [SerializeField] private float lifetime = 5f;

        private Vector3 direction = Vector3.forward;
        private float age;
        private bool exploded;

        public void Configure(ProjectileDefinition projectileDefinition, Vector3 flyDirection, RuntimeMessageBus bus)
        {
            definition = projectileDefinition;
            direction = flyDirection.sqrMagnitude > 0.001f ? flyDirection.normalized : transform.forward;
            messageBus = bus;
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
            Explode(collision.GetContact(0).point);
        }

        private void OnTriggerEnter(Collider other)
        {
            Explode(other.ClosestPoint(transform.position));
        }

        private void Explode(Vector3 position)
        {
            if (definition == null || exploded)
            {
                return;
            }

            exploded = true;
            if (definition.ImpactRadius > 0f)
            {
                DamageArea(position);
            }

            Destroy(gameObject);
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

                receiver.TakeDamage(new DamageRequest(
                    definition.Damage,
                    gameObject,
                    position,
                    definition.BurningStatusEffect));
            }
        }
    }
}
