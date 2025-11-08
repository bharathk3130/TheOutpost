using System;
using System.Collections;
using ShooterLearning;
using StarterAssets;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    [SerializeField] ThirdPersonController _controller;
    [SerializeField] Health _health;
    [SerializeField] public LayerMask ShootingLayerMask;

    [field: SerializeField] public ShooterAgent Agent { get; private set; }

    IDeath _death;
    ICharacterInput _input;
    GunBase _gun;
    public GunBase Gun => _gun;

    public event Action<GunBase> OnEquipGun = delegate { };

    public void Equip(GunBase gun)
    {
        _gun = gun;
        OnEquipGun.Invoke(gun);
    }

    void Start()
    {
        InitializeVariables();
        StartCoroutine(InitialSubscribers());
    }

    void InitializeVariables()
    {
        _input = _controller.CharacterInput;
        _death = _health.GetComponent<IDeath>();
    }
    
    IEnumerator InitialSubscribers()
    {
        // Wait till the end of the frame so that the gun can be equipped first
        yield return new WaitForEndOfFrame();
        
        if (_controller.IsPlayer)
        {
            _input.IsShooting.AddListener(shoot =>
            {
                if (!gameObject.activeInHierarchy)
                    return; // To prevent errors during play-testing with the character disabled

                if (shoot)
                {
                    if (enabled)
                    {
                        _gun.StartFiring();
                    }
                } else
                {
                    _gun.StopFiring();
                }
            });
            _input.OnStartReload += _gun.Reload;
        }

        if (IDeath.IsFreezeOnDeath())
        {
            //_health.Death.OnDeath += DropWeapon;
        }
    }

    void DropWeapon()
    {
        _gun.GetComponent<Collider>().isTrigger = false;
        _gun.GetComponent<Rigidbody>().isKinematic = false;
        _gun.GetComponent<Rigidbody>().useGravity = true;
    }

    public void Agent_SingleShot()
    {
        if (_death.IsDead) return;
        
        _gun.TryToShoot();
    }

    public bool PlayerAhead()
    {
        if (_gun == null)
        {
            return false;
        }
        
        return _gun.PlayerAhead();
    }

    public float GetNormalizedTimeTillNextShoot()
    {
        if (_gun == null)
        {
            return 0.1f;
        }
        
        return _gun.GetNormalizedTimeTillNextShoot();
    }
}
