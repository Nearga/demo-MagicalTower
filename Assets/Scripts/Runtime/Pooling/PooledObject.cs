using UnityEngine;

namespace MagicalTower.Runtime
{
    public sealed class PooledObject : MonoBehaviour
    {
        private GenericObjectPool owner;

        public GenericObjectPool Owner => owner;
        public bool HasOwner => owner != null;

        public void SetOwner(GenericObjectPool pool)
        {
            owner = pool;
        }

        public void Release()
        {
            if (owner != null)
            {
                owner.Return(gameObject);
                return;
            }

            Destroy(gameObject);
        }
    }
}
