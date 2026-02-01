using System;
using intheclouds;
using UnityEngine;
using UnityEngine.Serialization;

public class Projectile : MonoBehaviour
{
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
    }

    public void Init(Enemy spawner)
    {
        _prevPos = transform.position;
        _spawner = spawner;
        _speed = spawner.FireSpeed;
    }

    private void FixedUpdate()
    {
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
        Debug.Log($"Projectile deflected!", this);
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
        bool destroy = false;
        var playerHit = other.GetComponentInParent<PlayerManager>();
        var enemyHit = other.GetComponentInParent<Enemy>();
        var projectileHit = other.GetComponentInParent<Projectile>();
        if (playerHit)
        {
            var hitDir = other.transform.position - transform.position;
            hitDir.y = 0f;
            playerHit.TakeDamage(this, hitDir.normalized);
            destroy = true;
        }
        else if (enemyHit)
        {
            enemyHit.TakeDamage(Damage);
            destroy = true;
        }
        else if (projectileHit && _playerDeflected)
        {
            projectileHit.DeflectToSpawner();
            destroy = true;
        }

        if (destroy)
        {
            DestroyProjectile();
        }
    }

    private void DestroyProjectile()
    {
        Debug.Log($"Destroy projectile", this);

        if (DiedVFX)
        {
            DiedVFX.transform.parent = null;
            DiedVFX.Play();
        }

        Destroy(gameObject);
    }
}