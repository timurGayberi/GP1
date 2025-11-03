using UnityEngine;
using UnityEngine.UI;
using System;
using General;

namespace UI
{
    public class ControllerDisplay : MonoBehaviour
    {
        [Header("Icons")]
        [SerializeField]
        private Image gamepadIcon,keyboardIcon;
        
        private IInputService _inputService;
        
        private void OnEnable()
        {
            try
            {
                _inputService = ServiceLocator.GetService<IInputService>();
            }
            catch (InvalidOperationException e)
            {
                Debug.LogError("IInputService not found. Cannot display controller status. Error: " + e.Message);
                return;
            }
            
            _inputService.OnControlSchemeChange += UpdateDisplay;
            UpdateDisplay(_inputService.CurrentControlDevice);
        }

        private void OnDisable()
        {
            if (_inputService != null)
            {
                _inputService.OnControlSchemeChange -= UpdateDisplay;
            }
        }
        private void UpdateDisplay(ControlDevice newDevice)
        {
            if (gamepadIcon == null || keyboardIcon == null)
            {
                Debug.LogError("ControllerDisplay icons are not assigned in the Inspector!");
                return;
            }

            switch (newDevice)
            {
                case ControlDevice.Gamepad:
                    gamepadIcon.enabled = true;
                    keyboardIcon.enabled = false;
                    break;
                    
                case ControlDevice.KeyboardMouse:
                    keyboardIcon.enabled = true;
                    gamepadIcon.enabled = false;
                    break;
                    
                case ControlDevice.Unknown:
                default:
                    gamepadIcon.enabled = false;
                    keyboardIcon.enabled = false;
                    break;
            }
        }
    }
}
