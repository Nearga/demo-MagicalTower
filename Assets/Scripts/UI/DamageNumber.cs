using TMPro;
using UnityEngine;

namespace MagicalTower.UI
{
    /// <summary>
    /// Self-contained floating damage number. Rises and fades over its lifetime,
    /// then destroys itself. Driven by Update so no coroutine GC overhead.
    /// </summary>
    public sealed class DamageNumber : MonoBehaviour
    {
        [SerializeField] private TMP_Text label;
        [SerializeField] private CanvasGroup canvasGroup;

        private float lifetime;
        private float riseSpeed;
        private float fadeStartFraction;
        private float timer;
        private RectTransform rectTransform;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
        }

        public void Spawn(int amount, Color color, float lifetime, float riseSpeed, float fadeStartFraction)
        {
            this.lifetime           = lifetime;
            this.riseSpeed          = riseSpeed;
            this.fadeStartFraction  = Mathf.Clamp01(fadeStartFraction);
            timer                   = 0f;

            if (label != null)
            {
                label.text  = amount.ToString();
                label.color = color;
            }

            if (canvasGroup != null)
            {
                canvasGroup.alpha = 1f;
            }
        }

        private void Update()
        {
            timer += Time.deltaTime;

            // Rise
            if (rectTransform != null)
            {
                rectTransform.anchoredPosition += Vector2.up * (riseSpeed * Time.deltaTime);
            }

            // Fade
            if (canvasGroup != null && lifetime > 0f)
            {
                var progress = timer / lifetime;
                if (progress >= fadeStartFraction)
                {
                    var fadeProgress = (progress - fadeStartFraction) / (1f - fadeStartFraction);
                    canvasGroup.alpha = Mathf.Lerp(1f, 0f, fadeProgress);
                }
            }

            if (timer >= lifetime)
            {
                Destroy(gameObject);
            }
        }
    }
}
