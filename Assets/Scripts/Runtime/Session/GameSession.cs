using System;
using UnityEngine;

namespace MagicalTower.Runtime
{
    public sealed class GameSession : MonoBehaviour
    {
        [SerializeField] private RuntimeMessageBus messageBus;

        public event Action GameOver;

        public float ElapsedTime { get; private set; }
        public bool IsGameOver { get; private set; }

        public void Configure(RuntimeMessageBus bus)
        {
            messageBus = bus;
        }

        private void Update()
        {
            if (IsGameOver)
            {
                return;
            }

            ElapsedTime += Time.deltaTime;
        }

        public void EndGame(TowerHealth tower)
        {
            if (IsGameOver)
            {
                return;
            }

            IsGameOver = true;
            messageBus?.Publish(new TowerDestroyedMessage(tower));
            GameLog.Info(LogChannel.Session, $"Game over at {ElapsedTime:0.0}s.", this);
            GameOver?.Invoke();
        }
    }
}
