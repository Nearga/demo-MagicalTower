using System;
using MagicalTower.Runtime;
using UnityEngine;
using VContainer;

namespace MagicalTower.UI
{
    public sealed class DamageNumberSpawner : MonoBehaviour
    {
        [Header("Prefab")]
        [SerializeField] private DamageNumber numberPrefab;

        [Header("Style — Enemy Damage")]
        [SerializeField] private Color enemyDamageColor  = new Color(1f, 0.85f, 0f);   // warm yellow

        [Header("Style — Tower Damage")]
        [SerializeField] private Color towerDamageColor  = new Color(1f, 0.2f, 0.1f);  // red

        [Header("Style — Burning Tick")]
        [SerializeField] private Color burningTickColor  = new Color(1f, 0.45f, 0f);   // orange

        [Header("Motion")]
        [SerializeField] private float lifetime          = 0.9f;
        [SerializeField] private float riseSpeed         = 60f;
        [SerializeField] private float fadeStartFraction = 0.5f;

        private Camera viewCamera;
        private RectTransform canvasRect;
        private IDisposable damageSubscription;
        private IDisposable burningSubscription;

        [Inject]
        public void Construct(Camera camera, RuntimeMessageBus messageBus)
        {
            viewCamera          = camera;
            canvasRect          = GetComponent<RectTransform>();
            damageSubscription  = messageBus.Subscribe<DamageDealtMessage>(OnDamageDealt);
            burningSubscription = messageBus.Subscribe<BurningTickMessage>(OnBurningTick);
        }

        private void OnDestroy()
        {
            damageSubscription?.Dispose();
            burningSubscription?.Dispose();
            damageSubscription  = null;
            burningSubscription = null;
        }

        private void OnDamageDealt(DamageDealtMessage message)
        {
            var report = message.Report;
            if (report.Amount <= 0)
            {
                return;
            }

            var color = report.Target is TowerHealth ? towerDamageColor : enemyDamageColor;
            SpawnAt(report.Amount, color, report.WorldPosition);
        }

        private void OnBurningTick(BurningTickMessage message)
        {
            if (message.Amount <= 0)
            {
                return;
            }

            SpawnAt(message.Amount, burningTickColor, message.WorldPosition);
        }

        private void SpawnAt(int amount, Color color, Vector3 worldPosition)
        {
            if (numberPrefab == null || viewCamera == null)
            {
                return;
            }

            var screenPoint = viewCamera.WorldToScreenPoint(worldPosition);

            // Discard numbers behind the camera
            if (screenPoint.z < 0f)
            {
                return;
            }

            var instance = Instantiate(numberPrefab, transform);

            // Convert screen point to canvas local position
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    canvasRect, screenPoint, null, out var localPoint))
            {
                instance.GetComponent<RectTransform>().anchoredPosition = localPoint;
            }

            instance.Spawn(amount, color, lifetime, riseSpeed, fadeStartFraction);
        }
    }
}
