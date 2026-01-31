using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace intheclouds
{
    [AddComponentMenu(".Level Design/Mover")]
    public class Mover : MonoBehaviour
    {
        [SerializeField]
        private UnityEvent _onReachedTarget;
        [SerializeField]
        private Vector3 _startPosition;
        [SerializeField]
        private Vector3 _endPosition;
        [SerializeField]
        private float _moveDuration = 1f;
        [SerializeField]
        private Ease _moveEasing;
        [SerializeField]
        private AudioClip _moveSFX;
        [SerializeField]
        private float _moveSFXVolume = 0.7f;
        [SerializeField]
        private AudioClip _moveEndedSFX;

        private bool _isMoving;
        private float _distToMove;
        private TweenerCore<Vector3, Vector3, VectorOptions> _moveTweener;


        [Button]
        private void SetStartPosition()
        {
            _startPosition = transform.position;
        }

        [Button]
        private void MoveToStartPosition()
        {
            transform.position = _startPosition;
        }

        [Button]
        private void SetEndPosition()
        {
            _endPosition = transform.position;
        }

        [Button]
        private void MoveToEndPosition()
        {
            transform.position = _endPosition;
        }

        private void Awake()
        {
            transform.position = _startPosition;
        }

        public void StartMoving()
        {
            HandleMove(_endPosition);
        }

        public void MoveToStartLocalPosition()
        {
            HandleMove(_startPosition);
        }

        private void HandleMove(Vector3 targetLocalPos)
        {
            _isMoving = true;

            // if (_moveSFX) _moveAudioLoop = AudioManager.Instance.PlaySoundLoop(transform, _moveSFX, true, _moveSFXVolume);
            _moveTweener?.Kill();
            _moveTweener = transform.DOMove(targetLocalPos, _moveDuration).SetEase(_moveEasing).OnComplete(() =>
            {
                // _moveAudioLoop?.StopAndClear(ref _moveAudioLoop);
                // if (_moveEndedSFX) AudioManager.Instance.PlayOneShot(transform, _moveEndedSFX, true, _moveSFXVolume);
                _onReachedTarget?.Invoke();
                _isMoving = false;
            });
        }
    }
}