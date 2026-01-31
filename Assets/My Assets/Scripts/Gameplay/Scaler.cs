using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

namespace intheclouds
{
    public class Scaler : MonoBehaviour
    {
        public Vector3 ScaleUpTarget;
        public float ScaleUpDuration = 1f;
        public Ease ScaleUpEasing = Ease.OutElastic;
        public Vector3 ScaleDownTarget;
        public float ScaleDownDuration = 0.5f;
        public Ease ScaleDownEasing;

        TweenerCore<Vector3, Vector3, VectorOptions> _tweener;


        public void ScaleUp()
        {
            if (_tweener != null && _tweener.IsActive()) _tweener.Kill();
            
            var tweenScale = transform.localScale;
            // todo: consider normalizing duration based on actual scale difference
            _tweener = DOTween.To(() => tweenScale, x => tweenScale = x, ScaleUpTarget, ScaleUpDuration).SetEase(ScaleUpEasing)
                .OnUpdate(() => transform.localScale = tweenScale);
        }

        public void ScaleDown()
        {
            if (_tweener != null && _tweener.IsActive()) _tweener.Kill();
            
            var tweenScale = transform.localScale;
            _tweener = DOTween.To(() => tweenScale, x => tweenScale = x, ScaleDownTarget, ScaleDownDuration).SetEase(ScaleDownEasing)
                .OnUpdate(() => transform.localScale = tweenScale);
        }

        private void OnDisable()
        {
            _tweener?.Kill();
        }
    }
}