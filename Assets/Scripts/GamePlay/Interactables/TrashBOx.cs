using Interfaces;
using UnityEngine;
using PlayerScripts;

namespace GamePlay.Interactables
{
    public class NewMonoBehaviourScript : MonoBehaviour , IInteractable
    {
        public string InteractionPrompt { get; } = "Trash box";

        public void Interact(GameObject interactor, PlayerMovement playerMovement)
        {
            Debug.Log($"Player ({interactor.name}) is interacting with the {InteractionPrompt}");
            //Destroy(gameObject);
        }
    }
}
