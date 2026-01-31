using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace intheclouds
{
    public class RespawnScreen : MonoBehaviour
    {
        [SerializeField]
        private float _delayRespawnInputTime = 0.5f;
        [SerializeField]
        private float _delayRespawnInputTime_TipShown = 2f;
        [SerializeField]
        private float _fadeOutTime = 2f;
        [SerializeField]
        private TextMeshProUGUI _respawnText;
        [SerializeField]
        private GameObject _tipsParent;
        [SerializeField]
        private TextMeshProUGUI _dashTipText;
        [SerializeField]
        private TextMeshProUGUI _critTipText;
        [SerializeField]
        private TextMeshProUGUI _projectileDeathTipText;
        [SerializeField]
        private TextMeshProUGUI _dazedDeathTipText;

        private float _lastTimeShown;
        private List<CanvasGroup> _canvasGroups;
        private Animator _animator;


        private void Awake()
        {
            _animator = GetComponent<Animator>();
            HideTips();
            _canvasGroups = GetComponentsInChildren<CanvasGroup>(true).ToList();
        }

        private void OnEnable()
        {
            _lastTimeShown = Time.time;
            if (PauseMenu.Instance.IsPaused)
            {
                PauseMenu.Instance.ResumeGame(false);
                UIManager.Instance.ExitSettingsMenu();
            }

            PlayerManager _player = GameManager.Instance.Player1;
            if (!_player) return;

           

            _animator.SetTrigger("Show");
        }

        private void OnDisable()
        {
            foreach (var canvasGroup in _canvasGroups)
            {
                canvasGroup.alpha = 0f;
            }

            HideTips();
        }

        private void HideTips()
        {
            _tipsParent.SetActive(false);
            foreach (Transform child in _tipsParent.transform)
            {
                child.gameObject.SetActive(false);
            }
        }

        private void ShowTip(GameObject tipObj)
        {
            _tipsParent.SetActive(true);
            tipObj.SetActive(true);
        }

        private void Update()
        {
            if (GameManager.Instance.Respawning) return;

            var respawnInputDelay = _tipsParent.gameObject.activeSelf ? _delayRespawnInputTime_TipShown : _delayRespawnInputTime;
            if (Time.time >= _lastTimeShown + respawnInputDelay && InputManager.Instance.RespawnWasPressed &&
                !PauseMenu.Instance.IsPaused)
            {
                _animator.SetTrigger("StartRespawn");
                GameManager.Instance.LoadScene(SceneManager.GetActiveScene().name, _fadeOutTime);
            }
        }
    }
}