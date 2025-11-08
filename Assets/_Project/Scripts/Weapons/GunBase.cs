using Clickbait.Utilities;
using StarterAssets;
using Unity.MLAgents;
using UnityEngine;

public abstract class GunBase : MonoBehaviour
{
    [SerializeField] protected GunDataSO _gunData;
    
    [Header("References")]
    [SerializeField] AudioSource _audioSource;
    [SerializeField] protected ThirdPersonController _controller;
    [SerializeField] ThirdPersonShooterController _shooterController;
    [Tooltip("This will be from where the agent's shoot raycast will get shot from")]
    [SerializeField] protected Transform _aimTargetParent;

    [Header("Shooting related fields")]
    [SerializeField] protected WeaponManager _weaponManager;
    [SerializeField] protected Transform _shootPos;
    [SerializeField] protected Transform _gunTip;
    [SerializeField] protected Transform _playerFrontShootingPoint;

    [Header("Camera Shake")]
    [SerializeField] float _camShakeIntensity = 0.2f;
    [SerializeField] float _camShakeDuration = 0.1f;

    [Header("Settings")]
    [SerializeField] float _spreadMultiplier = 1;
    [SerializeField] protected bool _disableSpreadForAgent = true;

    protected IGunEffect _gunEffect;
    CountDownTimer _shootTimer;
    CountDownTimer _reloadTimer;
    CameraShake _cameraShake;

    public CountDownTimer ShootTimer => _shootTimer;
    
    GunAudioSO _gunAudio;
    ICharacterInput _input;

    float _timeToAimAfterReloading = 0.2f;
    bool _shooting;
    protected bool IsPlayer { get; private set; }

    public Observer<int> CurrentAmmo;
    public GunDataSO GunData => _gunData;

    // Sensors
    protected ScreenRaycastSensor _crosshairSensor;
    protected DynamicRaycastSensor _gunSensor;
    protected DynamicRaycastSensor _playerFrontSensor; // Shoots rays from the _playerFrontShootingPoint transform
    protected LocalRaycastSensor _agentSensorFromTargetParent; // Shoots a ray from _targetParent towards the target to hit something
    protected DynamicRaycastSensor _agentGunSensor;
    
    /// <summary>
    /// Use when the gun's current shoot point is not in a desired location. Ex: When we shoot while sprinting, we
    /// stop sprinting immediately to shoot but our hand may be down, making the bullet spawn from below
    /// </summary>
    protected bool _shootFromFrontOfPlayer;

    protected virtual void Awake()
    {
        InitializeReferences();
        InitialSubscriptions();

        CurrentAmmo = new Observer<int>(_gunData.MagazineSize);
    }

    void InitializeReferences()
    {
        _gunEffect = GetComponent<IGunEffect>();

        _shootTimer = new CountDownTimer(_gunData.ShootInterval);
        _reloadTimer = new CountDownTimer(_gunData.ReloadTime + _timeToAimAfterReloading);

        _gunAudio = _gunData.AudioSO;
    }

    void InitialSubscriptions()
    {
        if (_controller.IsPlayer)
        {
            _shootTimer.OnTimerStop += TryToShoot;
        }
        _reloadTimer.OnTimerStop += OnReloadComplete;
        _controller.CanShoot.AddListener(OnCanShootToggle);
    }

    void OnCanShootToggle(bool canShoot)
    {
        if (canShoot && _shooting)
        {
            TryToShoot();
        }
    }

    protected virtual void Start()
    {
        _cameraShake = CameraShake.Instance;
        _weaponManager.Equip(this);
        _input = _controller.CharacterInput;

        _crosshairSensor = new ScreenRaycastSensor(Camera.main, _gunData.Range);
        _gunSensor = new DynamicRaycastSensor(_shootPos, _gunData.Range);
        _playerFrontSensor = new DynamicRaycastSensor(_playerFrontShootingPoint, _gunData.Range);
        _agentSensorFromTargetParent = new LocalRaycastSensor(_aimTargetParent, LocalRaycastSensor.CastDirection.Forward, _gunData.Range);
        _agentGunSensor = new DynamicRaycastSensor(_shootPos, _gunData.Range);

        _crosshairSensor.SetLayerMask(_weaponManager.ShootingLayerMask);
        _gunSensor.SetLayerMask(_weaponManager.ShootingLayerMask);
        _playerFrontSensor.SetLayerMask(_weaponManager.ShootingLayerMask);
        _agentSensorFromTargetParent.SetLayerMask(_weaponManager.ShootingLayerMask);
        _agentGunSensor.SetLayerMask(_weaponManager.ShootingLayerMask);
        
        IsPlayer = _controller.IsPlayer;
    }

    protected virtual void Update()
    {
        _shootTimer.Tick(Time.deltaTime);
        _reloadTimer.Tick(Time.deltaTime);
    }

    public void StartFiring()
    {
        if (!enabled) return;

        if (_controller.IsSprinting.Value)
        {
            _shootFromFrontOfPlayer = true;
        }

        _shooting = true;
        TryToShoot();
    }

