using System;
using System.Collections;
using FMODUnity;
using intheclouds;
using UnityEngine;
using UnityEngine.InputSystem;

public class MaskManager : MonoBehaviour
{
    public enum MaskType
    {
        None,
        Enemy,
        Platforms,
        Pickups
    }

    [SerializeField] private StudioEventEmitter musicEmitter;
    [SerializeField] private GameObject actionMusicEmitterGo;
    

    private void SetmaskParameters(MaskType maskType)
    {
        float maskOnValue;
        float enemyMaskValue;


        switch (maskType)
        {
            case MaskType.None:
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
        
        SwapToMask(MaskType.None, true);
    }

    private void Update()
    {
        if (Keyboard.current.numpad0Key.wasPressedThisFrame)
        {
            SwapToMask(MaskType.None);
        }
        else if (Keyboard.current.numpad1Key.wasPressedThisFrame)
        {
            SwapToMask(MaskType.Enemy);
        }
        else if (Keyboard.current.numpad2Key.wasPressedThisFrame)
        {
            SwapToMask(MaskType.Platforms);
        }
        else if (Keyboard.current.numpad3Key.wasPressedThisFrame)
        {
            SwapToMask(MaskType.Pickups);
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
            if(actionMusicEmitterGo.activeSelf)
               actionMusicEmitterGo.SetActive(false);

        }
        


        SwappedMask?.Invoke(newMask);
    }
}
