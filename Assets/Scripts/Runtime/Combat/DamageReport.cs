using UnityEngine;

namespace MagicalTower.Runtime
{
    public readonly struct DamageReport
    {
        public DamageReport(IDamageReceiver target, int amount, Vector3 worldPosition, bool wasFatal)
        {
            Target = target;
            Amount = amount;
            WorldPosition = worldPosition;
            WasFatal = wasFatal;
        }

        public IDamageReceiver Target { get; }
        public int Amount { get; }
        public Vector3 WorldPosition { get; }
        public bool WasFatal { get; }
    }
}
