using System;
using UnityEngine;
using VContainer;

namespace MagicalTower.Runtime
{
    public sealed class GameSession : MonoBehaviour
    {
        public event Action GameOver;

        private RuntimeMessageBus messageBus;

        public float ElapsedTime { get; private set; }
        public bool IsGameOver { get; private set; }

        [Inject]
        public void Construct(RuntimeMessageBus bus)
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
