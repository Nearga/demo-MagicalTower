using MagicalTower.Content;
using UnityEngine;

namespace MagicalTower.Runtime
{
    public sealed class LinearExplosiveProjectile : MonoBehaviour
    {
        [SerializeField] private ProjectileDefinition definition;
        [SerializeField] private FireNovaEffect explosionEffectPrefab;
        [SerializeField] private LayerMask damageMask = ~0;
        [SerializeField] private float lifetime = 5f;

        private Vector3 direction = Vector3.forward;
        private Transform vfxRoot;
        private GenericObjectPool explosionEffectPool;
        private float age;
        private bool exploded;

        public void Configure(
            ProjectileDefinition projectileDefinition,
            Vector3 flyDirection,
            Transform effectsRoot,
            GenericObjectPool effectPool = null)
        {
            definition = projectileDefinition;
            direction = flyDirection.sqrMagnitude > 0.001f ? flyDirection.normalized : transform.forward;
            vfxRoot = effectsRoot;
            explosionEffectPool = effectPool;
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

        private void OnDisable()
        {
            ClearRuntimeState();
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

            ReleaseOrDestroy();
        }

        private void PlayExplosionEffect(Vector3 position)
        {
            FireNovaEffect instance = null;
            if (explosionEffectPool != null)
            {
                instance = explosionEffectPool.Rent<FireNovaEffect>(position, Quaternion.identity);
            }
            else if (explosionEffectPrefab != null)
            {
                instance = Instantiate(explosionEffectPrefab, position, Quaternion.identity, vfxRoot);
            }

            if (instance == null)
            {
                return;
            }

            var radius = definition != null ? definition.ImpactRadius : 0f;
            instance.Play(radius);
        }

        private void ReleaseOrDestroy()
        {
            if (TryGetComponent<PooledObject>(out var pooledObject) && pooledObject.HasOwner)
            {
                pooledObject.Release();
                return;
            }

            Destroy(gameObject);
        }

        private void ClearRuntimeState()
        {
            definition = null;
            direction = Vector3.forward;
            vfxRoot = null;
            explosionEffectPool = null;
            age = 0f;
            exploded = false;
        }

        private static readonly Collider[] OverlapBuffer = new Collider[64];

        private void DamageArea(Vector3 position)
        {
            var count = Physics.OverlapSphereNonAlloc(position, definition.ImpactRadius, OverlapBuffer, damageMask, QueryTriggerInteraction.Collide);
            var hitLimit = Mathf.Min(count, OverlapBuffer.Length);
            for (var i = 0; i < hitLimit; i++)
            {
                var hit = OverlapBuffer[i];
                if (hit == null)
                {
                    continue;
                }

                var receiver = hit.GetComponentInParent<IDamageReceiver>();
                if (receiver == null || !receiver.IsAlive)
                {
                    continue;
                }

                if (receiver is PlayersTower)
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

            for (var i = 0; i < hitLimit; i++)
            {
                OverlapBuffer[i] = null;
            }
        }

        private static bool IsTowerCollider(Collider collider)
        {
            return collider != null && collider.GetComponentInParent<IDamageReceiver>() is PlayersTower;
        }
    }
}
