using UnityEngine;
using VContainer;

namespace MagicalTower.Runtime
{
    [RequireComponent(typeof(EnemyAgent))]
    public sealed class EnemyMovementController : MonoBehaviour
    {
        [SerializeField] private EnemyAgent agent;
        [SerializeField] private Transform target;
        [SerializeField] private Collider enemyCollider;
        [SerializeField] private Collider targetCollider;
        [SerializeField] private float contactTolerance = 0.05f;
        [SerializeField] private float missingColliderStoppingDistance = 0.35f;

        private float stopDistanceSqr;
        private bool hasStopDistance;

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

        private void OnEnable()
        {
            hasStopDistance = false;
        }

        private void Update()
        {
            if (agent == null || agent.Definition == null || target == null || !agent.IsAlive)
            {
                return;
            }

            if (ShouldStop())
            {
                return;
            }

            var offset = target.position - transform.position;
            offset.y = 0f;
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

        private void RefreshStopDistance()
        {
            ResolveEnemyCollider();
            if (targetCollider == null && target != null)
            {
                targetCollider = target.GetComponent<Collider>();
            }

            stopDistanceSqr = EnemyPlanarContact.ContactDistanceSqr(
                enemyCollider,
                targetCollider,
                contactTolerance,
                missingColliderStoppingDistance);
            hasStopDistance = true;
        }

        private bool ShouldStop()
        {
            if (!hasStopDistance)
            {
                RefreshStopDistance();
            }

            return EnemyPlanarContact.DistanceSqrXZ(transform, target) <= stopDistanceSqr;
        }
    }
}
