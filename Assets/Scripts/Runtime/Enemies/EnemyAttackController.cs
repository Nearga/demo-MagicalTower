using UnityEngine;

namespace MagicalTower.Runtime
{
    public sealed class EnemyAttackController : MonoBehaviour
    {
        [SerializeField] private EnemyAgent agent;
        [SerializeField] private TowerHealth target;
        [SerializeField] private float attackRange = 1.7f;

        private float cooldown;

        public void Configure(EnemyAgent enemyAgent, TowerHealth targetTower)
        {
            agent = enemyAgent;
            target = targetTower;
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
            if (agent == null || agent.Definition == null || target == null || !agent.IsAlive || !target.IsAlive)
            {
                return;
            }

            cooldown -= Time.deltaTime;
            if (cooldown > 0f)
            {
                return;
            }

            var offset = target.transform.position - transform.position;
            offset.y = 0f;
            if (offset.sqrMagnitude > attackRange * attackRange)
            {
                return;
            }

            target.TakeDamage(new DamageRequest(agent.Definition.ContactDamage, gameObject, target.transform.position));
            cooldown = agent.Definition.AttackInterval;
        }
    }
}
