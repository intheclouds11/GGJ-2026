using System;
using UnityEngine;

namespace intheclouds
{
    public class PlayerManager : MonoBehaviour
    {
        [SerializeField]
        private float _distToTriggerFootstep = 2f;
        public InputManager Inputs { get; private set; }

        private float _distMovedSinceLastFootstep;
        private Vector3 _lastPosition;


        private void Awake()
        {
            Inputs = InputManager.Instance;
        }

        private void Update()
        {
            // todo: footstep logic
            _lastPosition = transform.position;
        }
    }
}