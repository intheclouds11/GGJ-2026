using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace intheclouds
{
    public class PauseMenu : MonoBehaviour
    {
        [SerializeField]
        private GameObject _defaultButton;
        [SerializeField]
        private Button _loadCheckpointButton;
        [SerializeField]
        private GameObject _loadCheckpointPrompt;
        [SerializeField]
        private Button _loadCheckpointButtonConfirm;
        [SerializeField]
        private Button _loadCheckpointButtonCancel;
        [SerializeField]
        private Button _returnToMainMenuButton;
        [SerializeField]
        private GameObject _returnToMainMenuPrompt;
        [SerializeField]
        private Button _returnToMainMenuButtonConfirm;
        [SerializeField]
        private Button _returnToMainMenuButtonCancel;

        public static PauseMenu Instance;
        public bool IsPaused { get; private set; }

        public Canvas PauseCanvas { get; private set; }


        private void Awake()
        {
            Instance = this;
            PauseCanvas = GetComponent<Canvas>();
            gameObject.SetActive(false);
            _loadCheckpointPrompt.SetActive(false);
            _returnToMainMenuPrompt.SetActive(false);
        }

        public void ToggleCanvas(bool toggle)
        {
            PauseCanvas.enabled = toggle;
            gameObject.SetActive(toggle);
            _loadCheckpointPrompt.SetActive(false);
            _returnToMainMenuPrompt.SetActive(false);
        }

        public void OnPauseButtonPressed()
        {
            if (!PauseCanvas.enabled)
            {
                OpenMenu();
            }
            else
            {
                if (_returnToMainMenuPrompt.activeInHierarchy)
                {
                    Button_ReturnToMainMenuCancel();
                }
                else if (_loadCheckpointPrompt.activeInHierarchy)
                {
                    Button_LoadCheckpointCancel();
                }
                else
                {
                    ResumeGame();
                }
            }
        }

        public void OpenMenu()
        {
            Cursor.lockState = CursorLockMode.None;
            IsPaused = true;
            EventSystem.current.SetSelectedGameObject(_defaultButton);
            ToggleCanvas(true);
            InputManager.Instance.ToggleInputsAllowed(false);
        }

        public void ResumeGame(bool allowInputs = true)
        {
            Cursor.lockState = CursorLockMode.Locked;

            IsPaused = false;
            ToggleCanvas(false);

            // if (GameManager.Instance.Player1.Health.IsAlive())
            {
                InputManager.Instance.ToggleInputsAllowed(allowInputs);
            }
        }

        public void OnReturnToMainMenu()
        {
            if (!_returnToMainMenuPrompt.activeInHierarchy)
            {
                _returnToMainMenuPrompt.SetActive(true);
            }
            
            EventSystem.current.SetSelectedGameObject(_returnToMainMenuButtonCancel.gameObject);
        }

        public void Button_ReturnToMainMenuConfirm()
        {
            UIManager.Instance.PreventPauseMenu = true;
            ResumeGame(false);

            GameManager.Instance.LoadScene("MainMenu");
        }

        public void Button_ReturnToMainMenuCancel()
        {
            EventSystem.current.SetSelectedGameObject(_returnToMainMenuButton.gameObject);
            _returnToMainMenuPrompt.SetActive(false);
        }

        public void Button_LoadCheckpoint()
        {
            if (!_loadCheckpointPrompt.activeInHierarchy)
            {
                _loadCheckpointPrompt.SetActive(true);
            }

            EventSystem.current.SetSelectedGameObject(_loadCheckpointButtonCancel.gameObject);
        }

        public void Button_LoadCheckpointConfirm()
        {
            UIManager.Instance.PreventPauseMenu = true;
            ResumeGame(false);
            GameManager.Instance.LoadScene(SceneManager.GetActiveScene().name);
        }

        public void Button_LoadCheckpointCancel()
        {
            EventSystem.current.SetSelectedGameObject(_loadCheckpointButton.gameObject);

            _loadCheckpointPrompt.SetActive(false);
        }
    }
}