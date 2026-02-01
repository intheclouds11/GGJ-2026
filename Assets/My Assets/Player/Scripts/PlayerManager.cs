using FMODUnity;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace intheclouds
{
    public class PlayerManager : MonoBehaviour
    {
        public static PlayerManager Instance;

        //Sounds stuff
        [SerializeField] private EventReference deflectEvent;

        [Title("Attack")]
        [SerializeField]
        private float _attackRadius = 2f;
        [SerializeField]
        private float _attackDuration = 0.5f;
        private Collider[] _hitCols = new Collider[5];
        [SerializeField]
        private Animator _attackAnimator;

        [Title("Health")]
        public ScreenShakeSettings DamagedScreenShakeSettings;

        [SerializeField]
        private float _distToTriggerFootstep = 2f;
        public InputManager Inputs { get; private set; }
        public FirstPersonController Controller { get; private set; }
        public MaskColorFilterSwapper MaskColorFilterSwapper { get; private set; }
        public MaskManager MaskManager { get; private set; }

        private float _distMovedSinceLastFootstep;
        private Vector3 _lastPosition;
        private Coroutine _attackCoroutine;


        private void Awake()
        {
            if (Instance)
            {
                Destroy(Instance.gameObject);
                return;
            }

            Instance = this;
            Inputs = InputManager.Instance;
            Controller = GetComponent<FirstPersonController>();
            MaskColorFilterSwapper = GetComponent<MaskColorFilterSwapper>();
            MaskManager = GetComponent<MaskManager>();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Mouse0) && !PauseMenu.Instance.IsPaused)
            {
                if (_attackCoroutine != null) StopCoroutine(_attackCoroutine);
                _attackCoroutine = StartCoroutine(AttackCoroutine());
            }

            // todo: footstep logic
            _lastPosition = transform.position;
        }

        private IEnumerator AttackCoroutine()
        {
            _attackAnimator.SetTrigger("Attack");

            var startTime = Time.time;
            while (Time.time < startTime + _attackDuration)
            {
                var hitCount = Physics.OverlapSphereNonAlloc(Controller.playerCamera.transform.position, _attackRadius, _hitCols,
                    LayerMask.GetMask("Enemy"));
                if (hitCount > 0)
                {
                    foreach (var hitCol in _hitCols)
                    {
                        if (!hitCol) continue;
                        var hitEnemy = hitCol.GetComponentInParent<Enemy>();
                        var hitProjectile = hitCol.GetComponentInParent<Projectile>();
                        if (hitEnemy)
                        {
                            hitEnemy.TakeDamage(1);
                        }
                        else if (hitProjectile)
                        {
                            //sound
                            RuntimeManager.PlayOneShot(deflectEvent, transform.position);


                            hitProjectile.DeflectToSpawner(true);
                        }
                    }
                }

                yield return null;
            }

            _attackCoroutine = null;
        }

        public void TakeDamage(Projectile projectile, Vector3 hitDir)
        {
            CameraShakeController.Instance.StartShake(DamagedScreenShakeSettings);
            Controller.OnDamaged(projectile, hitDir);
        }
    }
}