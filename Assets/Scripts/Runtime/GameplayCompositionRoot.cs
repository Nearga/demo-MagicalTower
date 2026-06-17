using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace MagicalTower.Runtime
{
    public sealed class GameplayCompositionRoot : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponentInHierarchy<RuntimeMessageBus>();
            
            builder.RegisterComponentInHierarchy<GameSession>();
            builder.RegisterComponentInHierarchy<PlayersTower>();
            builder.RegisterComponentInHierarchy<ActiveEnemyRegistry>();
            builder.RegisterComponentInHierarchy<EnemyPool>();
            builder.RegisterComponentInHierarchy<EnemySpawner>();
            builder.RegisterComponentInHierarchy<TowerSpellScheduler>();
            builder.RegisterComponentInHierarchy<Camera>();
            builder.RegisterComponentInHierarchy<MagicalTower.UI.HudPresenter>();
            builder.RegisterComponentInHierarchy<MagicalTower.UI.GameOverPresenter>();
            builder.RegisterComponentInHierarchy<MagicalTower.UI.DamageNumberSpawner>();
            builder.RegisterComponentInHierarchy<MagicalTower.UI.TimerGameOverButton>();

            builder.RegisterBuildCallback(container =>
            {
                var pools = FindObjectsByType<GenericObjectPool>(FindObjectsInactive.Include);
                for (var i = 0; i < pools.Length; i++)
                {
                    container.Inject(pools[i]);
                }
            });
        }
    }
}
