using System;
using intheclouds;
using UnityEngine;
using UnityEngine.Serialization;

public class Projectile : MonoBehaviour
{
    public GameObject NoMaskObjects;
    private ParticleSystem _noMaskParticles;
    public GameObject ProjectileObjects;
    public GameObject PlatformObjects;
    public int Damage = 1;
    public float MaxDistance = 30f;
    public float HitForce = 2f;
    public float HitStunDuration = 1f;
    public float DeflectedSpeedMultiplier = 1.25f;
    public ParticleSystem DiedVFX;

    private Enemy _spawner;
    private float _speed;
    private Vector3 _prevPos;
    private float _distTraveled;
    private bool _deflected;
    private bool _playerDeflected;
    private MeshRenderer _mr;


    private void Awake()
    {
        _mr = GetComponentInChildren<MeshRenderer>();
        if (!ProjectileObjects)
            Debug.LogWarning($"Missing ProjectileObjects", this);
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
    }

    private void Start()
    {
        PlayerManager.Instance.MaskManager.SwappedMask += OnMaskSwapped;
        OnMaskSwapped(PlayerManager.Instance.MaskManager.EquippedMask);
    }

    private void OnDisable()
    {
        PlayerManager.Instance.MaskManager.SwappedMask -= OnMaskSwapped;
    }

    private void OnMaskSwapped(MaskManager.MaskType newMask)
    {
        if (newMask is MaskManager.MaskType.None)
        {
            ProjectileObjects.SetActive(false);
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
            ProjectileObjects.SetActive(true);
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
            ProjectileObjects.SetActive(false);
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
            ProjectileObjects.SetActive(false);
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
    }

    public void Init(Enemy spawner)
    {
        _prevPos = transform.position;
        _spawner = spawner;
        _speed = spawner.FireSpeed;
    }

    private void FixedUpdate()
    {
        if (PlayerManager.Instance.MaskManager.EquippedMask is not MaskManager.MaskType.Enemy)
            return;
        
        if (_distTraveled >= MaxDistance)
        {
            DestroyProjectile();
            return;
        }

        transform.position += transform.forward * (_speed * Time.deltaTime);

        _distTraveled += Vector3.Distance(transform.position, _prevPos);
        _prevPos = transform.position;
    }

    public void DeflectToSpawner(bool playerDeflected = false)
    {
        _playerDeflected = playerDeflected;
        DeflectWithDirection(transform.DirectionTo(_spawner));
    }

    public void DeflectWithDirection(Vector3 dir)
    {
        // Debug.Log($"Projectile deflected!", this);
        if (!_deflected)
        {
            _deflected = true;
            _speed *= DeflectedSpeedMultiplier;
            _mr.material.color = new Color(1f, 1f, 1f);
        }

        transform.rotation = Quaternion.LookRotation(dir);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!enabled) return;

        var playerHit = other.GetComponentInParent<PlayerManager>();
        var enemyHit = other.GetComponentInParent<Enemy>();
        var projectileHit = other.GetComponentInParent<Projectile>();
        if (playerHit)
        {
            var hitDir = other.transform.position - transform.position;
            hitDir.y = 0f;
            playerHit.TakeDamage(this, hitDir.normalized);
        }
        else if (enemyHit)
        {
            enemyHit.TakeDamage(Damage);
        }
        else if (projectileHit && _playerDeflected)
        {
            projectileHit.DeflectToSpawner();
        }

        DestroyProjectile();
    }

    private void DestroyProjectile()
    {
        // Debug.Log($"Destroy projectile", this);

        if (DiedVFX)
        {
            DiedVFX.transform.parent = null;
            DiedVFX.Play();
        }

        Destroy(gameObject);
    }
}