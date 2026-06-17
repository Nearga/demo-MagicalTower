using DG.Tweening;
using UnityEngine;

namespace MagicalTower.Runtime
{
    public sealed class BurningStatusVisual : MonoBehaviour
    {
        [SerializeField] private Transform visualRoot;
        [SerializeField] private ParticleSystem[] particleSystems;
        [SerializeField] private float pulseScale = 1.12f;
        [SerializeField] private float pulseDuration = 0.36f;
        [SerializeField] private float stopScaleDuration = 0.16f;

        private Sequence pulseSequence;

        private void Awake()
        {
            if (visualRoot == null)
            {
                visualRoot = transform;
            }

            if (particleSystems == null || particleSystems.Length == 0)
            {
                particleSystems = GetComponentsInChildren<ParticleSystem>(true);
            }
        }

        private void OnDestroy()
        {
            pulseSequence?.Kill();
            pulseSequence = null;
        }

        private void OnDisable()
        {
            pulseSequence?.Kill();
            pulseSequence = null;
        }

        public void Play()
        {
            gameObject.SetActive(true);
            visualRoot.localScale = Vector3.one;

            for (var i = 0; i < particleSystems.Length; i++)
            {
                particleSystems[i].Play(true);
            }

            pulseSequence?.Kill();
            pulseSequence = DOTween.Sequence()
                .SetTarget(this)
                .Append(visualRoot.DOScale(Vector3.one * pulseScale, pulseDuration).SetEase(Ease.InOutSine))
                .Append(visualRoot.DOScale(Vector3.one, pulseDuration).SetEase(Ease.InOutSine))
                .SetLoops(-1);
        }

        public void StopAndDestroy()
        {
            pulseSequence?.Kill();
            pulseSequence = null;

            for (var i = 0; i < particleSystems.Length; i++)
            {
                particleSystems[i].Stop(true, ParticleSystemStopBehavior.StopEmitting);
            }

            visualRoot.DOScale(Vector3.zero, stopScaleDuration)
                .SetEase(Ease.InCubic)
                .SetTarget(this)
                .OnComplete(() =>
                {
                    if (this != null)
                    {
                        ReleaseOrDestroy();
                    }
                });
        }

        private void ReleaseOrDestroy()
        {
            if (TryGetComponent<PooledObject>(out var pooledObject) && pooledObject.HasOwner)
            {
                pooledObject.Release();
                return;
            }

            Destroy(gameObject);
        }
    }
}
