using UnityEngine;
using VContainer;

namespace MagicalTower.Runtime
{
    public sealed class EnemyAttackController : MonoBehaviour
    {
        [SerializeField] private EnemyAgent agent;
        [SerializeField] private PlayersTower target;
        [SerializeField] private Collider enemyCollider;
        [SerializeField] private Collider towerCollider;
        [SerializeField] private float contactTolerance = 0.05f;
        [SerializeField] private float missingColliderFallbackRange = 0.35f;

        private float cooldown;

        [Inject]
        public void Construct(PlayersTower targetTower)
        {
            target = targetTower;
            towerCollider = target != null ? target.GetComponent<Collider>() : null;
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
            if (agent == null || agent.Definition == null || target == null || !agent.IsAlive || !target.IsAlive)
            {
                return;
            }

            cooldown -= Time.deltaTime;
            if (cooldown > 0f)
            {
                return;
            }

            if (!IsTouchingTower())
            {
                return;
            }

            target.TakeDamage(new DamageRequest(agent.Definition.ContactDamage, gameObject, target.transform.position));
            GameLog.Info(
                LogChannel.Damage,
                $"{agent.Definition.DisplayName} dealt {agent.Definition.ContactDamage} contact damage to tower. Tower HP: {target.CurrentHealth}/{target.MaxHealth}.",
                this);
            cooldown = agent.Definition.AttackInterval;
        }

        private void ResolveEnemyCollider()
        {
            if (enemyCollider == null)
            {
                enemyCollider = GetComponent<Collider>();
            }
        }

        private bool IsTouchingTower()
        {
            ResolveEnemyCollider();
            if (towerCollider == null && target != null)
            {
                towerCollider = target.GetComponent<Collider>();
            }

            if (enemyCollider == null || towerCollider == null)
            {
                var offset = target.transform.position - transform.position;
                offset.y = 0f;
                return offset.sqrMagnitude <= missingColliderFallbackRange * missingColliderFallbackRange;
            }

            if (Physics.ComputePenetration(
                enemyCollider,
                enemyCollider.transform.position,
                enemyCollider.transform.rotation,
                towerCollider,
                towerCollider.transform.position,
                towerCollider.transform.rotation,
                out _,
                out _))
            {
                return true;
            }

            var enemyPoint = enemyCollider.ClosestPoint(towerCollider.bounds.center);
            var towerPoint = towerCollider.ClosestPoint(enemyPoint);
            return (towerPoint - enemyPoint).sqrMagnitude <= contactTolerance * contactTolerance;
        }
    }
}
