using System;
using System.Collections.Generic;
using UnityEngine;

namespace MagicalTower.Content
{
    public sealed class SpawnScheduleDefinition : ScriptableObject
    {
        [SerializeField] private SpawnTimeBand[] timeBands = Array.Empty<SpawnTimeBand>();

        public IReadOnlyList<SpawnTimeBand> TimeBands => timeBands;
    }

    [Serializable]
    public struct SpawnTimeBand
    {
        [SerializeField] private float startTime;
        [SerializeField] private float spawnInterval;
        [SerializeField] private WeightedEnemyEntry[] enemies;

        public float StartTime => startTime;
        public float SpawnInterval => spawnInterval;
        public IReadOnlyList<WeightedEnemyEntry> Enemies => enemies;
    }

    [Serializable]
    public struct WeightedEnemyEntry
    {
        [SerializeField] private EnemyDefinition enemyDefinition;
        [SerializeField] private int weight;

        public EnemyDefinition EnemyDefinition => enemyDefinition;
        public int Weight => weight;
    }
}
