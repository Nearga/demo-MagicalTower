using MagicalTower.Content;
using UnityEngine;

namespace MagicalTower.Runtime
{
    public readonly struct DamageRequest
    {
        public DamageRequest(
            int amount,
            GameObject source = null,
            Vector3? worldPosition = null,
            BurningStatusEffectDefinition burningStatusEffect = null)
        {
            Amount = amount;
            Source = source;
            WorldPosition = worldPosition;
            BurningStatusEffect = burningStatusEffect;
        }

        public int Amount { get; }
        public GameObject Source { get; }
        public Vector3? WorldPosition { get; }
        public BurningStatusEffectDefinition BurningStatusEffect { get; }
    }
}
