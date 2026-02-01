using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class MaskColorFilterSwapper : MonoBehaviour
{
    [SerializeField]
    private float _noMaskVignette = 0.25f;
    [SerializeField]
    private float _maskEquippedVignetteIntensity = 0.5f;
    [SerializeField]
    private Color _noMaskColor;
    [SerializeField]
    private Color _enemyMaskColor;
    [SerializeField]
    private Color _platformsMaskColor;
    [SerializeField]
    private Color _pickupsMaskColor;

    private Volume _globalVolume;
    private ColorAdjustments _colorAdjustments;
    private Vignette _vignette;


    private void Awake()
    {
        var volumes = FindObjectsByType<Volume>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        foreach (var volume in volumes)
        {
            if (volume.isGlobal)
            {
                _globalVolume = volume;
                break;
            }
        }

        _globalVolume.profile.TryGet(out _colorAdjustments);
        _globalVolume.profile.TryGet(out _vignette);

        OnNoMaskEquipped();
    }

    public void OnMaskSwapped(MaskManager.MaskType newMask)
    {
        if (newMask is MaskManager.MaskType.None)
        {
            OnNoMaskEquipped();
        }
        else if (newMask is MaskManager.MaskType.Enemy)
        {
            OnEnemyMaskEquipped();
        }
        else if (newMask is MaskManager.MaskType.Platforms)
        {
            OnPlatformMaskEquipped();
        }
        else if (newMask is MaskManager.MaskType.Pickups)
        {
            OnPickupsMaskEquipped();
        }
    }


    public void OnNoMaskEquipped()
    {
        _vignette.intensity.value = _noMaskVignette;
        _colorAdjustments.colorFilter.value = _noMaskColor;
    }

    public void OnEnemyMaskEquipped()
    {
        _vignette.intensity.value = _maskEquippedVignetteIntensity;
        _colorAdjustments.colorFilter.value = _enemyMaskColor;
    }

    public void OnPlatformMaskEquipped()
    {
        _vignette.intensity.value = _maskEquippedVignetteIntensity;
        _colorAdjustments.colorFilter.value = _platformsMaskColor;
    }

    public void OnPickupsMaskEquipped()
    {
        _vignette.intensity.value = _maskEquippedVignetteIntensity;
        _colorAdjustments.colorFilter.value = _pickupsMaskColor;
    }
}