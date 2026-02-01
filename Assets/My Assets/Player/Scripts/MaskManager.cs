using System;
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
    
    public MaskType EquippedMask { get; private set; }
    public event Action<MaskType> SwappedMask;


    private void Start()
    {
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
        PlayerManager.Instance.MaskColorFilterSwapper.OnMaskSwapped(newMask);
        
        SwappedMask?.Invoke(newMask);
    }
}
