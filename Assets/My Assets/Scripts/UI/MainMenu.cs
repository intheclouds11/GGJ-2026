using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace intheclouds
{
    public class MainMenu : MonoBehaviour
    {
        public static MainMenu Instance;
    
        [SerializeField]
        private float _fadeOutDuration = 2f;

        [SerializeField]
        private GameObject _newGameButton;
        [SerializeField]
        private GameObject _newGameConfirmCanvas;
        [SerializeField]
        private GameObject _newGameCancelButton;
        [SerializeField]
        private GameObject _continueButton;
        [SerializeField]
        private GameObject _feedbackButton;
        [SerializeField]
        private GameObject _settingsButton;
        [SerializeField]
        private GameObject _quitButton;
        [SerializeField]
        private AudioClip _menuMusic;
        [SerializeField]
        private float _menuMusicVolume = 0.8f;
        [SerializeField]
        private AudioClip _startGameSFX;
        [SerializeField]
        private float _skyboxRotationSpeed = 0.01f;
        [SerializeField]
        private UnityEvent _gameStarted;
        [SerializeField]
        private string _feedbackURL = "https://forms.gle/9t4dLaUHjLKMFwAr8";

        private Material _runtimeSkybox;
        private float _initialRot;
        private Canvas _mainMenuCanvas;
        
        private void Awake()
        {
            Instance = this;
            _mainMenuCanvas = GetComponent<Canvas>();
        }

        private void Start()
        {
            _runtimeSkybox = new Material(RenderSettings.skybox);
            RenderSettings.skybox = _runtimeSkybox;
            _initialRot = RenderSettings.skybox.GetFloat("_Rotation");

            StartCoroutine(ScreenFader.Instance.StartScreenFadeCoroutine(true, 1f));
        }

        private void Update()
        {
            RenderSettings.skybox.SetFloat("_Rotation", _initialRot + Time.time * _skyboxRotationSpeed);
        }

        public void ToggleMainMenuCanvas(bool toggle)
        {
            _mainMenuCanvas.enabled = toggle;
        }

        public void Button_NewGame()
        {
            
        }

        public void Button_NewGameConfirm()
        {
           
        }

        public void Button_NewGameCancel()
        {
            _newGameConfirmCanvas.SetActive(false);
            ToggleMainMenuCanvas(true);
        }

        public void Button_Continue()
        {
            StartGame();
        }

        private void StartGame()
        {
            StartCoroutine(StartGameCoroutine());
        }

        private IEnumerator StartGameCoroutine()
        {
            _gameStarted?.Invoke();

            InputManager.Instance.Vibrate(0.4f, 0.1f, 1.5f);
        
            yield return ScreenFader.Instance.StartScreenFadeCoroutine(false, _fadeOutDuration);
        
            GameManager.Instance.GameStart();
            
        }

        public void Button_ShowSettings()
        {
            UIManager.Instance.Button_ShowSettingsMenu();
        }

        public void Button_ExitGame()
        {
            UIManager.Instance.Button_ExitGame();
        }
    
        public void Button_LeaveFeedback()
        {
            Application.OpenURL(_feedbackURL);
        }
    }
}