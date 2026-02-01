using System;
using System.Collections;
using FMODUnity;
using intheclouds;
using UnityEngine;
using UnityEngine.InputSystem;

public class MaskManager : MonoBehaviour
{
    [Flags]
    public enum MaskType
    {
        NoMask = 1 << 0,
        Enemy = 1 << 1,
        Platforms = 1 << 2,
        Pickups = 1 << 3
    }

    //sound
    [Header("FMOD Player SFX")]
    [SerializeField] private EventReference maskSwitch;

    [SerializeField] private StudioEventEmitter musicEmitter;
    [SerializeField] private GameObject actionMusicEmitterGo;


    private void SetmaskParameters(MaskType maskType)
    {
        float maskOnValue;
        float enemyMaskValue;


        switch (maskType)
        {
            case MaskType.NoMask:
                maskOnValue = 0f;
                enemyMaskValue = 0f;
                break;

            case MaskType.Platforms:
                maskOnValue = 1f;
                enemyMaskValue = 0f;
                break;

            case MaskType.Enemy:
                maskOnValue = 1f;
                enemyMaskValue = 1f;
                break;

            case MaskType.Pickups:
                maskOnValue = 1f;
                enemyMaskValue = 2f;
                break;

            default:
                maskOnValue = 0f;
                enemyMaskValue = 0f;
                break;
        }

        musicEmitter.EventInstance.setParameterByName("MaskON", maskOnValue);
        musicEmitter.EventInstance.setParameterByName("EnemyMask", enemyMaskValue);
    }


    public MaskType EquippedMask { get; private set; }
    public event Action<MaskType> SwappedMask;


    private IEnumerator Start()
    {
        yield return null;

        if (!musicEmitter.IsPlaying())
            musicEmitter.Play();

        SwapToMask(MaskType.NoMask, true);
    }

    private void Update()
    {
        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            if (EquippedMask is MaskType.NoMask) SwapToMask(MaskType.Enemy);
            else if (EquippedMask is MaskType.Enemy) SwapToMask(MaskType.Platforms);
            else if (EquippedMask is MaskType.Platforms) SwapToMask(MaskType.Pickups);
            else if (EquippedMask is MaskType.Pickups) SwapToMask(MaskType.NoMask);
        }
        else if (Keyboard.current.qKey.wasPressedThisFrame)
        {
            if (EquippedMask is MaskType.NoMask) SwapToMask(MaskType.Pickups);
            else if (EquippedMask is MaskType.Enemy) SwapToMask(MaskType.NoMask);
            else if (EquippedMask is MaskType.Platforms) SwapToMask(MaskType.Enemy);
            else if (EquippedMask is MaskType.Pickups) SwapToMask(MaskType.Platforms);
        }
    }

    private void SwapToMask(MaskType newMask, bool force = false)
    {
        if (!force && EquippedMask == newMask)
        {
            Debug.LogWarning($"[MaskManager] EquippedMask is already {newMask}");
            return;
        }

        

        EquippedMask = newMask;

        SetmaskParameters(newMask);

        if (newMask == MaskType.Pickups)
        {
            if (!actionMusicEmitterGo.activeSelf)
                actionMusicEmitterGo.SetActive(true);
        }
        else
        {
            if (actionMusicEmitterGo.activeSelf)
                actionMusicEmitterGo.SetActive(false);
        }


        SwappedMask?.Invoke(newMask);
    }
}