using UnityEngine;

namespace MagicalTower.Content
{
    public sealed class EnemyPoolConfig : ScriptableObject
    {
        [SerializeField] private GameObject enemyPrefab;
        [SerializeField] private int initialCapacity = 12;
        [SerializeField] private int maxCapacity = 60;

        public GameObject EnemyPrefab => enemyPrefab;
        public int InitialCapacity => initialCapacity;
        public int MaxCapacity => maxCapacity;
    }
}
