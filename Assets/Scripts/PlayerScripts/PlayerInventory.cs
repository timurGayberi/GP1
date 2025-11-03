using UnityEngine;
using System;
using GamePlay.Collectibles;

namespace PlayerScripts
{
    public enum CarryingType
    {
        None, // empty handed
        
        //Tools
        Axe,
        Lantern,
        
        // Resources
        Wood,
        
        //Will come in future?
        Mushrooms,
        Trash
        
    }
    
    public class PlayerInventory : MonoBehaviour
    {
        #region Variables
        
        [Header("Resources")] 
        [Tooltip("True if the player is currently carrying wood.")] // Change 1: Revert tooltip
        [SerializeField]
        private bool hasWood = false;
        
        /*[Tooltip("The count of trash items the player is carrying.")]
        [SerializeField] 
        private int trashCount = 0;*/
        
        [Header("Tools")]
        [Tooltip("True if the player is currently carrying the axe.")]
        [SerializeField]
        private bool hasAxe = false;

        [Tooltip("True if the player is currently holding the lantern.")]
        [SerializeField]
        private bool hasLantern = false;
        
        [Header("Scene References (SINGLETONS)")]
        [Tooltip("The single Axe Collectible instance placed in the scene.")]
        [SerializeField]
        private GameObject sceneCollectibleAxe;

        [Tooltip("The single Lantern Collectible instance placed in the scene.")]
        [SerializeField]
        private GameObject sceneCollectibleLantern;
        
        [Header("Scene References (DROP PREFAB)")]
        [Tooltip("The Log Prefab that is instantiated when the player drops wood.")]
        [SerializeField]
        private GameObject woodLogPrefabForDropping;
        
        private GameObject _carriedWoodInstance;
        
        [Header("Visual Attachments (SCENE INSTANCES)")]
        [Tooltip("The AXE VISUAL GameObject (child of player) to turn on/off.")]
        [SerializeField]
        private GameObject heldAxeVisual; 
        
        [Tooltip("The GameObject representing the lantern visual, usually a child of the player.")]
        [SerializeField]
        private GameObject lanternVisual; 
        
        [Tooltip("The GameObject representing the wood log visual, usually a child of the player.")]
        [SerializeField]
        private GameObject woodVisual; 
        
        // State Properties
        public bool HasAxe => hasAxe; 
        public bool HasLantern => hasLantern;
        public bool HasWood => hasWood;
        
        public bool IsCarryingBurden => hasWood || hasAxe || hasLantern;
        
        #endregion
        
        // Sub-state indicator for animation/UI systems
        private CarryingType CurrentCarryingType { get; set; } = CarryingType.None;

        // Events
        public event Action<bool> OnHasAxeChanged;
        public event Action<int> OnWoodCountChanged;
        public event Action<CarryingType> OnCarryingTypeChanged; 

        private void Awake()
        {
            // Initial setup of tool visibility
            if (heldAxeVisual != null) heldAxeVisual.SetActive(hasAxe);
            if (lanternVisual != null) lanternVisual.SetActive(hasLantern);
            
            // Initial setup of wood visual based on count
            if(woodVisual != null) woodVisual.SetActive(hasWood);

            // Initial setup of collectible object visibility in the scene
            if (sceneCollectibleAxe != null) sceneCollectibleAxe.SetActive(!hasAxe);
            if(sceneCollectibleLantern != null) sceneCollectibleLantern.SetActive(!hasLantern);
        }

        private void Start()
        {
            UpdateCarryingType(); 
        }
        
        private void UpdateCarryingType()
        {
            CarryingType newType = CarryingType.None;
            
            // Priority: Tools first, then resources.
            if (hasAxe)
            {
                newType = CarryingType.Axe;
            }
            else if (hasLantern)
            {
                newType = CarryingType.Lantern;
            }
            else if (hasWood)
            {
                newType = CarryingType.Wood;
            }
            /*else if (hasTrash)
            {
                newType = CarryingType.Trash;
            }*/
            
            if (CurrentCarryingType != newType)
            {
                CurrentCarryingType = newType;
                OnCarryingTypeChanged?.Invoke(CurrentCarryingType);
                Debug.Log($"Inventory: Primary Carrying Type is now {CurrentCarryingType}.");
            }
        }

        private bool TryDropCurrentTool()
        {
            Vector3 dropPosition = transform.position + transform.forward * 1f;

            if (hasAxe)
            {
                DropAxe(dropPosition);
                return true;
            }
            if (hasLantern)
            {
                DropLantern(dropPosition);
                return true;
            }
            if (hasWood)
            {
                DropWood(dropPosition);
                return true;
            }
            
            return false;
        }
        
        //Tools

        #region  Axe_Logic
        
