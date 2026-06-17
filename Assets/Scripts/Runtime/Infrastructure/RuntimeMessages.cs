using UnityEngine;

namespace MagicalTower.Runtime
{
    public readonly struct DamageDealtMessage
    {
        public DamageDealtMessage(DamageReport report)
        {
            Report = report;
        }

        public DamageReport Report { get; }
    }

    public readonly struct PlayersTowerChangedMessage
    {
        public PlayersTowerChangedMessage(PlayersTower tower, int currentHealth, int maxHealth)
        {
            Tower = tower;
            CurrentHealth = currentHealth;
            MaxHealth = maxHealth;
        }

        public PlayersTower Tower { get; }
        public int CurrentHealth { get; }
        public int MaxHealth { get; }
    }

    public readonly struct TowerDestroyedMessage
    {
        public TowerDestroyedMessage(PlayersTower tower)
        {
            Tower = tower;
        }

        public PlayersTower Tower { get; }
    }

    public readonly struct EnemySpawnedMessage
    {
        public EnemySpawnedMessage(EnemyAgent enemy)
        {
            Enemy = enemy;
        }

        public EnemyAgent Enemy { get; }
    }

    public readonly struct EnemyDefeatedMessage
    {
        public EnemyDefeatedMessage(EnemyAgent enemy, GameObject source)
        {
            Enemy = enemy;
            Source = source;
        }

        public EnemyAgent Enemy { get; }
        public GameObject Source { get; }
    }

    public readonly struct BurningTickMessage
    {
        public BurningTickMessage(EnemyAgent target, int amount, Vector3 worldPosition)
        {
            Target        = target;
            Amount        = amount;
            WorldPosition = worldPosition;
        }

        public EnemyAgent Target        { get; }
        public int        Amount        { get; }
        public Vector3    WorldPosition { get; }
    }
}
