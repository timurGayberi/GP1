using GamePlay.Interactables;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class FireplaceUI : MonoBehaviour
    {
        [Header("References")]
        [Tooltip("Drag the FireplaceInteraction component here from your scene object.")]
        [SerializeField]
        private FireplaceInteraction fireplace; 

        [Tooltip("Drag the Unity UI Slider component here.")]
        [SerializeField]
        private Slider fuelSlider;

        private void OnEnable()
        {
            if (fireplace != null)
            {
                fireplace.OnFuelChanged += UpdateFuelUI;
            }
        }

        private void OnDisable()
        {
            if (fireplace != null)
            {
                fireplace.OnFuelChanged -= UpdateFuelUI;
            }
        }

        private void UpdateFuelUI(float currentFuel, float maxFuel)
        {
            if (fuelSlider != null)
            {
                if (fuelSlider.maxValue != maxFuel)
                {
                    fuelSlider.maxValue = maxFuel;
                }
                
                fuelSlider.value = currentFuel;
            }
        }
    }
}