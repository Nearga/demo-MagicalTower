using System;
using MagicalTower.Runtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MagicalTower.UI
{
    public sealed class HudPresenter : MonoBehaviour
    {
        [Header("Health")]
        [SerializeField] private Slider healthSlider;
        [SerializeField] private TMP_Text healthLabel;

        [Header("Time")]
        [SerializeField] private TMP_Text elapsedTimeLabel;

        private GameSession gameSession;
        private IDisposable healthSubscription;

        public void Configure(GameSession session, RuntimeMessageBus messageBus)
        {
            gameSession = session;
            healthSubscription = messageBus.Subscribe<TowerHealthChangedMessage>(OnTowerHealthChanged);
        }

        private void OnDestroy()
        {
            healthSubscription?.Dispose();
            healthSubscription = null;
        }

        private void Update()
        {
            if (gameSession == null || elapsedTimeLabel == null || gameSession.IsGameOver)
            {
                return;
            }

            elapsedTimeLabel.text = FormatTime(gameSession.ElapsedTime);
        }

        private void OnTowerHealthChanged(TowerHealthChangedMessage message)
        {
            if (healthSlider != null && message.MaxHealth > 0)
            {
                healthSlider.value = (float)message.CurrentHealth / message.MaxHealth;
            }

            if (healthLabel != null)
            {
                healthLabel.text = $"{message.CurrentHealth} / {message.MaxHealth}";
            }
        }

        private static string FormatTime(float seconds)
        {
            var minutes = Mathf.FloorToInt(seconds / 60f);
            var secs    = Mathf.FloorToInt(seconds % 60f);
            return $"{minutes:00}:{secs:00}";
        }
    }
}
