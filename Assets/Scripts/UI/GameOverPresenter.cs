using System;
using MagicalTower.Runtime;
using UnityEngine;

namespace MagicalTower.UI
{
    public sealed class GameOverPresenter : MonoBehaviour
    {
        [SerializeField] private GameObject panel;

        private IDisposable destroyedSubscription;

        public void Configure(RuntimeMessageBus messageBus)
        {
            destroyedSubscription = messageBus.Subscribe<TowerDestroyedMessage>(OnTowerDestroyed);
        }

        private void OnDestroy()
        {
            destroyedSubscription?.Dispose();
            destroyedSubscription = null;
        }

        private void OnTowerDestroyed(TowerDestroyedMessage message)
        {
            if (panel != null)
            {
                panel.SetActive(true);
            }
        }
    }
}
