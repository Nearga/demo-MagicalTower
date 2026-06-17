using System;
using MagicalTower.Runtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

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
        private int lastUpdatedSeconds = -1;

        [Inject]
        public void Construct(GameSession session, RuntimeMessageBus messageBus)
        {
            gameSession = session;
            healthSubscription = messageBus.Subscribe<PlayersTowerChangedMessage>(OnPlayersTowerChanged);
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

            var currentSeconds = Mathf.FloorToInt(gameSession.ElapsedTime);
            if (currentSeconds != lastUpdatedSeconds)
            {
                lastUpdatedSeconds = currentSeconds;
                elapsedTimeLabel.text = FormatTime(gameSession.ElapsedTime);
            }
        }

        private void OnPlayersTowerChanged(PlayersTowerChangedMessage message)
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
