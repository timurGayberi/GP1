using UnityEngine;
using UnityEngine.InputSystem;
using System;

namespace General
{
    public enum ControlDevice
    {
        KeyboardMouse,
        Gamepad,
        Unknown 
    }
    public class InputReader : MonoBehaviour, IInputService, InputSystem_Actions.IPlayerActions 
    {
        // --- IInputService Contract Members ---
        public event Action<ControlDevice> OnControlSchemeChange;
        public ControlDevice CurrentControlDevice { get; private set; } = ControlDevice.Unknown;
        public event Action<Vector2> OnMoveEvent;
        public event Action OnSprintStarted;
        public event Action OnSprintCanceled;
        public event Action<Vector2> OnSprintEvent;

        public event Action OnInteractEvent;
        // -------------------------------------
        
        private InputSystem_Actions _inputsInstance;

        private void Awake()
        {
            _inputsInstance = new InputSystem_Actions();
            _inputsInstance.Player.SetCallbacks(this);
            
            try
            {
                ServiceLocator.RegisterService<IInputService>((IInputService)this);
            }
            catch (Exception e)
            {
                Debug.LogError("Failed to register IInputService: " + e.Message);
                Destroy(gameObject);
                return;
            }

            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            if (Gamepad.all.Count > 0)
            {
                CurrentControlDevice = ControlDevice.Gamepad;
            }
            else
            {
                CurrentControlDevice = ControlDevice.KeyboardMouse;
            }
            Debug.Log($"Initial Device State: {CurrentControlDevice}");
        }
        private void OnEnable()
        {
            _inputsInstance.Player.Enable();
            InputSystem.onDeviceChange += OnInputDeviceChange;
        }
        private void OnDisable()
        {
            _inputsInstance.Player.Disable();
            InputSystem.onDeviceChange -= OnInputDeviceChange;
        }
        private void OnInputDeviceChange(InputDevice device, InputDeviceChange change)
        {
            if (change == InputDeviceChange.Added || change == InputDeviceChange.Disconnected)
            {
                Debug.Log($"System device change: {device.displayName} was {change}. Next input will determine scheme.");
            }
        }
        
        #region Movement
        public void OnMove(InputAction.CallbackContext context)
        {
            if (context.started) 
            {
                CheckAndReportDevice(context);
            }
            
            OnMoveEvent?.Invoke(context.ReadValue<Vector2>());
        }

        public void OnSprint(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                CheckAndReportDevice(context);
                OnSprintStarted?.Invoke();
            }
            
            if (context.canceled)
            {
                OnSprintCanceled?.Invoke();
            }
            
        }
        
        #endregion
        public void OnInteract(InputAction.CallbackContext context)
        {
            if (context.started ||context.performed) 
            {
                OnInteractEvent?.Invoke();
                CheckAndReportDevice(context);
            }
        }
        private void CheckAndReportDevice(InputAction.CallbackContext context)
        {
            ControlDevice detectedDevice = GetDeviceFromControl(context.control);
            
            if (CurrentControlDevice != detectedDevice)
            {
                SetNewControlDevice(detectedDevice);
            }
        }
        private void SetNewControlDevice(ControlDevice newDevice)
        {
            CurrentControlDevice = newDevice;
            OnControlSchemeChange?.Invoke(CurrentControlDevice);
            Debug.Log($"Control scheme switched to: {CurrentControlDevice}");
        }
        private ControlDevice GetDeviceFromControl(InputControl control)
        {
            // Check for Gamepad
            if (control.device is Gamepad)
            {
                return ControlDevice.Gamepad;
            }
            // Check for Keyboard
            else if (control.device is Keyboard || control.device is Mouse)
            {
                return ControlDevice.KeyboardMouse;
            }
            return ControlDevice.Unknown;
        }
        
        #region ImplementInFuture?
        public void OnLook(InputAction.CallbackContext context) { }
        public void OnJump(InputAction.CallbackContext context) { }
        public void OnAttack(InputAction.CallbackContext context) { }
        public void OnPrevious(InputAction.CallbackContext context) { }
        public void OnNext(InputAction.CallbackContext context) { }
        
        #endregion
    }
}