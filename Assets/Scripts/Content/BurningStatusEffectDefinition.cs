using UnityEngine;

namespace MagicalTower.Content
{
    public enum StatusStackPolicy
    {
        RefreshDuration
    }

    public sealed class BurningStatusEffectDefinition : ScriptableObject
    {
        [SerializeField] private int damagePerTick = 4;
        [SerializeField] private float duration = 3f;
        [SerializeField] private float tickInterval = 1f;
        [SerializeField] private StatusStackPolicy stackPolicy = StatusStackPolicy.RefreshDuration;

        public int DamagePerTick => damagePerTick;
        public float Duration => duration;
        public float TickInterval => tickInterval;
        public StatusStackPolicy StackPolicy => stackPolicy;
    }
}