    public void StopFiring()
    {
        _shooting = false;
        _shootTimer.Abort();
    }

    public void TryToShoot()
    {
        if (!_shootTimer.IsRunning && !_reloadTimer.IsRunning && _controller.CanShoot.Value && CurrentAmmo.Value > 0)
        {
            Shoot();
        }

        _shootFromFrontOfPlayer = false;
    }

    protected virtual void Shoot()
    {
        _shootTimer.Start();
        
        if (IsPlayer)
        {
            _cameraShake.Shake(_camShakeIntensity, _camShakeDuration);
        }
        
        _gunAudio.PlayFireClip(_audioSource);

        if (IsPlayer) // Agent doesn't reload
        {
            CurrentAmmo.Value--;
            if (CurrentAmmo.Value <= 0)
            {
                Reload();
            }
        }
    }

    public void Reload()
    {
        if (_reloadTimer.IsRunning || CurrentAmmo.Value == _gunData.MagazineSize) return;

        _gunAudio.PlayReloadClip(_audioSource);
        _reloadTimer.Start();
        _shooterController.PlayReloadAnimation(_gunData.ReloadTime);
    }

    void OnReloadComplete()
    {
        CurrentAmmo.Value = _gunData.MagazineSize;
        if (_shooting)
        {
            TryToShoot();
        }
    }

    protected DynamicRaycastSensor ShootRaycastAndGetSensor(Vector3 shootPos)
    {
        // Shoots the first raycast to see what the user/agent is aiming at
        RaycastSensorBase detectionSensor = IsPlayer ? _crosshairSensor : _agentSensorFromTargetParent;
        detectionSensor.Cast();
        
        Vector3 aimedPoint = detectionSensor.GetRayEndPosition();
        
        Vector3 aimDirectionAccountingForSpread;
        
        // Shoots a ray from shootPoint to _detectionSensor.hitPoint and whatever gets hit is what gets shot
        DynamicRaycastSensor shootingSensor;
        
        if (IsPlayer)
        {
            shootingSensor = _shootFromFrontOfPlayer ? _playerFrontSensor : _gunSensor;
            
            aimDirectionAccountingForSpread = GetShotDirection(shootPos, aimedPoint);
            shootingSensor.CastInDirection(aimDirectionAccountingForSpread);
        } else
        {
            shootingSensor = _agentGunSensor;
            Vector3 aimDirection = aimedPoint - shootPos;
            aimDirectionAccountingForSpread = _disableSpreadForAgent ? aimDirection : GetShotDirection(shootPos, aimedPoint);
        }
        
        shootingSensor.CastInDirection(aimDirectionAccountingForSpread);

        return shootingSensor;
    }

    protected Vector3 GetShootPos()
    {
        Vector3 shootPos;
        if (IsPlayer)
        {
            shootPos = _shootFromFrontOfPlayer ? _playerFrontShootingPoint.position : _shootPos.position;
        } else
        {
            shootPos = _shootPos.position;
        }

        return shootPos;
    }

    protected Vector3 GetShotDirection(Vector3 shootPos, Vector3 aimedPoint)
    {
        bool isMoving = _input.Move != Vector2.zero || !_controller.Grounded;
        bool isAiming = _input.IsAiming.Value;

        Vector3 baseDir = (aimedPoint - shootPos).normalized;
        float spread;
        if (isMoving)
        {
            spread = isAiming ? _gunData.WalkJumpSpread_Aim : _gunData.WalkJumpSpread;
        } else
        {
            spread = isAiming ? _gunData.StandSpread_Aim : _gunData.StandSpread;
        }
        
        spread *= _spreadMultiplier;

        if (spread <= 0f) return baseDir;

        // convert to a "radius" on the unit sphere slice
        float spreadRad = spread * Mathf.Deg2Rad;
        float radius = Mathf.Tan(spreadRad); // small-angle approx: ~spreadRad for tiny angles

        // build orthonormal basis around baseDir
        Vector3 right = Vector3.Cross(baseDir, Vector3.up); // This is the right w.r.t the spread circle
        if (right.sqrMagnitude < 0.001f) // baseDir nearly parallel to Vector3.up
            right = Vector3.Cross(baseDir, Vector3.forward);
        right.Normalize();
        Vector3 up = Vector3.Cross(right, baseDir).normalized;

        // sample a random displacement in the circle of radius 'radius'
        Vector2 rnd = Random.insideUnitCircle * radius;

        // perturbed direction
        Vector3 perturbed = (baseDir + right * rnd.x + up * rnd.y).normalized;
        return perturbed;
    }

    /// <summary>
    /// Goes from 1 to 0. We can shoot when it's 0
    /// </summary>
    public float GetNormalizedTimeTillNextShoot() => Mathf.Max(0, 1 - ShootTimer.Progress);

    public bool PlayerAhead()
    {
        DynamicRaycastSensor shootSensor = ShootRaycastAndGetSensor(GetShootPos());
        return (shootSensor.HasDetectedHit() && shootSensor.GetHitCollider().CompareTag("Player"));
    }
}