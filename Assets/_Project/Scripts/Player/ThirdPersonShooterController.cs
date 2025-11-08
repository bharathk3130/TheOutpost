using System;
using System.Collections;
using StarterAssets;
using UnityEngine;

public class ThirdPersonShooterController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] RigManager _rigManager;
    [SerializeField] Animator _anim;
    [field: SerializeField] public Transform TPSCamTarget { get; private set; }
    [field: SerializeField] public Transform FPSCamTarget { get; private set; }
    [field: SerializeField] public WeaponManager WeaponManager { get; private set; }
    
    [Header("Sensitivity")]
    [SerializeField] float _normalSensitivity = 1f;
    [SerializeField] float _aimSensitivity = 0.5f;

    [Header("Other settings")]
    [SerializeField] float _turnSmoothing = 20f;
    [SerializeField] float _sprintingTransitionSmoothing = 20f;

    public event Action<ControllerColliderHit> OnCollisionStayDetected = delegate { };
        
    CharacterAnimationLayerManager _animationLayerManager;
    Health _health;
    ThirdPersonController _controller;
    ICharacterInput _input;
    Transform _cinemachineCamTarget; // It's either the camera or the aimTargetParent depending on if it's a human or agent

    static int _reloadHash;

    bool _dead;
    bool _isAiming;
    
    void Awake()
    {
        _controller = GetComponent<ThirdPersonController>();
        _health = GetComponent<Health>();
        _animationLayerManager = GetComponent<CharacterAnimationLayerManager>();

        _reloadHash = Animator.StringToHash("Reload");
    }

    void Start()
    {
        _input = _controller.CharacterInput;
        
        _input.IsAiming.AddListener(OnAimToggle);
        if (IDeath.IsFreezeOnDeath())
        {
            _health.Death.OnDeath += OnDie;
        }

        _controller.IsSprinting.AddListener(OnSprintToggle);
        _cinemachineCamTarget = _controller.CinemachineCameraTarget.transform;
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        OnCollisionStayDetected.Invoke(hit);
    }
    
    void OnSprintToggle(bool sprint)
    {
        float aimWeight = sprint ? 0f : 1f;
        _animationLayerManager.SetLayerWeightSmoothed(CharacterAnimationLayerManager.AnimationLayerType.Gun, aimWeight);
        _rigManager.SetWeightSmoothed(aimWeight);
    }

    void OnDie()
    {
        OnAimToggle(false);
        _dead = true;
    }

    void Update()
    {
        if (_controller.IsPlayer)
        {
            Vector3 dir = _cinemachineCamTarget.forward;
            dir.y = 0;
            transform.forward = Vector3.Lerp(transform.forward, dir, Time.deltaTime * _turnSmoothing);
        }
    }
    
    void OnAimToggle(bool aiming)
    {
        if (_dead) return;

        _isAiming = aiming;
        _controller.SetSensitivity(aiming ? _aimSensitivity : _normalSensitivity);
    }

    public void PlayReloadAnimation(float reloadTime) => StartCoroutine(ReloadAnimation(reloadTime));

    IEnumerator ReloadAnimation(float reloadTime)
    {
        _anim.SetTrigger(_reloadHash);
        _rigManager.SetWeightSmoothed(0);
        
        _controller.IsSprinting.Value = false;
        _controller.SetIsReloading(true);
        yield return new WaitForSeconds(reloadTime);
        
        _rigManager.SetWeightSmoothed(1);
        _controller.SetIsReloading(false);
    }

    void OnValidate()
    {
        if (Application.isPlaying && _controller != null)
        {
            _controller.SetSensitivity(_isAiming ? _aimSensitivity : _normalSensitivity);
        }
    }
}
