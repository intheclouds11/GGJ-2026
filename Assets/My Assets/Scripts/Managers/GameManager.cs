using System;
using System.Collections;
using DG.Tweening;
using Sirenix.OdinInspector;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace intheclouds
{
    public enum GameState
    {
        MainMenu,
        Playing,
        GameOver,
        Paused
    }

    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;

        [SerializeField, ReadOnly]
        private GameState _state;
        public GameState State
        {
            get => _state;
            set
            {
                if (_state != value) OnStateChanged?.Invoke(value);
                Debug.Log($"[GameManager] {value}");
                _state = value;
            }
        }
        public event Action<GameState> OnStateChanged;
        public PlayerManager Player1 { get; private set; }
        public CinemachineCamera VCam_Player { get; private set; }
        public CinemachineCamera VCam_FallDeath { get; private set; }
        public CinemachineCamera VCam_Throwable { get; private set; }
        /// <summary>
        /// Ignore enemy attacks. Toggle: G key
        /// </summary>
        public bool GodMode { get; private set; }
        /// <summary>
        /// Allow player hits, but no damage. Toggle: Shift + G 
        /// </summary>
        public bool InfiniteHealth { get; private set; }
        public bool EnemyAIEnabled { get; private set; } = true;
        public static bool ReleaseMode { get; private set; } = true;
        public static event Action<bool> ToggledReleaseMode;
        public static event Action<bool> EnemyAIToggled;


        private void Awake()
        {
            Instance = this;
            DontDestroyOnLoad(transform.root);
        }

        private void Update()
        {
            
        }

        private void OnEnable()
        {
        }

        private void OnDisable()
        {
        }

        public void GameStart()
        {
        }

        public void OnPlayerDied()
        {
            InputManager.Instance.ToggleInputsAllowed(false);
        }

        public bool Respawning { get; private set; }

        /// Main way to load or reload scenes. BigHand.cs handles post-boss scene transition (different fade logic)
        public void LoadScene(string sceneName, float fadeOutDuration = 2f)
        {
            StartCoroutine(StartLoadSceneCoroutine(sceneName));
        }

        private IEnumerator StartLoadSceneCoroutine(string sceneName, float fadeOutDuration = 2f)
        {
            Respawning = true;

            // yield return ScreenFader.Instance.StartScreenFadeCoroutine(false, fadeOutDuration);
            yield return null;

            Respawning = false;
            SceneManager.LoadScene(sceneName);
        }

      

        public Coroutine OnBossDefeated()
        {
            return StartCoroutine(OnBossDefeatedCoroutine());
        }

        /// Slow down time and zoom in.
        public IEnumerator OnBossDefeatedCoroutine()
        {
            var activeVCam = CinemachineBrain.GetActiveBrain(0).ActiveVirtualCamera as CinemachineCamera;
            var startFOV = activeVCam.Lens.FieldOfView;
            var tweenFOV = startFOV;
            var zoomInTween = DOTween.To(() => tweenFOV, x => tweenFOV = x, 40f, 1.5f)
                .OnUpdate(() => activeVCam.Lens.FieldOfView = tweenFOV);

            var timeScaleTweener = TimeManager.UpdateTimeScale(0.1f, 0.5f, Ease.OutSine);

            yield return timeScaleTweener.WaitForCompletion();

            // Debug.Log("time scale complete");

            zoomInTween.Kill();
            DOTween.To(() => tweenFOV, x => tweenFOV = x, startFOV, 0.5f).OnUpdate(() => activeVCam.Lens.FieldOfView = tweenFOV);
            TimeManager.UpdateTimeScale(1f, 0.5f, Ease.InExpo);
        }
    }
}