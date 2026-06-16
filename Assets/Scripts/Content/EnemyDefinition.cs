using UnityEngine;

namespace MagicalTower.Content
{
    public sealed class EnemyDefinition : ScriptableObject
    {
        [SerializeField] private string displayName = "Enemy";
        [SerializeField] private int maxHealth = 30;
        [SerializeField] private float movementSpeed = 2f;
        [SerializeField] private int contactDamage = 5;
        [SerializeField] private float attackInterval = 1f;
        [SerializeField] private float visualScale = 1f;

        public string DisplayName => displayName;
        public int MaxHealth => maxHealth;
        public float MovementSpeed => movementSpeed;
        public int ContactDamage => contactDamage;
        public float AttackInterval => attackInterval;
        public float VisualScale => visualScale;
    }
}
