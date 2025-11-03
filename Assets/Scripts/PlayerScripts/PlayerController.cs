using UnityEngine;
using System;
using General; 
using ScriptableObjects;

namespace PlayerScripts
{
    public enum PlayerState
    {
        IsIdle,
        IsWalking,
        IsSprinting,
        IsInteracting // Only for active actions (like chopping, opening chests, etc.)
        // REMOVED: IsCarrying (Now handled passively by inventory check)
    }
    
    public class PlayerMovement : MonoBehaviour
    {
        #region References & Data
        
        [Header("Players Movement Data")] 
        [SerializeField]
        private PlayerStatsSo movementData;
        
        private CharacterController _characterController;
        private Vector3 _currentMoveDirection;
        private bool _isSprinting;
        private IInputService _inputService; 
        private PlayerInventory _playerInventory; 
        
        #endregion
        
        //  ------------------Actions-------------------------//
        
        public event Action OnInteractionAttempt; 
        public event Action <PlayerState> OnPlayerStateChange;
        
        // --------------------------------------------------//
        
        public PlayerState CurrentState { get; private set; } = PlayerState.IsIdle;
        
        private void Awake()
        {
            _characterController = GetComponent<CharacterController>();
            _playerInventory = GetComponent<PlayerInventory>();
            
            if (_characterController == null)
            {
                Debug.LogError("PlayerMovement requires a CharacterController component.");
            }
            if (movementData == null)
            {
                Debug.LogError("MovementData (PlayerStatsSo) is not assigned to PlayerMovement.");
            }
            if (_playerInventory == null)
            {
                Debug.LogError("PlayerMovement requires a PlayerInventory component on the same GameObject.");
            }
        }
        
        private void OnEnable()
        {
            try
            {
                _inputService = ServiceLocator.GetService<IInputService>();
                _inputService.OnMoveEvent += HandleMoveInput;
                _inputService.OnSprintStarted += HandleSprintStarted;
                _inputService.OnSprintCanceled += HandleSprintCanceled;
                
                OnPlayerStateChange?.Invoke(CurrentState); 
            }
            catch (InvalidOperationException e)
            {
                Debug.LogError("IInputService not found. Is InputReader registered in Awake? Error: " + e.Message);
            }
        }

        private void OnDisable()
        {
            if (_inputService != null)
            {
                _inputService.OnMoveEvent -= HandleMoveInput;
                _inputService.OnSprintStarted -= HandleSprintStarted;
                _inputService.OnSprintCanceled -= HandleSprintCanceled;
            }
        }

        public void SetPlayerState(PlayerState newState)
        {
            if (CurrentState != newState)
            {
                CurrentState = newState;
                OnPlayerStateChange?.Invoke(CurrentState); 
            }
        }

        private void HandleMoveInput(Vector2 inputVector)
        {
            // Only update direction if not currently locked in an interaction
            if (CurrentState != PlayerState.IsInteracting) 
            {
                _currentMoveDirection = new Vector3(inputVector.x, 0f, inputVector.y).normalized;
            }
        }

        private void HandleSprintStarted()
        {
            // Sprint is only allowed if the player is NOT carrying a burden AND NOT interacting.
            if (!_playerInventory.IsCarryingBurden && CurrentState != PlayerState.IsInteracting)
            {
                _isSprinting = true;
            }
        }

        private void HandleSprintCanceled()
        {
            _isSprinting = false;
        }
        
        private void Update()
        {
            if (_characterController == null || movementData == null || _playerInventory == null) return;
            
            var moveDirection = _currentMoveDirection;
            float currentSpeed;
            
            // -----------------------------------------------------------------
            // --- CORE FIX: BLOCK ALL MOVEMENT/STATE UPDATES IF INTERACTING ---
            // -----------------------------------------------------------------
            if (CurrentState == PlayerState.IsInteracting)
            {
                // Lock player rotation and movement while chopping/interacting
                _characterController.Move(Vector3.zero);
                return; // Exit Update loop immediately
            }
            
            // -----------------------------------------------------------------
            // --- MOVEMENT AND STATE LOGIC (Only runs if NOT IsInteracting) ---
            // -----------------------------------------------------------------

            if (moveDirection.sqrMagnitude > 0.01f) // Player is moving
            {
                // CARRYING CHECK (Passive State: affects speed, not enum state)
                if (_playerInventory.IsCarryingBurden)
                {
                    currentSpeed = movementData.movementSpeed / 2f; 
                    // State remains IsWalking if moving while carrying
                    if (CurrentState != PlayerState.IsWalking) SetPlayerState(PlayerState.IsWalking); 
                    _isSprinting = false; // Cannot sprint while carrying
                }
                // SPRINTING CHECK
                else if (_isSprinting)
                {
                    currentSpeed = movementData.sprintSpeed;
                    if (CurrentState != PlayerState.IsSprinting) SetPlayerState(PlayerState.IsSprinting);
                }
                // WALKING CHECK
                else 
                {
                    currentSpeed = movementData.movementSpeed;
                    if (CurrentState != PlayerState.IsWalking) SetPlayerState(PlayerState.IsWalking); 
                }
                
                // EXECUTE MOVEMENT
                _characterController.Move(moveDirection * (currentSpeed * Time.deltaTime));
                
                // HANDLE ROTATION
                var camForward = Camera.main.transform.forward;
                camForward.y = 0f;
                camForward.Normalize();
                
                var camRotation = Quaternion.LookRotation(camForward);
                var finalMoveDirection = camRotation * moveDirection;
                var targetRotation = Quaternion.LookRotation(finalMoveDirection);
                
                transform.rotation = Quaternion.Slerp(
                    transform.rotation, 
                    targetRotation, 
                    Time.deltaTime * 10f);
            }
            // --- IDLE CHECK ---
            else // Player is NOT moving
            {
                // If player is not moving and not interacting (checked above), set to Idle.
                if (CurrentState != PlayerState.IsIdle)
                {
                    SetPlayerState(PlayerState.IsIdle);
                }
            }
        }
    }
}
