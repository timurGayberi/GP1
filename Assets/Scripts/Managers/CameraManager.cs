using UnityEngine;
using General;
using Unity.Cinemachine;
using PlayerScripts;

namespace Managers
{
    [RequireComponent(typeof(CinemachineCamera))]
    public class CameraLockController : MonoBehaviour
    {
        private PlayerMovement _playerMovement;
        private CinemachineCamera _vCam;

        private CinemachineCamera VCam
        {
            get
            {
                if (_vCam == null)
                {
                    _vCam = GetComponent<CinemachineCamera>();
                }
                return _vCam;
            }
        }

        [Header("FOV Settings")]
        [SerializeField]
        private float interactionFOV = 25f;
        
        [Header("Zoom speed")]
        [SerializeField]
        private float zoomSpeed = 5f;

        private float   _defaultFOV,
                        _targetFOV;

        private void Awake()
        {
            if (VCam != null)
            {
                _defaultFOV = VCam.Lens.FieldOfView;
                _targetFOV = _defaultFOV;
            }
        }

        private void OnEnable()
        {

            _playerMovement = FindObjectOfType<PlayerMovement>();
            
            if (_playerMovement == null)
            {
                Debug.LogError("CameraLockController requires a PlayerMovement script instance in the scene.");
                return;
            }
            
            _playerMovement.OnPlayerStateChange += HandlePlayerStateChange;
            
            HandlePlayerStateChange(_playerMovement.CurrentState);
        }

        private void OnDisable()
        {
            if (_playerMovement != null)
            {
                _playerMovement.OnPlayerStateChange -= HandlePlayerStateChange;
            }
        }

        private void HandlePlayerStateChange(PlayerState newState)
        {
            if (newState == PlayerState.IsInteracting)
            {
                _targetFOV = interactionFOV;
                //Debug.Log("Camera Locked (Priority 0) for Interaction.");
            }
            else
            {
                _targetFOV = _defaultFOV;
            }
        }

        private void FixedUpdate()
        {
            if (VCam == null) return;

            VCam.Lens.FieldOfView = Mathf.Lerp
            (
                VCam.Lens.FieldOfView,
                _targetFOV,
                Time.fixedDeltaTime * zoomSpeed
            );
        }
    }
}
