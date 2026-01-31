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
    public ParticleSystem DiedVFX;

    private Enemy _spawner;
    private Vector3 _prevPos;
    private float _distTraveled;
    private bool _deflected;


    public void Init(Enemy spawner)
    {
        _prevPos = transform.position;
        _spawner = spawner;
    }

    private void FixedUpdate()
    {
        if (_distTraveled >= MaxDistance)
        {
            DestroyProjectile();
            return;
        }

        transform.position += transform.forward * (_spawner.FireSpeed * Time.deltaTime);

        _distTraveled += Vector3.Distance(transform.position, _prevPos);
        _prevPos = transform.position;
    }

    public void Deflect()
    {
        Debug.Log($"Projectile deflected!", this);
        
        _deflected = true;
        transform.rotation = Quaternion.LookRotation(transform.DirectionTo(_spawner));
    }

    private void OnTriggerEnter(Collider other)
    {
        var playerHit = other.GetComponentInParent<PlayerManager>();
        var enemyHit = other.GetComponentInParent<Enemy>();
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

        DestroyProjectile();
    }

    private void DestroyProjectile()
    {
        if (DiedVFX)
        {
            DiedVFX.transform.parent = null;
            DiedVFX.Play();
        }
        
        Destroy(gameObject);
    }
}