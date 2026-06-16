using UnityEngine;

namespace MagicalTower.Content
{
    public sealed class TowerDefinition : ScriptableObject
    {
        [SerializeField] private int maxHealth = 100;

        public int MaxHealth => maxHealth;
    }
}
