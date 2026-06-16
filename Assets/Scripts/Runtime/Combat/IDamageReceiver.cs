using UnityEngine;

namespace MagicalTower.Runtime
{
    public interface IDamageReceiver
    {
        bool IsAlive { get; }
        Transform Transform { get; }
        DamageReport TakeDamage(DamageRequest request);
    }
}
