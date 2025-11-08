using Clickbait.Utilities;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class AssaultRifle : GunBase
{
    protected override void Shoot()
    {
        base.Shoot();

        Vector3 shootPos = GetShootPos();
        DynamicRaycastSensor shootingSensor = ShootRaycastAndGetSensor(shootPos);

        bool hasHit = shootingSensor.HasDetectedHit();

        GameObject hitObject;
        Vector3 hitNormal;
        Vector3 hitPos = shootingSensor.GetRayEndPosition();
        if (hasHit)
        {
            hitNormal = shootingSensor.GetNormal();
            hitObject = shootingSensor.GetHitTransform().gameObject;
        } else
        {
            hitNormal = Vector3.zero;
            hitObject = null;
        }
        
        _gunEffect.PlayEffect(_gunTip.position, hitPos, hitNormal, hasHit, hitObject, _gunData);

        bool shotEnemy = false;
        
        if (hitObject != null)
        {
            if (hitObject.TryGetComponent(out Health health))
            {
                health.TakeDamage(_gunData.Damage);
                shotEnemy = true;
            }
        }

        if (shotEnemy)
        {
            _weaponManager.Agent?.OnShootEnemy();
        } else
        {
            _weaponManager.Agent?.OnMissEnemy();
        }
    }
}