using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using Sirenix.OdinInspector;
using UnityEngine;

namespace intheclouds
{
    public class TimeManager : MonoBehaviour
    {
        public float NewTimeScale = 1f;

        [ShowInInspector, ReadOnly]
        private float _currentTimeScale;
        private float _defaultTimeScale;
        private static TweenerCore<float, float, FloatOptions> _tweener;


        private void Awake()
        {
            _currentTimeScale = Time.timeScale;
            _defaultTimeScale = _currentTimeScale;
        }

        private void Update()
        {
            if (InputManager.Instance.TimeScaleUpWasPressed)
            {
                Time.timeScale += 0.3f;
                Debug.Log($"Timescale large increase to {Time.timeScale}");
            }

            else if (InputManager.Instance.TimeScaleDownWasPressed)
            {
                Time.timeScale -= 0.3f;
                Debug.Log($"Timescale large decrease to {Time.timeScale}");
            }
            else if (InputManager.Instance.TimeScaleResetWasPressed)
            {
                Time.timeScale = _defaultTimeScale;
                Debug.Log($"Timescale reset to {Time.timeScale}");
            }
        }

        public static TweenerCore<float, float, FloatOptions> UpdateTimeScale(float newTimeScale, float duration = 0f, Ease ease = Ease.Linear)
        {
            _tweener?.Kill();
            
            if (duration > 0f)
            {
                var tweenTimeScale = Time.timeScale;
                return _tweener = DOTween.To(() => tweenTimeScale, x => tweenTimeScale = x, newTimeScale, duration).SetEase(ease)
                    .OnUpdate(() => Time.timeScale = tweenTimeScale);
            }

            Time.timeScale = newTimeScale;
            return null;
        }
    }
}