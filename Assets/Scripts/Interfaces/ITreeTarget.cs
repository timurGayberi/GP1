using UnityEngine;
using PlayerScripts;

namespace Interfaces
{
    public interface ITreeTarget : IInteractable
    {
        void Chop(GameObject interactor, PlayerMovement playerMovement);
    }
}