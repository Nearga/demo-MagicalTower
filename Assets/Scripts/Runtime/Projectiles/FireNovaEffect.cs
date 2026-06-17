using DG.Tweening;
using UnityEngine;

namespace MagicalTower.Runtime
{
    public sealed class FireNovaEffect : MonoBehaviour
    {
        [SerializeField] private Transform visualRoot;
        [SerializeField] private ParticleSystem[] particleSystems;
        [SerializeField] private float expandDuration = 0.22f;
        [SerializeField] private float holdDuration = 0.08f;
        [SerializeField] private float fadeDuration = 0.18f;
        [SerializeField] private Ease expandEase = Ease.OutCubic;
        [SerializeField] private Ease fadeEase = Ease.InCubic;

        private Sequence sequence;

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
            sequence?.Kill();
            sequence = null;
        }

        private void OnDisable()
        {
            sequence?.Kill();
            sequence = null;
        }

        public void Play(float radius)
        {
            var targetRadius = Mathf.Max(0.01f, radius);
            sequence?.Kill();

            visualRoot.localScale = Vector3.one * 0.05f;
            for (var i = 0; i < particleSystems.Length; i++)
            {
                particleSystems[i].Play(true);
            }

            sequence = DOTween.Sequence()
                .SetTarget(this)
                .Append(visualRoot.DOScale(Vector3.one * targetRadius, expandDuration).SetEase(expandEase))
                .AppendInterval(holdDuration)
                .AppendCallback(StopParticles)
                .Append(visualRoot.DOScale(Vector3.one * (targetRadius * 0.82f), fadeDuration).SetEase(fadeEase))
                .OnComplete(ReleaseOrDestroy);
        }

        private void StopParticles()
        {
            for (var i = 0; i < particleSystems.Length; i++)
            {
                particleSystems[i].Stop(true, ParticleSystemStopBehavior.StopEmitting);
            }
        }

        private void ReleaseOrDestroy()
        {
            if (this == null)
            {
                return;
            }

            if (TryGetComponent<PooledObject>(out var pooledObject) && pooledObject.HasOwner)
            {
                pooledObject.Release();
                return;
            }

            Destroy(gameObject);
        }
    }
}
