using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

namespace intheclouds
{
    public enum InputDevice
    {
        Gamepad,
        KeyboardMouse
    }

    public class InputManager : MonoBehaviour
    {
        public static InputManager Instance;

        [SerializeField]
        private float _inputsEnabledDelay = 1f;
        [SerializeField]
        private float _inputsEnabledDelayOnDeathRespawn = 1.75f;
        public bool WaitingToGivePlayerControl { get; set; } = true;

        [Title("Gamepad Settings")]
        public float MovementDeadzone = 0.1f;
        public float AimActiveThreshold = 0.6f;
        public float AimReleaseThreshold = 0.2f;
        public float AirControlSpeedDeadzone = 0.4f;
        public float AirControlTurnDeadzone = 0.1f;

        public bool PlayerInputsAllowed { get; private set; }

        public Vector2 Translation { get; private set; }
        public Vector2 Direction { get; private set; }
        public bool DashWasPressed { get; private set; }

        public bool AttackWasPressed { get; private set; }
        public bool AttackHeld { get; private set; }
        public bool PrevAttackHeld { get; private set; }
        public Vector2 PrevAttackDirection { get; private set; }
        public bool AttackWasReleased { get; private set; }
        public Vector2 AttackDirectionBeforeRelease { get; private set; }
        public bool CritSpecialWasPressed { get; private set; }
        public bool WhirlwindWasReleased { get; private set; }
        public bool WhirlwindHeld { get; private set; }
        public bool AirControlWasPressed { get; private set; }
        public bool AirControlWasReleased { get; private set; }
        public bool AirControlHeld { get; private set; }

        public bool InteractWasPressed { get; private set; }
        public bool GamepadEastButtonWasPressed { get; private set; }

        public bool PauseWasPressed { get; private set; }
        public bool RespawnWasPressed { get; private set; }
        public bool OpenInventoryWasPressed { get; private set; }
        public bool ActivateExpressionUpWasPressed { get; private set; }
        public bool ActivateExpressionDownWasPressed { get; private set; }
        public bool ActivateExpressionLeftWasPressed { get; private set; }
        public bool ActivateExpressionRightWasPressed { get; private set; }

        // Dev tools
        public bool ToggleGodModeWasPressed { get; private set; }
        public bool ToggleEnemyAIWasPressed { get; private set; }
        public bool ToggleMusicWasPressed { get; private set; }
        public bool TimeScaleUpWasPressed { get; private set; }
        public bool TimeScaleDownWasPressed { get; private set; }
        public bool TimeScaleResetWasPressed { get; private set; }

        public bool UsingGamepad { get; private set; }
        public event Action<InputDevice> SwappedInputDevice;
        private bool _gamepadWasUsed;
        private MyInputs _inputs;
        private Coroutine _vibrateCoroutine;


        private void Awake()
        {
            Instance = this;
            _inputs = new MyInputs();
            _inputs.Enable();
            SwappedInputDevice = null;
        }

        private IEnumerator Start()
        {
            yield return null;
            _gamepadWasUsed = Gamepad.current != null;
            SwappedInputDevice?.Invoke(_gamepadWasUsed ? InputDevice.Gamepad : InputDevice.KeyboardMouse);
        }

        private void OnDisable()
        {
            Cursor.visible = true;
            VibrateCancel(_vibrateCoroutine);
        }

        public void DelayGivePlayerControl(bool longDelay)
        {
            WaitingToGivePlayerControl = true;
            var delay = longDelay ? _inputsEnabledDelay : _inputsEnabledDelayOnDeathRespawn;
            StartCoroutine(GiveControlCoroutine(delay));
        }

        private IEnumerator GiveControlCoroutine(float delay)
        {
            yield return new WaitForSeconds(delay);

            WaitingToGivePlayerControl = false;

            if (!UIManager.Instance.IsAMenuOpen())
            {
                ToggleInputsAllowed(true);
            }
        }

        public void ToggleInputsAllowed(bool toggle)
        {
            if (toggle && (WaitingToGivePlayerControl || UIManager.Instance.IsAMenuOpen())) return;
            PlayerInputsAllowed = toggle;
        }

