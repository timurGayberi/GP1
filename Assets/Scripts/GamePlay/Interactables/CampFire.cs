using UnityEngine;
using Interfaces;
using PlayerScripts;
using System;
using GamePlay.Collectibles;

namespace GamePlay.Interactables
{
    public class FireplaceInteraction : MonoBehaviour, IInteractable
    {
        #region Variables
        
        [Header("Interaction Settings")]
        [Tooltip("The text prompt shown to the player when near the fireplace.")]
        [SerializeField]
        private string interactionPrompt = "Add Wood to Fireplace";
        public string InteractionPrompt => interactionPrompt;
        
        [Header("Fire Status")]
        [Tooltip("The maximum amount of fuel the campfire can hold.")]
        [SerializeField]
        private float maxFuel = 100f;
        [Tooltip("The rate at which fuel decays per second.")]
        [SerializeField]
        private float decayRate = 1f;
        
        [Header("Current Status")]
        [Tooltip("The current fuel level (displayed at runtime).")]
        [SerializeField]
        private float _currentFuel;
        
        /*
        [Header("Gameplay Effects (Currently Disabled)")]
        [Tooltip("Optional: Reference to a particle system or light to activate/improve when wood is added.")]
        [SerializeField]
        private GameObject fireVisuals;
        */
        
        #endregion
        
        //Public event
        public event Action<float, float> OnFuelChanged;
        
        private void Awake()
        {
            _currentFuel = maxFuel;
            OnFuelChanged?.Invoke(_currentFuel, maxFuel);
        }
        
        private void Update()
        {
            if (_currentFuel > 0)
            {
                _currentFuel -= decayRate * Time.deltaTime;
                _currentFuel = Mathf.Max(0, _currentFuel); 
                
                OnFuelChanged?.Invoke(_currentFuel, maxFuel);

                if (_currentFuel <= 0)
                {
                    Debug.Log("The campfire has gone out.");
                    // TODO: Add logic to turn off fire visuals/effects
                }
            }
        }
        
        public void Interact(GameObject interactor, PlayerMovement playerMovement)
        {
            PlayerInventory inventory = interactor.GetComponent<PlayerInventory>();

            if (inventory == null)
            {
                Debug.LogError("PlayerInventory component not found on interactor!");
                return;
            }

            if (inventory.HasWood)
            {
                float fuelToAdd = GetFuelFromCarriedLog(inventory);

                if (fuelToAdd > 0)
                {
                    _currentFuel += fuelToAdd;
                    _currentFuel = Mathf.Min(maxFuel, _currentFuel);
                    
                    inventory.ConsumeWood();
                    
                    OnFuelChanged?.Invoke(_currentFuel, maxFuel);

                    Debug.Log($"[CAMPFIRE] Added {fuelToAdd} fuel. Current Fuel: {_currentFuel:F1}/{maxFuel}.");
                }
                else
                {
                    Debug.LogWarning("[CAMPFIRE] Log carried has zero fuel value, or the log instance wasn't found.");
                }
            }
            else
            {
                Debug.Log("[CAMPFIRE] Interaction attempted, but player is not carrying wood.");
            }
        }
        
        private float GetFuelFromCarriedLog(PlayerInventory inventory)
        {
            GameObject carriedLog = inventory.GetCarriedWoodInstance(); 
            if (carriedLog == null) 
            {
                Debug.LogError("[FUEL GET] Carried wood instance is NULL.");
                return 0f;
            }
            
            FireWoodLogs logComponent = carriedLog.GetComponent<FireWoodLogs>(); 
    
            if (logComponent == null) 
            {
                Debug.LogError("[FUEL GET] Carried log is missing FireWoodLogs component.");
                return 0f;
            }
            
            return logComponent.FuelValue;
        }
    }
}
