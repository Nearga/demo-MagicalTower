using MagicalTower.Runtime;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace MagicalTower.UI
{
    [RequireComponent(typeof(Button))]
    public sealed class TimerGameOverButton : MonoBehaviour
    {
        [SerializeField] private Button button;

        private GameSession gameSession;
        private PlayersTower playersTower;

        [Inject]
        public void Construct(GameSession session, PlayersTower tower)
        {
            gameSession = session;
            playersTower = tower;
        }

        private void Awake()
        {
            if (button == null)
            {
                button = GetComponent<Button>();
            }
        }

        private void OnEnable()
        {
            if (button != null)
            {
                button.onClick.AddListener(LoseRound);
            }
        }

        private void OnDisable()
        {
            if (button != null)
            {
                button.onClick.RemoveListener(LoseRound);
            }
        }

        private void LoseRound()
        {
            if (gameSession == null || playersTower == null || gameSession.IsGameOver)
            {
                return;
            }

            gameSession.EndGame(playersTower);
        }
    }
}
