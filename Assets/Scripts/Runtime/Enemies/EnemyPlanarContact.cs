using UnityEngine;

namespace MagicalTower.Runtime
{
    internal static class EnemyPlanarContact
    {
        public static float DistanceSqrXZ(Transform first, Transform second)
        {
            var delta = first.position - second.position;
            delta.y = 0f;
            return delta.sqrMagnitude;
        }

        public static float ContactDistanceSqr(Collider first, Collider second, float tolerance, float fallbackDistance)
        {
            if (first == null || second == null)
            {
                return Squared(fallbackDistance);
            }

            var distance = HorizontalRadius(first) + HorizontalRadius(second) + Mathf.Max(0f, tolerance);
            return Squared(distance);
        }

        private static float HorizontalRadius(Collider collider)
        {
            var extents = collider.bounds.extents;
            return Mathf.Max(extents.x, extents.z);
        }

        private static float Squared(float value)
        {
            value = Mathf.Max(0f, value);
            return value * value;
        }
    }
}
