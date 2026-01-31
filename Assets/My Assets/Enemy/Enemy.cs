using System;
using System.Collections;
using intheclouds;
using Sirenix.OdinInspector;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float AggroRange = 8f;
    public float AggroCancelRange = 12f;
    public float RotateSpeed = 100f;

    [Title("Projectiles")]
    public float FireRate = 1f;
    public float FireSpeed = 2f;
    public Transform ProjectileSpawnPoint;
    public Projectile ProjectilePrefab;

    [Title("Health")]
    public int Health = 2;
    public bool IsAlive = true;
    public ParticleSystem DiedVFX;

    private PlayerManager _player;
    private Coroutine _aggroCoroutine;


    private void Start()
    {
        _player = PlayerManager.Instance;
    }

    private void Update()
    {
        var distToPlayer = Vector3.Distance(transform.position, _player.transform.position);

        if (_aggroCoroutine == null && distToPlayer <= AggroRange)
        {
            _aggroCoroutine = StartCoroutine(AggroCoroutine());
        }
        else if (_aggroCoroutine != null)
        {
            var targetRot = Quaternion.LookRotation(transform.DirectionTo(_player.Controller.playerCamera));
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, RotateSpeed * Time.deltaTime);
            if (distToPlayer >= AggroCancelRange)
            {
                CancelAggro();
            }
        }
    }

    private IEnumerator AggroCoroutine()
    {
        while (true)
        {
            var projectile = Instantiate(ProjectilePrefab, ProjectileSpawnPoint.position, ProjectileSpawnPoint.rotation);
            projectile.Init(this);
            yield return new WaitForSeconds(FireRate);
        }
    }

    public void TakeDamage(int damage)
    {
        Debug.Log($"Enemy hit!", this);
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
    }

    private void CancelAggro()
    {
        StopCoroutine(_aggroCoroutine);
        _aggroCoroutine = null;
    }
}