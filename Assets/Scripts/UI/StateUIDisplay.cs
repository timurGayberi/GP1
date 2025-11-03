using UnityEngine;
using TMPro;
using PlayerScripts;
using Interfaces;

namespace UI
{
    public class StateUIDisplay : MonoBehaviour
    {
        #region Variables
        
        [Header("UI References")]
        [Tooltip("The TextMeshPro component to display the current state and prompt.")]
        [SerializeField]
        private TextMeshProUGUI displayText;
        
        private PlayerMovement _playerMovement;
        private PlayerInteraction _playerInteraction;
        private string _currentInteractionPrompt;

        #endregion

        private void Awake()
        {
            if (displayText == null)
            {
                Debug.LogError("StateUIDisplay requires a TextMeshProUGUI component assigned to 'displayText'.", this);
            }
            
            _playerMovement = FindObjectOfType<PlayerMovement>();
            _playerInteraction = FindObjectOfType<PlayerInteraction>();

            if (_playerMovement == null || _playerInteraction == null)
            {
                Debug.LogError("Could not find PlayerMovement or PlayerInteraction in the scene.", this);
            }
        }

        private void OnEnable()
        {
            if (_playerMovement != null)
            {
                _playerMovement.OnPlayerStateChange += HandlePlayerStateChange;
            }

            if (_playerInteraction != null)
            {
                _playerInteraction.OnInteractionPromptChange += HandleInteractionPromptChange;
            }
            
            if (_playerMovement != null)
            {
                HandlePlayerStateChange(_playerMovement.CurrentState);
            }
        }

        private void OnDisable()
        {
            if (_playerMovement != null)
            {
                _playerMovement.OnPlayerStateChange -= HandlePlayerStateChange;
            }
            if (_playerInteraction != null)
            {
                _playerInteraction.OnInteractionPromptChange -= HandleInteractionPromptChange;
            }
        }
        private void HandleInteractionPromptChange(string newPrompt)
        {
            _currentInteractionPrompt = newPrompt;
            
            HandlePlayerStateChange(_playerMovement.CurrentState);
        }
        private void HandlePlayerStateChange(PlayerState newState)
        {
            if (displayText == null) return;

            string stateText = $"State:{newState.ToString()}";
            string promptText = " ";
            
            if (!string.IsNullOrEmpty(_currentInteractionPrompt))
            {
                promptText = $"{_currentInteractionPrompt}";
            }
            
            displayText.text = stateText + promptText;
        }
    }
}