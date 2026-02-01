#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace intheclouds
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance;

        [SerializeField]
        private PauseMenu _pauseMenu;
        [SerializeField]
        private AudioSource _uiAudio;
        [SerializeField]
        private AudioClip _navigateSFX;
        [SerializeField]
        private float _selectionChangedVolume = 0.65f;

        public bool PreventPauseMenu;

        private GameObject _lastSelected;
        private EventSystem _eventSystem;


        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            GameManager.Instance.OnStateChanged += OnGameStateChanged;
            _eventSystem = EventSystem.current;
            _lastSelected = _eventSystem.currentSelectedGameObject;
        }

        private void OnDisable()
        {
            GameManager.Instance.OnStateChanged -= OnGameStateChanged;
        }

        private void OnGameStateChanged(GameState newState)
        {
            if (newState is GameState.MainMenu or GameState.Playing)
            {
                if (newState is GameState.MainMenu) PreventPauseMenu = true;
                if (newState is GameState.Playing) PreventPauseMenu = false;
                ToggleRespawnScreen(false);
            }
            else if (newState is GameState.GameOver)
            {
                PreventPauseMenu = true;
                ToggleRespawnScreen(true);
            }
        }

        private void Update()
        {
            if (_eventSystem.currentSelectedGameObject && _eventSystem.currentSelectedGameObject != _lastSelected)
            {
                if (_uiAudio)
                {
                    _uiAudio.volume = _selectionChangedVolume;
                    _uiAudio.pitch = Random.Range(0.95f, 1f);
                    _uiAudio.clip = _navigateSFX;
                    _uiAudio.Play();
                }
            }

            if (!_eventSystem.currentSelectedGameObject) _eventSystem.SetSelectedGameObject(_lastSelected);
            else _lastSelected = _eventSystem.currentSelectedGameObject;

            if (!PreventPauseMenu && InputManager.Instance.PauseWasPressed)
            {
                if (CanTogglePauseScreen())
                {
                    _pauseMenu.OnPauseButtonPressed();
                }
                else if (SceneManager.GetActiveScene().name != "MainMenu")
                {
                    ExitPauseMenu();
                }
            }
            else if (InputManager.Instance.GamepadEastButtonWasPressed)
            {
                if (_pauseMenu.PauseCanvas.enabled)
                {
                    _pauseMenu.OnPauseButtonPressed();
                }
            }
        }

        private bool CanTogglePauseScreen()
        {
            return SceneManager.GetActiveScene().name != "MainMenu";
        }

        public bool IsAMenuOpen()
        {
            return _pauseMenu.PauseCanvas.enabled || SceneManager.GetActiveScene().name == "MainMenu";
        }

        public void ToggleRespawnScreen(bool toggle)
        {
        }

        public void Button_ResumeGame()
        {
            _pauseMenu.ResumeGame();
        }

        public void Button_ReturnToMainMenu()
        {
            if (_pauseMenu.PauseCanvas.enabled)
            {
                _pauseMenu.OnReturnToMainMenu();
            }
        }

        public void Button_ShowSettingsMenu()
        {
            if (MainMenu.Instance)
            {
                MainMenu.Instance.ToggleMainMenuCanvas(false);
            }
            else if (_pauseMenu.PauseCanvas.enabled)
            {
                _pauseMenu.ToggleCanvas(false);
            }

        }

        public void ExitPauseMenu(bool allowInputs = true)
        {
            _pauseMenu.ResumeGame(allowInputs);
        }

        public void ExitSettingsMenu(bool returnToPauseMenu = false)
        {
            if (MainMenu.Instance)
            {
                MainMenu.Instance.ToggleMainMenuCanvas(true);
            }
            else if (returnToPauseMenu)
            {
                _pauseMenu.OpenMenu();
            }

        }

        public void Button_ExitGame()
        {
#if UNITY_EDITOR
             Cursor.visible = true;
             EditorApplication.isPlaying = false;
//             var fullscreenContainers = FullscreenEditor.Fullscreen.GetAllFullscreen().ToList();
//             if (fullscreenContainers.Any())
//             {
//                 FullscreenEditor.Fullscreen.GetFullscreenFromView(fullscreenContainers[0].FullscreenedView).Close();
//             }
#endif
            Application.Quit();
        }
    }
}