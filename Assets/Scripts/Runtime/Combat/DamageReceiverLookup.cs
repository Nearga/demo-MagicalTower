using UnityEngine;

namespace MagicalTower.Runtime
{
    internal static class DamageReceiverLookup
    {
        public static IDamageReceiver FromCollider(Collider hit)
        {
            if (hit == null)
            {
                return null;
            }

            var behaviours = hit.GetComponentsInParent<MonoBehaviour>();
            for (var i = 0; i < behaviours.Length; i++)
            {
                if (behaviours[i] is IDamageReceiver receiver)
                {
                    return receiver;
                }
            }

            return null;
        }
    }
}
