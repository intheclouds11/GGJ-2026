using System;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;

namespace intheclouds
{
    public class ShakePosition : MonoBehaviour
    {
        [SerializeField]
        private float _duration = 1f;
        [SerializeField]
        private float _strength = 1f;
        [SerializeField]
        private int _vibrato = 10;
        [SerializeField, Tooltip("Indicates how much the shake will be random (0 to 180 - values higher than 90 kind of suck, " +
                                 "so beware). Setting it to 0 will shake along a single direction.")]
        private float _randomness = 90f;
        [SerializeField]
        private bool _snapping;
        [SerializeField]
        private bool _fadeOut = true;
        [SerializeField]
        private ShakeRandomnessMode _randomnessMode = ShakeRandomnessMode.Full;

        private Tweener _tweener;
        public event Action OnTweenEnded;
        private Vector3 _startLocalPos;


        private void Awake()
        {
            _startLocalPos = transform.localPosition;
        }

        [Button, PropertyOrder(-1f)]
        public void StartShake()
        {
            _tweener?.Kill();
            transform.localPosition = _startLocalPos;
            _tweener = transform
                .DOShakePosition(_duration, _strength, _vibrato, _randomness, _snapping, _fadeOut, _randomnessMode)
                .OnComplete(() => OnTweenEnded?.Invoke());
        }

        public void StartShake(float duration, float strength = 1f, int vibrato = 10, float randomness = 90f, bool snapping = false,
            bool fadeOut = true, ShakeRandomnessMode mode = ShakeRandomnessMode.Full)
        {
            _tweener?.Kill();
            transform.localPosition = _startLocalPos;
            _tweener = transform.DOShakePosition(duration, strength, vibrato, randomness, snapping, fadeOut, mode)
                .OnComplete(() => OnTweenEnded?.Invoke());
        }

        public void StopShake()
        {
            _tweener?.Kill(true);
            transform.localPosition = _startLocalPos;
        }
    }
}