using System;
using System.Collections.Generic;
using MagicalTower.Content;
using UnityEngine;

namespace MagicalTower.Runtime
{
    public sealed class TowerSpellScheduler : MonoBehaviour
    {
        [SerializeField] private SpellCastBinding[] spells = Array.Empty<SpellCastBinding>();
        [SerializeField] private ActiveEnemyRegistry enemyRegistry;
        [SerializeField] private Transform projectileRoot;
        [SerializeField] private Transform vfxRoot;
        [SerializeField] private RuntimeMessageBus messageBus;
        [SerializeField] private Camera viewCamera;

        private readonly List<EnemyAgent> visibleEnemies = new List<EnemyAgent>();
        private float[] cooldowns = Array.Empty<float>();

        public void Configure(
            SpellCastBinding[] spellBindings,
            ActiveEnemyRegistry registry,
            Transform root,
            Transform effectsRoot,
            RuntimeMessageBus bus,
            Camera camera)
        {
            spells = spellBindings ?? Array.Empty<SpellCastBinding>();
            enemyRegistry = registry;
            projectileRoot = root;
            vfxRoot = effectsRoot;
            messageBus = bus;
            viewCamera = camera;
            EnsureCooldowns();
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
                if (spell.Definition == null || spell.ProjectilePrefab == null)
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

            var instance = Instantiate(spell.ProjectilePrefab, start, Quaternion.LookRotation(direction.normalized), projectileRoot);
            if (instance.TryGetComponent<LinearExplosiveProjectile>(out var projectile))
            {
                projectile.Configure(spell.Definition.ProjectileDefinition, direction.normalized, messageBus, vfxRoot);
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
                var instance = Instantiate(spell.ProjectilePrefab, start, rotation, projectileRoot);

                if (instance.TryGetComponent<ArcTargetProjectile>(out var projectile))
                {
                    projectile.Configure(spell.Definition.ProjectileDefinition, enemy, messageBus);
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

        public SpellDefinition Definition => definition;
        public GameObject ProjectilePrefab => projectilePrefab;
    }
}
