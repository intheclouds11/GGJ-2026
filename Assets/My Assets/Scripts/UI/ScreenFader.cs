using System.Collections;
using UnityEngine;

namespace intheclouds
{
    public class ScreenFader : MonoBehaviour
    {
        public static ScreenFader Instance;
        public static bool IsFading { get; private set; }
        
        [SerializeField]
        private CanvasGroup _blackFadeCanvasGroup, _whiteFadeCanvasGroup;
        [SerializeField]
        private float _defaultFadeDuration = 2f;

        private Coroutine _fadeCoroutine;


        private void Awake()
        {
            Instance = this;
        }

        public IEnumerator StartScreenFadeCoroutine(bool fadeIn, float fadeDuration = -1f, bool useBlack = true)
        {
            IsFading = true;
            if (_fadeCoroutine != null) StopCoroutine(_fadeCoroutine);
            yield return _fadeCoroutine = StartCoroutine(ScreenFadeCoroutine(fadeIn, fadeDuration, useBlack));
        }

        private IEnumerator ScreenFadeCoroutine(bool fadeIn, float fadeDuration, bool useBlack)
        {
            float startTime = Time.time;
            float duration = Mathf.Approximately(fadeDuration, -1) ? _defaultFadeDuration : fadeDuration;

            if (fadeIn)
            {
                while (Time.time < startTime + duration)
                {
                    _blackFadeCanvasGroup.alpha -= Time.deltaTime / duration;
                    _whiteFadeCanvasGroup.alpha -= Time.deltaTime / duration;
                    yield return null;
                }
            }
            else
            {
                while (Time.time < startTime + duration)
                {
                    if (useBlack)
                        _blackFadeCanvasGroup.alpha += Time.deltaTime / duration;
                    else
                        _whiteFadeCanvasGroup.alpha += Time.deltaTime / duration;

                    yield return null;
                }
                
                if (useBlack)
                    _blackFadeCanvasGroup.alpha = 1f;
                else
                    _whiteFadeCanvasGroup.alpha = 1f;
            }
            
            IsFading = false;
        }
    }
}
