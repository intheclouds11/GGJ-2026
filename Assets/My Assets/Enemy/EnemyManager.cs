using System;
using System.Collections.Generic;
using System.Linq;
using intheclouds;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public List<Enemy> SceneEnemies { get; private set; }
    
    
    private void Start()
    {
        SceneEnemies = FindObjectsByType<Enemy>(FindObjectsInactive.Exclude, FindObjectsSortMode.None).ToList();
        PlayerManager.Instance.MaskManager.SwappedMask += OnMaskSwapped;
    }

    private void OnDisable()
    {
        PlayerManager.Instance.MaskManager.SwappedMask -= OnMaskSwapped;
    }

    public static void OnMaskSwapped(MaskManager.MaskType newMask)
    {
        if (newMask is MaskManager.MaskType.NoMask)
        {
        }
        else if (newMask is MaskManager.MaskType.Enemy)
        {
        }
        else if (newMask is MaskManager.MaskType.Platforms)
        {
        }
        else if (newMask is MaskManager.MaskType.Pickups)
        {
        }
    }
}
