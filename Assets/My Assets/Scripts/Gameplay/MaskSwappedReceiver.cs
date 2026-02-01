using System;
using intheclouds;
using UnityEngine;

public class MaskSwappedReceiver : MonoBehaviour
{
    [SerializeField]
    private MaskManager.MaskType _visibleMaskLayers;

    
    private void Start()
    {
        PlayerManager.Instance.MaskManager.SwappedMask += OnMaskSwapped;
    }

    private void OnMaskSwapped(MaskManager.MaskType newMask)
    {
        if (_visibleMaskLayers.HasFlag(newMask))
        {
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
