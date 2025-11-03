
using PlayerScripts;
using UnityEngine;

namespace Interfaces
{
    public interface IInteractable
    {
        string InteractionPrompt { get; }
        void Interact(GameObject interactor, PlayerMovement playerMovement);
    }
}