using System.Collections.Generic;
using UnityEngine;

namespace MagicalTower.Runtime
{
    public sealed class ActiveEnemyRegistry : MonoBehaviour
    {
        private readonly List<EnemyAgent> enemies = new List<EnemyAgent>();

        public IReadOnlyList<EnemyAgent> Enemies => enemies;

        public void Register(EnemyAgent enemy)
        {
            if (enemy == null || enemies.Contains(enemy))
            {
                return;
            }

            enemies.Add(enemy);
        }

        public void Unregister(EnemyAgent enemy)
        {
            enemies.Remove(enemy);
        }

        public bool TryGetRandomEnemy(out EnemyAgent enemy)
        {
            RemoveInactive();
            if (enemies.Count == 0)
            {
                enemy = null;
                return false;
            }

            enemy = enemies[Random.Range(0, enemies.Count)];
            return true;
        }

        public List<EnemyAgent> GetVisibleEnemies(Camera viewCamera, List<EnemyAgent> results)
        {
            results.Clear();
            RemoveInactive();

            if (viewCamera == null)
            {
                results.AddRange(enemies);
                return results;
            }

            for (var i = 0; i < enemies.Count; i++)
            {
                var viewport = viewCamera.WorldToViewportPoint(enemies[i].transform.position);
                if (viewport.z > 0f && viewport.x >= 0f && viewport.x <= 1f && viewport.y >= 0f && viewport.y <= 1f)
                {
                    results.Add(enemies[i]);
                }
            }

            return results;
        }

        private void RemoveInactive()
        {
            for (var i = enemies.Count - 1; i >= 0; i--)
            {
                if (enemies[i] == null || !enemies[i].IsAlive)
                {
                    enemies.RemoveAt(i);
                }
            }
        }
    }
}
