using System;
using Clickbait.Utilities;
using UnityEngine;

public interface ICharacterInput
{
    // Movement
    Vector2 Move { get; }
    Vector2 Look { get; }
    bool Jump { get; }
    bool Sprint { get; }
    
    // Shooting
    Observer<bool> IsAiming { get; }
    
    // Settings
    bool AnalogMovement { get; }
    
    // Events
    Observer<bool> IsShooting { get; }
    Observer<bool> IsPaused { get; }
    public event Action OnStartReload;

    // Methods
    void Enable();
}
