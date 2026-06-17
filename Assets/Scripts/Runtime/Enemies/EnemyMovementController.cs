using UnityEngine;
using VContainer;

namespace MagicalTower.Runtime
{
    public sealed class EnemyMovementController : MonoBehaviour
    {
        [SerializeField] private EnemyAgent agent;
        [SerializeField] private Transform target;
        [SerializeField] private Collider enemyCollider;
        [SerializeField] private Collider targetCollider;
        [SerializeField] private float contactTolerance = 0.05f;
        [SerializeField] private float missingColliderStoppingDistance = 0.35f;

        [Inject]
        public void Construct(PlayersTower targetTower)
        {
            target = targetTower != null ? targetTower.transform : target;
            targetCollider = targetTower != null ? targetTower.GetComponent<Collider>() : targetCollider;
        }

        private void Awake()
        {
            if (agent == null)
            {
                agent = GetComponent<EnemyAgent>();
            }

            ResolveEnemyCollider();
        }

        private void Update()
        {
            if (agent == null || agent.Definition == null || target == null || !agent.IsAlive)
            {
                return;
            }

            var offset = target.position - transform.position;
            offset.y = 0f;
            if (ShouldStop(offset))
            {
                return;
            }

            var direction = offset.normalized;
            transform.position += direction * agent.Definition.MovementSpeed * Time.deltaTime;
            transform.forward = direction;
        }

        private void ResolveEnemyCollider()
        {
            if (enemyCollider == null)
            {
                enemyCollider = GetComponent<Collider>();
            }
        }

        private bool ShouldStop(Vector3 horizontalOffset)
        {
            ResolveEnemyCollider();
            if (targetCollider == null && target != null)
            {
                targetCollider = target.GetComponent<Collider>();
            }

            if (enemyCollider == null || targetCollider == null)
            {
                return horizontalOffset.sqrMagnitude <= missingColliderStoppingDistance * missingColliderStoppingDistance;
            }

            if (Physics.ComputePenetration(
                enemyCollider,
                enemyCollider.transform.position,
                enemyCollider.transform.rotation,
                targetCollider,
                targetCollider.transform.position,
                targetCollider.transform.rotation,
                out _,
                out _))
            {
                return true;
            }

            var enemyPoint = enemyCollider.ClosestPoint(targetCollider.bounds.center);
            var targetPoint = targetCollider.ClosestPoint(enemyPoint);
            return (targetPoint - enemyPoint).sqrMagnitude <= contactTolerance * contactTolerance;
        }
    }
}
