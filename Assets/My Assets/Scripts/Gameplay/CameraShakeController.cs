using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

namespace intheclouds
{
    [Serializable]
    public struct ScreenShakeSettings
    {
        public float Amplitude;
        public float Frequency;
        public float UpDuration;
        public float DownDuration;
        public Ease UpEasing;
        public Ease DownEasing;
        [Tooltip("Will use active virtual camera if not assigned")]
        public CinemachineCamera VirtualCam;
    }

    public class CameraShakeController : MonoBehaviour
    {
        public static CameraShakeController Instance;
        [SerializeField]
        private ScreenShakeSettings _enemyHitShake;
        [SerializeField]
        private ScreenShakeSettings _playerHitShake;
        [SerializeField]
        private Object _defaultNoiseProfile;

        private Sequence _shakeSequence;
        private List<CinemachineCamera> _sceneCameras;


        private void Awake()
        {
            Instance = this;
            SetupVCams();
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            SetupVCams();
            _shakeSequence?.Kill();
        }

        private void SetupVCams()
        {
            _sceneCameras = FindObjectsByType<CinemachineCamera>(FindObjectsInactive.Include, FindObjectsSortMode.None).ToList();
            foreach (var cinemachineCamera in _sceneCameras)
            {
                if (!cinemachineCamera.TryGetComponent<CinemachineBasicMultiChannelPerlin>(out var foundNoiseComp))
                {
                    var perlinNoise = cinemachineCamera.gameObject.AddComponent<CinemachineBasicMultiChannelPerlin>();
                    perlinNoise.enabled = false;
                    perlinNoise.NoiseProfile = _defaultNoiseProfile as NoiseSettings;
                }
            }
        }

        public void StartEnemyHitShake(bool interrupt = true)
        {
            StartShake(_enemyHitShake, interrupt);
        }
        
        public void StartPlayerHitShake(bool interrupt = true)
        {
            StartShake(_playerHitShake, interrupt);
        }

        public void StartShake(ScreenShakeSettings settings, bool interrupt = true)
        {
            if (!interrupt && _shakeSequence != null && _shakeSequence.IsActive()) return;

            if (settings.Amplitude == 0f) return;
            var virtualCam = settings.VirtualCam;
            if (!virtualCam)
            {
                virtualCam = CinemachineBrain.GetActiveBrain(0).ActiveVirtualCamera as CinemachineCamera;
                if (!virtualCam)
                {
                    Debug.LogError($"[CameraShakeController] No activeVCam found!");
                    return;
                }
            }

            var perlinNoise = virtualCam.GetComponent<CinemachineBasicMultiChannelPerlin>();
            perlinNoise.enabled = true;
            perlinNoise.FrequencyGain = settings.Frequency;

            var tweenAmp = 0f;
            _shakeSequence?.Kill(true);
            _shakeSequence = DOTween.Sequence();
            _shakeSequence.Append(DOTween.To(() => tweenAmp, x => tweenAmp = x, settings.Amplitude, settings.UpDuration)
                    .SetEase(settings.UpEasing))
                .Append(DOTween.To(() => tweenAmp, x => tweenAmp = x, 0f, settings.DownDuration).SetEase(settings.DownEasing))
                .OnUpdate(() => OnShakeUpdate(tweenAmp))
                .OnComplete(OnShakeComplete);
        }

        private void OnShakeUpdate(float tweenAmp)
        {
            var virtualCam = CinemachineBrain.GetActiveBrain(0).ActiveVirtualCamera as CinemachineCamera;
            var perlinNoise = virtualCam.GetComponent<CinemachineBasicMultiChannelPerlin>();
            perlinNoise.enabled = true;
            perlinNoise.AmplitudeGain = tweenAmp;
        }

        private void OnShakeComplete()
        {
            // Debug.Log("Sequence with custom easing complete!");
            foreach (var cinemachineCamera in _sceneCameras)
            {
                var perlinNoise = cinemachineCamera.GetComponent<CinemachineBasicMultiChannelPerlin>();
                if (perlinNoise)
                {
                    perlinNoise.AmplitudeGain = 0f;
                    perlinNoise.enabled = false;
                }
            }

        }
    }
}