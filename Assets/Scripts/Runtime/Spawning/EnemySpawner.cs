using MagicalTower.Content;
using UnityEngine;

namespace MagicalTower.Runtime
{
    public sealed class EnemySpawner : MonoBehaviour
    {
        [SerializeField] private SpawnScheduleDefinition schedule;
        [SerializeField] private EnemyPool enemyPool;
        [SerializeField] private GameSession gameSession;
        [SerializeField] private Transform spawnRoot;
        [SerializeField] private TowerHealth targetTower;
        [SerializeField] private Camera viewCamera;
        [SerializeField] private float fallbackSpawnRadius = 12f;

        private float spawnTimer;
        private bool warnedMissingReferences;

        public void Configure(
            SpawnScheduleDefinition spawnSchedule,
            EnemyPool pool,
            GameSession session,
            Transform root,
            TowerHealth tower,
            Camera camera)
        {
            schedule = spawnSchedule;
            enemyPool = pool;
            gameSession = session;
            spawnRoot = root;
            targetTower = tower;
            viewCamera = camera;
        }

        private void Update()
        {
            if (!CanSpawn())
            {
                return;
            }

            var band = GetCurrentBand(gameSession.ElapsedTime);
            if (band == null)
            {
                return;
            }

            spawnTimer -= Time.deltaTime;
            if (spawnTimer > 0f)
            {
                return;
            }

            spawnTimer = Mathf.Max(0.1f, band.Value.SpawnInterval);
            var definition = PickEnemy(band.Value);
            if (definition == null)
            {
                return;
            }

            var position = GetSpawnPosition();
            var direction = targetTower != null ? targetTower.transform.position - position : Vector3.forward;
            direction.y = 0f;
            var rotation = direction.sqrMagnitude > 0.001f ? Quaternion.LookRotation(direction.normalized) : Quaternion.identity;
            enemyPool.Spawn(definition, position, rotation);
            GameLog.Info(
                LogChannel.Spawning,
                $"Spawned {definition.DisplayName} at {position} after {gameSession.ElapsedTime:0.0}s.",
                this);
        }

        private bool CanSpawn()
        {
            var hasRequiredReferences =
                schedule != null &&
                enemyPool != null &&
                gameSession != null &&
                spawnRoot != null &&
                targetTower != null &&
                viewCamera != null;

            if (!hasRequiredReferences && !warnedMissingReferences)
            {
                Debug.LogWarning("EnemySpawner is missing required references.", this);
                warnedMissingReferences = true;
            }

            return hasRequiredReferences && !gameSession.IsGameOver;
        }

        private SpawnTimeBand? GetCurrentBand(float elapsedTime)
        {
            SpawnTimeBand? current = null;
            foreach (var band in schedule.TimeBands)
            {
                if (band.StartTime <= elapsedTime)
                {
                    current = band;
                }
            }

            return current;
        }

        private EnemyDefinition PickEnemy(SpawnTimeBand band)
        {
            var totalWeight = 0;
            foreach (var entry in band.Enemies)
            {
                if (entry.EnemyDefinition != null && entry.Weight > 0)
                {
                    totalWeight += entry.Weight;
                }
            }

            if (totalWeight <= 0)
            {
                return null;
            }

            var roll = Random.Range(0, totalWeight);
            foreach (var entry in band.Enemies)
            {
                if (entry.EnemyDefinition == null || entry.Weight <= 0)
                {
                    continue;
                }

                if (roll < entry.Weight)
                {
                    return entry.EnemyDefinition;
                }

                roll -= entry.Weight;
            }

            return null;
        }

        private Vector3 GetSpawnPosition()
        {
            var center = targetTower != null ? targetTower.transform.position : transform.position;
            var angle = Random.Range(0f, Mathf.PI * 2f);
            var radius = fallbackSpawnRadius;

            if (viewCamera != null && viewCamera.orthographic)
            {
                var halfHeight = viewCamera.orthographicSize;
                var halfWidth = halfHeight * viewCamera.aspect;
                radius = Mathf.Max(halfWidth, halfHeight) + 2f;
            }

            var offset = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)) * radius;
            var position = center + offset;

            if (spawnRoot != null)
            {
                position.y = spawnRoot.position.y;
            }

            return position;
        }
    }
}