        private void Update()
        {
            if (IsGamepadInUse())
                UsingGamepad = true;
            else if (KeyboardInUse() || MouseInUse())
                UsingGamepad = false;

            if (!_gamepadWasUsed && UsingGamepad)
            {
                _gamepadWasUsed = true;
                Cursor.visible = false;
                SwappedInputDevice?.Invoke(InputDevice.Gamepad);
            }
            else if (_gamepadWasUsed && !UsingGamepad)
            {
                _gamepadWasUsed = false;
                Cursor.visible = true;
                SwappedInputDevice?.Invoke(InputDevice.KeyboardMouse);
            }

            // Shared actions
            Translation = _inputs.Player.Translation.ReadValue<Vector2>();
            PauseWasPressed = _inputs.Player.Pause.WasPerformedThisFrame();
            DashWasPressed = _inputs.Player.Dash.WasPerformedThisFrame();
            WhirlwindWasReleased = _inputs.Player.ActivateWhirlwind.WasReleasedThisFrame();
            WhirlwindHeld = _inputs.Player.ActivateWhirlwind.IsPressed();
            AirControlWasPressed = _inputs.Player.ActivateAirControl.WasPerformedThisFrame();
            AirControlWasReleased = _inputs.Player.ActivateAirControl.WasReleasedThisFrame();
            AirControlHeld = _inputs.Player.ActivateAirControl.IsPressed();
            CritSpecialWasPressed = _inputs.Player.CritSpecial.WasPerformedThisFrame();
            InteractWasPressed = _inputs.Player.Interact.WasPerformedThisFrame();
            OpenInventoryWasPressed = _inputs.Player.OpenInventory.WasPerformedThisFrame();
            ActivateExpressionUpWasPressed = _inputs.Player.ActivateExpressionUp.WasPerformedThisFrame();
            ActivateExpressionDownWasPressed = _inputs.Player.ActivateExpressionDown.WasPerformedThisFrame();
            ActivateExpressionLeftWasPressed = _inputs.Player.ActivateExpressionLeft.WasPerformedThisFrame();
            ActivateExpressionRightWasPressed = _inputs.Player.ActivateExpressionRight.WasPerformedThisFrame();

            if (UsingGamepad)
            {
                Direction = _inputs.Player.Direction.ReadValue<Vector2>();
                AttackHeld = Direction.magnitude >= AimActiveThreshold;
                AttackWasPressed = !PrevAttackHeld && AttackHeld;
                AttackWasReleased = PrevAttackHeld && Direction.magnitude <= AimReleaseThreshold;
                if (PrevAttackHeld)
                {
                    if (AttackHeld) PrevAttackDirection = Direction;
                    else AttackDirectionBeforeRelease = PrevAttackDirection;
                }

                RespawnWasPressed = InteractWasPressed;
                GamepadEastButtonWasPressed = _inputs.UI.Back.WasPerformedThisFrame();
            }
            else
            {
                AttackWasPressed = _inputs.Player.Attack.WasPerformedThisFrame();
                AttackHeld = _inputs.Player.Attack.IsPressed();
                AttackWasReleased = _inputs.Player.Attack.WasReleasedThisFrame();
                RespawnWasPressed = DashWasPressed || InteractWasPressed || AttackWasPressed;
            }

            PrevAttackHeld = AttackHeld;

            if ((!Application.isEditor && Debug.isDebugBuild) || (Application.isEditor && !GameManager.ReleaseMode))
            {
                ToggleGodModeWasPressed = _inputs.Player.ToggleGodMode.WasPerformedThisFrame();
                ToggleEnemyAIWasPressed = _inputs.Player.ToggleEnemyAI.WasPerformedThisFrame();
                ToggleMusicWasPressed = _inputs.Player.ToggleMusic.WasPerformedThisFrame();
                TimeScaleUpWasPressed = _inputs.Player.IncreaseTimeScale.WasPerformedThisFrame();
                TimeScaleDownWasPressed = _inputs.Player.DecreaseTimeScale.WasPerformedThisFrame();
                TimeScaleResetWasPressed = _inputs.Player.ResetTimeScale.WasPerformedThisFrame();
            }
        }

        public bool MouseInUse()
        {
            return Mouse.current.delta.magnitude > 0.1f || Mouse.current.leftButton.isPressed || Mouse.current.rightButton.isPressed;
        }

