using Clickbait.Utilities;
using StarterAssets;
using UnityEngine;

public class CrosshairManager : MonoBehaviour
{
    [SerializeField] ThirdPersonController _controller;
    [SerializeField] GameObject _crosshair;
    [SerializeField] GameObject _dot;
    [SerializeField] float _smoothing = 10f;

    [Header("Crosshair Parts")]
    [SerializeField] Transform _topBar;
    [SerializeField] Transform _bottomBar;
    [SerializeField] Transform _leftBar;
    [SerializeField] Transform _rightBar;
    
    [Header("Crosshair Size")]
    [SerializeField] float _standingSpacing = 20f;
    [SerializeField] float _walkingSpacing = 26f;

    ICharacterInput _input;
    SmoothFloat _crosshairLerp;

    float _targetSpacing;
    bool _aiming;

    void Awake()
    {
        _targetSpacing = _standingSpacing;
        SetCrosshairSpacing(_targetSpacing);
        
        _crosshairLerp = new SmoothFloat(_smoothing);
        _crosshairLerp.SetCurrentVal(_targetSpacing);
    }
    
    void Start()
    {
        _controller.IsSprinting.AddListener(OnSprintToggle);
        _input = _controller.CharacterInput;
        _input?.IsAiming.AddListener(OnAimToggle);
    }

    void OnAimToggle(bool aiming)
    {
        _aiming = aiming;
        
        _dot.SetActive(_aiming);
        _crosshair.SetActive(!_aiming);
        
        if (_aiming)
        {
            // Set the crosshair correctly so that when it's re-enabled, it doesn't have to move from it's old spacing
            _crosshairLerp.Cancel();
            SetCrosshairSpacing(_targetSpacing);
        }
    }

    void Update()
    {
        if (!_aiming)
        {
            CalculateCrosshairSpacing();

            if (_crosshairLerp.IsSmoothing)
            {
                float spacing = _crosshairLerp.Step(Time.deltaTime);
                SetCrosshairSpacing(spacing);
            }
        }
    }

    void SetCrosshairSpacing(float spacing)
    {
        _topBar.localPosition = new Vector2(0, spacing);
        _bottomBar.localPosition = new Vector2(0, -spacing);
        _rightBar.localPosition = new Vector2(spacing, 0);
        _leftBar.localPosition = new Vector2(-spacing, 0);
    }

    void CalculateCrosshairSpacing()
    {
        if (_input == null) return;
        
        float spacing = (_input.Move == Vector2.zero && _controller.Grounded) ? _standingSpacing : _walkingSpacing;

        if (Mathf.Abs(spacing - _targetSpacing) > 0.01f)
        {
            _targetSpacing = spacing;
            _crosshairLerp.SmoothTo(_targetSpacing);
        }
    }

    void OnSprintToggle(bool sprint)
    {
        if (sprint)
        {
            _crosshair.SetActive(false);
            _dot.SetActive(false);
        } else
        {
            _dot.SetActive(_aiming);
            _crosshair.SetActive(!_aiming);
        }
    }
}
