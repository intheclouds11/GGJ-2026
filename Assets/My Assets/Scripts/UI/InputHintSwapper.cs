using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace intheclouds
{
    // todo: add Sony icons support
    public class InputHintSwapper : MonoBehaviour
    {
        [FormerlySerializedAs("_xboxGamepadObjects")]
        [FormerlySerializedAs("_gamepadObjects")]
        [FormerlySerializedAs("_gamepadCanvasGroups")]
        [SerializeField]
        private List<GameObject> _xboxGamepadIcons;
        [SerializeField]
        private List<GameObject> _sonyGamepadIcons;
        [FormerlySerializedAs("_kbmObjects")]
        [FormerlySerializedAs("_kbmCanvasGroups")]
        [SerializeField]
        private List<GameObject> _kbmIcons;


        private void Start()
        {
            InputManager.Instance.SwappedInputDevice += OnSwappedInputDevice;
            
            // Set initial iconType to use
            foreach (var gamepadIcon in _xboxGamepadIcons)
            {
                gamepadIcon.SetActive(InputManager.Instance.UsingGamepad);
            }
            foreach (var kbmIcon in _kbmIcons)
            {
                kbmIcon.SetActive(!InputManager.Instance.UsingGamepad);
            }
        }

        private void OnDestroy()
        {
            InputManager.Instance.SwappedInputDevice -= OnSwappedInputDevice;
        }

        private void OnSwappedInputDevice(InputDevice device)
        {
            // Debug.Log($"Swapping to {device}");
            if (device == InputDevice.Gamepad)
            {
                foreach (var gamepadIcon in _xboxGamepadIcons)
                {
                    gamepadIcon.SetActive(true);
                }

                foreach (var kbmIcon in _kbmIcons)
                {
                    kbmIcon.SetActive(false);
                }
            }
            else if (device == InputDevice.KeyboardMouse)
            {
                foreach (var gamepadIcon in _xboxGamepadIcons)
                {
                    gamepadIcon.SetActive(false);
                }

                foreach (var kbmIcon in _kbmIcons)
                {
                    kbmIcon.SetActive(true);
                }
            }
            else
            {
                Debug.LogError($"[InputTextSwitcher] Swapping not implemented for {device}.");
            }
        }
    }
}