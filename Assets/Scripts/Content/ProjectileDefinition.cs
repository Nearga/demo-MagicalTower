using UnityEngine;

namespace MagicalTower.Content
{
    public sealed class ProjectileDefinition : ScriptableObject
    {
        [SerializeField] private int damage = 10;
        [SerializeField] private float speed = 7f;
        [SerializeField] private float impactRadius;
        [SerializeField] private float arcHeight;
        [SerializeField] private BurningStatusEffectDefinition burningStatusEffect;

        public int Damage => damage;
        public float Speed => speed;
        public float ImpactRadius => impactRadius;
        public float ArcHeight => arcHeight;
        public BurningStatusEffectDefinition BurningStatusEffect => burningStatusEffect;
    }
}
