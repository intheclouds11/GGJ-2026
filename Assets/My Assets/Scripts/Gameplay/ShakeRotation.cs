using System;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;

namespace intheclouds
{
    public class ShakeRotation : MonoBehaviour
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
        private bool _fadeOut = true;
        [SerializeField]
        private ShakeRandomnessMode _randomnessMode = ShakeRandomnessMode.Full;

        [SerializeField]
        private bool _snapToStartLocalRot = true;

        private Tweener _tweener;
        public event Action OnTweenEnded;
        private Quaternion _startLocalRot;


        private void Awake()
        {
            _startLocalRot = transform.localRotation;
        }

        [Button, PropertyOrder(-1f)]
        public void StartShake(Quaternion startRot = default, bool snapToStartRot = false)
        {
            _tweener?.Kill();
            if (snapToStartRot)
            {
                transform.localRotation = startRot;
            }
            else if (_snapToStartLocalRot)
            {
                transform.localRotation = _startLocalRot;
            }

            _tweener = transform.DOShakeRotation(_duration, _strength, _vibrato, _randomness, _fadeOut, _randomnessMode)
                .OnComplete(() => OnTweenEnded?.Invoke());
        }

        public void StartShake(float duration, float strength = 1f, int vibrato = 10, float randomness = 90f, bool fadeOut = true,
            ShakeRandomnessMode mode = ShakeRandomnessMode.Full)
        {
            _tweener?.Kill();
            if (_snapToStartLocalRot) transform.localRotation = _startLocalRot;
            _tweener = transform.DOShakeRotation(duration, strength, vibrato, randomness, fadeOut, mode)
                .OnComplete(() => OnTweenEnded?.Invoke());
        }

        public void StopShake(Quaternion startRot = default, bool snapToStartRot = false)
        {
            _tweener?.Kill(true);
            if (snapToStartRot)
            {
                transform.localRotation = startRot;
            }
            else if (_snapToStartLocalRot)
            {
                transform.localRotation = _startLocalRot;
            }
        }

        public void ModifyParameters(float duration, float strength = 1f, int vibrato = 10, float randomness = 90f, bool fadeOut = true,
            ShakeRandomnessMode mode = ShakeRandomnessMode.Full)
        {
            _duration = duration;
            _strength = strength;
            _vibrato = vibrato;
            _randomness = randomness;
            _fadeOut = fadeOut;
            _randomnessMode = mode;
        }
    }
}