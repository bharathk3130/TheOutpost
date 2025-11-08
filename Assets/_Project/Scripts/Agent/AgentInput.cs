using System;
using Clickbait.Utilities;
using UnityEngine;

public class AgentInput : MonoBehaviour, ICharacterInput
{
    public Vector2 Move { get; set; }
    public Vector2 Look { get; set; }
    public bool Jump { get; set; }
    public bool Sprint { get; set; }
    [field: SerializeField] public Observer<bool> IsShooting { get; private set; } = new(false);

    public event Action OnStartReload = delegate { };

    // Don't change these
    public Observer<bool> IsAiming { get; } = new(false);
    public Observer<bool> IsPaused { get; } = new(false);
    public bool AnalogMovement { get; } = false;
    public void Enable() { }
    
    
    public void Reload() => OnStartReload.Invoke();

    public void ResetInputs()
    {
        Move = Vector2.zero;
        Look = Vector2.zero;
        Jump = false;
        Sprint = false;
        IsShooting.Value = false;
        IsAiming.Value = false;
    }
}
