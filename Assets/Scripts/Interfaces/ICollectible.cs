using PlayerScripts;
using UnityEngine;

namespace Interfaces
{
    public interface ICollectible
    {
        string CollectionPrompt { get; }
        void Collect(GameObject interactor, PlayerMovement playerMovement);
    }
}