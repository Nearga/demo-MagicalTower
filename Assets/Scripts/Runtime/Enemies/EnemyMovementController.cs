using UnityEngine;

namespace MagicalTower.Runtime
{
    public sealed class EnemyMovementController : MonoBehaviour
    {
        [SerializeField] private EnemyAgent agent;
        [SerializeField] private Transform target;
        [SerializeField] private float stoppingDistance = 1.5f;

        public void Configure(EnemyAgent enemyAgent, Transform targetTransform)
        {
            agent = enemyAgent;
            target = targetTransform;
        }

        private void Awake()
        {
            if (agent == null)
            {
                agent = GetComponent<EnemyAgent>();
            }
        }

        private void Update()
        {
            if (agent == null || agent.Definition == null || target == null || !agent.IsAlive)
            {
                return;
            }

            var offset = target.position - transform.position;
            offset.y = 0f;
            if (offset.sqrMagnitude <= stoppingDistance * stoppingDistance)
            {
                return;
            }

            var direction = offset.normalized;
            transform.position += direction * agent.Definition.MovementSpeed * Time.deltaTime;
            transform.forward = direction;
        }
    }
}
