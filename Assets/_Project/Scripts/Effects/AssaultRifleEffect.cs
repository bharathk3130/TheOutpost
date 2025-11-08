using System.Collections;
using UnityEngine;

public class AssaultRifleEffect : MonoBehaviour, IGunEffect, IObjectPoolHandler
{
    [SerializeField] GameObject _soldier;
    
    [Header("Muzzle flash")]
    [SerializeField] ParticleSystem _muzzleFlashEffect;
    [SerializeField] GameObject _muzzleFlashLight;
    [SerializeField] float _lightFlashDuration = 0.1f;

    [Header("Bullet effects")]
    [SerializeField] BulletTrail _bulletTrailPrefab;
    [SerializeField] BulletImpactEffect _bulletImpactEffectPrefab;
    
    DefaultObjectPool<BulletTrail> _bulletTrailPool;
    DefaultObjectPool<BulletImpactEffect> _impactEffectPool;
    
    TransmissionZone _transmissionZone;

    void Start()
    {
        _bulletTrailPool = new DefaultObjectPool<BulletTrail>(_bulletTrailPrefab, this,
            SceneReferences.Instance.BulletTrailParent);
        _impactEffectPool = new DefaultObjectPool<BulletImpactEffect>(_bulletImpactEffectPrefab, this,  
            SceneReferences.Instance.BulletImpactParent);
        
        _transmissionZone = TransmissionZone.Instance;
    }
    
    public void PlayEffect(Vector3 bulletSpawnPos, Vector3 endPos, Vector3 normal, bool hit, GameObject hitObject, GunDataSO gunData)
    {
        _muzzleFlashEffect.Emit(1);
        StartCoroutine(PlayMuzzleFlashEffect());

        BulletTrail trail = _bulletTrailPool.Spawn();
        trail.Initialize(bulletSpawnPos, endPos);

        if (hit)
        {
            // ReSharper disable once ReplaceWithSingleAssignment.True
            bool showHitImpactEffect = true;
            
            // Don't show the effect if you shoot outside from within the zone
            if (_transmissionZone.IsInZone(_soldier) && hitObject.layer == LayerMask.NameToLayer("InvisibleBorders"))
                showHitImpactEffect = false;

            if (hitObject.layer == LayerMask.NameToLayer("Agent"))
                showHitImpactEffect = false;
            
            if (showHitImpactEffect)
            {
                trail.OnReachTarget += () => OnBulletReachedTarget(endPos, normal, hitObject, gunData);
            }
        }
    }

    void OnBulletReachedTarget(Vector3 endPos, Vector3 normal, GameObject hitObject, GunDataSO gunData)
    {
        BulletImpactEffect impactEffect = _impactEffectPool.Spawn(endPos, Quaternion.LookRotation(normal));
        
        if (hitObject != null)
        {
            if (hitObject.TryGetComponent(out Renderer hitRenderer))
            {
                impactEffect.SetColour(hitRenderer.material.color);
            }

            if (hitObject.TryGetComponent(out Rigidbody rb))
            {
                Vector3 direction = (rb.position - endPos).normalized;
                rb.AddForceAtPosition(direction * gunData.BulletExplosionForce, endPos, ForceMode.Impulse);
            }
        }
    }

    IEnumerator PlayMuzzleFlashEffect()
    {
        _muzzleFlashLight.SetActive(true);
        yield return new WaitForSeconds(_lightFlashDuration);
        _muzzleFlashLight.SetActive(false);
    }

    public T InstantiateGameObject<T>(T go) where T : Component => Instantiate(go);
    public void DestroyGameObject(GameObject go) => Destroy(go);
}