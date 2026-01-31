using Sirenix.OdinInspector;
using UnityEngine;

namespace intheclouds
{
    public class Follower : MonoBehaviour
    {
        [SerializeField]
        private bool _useSmoothingCurve;
        [SerializeField, ShowIf(nameof(_useSmoothingCurve))]
        private AnimationCurve _smoothingCurve;
        [SerializeField]
        private Transform _target;
        public Transform Target => _target;
        [SerializeField]
        private bool _getAwakeTargetOffset;
        [SerializeField, HideIf(nameof(_getAwakeTargetOffset))]
        private Vector3 _offset;


        private void Awake()
        {
            if (_getAwakeTargetOffset)
            {
                if (!_target)
                {
                    Debug.LogWarning($"[Follower] no target assigned, but GetAwakeTargetOffset is true!", this);
                    return;
                }

                _offset = transform.position - _target.position;
            }
        }

        private void OnEnable()
        {
            SetToTargetPosition();
        }

        public void SetToTargetPosition()
        {
            if (_target)
            {
                transform.position = _target.position + _offset;
            }
        }

        public void SetTarget(Transform target)
        {
            enabled = target;
            _target = target;
        }

        private void Update()
        {
            if (_target)
            {
                var targetPos = _target.position + _offset;
                if (_useSmoothingCurve)
                {
                    var smoothing = _smoothingCurve.Evaluate(Vector3.Distance(transform.position, targetPos));
                    transform.position = Vector3.MoveTowards(transform.position, targetPos, smoothing * Time.deltaTime);
                }
                else
                {
                    transform.position = targetPos;
                }
            }
            else if (GameManager.Instance.Player1)
            {
                Debug.LogWarning($"[Follower] No target assigned! Using Player1.transform as target", this);
                _target = GameManager.Instance.Player1.transform;
            }
        }
    }
}