        public bool KeyboardInUse()
        {
            return Keyboard.current.wasUpdatedThisFrame;
        }

        public bool IsDirectionActive()
        {
            return Direction.magnitude >= AimActiveThreshold;
        }

        public bool IsMovementActive()
        {
            return Translation.magnitude >= MovementDeadzone;
        }

        public Vector3 GetTranslation()
        {
            Vector3 translation = new Vector3(Translation.x, 0f, Translation.y);
            if (translation.magnitude > MovementDeadzone)
                return translation.normalized * ((translation.magnitude - MovementDeadzone) / (1 - MovementDeadzone));
            return Vector3.zero;
        }
        
        public Vector2 GetAirControlInput()
        {
            if (UsingGamepad)
            {
                return new Vector2(GetAirControlXVal(), GetAirControlYVal());
            }

            return Translation;
        }
        
        public float GetAirControlXVal()
        {
            float xDirAbs = Mathf.Abs(Direction.x);
            if (xDirAbs > AirControlTurnDeadzone)
                return Mathf.Sign(Direction.x) * (xDirAbs - AirControlTurnDeadzone) / (1 - AirControlTurnDeadzone);
            return 0f;
        }
        
        public float GetAirControlYVal()
        {
            float yDirAbs = Mathf.Abs(Translation.y);
            if (yDirAbs > AirControlSpeedDeadzone)
                return Mathf.Sign(Translation.y) * (yDirAbs - AirControlSpeedDeadzone) / (1 - AirControlSpeedDeadzone);
            return 0f;
        }

        public bool AnyActiveActionInputs()
        {
            return !UIManager.Instance.IsAMenuOpen() && (IsMovementActive() || AttackHeld || InteractWasPressed || DashWasPressed);
        }

        /// <summary>
        /// </summary>
        /// <param name="lowFreq">0 to 1 intensity</param>
        /// <param name="highFreq">0 to 1 intensity</param>
        /// <returns>Coroutine that can be passed into VibrateCancel() to end it early</returns>
        public Coroutine Vibrate(float lowFreq, float highFreq, float duration)
        {
            var gamepad = Gamepad.current;
            if (gamepad == null || !UsingGamepad) return null;

            if (_vibrateCoroutine != null) StopCoroutine(_vibrateCoroutine);
            _vibrateCoroutine = StartCoroutine(VibrateCoroutine(gamepad, lowFreq, highFreq, duration));
            return _vibrateCoroutine;
        }

        private IEnumerator VibrateCoroutine(Gamepad gamepad, float low, float high, float time)
        {
            gamepad.SetMotorSpeeds(low, high);
            gamepad.ResumeHaptics();
            yield return new WaitForSeconds(time);
            gamepad.PauseHaptics();
            gamepad.SetMotorSpeeds(0, 0);
            _vibrateCoroutine = null;
        }

        public void VibrateCancel(Coroutine coroutine)
        {
            if (coroutine != null && _vibrateCoroutine == coroutine)
            {
                StopCoroutine(_vibrateCoroutine);
                _vibrateCoroutine = null;
                var gamepad = Gamepad.current;
                if (gamepad == null) return;
                gamepad.PauseHaptics();
                gamepad.SetMotorSpeeds(0, 0);
                // Debug.Log($"VIBRATE CANCELED");
            }
        }

        private bool IsGamepadInUse()
        {
            return Gamepad.current != null && (Gamepad.current.buttonNorth.wasPressedThisFrame ||
                                               Gamepad.current.buttonSouth.wasPressedThisFrame ||
                                               Gamepad.current.buttonWest.wasPressedThisFrame ||
                                               Gamepad.current.buttonEast.wasPressedThisFrame ||
                                               Gamepad.current.startButton.wasPressedThisFrame ||
                                               Gamepad.current.selectButton.wasPressedThisFrame ||
                                               Gamepad.current.dpad.ReadValue() != Vector2.zero ||
                                               Gamepad.current.leftTrigger.IsActuated() ||
                                               Gamepad.current.rightTrigger.IsActuated() ||
                                               Gamepad.current.leftStick.IsActuated() ||
                                               Gamepad.current.rightStick.IsActuated());
        }
    }
}