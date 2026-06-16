using UnityEngine;

namespace MagicalTower.Content
{
    public enum SpellTargetMode
    {
        RandomEnemyDirection,
        VisibleEnemies
    }

    public sealed class SpellDefinition : ScriptableObject
    {
        [SerializeField] private string displayName = "Spell";
        [SerializeField] private float cooldown = 1f;
        [SerializeField] private ProjectileDefinition projectileDefinition;
        [SerializeField] private SpellTargetMode targetMode = SpellTargetMode.RandomEnemyDirection;

        public string DisplayName => displayName;
        public float Cooldown => cooldown;
        public ProjectileDefinition ProjectileDefinition => projectileDefinition;
        public SpellTargetMode TargetMode => targetMode;
    }
}
