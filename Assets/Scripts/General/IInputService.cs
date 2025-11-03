using UnityEngine;
using System;

namespace General
{
    public interface IInputService 
    {
        event Action<Vector2> OnMoveEvent;

        public event Action OnSprintStarted;
        public event Action OnSprintCanceled;
        
        event Action OnInteractEvent;
        event Action<ControlDevice> OnControlSchemeChange;
        //event Action<bool> OnControlStateChange;
        ControlDevice CurrentControlDevice { get; }
    }
}