using TMPro;
using UnityEngine;

public class AmmoUI : MonoBehaviour
{
    [SerializeField] WeaponManager _weaponManager;
    [SerializeField] TextMeshProUGUI _currentAmmoText;
    [SerializeField] TextMeshProUGUI _magazingSizeText;
    
    GunBase _currentGun;

    void Awake()
    {
        _weaponManager.OnEquipGun += OnEquipGun;
    }

    void OnEquipGun(GunBase newGun)
    {
        if (_currentGun != null)
        {
            _currentGun.CurrentAmmo.RemoveListener(OnAmmoChange);
        }

        _currentGun = newGun;
        _currentGun.CurrentAmmo.AddListener(OnAmmoChange);
        _magazingSizeText.text = _currentGun.GunData.MagazineSize.ToString();
    }

    void OnAmmoChange(int ammo)
    {
        _currentAmmoText.text = ammo.ToString();
    }
}
