using FMODUnity;
using intheclouds;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;
using Utils;

public class Enemy : MonoBehaviour
{

    //sound
    
    [SerializeField] private EventReference shoot;
    [SerializeField] private EventReference enemyHit;


    public float AggroRange = 8f;
    public float AggroCancelRange = 12f;
    public float RotateSpeed = 100f;

    [Title("Mask Swapping")]
    public GameObject NoMaskObjects;
    private ParticleSystem _noMaskParticles;
    public GameObject EnemyObjects;
    public GameObject PlatformObjects;

    [Title("Projectiles")]
    [SerializeField]
    private float _startFiringDelay = 1f;
    private float _enemyActivatedTime;
    public float FireRate = 1f;
    public float FireSpeed = 2f;
    public Transform ProjectileSpawnPoint;
    public Projectile ProjectilePrefab;
    public ShakeRotation FireShakeRotation;

    [Title("Health")]
    public int Health = 2;
    public bool IsAlive = true;
    public ParticleSystem DiedVFX;
    public ScreenShakeSettings DamagedScreenShakeSettings;

    private PlayerManager _player;
    private Coroutine _aggroCoroutine;


    private void Start()
    {
        if (!EnemyObjects)
            Debug.LogWarning($"Missing EnemyObjects", this);
        if (!PlatformObjects)
            Debug.LogWarning($"Missing PlatformObjects", this);
        if (!NoMaskObjects)
            Debug.LogWarning($"Missing NoMaskObjects", this);
        else
        {
            _noMaskParticles = NoMaskObjects.GetComponentInChildren<ParticleSystem>();
            if (_noMaskParticles)
            {
                NoMaskObjects.SetActive(true);
            }
        }

        _player = PlayerManager.Instance;
        PlayerManager.Instance.MaskManager.SwappedMask += OnMaskSwapped;
    }

    public void OnMaskSwapped(MaskManager.MaskType newMask)
    {
        CancelAggro();

        if (newMask is MaskManager.MaskType.NoMask)
        {
            EnemyObjects.SetActive(false);
            PlatformObjects.SetActive(false);
            if (_noMaskParticles)
            {
                _noMaskParticles.GetComponent<ParticleSystemRenderer>().enabled = true;
            }
            else
            {
                NoMaskObjects.SetActive(true);
            }
        }
        else if (newMask is MaskManager.MaskType.Enemy)
        {
            _enemyActivatedTime = Time.time;
            EnemyObjects.SetActive(true);
            PlatformObjects.SetActive(false);
            if (_noMaskParticles)
            {
                _noMaskParticles.GetComponent<ParticleSystemRenderer>().enabled = false;
            }
            else
            {
                NoMaskObjects.SetActive(false);
            }
        }
        else if (newMask is MaskManager.MaskType.Platforms)
        {
            EnemyObjects.SetActive(false);
            PlatformObjects.SetActive(true);
            if (_noMaskParticles)
            {
                _noMaskParticles.GetComponent<ParticleSystemRenderer>().enabled = false;
            }
            else
            {
                NoMaskObjects.SetActive(false);
            }
        }
        else if (newMask is MaskManager.MaskType.Pickups)
        {
            EnemyObjects.SetActive(false);
            PlatformObjects.SetActive(false);
            if (_noMaskParticles)
            {
                _noMaskParticles.GetComponent<ParticleSystemRenderer>().enabled = true;
            }
            else
            {
                NoMaskObjects.SetActive(true);
            }
        }
    }

    private void Update()
    {
        if (PlayerManager.Instance.MaskManager.EquippedMask is not MaskManager.MaskType.Enemy)
            return;

        var distToPlayer = Vector3.Distance(transform.position, _player.transform.position);

        if (_aggroCoroutine == null && distToPlayer <= AggroRange && Time.time >= _enemyActivatedTime + _startFiringDelay)
        {
            _aggroCoroutine = StartCoroutine(AggroCoroutine());
        }

        if (distToPlayer <= AggroRange)
        {
            var targetRot = Quaternion.LookRotation(transform.DirectionTo(_player.Controller.playerCamera));
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, RotateSpeed * Time.deltaTime);
        }
        else if (distToPlayer >= AggroCancelRange)
        {
            CancelAggro();
        }
    }

    private IEnumerator AggroCoroutine()
    {
        while (true)
        {
            //sound
            RuntimeManager.PlayOneShot(shoot, transform.position);

            // Debug.Log($"Fire!", this);
            FireShakeRotation?.StartShake();
            var projectile = Instantiate(ProjectilePrefab, ProjectileSpawnPoint.position, ProjectileSpawnPoint.rotation);
            projectile.Init(this);
            yield return new WaitForSeconds(FireRate);
        }
    }

    public void TakeDamage(int damage)
    {
        CameraShakeController.Instance.StartShake(DamagedScreenShakeSettings);

        RuntimeManager.PlayOneShot(enemyHit, transform.position);

        // Debug.Log($"Enemy hit!", this);
        Health -= damage;
        if (Health <= 0)
        {
            OnDied();
        }
    }

    private void OnDied()
    {
        if (DiedVFX)
        {
            DiedVFX.transform.parent = null;
            DiedVFX.Play();
        }

        CancelAggro();
        gameObject.SetActive(false);
        
        // todo: Respawn after set time
    }

    private void CancelAggro()
    {
        if (_aggroCoroutine != null)
        {
            StopCoroutine(_aggroCoroutine);
            _aggroCoroutine = null;
            FireShakeRotation?.StopShake();
        }
    }

    private void OnDrawGizmosSelected()
    {
        var startColor = Gizmos.color;
        Gizmos.color = Color.yellow;
        GizmosExtensions.DrawWireSphere(transform.position, AggroRange);
        Gizmos.color = startColor;
    }
}