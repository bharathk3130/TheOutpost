using UnityEngine;

[CreateAssetMenu(fileName = "GunData", menuName = "ScriptableObjects/GunData")]
public class GunDataSO : ScriptableObject
{
    [Header("References")]
    public GunAudioSO AudioSO;
    
    [Header("Gun Settings")]
    public int Damage;
    public float HeadshotMultiplier;
    public float Range;
    [Tooltip("Applied to movable environment objects like boxes")]
    public int BulletExplosionForce;
    
    [Header("Ammo")]
    public int MagazineSize;

    [Header("Intervals")]
    public float ShootInterval;
    public float ReloadTime;
    
    [Header("Recoil & Spread (In Degrees)")]
    public float Recoil;
    public float StandSpread = 1.3f;
    public float WalkJumpSpread = 2.5f;
    public float StandSpread_Aim = 0.3f;
    public float WalkJumpSpread_Aim = 1.3f;
}
