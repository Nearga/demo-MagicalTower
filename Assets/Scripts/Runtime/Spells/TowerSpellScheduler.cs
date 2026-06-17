using System;
using System.Collections.Generic;
using MagicalTower.Content;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace MagicalTower.Runtime
{
    public sealed class TowerSpellScheduler : MonoBehaviour
    {
        [SerializeField] private SpellCastBinding[] spells = Array.Empty<SpellCastBinding>();
        [SerializeField] private Transform projectileRoot;
        [SerializeField] private Transform vfxRoot;
        [SerializeField] private Camera viewCamera;

        private readonly List<EnemyAgent> visibleEnemies = new List<EnemyAgent>();
        private float[] cooldowns = Array.Empty<float>();
        private IObjectResolver objectResolver;
        private ActiveEnemyRegistry enemyRegistry;

        [Inject]
        public void Construct(IObjectResolver resolver, ActiveEnemyRegistry registry)
        {
            objectResolver = resolver;
            enemyRegistry = registry;
        }

        private void Awake()
        {
            EnsureCooldowns();
        }

        private void Update()
        {
            if (enemyRegistry == null)
            {
                return;
            }

            EnsureCooldowns();
            for (var i = 0; i < spells.Length; i++)
            {
                cooldowns[i] -= Time.deltaTime;
                if (cooldowns[i] > 0f)
                {
                    continue;
                }

                var spell = spells[i];
                if (spell.Definition == null || !spell.HasProjectileSource)
                {
                    continue;
                }

                Cast(spell);
                cooldowns[i] = spell.Definition.Cooldown;
            }
        }

        private void Cast(SpellCastBinding spell)
        {
            switch (spell.Definition.TargetMode)
            {
                case SpellTargetMode.RandomEnemyDirection:
                    CastRandomDirection(spell);
                    break;
                case SpellTargetMode.VisibleEnemies:
                    CastAtVisibleEnemies(spell);
                    break;
            }
        }

        private void CastRandomDirection(SpellCastBinding spell)
        {
            if (!enemyRegistry.TryGetRandomEnemy(out var enemy))
            {
                return;
            }

            var start = transform.position;
            var direction = enemy.transform.position - start;
            if (direction.sqrMagnitude <= 0.001f)
            {
                direction = transform.forward;
            }

            var rotation = Quaternion.LookRotation(direction.normalized);
            var instance = spell.RentProjectile(objectResolver, start, rotation, projectileRoot);
            if (instance == null)
            {
                return;
            }

            if (instance.TryGetComponent<LinearExplosiveProjectile>(out var projectile))
            {
                projectile.Configure(spell.Definition.ProjectileDefinition, direction.normalized, vfxRoot, spell.ImpactEffectPool);
            }
        }

        private void CastAtVisibleEnemies(SpellCastBinding spell)
        {
            enemyRegistry.GetVisibleEnemies(viewCamera, visibleEnemies);
            for (var i = 0; i < visibleEnemies.Count; i++)
            {
                var enemy = visibleEnemies[i];
                if (enemy == null || !enemy.IsAlive)
                {
                    continue;
                }

                var start = transform.position;
                var direction = enemy.transform.position - start;
                var rotation = direction.sqrMagnitude > 0.001f ? Quaternion.LookRotation(direction.normalized) : Quaternion.identity;
                var instance = spell.RentProjectile(objectResolver, start, rotation, projectileRoot);
                if (instance == null)
                {
                    continue;
                }

                if (instance.TryGetComponent<ArcTargetProjectile>(out var projectile))
                {
                    projectile.Configure(spell.Definition.ProjectileDefinition, enemy);
                }
            }
        }

        private void EnsureCooldowns()
        {
            if (cooldowns.Length == spells.Length)
            {
                return;
            }

            cooldowns = new float[spells.Length];
        }
    }

    [Serializable]
    public struct SpellCastBinding
    {
        [SerializeField] private SpellDefinition definition;
        [SerializeField] private GameObject projectilePrefab;
        [SerializeField] private GenericObjectPool projectilePool;
        [SerializeField] private GenericObjectPool impactEffectPool;

        public SpellDefinition Definition => definition;
        public GameObject ProjectilePrefab => projectilePrefab;
        public GenericObjectPool ImpactEffectPool => impactEffectPool;
        public bool HasProjectileSource => projectilePool != null || projectilePrefab != null;

        public GameObject RentProjectile(
            IObjectResolver objectResolver,
            Vector3 position,
            Quaternion rotation,
            Transform fallbackParent)
        {
            if (projectilePool != null)
            {
                return projectilePool.Rent(position, rotation);
            }

            return objectResolver != null
                ? objectResolver.Instantiate(projectilePrefab, position, rotation, fallbackParent)
                : UnityEngine.Object.Instantiate(projectilePrefab, position, rotation, fallbackParent);
        }
    }
}
