using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace intheclouds
{
    public class PlayerManager : MonoBehaviour
    {
        public static PlayerManager Instance;

        [Title("Attack")]
        [SerializeField]
        private float _attackRadius = 2f;
        private Collider[] _hitCols = new Collider[5];
        [SerializeField]
        private Animator _attackAnimator;
        
        [SerializeField]
        private float _distToTriggerFootstep = 2f;
        public InputManager Inputs { get; private set; }
        public FirstPersonController Controller { get; private set; }

        private float _distMovedSinceLastFootstep;
        private Vector3 _lastPosition;


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
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                _attackAnimator.SetTrigger("Attack");
                
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
                            hitProjectile.Deflect();
                        }
                    }
                }
            }

            // todo: footstep logic
            _lastPosition = transform.position;
        }

        public void TakeDamage(Projectile projectile, Vector3 hitDir)
        {
            Controller.OnDamaged(projectile, hitDir);
        }
    }
}