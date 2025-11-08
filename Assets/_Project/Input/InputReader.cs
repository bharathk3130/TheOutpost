using System;
using Clickbait.Utilities;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "InputReader", menuName = "ScriptableObjects/Input Reader")]
public class InputReader : ScriptableObject, PlayerInputActions.IPlayerActions, ICharacterInput
{
    PlayerInputActions _inputActions;

    // Movement
    public Vector2 Move => _inputActions.Player.Move.ReadValue<Vector2>();
    public Vector2 Look => _inputActions.Player.Look.ReadValue<Vector2>();
    public bool Jump { get; private set; }
    public bool Sprint { get; private set; }

    // Shooting
    public Observer<bool> IsAiming { get; } = new(false);
    public Observer<bool> IsShooting { get; } = new(false);

    // Settings
    public bool AnalogMovement { get; private set; }
    public Observer<bool> IsPaused { get; } = new(false);
    public event Action OnStartReload = delegate { };

    void OnEnable()
    {
        if (_inputActions == null)
        {
            _inputActions = new PlayerInputActions();
            _inputActions.Player.SetCallbacks(this);
        }
        
        InputSystem.onDeviceChange += OnInputDeviceChange;
    }

    public void RemoveAllSubscribers()
    {
        IsAiming.RemoveAllListeners();
        IsShooting.RemoveAllListeners();
        IsPaused.RemoveAllListeners();
        OnStartReload = delegate { };
    }
    
    void OnDisable() => InputSystem.onDeviceChange -= OnInputDeviceChange;

    void OnInputDeviceChange(InputDevice device, InputDeviceChange change)
    {
        if (change == InputDeviceChange.Added || change == InputDeviceChange.Reconnected)
        {
            if (device is Gamepad)
            {
                AnalogMovement = true;
            } else if (device is Keyboard || device is Mouse)
            {
                AnalogMovement = false;
            }
        }
    }

    public void Enable() => _inputActions.Enable();
    public void Disable() => _inputActions.Disable();

    public void OnFire(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            if (IsPaused.Value)
            {
                IsPaused.Value = false;
            } else
            {
                IsShooting.Value = true;
            }
        } else if (context.phase == InputActionPhase.Canceled)
        {
            IsShooting.Value = false;
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
            Jump = true;
        else if (context.phase == InputActionPhase.Canceled)
            Jump = false;
    }
    
    public void OnSprint(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
            Sprint = true;
        else if (context.phase == InputActionPhase.Canceled)
            Sprint = false;
    }
    
    public void OnAim(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
            IsAiming.Value = true;
        else if (context.phase == InputActionPhase.Canceled)
            IsAiming.Value = false;
    }

    public void OnPause(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            if (!IsPaused.Value)
            {
                IsPaused.Value = true;
            }
        }
    }

    public void OnReload(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            OnStartReload.Invoke();
        }
    }

    public void OnMove(InputAction.CallbackContext context) { }
    public void OnLook(InputAction.CallbackContext context) { }
}