        // --- AXE LOGIC ---
        public void SetHasAxe(bool state)
        {
            if (hasAxe != state)
            {
                // ENFORCE SINGLE CARRY: If picking up the axe, drop the current tool first.
                if (state == true)
                {
                    TryDropCurrentTool(); // Drop tool, but don't drop resources (wood/trash)
                }

                hasAxe = state;
                OnHasAxeChanged?.Invoke(hasAxe);
                
                if (heldAxeVisual != null) heldAxeVisual.SetActive(hasAxe);
                // The scene collectible only appears when the player is NOT carrying it.
                if (sceneCollectibleAxe != null) sceneCollectibleAxe.SetActive(!hasAxe);

                UpdateCarryingType();
            }
        }
        
        public void DropAxe(Vector3 dropPosition)
        {
            if (hasAxe && sceneCollectibleAxe != null)
            {
                sceneCollectibleAxe.transform.position = dropPosition;
                sceneCollectibleAxe.SetActive(true); // Make the collectible visible
                SetHasAxe(false);
                Debug.Log("Axe dropped successfully.");
            }
            else if (sceneCollectibleAxe == null)
            {
                Debug.LogError("FATAL DROP ERROR: The Scene Collectible Axe is not assigned!");
            }
        }
        
        #endregion
        
        #region Lantern_Logic
        
        //lantern logic
        
        public void SetHasLantern(bool state)
        {
            if (hasLantern != state)
            {
                if (state == true)
                {
                    TryDropCurrentTool();
                }
                
                hasLantern = state;

                if (lanternVisual != null) lanternVisual.SetActive(hasLantern);
                if (sceneCollectibleLantern != null) sceneCollectibleLantern.SetActive(!hasLantern);
                
                UpdateCarryingType();
            }
        }
        
        public void DropLantern(Vector3 dropPosition)
        {
            if (hasLantern && sceneCollectibleLantern != null)
            {
                sceneCollectibleLantern.transform.position = dropPosition;
                sceneCollectibleLantern.SetActive(true);
                SetHasLantern(false); 
                Debug.Log("Lantern dropped successfully.");
            }
            else if (sceneCollectibleLantern == null)
            {
                Debug.LogError("FATAL DROP ERROR: The Scene Collectible Lantern is not assigned!");
            }
        }
        
        #endregion
        
        //Resources
        
        #region Wood_Logic
        
        //Wood logic
        
        public void SetHasWood(bool state, GameObject collectedInstance = null) 
        {
            if (hasWood != state)
            {
                if (state == true)
                {
                    TryDropCurrentTool();

                    // CRITICAL: Store the specific instance being picked up and hide it
                    if (collectedInstance != null)
                    {
                        _carriedWoodInstance = collectedInstance;
                        _carriedWoodInstance.SetActive(false); // Hide it from the world
                    }
                }
                else
                {
                    // CRITICAL: Clear the instance when dropped or consumed
                    _carriedWoodInstance = null;
                }

                hasWood = state;
                
                if (woodVisual != null) woodVisual.SetActive(hasWood);

                UpdateCarryingType();
                Debug.Log($"Wood state changed to: {hasWood}.");
            }
        }
        public void DropWood(Vector3 dropPosition) 
        {
            if (hasWood)
            {
                // Priority 1: Drop the actual carried instance if it exists
                if (_carriedWoodInstance != null)
                {
                    _carriedWoodInstance.transform.position = dropPosition;
                    _carriedWoodInstance.SetActive(true);
                    SetHasWood(false); // Clears the carried instance and updates state
                    Debug.Log("Wood dropped successfully (original instance returned).");
                    return;
                }
                
                // Fallback: Instantiate a new log if the instance was somehow lost
                if (woodLogPrefabForDropping == null)
                {
                    Debug.LogError("FATAL DROP ERROR: Wood Log Prefab For Dropping is not assigned!");
                    return;
                }
                
                Instantiate(woodLogPrefabForDropping, dropPosition, Quaternion.identity);
                SetHasWood(false);
                Debug.Log("Wood dropped successfully (instantiated new collectible).");
            }
        }
        
        public void ConsumeWood() 
        {
            if (hasWood && _carriedWoodInstance != null)
            {
                Destroy(_carriedWoodInstance);
                
                SetHasWood(false);
                
                Debug.Log("[INVENTORY] Successfully consumed and destroyed carried wood instance.");
            }
            else
            {
                Debug.LogWarning("[INVENTORY] Attempted to consume wood, but no trackable instance was carried.");
            }
        }
        public GameObject GetCarriedWoodInstance() 
        {
            return _carriedWoodInstance;
        }
        
        #endregion
        
        //Trash logic to come...
        //Mushroom logic to come ...
        
        private void TryDropCurrentBurden()
        {
            Debug.LogWarning("TryDropCurrentBurden is deprecated. Use TryDropCurrentTool() instead.");
        }
    }
}
